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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class NullObjectEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullObjectEndPoint _nullEndPoint;

    public override void SetUp ()
    {
      base.SetUp();
      _definition = DomainObjectIDs.OrderTicket1.ClassDefinition.GetRelationEndPointDefinition(typeof(OrderTicket).FullName + ".Order");
      _nullEndPoint = new NullObjectEndPoint(TestableClientTransaction, _definition);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That(_nullEndPoint.IsSynchronized, Is.True);
    }

    [Test]
    public void Synchronize_DoesNothing ()
    {
      _nullEndPoint.Synchronize();
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
    public void OppositeObjectID_Get ()
    {
      Assert.That(_nullEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    public void OriginalOppositeObjectID_Get ()
    {
      Assert.That(
          () => _nullEndPoint.OriginalOppositeObjectID,
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetOppositeObject ()
    {
      Assert.That(_nullEndPoint.GetOppositeObject(), Is.Null);
    }

    [Test]
    public void GetOriginalOppositeObject ()
    {
      Assert.That(
          () => _nullEndPoint.GetOriginalOppositeObject(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void IsDataComplete_True ()
    {
      Assert.That(_nullEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That(_nullEndPoint.HasChanged, Is.False);
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
    public void Touch ()
    {
      Assert.That(_nullEndPoint.HasBeenTouched, Is.False);
      _nullEndPoint.Touch();
      Assert.That(_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      var removedRelatedObject = DomainObjectIDs.Order4.GetObject<Order>();
      var command = (NullEndPointModificationCommand)_nullEndPoint.CreateRemoveCommand(removedRelatedObject);
      Assert.That(command.AffectedEndPoint, Is.SameAs(_nullEndPoint));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      var command = (NullEndPointModificationCommand)_nullEndPoint.CreateDeleteCommand();
      Assert.That(command.AffectedEndPoint, Is.SameAs(_nullEndPoint));
    }

    [Test]
    public void CreateSetCommand ()
    {
      var newRelatedObject = DomainObjectIDs.Order4.GetObject<Order>();
      var command = (NullEndPointModificationCommand)_nullEndPoint.CreateSetCommand(newRelatedObject);
      Assert.That(command.AffectedEndPoint, Is.SameAs(_nullEndPoint));
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
    public void GetOppositeRelationEndPointID ()
    {
      Assert.That(
          () => _nullEndPoint.GetOppositeRelationEndPointID(),
          Throws.InvalidOperationException);
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      _nullEndPoint.EnsureDataComplete();
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

    [Test]
    public void SetDataFromSubTransaction ()
    {
      Assert.That(
          () => _nullEndPoint.SetDataFromSubTransaction(new Mock<IRelationEndPoint>().Object),
          Throws.InvalidOperationException);
    }

    [Test]
    public void SerializeIntoFlatStructure ()
    {
      Assert.That(
          () => _nullEndPoint.SerializeIntoFlatStructure(new FlattenedSerializationInfo()),
          Throws.InvalidOperationException);
    }
  }
}
