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
using Remotion.Development.UnitTesting;
using Remotion.Scripting.StableBindingImplementation;
using Rhino.Mocks;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class ScriptContextTest
  {
    private ITypeFilter _typeFilterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeFilterStub = MockRepository.GenerateStub<ITypeFilter> ();
    }

    [TearDown]
    public void TearDown ()
    {
      ScriptContext.ClearScriptContexts ();
    }

    [Test]
    public void Create ()
    {
      var typeFilterStub = _typeFilterStub;
      
      var scriptContext = ScriptContext.Create ("ContextXyz1", typeFilterStub);
      
      Assert.That (scriptContext.Name, Is.EqualTo ("ContextXyz1"));
      Assert.That (scriptContext.TypeFilter, Is.SameAs (typeFilterStub));

      Assert.That (scriptContext.StableBindingProxyProvider.TypeFilter, Is.SameAs (typeFilterStub));
      
      var moduleScope = scriptContext.StableBindingProxyProvider.ModuleScope;
      Assert.That (moduleScope, Is.Not.Null);
      Assert.That (moduleScope.StrongNamedModuleName,Is.StringContaining("Scripting.ScriptContext.ContextXyz1"));
    }

    [Test]
    public void Create_RegistersContext ()
    {
      ScriptContext scriptContext = ScriptContext.Create ("Context", _typeFilterStub);

      Assert.That (ScriptContext.GetScriptContext ("Context"), Is.SameAs (scriptContext));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "ScriptContext 'DuplicateContext' already exists.")]
    public void Create_CreatingSameNamedContextFails ()
    {
      var scriptContext = ScriptContext.Create ("DuplicateContext", _typeFilterStub);
      Assert.That (scriptContext, Is.Not.Null);
     
      ScriptContext.Create ("DuplicateContext", _typeFilterStub);
    }

    [Test]
    public void GetScriptContext_NonExistingContext ()
    {
      Assert.That (ScriptContext.GetScriptContext ("NonExistingContext"), Is.Null);
    }

    [Test]
    public void Execute_RunsDelegate ()
    {
      var scriptContext = CreateScriptContext ();

      var result = scriptContext.Execute (() => 17 + 4);

      Assert.That (result, Is.EqualTo (21));
    }

    [Test]
    public void Execute_SetsScriptContext ()
    {
      var scriptContext = CreateScriptContext ();

      Assert.That (ScriptContext.Current, Is.Null);
      var scriptContextInExecute = scriptContext.Execute (() => ScriptContext.Current);

      Assert.That (scriptContextInExecute, Is.SameAs (scriptContext));
    }

    [Test]
    public void Execute_ResetsScriptContext ()
    {
      var scriptContext = CreateScriptContext ();

      Assert.That (ScriptContext.Current, Is.Null);
      scriptContext.Execute (() => ScriptContext.Current);

      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    public void Execute_ResetsScriptContext_InCaseOfError ()
    {
      var scriptContext = CreateScriptContext ();

      Assert.That (ScriptContext.Current, Is.Null);
      try
      {
        scriptContext.Execute<int> (() => { throw new ApplicationException ("Test"); });
        Assert.Fail ("Expected ApplicationException");
      }
      catch (ApplicationException)
      {
        // ok
      }

      Assert.That (ScriptContext.Current, Is.Null);
    }

    [Test]
    public void Execute_ScriptContext_ThreadStatic ()
    {
      var scriptContext = CreateScriptContext ();

      Assert.That (ScriptContext.Current, Is.Null);
      var scriptContextInExecute = scriptContext.Execute (
          delegate
          {
            Assert.That (ScriptContext.Current, Is.SameAs (scriptContext));
            ThreadRunner.Run (() => Assert.That (ScriptContext.Current, Is.Null));
            Assert.That (ScriptContext.Current, Is.SameAs (scriptContext));
            return ScriptContext.Current;
          });

      Assert.That (scriptContextInExecute, Is.SameAs (scriptContext));
    }

    private ScriptContext CreateScriptContext ()
    {
      return ScriptContext.Create ("Context", _typeFilterStub);
    }
  }
}
