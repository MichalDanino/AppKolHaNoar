using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.MainDataAccess;

namespace MediaProcessor.API;
public class DBHandler
{
    SQLiteAccess DBConnection1;

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
    #endregion

}
