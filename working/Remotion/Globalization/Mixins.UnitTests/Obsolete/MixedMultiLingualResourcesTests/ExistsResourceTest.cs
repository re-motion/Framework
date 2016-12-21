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
using Remotion.Globalization.Mixins.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;

namespace Remotion.Globalization.Mixins.UnitTests.Obsolete.MixedMultiLingualResourcesTests
{
  [Obsolete]
  [TestFixture]
  public class ExistsResourceTest
  {
    [Test]
    public void NoAttribute ()
    {
      Assert.That (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)), Is.False);
    }

    [Test]
    public void AttributesOnClass ()
    {
      Assert.That (MixedMultiLingualResources.ExistsResource (typeof (ClassWithMultiLingualResourcesAttributes)), Is.True);
    }

    [Test]
    public void AttributesOnBase ()
    {
      Assert.That (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithoutMultiLingualResourcesAttributes)), Is.True);
    }

    [Test]
    public void AttributesOnBaseAndClass ()
    {
      Assert.That (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithMultiLingualResourcesAttributes)), Is.True);
    }

    [Test]
    public void AttributesFromMixin_InheritedFalse ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
      {
        Assert.That (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)), Is.True);
      }
    }

    [Test]
    public void AttributesFromMixinsAndBaseAndClass ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        Assert.That (MixedMultiLingualResources.ExistsResource (typeof (InheritedClassWithMultiLingualResourcesAttributes)), Is.True);
      }
    }

    [Test]
    public void AttributesFromMixins ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithoutMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        Assert.That (MixedMultiLingualResources.ExistsResource (typeof (ClassWithoutMultiLingualResourcesAttributes)), Is.True);
      }
    }
  }
}
