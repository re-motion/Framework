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
using IronPython.Runtime;
using NUnit.Framework;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptEnvironmentTest
  {
    [TearDown]
    public void TearDown ()
    {
      ScriptContext.ClearScriptContexts();
    }

    [Test]
    public void Create ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      Assert.That (scriptEnvironment.ScriptScope, Is.Not.Null);

      var scriptEnvironment2 = ScriptEnvironment.Create ();
      Assert.That (scriptEnvironment2.ScriptScope, Is.Not.Null);

      Assert.That (scriptEnvironment.ScriptScope, Is.Not.SameAs (scriptEnvironment2.ScriptScope));
    }

    [Test]
    public void ImportClr ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      
      scriptEnvironment.ImportClr ();
      
      const string scriptText = "'ABcd'.Substring(1,2)";
      var expressionScript = new ExpressionScript<string> (
          ScriptContextObjectMother.CreateTestScriptContext ("ImportClr"), 
          ScriptLanguageType.Python, 
          scriptText, 
          scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("Bc"));
    }

    [Test]
    [ExpectedException (typeof (MissingMemberException), ExpectedMessage = "'str' object has no attribute 'Substring'")]
    public void NotImportClr ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      const string scriptText = "'ABcd'.Substring(1,2)";
      var expressionScript =
          new ExpressionScript<string> (
              ScriptContextObjectMother.CreateTestScriptContext ("NotImportClr"),
              ScriptLanguageType.Python,
              scriptText,
              scriptEnvironment);
      expressionScript.Execute ();
    }

    [Test]
    public void Import ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.Import ("Remotion.Scripting", "Remotion.Scripting", "ScriptEnvironment", "ScriptContext");

      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ScriptEnvironment"), Is.Not.Null);
      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ScriptContext"), Is.Not.Null);
    }

    [Test]
    public void Import_StrongName ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.Import (typeof (ScriptEnvironment).Assembly.FullName, "Remotion.Scripting", "ScriptEnvironment", "ScriptContext");

      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ScriptEnvironment"), Is.Not.Null);
      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ScriptContext"), Is.Not.Null);
    }

    [Test]
    public void SetVariable ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      var doc = new Document ("ScriptEnvironmentTest_SetVariable");
      
      scriptEnvironment.SetVariable ("ScriptEnvironmentTest_SetVariable", doc);
      
      Assert.That (scriptEnvironment.ScriptScope.GetVariable ("ScriptEnvironmentTest_SetVariable"), Is.SameAs (doc));
    }

    [Test]
    public void GetVariable ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      const string variableName = "ScriptEnvironmentTest_GetVariable";
      
      var variableInvalid = scriptEnvironment.GetVariable<Document> (variableName);
      Assert.That (variableInvalid.IsValid , Is.False);
      
      var doc = new Document ("GetVariable");
      scriptEnvironment.SetVariable (variableName, doc);
      
      var variableValid = scriptEnvironment.GetVariable<Document> (variableName);
      Assert.That (variableValid.IsValid, Is.True);
      Assert.That (variableValid.Value, Is.SameAs (doc));
    }

    [Test]
    public void ImportHelperFunctions_IIf ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportIifHelperFunctions();
      scriptEnvironment.SetVariable ("x", 100000);
      const string scriptText = "IIf(x > 1000,'big','small')";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextObjectMother.CreateTestScriptContext ("ImportIifHelperFunctions"), ScriptLanguageType.Python, 
            scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("big"));
    }

    [Test]
    [ExpectedException (typeof (UnboundNameException), ExpectedMessage = "name 'NonExistingSymbol' is not defined")]
    public void ImportIifHelperFunctions_IIfIsNotLazy ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportIifHelperFunctions ();
      scriptEnvironment.SetVariable ("x", 100000);
      const string scriptText = "IIf(x > 1000,'big',NonExistingSymbol)";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextObjectMother.CreateTestScriptContext ("ImportIifHelperFunctions"), ScriptLanguageType.Python,
            scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("big"));
    }


    [Test]
    public void ImportIifHelperFunctions_LazyIIf ()
    {
      var scriptEnvironment = ScriptEnvironment.Create ();
      scriptEnvironment.ImportIifHelperFunctions ();
      scriptEnvironment.SetVariable ("x", 100000);
      const string scriptText = "LazyIIf(x > 1000,lambda:'big',lambda:NonExistingSymbol)";
      var expressionScript =
          new ExpressionScript<string> (ScriptContextObjectMother.CreateTestScriptContext ("ImportIifHelperFunctions"), ScriptLanguageType.Python,
            scriptText, scriptEnvironment);
      Assert.That (expressionScript.Execute (), Is.EqualTo ("big"));
    }

  }
}
