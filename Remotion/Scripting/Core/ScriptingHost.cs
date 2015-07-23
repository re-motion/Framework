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
using System.Collections.ObjectModel;
using Microsoft.Scripting.Hosting;
using Remotion.Context;


namespace Remotion.Scripting
{
  /// <summary>
  /// Provides access to Dynamic Language Runtime <see cref="ScriptEngine"/>|s through its static members. 
  /// Returned <see cref="ScriptEngine"/>|s are local to the calling thread.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Note: Script Languages must be registered in the "App.config" &lt;microsoft.scripting&gt;-section of the scriptable application
  /// with the string representation of the respective <see cref="ScriptLanguageType"/> given under the &lt;languages&gt;-tag
  /// names-attribute. e.g.: 
  /// <code lang="XML"><![CDATA[
  /// <microsoft.scripting>
  ///   <languages>
  ///     <language names="Python" extensions=".py" displayName="IronPython 2.0" type="IronPython.Runtime.PythonContext, IronPython, Version=2.0.0.0000, Culture=neutral" />
  ///   </languages>  
  /// </microsoft.scripting>
  /// ]]></code>
  /// </para>
  /// <para>
  /// <example>
  /// <code  escaped="true" lang="C#">
  /// // Retrieve IronPython DLR ScriptEngine
  /// var pythonEngine = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python);
  /// </code>
  /// </example>
  /// </para>
  /// </remarks>
  public class ScriptingHost
  {
    private static readonly SafeContextSingleton<ScriptingHost> s_scriptingHost =
        new SafeContextSingleton<ScriptingHost> (SafeContextKeys.ScriptingScriptingHost, () => new ScriptingHost ());

    private ScriptRuntime _scriptRuntime;
    private ReadOnlyDictionary<ScriptLanguageType, ScriptEngine> _scriptEngines;


    /// <summary>
    /// The currently active <see cref="ScriptingHost"/>. Thread safe through <see cref="SafeContext"/>.
    /// </summary>
    public static ScriptingHost Current
    {
      get { return s_scriptingHost.Current; }
    }

    /// <summary>
    /// Retrieves the ScriptEngine given by the <paramref name="languageType"/> parameter. Throws if requested engine is not available on system.
    /// </summary>
    /// <remarks>
    /// Note: Executing scripts directly through a <see cref="ScriptEngine"/> will bypass re-motion's Extension Module separation
    /// (see <see cref="ScriptContext"/>). For guaranteed re-motion mixin stable binding use re-motion  
    /// <see cref="ScriptFunction{TFixedArg1,TResult}"/>.<see cref="ScriptFunction{TFixedArg1,TResult}.Execute"/> etc instead.
    /// </remarks>
    public static ScriptEngine GetScriptEngine (ScriptLanguageType languageType)
    {
      return Current.GetEngine (languageType);
    }



    private ScriptingHost () {}

    private ScriptRuntime GetScriptRuntime ()
    {
      if (_scriptRuntime == null)
      {
        _scriptRuntime = new ScriptRuntime (ScriptRuntimeSetup.ReadConfiguration());
      }
      return _scriptRuntime;
    }

    private ReadOnlyDictionary<ScriptLanguageType, ScriptEngine> GetScriptEngines ()
    {
      if (_scriptEngines == null)
      {
        _scriptEngines = FindScriptEngines ();
      }
      return _scriptEngines;
    }

    private ReadOnlyDictionary<ScriptLanguageType, ScriptEngine> FindScriptEngines ()
    {
      var scriptEngines = new Dictionary<ScriptLanguageType, ScriptEngine> ();
      foreach (ScriptLanguageType languageType in Enum.GetValues (typeof (ScriptLanguageType)))
      {
        ScriptEngine engine;
        var engineAvailable = GetScriptRuntime().TryGetEngine (languageType.ToString(), out engine);
        if (engineAvailable)
        {
          scriptEngines[languageType] = engine;
        }
      }
      return new ReadOnlyDictionary<ScriptLanguageType, ScriptEngine> (scriptEngines);
    }


    private ScriptEngine GetEngine (ScriptLanguageType languageType)
    {
      ScriptEngine scriptEngine;
      bool engineAvailable = GetScriptEngines().TryGetValue (languageType, out scriptEngine);
      if (!engineAvailable)
      {
        throw new NotSupportedException (String.Format ("ScriptEngine for ScriptLanguageType {0} cannot be supplied. Check App.config <microsoft.scripting>-section.",languageType));
      }
      return scriptEngine;
    }
  }
}
