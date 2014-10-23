using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace JspHashcodMarker
{
    public class FileNameToHashcode
    {
        //Taken from google search
        public string SHA1(string data)
        {
            byte[] hash = new SHA1CryptoServiceProvider().ComputeHash(
                                 new UTF8Encoding().GetBytes(data));
            string str = string.Empty;
            foreach (byte num in hash)
                str = str + string.Format("{0,0:x2}", (object)num);
            return str;
        }
    }
}
