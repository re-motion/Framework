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
  public class EventRaisingCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IDomainObjectCollectionEventRaiser _eventRaiserMock;

    private EventRaisingCollectionDataDecorator _eventRaisingDecoratorWithRealContent;

    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository ();
      _eventRaiserMock = _mockRepository.StrictMock<IDomainObjectCollectionEventRaiser> ();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();
      _order3 = DomainObjectIDs.Order3.GetObject<Order> ();
      _order4 = DomainObjectIDs.Order4.GetObject<Order> ();
      _order5 = DomainObjectIDs.Order5.GetObject<Order> ();

      var realContent = new DomainObjectCollectionData (new[] { _order1, _order3, _order4 });
      _eventRaisingDecoratorWithRealContent = new EventRaisingCollectionDataDecorator (_eventRaiserMock, realContent);

      _eventRaiserMock.BackToRecord ();
      _eventRaiserMock.Replay();
    }

    [Test]
    public void Clear ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (0, _order1)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.BeginRemove (2, _order4)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (3)));

        _eventRaiserMock.Expect (mock => mock.EndRemove (2, _order4)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (0)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (0)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (0, _order1)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (0)));
      }

      _eventRaiserMock.Replay ();

      _eventRaisingDecoratorWithRealContent.Clear ();

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Insert ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginAdd (2, _order5)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndAdd (2, _order5)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (4)));
      }

      _eventRaiserMock.Replay ();

      _eventRaisingDecoratorWithRealContent.Insert (2, _order5);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Remove ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (2)));
      }

      _eventRaiserMock.Replay ();

      var result = _eventRaisingDecoratorWithRealContent.Remove (_order3);

      _eventRaiserMock.VerifyAllExpectations ();

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      _eventRaiserMock.Replay ();

      var result = _eventRaisingDecoratorWithRealContent.Remove (_order5);

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));

      Assert.That (result, Is.False);
    }

    [Test]
    public void Remove_ID ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (3)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.Count, Is.EqualTo (2)));
      }

      _eventRaiserMock.Replay ();

      var result = _eventRaisingDecoratorWithRealContent.Remove (_order3.ID);

      _eventRaiserMock.VerifyAllExpectations ();

      Assert.That (result, Is.True);
    }

    [Test]
    public void Remove_ID_NoEventIfNoRemove ()
    {
      _eventRaiserMock.Replay ();

      var result = _eventRaisingDecoratorWithRealContent.Remove (_order5.ID);

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));

      Assert.That (result, Is.False);
    }

    [Test]
    public void Replace ()
    {
      using (_mockRepository.Ordered ())
      {
        _eventRaiserMock.Expect (mock => mock.BeginRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.GetObject (1), Is.SameAs (_order3)));
        _eventRaiserMock.Expect (mock => mock.BeginAdd (1, _order5)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.GetObject (1), Is.SameAs (_order3)));
        _eventRaiserMock.Expect (mock => mock.EndRemove (1, _order3)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.GetObject (1), Is.SameAs (_order5)));
        _eventRaiserMock.Expect (mock => mock.EndAdd (1, _order5)).WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent.GetObject (1), Is.SameAs (_order5)));
      }

      _eventRaiserMock.Replay ();

      _eventRaisingDecoratorWithRealContent.Replace (1, _order5);

      _eventRaiserMock.VerifyAllExpectations ();
    }

    [Test]
    public void Replace_NoEventsOnSelfReplace ()
    {
      _eventRaiserMock.Replay ();

      _eventRaisingDecoratorWithRealContent.Replace (1, _order3);

      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndRemove (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.BeginAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
      _eventRaiserMock.AssertWasNotCalled (mock => mock.EndAdd (Arg<int>.Is.Anything, Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void Sort ()
    {
      _eventRaiserMock
          .Expect (mock => mock.WithinReplaceData())
          .WhenCalled (mi => Assert.That (_eventRaisingDecoratorWithRealContent, Is.EqualTo (new[] { _order4, _order3, _order1 })));
      _eventRaiserMock.Replay();

      var weight = new Dictionary<DomainObject, int> { { _order1, 3 }, { _order3, 2 }, { _order4, 1 } };
      _eventRaisingDecoratorWithRealContent.Sort ((one, two) => weight[one].CompareTo (weight[two]));

      _eventRaiserMock.VerifyAllExpectations();
    }

    [Test]
    public void Serializable ()
    {
      var source = new EventRaisingCollectionDataDecorator (new SerializableDomainObjectCollectionEventRaiserFake (), new DomainObjectCollectionData ());
      source.Insert (0, _order1);
      source.Insert (1, _order3);
      source.Insert (2, _order4);

      var result = Serializer.SerializeAndDeserialize (source);
      Assert.That (result.Count, Is.EqualTo (3));
    }
  }
}
