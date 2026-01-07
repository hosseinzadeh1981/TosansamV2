// فایل `src\Tosansam.Generators\ModelGenerator.cs` را **جایگزین** کن با:
using Tosansam.Core;
using Tosansam.Core.Entities;

namespace Tosansam.Generators
{
    public class ModelGenerator
    {
        public string Generate(TableDefinition table)
        {
            // ساده‌سازی - فقط Properties پایه
            var properties = new List<string>();

            foreach (var field in table.Fields)
            {
                var property = $"public {field.Type} {field.Name} {{ get; set; }}";
                properties.Add(property);
            }

            // استفاده از Namespace جدید
            var namespaceName = string.IsNullOrEmpty(table.Namespace)
                ? "GeneratedApp.Models"
                : table.Namespace;

            return $@"using System;

namespace {namespaceName}
{{
    public class {table.Name}
    {{
        {string.Join("\n        ", properties)}
    }}
}}";
        }
    }
}