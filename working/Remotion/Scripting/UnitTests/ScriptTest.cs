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
using NUnit.Framework;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptTest
  {
    [TearDown]
    public void TearDown ()
    {
      ScriptContext.ClearScriptContexts();
    }

    [Test]
    public void Ctor ()
    {
      ScriptContext scriptContext = ScriptContextObjectMother.CreateTestScriptContext ();
      const ScriptLanguageType scriptLanguageType = ScriptLanguageType.Python;
      const string scriptFunctionName = "Test";

      const string scriptText =
@"def Test() :
  return 'CtorTest'";

      var scriptEnvironment = ScriptEnvironment.Create ();

      var script = new ScriptFunction<string> (scriptContext, scriptLanguageType, scriptText, scriptEnvironment, scriptFunctionName);

      Assert.That (script.ScriptContext, Is.EqualTo (scriptContext));
      Assert.That (script.ScriptLanguageType, Is.EqualTo (scriptLanguageType));
      Assert.That (script.ScriptText, Is.EqualTo (scriptText));
      Assert.That (script.Execute (), Is.EqualTo ("CtorTest"));
    }


    [Test]
    public void Ctor_NumberTemplateArguments_1 ()
    {
      const string scriptText =
@"def Test(s) :
  return 'Test: ' + s";

      //ScriptScope scriptScope = ScriptingHelper.CreateScriptScope (ScriptLanguageType.Python);
      var scriptScope = ScriptEnvironment.Create ();
      var script = new ScriptFunction<string, string> (ScriptContextObjectMother.CreateTestScriptContext (), ScriptLanguageType.Python, scriptText, scriptScope, "Test");
      Assert.That (script.Execute ("works"), Is.EqualTo ("Test: works"));
    }


    [Test]
    public void Ctor_NumberTemplateArguments_2 ()
    {
      const string scriptText =
@"def Test(s0,s1) :
  return 'Test: ' + s0 + ' ' + s1";

      var scriptEnvironment = ScriptEnvironment.Create ();
      var script = new ScriptFunction<string, string, string> (ScriptContextObjectMother.CreateTestScriptContext (), 
        ScriptLanguageType.Python, scriptText, scriptEnvironment, "Test");
      Assert.That (script.Execute ("really","works"), Is.EqualTo ("Test: really works"));
    }


    [Test]
    public void Ctor_NumberTemplateArguments_9 ()
    {
      const string scriptText =
@"def Test(s1,s2,s3,s4,s5,s6,s7,s8,s9) :
  return 'Test: '+s1+s2+s3+s4+s5+s6+s7+s8+s9";

      var scriptEnvironment = ScriptEnvironment.Create ();
      var script = new ScriptFunction<string, string, string, string, string, string, string, string, string, string> (
        ScriptContextObjectMother.CreateTestScriptContext (), ScriptLanguageType.Python, scriptText, scriptEnvironment, "Test");
      Assert.That (script.Execute ("1","2","3","4","5","6","7","8","9"), Is.EqualTo ("Test: 123456789"));
    }

    [Test]
    public void Execute_SwitchesAndReleasesScriptContext ()
    {
      Assert.That (ScriptContext.Current , Is.Null);

      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting')
from Remotion.Scripting import *
def Test() :
  return ScriptContext.Current
";

      ScriptContext scriptContextForScript = ScriptContextObjectMother.CreateTestScriptContext ("Execute_SwitchesScriptContext_Script");
      var scriptEnvironment = ScriptEnvironment.Create ();
      var script = new ScriptFunction<ScriptContext> (scriptContextForScript, ScriptLanguageType.Python, scriptText, scriptEnvironment, "Test");
      Assert.That (script.Execute (), Is.SameAs (scriptContextForScript));
      
      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    public void Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows ()
    {
      Assert.That (ScriptContext.Current, Is.Null);

      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting')
from Remotion.Scripting import *
def Test() :
  raise Exception('IntentionallyRaisedIronPythonException') 
";

      ScriptContext scriptContextForScript = ScriptContextObjectMother.CreateTestScriptContext ("Execute_SwitchesAndReleasesScriptContextIfScriptExecutionThrows");
      var scriptEnvironment = ScriptEnvironment.Create ();
      var script = new ScriptFunction<Object> (scriptContextForScript, ScriptLanguageType.Python, scriptText, scriptEnvironment, "Test");

      try
      {
        script.Execute ();
      }
      catch (Exception e)
      {
        Assert.That (e.Message, Is.EqualTo ("IntentionallyRaisedIronPythonException"));
      }

      Assert.That (ScriptContext.Current, Is.Null);
    }


    [Test]
    public void ScriptExecute_ImportIntoScriptScope ()
    {
      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests.TestDomain import Document
def Test() :
  return Document('Knows Document')
";

      ScriptContext scriptContextForScript = ScriptContextObjectMother.CreateTestScriptContext ("Execute_ImportIntoScriptScope");
      var scriptEnvironment = ScriptEnvironment.Create ();
      var script = new ScriptFunction<Document> (scriptContextForScript, ScriptLanguageType.Python, scriptText, scriptEnvironment, "Test");
      Document resultDocument = script.Execute ();
      Assert.That (resultDocument.Name, Is.EqualTo ("Knows Document"));
    }

    [Test]
    public void ScriptExecute_ImportMultipleIntoScriptScope ()
    {
      const string scriptText = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests import *
def Test() :
  return TestDomain.Document('Knows Document')
";

      ScriptContext scriptContextForScript = ScriptContextObjectMother.CreateTestScriptContext ("Execute_ImportIntoScriptScope");
      var scriptEnvironment = ScriptEnvironment.Create ();
      var script = new ScriptFunction<Document> (scriptContextForScript, ScriptLanguageType.Python, scriptText, scriptEnvironment, "Test");
      Document resultDocument = script.Execute ();
      Assert.That (resultDocument.Name, Is.EqualTo ("Knows Document"));
    }

  }
}
