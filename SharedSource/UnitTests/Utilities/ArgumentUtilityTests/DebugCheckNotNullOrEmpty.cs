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
#if !DEBUG
  [Ignore ("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckNotNullOrEmpty
  {
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullString ()
    {
      const string value = null;
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", value);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyString ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", "");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyArray ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", new string[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", new ArrayList());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyIEnumerable ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", GetEmptyEnumerable());
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (false);
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", enumerable);
    }

    [Test]
    public void Succeed_String ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", "Test");
    }

    [Test]
    public void Succeed_Array ()
    {
      var array = new[] { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", array);
    }

    [Test]
    public void Succeed_Collection ()
    {
      var list = new ArrayList { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", list);
    }

    [Test]
    public void Succeed_IEnumerable ()
    {
      IEnumerable enumerable = GetEnumerableWithValue();
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", enumerable);
    }

    [Test]
    public void Succeed_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (true);
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", enumerable);
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