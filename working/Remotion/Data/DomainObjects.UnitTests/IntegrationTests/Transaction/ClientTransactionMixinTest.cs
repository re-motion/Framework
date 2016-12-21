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
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionMixinTest
  {
    [Test]
    public void ClientTransactionCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction mixedTransaction = ClientTransaction.CreateRootTransaction ();
        Assert.That (mixedTransaction, Is.Not.Null);
        Assert.That (Mixin.Get<InvertingClientTransactionMixin> (mixedTransaction), Is.Not.Null);
      }
    }

    [Test]
    public void SubTransactionsAlsoMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction mixedTransaction = ClientTransaction.CreateRootTransaction ();
        ClientTransaction mixedSubTransaction = mixedTransaction.CreateSubTransaction ();
        Assert.That (mixedSubTransaction, Is.Not.Null);
        Assert.That (Mixin.Get<InvertingClientTransactionMixin> (mixedSubTransaction), Is.Not.Null);
      }
    }

    [Test]
    public void TransactionMethodsCanBeOverridden ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction invertedTransaction = ClientTransaction.CreateRootTransaction();

        bool committed = false;
        bool rolledBack = false;
        invertedTransaction.Committed += delegate { committed = true; };
        invertedTransaction.RolledBack += delegate { rolledBack = true; };

        Assert.That (rolledBack, Is.False);
        Assert.That (committed, Is.False);

        invertedTransaction.Commit();

        Assert.That (rolledBack, Is.True);
        Assert.That (committed, Is.False);

        rolledBack = false;
        invertedTransaction.Rollback();

        Assert.That (rolledBack, Is.False);
        Assert.That (committed, Is.True);
      }
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (ClientTransactionWithIDMixin)).EnterScope())
      {
        IClientTransactionWithID transactionWithID = (IClientTransactionWithID) ClientTransaction.CreateRootTransaction ();
        Assert.That (transactionWithID.ToString (), Is.EqualTo (transactionWithID.ID.ToString ()));
        IClientTransactionWithID subTransactionWithID = (IClientTransactionWithID) transactionWithID.AsClientTransaction.CreateSubTransaction ();
        Assert.That (subTransactionWithID.ID, Is.Not.EqualTo (transactionWithID.ID));
        Assert.That (((IClientTransactionWithID) subTransactionWithID.AsClientTransaction.ParentTransaction).ID, Is.EqualTo (transactionWithID.ID));
      }
    }
  }
}
