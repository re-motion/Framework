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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void CreateSubTransaction ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransaction, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_OfSubTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction();
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction2), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_SetsParentReadonly ()
    {
      Assert.That (TestableClientTransaction.IsWriteable, Is.True);
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (TestableClientTransaction.IsWriteable, Is.False);
      Assert.That (subTransaction.IsWriteable, Is.True);

      ClientTransaction subTransaction2 = subTransaction.CreateSubTransaction();
      Assert.That (subTransaction.IsWriteable, Is.False);
      Assert.That (subTransaction2.IsWriteable, Is.True);
    }

    [Test]
    public void ParentTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (subTransaction1.ParentTransaction, Is.SameAs (TestableClientTransaction));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction2.ParentTransaction, Is.SameAs (subTransaction1));
    }

    [Test]
    public void ActiveSubTansaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (TestableClientTransaction.SubTransaction, Is.SameAs (subTransaction1));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction1.SubTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.SubTransaction, Is.Null);

      subTransaction2.Discard();

      Assert.That (subTransaction1.SubTransaction, Is.Null);
      Assert.That (TestableClientTransaction.SubTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard();
      Assert.That (TestableClientTransaction.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (TestableClientTransaction.RootTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (subTransaction1.RootTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (subTransaction2.RootTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void LeafTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.LeafTransaction, Is.SameAs (subTransaction2));

      subTransaction2.Discard();

      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (subTransaction1));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard ();

      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void EnterDiscardingScopeEnablesDiscardBehavior ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }
    }

    [Test]
    public void SubTransactionHasDifferentExtensions ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransaction.Extensions, Is.Not.SameAs (TestableClientTransaction.Extensions));

      var extensionStub1 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub1.Stub (stub => stub.Key).Return ("E1");
      var extensionStub2 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub2.Stub (stub => stub.Key).Return ("E2");

      TestableClientTransaction.Extensions.Add (extensionStub1);
      Assert.That (TestableClientTransaction.Extensions, Has.Member (extensionStub1));
      Assert.That (subTransaction.Extensions, Has.No.Member (extensionStub1));

      subTransaction.Extensions.Add (extensionStub2);
      Assert.That (subTransaction.Extensions, Has.Member (extensionStub2));
      Assert.That (TestableClientTransaction.Extensions, Has.No.Member (extensionStub2));
    }

    [Test]
    public void SubTransactionHasSameApplicationData ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransaction.ApplicationData, Is.SameAs (TestableClientTransaction.ApplicationData));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage =
        "The operation cannot be executed because the ClientTransaction is read-only, probably because it has an open subtransaction. "
        + "Offending transaction modification: SubTransactionCreating.")]
    public void NoTwoSubTransactionsAtSameTime ()
    {
      TestableClientTransaction.CreateSubTransaction();
      TestableClientTransaction.CreateSubTransaction();
    }

   [Test]
    public void EnlistedObjects_SharedWithParentTransaction ()
    {
      var subTx = TestableClientTransaction.CreateSubTransaction ();

      var order = DomainObjectIDs.Order1.GetObject<Order> (subTx);
     
      Assert.That (subTx.IsEnlisted (order), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);
    }
    
    [Test]
    public void SubTransactionCreatedEvent ()
    {
      ClientTransaction subTransactionFromEvent = null;

      TestableClientTransaction.SubTransactionCreated += delegate (object sender, SubTransactionCreatedEventArgs args)
      {
        Assert.That (sender, Is.SameAs (TestableClientTransaction));
        Assert.That (args.SubTransaction, Is.Not.Null);
        subTransactionFromEvent = args.SubTransaction;
      };

      Assert.That (subTransactionFromEvent, Is.Null);
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransactionFromEvent, Is.Not.Null);
      Assert.That (subTransactionFromEvent, Is.SameAs (subTransaction));
    }
  }
}
