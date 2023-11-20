using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Reflection
{
  [TestFixture]
  public class TypeExtensionsTests
  {
    public enum TestEnum
    {
      One = 1,
      Two = 2
    }

    [Test]
    [TestCase(typeof(bool), false)]
    [TestCase(typeof(int), 0)]
    [TestCase(typeof(double), 0)]
    [TestCase(typeof(decimal), 0)]
    [TestCase(typeof(TestEnum), (TestEnum)0)]
    public void GetDefaultValue_ValueType_ReturnsDotNetDefaultValue (Type type, object expectedValue)
    {
      var defaultValue = type.GetDefaultValue();
      Assert.That(defaultValue, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase(typeof(bool?))]
    [TestCase(typeof(int?))]
    [TestCase(typeof(double?))]
    [TestCase(typeof(decimal?))]
    [TestCase(typeof(TestEnum?))]
    public void GetDefaultValue_NullableValueType_ReturnsNull (Type type)
    {
      var defaultValue = type.GetDefaultValue();
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    [TestCase(typeof(TypeExtensions))]
    public void GetDefaultValue_ReferenceType_ReturnsNull (Type type)
    {
      var defaultValue = type.GetDefaultValue();
      Assert.That(defaultValue, Is.Null);
    }
  }
}
