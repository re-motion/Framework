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
  [TestFixture]
  public class CheckNotNullAndType
  {
    // test names have the format {Succeed|Fail}_ExpectedType[_ActualTypeOrNull]
    [Test]
    public void Succeed_Int ()
    {
      int result = ArgumentUtility.CheckNotNullAndType<int>("arg", 1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Fail_Int_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<int>("arg", null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Int_NullableInt ()
    {
      int result = ArgumentUtility.CheckNotNullAndType<int>("arg", (int?)1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Succeed_NullableInt ()
    {
      int? result = ArgumentUtility.CheckNotNullAndType<int?>("arg", (int?)1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Fail_NullableInt_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<int?>("arg", null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_NullableInt_Int ()
    {
      int? result = ArgumentUtility.CheckNotNullAndType<int?>("arg", 1);
      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void Succeed_String ()
    {
      string result = ArgumentUtility.CheckNotNullAndType<string>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Fail_StringNull ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<string>("arg", null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    private enum TestEnum
    {
      TestValue
    }

    [Test]
    public void Succeed_Enum ()
    {
      TestEnum result = ArgumentUtility.CheckNotNullAndType<TestEnum>("arg", TestEnum.TestValue);
      Assert.That(result, Is.EqualTo(TestEnum.TestValue));
    }

    [Test]
    public void Fail_Enum_Null ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<TestEnum>("arg", null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Succeed_Object_String ()
    {
      object result = ArgumentUtility.CheckNotNullAndType<object>("arg", "test");
      Assert.That(result, Is.EqualTo("test"));
    }

    [Test]
    public void Fail_String_Int ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<string>("arg", 1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }

    [Test]
    public void Fail_Long_Int ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<long>("arg", 1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.Int64' was expected.", "arg"));
    }

    [Test]
    public void Fail_Int_String ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType<int>("arg", "test"),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.String' when type 'System.Int32' was expected.", "arg"));
    }

    [Test]
    public void Fail_Null_String_NonGeneric ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType("arg", (object)null, typeof(string)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_Type_String_NonGeneric ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType("arg", 13, typeof(string)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Int32' when type 'System.String' was expected.", "arg"));
    }


    [Test]
    public void Fail_Null_Int_NonGeneric ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType("arg", (object)null, typeof(int)),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void Fail_Type_Int_NonGeneric ()
    {
      Assert.That(
          () => ArgumentUtility.CheckNotNullAndType("arg", 13.0, typeof(int)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("Parameter 'arg' has type 'System.Double' when type 'System.Int32' was expected.", "arg"));
    }

    [Test]
    public void Succeed_Int_NonGeneric ()
    {
      ArgumentUtility.CheckNotNullAndType("arg", 10, typeof(int));
    }
  }
}
