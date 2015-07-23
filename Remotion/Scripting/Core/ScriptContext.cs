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
using Remotion.Context;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// Represents a re-motion script context, which is used to isolate different re-motion modules from one another.
  /// Static members give access to the currently active script context.
  /// </summary>
  /// <remarks>
  /// <seealso cref="ScriptEnvironment"/>
  /// <seealso cref="ExpressionScript{TResult}"/>
  /// <seealso cref="ScriptFunction{TResult}"/>
  /// </remarks>
  public class ScriptContext
  {
    private static readonly SafeContextSingleton<ScriptContext> s_scriptContext =
        new SafeContextSingleton<ScriptContext> (SafeContextKeys.ScriptingScriptContext, () => null);

    private static readonly Dictionary<string, ScriptContext> s_scriptContexts = new Dictionary<string, ScriptContext>();
    private static readonly Object s_scriptContextLock = new object();

    /// <summary>
    /// The currently active <see cref="ScriptContext"/>. Thread safe through <see cref="SafeContext"/>. When a script is executed, it cally
    /// <see cref="Execute{TResult}"/>, which temporarily sets <see cref="Current"/> to the context associated with the script.
    /// </summary>
    public static ScriptContext Current
    {
      get { return s_scriptContext.Current; }
      private set { s_scriptContext.SetCurrent (value); }
    }

    /// <summary>
    /// Creates a new <see cref="ScriptContext"/>.
    /// </summary>
    /// <param name="name">The tag name of the <see cref="ScriptContext"/>. Must be unique. Suggested naming scheme: 
    /// company domain name + namespace of module (e.g. "rubicon.eu.Remotion.Data.DomainObjects.Scripting", "microsoft.com.Word.Scripting").
    /// </param>
    /// <param name="typeFilter">
    /// The <see cref="ITypeFilter"/> which decides which <see cref="Type"/>|s are known in the <see cref="ScriptContext"/>.
    /// </param>
    public static ScriptContext Create (string name, ITypeFilter typeFilter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("typeFilter", typeFilter);

      lock (s_scriptContextLock)
      {
        var scriptContext = new ScriptContext (name, typeFilter);
        try
        {
          s_scriptContexts.Add (name, scriptContext);
        }
        catch (ArgumentException)
        {
          var message = string.Format ("ScriptContext '{0}' already exists.", name);
          throw new ArgumentException (message);
        }

        return scriptContext;
      }
    }

    /// <summary>
    /// Retrieves the <see cref="ScriptContext"/> corresponding the passed <paramref name="name"/>. 
    /// </summary>
    /// <returns>The <see cref="ScriptContext"/>, or <see langword="null" /> if no context with the given name has been defined using 
    /// <see cref="Create"/>.</returns>
    public static ScriptContext GetScriptContext (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      lock (s_scriptContextLock)
      {
        ScriptContext scriptContext;
        s_scriptContexts.TryGetValue (name, out scriptContext);
        return scriptContext;
      }
    }

    /// <summary>
    /// Clears the registry of script contexts created so far via <see cref="Create"/>.
    /// </summary>
    public static void ClearScriptContexts ()
    {
      lock (s_scriptContextLock)
      {
        s_scriptContexts.Clear ();
      }
    }

    private readonly string _name;
    private readonly StableBindingProxyProvider _proxyProvider;

    private ScriptContext (string name, ITypeFilter typeFilter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("typeFilter", typeFilter);

      _name = name;
      
      var moduleScope = ReflectionHelper.CreateModuleScope ("Scripting.ScriptContext." + name, false);
      _proxyProvider = new StableBindingProxyProvider (typeFilter, moduleScope);
    }

    /// <summary>
    /// The name through which this <see cref="ScriptContext"/> is uniquely identified.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// The <see cref="ITypeFilter"/> used in this <see cref="ScriptContext"/> to discern which methods/properties 
    /// shall be exposed to the DLR (see <see cref="ForwardingProxyBuilder"/>).
    /// </summary>
    public ITypeFilter TypeFilter
    {
      get { return _proxyProvider.TypeFilter; }
    }

    /// <summary>
    /// The <see cref="StableBindingProxyProvider"/> used in this <see cref="ScriptContext"/> to create stable binding proxies 
    /// (see <see cref="ForwardingProxyBuilder"/>).
    /// </summary>
    public StableBindingProxyProvider StableBindingProxyProvider
    {
      get { return _proxyProvider; }
    }

    /// <summary>
    /// Executes the specified delegate in the scope of this <see cref="ScriptContext"/>. During delegate execution, this <see cref="ScriptContext"/>
    /// will become the <see cref="Current"/> context. After execution, the <see cref="Current"/> is cleaned up.
    /// </summary>
    /// <typeparam name="TResult">The result type of the <paramref name="func"/> delegate to be executed.</typeparam>
    /// <param name="func">The delegate to be executed.</param>
    /// <returns>The result of the delegate.</returns>
    public TResult Execute<TResult> (Func<TResult> func)
    {
      ArgumentUtility.CheckNotNull ("func", func);

      Assertion.IsNull (Current);
      Current = this;
      try
      {
        return func ();
      }
      finally
      {
        Assertion.IsTrue (Current == this);
        Current = null;
      }
    }

    /// <summary>
    /// Given a DLR object, returns a proxy object that should be used to look up a member (= attribute) of that DLR object. 
    /// The proxy object implements stable binding, and objects requiring stable binding support should call this method from their implementation of
    /// <c>GetCustomMember</c>. (See also <see cref="StableBindingMixin"/>, which automatically calls <see cref="GetAttributeProxy"/>.)
    /// </summary>
    /// <param name="proxied">The object whose attribute should be looked up.</param>
    /// <param name="attributeName">The name of the attribute to be looked up.</param>
    /// <returns>A proxy object that can be returned to the DLR for look-up of <paramref name="attributeName"/> on the <paramref name="proxied"/>
    /// object.</returns>
    public object GetAttributeProxy (object proxied, string attributeName)
    {
      ArgumentUtility.CheckNotNull ("proxied", proxied);
      ArgumentUtility.CheckNotNullOrEmpty ("attributeName", attributeName);

      return StableBindingProxyProvider.GetAttributeProxy (proxied, attributeName);
    }
  }
}
