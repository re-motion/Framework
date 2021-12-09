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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create <see cref="PropertyDefinition"/> objects for testing.
  /// </summary>
  public static class PropertyDefinitionObjectMother
  {
    public static PropertyDefinition CreateForFakePropertyInfo ()
    {
      return CreateForFakePropertyInfo(TypeDefinitionObjectMother.CreateClassDefinition());
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo(TypeDefinitionObjectMother.CreateClassDefinition(), propertyName, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo(propertyName, propertyType, false);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, Type propertyType, bool isNullable)
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition();
      return CreateForFakePropertyInfo(typeDefinition, propertyName, false, propertyType, isNullable, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (TypeDefinition typeDefinition)
    {
      return CreateForFakePropertyInfo(typeDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo (TypeDefinition typeDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo(typeDefinition, propertyName, false, typeof(string), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (TypeDefinition typeDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo(typeDefinition, propertyName, false, typeof(string), true, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (TypeDefinition typeDefinition, string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo(typeDefinition, propertyName, false, propertyType, true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (TypeDefinition typeDefinition, bool isNullable)
    {
      return CreateForFakePropertyInfo(typeDefinition, "Test", false, typeof(string), isNullable, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        TypeDefinition typeDefinition,
        string propertyName,
        bool isObjectID,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      var declaringType = typeDefinition.Type;

      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.Name).Returns(propertyName + "FakeProperty");
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(propertyType);
      propertyInformationStub.Setup(stub => stub.DeclaringType).Returns(TypeAdapter.Create(declaringType));
      propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(TypeAdapter.Create(declaringType));

      var defaultValue = isNullable ? null : propertyType.GetDefaultValue();

      IPropertyInformation propertyInformation = propertyInformationStub.Object;
      return CreateForPropertyInformation(typeDefinition, propertyName, isObjectID, isNullable, maxLength, storageClass, propertyInformation, defaultValue);
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID ()
    {
      return CreateForFakePropertyInfo_ObjectID(TypeDefinitionObjectMother.CreateClassDefinition());
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (TypeDefinition typeDefinition)
    {
      return CreateForFakePropertyInfo_ObjectID(typeDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (TypeDefinition typeDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo(typeDefinition, propertyName, true, typeof(ObjectID), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (TypeDefinition typeDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo(typeDefinition, propertyName, true, typeof(ObjectID), true, null, storageClass);
    }

    public static PropertyDefinition CreateForRealPropertyInfo (TypeDefinition typeDefinition, Type declaringType, string propertyName)
    {
      var propertyInfo = declaringType.GetProperty(
          propertyName,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull(propertyInfo, "Property '" + propertyName + "' on type '" + declaringType + "'.");

      var fullPropertyName = declaringType.FullName + "." + propertyName;

      var isObjectID = ReflectionUtility.IsDomainObject(propertyInfo.PropertyType);

      IPropertyInformation propertyInformation = PropertyInfoAdapter.Create(propertyInfo);
      return CreateForPropertyInformation(
          typeDefinition,
          fullPropertyName,
          isObjectID,
          true,
          null,
          StorageClass.Persistent,
          propertyInformation,
          null);
    }

    public static PropertyDefinition CreateForPropertyInformation (
        TypeDefinition typeDefinition,
        string propertyName,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation(
          typeDefinition,
          propertyName,
          false,
          false,
          null,
          StorageClass.Persistent,
          propertyInformation,
          propertyInformation.PropertyType.GetDefaultValue());
    }

    public static PropertyDefinition CreateForPropertyInformation (
        TypeDefinition typeDefinition,
        StorageClass storageClass,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation(
          typeDefinition,
          "Test",
          false,
          false,
          null,
          storageClass,
          propertyInformation,
          propertyInformation.PropertyType.GetDefaultValue());
    }

    private static PropertyDefinition CreateForPropertyInformation (
        TypeDefinition typeDefinition,
        string propertyName,
        bool isObjectID,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass,
        IPropertyInformation propertyInformation,
        object defaultValue)
    {
      var propertyDefinition = new PropertyDefinition(
          typeDefinition,
          propertyInformation,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass,
          defaultValue);
      return propertyDefinition;
    }

    public static PropertyDefinition CreateForFakePropertyInfo_MaxLength (string propertyName, Type propertyType, int maxLength)
    {
      return CreateForFakePropertyInfo(TypeDefinitionObjectMother.CreateClassDefinition(), propertyName, false, propertyType, true, maxLength, StorageClass.Persistent);
    }
  }
}
