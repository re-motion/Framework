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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectExtensionsTest : StandardMappingTest
  {
    [Test]
    public void GetIDOrNull ()
    {
      Assert.That(((IDomainObject)null).GetSafeID(), Is.Null);

      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      Assert.That(domainObject.GetSafeID(), Is.EqualTo(domainObject.ID));
    }

    [Test]
    public void GetHandle ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();

      var handle = domainObject.GetHandle();
      var domainObjectTypedObjectID1 = domainObject.GetHandle<IDomainObject>();
      var domainObjectTypedObjectID2 = ((IDomainObject)domainObject).GetHandle();

      Assert.That(handle, Is.TypeOf<DomainObjectHandle<Order>>().And.Property("ObjectID").EqualTo(domainObject.ID));
      Assert.That(domainObjectTypedObjectID1, Is.TypeOf<DomainObjectHandle<Order>>().And.Property("ObjectID").EqualTo(domainObject.ID));
      Assert.That(domainObjectTypedObjectID2, Is.TypeOf<DomainObjectHandle<Order>>().And.Property("ObjectID").EqualTo(domainObject.ID));

      Assert.That(VariableTypeInferrer.GetVariableType(handle), Is.SameAs(typeof(IDomainObjectHandle<Order>)));
      Assert.That(VariableTypeInferrer.GetVariableType(domainObjectTypedObjectID1), Is.SameAs(typeof(IDomainObjectHandle<IDomainObject>)));
      Assert.That(VariableTypeInferrer.GetVariableType(domainObjectTypedObjectID2), Is.SameAs(typeof(IDomainObjectHandle<IDomainObject>)));
    }

    [Test]
    public void GetSafeHandle ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();

      var handle = domainObject.GetSafeHandle();
      var domainObjectTypedObjectID1 = domainObject.GetSafeHandle<IDomainObject>();
      var domainObjectTypedObjectID2 = ((IDomainObject)domainObject).GetSafeHandle();

      Assert.That(handle, Is.TypeOf<DomainObjectHandle<Order>>().And.Property("ObjectID").EqualTo(domainObject.ID));
      Assert.That(domainObjectTypedObjectID1, Is.TypeOf<DomainObjectHandle<Order>>().And.Property("ObjectID").EqualTo(domainObject.ID));
      Assert.That(domainObjectTypedObjectID2, Is.TypeOf<DomainObjectHandle<Order>>().And.Property("ObjectID").EqualTo(domainObject.ID));

      Assert.That(VariableTypeInferrer.GetVariableType(handle), Is.SameAs(typeof(IDomainObjectHandle<Order>)));
      Assert.That(VariableTypeInferrer.GetVariableType(domainObjectTypedObjectID1), Is.SameAs(typeof(IDomainObjectHandle<IDomainObject>)));
      Assert.That(VariableTypeInferrer.GetVariableType(domainObjectTypedObjectID2), Is.SameAs(typeof(IDomainObjectHandle<IDomainObject>)));
    }

    [Test]
    public void GetSafeHandle_Null ()
    {
      var handle = ((IDomainObject)null).GetSafeHandle();
      Assert.That(handle, Is.Null);
    }

    [Test]
    public void GetDefaultTransactionContext ()
    {
      IDomainObject domainObject = DomainObjectMother.CreateFakeObject<Order>();

      var actualTransactionContext = domainObject.GetDefaultTransactionContext();

      var expectedTransactionContext =
          domainObject.TransactionContext[domainObject.RootTransaction.ActiveTransaction];

      Assert.That(actualTransactionContext.ClientTransaction, Is.EqualTo(expectedTransactionContext.ClientTransaction));
      Assert.That(actualTransactionContext.DomainObject, Is.EqualTo(expectedTransactionContext.DomainObject));
      Assert.That(actualTransactionContext, Is.EqualTo(expectedTransactionContext));
    }

    [Test]
    public void GetState ()
    {
      IDomainObject domainObject = DomainObjectMother.CreateFakeObject<Order>();

      var state = domainObject.GetState();

      Assert.That(state, Is.EqualTo(domainObject.TransactionContext[domainObject.RootTransaction.ActiveTransaction].State));
    }

    [Test]
    public void GetTimestamp ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order>(ClientTransaction.CreateRootTransaction());

      var timestamp = domainObject.GetTimestamp();

      Assert.That(timestamp, Is.SameAs(domainObject.TransactionContext[domainObject.RootTransaction.ActiveTransaction].Timestamp));
    }

    [Test]
    public void RegisterForCommit ()
    {
      var transaction = new TestableClientTransaction();
      Order order = transaction.ExecuteInScope(() => DomainObjectIDs.Order1.GetObject<Order>());
      transaction.ExecuteInScope(() => Assert.That(order.State.IsUnchanged, Is.True));

      transaction.ExecuteInScope(order.RegisterForCommit);

      transaction.ExecuteInScope(() => Assert.That(order.State.IsChanged, Is.True));
    }

    [Test]
    public void EnsureDataAvailable ()
    {
      var transaction = new TestableClientTransaction();
      var order = DomainObjectMother.GetNotLoadedObject(transaction, DomainObjectIDs.Order1);
      Assert.That(transaction.DataManager.DataContainers[order.ID], Is.Null);

      transaction.ExecuteInScope(order.EnsureDataAvailable);

      Assert.That(transaction.DataManager.DataContainers[order.ID], Is.Not.Null);
      Assert.That(transaction.DataManager.DataContainers[order.ID].DomainObject, Is.SameAs(order));
    }

    [Test]
    public void TryEnsureDataAvailable ()
    {
      var transaction = new TestableClientTransaction();
      var order = DomainObjectMother.GetNotLoadedObject(transaction, DomainObjectIDs.Order1);
      Assert.That(transaction.DataManager.DataContainers[order.ID], Is.Null);

      transaction.ExecuteInScope(() => Assert.That(() => order.TryEnsureDataAvailable(), Is.True));

      Assert.That(transaction.DataManager.DataContainers[order.ID], Is.Not.Null);
      Assert.That(transaction.DataManager.DataContainers[order.ID].DomainObject, Is.SameAs(order));

      var nonExistingOrder = DomainObjectMother.GetNotLoadedObject(transaction, new ObjectID(typeof(ClassWithAllDataTypes), Guid.NewGuid()));
      Assert.That(transaction.DataManager.DataContainers[nonExistingOrder.ID], Is.Null);

      transaction.ExecuteInScope(() => Assert.That(() => nonExistingOrder.TryEnsureDataAvailable(), Is.False));

      Assert.That(transaction.DataManager.DataContainers[nonExistingOrder.ID], Is.Null);
    }
  }
}
