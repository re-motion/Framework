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
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public abstract class ReferenceTypeDefinitionBuilder<TBuilder> : IReferenceTypeDefinitionBuilder
      where TBuilder : IReferenceTypeDefinitionBuilder
  {
    private readonly List<ReferenceTypeDefinitionPropertyBuilder> _properties = new();

    public Type Type { get; }

    [CanBeNull]
    public Type StorageGroupType { get; protected set; }

    public DefaultStorageClass DefaultStorageClass { get; protected set; } = DefaultStorageClass.Persistent;

    [CanBeNull]
    public IStorageEntityDefinition StorageEntityDefinition { get; protected set; }

    protected ReferenceTypeDefinitionBuilder (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      Type = type;
    }

    public abstract TBuilder GetTBuilder ();

    public abstract TypeDefinition BuildTypeDefinition (ReferenceTypeDefinitionBuilderContext context);

    public TBuilder Property<T> (string propertyName)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      return Property<T>(propertyName, _ => { });
    }

    public TBuilder Property<T> (string propertyName, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("configure", configure);

      if (_properties.Any(e => e.PropertyName == propertyName))
        throw new InvalidOperationException($"A property with the name '{propertyName}' has already been added.");

      var propertyBuilder = new ReferenceTypeDefinitionPropertyBuilder(Type, typeof(T), propertyName);
      configure(propertyBuilder);

      _properties.Add(propertyBuilder);

      return GetTBuilder();
    }

    public TBuilder PersistentProperty<T> (string propertyName)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      return Property<T>(propertyName, c => c.WithStorageClass(StorageClass.Persistent).WithColumnName(propertyName));
    }

    public TBuilder PersistentProperty<T> (string propertyName, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("configure", configure);

      return Property<T>(propertyName, c => configure(c.WithStorageClass(StorageClass.Persistent).WithColumnName(propertyName)));
    }

    public TBuilder TransactionProperty<T> (string propertyName)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      return Property<T>(propertyName, c => c.WithStorageClass(StorageClass.Transaction));
    }

    public TBuilder TransactionProperty<T> (string propertyName, Action<ReferenceTypeDefinitionPropertyBuilder> configure)
    {
      ArgumentUtility.CheckNotNull("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("configure", configure);

      return Property<T>(propertyName, c => configure(c.WithStorageClass(StorageClass.Transaction)));
    }

    public TBuilder WithStorageGroupType ([CanBeNull] Type storageGroupType)
    {
      StorageGroupType = storageGroupType;

      return GetTBuilder();
    }

    public TBuilder WithDefaultStorageClass (DefaultStorageClass defaultStorageClass)
    {
      DefaultStorageClass = defaultStorageClass;

      return GetTBuilder();
    }

    public TBuilder WithEntityName (string entityName)
    {
      ArgumentUtility.CheckNotNull("entityName", entityName);

      StorageEntityDefinition = TableDefinitionObjectMother.Create(
          FakeMappingConfiguration.Current.DefaultStorageProviderDefinition,
          new EntityNameDefinition(null, entityName),
          new EntityNameDefinition(null, entityName + "View"));

      return GetTBuilder();
    }

    public TBuilder WithStorageEntityDefinition (IStorageEntityDefinition storageEntityDefinition)
    {
      ArgumentUtility.CheckNotNull("storageEntityDefinition", storageEntityDefinition);

      StorageEntityDefinition = storageEntityDefinition;

      return GetTBuilder();
    }

    protected void SetupTypeDefinition (TypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);

      if (StorageEntityDefinition == null)
        WithEntityName(Type.Name);

      typeDefinition.SetStorageEntity(StorageEntityDefinition);
      typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(_properties.Select(e => e.BuildPropertyDefinition(typeDefinition)), true));
      typeDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
    }

    protected string ResolvePropertyNameFromExpression<TClass, T> (Expression<Func<TClass, T>> expression)
    {
      return MemberInfoFromExpressionUtility.GetProperty(expression).Name;
    }
  }
}
