using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CventRegManager.Models
{
    public class CventAttendee
    {
        public string cventInvitteeId { get; set; }
        public string confirmationNumber { get; set; }
        public string personMembershipId { get; set; }
        public string orgMembershipId { get; set; }
        public string paymentMethod { get; set; }
        public string contactType { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string guestName { get; set; }
        public string regDate { get; set; }
        public int LuncheonTicketsPurchased { get; set; }
        public bool SponsorLunchTable { get; set; }
        public string HeardAboutValue{ get; set; }
        public bool Volunteer { get; set; }
        public bool OptOut{ get; set; }
        public bool PrimaryRegistration{ get; set; }
        public bool FirstTime { get; set; }
        public bool AttendLuncheon { get; set; }
        public string ProcessedGUID { get; set; }    

        public CventAttendee()
        {
            cventInvitteeId = "";
            confirmationNumber = "";
            personMembershipId = "";
            orgMembershipId= "";
            contactType = "";
            firstName= "";
            lastName= "";
            email = "";
            title= "";
            guestName = "";
            HeardAboutValue = "";
            regDate = "";
            paymentMethod= "";
            LuncheonTicketsPurchased = 0;
            SponsorLunchTable= false;
            Volunteer = false;
            OptOut= false;
            PrimaryRegistration=false;
            FirstTime = false;
            AttendLuncheon = false;
            ProcessedGUID = "";
        }

       

    }
}