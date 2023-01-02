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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Provided helper functions for working with URLs.
  /// </summary>
  public static class UrlUtility
  {
    /// <summary> Makes a relative URL absolute and prepends the name of the server used by the request. </summary>
    /// <param name="context"> The <see cref="HttpContextBase"/> to be used for retrieving the protocol and hostname. Must not be <see langword="null"/>. </param>
    /// <param name="virtualPath"> The virtual path. Must not be <see langword="null"/>. Must be rooted or absolute. </param>
    /// <returns> The absolute URL. </returns>
    /// <remarks>
    /// Note that the request casing is only preserved in the return value if the <paramref name="virtualPath"/> is a relative URL.
    /// For absolute URLs, the <paramref name="virtualPath"/> parameter is returned with the original casing preserved.
    /// </remarks>
    public static string GetAbsoluteUrlWithProtocolAndHostname (HttpContextBase context, string virtualPath)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("virtualPath", virtualPath);

      if (HasScheme(virtualPath))
        return virtualPath;

      var hostHeader = context.Request.Headers["Host"];
      string serverPart;
      if (string.IsNullOrEmpty(hostHeader))
        serverPart = hostHeader ?? context.Request.Url.GetLeftPart(UriPartial.Authority);
      else
        serverPart = context.Request.Url.GetLeftPart(UriPartial.Scheme) + hostHeader;
      string resolvedPath = ResolveUrlCaseSensitive(context, virtualPath);

      return serverPart + resolvedPath;
    }

    /// <summary> Makes a relative URL absolute. </summary>
    /// <param name="context"> The <see cref="HttpContextBase"/> to be used. Must not be <see langword="null"/>. </param>
    /// <param name="virtualPath"> The virtual path. Must not be <see langword="null"/>. Must be rooted or absolute. </param>
    /// <returns> The absolute URL. </returns>
    [Obsolete(
        "Use ResolveUrlCaseSensitive (HttpContextBase, string) instead. Note that ResolveUrlCaseSensitive() no longer supports cookieless sessions. (Version 1.16.23 and Version 1.17.11)",
        true)]
    public static string GetAbsoluteUrl (HttpContextBase context, string virtualPath)
    {
      throw new NotSupportedException(
          "Use ResolveUrlCaseSensitive (HttpContextBase, string) instead. Note that ResolveUrlCaseSensitive() no longer supports cookieless sessions. (Version 1.16.23 and Version 1.17.11)");
    }

    /// <summary>
    /// Make a URL absolute using the request's original path to preserve the casing. 
    /// The returned URL is for client use, and will not support cookieless sessions.
    /// </summary>
    /// <param name="context"> The <see cref="HttpContextBase"/> to be used. Must not be <see langword="null"/>. </param>
    /// <param name="relativeUrl"> The virtual path. Must not be <see langword="null"/>.</param>
    /// <returns> The absolute path. </returns>
    /// <remarks>
    /// Note that the request casing is only preserved in the return value if the <paramref name="relativeUrl"/> is actually a relative URL.
    /// For absolute URLs, the <paramref name="relativeUrl"/> parameter is returned with the original casing preserved.
    /// </remarks>
    public static string ResolveUrlCaseSensitive (HttpContextBase context, string relativeUrl)
    {
      ArgumentUtility.CheckNotNull("context", context);
      ArgumentUtility.CheckNotNull("relativeUrl", relativeUrl);

      // HtppResponse.ApplyAppPathModifier (string)
      // "~"            "/AppDir/"
      // "~/"           "/AppDir/"
      // "~\\test"      "/AppDir/test"
      // "~/test"       "/AppDir/test"
      // "\\test\\path" "/test/path"
      // "/test/path"   "/test/path"
      // "~/~/test"     "/AppDir/~/test"
      // "\\test/path"  "/test/path"
      // "/test\\path"  "/test/path"
      // "test/path"    "/appdIr/folder/test/path"
      // ""             ""
      // "~/."          "/AppDir"
      // "~/./path"     "/AppDir/path
      // "~/path/."     "/AppDir/path

      if (HasScheme(relativeUrl))
        return relativeUrl;

      if (relativeUrl.Length == 0)
        return relativeUrl;

      // Cookieless sessions are not implemented in ASP.NET MVC and are discouraged for security reasons.
      // There is no known usage of cookieless sessions in re-motion applications.
      var session = context.Session;
      if (session != null && session.IsCookieless)
        throw new InvalidOperationException("Cookieless sessions are not supported for resolving URLs.");

      var applicationPath = GetApplicationPathFromHttpContext(context);
      Assertion.IsTrue(System.Web.VirtualPathUtility.IsAbsolute(applicationPath));

      if (relativeUrl == "~")
        return System.Web.VirtualPathUtility.AppendTrailingSlash(applicationPath);

      if (relativeUrl == "~/")
        return System.Web.VirtualPathUtility.AppendTrailingSlash(applicationPath);

      if (relativeUrl[0] == '~' && relativeUrl[1] == '/')
      {
        var virtualDirectory = relativeUrl.Substring(2);
        // Protect nested root operatores given how they are not interpreted by HtppResponse.ApplyAppPathModifier (string)
        if (virtualDirectory.Length == 1 && virtualDirectory[0] == '~')
          return System.Web.VirtualPathUtility.AppendTrailingSlash(applicationPath) + "~";

        if (virtualDirectory.Length > 1 && virtualDirectory[0] == '~')
          return CombineVirtualPaths(applicationPath, "./" + virtualDirectory);

        return CombineVirtualPaths(applicationPath, virtualDirectory);
      }

      var virtualPathUriStyle = relativeUrl.Replace('\\', '/');
      if (virtualPathUriStyle[0] == '\\' || virtualPathUriStyle[0] == '/')
        return virtualPathUriStyle;

      var requestedDirectory = GetRequestedDirectory(context);
      return System.Web.VirtualPathUtility.AppendTrailingSlash(requestedDirectory) + virtualPathUriStyle;
    }

    private static string CombineVirtualPaths (string left, string right)
    {
      return System.Web.VirtualPathUtility.Combine(System.Web.VirtualPathUtility.AppendTrailingSlash(left), right);
    }

    private static string GetApplicationPathFromHttpContext (HttpContextBase context)
    {
      var applicationPath = System.Web.VirtualPathUtility.AppendTrailingSlash(context.Request.ApplicationPath) ?? "/";
      if (applicationPath == "/")
        return applicationPath;

      Assertion.IsNotNull(context.Request.Url, "context.Request.Url != null");
      var requestUrlAbsolutePath = System.Web.VirtualPathUtility.AppendTrailingSlash(context.Request.Url.AbsolutePath);
      if (requestUrlAbsolutePath.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
        return requestUrlAbsolutePath.Remove(applicationPath.Length - 1);

      if (System.Web.VirtualPathUtility.AppendTrailingSlash(context.Request.Url.LocalPath).StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
      {
        var applicationPathPartsCount = applicationPath.Split(new[] { '/' }, StringSplitOptions.None).Length - 1;

        var calculatedApplicationPath = requestUrlAbsolutePath
            .Split(new[] { '/' }, StringSplitOptions.None)
            .Take(applicationPathPartsCount)
            .Aggregate(new StringBuilder(requestUrlAbsolutePath.Length), (sb, part) => sb.Append(part).Append("/"))
            .ToString();

        Assertion.IsTrue(
            requestUrlAbsolutePath.StartsWith(calculatedApplicationPath),
            "Calculation of application path from request URL failed.\r\n"
            + "  Absolute path from request: {0}\r\n"
            + "  Calculated path: {1}\r\n"
            + "  Application path: {2}\r\n",
            requestUrlAbsolutePath,
            calculatedApplicationPath,
            applicationPath);

        return calculatedApplicationPath;
      }

      throw new InvalidOperationException(
          string.Format(
              "Cannot calculate the application path when the request URL does not start with the application path. "
              + "Possible reasons include the use of escape sequences in the path, e.g. when the application path contains whitespace.\r\n"
              + "  Absolute path from request: {0}\r\n"
              + "  Application path: {1}\r\n",
              requestUrlAbsolutePath,
              applicationPath));
    }

    private static string GetRequestedDirectory (HttpContextBase context)
    {
      Assertion.IsNotNull(context.Request.Url, "context.Request.Url != null");
      var absolutePath = context.Request.Url.AbsolutePath;
      if (absolutePath.EndsWith("/"))
        return absolutePath;

      return absolutePath.Remove(absolutePath.LastIndexOf('/'));
    }

    /// <summary>
    /// Combines 2 web URLs. 
    /// </summary>
    /// <param name="path1">Can be a relative or a absolute URL.</param>
    /// <param name="path2">Must be a relative URL or a filename.</param>
    /// <returns>The combined path.</returns>
    public static string Combine (string path1, string path2)
    {
      string path = Path.Combine(path1, path2);
      return path.Replace(@"\", "/");
    }

    /// <summary>
    /// Formats a URL string with URL encoding. (The <c>format</c> argument is not encoded.)
    /// </summary>
    public static string FormatUrl (string format, params object[]? args)
    {
      if (args == null)
        return format;

      string[] encodedArgs = new string[args.Length];
      Encoding encoding = GetResponseEncoding();
      for (int i = 0; i < args.Length; ++i)
        encodedArgs[i] = HttpUtility.UrlEncode(args[i].ToString()!, encoding); // TODO RM-8118: not null assertion

      return string.Format(format, encodedArgs);
    }


    /// <summary> Adds a <paramref name="name"/>/<paramref name="value"/> pair to the <paramref name="url"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/*' />
    public static string AddParameter (string url, string? name, string value, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull("url", url);
      ArgumentUtility.CheckNotEmpty("name", name);
      ArgumentUtility.CheckNotNull("value", value);
      ArgumentUtility.CheckNotNull("encoding", encoding);

      string delimiter;
      bool hasQueryString = url.IndexOf('?') != -1;
      if (hasQueryString)
      {
        char lastChar = url[url.Length - 1];
        if (IsParameterDelimiter(lastChar))
          delimiter = string.Empty;
        else
          delimiter = "&";
      }
      else
        delimiter = "?";

      value = HttpUtility.UrlEncode(value, encoding);
      url += delimiter + (name != null ? name + "=" : "") + value;

      return url;
    }

    /// <summary> Adds a <paramref name="name"/>/<paramref name="value"/> pair to the <paramref name="url"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/param[@name="url" or @name="name" or @name="value"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/returns' />
    public static string AddParameter (string url, string name, string value)
    {
      return AddParameter(url, name, value, GetResponseEncoding());
    }


    /// <summary> 
    ///   Adds the name/value pairs from the  <paramref name="queryStringCollection"/> to the <paramref name="url"/>. 
    /// </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/*' />
    public static string AddParameters (string url, NameValueCollection queryStringCollection, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull("queryStringCollection", queryStringCollection);

      for (int i = 0; i < queryStringCollection.Count; i++)
      {
        var key = queryStringCollection.GetKey(i);
        var values = queryStringCollection.GetValues(i);
        if (values == null)
        {
          url = AddParameter(url, key, "", encoding);
        }
        else
        {
          for (int j = 0; j < values.Length; j++)
            url = AddParameter(url, key, values[j], encoding);
        }
      }

      return url;
    }

    /// <summary> 
    ///   Adds the name/value pairs from the  <paramref name="queryStringCollection"/> to the <paramref name="url"/>. 
    /// </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/param[@name="url" or @name="queryStringCollection"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/returns' />
    public static string AddParameters (string url, NameValueCollection queryStringCollection)
    {
      return AddParameters(url, queryStringCollection, GetResponseEncoding());
    }

    /// <summary> Builds a query string from the <paramref name="queryStringCollection"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/*' />
    public static string FormatQueryString (NameValueCollection queryStringCollection, Encoding encoding)
    {
      return AddParameters(string.Empty, queryStringCollection, encoding);
    }

    /// <summary> Builds a query string from the <paramref name="queryStringCollection"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/param[@name="queryStringCollection"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/returns' />
    public static string FormatQueryString (NameValueCollection queryStringCollection)
    {
      return FormatQueryString(queryStringCollection, GetResponseEncoding());
    }

    [Obsolete("Use DeleteParameter (string, string, Encoding) instead. (Version: 1.18.2)")]
    public static string DeleteParameter (string url, string name)
    {
      return DeleteParameter(url, name, GetResponseEncoding());
    }

    /// <summary> Removes a <paramref name="name"/>/value pair from the <paramref name="url"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/DeleteParameter/*' />
    public static string DeleteParameter (string url, string name, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull("url", url);
      ArgumentUtility.CheckNotEmpty("name", name);
      ArgumentUtility.CheckNotNull("encoding", encoding);

      var urlParts = url.Split(new []{'?'}, 2, StringSplitOptions.None);
      if (urlParts.Length == 1)
        return url;
      var urlPath = urlParts[0];

      var queryString = HttpUtility.ParseQueryString(urlParts[1], encoding);
      if (queryString.Count == 0)
        return url;

      queryString.Remove(name);

      if (queryString.Count == 0)
        return urlPath;
      // Cannot use queryString.ToString() because of aspnet:DontUsePercentUUrlEncoding causing the URL Parameters to always be formatted as Unicode Code Points
      return urlPath + FormatQueryString(queryString);
    }

    /// <summary> Gets the decoded value of the parameter identified by <paramref name="name"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/GetParameter/*' />
    public static string? GetParameter (string url, string name, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull("url", url);
      ArgumentUtility.CheckNotEmpty("name", name);
      ArgumentUtility.CheckNotNull("encoding", encoding);

      var urlParts = url.Split(new []{'?'}, 2, StringSplitOptions.None);
      if (urlParts.Length == 1)
        return null;
      var queryString = HttpUtility.ParseQueryString(urlParts[1], encoding);
      return queryString[name];
    }

    /// <summary> Gets the decoded value of the parameter identified by <paramref name="name"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/GetParameter/param[@name="url" or @name="name"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/GetParameter/returns' />
    public static string? GetParameter (string url, string name)
    {
      return GetParameter(url, name, GetRequestEncoding());
    }

    /// <summary> Gets the index of the <paramref name="parameter"/> in the <paramref name="url"/>. </summary>
    /// <returns> The index of the <paramref name="parameter"/> or -1 if it is not part of the <paramref name="url"/>. </returns>
    private static int GetParameterPosition (string url, string parameter)
    {
      if (url.Length < parameter.Length + 1)
        return -1;

      int indexOfParameter;
      for (indexOfParameter = 1; indexOfParameter < url.Length; indexOfParameter++)
      {
        indexOfParameter = url.IndexOf(parameter, indexOfParameter);
        if (indexOfParameter == -1)
          break;
        if (IsParameterDelimiter(url[indexOfParameter - 1]))
          break;
      }
      return indexOfParameter;
    }

    private static bool IsParameterDelimiter (char c)
    {
      return c == '?' || c == '&';
    }

    private static bool HasScheme (string virtualPath)
    {
      int colonIndex = virtualPath.IndexOf(':');
      if (colonIndex == -1)
        return false;

      int slashIndex = virtualPath.IndexOf('/');
      if (slashIndex != -1)
        return (colonIndex < slashIndex);

      return true;
    }

    private static Encoding GetRequestEncoding ()
    {
      return HttpContext.Current != null ? HttpContext.Current.Request.ContentEncoding : Encoding.UTF8;
    }

    private static Encoding GetResponseEncoding ()
    {
      return HttpContext.Current != null ? HttpContext.Current.Response.ContentEncoding : Encoding.UTF8;
    }
  }
}
