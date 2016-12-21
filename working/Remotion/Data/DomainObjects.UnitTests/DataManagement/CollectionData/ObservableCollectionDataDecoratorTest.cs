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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class ObservableCollectionDataDecoratorTest : ClientTransactionBaseTest
  {
    private ObservableCollectionDataDecorator _observableDecorator;

    private Order _order1;

    public override void SetUp ()
    {
      base.SetUp();

      _order1 = DomainObjectIDs.Order1.GetObject<Order> ();

      var realContent = new DomainObjectCollectionData ();
      _observableDecorator = new ObservableCollectionDataDecorator (realContent);
    }

    [Test]
    public void OnDataChanging_RaisesBeginEvent ()
    {
      object eventSender = null;
      ObservableCollectionDataDecorator.DataChangeEventArgs eventArgs = null;

      _observableDecorator.CollectionChanging += (sender, args) =>
      {
        eventSender = sender;
        eventArgs = args;
      };

      PrivateInvoke.InvokeNonPublicMethod (
          _observableDecorator, "OnDataChanging", ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order1, 12);

      Assert.That (eventSender, Is.Not.Null);
      Assert.That (eventSender, Is.SameAs (_observableDecorator));
      Assert.That (eventArgs.Operation, Is.EqualTo (ObservableCollectionDataDecoratorBase.OperationKind.Insert));
      Assert.That (eventArgs.AffectedObject, Is.SameAs (_order1));
      Assert.That (eventArgs.Index, Is.EqualTo (12));
    }

    [Test]
    public void OnDataChanging_NoRegistration ()
    {
      PrivateInvoke.InvokeNonPublicMethod (
          _observableDecorator, "OnDataChanging", ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order1, 12);
    }

    [Test]
    public void OnDataChanged_RaisesEndEvent ()
    {
      object eventSender = null;
      ObservableCollectionDataDecorator.DataChangeEventArgs eventArgs = null;

      _observableDecorator.CollectionChanged += (sender, args) =>
      {
        eventSender = sender;
        eventArgs = args;
      };

      PrivateInvoke.InvokeNonPublicMethod (
          _observableDecorator, "OnDataChanged", ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order1, 12);

      Assert.That (eventSender, Is.Not.Null);
      Assert.That (eventSender, Is.SameAs (_observableDecorator));
      Assert.That (eventArgs.Operation, Is.EqualTo (ObservableCollectionDataDecoratorBase.OperationKind.Insert));
      Assert.That (eventArgs.AffectedObject, Is.SameAs (_order1));
      Assert.That (eventArgs.Index, Is.EqualTo (12));
    }

    [Test]
    public void OnDataChanged_NoRegistration ()
    {
      PrivateInvoke.InvokeNonPublicMethod (
          _observableDecorator, "OnDataChanged", ObservableCollectionDataDecoratorBase.OperationKind.Insert, _order1, 12);
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize (_observableDecorator);
    }
  }
}