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
  public class CheckNotEmpty
  {
    [Test]
    public void Succeed_NonEmptyString ()
    {
      var result = ArgumentUtility.CheckNotEmpty ("arg", "x");
      Assert.That (result, Is.EqualTo ("x"));
    }

    [Test]
    public void Succeed_NullString ()
    {
      const string s = null;
      var result = ArgumentUtility.CheckNotEmpty ("arg", s);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyString ()
    {
      ArgumentUtility.CheckNotEmpty ("arg", "");
    }

    [Test]
    public void Succeed_NonEmptyCollection ()
    {
      var result = ArgumentUtility.CheckNotEmpty ("arg", new[] { 1 });
      Assert.That (result, Is.EqualTo (new[] { 1 }));
    }

    [Test]
    public void Succeed_NullCollection ()
    {
      var result = ArgumentUtility.CheckNotEmpty ("arg", (IEnumerable) null);
      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.CheckNotEmpty ("arg", Type.EmptyTypes);
    }

    [Test]
    public void Succeed_NonEmptyGuid ()
    {
      Guid guid = Guid.NewGuid();
      var result = ArgumentUtility.CheckNotEmpty ("arg", guid);
      Assert.That (result, Is.EqualTo (guid));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyGuid ()
    {
      ArgumentUtility.CheckNotEmpty ("arg", Guid.Empty);
    }
  }
}