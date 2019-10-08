using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Parsnet
{
    public class Cryptor
    {
        static int KeySize = 2048;

        public static void GetRSAKey(out string privateKey, out string publicKey)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeySize);
            publicKey = provider.ToXmlString(false);
            privateKey = provider.ToXmlString(true);
        }

        public static string EncryptRSA(string input, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeySize);
            provider.FromXmlString(key);
            byte[] data = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(provider.Encrypt(data, true));
        }

        public static string DecryptRSA(string input, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeySize);
            provider.FromXmlString(key);
            byte[] data = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(provider.Decrypt(data, true));
        }

        public static string EncryptRijndael(string text, string Key, string IV)
        {
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Convert.FromBase64String(Key);
                rijAlg.IV = Convert.FromBase64String(IV);

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string DecryptRijndael(string cipherText, string Key, string IV)
        {
            string plaintext = null;

            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Convert.FromBase64String(Key);
                rijAlg.IV = Convert.FromBase64String(IV);

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                var cipher = Convert.FromBase64String(cipherText);

                using (MemoryStream msDecrypt = new MemoryStream(cipher))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;
        }

        public static void GenerateKey(out string key, out string iv)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.GenerateKey();
                key = Convert.ToBase64String(myRijndael.Key);
                iv = Convert.ToBase64String(myRijndael.IV);
            }
        }

    }
}
