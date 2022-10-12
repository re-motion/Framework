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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  // ReSharper disable InvokeAsExtensionMethod
  [TestFixture]
  public class RelationEndPointFactoryExtensionsTest : StandardMappingTest
  {
    private Mock<IRelationEndPointFactory> _endPointFactoryMock;

    public override void SetUp ()
    {
      base.SetUp();

      _endPointFactoryMock = new Mock<IRelationEndPointFactory>(MockBehavior.Strict);
    }

    [Test]
    public void CreateVirtualEndPoint_One_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      var endPointStub = new Mock<IVirtualObjectEndPoint>();

      _endPointFactoryMock
          .Setup(mock => mock.CreateVirtualObjectEndPoint(endPointID))
          .Returns(endPointStub.Object)
          .Verifiable();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, false);

      _endPointFactoryMock.Verify();

      Assert.That(result, Is.SameAs(endPointStub.Object));
      endPointStub.Verify(stub => stub.MarkDataComplete(It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void CreateVirtualEndPoint_One_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderTicket");
      var endPointStub = new Mock<IVirtualObjectEndPoint>();

      _endPointFactoryMock
          .Setup(mock => mock.CreateVirtualObjectEndPoint(endPointID))
          .Returns(endPointStub.Object)
          .Verifiable();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, true);

      _endPointFactoryMock.Verify();

      Assert.That(result, Is.SameAs(endPointStub.Object));
      endPointStub.Verify(stub => stub.MarkDataComplete(null), Times.AtLeastOnce());
    }

    [Test]
    public void CreateVirtualEndPoint_DomainObjectCollection_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPointStub = new Mock<IDomainObjectCollectionEndPoint>();

      _endPointFactoryMock
          .Setup(mock => mock.CreateDomainObjectCollectionEndPoint(endPointID))
          .Returns(endPointStub.Object)
          .Verifiable();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, false);

      _endPointFactoryMock.Verify();

      Assert.That(result, Is.SameAs(endPointStub.Object));
      endPointStub.Verify(stub => stub.MarkDataComplete(It.IsAny<DomainObject[]>()), Times.Never());
    }

    [Test]
    public void CreateVirtualEndPoint_DomainObjectCollection_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "OrderItems");
      var endPointStub = new Mock<IDomainObjectCollectionEndPoint>();

      _endPointFactoryMock
          .Setup(mock => mock.CreateDomainObjectCollectionEndPoint(endPointID))
          .Returns(endPointStub.Object)
          .Verifiable();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, true);

      _endPointFactoryMock.Verify();

      Assert.That(result, Is.SameAs(endPointStub.Object));
      endPointStub.Verify(stub => stub.MarkDataComplete(new DomainObject[0]), Times.AtLeastOnce());
    }

    [Test]
    public void CreateVirtualEndPoint_VirtualCollection_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Product1, typeof(Product), "Reviews");
      var endPointStub = new Mock<IVirtualCollectionEndPoint>();

      _endPointFactoryMock
          .Setup(mock => mock.CreateVirtualCollectionEndPoint(endPointID))
          .Returns(endPointStub.Object)
          .Verifiable();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, false);

      _endPointFactoryMock.Verify();

      Assert.That(result, Is.SameAs(endPointStub.Object));
      endPointStub.Verify(stub => stub.MarkDataComplete(It.IsAny<DomainObject[]>()), Times.Never());
    }

    [Test]
    public void CreateVirtualEndPoint_VirtualCollection_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Product1, typeof(Product), "Reviews");
      var endPointStub = new Mock<IVirtualCollectionEndPoint>();

      _endPointFactoryMock
          .Setup(mock => mock.CreateVirtualCollectionEndPoint(endPointID))
          .Returns(endPointStub.Object)
          .Verifiable();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, true);

      _endPointFactoryMock.Verify();

      Assert.That(result, Is.SameAs(endPointStub.Object));
      endPointStub.Verify(stub => stub.MarkDataComplete(new DomainObject[0]), Times.AtLeastOnce());
    }

    [Test]
    public void CreateVirtualEndPoint_NonVirtualID ()
    {
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");

      Assert.That(
          () => RelationEndPointFactoryExtensions.CreateVirtualEndPoint(_endPointFactoryMock.Object, endPointID, true),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The RelationEndPointID must identify a virtual end-point.", "endPointID"));
    }

    // ReSharper restore InvokeAsExtensionMethod
  }
}
