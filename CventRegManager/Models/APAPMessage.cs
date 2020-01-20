using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CventRegManager.Models
{

    public class APAPMessage
    {

        public string szUserID { get; set; }
        public string szRegType { get; set; }
        public string szSpouseRegFlag { get; set; }
        public string szFirstTimeFlag { get; set; }
        public string szVolunteerFlag { get; set; }
        public string szHeardAboutEvent { get; set; }
        public string szExcludeEmailFlag { get; set; }
        public string szAttendLuncheonFlag { get; set; }


        public string szNumTicketsPurchased { get; set; }
        public string szSponsorAwardTableFlag { get; set; }
        public string szPaymentMethod { get; set; }
        public string szRegID { get; set; }

        public string szJobTitle { get; set; }
        public string szPerID { get; set; }
        public string szRegDate { get; set; }

        public APAPMessage()
        { 
        
        
        }
    }
}