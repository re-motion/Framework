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
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class ResetTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void Reset_ReplacesCurrentTransaction ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));

        f.Reset ();

        Assert.That (transactionBefore, Is.Not.SameAs (ClientTransaction.Current));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));
      });
    }

    [Test]
    public void Reset_VariablesOfDomainObjectTypes_CauseException ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var order = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject> ();
        f.Variables.Add ("Object", order);
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction>()));

        Assert.That (
            () => f.Reset(),
            Throws.TypeOf<WxeException>().With.Message.EqualTo (
                string.Format (
                    "One or more of the variables of the WxeFunction are incompatible with the new transaction after the Reset. The following objects "
                    + "are incompatible with the target transaction: {0}. Objects of type 'Remotion.Data.DomainObjects.IDomainObjectHandle`1[T]' "
                    + "could be used instead. (To avoid this exception, clear the Variables collection from incompatible objects before calling "
                    + "Reset and repopulate it afterwards.)",
                    DomainObjectIDs.ClassWithAllDataTypes1)));

        Assert.That (ClientTransaction.Current, Is.Not.Null.And.Not.SameAs (transactionBefore));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction>()));
      });
    }

    [Test]
    public void Reset_VariablesOfDomainObjectHandleType_CauseNoException ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var objectHandle = DomainObjectIDs.ClassWithAllDataTypes1.GetHandle<SampleObject> ();
        f.Variables.Add ("ObjectHandle", objectHandle);
        var transactionBefore = ClientTransaction.Current;
        Assert.That (transactionBefore, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));

        Assert.That (() => f.Reset (), Throws.Nothing);

        Assert.That (ClientTransaction.Current, Is.Not.Null.And.Not.SameAs (transactionBefore));
        Assert.That (ClientTransaction.Current, Is.SameAs (f.Transaction.GetNativeTransaction<ClientTransaction> ()));
      });
    }

    [Test]
    public void Reset_NonVariables_CauseNoException ()
    {
      ExecuteDelegateInWxeFunction (WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        var obj = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject> ();

        Assert.That (() => f.Reset (), Throws.Nothing);

        var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction> ();
        Assert.That (transactionAfter.IsEnlisted (obj), Is.False);
      });
    }

    [Test]
    public void Reset_WithChildTransaction ()
    {
      ExecuteDelegateInSubWxeFunction (
          WxeTransactionMode<ClientTransactionFactory>.CreateRoot,
          WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParent,
          (ctx, f) =>
          {
            var transactionBefore = f.Transaction.GetNativeTransaction<ClientTransaction>();
            Assert.That (transactionBefore, Is.Not.Null.And.SameAs (ClientTransaction.Current));
            Assert.That (transactionBefore.ParentTransaction, Is.Not.Null);
            var parentBefore = transactionBefore.ParentTransaction;

            var obj = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<SampleObject>();
            f.Variables.Add ("Object", obj);

            f.Reset();

            var transactionAfter = f.Transaction.GetNativeTransaction<ClientTransaction>();
            Assert.That (transactionAfter, Is.Not.SameAs (transactionBefore));
            Assert.That (transactionAfter, Is.Not.Null.And.SameAs (ClientTransaction.Current));
            Assert.That (transactionAfter.ParentTransaction, Is.Not.Null.And.SameAs (parentBefore));

            // This is because it was automatically enlisted in the outer transaction before the reset
            Assert.That (transactionAfter.IsEnlisted (obj), Is.True);
          });
    }
  }
}