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
  public class EventRaisingDomainObjectCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private Mock<IDomainObjectCollectionEventRaiser> _eventRaiserMock;

    private EventRaisingDomainObjectCollectionDataDecorator _eventRaisingDomainObjectDecoratorWithRealContent;

    private Order _order1;
    private Order _order3;
    private Order _order4;
    private Order _order5;

    public override void SetUp ()
    {
      base.SetUp();

      _eventRaiserMock = new Mock<IDomainObjectCollectionEventRaiser>(MockBehavior.Strict);

      _order1 = DomainObjectIDs.Order1.GetObject<Order>();
      _order3 = DomainObjectIDs.Order3.GetObject<Order>();
      _order4 = DomainObjectIDs.Order4.GetObject<Order>();
      _order5 = DomainObjectIDs.Order5.GetObject<Order>();

      var realContent = new DomainObjectCollectionData(new[] { _order1, _order3, _order4 });
      _eventRaisingDomainObjectDecoratorWithRealContent = new EventRaisingDomainObjectCollectionDataDecorator(_eventRaiserMock.Object, realContent);

      _eventRaiserMock.Reset();
    }

    [Test]
    public void Clear ()
    {
      var sequence = new VerifiableSequence();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginRemove(0, _order1))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginRemove(2, _order4))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndRemove(2, _order4))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(0)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(0)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndRemove(0, _order1))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(0)))
          .Verifiable();

      _eventRaisingDomainObjectDecoratorWithRealContent.Clear();

      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Insert ()
    {
      var sequence = new VerifiableSequence();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginAdd(2, _order5))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndAdd(2, _order5))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(4)))
          .Verifiable();

      _eventRaisingDomainObjectDecoratorWithRealContent.Insert(2, _order5);

      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Remove ()
    {
      var sequence = new VerifiableSequence();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(2)))
          .Verifiable();

      var result = _eventRaisingDomainObjectDecoratorWithRealContent.Remove(_order3);

      _eventRaiserMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_NoEventIfNoRemove ()
    {
      var result = _eventRaisingDomainObjectDecoratorWithRealContent.Remove(_order5);

      _eventRaiserMock.Verify(mock => mock.BeginRemove(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
      _eventRaiserMock.Verify(mock => mock.EndRemove(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());

      Assert.That(result, Is.False);
    }

    [Test]
    public void Remove_ID ()
    {
      var sequence = new VerifiableSequence();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.Count, Is.EqualTo(2)))
          .Verifiable();

      var result = _eventRaisingDomainObjectDecoratorWithRealContent.Remove(_order3.ID);

      _eventRaiserMock.Verify();
      sequence.Verify();

      Assert.That(result, Is.True);
    }

    [Test]
    public void Remove_ID_NoEventIfNoRemove ()
    {
      var result = _eventRaisingDomainObjectDecoratorWithRealContent.Remove(_order5.ID);

      _eventRaiserMock.Verify(mock => mock.BeginRemove(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
      _eventRaiserMock.Verify(mock => mock.EndRemove(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());

      Assert.That(result, Is.False);
    }

    [Test]
    public void Replace ()
    {
      var sequence = new VerifiableSequence();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.BeginAdd(1, _order5))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order3)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndRemove(1, _order3))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order5)))
          .Verifiable();
      _eventRaiserMock
          .InVerifiableSequence(sequence)
          .Setup(mock => mock.EndAdd(1, _order5))
          .Callback((int index, DomainObject domainObject) => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent.GetObject(1), Is.SameAs(_order5)))
          .Verifiable();

      _eventRaisingDomainObjectDecoratorWithRealContent.Replace(1, _order5);

      _eventRaiserMock.Verify();
      sequence.Verify();
    }

    [Test]
    public void Replace_NoEventsOnSelfReplace ()
    {
      _eventRaisingDomainObjectDecoratorWithRealContent.Replace(1, _order3);

      _eventRaiserMock.Verify(mock => mock.BeginRemove(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
      _eventRaiserMock.Verify(mock => mock.EndRemove(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
      _eventRaiserMock.Verify(mock => mock.BeginAdd(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
      _eventRaiserMock.Verify(mock => mock.EndAdd(It.IsAny<int>(), It.IsAny<DomainObject>()), Times.Never());
    }

    [Test]
    public void Sort ()
    {
      _eventRaiserMock
          .Setup(mock => mock.WithinReplaceData())
          .Callback(() => Assert.That(_eventRaisingDomainObjectDecoratorWithRealContent, Is.EqualTo(new[] { _order4, _order3, _order1 })))
          .Verifiable();

      var weight = new Dictionary<IDomainObject, int> { { _order1, 3 }, { _order3, 2 }, { _order4, 1 } };
      _eventRaisingDomainObjectDecoratorWithRealContent.Sort((one, two) => weight[one].CompareTo(weight[two]));

      _eventRaiserMock.Verify();
    }

    [Test]
    public void Serializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var source = new EventRaisingDomainObjectCollectionDataDecorator(new SerializableDomainObjectCollectionEventRaiserFake(), new DomainObjectCollectionData());
      source.Insert(0, _order1);
      source.Insert(1, _order3);
      source.Insert(2, _order4);

      var result = Serializer.SerializeAndDeserialize(source);
      Assert.That(result.Count, Is.EqualTo(3));
    }
  }
}
