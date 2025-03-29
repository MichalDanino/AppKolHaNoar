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
    SQLiteAccess DBSQLite = new SQLiteAccess(AppConfig.NameDBFile);
    ExcelDataAccess DBExcel = new ExcelDataAccess(AppConfig.rootURL);
    static int TotalRowsCount;

    #region Database handling

    public bool AddSet<T>(List<T> entity,String key) where T : class
    {
        return DBSQLite.Execute(entity,key, DBConnection.ExecuteActions.Insert);

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
        DBSQLite = new SQLiteAccess(AppConfig.NameDBFile);
        return DBSQLite.GetDBSet<T>(selectedField,condition,parameters);
    }
     
    public Enums.eStatus ChangeDataInDB(string tableType)
    {
        eStatus status = eStatus.SUCCESS;
        switch(tableType)
        {
            case "עדכון הקמפיינים":
                status = LoadDataFromDBToExcel<Campaign>();
                break;
            case "עדכון שלוחות":
                status = LoadDataFromDBToExcel<CallRoutingDTO>();
                break;
            case "עדכון ערוצים":
                status = LoadDataFromDBToExcel<ChannelExtension>();
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
                case "CallRouting":
                    statusDBUdate = SyncDataToDB<CallRoutingDTO>();
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
            DBExcel.ApplyDataValidation<T>("יום לעדכון","שעה לעדכון");
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

        DBSQLite.UpdatelistData<T>(objectList.GetRange(0,TotalRowsCount));
        DBSQLite.InsetData<T>(objectList.GetRange(TotalRowsCount,objectList.Count-TotalRowsCount));
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
        

        foreach (ChannelExtension channel in objectList as List<ChannelExtension>)
        {
            if (channel.ChannelExtension_ChannelID != null)
            {
                if (channel.ChannelExtension_ChannelID.Contains("@"))
                {
                    channel.ChannelExtension_ChannelID = await YouTubeAPI.GetChannelIdByNameAsync(channel.ChannelExtension_ChannelID);
                }
            }
        }
        AppStaticParameter.channels.Clear();
        AppStaticParameter.channels.AddRange(objectList as List<ChannelExtension>);

        DBSQLite.UpdatelistData<T>(objectList.GetRange(0, TotalRowsCount));
        DBSQLite.InsetData<T>(objectList.GetRange(TotalRowsCount, objectList.Count - TotalRowsCount));
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
