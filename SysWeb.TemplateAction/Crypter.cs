using System;
using System.Security.Cryptography;
using System.Text;

namespace SysWeb.TemplateAction
{
    public class Crypter
    {
     

        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(key);
            provider.Mode = CipherMode.ECB;
            return provider.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// 对称解密
        /// </summary>
        /// <param name="encrypted"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string encrypted, string key)
        {
            byte[] buffer = Convert.FromBase64String(encrypted);
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            return Encoding.UTF8.GetString(Decrypt(buffer, bytes));
        }

        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = new MD5CryptoServiceProvider().ComputeHash(key);
            provider.Mode = CipherMode.ECB;
            return provider.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }
        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="original"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string original, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(original);
            byte[] buffer2 = Encoding.UTF8.GetBytes(key);
            return Convert.ToBase64String(Encrypt(bytes, buffer2));
        }

    }
}
