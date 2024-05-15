using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public interface IRdbmsStructuredTypeDefinitionProvider
  {
    IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (IEnumerable<TupleDefinition> tupleDefinitions);
  }
}
