using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CventRegManager.Models
{
    public class CventMessage
    {

        public string eventType { get; set; }
        public string messageTime { get; set; }
        public string messageStub { get; set; }
        //public CventRegObject message { get; set; }
        public CventRegMessage[] message { get; set; }



    }

    public class CventRegMessage
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string prefix { get; set; }
        public string nickName { get; set; }
        public string middleName { get; set; }
        public string fullName { get; set; }


        public string contactType { get; set; } //Member, Non-Member 
        public string isGuest { get; set; }  //false
        public string updatedDate { get; set; } //"2016-04-14"
        public string registrationDate { get; set; } //"2016-04-14",
        
        
        public string workAddress1 { get; set; }
        public string workAddress2 { get; set; }
        public string workAddress3 { get; set; }
        public string workStateCode { get; set; }
        public string workZipcode { get; set; }
        public string workCity { get; set; }
        public string workFax { get; set; }
        public string workCountry { get; set; } //"USA",


        public string homeAddress1 { get; set; }
        public string homeAddress3 { get; set; }
        public string homeAddress2 { get; set; }
        public string homeCity { get; set; }
        public string homeState { get; set; }
        public string homeZipcode { get; set; }
        public string homeFax { get; set; }
        public string homeStateCode { get; set; }
        public string homeCountry { get; set; }

        public string confirmationNumber { get; set; } // "KHN5593ZFTC"

        public string mobilePhone { get; set; }
        public string responseMethod { get; set; } //"Self-Responded"

        public string eventStub { get; set; } //"1B6B1CC5-32A5-4A8E-B093-7990E5E7CD45",

        public string designation { get; set; }
       



        public string eligibleCredits { get; set; } // "0.00",
        public string gender { get; set; }
        public customField[] customFields { get; set; }
        

        
        
        public string referenceId { get; set; }
        public string optedOut { get; set; } //"no",

        public string totalCredits { get; set; } // "0.00",

        public string workState { get; set; }
        public string company { get; set; }
        public string participantStatus { get; set; } // false,

        public string registrationPath { get; set; }//"Registration Option",

        public string accountStub { get; set; } //"B39363E5-5BB0-419A-84E8-C3D83EE3BA1D",

        public string email { get; set; }
        public string ccEmail { get; set; }
        public string visibleOnAttendeeList { get; set; } // 1,

        public string dateOfBirth { get; set; }
        
        public string inviteeStub { get; set; } //"9CC0C0F1-D39C-446E-A86E-235C848C7770",

        public string invitationBy { get; set; } //"No Invitation",

       

        public string unsubscribed { get; set; } //"no",

        
        public string inviteeStatus { get; set; } //"Accepted",

        public string internalNote { get; set; }
        
        public string workPhone { get; set; }


    }

    public class customField {
        public string id { get; set; } // "0541BE49-1A0A-48AD-9B49-D4F7DDCD5880",
        public string name { get; set; } //"Email Address Type","Professional Designation","Title Category",
        public string value { get; set; }
        
    
    }
}