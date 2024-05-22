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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Provides an <see cref="IRdbmsStructuredTypeDefinition"/> for each simple type supported by <see cref="SqlStorageTypeInformationProvider"/>.
  /// </summary>
  public class RdbmsStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
  {
    public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      ArgumentUtility.CheckNotNull(nameof(storageTypeInformationProvider), storageTypeInformationProvider);

      return new[]
             {
               CreateTypeDefinition(typeof(String), false),
               CreateTypeDefinition(typeof(Byte[]), false),
               CreateTypeDefinition(typeof(Boolean?), false),
               CreateTypeDefinition(typeof(Boolean?), true),
               CreateTypeDefinition(typeof(Byte?), false),
               CreateTypeDefinition(typeof(Byte?), true),
               CreateTypeDefinition(typeof(DateTime?), false),
               CreateTypeDefinition(typeof(DateTime?), true),
               CreateTypeDefinition(typeof(Decimal?), false),
               CreateTypeDefinition(typeof(Decimal?), true),
               CreateTypeDefinition(typeof(Double?), false),
               CreateTypeDefinition(typeof(Double?), true),
               CreateTypeDefinition(typeof(Guid?), false),
               CreateTypeDefinition(typeof(Guid?), true),
               CreateTypeDefinition(typeof(Int16?), false),
               CreateTypeDefinition(typeof(Int16?), true),
               CreateTypeDefinition(typeof(Int32?), false),
               CreateTypeDefinition(typeof(Int32?), true),
               CreateTypeDefinition(typeof(Int64?), false),
               CreateTypeDefinition(typeof(Int64?), true),
               CreateTypeDefinition(typeof(Single?), false),
               CreateTypeDefinition(typeof(Single?), true)
             };

      IRdbmsStructuredTypeDefinition CreateTypeDefinition (Type dotnetType, bool withUniqueConstraint)
      {
        var storageTypeInformation = storageTypeInformationProvider.GetStorageType(dotnetType);
        var dbType = storageTypeInformation.StorageDbType;
        var typeNameDefinition = new EntityNameDefinition(null, $"TVP_{dbType}{(withUniqueConstraint ? "_distinct" : null)}");
        var columnDefinition = new ColumnDefinition("Value", storageTypeInformation, false);
        var propertyDefinitions = new[] { new SimpleStoragePropertyDefinition(dotnetType, columnDefinition) };

        var tableConstraintDefinitions = Array.Empty<ITableConstraintDefinition>();
        if (withUniqueConstraint)
        {
          tableConstraintDefinitions =
          [
              new UniqueConstraintDefinition($"UQ_{typeNameDefinition.EntityName}_{columnDefinition.Name}", true, new[] { columnDefinition })
          ];
        }

        return new SqlTableTypeDefinition(
            typeNameDefinition,
            propertyDefinitions,
            tableConstraintDefinitions);
      }
    }
  }
}
