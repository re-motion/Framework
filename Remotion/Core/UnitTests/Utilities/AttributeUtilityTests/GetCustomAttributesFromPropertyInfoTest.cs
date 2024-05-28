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
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributesFromPropertyInfoTest : GetCustomAttributesFromMemberInfoTestBase
  {
    protected override MemberInfo BaseMemberWithSingleAttribute
    {
      get { return typeof(SampleClass).GetProperty("PropertyWithSingleAttribute"); }
    }

    protected override MemberInfo BaseMemberWithNonInheritedAttribute
    {
      get { return typeof(SampleClass).GetProperty("PropertyWithNotInheritedAttribute"); }
    }

    protected override MemberInfo DerivedMemberWithSingleAttribute
    {
      get { return typeof(DerivedSampleClass).GetProperty("PropertyWithSingleAttribute"); }
    }

    protected override MemberInfo DerivedMemberWithMultipleAttribute
    {
      get { return typeof(DerivedSampleClass).GetProperty("PropertyWithMultipleAttribute"); }
    }

    protected override MemberInfo DerivedProtectedMember
    {
      get
      {
        return typeof(DerivedSampleClass).GetProperty("ProtectedPropertyWithAttribute", BindingFlags.NonPublic | BindingFlags.Instance);
      }
    }

    protected override MemberInfo DerivedMemberNotInheritingAttribute
    {
      get { return typeof(DerivedSampleClass).GetProperty("PropertyWithNotInheritedAttribute"); }
    }

    protected override MemberInfo DerivedMemberHidingAttribute
    {
      get { return typeof(DerivedSampleClass).GetProperty("PropertyWithInheritedNotMultipleAttribute"); }
    }

    [Test]
    public void Test_FromOverrideWithBaseOverloads ()
    {
      var propertyInfo = typeof(DerivedSampleClass).GetProperty("Item", new[] { typeof(int) });
      object[] attributes = AttributeUtility.GetCustomAttributes(propertyInfo, typeof(InheritedAttribute), true);

      Assert.That(attributes.Length, Is.EqualTo(1));
      Assert.That(attributes[0], Is.Not.Null);
      Assert.That(attributes[0], Is.InstanceOf(typeof(InheritedAttribute)));
    }

    [Test]
    public void Test_NoGetter ()
    {
      var propertyInfo = typeof(DerivedSampleClass).GetProperty("PropertyWithoutGetter");
      object[] attributes = AttributeUtility.GetCustomAttributes(propertyInfo, typeof(InheritedAttribute), true);

      Assert.That(attributes.Length, Is.EqualTo(1));
      Assert.That(attributes[0], Is.Not.Null);
      Assert.That(attributes[0], Is.InstanceOf(typeof(InheritedAttribute)));
    }
  }
}
