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
  public class ChildTransactionExecutionListenerTest
  {
    private WxeContext _wxeContext;
    private Mock<ChildTransactionStrategy> _transactionStrategyMock;
    private Mock<IWxeFunctionExecutionListener> _innerListenerMock;
    private IWxeFunctionExecutionListener _transactionListener;
    private Mock<ITransaction> _childTransactionMock;

    [SetUp]
    public void SetUp ()
    {
      _wxeContext = WxeContextFactory.Create(new TestFunction());
      var outerTransactionStrategyStub = new Mock<TransactionStrategyBase>();
      var executionContextStub = new Mock<IWxeFunctionExecutionContext>();
      executionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      _childTransactionMock = new Mock<ITransaction>();
      var parentTransactionStub = new Mock<ITransaction>();
      parentTransactionStub.Setup(stub => stub.CreateChild()).Returns(_childTransactionMock.Object);
      _transactionStrategyMock = new Mock<ChildTransactionStrategy>(
          false, outerTransactionStrategyStub.Object, parentTransactionStub.Object, executionContextStub.Object);

      _innerListenerMock = new Mock<IWxeFunctionExecutionListener>();
      _transactionListener = new ChildTransactionExecutionListener(_transactionStrategyMock.Object, _innerListenerMock.Object);
    }

    [Test]
    public void OnExecutionPlay ()
    {
      InvokeTransactionStrategyPlay();

      _transactionListener.OnExecutionPlay(_wxeContext);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionPlay(_wxeContext, _innerListenerMock.Object), Times.Never());
      _innerListenerMock.Verify(mock => mock.OnExecutionPlay(_wxeContext), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionStop ()
    {
      InvokeTransactionStrategyPlay();

      _transactionListener.OnExecutionStop(_wxeContext);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionStop(_wxeContext, _innerListenerMock.Object), Times.AtLeastOnce());
      _innerListenerMock.Verify(mock => mock.OnExecutionStop(_wxeContext), Times.Never());
    }

    [Test]
    public void OnExecutionPause ()
    {
      InvokeTransactionStrategyPlay();

      _transactionListener.OnExecutionPause(_wxeContext);

      _transactionStrategyMock.Verify(mock => mock.OnExecutionPause(_wxeContext, _innerListenerMock.Object), Times.Never());
      _innerListenerMock.Verify(mock => mock.OnExecutionPause(_wxeContext), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionFail ()
    {
      InvokeTransactionStrategyPlay();

      Exception exception = new Exception();

      _transactionStrategyMock.Object.OnExecutionFail(_wxeContext, _innerListenerMock.Object, exception);

      _transactionListener.OnExecutionStop(_wxeContext);
      _transactionStrategyMock.Verify(mock => mock.OnExecutionFail(_wxeContext, _innerListenerMock.Object, exception), Times.AtLeastOnce());
      _innerListenerMock.Verify(mock => mock.OnExecutionFail(_wxeContext, exception), Times.Never());
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(_transactionListener.IsNull, Is.False);
    }

    private void InvokeTransactionStrategyPlay ()
    {
      _childTransactionMock.Setup(stub => stub.EnterScope()).Returns(new Mock<ITransactionScope>().Object);

      _transactionStrategyMock.CallBase = true;
      _transactionStrategyMock.Object.OnExecutionPlay(_wxeContext, new Mock<IWxeFunctionExecutionListener>().Object);
      _transactionStrategyMock.CallBase = false;
    }
  }
}
