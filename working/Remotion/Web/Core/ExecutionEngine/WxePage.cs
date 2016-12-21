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
using System.Web;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.UI;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   <b>WxePage</b> is the default implementation of the <see cref="IWxePage"/> interface. Use this type
  ///   a base class for pages that can be called by <see cref="WxePageStep"/>.
  /// </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxePage.xml' path='WxePage/Class/*' />
  public class WxePage : SmartPage, IWxePage, IWindowStateManager
  {
    #region IWxePage Impleplementation

    /// <summary> End this page step and continue with the WXE function. </summary>
    public void ExecuteNextStep ()
    {
      _wxePageInfo.ExecuteNextStep ();
    }

    /// <summary>Executes the <paramref name="function"/> using the specified <paramref name="callArguments"/>.</summary>
    /// <param name="function">The <see cref="WxeFunction"/> to be executed. Must not be <see langword="null" />.</param>
    /// <param name="callArguments">The <see cref="IWxeCallArguments"/> used to control the function invocation. Must not be <see langword="null" />.</param>
    public void ExecuteFunction (WxeFunction function, IWxeCallArguments callArguments)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("callArguments", callArguments);

      callArguments.Dispatch (_wxePageInfo.Executor, function);
    }

    /// <summary> Gets a flag describing whether this post-back has been triggered by returning from a WXE function. </summary>
    [Browsable (false)]
    public bool IsReturningPostBack
    {
      get { return _wxePageInfo.IsReturningPostBack; }
    }

    /// <summary> Gets the WXE function that has been executed in the current page. </summary>
    [Browsable (false)]
    public WxeFunction ReturningFunction
    {
      get { return _wxePageInfo.ReturningFunction; }
    }

    /// <summary> Gets the permanent URL parameters the current page. </summary>
    public NameValueCollection GetPermanentUrlParameters ()
    {
      return _wxePageInfo.GetPermanentUrlParameters ();
    }

    /// <summary> Gets the permanent URL for the current page. </summary>
    public string GetPermanentUrl ()
    {
      return _wxePageInfo.GetPermanentUrl ();
    }

    /// <summary> Gets the permanent URL for the current page using the specified <paramref name="queryString"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePage.xml' path='WxePage/GetPermanentUrl/param[@name="queryString"]' />
    public string GetPermanentUrl (NameValueCollection queryString)
    {
      return _wxePageInfo.GetPermanentUrl (queryString);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="queryString"/>.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxePage.xml' path='WxePage/GetPermanentUrl/param[@name="functionType" or @name="queryString"]' />
    public string GetPermanentUrl (Type functionType, NameValueCollection queryString)
    {
      return _wxePageInfo.GetPermanentUrl (functionType, queryString);
    }

    /// <summary> 
    ///   Gets or sets the message displayed when the user attempts to submit while the page is already aborting. 
    /// </summary>
    /// <remarks> 
    ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
    /// </remarks>
    [Description ("The message displayed when the user attempts to submit while the page is already aborting.")]
    [Category ("Appearance")]
    [DefaultValue ("")]
    public virtual string StatusIsAbortingMessage
    {
      get { return _wxePageInfo.StatusIsAbortingMessage; }
      set { _wxePageInfo.StatusIsAbortingMessage = value; }
    }

    /// <summary> 
    ///   Gets or sets the message displayed when the user returnes to a cached page that has already been submitted 
    ///   or aborted. 
    /// </summary>
    /// <remarks> 
    ///   In case of an empty <see cref="String"/>, the text is read from the resources for <see cref="WxePageInfo"/>. 
    /// </remarks>
    [Description ("The message displayed when the user returnes to a cached page that has already been submitted or aborted.")]
    [Category ("Appearance")]
    [DefaultValue ("")]
    public virtual string StatusIsCachedMessage
    {
      get { return _wxePageInfo.StatusIsCachedMessage; }
      set { _wxePageInfo.StatusIsCachedMessage = value; }
    }

    /// <summary> Gets or sets the <see cref="WxeHandler"/> of the current request. </summary>
    WxeHandler IWxePage.WxeHandler
    {
      get { return _wxePageInfo.WxeHandler; }
    }

    #endregion

    #region IWindowStateManager Implementation

    object IWindowStateManager.GetData (string key)
    {
      return _wxePageInfo.GetData (key);
    }

    void IWindowStateManager.SetData (string key, object value)
    {
      _wxePageInfo.SetData (key, value);
    }

    #endregion

    private readonly WxePageInfo _wxePageInfo;
    private bool _disposed;
    private bool? _enableOutOfSequencePostBacks;
    private bool? _enableAbort;
    private bool? _enableStatusMessages;

    public WxePage ()
    {
      _wxePageInfo = new WxePageInfo (this);
      _disposed = false;
    }

    public override Control FindControl (string id)
    {
      bool callBaseMethod;
      Control control = _wxePageInfo.FindControl (id, out callBaseMethod);
      if (callBaseMethod)
        return base.FindControl (id);
      else
        return control;
    }

    /// <summary> Overrides <see cref="Page.DeterminePostBackMode"/>. </summary>
    /// <remarks> Uses <see cref="WxePageInfo.EnsurePostBackModeDetermined"/> determine the postback mode. </remarks>
    protected override NameValueCollection DeterminePostBackMode ()
    {
      NameValueCollection result = _wxePageInfo.EnsurePostBackModeDetermined (Context);

      return result;
    }

    /// <summary> Gets the post-back data for the page. </summary>
    /// <remarks> Application developers should only rely on this collection for accessing the post-back data. </remarks>
    protected override NameValueCollection GetPostBackCollection ()
    {
      return _wxePageInfo.EnsurePostBackModeDetermined (Context);
    }

    /// <remarks> Uses <see cref="WxePageInfo.SavePageStateToPersistenceMedium"/> to save the viewstate. </remarks>
    protected override void SavePageStateToPersistenceMedium (object viewState)
    {
      _wxePageInfo.SavePageStateToPersistenceMedium (viewState);
    }

    /// <remarks> Uses <see cref="WxePageInfo.LoadPageStateFromPersistenceMedium"/> to load the viewstate. </remarks>
    protected override object LoadPageStateFromPersistenceMedium ()
    {
      object state = _wxePageInfo.LoadPageStateFromPersistenceMedium();
      PageStatePersister persister = this.PageStatePersister;
      if (state is Pair)
      {
        Pair pair = (Pair) state;
        persister.ControlState = pair.First;
        persister.ViewState = pair.Second;
      }
      else
      {
        persister.ViewState = state;
      }

      return state;
    }


    /// <remarks> Invokes <see cref="WxePageInfo.OnPreRenderComplete"/> before calling the base-implementation. </remarks>
    protected override void OnPreRenderComplete (EventArgs e)
    {
      // wxeInfo.OnPreRenderComplete() must be called before base.OnPreRenderComplete (EventArgs)
      // Base-Implementation uses SmartPageInfo, which also overrides OnPreRenderComplete 
      _wxePageInfo.OnPreRenderComplete ();

      base.OnPreRenderComplete (e);
    }

    /// <summary> Gets the <see cref="WxePageStep"/> that called this <see cref="WxePage"/>. </summary>
    [Browsable (false)]
    public WxePageStep CurrentPageStep
    {
      get { return _wxePageInfo.CurrentPageStep; }
    }

    WxePageStep IWxeTemplateControl.CurrentPageStep
    {
      get { return _wxePageInfo.CurrentPageStep; }
    }

    /// <summary> Gets the <see cref="WxeFunction"/> of which the <see cref="CurrentPageStep"/> is a part. </summary>
    /// <value> 
    ///   A <see cref="WxeFunction"/> or <see langwrpd="null"/> if the <see cref="CurrentPageStep"/> is not part of a
    ///   <see cref="WxeFunction"/>.
    /// </value>
    [Browsable (false)]
    public WxeFunction CurrentFunction
    {
      get { return _wxePageInfo.CurrentPageFunction; }
    }

    /// <summary> Gets the <see cref="WxeStep.Variables"/> collection of the <see cref="CurrentPageStep"/>. </summary>
    /// <value> 
    ///   A <see cref="NameObjectCollection"/> or <see langword="null"/> if the step is not part of a 
    ///   <see cref="WxeFunction"/>
    /// </value>
    [Browsable (false)]
    public NameObjectCollection Variables
    {
      get { return _wxePageInfo.PageVariables; }
    }


    /// <summary> Gets the <see cref="WxeForm"/> of this page. </summary>
    protected WxeForm WxeForm
    {
      get { return _wxePageInfo.WxeForm; }
    }


    /// <summary> Disposes the page. </summary>
    /// <remarks>
    ///   <b>Dispose</b> is part of the ASP.NET page execution life cycle. It does not actually implement the 
    ///   disposeable pattern.
    ///   <note type="inheritinfo">
    ///     Do not override this method.
    ///     Use <see cref="M:Remotion.Web.ExecutionEngine.WxePage.Dispose(System.Boolean)">Dispose(Boolean)</see> instead.
    ///   </note>
    /// </remarks>
    public override void Dispose ()
    {
      base.Dispose ();
      if (!_disposed)
      {
        Dispose (true);
        _disposed = true;
        _wxePageInfo.Dispose ();
      }
    }

    /// <summary> Disposes the page. </summary>
    protected virtual void Dispose (bool disposing)
    {
    }

    public override void ProcessRequest (System.Web.HttpContext httpContext)
    {
      try
      {
        base.ProcessRequest (httpContext);
      }
      catch (HttpException ex)
      {
        throw _wxePageInfo.WrapProcessRequestException (ex);
      }
    }

    /// <summary> Gets or sets the flag that determines whether to abort the session upon closing the window. </summary>
    /// <value> 
    ///   <see langword="true"/> to abort the session. Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="IsAbortEnabled"/> to evaluate this property.
    /// </remarks>
    [Description ("The flag that determines whether to abort the session when the window is closed. Undefined is interpreted as true.")]
    [Category ("Behavior")]
    [DefaultValue (null)]
    public virtual bool? EnableAbort
    {
      get { return _enableAbort; }
      set { _enableAbort = value; }
    }

    /// <summary> Gets the evaluated value for the <see cref="EnableAbort"/> property. </summary>
    /// <value>
    ///   <see langword="false"/> if <see cref="EnableAbort"/> is <see langword="alse"/>.
    /// </value>
    protected virtual bool IsAbortEnabled
    {
      get { return _enableAbort != false; }
    }

    /// <summary> Gets the value returned by <see cref="IsAbortEnabled"/>. </summary>
    bool IWxePage.IsAbortEnabled
    {
      get { return IsAbortEnabled; }
    }

    /// <summary>
    ///   Gets a flag that determines whether to allow out-of-sequence postbacks (i.e. post-backs from an already 
    ///   submitted page because of the cache). 
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> enable out of sequence post-backs. Defaults to <see langword="null"/>, which is interpreted as <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///   <para>
    ///     Use <see cref="AreOutOfSequencePostBacksEnabled"/> to evaluate this property.
    ///   </para><para>
    ///     Setting this flag disables the function abort and the abort confirmation message.
    ///   </para>
    /// </remarks>
    [Description ("The flag that determines whether to allow out-of-sequence postbacks (i.e. post-backs from an already "
                  + "submitted page because of the cache). Undefined is interpreted as false.")]
    [Category ("Behavior")]
    [DefaultValue (null)]
    public virtual bool? EnableOutOfSequencePostBacks
    {
      get { return _enableOutOfSequencePostBacks; }
      set { _enableOutOfSequencePostBacks = value; }
    }

    /// <summary> Gets the evaluated value for the <see cref="EnableOutOfSequencePostBacks"/> property. </summary>
    /// <value>
    ///   <see langword="true"/> if <see cref="EnableOutOfSequencePostBacks"/> is <see langword="true"/>
    ///   and <see cref="IsAbortEnabled"/> evaluates <see langword="false"/>.
    /// </value>
    protected virtual bool AreOutOfSequencePostBacksEnabled
    {
      get { return _enableOutOfSequencePostBacks == true && !IsAbortEnabled; }
    }

    /// <summary> Gets the value returned by <see cref="AreOutOfSequencePostBacksEnabled"/>. </summary>
    bool IWxePage.AreOutOfSequencePostBacksEnabled
    {
      get { return AreOutOfSequencePostBacksEnabled; }
    }

    /// <summary>
    ///   Gets a flag that describes whether the current postback cycle was caused by resubmitting a page from the client's cache.
    /// </summary>
    /// <value> <see langword="true"/> if the page has been re-submitted. </value>
    public bool IsOutOfSequencePostBack
    {
      get { return _wxePageInfo.IsOutOfSequencePostBack; }
    }

    /// <summary> Gets the evaluated value for the <see cref="ShowAbortConfirmation"/> property. </summary>
    /// <value> 
    ///   <see langword="true"/> if <see cref="SmartPage.IsAbortConfirmationEnabled"/> and <see cref="IsAbortEnabled"/> evaluate <see langword="true"/>. 
    /// </value>
    protected override bool IsAbortConfirmationEnabled
    {
      get { return IsAbortEnabled && base.IsAbortConfirmationEnabled; }
    }

    /// <summary> 
    ///   Gets the value of the base class's <see cref="SmartPage.IsDirtyStateTrackingEnabled"/> property ANDed with <see cref="IsAbortEnabled"/>.
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> if <see cref="SmartPage.IsDirtyStateTrackingEnabled"/> and <see cref="IsAbortEnabled"/> evaluate <see langword="true"/>. 
    /// </value>
    protected override bool IsDirtyStateTrackingEnabled
    {
      get { return IsAbortEnabled && base.IsDirtyStateTrackingEnabled; }
    }

    /// <summary> 
    ///   Gets or sets the flag that determines whether to display a message when the user tries to start a second
    ///   request or returns to a page that has already been submittet (i.e. a cached page).
    /// </summary>
    /// <value> 
    ///   <see langword="true"/> to enable the status messages. Defaults to <see langword="null"/>, which is interpreted as <see langword="true"/>.
    /// </value>
    /// <remarks>
    ///   Use <see cref="AreStatusMessagesEnabled"/> to evaluate this property.
    /// </remarks>
    [Description ("The flag that determines whether to display a status message when the user attempts to start a "
                  + "second request or returns to a page that has already been submitted (i.e. a cached page). "
                  + "Undefined is interpreted as true.")]
    [Category ("Behavior")]
    [DefaultValue (null)]
    public virtual bool? EnableStatusMessages
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
    bool IWxePage.AreStatusMessagesEnabled
    {
      get { return AreStatusMessagesEnabled; }
    }

    /// <exclude/>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public override bool? EnableStatusIsSubmittingMessage
    {
      get
      {
        return base.EnableStatusIsSubmittingMessage;
      }
      set
      {
        base.EnableStatusIsSubmittingMessage = value;
      }
    }

    /// <summary> Overridden to return the value of <see cref="AreStatusMessagesEnabled"/>. </summary>
    [EditorBrowsable (EditorBrowsableState.Never)]
    protected override bool IsStatusIsSubmittingMessageEnabled
    {
      get { return AreStatusMessagesEnabled; }
    }
  }
}
