// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.IntegrationTests.TestDomain.MetaValidation;

namespace Remotion.Validation.IntegrationTests
{
  [TestFixture]
  public class MetaValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildValidator_MaxLengthPropertyMetaValidationRule ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<MetaValidationTestClass1>(),
          Throws.TypeOf<ValidationConfigurationException>()
                .And.Message.EqualTo("MaxLength-Constraints greater 50 not allowed for validator 'LengthValidator'!"));
    }

    [Test]
    public void BuildValidator_MaxValidatorCountRule ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<MetaValidationTestClass2>(),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo("More than three validators per property are not allowed!"));
    }

    [Test]
    public void BuildValidator_MaxLengthPropertyMetaValidationRule_SupportsConditionalRules ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<MetaValidationTestClass3>(),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo("MaxLength-Constraints greater 50 not allowed for validator 'MaximumLengthValidator'!"));
    }

    [Test]
    public void BuildValidator_FuncPropertyMetaValidationRule ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<MetaValidationTestClass4>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Invalid length validator count!"
              ));
    }

    [Test]
    public void BuildValidator_ExpressionPropertyMetaValidationRule ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<MetaValidationTestClass5>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Meta validation rule 'lengthRules => (lengthRules.Count() <= 2)' failed for validator 'Remotion.Validation.Validators.LengthValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.MetaValidationTestClass5.Property1'."
                  ));
    }

    [Test]
    public void BuildValidator_BaseClass_RulesFromOverridedDerivedPropertiesAreIgnored ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<BaseMetaValidationTestClass1>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.BaseMetaValidationTestClass1.Property3'.\r\n"
                  + "----------\r\n"
                  + "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.BaseMetaValidationTestClass1.Property4'."));
    }

    [Test]
    public void BuildValidator_DerivedClassWithCollector_RulesFromOverridedBaseAndDerivedPropertiesAreApplied ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<DerivedMetaValidationTestClass1>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.BaseMetaValidationTestClass1.Property4'.\r\n"
                  + "----------\r\n"
                  + "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.BaseMetaValidationTestClass1.Property5'."));
    }

    [Test]
    public void BuildValidator_DerivedClassWithoutCollector_RulesFromOtherHierarchyLevelsAreIgnored ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<DerivedMetaValidationTestClass2>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.BaseMetaValidationTestClass1.Property3'.\r\n"
                  + "----------\r\n"
                  + "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IPropertyValidator' "
                  + "on property 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.BaseMetaValidationTestClass1.Property4'."));
    }

    [Test]
    public void BuildValidator_ExpressionObjectMetaValidationRule ()
    {
      Assert.That(
          () => ValidationBuilder.BuildValidator<MetaValidationTestClass6>(),
          Throws.TypeOf<ValidationConfigurationException>()
              .And.Message.EqualTo(
                  "Meta validation rule 'rules => rules.Any()' failed for validator 'Remotion.Validation.Validators.IObjectValidator' "
                  + "on type 'Remotion.Validation.IntegrationTests.TestDomain.MetaValidation.MetaValidationTestClass6'."));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void BuildValidator_WithObjectValidationRule ()
    {
      //possibly integrate with existing tests.
    }
  }
}
