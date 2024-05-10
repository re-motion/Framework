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
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  public class RdbmsStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
  {
    public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull(nameof(storageTypeInformationProvider), storageTypeInformationProvider);

      return new[]
             {
               CreateTypeDefinition(DbType.String, typeof(String)),
               CreateTypeDefinition(DbType.Binary, typeof(Byte[])),
               CreateTypeDefinition(DbType.Boolean, typeof(Boolean?)),
               CreateTypeDefinition(DbType.Byte, typeof(Byte?)),
               CreateTypeDefinition(DbType.DateTime, typeof(DateTime?)),
               CreateTypeDefinition(DbType.Decimal, typeof(Decimal?)),
               CreateTypeDefinition(DbType.Double, typeof(Double?)),
               CreateTypeDefinition(DbType.Guid, typeof(Guid?)),
               CreateTypeDefinition(DbType.Int16, typeof(Int16?)),
               CreateTypeDefinition(DbType.Int32, typeof(Int32?)),
               CreateTypeDefinition(DbType.Int64, typeof(Int64?)),
               CreateTypeDefinition(DbType.Single, typeof(Single?))
             };

      IRdbmsStructuredTypeDefinition CreateTypeDefinition (DbType dbType, Type dotnetType)
      {
        var storageTypeInformation = storageTypeInformationProvider.GetStorageType(dotnetType);
        var typeNameDefinition = new TypeNameDefinition(null, $"TVP_{dbType}");
        var propertyDefinitions = new[] { new SimpleStoragePropertyDefinition(dotnetType, new ColumnDefinition("Value", storageTypeInformation, false)) };
        return new TableTypeDefinition(
            typeNameDefinition,
            propertyDefinitions,
            Array.Empty<ITableConstraintDefinition>(),
            Array.Empty<IIndexDefinition>());
      }
    }
  }
}
