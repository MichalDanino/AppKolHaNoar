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
        public static string apiDID = "";
        public static string password = "";

   static  AppConfig()
        {
            string fileENV = MainDirectoryPath + @".env";
            // Assembly assembly = Assembly.Load("DTO");
            Env.Load(fileENV);
            password = Env.GetString("PASSWORD");
            apiDID = Env.GetString("USERNAME");
            rootURL = Env.GetString("RootURL");

        }

    }
}
