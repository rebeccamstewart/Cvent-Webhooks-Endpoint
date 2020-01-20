using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CventRegManager.Models
{
    public class CventRegObject
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string email { get; set; }
        public string optedOut { get; set; }
        public string ContactType { get; set; }
        public string confirmationNumber { get; set; }
        public DateTime registrationDate { get; set; }
        public List<customQuestion> customQuestions { get; set; }


    }

    public class customQuestion
    {
        public string id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}