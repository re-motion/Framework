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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class RelationEndPointDefinitionCollectionFactoryTest
  {
    private RelationEndPointDefinitionCollectionFactory _factory;
    private Mock<IMappingObjectFactory> _mappingObjectFactoryMock;
    private Mock<IMemberInformationNameResolver> _memberInformationNameResolverMock;

    [SetUp]
    public void SetUp ()
    {
      _mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);
      _memberInformationNameResolverMock = new Mock<IMemberInformationNameResolver>(MockBehavior.Strict);
      _factory = new RelationEndPointDefinitionCollectionFactory(
          _mappingObjectFactoryMock.Object,
          _memberInformationNameResolverMock.Object,
          new PropertyMetadataReflector());
    }

    [Test]
    public void CreateRelationEndPointDefinitionCollection ()
    {
      var typeDefinition = TypeDefinitionObjectMother.CreateClassDefinition(classType: typeof(OrderTicket));
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID(typeDefinition);
      typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      var fakeRelationEndPoint = new RelationEndPointDefinition(propertyDefinition, false);

      var expectedPropertyInfo = PropertyInfoAdapter.Create(typeof(OrderTicket).GetProperty("Order"));

      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateRelationEndPointDefinition(typeDefinition, PropertyInfoAdapter.Create(expectedPropertyInfo.PropertyInfo)))
          .Returns(fakeRelationEndPoint)
          .Verifiable();

      var result = _factory.CreateRelationEndPointDefinitionCollection(typeDefinition);

      _mappingObjectFactoryMock.Verify();
      _memberInformationNameResolverMock.Verify();
      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0], Is.SameAs(fakeRelationEndPoint));
    }
  }
}
