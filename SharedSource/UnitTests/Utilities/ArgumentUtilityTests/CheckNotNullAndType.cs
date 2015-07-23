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
  public class CheckNotNullAndType
  {
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]
    [Test]
    public void Succeed_Int ()
    {
      int result = ArgumentUtility.CheckNotNullAndType<int> ("arg", 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Int_Null ()
    {
      ArgumentUtility.CheckNotNullAndType<int> ("arg", null);
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      int result = ArgumentUtility.CheckNotNullAndType<int> ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckNotNullAndType<int?> ("arg", (int?) 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullableInt_Null ()
    {
      ArgumentUtility.CheckNotNullAndType<int?> ("arg", null);
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckNotNullAndType<int?> ("arg", 1);
      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckNotNullAndType<string> ("arg", "test");
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_StringNull ()
    {
      ArgumentUtility.CheckNotNullAndType<string> ("arg", null);
    }

    private enum TestEnum
    {
      TestValue
    }

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckNotNullAndType<TestEnum> ("arg", TestEnum.TestValue);
      Assert.That (result, Is.EqualTo (TestEnum.TestValue));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Enum_Null ()
    {
      ArgumentUtility.CheckNotNullAndType<TestEnum> ("arg", null);
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckNotNullAndType<object> ("arg", "test");
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.\r\nParameter name: arg")]
    public void Fail_String_Int ()
    {
      ArgumentUtility.CheckNotNullAndType<string> ("arg", 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.Int64' was expected.\r\nParameter name: arg")]
    public void Fail_Long_Int ()
    {
      ArgumentUtility.CheckNotNullAndType<long> ("arg", 1);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.String' when type 'System.Int32' was expected.\r\nParameter name: arg")]
    public void Fail_Int_String ()
    {
      ArgumentUtility.CheckNotNullAndType<int> ("arg", "test");
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Null_String_NonGeneric ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", (object) null, typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.\r\nParameter name: arg")]
    public void Fail_Type_String_NonGeneric ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", 13, typeof (string));
    }


    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Null_Int_NonGeneric ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", (object) null, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Double' when type 'System.Int32' was expected.\r\nParameter name: arg")]
    public void Fail_Type_Int_NonGeneric ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", 13.0, typeof (int));
    }

    [Test]
    public void Succeed_Int_NonGeneric ()
    {
      ArgumentUtility.CheckNotNullAndType ("arg", 10, typeof (int));
    }
  }
}