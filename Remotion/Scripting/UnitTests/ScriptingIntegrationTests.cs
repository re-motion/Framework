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
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptingIntegrationTests
  {
    // Create the ScriptContext for the scripts in this re-motion module. The ScriptContext separates scripts from
    // different modules and prevents ambiguitiy exceptions coming from mixed methods with the same signature
    // on the same class.
    // Note: For this example it is more convenient to use a TypeLevelTypeFilter here; in practice, using a 
    // AssemblyLevelTypeFilter will in most cases be the more fitting choice.
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("rubicon.eu.YourModuleName.ScriptingIntegrationTests",
      new TypeLevelTypeFilter(new[] {typeof(ProxiedChild), typeof(IAmbigous2), typeof(Document)}));

    private ScriptEnvironment _scriptEnvironment;

    [SetUp]
    public void SetUp ()
    {
      _scriptEnvironment = ScriptEnvironment.Create ();
    }

    // Shows how to create and use an ExpressionScript. ExpressionScript|s can only contain a single expression and automatically return 
    // the result of evaluating them (without the need for a return statement).
    [Test]
    public void ExpressionScript_CreateAndUse ()
    {
      const string expressionScriptSourceCode = "doc.Name.Contains('Rec') or doc.Number == 123456";

      var doc = new Document ("Receipt");

      // Create a separate script environment for the script expression
      var privateScriptEnvironment = ScriptEnvironment.Create ();
      // Import the CLR (e.g. string etc)
      privateScriptEnvironment.ImportClr();
      // Set variable doc to a Document instance
      privateScriptEnvironment.SetVariable ("doc", doc);

      // Create a script expression which checks the Document object stored in the variable "doc".
      var checkDocumentExpressionScript = 
        new ExpressionScript<bool> (_scriptContext, ScriptLanguageType.Python, expressionScriptSourceCode, privateScriptEnvironment);

      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.Name = "Record";
      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.Number = 123456;
      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.Name = "Report";
      Assert.That (checkDocumentExpressionScript.Execute (), Is.True);
      doc.Number = 21;
      Assert.That (checkDocumentExpressionScript.Execute (), Is.False);
    }

    [Test]
    public void ExpressionScript_HelperFunctions ()
    {
      const string expressionScriptSourceCode = 
        "IIf (doc.Name.Contains('Rec'), doc.Number, LazyIIf (doc.Number == 0, lambda:-1, lambda:1.0/doc.Number))";
        // TODO: Equivalent to "doc.Number if doc.Name.Contains('Rec') else -1 if doc.Number == 0 else 1.0/doc.Number"?;

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      privateScriptEnvironment.ImportClr ();
      // Import script helper functions (IIf and LazyIIf)
      privateScriptEnvironment.ImportIifHelperFunctions();

      var checkDocumentExpressionScript =
        new ExpressionScript<float> (_scriptContext, ScriptLanguageType.Python, expressionScriptSourceCode, privateScriptEnvironment);

      var doc = new Document ("Receipt", 4);
      privateScriptEnvironment.SetVariable ("doc", doc);

      Assert.That (checkDocumentExpressionScript.Execute (), Is.EqualTo (4.0));
      doc.Name = "Document";
      Assert.That (checkDocumentExpressionScript.Execute (), Is.EqualTo (0.25));
      doc.Number = 0;
      Assert.That (checkDocumentExpressionScript.Execute (), Is.EqualTo (-1.0));
    }

    [Test]
    public void ScriptFunction_CreateAndUse ()
    {
      const string scriptFunctionSourceCode = @"
import clr
clr.AddReferenceByPartialName('Remotion.Scripting.UnitTests')
from Remotion.Scripting.UnitTests.TestDomain import Document
def CreateDocument() :
  return Document('Here is your document, sir.')
";

      // Create a script function called "CreateDocument" which returns a Document object.
      var createDocumentScript = new ScriptFunction<Document> (
          _scriptContext, 
          ScriptLanguageType.Python, 
          scriptFunctionSourceCode,
        _scriptEnvironment, 
        "CreateDocument");
      
      Document resultDocument = createDocumentScript.Execute ();

      Assert.That (resultDocument.Name, Is.EqualTo ("Here is your document, sir."));
    }
    
    [Test]
    public void ScriptFunction_CreateAndUseScript_WithPrivateScriptEnvironment ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def CheckDocument(doc) :
  return doc.Name.Contains('Rec') or doc.Number == 123456
";

      var privateScriptEnvironment = ScriptEnvironment.Create ();
      
      // Create a script function called CheckDocument which takes a Document and returns a bool.
      var checkDocumentScript = new ScriptFunction<Document,bool> (
        ScriptContext.GetScriptContext ("rubicon.eu.YourModuleName.ScriptingIntegrationTests"), 
        ScriptLanguageType.Python,
        scriptFunctionSourceCode, 
        privateScriptEnvironment, 
        "CheckDocument");

      Assert.That (checkDocumentScript.Execute (new Document ("Receipt", 123456)), Is.True);
      Assert.That (checkDocumentScript.Execute (new Document ("XXX", 123456)), Is.True);
      Assert.That (checkDocumentScript.Execute (new Document ("Rec", 0)), Is.True);
      Assert.That (checkDocumentScript.Execute (new Document ("XXX", 0)), Is.False);
    }

    [Test]
    public void ScriptFunction_CreateAndUseScripts_InSameScriptEnvironment ()
    {
      // Import CLR (for CLR string functionality)
      _scriptEnvironment.ImportClr ();
      
      // Import the Document class into the shared script environment (otherwise we would not be able to call the Document ctors within
      // the script).
      _scriptEnvironment.Import ("Remotion.Scripting.UnitTests", "Remotion.Scripting.UnitTests.TestDomain", "Document");

      const string scriptFunctionSourceCode0 = @"
def CreateScriptDocSuccessor() :
  return Document(scriptDoc.Name,scriptDoc.Number+1)
";

      const string scriptFunctionSourceCode1 = @"
def ModifyDocument(newNamePostfix, addToNumber) :
  scriptDoc.Name += newNamePostfix
  scriptDoc.Number += addToNumber
  return scriptDoc
";

      // Create a script function which creates a new Document.
      var createScriptDocSuccessorScript = new ScriptFunction<Document> (
          _scriptContext, 
          ScriptLanguageType.Python, 
          scriptFunctionSourceCode0,
          _scriptEnvironment, 
          "CreateScriptDocSuccessor");

      // Create a script function which modifies the passed Document.
      var modifyDocumentScript = new ScriptFunction<string, int, Document> (
          _scriptContext, 
          ScriptLanguageType.Python, 
          scriptFunctionSourceCode1,
          _scriptEnvironment, 
          "ModifyDocument");


      var doc = new Document ("invoice", 1);

      Assert.That (doc.Name, Is.EqualTo ("invoice"));
      Assert.That (doc.Number, Is.EqualTo (1));

      _scriptEnvironment.SetVariable ("scriptDoc", doc);

      Document successorDoc = createScriptDocSuccessorScript.Execute ();
      Document scriptDoc = modifyDocumentScript.Execute (" - processed", 1000);

      Assert.That (successorDoc.Name, Is.EqualTo ("invoice"));
      Assert.That (successorDoc.Number, Is.EqualTo (2));

      Assert.That (scriptDoc.Name, Is.EqualTo ("invoice - processed"));
      Assert.That (scriptDoc.Number, Is.EqualTo (1001));
    }
  }
}
