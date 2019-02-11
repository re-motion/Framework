﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.Services
{
  /// <summary>
  /// Default implementation of the <see cref="IWebServiceFactory"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IWebServiceFactory), Lifetime = LifetimeKind.Singleton)]
  public class WebServiceFactory : IWebServiceFactory
  {
    private static readonly ConcurrentDictionary<Type, IReadOnlyCollection<Tuple<string, IReadOnlyCollection<string>>>> s_serviceMethodCache =
        new ConcurrentDictionary<Type, IReadOnlyCollection<Tuple<string, IReadOnlyCollection<string>>>>();

    private readonly IBuildManager _buildManager;
    private static readonly Func<Type, IReadOnlyCollection<Tuple<string, IReadOnlyCollection<string>>>> s_getServiceMethodsFunc = GetServiceMethods;

    public WebServiceFactory (IBuildManager buildManager)
    {
      ArgumentUtility.CheckNotNull ("buildManager", buildManager);
      _buildManager = buildManager;
    }

    public T CreateWebService<T> (string virtualPath) where T: class
    {
      var compiledType = GetAndCheckCompiledType<T> (virtualPath);

      foreach (var searchServiceMethod in GetServiceMethodsFromCache<T>())
        WebServiceUtility.CheckWebService (compiledType, searchServiceMethod.Item1);

      return (T) Activator.CreateInstance (compiledType);
    }

    public T CreateScriptService<T> (string virtualPath) where T: class
    {
      var compiledType = GetAndCheckCompiledType<T> (virtualPath);

      foreach (var serviceMethod in GetServiceMethodsFromCache<T>())
        WebServiceUtility.CheckScriptService (compiledType, serviceMethod.Item1);

      return (T) Activator.CreateInstance (compiledType);
    }

    public T CreateJsonService<T> (string virtualPath) where T: class
    {
      var compiledType = GetAndCheckCompiledType<T> (virtualPath);

      foreach (var serviceMethod in GetServiceMethodsFromCache<T>())
        WebServiceUtility.CheckJsonService (compiledType, serviceMethod.Item1, serviceMethod.Item2);

      return (T) Activator.CreateInstance (compiledType);
    }

    private Type GetAndCheckCompiledType<T> (string virtualPath)
    {
      var compiledType = _buildManager.GetCompiledType (virtualPath);

      if (compiledType == null)
        throw new InvalidOperationException (string.Format ("Web service '{0}' could not be compiled.", virtualPath));

      if (!typeof (T).IsAssignableFrom (compiledType))
      {
        var message = typeof (T).IsInterface
                          ? "Web service '{0}' does not implement mandatory interface '{1}'."
                          : "Web service '{0}' is not based on type '{1}'.";

        throw new ArgumentException (string.Format (message, virtualPath, typeof (T).FullName));
      }
      return compiledType;
    }

    private IReadOnlyCollection<Tuple<string, IReadOnlyCollection<string>>> GetServiceMethodsFromCache<T> ()
    {
      return s_serviceMethodCache.GetOrAdd (typeof (T), s_getServiceMethodsFunc);
    }

    private static IReadOnlyCollection<Tuple<string, IReadOnlyCollection<string>>> GetServiceMethods (Type type)
    {
      return Array.AsReadOnly (
          type.GetMethods().Select (
              mi => Tuple.Create<string, IReadOnlyCollection<string>> (
                  mi.Name,
                  Array.AsReadOnly (mi.GetParameters().Select (pi => pi.Name).ToArray()))).ToArray());
    }
  }
}