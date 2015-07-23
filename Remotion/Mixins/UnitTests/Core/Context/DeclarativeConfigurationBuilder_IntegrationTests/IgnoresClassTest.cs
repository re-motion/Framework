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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class IgnoresClassTest
  {
    [Test]
    public void IgnoredClass_IsExcluded ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (DerivedClassIgnoredByMixins), typeof (MixinIgnoringDerivedClass)), Is.False);
    }

    [Test]
    public void BaseClass_IsNotExcluded ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (BaseClassForDerivedClassIgnoredByMixin), typeof (MixinIgnoringDerivedClass)), Is.True);
    }

    [Test]
    public void DerivedClass_IsExcluded ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (DerivedDerivedClassIgnoredByMixin), typeof (MixinIgnoringDerivedClass)), Is.False);
    }

    // The following test does not work because GenericClassForMixinIgnoringDerivedClass<int> inherits the mixin from its base class
    // (BaseClassForDerivedClassIgnoredByMixin) even though its generic type definition (GCFMIDC<>) ignores the mixin.
    // At the moment, this is by design due to the rule that a closed generic type inherits mixins from both its base class and its generic
    // type definition.
    //[Test]
    //public void GenericSpecialization_IsExcluded ()
    //{
    //  Assert.IsFalse (MixinTypeUtility.HasMixin (typeof (GenericClassForMixinIgnoringDerivedClass<int>), typeof (MixinIgnoringDerivedClass)));
    //}

    [Test]
    public void ClosedGenericSpecialization_IsExcluded ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<int>), typeof (MixinIgnoringDerivedClass)), Is.False);
    }

    [Test]
    public void ClosedGenericSpecializationVariant_IsNotExcluded ()
    {
      Assert.That (MixinTypeUtility.HasMixin (typeof (ClosedGenericClassForMixinIgnoringDerivedClass<string>), typeof (MixinIgnoringDerivedClass)), Is.False);
    }
  }
}
