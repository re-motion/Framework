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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NoneTransactionStrategyTest
  {
    private Mock<IWxeFunctionExecutionListener> _executionListenerMock;
    private NoneTransactionStrategy _strategy;
    private WxeContext _context;
    private Mock<IWxeFunctionExecutionContext> _executionContextMock;
    private Mock<TransactionStrategyBase> _outerTransactionStrategyMock;

    [SetUp]
    public void SetUp ()
    {
      _context = WxeContextFactory.Create(new TestFunction());

      _executionListenerMock = new Mock<IWxeFunctionExecutionListener>();
      _executionContextMock = new Mock<IWxeFunctionExecutionContext>();
      _outerTransactionStrategyMock = new Mock<TransactionStrategyBase>();
      _strategy = new NoneTransactionStrategy(_outerTransactionStrategyMock.Object);
    }

    [Test]
    public void EvaluateDirtyState ()
    {
      Assert.That(_strategy.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void Commit ()
    {
      Assert.That(
          () => _strategy.Commit(),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void Rollback ()
    {
      Assert.That(
          () => _strategy.Rollback(),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void Reset ()
    {
      Assert.That(
          () => _strategy.Reset(),
          Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.That(_strategy.GetNativeTransaction<object>(), Is.Null);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(((INullObject)_strategy).IsNull, Is.True);
    }

    [Test]
    public void CreateExecutionListener ()
    {
      Assert.That(_strategy.CreateExecutionListener(_executionListenerMock.Object), Is.SameAs(_executionListenerMock.Object));
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      var grandParentTransactionStrategyMock = new Mock<TransactionStrategyBase>();

      var noneTransactionStrategy = new NoneTransactionStrategy(grandParentTransactionStrategyMock.Object);

      var childExecutionContextStub = new Mock<IWxeFunctionExecutionContext>();
      childExecutionContextStub.Setup(stub => stub.GetInParameters()).Returns(new object[0]);

      var fakeParentTransaction = new Mock<ITransaction>();
      fakeParentTransaction.Setup(stub => stub.CreateChild()).Returns(new Mock<ITransaction>().Object);
      var fakeChildTransactionStrategy = new ChildTransactionStrategy(
          false, grandParentTransactionStrategyMock.Object, fakeParentTransaction.Object, childExecutionContextStub.Object);

      grandParentTransactionStrategyMock
          .Setup(mock => mock.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, _context))
          .Returns(fakeChildTransactionStrategy)
          .Verifiable();

      TransactionStrategyBase actual = noneTransactionStrategy.CreateChildTransactionStrategy(true, childExecutionContextStub.Object, _context);
      Assert.That(actual, Is.SameAs(fakeChildTransactionStrategy));
    }

    [Test]
    public void UnregisterChildTransactionStrategy ()
    {
      Assert.That(_strategy.OuterTransactionStrategy, Is.SameAs(_outerTransactionStrategyMock.Object));
      var childTransactionStrategyStub = new Mock<TransactionStrategyBase>();

      _strategy.UnregisterChildTransactionStrategy(childTransactionStrategyStub.Object);

      _outerTransactionStrategyMock.Verify(mock => mock.UnregisterChildTransactionStrategy(childTransactionStrategyStub.Object), Times.AtLeastOnce());
    }

    [Test]
    public void EnsureCompatibility ()
    {
      var expectedObjects = new[] { new object() };

      _outerTransactionStrategyMock.Setup(mock => mock.EnsureCompatibility(expectedObjects)).Verifiable();

      _strategy.EnsureCompatibility(expectedObjects);

      _executionContextMock.Verify();
      _outerTransactionStrategyMock.Verify();
    }

    [Test]
    public void OnExecutionPlay ()
    {
      _strategy.OnExecutionPlay(_context, _executionListenerMock.Object);
      _executionListenerMock.Verify(mock => mock.OnExecutionPlay(_context), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionStop ()
    {
      _strategy.OnExecutionStop(_context, _executionListenerMock.Object);
      _executionListenerMock.Verify(mock => mock.OnExecutionStop(_context), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionPause ()
    {
      _strategy.OnExecutionPause(_context, _executionListenerMock.Object);
      _executionListenerMock.Verify(mock => mock.OnExecutionPause(_context), Times.AtLeastOnce());
    }

    [Test]
    public void OnExecutionFail ()
    {
      var exception = new Exception();
      _strategy.OnExecutionFail(_context, _executionListenerMock.Object, exception);
      _executionListenerMock.Verify(mock => mock.OnExecutionFail(_context, exception), Times.AtLeastOnce());
    }
  }
}
