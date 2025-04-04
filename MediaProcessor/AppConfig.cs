using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using DTO;
using MediaProcessor.API;

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
        public static string pathDBFile = "";
        public static string YouTubeDLPath = "";
        public static string FFmpegPath = "";
        public static string ManagerPassword = "";

        static byte[] iv = new byte[16];
        public static Dictionary<string, string> env = new Dictionary<string, string>();

        /*static parameter */
        public static List<GenericMessage> listExceptions;
        private static readonly string EnvFilePath = $"C:\\Program Files\\KolHaNoar\\env.secure";

        //ASC key
        public static  string EncryptionKey = "KolHaNoarYaf592";


        /// <summary>
        /// Constractor that init all configuration parameters 
        /// </summary>
        static AppConfig()
        {

            // The static constructor of the AppConfig class- executes only once 
            // when the AppConfig class is loaded for the first time, 
            // meaning when it is first accessed in the code.
            //Dictionary<string, string> config = ReadEnv();
            //string fileENV = MainDirectoryPath + @".env";
            //Env.Load(fileENV);
            //passwordYH = config["PASSWORD"];
            //UserNameYH = "0795553875";
            //rootURL = config["ROOTURL"];
            //apiKeyYT = config["YOUTUBE_API_KEY"];
            //apiProjectNameYT = config["APPLICATION_NAME"];
            //YouTubeDLPath = config["YOUTUBE_DL_PATH"];
            //ManagerPassword = config["MANAGERPASSWORD"];
            //listExceptions = new List<GenericMessage>();
            //NameDBFile = rootURL + "my_database.db";
            passwordYH = "";
            UserNameYH = "0795553875";
            rootURL = @"C:\Program Files\KolHaNoar\";
            apiKeyYT = "";
            apiProjectNameYT = "My First Project";
            YouTubeDLPath = @"C:\yt-dlgANDffmpeg\yt-dlg\yt-dlp.exe";
            FFmpegPath = @"C:\yt-dlgANDffmpeg\yt-dlg\ffmpeg.exe";
            ManagerPassword = "fvf";
            listExceptions = new List<GenericMessage>();
            pathDBFile = rootURL + "my_database.db";
            UpdateVideoSync();
        }

        public static string Decrypt(string key, string encryptedBase64)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedBase64);
            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));

            byte[] iv = new byte[16];
            Array.Copy(encryptedData, 0, iv, 0, iv.Length); // שליפת ה-IV מהנתונים

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd(); // החזרת הנתונים המפוענחים
                }
            }

        }

        
            //קוראת את המידע מהקובץ המוצפן, מפענחת אות ו בעזרת פונקצית הפיענוח
        public static Dictionary<string,string> ReadEnv()
        {
            EnsureEnvFileExists(); // ווידוא שהקובץ קיים

            string[] lines = File.ReadAllLines(EnvFilePath);
            
            foreach (string line in lines)
            {
                var parts = line.Split('=', 2);
                if (parts.Length == 2) 
                {
                    //env[parts[0].Trim()] = parts[1].Trim();
                    string key = parts[0].Trim();
                    string encryptedValue = parts[1].Trim();
                    try
                    {
                        //byte[] encryptedData = Convert.FromBase64String(encryptedValue);
                        string decryptedValue = Decrypt(EncryptionKey,encryptedValue );
                        env[key] = decryptedValue;
                    }
                    catch (FormatException ex)
                    {
                        throw new Exception($"Failed to decode Base64 for key {key}", ex);
                    }
                }
            }
            return env;
            //throw new KeyNotFoundException($"Key '{key}' not found in environment file.");
        }

        public static void EnsureEnvFileExists()
        {
            if (!File.Exists(EnvFilePath))
            {
                throw new Exception("קובץ הקונפיגורציה לא קיים אנא הרץ את הסקריפט RUNME כדי ליצור את הקובץ.");
            }
        }
       
        public static string Encrypt(string key, string plainText)
        {
            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key)); // שימוש ישיר ב-SHA256
            byte[] iv = new byte[16]; // יצירת IV רנדומלי
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv); // מילוי IV בערכים אקראיים
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var ms = new MemoryStream())
                {
                    ms.Write(iv, 0, iv.Length); // כתיבת ה-IV לזרם
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray()); // החזרת הנתונים המוצפנים
                }
            }
        }
        public static void UpdateEnv(string key, string newValue)
        {
            env[key] = newValue;

            using (var writer = new StreamWriter(EnvFilePath, false))
            {
                foreach (var kvp in env)
                {
                    string encryptedValue = Encrypt(EncryptionKey, kvp.Value);
                    writer.WriteLine($"{kvp.Key}={encryptedValue}");
                }
            }
        }



        /// <summary>
        /// Update the dictionary with channels sync time
        /// </summary>
        public static void UpdateVideoSync()
        {
            MultiSourceDataService dB = new MultiSourceDataService();  
            List<ChannelExtension> channels =  dB.GetDBSet<ChannelExtension>();
         //   AppStaticParameter.VideoSyncTime = channels.ToDictionary(obj => obj.ChannelExtension_ChannelID, obj => obj.ChannelExtension_RunningTime);

        }
     
      

    }
}
