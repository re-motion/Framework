// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable disable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
#if !DEBUG
  [Ignore("Skipped unless DEBUG build")]
#endif
  [TestFixture]
  public class DebugCheckNotNullAndType
  {
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]

    [Test]
    public void Succeed_Int ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", 1, typeof(int));
    }

    [Test]
    public void Fail_Int_Null ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", null, typeof(int)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", (int?)1, typeof(int));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", (int?)1, typeof(int?));
    }

    [Test]
    public void Fail_NullableInt_Null ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", null, typeof(int?)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", 1, typeof(int?));
    }

    [Test]
    public void Succeed_String ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", "test", typeof(string));
    }

    [Test]
    public void Fail_StringNull ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", null, typeof(string)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    private enum TestEnum
    {
      TestValue
    }

    [Test]
    public void Succeed_Enum ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", TestEnum.TestValue, typeof(TestEnum));
    }

    [Test]
    public void Fail_Enum_Null ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", null, typeof(TestEnum)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Object_String ()
    {
      ArgumentUtility.DebugCheckNotNullAndType("arg", "test", typeof(object));
    }

    [Test]
    public void Fail_String_Int ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", 1, typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }

    [Test]
    public void Fail_Long_Int ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", 1, typeof(long)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.Int64' was expected.", "arg"));
    }

    [Test]
    public void Fail_Int_String ()
    {
      Assert.That(
          () => ArgumentUtility.DebugCheckNotNullAndType("arg", "test", typeof(int)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.String' when type 'System.Int32' was expected.", "arg"));
    }
  }
}
