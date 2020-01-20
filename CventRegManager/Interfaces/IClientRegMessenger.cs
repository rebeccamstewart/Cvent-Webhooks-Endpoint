using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CventRegManager.Models;
namespace CventRegManager.Interfaces
{
    public interface IClientRegMessenger
    {
        string SendNewMsg(APAPMessage apapMsg, IEmail Email);
    }
}
