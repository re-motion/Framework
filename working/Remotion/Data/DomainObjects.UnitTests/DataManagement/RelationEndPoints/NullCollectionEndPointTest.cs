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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class NullCollectionEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullCollectionEndPoint _nullEndPoint;
    private OrderItem _relatedObject;
    private IRealObjectEndPoint _relatedEndPointStub;

    public override void SetUp ()
    {
      base.SetUp();
      _definition = MappingConfiguration.Current.GetTypeDefinition (typeof (Order))
          .GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _nullEndPoint = new NullCollectionEndPoint (TestableClientTransaction, _definition);
      _relatedObject = OrderItem.NewObject();
      _relatedEndPointStub = MockRepository.GenerateStub<IRealObjectEndPoint> ();
    }

    [Test]
    public void Definition ()
    {
      Assert.That (_nullEndPoint.Definition, Is.SameAs (_definition));
    }

    [Test]
    public void ObjectID ()
    {
      Assert.That (_nullEndPoint.ObjectID, Is.Null);
    }

    [Test]
    public void ID ()
    {
      var id = _nullEndPoint.ID;
      Assert.That (id.Definition, Is.SameAs (_definition));
      Assert.That (id.ObjectID, Is.Null);
    }

    [Test]
    public void Collection_Get ()
    {
      Assert.That (_nullEndPoint.Collection, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void OriginalCollection ()
    {
      Dev.Null = _nullEndPoint.OriginalCollection;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetData ()
    {
      Dev.Null = _nullEndPoint.GetData ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetOriginalData ()
    {
      Dev.Null = _nullEndPoint.GetOriginalData ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetCollectionEventRaiser ()
    {
      Dev.Null = _nullEndPoint.GetCollectionEventRaiser();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetCollectionWithOriginalData ()
    {
      Dev.Null = _nullEndPoint.GetCollectionWithOriginalData ();
    }

    [Test]
    public void IsDataComplete ()
    {
      Assert.That (_nullEndPoint.IsDataComplete, Is.True);
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
    public void HasChanged ()
    {
      Assert.That (_nullEndPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasChangedFast ()
    {
      Assert.That (_nullEndPoint.HasChangedFast, Is.False);
    }

    [Test]
    public void HasBeenTouched ()
    {
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void GetDomainObject_Null ()
    {
      Assert.That (_nullEndPoint.GetDomainObject (), Is.Null);
    }

    [Test]
    public void GetDomainObjectReference_Null ()
    {
      Assert.That (_nullEndPoint.GetDomainObjectReference (), Is.Null);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_nullEndPoint.IsNull, Is.True);
    }

    [Test]
    public void MarkDataComplete ()
    {
      _nullEndPoint.MarkDataComplete (new DomainObject[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void MarkDataIncomplete ()
    {
      _nullEndPoint.MarkDataIncomplete ();
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
      _nullEndPoint.Touch ();
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateSetCollectionCommand ()
    {
      Assert.That (_nullEndPoint.CreateSetCollectionCommand (new DomainObjectCollection()), Is.InstanceOf (typeof (NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateInsertCommand ()
    {
      Assert.That (_nullEndPoint.CreateInsertCommand (_relatedObject, 12), Is.InstanceOf (typeof (NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateAddCommand ()
    {
      Assert.That (_nullEndPoint.CreateAddCommand (_relatedObject), Is.InstanceOf (typeof (NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateRemoveCommand ()
    {
      Assert.That (_nullEndPoint.CreateRemoveCommand (_relatedObject), Is.InstanceOf (typeof (NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateReplaceCommand ()
    {
      Assert.That (_nullEndPoint.CreateReplaceCommand (12, _relatedObject), Is.InstanceOf (typeof (NullEndPointModificationCommand)));
    }

    [Test]
    public void CreateDeleteCommand ()
    {
      Assert.That (_nullEndPoint.CreateDeleteCommand(), Is.InstanceOf (typeof (NullEndPointModificationCommand)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void SortCurrentData ()
    {
      _nullEndPoint.SortCurrentData ((one, two) => 0);
    }

    [Test]
    public void RegisterOriginalOppositeEndPoint ()
    {
      var relatedEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint>();
      relatedEndPointMock.Expect (mock => mock.MarkSynchronized());
      relatedEndPointMock.Replay();

      _nullEndPoint.RegisterOriginalOppositeEndPoint (relatedEndPointMock);

      relatedEndPointMock.VerifyAllExpectations();
    }

    [Test]
    public void UnregisterOriginalOppositeEndPoint ()
    {
      var relatedEndPointMock = MockRepository.GenerateStrictMock<IRealObjectEndPoint> ();
      relatedEndPointMock.Expect (mock => mock.ResetSyncState());
      relatedEndPointMock.Replay ();

      _nullEndPoint.UnregisterOriginalOppositeEndPoint (relatedEndPointMock);

      relatedEndPointMock.VerifyAllExpectations ();
    }

    [Test]
    public void RegisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.RegisterCurrentOppositeEndPoint (_relatedEndPointStub);
    }

    [Test]
    public void UnregisterCurrentOppositeEndPoint ()
    {
      _nullEndPoint.UnregisterCurrentOppositeEndPoint (_relatedEndPointStub);
    }

    [Test]
    public void IsSynchronized ()
    {
      Assert.That (_nullEndPoint.IsSynchronized, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Synchronize ()
    {
      _nullEndPoint.Synchronize();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void SynchronizeOppositeEndPoint ()
    {
      _nullEndPoint.SynchronizeOppositeEndPoint (_relatedEndPointStub);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ValidateMandatory ()
    {
      _nullEndPoint.ValidateMandatory ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetOppositeRelationEndPointIDs ()
    {
      _nullEndPoint.GetOppositeRelationEndPointIDs ();
    }

    [Test]
    public void EnsureDataComplete_DoesNothing ()
    {
      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents (TestableClientTransaction);

      _nullEndPoint.EnsureDataComplete ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void SetDataFromSubTransaction ()
    {
      _nullEndPoint.SetDataFromSubTransaction (MockRepository.GenerateStub<IRelationEndPoint> ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Commit ()
    {
      _nullEndPoint.Commit ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Rollback ()
    {
      _nullEndPoint.Commit ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void SerializeIntoFlatStructure ()
    {
      _nullEndPoint.SerializeIntoFlatStructure (new FlattenedSerializationInfo ());
    }
  }
}
