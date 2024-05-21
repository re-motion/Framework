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
using Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class TransactionScopeTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void Execute_CreatesTransaction_ThenRestoresOriginal ()
    {
      Assert.That(ClientTransactionScope.HasCurrentTransaction, Is.False);

      ExecuteDelegateInWxeFunction(WxeTransactionMode<ClientTransactionFactory>.CreateRoot, (ctx, f) =>
      {
        Assert.That(ClientTransactionScope.HasCurrentTransaction, Is.True);
        Assert.That(f.Transaction.GetNativeTransaction<ClientTransaction>(), Is.Not.Null.And.SameAs(ClientTransaction.Current));
      });

      Assert.That(ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrows ()
    {
      try
      {
        new RemoveCurrentTransactionScopeFunction().Execute(Context);
      }
      catch (WxeFatalExecutionException ex)
      {
        Assert.That(ex.InnerException, Is.InstanceOf(typeof(InvalidOperationException)));
        Assert.That(ex.InnerException.Message, Is.EqualTo("The ClientTransactionScope has already been left."));
      }
    }

    [Test]
    public void RemoveCurrentScopeFromWithinFunctionThrowsWithPreviouslyExistingScope ()
    {
      try
      {
        ClientTransaction.CreateRootTransaction().EnterDiscardingScope();
        new RemoveCurrentTransactionScopeFunction().Execute(Context);
      }
      catch (WxeFatalExecutionException ex)
      {
        Assert.That(ex.InnerException, Is.InstanceOf(typeof(InvalidOperationException)));
        Assert.That(ex.InnerException.Message, Is.EqualTo("The ClientTransactionScope has already been left."));
      }
    }
  }
}
