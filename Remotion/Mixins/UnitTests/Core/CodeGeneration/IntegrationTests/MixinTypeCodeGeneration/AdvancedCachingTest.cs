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
using Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class AdvancedCachingTest : CodeGenerationBaseTest
  {
    [Test]
    public void BaseClassNotOverridingMixinMethod()
    {
      var instance = ObjectFactory.Create<BaseClassNotOverridingMixinMethod> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses>(instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("Mixin.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("Mixin.M2"));
    }

    [Test]
    public void DerivedClassOverridingMixinMethod ()
    {
      var instance = ObjectFactory.Create<DerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("Mixin.M2"));
    }

    [Test]
    public void DerivedClassOverridingMixinMethod2 ()
    {
      var instance = ObjectFactory.Create<DerivedClassOverridingMixinMethod2> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod2.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("Mixin.M2"));
    }

    [Test]
    public void DerivedDerivedClassOverridingMixinMethod ()
    {
      var instance = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedClassOverridingMixinMethod.M2"));
    }

    [Test]
    public void DerivedDerivedClassWithDifferentMixinConfigurations ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<DerivedDerivedClassOverridingMixinMethod> ().AddMixin<MixinWithMethodsOverriddenByDifferentClasses> ().EnterScope ())
      {
        var instance = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
        var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
        Assert.That (mixin.M1 (), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
        Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedClassOverridingMixinMethod.M2"));
      }

      using (MixinConfiguration.BuildNew ().ForClass<DerivedDerivedClassOverridingMixinMethod> ().AddMixin<MixinWithMethodsOverriddenByDifferentClasses2> ().EnterScope())
      {
        var instance = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod>(ParamList.Empty);
        var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses2> (instance);
        Assert.That (mixin.M1(), Is.EqualTo ("DerivedClassOverridingMixinMethod.M1"));
        Assert.That (mixin.M2(), Is.EqualTo ("DerivedDerivedClassOverridingMixinMethod.M2"));
      }
    }

    [Test]
    public void DerivedDerivedDerivedClassOverridingMixinMethod ()
    {
      var instance = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod.M2"));
    }

    [Test]
    public void DerivedDerivedDerivedClassOverridingMixinMethod2 ()
    {
      var instance = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod2> (ParamList.Empty);
      var mixin = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance);
      Assert.That (mixin.M1 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod2.M1"));
      Assert.That (mixin.M2 (), Is.EqualTo ("DerivedDerivedDerivedClassOverridingMixinMethod2.M2"));
    }

    [Test]
    public void Caching_Bottom_To_Top()
    {
      var instance1 = ObjectFactory.Create<BaseClassNotOverridingMixinMethod> (ParamList.Empty);
      var mixin1 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance1);

      var instance2 = ObjectFactory.Create<DerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin2 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2);

      var instance2b = ObjectFactory.Create<DerivedClassOverridingMixinMethod2> (ParamList.Empty);
      var mixin2b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2b);

      var instance3 = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin3 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance3);

      var instance4 = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin4 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4);

      var instance4b = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod2> (ParamList.Empty);
      var mixin4b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4b);

      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin2.GetType ()));
      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2b.GetType (), Is.SameAs (mixin2.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4b.GetType ()));
    }

    [Test]
    public void Caching_Top_To_Bottom ()
    {
      var instance4b = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod2> (ParamList.Empty);
      var mixin4b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4b);

      var instance4 = ObjectFactory.Create<DerivedDerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin4 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance4);

      var instance3 = ObjectFactory.Create<DerivedDerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin3 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance3);

      var instance2b = ObjectFactory.Create<DerivedClassOverridingMixinMethod2> (ParamList.Empty);
      var mixin2b = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2b);

      var instance2 = ObjectFactory.Create<DerivedClassOverridingMixinMethod> (ParamList.Empty);
      var mixin2 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance2);

      var instance1 = ObjectFactory.Create<BaseClassNotOverridingMixinMethod> (ParamList.Empty);
      var mixin1 = Mixin.Get<MixinWithMethodsOverriddenByDifferentClasses> (instance1);

      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin2.GetType ()));
      Assert.That (mixin1.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2.GetType (), Is.Not.SameAs (mixin3.GetType ()));
      Assert.That (mixin2b.GetType (), Is.SameAs (mixin2.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4.GetType ()));
      Assert.That (mixin3.GetType (), Is.SameAs (mixin4b.GetType ()));
    }

    [Test]
    public void Tests_Bottom_To_Top ()
    {
      BaseClassNotOverridingMixinMethod();
      DerivedClassOverridingMixinMethod ();
      DerivedClassOverridingMixinMethod2 ();
      DerivedDerivedClassOverridingMixinMethod();
      DerivedDerivedDerivedClassOverridingMixinMethod ();
      DerivedDerivedDerivedClassOverridingMixinMethod2 ();
    }

    [Test]
    public void Tests_Top_To_Bottom ()
    {
      DerivedDerivedDerivedClassOverridingMixinMethod2 ();
      DerivedDerivedDerivedClassOverridingMixinMethod ();
      DerivedDerivedClassOverridingMixinMethod ();
      DerivedClassOverridingMixinMethod2 ();
      DerivedClassOverridingMixinMethod ();
      BaseClassNotOverridingMixinMethod ();
    }

    [Test]
    public void Sharing_WithProtectedOverriders ()
    {
      var tc = ObjectFactory.Create<TargetClassWithProtectedOverrider> (ParamList.Empty);
      var mixin1 = Mixin.Get<MixinOverriddenByProtectedOverrider> (tc);
      Assert.That (mixin1.M1 (), Is.EqualTo ("TargetClassWithProtectedOverrider.M1()"));

      var dtc = ObjectFactory.Create<DerivedTargetClassWithProtectedOverrider> (ParamList.Empty);
      var mixin2 = Mixin.Get<MixinOverriddenByProtectedOverrider> (dtc);
      Assert.That (mixin2.M1 (), Is.EqualTo ("TargetClassWithProtectedOverrider.M1()"));

      Assert.That (mixin1.GetType (), Is.SameAs (mixin2.GetType ()));
    }
  }
}
