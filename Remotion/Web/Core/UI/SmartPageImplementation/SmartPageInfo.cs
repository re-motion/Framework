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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
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
    [MultiLingualResources ("Remotion.Web.Globalization.SmartPageInfo")]
    protected enum ResourceIdentifier
    {
      /// <summary> Displayed when the user attempts to leave the page. </summary>
      AbortMessage,
      /// <summary> Displayed when the user attempts to submit while the page is already submitting. </summary>
      StatusIsSubmittingMessage
    }

    public static readonly string CacheDetectionID = "SmartPage_CacheDetectionField";
    private const string c_smartScrollingID = "smartScrolling";
    private const string c_smartFocusID = "smartFocus";
    private const string c_scriptFileUrl = "SmartPage.js";
    private const string c_styleFileUrl = "SmartPage.css";
    private const string c_smartNavigationScriptFileUrl = "SmartNavigation.js";

    private static readonly string s_scriptFileKey = typeof (SmartPageInfo).FullName + "_Script";
    private static readonly string s_styleFileKey = typeof (SmartPageInfo).FullName + "_Style";
    private static readonly string s_smartNavigationScriptKey = typeof (SmartPageInfo).FullName + "_SmartNavigation";

    private readonly ISmartPage _page;

    private bool _isSmartNavigationDataDisacarded;
    private string _smartFocusID;
    private string _abortMessage;
    private string _statusIsSubmittingMessage = string.Empty;

    private bool _isPreRenderComplete;

    private readonly AutoInitDictionary<SmartPageEvents, NameValueCollection> _clientSideEventHandlers =
        new AutoInitDictionary<SmartPageEvents, NameValueCollection>();

    private string _checkFormStateFunction;
    private readonly Hashtable _trackedControls = new Hashtable();
    private readonly StringCollection _trackedControlsByID = new StringCollection();
    private readonly Hashtable _navigationControls = new Hashtable();
    private readonly List<Tuple<Control, string>> _synchronousPostBackCommands = new List<Tuple<Control, string>>();

    private ResourceManagerSet _cachedResourceManager;

    public SmartPageInfo (ISmartPage page)
    {
      ArgumentUtility.CheckNotNullAndType<Page> ("page", page);
      _page = page;
      _page.Init += Page_Init;
      // PreRenderComplete-handler must be registered before ScriptManager registers its own PreRenderComplete-handler during OnInit.
      _page.PreRenderComplete += Page_PreRenderComplete;
    }

    /// <summary> Implements <see cref="ISmartPage.RegisterClientSidePageEventHandler">ISmartPage.RegisterClientSidePageEventHandler</see>. </summary>
    public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("key", key);
      ArgumentUtility.CheckNotNullOrEmpty ("function", function);
      if (! Regex.IsMatch (function, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
        throw new ArgumentException ("Invalid function name: '" + function + "'.", "function");

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException (
            "RegisterClientSidePageEventHandler must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      NameValueCollection eventHandlers = _clientSideEventHandlers[pageEvent];
      eventHandlers[key] = function;
    }


    /// <summary> Implements <see cref="ISmartPage.RegisterControlForDirtyStateTracking">ISmartPage.RegisterClientSidePageEventHandler</see>. </summary>
    public void RegisterControlForDirtyStateTracking (IEditableControl control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException (
            "RegisterControlForDirtyStateTracking must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      _trackedControls[control] = control;
    }

    /// <summary> Implements <see cref="ISmartPage.RegisterControlForClientSideDirtyStateTracking">ISmartPage.RegisterControlForClientSideDirtyStateTracking</see>. </summary>
    public void RegisterControlForDirtyStateTracking (string clientID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("clientID", clientID);

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException (
            "RegisterControlForDirtyStateTracking must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      if (! _trackedControlsByID.Contains (clientID))
        _trackedControlsByID.Add (clientID);
    }

    /// <summary> Implements <see cref="ISmartPage.EvaluateDirtyState">ISmartPage.EvaluateDirtyState</see>. </summary>
    public bool EvaluateDirtyState ()
    {
      foreach (IEditableControl control in _trackedControls.Values)
      {
        if (control.IsDirty)
          return true;
      }
      return false;
    }


    public string CheckFormStateFunction
    {
      get { return _checkFormStateFunction; }
      set { _checkFormStateFunction = StringUtility.EmptyToNull (value); }
    }

    public void RegisterCommandForSynchronousPostBack ([NotNull]Control control, [NotNull]string eventArguments)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("eventArguments", eventArguments);

      if (_isPreRenderComplete)
      {
        throw new InvalidOperationException (
            "RegisterCommandForSynchronousPostBack must not be called after the PreRenderComplete method of the System.Web.UI.Page has been invoked.");
      }

      Tuple<Control, string> command = new Tuple<Control, string> (control, eventArguments);
      if (!_synchronousPostBackCommands.Contains (command))
      {
        var scriptManager = ScriptManager.GetCurrent (_page.WrappedInstance);
        if (scriptManager != null)
          scriptManager.RegisterAsyncPostBackControl (control);
        _synchronousPostBackCommands.Add (command);
      }
    }

    public void RegisterControlForSynchronousPostBack (Control control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var scriptManager = ScriptManager.GetCurrent (_page.WrappedInstance);
      if (scriptManager != null)
        scriptManager.RegisterPostBackControl (control);
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this SmartPageInfo. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager (typeof (ResourceIdentifier));
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control info. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
    ///   Typically an <b>enum</b> or the derived class itself.
    /// </param>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      var localResourceManager = GlobalizationService.GetResourceManager (localResourcesType);
      var pageResourceManager = ResourceManagerUtility.GetResourceManager (_page.WrappedInstance, true);

      _cachedResourceManager = ResourceManagerSet.Create (pageResourceManager, localResourceManager);

      return _cachedResourceManager;
    }

    private void Page_Init (object sender, EventArgs e)
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
          _page.Header.Controls.AddAt (0, new HtmlHeadContents());
      }

      if (!ControlHelper.IsDesignMode (_page))
      {
        HtmlHeadAppender.Current.RegisterUtilitiesJavaScriptInclude ();

        var resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory> ();
        
        var smartNavigationUrl = resourceUrlFactory.CreateResourceUrl (typeof (SmartPageInfo), ResourceType.Html, c_smartNavigationScriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_smartNavigationScriptKey, smartNavigationUrl);

        var scriptUrl = resourceUrlFactory.CreateResourceUrl (typeof (SmartPageInfo), ResourceType.Html, c_scriptFileUrl);
        HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, scriptUrl);

        var infrastructureResourceUrlFactory = SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>();
        var styleUrl = infrastructureResourceUrlFactory.CreateThemedResourceUrl (ResourceType.Html, c_styleFileUrl);
        HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }

      var scriptManager = ScriptManager.GetCurrent (_page.WrappedInstance);
      if (scriptManager != null)
      {
        var handler = new SmartPageAsyncPostBackErrorHandler (_page.Context);
        scriptManager.AsyncPostBackError += (o, args) => handler.HandleError (args.Exception);
      }
    }

    protected ResourceTheme ResourceTheme
    {
      get { return SafeServiceLocator.Current.GetInstance<ResourceTheme>(); }
    }

    private void Page_PreRenderComplete (object sender, EventArgs eventArgs)
    {
      PreRenderSmartPage();
      PreRenderSmartNavigation();

      _isPreRenderComplete = true;
    }

    private void PreRenderSmartPage ()
    {
      _page.ClientScript.RegisterHiddenField (_page, CacheDetectionID, null);

      RegisterSmartPageInitializationScript();
    }

    private void RegisterSmartPageInitializationScript ()
    {
      var htmlForm = _page.Form;
      if (htmlForm == null)
        throw new InvalidOperationException ("SmartPage requires an HtmlForm control on the page.");

      string abortMessage = GetAbortMessage();
      string statusIsSubmittingMessage = GetStatusIsSubmittingMessage();

      string checkFormStateFunction = "null";
      if (! string.IsNullOrEmpty (_checkFormStateFunction))
        checkFormStateFunction = "'" + _checkFormStateFunction + "'";

      string smartScrollingFieldID = "null";
      string smartFocusFieldID = "null";

      ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
      if (smartNavigablePage != null)
      {
        if (smartNavigablePage.IsSmartScrollingEnabled)
          smartScrollingFieldID = "'" + c_smartScrollingID + "'";
        if (smartNavigablePage.IsSmartFocusingEnabled)
          smartFocusFieldID = "'" + c_smartFocusID + "'";
      }

      string isDirtyStateTrackingEnabled = "false";
      string isDirty = "false";

      StringBuilder initScript = new StringBuilder (500);

      initScript.AppendLine ("function SmartPage_Initialize ()");
      initScript.AppendLine ("{");

      const string eventHandlersArray = "eventHandlers";
      initScript.Append ("  var ").Append (eventHandlersArray).AppendLine (" = new Array();");
      FormatPopulateEventHandlersArrayClientScript (initScript, eventHandlersArray);
      initScript.AppendLine();

      const string trackedControlsArray = "trackedControls";
      initScript.Append ("  var ").Append (trackedControlsArray).AppendLine (" = new Array();");
      if (_page.IsDirtyStateTrackingEnabled)
      {
        isDirtyStateTrackingEnabled = "true";
        if (_page.EvaluateDirtyState())
          isDirty = "true";
        else
          FormatPopulateTrackedControlsArrayClientScript (initScript, trackedControlsArray);
      }
      initScript.AppendLine();

      const string synchronousPostBackCommandsArray = "synchronousPostBackCommands";
      initScript.Append ("  var ").Append (synchronousPostBackCommandsArray).AppendLine (" = new Array();");
      FormatPopulateSynchronousPostBackCommandsArrayClientScript (initScript, synchronousPostBackCommandsArray);
      initScript.AppendLine();

      initScript.AppendLine ("  if (SmartPage_Context.Instance == null)");
      initScript.AppendLine ("  {");

      initScript.AppendLine();

      initScript.AppendLine ("    SmartPage_Context.Instance = new SmartPage_Context (");
      initScript.Append ("        '").Append (htmlForm.ClientID).AppendLine ("',");
      initScript.Append ("        ").Append (isDirtyStateTrackingEnabled).AppendLine (",");
      initScript.Append ("        ").Append (abortMessage).AppendLine (",");
      initScript.Append ("        ").Append (statusIsSubmittingMessage).AppendLine (",");
      initScript.Append ("        ").Append (smartScrollingFieldID).AppendLine (",");
      initScript.Append ("        ").Append (smartFocusFieldID).AppendLine (",");
      initScript.Append ("        ").Append (checkFormStateFunction).AppendLine (");");

      initScript.AppendLine ("  }");
      initScript.AppendLine();

      initScript.Append ("  SmartPage_Context.Instance.set_EventHandlers (").Append (eventHandlersArray).AppendLine (");");
      initScript.Append ("  SmartPage_Context.Instance.set_TrackedIDs (").Append (trackedControlsArray).AppendLine (");");
      initScript.Append ("  SmartPage_Context.Instance.set_SynchronousPostBackCommands (").Append (synchronousPostBackCommandsArray).AppendLine (");");
      initScript.AppendLine ("}");
      initScript.AppendLine();
      initScript.AppendLine ("SmartPage_Initialize ();");
      initScript.AppendLine();

      _page.ClientScript.RegisterClientScriptBlock (_page, typeof (SmartPageInfo), "smartPageInitialize", initScript.ToString ());

      string isAsynchronous = "false";
      if (IsInAsyncPostBack)
        isAsynchronous = "true";
      _page.ClientScript.RegisterStartupScriptBlock (_page, typeof (SmartPageInfo), "smartPageStartUp", "SmartPage_OnStartUp (" + isAsynchronous + ", " + isDirty + ");");

      // Ensure the __doPostBack function and the __EventTarget and __EventArgument hidden fields on the rendered page
      _page.ClientScript.GetPostBackEventReference (new PostBackOptions (_page.WrappedInstance) { ClientSubmit = true }, false);
    }

    private bool IsInAsyncPostBack
    {
      get
      {
        var scriptManager = ScriptManager.GetCurrent (_page.WrappedInstance);
        return scriptManager != null && scriptManager.IsInAsyncPostBack;
      }
    }

    private string GetAbortMessage ()
    {
      string abortMessage = "null";
      IResourceManager resourceManager = GetResourceManager();

      if (_page.IsAbortConfirmationEnabled)
      {
        string temp;
        if (string.IsNullOrEmpty (_page.AbortMessage))
          temp = resourceManager.GetString (ResourceIdentifier.AbortMessage);
        else
          temp = _page.AbortMessage;
        abortMessage = "'" + ScriptUtility.EscapeClientScript (temp) + "'";
      }

      return abortMessage;
    }

    private string GetStatusIsSubmittingMessage ()
    {
      string statusIsSubmittingMessage = "null";
      IResourceManager resourceManager = GetResourceManager();

      if (_page.IsStatusIsSubmittingMessageEnabled)
      {
        string temp;
        if (string.IsNullOrEmpty (_page.StatusIsSubmittingMessage))
          temp = resourceManager.GetString (ResourceIdentifier.StatusIsSubmittingMessage);
        else
          temp = _page.StatusIsSubmittingMessage;
        statusIsSubmittingMessage = "'" + ScriptUtility.EscapeClientScript (temp) + "'";
      }

      return statusIsSubmittingMessage;
    }

    private void FormatPopulateEventHandlersArrayClientScript (StringBuilder script, string eventHandlersArray)
    {
      const string eventHandlersByEventArray = "eventHandlersByEvent";

      foreach (SmartPageEvents pageEvent in _clientSideEventHandlers.Keys)
      {
        NameValueCollection eventHandlers = _clientSideEventHandlers[pageEvent];

        script.Append ("  ");
        script.Append (eventHandlersByEventArray).AppendLine (" = new Array();");

        for (int i = 0; i < eventHandlers.Keys.Count; i++)
        {
          // IE 5.0.1 does not understand push
          script.Append ("  ");
          script.Append (eventHandlersByEventArray).Append ("[").Append (eventHandlersByEventArray).Append (".length] = '");
          script.Append (eventHandlers.Get (i));
          script.AppendLine ("';");
        }

        script.Append ("  ");
        script.Append (eventHandlersArray).Append ("['");
        script.Append (pageEvent.ToString().ToLower());
        script.Append ("'] = ").Append (eventHandlersByEventArray).AppendLine (";");
        script.AppendLine();
      }
    }

    private void FormatPopulateTrackedControlsArrayClientScript (StringBuilder script, string trackedControlsArray)
    {
      foreach (IEditableControl control in _trackedControls.Values)
      {
        if (control.Visible)
        {
          string[] trackedIDs = control.GetTrackedClientIDs();
          for (int i = 0; i < trackedIDs.Length; i++)
          {
            // IE 5.0.1 does not understand push
            script.Append ("  ");
            script.Append (trackedControlsArray).Append ("[").Append (trackedControlsArray).Append (".length] = '");
            script.Append (trackedIDs[i]);
            script.AppendLine ("';");
          }
        }
      }

      foreach (string trackedID in _trackedControlsByID)
      {
        // IE 5.0.1 does not understand push
        script.Append ("  ");
        script.Append (trackedControlsArray).Append ("[").Append (trackedControlsArray).Append (".length] = '");
        script.Append (trackedID);
        script.AppendLine ("';");
      }
    }

    private void FormatPopulateSynchronousPostBackCommandsArrayClientScript (StringBuilder script, string array)
    {
      foreach (Tuple<Control, string> command in _synchronousPostBackCommands)
      {
        script.Append ("  ");
        script.Append (array).Append ("[").Append (array).Append (".length] = '");
        script.Append (command.Item1.UniqueID + "|" + command.Item2);
        script.AppendLine ("';");
      }
    }

    private void PreRenderSmartNavigation ()
    {
      ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
      if (smartNavigablePage == null)
        return;

      NameValueCollection postBackCollection = _page.GetPostBackCollection();

      if (smartNavigablePage.IsSmartScrollingEnabled)
      {
        string smartScrollingValue = null;
        if (postBackCollection != null && ! _isSmartNavigationDataDisacarded)
          smartScrollingValue = postBackCollection[c_smartScrollingID];
        _page.ClientScript.RegisterHiddenField (_page, c_smartScrollingID, smartScrollingValue);
      }

      if (smartNavigablePage.IsSmartFocusingEnabled)
      {
        string smartFocusValue = null;
        if (postBackCollection != null && ! _isSmartNavigationDataDisacarded)
          smartFocusValue = postBackCollection[c_smartFocusID];
        if (! string.IsNullOrEmpty (_smartFocusID))
          smartFocusValue = _smartFocusID;
        _page.ClientScript.RegisterHiddenField (_page, c_smartFocusID, smartFocusValue);
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
    public string StatusIsSubmittingMessage
    {
      get { return _statusIsSubmittingMessage; }
      set { _statusIsSubmittingMessage = value ?? string.Empty; }
    }

    /// <summary>
    ///   Implements <see cref="ISmartPage.AbortMessage">ISmartPage.AbortMessage</see>.
    /// </summary>
    public string AbortMessage
    {
      get { return _abortMessage; }
      set { _abortMessage = value ?? string.Empty; }
    }

    /// <summary>
    ///   Implements <see cref="ISmartNavigablePage.DiscardSmartNavigationData">ISmartNavigablePage.DiscardSmartNavigationData</see>.
    /// </summary>
    public void DiscardSmartNavigationData ()
    {
      _isSmartNavigationDataDisacarded = true;
    }

    /// <summary>
    ///   Implements <see cref="M:Remotion.Web.UI.ISmartNavigablePage.SetFocus(Remotion.Web.UI.Controls.IFocusableControl)">ISmartNavigablePage.SetFocus(IFocusableControl)</see>.
    /// </summary>
    public void SetFocus (IFocusableControl control)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      if (string.IsNullOrEmpty (control.FocusID))
        return;
      SetFocus (control.FocusID);
    }

    /// <summary>
    ///   Sets the focus ID.
    /// </summary>
    public void SetFocus ([NotNull] string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      _smartFocusID = id;
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.ISmartNavigablePage.RegisterNavigationControl">ISmartNavigablePage.RegisterNavigationControl</see>.
    /// </summary>
    public void RegisterNavigationControl (INavigationControl control)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      _navigationControls[control] = control;
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.ISmartNavigablePage.AppendNavigationUrlParameters">ISmartNavigablePage.AppendNavigationUrlParameters</see>.
    /// </summary>
    public string AppendNavigationUrlParameters (string url)
    {
      NameValueCollection urlParameters = GetNavigationUrlParameters();
      return UrlUtility.AddParameters (url, urlParameters);
    }

    /// <summary>
    ///   Implements <see cref="Remotion.Web.UI.ISmartNavigablePage.GetNavigationUrlParameters">ISmartNavigablePage.GetNavigationUrlParameters</see>.
    /// </summary>
    public NameValueCollection GetNavigationUrlParameters ()
    {
      NameValueCollection urlParameters = new NameValueCollection();
      foreach (INavigationControl control in _navigationControls.Values)
        NameValueCollectionUtility.Append (urlParameters, control.GetNavigationUrlParameters());

      return urlParameters;
    }
  }
}
