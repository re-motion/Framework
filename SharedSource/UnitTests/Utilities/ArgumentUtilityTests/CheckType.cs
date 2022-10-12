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
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckType
  {
    [Test]
    public void Fail_Type ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<string>("arg", 13),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }

    [Test]
    public void Fail_ValueType ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<int>("arg", (object?)null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Null ()
    {
      string result = ArgumentUtility.CheckType<string>("arg", null);
      Assert.That(result, Is.EqualTo(null));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckType<string>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Succeed_BaseType ()
    {
      string result = (string)ArgumentUtility.CheckType<object>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }
  }
}
