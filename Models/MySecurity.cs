using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BiometricAPI.Models
{
    public class MySecurity
    {
        public static String EncryptedPassword(string pass)
        {
            SHA256 sha = SHA256Managed.Create();

            byte[] en_data = sha.ComputeHash(Encoding.UTF8.GetBytes(pass));
            return BitConverter.ToString(en_data).Replace("-", "").ToLower();
        }
    }
}