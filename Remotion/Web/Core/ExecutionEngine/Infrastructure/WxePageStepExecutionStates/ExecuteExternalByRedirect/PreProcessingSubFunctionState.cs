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
using System.Collections.Specialized;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect
{
  /// <summary>
  /// The <see cref="PreProcessingSubFunctionState"/> is responsible for setting up the current <see cref="WxeStep"/> to execute the sub-function.
  /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="PreparingRedirectToSubFunctionState"/> .
  /// </summary>
  public class PreProcessingSubFunctionState : ExecutionStateBase<PreProcessingSubFunctionStateParameters>
  {
    private readonly WxeReturnOptions _returnOptions;

    public PreProcessingSubFunctionState (
        IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters, WxeReturnOptions returnOptions)
        : base(executionStateContext, parameters)
    {
      ArgumentUtility.CheckNotNull("returnOptions", returnOptions);
      _returnOptions = returnOptions;
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      NameValueCollection postBackCollection = BackupPostBackCollection();
      Parameters.Page.SaveAllState();

      var parameters = new PreparingRedirectToSubFunctionStateParameters(Parameters.SubFunction, postBackCollection, Parameters.PermaUrlOptions);
      ExecutionStateContext.SetExecutionState(new PreparingRedirectToSubFunctionState(ExecutionStateContext, parameters, _returnOptions));
    }

    public WxeReturnOptions ReturnOptions
    {
      get { return _returnOptions; }
    }

    private NameValueCollection BackupPostBackCollection ()
    {
      return Parameters.Page.GetPostBackCollection()!.Clone(); // TODO RM-8118: not null assertion
    }
  }
}
