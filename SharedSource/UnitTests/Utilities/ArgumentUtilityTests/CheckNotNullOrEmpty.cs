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
  public class CheckNotNullOrEmpty
  {
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullString ()
    {
      const string value = null;
      ArgumentUtility.CheckNotNullOrEmpty ("arg", value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyString ()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", "");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyArray ()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", new string[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", new ArrayList());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyIEnumerable ()
    {
      ArgumentUtility.CheckNotNullOrEmpty ("arg", GetEmptyEnumerable());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (false);
      ArgumentUtility.CheckNotNullOrEmpty ("arg", enumerable);
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckNotNullOrEmpty ("arg", "Test");
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void Succeed_Array ()
    {
      var array = new[] { "test" };
      string[] result = ArgumentUtility.CheckNotNullOrEmpty ("arg", array);
      Assert.That (result, Is.SameAs (array));
    }

    [Test]
    public void Succeed_Collection ()
    {
      var list = new ArrayList { "test" };
      ArrayList result = ArgumentUtility.CheckNotNullOrEmpty ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

    [Test]
    public void Succeed_IEnumerable ()
    {
      IEnumerable enumerable = GetEnumerableWithValue();
      IEnumerable result = ArgumentUtility.CheckNotNullOrEmpty ("arg", enumerable);
      Assert.That (result, Is.SameAs (enumerable));
      Assert.That (result.GetEnumerator().MoveNext(), Is.True);
    }

    [Test]
    public void Succeed_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (true);
      IEnumerable result = ArgumentUtility.CheckNotNullOrEmpty ("arg", enumerable);
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