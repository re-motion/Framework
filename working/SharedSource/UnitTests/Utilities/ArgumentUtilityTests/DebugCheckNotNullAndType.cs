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
#if !DEBUG
  [Ignore ("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckNotNullAndType
  {
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]

    [Test]
    public void Succeed_Int ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", 1, typeof (int));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Int_Null ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", null, typeof (int));
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", (int?) 1, typeof (int));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", (int?) 1, typeof (int?));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullableInt_Null ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", null, typeof (int?));
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", 1, typeof (int?));
    }

    [Test]
    public void Succeed_String ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", "test", typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_StringNull ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", null, typeof (string));
    }

    private enum TestEnum
    {
      TestValue
    }

    [Test]
    public void Succeed_Enum ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", TestEnum.TestValue, typeof (TestEnum));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_Enum_Null ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", null, typeof (TestEnum));
    }

    [Test]
    public void Succeed_Object_String ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", "test", typeof (object));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.\r\nParameter name: arg")]
    public void Fail_String_Int ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", 1, typeof (string));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.Int32' when type 'System.Int64' was expected.\r\nParameter name: arg")]
    public void Fail_Long_Int ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", 1, typeof (long));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'arg' has type 'System.String' when type 'System.Int32' was expected.\r\nParameter name: arg")]
    public void Fail_Int_String ()
    {
      ArgumentUtility.DebugCheckNotNullAndType ("arg", "test", typeof (int));
    }
  }
}