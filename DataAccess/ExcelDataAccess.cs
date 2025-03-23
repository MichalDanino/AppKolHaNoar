using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccess.MainDataAccess;
using DTO;

using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using static OfficeOpenXml.ExcelErrorValue;
using static DTO.TranslationTable;
using System.Globalization;

namespace DataAccess
{

    public class ExcelDataAccess : DBConnection
    {
        static int s_ExcelRowNumber = 1;
        static int s_ExcelColumnNumber = 0;

        private readonly string _databaseFilePath;

        private static readonly Dictionary<string, int> _propertyIndex = new Dictionary<string, int>();
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();

        public ExcelDataAccess(string databaseFilePath)
        {
            _databaseFilePath = databaseFilePath;
        }


        public override bool InsetData<T>(List<T> entityList) where T : class
        {

            // Get Path
            string filePath = GetFilePath<T>();
            bool uniqueNamecell = false;
            // Check if the file exists, if not create new file
            bool fileExists = File.Exists(filePath);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                //Create new Workbook
                var worksheet = fileExists
                    ? package.Workbook.Worksheets[0]
                    : package.Workbook.Worksheets.Add("Sheet1");

                // Create column headings
                if (!fileExists || worksheet.Dimension == null)
                {
                    var properties = typeof(T).GetProperties(); // class properties
                    int col = 1;

                    foreach (var property in properties)
                    {
                        var headerCell = worksheet.Cells[1, col];
                        headerCell.Value = PropertyNamesInHebrew[property.Name]; // put name propertie as heading
                        col++;
                        headerCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                        headerCell.Style.Font.Bold = true;
                        headerCell.Style.Locked = true;
                    }
                }
                s_ExcelRowNumber = worksheet.Dimension?.Rows + 1 ?? 2;
                worksheet.Cells.AutoFitColumns();

                //   Add data
                foreach (T entity in entityList)
                {
                    PropertyInfo[] properties = typeof(T).GetProperties();
                    int col = 1;

                    foreach (PropertyInfo property in properties)
                    {
                        var value = property.GetValue(entity);
                        uniqueNamecell = property.Name.Contains("_ID");

                        worksheet.Cells[s_ExcelRowNumber, col].Value = value?.ToString() ?? string.Empty;
                        worksheet.Cells[s_ExcelRowNumber, col].Style.Locked = false;
                        if (uniqueNamecell)
                        {

                            worksheet.Cells[s_ExcelRowNumber, col].Style.Locked = true;
                            worksheet.Cells[s_ExcelRowNumber, col].Value = value?.ToString() ?? string.Empty;


                        }

                        col++;
                    }
                    s_ExcelRowNumber++;

                }
                package.Workbook.Protection.LockStructure = true;
                worksheet.Protection.IsProtected = true;
                worksheet.Protection.AllowSelectUnlockedCells = true;
                worksheet.Protection.AllowInsertRows = true;


                package.Save();
            }
            return true;
        }
        public override bool UpdateData<T>(T DataUpdate) where T : class
        {
            string filePath = GetFilePath<T>();
            string uniqueValue = "";


            if (File.Exists(filePath))
            {


                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    PropertyInfo uniqueProperty = typeof(T).GetProperties()
                                     .FirstOrDefault(p => p.Name.Contains("_ID"));
                    // string uniqueColumn = uniqueProperty.Name;
                    uniqueValue = uniqueProperty.GetValue(DataUpdate)?.ToString();
                    if (uniqueValue != string.Empty)
                        throw new Exception("הערך של עמודת המזהה הייחודי ריק או null");

                    int rowToUpdate = FindLine(worksheet, uniqueValue);
                    if (rowToUpdate == -1)
                        return false; // הרשומה לא נמצאה

                    UpdateExcelData<T>(worksheet);

                    foreach (var property in SetClassProperties<T>())
                    {
                        if (property.Name.Contains("_ID"))
                            continue;

                        if (_propertyIndex.TryGetValue(property.Name, out int columnIndex))
                        {
                            object fieldValue = property.GetValue(DataUpdate);

                            // עדכון התא בעמודה המתאימה
                            worksheet.Cells[rowToUpdate, columnIndex].Value = fieldValue?.ToString() ?? string.Empty; ;
                        }
                    }


                    package.Save();
                }

            }
            // שמירת הקובץ

