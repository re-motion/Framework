// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullOrEmpty
  {
    [Test]
    public void Fail_NullString ()
    {
      const string value = null;
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", value),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_EmptyString ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", ""),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyArray ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", new string[0]),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyCollection ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", new ArrayList()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyICollectionOfT ()
    {
      ICollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyListOfT ()
    {
      IReadOnlyCollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckNotNullOrEmpty("arg", "Test");
      Assert.That(result, Is.EqualTo("Test"));
    }

    [Test]
    public void Succeed_Array ()
    {
      var array = new[] { "test" };
      string[] result = ArgumentUtility.CheckNotNullOrEmpty("arg", array);
      Assert.That(result, Is.SameAs(array));
    }

    [Test]
    public void Succeed_Collection ()
    {
      var list = new ArrayList { "test" };
      ArrayList result = ArgumentUtility.CheckNotNullOrEmpty("arg", list);
      Assert.That(result, Is.SameAs(list));
    }

    [Test]
    public void Succeed_ICollectionOfT ()
    {
      ICollection<string> value = new List<string> { "test" };
      ArgumentUtility.CheckNotNullOrEmpty("arg", value);
    }

    [Test]
    public void Succeed_IReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<string> value = new List<string> { "test" };
      ArgumentUtility.CheckNotNullOrEmpty("arg", value);
    }

    [Test]
    public void Succeed_ListOfT ()
    {
      List<string> value = new List<string> { "test" };
      ArgumentUtility.CheckNotNullOrEmpty("arg", value);
    }
  }
}
