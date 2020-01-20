using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CventRegManager.Models;

namespace CventRegManager.Interfaces
{
    public interface IHR_MRA_Repository
    {


        bool CreateRegLog(CventAttendee att, string jsonMsgBody, string eventStub);
        bool LogSimpleCall(string PostBodyData, string cventAuthHeader, string verb, bool Authorized);
        bool UpdateRegLogAsProcessed(CventAttendee att);
        bool UpdateRegLogWithErrorMessage(CventAttendee att, string ErrorMessage);
        string RegProcessedStatus(string inviteeStub);
    }
}
