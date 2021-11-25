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