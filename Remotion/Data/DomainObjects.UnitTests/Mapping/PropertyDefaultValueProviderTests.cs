using System;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PropertyDefaultValueProviderTests
  {
    private PropertyDefaultValueProvider _defaultValueProvider;
    private Mock<IPropertyInformation> _propertyInformationStub;

    [OneTimeSetUp]
    public void SetUp ()
    {
      _defaultValueProvider = new PropertyDefaultValueProvider();

      _propertyInformationStub = new Mock<IPropertyInformation>();
      _propertyInformationStub.Setup(stub => stub.Name).Returns("Test");
      _propertyInformationStub.Setup(stub => stub.DeclaringType).Returns(TypeAdapter.Create(typeof(Order)));
      _propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(TypeAdapter.Create(typeof(Order)));
    }

    [Test]
    public void DefaultValue_NullableValueType ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int?));

      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, true);
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_ReferenceType ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, true);
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_NotNullable_Array ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(byte[]));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_NotNullable_String ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(string));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_NotNullable_Enum ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(EnumNotDefiningZero));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.EqualTo(EnumNotDefiningZero.First));
    }

    [Test]
    public void DefaultValue_NotNullable_EnumWithoutValues ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(EnumNotDefiningAnyValues));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.EqualTo((EnumNotDefiningAnyValues)0));
    }

    [Test]
    public void DefaultValue_NotNullable_ExtensibleEnum ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(Color));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_NotNullable_ExtensibleEnumWithoutValues ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(ExtensibleEnumNotDefiningAnyValues));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.Null);
    }

    [Test]
    public void DefaultValue_ValueType ()
    {
      _propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(int));
      var defaultValue = _defaultValueProvider.GetDefaultValue(_propertyInformationStub.Object, false);
      Assert.That(defaultValue, Is.EqualTo(0));
    }
  }
}
