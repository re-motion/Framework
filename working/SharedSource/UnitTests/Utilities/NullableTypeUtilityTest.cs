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
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class NullableTypeUtilityTest
  {
    [Test]
    public void IsNullableType_ValueType ()
    {
      Assert.That (NullableTypeUtility.IsNullableType (typeof (int)), Is.False);
      Assert.That (NullableTypeUtility.IsNullableType (typeof (DateTime)), Is.False);
    }

    [Test]
    public void IsNullableType_NullableValueType ()
    {
      Assert.That (NullableTypeUtility.IsNullableType (typeof (int?)), Is.True);
      Assert.That (NullableTypeUtility.IsNullableType (typeof (DateTime?)), Is.True);
    }

    [Test]
    public void IsNullableType_ReferenceType ()
    {
      Assert.That (NullableTypeUtility.IsNullableType (typeof (object)), Is.True);
      Assert.That (NullableTypeUtility.IsNullableType (typeof (string)), Is.True);
    }

    [Test]
    public void IsNullableType_WithNull_ThrowsArgumentNullException ()
    {
      Assert.That (
          () => NullableTypeUtility.IsNullableType (null),
          Throws.TypeOf<ArgumentNullException>().With.Message.EndsWith ("Parameter name: type"));
    }

    [Test]
    public void GetNullableType_ValueType ()
    {
      Assert.That (NullableTypeUtility.GetNullableType (typeof (int)), Is.EqualTo (typeof (int?)));
    }

    [Test]
    public void GetNullableType_NullableValueType ()
    {
      Assert.That (NullableTypeUtility.GetNullableType (typeof (int?)), Is.EqualTo (typeof (int?)));
    }

    [Test]
    public void GetNullableType_ReferenceType ()
    {
      Assert.That (NullableTypeUtility.GetNullableType (typeof (string)), Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetNullableType_WithNull_ThrowsArgumentNullException ()
    {
      Assert.That (
          () => NullableTypeUtility.GetNullableType (null),
          Throws.TypeOf<ArgumentNullException>().With.Message.EndsWith ("Parameter name: type"));
    }

    [Test]
    public void GetBasicType_ValueType ()
    {
      Assert.That (NullableTypeUtility.GetBasicType (typeof (int)), Is.EqualTo (typeof (int)));
    }

    [Test]
    public void GetBasicType_NullableValueType ()
    {
      Assert.That (NullableTypeUtility.GetBasicType (typeof (int?)), Is.EqualTo (typeof (int)));
    }

    [Test]
    public void GetBasicType_ReferenceType ()
    {
      Assert.That (NullableTypeUtility.GetBasicType (typeof (string)), Is.EqualTo (typeof (string)));
    }

    [Test]
    public void GetBasicType_WithNull_ThrowsArgumentNullException ()
    {
      Assert.That (
          () => NullableTypeUtility.GetBasicType (null),
          Throws.TypeOf<ArgumentNullException>().With.Message.EndsWith ("Parameter name: type"));
    }
  }
}