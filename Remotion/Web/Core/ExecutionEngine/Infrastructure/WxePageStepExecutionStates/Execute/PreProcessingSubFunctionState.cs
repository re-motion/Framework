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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  /// <summary>
  /// The <see cref="PreProcessingSubFunctionState"/> is responsible for setting up the current <see cref="WxeStep"/> to execute the sub-function.
  /// Executing this state will transition the <see cref="IExecutionStateContext"/> into the <see cref="PreparingRedirectToSubFunctionState"/> if the
  /// sub-function has a perma-URL and into the <see cref="ExecutingSubFunctionWithoutPermaUrlState"/> otherwise.
  /// </summary>
  public class PreProcessingSubFunctionState : ExecutionStateBase<PreProcessingSubFunctionStateParameters>
  {
    private readonly WxeRepostOptions _repostOptions;

    public PreProcessingSubFunctionState (
        IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters, WxeRepostOptions repostOptions)
        : base(executionStateContext, parameters)
    {
      _repostOptions = repostOptions;
    }

    public WxeRepostOptions RepostOptions
    {
      get { return _repostOptions; }
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      Parameters.SubFunction.SetParentStep(ExecutionStateContext.CurrentStep);
      NameValueCollection postBackCollection = BackupPostBackCollection();
      EnsureSenderPostBackRegistration(postBackCollection);

      Parameters.Page.SaveAllState();

      if (Parameters.PermaUrlOptions.UsePermaUrl)
      {
        var parameters = new PreparingRedirectToSubFunctionStateParameters(Parameters.SubFunction, postBackCollection, Parameters.PermaUrlOptions);
        ExecutionStateContext.SetExecutionState(new PreparingRedirectToSubFunctionState(ExecutionStateContext, parameters));
      }
      else
      {
        var parameters = new ExecutionStateParameters(Parameters.SubFunction, postBackCollection);
        ExecutionStateContext.SetExecutionState(new ExecutingSubFunctionWithoutPermaUrlState(ExecutionStateContext, parameters));
      }
    }

    private NameValueCollection BackupPostBackCollection ()
    {
      var postBackCollection = Assertion.IsNotNull(Parameters.Page.GetPostBackCollection()).Clone();

      if (_repostOptions.SuppressesRepost)
      {
        if (_repostOptions.UsesEventTarget)
        {
          postBackCollection.Remove(ControlHelper.PostEventSourceID);
          postBackCollection.Remove(ControlHelper.PostEventArgumentID);
        }
        else
          postBackCollection.Remove(_repostOptions.Sender!.UniqueID); // TODO RM-8118: not null assertion
      }
      return postBackCollection;
    }

    private void EnsureSenderPostBackRegistration (NameValueCollection postBackCollection)
    {
      if (_repostOptions.SuppressesRepost)
        return;

      if (_repostOptions.Sender is IPostBackDataHandler && postBackCollection[_repostOptions.Sender.UniqueID] == null)
        Parameters.Page.RegisterRequiresPostBack(_repostOptions.Sender);
    }
  }
}
