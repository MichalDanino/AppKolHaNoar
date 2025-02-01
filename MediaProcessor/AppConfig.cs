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
        public static string MainDirectoryPath = @"C:\Program Files\KolHaNoar\";
        public static List<GenericException> exceptions = new List<GenericException>();
        public static string rootURL = "";
        public static string UserNameYH = "";
        public static string passwordYH = "";
        public static string apiKeyYT = "";
        public static string apiProjectNameYT = "";

        static  AppConfig()
        {
            string fileENV = MainDirectoryPath + @".env";
            // Assembly assembly = Assembly.Load("DTO");
            Env.Load(fileENV);
            passwordYH = Env.GetString("PASSWORD");
            UserNameYH = Env.GetString("USERNAME");
            rootURL = Env.GetString("RootURL");
            apiKeyYT = Env.GetString("YOUTUBE_API_KEY");
            apiProjectNameYT = Env.GetString("ApplicationName");


        }

    }
}
