using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.MainDataAccess;
using DTO;
using OfficeOpenXml;
using static DTO.Enums;


namespace MediaProcessor.API;
public class MultiSourceDataService
{
    SQLiteAccess DBSQLite = new SQLiteAccess(AppConfig.pathDBFile);
    ExcelDataAccess DBExcel = new ExcelDataAccess(AppConfig.rootURL);
    static int TotalRowsCount;

    #region Database handling

    public bool AddSet<T>(List<T> entity,String key="") where T : class
    {
        return DBSQLite.Execute(entity,key="", DBConnection.ExecuteActions.Insert);

    }
    public bool UpdateSet<T>(List<T> entity, String key) where T : class
    {
        return DBSQLite.Execute(entity, key, DBConnection.ExecuteActions.Update);

    }
    public bool DeleteSet<T>(List<T> entity, String key) where T : class
    {
        return DBSQLite.Execute(entity, key, DBConnection.ExecuteActions.Delete);

    }

    public List<T> GetDBSet<T>(string selectedField = "",string condition="", object parameters = null)
    {
        DBSQLite = new SQLiteAccess(AppConfig.pathDBFile);
        return DBSQLite.GetDBSet<T>(selectedField,condition,parameters);
    }
     
    public Enums.eStatus ChangeDataInDB(string tableType)
    {
        eStatus status = eStatus.SUCCESS;
        switch(tableType)
        {
            case "עדכון הקמפיינים":
                status = LoadDataFromDBToExcel<Campaign>();
                DBExcel.ApplyDataTimeValidation<Campaign>("יום לעדכון", "שעה לעדכון");
                DBExcel.LockColunm<Campaign>(new List<string>() { "שם הקמפיין", "מס' קמפיין" });
                break;
            
            case "עדכון ערוצים":
                status = LoadDataFromDBToExcel<ChannelExtension>();
                DBExcel.ApplyDataTimeValidation<ChannelExtension>("יום לעדכון", "שעה לעדכון");
                DBExcel.ApplyNotEmptyValidation<ChannelExtension>(new List<string>() { "מספר השלוחה לסרטונים קצרים(פחות מ10 דק')", "מס' השלוחה לסרטונים ארוכים" });
                break;
            case "מילים לסינון":
                status = LoadDataFromDBToExcel<ForbiddenWords>();
                break;
            default:
                return Enums.eStatus.FAILED;

        }
        return status;
    }
    

    public Enums.eStatus ControlDataSync()
    {
        string[] videoFiles = Directory.GetFiles(AppConfig.rootURL, "*.xls*");
        //string[] fileNames = filePaths.Select(file => Path.GetFileName(file)).ToArray();
        Enums.eStatus statusDBUdate = Enums.eStatus.NotHaveNews;

        foreach (var classType in videoFiles)
        {
            switch (Path.GetFileName(classType).Split(".")[0])
            {
                case "Campaign":
                    statusDBUdate = SyncDataToDB<Campaign>();
                  

                    break;
               
                case "ChannelExtension":
                    statusDBUdate = SyncDataToDB<ChannelExtension>();
                  
                    break;
                case "ForbiddenWords":
                    statusDBUdate = SyncDataToDB<ForbiddenWords>();
                    break;
                default:
                    break;
            }
        }
        return statusDBUdate;   
    }
    private Enums.eStatus  LoadDataFromDBToExcel<T>() where T : class, new()
    {
        bool status= false;
        //Get Data from SQLite
        List<T> list = GetDBSet<T>();

        //Delete Excel
        DBExcel.DeleteExcelPackage<T>();

        //Create New EXCEL with data
        status = DBExcel.InsetData<T>(list);
        //Data validation
        if (typeof(T).Name == "ChannelExtension"|| typeof(T).Name == "Campaign")
        {
        }

        TotalRowsCount = list.Count;

        if (status== true) 
        {
            DBExcel.OpenExcelForUser<T>();
        }

        
        return Enums.eStatus.SUCCESS;
    }

