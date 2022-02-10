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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [DebuggerDisplay("{GetType().Name} for {Type}")]
  public class InterfaceDefinition : TypeDefinition
  {
    public ReadOnlyCollection<InterfaceDefinition> ExtendedInterfaces { get; }

    private ReadOnlyCollection<ClassDefinition>? _implementingClasses;
    private ReadOnlyCollection<InterfaceDefinition>? _extendingInterfaces;

    public InterfaceDefinition (
        Type type,
        IEnumerable<InterfaceDefinition> extendedInterfaces,
        Type? storageGroupType,
        DefaultStorageClass defaultStorageClass)
        : base(type, storageGroupType, defaultStorageClass)
    {
      ArgumentUtility.CheckNotNull(nameof(extendedInterfaces), extendedInterfaces);

      var extendedInterfacesReadOnly = extendedInterfaces.ToList().AsReadOnly();
      ArgumentUtility.CheckNotNullOrItemsNull(nameof(extendedInterfaces), extendedInterfacesReadOnly);

      ExtendedInterfaces = extendedInterfacesReadOnly;
    }

    public ReadOnlyCollection<ClassDefinition> ImplementingClasses
    {
      get
      {
        if (_implementingClasses == null)
          throw new InvalidOperationException($"No implementing classes have been set for interface '{Type.GetFullNameSafe()}'.");

        return _implementingClasses;
      }
    }

    public ReadOnlyCollection<InterfaceDefinition> ExtendingInterfaces
    {
      get
      {
        if (_extendingInterfaces == null)
          throw new InvalidOperationException($"No extending interfaces have been set for interface '{Type.GetFullNameSafe()}'.");

        return _extendingInterfaces;
      }
    }

    public override bool IsPartOfInheritanceHierarchy => ExtendedInterfaces.Count > 0 || ImplementingClasses.Count > 0 || ExtendedInterfaces.Count > 0;

    public void SetImplementingClasses (IEnumerable<ClassDefinition> implementingClasses)
    {
      ArgumentUtility.CheckNotNull(nameof(implementingClasses), implementingClasses);

      if (IsReadOnly)
        throw new NotSupportedException($"Interface '{Type.GetFullNameSafe()}' is read-only.");

      var derivedClassesReadOnly = implementingClasses.ToList().AsReadOnly();
      CheckImplementingClasses(derivedClassesReadOnly);

      _implementingClasses = derivedClassesReadOnly;
    }

    public void SetExtendingInterfaces (IEnumerable<InterfaceDefinition> extendingInterfaces)
    {
      ArgumentUtility.CheckNotNull(nameof(extendingInterfaces), extendingInterfaces);

      if (IsReadOnly)
        throw new NotSupportedException($"Interface '{Type.GetFullNameSafe()}' is read-only.");

      var derivedClassesReadOnly = extendingInterfaces.ToList().AsReadOnly();
      CheckExtendingInterfaces(derivedClassesReadOnly);

      _extendingInterfaces = derivedClassesReadOnly;
    }

    public override void Accept (ITypeDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);

      visitor.VisitInterfaceDefinition(this);
    }

    public override T? Accept<T> (ITypeDefinitionVisitor<T> visitor)
        where T : default
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);

      return visitor.VisitInterfaceDefinition(this);
    }

    public override void SetReadOnly ()
    {
      if (_implementingClasses == null)
        throw new InvalidOperationException("Cannot set the type definition read-only as the implementing classes are not set.");
      if (_extendingInterfaces == null)
        throw new InvalidOperationException("Cannot set the type definition read-only as the extending interfaces are not set.");

      base.SetReadOnly();
    }

    public override bool IsAssignableFrom (TypeDefinition other)
    {
      ArgumentUtility.CheckNotNull("other", other);

      if (ReferenceEquals(this, other))
        return true;

      return ImplementingClasses.Any(e => e.IsAssignableFrom(other))
             || ExtendingInterfaces.Any(e => e.IsAssignableFrom(other));
    }

    protected override void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions)
    {
      foreach (var propertyDefinition in propertyDefinitions)
      {
        if (!ReferenceEquals(propertyDefinition.TypeDefinition, this))
        {
          throw new MappingException(
              $"Property '{propertyDefinition.PropertyName}' cannot be added to interface '{Type.GetFullNameSafe()}',"
              + $" because it was initialized for type '{propertyDefinition.TypeDefinition.Type.GetFullNameSafe()}'.");
        }

        foreach (var extendedInterface in ExtendedInterfaces)
        {
          var basePropertyDefinition = extendedInterface.GetPropertyDefinition(propertyDefinition.PropertyName);
          if (basePropertyDefinition != null)
          {
            throw new MappingException(
                $"Property '{propertyDefinition.PropertyName}' cannot be added to interface '{Type.GetFullNameSafe()}',"
                + $" because extended interface '{basePropertyDefinition.TypeDefinition.Type.GetFullNameSafe()}' already defines a property with the same name.");
          }
        }
      }
    }

    protected override void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints)
    {
      foreach (var endPointDefinition in relationEndPoints)
      {
        var relationEndPointTypeDefinition = endPointDefinition.TypeDefinition;
        var relationEndPointPropertyName = endPointDefinition.PropertyName;
        Assertion.DebugAssert(endPointDefinition.IsAnonymous == false, "endPointDefinition.IsAnonymous == false");
        Assertion.DebugIsNotNull(
            relationEndPointPropertyName,
            "endPointDefinition.PropertyName != null when endPointDefinition.IsAnonymous == false");

        if (!ReferenceEquals(relationEndPointTypeDefinition, this))
        {
          throw new MappingException(
              $"Relation end point for property '{relationEndPointPropertyName}' cannot be added to interface '{Type.GetFullNameSafe()}',"
              + $" because it was initialized for interface '{relationEndPointTypeDefinition.Type.GetFullNameSafe()}'.");
        }

        foreach (var extendedInterface in ExtendedInterfaces)
        {
          var baseEndPointDefinition = extendedInterface.GetRelationEndPointDefinition(relationEndPointPropertyName);
          if (baseEndPointDefinition != null)
          {
            throw new MappingException(
                $"Relation end point for property '{relationEndPointPropertyName}' cannot be added to interface '{Type.GetFullNameSafe()}',"
                + $" because extended interface '{baseEndPointDefinition.TypeDefinition.Type.GetFullNameSafe()}' already defines a relation end point with the same property name.");
          }
        }
      }
    }

    private void CheckImplementingClasses (IEnumerable<ClassDefinition> implementingClasses)
    {
      foreach (var implementingClass in implementingClasses)
      {
        if (!implementingClass.ImplementedInterfaces.Contains(this))
        {
          throw new MappingException(
              $"Interface '{Type.GetFullNameSafe()}' cannot be implemented by class '{implementingClass.Type.GetFullNameSafe()}', "
              + $"because '{Type.Name}' is not contained in the list of implemented interfaces on '{implementingClass.Type.Name}'.");
        }
      }
    }

    private void CheckExtendingInterfaces (IEnumerable<InterfaceDefinition> extendingInterfaces)
    {
      foreach (var extendingInterface in extendingInterfaces)
      {
        if (!extendingInterface.ExtendedInterfaces.Contains(this))
        {
          throw new MappingException(
              $"Interface '{Type.GetFullNameSafe()}' cannot be extended by interface '{extendingInterface.Type.GetFullNameSafe()}', "
              + $"because '{Type.Name}' is not contained in the list of extended interfaces on '{extendingInterface.Type.Name}'.");
        }
      }
    }
  }
}
