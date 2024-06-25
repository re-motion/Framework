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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  /// <summary>
  /// The <see cref="PostProcessingSubFunctionState"/> is responsible for setting the current <see cref="WxeStep"/>'s state after the sub-function
  /// has completed execution. Executing this state will transition the <see cref="IExecutionStateContext"/> into the 
  /// <see cref="NullExecutionState"/>.
  /// </summary>
  public class PostProcessingSubFunctionState : ExecutionStateBase<ExecutionStateParameters>
  {
    public PostProcessingSubFunctionState (IExecutionStateContext executionStateContext, ExecutionStateParameters parameters)
        : base(executionStateContext, parameters)
    {
    }

    //TODO: CleanUp duplication with other PostProcessSubFunction-implemenations
    public override void ExecuteSubFunction (WxeContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      bool isPostRequest = string.Equals(context.HttpContext.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase);
      if (isPostRequest)
      {
        //  Provide the executed sub-function to the executing page and use original postback data
        ExecutionStateContext.SetReturnState(Parameters.SubFunction, false, null);
      }
      else
      {
        // Correct the PostBack-Sequence number
        Parameters.PostBackCollection[WxePageInfo.PostBackSequenceNumberID] = context.PostBackID.ToString();

        //  Provide the executed sub-function and the backed up postback data to the executing page
        ExecutionStateContext.SetReturnState(Parameters.SubFunction, true, Parameters.PostBackCollection);
      }

      ExecutionStateContext.SetExecutionState(NullExecutionState.Null);
    }
  }
}
