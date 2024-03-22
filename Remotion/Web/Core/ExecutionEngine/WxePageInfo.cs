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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxePageInfo : WxeTemplateControlInfo, IDisposable
  {
    /// <summary> A list of resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.Web.Globalization.WxePageInfo")]
    public enum ResourceIdentifier
    {
      /// <summary> Displayed when the user attempts to submit while the page is already aborting. </summary>
      StatusIsAbortingMessage,
      /// <summary> Displayed when the user returnes to a cached page that has already been submitted or aborted. </summary>
      StatusIsCachedMessage
    }

    public static readonly string ReturningTokenID = "wxeReturningTokenField";
    public static readonly string PageTokenID = "wxePageTokenField";
    public static readonly string PostBackSequenceNumberID = "wxePostBackSequenceNumberField";
    public static readonly string DmaPostBackSequenceNumberID = "dmaWxePostBackSequenceNumberField";

    private const int HttpStatusCode_ServerError = 500;
    private const int HttpStatusCode_NotFound = 404;

    private const string c_styleFileUrl = "ExecutionEngine.css";
    private const string c_styleFileUrlForIE = "ExecutionEngineIE.css";

    private static readonly string s_scriptFileKey = typeof(WxePageInfo).GetFullNameChecked() + "_Script";
    private static readonly string s_styleFileKey = typeof(WxePageInfo).GetFullNameChecked() + "_Style";
    private static readonly string s_styleFileKeyForIE = typeof(WxePageInfo).GetFullNameChecked() + "_StyleIE";

    private static IEnumerable<string> s_dirtyStatesForForCurrentFunctionAndRootFunction =
        new ReadOnlyCollection<string>(new[] { WxePageDirtyStates.CurrentFunction, WxePageDirtyStates.RootFunction });
    private static IEnumerable<string> s_dirtyStatesForCurrentFunction = EnumerableUtility.Singleton(WxePageDirtyStates.CurrentFunction);
    private static IEnumerable<string> s_dirtyStatesForRootFunction = EnumerableUtility.Singleton(WxePageDirtyStates.RootFunction);

    private readonly IWxePage _page;
    private WxeForm? _wxeForm;
    private WxeExecutor? _wxeExecutor;
    private bool _postbackCollectionInitialized = false;
    private bool _isPostDataHandled = false;
    private NameValueCollection? _postbackCollection = null;
    /// <summary> The <see cref="WxeFunctionState"/> designated by <b>WxeForm.ReturningToken</b>. </summary>
    private WxeFunctionState? _returningFunctionState = null;

    private bool _executeNextStep = false;

    private WebString _statusIsAbortingMessage = WebString.Empty;
    private WebString _statusIsCachedMessage = WebString.Empty;

    /// <summary> Initializes a new instance of the <b>WxePageInfo</b> type. </summary>
    /// <param name="page"> 
    ///   The <see cref="IWxePage"/> containing this <b>WxePageInfo</b> object. 
    ///   The page must be derived from <see cref="System.Web.UI.Page">System.Web.UI.Page</see>.
    /// </param>
    public WxePageInfo (IWxePage page)
      : base(page)
    {
      ArgumentUtility.CheckNotNull("page", page);
      _page = page;
    }

    /// <summary>
    /// Finds the control if it is the page's form. Otherwise, call the find method of the page's base class.
    /// </summary>
    public Control? FindControl (string id, out bool callBaseMethod)
    {
      if (_wxeForm != null && !string.IsNullOrEmpty(_wxeForm.ID) && id == _wxeForm.UniqueID)
      {
        callBaseMethod = false;
        return _wxeForm;
      }
      else
      {
        callBaseMethod = true;
        return null;
      }
    }

    public override void Initialize (HttpContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);
      base.Initialize(context);

      _wxeExecutor = new WxeExecutor(context, _page, this);

      _page.PreInit += HandlePagePreInit;
    }

    public NameValueCollection? EnsurePostBackModeDetermined (HttpContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      if (!_postbackCollectionInitialized)
      {
        Initialize(context);

        _postbackCollection = DeterminePostBackMode(context);
        _postbackCollectionInitialized = true;
        if (_postbackCollection != null)
          EnsurePostDataHandled(_postbackCollection);
      }
      return _postbackCollection;
    }

    private NameValueCollection? DeterminePostBackMode (HttpContext httpContext)
    {
      WxeContext? wxeContext = WxeContext.Current;
      if (wxeContext == null)
        return null;
      if (!CurrentPageStep.IsPostBack)
        return null;
      if (CurrentPageStep.PostBackCollection != null)
        return CurrentPageStep.PostBackCollection;
      if (httpContext.Request == null)
        return null;

      NameValueCollection collection;
      if (StringUtility.AreEqual(httpContext.Request.HttpMethod, "POST", false))
        collection = httpContext.Request.Form;
      else
        collection = httpContext.Request.QueryString;

      if ((collection[ControlHelper.ViewStateFieldPrefixID] == null) && (collection[ControlHelper.PostEventSourceID] == null))
        return null;
      else
        return collection;
    }

    private void HandlePagePreInit (object? sender, EventArgs eventArgs)
    {
      var existingForm = FindHtmlForm();
      // Can only be NULL without an exception during design mode
      if (existingForm == null)
        return;

      _wxeForm = WxeForm.Replace(existingForm);

      if (CurrentPageStep != null)
        _page.ClientScript.RegisterHiddenField(_page, WxePageInfo.PageTokenID, CurrentPageStep.PageToken);

      _wxeForm.LoadPostData += Form_LoadPostData;
    }

    private HtmlForm? FindHtmlForm ()
    {
      Control? page = _page.WrappedInstance;
      MemberInfo[] fields;
      do
      {
        fields = page.GetType().FindMembers(
            MemberTypes.Field,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            FindHtmlFormControlFilter,
            null);

        if (fields.Length == 0)
        {
          if (page is Page)
            page = ((Page)page).Master;
          else
            page = ((MasterPage)page).Master;
        }
      } while (fields.Length == 0 && page != null);

      if (fields.Length == 0)
      {
        throw new ApplicationException(
            message: "Page class " + _page.GetType().GetFullNameSafe() + " has no field of type HtmlForm. Please add a field or override property IWxePage.HtmlForm.");
      }
      else if (fields.Length > 1)
      {
        throw new ApplicationException(
            "Page class " + _page.GetType().GetFullNameSafe()
            + " has more than one field of type HtmlForm. Please remove excessive fields or override property IWxePage.HtmlForm.");
      }

      if (fields.Length == 1) // Can only be 0 without an exception during design mode
        return (HtmlForm?)((FieldInfo)fields[0]).GetValue(page);
      else
        return null;
    }

    private bool FindHtmlFormControlFilter (MemberInfo member, object? filterCriteria)
    {
      return (member is FieldInfo && ((FieldInfo)member).FieldType == typeof(HtmlForm));
    }

    private void Form_LoadPostData (object? sender, EventArgs e)
    {
      _page.Visible = true;

      NameValueCollection? postBackCollection = _page.GetPostBackCollection();
      if (postBackCollection == null)
        throw new InvalidOperationException("The IWxePage has no PostBackCollection even though this is a post back.");
      EnsurePostDataHandled(postBackCollection);

      if (CurrentPageStep.IsOutOfSequencePostBack && !_page.AreOutOfSequencePostBacksEnabled)
        throw new WxePostbackOutOfSequenceException();
    }

    protected void EnsurePostDataHandled (NameValueCollection postBackCollection)
    {
      if (_isPostDataHandled)
        return;

      _isPostDataHandled = true;
      HandleLoadPostData(postBackCollection);
    }

    /// <exception cref="WxePostbackOutOfSequenceException"> 
    ///   Thrown if a postback with an incorrect sequence number is handled. 
    /// </exception>
    protected virtual void HandleLoadPostData (NameValueCollection postBackCollection)
    {
      ArgumentUtility.CheckNotNull("postBackCollection", postBackCollection);

      WxeContext? wxeContext = WxeContext.Current;

      int postBackID = int.Parse(postBackCollection[WxePageInfo.PostBackSequenceNumberID]!); // TODO RM-8118: debug not null assertion
      if (postBackID != wxeContext!.PostBackID) // TODO RM-8118: not null assertion
        CurrentPageStep.SetIsOutOfSequencePostBack(true);

      string? returningToken = postBackCollection[WxePageInfo.ReturningTokenID];
      if (!string.IsNullOrEmpty(returningToken))
      {
        WxeFunctionStateManager functionStates = WxeFunctionStateManager.Current;
        WxeFunctionState? functionState = functionStates.GetItem(returningToken);
        if (functionState != null)
        {
          CurrentPageStep.SetReturnState(functionState.Function, true, null);
          _returningFunctionState = functionState;
        }
      }
    }

    public void OnPreRenderComplete ()
    {
      Assertion.DebugIsNotNull(_wxeForm, "_wxeForm must not be null.");

      var isPageDirty = WxePageStep.EvaluateDirtyStateOfPage(_page);
      CurrentPageStep.SetDirtyStateForCurrentPage(isPageDirty);

      WxeContext? wxeContext = WxeContext.Current;

      _page.ClientScript.RegisterHiddenField(_page, WxeHandler.Parameters.WxeFunctionToken, wxeContext!.FunctionToken); // TODO RM-8118: not null assertion
      _page.ClientScript.RegisterHiddenField(_page, WxePageInfo.ReturningTokenID, null);
      int currentPostBackID = wxeContext.PostBackID;
      int nextPostBackID = currentPostBackID + 1;
      _page.ClientScript.RegisterHiddenField(_page, WxePageInfo.PostBackSequenceNumberID, nextPostBackID.ToString());

      if (SafeServiceLocator.Current.GetInstance<IRenderingFeatures>().EnableDiagnosticMetadata)
        _page.ClientScript.RegisterHiddenField(_page, WxePageInfo.DmaPostBackSequenceNumberID, currentPostBackID.ToString());

      _page.ClientScript.RegisterClientScriptBlock(_page, typeof(WxePageInfo),  "wxeDoSubmit",
            "function wxeDoSubmit (button, pageToken) { \r\n"
          + "  var theForm = document." + _wxeForm.ClientID + "; \r\n"
          + "  theForm." + WxePageInfo.ReturningTokenID + ".value = pageToken; \r\n"
          + "  document.getElementById(button).click(); \r\n"
          + "}");

      _page.ClientScript.RegisterClientScriptBlock(_page, typeof(WxePageInfo), "wxeDoPostBack",
            "function wxeDoPostBack (control, argument, returningToken) { \r\n"
          + "  var theForm = document." + _wxeForm.ClientID + "; \r\n"
          + "  theForm." + WxePageInfo.ReturningTokenID + ".value = returningToken; \r\n"
          + "  __doPostBack (control, argument); \r\n"
          + "}");

      HtmlHeadAppender.Current.RegisterWebClientScriptInclude();

      var infrastructureResourceUrlFactory = SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>();
      var styleUrl = infrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink(s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);

      RegisterWxeInitializationScript();
      SetCacheSettings();
    }

    private void SetCacheSettings ()
    {
      WxeContext.Current!.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Private); // TODO RM-8118: not null assertion
    }

    private void RegisterWxeInitializationScript ()
    {
      IResourceManager resourceManager = GetResourceManager();

      WebString temp;
      WxeContext wxeContext = WxeContext.Current!; // TODO RM-8118: not null assertion

      int refreshInterval = 0;
      string refreshPath = "null";
      string abortPath = "null";
      if (_page.WxeHandler.IsSessionManagementEnabled)
      {
        //  Ensure the registration of "__doPostBack" on the page.
        _ = _page.ClientScript.GetPostBackEventReference(_page, null);

        bool isAbortEnabled = _page.IsAbortEnabled;

        string resumePath = wxeContext.GetPath(wxeContext.FunctionToken, null);

        if (_page.WxeHandler.IsSessionRefreshEnabled)
        {
          refreshInterval = _page.WxeHandler.RefreshInterval * 60000;
          refreshPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Refresh + "'";
        }

        if (isAbortEnabled)
          abortPath = "'" + resumePath + "&" + WxeHandler.Parameters.WxeAction + "=" + WxeHandler.Actions.Abort + "'";
      }

      string statusIsAbortingMessage = "null";
      string statusIsCachedMessage = "null";
      if (_page.AreStatusMessagesEnabled)
      {
        if (_page.StatusIsAbortingMessage.IsEmpty)
          temp = resourceManager.GetHtml(ResourceIdentifier.StatusIsAbortingMessage);
        else
          temp = _page.StatusIsAbortingMessage;
        statusIsAbortingMessage = "'" + ScriptUtility.EscapeClientScript(temp) + "'";

        if (_page.StatusIsCachedMessage.IsEmpty)
          temp = resourceManager.GetHtml(ResourceIdentifier.StatusIsCachedMessage);
        else
          temp = _page.StatusIsCachedMessage;
        statusIsCachedMessage = "'" + ScriptUtility.EscapeClientScript(temp) + "'";
      }

      _page.RegisterClientSidePageEventHandler(SmartPageEvents.OnLoading, "WxePage_OnLoading", "WxePage_OnLoading");
      _page.RegisterClientSidePageEventHandler(SmartPageEvents.OnLoaded, "WxePage_OnLoaded", "WxePage_OnLoaded");
      _page.RegisterClientSidePageEventHandler(SmartPageEvents.OnAbort, "WxePage_OnAbort", "WxePage_OnAbort");
      _page.RegisterClientSidePageEventHandler(SmartPageEvents.OnUnload, "WxePage_OnUnload", "WxePage_OnUnload");
      _page.CheckFormStateFunction = "WxePage_CheckFormState";

      string isCacheDetectionEnabled = _page.AreOutOfSequencePostBacksEnabled ? "false" : "true";

      StringBuilder initScript = new StringBuilder(500);

      string wxePostBackSequenceNumberFieldID = "'" + PostBackSequenceNumberID + "'";
      string dmaWxePostBackSequenceNumberFieldID = "null";

      if (SafeServiceLocator.Current.GetInstance<IRenderingFeatures>().EnableDiagnosticMetadata)
        dmaWxePostBackSequenceNumberFieldID = "'" + DmaPostBackSequenceNumberID + "'";

      initScript.AppendLine("WxePage_Context.SetInstance (new WxePage_Context (");
      initScript.Append("    ").Append(isCacheDetectionEnabled).AppendLine(",");
      initScript.Append("    ").Append(refreshInterval).AppendLine(",");
      initScript.Append("    ").Append(refreshPath).AppendLine(",");
      initScript.Append("    ").Append(abortPath).AppendLine(",");
      initScript.Append("    ").Append(statusIsAbortingMessage).AppendLine(",");
      initScript.Append("    ").Append(statusIsCachedMessage).AppendLine(",");
      initScript.Append("    ").Append(wxePostBackSequenceNumberFieldID).AppendLine(",");
      initScript.Append("    ").Append(dmaWxePostBackSequenceNumberFieldID).AppendLine("));");

      _page.ClientScript.RegisterClientScriptBlock(_page, typeof(WxePageInfo), "wxeInitialize", initScript.ToString());
    }


    /// <summary> Implements <see cref="IWxePage.StatusIsCachedMessage">IWxePage.StatusIsCachedMessage</see>. </summary>
    public WebString StatusIsCachedMessage
    {
      get { return _statusIsCachedMessage; }
      set { _statusIsCachedMessage = value; }
    }

    /// <summary> Implements <see cref="IWxePage.StatusIsAbortingMessage">IWxePage.StatusIsAbortingMessage</see>. </summary>
    public WebString StatusIsAbortingMessage
    {
      get { return _statusIsAbortingMessage; }
      set { _statusIsAbortingMessage = value; }
    }

    /// <summary> Implements <see cref="IWxePage.ExecuteNextStep">IWxePage.ExecuteNextStep</see>. </summary>
    public void ExecuteNextStep ()
    {
      var isPageDirty = WxePageStep.EvaluateDirtyStateOfPage(_page);
      CurrentPageStep.SetDirtyStateForCurrentPage(isPageDirty);

      _executeNextStep = true;
      _page.Visible = false; // suppress prerender and render events
    }


    /// <summary>
    ///   Implements <see cref="IWxePage.GetPermanentUrlParameters">IWxePage.GetPermanentUrlParameters()</see>.
    /// </summary>
    public NameValueCollection GetPermanentUrlParameters ()
    {
      NameValueCollection urlParameters = CurrentPageFunction.VariablesContainer.SerializeParametersForQueryString();

      ISmartNavigablePage? smartNavigablePage = _page as ISmartNavigablePage;
      if (smartNavigablePage != null)
        NameValueCollectionUtility.Append(urlParameters, smartNavigablePage.GetNavigationUrlParameters());

      return urlParameters;
    }

    /// <summary>
    ///   Implements <see cref="IWxePage.GetPermanentUrl()">IWxePage.GetPermanentUrl()</see>.
    /// </summary>
    public string GetPermanentUrl ()
    {
      return GetPermanentUrl(CurrentPageFunction.GetType(), GetPermanentUrlParameters());
    }

    /// <summary>
    ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.GetPermanentUrl(System.Collections.Specialized.NameValueCollection)">IWxePage.GetPermanentUrl(NameValueCollection)</see>.
    /// </summary>
    public string GetPermanentUrl (NameValueCollection urlParameters)
    {
      return GetPermanentUrl(CurrentPageFunction.GetType(), urlParameters);
    }

    /// <summary>
    ///   Implements <see cref="M:Remotion.Web.ExecutionEngine.IWxePage.GetPermanentUrl(System.Type,System.Collections.Specialized.NameValueCollection)">IWxePage.GetPermanentUrl(Type,NameValueCollection)</see>.
    /// </summary>
    public string GetPermanentUrl (Type functionType, NameValueCollection urlParameters)
    {
      return WxeContext.Current!.GetPermanentUrl(functionType, urlParameters); // TODO RM-8118: not null assertion
    }

    /// <summary> Implements <see cref="IWxePage.IsOutOfSequencePostBack">IWxePage.IsOutOfSequencePostBack</see>. </summary>
    public bool IsOutOfSequencePostBack
    {
      get { return CurrentPageStep.IsOutOfSequencePostBack; }
    }

    /// <summary> Implements <see cref="IWxePage.IsReturningPostBack">IWxePage.IsReturningPostBack</see>. </summary>
    [MemberNotNullWhen(true, nameof(ReturningFunction))]
    public bool IsReturningPostBack
    {
      get { return CurrentPageStep.IsReturningPostBack; }
    }

    /// <summary> Implements <see cref="IWxePage.ReturningFunction">IWxePage.ReturningFunction</see>. </summary>
    public WxeFunction? ReturningFunction
    {
      get { return CurrentPageStep.ReturningFunction; }
    }

    /// <summary> Saves the viewstate into the executing <see cref="WxePageStep"/>. </summary>
    /// <param name="state"> An <b>ASP.NET</b> viewstate object. </param>
    public void SavePageStateToPersistenceMedium (object state)
    {
      CurrentPageStep.SavePageStateToPersistenceMedium(state);
    }

    /// <summary> Returns the viewstate previously saved into the executing <see cref="WxePageStep"/>. </summary>
    /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
    public object LoadPageStateFromPersistenceMedium ()
    {
      return CurrentPageStep.LoadPageStateFromPersistenceMedium();
    }


    /// <summary> 
    ///   If <see cref="ExecuteNextStep"/> has been called prior to disposing the page, <b>Dispose</b> will
    ///   break execution of this page life cycle and allow the Execution Engine to continue with the next step.
    /// </summary>
    /// <remarks> 
    ///   <para>
    ///     If <see cref="ExecuteNextStep"/> has been called, <b>Dispose</b> clears the <see cref="HttpResponse"/>'s
    ///     output and ends the execution of the current step by throwing a <see cref="WxeExecuteNextStepException"/>. 
    ///     This exception is handled by the Execution Engine framework.
    ///   </para>
    ///   <note>
    ///     See the remarks section of <see cref="IWxePage"/> for details on calling <b>Dispose</b>.
    ///   </note>
    /// </remarks>
    public void Dispose ()
    {
      HttpContext? httpContext = null;
      if (_wxeExecutor != null)
      {
        httpContext = _wxeExecutor.HttpContext;
        ((IDisposable)_wxeExecutor).Dispose();
      }

      if (_returningFunctionState != null)
      {
        bool isRootFunction = _returningFunctionState.Function == _returningFunctionState.Function.RootFunction;
        if (isRootFunction)
          WxeFunctionStateManager.Current.Abort(_returningFunctionState);
      }

      if (_executeNextStep)
      {
        if (httpContext != null)
          httpContext.Response.Clear(); // throw away page trace output
        throw new WxeExecuteNextStepException();
      }
    }

    [JetBrains.Annotations.NotNull]
    public Exception WrapProcessRequestException ([JetBrains.Annotations.NotNull] HttpException exception)
    {
      ArgumentUtility.CheckNotNull("exception", exception);

      if (exception.GetHttpCode() == HttpStatusCode_NotFound)
        return new WxeResourceNotFoundException("Resource not found.", exception);

      return new WxeHttpExceptionPreservingException(exception);
    }

    public WxeForm? WxeForm
    {
      get { return _wxeForm; }
    }


    /// <summary> Find the <see cref="IResourceManager"/> for this WxePageInfo. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    public IEnumerable<string> GetDirtyStates (IReadOnlyCollection<string>? requestedStates)
    {
      var isDirtyStateForCurrentFunctionRequested =
          requestedStates == null
          || s_dirtyStatesForCurrentFunction.Intersect(requestedStates, StringComparer.InvariantCultureIgnoreCase).Any();

      var isDirtyStateForRootFunctionRequested =
          requestedStates == null
          || s_dirtyStatesForRootFunction.Intersect(requestedStates, StringComparer.InvariantCultureIgnoreCase).Any();

      var isCurrentFunctionDirty = isDirtyStateForCurrentFunctionRequested && CurrentFunction.EvaluateDirtyState();

      var isRootFunctionDirty =
          isDirtyStateForRootFunctionRequested
          && (CurrentFunction.RootFunction?.EvaluateDirtyState() ?? false);

      if (isCurrentFunctionDirty && isRootFunctionDirty)
        return s_dirtyStatesForForCurrentFunctionAndRootFunction;

      if (isCurrentFunctionDirty)
        return s_dirtyStatesForCurrentFunction;

      if (isRootFunctionDirty)
        return s_dirtyStatesForRootFunction;

      return Enumerable.Empty<string>();
    }

    public bool IsDirtyStateEnabled
    {
      get { return CurrentPageStep.IsDirtyStateEnabled; }
    }

    private NameObjectCollection WindowState
    {
      get
      {
        NameObjectCollection? windowState =
            (NameObjectCollection?)CurrentPageFunction.RootFunction!.Variables["WxeWindowState"];
        if (windowState == null)
        {
          windowState = new NameObjectCollection();
          CurrentPageFunction.RootFunction.Variables["WxeWindowState"] = windowState;
        }
        return windowState;
      }
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.IWindowStateManager.GetData">Remotion.Web.UI.IWindowStateManager.GetData</see>.
    /// </summary>
    public object? GetData (string key)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);
      return WindowState[key];
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.IWindowStateManager.SetData">Remotion.Web.UI.IWindowStateManager.SetData</see>.
    /// </summary>
    public void SetData (string key, object? value)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);
      WindowState[key] = value;
    }

    public WxeExecutor Executor
    {
      get { return _wxeExecutor!; } // TODO RM-8118: debug not null
    }
  }
}
