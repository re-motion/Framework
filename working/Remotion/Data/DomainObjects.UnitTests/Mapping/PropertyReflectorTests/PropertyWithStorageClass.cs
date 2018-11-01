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
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class PropertyWithStorageClass : BaseTest
  {
    [Test]
    public void StorageClass_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "NoAttribute", DomainModelConstraintProviderStub);
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.Persistent));
    }

    [Test]
    public void StorageClass_WithPersistentAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "Persistent", DomainModelConstraintProviderStub);
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.Persistent));
    }

    [Test]
    public void StorageClass_WithTransactionAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "Transaction", DomainModelConstraintProviderStub);
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.Transaction));
    }

    [Test]
    public void StorageClass_WithNoneAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "None", DomainModelConstraintProviderStub);
      Assert.That (propertyReflector.StorageClass, Is.EqualTo (StorageClass.None));
    }

    [Test]
    public void GetMetadata_WithNoAttribute ()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "NoAttribute", DomainModelConstraintProviderStub);

      var actual = propertyReflector.GetMetadata();
      actual.SetStorageProperty (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("NoAttribute"));

      Assert.That (actual.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.NoAttribute"));
      Assert.That (actual.StorageClass, Is.EqualTo (StorageClass.Persistent));
    }

    [Test]
    public void GetMetadata_WithStorageClassPersistent ()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "Persistent", DomainModelConstraintProviderStub);

      var actual = propertyReflector.GetMetadata();
      actual.SetStorageProperty (SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty("Persistent"));

      Assert.That (actual.PropertyName, Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithPropertiesHavingStorageClassAttribute.Persistent"));
      Assert.That (actual.StorageClass, Is.EqualTo (StorageClass.Persistent));
    }

    [Test]
    public void GetMetadata_WithStorageClassTransaction_DoesntThrow ()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "Transaction", DomainModelConstraintProviderStub);
      
      propertyReflector.GetMetadata();
    }

    [Test]
    public void GetMetadata_WithStorageClassTransaction_SetsStorageClass ()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "Transaction", DomainModelConstraintProviderStub);

      var propertyDefinition = propertyReflector.GetMetadata();
      Assert.That (propertyDefinition.StorageClass, Is.EqualTo (StorageClass.Transaction));
    }

    [Test]
    public void GetMetadata_WithStorageClassTransaction_NonPersistableDataType ()
    {
      var propertyReflector = CreatePropertyReflector<ClassWithPropertiesHavingStorageClassAttribute> (
          "TransactionWithDateTimeDataType", DomainModelConstraintProviderStub);

      var propertyDefinition = propertyReflector.GetMetadata();
      Assert.That (propertyDefinition.StorageClass, Is.EqualTo (StorageClass.Transaction));
      Assert.That (propertyDefinition.PropertyType, Is.EqualTo (typeof (DateTime)));
    }
  }
}