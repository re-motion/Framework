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
using NUnit.Framework;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckType3
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.\r\nParameter name: arg")]
    public void Fail_Type ()
    {
      ArgumentUtility.CheckType ("arg", 13, typeof (string));
    }

    [Test]
    public void Succeed_ValueType ()
    {
      Assert.That (ArgumentUtility.CheckType ("arg", (object) 1, typeof (int)), Is.EqualTo (1));
    }

    [Test]
    public void Succeed_NullableValueTypeNull ()
    {
      Assert.That (ArgumentUtility.CheckType ("arg", (object) null, typeof (int?)), Is.EqualTo (null));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type '<null>' when type 'System.Int32' was expected.\r\nParameter name: arg")]
    public void Fail_ValueTypeNull ()
    {
      ArgumentUtility.CheckType ("arg", (object) null, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.DateTime' when type 'System.Int32' was expected.\r\nParameter name: arg")]
    public void Fail_ValueType ()
    {
      ArgumentUtility.CheckType ("arg", (object) DateTime.MinValue, typeof (int));
    }

    [Test]
    public void Succeed_ReferenceTypeNull ()
    {
      Assert.That (ArgumentUtility.CheckType ("arg", (object) null, typeof (string)), Is.EqualTo (null));
    }

    [Test]
    public void Succeed_NotNull ()
    {
      Assert.That (ArgumentUtility.CheckType ("arg", "test", typeof (string)), Is.EqualTo ("test"));
    }
  }
}