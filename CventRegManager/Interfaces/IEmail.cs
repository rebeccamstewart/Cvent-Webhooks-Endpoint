using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CventRegManager.Interfaces
{
    public interface IEmail
    {
         bool Send(string ToEmail, string Body);
         void UpdateSubject(string subject);
    }
}
