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
using NUnit.Framework;
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class RootTransactionStrategyTest
  {
    private RootTransactionStrategy _strategy;
    private Mock<ITransaction> _transactionMock;
    private Mock<TransactionStrategyBase> _outerTransactionStrategyStub;
    private Mock<IWxeFunctionExecutionContext> _executionContextStub;

    [SetUp]
    public void SetUp ()
    {
      _transactionMock = new Mock<ITransaction>();
      _outerTransactionStrategyStub = new Mock<TransactionStrategyBase>();
      _executionContextStub = new Mock<IWxeFunctionExecutionContext>();
      _executionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      _strategy = new RootTransactionStrategy(true, () => _transactionMock.Object, _outerTransactionStrategyStub.Object, _executionContextStub.Object);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_strategy.Transaction, Is.SameAs(_transactionMock.Object));
      Assert.That(_strategy.OuterTransactionStrategy, Is.SameAs(_outerTransactionStrategyStub.Object));
      Assert.That(_strategy.ExecutionContext, Is.SameAs(_executionContextStub.Object));
      Assert.That(_strategy.AutoCommit, Is.True);
      Assert.That(_strategy.IsNull, Is.False);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      var innerExecutionListenerStub = new Mock<IWxeFunctionExecutionListener>();
      IWxeFunctionExecutionListener executionListener = _strategy.CreateExecutionListener(innerExecutionListenerStub.Object);

      Assert.That(executionListener, Is.InstanceOf(typeof(RootTransactionExecutionListener)));
      var transactionExecutionListener = (RootTransactionExecutionListener)executionListener;
      Assert.That(transactionExecutionListener.InnerListener, Is.SameAs(innerExecutionListenerStub.Object));
      Assert.That(transactionExecutionListener.TransactionStrategy, Is.SameAs(_strategy));
    }
  }
}
