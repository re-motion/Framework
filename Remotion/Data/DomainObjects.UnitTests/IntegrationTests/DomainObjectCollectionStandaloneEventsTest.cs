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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectCollectionStandaloneEventsTest : ClientTransactionBaseTest
  {
    private Mock<OrderCollection.ICollectionEventReceiver> _eventReceiverMock;

    private Order _itemA;
    private Order _itemB;
    private Order _itemCNotInCollection;

    private OrderCollection _collection;

    public override void SetUp ()
    {
      base.SetUp();

      _eventReceiverMock = new Mock<OrderCollection.ICollectionEventReceiver>(MockBehavior.Strict);

      _itemA = DomainObjectMother.CreateFakeObject<Order>();
      _itemB = DomainObjectMother.CreateFakeObject<Order>();
      _itemCNotInCollection = DomainObjectMother.CreateFakeObject<Order>();

      _collection = new OrderCollection(new[] { _itemA, _itemB });
    }

    [Test]
    public void Add_Events ()
    {
      var sequence = new VerifiableSequence();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAdding(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAdded(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB, _itemCNotInCollection })))
            .Verifiable();

      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Add(_itemCNotInCollection);

      _eventReceiverMock.Verify();
      sequence.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB, _itemCNotInCollection }));
    }

    [Test]
    public void Add_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Setup(mock => mock.OnAdding(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
          .Throws(exception)
          .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Add(_itemCNotInCollection), Throws.Exception.SameAs(exception));

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB }));
    }

    [Test]
    public void Clear_Events ()
    {
      var sequence = new VerifiableSequence();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemB)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoved(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemB)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.Empty))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoved(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.Empty))
            .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Clear();

      _eventReceiverMock.Verify();
      sequence.Verify();
      Assert.That(_collection, Is.Empty);
    }


    [Test]
    public void Clear_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock.Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA))).Verifiable();
      _eventReceiverMock
          .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemB)))
          .Throws(exception)
          .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Clear(), Throws.Exception.SameAs(exception));

      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB }));
    }

    [Test]
    public void Remove_Events ()
    {
      var sequence = new VerifiableSequence();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoved(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemB })))
            .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Remove(_itemA);

      Assert.That(_collection, Is.EqualTo(new[] { _itemB }));
      sequence.Verify();
    }

    [Test]
    public void Remove_Null_Events ()
    {
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Remove((DomainObject)null), Throws.TypeOf<ArgumentNullException>());

      _eventReceiverMock.Verify();
    }

    [Test]
    public void Remove_ObjectNotInCollection_Events ()
    {
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Remove(_itemCNotInCollection);

      _eventReceiverMock.Verify();
    }

    [Test]
    public void Remove_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
          .Throws(exception)
          .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Remove(_itemA), Throws.Exception.SameAs(exception));

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB }));
    }

    [Test]
    public void Remove_ID_Events ()
    {
      var sequence = new VerifiableSequence();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoved(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemB })))
            .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Remove(_itemA.ID);

      Assert.That(_collection, Is.EqualTo(new[] { _itemB }));
    }

    [Test]
    public void Remove_ID_Null_Events ()
    {
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Remove((ObjectID)null), Throws.TypeOf<ArgumentNullException>());

      _eventReceiverMock.Verify();
    }

    [Test]
    public void Remove_ID_ObjectNotInCollection_Events ()
    {
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Remove(_itemCNotInCollection.ID);

      _eventReceiverMock.Verify();
    }

    [Test]
    public void Remove_ID_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
          .Throws(exception)
          .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Remove(_itemA.ID), Throws.Exception.SameAs(exception));

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB }));
    }


    [Test]
    public void Insert_Events ()
    {
      var sequence = new VerifiableSequence();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAdding(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAdded(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemCNotInCollection, _itemA, _itemB })))
            .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection.Insert(0, _itemCNotInCollection);

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemCNotInCollection, _itemA, _itemB }));
    }

    [Test]
    public void Insert_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Setup(mock => mock.OnAdding(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
          .Throws(exception)
          .Verifiable();

      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection.Insert(0, _itemCNotInCollection), Throws.Exception.SameAs(exception));

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB }));
    }

    [Test]
    public void Item_Set_Events ()
    {
      var sequence = new VerifiableSequence();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAdding(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnRemoved(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemCNotInCollection, _itemB })))
            .Verifiable();
      _eventReceiverMock
            .InVerifiableSequence(sequence)
            .Setup(mock => mock.OnAdded(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
            .Callback((DomainObjectCollectionChangeEventArgs args) => Assert.That(_collection, Is.EqualTo(new[] { _itemCNotInCollection, _itemB })))
            .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      _collection[0] = _itemCNotInCollection;

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemCNotInCollection, _itemB }));
    }

    [Test]
    public void Item_Set_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock.Setup(mock => mock.OnRemoving(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemA))).Verifiable();
      _eventReceiverMock
          .Setup(mock => mock.OnAdding(It.Is<DomainObjectCollectionChangeEventArgs>(args => args.DomainObject == _itemCNotInCollection)))
          .Throws(exception)
          .Verifiable();
      _collection.SetEventReceiver(_eventReceiverMock.Object);

      Assert.That(() => _collection[0] = _itemCNotInCollection, Throws.Exception.SameAs(exception));

      _eventReceiverMock.Verify();
      Assert.That(_collection, Is.EqualTo(new[] { _itemA, _itemB }));
    }

    [Test]
    public void Sort_Events ()
    {
      _collection.SetEventReceiver(_eventReceiverMock.Object);
      _eventReceiverMock.Setup(mock => mock.OnReplaceData()).Callback(() => Assert.That(_collection, Is.EqualTo(new[] { _itemB, _itemA}))).Verifiable();

      var weights = new Dictionary<IDomainObject, int> { { _itemA, 2 }, { _itemB, 1 } };
      _collection.Sort((one, two) => weights[one].CompareTo(weights[two]));

      _eventReceiverMock.Verify();
    }
  }
}
