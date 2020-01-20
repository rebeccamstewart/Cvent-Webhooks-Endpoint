using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CventRegManager.Models;
using CventRegManager.MRACventAPI;
using CventRegManager.Interfaces;
using CventRegManager.Domain;
using CventRegManager.Helpers;
using System.Text;

namespace CventRegManager.Services
{
    public class WH_UpdateLog
    { 
        public int SuccessfullyProcessed;
        public int FailedProcessed;
        public DateTime ProcessingStartTime;
        public List<InviteeToProcess> Invitees;
        public DateTime ProcessingEndTime;

        public WH_UpdateLog()
        {
            SuccessfullyProcessed = 0;
            FailedProcessed = 0;
            ProcessingStartTime = DateTime.Now;
            Invitees = new List<InviteeToProcess>();
            
         }
    }
    
     public class InviteeToProcess
    { 
        public string InviteeIdGuid;
        public string ProcessingAction;
        public string ProblemWithRecord;
        public bool PushToClientSuccess;
        public string ProcessingException;
        
       
        
       
    }
    
    
    public class WebhookManager


    {
        private List<CventAttendee> Attendees;

        
        private string messageStub;
        private string eventType;
        private string eventStub;
        private string postBodyData;
        private string cventAuthHeader;
        private string verb;
        private bool authorized;
        private bool SuccessfullyPushedReg;
        private string APIEventPW;
        private ICventRegRepository cventRegRepo;
        private IClientRegMessenger regMessenger;
        private IHR_MRA_Repository hrMRARepo;
        public string ProcessLogData;

        public WebhookManager(ICventRegRepository CventRegRepo, IClientRegMessenger APAPRegMsgr, IHR_MRA_Repository HRMRARepo)
        {
            cventRegRepo = CventRegRepo;
            regMessenger = APAPRegMsgr;
            hrMRARepo = HRMRARepo;
            eventStub = "";
            Attendees = new List<CventAttendee>();
            APIEventPW = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.APIEventPW");
         }

        public bool LogJsonData(string PostBodyData, string cventAuthHeader, string verb, bool Authorized)
        {

           
            return hrMRARepo.LogSimpleCall(PostBodyData, cventAuthHeader, verb, Authorized);
        }

     

