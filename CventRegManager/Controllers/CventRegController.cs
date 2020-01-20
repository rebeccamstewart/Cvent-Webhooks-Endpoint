using CventRegManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using CventRegManager.Domain;
using CventRegManager.Services;

using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CventRegManager.Controllers
{
    public class CventRegController : ApiController
    {
       

        public CventRegController()
        {
           
        }


        public HttpResponseMessage Get()
        {
            string headerValues;
            if (Request.Headers.Authorization == null)
            {
                headerValues = "Null";
            }

            else
            {
                headerValues = Request.Headers.Authorization.ToString();
            }

            var AuthKey = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.DataAccess");



            bool KeysMatch = false;
            if (AuthKey == headerValues)
            {
                KeysMatch = true;
            }

            var CventAttRepo = new CventAttendeeRepository();
            var APAP_Msnger = new APAPRegMessenger();
            var HR_MRA_Repo = new HR_MRA_Repository();
            var WL = new WebhookManager(CventAttRepo, APAP_Msnger, HR_MRA_Repo);


            WL.LogJsonData("", headerValues, "GET", KeysMatch);

            if (KeysMatch == true)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);

            }




        }
        
        public HttpResponseMessage Post(CventMessage cventMessage) //public HttpResponseMessage Post(JObject  cventMessage)
        {
            
            //Extract Body, Cvent Objects and/or header information from posted data
            bool LogSuccess = false;
            string headerValues;
            var BodyData = "";
            var MemberCustomFieldGUID = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.MemberCustomFieldGUID");
            try
            {
                BodyData = Newtonsoft.Json.JsonConvert.SerializeObject(cventMessage);
            }
            catch
            {
                BodyData = "Error converting from  object";
            }
            headerValues = (Request.Headers.Authorization == null) ? "Null" : Request.Headers.Authorization.ToString();

            var CventAttRepo = new CventAttendeeRepository();
            var APAP_Msnger = new APAPRegMessenger();
            var HR_MRA_Repo = new HR_MRA_Repository();
            var WL = new WebhookManager(CventAttRepo, APAP_Msnger, HR_MRA_Repo);

            //Validate Authorization Header
            bool KeysMatch = IsValidAuth(headerValues);

            WL.LogJsonData(BodyData, headerValues, "Post", KeysMatch);

            //Log call to endpoint regardless of valid authcode and body of post
            if (cventMessage.message[0] != null && KeysMatch)
            {
                 LogSuccess = WL.ProcessPostData(BodyData, cventMessage, headerValues, "Post", KeysMatch);
                 WL.LogJsonData(WL.ProcessLogData, headerValues, "Post", KeysMatch);
            }
            else
            {
                
                LogSuccess = false;
            }

            //Return correct response based on valid auth header and/or valid post data
            if (KeysMatch == true)
            {
                if (LogSuccess == true)
                {
                    return Request.CreateResponse<object>(System.Net.HttpStatusCode.Created, cventMessage);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }


        }
        private bool IsValidAuth(string authValuePassedIn)
        {

            var AuthKey = System.Configuration.ConfigurationManager.AppSettings.Get("Cvent.Webhooks.DataAccess");
            return (AuthKey == authValuePassedIn);
        }

    }
}
