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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class StringProperty : BaseTest
  {
    [Test]
    public void GetMetadata_WithNullableFalse ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties>("NoAttribute", DomainModelConstraintProviderStub.Object, PropertyDefaultValueProviderStub.Object);

      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(propertyReflector.PropertyInfo))
          .Returns(false);

      PropertyDefaultValueProviderStub
          .Setup(stub => stub.GetDefaultValue(propertyReflector.PropertyInfo, false))
          .Returns(string.Empty);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(
          actual.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NoAttribute"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(string)));
      Assert.That(actual.IsNullable, Is.False);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(string.Empty));
    }

    [Test]
    public void GetMetadata_WithNullableTrue ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties>(
          "NullableFromAttribute",
          DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);

      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(propertyReflector.PropertyInfo))
          .Returns(true);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(
          actual.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NullableFromAttribute"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(string)));
      Assert.That(actual.IsNullable, Is.True);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(null));
    }

    [Test]
    public void GetMetadata_WithMaximumLength ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties>("MaximumLength", DomainModelConstraintProviderStub.Object, PropertyDefaultValueProviderStub.Object);

      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(propertyReflector.PropertyInfo))
          .Returns(true);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.GetMaxLength(propertyReflector.PropertyInfo))
          .Returns(100);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(
          actual.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.MaximumLength"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(string)));
      Assert.That(actual.IsNullable, Is.True);
      Assert.That(actual.MaxLength, Is.EqualTo(100));
      Assert.That(actual.DefaultValue, Is.EqualTo(null));
    }

    [Test]
    public void GetMetadata_WithNotNullableAndMaximumLength ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithStringProperties>(
          "NotNullableAndMaximumLength",
          DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);

      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(propertyReflector.PropertyInfo))
          .Returns(false);
      DomainModelConstraintProviderStub
          .Setup(stub => stub.GetMaxLength(propertyReflector.PropertyInfo))
          .Returns(100);

      PropertyDefaultValueProviderStub
          .Setup(stub => stub.GetDefaultValue(propertyReflector.PropertyInfo, false))
          .Returns(string.Empty);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(
          actual.PropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithStringProperties.NotNullableAndMaximumLength"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(string)));
      Assert.That(actual.IsNullable, Is.False);
      Assert.That(actual.MaxLength, Is.EqualTo(100));
      Assert.That(actual.DefaultValue, Is.EqualTo(string.Empty));
    }

    [StringProperty]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}
