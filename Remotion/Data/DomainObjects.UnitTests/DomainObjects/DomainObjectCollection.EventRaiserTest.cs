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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  public partial class DomainObjectCollectionTest
  {
    [Test]
    public void EventRaiser_BeginAdd ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.BeginAdd(1, _customer1);

      collectionMock.Verify (          mock => PrivateInvoke.InvokeNonPublicMethod(
                      mock,
                      "OnAdding",
                      It.Is<DomainObjectCollectionChangeEventArgs> (args => args.DomainObject == _customer1)), Times.AtLeastOnce());
    }

    [Test]
    public void EventRaiser_EndAdd ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.EndAdd(1, _customer1);

      collectionMock.Verify (          mock => PrivateInvoke.InvokeNonPublicMethod(
                      mock,
                      "OnAdded",
                      It.Is<DomainObjectCollectionChangeEventArgs> (args => args.DomainObject == _customer1)), Times.AtLeastOnce());
    }

    [Test]
    public void EventRaiser_BeginRemove ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.BeginRemove(1, _customer1);

      collectionMock.Verify (          mock => PrivateInvoke.InvokeNonPublicMethod(
                      mock,
                      "OnRemoving",
                      It.Is<DomainObjectCollectionChangeEventArgs> (args => args.DomainObject == _customer1)), Times.AtLeastOnce());
    }

    [Test]
    public void EventRaiser_EndRemove ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.EndRemove(1, _customer1);

      collectionMock.Verify (          mock => PrivateInvoke.InvokeNonPublicMethod(
                      mock,
                      "OnRemoved",
                      It.Is<DomainObjectCollectionChangeEventArgs> (args => args.DomainObject == _customer1)), Times.AtLeastOnce());
    }

    [Test]
    public void EventRaiser_BeginDelete ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.BeginDelete();

      collectionMock.Verify (mock => PrivateInvoke.InvokeNonPublicMethod(mock, "OnDeleting"), Times.AtLeastOnce());
    }

    [Test]
    public void EventRaiser_EndDelete ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.EndDelete();

      collectionMock.Verify (mock => PrivateInvoke.InvokeNonPublicMethod(mock, "OnDeleted"), Times.AtLeastOnce());
    }

    [Test]
    public void EventRaiser_WithinReplaceData ()
    {
      var collectionMock = new Mock<DomainObjectCollection>() { CallBase = true };

      var eventRaiser = (IDomainObjectCollectionEventRaiser)collectionMock;
      eventRaiser.WithinReplaceData();

      collectionMock.Verify (mock => PrivateInvoke.InvokeNonPublicMethod(mock, "OnReplaceData"), Times.AtLeastOnce());
    }
  }
}
