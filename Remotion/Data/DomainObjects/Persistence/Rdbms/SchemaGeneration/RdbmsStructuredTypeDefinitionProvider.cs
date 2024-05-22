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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// Provides an <see cref="IRdbmsStructuredTypeDefinition"/> for each simple type supported by <see cref="SqlStorageTypeInformationProvider"/>.
  /// </summary>
  public class RdbmsStructuredTypeDefinitionProvider : IRdbmsStructuredTypeDefinitionProvider
  {
    public IReadOnlyCollection<IRdbmsStructuredTypeDefinition> GetTypeDefinitions (RdbmsProviderDefinition rdbmsProviderDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(rdbmsProviderDefinition), rdbmsProviderDefinition);

      var storageTypeInformationProvider = rdbmsProviderDefinition.Factory.CreateStorageTypeInformationProvider(rdbmsProviderDefinition);

      var ansiStringStorageTypeInformation = new StorageTypeInformation(
          typeof(string),
          "varchar (max)",
          DbType.AnsiString,
          true,
          -1,
          typeof(string),
          new DefaultConverter(typeof(string)));

      return new[]
             {
                 CreateTypeDefinition(typeof(String), false, storageTypeInformationProvider),
                 CreateTypeDefinition(ansiStringStorageTypeInformation, false),
                 CreateTypeDefinition(typeof(Byte[]), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Boolean?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Boolean?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Byte?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Byte?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(DateTime?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(DateTime?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Decimal?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Decimal?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Double?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Double?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Guid?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Guid?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Int16?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Int16?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Int32?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Int32?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Int64?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Int64?), true, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Single?), false, storageTypeInformationProvider),
                 CreateTypeDefinition(typeof(Single?), true, storageTypeInformationProvider)
             };
    }

    private IRdbmsStructuredTypeDefinition CreateTypeDefinition (Type dotnetType, bool withUniqueConstraint, IStorageTypeInformationProvider storageTypeInformationProvider)
    {
      var storageTypeInformation = storageTypeInformationProvider.GetStorageType(dotnetType);
      return CreateTypeDefinition(storageTypeInformation, withUniqueConstraint);
    }

    private IRdbmsStructuredTypeDefinition CreateTypeDefinition (IStorageTypeInformation storageTypeInformation, bool withUniqueConstraint)
    {
      var dbType = storageTypeInformation.StorageDbType;
      var typeNameDefinition = new EntityNameDefinition(null, $"TVP_{dbType}{(withUniqueConstraint ? "_Distinct" : null)}");
      var columnDefinition = new ColumnDefinition("Value", storageTypeInformation, false);
      var propertyDefinitions = new[] { new SimpleStoragePropertyDefinition(storageTypeInformation.DotNetType, columnDefinition) };

      var tableConstraintDefinitions = Array.Empty<ITableConstraintDefinition>();
      if (withUniqueConstraint)
      {
        tableConstraintDefinitions =
        [
            new UniqueConstraintDefinition($"UQ_{typeNameDefinition.EntityName}_{columnDefinition.Name}", true, new[] { columnDefinition })
        ];
      }

      return new TableTypeDefinition(
          typeNameDefinition,
          propertyDefinitions,
          tableConstraintDefinitions);
    }
  }
}
