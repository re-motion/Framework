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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.MemberFiltering;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class OverriddenMemberFilterTest
  {
    private OverriddenMemberFilter _filter;
    private MethodInfo _overriddenMethod;
    private MethodInfo _overridingMethod1;
    private MethodInfo _overridingMethod2;
    private PropertyInfo _overriddenProperty;
    private PropertyInfo _overridingPropertyWithGetAndSet;
    private PropertyInfo _overridingPropertyWithGetOnly;
    private PropertyInfo _overridingPropertyWithSetOnly;
    private PropertyInfo _overridingPropertyWithGetAndSetAfterSingleOverrides;

    [SetUp]
    public void SetUp ()
    {
      _filter = new OverriddenMemberFilter ();
      _overriddenMethod = typeof (DerivedWithNewVirtualMembers).GetMethod ("Method");
      _overridingMethod1 = typeof (DerivedDerivedWithOverrides).GetMethod ("Method");
      _overridingMethod2 = typeof (DerivedDerivedDerivedWithOverrides).GetMethod ("Method");
      _overriddenProperty = typeof (DerivedWithNewVirtualMembers).GetProperty ("Property");
      _overridingPropertyWithGetAndSet = typeof (DerivedDerivedWithOverrides).GetProperty ("Property");
      _overridingPropertyWithGetOnly = typeof (DerivedDerivedWithGetOnlyOverride).GetProperty ("Property");
      _overridingPropertyWithSetOnly = typeof (DerivedDerivedDerivedWithSetOnlyOverride).GetProperty ("Property");
      _overridingPropertyWithGetAndSetAfterSingleOverrides = typeof (DerivedDerivedDerivedDerivedWithGetSetOverride).GetProperty ("Property");
    }

    [Test]
    public void RemoveOverriddenMembers_NonOverriddenMembers ()
    {
      var members = new MemberInfo[] { _overriddenMethod, _overriddenProperty };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (members));
    }

    [Test]
    public void RemoveOverriddenMembers_OverriddenMembers ()
    {
      var members = new[] { _overriddenMethod, _overridingMethod1 };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (new[] { _overridingMethod1 }));
    }

    [Test]
    public void RemoveOverriddenMembers_TwoOverridingMembers ()
    {
      var members = new[] { _overriddenMethod, _overridingMethod1, _overridingMethod2 };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (new[] { _overridingMethod2 }));
    }

    [Test]
    public void RemoveOverriddenMembers_AllAccessorsOverridden ()
    {
      var members = new[] { _overriddenProperty, _overridingPropertyWithGetAndSet };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (new[] { _overridingPropertyWithGetAndSet }));
    }

    [Test]
    public void RemoveOverriddenMembers_OnlyOneAccessorOverridden ()
    {
      var members = new[] { _overriddenProperty, _overridingPropertyWithGetOnly };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (new[] { _overriddenProperty, _overridingPropertyWithGetOnly }));
    }

    [Test]
    public void RemoveOverriddenMembers_OnlyOneAccessorOverridden_InTwoClasses ()
    {
      var members = new[] { _overriddenProperty, _overridingPropertyWithGetOnly, _overridingPropertyWithSetOnly };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (new[] { _overridingPropertyWithGetOnly, _overridingPropertyWithSetOnly }));
    }

    [Test]
    public void RemoveOverriddenMembers_AllAccessorsOverridden_AfterOnlyOneAccessorOverridden ()
    {
      var members = new[] { _overriddenProperty, _overridingPropertyWithGetOnly, _overridingPropertyWithSetOnly, _overridingPropertyWithGetAndSetAfterSingleOverrides };
      var result = _filter.RemoveOverriddenMembers (members);

      Assert.That (result, Is.EquivalentTo (new[] { _overridingPropertyWithGetAndSetAfterSingleOverrides }));
    }

    [Test]
    public void RemoveOverriddenMembers_AllAccessorsOverridden_AfterOnlyOneAccessorOverridden_OrderAgnostic ()
    {
      var members = new[] { _overriddenProperty, _overridingPropertyWithGetOnly, _overridingPropertyWithSetOnly, _overridingPropertyWithGetAndSetAfterSingleOverrides };
      var result = _filter.RemoveOverriddenMembers (members.Reverse());

      Assert.That (result, Is.EquivalentTo (new[] { _overridingPropertyWithGetAndSetAfterSingleOverrides }));
    }
  }
}
