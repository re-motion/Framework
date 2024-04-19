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
using System.Data;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  public class CustomDataTypeStorageProeprtyDefinitionFactoryDecorator : IDataStoragePropertyDefinitionFactory
  {
    private readonly IDataStoragePropertyDefinitionFactory _innerDataStoragePropertyDefinitionFactory;
    private readonly IStorageNameProvider _storageNameProvider;

    public CustomDataTypeStorageProeprtyDefinitionFactoryDecorator (
        IDataStoragePropertyDefinitionFactory innerDataStoragePropertyDefinitionFactory,
        IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull("innerDataStoragePropertyDefinitionFactory", innerDataStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);

      _innerDataStoragePropertyDefinitionFactory = innerDataStoragePropertyDefinitionFactory;
      _storageNameProvider = storageNameProvider;
    }

    public IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (propertyDefinition.PropertyType == typeof(CompoundDataType))
        return CreateStoragePropertyDefinitionForCompoundDataType(_storageNameProvider.GetColumnName(propertyDefinition));
      return _innerDataStoragePropertyDefinitionFactory.CreateStoragePropertyDefinition(propertyDefinition);
    }

    private IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinitionForCompoundDataType (string columnName)
    {
      return new CompoundStoragePropertyDefinition(
          typeof(CompoundDataType),
          new[]
          {
              new CompoundStoragePropertyDefinition.NestedPropertyInfo(
                  new SimpleStoragePropertyDefinition(
                      typeof(string),
                      new ColumnDefinition(
                          columnName + "StringValue",
                          new StorageTypeInformation(
                              typeof(string),
                              "nvarchar (100)",
                              DbType.String,
                              true,
                              100,
                              typeof(string),
                              new DefaultConverter(typeof(string))),
                          false)),
                  obj => obj == null ? null : ((CompoundDataType)obj).StringValue),
              new CompoundStoragePropertyDefinition.NestedPropertyInfo(
                  new SimpleStoragePropertyDefinition(
                      typeof(int),
                      new ColumnDefinition(
                          columnName + "Int32Value",
                          new StorageTypeInformation(
                              typeof(int?),
                              "int",
                              DbType.Int32,
                              true,
                              null,
                              typeof(int?),
                              new DefaultConverter(typeof(int?))),
                          false)),
                  obj => obj == null ? (int?)null : ((CompoundDataType)obj).Int32Value)
          },
          values => values[0] == null && values[1] == null ? null : new CompoundDataType((string)values[0], (int)values[1]));
    }
  }
}
