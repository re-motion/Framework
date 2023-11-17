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
using System.Transactions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Development.Web.UnitTesting.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  public class WxeTransactedFunctionIntegrationTestBase
  {
    private WxeContext _context;
    private TransactionScope _transactionScope;

    [SetUp]
    public virtual void SetUp ()
    {
      _context = WxeContextFactory.Create(WxeContextFactory.CreateHttpContext(), new WxeUrlSettings(), new WxeLifetimeManagementSettings());
      _transactionScope = new TransactionScope();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      _transactionScope.Dispose();
    }

    public WxeContext Context
    {
      get { return _context; }
    }

    protected void ExecuteDelegateInWxeFunction (ITransactionMode transactionMode, Action<WxeContext, DelegateExecutingTransactedFunction> testDelegate)
    {
      var function = new DelegateExecutingTransactedFunction(transactionMode, testDelegate);

      function.Execute(Context);
      Assert.That(function.DelegatesExecuted, Is.True);
    }

    protected void ExecuteDelegateInSubWxeFunction (
        ITransactionMode parentFunctionTransactionMode,
        ITransactionMode subFunctionTransactionMode,
        Action<WxeContext, DelegateExecutingTransactedFunction> testDelegate)
    {
      var subFunction = new DelegateExecutingTransactedFunction(subFunctionTransactionMode, testDelegate);

      var rootFunction = new TransactedFunctionWithChildFunction(parentFunctionTransactionMode, subFunction);
      rootFunction.Execute(Context);

      Assert.That(subFunction.DelegatesExecuted, Is.True);
    }

    protected void ExecuteDelegateInWxeFunctionWithParameters (
        ITransactionMode transactionMode,
        Action<WxeContext, DomainObjectParameterTestTransactedFunction> testDelegate,
        SampleObject inParameter,
        SampleObject[] inParameterArray,
        out SampleObject outParameter,
        out SampleObject[] outParameterArray)
    {
      var function = new DomainObjectParameterTestTransactedFunction(
          transactionMode,
          testDelegate,
          inParameter,
          inParameterArray);
      function.Execute(Context);

      Assert.That(function.DelegatesExecuted, Is.True);

      outParameter = function.OutParameter;
      outParameterArray = function.OutParameterArray;
    }

    protected void ExecuteDelegateInSubWxeFunctionWithParameters (
        ITransactionMode parentFunctionTransactionMode,
        ITransactionMode subFunctionTransactionMode,
        Action<WxeContext, DomainObjectParameterTestTransactedFunction> testDelegate,
        SampleObject inParameter,
        SampleObject[] inParameterArray,
        out SampleObject outParameter,
        out SampleObject[] outParameterArray)
    {
      var subFunction = new DomainObjectParameterTestTransactedFunction(
          subFunctionTransactionMode,
          testDelegate,
          inParameter,
          inParameterArray);

      var rootFunction = new TransactedFunctionWithChildFunction(parentFunctionTransactionMode, subFunction);
      rootFunction.Execute(Context);

      Assert.That(subFunction.DelegatesExecuted, Is.True);

      outParameter = subFunction.OutParameter;
      outParameterArray = subFunction.OutParameterArray;
    }
  }
}
