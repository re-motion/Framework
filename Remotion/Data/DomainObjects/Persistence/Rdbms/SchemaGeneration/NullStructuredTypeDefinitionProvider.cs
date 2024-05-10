using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

public class NullStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
{
  public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (IStorageTypeInformationProvider storageTypeInformationProvider)
  {
    return Array.Empty<IRdbmsStructuredTypeDefinition>();
  }
}
