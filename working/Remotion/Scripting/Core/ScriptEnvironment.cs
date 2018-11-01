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
  /// Wrapper around a DLR <see cref="Microsoft.Scripting.Hosting.ScriptScope"/>. 
  /// A <see cref="ScriptEnvironment"/> contains the symbols (DLR: objects) visible to a script running in this environment.
  /// </summary>
  public class ScriptEnvironment
  {
    private readonly ScriptScope _scriptScope;

    /// <summary>
    /// <see cref="ScriptEnvironment"/> factory method.
    /// </summary>
    public static ScriptEnvironment Create ()
    {
      return new ScriptEnvironment (ScriptingHost.GetScriptEngine (ScriptLanguageType.Python).CreateScope ());
    }
    
    /// <summary>
    /// Wraps the passed <see cref="ScriptScope"/> in a <see cref="ScriptEnvironment"/>. 
    /// </summary>
    /// <param name="scriptScope"></param>
    private ScriptEnvironment (ScriptScope scriptScope)
    {
      ArgumentUtility.CheckNotNull ("scriptScope", scriptScope);
      _scriptScope = scriptScope;
    }

    /// <summary>
    /// Gets the <see cref="ScriptScope"/> wrapped by this <see cref="ScriptEnvironment"/>.
    /// </summary>
    /// <value>The <see cref="ScriptScope"/>.</value>
    public ScriptScope ScriptScope
    {
      get { return _scriptScope; }
    }

    /// <summary>
    /// Imports the special CLR module into the <see cref="ScriptEnvironment"/>. This enables .NET interop functionalities, and it is necessary to
    /// import additional assemblies.
    /// </summary>
    public void ImportClr ()
    {
      const string scriptText = "import clr";

      var engine = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (_scriptScope);
    }

    /// <summary>
    /// Introduces IIf(condition,trueValue,falseValue) and LazyIIf(condition,lambda:trueValue,lambda:falseValue)
    /// functions into the <see cref="ScriptEnvironment"/>. This enables script code to use these functions to return values based on a condition.
    /// </summary>
    public void ImportIifHelperFunctions ()
    {
      _scriptScope.SetVariable ("IIf", new Func<bool, object, object, object> ((cond, trueVal, falseVal) => cond ? trueVal : falseVal));
      _scriptScope.SetVariable ("LazyIIf", new Func<bool, Func<object>, Func<object>, object> ((cond, trueVal, falseVal) => cond ? trueVal () : falseVal ()));
    }

    /// <summary>
    /// Imports the passed symbols from the given namespace in the given assembly into the <see cref="ScriptEnvironment"/>. 
    /// </summary>
    /// <param name="assembly">Partial name of the assembly to import from (e.g. "Remotion")</param>
    /// <param name="nameSpace">Namespace name in assembly to import from (e.g. "Remotion.Diagnostics.ToText")</param>
    /// <param name="symbols">array of symbol names to import (e.g. "To", "ToTextBuilder", ...)</param>
    public void Import (string assembly, string nameSpace, params string[] symbols)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assembly", assembly);
      ArgumentUtility.CheckNotNullOrEmpty ("nameSpace", nameSpace);
      ArgumentUtility.CheckNotNullOrEmpty ("symbols", symbols);

      string scriptText = @"
import clr
clr.AddReferenceByPartialName('" + assembly + "')" +
@"
from " + nameSpace + " import " + string.Join (",", symbols);

      const ScriptLanguageType scriptLanguageType = ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (scriptText, SourceCodeKind.Statements);
      scriptSource.Execute (_scriptScope);
    }

    /// <summary>
    /// Sets a variable with the passed <paramref name="name"/> to the passed <paramref name="value"/> within the <see cref="ScriptEnvironment"/>.
    /// </summary>
    public void SetVariable (string name, Object value)
    {
      _scriptScope.SetVariable (name, value);
    }

    /// <summary>
    /// Gets the value of the variable with the passed <paramref name="name"/> as a <see cref="ScriptVariable{T}"/> struct.
    /// If the variable does not exist within the <see cref="ScriptEnvironment"/>, the 
    /// <see cref="ScriptVariable{T}"/>.<see cref="ScriptVariable{T}.IsValid"/>-property is <see langword="false" />.
    /// </summary> 
    public ScriptVariable<T> GetVariable<T> (string name)
    {
      T value;
      bool isValid = _scriptScope.TryGetVariable (name, out value);
      return new ScriptVariable<T> (value, isValid);
    }
  }
}
