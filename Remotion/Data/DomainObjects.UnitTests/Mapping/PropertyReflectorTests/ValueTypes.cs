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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class ValueTypes : BaseTest
  {
    [Test]
    public void GetMetadata_WithBasicType ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes>("BooleanProperty", DomainModelConstraintProviderStub.Object, PropertyDefaultValueProviderStub.Object);

      PropertyDefaultValueProviderStub
          .Setup(stub => stub.GetDefaultValue(propertyReflector.PropertyInfo, false))
          .Returns(false);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(actual.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithAllDataTypes.BooleanProperty"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(bool)));
      Assert.That(actual.IsNullable, Is.False);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(false));
    }

    [Test]
    public void GetMetadata_WithNullableBasicType ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes>("NaBooleanProperty", DomainModelConstraintProviderStub.Object, PropertyDefaultValueProviderStub.Object);

      PropertyDefaultValueProviderStub
          .Setup(stub => stub.GetDefaultValue(propertyReflector.PropertyInfo, true))
          .Returns(null);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(actual.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithAllDataTypes.NaBooleanProperty"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(bool?)));
      Assert.That(actual.IsNullable, Is.True);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.Null);
    }

    [Test]
    public void GetMetadata_WithEnumProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes>("EnumProperty", DomainModelConstraintProviderStub.Object, PropertyDefaultValueProviderStub.Object);

      PropertyDefaultValueProviderStub
          .Setup(stub => stub.GetDefaultValue(propertyReflector.PropertyInfo, false))
          .Returns(ClassWithAllDataTypes.EnumType.Value0);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(actual.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithAllDataTypes.EnumProperty"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(ClassWithAllDataTypes.EnumType)));
      Assert.That(actual.IsNullable, Is.False);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(ClassWithAllDataTypes.EnumType.Value0));
    }

    [Test]
    public void GetMetadata_WithExtensibleEnumProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes>(
          "ExtensibleEnumProperty", DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);

      DomainModelConstraintProviderStub
          .Setup(stub => stub.IsNullable(propertyReflector.PropertyInfo))
          .Returns(false);

      PropertyDefaultValueProviderStub
          .Setup(stub => stub.GetDefaultValue(propertyReflector.PropertyInfo, false))
          .Returns(Color.Values.Blue());

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(actual.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithAllDataTypes.ExtensibleEnumProperty"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(Color)));
      Assert.That(actual.IsNullable, Is.False);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(Color.Values.Blue()));
    }

    [Test]
    public void GetMetadata_WithOptionalRelationProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithGuidKey>(
          "ClassWithValidRelationsOptional", DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(actual.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(ObjectID)));
      Assert.That(actual.IsNullable, Is.True);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(null));
    }

    public object ObjectProperty
    {
      get { return null; }
      set { }
    }
  }
}
