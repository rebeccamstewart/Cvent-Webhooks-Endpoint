using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CventRegManager.MRAProTechAPI;
using CventRegManager.Models;
using CventRegManager.Interfaces;
using System.Text;

namespace CventRegManager.Domain
{
    public class APAPRegMessenger : IClientRegMessenger
    {
       

        public string SendNewMsg(APAPMessage apapMsg, IEmail Email)
        {

            string returnMessage;
            var PushToAPAP = new apapws();
            string UpdateLinkToEmail = CreateUpdateLink(apapMsg);

            try
            {
                string Body = "";
                string CallResult = PushToAPAP.UpdateUserInfo(apapMsg.szUserID, apapMsg.szRegType, apapMsg.szSpouseRegFlag, apapMsg.szFirstTimeFlag, apapMsg.szVolunteerFlag, apapMsg.szHeardAboutEvent, apapMsg.szExcludeEmailFlag, apapMsg.szAttendLuncheonFlag, apapMsg.szNumTicketsPurchased, apapMsg.szSponsorAwardTableFlag, apapMsg.szPaymentMethod, apapMsg.szRegID, apapMsg.szJobTitle, apapMsg.szPerID, DateTime.Today);

               
                returnMessage = CallResult;
                if (CallResult.Length > 150)
                {
                    Body = "Failed to Update APAP Member " + " at " + DateTime.Now + '\n' + CallResult + "\n" + UpdateLinkToEmail;
                    Email.UpdateSubject("APAP Post to ProTech Failed");
                }

                else
                {
                    Body = "Succeeded to Update APAP Member " + " at " + DateTime.Now + '\n' + CallResult + "\n" + UpdateLinkToEmail;
                    Email.UpdateSubject("APAP Post to ProTech Succeeded");
                }
                bool result = Email.Send("rebecca@eventproducers.events", Body);

            }
            catch (Exception e)
            {
               
                string Body = "Error occured "+ " at " + DateTime.Now + ". This call was not made: " + UpdateLinkToEmail + '\n' + " Message: "  + e.Message + " InnerException: " + e.InnerException  + " StackTrace: " + e.StackTrace ;
                bool result = Email.Send("rebecca@eventproducers.events", Body);
                returnMessage = e.ToString();
            }
            return returnMessage;
         

        }

        private string CreateUpdateLink(APAPMessage apapMsg)
        {
            
            string sURL = "http://www.mra-services.com/_webservice/APAP_UpdateMember.asmx/UpdateUserInfo";
            StringBuilder UpdateLink = new StringBuilder();
            UpdateLink.Append(sURL).Append("?").Append("szUserID=").Append(apapMsg.szUserID);
            UpdateLink.Append("&").Append("szRegType=").Append(apapMsg.szRegType);
            UpdateLink.Append("&").Append("szSpouseRegFlag=").Append(apapMsg.szSpouseRegFlag);
            UpdateLink.Append("&").Append("szFirstTimeFlag=").Append(apapMsg.szFirstTimeFlag);
            UpdateLink.Append("&").Append("szVolunteerFlag=").Append(apapMsg.szVolunteerFlag);
            UpdateLink.Append("&").Append("szHeardAboutEvent=").Append(HttpUtility.UrlEncode(apapMsg.szHeardAboutEvent));
            UpdateLink.Append("&").Append("szExcludeEmailFlag=").Append(apapMsg.szExcludeEmailFlag);
            UpdateLink.Append("&").Append("szAttendLuncheonFlag=").Append(apapMsg.szAttendLuncheonFlag);
            UpdateLink.Append("&").Append("szNumTicketsPurchased=").Append(apapMsg.szNumTicketsPurchased);
            UpdateLink.Append("&").Append("szSponsorAwardTableFlag=").Append(apapMsg.szSponsorAwardTableFlag);
            UpdateLink.Append("&").Append("szPaymentMethod=").Append(apapMsg.szPaymentMethod);
            UpdateLink.Append("&").Append("szRegID=").Append(apapMsg.szRegID);
            UpdateLink.Append("&").Append("szJobTitle=").Append(apapMsg.szJobTitle);
            UpdateLink.Append("&").Append("szPerID=").Append(apapMsg.szPerID);
            UpdateLink.Append("&").Append("szRegDate=").Append(HttpUtility.UrlEncode(apapMsg.szRegDate));

            return UpdateLink.ToString();

        }
    }
}