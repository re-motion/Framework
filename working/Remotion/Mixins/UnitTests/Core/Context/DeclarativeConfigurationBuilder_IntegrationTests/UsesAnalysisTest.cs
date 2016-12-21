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
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class UsesAnalysisTest
  {
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class User
    {
    }

    [Test]
    public void Origin ()
    {
      var configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (User)).BuildConfiguration();
      var context = configuration.GetContext (typeof (User));
      var mixinContext = context.Mixins.Single();

      var expectedOrigin = new MixinContextOrigin (
          "UsesAttribute",
          typeof (User).Assembly,
          "Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests.UsesAnalysisTest+User");
      Assert.That (mixinContext.Origin, Is.EqualTo (expectedOrigin));
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class UserWithoutDependencies
    {
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new[] { typeof (string) })]
    [IgnoreForMixinConfiguration]
    public class UserWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      MixinConfiguration configuration =
          new DeclarativeConfigurationBuilder (null).AddType (typeof (UserWithoutDependencies)).AddType (typeof (UserWithDependencies)).BuildConfiguration();
      Assert.That (configuration.GetContext (typeof (UserWithoutDependencies)).Mixins[typeof (NullMixin)].ExplicitDependencies.Count, Is.EqualTo (0));
      Assert.That (configuration.GetContext (typeof (UserWithDependencies)).Mixins[typeof (NullMixin)].ExplicitDependencies.Count, Is.EqualTo (1));
      Assert.That (configuration.GetContext (typeof (UserWithDependencies)).Mixins[typeof (NullMixin)].ExplicitDependencies, Has.Member (typeof (string)));
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new[] { typeof (object) })]
    [IgnoreForMixinConfiguration]
    public class BaseWithUses { }

    public class DerivedWithoutUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithoutUses)).BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (DerivedWithoutUses)).Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (configuration.GetContext (typeof (DerivedWithoutUses)).Mixins[typeof (NullMixin)].ExplicitDependencies, Has.Member (typeof (object)));
      Assert.That (configuration.GetContext (typeof (DerivedWithoutUses)).Mixins.Count, Is.EqualTo (1));
    }

    public class DedicatedMixin {}

    [Uses( typeof (DedicatedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOwnUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited_AndAugmentedWithOwn ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOwnUses)).BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (DerivedWithOwnUses)).Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (configuration.GetContext (typeof (DerivedWithOwnUses)).Mixins.ContainsKey (typeof (DedicatedMixin)), Is.True);

      Type[] mixinTypes = configuration.GetContext (typeof (DerivedWithOwnUses)).Mixins.Select (mixin => mixin.MixinType).ToArray();
      
      Assert.That (mixinTypes, Is.EquivalentTo (new[] {typeof (NullMixin), typeof (DedicatedMixin)}));
      Assert.That (configuration.GetContext (typeof (DerivedWithOwnUses)).Mixins.Count, Is.EqualTo (2));
    }

    [Uses (typeof (NullMixin))]
// ReSharper disable UnusedTypeParameter
    public class GenericBaseWithMixin<T>
    {
    }

    public class GenericDerivedWithInheritedMixin<T> : GenericBaseWithMixin<T>
    {
    }
