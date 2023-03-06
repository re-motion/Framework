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
using System.Diagnostics;
using System.Linq;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [DebuggerDisplay("{GetType().Name}: {PropertyName}")]
  public class PropertyDefinition
  {
    private readonly string _propertyName;
    private readonly ClassDefinition _classDefinition;
    private readonly int? _maxLength;
    private readonly StorageClass _storageClass;
    private IStoragePropertyDefinition? _storagePropertyDefinition;
    private readonly IPropertyInformation _propertyInfo;
    private readonly Type _propertyType;
    private readonly bool _isNullable;
    private bool _isNullablePropertyType;
    private readonly bool _isObjectID;
    private readonly IPropertyDefaultValueProvider _defaultValueProvider;

    public PropertyDefinition (
        ClassDefinition classDefinition,
        IPropertyInformation propertyInfo,
        string propertyName,
        bool isObjectID,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      _classDefinition = classDefinition;
      _propertyInfo = propertyInfo;
      _propertyType = isObjectID ? typeof(ObjectID) : propertyInfo.PropertyType;
      _propertyName = propertyName;
      _isObjectID = isObjectID;
      _isNullablePropertyType = NullableTypeUtility.IsNullableType(propertyInfo.PropertyType);
      _isNullable = isNullable;
      _maxLength = maxLength;
      _storageClass = storageClass;
      _defaultValueProvider = new LegacyPropertyDefaultValueProvider();
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public bool HasStoragePropertyDefinitionBeenSet
    {
      get { return _storagePropertyDefinition != null; }
    }

    public IStoragePropertyDefinition StoragePropertyDefinition
    {
      get
      {
        if (StorageClass != StorageClass.Persistent)
          throw new InvalidOperationException("Cannot access property 'storagePropertyDefinition' for non-persistent property definitions.");

        Assertion.IsNotNull(_storagePropertyDefinition, "StoragePropertyDefinition has not been set for property '{0}' of class '{1}'.", PropertyName, _classDefinition.ID);
        return _storagePropertyDefinition;
      }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public Type PropertyType
    {
      get { return _propertyType; }
    }

    public object? DefaultValue
    {
      get
      {
        return _defaultValueProvider.GetDefaultValue(_propertyInfo, _isNullable);
      }
    }

    /// <summary>
    /// Caches the information on whether the property's .NET type is nullable.
    /// </summary>
    internal bool IsNullablePropertyType
    {
      get { return _isNullablePropertyType; }
    }

    /// <summary>
    /// Gets a flag describing whether the property's value is required for persistence.
    /// </summary>
    public bool IsNullable
    {
      get { return _isNullable; }
    }

    public bool IsObjectID
    {
      get { return _isObjectID; }
    }

    /// <summary>
    /// Gets the maximum length of the property's value when the value is persisted.
    /// </summary>
    public int? MaxLength
    {
      get { return _maxLength; }
    }

    public StorageClass StorageClass
    {
      get { return _storageClass; }
    }

    public void SetStorageProperty (IStoragePropertyDefinition storagePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull("storagePropertyDefinition", storagePropertyDefinition);

      _storagePropertyDefinition = storagePropertyDefinition;
    }

    public override string ToString ()
    {
      return GetType().GetFullNameSafe() + ": " + _propertyName;
    }
  }
}
