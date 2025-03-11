using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Dapper;
using System.Linq;
using System.Data.Common;
using DataAccess.MainDataAccess;
using static Dapper.SqlMapper;
using System.Reflection;


namespace DataAccess;
public class SQLiteAccess : DBConnection
{
    private readonly string _connectionString;
    private readonly string _databaseFilePath;


    // מתכנן (Constructor) שמקבל את נתיב קובץ SQLite
    public SQLiteAccess(string databaseFilePath)
    {
        _databaseFilePath = databaseFilePath;
        _connectionString = $"Data Source={databaseFilePath};Version=3;";
        EnsureDatabaseExists();

    }

    // פונקציה להחזרת חיבור למסד הנתונים
    private SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }

    // בדיקה ויצירת הקובץ אם לא קיים
    private void EnsureDatabaseExists()
    {
        if (!File.Exists(_databaseFilePath))
        {
            SQLiteConnection.CreateFile(_databaseFilePath); // יצירת הקובץ
            Console.WriteLine("Database file created.");
        }
    }
    // הוספת נתונים
    public override bool InsetData<T>(List<T> entity) where T : class
    {
        var typeClass = typeof(T);
        using (var connection = GetConnection())
        {
            string tableName = typeClass.Name;
            connection.Open();
            // Check if the table exists
            string tableExistsQuery = $"SELECT name FROM sqlite_master WHERE type='table' AND name=@TableName";
            bool tableExists = connection.Query<string>(tableExistsQuery, new { TableName = tableName }).Any(); // **שימוש בפרמטר במקום הכנסת שם הטבלה ישירות**

            if (!tableExists)
            {
                // Create table if it doesn't exist

                string columns = string.Join(",", typeClass.GetProperties().Select(p =>
                {
                    if (p.Name.Contains("_ID", StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(int))
                        return $"{p.Name} INTEGER PRIMARY KEY AUTOINCREMENT";
                    else
                        return $"{p.Name} TEXT";
                }));

                string createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} ({columns})";
                connection.Execute(createTableQuery);
            }
            var properties = typeClass.GetProperties()
          .Where(p => !(p.Name.Contains("_ID", StringComparison.OrdinalIgnoreCase) && p.PropertyType == typeof(int)))
          .ToList();

            string query = $"INSERT INTO {tableName} ({string.Join(",", properties.Select(p => p.Name))}) " +
                         $"VALUES ({string.Join(",", properties.Select(p => "@" + p.Name))})";

            connection.Execute(query, entity);
        }
        return true;
    }

    public override bool UpdateData<T>(T entity)
    {
        object uniqueValue = "";
        using (var connection = GetConnection())
        {
            string tableName = typeof(T).Name;
            connection.Open();

            string uniqueValueName = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Contains("_ID"))?.Name.ToString() ?? string.Empty;
            var property = typeof(T).GetProperty(uniqueValueName);
            if (property == null)
            {
                throw new Exception("שדה המזהה הייחודי לא קיים בישות.");

            }
            uniqueValue = property.GetValue(entity);

            var setClause = string.Join(", ", entity.GetType()
                             .GetProperties()
                             .Where(p => p.Name != uniqueValueName) // מוודא שה-ID לא יהיה בסט
                             .Select(p => $"{p.Name} = @{p.Name}"));

            // יצירת שאילתת עדכון
            var query = $"UPDATE {tableName} SET {setClause} WHERE {uniqueValueName} = @UniqueValue";

            //var setClause = string.Join(", ", entity.GetType().GetProperties().Select(p => $"{p.Name} = @{p.Name}"));
            //var query = $"UPDATE {tableName} SET {setClause} WHERE {uniqueValueName} = @uniqueValue ";
            var parameters = entity.GetType()
                              .GetProperties()
                              .Where(p => p.Name != uniqueValueName)
                              .ToDictionary(p => p.Name, p => p.GetValue(entity));

            parameters["UniqueValue"] = uniqueValue;
            connection.Execute(query, parameters);
        }
        return true;
    }

    // Delete Operation
    public override bool RemoveData<T>(string condition) where T : class
    {
        using (var connection = GetConnection())
        {
            string tableName = typeof(T).Name;
            string setClause = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Contains("_ID"))?.Name.ToString() ?? string.Empty;
            connection.Open();
            var query =  condition!= ""? $"DELETE FROM {tableName} WHERE {setClause}= @condition" : $"DELETE FROM {tableName}";
            connection.Execute(query);
            return true;
        }
        return false;
    }


    protected override string GetFilePath<T>() where T : class
    {
        //not need in this class
        return null;
    }
    // Select Operation
    public List<T> GetDBSet<T>(string selectedField = "", object parameters = null)
    {
        try
        {
            using (var connection = GetConnection())
            {
                string query = "SELECT * FROM " + typeof(T).Name;
                query = (selectedField != "") ? query.Replace("*", selectedField) : query;
                connection.Open();
                return connection.Query<T>(query, parameters).ToList();
            }
        }
        catch (Exception ex)
        {
            return new List<T>();
        }
    }

    public bool UpdatelistData<T>(List<T> values) where T : class
    {
        bool status = false;
        foreach (var item in values)
        {
            status = UpdateData<T>(item);
        }
        return status;
    }

    /// <summary>
    /// The ExecuteScalarQuery function runs a SQL query on an SQLite database and returns a single value. 
    /// It connects to the database, runs the query, and retrieves the result. This is useful for getting 
    /// simple values like the number of rows in a table.
    /// </summary>
    /// <param name="tableName">name table</param>
    /// <param name="CurrectQuery">the query</param>
    /// <returns></returns>

    public int ExecuteScalarQueryInt(string tableName, string CurrectQuery)
    {
        try
        {
            using (var connection = GetConnection())
            {
                string query = $"SELECT {CurrectQuery} FROM {tableName}";

                connection.Open();
                return connection.Query<int>(query).FirstOrDefault();

            }
        }
        catch
        {
            return 0;
        }
    }
}
