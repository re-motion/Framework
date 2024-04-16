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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [DebuggerDisplay("{GetType().Name} for {Type.Name}")]
  public class ClassDefinition : TypeDefinition
  {
    private readonly Lazy<Func<ObjectID, IDomainObjectHandle<DomainObject>>> _handleCreator;

    private ReadOnlyCollection<ClassDefinition>? _derivedClasses;

    public string ID { get; }

    public bool IsAbstract { get; }

    public IDomainObjectCreator InstanceCreator { get; }

    public ClassDefinition? BaseClass { get; }

    public ReadOnlyCollection<InterfaceDefinition> ImplementedInterfaces { get; }

    public IPersistentMixinFinder PersistentMixinFinder { get; }

    public ClassDefinition (
        string id,
        Type type,
        bool isAbstract,
        ClassDefinition? baseClass,
        IEnumerable<InterfaceDefinition> implementedInterfaces,
        Type? storageGroupType,
        DefaultStorageClass defaultStorageClass,
        IPersistentMixinFinder persistentMixinFinder,
        IDomainObjectCreator instanceCreator)
        : base(type, storageGroupType, defaultStorageClass)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("persistentMixinFinder", persistentMixinFinder);

      var implementedInterfacesReadOnly = implementedInterfaces.ToList().AsReadOnly();
      ArgumentUtility.CheckNotNullOrItemsNull(nameof(implementedInterfaces), implementedInterfacesReadOnly);

      ID = id;

      PersistentMixinFinder = persistentMixinFinder;
      IsAbstract = isAbstract;

      BaseClass = baseClass;
      ImplementedInterfaces = implementedInterfacesReadOnly;

      InstanceCreator = instanceCreator;
      _handleCreator = new Lazy<Func<ObjectID, IDomainObjectHandle<DomainObject>>>(BuildHandleCreator, LazyThreadSafetyMode.PublicationOnly);
    }

    // methods and properties

    public bool IsSameOrBaseClassOf (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

      if (ReferenceEquals(this, classDefinition))
        return true;

      ClassDefinition? baseClassOfProvidedClassDefinition = classDefinition.BaseClass;
      while (baseClassOfProvidedClassDefinition != null)
      {
        if (ReferenceEquals(this, baseClassOfProvidedClassDefinition))
          return true;

        baseClassOfProvidedClassDefinition = baseClassOfProvidedClassDefinition.BaseClass;
      }

      return false;
    }

    public override bool IsAssignableFrom (TypeDefinition other)
    {
      if (ReferenceEquals(this, other))
        return true;

      return DerivedClasses.Any(e => e.IsAssignableFrom(other));
    }

    public ClassDefinition[] GetAllDerivedClasses ()
    {
      var allDerivedClasses = new List<ClassDefinition>();
      FillAllDerivedClasses(allDerivedClasses);
      return allDerivedClasses.ToArray();
    }

    public bool IsMyRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      return (relationEndPointDefinition.TypeDefinition == this && !relationEndPointDefinition.IsAnonymous);
    }

    public bool IsRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);

      if (IsMyRelationEndPoint(relationEndPointDefinition))
        return true;

      if (BaseClass != null)
        return BaseClass.IsRelationEndPoint(relationEndPointDefinition);

      return false;
    }

    public override PropertyDefinition? GetPropertyDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);

      var propertyDefinition = base.GetPropertyDefinition(propertyName);
      if (propertyDefinition == null && BaseClass != null)
        return BaseClass.GetPropertyDefinition(propertyName);

      return propertyDefinition;
    }

    public void SetDerivedClasses (IEnumerable<ClassDefinition> derivedClasses)
    {
      ArgumentUtility.CheckNotNull("derivedClasses", derivedClasses);

      if (IsReadOnly)
        throw new NotSupportedException($"Class '{ID}' is read-only.");

      var derivedClassesReadOnly = derivedClasses.ToList().AsReadOnly();
      CheckDerivedClass(derivedClassesReadOnly);

      _derivedClasses = derivedClassesReadOnly;
    }

    public Func<ObjectID, IDomainObjectHandle<DomainObject>> HandleCreator => _handleCreator.Value;

    public ReadOnlyCollection<ClassDefinition> DerivedClasses
    {
      get
      {
        if (_derivedClasses == null)
          throw new InvalidOperationException($"No derived classes have been set for class '{ID}'.");

        return _derivedClasses;
      }
    }

    public override bool IsPartOfInheritanceHierarchy => BaseClass != null || DerivedClasses.Count > 0;

    public IEnumerable<Type> PersistentMixins => PersistentMixinFinder.GetPersistentMixins();

    public Type? GetPersistentMixin (Type mixinToSearch)
    {
      ArgumentUtility.CheckNotNull("mixinToSearch", mixinToSearch);
      if (PersistentMixins.Contains(mixinToSearch))
        return mixinToSearch;
      else
        return PersistentMixins.FirstOrDefault(mixinToSearch.IsAssignableFrom);
    }

    public void ValidateCurrentMixinConfiguration ()
    {
      var currentMixinConfiguration = Mapping.PersistentMixinFinder.GetMixinConfigurationForDomainObjectType(Type);
      if (!Equals(currentMixinConfiguration, PersistentMixinFinder.MixinConfiguration))
      {
        string message = string.Format(
            "The mixin configuration for domain object type '{0}' was changed after the mapping information was built." + Environment.NewLine
            + "Original configuration: {1}." + Environment.NewLine
            + "Active configuration: {2}",
            Type,
            PersistentMixinFinder.MixinConfiguration,
            currentMixinConfiguration);
        throw new MappingException(message);
      }
    }

    private void FillAllDerivedClasses (List<ClassDefinition> allDerivedClasses)
    {
      foreach (ClassDefinition derivedClass in DerivedClasses)
      {
        allDerivedClasses.Add(derivedClass);
        derivedClass.FillAllDerivedClasses(allDerivedClasses);
      }
    }

    public override void Accept (ITypeDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);

      visitor.VisitClassDefinition(this);
    }

    [return: MaybeNull]
    public override T Accept<T> (ITypeDefinitionVisitor<T> visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);

      return visitor.VisitClassDefinition(this);
    }

    public override void SetReadOnly ()
    {
      if (_derivedClasses == null)
        throw new InvalidOperationException("Cannot set the type definition read-only as the derived classes are not set.");

      base.SetReadOnly();
    }

    public override string ToString () => $"{GetType().Name}: {ID}";

    protected override void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions)
    {
      foreach (var propertyDefinition in propertyDefinitions)
      {
        if (!ReferenceEquals(propertyDefinition.TypeDefinition, this))
        {
          throw new MappingException(
              $"Property '{propertyDefinition.PropertyName}' cannot be added to class '{ID}',"
              + $" because it was initialized for type '{propertyDefinition.TypeDefinition.Type.GetFullNameSafe()}'.");
        }

        var baseClass = BaseClass;
        var basePropertyDefinition = baseClass?.GetPropertyDefinition(propertyDefinition.PropertyName);
        if (baseClass != null && basePropertyDefinition != null)
        {
          throw new MappingException(
              $"Property '{propertyDefinition.PropertyName}' cannot be added to class '{ID}',"
              + $" because base type '{basePropertyDefinition.TypeDefinition.Type.GetFullNameSafe()}' already defines a property with the same name.");
        }
      }
    }

    protected override void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationEndPoints)
      {
        var relationEndPointClassDefinition = endPointDefinition.TypeDefinition;
        var relationEndPointPropertyName = endPointDefinition.PropertyName;
        Assertion.DebugAssert(endPointDefinition.IsAnonymous == false, "endPointDefinition.IsAnonymous == false");
        Assertion.DebugIsNotNull(relationEndPointPropertyName, "endPointDefinition.PropertyName != null when endPointDefinition.IsAnonymous == false");

        if (!ReferenceEquals(relationEndPointClassDefinition, this))
        {
          throw new MappingException(
              $"Relation end point for property '{relationEndPointPropertyName}' cannot be added to class '{ID}',"
              + $" because it was initialized for class '{relationEndPointClassDefinition.Type.GetFullNameSafe()}'.");
        }

        var baseClass = BaseClass;
        var baseEndPointDefinition = baseClass?.GetRelationEndPointDefinition(relationEndPointPropertyName);
        if (baseClass != null && baseEndPointDefinition != null)
        {
          throw new MappingException(
              $"Relation end point for property '{relationEndPointPropertyName}' cannot be added to class '{ID}',"
              + $" because base class '{baseClass.ID}' already defines a relation end point with the same property name.");
        }
      }
    }

    private void CheckDerivedClass (IEnumerable<ClassDefinition> derivedClasses)
    {
      foreach (var derivedClass in derivedClasses)
      {
        if (derivedClass.BaseClass == null)
        {
          throw new MappingException(
              $"Derived class '{derivedClass.ID}' cannot be added to class '{ID}', because it has no base class definition defined.");
        }

        if (derivedClass.BaseClass != this)
        {
          throw new MappingException(
              $"Derived class '{derivedClass.ID}' cannot be added to class '{ID}', because it has class '{derivedClass.BaseClass.ID}' as its base class definition defined.");
        }
      }
    }

    private Func<ObjectID, IDomainObjectHandle<DomainObject>> BuildHandleCreator ()
    {
      var objectIDParameter = Expression.Parameter(typeof(ObjectID), "objectID");

      Expression body;
      if (typeof(DomainObject).IsAssignableFrom(Type))
      {
        var handleType = typeof(DomainObjectHandle<>).MakeGenericType(Type);

        var constructorInfo = handleType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(ObjectID) }, null);
        Assertion.DebugAssert(constructorInfo != null);

        body = Expression.New(constructorInfo, objectIDParameter);
      }
      else
      {
        var throwingDelegate =
            (Func<IDomainObjectHandle<DomainObject>>)(() =>
            {
              throw new InvalidOperationException("Handles cannot be created when the Type does not derive from DomainObject.");
            });
        body = Expression.Invoke(Expression.Constant(throwingDelegate));
      }

      return Expression.Lambda<Func<ObjectID, IDomainObjectHandle<DomainObject>>>(body, objectIDParameter).Compile();
    }
  }
}
