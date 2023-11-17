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
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Development.Web.UnitTesting.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class RootTransactionExecutionListenerTest
  {
    private WxeContext _wxeContext;
    private Mock<RootTransactionStrategy> _transactionStrategyMock;
    private Mock<IWxeFunctionExecutionListener> _innerListenerStub;
    private IWxeFunctionExecutionListener _transactionListener;

    [SetUp]
    public void SetUp ()
    {
      _wxeContext = WxeContextFactory.Create(new TestFunction());
      var transactionMock = new Mock<ITransaction>();
      var outerTransactionStrategyStub = new Mock<TransactionStrategyBase>();
      var executionContextStub = new Mock<IWxeFunctionExecutionContext>();
      executionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      _transactionStrategyMock = new Mock<RootTransactionStrategy>(
          false, (Func<ITransaction>)(() => transactionMock.Object), outerTransactionStrategyStub.Object, executionContextStub.Object);

      _innerListenerStub = new Mock<IWxeFunctionExecutionListener>();
      _transactionListener = new RootTransactionExecutionListener(_transactionStrategyMock.Object, _innerListenerStub.Object);
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _transactionListener.OnExecutionPlay(_wxeContext);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionPlay(_wxeContext, _innerListenerStub.Object), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionStop ()
    {
      _transactionListener.OnExecutionStop(_wxeContext);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionStop(_wxeContext, _innerListenerStub.Object), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionPause ()
    {
      _transactionListener.OnExecutionPause(_wxeContext);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionPause(_wxeContext, _innerListenerStub.Object), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionFail ()
    {
      Exception exception = new Exception();

      _transactionListener.OnExecutionFail(_wxeContext, exception);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionFail(_wxeContext, _innerListenerStub.Object, exception), Times.AtLeastOnce());
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(_transactionListener.IsNull, Is.False);
    }
  }
}
