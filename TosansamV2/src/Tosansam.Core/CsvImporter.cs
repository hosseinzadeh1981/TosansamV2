using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tosansam.Core.Entities;

namespace Tosansam.Core
{
    public class CsvImporter
    {
        public List<TableDefinition> ImportFromCsv(string filePath)
        {
            var tables = new List<TableDefinition>();
            
            try
            {
                Console.WriteLine($"📖 در حال خواندن فایل CSV: {Path.GetFileName(filePath)}");
                
                var lines = File.ReadAllLines(filePath, System.Text.Encoding.UTF8);
                
                if (lines.Length == 0)
                {
                    Console.WriteLine("⚠️ فایل CSV خالی است.");
                    return tables;
                }
                
                // سطر اول = header
                var headers = lines[0].Split(',')
                    .Select(h => CleanName(h.Trim()))
                    .ToArray();
                
                var tableDef = new TableDefinition
                {
                    Name = CleanName(Path.GetFileNameWithoutExtension(filePath)),
                    Fields = new List<FieldDefinition>()
                };
                
                Console.WriteLine($"📊 پردازش فایل: {tableDef.Name}");
                
                // ایجاد فیلدها از headers
                foreach (var header in headers)
                {
                    var fieldDef = new FieldDefinition
                    {
                        Name = header,
                        Type = "string", // پیش‌فرض
                        IsNullable = true
                    };
                    
                    tableDef.Fields.Add(fieldDef);
                    Console.WriteLine($"   • {fieldDef.Name}");
                }
                
                // اگر داده داریم، نوع داده را تشخیص بده
                if (lines.Length > 1)
                {
                    DetectDataTypes(tableDef, lines.Skip(1).ToArray());
                }
                
                tables.Add(tableDef);
                Console.WriteLine($"✅ فایل CSV خوانده شد. ۱ جدول شناسایی شد.");
                
                return tables;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در خواندن CSV: {ex.Message}");
                return tables;
            }
        }
        
        private void DetectDataTypes(TableDefinition table, string[] dataRows)
        {
            // نمونه ساده برای تشخیص نوع
            for (int i = 0; i < table.Fields.Count; i++)
            {
                var samples = dataRows.Take(5)
                    .Select(row => row.Split(',')[i].Trim())
                    .Where(val => !string.IsNullOrEmpty(val))
                    .ToList();
                
                if (samples.Count == 0) continue;
                
                // تشخیص نوع
                if (samples.All(s => int.TryParse(s, out _)))
                {
                    table.Fields[i].Type = "int";
                }
                else if (samples.All(s => decimal.TryParse(s, out _)))
                {
                    table.Fields[i].Type = "decimal";
                }
                else if (samples.All(s => DateTime.TryParse(s, out _)))
                {
                    table.Fields[i].Type = "DateTime";
                }
                else if (samples.All(s => bool.TryParse(s, out _)))
                {
                    table.Fields[i].Type = "bool";
                }
                // در غیر این صورت string می‌ماند
            }
        }
        
        private string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unnamed";
            
            // حذف کاراکترهای غیرمجاز
            var invalidChars = new System.Text.RegularExpressions.Regex(@"[^\\w\\d_]+");
            name = invalidChars.Replace(name, "_");
            
            if (char.IsDigit(name[0]))
                name = "_" + name;
            
            return name;
        }
    }
}
