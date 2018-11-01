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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  /// <summary>
  /// Provides simple factory methods to manually create <see cref="PropertyDefinition"/> objects for testing.
  /// </summary>
  public static class PropertyDefinitionObjectMother
  {
    public static PropertyDefinition CreateForFakePropertyInfo ()
    {
      return CreateForFakePropertyInfo (ClassDefinitionObjectMother.CreateClassDefinition ());
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (ClassDefinitionObjectMother.CreateClassDefinition(), propertyName, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo (propertyName, propertyType, false);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (string propertyName, Type propertyType, bool isNullable)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      return CreateForFakePropertyInfo (classDefinition, propertyName, false, propertyType, isNullable, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition)
    {
      return CreateForFakePropertyInfo (classDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, false, typeof (string), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, false, typeof (string), true, null, storageClass);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, string propertyName, Type propertyType)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, false, propertyType, true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo (ClassDefinition classDefinition, bool isNullable)
    {
      return CreateForFakePropertyInfo (classDefinition, "Test", false, typeof (string), isNullable, null, StorageClass.Persistent);
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
      
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub.Stub (stub => stub.Name).Return (propertyName + "FakeProperty");
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (propertyType);
      propertyInformationStub.Stub (stub => stub.DeclaringType).Return (TypeAdapter.Create (declaringType));
      propertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (TypeAdapter.Create (declaringType));

      return CreateForPropertyInformation (classDefinition, propertyName, isObjectID, isNullable, maxLength, storageClass, propertyInformationStub);
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID ()
    {
      return CreateForFakePropertyInfo_ObjectID (ClassDefinitionObjectMother.CreateClassDefinition());
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition)
    {
      return CreateForFakePropertyInfo_ObjectID (classDefinition, "Test");
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition, string propertyName)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, true, typeof (ObjectID), true, null, StorageClass.Persistent);
    }

    public static PropertyDefinition CreateForFakePropertyInfo_ObjectID (ClassDefinition classDefinition, string propertyName, StorageClass storageClass)
    {
      return CreateForFakePropertyInfo (classDefinition, propertyName, true, typeof (ObjectID), true, null, storageClass);
    }

    public static PropertyDefinition CreateForRealPropertyInfo (ClassDefinition classDefinition, Type declaringClassType, string propertyName)
    {
      var propertyInfo = declaringClassType.GetProperty (
          propertyName, 
          BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '" + propertyName + "' on type '" + declaringClassType + "'.");

      var fullPropertyName = declaringClassType.FullName + "." + propertyName;

      var isObjectID = ReflectionUtility.IsDomainObject (propertyInfo.PropertyType);

      return CreateForPropertyInformation (
          classDefinition,
          fullPropertyName,
          isObjectID,
          true,
          null,
          StorageClass.Persistent,
          PropertyInfoAdapter.Create (propertyInfo));
    }

    public static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition,
        string propertyName,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation (
          classDefinition,
          propertyName,
          false,
          false,
          null,
          StorageClass.Persistent,
          propertyInformation);
    }

    public static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition,
        StorageClass storageClass,
        IPropertyInformation propertyInformation)
    {
      return CreateForPropertyInformation (
          classDefinition,
          "Test",
          false,
          false,
          null,
          storageClass,
          propertyInformation);
    }

    private static PropertyDefinition CreateForPropertyInformation (
        ClassDefinition classDefinition, 
        string propertyName,
        bool isObjectID,
        bool isNullable, 
        int? maxLength, 
        StorageClass storageClass,
        IPropertyInformation propertyInformation)
    {
      var propertyDefinition = new PropertyDefinition (
          classDefinition,
          propertyInformation,
          propertyName,
          isObjectID,
          isNullable,
          maxLength,
          storageClass);
      return propertyDefinition;
    }

    public static PropertyDefinition CreateForFakePropertyInfo_MaxLength (string propertyName, Type propertyType, int maxLength)
    {
      return CreateForFakePropertyInfo (ClassDefinitionObjectMother.CreateClassDefinition (), propertyName, false, propertyType, true, maxLength, StorageClass.Persistent);
    }
  }
}