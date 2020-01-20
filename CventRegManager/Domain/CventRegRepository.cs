using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using CventRegManager.Models;
using CventRegManager.MRACventAPI;
using CventRegManager.Interfaces;

namespace CventRegManager.Domain
{
    public class CventAttendeeRepository : ICventRegRepository
    {
        private string SponsorAwardTableCustomFieldGUID;
        private string LuncheonSessionCustomFieldGUID;
        private string LuncheonTicketCustomFieldGUID;
        private string PayByCheckCustomFieldGUID;
        private string PayByWireTransferCustomFieldGUID;
        private string PersonIdFieldGUID;
        private string OrgIdFieldGUID;
        private string FirstTimeCustomFieldGUID;
        private string VolunteerCustomFieldGUID;
        private string HeardAboutEventCustomFieldGUID;
        private string EmailOptOutCustomFieldGUID;
        public string SpouseRegFieldGUID;

        public CventAttendeeRepository()
        {
            SpouseRegFieldGUID = "";
            VolunteerCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.VolunteerCustomFieldGUID");
            HeardAboutEventCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.HeardAboutEventCustomFieldGUID");
            SpouseRegFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.MemberCustomFieldGUID");
            SponsorAwardTableCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.SponsorAwardTableCustomFieldGUID");
            LuncheonSessionCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.LuncheonSessionCustomFieldGUID");
            LuncheonTicketCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.LuncheonTicketCustomFieldGUID");
            PayByCheckCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.PayByCheckCustomFieldGUID");
            PayByWireTransferCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.PayByWireTransferCustomFieldGUID");
            PersonIdFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.PersonIdFieldGUID");
            OrgIdFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.OrgIdFieldGUID");
            FirstTimeCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.FirstTimeCustomFieldGUID");
            EmailOptOutCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.EmailOptOutCustomFieldGUID");

        }

        public CventAttendee GetCventAttendee(string registrationGUID, string eventGUID, string APIEventPW)
        {
            int iNumPurchased = 0;
            CventAttendee cur_Attendee = new CventAttendee();
            cur_Attendee.cventInvitteeId = registrationGUID;

            //try
            //{
            var PullFromCvent = new MRARegForIS();
            //DetailedRegistrant[]
            var CventRegData = PullFromCvent.GetRegistrationDetails(eventGUID, APIEventPW, registrationGUID);

            if (CventRegData != null)
            {
                //There will really only be one record, but, need to treat return type like an array
                foreach (DetailedRegistrant dr in CventRegData)
                {
                    if (dr != null)
                    {
                        //Guest Registration - only need to check if there is one
                        cur_Attendee.confirmationNumber = dr.MRARegConfirmation;
                        cur_Attendee.contactType = dr.RegistrationType;
                        if (cur_Attendee.contactType == "Board Track (Code Required)") cur_Attendee.contactType = "Board Track";
                        if (cur_Attendee.contactType == "APAP Programs (Code Required)") cur_Attendee.contactType = "APAP Program";
                        if (cur_Attendee.contactType == "Student (Code Required)") cur_Attendee.contactType = "Student";





                        cur_Attendee.firstName = dr.FirstName;
                        cur_Attendee.lastName = dr.LastName;
                        cur_Attendee.email = dr.Email;
                       
                        cur_Attendee.regDate = dr.RegistrationDate.ToString();
                        cur_Attendee.title = dr.Title;

                        if (dr.GuestRegistrations != null)
                        {

                            foreach (GuestRegistration guest in dr.GuestRegistrations)
                            {
                                if (guest.LastName.Length > 0 && guest.FirstName.Length > 0)
                                {
                                    cur_Attendee.guestName = guest.LastName + " " + guest.FirstName;
                                }
                                else
                                {
                                    cur_Attendee.guestName = guest.LastName + guest.FirstName;
                                }
                            }
                        }


                        foreach (OptionalItem oi in dr.OptionalItemRegistrations)
                        {
                            //Luncheon Session for Primary REgistrant
                            if (oi.SessionId.ToUpper() == LuncheonSessionCustomFieldGUID.ToUpper() && oi.Quantity == 1)
                            {
                                if (oi.OptionalFee == 0)
                                {
                                    cur_Attendee.AttendLuncheon = true;
                                }
                                else
                                {
                                    iNumPurchased = iNumPurchased + 1;
                                }
                            }

                            //Luncheon Session for Guest
                            if (oi.SessionId.ToUpper() == LuncheonTicketCustomFieldGUID.ToUpper() && oi.Quantity == 1)
                            {
                                iNumPurchased = iNumPurchased + 1;
                            }

                            if (oi.SessionId.ToUpper() == PayByCheckCustomFieldGUID.ToUpper() && oi.Quantity == 1)
                            {
                                cur_Attendee.paymentMethod = "Check";
                            }
                            if (oi.SessionId.ToUpper() == PayByWireTransferCustomFieldGUID.ToUpper() && oi.Quantity == 1)
                            {
                                cur_Attendee.paymentMethod = "Wire Transfer";
                            }
                        }
                        cur_Attendee.LuncheonTicketsPurchased = iNumPurchased;

                        //Check to see if there is a credit card payment
                        foreach (Transaction tract in dr.Transactions)
                        {
                            if (tract.TransactionType.ToUpper() == "Credit Card" && tract.TransactionAmount > 0) cur_Attendee.paymentMethod = "Credit Card";
                        }

                        foreach (CustomQuestion cq in dr.CustomQuestions)
                        {
                            //First Time Attendee
                            if (cq.MRAId.ToUpper() == FirstTimeCustomFieldGUID.ToUpper())
                            {
                                cur_Attendee.FirstTime = (cq.Answer == "Yes - this is my first APAP Conference.") ? true : false;
                            }

                            //Email - would like to opt out
                            if (cq.MRAId == EmailOptOutCustomFieldGUID)
                            {
                                cur_Attendee.OptOut = (cq.Answer == "No") ? true : false;
                            }
                            //Volunteer - would like to be one
                            if (cq.MRAId == VolunteerCustomFieldGUID)
                            {
                                cur_Attendee.Volunteer = (cq.Answer == "No") ? false : true;
                            }
                            if (cq.MRAId == SponsorAwardTableCustomFieldGUID)
                            {
                                cur_Attendee.SponsorLunchTable = (cq.Answer == "No") ? false : true;
                            }
                            if (cq.MRAId == HeardAboutEventCustomFieldGUID) cur_Attendee.HeardAboutValue = cq.Answer;
                            if (cq.MRAId == PersonIdFieldGUID) cur_Attendee.personMembershipId = cq.Answer;
                            if (cq.MRAId == OrgIdFieldGUID) cur_Attendee.orgMembershipId = cq.Answer;
                        }
                    }
                }
            }

            //}
            //catch (Exception)
            // {

            

            return cur_Attendee;
        }

   
    }
}