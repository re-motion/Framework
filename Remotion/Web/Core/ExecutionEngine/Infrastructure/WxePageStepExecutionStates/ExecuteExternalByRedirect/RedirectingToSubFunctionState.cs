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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  public class RedirectingToSubFunctionState : ExecutionStateBase<RedirectingToSubFunctionStateParameters>
  {
    /// <summary>
    /// The <see cref="RedirectingToSubFunctionState"/> is responsible for redirecting the user's client to a sub-function's Perma-URL.
    /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="PostProcessingSubFunctionState"/>.
    /// </summary>
    public RedirectingToSubFunctionState (IExecutionStateContext executionStateContext, RedirectingToSubFunctionStateParameters parameters)
        : base(executionStateContext, parameters)
    {
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      try
      {
        context.HttpContext.Response.Redirect(Parameters.DestinationUrl);
        throw new InvalidOperationException(string.Format("Redirect to '{0}' failed.", Parameters.DestinationUrl));
      }
      catch (ThreadAbortException)
      {
        ExecutionStateContext.SetExecutionState(new PostProcessingSubFunctionState(ExecutionStateContext, Parameters));
        throw;
      }
    }
  }
}
