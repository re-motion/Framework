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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class ObservableCollectionDataDecoratorBaseTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private TestableObservableCollectionDataDecorator.IEventSink _eventSinkMock;

    private TestableObservableCollectionDataDecorator _observableDecoratorWithRealContent;

    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _order3 = DomainObjectIDs.Order3.GetObject<Order> ();
      _order4 = DomainObjectIDs.Order4.GetObject<Order> ();
      _order5 = DomainObjectIDs.Order5.GetObject<Order> ();

      var realContent = new DomainObjectCollectionData (new[] { _order1, _order3, _order4 });

      _eventSinkMock = _mockRepository.StrictMock<TestableObservableCollectionDataDecorator.IEventSink>();
      _observableDecoratorWithRealContent = new TestableObservableCollectionDataDecorator (realContent, _eventSinkMock);

      _eventSinkMock.Replay();
    }

    [Test]
    public void Clear ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order1, 0))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order4, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));

        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order4, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (0)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (0)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order1, 0))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (0)));
      }

      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Clear();

      _eventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Insert ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order5, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order5, 2))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (4)));
      }

      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Insert (2, _order5);

      _eventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Remove ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (2)));
      }

      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order3);

      _eventSinkMock.VerifyAllExpectations();

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order5);

      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg <ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ID ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.Count, Is.EqualTo (2)));
      }

      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order3.ID);

      _eventSinkMock.VerifyAllExpectations();

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_NoEventIfNoRemove ()
    {
      _eventSinkMock.Replay();

      var result = _observableDecoratorWithRealContent.Remove (_order5.ID);

      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));

      Assert.That (result, Is.False);
    }

    [Test]
    public void Replace ()
    {
      using (_mockRepository.Ordered())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order5, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order3)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order5)));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order5, 1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent.GetObject (1), Is.SameAs (_order5)));
      }

      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Replace (1, _order5);

      _eventSinkMock.VerifyAllExpectations();
    }

    [Test]
    public void Replace_NoEventsOnSelfReplace ()
    {
      _eventSinkMock.Replay();

      _observableDecoratorWithRealContent.Replace (1, _order3);

      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg < ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanging (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
      _eventSinkMock.AssertWasNotCalled (
          mock =>
          mock.CollectionChanged (
              Arg<ObservableCollectionDataDecoratorBase.OperationKind>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<int>.Is.Anything));
    }

    [Test]
    public void Sort ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Sort, null, -1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent, Is.EqualTo (new[] { _order1, _order3, _order4 })));
        _eventSinkMock
            .Expect (
                mock =>
                mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Sort, null, -1))
            .WhenCalled (mi => Assert.That (_observableDecoratorWithRealContent, Is.EqualTo (new[] { _order4, _order3, _order1 })));
      }

      _eventSinkMock.Replay ();

      var weights = new Dictionary<DomainObject, int> { { _order1, 3 }, { _order3, 2 }, { _order4, 1 } };
      Comparison<DomainObject> comparison = (one, two) => weights[one].CompareTo (weights[two]);

      _observableDecoratorWithRealContent.Sort (comparison);

      _eventSinkMock.VerifyAllExpectations ();
    }

    [Test]
    public void Sort_WithException ()
    {
      _eventSinkMock.Expect (mock => mock.CollectionChanging (ObservableCollectionDataDecoratorBase.OperationKind.Sort, null, -1));
      _eventSinkMock.Expect (mock => mock.CollectionChanged (ObservableCollectionDataDecoratorBase.OperationKind.Sort, null, -1));
      _eventSinkMock.Replay ();

      Comparison<DomainObject> comparison = (one, two) => { throw new Exception(); };

      Assert.That (() => _observableDecoratorWithRealContent.Sort (comparison), Throws.Exception);

      _eventSinkMock.VerifyAllExpectations ();
    }

    [Test]
    public void Serializable ()
    {
      var source = new TestableObservableCollectionDataDecorator (new DomainObjectCollectionData(), null);
      source.Insert (0, _order1);
      source.Insert (1, _order3);
      source.Insert (2, _order4);

      var result = Serializer.SerializeAndDeserialize (source);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}