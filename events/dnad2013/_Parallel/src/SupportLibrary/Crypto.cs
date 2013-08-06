//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SupportLibrary
{
    public static class Crypto
    {
        private static char GetHexValue(int i)
        {
            return i < 10 ? (char)(i + 0x30) : (char)(i - 10 + 0x41);
        }

        [ThreadStatic]
        private static MD5 MD5Provider_;

        public static string ToMD5Hash(this byte[] byteArray)
        {
            if (null == MD5Provider_) MD5Provider_ = MD5.Create();
            return MD5Provider_.ComputeHash(byteArray).ToHexString();
        }

        [ThreadStatic]
        private static AesManaged AESProvider_;

        public static string GetAESKey()
        {
            if (null == AESProvider_) AESProvider_ = new AesManaged();
            AESProvider_.GenerateKey();
            return AESProvider_.Key.ToHexString();
        }

        public static string ToHexString(this byte[] byteArray)
        {
            int len = byteArray.Length * 2;
            char[] result = new char[len];           
            for (int i = 0, j = 0; i < len; i += 2, ++j)
            {
                byte b = byteArray[j];
                result[i] = GetHexValue(b / 0x10);
                result[i + 1] = GetHexValue(b % 0x10);
            }
            return new string(result, 0, result.Length);
            //or
            //var sb = new StringBuilder(byteArray.Length);
            //for (int i = 0; i < byteArray.Length; ++i)
            //    sb.Append(byteArray[i].ToString("X2"));
            //return sb.ToString();
            //or
            //return BitConverter.ToString(byteArray);
        }
    }
}