        public bool ProcessPostData(string BodyData, CventMessage CventBodyMsg, string cv_header, string cv_verb, bool Authorized)
        {

            var LogData = new WH_UpdateLog();
            bool SuccessForAll = true;
             cventAuthHeader = cv_header;
            verb = cv_verb;
            messageStub = CventBodyMsg.messageStub;
            eventType = CventBodyMsg.eventType;
            postBodyData = BodyData;
            authorized = Authorized;
            CventAttendee cur_Attendee;
            //need to loop through all messages
            bool LoggedCallSuccessfully = false;

            SuccessfullyPushedReg = true;

     

            foreach (CventRegMessage reg_msg in CventBodyMsg.message)
            {

                if (reg_msg != null)
                {
                    if (reg_msg.isGuest != "true")
                    {
                        eventStub = reg_msg.eventStub;
                        var CventRegRepo = new CventAttendeeRepository();
                        string ResponseFromPushCall;
                        string RegInJason = Newtonsoft.Json.JsonConvert.SerializeObject(reg_msg);


                        //CHECK TO SEE IF WE ALREADY PROCESS THIS ONE

                        string status = hrMRARepo.RegProcessedStatus(reg_msg.inviteeStub);
                        var tempInvitee = new InviteeToProcess { InviteeIdGuid = reg_msg.inviteeStub.ToString(), };


                        if (status != "Processed")
                        {
                            cur_Attendee = CventRegRepo.GetCventAttendee(reg_msg.inviteeStub, reg_msg.eventStub, APIEventPW);

                            cur_Attendee.PrimaryRegistration = (reg_msg.isGuest == "True") ? false : true;
                            Attendees.Add(cur_Attendee);

                            // if (cur_Attendee.excludeEmail == "no") ? false : true;
                            if (status == "Missing")
                            {
                                LoggedCallSuccessfully = hrMRARepo.CreateRegLog(cur_Attendee, RegInJason, eventStub);
                                tempInvitee.ProcessingAction = "Created Log Record";
                            }
                            else
                            {
                                LoggedCallSuccessfully = true;
                                tempInvitee.ProcessingAction = "Log Record Exists";
                            }


                            if (LoggedCallSuccessfully && cur_Attendee.PrimaryRegistration)
                            {
                                try
                                {

                                    if (cur_Attendee.personMembershipId.Length > 0)
                                    {
                                        ResponseFromPushCall = SendRegToClient(cur_Attendee);
                                    }
                                    else
                                    {
                                        ResponseFromPushCall = "No Member Id";
                                        tempInvitee.ProblemWithRecord = ResponseFromPushCall;
                                    }

                                    if (ResponseFromPushCall.Length == 36)
                                    {
                                        cur_Attendee.ProcessedGUID = ResponseFromPushCall;
                                        hrMRARepo.UpdateRegLogAsProcessed(cur_Attendee);
                                        LogData.SuccessfullyProcessed = +1;
                                        tempInvitee.PushToClientSuccess = true;
                                    }
                                    else
                                    {
                                        hrMRARepo.UpdateRegLogWithErrorMessage(cur_Attendee, ResponseFromPushCall);
                                        tempInvitee.PushToClientSuccess = false;
                                        LogData.FailedProcessed = +1;
                                    }

                                }
                                catch (Exception ex)
                                {

                                    tempInvitee.ProcessingException = ex.InnerException.ToString();
                                    SuccessfullyPushedReg = false;
                                    LogData.FailedProcessed = +1;
                                }
                            }
                            else
                            {
                                tempInvitee.ProcessingAction = "Already processed by Client";
                            }

                            SuccessForAll = false;
                        }
                        LogData.Invitees.Add(tempInvitee);

                    }
                }

            }
            LogData.ProcessingEndTime = DateTime.Now;


            ProcessLogData = Newtonsoft.Json.JsonConvert.SerializeObject(LogData); ;
            return true;
       
        }

  
        public string SendRegToClient(CventAttendee cur_Attendee)
        {
            JangoEmail JEmail = new JangoEmail();
           

                var newAPAP_RegMsg = new APAPMessage();
                newAPAP_RegMsg.szUserID = cur_Attendee.personMembershipId;
                newAPAP_RegMsg.szRegType = cur_Attendee.contactType;
                newAPAP_RegMsg.szSpouseRegFlag = (cur_Attendee.guestName.Length > 0) ? "yes" : "no";
                newAPAP_RegMsg.szAttendLuncheonFlag = (cur_Attendee.AttendLuncheon == true) ? "yes" : "no";
                newAPAP_RegMsg.szNumTicketsPurchased = cur_Attendee.LuncheonTicketsPurchased.ToString();
                newAPAP_RegMsg.szFirstTimeFlag = (cur_Attendee.FirstTime == true) ? "yes" : "no";
                newAPAP_RegMsg.szVolunteerFlag = (cur_Attendee.Volunteer == true) ? "yes" : "no";
                newAPAP_RegMsg.szExcludeEmailFlag = (cur_Attendee.OptOut == true) ? "yes" : "no"; 
                newAPAP_RegMsg.szHeardAboutEvent = cur_Attendee.HeardAboutValue;
                newAPAP_RegMsg.szPerID = cur_Attendee.cventInvitteeId;
                newAPAP_RegMsg.szRegDate = cur_Attendee.regDate;
                newAPAP_RegMsg.szRegID = cur_Attendee.confirmationNumber;
                newAPAP_RegMsg.szJobTitle = cur_Attendee.title;
                newAPAP_RegMsg.szSponsorAwardTableFlag = (cur_Attendee.SponsorLunchTable == true) ? "yes" : "no";
                newAPAP_RegMsg.szPaymentMethod = cur_Attendee.paymentMethod;

                string result = regMessenger.SendNewMsg(newAPAP_RegMsg, JEmail);
                return result;
  
        }


       


    }
}