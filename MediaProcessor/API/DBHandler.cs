using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.MainDataAccess;
using DTO;
using OfficeOpenXml;
using static DTO.Enums;


namespace MediaProcessor.API;
public class DBHandler
{
    SQLiteAccess DBConnection1;
    ExcelDataAccess excelDataAccess = new ExcelDataAccess(AppConfig.rootURL);
    static int TotalRowsCount;
    #region Database handling

    public bool AddSet<T>(List<T> entity,String key) where T : class
    {
        DBConnection1 = new SQLiteAccess(AppConfig.NameDBFile);
        return DBConnection1.Execute(entity,key, DBConnection.ExecuteActions.Insert);

    }
    public bool UpdateSet<T>(List<T> entity, String key) where T : class
    {
        return DBConnection1.Execute(entity, key, DBConnection.ExecuteActions.Update);

    }
    public bool DeleteSet<T>(List<T> entity, String key) where T : class
    {
        return DBConnection1.Execute(entity, key, DBConnection.ExecuteActions.Delete);

    }

    public List<T> GetDBSet<T>(object parameters = null)
    {
        DBConnection1 = new SQLiteAccess(AppConfig.NameDBFile);
        return DBConnection1.GetDBSet<T>();
    }

    public Enums.eStatus ChangeDataInDB(string tableType)
    {
        switch(tableType)
        {
            case "עדכון הקמפיינים":
                LoadDataFromDBToExcel<Campaign>();
                break;
            case "עדכון שלוחות":
                LoadDataFromDBToExcel<CallRoutingDTO>();
                break;
            case "עדכון ערוצים":
                LoadDataFromDBToExcel<ChannelExtension>();
                break;
            default:
                return Enums.eStatus.SUCCESS;


        }

        return Enums.eStatus.FAILED;
    }

    public Enums.eStatus ControlDataSync()
    {
        string[] videoFiles = Directory.GetFiles(AppConfig.rootURL, "*.xls*");
        //string[] fileNames = filePaths.Select(file => Path.GetFileName(file)).ToArray();
        Enums.eStatus statusDbUdate = Enums.eStatus.FAILED;

        foreach (var classType in videoFiles)
        {
            switch (Path.GetFileName(classType).Split(".")[0])
            {
                case "Campaign":
                    statusDbUdate = SyncDataToDB<Campaign>();
                    break;
                case "CallRouting":
                    statusDbUdate = SyncDataToDB<CallRoutingDTO>();
                    break;
                case "ChannelExtension":
                    statusDbUdate = SyncDataToDB<ChannelExtension>();
                    break;
                default:
                    break;
            }
        }
        return statusDbUdate;   
    }
    private Enums.eStatus  LoadDataFromDBToExcel<T>() where T : class, new()
    {
        bool status= false;
        List<T> list = GetDBSet<T>();
        excelDataAccess.DeleteExcelPackage<T>();
        status = excelDataAccess.InsetData<T>(list);
        TotalRowsCount = list.Count;
        if (status== true) {
            excelDataAccess.OpenExcelForUser<T>();
             }

        
        return Enums.eStatus.SUCCESS;
    }

    private Enums.eStatus SyncDataToDB<T>() where T : class, new()
    {
       List<T> objectList =  excelDataAccess.GetDataFromExcel<T>();
       
       DBConnection1.UpdatelistData<T>(objectList.GetRange(0,TotalRowsCount));
        DBConnection1.InsetData<T>(objectList.GetRange(TotalRowsCount,objectList.Count-TotalRowsCount));
      return excelDataAccess.DeleteExcelPackage<T>();
    }

    #endregion

}
