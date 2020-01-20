using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CventRegManager.Models;
namespace CventRegManager.Interfaces
{
    public interface ICventRegRepository
    {

        
        CventAttendee GetCventAttendee(string registrationGUID, string eventGUID, string APIEventPW);
    }
}
