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
using System.Threading;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  public abstract class TypeDefinition
  {
    private readonly Lazy<RelationEndPointDefinitionCollection> _cachedRelationEndPointDefinitions;
    private readonly Lazy<PropertyDefinitionCollection> _cachedPropertyDefinitions;

    private PropertyDefinitionCollection? _propertyDefinitions;
    private RelationEndPointDefinitionCollection? _relationEndPoints;
    private IStorageEntityDefinition? _storageEntityDefinition;

    public Type Type { get; }

    public bool IsReadOnly { get; private set; }

    public PropertyAccessorDataCache PropertyAccessorDataCache { get; }

    public Type? StorageGroupType { get; }

    public DefaultStorageClass DefaultStorageClass { get; }

    protected TypeDefinition (Type type, Type? storageGroupType, DefaultStorageClass defaultStorageClass)
    {
      ArgumentUtility.CheckNotNull(nameof(type), type);

      Type = type;
      StorageGroupType = storageGroupType;
      DefaultStorageClass = defaultStorageClass;

      PropertyAccessorDataCache = new PropertyAccessorDataCache(this);
      _cachedRelationEndPointDefinitions = new Lazy<RelationEndPointDefinitionCollection>(
           CreateRelationEndPointDefinitionCollection,
           LazyThreadSafetyMode.ExecutionAndPublication);
      _cachedPropertyDefinitions =
          new Lazy<PropertyDefinitionCollection>(
              CreatePropertyDefinitionCollection,
              LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public abstract void Accept (ITypeDefinitionVisitor visitor);

    public abstract T? Accept<T> (ITypeDefinitionVisitor<T> visitor);

    public abstract bool IsPartOfInheritanceHierarchy { get; }

    public abstract bool IsAssignableFrom (TypeDefinition other);

    protected abstract void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions);

    protected abstract void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints);

    protected virtual PropertyDefinitionCollection CreatePropertyDefinitionCollection ()
    {
      return PropertyDefinitionCollection.CreateForAllProperties(this, true);
    }

    protected virtual RelationEndPointDefinitionCollection CreateRelationEndPointDefinitionCollection ()
    {
      return RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints(this, true);
    }

    public IStorageEntityDefinition StorageEntityDefinition
    {
      get
      {
        if (_storageEntityDefinition == null)
          throw new InvalidOperationException($"StorageEntityDefinition has not been set for type '{Type.GetFullNameSafe()}'.");

        return _storageEntityDefinition;
      }
    }

    public bool HasStorageEntityDefinitionBeenSet => _storageEntityDefinition != null;

    public PropertyDefinitionCollection MyPropertyDefinitions
    {
      get
      {
        if (_propertyDefinitions == null)
          throw new InvalidOperationException($"No property definitions have been set for type '{Type.GetFullNameSafe()}'.");

        return _propertyDefinitions;
      }
    }

    public RelationEndPointDefinitionCollection MyRelationEndPointDefinitions
    {
      get
      {
        if (_relationEndPoints == null)
          throw new InvalidOperationException($"No relation end point definitions have been set for type '{Type.GetFullNameSafe()}'.");

        return _relationEndPoints;
      }
    }

    public virtual bool IsTypeResolved => true;

    public PropertyDefinition? this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyName), propertyName);
        return MyPropertyDefinitions[propertyName];
      }
    }

    public void SetStorageEntity (IStorageEntityDefinition storageEntityDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(storageEntityDefinition), storageEntityDefinition);

      if (IsReadOnly)
        throw new NotSupportedException($"Type '{Type.GetFullNameSafe()}' is read-only.");

      _storageEntityDefinition = storageEntityDefinition;
    }

    public PropertyDefinitionCollection GetPropertyDefinitions ()
    {
      return IsReadOnly
          ? _cachedPropertyDefinitions.Value
          : CreatePropertyDefinitionCollection();
    }

    public virtual PropertyDefinition? GetPropertyDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyName), propertyName);

      return GetPropertyDefinitions()[propertyName];
    }

    [Obsolete(
        "Contains (...) method was ambiguous between GetPropertyDefinitions() and MyPropertyDefinitions. Use GetPropertyDefinitions().Contains (...) instead. (Version: 3.0.0)",
        true)]
    public bool Contains (PropertyDefinition propertyDefinition)
    {
      throw new NotSupportedException("Use GetPropertyDefinitions().Contains (...) instead. (Version: 3.0.0)");
    }

    public PropertyDefinition GetMandatoryPropertyDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

      var propertyDefinition = GetPropertyDefinition(propertyName);
      if (propertyDefinition == null)
        throw new MappingException($"Type '{Type.GetFullNameSafe()}' does not contain the property '{propertyName}'.");

      return propertyDefinition;
    }

    public void SetPropertyDefinitions (PropertyDefinitionCollection propertyDefinitions)
    {
      ArgumentUtility.CheckNotNull(nameof(propertyDefinitions), propertyDefinitions);

      if (IsReadOnly)
        throw new NotSupportedException($"Type '{Type.GetFullNameSafe()}' is read-only.");

      CheckPropertyDefinitions(propertyDefinitions);

      _propertyDefinitions = propertyDefinitions;
      _propertyDefinitions.SetReadOnly();
    }

    public RelationEndPointDefinitionCollection GetRelationEndPointDefinitions ()
    {
      return IsReadOnly
        ? _cachedRelationEndPointDefinitions.Value
        : CreateRelationEndPointDefinitionCollection();
    }

    public IRelationEndPointDefinition? GetRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyName), propertyName);

      return GetRelationEndPointDefinitions()[propertyName];
    }

    public IRelationEndPointDefinition GetMandatoryRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty(nameof(propertyName), propertyName);

      var relationEndPointDefinition = GetRelationEndPointDefinition(propertyName);
      if (relationEndPointDefinition == null)
        throw new MappingException($"No relation found for type '{Type.GetFullNameSafe()}' and property '{propertyName}'.");

      return relationEndPointDefinition;
    }

    public void SetRelationEndPointDefinitions (RelationEndPointDefinitionCollection relationEndPoints)
    {
      ArgumentUtility.CheckNotNull(nameof(relationEndPoints), relationEndPoints);

      if (IsReadOnly)
        throw new NotSupportedException($"Type '{Type.GetFullNameSafe()}' is read-only.");

      CheckRelationEndPointDefinitions(relationEndPoints);

      _relationEndPoints = relationEndPoints;
      _relationEndPoints.SetReadOnly();
    }

    public PropertyDefinition? ResolveProperty (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull(nameof(propertyInformation), propertyInformation);

      var propertyAccessorData = PropertyAccessorDataCache.ResolvePropertyAccessorData(propertyInformation);
      return propertyAccessorData?.PropertyDefinition;
    }

    public IRelationEndPointDefinition? ResolveRelationEndPoint (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull(nameof(propertyInformation), propertyInformation);

      var propertyAccessorData = PropertyAccessorDataCache.ResolvePropertyAccessorData(propertyInformation);
      return propertyAccessorData?.RelationEndPointDefinition;
    }

    public virtual void SetReadOnly ()
    {
      if (_storageEntityDefinition == null)
        throw new InvalidOperationException("Cannot set the type definition read-only as the storage entity definition is not set.");
      if (_propertyDefinitions == null)
        throw new InvalidOperationException("Cannot set the type definition read-only as the property definitions are not set.");
      if (_relationEndPoints == null)
        throw new InvalidOperationException("Cannot set the type definition read-only as the relation endpoint definitions are not set.");

      IsReadOnly = true;
    }

    public override string ToString () => $"{GetType().Name}: {Type.GetFullNameSafe()}";
  }
}
