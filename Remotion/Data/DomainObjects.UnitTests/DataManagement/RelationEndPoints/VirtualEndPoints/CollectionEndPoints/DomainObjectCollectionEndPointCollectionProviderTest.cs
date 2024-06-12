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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  [TestFixture]
  public class DomainObjectCollectionEndPointCollectionProviderTest : StandardMappingTest
  {
    private Mock<IAssociatedDomainObjectCollectionDataStrategyFactory> _associatedDomainObjectCollectionDataStrategyFactoryMock;
    private RelationEndPointID _endPointID;

    private DomainObjectCollectionEndPointCollectionProvider _provider;

    private Mock<IDomainObjectCollectionData> _dataStrategyStub;

    public override void SetUp ()
    {
      base.SetUp();

      _associatedDomainObjectCollectionDataStrategyFactoryMock = new Mock<IAssociatedDomainObjectCollectionDataStrategyFactory>(MockBehavior.Strict);

      _provider = new DomainObjectCollectionEndPointCollectionProvider(_associatedDomainObjectCollectionDataStrategyFactoryMock.Object);

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Customer1, typeof(Customer), "Orders");

      _dataStrategyStub = new Mock<IDomainObjectCollectionData>();
      _dataStrategyStub.Setup(stub => stub.RequiredItemType).Returns(typeof(Order));
    }

    [Test]
    public void GetCollection ()
    {
      _associatedDomainObjectCollectionDataStrategyFactoryMock
          .Setup(mock => mock.CreateDataStrategyForEndPoint(_endPointID))
          .Returns(_dataStrategyStub.Object)
          .Verifiable();

      _dataStrategyStub.Setup(stub => stub.AssociatedEndPointID).Returns(_endPointID);

      var result = _provider.GetCollection(_endPointID);

      _associatedDomainObjectCollectionDataStrategyFactoryMock.Verify();
      Assert.That(result, Is.TypeOf<OrderCollection>());
      Assert.That(DomainObjectCollectionDataTestHelper.GetDataStrategy(result), Is.SameAs(_dataStrategyStub.Object));

      Assert.That(result, Is.SameAs(_provider.GetCollection(_endPointID)));
    }

    [Test]
    public void GetCollection_CollectionWithWrongCtor ()
    {
      var classDefinition = GetTypeDefinition(typeof(DomainObjectWithCollectionMissingCtor));
      var relationEndPointDefinition = GetEndPointDefinition(typeof(DomainObjectWithCollectionMissingCtor), "OppositeObjects");
      var endPointID = RelationEndPointID.Create(new ObjectID(classDefinition, Guid.NewGuid()), relationEndPointDefinition);

      _associatedDomainObjectCollectionDataStrategyFactoryMock
          .Setup(mock => mock.CreateDataStrategyForEndPoint(endPointID))
          .Returns(_dataStrategyStub.Object);

      Assert.That(() => _provider.GetCollection(endPointID), Throws.TypeOf<MissingMethodException>()
          .With.Message.Contains("does not provide a constructor taking an IDomainObjectCollectionData object"));
    }

    [Test]
    public void GetCollection_WithAnonymousEndPoint ()
    {
      var relationEndPointDefinition = GetEndPointDefinition(typeof(Location), "Client").GetOppositeEndPointDefinition();
      var endPointID = RelationEndPointID.Create(null, relationEndPointDefinition);
      Assert.That(endPointID.Definition.IsAnonymous, Is.True);

      Assert.That(
          () => _provider.GetCollection(endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("End point ID must not refer to an anonymous end point.", "endPointID"));
    }

    [Test]
    public void RegisterCollection ()
    {
      _dataStrategyStub.Setup(stub => stub.AssociatedEndPointID).Returns(_endPointID);
      var collection = new DomainObjectCollection(_dataStrategyStub.Object);

      _provider.RegisterCollection(_endPointID, collection);

      Assert.That(_provider.GetCollection(_endPointID), Is.SameAs(collection));
    }

    [Test]
    public void RegisterCollection_OverwritesFormerCollection ()
    {
      _dataStrategyStub.Setup(stub => stub.AssociatedEndPointID).Returns(_endPointID);
      _associatedDomainObjectCollectionDataStrategyFactoryMock
          .Setup(mock => mock.CreateDataStrategyForEndPoint(_endPointID))
          .Returns(_dataStrategyStub.Object)
          .Verifiable();

      var collection1 = _provider.GetCollection(_endPointID);
      var collection2 = new DomainObjectCollection(_dataStrategyStub.Object);

      Assert.That(_provider.GetCollection(_endPointID), Is.SameAs(collection1));

      _provider.RegisterCollection(_endPointID, collection2);

      Assert.That(_provider.GetCollection(_endPointID), Is.SameAs(collection2));

      var collection3 = new DomainObjectCollection(_dataStrategyStub.Object);
      _provider.RegisterCollection(_endPointID, collection3);

      Assert.That(_provider.GetCollection(_endPointID), Is.SameAs(collection3));
    }

    [Test]
    public void RegisterCollection_NonAssociatedCollection ()
    {
      _dataStrategyStub.Setup(stub => stub.AssociatedEndPointID).Returns((RelationEndPointID)null);
      var collection = new DomainObjectCollection(_dataStrategyStub.Object);
      Assert.That(
          () => _provider.RegisterCollection(_endPointID, collection),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The collection must be associated with the given endPointID.", "collection"));
    }

    [Test]
    public void RegisterCollection_CollectionAssociatedToDifferentEndPoint ()
    {
      _dataStrategyStub.Setup(stub => stub.AssociatedEndPointID).Returns(RelationEndPointID.Create(DomainObjectIDs.Customer2, typeof(Customer), "Orders"));
      var collection = new DomainObjectCollection(_dataStrategyStub.Object);

      Assert.That(
          () => _provider.RegisterCollection(_endPointID, collection),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The collection must be associated with the given endPointID.", "collection"));
    }
  }
}
