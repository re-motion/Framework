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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.IntegrationTests.Ordering;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class MixinReflectorTest
  {
    [Test]
    public void GetTargetProperty ()
    {
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT1Mixin1)), Is.Null);

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin1)), Is.Not.Null);
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin1)),
          Is.EqualTo (typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Target", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin2)), Is.Not.Null);
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin2)),
          Is.EqualTo (typeof (Mixin<IBaseType32>).GetProperty ("Target", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), Is.Not.Null);
      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), 
          Is.Not.EqualTo (typeof (Mixin<,>).GetProperty ("Target", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetTargetProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)),
          Is.EqualTo (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Target", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetBaseProperty ()
    {
      Assert.That (MixinReflector.GetNextProperty (typeof (BT1Mixin1)), Is.Null);

      Assert.That (MixinReflector.GetNextProperty (typeof (BT3Mixin1)), Is.Not.Null);
      Assert.That (
          MixinReflector.GetNextProperty (typeof (BT3Mixin1)),
          Is.EqualTo (
              typeof (Mixin<IBaseType31, IBaseType31>).GetProperty ("Next", BindingFlags.NonPublic | BindingFlags.Instance)));

      Assert.That (MixinReflector.GetNextProperty (typeof (BT3Mixin2)), Is.Null);

      Assert.That (MixinReflector.GetNextProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), Is.Not.Null);
      Assert.That (MixinReflector.GetNextProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)), 
          Is.Not.EqualTo (typeof (Mixin<,>).GetProperty ("Next", BindingFlags.NonPublic | BindingFlags.Instance)));
      Assert.That (MixinReflector.GetNextProperty (typeof (BT3Mixin3<BaseType3, IBaseType33>)),
          Is.EqualTo (typeof (Mixin<BaseType3, IBaseType33>).GetProperty ("Next", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    public void GetMixinNextCallProxyType ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      Type bcpt = MixinReflector.GetNextCallProxyType (bt1);
      Assert.That (bcpt, Is.Not.Null);
      Assert.That (bcpt, Is.EqualTo (bt1.GetType ().GetNestedType ("NextCallProxy")));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not a mixin target", MatchType = MessageMatch.Contains)]
    public void GetMixinNextCallProxyTypeThrowsIfWrongType1 ()
    {
      MixinReflector.GetNextCallProxyType (new object ());
    }

    [Test]
    public void GetOrderedMixinTypes_NullWhenNoMixedType ()
    {
      Assert.That (MixinReflector.GetOrderedMixinTypesFromConcreteType (typeof (object)), Is.Null);
    }

    [Test]
    public void GetOrderedMixinTypes_OrderedMixinTypes ()
    {
      var concreteMixedType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType7));

      Assert.That (
          MixinReflector.GetOrderedMixinTypesFromConcreteType (concreteMixedType),
          Is.EqualTo (BigTestDomainScenarioTest.ExpectedBaseType7OrderedMixinTypesSmall));
    }

    [Test]
    public void GetOrderedMixinTypes_OpenGenericMixinTypesAreClosed ()
    {
      var concreteMixedType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType3));

      Assert.That (MixinReflector.GetOrderedMixinTypesFromConcreteType (concreteMixedType), 
          Has.Member(typeof (BT3Mixin3<BaseType3, IBaseType33>)));
    }
  }
}
