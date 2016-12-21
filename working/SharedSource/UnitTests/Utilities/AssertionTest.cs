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
  public class AssertionTest
  {
    [Test]
    public void TestIsTrueHolds ()
    {
      Assertion.IsTrue (true);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Assertion failed: Expression evaluates to false.")]
    public void TestIsTrueFails ()
    {
      Assertion.IsTrue (false);
    }

    [Test]
    public void TestIsFalseHolds ()
    {
      Assertion.IsFalse (false);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Assertion failed: Expression evaluates to true.")]
    public void TestIsFalseFails ()
    {
      Assertion.IsFalse (true);
    }

    [Test]
    public void IsNull_True ()
    {
      Assertion.IsNull (null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Assertion failed: Expression does not evaluate to a null reference.")]
    public void IsNull_False ()
    {
      Assertion.IsNull ("x");
    }

    [Test]
    public void INull_ValueType_True ()
    {
      int? value = null;
      Assertion.IsNull (value);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Assertion failed: Expression does not evaluate to a null reference.")]
    public void IsNull_ValueType_False ()
    {
      int? value = 5;
      Assertion.IsNull (value);
    }

    [Test]
    public void IsNull_Message_True ()
    {
      Assertion.IsNull (null, "a");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "a")]
    public void IsNull_Message_False ()
    {
      Assertion.IsNull ("x", "a");
    }

    [Test]
    public void IsNull_Message_Args_True ()
    {     
      Assertion.IsNull (null, "a{0}b", 5);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "a5b")]
    public void IsNull_Message_Args_False ()
    {
      Assertion.IsNull ("x", "a{0}b", 5);
    }

    [Test]
    public void IsNotNull_True ()
    {
      var instance = "x";

      var result = Assertion.IsNotNull (instance);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Assertion failed: Expression evaluates to a null reference.")]
    public void IsNotNull_False ()
    {
      Assertion.IsNotNull<object> (null);
    }

    [Test]
    public void IsNotNull_ValueType_True ()
    {
      int? value = 5;
      var result = Assertion.IsNotNull (value);
      Assert.That (result, Is.EqualTo (value));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Assertion failed: Expression evaluates to a null reference.")]
    public void IsNotNull_ValueType_False ()
    {
      int? value = null;
      Assertion.IsNotNull (value);
    }

    [Test]
    public void IsNotNull_Message_True ()
    {
      var instance = "x";

      var result = Assertion.IsNotNull (instance, "a");

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "a")]
    public void IsNotNull_Message_False ()
    {
      Assertion.IsNotNull<object> (null, "a");
    }

#if !DEBUG
  [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    public void DebugIsNotNull_Message_True ()
    {
      var instance = "x";

      Assert.That (()=>Assertion.DebugIsNotNull (instance, "a"), Throws.Nothing);
    }

#if !DEBUG
  [Ignore ("Skipped unless DEBUG build")]
#endif
    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "a")]
    public void DebugIsNotNull_Message_False ()
    {
      Assertion.DebugIsNotNull<object> (null, "a");
    }

    [Test]
    public void IsNotNull_Message_Args_True ()
    {
      var instance = "x";

      var result = Assertion.IsNotNull (instance, "a{0}b", 5);

      Assert.That (result, Is.SameAs (instance));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "a5b")]
    public void IsNotNull_Message_Args_False ()
    {
      Assertion.IsNotNull<object> (null, "a{0}b", 5);
    }
  }
}
