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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Web.IntegrationTests.TestDomain;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.IntegrationTests.WxeTransactedFunctionIntegrationTests.WxeFunctions
{
  [WxeDemandTargetMethodPermission("SecuredMethod", typeof(SecurableDomainObject), ParameterName = "SecurableParameter")]
  public class FunctionWithSecuredDomainObjectParameter : WxeFunction
  {
    public FunctionWithSecuredDomainObjectParameter (ITransactionMode transactionMode)
        : base(transactionMode)
    {
    }

    [WxeParameter(1, false, WxeParameterDirection.In)]
    public SecurableDomainObject SecurableParameter
    {
      get { return (SecurableDomainObject)Variables["SecurableParameter"]; }
      set { Variables["SecurableParameter"] = value; }
    }

    [UsedImplicitly]
    private void Step1 ()
    {
      // Nothing to do here - we just check whether execution throws an exception
    }
  }
}
