using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Tosansam.Core.Entities;

namespace Tosansam.Core
{
    public class ExcelImporter
    {
        public List<TableDefinition> ImportFromExcel(string filePath)
        {
            var tables = new List<TableDefinition>();
            
            try
            {
                Console.WriteLine($"📖 در حال خواندن فایل Excel: {Path.GetFileName(filePath)}");
                
                // تنظیم Encoding برای پشتیبانی از فارسی
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // خواندن کل فایل
                    var dataSet = reader.AsDataSet();
                    
                    int sheetIndex = 0;
                    foreach (DataTable dataTable in dataSet.Tables)
                    {
                        var tableDef = ConvertToTableDefinition(dataTable, sheetIndex);
                        tables.Add(tableDef);
                        sheetIndex++;
                    }
                }
                
                Console.WriteLine($"✅ فایل Excel خوانده شد. {tables.Count} جدول شناسایی شد.");
                return tables;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در خواندن فایل: {ex.Message}");
                
                // اگر Excel نیست، شاید CSV است
                if (ex.Message.Contains("signature") || ex.Message.Contains("Invalid file"))
                {
                    Console.WriteLine("🔍 ممکن است فایل CSV باشد. در حال تلاش برای خواندن به عنوان CSV...");
                    return ImportFromCsv(filePath);
                }
                
                return tables;
            }
        }
        
        private List<TableDefinition> ImportFromCsv(string filePath)
        {
            var tables = new List<TableDefinition>();
            
            try
            {
                var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
                
                if (lines.Length == 0)
                    return tables;
                
                // خواندن هدرها
                var headers = lines[0].Split(',')
                    .Select(h => CleanName(h.Trim()))
                    .ToArray();
                
                var tableDef = new TableDefinition
                {
                    Name = CleanName(Path.GetFileNameWithoutExtension(filePath)),
                    Fields = new List<FieldDefinition>()
                };
                
                // اضافه کردن فیلدها
                foreach (var header in headers)
                {
                    tableDef.Fields.Add(new FieldDefinition
                    {
                        Name = header,
                        Type = "string",
                        IsNullable = true
                    });
                }
                
                // تشخیص نوع داده از ردیف‌های داده
                if (lines.Length > 1)
                {
                    DetectDataTypesFromCsv(tableDef, lines.Skip(1).Take(10).ToArray());
                }
                
                tables.Add(tableDef);
                Console.WriteLine($"✅ فایل CSV خوانده شد. ۱ جدول با {tableDef.Fields.Count} فیلد شناسایی شد.");
                
                return tables;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در خواندن CSV: {ex.Message}");
                return tables;
            }
        }
        
        private void DetectDataTypesFromCsv(TableDefinition table, string[] sampleRows)
        {
            if (sampleRows.Length == 0) return;
            
            for (int i = 0; i < table.Fields.Count; i++)
            {
                var samples = sampleRows
                    .Select(row => 
                    {
                        var columns = row.Split(',');
                        return i < columns.Length ? columns[i].Trim() : "";
                    })
                    .Where(val => !string.IsNullOrEmpty(val))
                    .ToList();
                
                if (samples.Count == 0) continue;
                
                // تشخیص نوع داده
                if (samples.All(s => int.TryParse(s, out _)))
                    table.Fields[i].Type = "int";
                else if (samples.All(s => decimal.TryParse(s, out _)))
                    table.Fields[i].Type = "decimal";
                else if (samples.All(s => DateTime.TryParse(s, out _)))
                    table.Fields[i].Type = "DateTime";
                else if (samples.All(s => bool.TryParse(s, out _)))
                    table.Fields[i].Type = "bool";
                // در غیر این صورت string می‌ماند
            }
        }
        
        private TableDefinition ConvertToTableDefinition(DataTable dataTable, int sheetIndex)
        {
            var tableDef = new TableDefinition
            {
                Name = $"Table_{sheetIndex + 1}",
                Fields = new List<FieldDefinition>()
            };
            
            // نام شیت
            if (!string.IsNullOrEmpty(dataTable.TableName) && dataTable.TableName != "Table")
            {
                tableDef.Name = CleanName(dataTable.TableName);
            }
            
            Console.WriteLine($"📊 پردازش جدول: {tableDef.Name}");
            
            // اگر داده داریم
            if (dataTable.Rows.Count > 0 && dataTable.Columns.Count > 0)
            {
                // فرض می‌کنیم سطر اول header است
                var firstRow = dataTable.Rows[0];
                
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    string Name = $"Column{i + 1}";
                    
                    // اگر مقدار اول valid است، به عنوان نام فیلد استفاده کن
                    if (firstRow[i] != DBNull.Value && !string.IsNullOrEmpty(firstRow[i].ToString()))
                    {
                        Name = CleanName(firstRow[i].ToString());
                    }
                    
                    tableDef.Fields.Add(new FieldDefinition
                    {
                        Name = Name,
                        Type = GuessDataTypeFromColumn(dataTable, i),
                        IsNullable = true
                    });
                    
                    Console.WriteLine($"   • {Name}");
                }
            }
            else if (dataTable.Columns.Count > 0)
            {
                // فقط ستون‌ها بدون داده
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    tableDef.Fields.Add(new FieldDefinition
                    {
                        Name = $"Column{i + 1}",
                        Type = "string",
                        IsNullable = true
                    });
                }
            }
            
            return tableDef;
        }
        
        private string GuessDataTypeFromColumn(DataTable dataTable, int columnIndex)
        {
            if (dataTable.Rows.Count < 2) return "string";
            
            int samples = Math.Min(10, dataTable.Rows.Count);
            bool allInts = true;
            bool allDecimals = true;
            bool allDates = true;
            
            for (int i = 1; i < samples; i++) // از ردیف دوم شروع کن (ردیف اول header)
            {
                var value = dataTable.Rows[i][columnIndex];
                if (value == DBNull.Value) continue;
                
                var strValue = value.ToString();
                
                if (!int.TryParse(strValue, out _)) allInts = false;
                if (!decimal.TryParse(strValue, out _)) allDecimals = false;
                if (!DateTime.TryParse(strValue, out _)) allDates = false;
            }
            
            if (allInts) return "int";
            if (allDecimals) return "decimal";
            if (allDates) return "DateTime";
            
            return "string";
        }
        
        private string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unnamed";
            
            // حذف فاصله‌های اضافی
            name = name.Trim();
            
            // جایگزینی کاراکترهای مشکل‌ساز
            name = name.Replace(" ", "_")
                      .Replace("-", "_")
                      .Replace(".", "_")
                      .Replace("(", "_")
                      .Replace(")", "_");
            
            // حذف فقط کاراکترهای واقعاً غیرمجاز
            // حروف فارسی: \\p{IsArabic} + حروف انگلیسی: a-zA-Z
            // استفاده از regex ساده‌تر
            var cleaned = System.Text.RegularExpressions.Regex.Replace(name, @"[^\\w\\d_]+", "_");
            
            // اگر با عدد شروع می‌شود
            if (cleaned.Length > 0 && char.IsDigit(cleaned[0]))
                cleaned = "_" + cleaned;
            
            // اگر خالی شد
            if (string.IsNullOrEmpty(cleaned))
                cleaned = "Unnamed";
            
            return cleaned;
        }
    }
}
