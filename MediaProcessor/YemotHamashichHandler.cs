using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DotNetEnv;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Linq;
using DataAccess;
using DTO;


namespace MediaProcessor
{
    public class YemotHamashichHandler
    {
        YemotHamashichHandler HandlerYH  = new YemotHamashichHandler();
        SQLiteAccess Access = new SQLiteAccess(AppConfig.rootURL + @"my_database.db");
        
        public bool UplaodFiles()
        {
            HandlerYH.UplaodFiles();
            return true;
        }

      

    }
}
