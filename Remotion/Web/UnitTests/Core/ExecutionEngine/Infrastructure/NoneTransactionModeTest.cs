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
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NoneTransactionModeTest
  {
    [Test]
    public void CreateTransactionStrategy_WithoutParentFunction ()
    {
      WxeContext context = WxeContextFactory.Create(new TestFunction());

      ITransactionMode transactionMode = new NoneTransactionMode();
      TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy(new TestFunction2(transactionMode), context);

      Assert.That(strategy, Is.InstanceOf(typeof(NoneTransactionStrategy)));
      Assert.That(strategy.OuterTransactionStrategy, Is.SameAs(NullTransactionStrategy.Null));
    }

    [Test]
    public void CreateTransactionStrategy_WithParentFunction ()
    {
      ITransactionMode transactionMode = new NoneTransactionMode();

      WxeFunction parentFunction = new TestFunction2(new NoneTransactionMode());
      WxeFunction childFunction = new TestFunction2(transactionMode);
      parentFunction.Add(childFunction);

      var stepMock = new Mock<WxeStep>();
      childFunction.Add(stepMock.Object);

      WxeContext context = WxeContextFactory.Create(new TestFunction());

      stepMock
          .Setup(mock => mock.Execute(context))
          .Callback(
              (WxeContext context) =>
              {
                TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy(childFunction, context);
                Assert.That(strategy, Is.InstanceOf(typeof(NoneTransactionStrategy)));
                Assert.That(strategy.OuterTransactionStrategy, Is.SameAs(((TestFunction2)parentFunction).TransactionStrategy));
              })
          .Verifiable();

      parentFunction.Execute(context);
    }

    [Test]
    public void IsSerializeable ()
    {
      var deserialized = Serializer.SerializeAndDeserialize(new NoneTransactionMode());

      Assert.That(deserialized.AutoCommit, Is.False);
    }
  }
}
