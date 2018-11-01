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
  public class CheckType2
  {
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]

    [Test]
    public void Succeed_Int ()
    {
      int result = ArgumentUtility.CheckType<int> ("arg", 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Int_Null ()
    {
      ArgumentUtility.CheckType<int> ("arg", null);
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      int result = ArgumentUtility.CheckType<int> ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_NullableInt_Null ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", null);
      Assert.That (result, Is.EqualTo (null));
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckType<int?> ("arg", 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckType<string> ("arg", "test");
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void Succeed_StringNull ()
    {
      string result = ArgumentUtility.CheckType<string> ("arg", null);
      Assert.That (result, Is.EqualTo (null));
    }

    private enum TestEnum
    {
      TestValue
    }

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckType<TestEnum> ("arg", TestEnum.TestValue);
      Assert.That (result, Is.EqualTo (TestEnum.TestValue));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Enum_Null ()
    {
      ArgumentUtility.CheckType<TestEnum> ("arg", null);
    }

    [Test]
    public void Succeed_NullableEnum_Null ()
    {
      TestEnum? result = ArgumentUtility.CheckType<TestEnum?> ("arg", null);
      Assert.That (result, Is.EqualTo (null));
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckType<object> ("arg", "test");
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.\r\nParameter name: arg")]
    public void Fail_String_Int ()
    {
      ArgumentUtility.CheckType<string> ("arg", 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.Int64' was expected.\r\nParameter name: arg")]
    public void Fail_Long_Int ()
    {
      ArgumentUtility.CheckType<long> ("arg", 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.String' when type 'System.Int32' was expected.\r\nParameter name: arg")]
    public void Fail_Int_String ()
    {
      ArgumentUtility.CheckType<int> ("arg", "test");
    }
  }
}