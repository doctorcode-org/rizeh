namespace Parsnet
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;

    public class Cryptor
    {
        private static int KeySize = 0x800;

        public static string DecryptRijndael(string cipherText, string Key)
        {
            string str;
            byte[] rgbIV = Encoding.ASCII.GetBytes("tu89geji340t89u2");
            int num = 0x100;
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (PasswordDeriveBytes bytes = new PasswordDeriveBytes(Key, null))
            {
                byte[] rgbKey = bytes.GetBytes(num / 8);
                using (RijndaelManaged managed = new RijndaelManaged())
                {
                    managed.Mode = CipherMode.CBC;
                    using (ICryptoTransform transform = managed.CreateDecryptor(rgbKey, rgbIV))
                    {
                        using (MemoryStream stream = new MemoryStream(buffer))
                        {
                            using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                            {
                                byte[] buffer4 = new byte[buffer.Length];
                                int count = stream2.Read(buffer4, 0, buffer4.Length);
                                str = Encoding.UTF8.GetString(buffer4, 0, count);
                            }
                        }
                    }
                }
            }
            return str;
        }

        public static string DecryptRijndael(string cipherText, string Key, string IV)
        {
            using (RijndaelManaged managed = new RijndaelManaged())
            {
                managed.Key = Convert.FromBase64String(Key);
                managed.IV = Convert.FromBase64String(IV);
                ICryptoTransform transform = managed.CreateDecryptor(managed.Key, managed.IV);
                using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(stream2))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string DecryptRSA(string input, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeySize);
            provider.FromXmlString(key);
            byte[] rgb = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(provider.Decrypt(rgb, true));
        }

        public static string EncryptRijndael(string text, string Key)
        {
            string str;
            byte[] rgbIV = Encoding.ASCII.GetBytes("tu89geji340t89u2");
            int num = 0x100;
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            using (PasswordDeriveBytes bytes = new PasswordDeriveBytes(Key, null))
            {
                byte[] rgbKey = bytes.GetBytes(num / 8);
                using (RijndaelManaged managed = new RijndaelManaged())
                {
                    managed.Mode = CipherMode.CBC;
                    using (ICryptoTransform transform = managed.CreateEncryptor(rgbKey, rgbIV))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                            {
                                stream2.Write(buffer, 0, buffer.Length);
                                stream2.FlushFinalBlock();
                                str = Convert.ToBase64String(stream.ToArray());
                            }
                        }
                    }
                }
            }
            return str;
        }

        public static string EncryptRijndael(string text, string Key, string IV)
        {
            string str;
            using (RijndaelManaged managed = new RijndaelManaged())
            {
                managed.Key = Convert.FromBase64String(Key);
                managed.IV = Convert.FromBase64String(IV);
                ICryptoTransform transform = managed.CreateEncryptor(managed.Key, managed.IV);
                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream2))
                        {
                            writer.Write(text);
                        }
                        str = Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
            return str;
        }

        public static string EncryptRSA(string input, string key)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeySize);
            provider.FromXmlString(key);
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(provider.Encrypt(bytes, true));
        }

        public void GenerateKey(out string key, out string iv)
        {
            using (RijndaelManaged managed = new RijndaelManaged())
            {
                managed.GenerateKey();
                key = Convert.ToBase64String(managed.Key);
                iv = Convert.ToBase64String(managed.IV);
            }
        }

        public static void GetRSAKey(out string privateKey, out string publicKey)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider(KeySize);
            publicKey = provider.ToXmlString(false);
            privateKey = provider.ToXmlString(true);
        }
    }
}

