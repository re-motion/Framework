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
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public sealed class ReferenceClassDefinitionBuilder<TClass> : ReferenceTypeDefinitionBuilder<ReferenceClassDefinitionBuilder<TClass>>
      where TClass : DomainObject
  {
    private readonly List<Type> _implementedInterfaces = new();
    private readonly List<Type> _persistentMixins = new();

    public string ClassID { get; private set; }

    [CanBeNull]
    public Type BaseClass { get; private set; }

    public bool IsAbstract { get; private set; }

    public ReferenceClassDefinitionBuilder ()
        : base(typeof(TClass))
    {
      ClassID = Type.Name;
    }

    /// <inheritdoc />
    public override ReferenceClassDefinitionBuilder<TClass> GetTBuilder () => this;

    /// <inheritdoc />
    public override TypeDefinition BuildTypeDefinition (ReferenceTypeDefinitionBuilderContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var classDefinition = new ClassDefinition(
          ClassID,
          Type,
          IsAbstract,
          BaseClass != null ? context.ResolveClassDefinition(BaseClass) : null,
          _implementedInterfaces.Select(context.ResolveInterfaceDefinition),
          StorageGroupType,
          DefaultStorageClass,
          new PersistentMixinFinderStub(Type, _persistentMixins.ToArray()),
          MappingReflectorObjectMother.DomainObjectCreator);

      SetupTypeDefinition(classDefinition);
      return classDefinition;
    }

    public ReferenceClassDefinitionBuilder<TClass> Property<T> (Expression<Func<TClass, T>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      return Property<T>(ResolvePropertyNameFromExpression(propertySelector));
    }

    public ReferenceClassDefinitionBuilder<TClass> Property<T> (Expression<Func<TClass, T>> propertySelector, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);
      ArgumentUtility.CheckNotNull("configure", configure);

      return Property<T>(ResolvePropertyNameFromExpression(propertySelector), configure);
    }

    public ReferenceClassDefinitionBuilder<TClass> PersistentProperty<T> (Expression<Func<TClass, T>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      return PersistentProperty<T>(ResolvePropertyNameFromExpression(propertySelector));
    }

    public ReferenceClassDefinitionBuilder<TClass> PersistentProperty<T> (Expression<Func<TClass, T>> propertySelector, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);
      ArgumentUtility.CheckNotNull("configure", configure);

      return PersistentProperty<T>(ResolvePropertyNameFromExpression(propertySelector), configure);
    }

    public ReferenceClassDefinitionBuilder<TClass> TransactionProperty<T> (Expression<Func<TClass, T>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      return TransactionProperty<T>(ResolvePropertyNameFromExpression(propertySelector));
    }

    public ReferenceClassDefinitionBuilder<TClass> TransactionProperty<T> (Expression<Func<TClass, T>> propertySelector, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);
      ArgumentUtility.CheckNotNull("configure", configure);

      return TransactionProperty<T>(ResolvePropertyNameFromExpression(propertySelector), configure);
    }

    public ReferenceClassDefinitionBuilder<TClass> Extends<TBaseClass> ()
        where TBaseClass : DomainObject
    {
      if (BaseClass != null)
        throw new InvalidOperationException("A base class has already been set.");

      BaseClass = typeof(TBaseClass);

      return this;
    }

    public ReferenceClassDefinitionBuilder<TClass> Implements<TInterface> ()
        where TInterface : IDomainObject
    {
      if (!typeof(TInterface).IsInterface)
        throw new InvalidOperationException("Can only implement interfaces.");

      if (!_implementedInterfaces.Contains(typeof(TInterface)))
        _implementedInterfaces.Add(typeof(TInterface));

      return this;
    }

    public ReferenceClassDefinitionBuilder<TClass> WithClassID (string classID)
    {
      ArgumentUtility.CheckNotNull("classID", classID);

      ClassID = classID;

      return this;
    }

    public ReferenceClassDefinitionBuilder<TClass> SetIsAbstract (bool value = true)
    {
      IsAbstract = value;

      return this;
    }

    public ReferenceClassDefinitionBuilder<TClass> WithPersistentMixing<T> ()
    {
      if (!_persistentMixins.Contains(typeof(T)))
        _persistentMixins.Add(typeof(T));

      return this;
    }
  }
}
