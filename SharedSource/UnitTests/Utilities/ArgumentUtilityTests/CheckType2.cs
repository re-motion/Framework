// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

#nullable enable
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
      int result = ArgumentUtility.CheckType<int>("arg", 1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Fail_Int_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<int>("arg", null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      int result = ArgumentUtility.CheckType<int>("arg", (int?)1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckType<int?>("arg", (int?)1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Succeed_NullableInt_Null ()
    {
      int? result = ArgumentUtility.CheckType<int?>("arg", null);
      Assert.That(result, Is.EqualTo(null));
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckType<int?>("arg", 1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckType<string>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Succeed_StringNull ()
    {
      string result = ArgumentUtility.CheckType<string>("arg", null);
      Assert.That(result, Is.EqualTo(null));
    }

    private enum TestEnum
    {
      TestValue
    }

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckType<TestEnum>("arg", TestEnum.TestValue);
      Assert.That(result, Is.EqualTo(TestEnum.TestValue));
    }

    [Test]
    public void Fail_Enum_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<TestEnum>("arg", null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_NullableEnum_Null ()
    {
      TestEnum? result = ArgumentUtility.CheckType<TestEnum?>("arg", null);
      Assert.That(result, Is.EqualTo(null));
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckType<object>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Fail_String_Int ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<string>("arg", 1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }

    [Test]
    public void Fail_Long_Int ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<long>("arg", 1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.Int64' was expected.", "arg"));
    }

    [Test]
    public void Fail_Int_String ()
    {
      Assert.That(
          () => ArgumentUtility.CheckType<int>("arg", "test"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.String' when type 'System.Int32' was expected.", "arg"));
    }
  }
}
