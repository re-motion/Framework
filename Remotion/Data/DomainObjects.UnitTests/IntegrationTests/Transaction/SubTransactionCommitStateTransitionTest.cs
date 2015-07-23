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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void CommitRootChangedSubChanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ++obj.OrderNumber;
        Assert.That (obj.State, Is.EqualTo (StateType.Changed));
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void CommitRootChangedSubUnchanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        obj.EnsureDataAvailable ();
        Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void CommitRootChangedSubNotLoaded ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (obj.State, Is.EqualTo (StateType.NotLoadedYet));
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void CommitRootChangedSubDeleted ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        FullyDeleteOrder (obj);
        Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.That (obj.State, Is.EqualTo (StateType.Invalid));
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void CommitRootUnchangedSubChanged ()
    {
      Order obj = GetUnchanged();
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        ++obj.OrderNumber;
        Assert.That (obj.State, Is.EqualTo (StateType.Changed));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void CommitRootUnchangedSubUnchanged ()
    {
      Order obj = GetUnchanged();
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.EnsureDataAvailable ();
        Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CommitRootUnchangedSubNotLoaded ()
    {
      Order obj = GetUnchanged ();
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (obj.State, Is.EqualTo (StateType.NotLoadedYet));
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CommitRootUnchangedSubDeleted ()
    {
      Order obj = GetUnchanged();
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        FullyDeleteOrder (obj);
        Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void CommitRootNewSubChanged ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged();
      Assert.That (obj.State, Is.EqualTo (StateType.New));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        ++obj.Int32Property;
        Assert.That (obj.State, Is.EqualTo (StateType.Changed));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CommitRootNewSubUnchanged ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.That (obj.State, Is.EqualTo (StateType.New));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.EnsureDataAvailable ();
        Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CommitRootNewSubNotLoaded ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.That (obj.State, Is.EqualTo (StateType.New));
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (obj.State, Is.EqualTo (StateType.NotLoadedYet));
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CommitRootNewSubDeleted ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.That (obj.State, Is.EqualTo (StateType.New));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.Delete();
        Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void CommitRootDeletedSubDiscarded ()
    {
      Order obj = GetDeleted();
      Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (obj.State, Is.EqualTo (StateType.Invalid));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void CommitRootDiscardedSubDiscarded ()
    {
      Order obj = GetInvalid();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (obj.State, Is.EqualTo (StateType.Invalid));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void CommitRootUnknownSubChanged ()
    {
      Order obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetChangedThroughPropertyValue();
        Assert.That (obj.State, Is.EqualTo (StateType.Changed));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void CommitRootUnknownSubUnchanged ()
    {
      Order obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetUnchanged();
        Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (TestableClientTransaction.DataManager.DataContainers[obj.ID], Is.Not.Null);
      Assert.That (obj.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CommitRootUnknownSubNew ()
    {
      ClassWithAllDataTypes obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetNewUnchanged();
        Assert.That (obj.State, Is.EqualTo (StateType.New));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CommitRootUnknown_SubNew_SubSubAlsoNew ()
    {
      ClassWithAllDataTypes objectCreatedInSub;
      ClassWithAllDataTypes objectCreatedInSubSub;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        objectCreatedInSub = GetNewUnchanged ();
        Assert.That (objectCreatedInSub.State, Is.EqualTo (StateType.New));

        using (ClientTransactionScope.CurrentTransaction.CreateSubTransaction ().EnterDiscardingScope ())
        {
          objectCreatedInSubSub = GetNewUnchanged ();

          Assert.That (objectCreatedInSub.State, Is.EqualTo (StateType.NotLoadedYet));
          Assert.That (objectCreatedInSubSub.State, Is.EqualTo (StateType.New));

          ClientTransactionScope.CurrentTransaction.Commit ();

          Assert.That (objectCreatedInSub.State, Is.EqualTo (StateType.NotLoadedYet));
          Assert.That (objectCreatedInSubSub.State, Is.EqualTo (StateType.Unchanged));
        }

        Assert.That (objectCreatedInSub.State, Is.EqualTo (StateType.New));
        Assert.That (objectCreatedInSubSub.State, Is.EqualTo (StateType.New));

        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.That (objectCreatedInSub.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (objectCreatedInSubSub.State, Is.EqualTo (StateType.Unchanged));
      }

      Assert.That (objectCreatedInSub.State, Is.EqualTo (StateType.New));
      Assert.That (objectCreatedInSubSub.State, Is.EqualTo (StateType.New));
    }

    [Test]
    public void CommitRootUnknownSubDeleted ()
    {
      Order obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetDeleted();
        Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (obj.State, Is.EqualTo (StateType.Deleted));
    }

    [Test]
    public void CommitRootUnknownSubDiscarded ()
    {
      Order obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetInvalid();
        Assert.That (obj.State, Is.EqualTo (StateType.Invalid));
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.That (TestableClientTransaction.DataManager.DataContainers[obj.ID], Is.Null);
    }

    [Test]
    public void CommitRootChangedSubMakesUnchanged ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      ++order.OrderNumber;
      Assert.That (order.State, Is.EqualTo (StateType.Changed));

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        --order.OrderNumber;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }
  }
}
