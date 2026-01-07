namespace Tosansam.Core.Entities
{
    public class TableDefinition
    {
        public string ModelName { get; set; } = "";
        public string Title { get; set; } = "";
        public string Namespace { get; set; } = "GeneratedApp"; // اضافه شد
        public List<FieldDefinition> Fields { get; set; } = new();
    }

    public class FieldDefinition
    {
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string Type { get; set; } = "string";
        public bool IsRequired { get; set; }
        public int MaxLength { get; set; } // اضافه شد - مقدار پیش‌فرض 0
    }
}