using System;
using System.IO;
using System.Text;
using Tosansam.Core.Entities;

namespace Tosansam.Cli.Services
{
    public class FileService
    {
        private readonly string _outputDirectory;
        
        public FileService(string outputDirectory = "GeneratedModels")
        {
            _outputDirectory = outputDirectory;
        }
        
        public string SaveModel(TableDefinition table, string generatedCode)
        {
            try
            {
                // ایجاد پوشه اگر وجود ندارد
                if (!Directory.Exists(_outputDirectory))
                {
                    Directory.CreateDirectory(_outputDirectory);
                    Console.WriteLine($"📁 پوشه خروجی ایجاد شد: {Path.GetFullPath(_outputDirectory)}");
                }
                
                // نام فایل
                string fileName = $"{table.Name}.cs";
                string filePath = Path.Combine(_outputDirectory, fileName);
                
                // ذخیره فایل با encoding UTF-8
                File.WriteAllText(filePath, generatedCode, Encoding.UTF8);
                
                // نمایش پیام موفقیت
                Console.WriteLine($"✅ مدل ذخیره شد: {fileName}");
                Console.WriteLine($"📍 مسیر: {filePath}");
                Console.WriteLine($"🔗 مسیر کامل: {Path.GetFullPath(filePath)}");
                
                // لاگ
                LogToFile(table.Name, filePath);
                
                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطا در ذخیره فایل: {ex.Message}");
                return null;
            }
        }
        
        private void LogToFile(string modelName, string filePath)
        {
            string logMessage = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] مدل '{modelName}' ذخیره شد در: {filePath}\n";
            File.AppendAllText("GenerationLog.txt", logMessage);
        }
    }
}
