using System;
using Tosansam.Cli.Services;
using Tosansam.Core.Entities;
using Tosansam.Generators;

namespace Tosansam.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 توسن‌سام - Generator با ذخیره فایل");
            Console.WriteLine(new string('═', 50));

            // 1. ایجاد یک نمونه جدول تست
            var table = CreateSampleTable();

            // 2. تولید کد
            var generator = new ModelGenerator();
            string generatedCode = generator.Generate(table);

            Console.WriteLine("\n✅ کد تولید شده:\n");
            Console.WriteLine(new string('─', 40));
            Console.WriteLine(generatedCode);
            Console.WriteLine(new string('─', 40));

            // 3. ذخیره فایل
            var fileService = new FileService();
            string savedPath = fileService.SaveModel(table, generatedCode);

            // 4. نمایش نتیجه نهایی
            Console.WriteLine("\n" + new string('═', 50));
            Console.WriteLine("🎉 عملیات کامل شد!");
            Console.WriteLine($"📅 {DateTime.Now:yyyy/MM/dd HH:mm}");

            if (!string.IsNullOrEmpty(savedPath))
            {
                Console.WriteLine($"📁 پوشه خروجی: {Path.GetFullPath("GeneratedModels")}");
                Console.WriteLine($"📄 فایل ذخیره شده: {savedPath}");
            }

            Console.WriteLine("\n🔧 برای خروج یک کلید بزنید...");
            Console.ReadKey();
        }

        static TableDefinition CreateSampleTable()
        {
            return new TableDefinition
            {
                Name = "Customer",
                Fields = new List<FieldDefinition>
                {
                    new FieldDefinition { Name = "Id", Type = "int", IsNullable = false },
                    new FieldDefinition { Name = "Name", Type = "string", IsNullable = false },
                    new FieldDefinition { Name = "Email", Type = "string", IsNullable = false },
                    new FieldDefinition { Name = "Age", Type = "int", IsNullable = true }
                }
            };
        }
    }
}