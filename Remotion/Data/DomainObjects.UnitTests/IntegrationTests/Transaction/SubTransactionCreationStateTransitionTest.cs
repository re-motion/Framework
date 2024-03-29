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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCreationStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void RootToSubUnchanged ()
    {
      DomainObject obj = GetUnchanged();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RootToSubChangedThroughPropertyValue ()
    {
      Order obj = GetChangedThroughPropertyValue();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        Assert.That(obj.Properties[typeof(Order) + ".OrderNumber"].GetOriginalValue<int>(), Is.EqualTo(obj.OrderNumber));
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsChanged, Is.True);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjects ()
    {
      Order obj = GetChangedThroughRelatedObjects();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        Assert.That(obj.Properties[typeof(Order) + ".OrderItems"].GetOriginalValue<ObjectList<OrderItem>>().Count, Is.EqualTo(obj.OrderItems.Count));
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsChanged, Is.True);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectRealSide ()
    {
      Computer obj = GetChangedThroughRelatedObjectRealSide();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        Assert.That(obj.Properties[typeof(Computer) + ".Employee"].GetOriginalValue<Employee>(), Is.EqualTo(obj.Employee));
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsChanged, Is.True);
    }

    [Test]
    public void RootToSubChangedThroughRelatedObjectVirtualSide ()
    {
      Employee obj = GetChangedThroughRelatedObjectVirtualSide();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        Assert.That(obj.Properties[typeof(Employee) + ".Computer"].GetOriginalValue<Computer>(), Is.EqualTo(obj.Computer));
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsChanged, Is.True);
    }

    [Test]
    public void RootToSubNewUnchanged ()
    {
      DomainObject obj = GetNewUnchanged();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsNew, Is.True);
    }

    [Test]
    public void RootToSubNewChanged ()
    {
      DomainObject obj = GetNewChanged();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsNew, Is.True);
    }

    [Test]
    public void RootToSubDeleted ()
    {
      Order obj = GetDeleted();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsInvalid, Is.True);
      }
      Assert.That(obj.State.IsDeleted, Is.True);
    }

    [Test]
    public void RootToSubDeletedThrowsWhenReloadingTheObject ()
    {
      Order obj = GetDeleted();
      ObjectID id = obj.ID;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsInvalid, Is.True);
        Assert.That(
            () => id.GetObject<Order>(),
            Throws.InstanceOf<ObjectInvalidException>()
                .With.Message.EqualTo("Object 'Order|90e26c86-611f-4735-8d1b-e1d0918515c2|System.Guid' is invalid in this transaction."));
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeleted ()
    {
      Client deleted = DomainObjectIDs.Client1.GetObject<Client>();
      Location obj = GetUnidirectionalWithDeleted();
      Assert.That(deleted.State.IsDeleted, Is.True);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
        Assert.That(deleted.State.IsInvalid, Is.True);
      }
      Assert.That(obj.State.IsUnchanged, Is.True);
      Assert.That(deleted.State.IsDeleted, Is.True);
    }

    [Test]
    public void RootToSubUnidirectional_WithDeleted_ReturnsInvalidObject ()
    {
      Location obj = GetUnidirectionalWithDeleted();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.Client.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void RootToSubUnidirectionalWithDeletedNew ()
    {
      Location obj = GetUnidirectionalWithDeletedNew();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
      }
      Assert.That(obj.State.IsChanged, Is.True);
    }

    [Test]
    public void RootToSubUnidirectional_WithDeletedNew_ReturnsInvalidObject ()
    {
      Location obj = GetUnidirectionalWithDeletedNew();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.Client.State.IsInvalid, Is.True);
      }
    }

    [Test]
    public void RootToSubDiscarded ()
    {
      DomainObject obj = GetInvalid();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsInvalid, Is.True);
      }
      Assert.That(obj.State.IsInvalid, Is.True);
    }

    [Test]
    public void RootToSubNewIsNewInHierarchy ()
    {
      DomainObject obj = GetNewUnchanged();
      Assert.That(obj.State.IsNew, Is.True);
      Assert.That(obj.State.IsNewInHierarchy, Is.True);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        Assert.That(obj.State.IsNewInHierarchy, Is.True);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
        Assert.That(obj.State.IsNewInHierarchy, Is.True);
      }
    }

    [Test]
    public void RootToSubUnchangedIsNewInHierarchy ()
    {
      DomainObject obj = GetUnchanged();
      Assert.That(obj.State.IsUnchanged, Is.True);
      Assert.That(obj.State.IsNewInHierarchy, Is.False);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsNotLoadedYet, Is.True);
        Assert.That(obj.State.IsNewInHierarchy, Is.False);
        obj.EnsureDataAvailable();
        Assert.That(obj.State.IsUnchanged, Is.True);
        Assert.That(obj.State.IsNewInHierarchy, Is.False);
      }
    }

    [Test]
    public void RootToSubDeletedIsNewInHierarchy ()
    {
      Order obj = GetDeleted();
      Assert.That(obj.State.IsDeleted, Is.True);
      Assert.That(obj.State.IsNewInHierarchy, Is.False);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(obj.State.IsInvalid, Is.True);
        Assert.That(obj.State.IsNewInHierarchy, Is.False);
      }
    }

  }
}
