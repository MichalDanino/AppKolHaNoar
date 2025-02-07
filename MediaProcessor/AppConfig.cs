using System;
using System.Collections.Generic;
using System.Linq;
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
        public static List<GenericException> exceptions = new List<GenericException>();
        public static string rootURL = "";
        public static string UserNameYH = "";
        public static string passwordYH = "";
        public static string apiKeyYT = "";
        public static string apiProjectNameYT = "";

        /*static parameter */
        public static List<GenericException> listExceptions;

        /// <summary>
        /// Constractor that init all configuration parameters 
        /// </summary>
        static  AppConfig()
        {
            // The static constructor of the AppConfig class- executes only once 
            // when the AppConfig class is loaded for the first time, 
            // meaning when it is first accessed in the code.

            string fileENV = MainDirectoryPath + @".env";
            // Assembly assembly = Assembly.Load("DTO");
            Env.Load(fileENV);
            passwordYH = Env.GetString("PASSWORD");
            UserNameYH = Env.GetString("USERNAME");
            rootURL = Env.GetString("ROOTURL");
            apiKeyYT = Env.GetString("YOUTUBE_API_KEY");
            apiProjectNameYT = Env.GetString("ApplicationName");
            listExceptions = new List<GenericException>();

        }

      

    }
}
