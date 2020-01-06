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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Merging;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.UnitTests.TestDomain.Collectors;
using Remotion.Validation.UnitTests.TestHelpers;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class AddingComponentPropertyRule1Test
  {
    private Expression<Func<Customer, string>> _userNameExpression;
    private Expression<Func<Customer, string>> _lastNameExpression;
    private IAddingComponentPropertyRule _addingComponentPropertyRule;
    private IPropertyInformation _property;
    private IPropertyValidatorExtractor _propertyValidatorExtractorMock;
    private StubPropertyValidator _stubPropertyValidator1;
    private NotEmptyValidator _stubPropertyValidator2;
    private NotEqualValidator _stubPropertyValidator3;

    [SetUp]
    public void SetUp ()
    {
      _property = PropertyInfoAdapter.Create(typeof (Customer).GetProperty ("UserName"));

      _userNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.UserName);
      _lastNameExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.LastName);

      _stubPropertyValidator1 = new StubPropertyValidator();
      _stubPropertyValidator2 = new NotEmptyValidator (new InvariantValidationMessage ("Fake Message"));
      _stubPropertyValidator3 = new NotEqualValidator ("gfsf", new InvariantValidationMessage ("Fake Message"));

      _propertyValidatorExtractorMock = MockRepository.GenerateStrictMock<IPropertyValidatorExtractor>();

      _addingComponentPropertyRule = AddingComponentPropertyRule.Create (_userNameExpression, typeof (CustomerValidationCollector1));
    }

    [Test]
    public void Initialization_PropertyDeclaredInSameClass ()
    {
      var propertyInfo = ((PropertyInfoAdapter) _addingComponentPropertyRule.Property).PropertyInfo;
      Assert.That (_addingComponentPropertyRule.Property.Equals(_property), Is.True);
      Assert.That (_addingComponentPropertyRule.Property, Is.EqualTo (_property));
      Assert.That (propertyInfo.DeclaringType, Is.EqualTo (typeof (Customer)));
      Assert.That (propertyInfo.ReflectedType, Is.EqualTo (typeof (Customer)));
      Assert.That (_addingComponentPropertyRule.CollectorType, Is.EqualTo (typeof (CustomerValidationCollector1)));
      Assert.That (_addingComponentPropertyRule.Validators.Any(), Is.False);
      Assert.That (_addingComponentPropertyRule.IsHardConstraint, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void Initialization_CollectorTypeDoesNotImplementIComponentValidationCollector_ThrowsArgumentException ()
    {
    }

    [Test]
    public void Create_MemberInfoIsNoPropertyInfo_ExceptionIsThrown ()
    {
      var dummyExpression = ExpressionHelper.GetTypedMemberExpression<Customer, string> (c => c.Dummy());

      Assert.That (
          () => AddingComponentPropertyRule.Create (dummyExpression, typeof (CustomerValidationCollector1)),
          Throws.ArgumentException.With.Message.EqualTo ("Must be a MemberExpression.\r\nParameter name: expression"));
    }

    [Test]
    public void Create_PropertyDeclaredInBaseClass ()
    {
      var componentPropertyRule = AddingComponentPropertyRule.Create (_lastNameExpression, typeof (CustomerValidationCollector1));
      var propertyInfo = ((PropertyInfoAdapter) componentPropertyRule.Property).PropertyInfo;

      //TODO-5906 simplify assertion with PropertyInfoAdapter compare
      Assert.That (
          MemberInfoEqualityComparer<MemberInfo>.Instance.Equals (propertyInfo, typeof (Customer).GetMember ("LastName")[0]),
          Is.True);
      Assert.That (propertyInfo.DeclaringType, Is.EqualTo (typeof (Person)));
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void Create_BuildsFuncForPropertyAccess ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void CreateValidationRule_InitializesDeferredInitializationValidationMessages ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void CreateValidationRule_WhenCalledTwice_InitializesDeferredInitializationValidationMessagesOnlyForNewlyRegisteredValidators ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void CreateValidationRule_IgnoresValidationMessagesForValidatorsWithoutADeferredInitializationValidationMessage ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void CreateValidationRule_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void CreateValidationRule_WithCondition_UsesConditionForCreatedPropertyValidationRule ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void CreateValidationRule_WithoutCondition_UsesAlwaysTrueConditionForCreatedPropertyValidationRule ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void SetCondition ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void SetCondition_Twice_UsesNewCondition ()
    {
    }

    [Test]
    public void RegisterValidator ()
    {
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator1);
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator2);

      Assert.That (_addingComponentPropertyRule.Validators.Count(), Is.EqualTo (2));
      Assert.That (
          _addingComponentPropertyRule.Validators,
          Is.EquivalentTo (new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator2 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_NoHardConstraint ()
    {
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator1);
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator2);
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator3);
      Assert.That (_addingComponentPropertyRule.Validators.Count(), Is.EqualTo (3));

      _propertyValidatorExtractorMock
          .Expect (
              mock => mock.ExtractPropertyValidatorsToRemove (_addingComponentPropertyRule))
          .Return (new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator3 });

      _addingComponentPropertyRule.ApplyRemoveValidatorRegistrations (_propertyValidatorExtractorMock);

      _propertyValidatorExtractorMock.VerifyAllExpectations();
      Assert.That (_addingComponentPropertyRule.Validators, Is.EqualTo (new[] { _stubPropertyValidator2 }));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_HardConstraintAndNoValidatorsToRemove_NoExceptionIsThrown ()
    {
      _addingComponentPropertyRule.SetHardConstraint();
      Assert.That (_addingComponentPropertyRule.IsHardConstraint, Is.True);
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator1);
      Assert.That (_addingComponentPropertyRule.Validators.Count(), Is.EqualTo (1));

      _propertyValidatorExtractorMock
          .Stub (
              stub => stub.ExtractPropertyValidatorsToRemove (_addingComponentPropertyRule))
          .Return (new IPropertyValidator[0]);

      _addingComponentPropertyRule.ApplyRemoveValidatorRegistrations (_propertyValidatorExtractorMock);

      Assert.That (_addingComponentPropertyRule.Validators.Count(), Is.EqualTo (1));
    }

    [Test]
    public void ApplyRemoveValidatorRegistrations_HardConstraintAndValidatorsToRemove ()
    {
      _addingComponentPropertyRule.SetHardConstraint();
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator1);
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator2);
      _addingComponentPropertyRule.RegisterValidator (_ => _stubPropertyValidator3);
      Assert.That (_addingComponentPropertyRule.Validators.Count(), Is.EqualTo (3));

      _propertyValidatorExtractorMock
          .Stub (
              stub => stub.ExtractPropertyValidatorsToRemove (_addingComponentPropertyRule))
          .Return (new IPropertyValidator[] { _stubPropertyValidator1, _stubPropertyValidator3 });

      Assert.That (
          () => _addingComponentPropertyRule.ApplyRemoveValidatorRegistrations (_propertyValidatorExtractorMock),
          Throws.TypeOf<ValidationConfigurationException>().And.Message.EqualTo (
              "Hard constraint validator(s) 'StubPropertyValidator, NotEqualValidator' on property "
              + "'Remotion.Validation.UnitTests.TestDomain.Customer.UserName' cannot be removed."));
    }

    [Test]
    public void ToString_NoHardConstraint ()
    {
      var result = _addingComponentPropertyRule.ToString();

      Assert.That (
          result,
          Is.EqualTo ("AddingComponentPropertyRule: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_HardConstraint ()
    {
      _addingComponentPropertyRule.SetHardConstraint();
      var result = _addingComponentPropertyRule.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingComponentPropertyRule (HARD CONSTRAINT): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithoutCondition ()
    {
      var result = _addingComponentPropertyRule.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingComponentPropertyRule: Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithCondition ()
    {
      _addingComponentPropertyRule.SetCondition ((Customer o) => true);
      var result = _addingComponentPropertyRule.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingComponentPropertyRule (CONDITIONAL): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }

    [Test]
    public void ToString_WithConditionAndHardConstraint ()
    {
      _addingComponentPropertyRule.SetCondition ((Customer o) => true);
      _addingComponentPropertyRule.SetHardConstraint();
      var result = _addingComponentPropertyRule.ToString();

      Assert.That (
          result,
          Is.EqualTo (
              "AddingComponentPropertyRule (CONDITIONAL, HARD CONSTRAINT): Remotion.Validation.UnitTests.TestDomain.Customer#UserName"));
    }
  }
}