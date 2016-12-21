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
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [DebuggerDisplay ("{GetType().Name} for {ClassType.FullName}")]
  public class ClassDefinition
  {
    private readonly string _id;
    private bool _isReadOnly;
    private readonly Type _storageGroupType;
    private readonly PropertyAccessorDataCache _propertyAccessorDataCache;
    private readonly DoubleCheckedLockingContainer<RelationEndPointDefinitionCollection> _cachedRelationEndPointDefinitions;
    private readonly DoubleCheckedLockingContainer<PropertyDefinitionCollection> _cachedPropertyDefinitions;
    private readonly ClassDefinition _baseClass;
    private PropertyDefinitionCollection _propertyDefinitions;
    private RelationEndPointDefinitionCollection _relationEndPoints;
    private IStorageEntityDefinition _storageEntityDefinition;
    private ReadOnlyCollection<ClassDefinition> _derivedClasses;
    private readonly bool _isAbstract;
    private readonly Type _classType;
    private readonly IPersistentMixinFinder _persistentMixinFinder;
    private readonly IDomainObjectCreator _instanceCreator;
    private readonly Lazy<Func<ObjectID, IDomainObjectHandle<DomainObject>>> _handleCreator;

    public ClassDefinition (
        string id,
        Type classType,
        bool isAbstract,
        ClassDefinition baseClass,
        Type storageGroupType,
        IPersistentMixinFinder persistentMixinFinder,
        IDomainObjectCreator instanceCreator)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      ArgumentUtility.CheckNotNull ("classType", classType);
      ArgumentUtility.CheckNotNull ("persistentMixinFinder", persistentMixinFinder);

      _id = id;
      _storageGroupType = storageGroupType;

      _classType = classType;
      _persistentMixinFinder = persistentMixinFinder;
      _isAbstract = isAbstract;

      _propertyAccessorDataCache = new PropertyAccessorDataCache (this);
      _cachedRelationEndPointDefinitions = new DoubleCheckedLockingContainer<RelationEndPointDefinitionCollection> (
           () => RelationEndPointDefinitionCollection.CreateForAllRelationEndPoints (this, true));
      _cachedPropertyDefinitions =
          new DoubleCheckedLockingContainer<PropertyDefinitionCollection> (
              () => new PropertyDefinitionCollection (PropertyDefinitionCollection.CreateForAllProperties (this, true), true));

      _baseClass = baseClass;
      _instanceCreator = instanceCreator;
      _handleCreator = new Lazy<Func<ObjectID, IDomainObjectHandle<DomainObject>>> (BuildHandleCreator, LazyThreadSafetyMode.PublicationOnly);
    }

    // methods and properties

    public PropertyAccessorDataCache PropertyAccessorDataCache
    {
      get { return _propertyAccessorDataCache; }
    }

    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    public void SetReadOnly ()
    {
      _isReadOnly = true;
    }

    public bool IsSameOrBaseClassOf (ClassDefinition classDefinition)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      if (ReferenceEquals (this, classDefinition))
        return true;

      ClassDefinition baseClassOfProvidedClassDefinition = classDefinition.BaseClass;
      while (baseClassOfProvidedClassDefinition != null)
      {
        if (ReferenceEquals (this, baseClassOfProvidedClassDefinition))
          return true;

        baseClassOfProvidedClassDefinition = baseClassOfProvidedClassDefinition.BaseClass;
      }

      return false;
    }

    public ClassDefinition[] GetAllDerivedClasses ()
    {
      var allDerivedClasses = new List<ClassDefinition>();
      FillAllDerivedClasses (allDerivedClasses);
      return allDerivedClasses.ToArray();
    }

    public ClassDefinition GetInheritanceRootClass ()
    {
      if (BaseClass != null)
        return BaseClass.GetInheritanceRootClass();

      return this;
    }

    public bool Contains (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      return MyPropertyDefinitions.Contains (propertyDefinition);
    }

    [Obsolete (
        "This method is obsolete because it can lead to inefficient code. Use 'GetEndPointDefinition (propertyName).GetOppositeEndPointDefinition()' "
        + "instead. If you already have an IRelationEndPointDefinition, just use 'endPointDefinition.GetOppositeEndPointDefinition()'. (1.13.176)")]
    public IRelationEndPointDefinition GetOppositeEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);
      if (relationEndPointDefinition == null)
        return null;

      return relationEndPointDefinition.GetOppositeEndPointDefinition();
    }

    [Obsolete (
        "This method is obsolete because it can lead to inefficient code. Use 'GetMandatoryEndPointDefinition (propertyName).GetOppositeEndPointDefinition()' "
        + "instead. If you already have an IRelationEndPointDefinition, just use 'endPointDefinition.GetOppositeEndPointDefinition()'. (1.13.176)")]
    public IRelationEndPointDefinition GetMandatoryOppositeEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetMandatoryRelationEndPointDefinition (propertyName);
      return relationEndPointDefinition.GetMandatoryOppositeEndPointDefinition();
    }

    public PropertyDefinitionCollection GetPropertyDefinitions ()
    {
      return _cachedPropertyDefinitions.Value;
    }

    public RelationEndPointDefinitionCollection GetRelationEndPointDefinitions ()
    {
      return _cachedRelationEndPointDefinitions.Value;
    }

    [Obsolete (
        "This method is obsolete because it can lead to inefficient code. Use "
        + "'GetEndPointDefinition (propertyName).GetOppositeEndPointDefinition().ClassDefinition' instead. If you already have an "
        + "IRelationEndPointDefinition, just use 'endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition'. (1.13.176)")]
    public ClassDefinition GetOppositeClassDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var endPointDefinition = GetRelationEndPointDefinition (propertyName);
      if (endPointDefinition == null)
        return null;

      return endPointDefinition.GetOppositeClassDefinition();
    }

    [Obsolete (
        "This method is obsolete because it can lead to inefficient code. Use "
        + "'GetEndPointDefinition (propertyName).GetMandatoryOppositeEndPointDefinition().ClassDefinition' instead. If you already have an "
        + "IRelationEndPointDefinition, just use 'endPointDefinition.GetOppositeEndPointDefinition().ClassDefinition'. (1.13.176)")]
    public ClassDefinition GetMandatoryOppositeClassDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      ClassDefinition oppositeClassDefinition = GetOppositeClassDefinition (propertyName);

      if (oppositeClassDefinition == null)
        throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

      return oppositeClassDefinition;
    }

    public IRelationEndPointDefinition GetRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return _cachedRelationEndPointDefinitions.Value[propertyName];
    }

    public IRelationEndPointDefinition GetMandatoryRelationEndPointDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      IRelationEndPointDefinition relationEndPointDefinition = GetRelationEndPointDefinition (propertyName);

      if (relationEndPointDefinition == null)
        throw CreateMappingException ("No relation found for class '{0}' and property '{1}'.", ID, propertyName);

      return relationEndPointDefinition;
    }

    public bool IsMyRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      return (relationEndPointDefinition.ClassDefinition == this && !relationEndPointDefinition.IsAnonymous);
    }

    public bool IsRelationEndPoint (IRelationEndPointDefinition relationEndPointDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);

      if (IsMyRelationEndPoint (relationEndPointDefinition))
        return true;

      if (BaseClass != null)
        return BaseClass.IsRelationEndPoint (relationEndPointDefinition);

      return false;
    }

    public PropertyDefinition GetPropertyDefinition (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      var propertyDefinition = MyPropertyDefinitions[propertyName];

      if (propertyDefinition == null && BaseClass != null)
        return BaseClass.GetPropertyDefinition (propertyName);

      return propertyDefinition;
    }

    public void SetStorageEntity (IStorageEntityDefinition storageEntityDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageEntityDefinition", storageEntityDefinition);

      if (_isReadOnly)
        throw new NotSupportedException (String.Format ("Class '{0}' is read-only.", ID));

      _storageEntityDefinition = storageEntityDefinition;
    }

    public void SetPropertyDefinitions (PropertyDefinitionCollection propertyDefinitions)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinitions", propertyDefinitions);

      if (_propertyDefinitions != null)
        throw new InvalidOperationException (String.Format ("The property-definitions for class '{0}' have already been set.", ID));

      if (_isReadOnly)
        throw new NotSupportedException (String.Format ("Class '{0}' is read-only.", ID));

      CheckPropertyDefinitions (propertyDefinitions);

      _propertyDefinitions = propertyDefinitions;
      _propertyDefinitions.SetReadOnly();
    }

    public void SetRelationEndPointDefinitions (RelationEndPointDefinitionCollection relationEndPoints)
    {
      ArgumentUtility.CheckNotNull ("relationEndPoints", relationEndPoints);

      if (_relationEndPoints != null)
        throw new InvalidOperationException (String.Format ("The relation end point definitions for class '{0}' have already been set.", ID));

      if (_isReadOnly)
        throw new NotSupportedException (String.Format ("Class '{0}' is read-only.", ID));

      CheckRelationEndPointDefinitions (relationEndPoints);

      _relationEndPoints = relationEndPoints;
      _relationEndPoints.SetReadOnly();
    }

    public void SetDerivedClasses (IEnumerable<ClassDefinition> derivedClasses)
    {
      ArgumentUtility.CheckNotNull ("derivedClasses", derivedClasses);

      if (_derivedClasses != null)
        throw new InvalidOperationException (String.Format ("The derived-classes for class '{0}' have already been set.", ID));

      if (_isReadOnly)
        throw new NotSupportedException (String.Format ("Class '{0}' is read-only.", ID));

      var derivedClassesReadOnly = derivedClasses.ToList ().AsReadOnly ();
      CheckDerivedClass (derivedClassesReadOnly);

      _derivedClasses = derivedClassesReadOnly;
    }

    public PropertyDefinition GetMandatoryPropertyDefinition (string propertyName)
    {
      PropertyDefinition propertyDefinition = GetPropertyDefinition (propertyName);

      if (propertyDefinition == null)
        throw CreateMappingException ("Class '{0}' does not contain the property '{1}'.", ID, propertyName);

      return propertyDefinition;
    }

    public PropertyDefinition this [string propertyName]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
        return MyPropertyDefinitions[propertyName];
      }
    }

    public string ID
    {
      get { return _id; }
    }

    public IStorageEntityDefinition StorageEntityDefinition
    {
      get { return _storageEntityDefinition; }
    }

    public Type ClassType
    {
      get { return _classType; }
    }

    public virtual bool IsClassTypeResolved
    {
      get { return true; }
    }

    public bool IsAbstract
    {
      get { return _isAbstract; }
    }

    public IDomainObjectCreator InstanceCreator
    {
      get { return _instanceCreator; }
    }

    public Func<ObjectID, IDomainObjectHandle<DomainObject>> HandleCreator
    {
      get { return _handleCreator.Value; }
    }

    public PropertyDefinition ResolveProperty (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

      var propertyAccessorData = PropertyAccessorDataCache.ResolvePropertyAccessorData (propertyInformation);
      return propertyAccessorData == null ? null : propertyAccessorData.PropertyDefinition;
    }

    public IRelationEndPointDefinition ResolveRelationEndPoint (IPropertyInformation propertyInformation)
    {
      ArgumentUtility.CheckNotNull ("propertyInformation", propertyInformation);

      var propertyAccessorData = PropertyAccessorDataCache.ResolvePropertyAccessorData (propertyInformation);
      return propertyAccessorData == null ? null : propertyAccessorData.RelationEndPointDefinition;
    }

    public ClassDefinition BaseClass
    {
      get { return _baseClass; }
    }

    public Type StorageGroupType
    {
      get { return _storageGroupType; }
    }

    public PropertyDefinitionCollection MyPropertyDefinitions
    {
      get
      {
        if (_propertyDefinitions == null)
          throw new InvalidOperationException (String.Format ("No property definitions have been set for class '{0}'.", ID));

        return _propertyDefinitions;
      }
    }

    public RelationEndPointDefinitionCollection MyRelationEndPointDefinitions
    {
      get 
      {
        if (_relationEndPoints == null)
          throw new InvalidOperationException (String.Format ("No relation end point definitions have been set for class '{0}'.", ID));

        return _relationEndPoints;
      }
    }

    public ReadOnlyCollection<ClassDefinition> DerivedClasses
    {
      get
      {
        if (_derivedClasses == null)
          throw new InvalidOperationException (String.Format ("No derived classes have been set for class '{0}'.", ID));

        return _derivedClasses;
      }
    }

    public bool IsPartOfInheritanceHierarchy
    {
      get { return (BaseClass != null || DerivedClasses.Count > 0); }
    }

    public IPersistentMixinFinder PersistentMixinFinder
    {
      get { return _persistentMixinFinder; }
    }

    public IEnumerable<Type> PersistentMixins
    {
      get { return _persistentMixinFinder.GetPersistentMixins (); }
    }

    public Type GetPersistentMixin (Type mixinToSearch)
    {
      ArgumentUtility.CheckNotNull ("mixinToSearch", mixinToSearch);
      if (PersistentMixins.Contains (mixinToSearch))
        return mixinToSearch;
      else
        return PersistentMixins.FirstOrDefault (mixinToSearch.IsAssignableFrom);
    }

    public void ValidateCurrentMixinConfiguration ()
    {
      var currentMixinConfiguration = Mapping.PersistentMixinFinder.GetMixinConfigurationForDomainObjectType (ClassType);
      if (!Equals (currentMixinConfiguration, PersistentMixinFinder.MixinConfiguration))
      {
        string message = String.Format (
            "The mixin configuration for domain object type '{0}' was changed after the mapping information was built." + Environment.NewLine
            + "Original configuration: {1}." + Environment.NewLine
            + "Active configuration: {2}",
            ClassType,
            PersistentMixinFinder.MixinConfiguration,
            currentMixinConfiguration);
        throw new MappingException (message);
      }
    }

    public override string ToString ()
    {
      return GetType().FullName + ": " + _id;
    }

    private MappingException CreateMappingException (string message, params object[] args)
    {
      return new MappingException (String.Format (message, args));
    }

    private void FillAllDerivedClasses (List<ClassDefinition> allDerivedClasses)
    {
      foreach (ClassDefinition derivedClass in DerivedClasses)
      {
        allDerivedClasses.Add (derivedClass);
        derivedClass.FillAllDerivedClasses (allDerivedClasses);
      }
    }

    private void CheckPropertyDefinitions (IEnumerable<PropertyDefinition> propertyDefinitions)
    {
      foreach (var propertyDefinition in propertyDefinitions)
      {
        if (!ReferenceEquals (propertyDefinition.ClassDefinition, this))
        {
          throw CreateMappingException (
              "Property '{0}' cannot be added to class '{1}', because it was initialized for class '{2}'.",
              propertyDefinition.PropertyName,
              _id,
              propertyDefinition.ClassDefinition.ID);
        }

        var basePropertyDefinition = BaseClass != null ? BaseClass.GetPropertyDefinition (propertyDefinition.PropertyName) : null;
        if (basePropertyDefinition != null)
        {
          string definingClass = String.Format ("base class '{0}'", basePropertyDefinition.ClassDefinition.ID);

          throw CreateMappingException (
              "Property '{0}' cannot be added to class '{1}', because {2} already defines a property with the same name.",
              propertyDefinition.PropertyName,
              _id,
              definingClass);
        }
      }
    }

    private void CheckRelationEndPointDefinitions (IEnumerable<IRelationEndPointDefinition> relationEndPoints)
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationEndPoints)
      {
        if (!ReferenceEquals (endPointDefinition.ClassDefinition, this))
        {
          throw CreateMappingException (
              "Relation end point for property '{0}' cannot be added to class '{1}', because it was initialized for class '{2}'.",
              endPointDefinition.PropertyName,
              _id,
              endPointDefinition.ClassDefinition.ID);
        }

        var baseEndPointDefinition = BaseClass != null ? BaseClass.GetRelationEndPointDefinition (endPointDefinition.PropertyName) : null;
        if (baseEndPointDefinition != null)
        {
          string definingClass = String.Format ("base class '{0}'", baseEndPointDefinition.ClassDefinition.ID);

          throw CreateMappingException (
              "Relation end point for property '{0}' cannot be added to class '{1}', because {2} already defines a relation end point with the same property name.",
              endPointDefinition.PropertyName,
              _id,
              definingClass);
        }
      }
    }

    private void CheckDerivedClass (IEnumerable<ClassDefinition> derivedClasses)
    {
      foreach (var derivedClass in derivedClasses)
      {
        if (derivedClass.BaseClass == null)
        {
          throw CreateMappingException (
              "Derived class '{0}' cannot be added to class '{1}', because it has no base class definition defined.", derivedClass.ID, _id);
        }

        if (derivedClass.BaseClass != this)
        {
          throw CreateMappingException (
              "Derived class '{0}' cannot be added to class '{1}', because it has class '{2}' as its base class definition defined.",
              derivedClass.ID,
              _id,
              derivedClass.BaseClass.ID);
        }
      }
    }

    private Func<ObjectID, IDomainObjectHandle<DomainObject>> BuildHandleCreator ()
    {
      var objectIDParameter = Expression.Parameter (typeof (ObjectID), "objectID");

      Expression body;
      if (typeof (DomainObject).IsAssignableFrom (ClassType))
      {
        var handleType = typeof (DomainObjectHandle<>).MakeGenericType (ClassType);

        var constructorInfo = handleType.GetConstructor (
            BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof (ObjectID) }, null);
        Assertion.DebugAssert (constructorInfo != null);

        body = Expression.New (constructorInfo, objectIDParameter);
      }
      else
      {
        var throwingDelegate =
            (Func<IDomainObjectHandle<DomainObject>>)
            (() => { throw new InvalidOperationException ("Handles cannot be created when the ClassType does not derive from DomainObject."); });
        body = Expression.Invoke (Expression.Constant (throwingDelegate));
      }

      return Expression.Lambda<Func<ObjectID, IDomainObjectHandle<DomainObject>>> (body, objectIDParameter).Compile ();
    }

    [Obsolete ("This method has been removed. Use the StorageEntityDefinition property instead. (1.13.118)")]
    public string GetEntityName ()
    {
      throw new NotImplementedException();
    }

    [Obsolete ("This method has been removed. Use the StorageEntityDefinition property instead. (1.13.118)")]
    public string[] GetAllConcreteEntityNames ()
    {
      throw new NotImplementedException ();
    }
  }
}