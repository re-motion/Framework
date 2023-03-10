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
  public class RelationProperty : BaseTest
  {
    [Test]
    public void GetMetadata_WithNoAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithRealRelationEndPoints>(
          "NoAttributeForDomainObjectCollection",
          DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(
          actual.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NoAttributeForDomainObjectCollection"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(ObjectID)));
      Assert.That(actual.IsNullable, Is.True);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(null));
    }

    [Test]
    public void GetMetadata_WithNotNullableFromAttribute ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithRealRelationEndPoints>(
          "NotNullableForVirtualCollection",
          DomainModelConstraintProviderStub.Object,
          PropertyDefaultValueProviderStub.Object);

      PropertyDefinition actual = propertyReflector.GetMetadata();

      Assert.That(
          actual.PropertyName,
          Is.EqualTo(
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithRealRelationEndPoints.NotNullableForVirtualCollection"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(ObjectID)));
      Assert.That(actual.IsNullable, Is.True);
      Assert.That(actual.MaxLength, Is.Null);
      Assert.That(actual.DefaultValue, Is.EqualTo(null));
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}
