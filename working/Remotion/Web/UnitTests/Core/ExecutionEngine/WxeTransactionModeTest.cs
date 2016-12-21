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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeTransactionModeTest
  {
    [Test]
    public void GetNone ()
    {
      Assert.That (WxeTransactionMode<TestTransactionFactory>.None, Is.InstanceOf (typeof (NoneTransactionMode)));
    }

    [Test]
    public void GetCreateRoot ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateRoot;
      Assert.That (transactionMode, Is.InstanceOf (typeof (CreateRootTransactionMode)));
      Assert.That (transactionMode.AutoCommit, Is.False);
    }

    [Test]
    public void GetCreateRootWithAutoCommit ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateRootWithAutoCommit;
      Assert.That (transactionMode, Is.InstanceOf (typeof (CreateRootTransactionMode)));
      Assert.That (transactionMode.AutoCommit, Is.True);
    }

    [Test]
    public void GetCreateChildIfParent ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateChildIfParent;
      Assert.That (transactionMode, Is.InstanceOf (typeof (CreateChildIfParentTransactionMode)));
      Assert.That (transactionMode.AutoCommit, Is.False);
    }

    [Test]
    public void GetCreateChildIfParentWithAutoCommit ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateChildIfParentWithAutoCommit;
      Assert.That (transactionMode, Is.InstanceOf (typeof (CreateChildIfParentTransactionMode)));
      Assert.That (transactionMode.AutoCommit, Is.True);
    }
  }
}
