// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class TableTypeDefinitionObjectMother
  {
    public static TableTypeDefinition Create (
        string typeName = "TestType",
        [CanBeNull] string schemaName = null,
        IReadOnlyCollection<IRdbmsStoragePropertyDefinition> propertyDefinitions = null,
        IReadOnlyCollection<ITableConstraintDefinition> constraints = null)
    {
      propertyDefinitions ??= new[] { SimpleStoragePropertyDefinitionObjectMother.CreateStringStorageProperty("TestColumn") };
      constraints ??= Array.Empty<ITableConstraintDefinition>();

      return new TableTypeDefinition(new EntityNameDefinition(schemaName, typeName), propertyDefinitions, constraints);
    }
  }
}
