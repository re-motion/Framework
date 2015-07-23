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
using System.Linq;
using System.Reflection;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Web.Services
{
  /// <summary>
  /// Provides utility methods for common web service operations.
  /// </summary>
  public static class WebServiceUtility
  {
    private static readonly ICache<Tuple<MemberInfo, Type>, Attribute> s_attributeCache =
        CacheFactory.CreateWithLocking<Tuple<MemberInfo, Type>, Attribute>();

    private static readonly ICache<Tuple<Type, string>, MethodInfo> s_methodInfoCache =
        CacheFactory.CreateWithLocking<Tuple<Type, string>, MethodInfo>();

    /// <summary>
    /// Checks that <paramref name="type"/> and <paramref name="method"/> declare a valid web service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the web service. Must not be <see langword="null" />.</param>
    /// <param name="method">The service method of the web service. Must not be <see langword="null" /> or empty.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the required attributes for a web service are not set or the web service declaration itself is invalid.
    /// </exception>
    public static void CheckWebService (Type type, string method)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("method", method);

      CheckType (type);
      GetCheckedAttribute<WebServiceAttribute> (type);
      var methodinfo = GetCheckedMethodInfo (type, method);
      GetCheckedAttribute<WebMethodAttribute> (type, methodinfo);
    }

    /// <summary>
    /// Checks that <paramref name="type"/> and <paramref name="method"/> declare a valid script web service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the script web service. Must not be <see langword="null" />.</param>
    /// <param name="method">The service method of the script web service. Must not be <see langword="null" /> or empty.</param>
    /// <exception cref="ArgumentException">
    /// Thrown if the required attributes for a script web service are not set or the web service declaration itself is invalid.
    /// </exception>
    public static void CheckScriptService (Type type, string method)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("method", method);

      CheckWebService (type, method);

      GetCheckedAttribute<ScriptServiceAttribute> (type);
      var methodInfo = GetCheckedMethodInfo (type, method);
      GetCheckedAttribute<ScriptMethodAttribute> (type, methodInfo);
    }

    /// <summary>
    /// Checks that <paramref name="type"/> and <paramref name="method"/> declare a valid JSON web service.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the JSON web service. Must not be <see langword="null" />.</param>
    /// <param name="method">The service method of the JSON web service. Must not be <see langword="null" /> or empty.</param>
    /// <param name="parameters">The parameters of the JSON web service. Must not be <see langword="null" />. </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the required attributes for a JSON web service are not set or the web service declaration itself is invalid.
    /// </exception>
    public static void CheckJsonService (Type type, string method, params string[] parameters)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("method", method);
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      CheckScriptService (type, method);

      var methodInfo = GetCheckedMethodInfo (type, method);
      var scriptMethodAttribute = GetCheckedAttribute<ScriptMethodAttribute> (type, methodInfo);
      if (scriptMethodAttribute.ResponseFormat != ResponseFormat.Json)
      {
        throw CreateArgumentException (
            "Web method '{0}' on web service type '{1}' does not have the ResponseFormat property of the {2} set to Json.",
            method,
            type.FullName,
            typeof (ScriptMethodAttribute).Name);
      }
      CheckParameters (type, methodInfo, parameters);
    }

    private static void CheckType (Type type)
    {
      if (!typeof (WebService).IsAssignableFrom (type))
        throw CreateArgumentException ("Web service type '{0}' does not derive from '{1}'.", type.FullName, typeof (WebService).FullName);
    }

    private static MethodInfo GetCheckedMethodInfo (Type type, string method)
    {
      var methodInfo = s_methodInfoCache.GetOrCreateValue (
          Tuple.Create (type, method),
          key => key.Item1.GetMethod (key.Item2, BindingFlags.Instance | BindingFlags.Public));

      if (methodInfo == null)
      {
        throw CreateArgumentException (
            "Web method '{0}' was not found on the public API of web service type '{1}'.",
            method,
            type.FullName);
      }
      return methodInfo;
    }

    private static T GetCheckedAttribute<T> (Type type)
        where T: Attribute
    {
      var attribute = GetAttributeFromCache<T> (type);
      if (attribute == null)
      {
        throw CreateArgumentException (
            "Web service type '{0}' does not have the '{1}' applied.",
            type.FullName,
            typeof (T).FullName);
      }
      return attribute;
    }

    private static T GetCheckedAttribute<T> (Type type, MethodInfo methodInfo)
        where T: Attribute
    {
      var attribute = GetAttributeFromCache<T> (methodInfo);
      if (attribute == null)
      {
        throw CreateArgumentException (
            "Web method '{0}' on web service type '{1}' does not have the '{2}' applied.",
            methodInfo.Name,
            type.FullName,
            typeof (T).FullName);
      }
      return attribute;
    }

    private static void CheckParameters (Type type, MethodInfo methodInfo, string[] expectedParameters)
    {
      var actualParameters = methodInfo.GetParameters().Select (pi => pi.Name).ToArray();
      var missingParameters = expectedParameters.Except (actualParameters);
      var unexpectedParameters = actualParameters.Except (expectedParameters);

      var firstMissingParameter = missingParameters.FirstOrDefault();
      if (firstMissingParameter != null)
      {
        throw CreateArgumentException (
            "Web method '{0}' on web service type '{1}' does not declare the required parameter '{2}'. Parameters are matched by name and case.",
            methodInfo.Name,
            type.FullName,
            firstMissingParameter);
      }

      var firstUnexpectedParameter = unexpectedParameters.FirstOrDefault();
      if (firstUnexpectedParameter != null)
      {
        throw CreateArgumentException (
            "Web method '{0}' on web service type '{1}' has unexpected parameter '{2}'.",
            methodInfo.Name,
            type.FullName,
            firstUnexpectedParameter);
      }
    }

    private static T GetAttributeFromCache<T> (MemberInfo memberInfo)
        where T: Attribute
    {
      return (T) s_attributeCache.GetOrCreateValue (
          Tuple.Create (memberInfo, typeof (T)),
          key => AttributeUtility.GetCustomAttribute<T> (key.Item1, true));
    }

    private static ArgumentException CreateArgumentException (string message, params object[] args)
    {
      return new ArgumentException (string.Format (message, args));
    }
  }
}