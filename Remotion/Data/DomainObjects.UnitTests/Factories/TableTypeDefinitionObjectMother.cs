using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class TableTypeDefinitionObjectMother
  {
    public static TableTypeDefinition Create (
        string typeName = "TestType",
        [CanBeNull] string schemaName = null,
        IReadOnlyCollection<IRdbmsStoragePropertyDefinition> propertyDefinitions = null,
        IReadOnlyCollection<ITableConstraintDefinition> constraints = null,
        IReadOnlyCollection<IIndexDefinition> indexes = null)
    {
      propertyDefinitions ??= new [] { SimpleStoragePropertyDefinitionObjectMother.CreateStringStorageProperty("TestColumn")};
      constraints ??= Array.Empty<ITableConstraintDefinition>();
      indexes ??= Array.Empty<IIndexDefinition>();

      return new TableTypeDefinition(new TypeNameDefinition(schemaName, typeName), propertyDefinitions, constraints, indexes);
    }
  }
}
