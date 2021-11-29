﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Development.UnitTesting.NUnit;

namespace Remotion.UnitTests.Development.NUnit
{
  [TestFixture]
  public class NUnitExtensionsTests
  {
    [Test]
    public void ArgumentExceptionMessageEqualTo_MessageAndParameterEqual_Succeeds ()
    {
      Assert.That(
          () => throw new ArgumentException("The argument must not be null.", "param"),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The argument must not be null.", "param"));
    }

    [Test]
    public void ArgumentExceptionMessageEqualTo_MessageNotEqual_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentException("Must not be null.", "param"),
              Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The argument must not be null.", "param")),
          Throws.InstanceOf<AssertionException>());
    }

    [Test]
    public void ArgumentExceptionMessageEqualTo_ParameterNotEqual_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentException("The argument must not be null.", "argument"),
              Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("The argument must not be null.", "param")),
          Throws.InstanceOf<AssertionException>());
    }

    [Test]
    public void ArgumentExceptionMessageEqualTo_MessageDoesNotContainWhitespace_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentException("Message.", "param"),
              Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Message.", "param")),
          Throws.InstanceOf<InvalidOperationException>()
              .With.Message.EqualTo("The exception message must contain at least one whitespace.\r\nmessage: Message."));
    }

    [Test]
    public void ArgumentExceptionMessageEqualTo_ParameterContainsWhitespace_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentException("Invalid argument.", "param eter"),
              Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo("Invalid argument.", "param eter")),
          Throws.InstanceOf<InvalidOperationException>()
              .With.Message.EqualTo("The parameter must not contain any whitespaces.\r\nparamName: param eter"));
    }

    [Test]
    public void ArgumentOutOfRangeExceptionMessageEqualTo_MessageAndParameterAndActualValueEqual_Succeeds ()
    {
      Assert.That(
          () => throw new ArgumentOutOfRangeException(message: "Not a valid enum value.", paramName: "param", actualValue: -1),
          Throws.InstanceOf<ArgumentOutOfRangeException>().With.ArgumentOutOfRangeExceptionMessageEqualTo("Not a valid enum value.", "param", -1));
    }

    [Test]
    public void ArgumentOutOfRangeExceptionMessageEqualTo_MessageNotEqual_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentOutOfRangeException(message: "Invalid enum value.", paramName: "param", actualValue: -1),
              Throws.InstanceOf<ArgumentOutOfRangeException>()
                  .With.ArgumentOutOfRangeExceptionMessageEqualTo("Not a valid enum value.", "param", -1)),
          Throws.InstanceOf<AssertionException>());
    }

    [Test]
    public void ArgumentOutOfRangeExceptionMessageEqualTo_ParameterNotEqual_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentOutOfRangeException(message: "Not a valid enum value.", paramName: "argument", actualValue: -1),
              Throws.InstanceOf<ArgumentOutOfRangeException>()
                  .With.ArgumentOutOfRangeExceptionMessageEqualTo("Not a valid enum value.", "param", -1)),
          Throws.InstanceOf<AssertionException>());
    }

    [Test]
    public void ArgumentOutOfRangeExceptionMessageEqualTo_ActualValueNotEqual_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentOutOfRangeException(message: "Not a valid enum value.", paramName: "param", actualValue: 99),
              Throws.InstanceOf<ArgumentOutOfRangeException>()
                  .With.ArgumentOutOfRangeExceptionMessageEqualTo("Not a valid enum value.", "param", -1)),
          Throws.InstanceOf<AssertionException>());
    }

    [Test]
    public void ArgumentOutOfRangeExceptionMessageEqualTo_MessageDoesNotContainWhitespace_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentOutOfRangeException(message: "Message.", paramName: "param", actualValue: -1),
              Throws.ArgumentException.With.ArgumentOutOfRangeExceptionMessageEqualTo("Message.", "param", -1)),
          Throws.InstanceOf<InvalidOperationException>()
              .With.Message.EqualTo("The exception message must contain at least one whitespace.\r\nmessage: Message."));
    }

    [Test]
    public void ArgumentOutOfRangeExceptionMessageEqualTo_ParameterContainsWhitespace_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentOutOfRangeException(message: "Invalid argument.", paramName: "param eter", actualValue: -1),
              Throws.ArgumentException.With.ArgumentOutOfRangeExceptionMessageEqualTo("Invalid argument.", "param eter", -1)),
          Throws.InstanceOf<InvalidOperationException>()
              .With.Message.EqualTo("The parameter must not contain any whitespaces.\r\nparamName: param eter"));
    }

    [Test]
    public void ArgumentExceptionWithParameterNameEqualTo_ParameterEqual_Succeeds ()
    {
      Assert.That(
          () => throw new ArgumentException(message: string.Empty, paramName: "param"),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("param"));
    }

    [Test]
    public void ArgumentExceptionWithParameterNameEqualTo_ParameterNotEqual_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentException(message: string.Empty, paramName: "param"),
              Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("parameter")),
          Throws.InstanceOf<AssertionException>());
    }

    [Test]
    public void ArgumentExceptionWithParameterNameEqualTo_ParameterContainsWhitespace_Fails ()
    {
      Assert.That(
          () => Assert.That(
              () => throw new ArgumentException(message: string.Empty, paramName: "param eter"),
              Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("param eter")),
          Throws.InstanceOf<InvalidOperationException>()
              .With.Message.EqualTo("The parameter must not contain any whitespaces.\r\nparamName: param eter"));
    }
  }
}
