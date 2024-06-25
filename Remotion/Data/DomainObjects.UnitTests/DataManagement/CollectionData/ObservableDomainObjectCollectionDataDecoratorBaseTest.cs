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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class ObservableDomainObjectCollectionDataDecoratorBaseTest : ClientTransactionBaseTest
  {
    private Mock<TestableObservableDomainObjectCollectionDataDecorator.IEventSink> _eventSinkMock;

    private TestableObservableDomainObjectCollectionDataDecorator _observableDomainObjectDecoratorWithRealContent;

    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();
      _order4 = DomainObjectIDs.Order4.GetObject<Order>();
      _order5 = DomainObjectIDs.Order5.GetObject<Order>();

      var realContent = new DomainObjectCollectionData(new[] { _order1, _order3, _order4 });

      _eventSinkMock = new Mock<TestableObservableDomainObjectCollectionDataDecorator.IEventSink>(MockBehavior.Strict);
      _observableDomainObjectDecoratorWithRealContent = new TestableObservableDomainObjectCollectionDataDecorator(realContent, _eventSinkMock.Object);
    }

    [Test]
    public void Clear ()
    {
      var sequence = new VerifiableSequence();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order1, 0))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order4, 2))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order4, 2))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(0)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(0)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order1, 0))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(0)))
          .Verifiable();

      _observableDomainObjectDecoratorWithRealContent.Clear();

      _eventSinkMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Insert ()
    {
      var sequence = new VerifiableSequence();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Insert, _order5, 2))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Insert, _order5, 2))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(4)))
          .Verifiable();

      _observableDomainObjectDecoratorWithRealContent.Insert(2, _order5);

      _eventSinkMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Remove ()
    {
      var sequence = new VerifiableSequence();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(2)))
          .Verifiable();

      var result = _observableDomainObjectDecoratorWithRealContent.Remove(_order3);

      _eventSinkMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      var result = _observableDomainObjectDecoratorWithRealContent.Remove(_order5);

      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanging(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());
      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanged(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());

      Assert.That(result, Is.False);
    }

    [Test]
    public void Remove_ID ()
    {
      var sequence = new VerifiableSequence();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(2)))
          .Verifiable();

      var result = _observableDomainObjectDecoratorWithRealContent.Remove(_order3.ID);

      _eventSinkMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_ID_NoEventIfNoRemove ()
    {
      var result = _observableDomainObjectDecoratorWithRealContent.Remove(_order5.ID);

      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanging(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());
      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanged(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());

      Assert.That(result, Is.False);
    }

    [Test]
    public void Replace ()
    {
      var sequence = new VerifiableSequence();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Insert, _order5, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order3)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Remove, _order3, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order5)))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Insert, _order5, 1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order5)))
          .Verifiable();

      _observableDomainObjectDecoratorWithRealContent.Replace(1, _order5);

      _eventSinkMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Replace_NoEventsOnSelfReplace ()
    {
      _observableDomainObjectDecoratorWithRealContent.Replace(1, _order3);

      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanging(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());
      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanged(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());
      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanging(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());
      _eventSinkMock
          .Verify(
              mock => mock.CollectionChanged(
                  It.IsAny<ObservableDomainObjectCollectionDataDecoratorBase.OperationKind>(),
                  It.IsAny<DomainObject>(),
                  It.IsAny<int>()),
              Times.Never());
    }

    [Test]
    public void Sort ()
    {
      var sequence = new VerifiableSequence();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Sort, null, -1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent, Is.EqualTo(new[] { _order1, _order3, _order4 })))
          .Verifiable();
      _eventSinkMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Sort, null, -1))
          .Callback((IInvocation _) => Assert.That(_observableDomainObjectDecoratorWithRealContent, Is.EqualTo(new[] { _order4, _order3, _order1 })))
          .Verifiable();

      var weights = new Dictionary<DomainObject, int> { { _order1, 3 }, { _order3, 2 }, { _order4, 1 } };
      Comparison<DomainObject> comparison = (one, two) => weights[one].CompareTo(weights[two]);

      _observableDomainObjectDecoratorWithRealContent.Sort(comparison);

      _eventSinkMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Sort_WithException ()
    {
      _eventSinkMock
          .Setup(mock => mock.CollectionChanging(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Sort, null, -1))
          .Verifiable();
      _eventSinkMock
          .Setup(mock => mock.CollectionChanged(ObservableDomainObjectCollectionDataDecoratorBase.OperationKind.Sort, null, -1))
          .Verifiable();

      Comparison<DomainObject> comparison = (one, two) => { throw new Exception(); };

      Assert.That(() => _observableDomainObjectDecoratorWithRealContent.Sort(comparison), Throws.Exception);

      _eventSinkMock.Verify();
    }
  }
}
