using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccess.MainDataAccess;
using DTO;
using OfficeOpenXml;
using static OfficeOpenXml.ExcelErrorValue;

namespace DataAccess
{
   
       public class ExcelDataAccess : DBConnection
        {
            static int s_ExcelRowNumber = 0;
            static int s_ExcelColumnNumber = 0;
            public static ColumnValuePair columnValuePair = new ColumnValuePair();

            private static readonly Dictionary<string, int> _propertyIndex = new Dictionary<string, int>();
            private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();

            public override T AddData<T>(List<T> entityList) where T : class
            {
                // Get Path
                string filePath = GetFilePath<T>();

                // Check if the file exists, if not create new file
                var fileExists = File.Exists(filePath);
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
                            headerCell.Value = property.Name; // put name propertie as heading
                            col++;
                            headerCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCyan);
                            headerCell.Style.Font.Bold = true;
                        }
                    }



                    // Add data
                    foreach (var entity in entityList)
                    {
                        var properties = typeof(T).GetProperties();
                        int col = 1;

                        foreach (var property in properties)
                        {
                            var value = property.GetValue(entity);
                            worksheet.Cells[s_ExcelRowNumber + 1, col].Value = value?.ToString() ?? string.Empty;

                            col++;
                        }

                        s_ExcelRowNumber++;
                    }

                    package.Save();
                }
                return null;
            }
            public override bool UpdateData<T>(T DataUpdate, String updatePrimaryKey) where T : class
            {
                string filePath = GetFilePath<T>();
                string fieldValue = "";

                columnValuePair.columnName = updatePrimaryKey.Split('.')[0];
                columnValuePair.Value = updatePrimaryKey.Split('.')[1];


                if (File.Exists(filePath))
                {

                    //מקבל את ערך לעדכון
                    // properties = GetspecificProperty<T>(columnValuePair.columnName).GetValue(entity).ToString();

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {

                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        int lineToUpdate = FindLine(worksheet, columnValuePair.Value, columnValuePair.columnName);

                        UpdateExcelData<T>(worksheet);

                        foreach (var property in SetClassProperties<T>())
                        {
                            fieldValue = property.GetValue(DataUpdate)?.ToString();
                            if (_propertyIndex.TryGetValue(property.Name, out int columnIndex) && fieldValue != null)
                            {
                                // עדכון התא בעמודה המתאימה
                                worksheet.Cells[lineToUpdate, columnIndex].Value = fieldValue;
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

                columnValuePair.columnName = RemovePrimaryKey.Split('.')[0];
                columnValuePair.Value = RemovePrimaryKey.Split('.')[1];

                if (File.Exists(filePath))
                {

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        UpdateExcelData<T>(worksheet);

                        //Get line to delete
                        int lineToDelete = FindLine(worksheet, columnValuePair.Value, columnValuePair.columnName);

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
                string filePath = fileName + ".xlsx";
                return filePath;
            }

            /// <summary>
            /// Find specipic line according to name column and field value
            /// </summary>
            /// <param name="worksheet"></param>
            /// <param name="entity"></param>
            /// <param name="columnName"></param>
            /// <returns>return line index if find the spesipice line, otherwise return 0 </returns>
            private int FindLine(ExcelWorksheet worksheet, string fieldValue, string columnName)
            {
                // חיפוש עמודת המפתח
                int keyColumnIndex = 1;
                for (; keyColumnIndex <= s_ExcelColumnNumber; keyColumnIndex++)
                {
                    if (worksheet.Cells[1, keyColumnIndex].Text == columnName)
                    {
                        break;
                    }
                }
                if (keyColumnIndex > s_ExcelColumnNumber)
                {
                    return 0;
                }
                for (int row = 2; row <= s_ExcelRowNumber; row++)
                {
                    if (worksheet.Cells[row, keyColumnIndex].Text == fieldValue)
                    {

                        return row;
                    }
                }
                return 0;
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

        }

   
}
