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
#if !DEBUG
  [Ignore("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckNotNullOrEmpty
  {
    [Test]
    public void Fail_NullString ()
    {
      const string value = null;
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_EmptyString ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", ""),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyArray ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", new string[0]),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyCollection ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", new ArrayList()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyICollectionOfT ()
    {
      ICollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyListOfT ()
    {
      IReadOnlyCollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Succeed_String ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty("arg", "Test");
    }

    [Test]
    public void Succeed_Array ()
    {
      var array = new[] { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty("arg", array);
    }

    [Test]
    public void Succeed_Collection ()
    {
      var list = new ArrayList { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty("arg", list);
    }

    [Test]
    public void Succeed_ICollectionOfT ()
    {
      ICollection<string> value = new List<string> { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value);
    }

    [Test]
    public void Succeed_IReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<string> value = new List<string> { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value);
    }

    [Test]
    public void Succeed_ListOfT ()
    {
      List<string> value = new List<string> { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty("arg", value);
    }
  }
}
