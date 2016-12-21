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
using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingHostTest
  {
    [Test]
    public void FindScriptEngines ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var scriptEngines = scriptingHost.FindScriptEngines ();
      Assert.That (scriptEngines, Is.Not.Null);
      Assert.That (scriptEngines.Count, Is.EqualTo(1));
      Assert.That (scriptEngines[ScriptLanguageType.Python], Is.Not.Null);
    }

    [Test]
    public void GetScriptEngines ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var scriptEngines = scriptingHost.GetScriptEngines ();
      Assert.That (scriptEngines.ToArray(), Is.EquivalentTo (scriptingHost.FindScriptEngines ().ToArray()));
    }


    [Test]
    public void GetScriptRuntime ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost();
      Assert.That (scriptingHost, Is.Not.Null);
      Assert.That (scriptingHost.GetScriptRuntime (), Is.Not.Null);
      Assert.That (scriptingHost.GetScriptRuntime ().Setup.LanguageSetups.Count, Is.EqualTo (1));
      Assert.That (scriptingHost.GetScriptRuntime ().Setup.LanguageSetups[0].TypeName, Is.StringStarting("IronPython.Runtime.PythonContext"));
    }

    [Test]
    public void ScriptingHost_Current ()
    {
      ScriptingHost scriptingHost = ScriptingHost.Current;
      Assert.That (scriptingHost, Is.Not.Null);
      ScriptingHost scriptingHost2 = ScriptingHost.Current;
      Assert.That (scriptingHost,Is.SameAs(scriptingHost2));
    }
 
    [Test]
    public void ScriptingHost_Current_ThreadStatic ()
    {
      ScriptingHost scriptingHostDifferentThread = null;
      var threadRunner = new ThreadRunner (delegate { scriptingHostDifferentThread = ScriptingHost.Current; });
      threadRunner.Run ();
      ScriptingHost scriptingHost = ScriptingHost.Current; 
      Assert.That (scriptingHost, Is.Not.Null);
      Assert.That (scriptingHostDifferentThread, Is.Not.Null);
      Assert.That (scriptingHost, Is.Not.SameAs (scriptingHostDifferentThread));
    }


    [Test]
    public void GetEngine ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      var pythonEngine = scriptingHost.GetEngine (ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "ScriptEngine for ScriptLanguageType None cannot be supplied. Check App.config <microsoft.scripting>-section.")]
    public void GetEngine_Fails ()
    {
      ScriptingHost scriptingHost = CreateScriptingHost ();
      scriptingHost.GetEngine (ScriptLanguageType.None);
    } 


    [Test]
    public void GetEngine_Static ()
    {
      var pythonEngine = ScriptingHost.GetScriptEngine (ScriptLanguageType.Python);
      Assert.That (pythonEngine, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "ScriptEngine for ScriptLanguageType None cannot be supplied. Check App.config <microsoft.scripting>-section.")]
    public void GetEngine_Static_Fails ()
    {
      ScriptingHost.GetScriptEngine (ScriptLanguageType.None);
    }



    private static ScriptingHost CreateScriptingHost ()
    {
      return (ScriptingHost) PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ScriptingHost).Assembly, "Remotion.Scripting.ScriptingHost");
    }
  }
}
