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
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class SecuredFunctionViaDomainObjectHandleParameterTest : SecuredFunctionTestBase
  {
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectHandleParameter_WithObjectHasAccessTrue_Succeeds ()
    {
      var wxeFunction = CreateWxeFunction(_clientTransaction);
      ObjectSecurityStrategyStub
          .Setup(
              stub => stub.HasAccess(
                  SecurityProviderStub.Object,
                  SecurityPrincipalStub.Object,
                  new[] { TestAccessTypeValue }))
          .Returns(true);

      wxeFunction.Execute(Context);
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectHandleParameter_WithObjectHasAccessFalse_Fails ()
    {
      var wxeFunction = CreateWxeFunction(_clientTransaction);
      ObjectSecurityStrategyStub
          .Setup(
              stub => stub.HasAccess(
                  SecurityProviderStub.Object,
                  SecurityPrincipalStub.Object,
                  new[] { TestAccessTypeValue }))
          .Returns(false);

      Assert.That(
          () => wxeFunction.Execute(Context),
          Throws.TypeOf<WxeUnhandledException>().With.InnerException.TypeOf<PermissionDeniedException>());
    }

    [Test]
    public void HasAccess_ViaDomainObjectHandleParameter_WithFunctionalHasAccessTrue_ReturnsTrue ()
    {
      FunctionalSecurityStrategyStub
          .Setup(
              stub => stub.HasAccess(
                  typeof(SecurableDomainObject),
                  SecurityProviderStub.Object,
                  SecurityPrincipalStub.Object,
                  new[] { TestAccessTypeValue }))
          .Returns(true);

      Assert.That(WxeFunction.HasAccess(typeof(FunctionWithSecuredDomainObjectHandleParameter)), Is.True);
    }

    [Test]
    public void HasAccess_ViaDomainObjectHandleParameter_WithFunctionalHasAccessFalse_ReturnsFalse ()
    {
      FunctionalSecurityStrategyStub
          .Setup(
              stub => stub.HasAccess(
                  typeof(SecurableDomainObject),
                  SecurityProviderStub.Object,
                  SecurityPrincipalStub.Object,
                  new[] { TestAccessTypeValue }))
          .Returns(false);

      Assert.That(WxeFunction.HasAccess(typeof(FunctionWithSecuredDomainObjectHandleParameter)), Is.False);
    }

    private FunctionWithSecuredDomainObjectHandleParameter CreateWxeFunction (ClientTransaction clientTransaction)
    {
      var securableDomainObject = CreateSecurableDomainObject(clientTransaction);

      var mode = CreateTransactionModeForClientTransaction(clientTransaction);

      var wxeFunction = new FunctionWithSecuredDomainObjectHandleParameter(mode);
      wxeFunction.SecurableParameter = securableDomainObject.GetHandle();
      return wxeFunction;
    }
  }
}
