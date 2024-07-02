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
  public class CheckNotNullOrItemsNull
  {
    [Test]
    public void Succeed_ICollection ()
    {
      ArrayList list = new ArrayList();
      ArrayList result = ArgumentUtility.CheckNotNullOrItemsNull("arg", list);
      Assert.That(result, Is.SameAs(list));
    }

    [Test]
    public void Succeed_ICollectionOfT ()
    {
      ICollection<string> value = new List<string> { "test" };

      ArgumentUtility.CheckNotNullOrItemsNull("arg", value);
    }

    [Test]
    public void Succeed_IReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<string> value = new List<string> { "test" };

      ArgumentUtility.CheckNotNullOrItemsNull("arg", value);
    }

    [Test]
    public void Succeed_ListOfT ()
    {
      List<string> value = new List<string> { "test" };

      ArgumentUtility.CheckNotNullOrItemsNull("arg", value);
    }

    [Test]
    public void Fail_NullICollection ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", (ICollection)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_NullICollectionOfT ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", (ICollection<object>)null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "arg"));
    }

    [Test]
    public void Fail_NullIReadOnlyCollectionOfT ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", (IReadOnlyCollection<object>)null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "arg"));
    }

    [Test]
    public void Fail_NullListOfT ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", (List<object>)null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullICollection ()
    {
      ArrayList list = new ArrayList();
      list.Add(null);
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullICollectionOfT ()
    {
      ICollection<object> list = new List<object> { null };

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> list = new List<object> { null };

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullListOfT ()
    {
      List<object> list = new List<object> { null };

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }
  }
}