    private Enums.eStatus SyncDataToDB<T>() where T : class, new()
    {
       List<T> objectList =  DBExcel.GetDataFromExcel<T>();
        if (typeof(T).Name == "ChannelExtension") 
        { 
            UpdateChannelData(objectList);
        return DBExcel.DeleteExcelPackage<T>();
        }
        List<T> tempList = FilterListByProperty(objectList,"_ID",0,eFilter.notEqual);
        DBSQLite.InsetData<T>(tempList);
        tempList = FilterListByProperty(objectList,"_ID",0,eFilter.EQUALS);

        DBSQLite.UpdatelistData<T>(tempList);
         return DBExcel.DeleteExcelPackage<T>();
    }

    public async Task UpdateTable<T>(List<T> values, string propertyToComper) where T : class, new()
    {
        PropertyInfo properties = typeof(T).GetProperty(propertyToComper);

        int count = DBSQLite.ExecuteScalarQueryInt(typeof(T).Name, "COUNT(*)");
        if (count < values.Count)
        {
            List<T>valueInDB = DBSQLite.GetDBSet<T>();
            List<T> NewValue = values.Where(x => !valueInDB.Any(y => properties.GetValue (y) ==properties.GetValue( x))).ToList();
            DBSQLite.InsetData(NewValue);
        }
    }

    public async Task UpdateCampainTable(List<Campaign> campainTable)
    {
        if(DBSQLite.TableExists<Campaign>())
        { 
            DBSQLite.RemoveData<Campaign>("");
            UpdateTable(campainTable, "Campaign_Number");
        }
        else
            DBSQLite.InsetData(campainTable);
    }

    public async Task UpdateChannelData<T>(List<T> objectList) where T : class, new() 
    {
        
        List<ChannelExtension> channelExtensions = objectList as List<ChannelExtension>?? new List<ChannelExtension>();

        foreach (ChannelExtension channel in channelExtensions)
        {
            if (channel.ChannelExtension_ChannelID != null)
            {
                if (channel.ChannelExtension_ChannelID.Contains("@"))
                {
                    channel.ChannelExtension_ChannelID = await YouTubeAPI.GetChannelIdByNameAsync(channel.ChannelExtension_ChannelID);
                }
            }
            if(channel.ChannelExtension_Long == "" &&  channel.ChannelExtension_Short == "")
            {
          //      efrdc
        //      await  Exceptions.checkExtensionMappingValidity(channel.ChannelExtension_ChannelID);
            }
        }
        AppStaticParameter.channels.Clear();
     

        List<T> tempList = FilterListByProperty(objectList, "_ID", 0, eFilter.notEqual);
        DBSQLite.UpdatelistData<T>(tempList);

       tempList = FilterListByProperty(objectList, "_ID", 0, eFilter.EQUALS);
        DBSQLite.InsetData<T>(tempList);

        AppStaticParameter.channels.AddRange(GetDBSet<ChannelExtension>());

    }
    private static List<T> FilterListByProperty<T>(List<T> list, string propertyName, object value ,eFilter filter)
    {
        PropertyInfo prop = typeof(T).GetProperties().FirstOrDefault(a => a.Name.Contains(propertyName));
        if (prop == null) return null; // אם אין מאפיין כזה, דלג על האובייקט

        return list.Where(item =>
        {
            return (filter ==eFilter.EQUALS) ? prop.GetValue(item)?.Equals(value) == true: prop.GetValue(item)?.Equals(value) != true;
        }).ToList();
    }

    #endregion


    #region Configuration file


    public List<string>   ShowPasswort()
    {
        List<string> strings = new List<string>();
        AppConfig.ReadEnv();
        foreach (var kvp in AppConfig.env)
        {
            strings.Add($"{kvp.Key} = {kvp.Value}");
        }
        return strings;
    }
    public eStatus UpdatePassword(string nameProperty, string password)
    {
        AppConfig.UpdateEnv(nameProperty, password);
        return eStatus.SUCCESS;
    }

    #endregion

}
