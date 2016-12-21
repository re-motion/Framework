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
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.TypePipe;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class StableBindingProxyProviderPerformanceTests
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("rubicon.eu.Remotion.Scripting.StableBindingProxyProviderPerformanceTests",
      new TypeLevelTypeFilter (new[] { typeof (ICascade1) }));



    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name
";

      const int numberChildren = 10;
      var cascadeWithoutStableBinding = new Cascade (numberChildren);
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import (typeof (TestDomain.Cascade).Assembly.GetName().Name, typeof (TestDomain.Cascade).Namespace, typeof (TestDomain.Cascade).Name);

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      //var nrLoopsArray = new[] { 1, 1, 10000 };
      var nrLoopsArray = new[] { 1, 1, 100000 };

      // Warm up
      ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding)).Last ();
      
      double timingStableBinding = ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding)).Last ();
      double timingWithoutStableBinding = ScriptingHelper.ExecuteAndTime (nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeWithoutStableBinding)).Last ();

      //To.ConsoleLine.e (() => timingStableBinding).e (() => timingWithoutStableBinding);
      //To.ConsoleLine.e ("timingStableBinding / timingWithoutStableBinding = ", timingStableBinding / timingWithoutStableBinding);

      Assert.That (timingStableBinding / timingWithoutStableBinding, Is.LessThan (7.0));
    }


    [Test]
    [Explicit]
    public void CompiledVsUncompiled ()
    {
      var nrLoopsArray = new[] { 1, 1, 1000 };
      const string scriptExpressionSourceCode = "GLOBAL_cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import (typeof (TestDomain.Cascade).Assembly.GetName().Name, typeof (TestDomain.Cascade).Namespace, typeof (TestDomain.Cascade).Name);
      privateScriptEnvironment.SetVariable ("GLOBAL_cascade", cascade);

      ExpressionScript<string> expressionScript = new ExpressionScript<string> (
          _scriptContext, ScriptLanguageType.Python,
          scriptExpressionSourceCode, privateScriptEnvironment
          );

      // Warm up the DLR
      expressionScript.ExecuteUncompiled ();
      ScriptingHelper.ExecuteAndTime ("CompiledVsUncompiled (compiled)", nrLoopsArray, () => expressionScript.Execute ());
      ScriptingHelper.ExecuteAndTime ("CompiledVsUncompiled (uncompiled)", nrLoopsArray, () => expressionScript.ExecuteUncompiled ());
    }


    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember1 ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name
";

      const int numberChildren = 10;
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import (typeof (TestDomain.Cascade).Assembly.GetName().Name, typeof (TestDomain.Cascade).Namespace, typeof (TestDomain.Cascade).Name);

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      var nrLoopsArray = new[] { 1, 1, 10000 };
      ScriptingHelper.ExecuteAndTime ("SimplePropertyAccess_GetCustomMember1 (StableBinding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
    }


    [Test]
    [Explicit]
    public void SimplePropertyAccess_GetCustomMember2 ()
    {
      const string scriptFunctionSourceCode = @"
import clr
def PropertyPathAccess(cascade) :
  return cascade.Child.Child.Child.Child.Child.Child.Child.Child.Child.Name
";

      const int numberChildren = 10;
      var cascade = new Cascade (numberChildren);
      var cascadeStableBinding = new CascadeStableBinding (numberChildren);
      var cascadeStableBindingFromMixin = ObjectFactory.Create<CascadeStableBindingFromMixin> (ParamList.Create (numberChildren));

      var privateScriptEnvironment = ScriptEnvironment.Create ();

      privateScriptEnvironment.Import (typeof (TestDomain.Cascade).Assembly.GetName().Name, typeof (TestDomain.Cascade).Namespace, typeof (TestDomain.Cascade).Name);

      var propertyPathAccessScript = new ScriptFunction<Cascade, string> (
        _scriptContext, ScriptLanguageType.Python,
        scriptFunctionSourceCode, privateScriptEnvironment, "PropertyPathAccess"
      );

      var nrLoopsArray = new[] { 1, 1, 100000 };
      ScriptingHelper.ExecuteAndTime ("SimplePropertyAccess_GetCustomMember2 (No StableBinding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascade));
      ScriptingHelper.ExecuteAndTime ("SimplePropertyAccess_GetCustomMember2 (StableBinding from Mixin)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBindingFromMixin));
      ScriptingHelper.ExecuteAndTime ("SimplePropertyAccess_GetCustomMember2 (StableBinding)", nrLoopsArray, () => propertyPathAccessScript.Execute (cascadeStableBinding));
    }


    public interface ICascade1
    {
      Cascade Child { get; set; }
      string Name { get; set; }
    }

    public interface ICascade2
    {
      string Name { get; set; }
    }

    public class Cascade : ICascade1
    {
      protected Cascade _child;
      protected string _name;

      public Cascade ()
      {

      }

      public Cascade (int nrChildren)
      {
        --nrChildren;
        _name = "C" + nrChildren;
        if (nrChildren > 0)
        {
          Child = new Cascade (nrChildren);
        }
      }

      public Cascade Child
      {
        get { return _child; }
        set { _child = value; }
      }

      string ICascade1.Name
      {
        get { return "ICascade1.Name"; }
        set { _name = value + "-ICascade1.Name"; }
      }

      public Cascade GetChild ()
      {
        return Child;
      }

      public string GetName ()
      {
        return _name;
      }
    }


     public class CascadeAmbigous : Cascade, ICascade2
    {
      string ICascade2.Name
      {
        get { return "ICascade2.Name"; }
        set { _name = value + "-ICascade2.Name"; }
      }
    }


    public class CascadeStableBinding : CascadeAmbigous
    {
      public CascadeStableBinding (int nrChildren)
      {
        --nrChildren;
        _name = "C" + nrChildren;
        if (nrChildren > 0)
        {
          Child = new CascadeStableBinding (nrChildren);
        }
      }

      [SpecialName]
      public object GetCustomMember (string name)
      {
        return ScriptContext.Current.GetAttributeProxy (this, name);
      }
    }


    [Uses (typeof (StableBindingMixin))]
    public class CascadeStableBindingFromMixin : Cascade
    {
      public CascadeStableBindingFromMixin (int nrChildren)
      {
        --nrChildren;
        _name = "C" + nrChildren;
        if (nrChildren > 0)
        {
          Child = ObjectFactory.Create<CascadeStableBindingFromMixin> (ParamList.Create (nrChildren));
        }
      }
    }

  }
}
