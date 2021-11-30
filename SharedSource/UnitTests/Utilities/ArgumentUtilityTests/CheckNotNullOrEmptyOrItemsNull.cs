// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullOrEmptyOrItemsNull
  {
    [Test]
    public void Fail_NullICollection ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", (ICollection)null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "arg"));
    }

    [Test]
    public void Fail_NullICollectionOfT ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", (ICollection<object>)null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "arg"));
    }

    [Test]
    public void Fail_NullIReadOnlyCollectionOfT ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", (IReadOnlyCollection<object>)null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Value cannot be null.", "arg"));
    }

    [Test]
    public void Fail_NullListOfT ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", (List<object>)null),
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
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullICollectionOfT ()
    {
      ICollection<object> list = new List<object> { null };

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> list = new List<object> { null };

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_ItemNullListOfT ()
    {
      List<object> list = new List<object> { null };

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 0 of parameter 'arg' is null.", "arg"));
    }

    [Test]
    public void Fail_EmptyArray ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", new string[0]),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyCollection ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", new ArrayList()),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyICollectionOfT ()
    {
      ICollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyIReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Fail_EmptyListOfT ()
    {
      List<object> value = new List<object>();

      Assert.That(
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", value),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'arg' cannot be empty.", "arg"));
    }

    [Test]
    public void Succeed_Array ()
    {
      string[] array = new string[] { "test" };
      string[] result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", array);
      Assert.That(result, Is.SameAs(array));
    }

    [Test]
    public void Succeed_Collection ()
    {
      ArrayList list = new ArrayList();
      list.Add("test");
      ArrayList result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", list);
      Assert.That(result, Is.SameAs(list));
    }

    [Test]
    public void Succeed_ICollectionOfT ()
    {
      ICollection<string> value = new List<string> { "test" };

      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", value);
    }

    [Test]
    public void Succeed_IReadOnlyCollectionOfT ()
    {
      IReadOnlyCollection<string> value = new List<string> { "test" };

      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", value);
    }

    [Test]
    public void Succeed_ListOfT ()
    {
      List<string> value = new List<string> { "test" };

      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull("arg", value);
    }
  }
}
