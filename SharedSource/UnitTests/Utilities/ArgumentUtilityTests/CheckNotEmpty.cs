// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotEmpty
  {
    [Test]
    public void Succeed_NonEmptyString ()
    {
      var result = ArgumentUtility.CheckNotEmpty("arg", "x");
      Assert.That(result, Is.EqualTo("x"));
    }

    [Test]
    public void Succeed_NullString ()
    {
      const string? s = null;
      var result = ArgumentUtility.CheckNotEmpty("arg", s);
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Fail_EmptyString ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", ""),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Succeed_NonEmptyCollection ()
    {
      ICollection value = new[] { 1 };

      var result = ArgumentUtility.CheckNotEmpty("arg", value);

      Assert.That(result, Is.SameAs(value));
    }

    [Test]
    public void Succeed_NonEmptyICollectionOfT ()
    {
      ICollection<object> value = new List<object> { 1 };

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NonEmptyIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> value = new List<object> { 1 };

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NonEmptyArray ()
    {
      object[] value = new object[] { 1 };

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NonEmptyListOfT ()
    {
      List<object> value = new List<object> { 1 };

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NullCollection ()
    {
      var result = ArgumentUtility.CheckNotEmpty("arg", (ICollection?)null);
      Assert.That(result, Is.Null);
    }

    [Test]
    public void Succeed_NullICollectionOfT ()
    {
      ICollection<object>? value = null;

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NullIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object>? value = null;

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NullArray ()
    {
      object[]? value = null;

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Succeed_NullListOfT ()
    {
      List<object>? value = null;

      ArgumentUtility.CheckNotEmpty("arg", value);
    }

    [Test]
    public void Fail_EmptyICollection ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", Type.EmptyTypes),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyICollectionOfT ()
    {
      ICollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyArray ()
    {
      object[] value = new object[0];

      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyListOfT ()
    {
      List<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Succeed_NonEmptyGuid ()
    {
      Guid guid = Guid.NewGuid();
      var result = ArgumentUtility.CheckNotEmpty("arg", guid);
      Assert.That(result, Is.EqualTo(guid));
    }

    [Test]
    public void Fail_EmptyGuid ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotEmpty("arg", Guid.Empty),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }
  }
}
