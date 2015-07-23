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
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.ControlReplacing;
using ExecuteByRedirect_PreProcessingSubFunctionState = 
  Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.ExecuteExternalByRedirect.PreProcessingSubFunctionState;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary> This step interrupts the server side execution to display a page to the user. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Class/*' />
  [Serializable]
  public class WxePageStep : WxeStep, IExecutionStateContext
  {
    private IWxePageExecutor _pageExecutor = new WxePageExecutor();
    private readonly ResourceObjectBase _page;
    private readonly string _pageToken;
    private string _pageState;
    private bool _isPostBack;
    private bool _isOutOfSequencePostBack;
    private bool _isExecutionStarted;
    private bool _isReturningPostBack;
    private WxeFunction _returningFunction;
    private NameValueCollection _postBackCollection;

    [NonSerialized]
    private WxeHandler _wxeHandler;

    private IExecutionState _executionState = NullExecutionState.Null;
    private IUserControlExecutor _userControlExecutor = NullUserControlExecutor.Null;

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="page"]' />
    public WxePageStep (string page)
      : this (new ResourceObject (ArgumentUtility.CheckNotNullOrEmpty("page", page)))
    {
    }

    /// <summary> Initializes a new instance of the <b>WxePageStep</b> type. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Ctor/param[@name="pageref"]' />
    public WxePageStep (WxeVariableReference pageref)
        : this (new ResourceObjectWithVarRef (pageref))
    {
    }

    protected WxePageStep (ResourceObjectBase page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      _page = page;
      _pageToken = Guid.NewGuid().ToString();
   }

    /// <summary> The URL of the page to be displayed by this <see cref="WxePageStep"/>. </summary>
    public string Page
    {
      get { return _page.GetResourcePath (Variables); }
    }

    /// <summary> Gets the currently executing <see cref="WxeStep"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/ExecutingStep/*' />
    public override WxeStep ExecutingStep
    {
      get
      {
        if (_executionState.IsExecuting)
          return _executionState.Parameters.SubFunction.ExecutingStep;
        else
          return _userControlExecutor.ExecutingStep ?? this;
      }
    }

    /// <summary> 
    ///   Displays the <see cref="WxePageStep"/>'s page or the sub-function that has been invoked by the 
    ///   <see cref="ExecuteFunction(Infrastructure.WxePageStepExecutionStates.PreProcessingSubFunctionStateParameters,Infrastructure.WxeRepostOptions)"/> method.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/Execute/*' />
    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (_wxeHandler != null)
      {
        context.HttpContext.Handler = _wxeHandler;
        _wxeHandler = null;
      }

      if (!_isExecutionStarted)
      {
        _isExecutionStarted = true;
        _isPostBack = false;
      }
      else
      {
        _isPostBack = true;
      }

      ClearIsOutOfSequencePostBack();
      ClearReturnState();

      while (_executionState.IsExecuting)
        _executionState.ExecuteSubFunction (context);

      _userControlExecutor.Execute (context);

      try
      {
        _pageExecutor.ExecutePage (context, Page, _isPostBack);
      }
      finally
      {
        if (_userControlExecutor.IsReturningPostBack)
          _userControlExecutor = NullUserControlExecutor.Null;

        ClearIsOutOfSequencePostBack();
        ClearReturnState();
      }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (PreProcessingSubFunctionStateParameters parameters, WxeRepostOptions repostOptions)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("repostOptions", repostOptions);

      if (_executionState.IsExecuting)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      _wxeHandler = parameters.Page.WxeHandler;

      _executionState = new PreProcessingSubFunctionState (this, parameters, repostOptions);
      Execute();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunctionExternalByRedirect (PreProcessingSubFunctionStateParameters parameters, WxeReturnOptions returnOptions)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      ArgumentUtility.CheckNotNull ("returnOptions", returnOptions);

      if (_executionState.IsExecuting)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      _wxeHandler = parameters.Page.WxeHandler;

      _executionState = new ExecuteByRedirect_PreProcessingSubFunctionState (this, parameters, returnOptions);
      Execute();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (WxeUserControl userControl, WxeFunction subFunction, Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull ("userControl", userControl);
      ArgumentUtility.CheckNotNull ("subFunction", subFunction);
      ArgumentUtility.CheckNotNull ("sender", sender);

      IWxePage wxePage = userControl.WxePage;
      _wxeHandler = wxePage.WxeHandler;

      _userControlExecutor = new UserControlExecutor (this, userControl, subFunction, sender, usesEventTarget);

      IReplaceableControl replaceableControl = userControl;
      replaceableControl.Replacer.Controls.Clear();
      wxePage.SaveAllState();

      Execute();
    }

    /// <summary> Gets the token for this page step. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePageStep.xml' path='WxePageStep/PageToken/*' />
    public string PageToken
    {
      get { return _pageToken; }
    }

    /// <summary>
    ///   Gets a flag that corresponds to the <see cref="System.Web.UI.Page.IsPostBack">Page.IsPostBack</see> flag, but is 
    ///   available from the beginning of the execution cycle, i.e. even before <b>OnInit</b>.
    /// </summary>
    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    /// <summary>
    ///   Gets a flag that describes whether the current postback cycle was caused by resubmitting a page from the 
    ///   client's cache.
    /// </summary>
    public bool IsOutOfSequencePostBack
    {
      get { return _isOutOfSequencePostBack; }
    }

    /// <summary>
    ///   During the execution of a page, specifies whether the current postback cycle was caused by returning from a 
    ///   <see cref="WxeFunction"/>.
    /// </summary>
    public bool IsReturningPostBack
    {
      get { return _isReturningPostBack; }
    }

    public WxeFunction ReturningFunction
    {
      get { return _returningFunction; }
    }

    /// <summary> Gets or sets the postback data for the page if it has executed a sub-function. </summary>
    /// <value> The postback data generated during the roundtrip that led to the execution of the sub-function. </value>
    /// <remarks> 
    ///   <para>
    ///     This property is used only for transfering the postback data from the backup location to the page's
    ///     initialization infrastructure.
    ///   </para><para>
    ///     Application developers should only use the 
    ///     <see cref="ISmartPage.GetPostBackCollection">ISmartPage.GetPostBackCollection</see> method to access the
    ///     postback data.
    ///   </para><para>
    ///     Control developers should either implement <see cref="System.Web.UI.IPostBackDataHandler"/> to access 
    ///     postback data relevant to their control or, if they develop a composite control, use the child controls' 
    ///     integrated data handling features to access the data.
    ///   </para>
    /// </remarks>
    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }

    public void SetPostBackCollection (NameValueCollection postBackCollection)
    {
      _postBackCollection = postBackCollection;
    }

    public void SetReturnState (WxeFunction returningFunction, bool isReturningPostBack, NameValueCollection previousPostBackCollection)
    {
      ArgumentUtility.CheckNotNull ("returningFunction", returningFunction);

      _returningFunction = returningFunction;
      _isReturningPostBack = isReturningPostBack;
      _postBackCollection = previousPostBackCollection;
    }

    private void ClearReturnState ()
    {
      _returningFunction = null;
      _isReturningPostBack = false;
      _postBackCollection = null;
    }

    public void SetIsOutOfSequencePostBack (bool value)
    {
      _isOutOfSequencePostBack = value;
    }
    
    private void ClearIsOutOfSequencePostBack ()
    {
      _isOutOfSequencePostBack = false;
    }

    public override string ToString ()
    {
      return "WxePageStep: " + Page;
    }

    /// <summary> Saves the passed <paramref name="state"/> object into the <see cref="WxePageStep"/>. </summary>
    /// <param name="state"> An <b>ASP.NET</b> viewstate object. </param>
    public void SavePageStateToPersistenceMedium (object state)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      _pageState = writer.ToString();
    }

    /// <summary> 
    ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
    /// </summary>
    /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
    public object LoadPageStateFromPersistenceMedium ()
    {
      LosFormatter formatter = new LosFormatter();
      return formatter.Deserialize (_pageState);
    }

    /// <summary> 
    ///   Aborts the <see cref="WxePageStep"/>. Aborting will cascade to any <see cref="WxeFunction"/> executed
    ///   in the scope of this step if they are part of the same hierarchy, i.e. share a common <see cref="WxeStep.RootFunction"/>.
    /// </summary>
    protected override void AbortRecursive ()
    {
      base.AbortRecursive();
      if (_executionState.IsExecuting && _executionState.Parameters.SubFunction.RootFunction == this.RootFunction)
        _executionState.Parameters.SubFunction.Abort();
    }

    public IWxePageExecutor PageExecutor
    {
      get { return _pageExecutor; }
    }

    public IUserControlExecutor UserControlExecutor
    {
      get { return _userControlExecutor; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetPageExecutor (IWxePageExecutor pageExecutor)
    {
      ArgumentUtility.CheckNotNull ("pageExecutor", pageExecutor);
      _pageExecutor = pageExecutor;
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetUserControlExecutor (IUserControlExecutor userControlExecutor)
    {
      ArgumentUtility.CheckNotNull ("userControlExecutor", userControlExecutor);
      _userControlExecutor = userControlExecutor;
    }

    WxeStep IExecutionStateContext.CurrentStep
    {
      get { return this; }
    }

    WxeFunction IExecutionStateContext.CurrentFunction
    {
      get { return ParentFunction; }
    }

    IExecutionState IExecutionStateContext.ExecutionState
    {
      get { return _executionState; }
    }

    void IExecutionStateContext.SetExecutionState (IExecutionState executionState)
    {
      ArgumentUtility.CheckNotNull ("executionState", executionState);

      _executionState = executionState;
    }
  }
}
