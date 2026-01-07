using Tosansam.Core.Entities;

namespace Tosansam.Core.Interfaces
{
    public interface ICodeGenerator
    {
        string GenerateModel(TableDefinition table);
    }
}