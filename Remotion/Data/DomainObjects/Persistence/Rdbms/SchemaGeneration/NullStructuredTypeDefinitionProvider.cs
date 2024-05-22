using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;

/// <summary>
/// An implementation of <see cref="IRdbmsStructuredTypeDefinitionProvider"/> that never returns any <see cref="IRdbmsStructuredTypeDefinition"/>s.
/// </summary>
public class NullStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
{
  public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (IStorageTypeInformationProvider storageTypeInformationProvider)
  {
    return Array.Empty<IRdbmsStructuredTypeDefinition>();
  }
}
