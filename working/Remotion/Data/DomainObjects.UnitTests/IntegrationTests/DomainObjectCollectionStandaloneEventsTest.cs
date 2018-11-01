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
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectCollectionStandaloneEventsTest : ClientTransactionBaseTest
  {
    private OrderCollection.ICollectionEventReceiver _eventReceiverMock;
    
    private Order _itemA;
    private Order _itemB;
    private Order _itemCNotInCollection;
    
    private OrderCollection _collection;

    public override void SetUp ()
    {
      base.SetUp();

      _eventReceiverMock = MockRepository.GenerateStrictMock<OrderCollection.ICollectionEventReceiver> ();
      
      _itemA = DomainObjectMother.CreateFakeObject<Order> ();
      _itemB = DomainObjectMother.CreateFakeObject<Order> ();
      _itemCNotInCollection = DomainObjectMother.CreateFakeObject<Order> ();

      _collection = new OrderCollection (new[] { _itemA, _itemB });
    }

    [Test]
    public void Add_Events ()
    {
      using (_eventReceiverMock.GetMockRepository().Ordered())
      {
        _eventReceiverMock
            .Expect (mock => mock.OnAdding (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnAdded (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB, _itemCNotInCollection })));
      }

    _eventReceiverMock.Replay();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Add (_itemCNotInCollection);

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB, _itemCNotInCollection }));
    }

    [Test]
    public void Add_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Expect (mock => mock.OnAdding (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
          .Throw (exception);
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Add (_itemCNotInCollection), Throws.Exception.SameAs (exception));

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB }));
    }

    [Test]
    public void Clear_Events ()
    {
      using (_eventReceiverMock.GetMockRepository ().Ordered ())
      {
        _eventReceiverMock
            .Expect (
                mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemB)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (
                mock => mock.OnRemoved (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemB)))
            .WhenCalled (mi => Assert.That (_collection, Is.Empty));
        _eventReceiverMock
            .Expect (mock => mock.OnRemoved (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.Empty));
      }
      _eventReceiverMock.Replay();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Clear ();

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.Empty);
    }

    
    [Test]
    public void Clear_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock.Expect (mock => mock.OnRemoving(Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)));
      _eventReceiverMock
          .Expect (mock => mock.OnRemoving(Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemB)))
          .Throw (exception);
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Clear (), Throws.Exception.SameAs (exception));

      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB }));
    }

    [Test]
    public void Remove_Events ()
    {
      using (_eventReceiverMock.GetMockRepository ().Ordered ())
      {
        _eventReceiverMock
            .Expect (
                mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnRemoved (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemB })));
      }
      _eventReceiverMock.Replay();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Remove (_itemA);

      Assert.That (_collection, Is.EqualTo (new[] { _itemB }));
    }

    [Test]
    public void Remove_Null_Events ()
    {
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Remove ((DomainObject) null), Throws.TypeOf<ArgumentNullException>());

      _eventReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void Remove_ObjectNotInCollection_Events ()
    {
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Remove (_itemCNotInCollection);

      _eventReceiverMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void Remove_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Expect (mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
          .Throw (exception);
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Remove (_itemA), Throws.Exception.SameAs (exception));

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB }));
    }

    [Test]
    public void Remove_ID_Events ()
    {
      using (_eventReceiverMock.GetMockRepository ().Ordered ())
      {
        _eventReceiverMock
            .Expect (
                mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnRemoved (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemB })));
      }
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Remove (_itemA.ID);

      Assert.That (_collection, Is.EqualTo (new[] { _itemB }));
    }

    [Test]
    public void Remove_ID_Null_Events ()
    {
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Remove ((ObjectID) null), Throws.TypeOf<ArgumentNullException> ());

      _eventReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove_ID_ObjectNotInCollection_Events ()
    {
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Remove (_itemCNotInCollection.ID);

      _eventReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove_ID_Events_Cancel ()
    {
      var exception = new Exception ();
      _eventReceiverMock
          .Expect (mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
          .Throw (exception);
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Remove (_itemA.ID), Throws.Exception.SameAs (exception));

      _eventReceiverMock.VerifyAllExpectations ();
      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB }));
    }


    [Test]
    public void Insert_Events ()
    {
      using (_eventReceiverMock.GetMockRepository ().Ordered ())
      {
        _eventReceiverMock
            .Expect (mock => mock.OnAdding (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnAdded (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemCNotInCollection, _itemA, _itemB })));
      }
      _eventReceiverMock.Replay();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection.Insert (0, _itemCNotInCollection);

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.EqualTo (new[] { _itemCNotInCollection, _itemA, _itemB }));
    }

    [Test]
    public void Insert_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock
          .Expect (mock => mock.OnAdding (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
          .Throw (exception);
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection.Insert (0, _itemCNotInCollection), Throws.Exception.SameAs (exception));

      _eventReceiverMock.VerifyAllExpectations ();
      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB }));
    }

    [Test]
    public void Item_Set_Events ()
    {
      using (_eventReceiverMock.GetMockRepository ().Ordered ())
      {
        _eventReceiverMock
            .Expect (mock => mock.OnRemoving (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnAdding (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnRemoved (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemCNotInCollection, _itemB })));
        _eventReceiverMock
            .Expect (mock => mock.OnAdded (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
            .WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemCNotInCollection, _itemB })));
      }
      _eventReceiverMock.Replay();
      _collection.SetEventReceiver (_eventReceiverMock);

      _collection[0] = _itemCNotInCollection;

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.EqualTo (new[] { _itemCNotInCollection, _itemB }));
    }

    [Test]
    public void Item_Set_Events_Cancel ()
    {
      var exception = new Exception();
      _eventReceiverMock.Expect (mock => mock.OnRemoving(Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemA)));
      _eventReceiverMock
          .Expect (mock => mock.OnAdding (Arg<DomainObjectCollectionChangeEventArgs>.Matches (args => args.DomainObject == _itemCNotInCollection)))
          .Throw (exception);
      _eventReceiverMock.Replay ();
      _collection.SetEventReceiver (_eventReceiverMock);

      Assert.That (() => _collection[0] = _itemCNotInCollection, Throws.Exception.SameAs (exception));

      _eventReceiverMock.VerifyAllExpectations();
      Assert.That (_collection, Is.EqualTo (new[] { _itemA, _itemB }));
    }

    [Test]
    public void Sort_Events ()
    {
      _collection.SetEventReceiver (_eventReceiverMock);
      _eventReceiverMock.Expect (mock => mock.OnReplaceData()).WhenCalled (mi => Assert.That (_collection, Is.EqualTo (new[] { _itemB, _itemA})));
      _eventReceiverMock.Replay();

      var weights = new Dictionary<DomainObject, int> { { _itemA, 2 }, { _itemB, 1 } };
      _collection.Sort ((one, two) => weights[one].CompareTo (weights[two]));

      _eventReceiverMock.VerifyAllExpectations();
    }
  }
}