// فایل جدید: src\Tosansam.Core\ExcelImporter.cs
using System;
using System.Collections.Generic;
using OfficeOpenXml;
using Tosansam.Core.Entities;

namespace Tosansam.Core
{
    public class ExcelImporter
    {
        public List<TableDefinition> ImportFromExcel(string filePath)
        {
            var tables = new List<TableDefinition>();
            
            // ساده‌شده از کد قدیمی شما
            using (var package = new ExcelPackage(new System.IO.FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                
                var table = new TableDefinition
                {
                    Name = worksheet.Cells[1, 1].Text,
                    Title = worksheet.Cells[2, 1].Text
                };
                
                // خواندن فیلدها (ساده‌شده)
                for (int row = 3; row <= 10; row++)
                {
                    if (!string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                    {
                        table.Fields.Add(new FieldDefinition
                        {
                            Name = worksheet.Cells[row, 1].Text,
                            Type = worksheet.Cells[row, 2].Text,
                            Title = worksheet.Cells[row, 3].Text
                        });
                    }
                }
                
                tables.Add(table);
            }
            
            return tables;
        }
    }
}