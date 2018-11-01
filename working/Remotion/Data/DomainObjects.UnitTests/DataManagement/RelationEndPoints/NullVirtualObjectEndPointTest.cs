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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class NullVirtualObjectEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullVirtualObjectEndPoint _nullEndPoint;
    private IRealObjectEndPoint _oppositeEndPointStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _definition = DomainObjectIDs.Order1.ClassDefinition.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      _nullEndPoint = new NullVirtualObjectEndPoint(TestableClientTransaction, _definition);

      _oppositeEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint>();
    }

    [Test]
    public void CanBeCollected ()
    {
      Assert.That (_nullEndPoint.CanBeCollected, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete ()
    {
      Assert.That (_nullEndPoint.CanBeMarkedIncomplete, Is.False);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "A NullObjectEndPoint cannot be used to synchronize an opposite end-point.")]
    public void SynchronizeOppositeEndPoint ()
    {
      var objectEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();

      _nullEndPoint.SynchronizeOppositeEndPoint (objectEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void MarkDataIncomplete ()
    {
      _nullEndPoint.MarkDataIncomplete();
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var relatedEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      relatedEndPointMock.Expect (mock => mock.MarkSynchronized ());
      relatedEndPointMock.Replay ();

      _nullEndPoint.RegisterOriginalOppositeEndPoint (relatedEndPointMock);

      relatedEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var relatedEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      relatedEndPointMock.Expect (mock => mock.ResetSyncState ());
      relatedEndPointMock.Replay ();

      _nullEndPoint.UnregisterOriginalOppositeEndPoint (relatedEndPointMock);

      relatedEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.RegisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.UnregisterCurrentOppositeEndPoint (_oppositeEndPointStub);
    }
  }
}