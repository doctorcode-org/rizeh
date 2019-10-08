namespace Parsnet
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public static class Log
    {
        public static void WriteError(MemberInfo callingMethod, Exception exp)
        {
            WriteError(callingMethod.Name, exp.Message);
        }

        public static void WriteError(string methodName, string exception)
        {
            string path = string.Format(@"{0}\Log.xml", Directory.GetCurrentDirectory());

            try
            {
                if (!File.Exists(path))
                {
                    try
                    {
                        string s = "<?xml version='1.0' encoding='utf-8'?><RunTimeError></RunTimeError>";
                        byte[] bytes = Encoding.UTF8.GetBytes(s);
                        FileStream stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                        stream.Close();
                        stream = null;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }

                XDocument document = XDocument.Load(path);
                document.Root.Add(new XElement("Error", new object[] { new XElement("Method", methodName), new XElement("Message", exception), new XAttribute("Date", DateTime.Now) }));
                document.Save(path);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}

