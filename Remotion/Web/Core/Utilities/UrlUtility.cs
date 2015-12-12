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
    public static string GetAbsoluteUrlWithProtocolAndHostname (HttpContextBase context, string virtualPath)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("virtualPath", virtualPath);

      if (HasScheme (virtualPath))
        return virtualPath;

      string serverPart = context.Request.Url.GetLeftPart (UriPartial.Authority);
      string resolvedPath = GetAbsoluteUrl (context, virtualPath);

      return serverPart + resolvedPath;
    }

    /// <summary> Makes a relative URL absolute. </summary>
    /// <param name="context"> The <see cref="HttpContextBase"/> to be used. Must not be <see langword="null"/>. </param>
    /// <param name="virtualPath"> The virtual path. Must not be <see langword="null"/>. Must be rooted or absolute. </param>
    /// <returns> The absolute URL. </returns>
    public static string GetAbsoluteUrl (HttpContextBase context, string virtualPath)
    {
      ArgumentUtility.CheckNotNull ("virtualPath", virtualPath);

      if (HasScheme (virtualPath))
        return virtualPath;

      if (virtualPath.Length == 0)
        return virtualPath;

      if (virtualPath[0] == '\\' || virtualPath[0] == '/' || virtualPath[0] == '~')
      {
        return context.Response.ApplyAppPathModifier (virtualPath);
      }
      else
      {
        throw new ArgumentException (
            "The path could not be resolved to an app-relative path. Most likely reason: The path did not begin with the root-operator ('~').");
      }
    }

    /// <summary>
    /// Combines 2 web URLs. 
    /// </summary>
    /// <param name="path1">Can be a relative or a absolute URL.</param>
    /// <param name="path2">Must be a relative URL or a filename.</param>
    /// <returns>The combined path.</returns>
    public static string Combine (string path1, string path2)
    {
      string path = Path.Combine (path1, path2);
      return path.Replace (@"\", "/");
    }

    /// <summary>
    /// Formats a URL string with URL encoding. (The <c>format</c> argument is not encoded.)
    /// </summary>
    public static string FormatUrl (string format, params object[] args)
    {
      if (args == null)
        return format;

      string[] encodedArgs = new string[args.Length];
      Encoding encoding = GetResponseEncoding();
      for (int i = 0; i < args.Length; ++i)
        encodedArgs[i] = HttpUtility.UrlEncode (args[i].ToString(), encoding);

      return string.Format (format, encodedArgs);
    }


    /// <summary> Adds a <paramref name="name"/>/<paramref name="value"/> pair to the <paramref name="url"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/*' />
    public static string AddParameter (string url, string name, string value, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull ("url", url);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("encoding", encoding);

      string delimiter;
      bool hasQueryString = url.IndexOf ('?') != -1;
      if (hasQueryString)
      {
        char lastChar = url[url.Length - 1];
        if (IsParameterDelimiter (lastChar))
          delimiter = string.Empty;
        else
          delimiter = "&";
      }
      else
        delimiter = "?";

      value = HttpUtility.UrlEncode (value, encoding);
      url += delimiter + name + "=" + value;

      return url;
    }

    /// <summary> Adds a <paramref name="name"/>/<paramref name="value"/> pair to the <paramref name="url"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/param[@name="url" or @name="name" or @name="value"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameter/returns' />
    public static string AddParameter (string url, string name, string value)
    {
      return AddParameter (url, name, value, GetResponseEncoding());
    }


    /// <summary> 
    ///   Adds the name/value pairs from the  <paramref name="queryStringCollection"/> to the <paramref name="url"/>. 
    /// </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/*' />
    public static string AddParameters (string url, NameValueCollection queryStringCollection, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull ("queryStringCollection", queryStringCollection);

      for (int i = 0; i < queryStringCollection.Count; i++)
        url = AddParameter (url, queryStringCollection.GetKey (i), queryStringCollection.Get (i), encoding);
      return url;
    }

    /// <summary> 
    ///   Adds the name/value pairs from the  <paramref name="queryStringCollection"/> to the <paramref name="url"/>. 
    /// </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/param[@name="url" or @name="queryStringCollection"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/AddParameters/returns' />
    public static string AddParameters (string url, NameValueCollection queryStringCollection)
    {
      return AddParameters (url, queryStringCollection, GetResponseEncoding());
    }

    /// <summary> Builds a query string from the <paramref name="queryStringCollection"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/*' />
    public static string FormatQueryString (NameValueCollection queryStringCollection, Encoding encoding)
    {
      return AddParameters (string.Empty, queryStringCollection, encoding);
    }

    /// <summary> Builds a query string from the <paramref name="queryStringCollection"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/param[@name="queryStringCollection"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/FormatQueryString/returns' />
    public static string FormatQueryString (NameValueCollection queryStringCollection)
    {
      return FormatQueryString (queryStringCollection, GetResponseEncoding());
    }


    /// <summary> Removes a <paramref name="name"/>/value pair from the <paramref name="url"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/DeleteParameter/*' />
    public static string DeleteParameter (string url, string name)
    {
      ArgumentUtility.CheckNotNull ("url", url);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      int indexOfParameter = GetParameterPosition (url, name);

      if (indexOfParameter != -1)
      {
        int indexOfNextDelimiter = url.IndexOf ('&', indexOfParameter + name.Length);
        if (indexOfNextDelimiter == -1)
        {
          int start = indexOfParameter - 1;
          int length = url.Length - start;
          url = url.Remove (start, length);
        }
        else
        {
          int indexOfNextParameter = indexOfNextDelimiter + 1;
          int length = indexOfNextParameter - indexOfParameter;
          url = url.Remove (indexOfParameter, length);
        }
      }

      return url;
    }

    /// <summary> Gets the decoded value of the parameter identified by <paramref name="name"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/GetParameter/*' />
    public static string GetParameter (string url, string name, Encoding encoding)
    {
      ArgumentUtility.CheckNotNull ("url", url);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("encoding", encoding);

      string value = null;

      int indexOfParameter = GetParameterPosition (url, name);
      if (indexOfParameter != -1)
      {
        int indexOfValueDelimiter = indexOfParameter + name.Length;
        if (indexOfValueDelimiter < url.Length && url[indexOfValueDelimiter] == '=')
        {
          int indexOfValue = indexOfValueDelimiter + 1;
          int length;
          int indexOfNextDelimiter = url.IndexOf ('&', indexOfValue);
          if (indexOfNextDelimiter == -1)
            length = url.Length - indexOfValue;
          else
            length = indexOfNextDelimiter - indexOfValue;

          value = url.Substring (indexOfValue, length);
          value = HttpUtility.UrlDecode (value, encoding);
        }
      }

      return value;
    }

    /// <summary> Gets the decoded value of the parameter identified by <paramref name="name"/>. </summary>
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/GetParameter/param[@name="url" or @name="name"]' />
    /// <include file='..\doc\include\Utilities\UrlUtility.xml' path='UrlUtility/GetParameter/returns' />
    public static string GetParameter (string url, string name)
    {
      return GetParameter (url, name, GetRequestEncoding());
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
        indexOfParameter = url.IndexOf (parameter, indexOfParameter);
        if (indexOfParameter == -1)
          break;
        if (IsParameterDelimiter (url[indexOfParameter - 1]))
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
      int colonIndex = virtualPath.IndexOf (':');
      if (colonIndex == -1)
        return false;
      
      int slashIndex = virtualPath.IndexOf ('/');
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