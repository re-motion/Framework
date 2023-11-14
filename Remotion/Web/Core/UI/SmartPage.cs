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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Compilation;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.SmartPageImplementation;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI
{
  /// <summary>
///   <b>SmartPage</b> is the default implementation of the <see cref="ISmartPage"/> interface. Use this type
///   a base class for pages that should supress multiple postbacks, require smart navigation, or have a dirty-state.
/// </summary>
/// <include file='..\doc\include\UI\SmartPage.xml' path='SmartPage/Class/*' />
[FileLevelControlBuilder(typeof(CodeProcessingPageControlBuilder))]
public class SmartPage : Page, ISmartPage, ISmartNavigablePage
{
  #region IPage Implementation

  public Page WrappedInstance
  {
    get { return this; }
  }

  HttpContextBase? IPage.Context
  {
    get { return _httpContext; }
  }

  IClientScriptManager IPage.ClientScript
  {
    get { return _clientScriptManager; }
  }

  HttpApplicationStateBase? IPage.Application
  {
    get { return _httpContext != null ?_httpContext.Application : null; }
  }

  HttpRequestBase? IPage.Request
  {
    get { return _httpContext != null ? _httpContext.Request : null; }
  }

  HttpResponseBase? IPage.Response
  {
    get { return _httpContext != null ? _httpContext.Response : null; }
  }

  HttpServerUtilityBase? IPage.Server
  {
    get { return _httpContext != null ? _httpContext.Server : null; }
  }

  HttpSessionStateBase? IPage.Session
  {
    get { return _httpContext != null ? _httpContext.Session : null; }
  }

  #endregion

  #region ISmartPage Implementation

  /// <summary>
  /// Gets an <see cref="ISmartPageClientScriptManager"/> object used to manage, register, and add scripts to the page.
  /// </summary>
  /// <returns>An <see cref="ISmartPageClientScriptManager"/> object.</returns>
  public new ISmartPageClientScriptManager ClientScript
  {
    get { return _clientScriptManager; }
  }

  /// <summary> 
  ///   Registers Java Script functions to be executed when the respective <paramref name="pageEvent"/> is raised.
  /// </summary>
  /// <include file='..\doc\include\UI\SmartPage.xml' path='SmartPage/RegisterClientSidePageEventHandler/*' />
  public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
  {
    _smartPageInfo.RegisterClientSidePageEventHandler(pageEvent, key, function);
  }

  string? ISmartPage.CheckFormStateFunction
  {
    get { return _smartPageInfo.CheckFormStateFunction; }
    set { _smartPageInfo.CheckFormStateFunction = value; }
  }

  /// <summary> Gets or sets the message displayed when the user attempts to leave the page. </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to leave the page.")]
  [Category("Appearance")]
  [DefaultValue("")]
  public virtual PlainTextString AbortMessage
  {
    get { return _smartPageInfo.AbortMessage; }
    set { _smartPageInfo.AbortMessage = value; }
  }

  /// <summary> 
  ///   Gets or sets the message displayed when the user attempts to submit while the page is already submitting. 
  /// </summary>
  /// <remarks> 
  ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="SmartPageInfo"/>. 
  /// </remarks>
  [Description("The message displayed when the user attempts to submit while the page is already submitting.")]
  [Category("Appearance")]
  [DefaultValue("")]
  public virtual WebString StatusIsSubmittingMessage
  {
    get { return _smartPageInfo.StatusIsSubmittingMessage; }
    set { _smartPageInfo.StatusIsSubmittingMessage = value; }
  }

  /// <summary>
  ///   Registers a control implementing <see cref="IEditableControl"/> for tracking of it's server- and client-side
  ///   dirty state.
  /// </summary>
  /// <param name="control"> A control implementing <see cref="IEditableControl"/> that will be tracked.  </param>
  public void RegisterControlForDirtyStateTracking (IEditableControl control)
  {
    _smartPageInfo.RegisterControlForDirtyStateTracking(control);
  }

  /// <summary>
  ///   Resiters a <see cref="Control.ClientID"/> for the tracking of the controls client-side dirty state.
  /// </summary>
  /// <param name="clientID"> The ID of an HTML input/textarea/select element. </param>
  public void RegisterControlForClientSideDirtyStateTracking (string clientID)
  {
    _smartPageInfo.RegisterControlForDirtyStateTracking(clientID);
  }

  public void RegisterCommandForSynchronousPostBack (Control control, string eventArguments)
  {
    _smartPageInfo.RegisterCommandForSynchronousPostBack(control, eventArguments);
  }

    public void RegisterControlForSynchronousPostBack (Control control)
    {
      _smartPageInfo.RegisterControlForSynchronousPostBack(control);
    }

    #endregion

  #region ISmartNavigablePage Implementation

  /// <summary> Clears scrolling and focus information on the page. </summary>
  public void DiscardSmartNavigationData (SmartNavigationData smartNavigationData = SmartNavigationData.All)
  {
    _smartPageInfo.DiscardSmartNavigationData(smartNavigationData);
  }

  /// <summary> Sets the focus to the passed control. </summary>
  /// <param name="control"> 
  ///   The <see cref="IFocusableControl"/> to assign the focus to. Must no be <see langword="null"/>.
  /// </param>
  public void SetFocus (IFocusableControl control)
  {
    _smartPageInfo.SetFocus(control);
  }

  /// <summary> Sets the focus to the passed control ID. </summary>
  /// <param name="id"> 
  ///   The client side ID of the control to assign the focus to. Must no be <see langword="null"/> or empty. 
  /// </param>
  public new void SetFocus (string id)
  {
    _smartPageInfo.SetFocus(id);
  }

  /// <summary> Registers a <see cref="INavigationControl"/> with the <see cref="ISmartNavigablePage"/>. </summary>
  /// <param name="control"> The <see cref="INavigationControl"/> to register. Must not be <see langword="null"/>. </param>
  public void RegisterNavigationControl (INavigationControl control)
  {
    _smartPageInfo.RegisterNavigationControl(control);
  }

  /// <summary> 
  ///   Appends the URL parameters returned by <see cref="GetNavigationUrlParameters"/> to the <paramref name="url"/>.
  /// </summary>
  /// <param name="url"> A URL or a query string. Must not be <see langword="null"/>. </param>
  /// <returns> 
  ///   The <paramref name="url"/> appended with the URL parameters returned by 
  ///   <see cref="GetNavigationUrlParameters"/>. 
  /// </returns>
  public string AppendNavigationUrlParameters (string url)
  {
    return _smartPageInfo.AppendNavigationUrlParameters(url);
  }

  /// <summary> 
  ///   Evaluates the <see cref="INavigationControl.GetNavigationUrlParameters"/> methods of all controls registered
  ///   using <see cref="RegisterNavigationControl"/>.
  /// </summary>
  /// <returns>
  ///   A <see cref="NameValueCollection"/> containing the URL parameters required by this 
  ///   <see cref="ISmartNavigablePage"/> to restore its navigation state when using hyperlinks.
  /// </returns>
  public NameValueCollection GetNavigationUrlParameters ()
  {
    return _smartPageInfo.GetNavigationUrlParameters();
  }

  #endregion

  private HttpContextBase? _httpContext;
  private readonly SmartPageInfo _smartPageInfo;
  private readonly ValidatableControlInitializer _validatableControlInitializer;
  private readonly PostLoadInvoker _postLoadInvoker;
  private bool _isDirty;
  private bool? _enableDirtyState;
  private ShowAbortConfirmation _showAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
  private bool? _enableStatusMessages;
  private bool? _abortQueuedSubmit;
  private bool? _enableSmartScrolling;
  private bool? _enableSmartFocusing;
  private readonly SmartPageClientScriptManager _clientScriptManager;

  public SmartPage ()
  {
    _smartPageInfo = new SmartPageInfo(this);
    _validatableControlInitializer = new ValidatableControlInitializer(this);
    _postLoadInvoker = new PostLoadInvoker(this);
    _clientScriptManager = new SmartPageClientScriptManager(base.ClientScript);
  }

  protected override NameValueCollection? DeterminePostBackMode ()
  {
    NameValueCollection? result = base.DeterminePostBackMode();
    return result;
  }

  /// <summary> Gets the post back data for the page. </summary>
  NameValueCollection? ISmartPage.GetPostBackCollection ()
  {
    return GetPostBackCollection();
  }

  /// <summary> Gets the post-back data for the page. </summary>
  /// <remarks> Application developers should only rely on this collection for accessing the post-back data. </remarks>
  protected virtual NameValueCollection? GetPostBackCollection ()
  {
    if (string.Compare(Request.HttpMethod, "POST", true) == 0)
      return Request.Form;
    else
      return Request.QueryString;
  }

  /// <summary>
  ///   Call this method before validating when using <see cref="Remotion.Web.UI.Controls.FormGridManager"/> 
  ///   and <see cref="M:Remotion.ObjectBinding.Web.UI.Controls.IBusinessObjectDataSourceControl.Validate()"/>.
  /// </summary>
  public void PrepareValidation ()
  {
    EnsurePostLoadInvoked();
    EnsureValidatableControlsInitialized();
  }

  /// <summary> Ensures that PostLoad is called on all controls that support <see cref="ISupportsPostLoadControl"/>. </summary>
  public void EnsurePostLoadInvoked ()
  {
    _postLoadInvoker.EnsurePostLoadInvoked();
  }

  /// <summary> Ensures that all validators are registered with their <see cref="IValidatableControl"/> controls. </summary>
  public void EnsureValidatableControlsInitialized ()
  {
    _validatableControlInitializer.EnsureValidatableControlsInitialized();
  }

  /// <summary>
  /// Evaluates the page's dirty state and returns a list of identifiers, one for each distinct dirty state condition.
  /// </summary>
  /// <param name="requestedStates">
  /// Optional list of dirty state conditions the caller is interested in. When this parameter is set, the implementation may optimize the evaluation of the dirty state
  /// to only test for a specific dirty state condition if it was actually requested.
  /// </param>
  /// <returns>
  /// The set of dirty states. Note that the result may contain values that have not been requested by the caller.
  /// </returns>
  /// <remarks>
  /// <para>
  ///   Evaluates if either <see cref="IsDirty"/> is <see langword="true" />
  ///   or if any control registered using <see cref="RegisterControlForDirtyStateTracking"/> has values that must be persisted before the user leaves the page.
  /// </para>
  /// <para>
  ///   When page-local dirty state tracking has been disabled by setting <see cref="EnableDirtyState"/> to <see langword="false" />,
  ///   <see cref="GetDirtyStates"/> will exclude the <see cref="SmartPageDirtyStates.CurrentPage"/> entry from the result.
  /// </para>
  ///  <note type="inheritinfo">
  ///    Override to introduce additional flags for specific dirty scenarios (e.g. non-persistent changes in the session).
  ///   </note>
  /// </remarks>
  public virtual IEnumerable<string> GetDirtyStates (IReadOnlyCollection<string>? requestedStates)
  {
    return _smartPageInfo.GetDirtyStates(requestedStates);
  }

  /// <summary> Gets or sets a flag describing whether the page is dirty. </summary>
  /// <value> <see langword="true"/> if the page requires saving. Defaults to <see langword="false"/>.  </value>
  /// <remarks>
  ///   If <see cref="EnableDirtyState"/> is <see langword="false" />, the current value of <see cref="IsDirty"/> is ignored by the infrastructure.
  /// </remarks>
  public bool IsDirty
  {
    get { return _isDirty; }
    set { _isDirty = value; }
  }

  /// <summary>
  ///   Gets or sets the flag that determines whether to include this page's dirty state when evaluating <see cref="GetDirtyStates"/>.
  /// </summary>
  /// <value>
  ///   <see langword="true"/> to include this page's dirty state when evaluating <see cref="GetDirtyStates"/>.
  ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsDirtyStateEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to include this page's dirty state when evaluating GetDirtyStates().")]
  [Category("Behavior")]
  [DefaultValue(null)]
  public bool? EnableDirtyState
  {
    get { return _enableDirtyState; }
    set { _enableDirtyState = value; }
  }

  /// <summary>  Gets the flag that determines whether to include this page's dirty state when evaluating <see cref="GetDirtyStates"/>. </summary>
  protected virtual bool IsDirtyStateEnabled
  {
    get { return _enableDirtyState != false; }
  }

  /// <summary> Gets the value returned by <see cref="IsDirtyStateEnabled"/>. </summary>
  bool ISmartPage.IsDirtyStateEnabled
  {
    get { return IsDirtyStateEnabled; }
  }

  /// <summary> Gets a flag whether to only show the abort confirmation if the page is dirty. </summary>
  /// <value> 
  ///   <see langword="true"/> if <see cref="ShowAbortConfirmation"/> is set to
  ///   <see cref="F:ShowAbortConfirmation.Always"/>.
  /// </value>
  protected virtual bool HasUnconditionalAbortConfirmation
  {
    get { return ShowAbortConfirmation == ShowAbortConfirmation.Always; }
  }

  /// <summary> Gets the value returned by <see cref="HasUnconditionalAbortConfirmation"/>. </summary>
  bool ISmartPage.HasUnconditionalAbortConfirmation
  {
    get { return HasUnconditionalAbortConfirmation; }
  }

  /// <summary> 
  ///   Gets or sets a value that determines whether to display a confirmation dialog before leaving the page. 
  /// </summary>
  /// <value> 
  ///   <see cref="F:ShowAbortConfirmation.Always"/> to always display a confirmation dialog before leaving the page. 
  ///   <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/> to display a confirmation dialog only when the page is dirty. 
  ///   <see cref="F:ShowAbortConfirmation.Never"/> to disable the confirmation dialog. 
  ///   Defaults to <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/>.
  /// </value>
  [Description("Determines whether to display a confirmation dialog before leaving the page.")]
  [Category("Behavior")]
  [DefaultValue(ShowAbortConfirmation.OnlyIfDirty)]
  public ShowAbortConfirmation ShowAbortConfirmation
  {
    get { return _showAbortConfirmation; }
    set { _showAbortConfirmation = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="ShowAbortConfirmation"/> property. </summary>
  /// <value> 
  ///   <see langword="true"/> if <see cref="ShowAbortConfirmation"/> is set to
  ///   <see cref="F:ShowAbortConfirmation.Always"/> or <see cref="F:ShowAbortConfirmation.OnlyIfDirty"/>. 
  /// </value>
  /// <remarks> 
  ///   If <see cref="HasUnconditionalAbortConfirmation"/> evaluates <see langword="false"/>, a confirmation will only be
  ///   displayed if the page is dirty.
  /// </remarks>
  protected virtual bool IsAbortConfirmationEnabled
  {
    get
    {
      return   ShowAbortConfirmation == ShowAbortConfirmation.Always
            || ShowAbortConfirmation == ShowAbortConfirmation.OnlyIfDirty;
    }
  }

  /// <summary> Gets the value returned by <see cref="IsAbortConfirmationEnabled"/>. </summary>
  bool ISmartPage.IsAbortConfirmationEnabled
  {
    get { return IsAbortConfirmationEnabled; }
  }


  /// <summary>
  ///   Gets or sets the flag that determines whether to display a message when the user tries to start a second
  ///   request or returns to a page that has already been submitted (i.e. a cached page).
  /// </summary>
  /// <value>
  ///   <see langword="true"/> to enable the status messages. Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="AreStatusMessagesEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to display a status message when the user attempts to start a second request "
               + "or returns to a page that has already been submitted (i.e. a cached page). Undefined is interpreted as true.")]
  [Category("Behavior")]
  [DefaultValue(null)]
  public bool? EnableStatusMessages
  {
    get { return _enableStatusMessages; }
    set { _enableStatusMessages = value; }
  }

  /// <summary>
  ///   Gets a flag whether the status messages (i.e. is submitting, is aborting) will be displayed when the user
  ///   tries to e.g. postback while a request is being processed.
  /// </summary>
  protected virtual bool AreStatusMessagesEnabled
  {
    get { return _enableStatusMessages != false; }
  }

  /// <summary> Gets the value returned by <see cref="AreStatusMessagesEnabled"/>. </summary>
  bool ISmartPage.AreStatusMessagesEnabled
  {
    get { return AreStatusMessagesEnabled; }
  }

  /// <summary> 
  ///   Gets a flag whether a queued submit should be executed or aborted upon completion of the postback.
  /// </summary>
  /// <value> 
  ///   <see langword="true"/> to abort the queued submit. Defaults to <see langword="null"/>, which is interpreted as <see langword="false"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsQueuedSubmitToBeAborted"/> to evaluate this property.
  /// </remarks>
  [Description(
      "The flag that determines whether to continue or abort a queued submit upon completion of the postback. Undefined is interpreted as false.")]
  [Category("Behavior")]
  [DefaultValue(null)]
  public bool? AbortQueuedSubmit
  {
    get { return _abortQueuedSubmit; }
    set { _abortQueuedSubmit = value; }
  }

  /// <summary> 
    ///   Gets a flag whether a queued submit should be executed or aborted upon completion of the postback.
  /// </summary>
  protected virtual bool IsQueuedSubmitToBeAborted
  {
    get { return _abortQueuedSubmit ?? false; }
  }

  /// <summary> Gets the value returned by <see cref="IsQueuedSubmitToBeAborted"/>. </summary>
  bool ISmartPage.IsQueuedSubmitToBeAborted
  {
    get { return IsQueuedSubmitToBeAborted; }
  }


  /// <summary> Gets or sets the flag that determines whether to use smart scrolling. </summary>
  /// <value> 
  ///   <see langword="true"/> to use smart scrolling. Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartScrollingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart scrolling. Undefined is interpreted as true.")]
  [Category("Behavior")]
  [DefaultValue(null)]
  public bool? EnableSmartScrolling
  {
    get { return _enableSmartScrolling; }
    set { _enableSmartScrolling = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartScrolling"/> property. </summary>
  /// <value> 
  ///   <see langword="false"/> if <see cref="EnableSmartScrolling"/> is <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartScrollingEnabled
  {
    get
    {
      return _enableSmartScrolling != false;
    }
  }

  /// <summary> Gets the value returned by <see cref="IsSmartScrollingEnabled"/>. </summary>
  bool ISmartNavigablePage.IsSmartScrollingEnabled
  {
    get { return IsSmartScrollingEnabled; }
  }


  /// <summary> Gets or sets the flag that determines whether to use smart navigation. </summary>
  /// <value> 
  ///   <see langword="true"/> to use smart navigation. 
  ///   Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
  /// </value>
  /// <remarks>
  ///   Use <see cref="IsSmartFocusingEnabled"/> to evaluate this property.
  /// </remarks>
  [Description("The flag that determines whether to use smart navigation. Undefined is interpreted as true.")]
  [Category("Behavior")]
  [DefaultValue(null)]
  public bool? EnableSmartFocusing
  {
    get { return _enableSmartFocusing; }
    set { _enableSmartFocusing = value; }
  }

  /// <summary> Gets the evaluated value for the <see cref="EnableSmartFocusing"/> property. </summary>
  /// <value> 
  ///   <see langword="false"/> if <see cref="EnableSmartFocusing"/> is <see langword="false"/>.
  /// </value>
  protected virtual bool IsSmartFocusingEnabled
  {
    get
    {
      return _enableSmartFocusing != false;
    }
  }

  /// <summary> Gets the value returned by <see cref="IsSmartFocusingEnabled"/>. </summary>
  bool ISmartNavigablePage.IsSmartFocusingEnabled
  {
    get { return IsSmartFocusingEnabled; }
  }

  protected override void OnInit (EventArgs e)
  {
    base.OnInit(e);
    RegisterRequiresControlState(this);
  }

  protected override void LoadControlState (object? savedState)
  {
    object?[] values = (object?[])savedState!;
    base.LoadControlState(values[0]);
    _isDirty = (bool)values[1]!;
  }

  protected override object? SaveControlState ()
  {
    object?[] values = new object?[2];
    values[0] = base.SaveControlState();
    values[1] = _isDirty;
    return values;
  }

  void ISmartPage.SaveAllState ()
  {
    MemberCaller.SaveAllState(this);
  }

  /// <summary>
  /// Use <see cref="ProcessRequestImplementation"/> instead.
  /// </summary>
  public sealed override void ProcessRequest (HttpContext httpContext)
  {
    ArgumentUtility.CheckNotNull("httpContext", httpContext);
    _httpContext = new HttpContextWrapper(httpContext);
    ProcessRequestImplementation(httpContext);
  }

  /// <inheritdoc cref="Page.ProcessRequest"/>
  protected virtual void ProcessRequestImplementation (HttpContext httpContext)
  {
    ArgumentUtility.CheckNotNull("httpContext", httpContext);
    base.ProcessRequest(httpContext);
  }

  protected virtual IServiceLocator ServiceLocator
  {
    get { return SafeServiceLocator.Current; }
  }

  private IInternalControlMemberCaller MemberCaller
  {
    get { return ServiceLocator.GetInstance<IInternalControlMemberCaller>(); }
  }

  IPage IControl.Page
  {
    get { return this; }
  }
}
}
