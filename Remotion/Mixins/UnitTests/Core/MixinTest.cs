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
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class MixinTest
  {
    [Test]
    public void MixinGet_FindsMixinInstanceInTarget ()
    {
      var bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      var mixin = Mixin.Get<BT3Mixin2> (bt3);
      Assert.That (mixin, Is.Not.Null);
    }

    [Test]
    public void MixinGet_ReturnsNullIfMixinNotFound ()
    {
      var mixin = Mixin.Get<BT3Mixin2> (new object());
      Assert.That (mixin, Is.Null);
    }

    [Test]
    public void MixinGet_FindsMixinWithAssignableMatch ()
    {
      var bt1 = ObjectFactory.Create<BaseType1> (ParamList.Empty);
      var mixin = Mixin.Get<IBT1Mixin1> (bt1);
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin, Is.InstanceOf (typeof (BT1Mixin1)));
    }

    [Test]
    public void MixinGet_FindsMixinWithGenericMatch ()
    {
      BaseType3 bt3 = ObjectFactory.Create<BaseType3> (ParamList.Empty);
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), bt3);
      Assert.That (mixin, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Both mixins 'Remotion.Mixins.UnitTests.Core.TestDomain."
        + "DerivedDerivedNullMixin' and 'Remotion.Mixins.UnitTests.Core.TestDomain.DerivedNullMixin' match the given type 'NullMixin'.")]
    public void MixinGet_AssignableMatchAmbiguity ()
    {
      using (MixinConfiguration.BuildNew().ForClass<NullTarget>().AddMixin<DerivedNullMixin>().AddMixin<DerivedDerivedNullMixin>().EnterScope())
      {
        var instance = ObjectFactory.Create<NullTarget> (ParamList.Empty);
        Mixin.Get<NullMixin> (instance);
      }
    }
  }
}
