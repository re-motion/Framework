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
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Remotion.Utilities;

namespace Remotion.Scripting
{
  /// <summary>
  /// An <see cref="ExpressionScript{TResult}"/> is a script which contains only a single expression. During script execution this 
  /// expression is evaluated and the result returned. 
  /// </summary>
  /// <remarks>
  /// Under IronPython, <see cref="ExpressionScript{TResult}"/>|s are safer than 
  /// full blown <see cref="ScriptFunction{TResult}"/>|s, since they cannot contain import statements, i.e. they can only access the objects
  /// which can be reached from within their <see cref="ScriptScope"/>.
  /// </remarks>
  /// <typeparam name="TResult">The expression result type.</typeparam>
  public class ExpressionScript<TResult> : ScriptBase
  {
    private readonly ScriptSource _scriptSource;
    private readonly ScriptEnvironment _scriptEnvironment;

    private CompiledCode _compiledScript;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionScript{TResult}"/> class.
    /// </summary>
    /// <param name="scriptContext">
    ///   The <see cref="ScriptContext"/> to use when executing the script. The script context is used to isolate re-motion modules from each other.
    /// </param>
    /// <param name="scriptLanguageType">The script language to use for the script.</param>
    /// <param name="scriptText">
    ///   The source code of the script. This must be an expression in the language defined by <paramref name="scriptLanguageType"/>.
    /// </param>
    /// <param name="scriptEnvironment">
    ///   The <see cref="ScriptEnvironment"/> defining the variables and imported symbols the script has access to.
    /// </param>
    public ExpressionScript (
        ScriptContext scriptContext,
        ScriptLanguageType scriptLanguageType,
        string scriptText,
        ScriptEnvironment scriptEnvironment)
      : base (
          ArgumentUtility.CheckNotNull ("scriptContext", scriptContext), 
          scriptLanguageType, 
          ArgumentUtility.CheckNotNullOrEmpty ("scriptText", scriptText))
    {
      ArgumentUtility.CheckNotNull ("scriptEnvironment", scriptEnvironment);

      _scriptEnvironment = scriptEnvironment;

      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      _scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Expression);
    }

    /// <summary>
    /// Executes the script expression in compiled form. If the script hasn't been compiled yet, it is compiled before execution. The compilation
    /// results are reused when this <see cref="ExpressionScript{TResult}"/> is executed again.
    /// </summary>
    /// <returns>The result of running the script.</returns>   
    public TResult Execute ()
    {
      if (_compiledScript == null)
      {
         _compiledScript = _scriptSource.Compile ();
      }

      return ScriptContext.Execute (() => _compiledScript.Execute<TResult> (_scriptEnvironment.ScriptScope));
    }

    /// <summary>
    /// Executes the script expression without compiling it. Use to avoid the compilation overhead of running a script.
    /// </summary>
    /// <remarks>An uncompiled script executes orders of magnitude slower than a compiled one. So make sure the
    /// compiliation overhead saved is not offset by the increased runtime cost.</remarks>
    /// <returns>The result of running the script.</returns>
    public TResult ExecuteUncompiled ()
    {
      return ScriptContext.Execute (() => _scriptSource.Execute<TResult> (_scriptEnvironment.ScriptScope));
    }
  }
}
