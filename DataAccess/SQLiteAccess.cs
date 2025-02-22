using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Dapper;
using System.Linq;
using System.Data.Common;
using DataAccess.MainDataAccess;


namespace DataAccess;
public class SQLiteAccess :DBConnection
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
            SQLiteConnection.CreateFile(_databaseFilePath+ "database.db"); // יצירת הקובץ
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
            var tableExistsQuery = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
            var tableExists = connection.Query<string>(tableExistsQuery).Any();

            if (!tableExists)
            {
                // Create table if it doesn't exist
              
                var columns = string.Join(",", typeClass.GetProperties().Select(p => $"{p.Name} TEXT"));
                var createTableQuery = $"CREATE TABLE IF NOT EXISTS {tableName} ({columns})";
                connection.Execute(createTableQuery);
            }
            var query = $"INSERT INTO {tableName} ({string.Join(",", typeClass.GetProperties().Select(p => p.Name))}) " +
                        $"VALUES ({string.Join(",", typeClass.GetProperties().Select(p => "@" + p.Name))})";

             connection.Execute(query, entity);
        }
        return true;
    }

    public override bool UpdateData<T>( T entity, string condition)
    {
        using (var connection = GetConnection())
        {
            string tableName = typeof(T).Name;  
            connection.Open();
            var setClause = string.Join(", ", entity.GetType().GetProperties().Select(p => $"{p.Name} = @{p.Name}"));
            var query = $"UPDATE {tableName} SET {setClause} WHERE {condition}";

             connection.Execute(query, entity);
        }
        return true;
    }

    // Delete Operation
    public override bool RemoveData<T>(string condition) where T : class
    {
        using (var connection = GetConnection())
        {
            string tableName = typeof (T).Name; 
            connection.Open();
            var query = $"DELETE FROM {tableName} WHERE {condition}";
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
    public  List<T> GetDBSet<T>(string selectedField="", object parameters = null)
    {
        using (var connection = GetConnection())
        {
           string query = "SELECT * FROM " + typeof(T).Name;
            query = (selectedField!="")? query.Replace("*",selectedField): query;
            connection.Open();
            return connection.Query<T>(query, parameters).ToList();
        }
    }
}
