using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class MandatoryNetEnumTypeHasValuesDefinedValidationRuleTest : ValidationRuleTestBase
  {
    private MandatoryNetEnumTypeHasValuesDefinedValidationRule _validationRule;
    private TypeDefinition _typeDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new MandatoryNetEnumTypeHasValuesDefinedValidationRule();

      _typeDefinition = TypeDefinitionObjectMother.CreateClassDefinitionWithMixinsAndDefaultProperties(typeof(EnumTypeValidationDomainObjectClass));
    }

    [Test]
    public void PropertyWithOtherType_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _typeDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithMandatoryOtherType")),
          "PropertyWithMandatoryOtherType",
          false,
          false,
          20,
          StorageClass.Persistent,
          default(float));
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithMandatoryOtherType"));
      _typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _typeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void MandatoryNetEnumWithValuesProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _typeDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithMandatoryNetEnumTypeWithValues")),
          "PropertyWithMandatoryNetEnumTypeWithValues",
          false,
          false,
          20,
          StorageClass.Persistent,
          default(EnumTypeValidationDomainObjectClass.NetEnumTypeWithValues));
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithMandatoryNetEnumTypeWithValues"));
      _typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _typeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NullableNetEnumWithValuesProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _typeDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithNullableNetEnumTypeWithValues")),
          "PropertyWithNullableNetEnumTypeWithValues",
          false,
          true,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithNullableNetEnumTypeWithValues"));
      _typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _typeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void MandatoryNetEnumWithoutValuesProperty_UnsupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _typeDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithMandatoryNetEnumTypeWithoutValues")),
          "PropertyWithMandatoryNetEnumTypeWithoutValues",
          false,
          false,
          20,
          StorageClass.Persistent,
          default(EnumTypeValidationDomainObjectClass.NetEnumTypeWithoutValues));
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithMandatoryNetEnumTypeWithoutValues"));
      _typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _typeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_typeDefinition);

      var expectedMessage =
          "Enum type 'NetEnumTypeWithoutValues' cannot be used for property 'PropertyWithMandatoryNetEnumTypeWithoutValues' "
          + "on type 'EnumTypeValidationDomainObjectClass' because the property is mandatory but there are not values defined for the enum type.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.EnumTypeValidationDomainObjectClass\r\n"
          + "Property: PropertyWithMandatoryNetEnumTypeWithoutValues";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void NullableNetEnumWithoutValuesProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _typeDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithNullableNetEnumTypeWithoutValues")),
          "PropertyWithNullableNetEnumTypeWithoutValues",
          false,
          true,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithNullableNetEnumTypeWithoutValues"));
      _typeDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _typeDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_typeDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
