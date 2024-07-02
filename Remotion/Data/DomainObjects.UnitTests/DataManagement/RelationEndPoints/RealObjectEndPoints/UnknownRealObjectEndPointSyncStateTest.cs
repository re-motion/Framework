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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.RealObjectEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.RealObjectEndPoints
{
  [TestFixture]
  public class UnknownRealObjectEndPointSyncStateTest : StandardMappingTest
  {

    private Mock<IVirtualEndPointProvider> _virtualEndPointProviderMock;
    private UnknownRealObjectEndPointSyncState _state;

    private RelationEndPointID _endPointID;
    private Mock<IRealObjectEndPoint> _endPointMock;
    private Mock<IVirtualEndPoint> _oppositeEndPointMock;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointProviderMock = new Mock<IVirtualEndPointProvider>(MockBehavior.Strict);
      _state = new UnknownRealObjectEndPointSyncState(_virtualEndPointProviderMock.Object);

      _endPointID = RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer");
      _endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      _endPointMock.Setup(stub => stub.ID).Returns(_endPointID);
      _endPointMock.Setup(stub => stub.Definition).Returns(_endPointID.Definition);
      _endPointMock.Setup(stub => stub.OppositeObjectID).Returns(DomainObjectIDs.Customer1);

      _oppositeEndPointMock = new Mock<IVirtualEndPoint>(MockBehavior.Strict);
    }

    [Test]
    public void IsSynchronized ()
    {
      var result = _state.IsSynchronized(_endPointMock.Object);
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Synchronize ()
    {
      var sequence = new VerifiableSequence();
      ExpectLoadOpposite(sequence);
      _endPointMock.InVerifiableSequence(sequence).Setup(mock => mock.Synchronize()).Verifiable();

      _state.Synchronize(_endPointMock.Object, _oppositeEndPointMock.Object);

      _virtualEndPointProviderMock.Verify();
      _endPointMock.Verify();
      _oppositeEndPointMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var fakeCommand = new Mock<IDataManagementCommand>();
      var sequence = new VerifiableSequence();
      ExpectLoadOpposite(sequence);
      _endPointMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateDeleteCommand()).Returns(fakeCommand.Object).Verifiable();

      var result = _state.CreateDeleteCommand(_endPointMock.Object, () => Assert.Fail("should not be called."));

      _virtualEndPointProviderMock.Verify();
      _endPointMock.Verify();
      _oppositeEndPointMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var newRelatedObject = DomainObjectMother.CreateFakeObject<Order>();
      var fakeCommand = new Mock<IDataManagementCommand>();
      var sequence = new VerifiableSequence();
      ExpectLoadOpposite(sequence);
      _endPointMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateSetCommand(newRelatedObject)).Returns(fakeCommand.Object).Verifiable();

      var result = _state.CreateSetCommand(_endPointMock.Object, newRelatedObject, id => Assert.Fail("should not be called."));

      _virtualEndPointProviderMock.Verify();
      _endPointMock.Verify();
      _oppositeEndPointMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    [Test]
    public void CreateSetCommand_Null ()
    {
      var fakeCommand = new Mock<IDataManagementCommand>();
      var sequence = new VerifiableSequence();
      ExpectLoadOpposite(sequence);
      _endPointMock.InVerifiableSequence(sequence).Setup(mock => mock.CreateSetCommand(null)).Returns(fakeCommand.Object).Verifiable();

      var result = _state.CreateSetCommand(_endPointMock.Object, null, id => Assert.Fail("should not be called."));

      _virtualEndPointProviderMock.Verify();
      _endPointMock.Verify();
      _oppositeEndPointMock.Verify();
      sequence.Verify();
      Assert.That(result, Is.SameAs(fakeCommand.Object));
    }

    private void ExpectLoadOpposite (VerifiableSequence sequence)
    {
      var oppositeID = RelationEndPointID.CreateOpposite(_endPointID.Definition, DomainObjectIDs.Customer1);
      _virtualEndPointProviderMock.InVerifiableSequence(sequence).Setup(mock => mock.GetOrCreateVirtualEndPoint(oppositeID)).Returns(_oppositeEndPointMock.Object).Verifiable();
      _oppositeEndPointMock.InVerifiableSequence(sequence).Setup(mock => mock.EnsureDataComplete()).Verifiable();
    }
  }
}
