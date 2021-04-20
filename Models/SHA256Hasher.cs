using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NewsConcentratorSystem.Models
{
    //a class to getting MessageMedia hashcode.
    public class SHA256Hasher : IHasher
    {
        private SHA256 sha256;
        public SHA256Hasher()
        {
            sha256 = SHA256.Create();
        }

       public string Getfilehash(byte[] mediaBytes)
       {
           return sha256.ComputeHash(mediaBytes).ToString();
       }
    }
}
