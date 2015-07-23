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
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GenericsTest : CodeGenerationBaseTest
  {
    [Test]
    public void GenericMixinsAreSpecialized ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>));
      object mixin = Mixin.Get (typeof (BT3Mixin3<,>), bt3);
      Assert.That (mixin, Is.Not.Null);

      PropertyInfo targetProperty = MixinReflector.GetTargetProperty (mixin.GetType ());
      Assert.That (targetProperty, Is.Not.Null);

      Assert.That (targetProperty.GetValue (mixin, null), Is.Not.Null);
      Assert.That (targetProperty.GetValue (mixin, null), Is.SameAs (bt3));
      Assert.That (targetProperty.PropertyType, Is.EqualTo (typeof (BaseType3)));

      PropertyInfo nextProperty = MixinReflector.GetNextProperty (mixin.GetType ());
      Assert.That (nextProperty, Is.Not.Null);

      Assert.That (nextProperty.GetValue (mixin, null), Is.Not.Null);
      Assert.That (nextProperty.GetValue (mixin, null).GetType (), Is.SameAs (bt3.GetType ().GetField ("__first", BindingFlags.NonPublic | BindingFlags.Instance).FieldType));
      Assert.That (nextProperty.PropertyType, Is.EqualTo (typeof (IBaseType33)));
    }

    [Test]
    public void MuchGenericityWithoutOverriding ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (VeryGenericMixin<,>), typeof (BT3Mixin4));
      var m = bt3 as IVeryGenericMixin;
      Assert.That (m, Is.Not.Null);
      Assert.That (m.GetMessage ("5"), Is.EqualTo ("IVeryGenericMixin.GenericIfcMethod-5"));
    }

    [Test]
    public void MuchGenericityWithOverriding ()
    {
      var cougs = CreateMixedObject<ClassOverridingUltraGenericStuff> (typeof (AbstractDerivedUltraGenericMixin<,>), typeof (BT3Mixin4));
      var m = cougs as IUltraGenericMixin;
      Assert.That (m, Is.Not.Null);
      Assert.That (m.GetMessage ("5"), Is.EqualTo ("String-IVeryGenericMixin.GenericIfcMethod-5"));
    }

  }
}
