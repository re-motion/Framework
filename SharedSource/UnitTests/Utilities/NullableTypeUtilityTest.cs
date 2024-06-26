// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class NullableTypeUtilityTest
  {
    [Test]
    public void IsNullableType_ValueType ()
    {
      Assert.That(NullableTypeUtility.IsNullableType(typeof(int)), Is.False);
      Assert.That(NullableTypeUtility.IsNullableType(typeof(DateTime)), Is.False);
    }

    [Test]
    public void IsNullableType_NullableValueType ()
    {
      Assert.That(NullableTypeUtility.IsNullableType(typeof(int?)), Is.True);
      Assert.That(NullableTypeUtility.IsNullableType(typeof(DateTime?)), Is.True);
    }

    [Test]
    public void IsNullableType_ReferenceType ()
    {
      Assert.That(NullableTypeUtility.IsNullableType(typeof(object)), Is.True);
      Assert.That(NullableTypeUtility.IsNullableType(typeof(string)), Is.True);
    }

    [Test]
    public void IsNullableType_WithNull_ThrowsArgumentNullException ()
    {
      Assert.That(
          () => NullableTypeUtility.IsNullableType(null!),
          Throws.TypeOf<ArgumentNullException>().With.ArgumentExceptionMessageWithParameterNameEqualTo("type"));
    }

    [Test]
    public void GetNullableType_ValueType ()
    {
      Assert.That(NullableTypeUtility.GetNullableType(typeof(int)), Is.EqualTo(typeof(int?)));
    }

    [Test]
    public void GetNullableType_NullableValueType ()
    {
      Assert.That(NullableTypeUtility.GetNullableType(typeof(int?)), Is.EqualTo(typeof(int?)));
    }

    [Test]
    public void GetNullableType_ReferenceType ()
    {
      Assert.That(NullableTypeUtility.GetNullableType(typeof(string)), Is.EqualTo(typeof(string)));
    }

    [Test]
    public void GetNullableType_WithNull_ThrowsArgumentNullException ()
    {
      Assert.That(
          () => NullableTypeUtility.GetNullableType(null!),
          Throws.TypeOf<ArgumentNullException>().With.ArgumentExceptionMessageWithParameterNameEqualTo("type"));
    }

    [Test]
    public void GetBasicType_ValueType ()
    {
      Assert.That(NullableTypeUtility.GetBasicType(typeof(int)), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void GetBasicType_NullableValueType ()
    {
      Assert.That(NullableTypeUtility.GetBasicType(typeof(int?)), Is.EqualTo(typeof(int)));
    }

    [Test]
    public void GetBasicType_ReferenceType ()
    {
      Assert.That(NullableTypeUtility.GetBasicType(typeof(string)), Is.EqualTo(typeof(string)));
    }

    [Test]
    public void GetBasicType_WithNull_ThrowsArgumentNullException ()
    {
      Assert.That(
          () => NullableTypeUtility.GetBasicType(null!),
          Throws.TypeOf<ArgumentNullException>().With.ArgumentExceptionMessageWithParameterNameEqualTo("type"));
    }
  }
}
