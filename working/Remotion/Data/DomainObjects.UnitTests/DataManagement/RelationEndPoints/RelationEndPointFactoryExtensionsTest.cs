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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  // ReSharper disable InvokeAsExtensionMethod
  [TestFixture]
  public class RelationEndPointFactoryExtensionsTest : StandardMappingTest
  {
    private IRelationEndPointFactory _endPointFactoryMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _endPointFactoryMock = MockRepository.GenerateStrictMock<IRelationEndPointFactory>();
    }

    [Test]
    public void CreateVirtualEndPoint_One_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      var endPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint>();

      _endPointFactoryMock
          .Expect (mock => mock.CreateVirtualObjectEndPoint (endPointID))
          .Return (endPointStub);
      _endPointFactoryMock.Replay();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint (_endPointFactoryMock, endPointID, false);

      _endPointFactoryMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (endPointStub));
      endPointStub.AssertWasNotCalled (stub => stub.MarkDataComplete (Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void CreateVirtualEndPoint_One_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderTicket");
      var endPointStub = MockRepository.GenerateStub<IVirtualObjectEndPoint> ();

      _endPointFactoryMock
          .Expect (mock => mock.CreateVirtualObjectEndPoint (endPointID))
          .Return (endPointStub);
      _endPointFactoryMock.Replay ();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint (_endPointFactoryMock, endPointID, true);

      _endPointFactoryMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (endPointStub));
      endPointStub.AssertWasCalled (stub => stub.MarkDataComplete (null));
    }

    [Test]
    public void CreateVirtualEndPoint_Many_MarkDataCompleteFalse ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();

      _endPointFactoryMock
          .Expect (mock => mock.CreateCollectionEndPoint (endPointID))
          .Return (endPointStub);
      _endPointFactoryMock.Replay ();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint (_endPointFactoryMock, endPointID, false);

      _endPointFactoryMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (endPointStub));
      endPointStub.AssertWasNotCalled (stub => stub.MarkDataComplete (Arg<DomainObject[]>.Is.Anything));
    }

    [Test]
    public void CreateVirtualEndPoint_Many_MarkDataCompleteTrue ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "OrderItems");
      var endPointStub = MockRepository.GenerateStub<ICollectionEndPoint> ();

      _endPointFactoryMock
          .Expect (mock => mock.CreateCollectionEndPoint (endPointID))
          .Return (endPointStub);
      _endPointFactoryMock.Replay ();

      var result = RelationEndPointFactoryExtensions.CreateVirtualEndPoint (_endPointFactoryMock, endPointID, true);

      _endPointFactoryMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (endPointStub));
      endPointStub.AssertWasCalled (stub => stub.MarkDataComplete (Arg<DomainObject[]>.List.Equal (new DomainObject[0])));
    }

    [Test]
    public void CreateVirtualEndPoint_NonVirtualID ()
    {
      var endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");
      _endPointFactoryMock.Replay ();

      Assert.That (
          () => RelationEndPointFactoryExtensions.CreateVirtualEndPoint (_endPointFactoryMock, endPointID, true),
          Throws.ArgumentException.With.Message.EqualTo ("The RelationEndPointID must identify a virtual end-point.\r\nParameter name: endPointID"));
    }

    // ReSharper restore InvokeAsExtensionMethod
  }
}