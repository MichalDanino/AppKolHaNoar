using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using DTO;

namespace MediaProcessor
{
    public static class AppConfig
    {
        /*Configuration parameter */
        public static string MainDirectoryPath = @"C:\Program Files\KolHaNoar\";
        public static List<GenericMessage> exceptions = new List<GenericMessage>();
        public static string rootURL = "";
        public static string UserNameYH = "";
        public static string passwordYH = "";
        public static string apiKeyYT = "";
        public static string apiProjectNameYT = "";
        public static string NameDBFile = "";

        /*static parameter */
        public static List<GenericMessage> listExceptions;
        private static readonly string EnvFilePath = "secure.env";
        private static readonly string EncryptionKey = "Your32CharSecureKeyHere!1234567890"; // יש להחליף במפתח מאובטח


        /// <summary>
        /// Constractor that init all configuration parameters 
        /// </summary>
        static AppConfig()
        {

            // The static constructor of the AppConfig class- executes only once 
            // when the AppConfig class is loaded for the first time, 
            // meaning when it is first accessed in the code.

            string fileENV = MainDirectoryPath + @".env";
            Env.Load(fileENV);
            passwordYH = Env.GetString("PASSWORD");
            UserNameYH = Env.GetString("USERNAME");
            rootURL = Env.GetString("ROOTURL");
            apiKeyYT = Env.GetString("YOUTUBE_API_KEY");
            apiProjectNameYT = Env.GetString("ApplicationName");
            listExceptions = new List<GenericMessage>();
            NameDBFile = rootURL + "my_database.db";
        }

        private static byte[] EncryptData(string plainText, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }
                    return ms.ToArray();
                }
            }
        }

        public static string ReadEnv(string key)
        {
            EnsureEnvFileExists(); // ווידוא שהקובץ קיים

            byte[] encryptedData = File.ReadAllBytes(EnvFilePath);
            string decryptedData = DecryptData(encryptedData, EncryptionKey);

            foreach (string line in decryptedData.Split('\n'))
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2 && parts[0].Trim() == key)
                    return parts[1].Trim();
            }

            throw new KeyNotFoundException($"Key '{key}' not found in environment file.");
        }

        public static void EnsureEnvFileExists()
        {
            if (!File.Exists(EnvFilePath))
            {
                Console.WriteLine("Environment file not found. Creating a new one...");
                WriteEnv("DUMMY_KEY", "DEFAULT_VALUE"); // יצירת קובץ עם ערך ברירת מחדל
            }
        }

        public static void WriteEnv(string key, string value)
        {
            string data = $"{key}={value}";
            byte[] encryptedData = EncryptData(data, EncryptionKey);
            File.WriteAllBytes(EnvFilePath, encryptedData);
        }

        private static string DecryptData(byte[] encryptedData, string key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                byte[] iv = new byte[aes.IV.Length];
                Array.Copy(encryptedData, iv, iv.Length);

                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                using (var ms = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }

    }
}
