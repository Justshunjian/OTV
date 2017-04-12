using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Otv.Utils
{
    public class MD5Utils
    {
        /// <summary>
        /// MD5　32位加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string encode(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            MD5 md5 = new MD5CryptoServiceProvider();
            StringBuilder sb = new StringBuilder(32);
            ///加密后是一个字节类型的数组(16Byte)
            byte[] s = md5.ComputeHash(bytes);
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}