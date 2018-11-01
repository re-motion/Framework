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
using Remotion.Scripting;
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests;
using Remotion.Scripting.UnitTests.TestDomain;
using Remotion.TypePipe;

[assembly:Mix (typeof (MixinTestChildAssemblyMix), typeof (StableBindingMixin))]


namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingMixinTest
  {
    [SetUp]
    public void SetUp ()
    {
      ScriptContext.ClearScriptContexts();
    }


    [Test]
    [ExpectedException (typeof (MissingMemberException), ExpectedMessage = "'MixinTest' object has no attribute 'StringTimes'")]
    public void MixinTest_IsAmbigous ()
    {
      AssertGetCustomMemberFromScript (new MixinTest (), "MixinTest_IsAmbigous");
    }




    [Test]
    public void MixinTestChildExplicitGetCustomMemberNoMix_IsNotAmbigous ()
    {
      AssertGetCustomMemberFromScript (new MixinTestChildExplicitGetCustomMemberNoMix(), "MixinTestChildExplicitGetCustomMemberNoMix_IsNotAmbigous");
    }


    public void AssertGetCustomMemberFromScript(MixinTest mixinTestChild, string scriptContextName)
    {
      ScriptContext scriptContext = ScriptContext.Create (scriptContextName, new TypeLevelTypeFilter (typeof (IAmbigous1)));

      var result = scriptContext.Execute (() => ScriptingHelper.ExecuteScriptExpression<string> ("p0.StringTimes('intj',3)", mixinTestChild));
      Assert.That (result, Is.EqualTo ("IAmbigous1.StringTimesintjintjintj"));
    }



    [Test]
    public void MixinTestChildUsesMix_IsNotAmbigous ()
    {
      AssertGetCustomMemberFromScript (ObjectFactory.Create<MixinTestChildUsesMix> (ParamList.Empty), "MixinTestChildUsesMix_IsNotAmbigous");
    }

    [Test]
    public void MixinTestChildAssemblyMix_IsNotAmbigous ()
    {
      AssertGetCustomMemberFromScript (ObjectFactory.Create<MixinTestChildAssemblyMix> (ParamList.Empty), "MixinTestChildAssemblyMix_IsNotAmbigous");
    }

    [Test]
    public void MixinTestChild_IsNotAmbigous_Scope ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<MixinTestChild> ().AddMixin<StableBindingMixin> ().EnterScope ())
      {
        AssertGetCustomMemberFromScript (ObjectFactory.Create<MixinTestChild> (ParamList.Empty), "MixinTestChild_IsNotAmbigous_Scope");

      }
    }  
  }


  public class MixinTest : IAmbigous1, IAmbigous2
  {
    string IAmbigous1.StringTimes (string text, int number)
    {
      return "IAmbigous1.StringTimes" + StringTimes (text, number);
    }
    
    string IAmbigous2.StringTimes (string text, int number)
    {
      return "IAmbigous2.StringTimes" + StringTimes (text, number);
    }

    private string StringTimes (string text, int number)
    {
      return text.ToSequence (number).Aggregate ((sa, s) => sa + s);
    }
  }


  public class MixinTestChild : MixinTest
  {
  }

  public class MixinTestChildExplicitGetCustomMemberNoMix : MixinTest
  {
    [SpecialName]
    public object GetCustomMember (string name)
    {
      return ScriptContext.Current.GetAttributeProxy (this, name);
    }
  }

  [Uses (typeof (StableBindingMixin))]
  public class MixinTestChildUsesMix : MixinTest
  {
  }

  public class MixinTestChildAssemblyMix : MixinTest
  {
  }

 
}
