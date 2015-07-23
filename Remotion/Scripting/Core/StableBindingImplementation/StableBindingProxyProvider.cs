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
using System.Reflection;
using Castle.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Scripting.StableBindingImplementation
{
  /// <summary>
  /// Creates and caches forwarding proxy objects which expose only the members known in the current <see cref="ScriptContext"/>.
  /// </summary>
  /// <remarks>
  /// Used by the re-motion mixin engine to present only the members of a class known in the current <see cref="ScriptContext"/>
  /// to the Dynamic Language Runtime, thereby guaranteeing that mixins coming from different re-motion modules do not 
  /// interfere with the mixins and scripts coming from a specific module.
  /// </remarks>
  public class StableBindingProxyProvider
  {
    protected static void SetProxiedFieldValue (object proxy, object value)
    {
      ArgumentUtility.CheckNotNull ("proxy", proxy);

      var proxiedField = GetProxiedField (proxy);
      proxiedField.SetValue (proxy, value);
    }

    public static FieldInfo GetProxiedField (object proxy)
    {
      ArgumentUtility.CheckNotNull ("proxy", proxy);

      Type proxyType = GetActualType (proxy);
      return proxyType.GetField ("_proxied", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    protected static Type GetActualType (object proxy)
    {
      ArgumentUtility.CheckNotNull ("proxy", proxy);

      var objectGetType = typeof (object).GetMethod ("GetType");
      return (Type) objectGetType.Invoke (proxy, new object[0]);
    }

    private readonly ITypeFilter _typeFilter;
    private readonly ModuleScope _moduleScope;
    private readonly Dictionary<Type, Type> _proxiedTypeToProxyTypeCache = new Dictionary<Type, Type> ();
    private readonly Dictionary<Type, object> _proxiedTypeToProxyCache = new Dictionary<Type, object> ();
    private readonly Dictionary<Tuple<Type, string>, AttributeProxyCached> _proxiedTypeToAttributeProxyCache = new Dictionary<Tuple<Type, string>, AttributeProxyCached> ();

    public StableBindingProxyProvider (ITypeFilter typeFilter, ModuleScope moduleScope)
    {
      _typeFilter = typeFilter;
      _moduleScope = moduleScope;
    }

    public ITypeFilter TypeFilter
    {
      get { return _typeFilter; }
    }

    public ModuleScope ModuleScope
    {
      get
      {
        return _moduleScope;
      }
    }


    public object GetAttributeProxy (Object proxied, string attributeName)
    {
      var key = new Tuple<Type, string> (proxied.GetType(), attributeName);

      AttributeProxyCached attributeProxyCached;
      if (!_proxiedTypeToAttributeProxyCache.TryGetValue (key, out attributeProxyCached))
      {
        object proxy = GetProxy (proxied);
        var attributeProxy = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python).Operations.GetMember (proxy, attributeName);
        attributeProxyCached = new AttributeProxyCached (proxy, attributeProxy);
        _proxiedTypeToAttributeProxyCache[key] = attributeProxyCached;
      }

      ((IProxy) attributeProxyCached.Proxy).SetProxied (proxied);

      return attributeProxyCached.AttributeProxy;
    }

    protected Type BuildProxyType (Type proxiedType)
    {
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, _typeFilter, _moduleScope);
      return stableBindingProxyBuilder.BuildProxyType ();
    }

    protected Type GetProxyType (Type proxiedType)
    {
      var key = proxiedType;
      Type proxyType;
      if (!_proxiedTypeToProxyTypeCache.TryGetValue (key, out proxyType))
      {
        proxyType = BuildProxyType (proxiedType);
        _proxiedTypeToProxyTypeCache[key] = proxyType;
      }

      return proxyType;
    }


    protected object BuildProxy (object proxied)
    {
      Type proxyType = GetProxyType (proxied.GetType ());
      var proxy = Activator.CreateInstance (proxyType, proxied);
      // Set proxied member in proxy to null, so it will not keep the proxied object alive.
      SetProxiedFieldValue (proxy, null);
      return proxy;
    }


    protected object GetProxy (object proxied)
    {
      var key = proxied.GetType ();
      object proxy;
      if (!_proxiedTypeToProxyCache.TryGetValue (key, out proxy))
      {
        proxy = BuildProxy (proxied);
        _proxiedTypeToProxyCache[key] = proxy;
      }

      SetProxiedFieldValue (proxy, proxied);
      Assertion.IsNotNull (proxy);
      return proxy;
    }


  }
}
