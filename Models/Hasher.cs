using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    interface IHasher
    {
        public string Getfilehash(byte[] mediaBytes);
    }
}
