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
using System.Collections.ObjectModel;
using Microsoft.Scripting.Hosting;
using Remotion.Development.UnitTesting;

namespace Remotion.Scripting.UnitTests
{
  static class ScriptingHostTestHelper 
  {
    public static ScriptRuntime GetScriptRuntime (this ScriptingHost scriptingHost)
    {
      return (ScriptRuntime) PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "GetScriptRuntime");
    }

    public static ReadOnlyDictionary<ScriptLanguageType, ScriptEngine> GetScriptEngines (this ScriptingHost scriptingHost)
    {
      return (ReadOnlyDictionary<ScriptLanguageType, ScriptEngine>)
             PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "GetScriptEngines");
    }

    public static ReadOnlyDictionary<ScriptLanguageType, ScriptEngine> FindScriptEngines (this ScriptingHost scriptingHost)
    {
      return (ReadOnlyDictionary<ScriptLanguageType, ScriptEngine>) 
             PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "FindScriptEngines");
    }

    public static ScriptEngine GetEngine (this ScriptingHost scriptingHost, ScriptLanguageType languageType)
    {
      return (ScriptEngine) PrivateInvoke.InvokeNonPublicMethod (scriptingHost, "GetEngine", languageType);
    }
  }
}
