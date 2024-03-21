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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building
{
  /// <summary>
  /// Creates <see cref="IRdbmsStoragePropertyDefinition"/> instances for value properties (as opposed to relation properties).
  /// </summary>
  public class ValueStoragePropertyDefinitionFactory : IValueStoragePropertyDefinitionFactory
  {
    private readonly IStorageTypeInformationProvider _storageTypeInformationProvider;
    private readonly IStorageNameProvider _storageNameProvider;

    public ValueStoragePropertyDefinitionFactory (
        IStorageTypeInformationProvider storageTypeInformationProvider, IStorageNameProvider storageNameProvider)
    {
      ArgumentUtility.CheckNotNull("storageTypeInformationProvider", storageTypeInformationProvider);
      ArgumentUtility.CheckNotNull("storageNameProvider", storageNameProvider);

      _storageTypeInformationProvider = storageTypeInformationProvider;
      _storageNameProvider = storageNameProvider;
    }

    public IStorageTypeInformationProvider StorageTypeInformationProvider
    {
      get { return _storageTypeInformationProvider; }
    }

    public IStorageNameProvider StorageNameProvider
    {
      get { return _storageNameProvider; }
    }

    public IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      IStorageTypeInformation storageType;
      try
      {
        storageType = _storageTypeInformationProvider.GetStorageType(propertyDefinition, MustBeNullable(propertyDefinition));
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format(
            "There was an error when retrieving storage type for property '{0}' (declaring type: '{1}'): {2}",
            propertyDefinition.PropertyName,
            propertyDefinition.TypeDefinition.Type.GetFullNameSafe(),
            ex.Message);
        return new UnsupportedStoragePropertyDefinition(propertyDefinition.PropertyType, message, ex);
      }
      var columnName = _storageNameProvider.GetColumnName(propertyDefinition);
      return CreateStoragePropertyDefinition(columnName, storageType, propertyDefinition.PropertyType);
    }

    public IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (object? value, string columnName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("columnName", columnName);

      var propertyType = value != null ? value.GetType() : typeof(object);
      IStorageTypeInformation storageType;
      try
      {
        storageType = _storageTypeInformationProvider.GetStorageType(value);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format(
            "There was an error when retrieving storage type for value of type '{0}': {1}",
            value != null ? value.GetType().Name : "<null>",
            ex.Message);
        return new UnsupportedStoragePropertyDefinition(propertyType, message, ex);
      }

      return CreateStoragePropertyDefinition(columnName, storageType, propertyType);
    }

    protected virtual bool MustBeNullable (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      if (propertyDefinition.TypeDefinition is not ClassDefinition classDefinition) // TODO R2I Persistence: Support TypeDefinition
        throw new NotSupportedException("Only class definitions are supported");

      // CreateSequence can deal with null source objects
      var baseClasses = classDefinition.BaseClass.CreateSequence(cd => cd.BaseClass);
      return baseClasses.Any(cd => _storageNameProvider.GetTableName(cd) != null);
    }

    private IRdbmsStoragePropertyDefinition CreateStoragePropertyDefinition (string columnName, IStorageTypeInformation storageType, Type propertyType)
    {
      var columnDefinition = new ColumnDefinition(columnName, storageType, false);
      return new SimpleStoragePropertyDefinition(propertyType, columnDefinition);
    }
  }
}
