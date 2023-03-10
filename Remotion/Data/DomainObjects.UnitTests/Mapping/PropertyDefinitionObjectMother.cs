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
      return CreateForFakePropertyInfo(ClassDefinitionObjectMother.CreateClassDefinition());
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo(ClassDefinitionObjectMother.CreateClassDefinition(), propertyName, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo(propertyName, propertyType, false);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, Type propertyType, bool isNullable)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      return CreateForFakePropertyInfo(
          classDefinition,
          propertyName,
          false,
          propertyType,
          isNullable,
          null,
          StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition)
    {
      return CreateForFakePropertyInfo(classDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo(classDefinition, propertyName, false, typeof(string), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo(classDefinition, propertyName, false, typeof(string), true, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo(
          classDefinition,
          propertyName,
          false,
          propertyType,
          true,
          null,
          StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, bool isNullable)
    {
      return CreateForFakePropertyInfo(classDefinition, "Test", false, typeof(string), isNullable, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (
        ClassDefinition classDefinition,
        string propertyName,
        bool isObjectID,
        Type propertyType,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass)
    {
      Type declaringType = classDefinition.ClassType;

      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.Name).Returns(propertyName + "FakeProperty");
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(propertyType);
      propertyInformationStub.Setup(stub => stub.DeclaringType).Returns(TypeAdapter.Create(declaringType));
      propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(TypeAdapter.Create(declaringType));

      var defaultValue = !isNullable && propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;

      IPropertyInformation propertyInformation = propertyInformationStub.Object;
      return CreateForPropertyInformation(classDefinition, propertyName, isObjectID, isNullable, maxLength, storageClass, propertyInformation, defaultValue);
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID ()
    {
      return CreateForFakePropertyInfo_ObjectID(ClassDefinitionObjectMother.CreateClassDefinition());
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition)
    {
      return CreateForFakePropertyInfo_ObjectID(classDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo(classDefinition, propertyName, true, typeof(ObjectID), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo(classDefinition, propertyName, true, typeof(ObjectID), true, null, storageClass);
    }

    public static PropertyDefinition CreateForRealPropertyInfo (ClassDefinition classDefinition, Type declaringClassType, string propertyName)
    {
      var propertyInfo = declaringClassType.GetProperty(
          propertyName,
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull(propertyInfo, "Property '" + propertyName + "' on type '" + declaringClassType + "'.");

      var fullPropertyName = declaringClassType.FullName + "." + propertyName;

      var isObjectID = ReflectionUtility.IsDomainObject(propertyInfo.PropertyType);

      IPropertyInformation propertyInformation = PropertyInfoAdapter.Create(propertyInfo);
      return CreateForPropertyInformation(
          classDefinition,
          fullPropertyName,
          isObjectID,
          true,
          null,
          StorageClass.Persistent,
          propertyInformation,
          null);
    }

    public static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition,
        string propertyName,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation(
          classDefinition,
          propertyName,
          false,
          false,
          null,
          StorageClass.Persistent,
          propertyInformation,
          propertyInformation.PropertyType.IsValueType ? Activator.CreateInstance(propertyInformation.PropertyType) : null);
    }

    public static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation(
          classDefinition,
          "Test",
          false,
          false,
          null,
          storageClass,
          propertyInformation,
          propertyInformation.PropertyType.IsValueType ? Activator.CreateInstance(propertyInformation.PropertyType) : null);
    }

    private static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition,
        string propertyName,
        bool isObjectID,
        bool isNullable,
        int? maxLength,
        StorageClass storageClass,
        IPropertyInformation propertyInformation,
        object defaultValue)
    {
      var propertyDefinition = new PropertyDefinition(
          classDefinition,
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
      return CreateForFakePropertyInfo(ClassDefinitionObjectMother.CreateClassDefinition(), propertyName, false, propertyType, true, maxLength, StorageClass.Persistent);
    }
  }
}
