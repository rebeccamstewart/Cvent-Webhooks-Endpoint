using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CventRegManager.Models
{
    public class CventReg
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public bool FirstTime { get; set; }
    }
}