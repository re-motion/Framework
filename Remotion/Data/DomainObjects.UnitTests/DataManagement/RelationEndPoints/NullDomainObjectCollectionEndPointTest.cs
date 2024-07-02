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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class NullDomainObjectCollectionEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullDomainObjectCollectionEndPoint _nullEndPoint;
    private OrderItem _relatedObject;
    private Mock<IRealObjectEndPoint> _relatedEndPointStub;

    public override void SetUp ()
    {
      base.SetUp();
      _definition = MappingConfiguration.Current.GetTypeDefinition(typeof(Order))
          .GetRelationEndPointDefinition(typeof(Order).FullName + ".OrderItems");
      _nullEndPoint = new NullDomainObjectCollectionEndPoint(TestableClientTransaction, _definition);
      _relatedObject = OrderItem.NewObject();
      _relatedEndPointStub = new Mock<IRealObjectEndPoint>();
    }

    [Test]
    public void Definition ()
    {
      Assert.That(_nullEndPoint.Definition, Is.SameAs(_definition));
    }

    [Test]
    public void ObjectID ()
    {
      Assert.That(_nullEndPoint.ObjectID, Is.Null);
    }

    [Test]
    public void ID ()
    {
      var id = _nullEndPoint.ID;
      Assert.That(id.Definition, Is.SameAs(_definition));
      Assert.That(id.ObjectID, Is.Null);
    }

    [Test]
    public void Collection_Get ()
    {
      Assert.That(_nullEndPoint.Collection, Is.Empty);
    }

    [Test]
    public void OriginalCollection ()
    {
      Assert.That(
          () => _nullEndPoint.OriginalCollection,
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetData ()
    {
      Assert.That(
          () => _nullEndPoint.GetData(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetOriginalData ()
    {
      Assert.That(
          () => _nullEndPoint.GetOriginalData(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetCollectionEventRaiser ()
    {
      Assert.That(
          () => _nullEndPoint.GetCollectionEventRaiser(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetCollectionWithOriginalData ()
    {
      Assert.That(
          () => _nullEndPoint.GetCollectionWithOriginalData(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That(_nullEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void CanBeCollected ()
    {
      Assert.That(_nullEndPoint.CanBeCollected, Is.False);
    }

    [Test]
    public void CanBeMarkedIncomplete ()
    {
      Assert.That(_nullEndPoint.CanBeMarkedIncomplete, Is.False);
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That(_nullEndPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasChangedFast ()
    {
      Assert.That(_nullEndPoint.HasChangedFast, Is.False);
    }

    [Test]
    public void HasBeenTouched ()
    {
      Assert.That(_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void GetDomainObject_Null ()
    {
      Assert.That(_nullEndPoint.GetDomainObject(), Is.Null);
    }

    [Test]
    public void GetDomainObjectReference_Null ()
    {
      Assert.That(_nullEndPoint.GetDomainObjectReference(), Is.Null);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(_nullEndPoint.IsNull, Is.True);
    }

    [Test]
    public void MarkDataComplete ()
    {
      _nullEndPoint.MarkDataComplete(new DomainObject[0]);
    }

    [Test]
    public void MarkDataIncomplete ()
    {
      Assert.That(
          () => _nullEndPoint.MarkDataIncomplete(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void Touch ()
    {
      Assert.That(_nullEndPoint.HasBeenTouched, Is.False);
      _nullEndPoint.Touch();
      Assert.That(_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      Assert.That(_nullEndPoint.CreateSetCollectionCommand(new DomainObjectCollection()), Is.InstanceOf(typeof(NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      Assert.That(_nullEndPoint.CreateInsertCommand(_relatedObject, 12), Is.InstanceOf(typeof(NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateAddCommand ()
    {
      Assert.That(_nullEndPoint.CreateAddCommand(_relatedObject), Is.InstanceOf(typeof(NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      Assert.That(_nullEndPoint.CreateRemoveCommand(_relatedObject), Is.InstanceOf(typeof(NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      Assert.That(_nullEndPoint.CreateReplaceCommand(12, _relatedObject), Is.InstanceOf(typeof(NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      Assert.That(_nullEndPoint.CreateDeleteCommand(), Is.InstanceOf(typeof(NullEndPointModificationCommand)));
    }

    [Test]
    public void SortCurrentData ()
    {
      Assert.That(
          () => _nullEndPoint.SortCurrentData((one, two) => 0),
          Throws.InvalidOperationException);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var relatedEndPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      relatedEndPointMock.Setup(mock => mock.MarkSynchronized()).Verifiable();

      _nullEndPoint.RegisterOriginalOppositeEndPoint(relatedEndPointMock.Object);

      relatedEndPointMock.Verify();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var relatedEndPointMock = new Mock<IRealObjectEndPoint>(MockBehavior.Strict);
      relatedEndPointMock.Setup(mock => mock.ResetSyncState()).Verifiable();

      _nullEndPoint.UnregisterOriginalOppositeEndPoint(relatedEndPointMock.Object);

      relatedEndPointMock.Verify();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.RegisterCurrentOppositeEndPoint(_relatedEndPointStub.Object);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.UnregisterCurrentOppositeEndPoint(_relatedEndPointStub.Object);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That(_nullEndPoint.IsSynchronized, Is.True);
    }

    [Test]
    public void Synchronize ()
    {
      Assert.That(
          () => _nullEndPoint.Synchronize(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void SynchronizeOppositeEndPoint ()
    {
      Assert.That(
          () => _nullEndPoint.SynchronizeOppositeEndPoint(_relatedEndPointStub.Object),
          Throws.InvalidOperationException);
    }

    [Test]
    public void ValidateMandatory ()
    {
      Assert.That(
          () => _nullEndPoint.ValidateMandatory(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetOppositeRelationEndPointIDs ()
    {
      Assert.That(
          () => _nullEndPoint.GetOppositeRelationEndPointIDs(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      _nullEndPoint.EnsureDataComplete();
    }

    [Test]
    public void SetDataFromSubTransaction ()
    {
      Assert.That(
          () => _nullEndPoint.SetDataFromSubTransaction(new Mock<IRelationEndPoint>().Object),
          Throws.InvalidOperationException);
    }

    [Test]
    public void Commit ()
    {
      Assert.That(
          () => _nullEndPoint.Commit(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void Rollback ()
    {
      Assert.That(
          () => _nullEndPoint.Commit(),
          Throws.InvalidOperationException);
    }
  }
}
