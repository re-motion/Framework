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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.ForbiddenOperations
{
  [TestFixture]
  public class RollbackTest : ReadOnlyTransactionsTestBase
  {
    protected override void InitializeReadOnlyRootTransaction ()
    {
      base.InitializeReadOnlyRootTransaction ();
      ClassWithAllDataTypes.NewObject ();
    }

    protected override void InitializeReadOnlyMiddleTransaction ()
    {
      base.InitializeReadOnlyMiddleTransaction ();
      ClassWithAllDataTypes.NewObject ();
    }

    protected override void InitializeWriteableSubTransaction ()
    {
      base.InitializeWriteableSubTransaction ();
      ClassWithAllDataTypes.NewObject ();
    }
    
    [Test]
    public void RollbackInReadOnlyRootTransaction_IsForbidden ()
    {
      Assert.That (ReadOnlyRootTransaction.HasChanged (), Is.True);
      Assert.That (ReadOnlyMiddleTransaction.HasChanged (), Is.True);
      Assert.That (WriteableSubTransaction.HasChanged (), Is.True);

      CheckForbidden (() => ReadOnlyRootTransaction.Rollback (), "TransactionRollingBack");

      Assert.That (ReadOnlyRootTransaction.HasChanged (), Is.True);
      Assert.That (ReadOnlyMiddleTransaction.HasChanged (), Is.True);
      Assert.That (WriteableSubTransaction.HasChanged (), Is.True);
    }

    [Test]
    public void RollbackInReadOnlyMiddleTransaction_IsForbidden ()
    {
      Assert.That (ReadOnlyRootTransaction.HasChanged (), Is.True);
      Assert.That (ReadOnlyMiddleTransaction.HasChanged (), Is.True);
      Assert.That (WriteableSubTransaction.HasChanged (), Is.True);

      CheckForbidden (() => ReadOnlyMiddleTransaction.Rollback (), "TransactionRollingBack");

      Assert.That (ReadOnlyRootTransaction.HasChanged (), Is.True);
      Assert.That (ReadOnlyMiddleTransaction.HasChanged (), Is.True);
      Assert.That (WriteableSubTransaction.HasChanged (), Is.True);
    }
  }
}