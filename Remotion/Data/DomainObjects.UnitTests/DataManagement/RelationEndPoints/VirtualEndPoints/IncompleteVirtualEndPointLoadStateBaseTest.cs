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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints.VirtualEndPoints
{
  [TestFixture]
  public class IncompleteVirtualEndPointLoadStateBaseTest : StandardMappingTest
  {
    private Mock<IVirtualEndPoint<object>> _virtualEndPointMock;
    private Mock<TestableIncompleteVirtualEndPointLoadState.IEndPointLoader> _endPointLoaderMock;

    private TestableIncompleteVirtualEndPointLoadState _loadState;

    private Mock<IRealObjectEndPoint> _relatedEndPointStub1;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub2;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _virtualEndPointMock = new Mock<IVirtualEndPoint<object>>(MockBehavior.Strict);
      _endPointLoaderMock = new Mock<TestableIncompleteVirtualEndPointLoadState.IEndPointLoader>(MockBehavior.Strict);

      _loadState = new TestableIncompleteVirtualEndPointLoadState(_endPointLoaderMock.Object);

      _relatedEndPointStub1 = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub1.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      _relatedEndPointStub1.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order1, typeof(Order), "Customer"));

      _relatedEndPointStub2 = new Mock<IRealObjectEndPoint>();
      _relatedEndPointStub2.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order3);
      _relatedEndPointStub2.Setup(stub => stub.ID).Returns(RelationEndPointID.Create(DomainObjectIDs.Order3, typeof(Order), "Customer"));
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That(_loadState.IsDataComplete(), Is.False);
    }

    [Test]
    public void CanDataBeMarkedIncomplete ()
    {
      Assert.That(_loadState.CanDataBeMarkedIncomplete(_virtualEndPointMock.Object), Is.True);
    }

    [Test]
    public void MarkDataIncomplete_DoesNothing ()
    {
      _loadState.MarkDataIncomplete(_virtualEndPointMock.Object, () => Assert.Fail("Must not be called."));
    }

    [Test]
    public void CanEndPointBeCollected_False ()
    {
      _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub1.Object);

      var result = _loadState.CanEndPointBeCollected(_virtualEndPointMock.Object);

      Assert.That(result, Is.False);
    }

    [Test]
    public void CanEndPointBeCollected_True ()
    {
      var result = _loadState.CanEndPointBeCollected(_virtualEndPointMock.Object);
      Assert.That(result, Is.True);
    }

    [Test]
    public void GetData ()
    {
      CheckOperationDelegatesToCompleteState(s => s.GetData(_virtualEndPointMock.Object), new object());
    }

    [Test]
    public void GetOriginalData ()
    {
      CheckOperationDelegatesToCompleteState(s => s.GetOriginalData(_virtualEndPointMock.Object), new object());
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      Assert.That(_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo(0));

      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      endPointMock.Setup(mock => mock.ResetSyncState()).Verifiable();

      _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);

      Assert.That(_loadState.OriginalOppositeEndPoints.ToArray(), Is.EqualTo(new[] { endPointMock.Object }));
      endPointMock.Verify();
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint_WithNullOppositeEndPoint_Throws ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>();
      endPointMock.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_RegisteredInDataManager ()
    {
      Assert.That(_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo(0));
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      endPointMock.Setup(mock => mock.ResetSyncState()).Verifiable();

      _loadState.RegisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);
      Assert.That(_loadState.OriginalOppositeEndPoints.ToArray(), Is.EqualTo(new[] { endPointMock.Object }));

      _loadState.UnregisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object);

      Assert.That(_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo(0));
      endPointMock.Verify();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_ThrowsIfNotRegistered ()
    {
      Assert.That(_loadState.OriginalOppositeEndPoints.Count, Is.EqualTo(0));
      var endPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      endPointMock.Setup(stub => stub.IsNull).Returns(false);
      endPointMock.Setup(stub => stub.ObjectID).Returns(DomainObjectIDs.Order1);
      Assert.That(
          () => _loadState.UnregisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The opposite end-point has not been registered."));
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint_WithNullOppositeEndPoint_Throws ()
    {
      var endPointMock = new Mock<IRealObjectEndPoint>();
      endPointMock.Setup(stub => stub.IsNull).Returns(true);

      Assert.That(
          () => _loadState.UnregisterOriginalOppositeEndPoint(_virtualEndPointMock.Object, endPointMock.Object),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("End point must not be a null object.", "oppositeEndPoint"));
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState(s => s.RegisterCurrentOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub1.Object));
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      CheckOperationDelegatesToCompleteState(s => s.UnregisterCurrentOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub1.Object));
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That(_loadState.IsSynchronized(_virtualEndPointMock.Object), Is.Null);
    }

    [Test]
    public void Synchronize ()
    {
      CheckOperationDelegatesToCompleteState(s => s.Synchronize(_virtualEndPointMock.Object));
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      Assert.That(
          () => _loadState.SynchronizeOppositeEndPoint(_virtualEndPointMock.Object, _relatedEndPointStub1.Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot synchronize an opposite end-point with a virtual end-point in incomplete state."));
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      Assert.That(
          () => _loadState.SetDataFromSubTransaction(
          _virtualEndPointMock.Object,
          new Mock<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>>().Object),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot comit data from a sub-transaction into a virtual end-point in incomplete state."));
    }

    [Test]
    public void HasChanged ()
    {
      var result = _loadState.HasChanged();

      Assert.That(result, Is.False);
    }

    [Test]
    public void Commit ()
    {
      _loadState.Commit(_virtualEndPointMock.Object);
    }

    [Test]
    public void Rollback ()
    {
      _loadState.Rollback(_virtualEndPointMock.Object);
    }

    private void CheckOperationDelegatesToCompleteState (
        Expression<Action<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>>> operation)
    {
      var newStateMock = new Mock<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>>(MockBehavior.Strict);
      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_virtualEndPointMock.Object))
          .Returns(newStateMock.Object)
          .Verifiable();

      newStateMock.Setup(operation).Verifiable();

      var compiledOperation = operation.Compile();
      compiledOperation(_loadState);

      newStateMock.Verify();
    }

    private void CheckOperationDelegatesToCompleteState<T> (
        Expression<Func<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>, T>> operation,
        T fakeResult)
    {
      var newStateMock = new Mock<IVirtualEndPointLoadState<IVirtualEndPoint<object>, object, IVirtualEndPointDataManager>>(MockBehavior.Strict);
      _endPointLoaderMock
          .Setup(mock => mock.LoadEndPointAndGetNewState(_virtualEndPointMock.Object))
          .Returns(newStateMock.Object)
          .Verifiable();

      newStateMock.Setup(operation).Returns(fakeResult).Verifiable();

      var compiledOperation = operation.Compile();
      var result = compiledOperation(_loadState);

      newStateMock.Verify();
      Assert.That(result, Is.EqualTo(fakeResult));
    }
  }
}
