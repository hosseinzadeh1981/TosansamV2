using Tosansam.Core;
using Tosansam.Core.Entities;
using Tosansam.Generators;

var table = new TableDefinition
{
    ModelName = "Customer",
    Title = "مشتری",
    Namespace = "GeneratedApp.Models",
    Fields = new List<FieldDefinition>
    {
        new() { Name = "Id", Type = "int" },
        new() { Name = "Name", Type = "string", Title = "نام کامل" },
        new() { Name = "Email", Type = "string", Title = "ایمیل", MaxLength = 100 },
        new() { Name = "Age", Type = "int", Title = "سن" }
    }
};

Console.WriteLine("🚀 توسن‌سام - اولین اجرا\n");

var generator = new ModelGenerator();
var code = generator.Generate(table);  // تغییر: Generate() به جای GenerateModel()

Console.WriteLine("✅ کد تولید شده:\n");
Console.WriteLine(code);
Console.WriteLine("\n🎉 Generator کار می‌کند!");
Console.WriteLine("📅 " + DateTime.Now.ToString("yyyy/MM/dd HH:mm"));
Console.WriteLine("\n📊 آمار: " + table.Fields.Count + " فیلد تولید شد");