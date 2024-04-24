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
using System.ComponentModel;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Utilities;
using VirtualPathUtility = System.Web.VirtualPathUtility;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   The <see cref="IHttpHandler"/> implementation responsible for handling requests to the 
  ///   <b>Web Execution Engine.</b>
  /// </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/Class/*' />
  public class WxeHandler : IHttpHandler, IRequiresSessionState
  {
    /// <summary> Contains a list of parameters supported by the <see cref="WxeHandler"/>. </summary>
    /// <remarks> 
    ///   The available parameters are <see cref="WxeFunctionType"/>, <see cref="WxeFunctionToken"/>,
    ///   <see cref="ReturnUrl"/>, and <see cref="WxeAction"/>.
    /// </remarks>
    public class Parameters
    {
      /// <summary> Denotes the type of the <see cref="WxeFunction"/> to initialize. </summary>
      /// <remarks> 
      ///     The argument may be presented as a regular .net Type string or the abbreviated version as specified in
      ///     <see cref="TypeUtility.ParseAbbreviatedTypeName"/>.
      /// </remarks>
      public static readonly string WxeFunctionType = "WxeFunctionType";

      /// <summary> Denotes the <b>ID</b> of the <see cref="WxeFunction"/> to be resumed. </summary>
      public static readonly string WxeFunctionToken = "WxeFunctionToken";

      /// <summary> Denotes the <b>URL</b> to return to after the function has completed. </summary>
      /// <remarks>   
      ///   Only evaluated during initialization. Replaces the <see cref="WxeFunction.ReturnUrl"/> defined by the 
      ///   function it self. 
      /// </remarks>
      public static readonly string ReturnUrl = "ReturnUrl";

      /// <summary> Denotes a special action to be executed. </summary>
      /// <remarks> See the <see cref="Actions"/> type for a list of supported arguments. </remarks>
      public static readonly string WxeAction = "WxeAction";

      /// <summary> Denotes whether to the function should restart after it has completed. </summary>
      /// <remarks>   
      ///   Only evaluated during initialization. Replaces the <see cref="WxeFunction.ReturnUrl"/> defined by the 
      ///   function it self. Will be overruled by an explicitly specified <see cref="ReturnUrl"/>.
      /// </remarks>
      public static readonly string WxeReturnToSelf = "WxeReturnToSelf";
    }

    /// <summary> Denotes the arguments supported for the <see cref="Parameters.WxeAction"/> parameter. </summary>
    /// <remarks> The available actions are <see cref="Refresh"/> and <see cref="Abort"/>. </remarks>
    public class Actions
    {
      /// <summary> Denotes a session refresh. </summary>
      public static readonly string Refresh = "Refresh";

      /// <summary> Denotes a session abort. </summary>
      public static readonly string Abort = "Abort";

      /// <summary> Denotes a session abort. (Obsolete) </summary>
      public static readonly string Cancel = "Cancel";
    }


    private const int c_HttpSessionTimeout = 409;
    private const int c_HttpFunctionTimeout = 409;

    private static ILogger s_logger = LazyLoggerFactory.CreateLogger<WxeHandler>();

    /// <summary> The <see cref="WxeFunctionState"/> representing the <see cref="RootFunction"/> and its context. </summary>
    private WxeFunctionState? _currentFunctionState;

    private readonly IWxeLifetimeManagementSettings _wxeLifetimeManagementSettings;
    private readonly WxeUrlSettings _wxeUrlSettings;

    public WxeHandler ()
    {
      _wxeLifetimeManagementSettings = SafeServiceLocator.Current.GetInstance<IWxeLifetimeManagementSettings>();
      _wxeUrlSettings = SafeServiceLocator.Current.GetInstance<WxeUrlSettings>();
    }

    /// <summary> The root function executed by the <b>WxeHanlder</b>. </summary>
    /// <value> The <see cref="WxeFunction"/> invoked by the <see cref="Parameters.WxeFunctionType"/> parameter. </value>
    public WxeFunction RootFunction
    {
      get { return _currentFunctionState!.Function; } // TODO RM-8118: not null assertion
    }

    /// <summary> Gets a flag indication whether session management is enabled for the application. </summary>
    /// <value> <see langword="true"/> if session management is enabled. </value>
    /// <remarks> Without session management both session refreshing and session aborting are disabled. </remarks>
    public bool IsSessionManagementEnabled
    {
      get { return  _wxeLifetimeManagementSettings.EnableSessionManagement; }
    }

    /// <summary> Gets a flag indication whether session refreshing is enabled for the application. </summary>
    /// <value> <see langword="true"/> if session refreshing is enabled. </value>
    public bool IsSessionRefreshEnabled
    {
      get { return _wxeLifetimeManagementSettings.RefreshInterval > 0; }
    }

    /// <summary> Gets session refresh interval for the application. </summary>
    /// <value> The time between refresh postbacks in minutes. </value>
    public int RefreshInterval
    {
      get { return _wxeLifetimeManagementSettings.RefreshInterval; }
    }

    /// <summary> Processes the requests associated with the <see cref="WxeHandler"/>. </summary>
    /// <param name="context"> The <see cref="HttpContext"/> of the request. Must not be <see langword="null"/>. </param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ProcessRequest (HttpContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);
      CheckTimeoutConfiguration(context);

      string? functionToken = context.Request.Params[Parameters.WxeFunctionToken];

      if (string.IsNullOrEmpty(functionToken))
      {
        _currentFunctionState = CreateNewFunctionState(context, GetType(context));
        ProcessFunctionState(context, _currentFunctionState, true);
      }
      else
      {
        _currentFunctionState = ResumeExistingFunctionState(context, functionToken);
        if (_currentFunctionState != null)
        {
          ProcessFunctionState(context, _currentFunctionState, false);
        }
        else
        {
          context.Response.Clear();
          context.Response.End();
        }
      }
    }

    /// <summary> Checks whether the timeout settings are valid. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/CheckTimeoutConfiguration/*' />
    protected void CheckTimeoutConfiguration (HttpContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      if (!IsSessionManagementEnabled)
        return;

      int functionTimeout = _wxeLifetimeManagementSettings.FunctionTimeout;
      if (functionTimeout > context.Session.Timeout)
        throw new WxeException("The FunctionTimeout setting in the configuration must not be greater than the session timeout.");
      int refreshInterval = _wxeLifetimeManagementSettings.RefreshInterval;
      if (refreshInterval > 0)
      {
        if (refreshInterval >= functionTimeout)
          throw new WxeException("The RefreshInterval setting in the configuration must be less than the FunctionTimeout.");
      }
    }

    /// <summary> Gets the <see cref="Type"/> from the information provided by the <paramref name="context"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/GetType/*' />
    protected Type GetType (HttpContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      string? typeName = context.Request.Params[Parameters.WxeFunctionType];

      if (string.IsNullOrEmpty(typeName))
        return GetTypeByPath(context.Request.Url.AbsolutePath);
      else
        return GetTypeByTypeName(typeName);
    }

    /// <summary> Gets the <see cref="Type"/> for the specified <paramref name="absolutePath"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/GetTypeByPath/*' />
    protected virtual Type GetTypeByPath (string absolutePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty("absolutePath", absolutePath);

      string relativePath = VirtualPathUtility.ToAppRelative(absolutePath);

      Type? type = UrlMapping.UrlMappingConfiguration.Current.Mappings.FindType(relativePath);
      if (type == null)
        throw new WxeException(string.Format("Could not map the path '{0}' to a WXE function.", absolutePath));

      return type;
    }

    /// <summary> Gets the <see cref="Type"/> for the specified <paramref name="typeName"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/GetTypeByTypeName/*' />
    protected Type GetTypeByTypeName (string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("typeName", typeName);
      try
      {
        var type = WebTypeUtility.GetType(typeName, true, ignoreCase : true);
        if (!typeof(WxeFunction).IsAssignableFrom(type))
        {
          throw new WxeException(
              string.Format("The function type '{0}' is invalid. Wxe functions must be derived from '{1}'.", typeName, typeof(WxeFunction).GetFullNameSafe()));
        }
        return type;
      }
      catch (TypeLoadException e)
      {
        throw new WxeException(string.Format("The function type '{0}' is invalid.", typeName), e);
      }
      catch (FileNotFoundException e)
      {
        throw new WxeException(string.Format("The function type '{0}' is invalid.", typeName), e);
      }
    }


    /// <summary> Initializes a new <see cref="WxeFunction"/>, encapsulated in a <see cref="WxeFunctionState"/> object. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/CreateNewFunctionState/*' />
    protected WxeFunctionState CreateNewFunctionState (HttpContext context, Type type)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("type", type, typeof(WxeFunction));

      WxeFunctionStateManager functionStates = WxeFunctionStateManager.Current;
      functionStates.CleanUpExpired();

      WxeFunction function = (WxeFunction)Activator.CreateInstance(type)!;

      WxeFunctionState functionState = new WxeFunctionState(function, _wxeLifetimeManagementSettings.FunctionTimeout, true);
      functionStates.Add(functionState);

      function.VariablesContainer.InitializeParameters(context.Request.QueryString);

      string? returnUrlArg = context.Request.QueryString[Parameters.ReturnUrl];
      string? returnToSelfArg = context.Request.QueryString[Parameters.WxeReturnToSelf];
      if (! string.IsNullOrEmpty(returnUrlArg))
      {
        function.ReturnUrl = returnUrlArg;
      }
      else if (! string.IsNullOrEmpty(returnToSelfArg))
      {
        if (bool.Parse(returnToSelfArg))
          function.ReturnUrl = context.Request.RawUrl;
      }

      return functionState;
    }

    /// <summary> Resumes an existing <see cref="WxeFunction"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ResumeExistingFunctionState/*' />
    protected WxeFunctionState? ResumeExistingFunctionState (HttpContext context, string functionToken)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("functionToken", functionToken);

      string? action = context.Request.Params[Parameters.WxeAction];
      bool isRefresh = StringUtility.AreEqual(action, Actions.Refresh, true);
      bool isAbort = StringUtility.AreEqual(action, Actions.Abort, true)
                     || StringUtility.AreEqual(action, Actions.Cancel, true);
      bool isPostBackAction = isRefresh || isAbort;

      bool isPostRequest = string.Compare(context.Request.HttpMethod, "POST", true) == 0;

      if (! WxeFunctionStateManager.HasSession)
      {
        if (isPostBackAction)
        {
          s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The ASP.NET session has timed out.", functionToken));
          context.Response.StatusCode = c_HttpSessionTimeout;
          context.Response.StatusDescription = "Session Timeout.";
          return null;
        }
        if (isPostRequest)
        {
          s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The ASP.NET session has timed out.", functionToken));
          throw new WxeTimeoutException("Session Timeout.", functionToken); // TODO RM-8118: display error message
        }
        try
        {
          return CreateNewFunctionState(context, GetType(context));
        }
        catch (WxeException e)
        {
          s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The ASP.NET session has timed out.", functionToken));
          throw new WxeTimeoutException("Session timeout.", functionToken, e); // TODO RM-8118: display error message
        }
      }

      WxeFunctionStateManager functionStateManager = WxeFunctionStateManager.Current;
      if (!functionStateManager.TryGetLiveValue(functionToken, out var functionState))
      {
        if (isPostBackAction)
        {
          s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The function state has timed out or was aborted.", functionToken));
          context.Response.StatusCode = c_HttpFunctionTimeout;
          context.Response.StatusDescription = "Function Timeout.";
          return null;
        }
        if (isPostRequest)
        {
          s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The function state has timed out or was aborted.", functionToken));
          throw new WxeTimeoutException("Function Timeout.", functionToken); // TODO RM-8118: display error message
        }
        try
        {
          return CreateNewFunctionState(context, GetType(context));
        }
        catch (WxeException e)
        {
          s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The function state has timed out or was aborted.", functionToken));
          throw new WxeTimeoutException("Function Timeout.", functionToken, e); // TODO RM-8118: display error message
        }
      }

      if (functionState.IsAborted)
      {
        s_logger.LogError(string.Format("Error resuming WxeFunctionState {0}: The function state has been aborted.", functionState.FunctionToken));
        throw new InvalidOperationException(string.Format("WxeFunctionState {0} is aborted.", functionState.FunctionToken));
            // TODO: display error message
      }

      if (isRefresh)
      {
        functionStateManager.Touch(functionToken);
        functionStateManager.CleanUpExpired();
        return null;
      }
      else if (isAbort)
      {
        functionStateManager.CleanUpExpired();
        functionStateManager.Abort(functionState);
        return null;
      }
      else
      {
        functionStateManager.Touch(functionToken);
        functionStateManager.CleanUpExpired();
        if (functionState.Function == null)
          throw new WxeException(string.Format("Function missing in WxeFunctionState {0}.", functionState.FunctionToken));
        return functionState;
      }
    }

    /// <summary> Redirects the <see cref="HttpContext.Response"/> to an optional <see cref="WxeFunction.ReturnUrl"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ProcessFunctionState/*' />
    protected void ProcessFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("functionState", functionState);

      ExecuteFunctionState(context, functionState, isNewFunction);

      //  This point is only reached after the WxeFunction has completed execution.

      string? returnUrl = functionState.Function.ReturnUrl;
      string? executionCompletedScript = functionState.Function.ExecutionCompletedScript;

      CleanUpFunctionState(functionState);

      if (! string.IsNullOrEmpty(executionCompletedScript))
        ProcessExecutionCompletedScript(context, executionCompletedScript);
      else if (! string.IsNullOrEmpty(returnUrl))
        ProcessReturnUrl(context, returnUrl);
    }

    /// <summary> 
    ///   Sets the current <see cref="WxeContext"/> and invokes <see cref="ExecuteFunction"/> on the
    ///   <paramref name="functionState"/>'s <see cref="WxeFunctionState.Function"/>.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ExecuteFunctionState/*' />
    protected void ExecuteFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("functionState", functionState);
      if (functionState.IsAborted)
        throw new ArgumentException("The function state " + functionState.FunctionToken + " is aborted.");

      WxeContext wxeContext = new WxeContext(
          new HttpContextWrapper(context),
          WxeFunctionStateManager.Current,
          functionState,
          context.Request.QueryString,
          _wxeUrlSettings,
          _wxeLifetimeManagementSettings);
      WxeContext.SetCurrent(wxeContext);

      functionState.PostBackID++;
      ExecuteFunction(functionState.Function, wxeContext, isNewFunction);
    }

    /// <summary>  Invokes <see cref="WxeFunction.Execute(WxeContext)"/> on the <paramref name="function"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ExecuteFunction/*' />
    protected virtual void ExecuteFunction (WxeFunction function, WxeContext context, bool isNew)
    {
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("context", context);
      if (function.IsAborted)
        throw new ArgumentException("The function " + function.GetType().GetFullNameSafe() + " is aborted.");

      function.ExceptionHandler.AppendCatchExceptionTypes(typeof(WxeUserCancelException));
      function.Execute(context);
    }

    /// <summary> Aborts the <paramref name="functionState"/> after its function has executed. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/CleanUpFunctionState/*' />
    protected void CleanUpFunctionState (WxeFunctionState functionState)
    {
      ArgumentUtility.CheckNotNull("functionState", functionState);

      bool isRootFunction = functionState.Function == functionState.Function.RootFunction;
      if (functionState.IsCleanUpEnabled && isRootFunction)
        WxeFunctionStateManager.Current.Abort(functionState);
    }

    /// <summary> Redirects the <see cref="HttpContext.Response"/> to an optional <see cref="WxeFunction.ReturnUrl"/>. </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeHandler.xml' path='WxeHandler/ProcessReturnUrl/*' />
    protected void ProcessReturnUrl (HttpContext context, string returnUrl)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNullOrEmpty("returnUrl", returnUrl);

      context.Response.Redirect(returnUrl, true);
    }

    private void ProcessExecutionCompletedScript (HttpContext context, string script)
    {
      context.Response.Clear();
      context.Response.Write("<html><script language=\"JavaScript\">" + script + "</script></html>");
      context.Response.End();
    }

    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }
  }
}
