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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.SmartPageImplementation
{
  public class SmartPageInfo
  {
    /// <summary> A list of resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.Web.Globalization.SmartPageInfo")]
    public enum ResourceIdentifier
    {
      /// <summary> Displayed when the user attempts to leave the page. </summary>
      AbortMessage,
      /// <summary> Displayed when the user attempts to submit while the page is already submitting. </summary>
      StatusIsSubmittingMessage
    }

    public static readonly string CacheDetectionID = "SmartPage_CacheDetectionField";
    private const string c_smartPageTokenID = "SmartPage_TokenField";
    private const string c_smartScrollingID = "smartScrolling";
    private const string c_smartFocusID = "smartFocus";
    private const string c_styleFileUrl = "SmartPage.css";

    private static readonly DateTime s_pageTokenStartDate = new(2022, 01, 01);
    private static readonly string s_scriptFileKey = typeof(SmartPageInfo).GetFullNameChecked() + "_Script";
    private static readonly string s_styleFileKey = typeof(SmartPageInfo).GetFullNameChecked() + "_Style";
    private static readonly string s_smartNavigationScriptKey = typeof(SmartPageInfo).GetFullNameChecked() + "_SmartNavigation";
    private static IEnumerable<string> s_dirtyStateForCurrentPage = EnumerableUtility.Singleton(SmartPageDirtyStates.CurrentPage);
    private static IEnumerable<string> s_dirtyStateForCurrentPageOnServerOrClient = new[] { SmartPageDirtyStates.CurrentPage, SmartPageDirtyStates.ClientSide };

    private readonly ISmartPage _page;

    private SmartNavigationData _smartNavigationDataToBeDisacarded;
    private string? _smartFocusID;
    private PlainTextString _abortMessage = PlainTextString.Empty;
    private WebString _statusIsSubmittingMessage = WebString.Empty;

    private bool _isPreRenderComplete;

    private readonly AutoInitDictionary<SmartPageEvents, NameValueCollection> _clientSideEventHandlers =
        new AutoInitDictionary<SmartPageEvents, NameValueCollection>();

    private string? _checkFormStateFunction;
    private readonly HashSet<IEditableControl> _trackedControls = new HashSet<IEditableControl>();
    private readonly StringCollection _trackedControlsByID = new StringCollection();
    private readonly HashSet<INavigationControl> _navigationControls = new HashSet<INavigationControl>();
    private readonly List<Tuple<Control, string>> _synchronousPostBackCommands = new List<Tuple<Control, string>>();

    private ResourceManagerSet? _cachedResourceManager;

    public SmartPageInfo (ISmartPage page)
    {
      ArgumentUtility.CheckNotNullAndType<Page>("page", page);
      _page = page;
      _page.Init += Page_Init;
      // PreRenderComplete-handler must be registered before ScriptManager registers its own PreRenderComplete-handler during OnInit.
      _page.PreRenderComplete += Page_PreRenderComplete;
    }

    /// <summary> Implements <see cref="ISmartPage.RegisterClientSidePageEventHandler">ISmartPage.RegisterClientSidePageEventHandler</see>. </summary>
    public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
    {
      ArgumentUtility.CheckNotNullOrEmpty("key", key);
      ArgumentUtility.CheckNotNullOrEmpty("function", function);
      if (! Regex.IsMatch(function, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
        throw new ArgumentException("Invalid function name: '" + function + "'.", "function");

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException(
            "RegisterClientSidePageEventHandler must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

#pragma warning disable 618
      if (pageEvent == SmartPageEvents.OnLoad)
        pageEvent = SmartPageEvents.OnLoaded;
#pragma warning restore 618

      NameValueCollection eventHandlers = _clientSideEventHandlers[pageEvent];
      var value = eventHandlers[key];

      if (value is null)
        eventHandlers[key] = function;
    }


    /// <summary> Implements <see cref="ISmartPage.RegisterControlForDirtyStateTracking">ISmartPage.RegisterClientSidePageEventHandler</see>. </summary>
    public void RegisterControlForDirtyStateTracking (IEditableControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException(
            "RegisterControlForDirtyStateTracking must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      // Registration is typically done during the Control's init-phase to allow the page's code-behind logic access to the complete dirty state.
      _trackedControls.Add(control);
      control.Unload += UnregisterControlForDirtyStateTracking;
    }

    private void UnregisterControlForDirtyStateTracking (object? sender, EventArgs args)
    {
      var control = ArgumentUtility.CheckNotNullAndType<IEditableControl>("sender", sender!);

      if (_isPreRenderComplete)
      {
        // After the PreRender phase in the page's lifecycle, controls will only get removed when the page is disposed.
        // We no longer require cleanup for the controls at this point.
        return;
      }

      _trackedControls.Remove(control);
      control.Unload -= UnregisterControlForDirtyStateTracking;
    }

    /// <summary> Implements <see cref="ISmartPage.RegisterControlForDirtyStateTracking">ISmartPage.RegisterControlForDirtyStateTracking</see>. </summary>
    public void RegisterControlForDirtyStateTracking (string clientID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("clientID", clientID);

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException(
            "RegisterControlForDirtyStateTracking must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      if (! _trackedControlsByID.Contains(clientID))
        _trackedControlsByID.Add(clientID);
    }

    public IEnumerable<string> GetDirtyStates (IReadOnlyCollection<string>? requestedStates)
    {
      var isDirtyStateForCurrentPageRequested =
          requestedStates == null
          || s_dirtyStateForCurrentPage.Intersect(requestedStates, StringComparer.InvariantCultureIgnoreCase).Any();

      if (_page.IsDirtyStateEnabled && isDirtyStateForCurrentPageRequested)
      {
        if (_page.IsDirty)
          return s_dirtyStateForCurrentPage;

        if (_trackedControls.Any(c => c.IsDirty))
          return s_dirtyStateForCurrentPage;
      }

      return Enumerable.Empty<string>();
    }

    public string? CheckFormStateFunction
    {
      get { return _checkFormStateFunction; }
      set { _checkFormStateFunction = StringUtility.EmptyToNull(value); }
    }

    public void RegisterCommandForSynchronousPostBack ([NotNull]Control control, [NotNull]string eventArguments)
    {
      ArgumentUtility.CheckNotNull("control", control);
      ArgumentUtility.CheckNotNullOrEmpty("eventArguments", eventArguments);

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException(
            "RegisterCommandForSynchronousPostBack must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      Tuple<Control, string> command = new Tuple<Control, string>(control, eventArguments);
      if (!_synchronousPostBackCommands.Contains(command))
      {
        var scriptManager = ScriptManager.GetCurrent(_page.WrappedInstance);
        if (scriptManager != null)
          scriptManager.RegisterAsyncPostBackControl(control);
        // Do not add logic for removing the control/command from the collection upon removing the control from the page tree.
        // The ASP.NET ScriptManager does not remove the the control either and the registration is only performed during the PreRender phase.
        // This implies that it is unlikely the control is removed again before getting rendered.
        _synchronousPostBackCommands.Add(command);
      }
    }

    public void RegisterControlForSynchronousPostBack (Control control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      var scriptManager = ScriptManager.GetCurrent(_page.WrappedInstance);
      if (scriptManager != null)
        scriptManager.RegisterPostBackControl(control);
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this SmartPageInfo. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager(typeof(ResourceIdentifier));
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control info. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
    ///   Typically an <b>enum</b> or the derived class itself.
    /// </param>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      ArgumentUtility.CheckNotNull("localResourcesType", localResourcesType);

      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      var localResourceManager = GlobalizationService.GetResourceManager(localResourcesType);
      var pageResourceManager = ResourceManagerUtility.GetResourceManager(_page.WrappedInstance, true);

      _cachedResourceManager = ResourceManagerSet.Create(pageResourceManager, localResourceManager);

      return _cachedResourceManager;
    }

    private void Page_Init (object? sender, EventArgs e)
    {
      if (_page.Header != null)
      {
        bool hasHeadContents = false;
        foreach (Control control in _page.Header.Controls)
        {
          if (control is HtmlHeadContents)
          {
            hasHeadContents = true;
            break;
          }
        }
        if (! hasHeadContents)
          _page.Header.Controls.AddAt(0, new HtmlHeadContents());
      }

      var scriptManager = ScriptManager.GetCurrent(_page.WrappedInstance);
      if (scriptManager != null)
      {
        var handler = new SmartPageAsyncPostBackErrorHandler(_page.Context!); // TODO RM-8118: Add not null assertion
        scriptManager.AsyncPostBackError += (o, args) => handler.HandleError(args.Exception);
      }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return SafeServiceLocator.Current.GetInstance<ResourceTheme>(); }
    }

    private void Page_PreRenderComplete (object? sender, EventArgs eventArgs)
    {
      PreRenderSmartPage();
      PreRenderSmartNavigation();

      _isPreRenderComplete = true;
    }

    private void PreRenderSmartPage ()
    {
      var smartPageTokenValue = (DateTime.UtcNow - s_pageTokenStartDate).Ticks;

      _page.ClientScript.RegisterHiddenField(_page, c_smartPageTokenID, smartPageTokenValue.ToString());
      _page.ClientScript.RegisterHiddenField(_page, CacheDetectionID, null);

      HtmlHeadAppender.Current.RegisterWebClientScriptInclude();
      HtmlHeadAppender.Current.RegisterCommonStyleSheet();

      var infrastructureResourceUrlFactory = SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>();
      var styleUrl = infrastructureResourceUrlFactory.CreateThemedResourceUrl(ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink(s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);

      RegisterSmartPageInitializationScript();
    }

    private void RegisterSmartPageInitializationScript ()
    {
      var htmlForm = _page.Form;
      if (htmlForm == null)
        throw new InvalidOperationException("SmartPage requires an HtmlForm control on the page.");

      string abortMessage = GetAbortMessage();
      string hasUnconditionalAbortConfirmation = _page.HasUnconditionalAbortConfirmation ? "true" : "false";
      string statusIsSubmittingMessage = GetStatusIsSubmittingMessage();
      string abortQueuedSubmit = _page.IsQueuedSubmitToBeAborted ? "true" : "false";

      string checkFormStateFunction = "null";
      if (! string.IsNullOrEmpty(_checkFormStateFunction))
        checkFormStateFunction = "'" + _checkFormStateFunction + "'";

      string smartScrollingFieldID = "null";
      string smartFocusFieldID = "null";

      ISmartNavigablePage? smartNavigablePage = _page as ISmartNavigablePage;
      if (smartNavigablePage != null)
      {
        if (smartNavigablePage.IsSmartScrollingEnabled)
          smartScrollingFieldID = "'" + c_smartScrollingID + "'";
        if (smartNavigablePage.IsSmartFocusingEnabled)
          smartFocusFieldID = "'" + c_smartFocusID + "'";
      }

      StringBuilder initScript = new StringBuilder(500);
      StringBuilder startupScript = new StringBuilder(500);

      initScript.AppendLine("function SmartPage_Initialize ()");
      initScript.AppendLine("{");

      const string eventHandlersArray = "eventHandlers";
      initScript.Append("  var ").Append(eventHandlersArray).AppendLine(" = new Array();");
      FormatPopulateEventHandlersArrayClientScript(initScript, eventHandlersArray);
      initScript.AppendLine();

      const string trackedControlsArray = "trackedControls";
      initScript.Append("  var ").Append(trackedControlsArray).AppendLine(" = new Array();");

      var dirtyStates = _page.GetDirtyStates();
      const string dirtyStatesSet = "dirtyStates";
      startupScript.Append("  var ").Append(dirtyStatesSet).AppendLine(" = new Set();");
      if (!_page.IsDirtyStateEnabled)
      {
        // Filter is only applied to ensure consistency.
        // SmartPageInfo.GetDirtyStates() will already remove page-level dirty state values when IsDirtyStateEnabled is false.
        dirtyStates = dirtyStates.Except(s_dirtyStateForCurrentPageOnServerOrClient);
      }
      foreach (var dirtyState in dirtyStates)
        startupScript.Append(dirtyStatesSet).Append(".add('").Append(dirtyState).AppendLine("');");

      if (_page.IsDirtyStateEnabled)
        FormatPopulateTrackedControlsArrayClientScript(initScript, trackedControlsArray);

      initScript.AppendLine();

      const string synchronousPostBackCommandsArray = "synchronousPostBackCommands";
      initScript.Append("  var ").Append(synchronousPostBackCommandsArray).AppendLine(" = new Array();");
      FormatPopulateSynchronousPostBackCommandsArrayClientScript(initScript, synchronousPostBackCommandsArray);
      initScript.AppendLine();

      initScript.AppendLine("  if (SmartPage_Context.Instance == null)");
      initScript.AppendLine("  {");

      initScript.AppendLine();

      initScript.AppendLine("    SmartPage_Context.Instance = new SmartPage_Context (");
      initScript.Append("        '").Append(htmlForm.ClientID).AppendLine("',");
      initScript.Append("        ").Append(hasUnconditionalAbortConfirmation).AppendLine(",");
      initScript.Append("        ").Append(abortMessage).AppendLine(",");
      initScript.Append("        ").Append(statusIsSubmittingMessage).AppendLine(",");
      initScript.Append("        ").Append(smartScrollingFieldID).AppendLine(",");
      initScript.Append("        ").Append(smartFocusFieldID).AppendLine(",");
      initScript.Append("        '").Append(c_smartPageTokenID).AppendLine("',");
      initScript.Append("        ").Append(checkFormStateFunction).AppendLine(");");

      initScript.AppendLine("  }");
      initScript.AppendLine();

      initScript.Append("  SmartPage_Context.Instance.set_AbortQueuedSubmit (").Append(abortQueuedSubmit).AppendLine(");");
      initScript.Append("  SmartPage_Context.Instance.set_EventHandlers (").Append(eventHandlersArray).AppendLine(");");
      initScript.Append("  SmartPage_Context.Instance.set_TrackedIDs (").Append(trackedControlsArray).AppendLine(");");
      initScript.Append("  SmartPage_Context.Instance.set_SynchronousPostBackCommands (").Append(synchronousPostBackCommandsArray).AppendLine(");");
      initScript.AppendLine("}");
      initScript.AppendLine();
      initScript.AppendLine("SmartPage_Initialize ();");
      initScript.AppendLine();

      _page.ClientScript.RegisterClientScriptBlock(_page, typeof(SmartPageInfo), "smartPageInitialize", initScript.ToString());

      string isAsynchronous = "false";
      if (IsInAsyncPostBack)
        isAsynchronous = "true";
      startupScript.Append("SmartPage_Context.Instance.OnStartUp (").Append(isAsynchronous).Append(", ").Append(dirtyStatesSet).AppendLine(");");
      _page.ClientScript.RegisterStartupScriptBlock(_page, typeof(SmartPageInfo), "smartPageStartUp", startupScript.ToString());

      // Ensure the __doPostBack function and the __EventTarget and __EventArgument hidden fields on the rendered page
      _page.ClientScript.GetPostBackEventReference(new PostBackOptions(_page.WrappedInstance) { ClientSubmit = true }, false);
    }

    private bool IsInAsyncPostBack
    {
      get
      {
        var scriptManager = ScriptManager.GetCurrent(_page.WrappedInstance);
        return scriptManager != null && scriptManager.IsInAsyncPostBack;
      }
    }

    private string GetAbortMessage ()
    {
      string abortMessage = "null";
      IResourceManager resourceManager = GetResourceManager();

      if (_page.IsAbortConfirmationEnabled)
      {
        PlainTextString temp;
        if (_page.AbortMessage.IsEmpty)
          temp = resourceManager.GetText(ResourceIdentifier.AbortMessage);
        else
          temp = _page.AbortMessage;
        var unescapedValueForUseInBrowserApis = temp.GetValue();
        abortMessage = "'" + ScriptUtility.EscapeClientScript(unescapedValueForUseInBrowserApis) + "'";
      }

      return abortMessage;
    }

    private string GetStatusIsSubmittingMessage ()
    {
      string statusIsSubmittingMessage = "null";
      IResourceManager resourceManager = GetResourceManager();

      if (_page.AreStatusMessagesEnabled)
      {
        WebString temp;
        if (_page.StatusIsSubmittingMessage.IsEmpty)
          temp = resourceManager.GetHtml(ResourceIdentifier.StatusIsSubmittingMessage);
        else
          temp = _page.StatusIsSubmittingMessage;
        statusIsSubmittingMessage = "'" + ScriptUtility.EscapeClientScript(temp) + "'";
      }

      return statusIsSubmittingMessage;
    }

    private void FormatPopulateEventHandlersArrayClientScript (StringBuilder script, string eventHandlersArray)
    {
      const string eventHandlersByEventArray = "eventHandlersByEvent";

      foreach (SmartPageEvents pageEvent in _clientSideEventHandlers.Keys)
      {
        NameValueCollection eventHandlers = _clientSideEventHandlers[pageEvent];

        script.Append("  ");
        script.Append(eventHandlersByEventArray).AppendLine(" = new Array();");

        for (int i = 0; i < eventHandlers.Keys.Count; i++)
        {
          // IE 5.0.1 does not understand push
          script.Append("  ");
          script.Append(eventHandlersByEventArray).Append("[").Append(eventHandlersByEventArray).Append(".length] = '");
          script.Append(eventHandlers.Get(i));
          script.AppendLine("';");
        }

        script.Append("  ");
        script.Append(eventHandlersArray).Append("['");
        script.Append(pageEvent.ToString().ToLower());
        script.Append("'] = ").Append(eventHandlersByEventArray).AppendLine(";");
        script.AppendLine();
      }
    }

    private void FormatPopulateTrackedControlsArrayClientScript (StringBuilder script, string trackedControlsArray)
    {
      foreach (IEditableControl control in _trackedControls)
      {
        if (control.Visible)
        {
          string[] trackedIDs = control.GetTrackedClientIDs();
          for (int i = 0; i < trackedIDs.Length; i++)
          {
            // IE 5.0.1 does not understand push
            script.Append("  ");
            script.Append(trackedControlsArray).Append("[").Append(trackedControlsArray).Append(".length] = '");
            script.Append(trackedIDs[i]);
            script.AppendLine("';");
          }
        }
      }

      foreach (string? trackedID in _trackedControlsByID)
      {
        // IE 5.0.1 does not understand push
        script.Append("  ");
        script.Append(trackedControlsArray).Append("[").Append(trackedControlsArray).Append(".length] = '");
        script.Append(trackedID);
        script.AppendLine("';");
      }
    }

    private void FormatPopulateSynchronousPostBackCommandsArrayClientScript (StringBuilder script, string array)
    {
      foreach (Tuple<Control, string> command in _synchronousPostBackCommands)
      {
        script.Append("  ");
        script.Append(array).Append("[").Append(array).Append(".length] = '");
        script.Append(command.Item1.UniqueID + "|" + command.Item2);
        script.AppendLine("';");
      }
    }

    private void PreRenderSmartNavigation ()
    {
      ISmartNavigablePage? smartNavigablePage = _page as ISmartNavigablePage;
      if (smartNavigablePage == null)
        return;

      NameValueCollection? postBackCollection = _page.GetPostBackCollection();

      if (smartNavigablePage.IsSmartScrollingEnabled)
      {
        string? smartScrollingValue = null;
        if (postBackCollection != null && (_smartNavigationDataToBeDisacarded & SmartNavigationData.ScrollPosition) == SmartNavigationData.None)
          smartScrollingValue = postBackCollection[c_smartScrollingID];
        _page.ClientScript.RegisterHiddenField(_page, c_smartScrollingID, smartScrollingValue);
      }

      if (smartNavigablePage.IsSmartFocusingEnabled)
      {
        string? smartFocusValue = null;
        if (postBackCollection != null && (_smartNavigationDataToBeDisacarded & SmartNavigationData.Focus) == SmartNavigationData.None)
          smartFocusValue = postBackCollection[c_smartFocusID];
        if (! string.IsNullOrEmpty(_smartFocusID))
          smartFocusValue = _smartFocusID;
        _page.ClientScript.RegisterHiddenField(_page, c_smartFocusID, smartFocusValue);
      }
    }

    public IGlobalizationService GlobalizationService
    {
      get
      {
        return SafeServiceLocator.Current.GetInstance<IGlobalizationService>();
      }
    }

    /// <summary>
    ///   Implements <see cref="ISmartPage.StatusIsSubmittingMessage">ISmartPage.StatusIsSubmittingMessage</see>.
    /// </summary>
    public WebString StatusIsSubmittingMessage
    {
      get { return _statusIsSubmittingMessage; }
      set { _statusIsSubmittingMessage = value; }
    }

    /// <summary>
    ///   Implements <see cref="ISmartPage.AbortMessage">ISmartPage.AbortMessage</see>.
    /// </summary>
    public PlainTextString AbortMessage
    {
      get { return _abortMessage; }
      set { _abortMessage = value; }
    }

    /// <summary>
    ///   Implements <see cref="ISmartNavigablePage.DiscardSmartNavigationData">ISmartNavigablePage.DiscardSmartNavigationData</see>.
    /// </summary>
    public void DiscardSmartNavigationData (SmartNavigationData smartNavigationData = SmartNavigationData.All)
    {
      _smartNavigationDataToBeDisacarded = smartNavigationData;
    }

    /// <summary>
    ///   Implements <see cref="M:Remotion.Web.UI.ISmartNavigablePage.SetFocus(Remotion.Web.UI.Controls.IFocusableControl)">ISmartNavigablePage.SetFocus(IFocusableControl)</see>.
    /// </summary>
    public void SetFocus (IFocusableControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);
      if (string.IsNullOrEmpty(control.FocusID))
        return;
      SetFocus(control.FocusID);
    }

    /// <summary>
    ///   Sets the focus ID.
    /// </summary>
    public void SetFocus ([NotNull] string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      _smartFocusID = id;
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.ISmartNavigablePage.RegisterNavigationControl">ISmartNavigablePage.RegisterNavigationControl</see>.
    /// </summary>
    public void RegisterNavigationControl (INavigationControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      // Registration is typically done during the Control's init-phase to allow building of navigation urls during the entire life cycle.
      // Note: If the control is removed again, the previously built navigation urls will no longer be valid.
      _navigationControls.Add(control);
      control.Unload += UnregisterNavigationControl;
    }

    private void UnregisterNavigationControl (object? sender, EventArgs args)
    {
      var control = ArgumentUtility.CheckNotNullAndType<INavigationControl>("sender", sender!);
      _navigationControls.Remove(control);
      control.Unload -= UnregisterNavigationControl;
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.ISmartNavigablePage.AppendNavigationUrlParameters">ISmartNavigablePage.AppendNavigationUrlParameters</see>.
    /// </summary>
    public string AppendNavigationUrlParameters (string url)
    {
      NameValueCollection urlParameters = GetNavigationUrlParameters();
      return UrlUtility.AddParameters(url, urlParameters);
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.ISmartNavigablePage.GetNavigationUrlParameters">ISmartNavigablePage.GetNavigationUrlParameters</see>.
    /// </summary>
    public NameValueCollection GetNavigationUrlParameters ()
    {
      NameValueCollection urlParameters = new NameValueCollection();
      foreach (INavigationControl control in _navigationControls)
        NameValueCollectionUtility.Append(urlParameters, control.GetNavigationUrlParameters());

      return urlParameters;
    }
  }
}
