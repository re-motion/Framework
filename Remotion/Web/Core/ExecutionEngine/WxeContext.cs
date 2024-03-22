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
using Remotion.Context;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary>
  ///   The <b>WxeContext</b> contains information about the current WXE execution cycle.
  /// </summary>
  public class WxeContext
  {
    private static readonly SafeContextSingleton<WxeContext> s_context =
        new SafeContextSingleton<WxeContext>(SafeContextKeys.WebExecutionEngineWxeContextCurrent, () => null!);

    /// <summary> The current <see cref="WxeContext"/>. </summary>
    /// <value> 
    ///   An instance of the <see cref="WxeContext"/> type 
    ///   or <see langword="null"/> if no <see cref="WxeFunction"/> is executing.
    /// </value>
    public static WxeContext? Current
    {
      get { return s_context.Current; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetCurrent (WxeContext value)
    {
      s_context.SetCurrent(value);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <remarks> Call this method only from pages not implementing <see cref="IWxePage"/>. </remarks>
    /// <exception cref="WxeException">
    ///   Thrown if no mapping for the <paramref name="functionType"/> has been defined, and the 
    ///   <see cref="Remotion.Web.ExecutionEngine.WxeUrlSettings.DefaultWxeHandler"/> is not set.
    /// </exception>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="httpContext" or @name="functionType" or @name="urlParameters"]' />
    public static string GetPermanentUrl (HttpContextBase httpContext, Type functionType, NameValueCollection urlParameters)
    {
      var wxeUrlSettings = SafeServiceLocator.Current.GetInstance<WxeUrlSettings>();
      return GetPermanentUrl(httpContext, functionType, urlParameters, false, wxeUrlSettings);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="httpContext" or @name="functionType" or @name="urlParameters" or @name="fallbackOnCurrentUrl" or @name="wxeUrlSettings"]' />
    protected static string GetPermanentUrl (HttpContextBase httpContext, Type functionType, NameValueCollection urlParameters, bool fallbackOnCurrentUrl, WxeUrlSettings wxeUrlSettings)
    {
      ArgumentUtility.CheckNotNull("httpContext", httpContext);
      ArgumentUtility.CheckNotNull("functionType", functionType);
      if (!typeof(WxeFunction).IsAssignableFrom(functionType))
        throw new ArgumentException(string.Format("The functionType '{0}' must be derived from WxeFunction.", functionType), "functionType");
      ArgumentUtility.CheckNotNull("urlParameters", urlParameters);
      ArgumentUtility.CheckNotNull("wxeUrlSettings", wxeUrlSettings);

      NameValueCollection internalUrlParameters = NameValueCollectionUtility.Clone(urlParameters);
      UrlMapping.UrlMappingEntry? mappingEntry = UrlMapping.UrlMappingConfiguration.Current.Mappings[functionType];
      if (mappingEntry == null)
      {
        string functionTypeName = WebTypeUtility.GetQualifiedName(functionType);
        internalUrlParameters.Set(WxeHandler.Parameters.WxeFunctionType, functionTypeName);
      }

      string path;
      if (mappingEntry == null)
      {
        if (string.IsNullOrEmpty(wxeUrlSettings.DefaultWxeHandler))
        {
          if (fallbackOnCurrentUrl)
            path = httpContext.Request.Url.AbsolutePath;
          else
            throw new WxeException(
                string.Format(
                    "No URL mapping has been defined for WXE Function '{0}', nor has a default WxeHandler URL been specified in the application configuration (web.config).",
                    functionType.GetFullNameSafe()));
        }
        else
        {
          path = wxeUrlSettings.DefaultWxeHandler;
        }
      }
      else
      {
        path = mappingEntry.Resource;
      }

      string permanentUrl = UrlUtility.ResolveUrlCaseSensitive(httpContext, path)
                            + UrlUtility.FormatQueryString(internalUrlParameters, httpContext.Response.ContentEncoding);

      var maximumUrlLength = wxeUrlSettings.MaximumUrlLength;
      if (permanentUrl.Length > maximumUrlLength)
      {
        throw new WxePermanentUrlTooLongException(
            string.Format(
                "Error while creating the permanent URL for WXE function '{0}'. "
                + "The URL exceeds the maximum length of {1} bytes. Generated URL: {2}",
                functionType.Name,
                maximumUrlLength,
                permanentUrl));
      }

      return permanentUrl;
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the current window from any <see cref="Page"/> by using a redirect.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="urlParameters" or @name="returnToCaller"]' />
    public static void ExecuteFunctionExternal (Page page, WxeFunction function, NameValueCollection urlParameters, bool returnToCaller)
    {
      ExecuteFunctionExternal(page, function, false, urlParameters, returnToCaller);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the current window from any <see cref="Page"/> by using a redirect.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="createPermaUrl" or @name="urlParameters" or @name="returnToCaller"]' />
    public static void ExecuteFunctionExternal (
        Page page, WxeFunction function, bool createPermaUrl, NameValueCollection urlParameters, bool returnToCaller)
    {
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("function", function);

      string href = GetExternalFunctionUrl(function, createPermaUrl, urlParameters);
      if (returnToCaller)
        function.ReturnUrl = page.Request.RawUrl;
      System.Web.HttpContext.Current.Response.Redirect(href);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the specified window or frame from any <see cref="Page"/> 
    ///   by using java script.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (Page page, WxeFunction function, string target, string features, NameValueCollection urlParameters)
    {
      ExecuteFunctionExternal(page, function, target, features, false, urlParameters);
    }

    /// <summary> 
    ///   Executes a <see cref="WxeFunction"/> in the specified window or frame from any <see cref="Page"/>  
    ///   by using java script.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/ExecuteFunctionExternal/param[@name="page" or @name="function" or @name="target" or @name="features" or @name="createPermaUrl" or @name="urlParameters"]' />
    public static void ExecuteFunctionExternal (
        Page page, WxeFunction function, string target, string features, bool createPermaUrl, NameValueCollection urlParameters)
    {
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNullOrEmpty("target", target);

      string href = GetExternalFunctionUrl(function, createPermaUrl, urlParameters);

      string openScript;
      if (features != null)
        openScript = string.Format("window.open('{0}', '{1}', '{2}');\r\n", href, target, features);
      else
        openScript = string.Format("window.open('{0}', '{1}');\r\n", href, target);
      ScriptManager.RegisterStartupScript(page, typeof(WxeContext), "WxeExecuteFunction", openScript, true);

      function.SetExecutionCompletedScript("window.close();");
    }

    private static string GetExternalFunctionUrl (WxeFunction function, bool createPermaUrl, NameValueCollection? urlParameters)
    {
      string functionToken = WxeContext.Current!.GetFunctionTokenForExternalFunction(function, false); // TODO RM-8118: not null assertion

      NameValueCollection internalUrlParameters;
      if (urlParameters == null)
      {
        if (createPermaUrl)
          internalUrlParameters = function.VariablesContainer.SerializeParametersForQueryString();
        else
          internalUrlParameters = new NameValueCollection();
      }
      else
      {
        internalUrlParameters = NameValueCollectionUtility.Clone(urlParameters);
      }
      internalUrlParameters.Set(WxeHandler.Parameters.WxeFunctionToken, functionToken);
      return WxeContext.GetPermanentUrl(WxeContext.Current.HttpContext, function.GetType(), internalUrlParameters);
    }


    private readonly HttpContextBase _httpContext;
    private readonly WxeFunctionStateManager _functionStateManager;
    private readonly WxeFunctionState _functionState;
    private readonly NameValueCollection _queryString;

    private readonly WxeUrlSettings _wxeUrlSettings;
    private readonly IWxeLifetimeManagementSettings _wxeLifetimeManagementSettings;

    public WxeContext (
        HttpContextBase context,
        WxeFunctionStateManager functionStateManager,
        WxeFunctionState functionState,
        NameValueCollection queryString,
        WxeUrlSettings wxeUrlSettings,
        IWxeLifetimeManagementSettings wxeLifetimeManagementSettings)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("functionStateManager", functionStateManager);
      ArgumentUtility.CheckNotNull("functionState", functionState);
      ArgumentUtility.CheckNotNull("wxeUrlSettings", wxeUrlSettings);
      ArgumentUtility.CheckNotNull("wxeLifetimeManagementSettings", wxeLifetimeManagementSettings);

      _httpContext = context;
      _functionStateManager = functionStateManager;

      _functionState = functionState;
      _wxeUrlSettings = wxeUrlSettings;
      _wxeLifetimeManagementSettings = wxeLifetimeManagementSettings;

      if (queryString == null)
      {
        _queryString = new NameValueCollection();
      }
      else
      {
        _queryString = NameValueCollectionUtility.Clone(queryString);
        _queryString.Remove(WxeHandler.Parameters.WxeFunctionToken);
      }
    }

    public HttpContextBase HttpContext
    {
      get { return _httpContext; }
    }

    public WxeFunctionStateManager FunctionStateManager
    {
      get { return _functionStateManager; }
    }

    protected WxeFunctionState FunctionState
    {
      get { return _functionState; }
    }

    public string FunctionToken
    {
      get { return _functionState.FunctionToken; }
    }

    public int PostBackID
    {
      get { return _functionState.PostBackID; }
    }

    public NameValueCollection QueryString
    {
      get { return _queryString; }
    }

    /// <summary> Gets the URL that resumes the current function. </summary>
    /// <remarks>
    /// <para>
    ///   If a WXE application branches to an external web site, the external site can
    ///   link back to this URL to resume the current function at the point where 
    ///   it was interrupted. Note that if the user stays on the external site longer
    ///   than the session or function timeout, resuming will fail with a timeout
    ///   exception.
    /// </para><para>
    /// Note that cookieless sessions are not supported.
    /// </para>
    /// </remarks>
    public string GetResumeUrl (bool includeServer)
    {
      string pathPart = GetResumePath(_httpContext.Request.Url.AbsolutePath, FunctionToken, QueryString);
      if (includeServer)
        return UrlUtility.GetAbsoluteUrlWithProtocolAndHostname(_httpContext, pathPart);
      else
        return pathPart;
    }

    /// <summary> Gets the absolute path to the WXE handler used for the current function. </summary>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the path. </param>
    /// <remarks>Note that cookieless sessions are not supported.</remarks>
    protected internal string GetPath (NameValueCollection queryString)
    {
      if (queryString == null)
        queryString = new NameValueCollection();

      string path = _httpContext.Request.Url.AbsolutePath;
      return UrlUtility.AddParameters(path, queryString, _httpContext.Response.ContentEncoding);
    }

    /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
    /// <param name="functionToken"> 
    ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
    /// </param>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the path. </param>
    /// <remarks>Note that cookieless sessions are not supported.</remarks>
    protected internal string GetPath (string functionToken, NameValueCollection? queryString)
    {
      return GetResumePath(_httpContext.Request.Url.AbsolutePath, functionToken, queryString);
    }

    /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
    /// <param name="path"> The absolute path to the <see cref="WxeHandler"/>. Must not be <see langword="null"/> or emtpy. </param>
    /// <param name="functionToken"> 
    ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
    /// </param>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the <paramref name="path"/>. </param>
    private string GetResumePath (string path, string functionToken, NameValueCollection? queryString)
    {
      ArgumentUtility.CheckNotNullOrEmpty("path", path);
      ArgumentUtility.CheckNotNullOrEmpty("functionToken", functionToken);

      if (!path.StartsWith("/"))
        throw new ArgumentException("The path must be absolute", "path");

      if (path.IndexOf("?", StringComparison.InvariantCultureIgnoreCase) != -1)
        throw new ArgumentException("The path must be provided without a query string. Use the query string parameter instead.", "path");

      if (queryString == null)
        queryString = new NameValueCollection();
      else
        queryString = NameValueCollectionUtility.Clone(queryString);

      queryString.Set(WxeHandler.Parameters.WxeFunctionToken, functionToken);

      return UrlUtility.AddParameters(path, queryString, _httpContext.Response.ContentEncoding);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="functionType" or @name="urlParameters"]' />
    public string GetPermanentUrl (Type functionType, NameValueCollection urlParameters)
    {
      return GetPermanentUrl(functionType, urlParameters, false);
    }

    /// <summary> 
    ///   Gets the permanent URL for the <see cref="WxeFunction"/> of the specified <paramref name="functionType"/> 
    ///   and using the <paramref name="urlParameters"/>.
    /// </summary>
    /// <include file='..\doc\include\ExecutionEngine\WxeContext.xml' path='WxeContext/GetPermanentUrl/param[@name="functionType" or @name="urlParameters" or @name="useParentPermanentUrl"]' />
    public string GetPermanentUrl (Type functionType, NameValueCollection urlParameters, bool useParentPermanentUrl)
    {
      ArgumentUtility.CheckNotNull("urlParameters", urlParameters);

      string permanentUrl = GetPermanentUrl(_httpContext, functionType, urlParameters, true, _wxeUrlSettings);

      if (useParentPermanentUrl)
      {
        if (urlParameters[WxeHandler.Parameters.ReturnUrl] != null)
          throw new ArgumentException("The 'urlParameters' collection must not contain a 'ReturnUrl' parameter when creating a parent permanent URL.", "urlParameters");

        var maximumUrlLength = _wxeUrlSettings.MaximumUrlLength;

        string currentFunctionUrl = UrlUtility.AddParameters(_httpContext.Request.Url.AbsolutePath, _queryString, _httpContext.Response.ContentEncoding);
        StringCollection parentPermanentUrls = ExtractReturnUrls(currentFunctionUrl);

        int count = GetMergeablePermanentUrlCount(permanentUrl, parentPermanentUrls, maximumUrlLength);
        string? parentPermanentUrl = FormatParentPermanentUrl(parentPermanentUrls, count);

        if (!string.IsNullOrEmpty(parentPermanentUrl))
          permanentUrl = UrlUtility.AddParameter(permanentUrl, WxeHandler.Parameters.ReturnUrl, parentPermanentUrl, _httpContext.Response.ContentEncoding);
      }
      return permanentUrl;
    }

    /// <summary> Gets the URL to be used for transfering to the external function. </summary>
    internal string GetDestinationUrlForExternalFunction (WxeFunction function, string functionToken, WxePermaUrlOptions permaUrlOptions)
    {
      string href;
      if (permaUrlOptions.UsePermaUrl)
      {
        NameValueCollection internalUrlParameters;
        if (permaUrlOptions.UrlParameters == null)
          internalUrlParameters = function.VariablesContainer.SerializeParametersForQueryString();
        else
          internalUrlParameters = permaUrlOptions.UrlParameters.Clone();
        internalUrlParameters.Set(WxeHandler.Parameters.WxeFunctionToken, functionToken);

        href = GetPermanentUrl(function.GetType(), internalUrlParameters, permaUrlOptions.UseParentPermaUrl);
      }
      else
      {
        UrlMappingEntry? mappingEntry = UrlMappingConfiguration.Current.Mappings[function.GetType()];
        string path = mappingEntry != null
            ? UrlUtility.ResolveUrlCaseSensitive(_httpContext, mappingEntry.Resource!) // TODO RM-8118: not null assertion
            : _httpContext.Request.Url.AbsolutePath;
        href = GetResumePath(path, functionToken, permaUrlOptions.UrlParameters);
      }

      return href;
    }

    /// <summary>Initalizes a new <see cref="WxeFunctionState"/> with the passed <paramref name="function"/> and returns the associated function token.</summary>
    internal string GetFunctionTokenForExternalFunction (WxeFunction function, bool returnFromExecute)
    {
      bool enableCleanUp = !returnFromExecute;
      WxeFunctionState functionState = new WxeFunctionState(function, _wxeLifetimeManagementSettings.FunctionTimeout, enableCleanUp);
      _functionStateManager.Add(functionState);
      return functionState.FunctionToken;
    }

    private StringCollection ExtractReturnUrls (string? url)
    {
      StringCollection returnUrls = new StringCollection();

      while (!string.IsNullOrEmpty(url))
      {
        string currentUrl = url;
        url = UrlUtility.GetParameter(currentUrl, WxeHandler.Parameters.ReturnUrl, _httpContext.Request.ContentEncoding);

        if (!string.IsNullOrEmpty(url))
          currentUrl = UrlUtility.DeleteParameter(currentUrl, WxeHandler.Parameters.ReturnUrl, _httpContext.Request.ContentEncoding);

        returnUrls.Add(currentUrl);
      }
      return returnUrls;
    }

    private string? FormatParentPermanentUrl (StringCollection parentPermanentUrls, int count)
    {
      if (count > parentPermanentUrls.Count)
        throw new ArgumentOutOfRangeException("count");

      string? parentPermanentUrl = null;
      for (int i = count - 1; i >= 0; i--)
      {
        string? temp = parentPermanentUrls[i];
        if (string.IsNullOrEmpty(parentPermanentUrl))
        {
          parentPermanentUrl = temp;
        }
        else
        {
          parentPermanentUrl = UrlUtility.AddParameter(temp!, WxeHandler.Parameters.ReturnUrl, parentPermanentUrl, _httpContext.Response.ContentEncoding);
        }
      }
      return parentPermanentUrl;
    }

    private int GetMergeablePermanentUrlCount (string baseUrl, StringCollection parentPermanentUrls, int maxLength)
    {
      int i = 0;
      for (; i < parentPermanentUrls.Count; i++)
      {
        string parentPermanentUrl = FormatParentPermanentUrl(parentPermanentUrls, i + 1)!;
        if (parentPermanentUrl.Length >= maxLength)
          break;
        string url = UrlUtility.AddParameter(baseUrl, WxeHandler.Parameters.ReturnUrl, parentPermanentUrl, _httpContext.Response.ContentEncoding);
        if (url.Length > maxLength)
          break;
      }
      return i;
    }
  }

}
