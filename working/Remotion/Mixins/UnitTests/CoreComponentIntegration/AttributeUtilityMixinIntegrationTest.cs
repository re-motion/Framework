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
using Remotion.Mixins.UnitTests.CoreComponentIntegration.TestDomain;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.CoreComponentIntegration
{
  [TestFixture]
  public class AttributeUtilityMixinIntegrationTest
  {
    [Test]
    public void MixinAttribute_IsAlsoSuppressed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<DerivedWithAttributesAndSuppressed> ().AddMixin<MixinAddingInheritedAttribute> ().EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (DerivedWithAttributesAndSuppressed));
        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            Has.No.Member (new BaseInheritedAttribute ("MixinAddingInheritedAttribute")));
      }
    }

    [Test]
    public void MixinSuppressingAttribute ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<BaseWithAttributesForSuppressed> ().AddMixin<MixinAddingSuppressAttribute> ().EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (BaseWithAttributesForSuppressed));
        Assert.That (type.GetCustomAttributes (true), Has.Member (new BaseInheritedAttribute ("BaseWithAttributesForSuppressed")));
        Assert.That (type.GetCustomAttributes (true), Has.Member (new BaseInheritedAttribute ("MixinAddingSuppressAttribute")));

        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            Has.No.Member (new BaseInheritedAttribute ("BaseWithAttributesForSuppressed")));
        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            Has.Member (new BaseInheritedAttribute ("MixinAddingSuppressAttribute")));
      }
    }

    [Test]
    public void MixinSuppressingNonInheritedAttributeOnTargetClass ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassWithNonInheritedAttribute> ().AddMixin<MixinSuppressingNonInheritedAttribute> ().EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (ClassWithNonInheritedAttribute));

        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            Has.No.Member (new BaseNonInheritedAttribute ("ClassWithNonInheritedAttribute")));
      }
    }

    [Test]
    public void MixinSuppressingAttributeOnOtherMixin ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutAttributes> ().AddMixin<MixinAddingInheritedAttribute> ().AddMixin<MixinAddingSuppressAttribute> ()
          .EnterScope ())
      {
        Type type = TypeFactory.GetConcreteType (typeof (ClassWithoutAttributes));

        Assert.That (AttributeUtility.GetCustomAttributes (type, typeof (Attribute), true),
            Has.No.Member (new BaseInheritedAttribute ("MixinAddingInheritedAttribute")));
      }
    }

  }
}
