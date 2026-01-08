using System.ComponentModel;
using Tosansam.Cli.Services;
using Tosansam.Core;
using Tosansam.Core.Entities;
using Tosansam.Generators;

namespace Tosansam.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 توسن‌سام نسخه ۲ - Generator پیشرفته");
            Console.WriteLine(new string('═', 60));

if (args.Length > 0 && File.Exists(args[0]))
{
    // حالت ۱: خواندن از Excel/CSV
    ProcessFile(args[0]);
}
else
{
    // حالت ۲: نمونه تست
    RunSampleTest();
}
        }
        
        static void ProcessFile(string filePath)
{
    Console.WriteLine($"📂 پردازش فایل: {Path.GetFileName(filePath)}");

    Console.WriteLine($"📌 مسیر: {Path.GetFullPath(filePath)}");


            try
    {
        // ۱. وارد کردن از Excel/CSV
        var importer = new ExcelImporter();
        var tables = importer.ImportFromExcel(filePath);

        if (tables.Count == 0)
        {
            Console.WriteLine("⚠️ هیچ جدولی در فایل یافت نشد.");
                    return;
        }

        // ۲. تولید کد برای هر جدول
        var generator = new ModelGenerator();
        var fileService = new FileService();

        int generatedCount = 0;
        foreach (var table in tables)
        {
            Console.WriteLine($"\\n🎯 تولید مدل: {table.Name}");

                    // تولید کد
            string generatedCode = generator.Generate(table);

            // ذخیره فایل
            string savedPath = fileService.SaveModel(table, generatedCode);

            if (!string.IsNullOrEmpty(savedPath))
                generatedCount++;
        }

        // ۳. نمایش نتیجه
        Console.WriteLine("\\n" + new string('═', 60));

        Console.WriteLine($"🎉 عملیات کامل شد! {generatedCount} مدل تولید شد.");

        Console.WriteLine($"📁 پوشه خروجی: {Path.GetFullPath("GeneratedModels")}");
            }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ خطا: {ex.Message}");

        Console.WriteLine($"🔍 جزئیات: {ex.ToString()}");
            }
}

static void RunSampleTest()
{
    Console.WriteLine("\\n🔧 اجرای نمونه تست (بدون فایل ورودی)");

            // ایجاد نمونه جدول تست
    var table = CreateSampleTable();

    // تولید کد
    var generator = new ModelGenerator();
    string generatedCode = generator.Generate(table);

    Console.WriteLine("\\n✅ کد تولید شده:\\n");

    Console.WriteLine(new string('─', 40));
    Console.WriteLine(generatedCode);
    Console.WriteLine(new string('─', 40));

    // ذخیره فایل
    var fileService = new FileService();
    string savedPath = fileService.SaveModel(table, generatedCode);

    Console.WriteLine("\\n" + new string('═', 60));

    Console.WriteLine("🎉 تست کامل شد!");

    Console.WriteLine($"📅 {DateTime.Now:yyyy/MM/dd HH:mm}");


            if (!string.IsNullOrEmpty(savedPath))
    {
        Console.WriteLine($"📄 فایل ذخیره شده: {savedPath}");
            }

    Console.WriteLine("\\n💡 نکته: برای استفاده از فایل Excel/CSV، آن را به عنوان آرگومان وارد کنید.");

    Console.WriteLine("   مثال: dotnet run --project src\\\\Tosansam.Cli فایل.xlsx");

    Console.WriteLine("   یا:   dotnet run --project src\\\\Tosansam.Cli فایل.csv");
        }

static TableDefinition CreateSampleTable()
{
    return new TableDefinition
    {
        Name = "Customer",
                Fields = new List<FieldDefinition>
                {
                    new FieldDefinition { Name = "Id", Type = "int" },
                    new FieldDefinition { Name = "Name", Type = "string" },
                    new FieldDefinition { Name = "Email", Type = "string" },
                    new FieldDefinition { Name = "Age", Type = "int" }
                }
            };
        }
    }
}
