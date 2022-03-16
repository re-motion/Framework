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
using System.Reflection;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests
{
  public class ReferenceTypeDefinitionPropertyBuilder
  {
    public Type MemberType { get; }

    public Type PropertyType { get; }

    public string PropertyName { get; }

    public int? MaxLength { get; private set; }

    public bool IsNullable { get; private set; }

    public StorageClass StorageClass { get; private set; } = StorageClass.None;

    [CanBeNull]
    public IStoragePropertyDefinition StoragePropertyDefinition { get; private set; }

    public ReferenceTypeDefinitionPropertyBuilder (Type memberType, Type propertyType, string propertyName)
    {
      ArgumentUtility.CheckNotNull("memberType", memberType);
      ArgumentUtility.CheckNotNull("propertyType", propertyType);
      ArgumentUtility.CheckNotNull("propertyName", propertyName);

      MemberType = memberType;
      PropertyType = propertyType;
      PropertyName = propertyName;
    }

    public ReferenceTypeDefinitionPropertyBuilder WithMaxLength (int? value)
    {
      MaxLength = value;

      return this;
    }

    public ReferenceTypeDefinitionPropertyBuilder SetIsNullable (bool value = true)
    {
      IsNullable = value;

      return this;
    }

    public ReferenceTypeDefinitionPropertyBuilder WithStorageClass (StorageClass storageClass)
    {
      StorageClass = storageClass;

      return this;
    }

    public ReferenceTypeDefinitionPropertyBuilder WithColumnName (string columnName)
    {
      ArgumentUtility.CheckNotNull("columnName", columnName);
      StoragePropertyDefinition = new FakeStoragePropertyDefinition(columnName);

      return this;
    }

    public ReferenceTypeDefinitionPropertyBuilder WithStoragePropertyDefinition (IStoragePropertyDefinition storagePropertyDefinition)
    {
      ArgumentUtility.CheckNotNull("storagePropertyDefinition", storagePropertyDefinition);
      StoragePropertyDefinition = storagePropertyDefinition;

      return this;
    }

    public PropertyDefinition BuildPropertyDefinition (TypeDefinition memberTypeDefinition)
    {
      ArgumentUtility.CheckNotNull("memberTypeDefinition", memberTypeDefinition);
      if (memberTypeDefinition.Type != MemberType)
        throw new InvalidOperationException("The specified type definition and member type do not match.");

      var propertyInfo = MemberType.GetProperty(
          PropertyName,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assertion.IsNotNull(propertyInfo);
      if (propertyInfo.PropertyType != PropertyType)
        throw new InvalidOperationException("The type of the found property does not match the declared property type.");

      var result = new PropertyDefinition(
          memberTypeDefinition,
          PropertyInfoAdapter.Create(propertyInfo),
          string.Join(".", MemberType.FullName, PropertyName),
          ReflectionUtility.IsDomainObject(PropertyType),
          IsNullable,
          MaxLength,
          StorageClass,
          propertyInfo.PropertyType.GetDefaultValue());

      if (StoragePropertyDefinition != null)
        result.SetStorageProperty(StoragePropertyDefinition);

      return result;
    }
  }
}
