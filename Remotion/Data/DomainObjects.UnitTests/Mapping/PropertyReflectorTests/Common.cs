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
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.PropertyReflectorTests
{
  [TestFixture]
  public class Common : BaseTest
  {
    [Test]
    public void GetMetadata_ForSingleProperty ()
    {
      PropertyReflector propertyReflector = CreatePropertyReflector<ClassWithAllDataTypes>("BooleanProperty", DomainModelConstraintProviderStub.Object, PropertyDefaultValueProviderStub.Object);

      PropertyDefinition actual = propertyReflector.GetMetadata();
      actual.SetStorageProperty(SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty());

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual.PropertyName, Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.ClassWithAllDataTypes.BooleanProperty"));
      Assert.That(actual.PropertyType, Is.SameAs(typeof(bool)));
    }
  }
}
