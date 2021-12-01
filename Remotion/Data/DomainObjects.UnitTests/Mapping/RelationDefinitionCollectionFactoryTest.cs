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
using System.Linq;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class RelationDefinitionCollectionFactoryTest : StandardMappingTest
  {
    private RelationDefinitionCollectionFactory _factory;
    private Mock<IMappingObjectFactory> _mappingObjectFactoryMock;
    private ClassDefinition _orderClassDefinition;
    private ClassDefinition _orderItemClassDefinition;
    private RelationDefinition _fakeRelationDefinition1;
    private RelationDefinition _fakeRelationDefinition2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _mappingObjectFactoryMock = new Mock<IMappingObjectFactory> (MockBehavior.Strict);
      _factory = new RelationDefinitionCollectionFactory(_mappingObjectFactoryMock.Object);
      _orderClassDefinition = MappingConfiguration.Current.GetClassDefinition("Order");
      _orderItemClassDefinition = MappingConfiguration.Current.GetClassDefinition("OrderItem");
      _fakeRelationDefinition1 = new RelationDefinition(
          "Fake1",
          new AnonymousRelationEndPointDefinition(_orderClassDefinition),
          new AnonymousRelationEndPointDefinition(_orderItemClassDefinition));
      _fakeRelationDefinition2 = new RelationDefinition(
          "Fake2",
          new AnonymousRelationEndPointDefinition(_orderItemClassDefinition),
          new AnonymousRelationEndPointDefinition(_orderClassDefinition));
    }

    [Test]
    public void CreateRelationDefinitionCollection_OneClassDefinitionWithOneEndPoint ()
    {
      var classDefinitions = new[] { _orderItemClassDefinition }.ToDictionary(cd => cd.ClassType);

      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderItemClassDefinition,
                   _orderItemClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition1)
          .Verifiable();

      var result = _factory.CreateRelationDefinitionCollection(classDefinitions);

      _mappingObjectFactoryMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeRelationDefinition1 }));
    }

    [Test]
    public void CreateRelationDefinitionCollection_OneClassDefinitionWithSeveralEndPoints_DuplicatedRelationDefinitionsGetFiltered ()
    {
      var classDefinitions = new[] { _orderClassDefinition }.ToDictionary(cd => cd.ClassType);

      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition1)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition1)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition2)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition2)
          .Verifiable();
      _mappingObjectFactoryMock.Object.Replay();

      var result = _factory.CreateRelationDefinitionCollection(classDefinitions);

      _mappingObjectFactoryMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeRelationDefinition1, _fakeRelationDefinition2 }));
    }

    [Test]
    public void CreateRelationDefinitionCollection_SeveralClassDefinitionWithSeveralEndPoints_DuplicatedRelationDefinitionsGetFiltered ()
    {
      var classDefinitions = new[] { _orderClassDefinition, _orderItemClassDefinition }.ToDictionary(cd => cd.ClassType);

      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition1)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition1)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition2)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderClassDefinition,
                   _orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition2)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(
              mock =>
              mock.CreateRelationDefinition(
                  classDefinitions,
                  _orderItemClassDefinition,
                   _orderItemClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order"]
                       .PropertyInfo))
          .Returns(_fakeRelationDefinition1)
          .Verifiable();

      var result = _factory.CreateRelationDefinitionCollection(classDefinitions);

      _mappingObjectFactoryMock.Verify();
      Assert.That(result, Is.EqualTo(new[] { _fakeRelationDefinition1, _fakeRelationDefinition2 }));
    }
  }
}
