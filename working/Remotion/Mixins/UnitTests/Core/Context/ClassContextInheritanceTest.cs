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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context
{
  [TestFixture]
  public class ClassContextInheritanceTest
  {
    [Test]
    public void InheritFrom_Mixins ()
    {
      var baseContext = ClassContextObjectMother.Create(typeof (string), typeof (DateTime), typeof (int), typeof (DerivedNullTarget));
      var inheritor = ClassContextObjectMother.Create(typeof (double)).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (3));
      Assert.That (inheritor.Mixins, Is.EquivalentTo (baseContext.Mixins));
    }

    [Test]
    public void MixinContext ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DateTime>().WithDependency<int>().BuildClassContext();
      ClassContext inheritor = ClassContextObjectMother.Create(typeof (double)).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins[typeof (DateTime)], Is.EqualTo (baseContext.Mixins[typeof (DateTime)]));
    }

    [Test]
    public void ExistingMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DateTime>().WithDependency<int>().BuildClassContext();
      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DateTime>().WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext }); // ignores inherited DateTime because DateTime already exists

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (1));
      Assert.That (inheritor.Mixins.ContainsKey (typeof (DateTime)), Is.True);
      Assert.That (inheritor.Mixins[typeof (DateTime)].ExplicitDependencies, Has.No.Member (typeof (int)));
      Assert.That (inheritor.Mixins[typeof (DateTime)].ExplicitDependencies, Has.Member (typeof (decimal)));
    }

    [Test]
    public void DerivedMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<NullTarget>().WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DerivedNullTarget>().WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext }); // ignores inherited NullTarget because DerivedNullTarget already exists

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (1));
      Assert.That (inheritor.Mixins.ContainsKey (typeof (NullTarget)), Is.False);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (DerivedNullTarget)), Is.True);
      Assert.That (inheritor.Mixins[typeof (DerivedNullTarget)].ExplicitDependencies, Has.No.Member (typeof (int)));
      Assert.That (inheritor.Mixins[typeof (DerivedNullTarget)].ExplicitDependencies, Has.Member (typeof (decimal)));
    }

    [Test]
    public void BaseAndDerivedMixin_CanBeInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<NullMixin> ().AddMixin<DerivedNullMixin>().BuildClassContext ();
      ClassContext inheritor = ClassContextObjectMother.Create (typeof (double)).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (2));
      Assert.That (inheritor.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
    }

    [Test]
    public void BaseAndDerivedMixin_CanBeInherited_DifferentOrder ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedNullMixin> ().AddMixin<NullMixin> ().BuildClassContext ();
      ClassContext inheritor = ClassContextObjectMother.Create(typeof (double)).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (2));
      Assert.That (inheritor.Mixins.ContainsKey (typeof (NullMixin)), Is.True);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (DerivedNullMixin)), Is.True);
    }

    [Test]
    public void SpecializedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<GenericMixinWithVirtualMethod<object>>().WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (1));
      Assert.That (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<>)), Is.False);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<object>)), Is.True);
      Assert.That (inheritor.Mixins[typeof (GenericMixinWithVirtualMethod<object>)].ExplicitDependencies, Has.No.Member (typeof (int)));
      Assert.That (inheritor.Mixins[typeof (GenericMixinWithVirtualMethod<object>)].ExplicitDependencies, Has.Member (typeof (decimal)));
    }

    [Test]
    public void SpecializedDerivedGenericMixin_OverridesInherited ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<int>().BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double)).AddMixin<DerivedGenericMixin<object>>().WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (1));
      Assert.That (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<>)), Is.False);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (GenericMixinWithVirtualMethod<object>)), Is.False);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (DerivedGenericMixin<>)), Is.False);
      Assert.That (inheritor.Mixins.ContainsKey (typeof (DerivedGenericMixin<object>)), Is.True);

      Assert.That (inheritor.Mixins[typeof (DerivedGenericMixin<object>)].ExplicitDependencies, Has.No.Member (typeof (int)));
      Assert.That (inheritor.Mixins[typeof (DerivedGenericMixin<object>)].ExplicitDependencies, Has.Member (typeof (decimal)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedDerivedMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedNullTarget>().WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin<NullTarget>().WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext });
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void InheritedSpecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin<DerivedGenericMixin<object>>().WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext });
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The class System.Double inherits the mixin "
                                                                           + ".*DerivedGenericMixin\\`1 from class System.String, but it is explicitly configured for the less "
                                                                           + "specific mixin .*GenericMixinWithVirtualMethod\\`1\\[T\\].", MatchType = MessageMatch.Regex)]
    public void InheritedUnspecializedDerivedGenericMixin_Throws ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string)).AddMixin (typeof (DerivedGenericMixin<>)).WithDependency<int>().BuildClassContext();

      new ClassContextBuilder (typeof (double)).AddMixin (typeof (GenericMixinWithVirtualMethod<>)).WithDependency<decimal>().BuildClassContext().InheritFrom (new[] { baseContext });
    }

    [Test]
    public void ComposedInterfaces ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string))
          .AddComposedInterface (typeof (object))
          .AddComposedInterface (typeof (int))
          .BuildClassContext();

      ClassContext inheritor = ClassContextObjectMother.Create(typeof (double)).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.ComposedInterfaces.Count, Is.EqualTo (2));
      Assert.That (inheritor.ComposedInterfaces, Is.EquivalentTo (inheritor.ComposedInterfaces));
    }

    [Test]
    public void ContainsComposedInterface ()
    {
      var baseContext = new ClassContext (typeof (string), new MixinContext[0], new[] {typeof (object)});

      ClassContext inheritor = ClassContextObjectMother.Create(typeof (double)).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.ComposedInterfaces, Has.Member (typeof (object)));
    }

    [Test]
    public void ExistingComposedInterface_NotReplacedByInheritance ()
    {
      var baseContext = new ClassContext (typeof (string), new MixinContext[0], new[] { typeof (object) });

      ClassContext inheritor = new ClassContext (typeof (double), new MixinContext[0], new[] {typeof (object)}).InheritFrom (new[] { baseContext });

      Assert.That (inheritor.ComposedInterfaces.Count, Is.EqualTo (1));
      Assert.That (inheritor.ComposedInterfaces, Has.Member (typeof (object)));
    }

    [Test]
    public void InheritFrom_LeavesExistingData ()
    {
      ClassContext baseContext = new ClassContextBuilder (typeof (string))
          .AddMixin (typeof (DateTime))
          .AddComposedInterface (typeof (object))
          .BuildClassContext();

      ClassContext inheritor = new ClassContextBuilder (typeof (double))
          .AddMixin (typeof (string))
          .AddComposedInterface (typeof (int)).BuildClassContext().InheritFrom (new[] { baseContext });

      Assert.That (inheritor.Mixins.Count, Is.EqualTo (2));
      Assert.That (inheritor.ComposedInterfaces.Count, Is.EqualTo (2));
    }

    [Test]
    public void MixinsOnInterface ()
    {
      MixinConfiguration configuration = MixinConfiguration.BuildNew ().ForClass<IBaseType2> ().AddMixin<BT2Mixin1> ().BuildConfiguration ();

      ClassContext classContext = configuration.GetContext (typeof (IBaseType2));
      Assert.That (classContext, Is.Not.Null);

      Assert.That (classContext.Mixins.ContainsKey (typeof (BT2Mixin1)), Is.True);
    }
  }
}
