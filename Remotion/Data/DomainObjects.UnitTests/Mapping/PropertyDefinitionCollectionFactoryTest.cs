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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PropertyDefinitionCollectionFactoryTest
  {
    private PropertyDefinitionCollectionFactory _factory;
    private Mock<IMappingObjectFactory> _mappingObjectFactoryMock;

    [SetUp]
    public void SetUp ()
    {
      _mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);
      _factory = new PropertyDefinitionCollectionFactory(_mappingObjectFactoryMock.Object);
    }

    [Test]
    public void CreatePropertyDefinitions ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);

      var propertyInfo1 = PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderNumber"));
      var propertyInfo2 = PropertyInfoAdapter.Create(typeof(Order).GetProperty("DeliveryDate"));

      var fakePropertyDefinition1 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(typeDefinition, "P1");
      var fakePropertyDefinition2 = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(typeDefinition, "P2");

      _mappingObjectFactoryMock
          .Setup(mock => mock.CreatePropertyDefinition(typeDefinition, propertyInfo1))
          .Returns(fakePropertyDefinition1)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreatePropertyDefinition(typeDefinition, propertyInfo2))
          .Returns(fakePropertyDefinition2)
          .Verifiable();

      var result = _factory.CreatePropertyDefinitions(typeDefinition, new[] { propertyInfo1, propertyInfo2 });

      _mappingObjectFactoryMock.Verify();
      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0], Is.SameAs(fakePropertyDefinition1));
      Assert.That(result[1], Is.SameAs(fakePropertyDefinition2));
    }
  }
}