// ReSharper restore UnusedTypeParameter

    [Test]
    public void UsesAttributeIsInheritedOnOpenGenericTypes ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (GenericDerivedWithInheritedMixin<>)).BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (GenericDerivedWithInheritedMixin<>)).Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (configuration.GetContext (typeof (GenericDerivedWithInheritedMixin<>)).Mixins.Count, Is.EqualTo (1));
    }

    public class NonGenericDerivedWithInheritedMixinFromGeneric : GenericBaseWithMixin<int>
    {
    }

    [Test]
    public void UsesAttributeIsInheritedOnNonGenericTypesInheritingFromGeneric ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (configuration.GetContext (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).Mixins.Count, Is.EqualTo (1));
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesDuplicateIsIgnored ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithUses)).BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (DerivedWithUses)).Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (configuration.GetContext (typeof (DerivedWithUses)).Mixins[typeof (NullMixin)].ExplicitDependencies, Has.No.Member (typeof (object)));
      Assert.That (configuration.GetContext (typeof (DerivedWithUses)).Mixins.Count, Is.EqualTo (1));
    }

    [Uses (typeof (DerivedNullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithMoreSpecificUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesCanBeOverridden ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithMoreSpecificUses)).BuildConfiguration ();
      Assert.That (configuration.GetContext (typeof (DerivedWithMoreSpecificUses)).Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
      Assert.That (configuration.GetContext (typeof (DerivedWithMoreSpecificUses)).Mixins.ContainsKey (typeof (NullMixin)), Is.False);
      Assert.That (configuration.GetContext (typeof (DerivedWithMoreSpecificUses)).Mixins.Count, Is.EqualTo (1));
    }

    public class BaseGenericMixin<TTarget, TNext> : Mixin<TTarget, TNext>
        where TTarget : class
        where TNext : class
    { }

    public class DerivedGenericMixin<TTarget, TNext> : BaseGenericMixin<TTarget, TNext>
        where TTarget : class
        where TNext : class
    {
    }

    public class DerivedClosedMixin : BaseGenericMixin<object, object> { }

    [Uses (typeof (BaseGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class BaseWithOpenGeneric
    {
    }

    [Uses (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>))]
    [IgnoreForMixinConfiguration]
    public class BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOpenOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithClosedOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOpenOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithClosedOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedClosedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithRealClosedOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedClosedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithRealClosedOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOpenOverridingOpen)).BuildConfiguration ().GetContext (typeof (DerivedWithOpenOverridingOpen));
      Assert.That (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<,>)), Is.True);
      Assert.That (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<,>)), Is.False);
      Assert.That (ctx.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOpenOverridingClosed)).BuildConfiguration ().GetContext (typeof (DerivedWithOpenOverridingClosed));
      Assert.That (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<,>)), Is.True);
      Assert.That (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)), Is.False);
      Assert.That (ctx.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithClosedOverridingOpen)).BuildConfiguration ().GetContext (typeof (DerivedWithClosedOverridingOpen));
      Assert.That (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<object, object>)), Is.True);
      Assert.That (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<,>)), Is.False);
      Assert.That (ctx.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithClosedOverridingClosed)).BuildConfiguration ().GetContext (typeof (DerivedWithClosedOverridingClosed));
      Assert.That (ctx.Mixins.ContainsKey (typeof (DerivedGenericMixin<object, object>)), Is.True);
      Assert.That (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)), Is.False);
      Assert.That (ctx.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingOpen)).BuildConfiguration ().GetContext (typeof (DerivedWithRealClosedOverridingOpen));
      Assert.That (ctx.Mixins.ContainsKey (typeof (DerivedClosedMixin)), Is.True);
      Assert.That (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<,>)), Is.False);
      Assert.That (ctx.Mixins.Count, Is.EqualTo (1));
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingClosed)).BuildConfiguration ().GetContext (typeof (DerivedWithRealClosedOverridingClosed));
      Assert.That (ctx.Mixins.ContainsKey (typeof (DerivedClosedMixin)), Is.True);
      Assert.That (ctx.Mixins.ContainsKey (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)), Is.False);
      Assert.That (ctx.Mixins.Count, Is.EqualTo (1));
    }

    [Uses (typeof (NullMixin))]
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithDuplicateUses : BaseWithUses
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*NullMixin are configured for target type "
        + ".*DerivedWithDuplicateUses.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnUsesDuplicateOnSameClass ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithDuplicateUses)).BuildConfiguration ();
    }

    [Uses (typeof (BaseGenericMixin<,>))]
    [Uses (typeof (BaseGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics1
    {
    }

    [Uses (typeof (BaseGenericMixin<,>))]
    [Uses (typeof (BaseGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics2
    {
    }

    [Uses (typeof (BaseGenericMixin<DuplicateWithGenerics3, object>))]
    [Uses (typeof (BaseGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics3
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics1.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics1 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics1)).BuildConfiguration ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics2.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics2 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics2)).BuildConfiguration ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics3.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics3 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics3)).BuildConfiguration ();
    }
  }
}
