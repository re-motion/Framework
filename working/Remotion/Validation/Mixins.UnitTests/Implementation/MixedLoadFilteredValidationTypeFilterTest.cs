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
using Remotion.Mixins;
using Remotion.TypePipe.Implementation;
using Remotion.Validation.Mixins.Implementation;
using Remotion.Validation.Mixins.UnitTests.Implementation.TestDomain;

namespace Remotion.Validation.Mixins.UnitTests.Implementation
{
  [TestFixture]
  public class MixedLoadFilteredValidationTypeFilterTest
  {
    private MixedLoadFilteredValidationTypeFilter _filter;

    [SetUp]
    public void SetUp ()
    {
      _filter = new MixedLoadFilteredValidationTypeFilter();
    }

    [Test]
    public void IsValid_FilteredTypes_ReturnFalse ()
    {
      Assert.That (_filter.IsValidatableType (typeof (IMixinTarget)), Is.False);
      Assert.That (_filter.IsValidatableType (typeof (IInitializableMixin)), Is.False);
      Assert.That (_filter.IsValidatableType (typeof (IInitializableObject)), Is.False);
    }

    [Test]
    public void IsValid_NoneFilteredType_ReturnTrue ()
    {
      Assert.That (_filter.IsValidatableType (typeof (string)), Is.True);
      Assert.That (_filter.IsValidatableType (MixinTypeUtility.GetConcreteMixedType (typeof (BaseConcreteTypeForMixin))), Is.True);
      Assert.That (_filter.IsValidatableType (typeof (BaseConcreteTypeForMixin)), Is.True);
    }
  }
}