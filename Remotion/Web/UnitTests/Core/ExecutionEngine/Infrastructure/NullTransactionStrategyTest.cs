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
  public class NullTransactionStrategyTest
  {
    private TransactionStrategyBase _strategy;
    private WxeContext _context;
    private Mock<IWxeFunctionExecutionListener> _executionListenerMock;

    [SetUp]
    public void SetUp ()
    {
      _context = WxeContextFactory.Create(new TestFunction());

      _executionListenerMock = new Mock<IWxeFunctionExecutionListener>();

      _strategy = NullTransactionStrategy.Null;
    }

    [Test]
    public void IsNull ()
    {
      Assert.That(_strategy.IsNull, Is.True);
    }

    [Test]
    public void GetOuterTransactionStrategy ()
    {
      Assert.That(_strategy.OuterTransactionStrategy, Is.Null);
    }

    [Test]
    public void GetNativeTransaction ()
    {
      Assert.That(_strategy.GetNativeTransaction<ITransaction>(), Is.Null);
    }

    [Test]
    public void CreateChildTransactionStrategy ()
    {
      Assert.That(_strategy.CreateChildTransactionStrategy(true, new Mock<IWxeFunctionExecutionContext>().Object, _context), Is.Null);
    }

    [Test]
    public void UnregisterChildTransactionStrategy ()
    {
      _strategy.UnregisterChildTransactionStrategy(new Mock<TransactionStrategyBase>().Object);
    }

    [Test]
    public void EnsureCompatibility ()
    {
      _strategy.EnsureCompatibility(null);
    }

    [Test]
    public void EvaluateDirtyState ()
    {
      Assert.That(_strategy.EvaluateDirtyState(), Is.False);
    }

    [Test]
    public void Commit ()
    {
      _strategy.Commit();
    }

    [Test]
    public void Rollback ()
    {
      _strategy.Rollback();
    }

    [Test]
    public void Reset ()
    {
      _strategy.Reset();
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
