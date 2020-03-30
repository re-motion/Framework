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
using NUnit.Framework;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullOrEmptyOrItemsNull
  {
    [Test]
    public void Fail_NullICollection ()
    {
      Assert.That (
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", (ICollection) null),
          Throws.InstanceOf<ArgumentNullException>()
              .With.Message.EqualTo (
                  "Value cannot be null.\r\nParameter name: arg"));
    }

    [Test]
    public void Fail_zItemNullICollection ()
    {
      ArrayList list = new ArrayList();
      list.Add (null);
      Assert.That (
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", list),
          Throws.InstanceOf<ArgumentNullException>()
              .With.Message.EqualTo (
                  "Item 0 of parameter 'arg' is null.\r\nParameter name: arg"));
    }

    [Test]
    public void Fail_EmptyArray ()
    {
      Assert.That (
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", new string[0]),
          Throws.ArgumentException
              .With.Message.EqualTo (
                  "Parameter 'arg' cannot be empty.\r\nParameter name: arg"));
    }

    [Test]
    public void Fail_EmptyCollection ()
    {
      Assert.That (
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", new ArrayList()),
          Throws.ArgumentException
              .With.Message.EqualTo (
                  "Parameter 'arg' cannot be empty.\r\nParameter name: arg"));
    }

    [Test]
    public void Fail_EmptyIEnumerable ()
    {
      Assert.That (
          () => ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", GetEmptyEnumerable()),
          Throws.ArgumentException
              .With.Message.EqualTo (
                  "Parameter 'arg' cannot be empty.\r\nParameter name: arg"));
    }

    [Test]
    public void Succeed_Array ()
    {
      string[] array = new string[] { "test" };
      string[] result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", array);
      Assert.That (result, Is.SameAs (array));
    }

    [Test]
    public void Succeed_Collection ()
    {
      ArrayList list = new ArrayList();
      list.Add ("test");
      ArrayList result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

    [Test]
    public void Succeed_IEnumerable ()
    {
      IEnumerable enumerable = GetEnumerableWithValue();
      IEnumerable result = ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("arg", enumerable);
      Assert.That (result, Is.SameAs (enumerable));
      Assert.That (result.GetEnumerator().MoveNext(), Is.True);
    }

    private IEnumerable GetEnumerableWithValue ()
    {
      yield return "test";
    }

    private IEnumerable GetEmptyEnumerable ()
    {
      yield break;
    }
  }
}