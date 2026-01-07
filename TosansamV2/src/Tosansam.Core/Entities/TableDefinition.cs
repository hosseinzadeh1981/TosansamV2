namespace Tosansam.Core.Entities
{
    public class TableDefinition
    {
        public string Name { get; set; }                     // ✅ نام جدول/مدل
        public string Title { get; set; }          // ✅ عنوان
        public string Namespace { get; set; }                // ✅ Namespace
        public List<FieldDefinition> Fields { get; set; }    // ✅ لیست فیلدها

        // Constructor
        public TableDefinition()
        {
            Fields = new List<FieldDefinition>();
            Namespace = "GeneratedApp.Models";  // مقدار پیش‌فرض
        }
    }


    public class FieldDefinition
    {
        public string Name { get; set; }          // ✅ نام فیلد
        public string Title { get; set; }          // ✅ عنوان
        public string Type { get; set; }          // ✅ نوع داده (int, string, etc.)
        public bool IsNullable { get; set; }      // ✅ قابل null بودن
        public bool IsPrimaryKey { get; set; }    // ✅ کلید اصلی
        public int MaxLength { get; set; }        // ✅ حداکثر طول (برای stringها)

        // Constructor
        public FieldDefinition()
        {
            IsNullable = false;
            IsPrimaryKey = false;
            MaxLength = 0;
        }
    }
}