            return true;

        }
        public override bool RemoveData<T>(string RemovePrimaryKey) where T : class
        {
            string filePath = GetFilePath<T>();

            string uniqueValue = RemovePrimaryKey;

            if (File.Exists(filePath))
            {

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    UpdateExcelData<T>(worksheet);

                    //Get line to delete
                    int lineToDelete = FindLine(worksheet, uniqueValue);

                    //Check line validity
                    if (lineToDelete == 0)
                        return false;
                    else
                        worksheet.DeleteRow(lineToDelete);

                    // Save file
                    package.Save();

                }
            }


            return true;
        }

        /// <summary>
        /// Get path Excel
        /// </summary>
        /// <typeparam name="T">The type of the object on which the action is performed</typeparam>
        /// <returns>path excel</returns>
        protected override string GetFilePath<T>()
        {
            string fileName = typeof(T).Name.Replace("DTO", "");
            string filePath = _databaseFilePath + fileName + ".xlsx";

            return filePath;
        }

        /// <summary>
        /// Find specipic line according to name column and field value
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="entity"></param>
        /// <param name="columnName"></param>
        /// <returns>return line index if find the spesipice line, otherwise return 0 </returns>
        private int FindLine(ExcelWorksheet worksheet, string uniqueColumn)
        {
            // חיפוש עמודת המפתח
            int keyColumnIndex = 1;
            for (; keyColumnIndex <= s_ExcelColumnNumber; keyColumnIndex++)
            {
                if (worksheet.Cells[1, keyColumnIndex].Text.Contains("_ID"))
                {
                    break;
                }
            }
            if (keyColumnIndex > s_ExcelColumnNumber)
            {
                return -1;
            }
            for (int row = 2; row <= s_ExcelRowNumber; row++)
            {
                if (worksheet.Cells[row, keyColumnIndex].Text == uniqueColumn)
                {

                    return row;
                }
            }
            return -1;
        }

        private void UpdateExcelData<T>(ExcelWorksheet worksheet)
        {
            s_ExcelRowNumber = worksheet.Dimension.Rows;
            s_ExcelColumnNumber = worksheet.Dimension.Columns;
            // Column index mapping
            for (int col = 1; col <= s_ExcelColumnNumber; col++)
            {
                var header = worksheet.Cells[1, col].Text;
                if (!string.IsNullOrEmpty(header))
                    _propertyIndex[header] = col;
            }
            SetClassProperties<T>();


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
        public void OpenExcelForUser<T>() where T : class
        {
            string filePath = GetFilePath<T>();
            if (File.Exists(filePath))
            {

                // Start Excel and open the file
                Process.Start($"C:\\Program Files\\Microsoft Office\\root\\Office16\\EXCEL.EXE", $"\"{filePath}\"");
            }
        }


        public List<T> GetDataFromExcel<T>() where T : class, new()
        {
            string filePath = GetFilePath<T>();

            var result = new List<T>();

            if (File.Exists(filePath))
            {

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];  // בחר את הגיליון הראשון
                    var properties = typeof(T).GetProperties();  // קבל את התכונות של המחלקה T

                    // הנחת עבודה היא שהשורה הראשונה מכילה את כותרות העמודות
                    // מתחילים בקריאה מהשורה השנייה כדי לדלג על כותרות העמודות
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var entity = new T();  // יצירת אובייקט חדש מהמחלקה T

                        int col = 1;
                        foreach (var property in properties)
                        {
                            var value = worksheet.Cells[row, col].Text;  // קריאה לערך בתא
                            if (value != null && value !="")
                            {
                                property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));  // המרת הערך והגדרתו בתכונה המתאימה
                            }
                            col++;
                        }

                        result.Add(entity);  // הוספת האובייקט שנוצר לרשימה
                    }
                }

                return result;
            }

            return null;
        }

        public Enums.eStatus DeleteExcelPackage<T>() where T : class
        {
            string filePath = GetFilePath<T>();

            if (File.Exists(filePath))
            {
                ClouseExcelFile(filePath);
                Thread.Sleep(9000);
                File.Delete(filePath);
                return Enums.eStatus.SUCCESS;
            }
            return Enums.eStatus.FAILED;
        }
        public void ClouseExcelFile(string filePath)
        {
           
            var processes = Process.GetProcessesByName("EXCEL");
            foreach (var process in processes)
            {
                process.Kill(); // סוגר את התהליך של Excel
            }
        }

        /// <summary>
        /// For ChannelExtension class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ApplyDataValidation<T>(string Day,String Houres) where T : class
        {
            string filePath = GetFilePath<T>();
            if (!File.Exists(filePath))
                return false;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                if (worksheet.Dimension == null)
                    return false;

                Dictionary<string, int> columnIndexes = new Dictionary<string, int>();

                // איתור עמודות לפי כותרות
                int totalColumns = worksheet.Dimension.Columns;
                for (int col = 1; col <= totalColumns; col++)
                {
                    string header = worksheet.Cells[1, col].Text;
                    columnIndexes[header] = col;
                }

                int lastRow = worksheet.Dimension.Rows;
                int startRow = 2;
                int endRow = Math.Max(lastRow, startRow);
                // 🎯 **הוספת רשימה נפתחת לעמודה "יום בשבוע"**
                if (columnIndexes.TryGetValue(Day, out int dayColumn))
                {
                    var daysOfWeek = new[] {" ","ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת" };
                    
                    var validation = worksheet.DataValidations.AddListValidation(
                        worksheet.Cells[startRow, dayColumn, endRow, dayColumn].Address);
                    foreach (var day in daysOfWeek)
                    {
                        validation.Formula.Values.Add(day);
                    }

                    validation.AllowBlank = false;

                    // בנוסף, אפשר להוסיף הודעת שגיאה במקרה של ערך לא חוקי
                    validation.ErrorTitle = "ערך לא חוקי";
                    validation.Error = "עליך לבחור יום מהרשימה.";
                    validation.ShowErrorMessage = true;
                }

                // 🎯 **הוספת הגבלת שעה לעמודה "שעת התחלה"**
                if (columnIndexes.TryGetValue(Houres, out int timeColumn))
                {
                    for (int row = 2; row <= lastRow; row++) // עובר על כל השורות בעמודה
                    {
                        if (DateTime.TryParse(worksheet.Cells[row, timeColumn].Text, out DateTime parsedTime))
                        {
                            worksheet.Cells[row, timeColumn].Value = parsedTime.TimeOfDay.TotalDays; // שומר את הערך כמספר יומי
                        }
                        else
                        {
                            worksheet.Cells[row, timeColumn].Value = new DateTime(1, 1, 1, 0, 0, 0).TimeOfDay.TotalDays;
                        }
                    }

                    // קובע פורמט תצוגה של שעה בלבד (ללא תאריך)
                    worksheet.Column(timeColumn).Style.Numberformat.Format = "hh:mm";

                    // הוספת הולידציה לחסימת טקסט וערכים לא חוקיים
                    var timeValidation = worksheet.DataValidations.AddCustomValidation(worksheet.Cells[2, timeColumn, lastRow, timeColumn].Address);
                    timeValidation.Formula.ExcelFormula = $"AND(ISNUMBER({worksheet.Cells[2, timeColumn].Address}), {worksheet.Cells[2, timeColumn].Address}=TIME(HOUR({worksheet.Cells[2, timeColumn].Address}), MINUTE({worksheet.Cells[2, timeColumn].Address}), 0))";
                    timeValidation.ShowErrorMessage = true;
                    timeValidation.ErrorTitle = "שגיאה";
                    timeValidation.Error = "נא להזין שעה בפורמט תקין (hh:mm)";

                }

                package.Save();
            }
            return true;
        }

    }
}
