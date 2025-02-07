using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataAccess.MainDataAccess;
using DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess
{
    public class JSONDataAccess : DBConnection
    {
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();

        public override bool InsetData<T>(List<T> entityList) where T : class
        {
            string filePath = GetFilePath<T>();

            // בדיקה אם הקובץ קיים, יצירת JSON ריק אם לא
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]"); // נתחיל כמערך ריק
            }

            // קריאת תוכן הקובץ והמרתו ל-JArray
            var jsonContent = File.ReadAllText(filePath);
            var jsonArray = JArray.Parse(jsonContent);
            foreach (var item in entityList)
            {
                // המרת האובייקט לפורמט JSON והוספתו למערך
                var newObject = JToken.FromObject(item);
                jsonArray.Add(newObject);
            }
            // שמירת השינויים חזרה לקובץ
            // File.WriteAllText(filePath, jsonArray.ToString(Formatting.Indented));
            return true;
        }
        public override bool UpdateData<T>(T entity, string updatePrimaryKey) where T : class
        {

            string filePath = GetFilePath<T>();

            columnValuePair.columnName = updatePrimaryKey.Split('.')[0];
            columnValuePair.Value = updatePrimaryKey.Split('.')[1];
            // בדיקה אם הקובץ קיים
            if (!File.Exists(filePath))
            {
                return false;
            }


            // קריאת תוכן הקובץ
            var jsonContent = File.ReadAllText(filePath);

            // המרת התוכן למערך JSON
            var jsonToken = JToken.Parse(jsonContent);
            if (jsonToken is JArray jsonArray)
            {
                // חיפוש האובייקט שצריך לעדכן
                foreach (var item in jsonArray)
                {
                    if (item[columnValuePair.columnName]?.ToString() == columnValuePair.Value)
                    {
                        // עידכון הערכים באובייקט שנמצא
                        //foreach (var update in entity)
                        //{
                        //    item[update.Key] = update.Value;
                        //}
                    }
                }

                // שמירת השינויים בקובץ
                File.WriteAllText(filePath, jsonArray.ToString(Newtonsoft.Json.Formatting.Indented));
            }
            else
            {
                throw new InvalidDataException("קובץ JSON חייב להיות מערך.");
            }
            return false;
        }
        public override bool RemoveData<T>(string RemovePrimaryKey) where T : class
        {
            string filePath = GetFilePath<T>();

            columnValuePair.columnName = RemovePrimaryKey.Split('.')[0];
            columnValuePair.Value = RemovePrimaryKey.Split('.')[1];
            // בדיקה אם הקובץ קיים
            if (!File.Exists(filePath))
            {
                return false;
            }

            // קריאת תוכן הקובץ
            var jsonContent = File.ReadAllText(filePath);

            // המרת התוכן למערך JSON
            var jsonToken = JToken.Parse(jsonContent);
            if (jsonToken is JArray jsonArray)
            {
                // מחיקת אובייקטים במערך בהם הערך מתאים למפתח
                for (int i = jsonArray.Count - 1; i >= 0; i--)
                {
                    var item = jsonArray[i];
                    if (item[columnValuePair.columnName]?.ToString() == columnValuePair.Value)
                    {
                        jsonArray.RemoveAt(i);
                    }
                }
                File.WriteAllText(filePath, jsonArray.ToString(Newtonsoft.Json.Formatting.Indented));

            }

            // שמירת השינויים בקובץ
            return false;
        }

        protected override string GetFilePath<T>() where T : class
        {
            string fileName = typeof(T).Name.Replace("DTO", "");
            string filePath = fileName + ".json";
            return filePath;
        }
        public static PropertyInfo[] SetClassProperties<T>()
        {
            var type = typeof(T);

            if (!_propertyCache.ContainsKey(type))
            {

                _propertyCache[type] = type.GetProperties();
            }

            return _propertyCache[type];
        }

      
    }
}
