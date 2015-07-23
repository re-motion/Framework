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
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.HtmlControls;
using JetBrains.Annotations;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI
{
  /// <summary>
  ///   This interface contains all public members of System.Web.UI.Page. It is used to derive interfaces that will be
  ///   implemented by deriving from System.Web.UI.Page.
  /// </summary>
  /// <remarks>
  ///   The reason for providing this interface is that derived interfaces do not need to be casted to System.Web.UI.Page.
  /// </remarks>
  public interface IPage : ITemplateControl, IHttpHandler
  {
    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IPage"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="Page"/>. </exception>
    Page WrappedInstance { get; }

    /// <summary>
    /// Gets or sets the application-relative, virtual directory path to the file from which the control is parsed and compiled. 
    /// </summary>
    /// <returns>
    /// A string representing the path.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">The path that is set is null. 
    /// </exception><exception cref="T:System.ArgumentException">The path that is set is not rooted. 
    /// </exception>
    string AppRelativeVirtualPath { get; set; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpApplicationState"/> object for the current Web request.
    /// </summary>
    /// <returns>
    /// The current data in the <see cref="T:System.Web.HttpApplicationState"/> class wrapped in a class implementing <see cref="HttpApplicationStateBase"/>.
    /// </returns>
    HttpApplicationStateBase Application { get; }

    /// <summary>
    /// Gets a <see cref="IClientScriptManager"/> object used to manage, register, and add script to the page.
    /// </summary>
    /// <returns>
    /// A <see cref="IClientScriptManager"/> object.
    /// </returns>
    IClientScriptManager ClientScript { get; }

    /// <summary>
    /// Gets or sets a value that allows you to override automatic detection of browser capabilities 
    /// and to specify how a page is rendered for particular browser clients.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that specifies the browser capabilities that you want to override.
    /// </returns>
    string ClientTarget { get; set; }

    /// <summary>
    /// Gets the query string portion of the requested URL.
    /// </summary>
    /// <returns>
    /// The query string portion of the requested URL.
    /// </returns>
    string ClientQueryString { get; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpContext"/> object for the current Web request.
    /// </summary>
    /// <returns>
    /// An <see cref="HttpContext"/> wrapped in a class implementing <see cref="HttpContextBase"/>.
    /// </returns>
    HttpContextBase Context { get; }

    /// <summary>
    /// Gets or sets the error page to which the requesting browser is redirected in the event of an unhandled page exception.
    /// </summary>
    /// <returns>
    /// The error page to which the browser is redirected.
    /// </returns>
    string ErrorPage { get; set; }

    /// <summary>
    /// Gets a value indicating whether the page request is the result of a call back.
    /// </summary>
    /// <returns>
    /// true if the page request is the result of a call back; otherwise, false.
    /// </returns>
    bool IsCallback { get; }

    /// <summary>
    /// Gets or sets the control of the page that is used to postback to the server.
    /// </summary>
    /// <returns>
    /// The control that is used to postback to the server.
    /// </returns>
    Control AutoPostBackControl { get; set; }

    /// <summary>
    /// Gets the document header for the page if the head element is defined with a runat=server in the page declaration.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.UI.HtmlControls.HtmlHead"/> containing the page header.
    /// </returns>
    HtmlHead Header { get; }

    /// <summary>
    /// Gets the character used to separate control identifiers when building a unique ID for a control on a page.
    /// </summary>
    /// <returns>
    /// The character used to separate control identifiers. 
    /// The default is set by the <see cref="T:System.Web.UI.Adapters.PageAdapter"/> instance that renders the page. 
    /// The <see cref="P:System.Web.UI.Page.IdSeparator"/> is a server-side field and should not be modified.
    /// </returns>
    char IdSeparator { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to return the user to the same position in the client browser after postback. 
    /// </summary>
    /// <returns>
    /// true if the client position should be maintained; otherwise, false.
    /// </returns>
    bool MaintainScrollPositionOnPostBack { get; set; }

    /// <summary>
    /// Gets the master page that determines the overall look of the page.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.MasterPage"/> associated with this page if it exists; otherwise, null. 
    /// </returns>
    MasterPage Master { get; }

    /// <summary>
    /// Gets or sets the file name of the master page.
    /// </summary>
    /// <returns>
    /// The file name of the master page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    ///   The <see cref="P:System.Web.UI.Page.MasterPageFile"/> property is set after the 
    ///   <see cref="E:System.Web.UI.Page.PreInit"/> event is complete.
    /// </exception>
    /// <exception cref="T:System.Web.HttpException">
    ///   The file specified in the <see cref="P:System.Web.UI.Page.MasterPageFile"/> property does not exist.
    ///     <para>- or -</para>
    ///     The page does not have a <see cref="T:System.Web.UI.WebControls.Content"/> control as the top level control.
    /// </exception>
    string MasterPageFile { get; set; }

    /// <summary>
    /// Gets or sets the maximum length for the page's state field.
    /// </summary>
    /// <returns>
    /// The maximum length, in bytes, for the page's state field. The default is -1.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    ///   The <see cref="P:System.Web.UI.Page.MaxPageStateFieldLength"/> property  is not equal to -1 or a positive number.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    ///   The <see cref="P:System.Web.UI.Page.MaxPageStateFieldLength"/> property was set after the page was initialized.
    /// </exception>
    int MaxPageStateFieldLength { get; set; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.TraceContext"/> object for the current Web request.
    /// </summary>
    /// <returns>
    /// Data from the <see cref="T:System.Web.TraceContext"/> object for the current Web request.
    /// </returns>
    TraceContext Trace { get; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpRequest"/> object for the requested page.
    /// </summary>
    /// <returns>
    /// The current <see cref="T:System.Web.HttpRequest"/> associated with the page wrapped in a class implementing <see cref="HttpRequestBase"/>.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">Occurs when the <see cref="T:System.Web.HttpRequest"/> object is not available. 
    /// </exception>
    HttpRequestBase Request { get; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.HttpResponse"/> object associated with the <see cref="T:System.Web.UI.Page"/> object. 
    /// This object allows you to send HTTP response data to a client and contains information about that response.
    /// </summary>
    /// <returns>
    /// The current <see cref="T:System.Web.HttpResponse"/> associated with the page wrapped in a class implementing <see cref="HttpResponseBase"/>.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">The <see cref="T:System.Web.HttpResponse"/> object is not available. 
    /// </exception>
    HttpResponseBase Response { get; }

    /// <summary>
    /// Gets the Server object, which is an instance of the <see cref="T:System.Web.HttpServerUtility"/> class wrapped in a class implementing <see cref="HttpServerUtilityBase"/>.
    /// </summary>
    /// <returns>
    /// The current Server object associated with the page.
    /// </returns>
    HttpServerUtilityBase Server { get; }

    /// <summary>
    /// Gets the <see cref="T:System.Web.Caching.Cache"/> object associated with the application in which the page resides.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.Caching.Cache"/> associated with the page's application.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">An instance of <see cref="T:System.Web.Caching.Cache"/> is not created. 
    /// </exception>
    Cache Cache { get; }

    /// <summary>
    /// Gets the current Session object provided by ASP.NET.
    /// </summary>
    /// <returns>
    /// The current session-state data.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">Occurs when the session information is set to null. 
    /// </exception>
    HttpSessionStateBase Session { get; }

    /// <summary>
    /// Gets or sets the title for the page.
    /// </summary>
    /// <returns>
    /// The title of the page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.Title"/> property requires a header control on the page.
    /// </exception>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the name of the page theme.
    /// </summary>
    /// <returns>
    /// The name of the page theme.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    ///   An attempt was made to set <see cref="P:System.Web.UI.Page.Theme"/> after the <see cref="E:System.Web.UI.Page.PreInit"/> event has occurred.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    ///   <see cref="P:System.Web.UI.Page.Theme"/> is set to an invalid theme name.
    /// </exception>
    string Theme { get; set; }

    /// <summary>
    /// Gets or sets the name of the style sheet applied to this page.
    /// </summary>
    /// <returns>
    /// The name of the style sheet applied to this page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.StyleSheetTheme"/> property is set before the <see cref="E:System.Web.UI.Control.Init"/> event completes.
    /// </exception>
    string StyleSheetTheme { get; set; }

    /// <summary>
    /// Gets information about the user making the page request.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Security.Principal.IPrincipal"/> that represents the user making the page request.
    /// </returns>
    IPrincipal User { get; }

    /// <summary>
    /// Gets a value indicating whether the page is involved in a cross-page postback.
    /// </summary>
    /// <returns>
    /// true if the page is participating in a cross-page request; otherwise, false.
    /// </returns>
    bool IsCrossPagePostBack { get; }

    /// <summary>
    /// Gets a value indicating whether the page is being loaded in response to a client postback, 
    /// or if it is being loaded and accessed for the first time.
    /// </summary>
    /// <returns>
    /// true if the page is being loaded in response to a client postback; otherwise, false.
    /// </returns>
    bool IsPostBack { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the page validates postback and callback events.
    /// </summary>
    /// <returns>
    /// true if the page validates events; otherwise, false.The default is true.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.EnableEventValidation"/> property was set after the page was initialized.
    /// </exception>
    bool EnableEventValidation { get; set; }

    /// <summary>
    /// Gets or sets the encryption mode of the view state.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.UI.ViewStateEncryptionMode"/> values. 
    /// The default value is <see cref="F:System.Web.UI.ViewStateEncryptionMode.Auto"/>. 
    /// </returns>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// The value set is not a member of the <see cref="T:System.Web.UI.ViewStateEncryptionMode"/> enumeration.
    /// </exception>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="P:System.Web.UI.Page.ViewStateEncryptionMode"/> property can be set only in or before 
    /// the page PreRenderphase in the page life cycle.
    /// </exception>
    ViewStateEncryptionMode ViewStateEncryptionMode { get; set; }

    /// <summary>
    /// Assigns an identifier to an individual user in the view-state variable associated with the current page.
    /// </summary>
    /// <returns>
    /// The identifier for the individual user.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">
    /// The <see cref="P:System.Web.UI.Page.ViewStateUserKey"/> property was accessed too late during page processing. 
    /// </exception>
    string ViewStateUserKey { get; set; }

    /// <summary>
    /// Gets a value indicating whether the control of the page that submits a postback is registered.
    /// </summary>
    /// <returns>
    /// true if the control is registered; otherwise, false.
    /// </returns>
    bool IsPostBackEventControlRegistered { get; }

    /// <summary>
    /// Gets a value indicating whether page validation succeeded.
    /// </summary>
    /// <returns>
    /// true if page validation succeeded; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.Web.HttpException">The <see cref="P:System.Web.UI.Page.IsValid"/> property is called before validation has occurred.
    /// </exception>
    bool IsValid { get; }

    /// <summary>
    /// Gets a collection of all validation controls contained on the requested page.
    /// </summary>
    /// <returns>
    /// The collection of validation controls.
    /// </returns>
    ValidatorCollection Validators { get; }

    /// <summary>
    /// Gets the page that transferred control to the current page.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Page"/> representing the page that transferred control to the current page.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The current user is not allowed to access the previous page.
    /// </exception>
    Page PreviousPage { get; }

    /// <summary>
    /// Sets a value indicating whether the page output is buffered.
    /// </summary>
    /// <returns>
    /// true if page output is buffered; otherwise, false. The default is true.
    /// </returns>
    bool Buffer { get; set; }

    /// <summary>
    /// Sets the HTTP MIME type for the <see cref="T:System.Web.HttpResponse"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// The HTTP MIME type associated with the current page.
    /// </returns>
    string ContentType { get; set; }

    /// <summary>
    /// Sets the code page identifier for the current <see cref="T:System.Web.UI.Page"/>.
    /// </summary>
    /// <returns>
    /// An integer that represents the code page identifier for the current <see cref="T:System.Web.UI.Page"/>.
    /// </returns>
    int CodePage { get; set; }

    /// <summary>
    /// Sets the encoding language for the current <see cref="T:System.Web.HttpResponse"/> object.
    /// </summary>
    /// <returns>
    /// A string that contains the encoding language for the current <see cref="T:System.Web.HttpResponse"/>.
    /// </returns>
    string ResponseEncoding { get; set; }

    /// <summary>
    /// Sets the culture ID for the <see cref="T:System.Threading.Thread"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// A valid culture ID.
    /// </returns>
    string Culture { get; set; }

    /// <summary>
    /// Sets the locale identifier for the <see cref="T:System.Threading.Thread"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// The locale identifier to pass to the <see cref="T:System.Threading.Thread"/>.
    /// </returns>
    int LCID { get; set; }

    /// <summary>
    /// Sets the user interface (UI) ID for the <see cref="T:System.Threading.Thread"/> object associated with the page.
    /// </summary>
    /// <returns>
    /// The UI ID.
    /// </returns>
    string UICulture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the time-out interval used when processing asynchronous tasks.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.TimeSpan"/> that contains the allowed time interval for completion of the asynchronous task. 
    /// The default time interval is 45 seconds.
    /// </returns>
    TimeSpan AsyncTimeout { get; set; }

    /// <summary>
    /// Sets a value indicating whether tracing is enabled for the <see cref="T:System.Web.UI.Page"/> object.
    /// </summary>
    /// <returns>
    /// true if tracing is enabled for the page; otherwise, false. The default is false.
    /// </returns>
    bool TraceEnabled { get; set; }

    /// <summary>
    /// Sets the mode in which trace statements are displayed on the page.
    /// </summary>
    /// <returns>
    /// One of the <see cref="T:System.Web.TraceMode"/> enumeration members.
    /// </returns>
    TraceMode TraceModeValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether ASP.NET should run a message authentication check (MAC) on the page's view state 
    /// when the page is posted back from the client.
    /// </summary>
    /// <returns>
    /// true if the view state should be MAC checked and encoded; otherwise, false. The default is false.
    /// </returns>
    bool EnableViewStateMac { get; set; }

    /// <summary>
    /// Gets a value indicating whether the page is processed asynchronously.
    /// </summary>
    /// <returns>
    /// true if the page is in asynchronous mode; otherwise, false;
    /// </returns>
    bool IsAsync { get; }

    /// <summary>
    /// Gets the HTML form for the page.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> object associated with the page.
    /// </returns>
    HtmlForm Form { get; }

    /// <summary>
    /// Gets the adapter that renders the page for the specific requesting browser.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Adapters.PageAdapter"/> that renders the page.
    /// </returns>
    PageAdapter PageAdapter { get; }

    /// <summary>
    /// Gets a list of objects stored in the page context.
    /// </summary>
    /// <returns>
    /// A reference to an <see cref="T:System.Collections.IDictionary"/> containing objects stored in the page context.
    /// </returns>
    IDictionary Items { get; }

    /// <summary>
    /// Reads a string resource. The <see cref="M:System.Web.UI.TemplateControl.ReadStringResource"/> 
    /// method is not intended for use from within your code.
    /// </summary>
    /// <returns>
    /// An object representing the resource.
    /// </returns>
    /// <exception cref="T:System.NotSupportedException">The <see cref="M:System.Web.UI.TemplateControl.ReadStringResource"/> is no longer supported.
    /// </exception>
    object ReadStringResource ();

    /// <summary>
    /// Returns a Boolean value indicating whether a device filter applies to the HTTP request.
    /// </summary>
    /// <returns>
    /// true if the client browser specified in <paramref name="filterName"/> is the same as the specified browser; otherwise, false. 
    /// The default is false.
    /// </returns>
    /// <param name="filterName">The browser name to test. 
    /// </param>
    bool TestDeviceFilter (string filterName);

    /// <summary>
    /// Loads a <see cref="T:System.Web.UI.Control"/> object based on a specified type and constructor parameters.
    /// </summary>
    /// <returns>
    /// Returns the specified <see cref="T:System.Web.UI.UserControl"/>.
    /// </returns>
    /// <param name="t">The type of the control.
    /// </param><param name="parameters">An array of arguments that match in number, order, and type the parameters of the constructor to invoke. 
    /// If <paramref name="parameters"/> is an empty array or null, the constructor that takes no parameters (the default constructor) is invoked.
    /// </param>
    Control LoadControl (Type t, object[] parameters);

    /// <summary>
    /// Parses an input string into a <see cref="T:System.Web.UI.Control"/> object on the ASP.NET Web page or user control.
    /// </summary>
    /// <returns>
    /// The parsed control.
    /// </returns>
    /// <param name="content">A string that contains a user control.
    /// </param><param name="ignoreParserFilter">A value that specifies whether to ignore the parser filter.
    /// </param>
    Control ParseControl (string content, bool ignoreParserFilter);

    /// <summary>
    /// Retrieves a hash code that is generated by <see cref="T:System.Web.UI.Page"/> objects that are generated at run time. 
    /// This hash code is unique to the <see cref="T:System.Web.UI.Page"/> object's control hierarchy.
    /// </summary>
    /// <returns>
    /// The hash code generated at run time. The default is 0.
    /// </returns>
    int GetTypeHashCode ();

    /// <summary>
    /// Performs any initialization of the instance of the <see cref="T:System.Web.UI.Page"/> class that is required by RAD designers. 
    /// This method is used only at design time.
    /// </summary>
    void DesignerInitialize ();

    /// <summary>
    /// Sets the browser focus to the specified control. 
    /// </summary>
    /// <param name="control">The control to receive focus.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="control"/> is null. 
    /// </exception><exception cref="T:System.InvalidOperationException">
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.Web.UI.Control)"/> is called when the control is not part of a Web Forms page. 
    ///     <para>- or -</para>
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.Web.UI.Control)"/> is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event. 
    /// </exception>
    void SetFocus (Control control);

    /// <summary>
    /// Sets the browser focus to the control with the specified identifier. 
    /// </summary>
    /// <param name="clientID">The ID of the control to set focus to.
    /// </param><exception cref="T:System.ArgumentNullException"><paramref name="clientID"/> is null.
    /// </exception><exception cref="T:System.InvalidOperationException">
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.String)"/> is called when the control is not part of a Web Forms page.
    ///     <para>- or -</para>
    /// <see cref="M:System.Web.UI.Page.SetFocus(System.String)"/> is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    void SetFocus ([NotNull] string clientID);

    /// <summary>
    /// Registers a control as one whose control state must be persisted.
    /// </summary>
    /// <param name="control">The control to register.
    /// </param><exception cref="T:System.ArgumentException">The control to register is null.
    /// </exception><exception cref="T:System.InvalidOperationException">
    /// The <see cref="M:System.Web.UI.Page.RegisterRequiresControlState(System.Web.UI.Control)"/> 
    /// method can be called only before or during the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    void RegisterRequiresControlState (Control control);

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Web.UI.Control"/> object is registered to participate in control state management.
    /// </summary>
    /// <returns>
    /// true if the specified <see cref="T:System.Web.UI.Control"/> requires control state; otherwise, false
    /// </returns>
    /// <param name="control">The <see cref="T:System.Web.UI.Control"/> to check for a control state requirement.
    /// </param>
    bool RequiresControlState (Control control);

    /// <summary>
    /// Stops persistence of control state for the specified control.
    /// </summary>
    /// <param name="control">The <see cref="T:System.Web.UI.Control"/> for which to stop persistence of control state.
    /// </param><exception cref="T:System.ArgumentException">The <see cref="T:System.Web.UI.Control"/> is null.
    /// </exception>
    void UnregisterRequiresControlState (Control control);

    /// <summary>
    /// Registers a control as one that requires postback handling when the page is posted back to the server. 
    /// </summary>
    /// <param name="control">The control to be registered. 
    /// </param><exception cref="T:System.Web.HttpException">The control to register does not implement the 
    /// <see cref="T:System.Web.UI.IPostBackDataHandler"/> interface. 
    /// </exception>
    void RegisterRequiresPostBack (Control control);

    /// <summary>
    /// Registers an ASP.NET server control as one requiring an event to be raised when the control is processed on the 
    /// <see cref="T:System.Web.UI.Page"/> object.
    /// </summary>
    /// <param name="control">The control to register. 
    /// </param>
    void RegisterRequiresRaiseEvent (IPostBackEventHandler control);

    /// <summary>
    /// Retrieves the physical path that a virtual path, either absolute or relative, or an application-relative path maps to.
    /// </summary>
    /// <returns>
    /// The physical path associated with the virtual path or application-relative path.
    /// </returns>
    /// <param name="virtualPath">A <see cref="T:System.String"/> that represents the virtual path. 
    /// </param>
    string MapPath (string virtualPath);

    /// <summary>
    /// Registers a control with the page as one requiring view-state encryption. 
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">
    /// The <see cref="M:System.Web.UI.Page.RegisterRequiresViewStateEncryption"/> method must be called before or during 
    /// the page PreRenderphase in the page life cycle. 
    /// </exception>
    void RegisterRequiresViewStateEncryption ();

    /// <summary>
    /// Causes page view state to be persisted, if called.
    /// </summary>
    void RegisterViewStateHandler ();

    /// <summary>
    /// Starts the execution of an asynchronous task.
    /// </summary>
    /// <exception cref="T:System.Web.HttpException">There is an exception in the asynchronous task.
    /// </exception>
    void ExecuteRegisteredAsyncTasks ();

    /// <summary>
    /// Registers a new asynchronous task with the page.
    /// </summary>
    /// <param name="task">A <see cref="T:System.Web.UI.PageAsyncTask"/> that defines the asynchronous task.
    /// </param><exception cref="T:System.ArgumentNullException">The asynchronous task is null. 
    /// </exception>
    void RegisterAsyncTask (PageAsyncTask task);

    /// <summary>
    /// Registers beginning and ending event handler delegates that do not require state information for an asynchronous page.
    /// </summary>
    /// <param name="beginHandler">The delegate for the <see cref="T:System.Web.BeginEventHandler"/> method.
    /// </param>
    /// <param name="endHandler">The delegate for the <see cref="T:System.Web.EndEventHandler"/> method.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// The &lt;async&gt; page directive is not set to true.
    ///     <para>- or -</para>
    ///     The <see cref="M:System.Web.UI.Page.AddOnPreRenderCompleteAsync(System.Web.BeginEventHandler,System.Web.EndEventHandler)"/> 
    ///     method is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The <see cref="P:System.Web.UI.PageAsyncTask.BeginHandler"/> or <see cref="P:System.Web.UI.PageAsyncTask.EndHandler"/> is null. 
    /// </exception>
    void AddOnPreRenderCompleteAsync (BeginEventHandler beginHandler, EndEventHandler endHandler);

    /// <summary>
    /// Registers beginning and ending  event handler delegates for an asynchronous page.
    /// </summary>
    /// <param name="beginHandler">The delegate for the <see cref="T:System.Web.BeginEventHandler"/> method.
    /// </param>
    /// <param name="endHandler">The delegate for the <see cref="T:System.Web.EndEventHandler"/> method.
    /// </param>
    /// <param name="state">An object containing state information for the event handlers.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    ///   The &lt;async&gt; page directive is not set to true.
    ///     <para>- or -</para>
    ///     The <see cref="M:System.Web.UI.Page.AddOnPreRenderCompleteAsync(System.Web.BeginEventHandler,System.Web.EndEventHandler)"/> 
    ///     method is called after the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    ///   The <see cref="P:System.Web.UI.PageAsyncTask.BeginHandler"/> or <see cref="P:System.Web.UI.PageAsyncTask.EndHandler"/> is null. 
    /// </exception>
    void AddOnPreRenderCompleteAsync (BeginEventHandler beginHandler, EndEventHandler endHandler, object state);

    /// <summary>
    /// Instructs any validation controls included on the page to validate their assigned information.
    /// </summary>
    void Validate ();

    /// <summary>
    /// Instructs the validation controls in the specified validation group to validate their assigned information.
    /// </summary>
    /// <param name="validationGroup">The validation group name of the controls to validate.
    /// </param>
    void Validate (string validationGroup);

    /// <summary>
    /// Returns a collection of control validators for a specified validation group.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Web.UI.ValidatorCollection"/> that contains the control validators for the specified validation group.
    /// </returns>
    /// <param name="validationGroup">The validation group to return, or null to return the default validation group.
    /// </param>
    ValidatorCollection GetValidators (string validationGroup);

    /// <summary>
    /// Confirms that an <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> control is rendered for the specified ASP.NET server control at run time.
    /// </summary>
    /// <param name="control">The ASP.NET server control that is required in the <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> control. 
    /// </param>
    /// <exception cref="T:System.Web.HttpException">
    /// The specified server control is not contained between the opening and closing tags 
    /// of the <see cref="T:System.Web.UI.HtmlControls.HtmlForm"/> server control at run time. 
    /// </exception>
    /// <exception cref="T:System.ArgumentNullException">
    /// The control to verify is null.
    /// </exception>
    void VerifyRenderingInServerForm (Control control);

    /// <summary>
    /// Gets the data item at the top of the data-binding context stack.
    /// </summary>
    /// <returns>
    /// The object at the top of the data binding context stack.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">There is no data-binding context for the page.
    /// </exception>
    object GetDataItem ();

    event EventHandler LoadComplete;
    event EventHandler PreInit;
    event EventHandler PreLoad;
    event EventHandler PreRenderComplete;
    event EventHandler InitComplete;
    event EventHandler SaveStateComplete;
  }
}
