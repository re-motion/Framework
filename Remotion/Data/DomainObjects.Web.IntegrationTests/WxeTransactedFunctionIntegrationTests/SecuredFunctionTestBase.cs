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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UI;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  public class SecuredFunctionTestBase : WxeTransactedFunctionIntegrationTestBase
  {
    private ISecurityProvider _securityProviderStub;
    private ISecurityPrincipal _securityPrincipalStub;
    private IFunctionalSecurityStrategy _functionalSecurityStrategyStub;
    private IObjectSecurityStrategy _objectSecurityStrategyStub;
    private AccessType _testAccessTypeValue;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp ();

      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      _securityProviderStub.Stub (stub => stub.IsNull).Return (false);

      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      var principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider> ();
      principalProviderStub.Stub (stub => stub.GetPrincipal ()).Return (_securityPrincipalStub);

      _functionalSecurityStrategyStub = MockRepository.GenerateStub<IFunctionalSecurityStrategy> ();
      _objectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy> ();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _securityProviderStub);
      serviceLocator.RegisterSingle (() => principalProviderStub);
      serviceLocator.RegisterSingle (() => _functionalSecurityStrategyStub);
      serviceLocator.RegisterMultiple<IWebSecurityAdapter>();
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter> (() => new WxeSecurityAdapter());
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      _testAccessTypeValue = AccessType.Get (TestAccessTypes.Value);
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();

      base.TearDown ();
    }

    protected ISecurityProvider SecurityProviderStub
    {
      get { return _securityProviderStub; }
    }

    protected ISecurityPrincipal SecurityPrincipalStub
    {
      get { return _securityPrincipalStub; }
    }

    protected IFunctionalSecurityStrategy FunctionalSecurityStrategyStub
    {
      get { return _functionalSecurityStrategyStub; }
    }

    protected IObjectSecurityStrategy ObjectSecurityStrategyStub
    {
      get { return _objectSecurityStrategyStub; }
    }

    protected AccessType TestAccessTypeValue
    {
      get { return _testAccessTypeValue; }
    }

    protected SecurableDomainObject CreateSecurableDomainObject (ClientTransaction clientTransaction = null)
    {
      clientTransaction = clientTransaction ?? ClientTransaction.CreateRootTransaction();

      var securableDomainObject =
          (SecurableDomainObject) LifetimeService.NewObject (clientTransaction, typeof (SecurableDomainObject), ParamList.Empty);
      securableDomainObject.SecurableType = typeof (SecurableDomainObject);
      securableDomainObject.SecurityStrategy = _objectSecurityStrategyStub;
      return securableDomainObject;
    }

    protected static ITransactionMode CreateTransactionModeForClientTransaction (ClientTransaction clientTransaction)
    {
      var mode = MockRepository.GenerateStub<ITransactionMode>();
      mode.Stub (stub => stub.CreateTransactionStrategy (Arg<WxeFunction>.Is.Anything, Arg<WxeContext>.Is.Anything))
          .Do (
              (Func<WxeFunction, WxeContext, TransactionStrategyBase>)
              ((function, context) => new RootTransactionStrategy (false, clientTransaction.ToITransaction, NullTransactionStrategy.Null, function)));
      return mode;
    }
  }
}