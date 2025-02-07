using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.MainDataAccess;

namespace MediaProcessor.API;
public class DBHandler
{
    DBConnection DBConnection;

    #region Database handling

    public bool AddSet<T>(List<T> entity,String key) where T : class
    {

        return DBConnection.Execute(entity,key, DBConnection.ExecuteActions.Insert);

    }
    public bool UpdateSet<T>(List<T> entity, String key) where T : class
    {
        return DBConnection.Execute(entity, key, DBConnection.ExecuteActions.Update);

    }
    public bool DeleteSet<T>(List<T> entity, String key) where T : class
    {
        return DBConnection.Execute(entity, key, DBConnection.ExecuteActions.Delete);

    }
    
    #endregion

}
