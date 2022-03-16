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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public sealed class ReferenceInterfaceDefinitionBuilder<TInterface> : ReferenceTypeDefinitionBuilder<ReferenceInterfaceDefinitionBuilder<TInterface>>
      where TInterface : IDomainObject
  {
    private readonly List<Type> _extendedInterfaces = new();

    public ReferenceInterfaceDefinitionBuilder ()
        : base(typeof(TInterface))
    {
      if (!typeof(TInterface).IsInterface)
        throw new ArgumentException("The specified type must be an interface.");
    }

    public IReadOnlyList<Type> ExtendedInterfaces => _extendedInterfaces;

    /// <inheritdoc />
    public override ReferenceInterfaceDefinitionBuilder<TInterface> GetTBuilder () => this;

    /// <inheritdoc />
    public override TypeDefinition BuildTypeDefinition (ReferenceTypeDefinitionBuilderContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var interfaceDefinition = new InterfaceDefinition(
          Type,
          ExtendedInterfaces.Select(context.ResolveInterfaceDefinition),
          StorageGroupType,
          DefaultStorageClass);

      SetupTypeDefinition(interfaceDefinition);
      return interfaceDefinition;
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> Property<T> (Expression<Func<TInterface, T>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      return Property<T>(ResolvePropertyNameFromExpression(propertySelector));
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> Property<T> (Expression<Func<TInterface, T>> propertySelector, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);
      ArgumentUtility.CheckNotNull("configure", configure);

      return Property<T>(ResolvePropertyNameFromExpression(propertySelector), configure);
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> PersistentProperty<T> (Expression<Func<TInterface, T>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      return PersistentProperty<T>(ResolvePropertyNameFromExpression(propertySelector));
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> PersistentProperty<T> (
        Expression<Func<TInterface, T>> propertySelector,
        Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);
      ArgumentUtility.CheckNotNull("configure", configure);

      return PersistentProperty<T>(ResolvePropertyNameFromExpression(propertySelector), configure);
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> TransactionProperty<T> (Expression<Func<TInterface, T>> propertySelector)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);

      return TransactionProperty<T>(ResolvePropertyNameFromExpression(propertySelector));
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> TransactionProperty<T> (
        Expression<Func<TInterface, T>> propertySelector,
        Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertySelector", propertySelector);
      ArgumentUtility.CheckNotNull("configure", configure);

      return TransactionProperty<T>(ResolvePropertyNameFromExpression(propertySelector), configure);
    }

    public ReferenceInterfaceDefinitionBuilder<TInterface> Extends<T> ()
        where T : IDomainObject
    {
      if (!typeof(T).IsInterface)
        throw new InvalidOperationException("Can only extend from other interfaces.");

      if (!_extendedInterfaces.Contains(typeof(T)))
        _extendedInterfaces.Add(typeof(T));

      return this;
    }
  }
}
