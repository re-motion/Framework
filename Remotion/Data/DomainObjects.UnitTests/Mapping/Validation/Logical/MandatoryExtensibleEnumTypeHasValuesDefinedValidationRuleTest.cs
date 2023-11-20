using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.Validation.Logical
{
  [TestFixture]
  public class MandatoryExtensibleEnumTypeHasValuesDefinedValidationRuleTest : ValidationRuleTestBase
  {
    private MandatoryExtensibleEnumTypeHasValuesDefinedValidationRule _validationRule;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new MandatoryExtensibleEnumTypeHasValuesDefinedValidationRule();

      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(EnumTypeValidationDomainObjectClass));
    }

    [Test]
    public void MandatoryPropertyWithOtherType_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithMandatoryOtherType")),
          "PropertyWithMandatoryOtherType",
          false,
          false,
          20,
          StorageClass.Persistent,
          default(float));
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithMandatoryOtherType"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void MandatoryExtensibleEnumWithValuesProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithMandatoryExtensibleEnumTypeWithValues")),
          "PropertyWithMandatoryExtensibleEnumTypeWithValues",
          false,
          false,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithMandatoryExtensibleEnumTypeWithValues"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void NullableExtensibleEnumWithValuesProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithNullableExtensibleEnumTypeWithValues")),
          "PropertyWithNullableExtensibleEnumTypeWithValues",
          false,
          true,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithNullableExtensibleEnumTypeWithValues"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }

    [Test]
    public void MandatoryExtensibleEnumWithoutValuesProperty_UnsupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithMandatoryExtensibleEnumTypeWithoutValues")),
          "PropertyWithMandatoryExtensibleEnumTypeWithoutValues",
          false,
          false,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithMandatoryExtensibleEnumTypeWithoutValues"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      var expectedMessage =
          "Extensible enum type 'ExtensibleEnumTypeWithoutValues' cannot be used for property 'PropertyWithMandatoryExtensibleEnumTypeWithoutValues' "
          + "on type 'EnumTypeValidationDomainObjectClass' because the property is mandatory but there are not values defined for the enum type.\r\n\r\n"
          + "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.EnumTypeValidationDomainObjectClass\r\n"
          + "Property: PropertyWithMandatoryExtensibleEnumTypeWithoutValues";
      AssertMappingValidationResult(validationResult, false, expectedMessage);
    }

    [Test]
    public void NullableExtensibleEnumWithoutValuesProperty_SupportedType ()
    {
      var propertyDefinition = new PropertyDefinition(
          _classDefinition,
          PropertyInfoAdapter.Create(typeof(EnumTypeValidationDomainObjectClass).GetProperty("PropertyWithNullableExtensibleEnumTypeWithoutValues")),
          "PropertyWithNullableExtensibleEnumTypeWithoutValues",
          false,
          true,
          20,
          StorageClass.Persistent,
          null);
      propertyDefinition.SetStorageProperty(new FakeStoragePropertyDefinition("PropertyWithNullableExtensibleEnumTypeWithoutValues"));
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection(new[]{propertyDefinition}, true));
      _classDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate(_classDefinition);

      AssertMappingValidationResult(validationResult, true, null);
    }
  }
}
