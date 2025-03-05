using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;

namespace DataAccess.MainDataAccess;
public abstract class DBConnection
{
    public static string utilityString = "";
    public static ColumnValuePair columnValuePair = new ColumnValuePair();

 
    

    /// <summary>
    /// Defines action status
    /// </summary>
    public enum ExecuteActions
    {
        Insert,
        Update,
        Delete,
        GetAll
    }

    /// <summary>
    /// Brif : Handler data access
    /// </summary>
    /// <typeparam name="T">The type of the object on which the action is performed. T must be a class type. </typeparam>
    /// <param name="entity">entity data for action</param>
    /// <param name="exAction">Type of action</param>
    /// <returns>true if successful otherwise, returns false</returns>
    public bool Execute<T>(List<T> entity, string updatePrimaryKey, ExecuteActions exAction) where T : class
    {

        bool isSecssed = false;
        if (entity.Any())
        {
            try
            {
                switch (exAction)
                {
                    case ExecuteActions.Insert:
                        InsetData(entity);
                        isSecssed = true;
                        break;
                    case ExecuteActions.Update:
                        UpdateData(entity[0]);

                        isSecssed = true;
                        break;
                    case ExecuteActions.Delete:
                        RemoveData<T>(updatePrimaryKey);
                        isSecssed = true;
                        break;
                    default:
                        break;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        return false;

    }

    /// <summary>
    /// Add data to Database
    /// </summary>
    /// <typeparam name="T">The type of the object on which the action is performed. T must be a class type. </typeparam>
    /// <param name="entity">entity data for action</param>
    /// <returns>true if successful otherwise, returns false</returns>
    public abstract bool InsetData<T>(List<T> entityList) where T : class;

    /// <summary>
    /// Updates data by unique identifier
    /// </summary>
    /// <typeparam name="T">The type of the object on which the action is performed. T must be a class type. </typeparam>
    /// <param name="entity">entity data for update</param>
    /// <param name="updatePrimaryKey">Holds the column name and value used to identify the row to be update</param>
    /// <returns>true if successful otherwise, returns false</returns>
    public abstract bool UpdateData<T>(T entity) where T : class;

    /// <summary>
    /// Delete date by unique identifier
    /// </summary>
    /// <typeparam name="T">The type of the object on which the action is performed. T must be a class type. </typeparam>
    /// <param name="RemovePrimaryKey">Holds the column name and value used to identify the row to be deleted</param>
    /// <returns>true if successful otherwise, returns false</returns>
    public abstract bool RemoveData<T>(string RemovePrimaryKey) where T : class;

    protected abstract string GetFilePath<T>() where T : class;
   


}

