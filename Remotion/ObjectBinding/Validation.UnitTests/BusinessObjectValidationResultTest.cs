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
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Validation.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Results;

namespace Remotion.ObjectBinding.Validation.UnitTests
{
  [TestFixture]
  public class BusinessObjectValidationResultTest
  {
    private IValidatorBuilder _validatorBuilder;
    private IValidator _personValidator;
    private BindableObjectClass _personClass;
    private BindableObjectClass _jobClass;
    private IBusinessObjectProperty _firstNameBusinessObjectProperty;
    private IBusinessObjectProperty _lastNameBusinessObjectProperty;
    private IBusinessObjectProperty _phoneNumberBusinessObjectProperty;
    private IBusinessObjectProperty _customerNumberBusinessObjectProperty;
    private IBusinessObjectProperty _jobTitleBusinessObjectProperty;
    private PropertyInfoAdapter _firstNameProperty;
    private PropertyInfoAdapter _lastNameProperty;
    private PropertyInfoAdapter _phoneNumberProperty;
    private PropertyInfoAdapter _phoneNumberInterfaceProperty;
    private PropertyInfoAdapter _customerNumberInterfaceProperty;
    private PropertyInfoAdapter _jobTitleProperty;

    [SetUp]
    public void SetUp ()
    {
      _validatorBuilder = SafeServiceLocator.Current.GetInstance<IValidatorBuilder>();
      _personValidator = _validatorBuilder.BuildValidator(typeof(Person));

      _personClass = BindableObjectProvider.GetProviderForBindableObjectType(typeof(Person)).GetBindableObjectClass(typeof(Person));
      _jobClass = BindableObjectProvider.GetProviderForBindableObjectType(typeof(Job)).GetBindableObjectClass(typeof(Job));
      _firstNameBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("FirstName"));
      _lastNameBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("LastName"));
      _phoneNumberBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("PhoneNumber"));
      _customerNumberBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("CustomerNumber"));
      _jobTitleBusinessObjectProperty = Assertion.IsNotNull(_jobClass.GetPropertyDefinition("JobTitle"));

      _firstNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.FirstName));
      _lastNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.LastName));
      _phoneNumberProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.PhoneNumber));
      _phoneNumberInterfaceProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((ICustomer _) => _.PhoneNumber));
      _customerNumberInterfaceProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((ICustomer _) => _.CustomerNumber));
      _jobTitleProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Job _) => _.JobTitle));
    }

    [Test]
    public void Create_WithObjectValidationFailureAndNoAffectedProperties_ReturnsSingleBusinessObjectValidationFailure ()
    {
      var person = new Person();
      var objectValidationFailure = ValidationFailure.CreateObjectValidationFailure(
          person,
          "Error",
          "Localized error");
      var validationResult = new ValidationResult(new [] { objectValidationFailure });
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var unhandledPersonValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(person, markAsHandled: false).ToArray();
      Assert.That(unhandledPersonValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledPersonValidationFailures[0].ErrorMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledPersonValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(unhandledPersonValidationFailures[0].ValidatedProperty, Is.Null);


      var unhandledValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures().ToArray();
      Assert.That(unhandledValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledValidationFailures[0].ErrorMessage, Is.EqualTo("Error"));
      Assert.That(unhandledValidationFailures[0].LocalizedValidationMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledValidationFailures[0].ValidatedObject, Is.SameAs(person));
    }

    [Test]
    public void Create_WithObjectValidationFailureAndAffectedProperties_ReturnsBusinessObjectValidationFailureForEachAffectedProperty ()
    {
      var person = new Person();
      var job = new Job();
      var objectValidationFailure = ValidationFailure.CreateObjectValidationFailure(
          person,
          new ValidatedProperty []
          {
              new(person, _firstNameProperty),
              new(person, _lastNameProperty),
              new(job, _jobTitleProperty),
              new(new object(), _phoneNumberProperty)
          },
          "Error",
          "Localized error");
      var validationResult = new ValidationResult(new [] { objectValidationFailure });
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var unhandledPersonValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(person, markAsHandled: false).ToArray();
      Assert.That(unhandledPersonValidationFailures.Count, Is.EqualTo(2));
      Assert.That(unhandledPersonValidationFailures[0].ErrorMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledPersonValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(unhandledPersonValidationFailures[0].ValidatedProperty, Is.SameAs(_firstNameBusinessObjectProperty));
      Assert.That(unhandledPersonValidationFailures[1].ErrorMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledPersonValidationFailures[1].ValidatedObject, Is.SameAs(person));
      Assert.That(unhandledPersonValidationFailures[1].ValidatedProperty, Is.SameAs(_lastNameBusinessObjectProperty));

      var unhandledJobValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(job, markAsHandled: false).ToArray();
      Assert.That(unhandledJobValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledJobValidationFailures[0].ErrorMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledJobValidationFailures[0].ValidatedObject, Is.SameAs(job));
      Assert.That(unhandledJobValidationFailures[0].ValidatedProperty, Is.SameAs(_jobTitleBusinessObjectProperty));

      var unhandledValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures().ToArray();
      Assert.That(unhandledValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledValidationFailures[0].ErrorMessage, Is.EqualTo("Error"));
      Assert.That(unhandledValidationFailures[0].LocalizedValidationMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledValidationFailures[0].ValidatedObject, Is.SameAs(person));
    }

    [Test]
    public void Create_WithObjectValidationFailureAndAffectedPropertiesButNoBusinessObject_ReturnsDefaultFailure ()
    {
      var person = new Person();
      var objectValidationFailure = ValidationFailure.CreateObjectValidationFailure(
          person,
          new ValidatedProperty []
          {
              new(new object(), _phoneNumberProperty)
          },
          "Error",
          "Localized error");
      var validationResult = new ValidationResult(new [] { objectValidationFailure });
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var unhandledPersonValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(person, markAsHandled: false).ToArray();
      Assert.That(unhandledPersonValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledPersonValidationFailures[0].ErrorMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledPersonValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(unhandledPersonValidationFailures[0].ValidatedProperty, Is.Null);

      var unhandledValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures().ToArray();
      Assert.That(unhandledValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledValidationFailures[0].ErrorMessage, Is.EqualTo("Error"));
      Assert.That(unhandledValidationFailures[0].LocalizedValidationMessage, Is.EqualTo("Localized error"));
      Assert.That(unhandledValidationFailures[0].ValidatedObject, Is.SameAs(person));
    }

    [Test]
    public void GetValidationFailures_ReturnsMatchingValidationFailures ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer)person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.SelectMany(f => f.ValidatedProperties.Select(vp => vp.Property)).Distinct().Count(),
          Is.EqualTo(4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var firstNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(firstNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(firstNameValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(firstNameValidationFailures[0].ValidatedProperty, Is.SameAs(_firstNameBusinessObjectProperty));
      Assert.That(firstNameValidationFailures[0].ErrorMessage, Is.EqualTo("NotNullValidator: Validation error."));

      var lastNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person, _lastNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(lastNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(lastNameValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(lastNameValidationFailures[0].ValidatedProperty, Is.SameAs(_lastNameBusinessObjectProperty));
      Assert.That(lastNameValidationFailures[0].ErrorMessage, Is.EqualTo("NotEmptyOrWhitespaceValidator: Validation error."));

      var phoneNumberValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person, _phoneNumberBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(phoneNumberValidationFailures.Length, Is.EqualTo(1));
      Assert.That(phoneNumberValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(phoneNumberValidationFailures[0].ValidatedProperty, Is.SameAs(_phoneNumberBusinessObjectProperty));
      Assert.That(phoneNumberValidationFailures[0].ErrorMessage, Is.EqualTo("NotNullValidator: Validation error."));

      var customerNumberValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person, _customerNumberBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(customerNumberValidationFailures.Length, Is.EqualTo(1));
      Assert.That(customerNumberValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(customerNumberValidationFailures[0].ValidatedProperty, Is.SameAs(_customerNumberBusinessObjectProperty));
      Assert.That(customerNumberValidationFailures[0].ErrorMessage, Is.EqualTo("NotEmptyOrWhitespaceValidator: Validation error."));
    }

    [Test]
    public void GetValidationFailures_WithMarkHasHandledTrueOrFalse_ReturnsMatchingValidationFailuresOnEachCall ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer)person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.SelectMany(f => f.ValidatedProperties.Select(vp => vp.Property)).Distinct().Count(),
          Is.EqualTo(4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var firstNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: false).ToArray();
      Assert.That(firstNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(firstNameValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(firstNameValidationFailures[0].ValidatedProperty, Is.SameAs(_firstNameBusinessObjectProperty));
      Assert.That(
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: false),
          Is.EquivalentTo(firstNameValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: false),
          Is.EquivalentTo(firstNameValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: true),
          Is.EquivalentTo(firstNameValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: true),
          Is.EquivalentTo(firstNameValidationFailures));
    }

    [Test]
    public void GetValidationFailuresForBusinessObject_ReturnsMatchingValidationFailures ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer)person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.SelectMany(f => f.ValidatedProperties.Select(vp => vp.Property)).Distinct().Count(),
          Is.EqualTo(4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var businessObjectValidationFailures =
          businessObjectValidationResult.GetUnhandledValidationFailures(person, false, false).ToArray();

      var firstNamePropertyFailures = businessObjectValidationFailures.Single(f => f.ValidatedProperty == _firstNameBusinessObjectProperty);
      Assert.That(firstNamePropertyFailures.ValidatedObject, Is.SameAs(person));
      Assert.That(firstNamePropertyFailures.ValidatedProperty, Is.SameAs(_firstNameBusinessObjectProperty));
      Assert.That(firstNamePropertyFailures.ErrorMessage, Is.EqualTo("NotNullValidator: Validation error."));

      var lastNamePropertyFailures = businessObjectValidationFailures.Single(f => f.ValidatedProperty == _lastNameBusinessObjectProperty);
      Assert.That(lastNamePropertyFailures.ValidatedObject, Is.SameAs(person));
      Assert.That(lastNamePropertyFailures.ValidatedProperty, Is.SameAs(_lastNameBusinessObjectProperty));
      Assert.That(lastNamePropertyFailures.ErrorMessage, Is.EqualTo("NotEmptyOrWhitespaceValidator: Validation error."));

      var phoneNumberValidationFailures = businessObjectValidationFailures.Single(f => f.ValidatedProperty == _phoneNumberBusinessObjectProperty);
      Assert.That(phoneNumberValidationFailures.ValidatedObject, Is.SameAs(person));
      Assert.That(phoneNumberValidationFailures.ValidatedProperty, Is.SameAs(_phoneNumberBusinessObjectProperty));
      Assert.That(phoneNumberValidationFailures.ErrorMessage, Is.EqualTo("NotNullValidator: Validation error."));

      var customerNumberValidationFailures = businessObjectValidationFailures.Single(f => f.ValidatedProperty == _customerNumberBusinessObjectProperty);
      Assert.That(customerNumberValidationFailures.ValidatedObject, Is.SameAs(person));
      Assert.That(customerNumberValidationFailures.ValidatedProperty, Is.SameAs(_customerNumberBusinessObjectProperty));
      Assert.That(customerNumberValidationFailures.ErrorMessage, Is.EqualTo("NotEmptyOrWhitespaceValidator: Validation error."));
    }

    [Test]
    public void GetValidationFailuresForBusinessObject_WithPartiallyHandledAndMarkAsHandledTrueOrFalse_ReturnsMatchingValidationFailuresOnEachCall ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer)person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.SelectMany(f => f.ValidatedProperties.Select(vp => vp.Property)).Distinct().Count(),
          Is.EqualTo(4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var businessObjectValidationFailures =
          businessObjectValidationResult.GetUnhandledValidationFailures(person, true, markAsHandled: false).ToArray();
      Assert.That(businessObjectValidationFailures.Length, Is.EqualTo(4));
      Assert.That(businessObjectValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, true, markAsHandled: false),
          Is.EquivalentTo(businessObjectValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, true, markAsHandled: false),
          Is.EquivalentTo(businessObjectValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, true, markAsHandled: true),
          Is.EquivalentTo(businessObjectValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, true, markAsHandled: true),
          Is.Empty);
    }

    [Test]
    public void GetValidationFailuresForBusinessObject_WithoutPartiallyHandledAndMarkAsHandledTrueOrFalse_ReturnsOnlyUnmarkedValidationFailures ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer)person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.SelectMany(f => f.ValidatedProperties.Select(vp => vp.Property)).Distinct().Count(),
          Is.EqualTo(4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      var businessObjectValidationFailures =
          businessObjectValidationResult.GetUnhandledValidationFailures(person, false, markAsHandled: false).ToArray();
      Assert.That(businessObjectValidationFailures.Length, Is.EqualTo(4));
      Assert.That(businessObjectValidationFailures[0].ValidatedObject, Is.SameAs(person));
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, false, markAsHandled: true),
          Is.EquivalentTo(businessObjectValidationFailures));
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, false, markAsHandled: false),
          Is.Empty);
      Assert.That(
          businessObjectValidationResult.GetUnhandledValidationFailures(person, false, markAsHandled: true),
          Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailures_ForBusinessObject_ReturnsFilteredResult ()
    {
      var person1 = new Person();
      person1.FirstName = null;
      person1.LastName = "";
      person1.PhoneNumber = null;
      ((ICustomer)person1).CustomerNumber = "";

      var person2 = new Person();
      person2.FirstName = null;
      person2.LastName = "LastName";
      person2.PhoneNumber = "PhoneNumber";
      ((ICustomer)person2).CustomerNumber = "CustomerNumber";

      var validationResult1 = _personValidator.Validate(person1);
      Assert.That(validationResult1.IsValid, Is.False);
      Assert.That(validationResult1.Errors.Count, Is.EqualTo(4));

      var validationResult2 = _personValidator.Validate(person2);
      Assert.That(validationResult2.IsValid, Is.False);
      Assert.That(validationResult2.Errors.Count, Is.EqualTo(1));

      var combinedValidationResult = new ValidationResult(
          validationResult1.Errors
              .Concat(validationResult2.Errors)
              .Concat(new []
                      {
                          ValidationFailure.CreateObjectValidationFailure(
                              person1,
                              "Object Validation Error",
                              "Localized Object Validation Error")
                      })
              .ToArray());
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(combinedValidationResult);

      var firstNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person1, _firstNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(firstNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(firstNameValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(firstNameValidationFailures[0].ValidatedProperty, Is.SameAs(_firstNameBusinessObjectProperty));

      var lastNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person1, _lastNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(lastNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(lastNameValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(lastNameValidationFailures[0].ValidatedProperty, Is.SameAs(_lastNameBusinessObjectProperty));

      var phoneNumberValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person1, _phoneNumberBusinessObjectProperty, markAsHandled: false).ToArray();
      Assert.That(phoneNumberValidationFailures.Length, Is.EqualTo(1));
      Assert.That(phoneNumberValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(phoneNumberValidationFailures[0].ValidatedProperty, Is.SameAs(_phoneNumberBusinessObjectProperty));

      var unhandledValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(person1);
      Assert.That(unhandledValidationFailures.Count, Is.EqualTo(3));
      var sortedUnhandledValidationFailures = unhandledValidationFailures.OrderBy(f => f.ValidatedProperty?.Identifier).ToArray();

      Assert.That(sortedUnhandledValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(sortedUnhandledValidationFailures[0].ValidatedProperty, Is.Null);
      Assert.That(sortedUnhandledValidationFailures[0].ErrorMessage, Is.EqualTo("Localized Object Validation Error"));

      Assert.That(sortedUnhandledValidationFailures[1].ValidatedObject, Is.SameAs(person1));
      Assert.That(sortedUnhandledValidationFailures[1].ValidatedProperty, Is.SameAs(_customerNumberBusinessObjectProperty));
      Assert.That(sortedUnhandledValidationFailures[1].ErrorMessage, Is.EqualTo("NotEmptyOrWhitespaceValidator: Validation error."));

      Assert.That(sortedUnhandledValidationFailures[2].ValidatedObject, Is.SameAs(person1));
      Assert.That(sortedUnhandledValidationFailures[2].ValidatedProperty, Is.SameAs(_phoneNumberBusinessObjectProperty));
      Assert.That(sortedUnhandledValidationFailures[2].ErrorMessage, Is.EqualTo("NotNullValidator: Validation error."));
    }

    [Test]
    public void GetUnhandledValidationFailures_Twice_DoesNotChangeTheResult ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer)person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);

      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures(person).Count, Is.EqualTo(4));

      Assert.That(
          businessObjectValidationResult.GetValidationFailures(person, _firstNameBusinessObjectProperty, markAsHandled: true).Count,
          Is.EqualTo(1));
      Assert.That(
          businessObjectValidationResult.GetValidationFailures(person, _lastNameBusinessObjectProperty, markAsHandled: true).Count,
          Is.EqualTo(1));

      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures(person).Count, Is.EqualTo(2));
    }

    [Test]
    public void GetUnhandledValidationFailures_ForAllObjects_ReturnsFilteredResult ()
    {
      var person1 = new Person();
      person1.FirstName = null;
      person1.LastName = "";
      person1.PhoneNumber = null;
      ((ICustomer)person1).CustomerNumber = "";

      var person2 = new Person();
      person2.FirstName = null;
      person2.LastName = "LastName";
      person2.PhoneNumber = "PhoneNumber";
      ((ICustomer)person2).CustomerNumber = "CustomerNumber";

      var validationResult1 = _personValidator.Validate(person1);
      Assert.That(validationResult1.IsValid, Is.False);
      Assert.That(validationResult1.Errors.Count, Is.EqualTo(4));

      var validationResult2 = _personValidator.Validate(person2);
      Assert.That(validationResult2.IsValid, Is.False);
      Assert.That(validationResult2.Errors.Count, Is.EqualTo(1));

      var combinedValidationResult = new ValidationResult(
          validationResult1.Errors
              .Concat(validationResult2.Errors)
              .Concat(new []
                      {
                          ValidationFailure.CreateObjectValidationFailure(
                              person1,
                              "Object Validation Error",
                              "Localized Object Validation Error")
                      })
              .ToArray());
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(combinedValidationResult);

      var firstNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person1, _firstNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(firstNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(firstNameValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(firstNameValidationFailures[0].ValidatedProperty, Is.SameAs(_firstNameBusinessObjectProperty));

      var lastNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person1, _lastNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(lastNameValidationFailures.Length, Is.EqualTo(1));
      Assert.That(lastNameValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(lastNameValidationFailures[0].ValidatedProperty, Is.SameAs(_lastNameBusinessObjectProperty));

      var phoneNumberValidationFailures =
          businessObjectValidationResult.GetValidationFailures(person1, _phoneNumberBusinessObjectProperty, markAsHandled: false).ToArray();
      Assert.That(phoneNumberValidationFailures.Length, Is.EqualTo(1));
      Assert.That(phoneNumberValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(phoneNumberValidationFailures[0].ValidatedProperty, Is.SameAs(_phoneNumberBusinessObjectProperty));

      var unhandledValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures();

      var unhandledValidationFailuresForResult1 = validationResult1.Errors
          .Where(f => f.ValidatedProperties
              .Select(vp => vp.Property)
              .All(p => p.Equals(_firstNameProperty) || p.Equals(_lastNameProperty)));

      Assert.That(unhandledValidationFailures, Is.EquivalentTo(combinedValidationResult.Errors.Except(unhandledValidationFailuresForResult1)));
    }

    [Test]
    public void GetUnhandledValidationFailures_WithMultipleBusinessObjectFailuresForSameValidationFailure_DoesNotReturnPartiallyHandledValidationFailure ()
    {
      var person1 = new Person();
      var objectValidationFailure = ValidationFailure.CreateObjectValidationFailure(
          person1,
          new ValidatedProperty []
          {
              new(person1, _firstNameProperty),
              new(person1, _lastNameProperty)
          },
          "Error",
          "Localized error");
      var validationResult = new ValidationResult(new [] { objectValidationFailure });
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);
      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures().Count, Is.EqualTo(1));
      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures(person1).Count, Is.EqualTo(2));

      var failure = businessObjectValidationResult.GetValidationFailures(person1, _firstNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(failure.Length, Is.EqualTo(1));

      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures(), Is.Empty);
      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures(person1), Is.Empty);
    }

    [Test]
    public void GetUnhandledValidationFailures_WithMultipleBusinessObjectFailuresForSameValidationFailureAndIncludePartiallyHandled_ReturnsPartiallyHandledValidationFailure ()
    {
      var person1 = new Person();
      var objectValidationFailure = ValidationFailure.CreateObjectValidationFailure(
          person1,
          new ValidatedProperty []
          {
              new(person1, _firstNameProperty),
              new(person1, _lastNameProperty)
          },
          "Error",
          "Localized error");
      var validationResult = new ValidationResult(new [] { objectValidationFailure });
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(validationResult);
      Assert.That(businessObjectValidationResult.GetUnhandledValidationFailures().Count, Is.EqualTo(1));

      var failure = businessObjectValidationResult.GetValidationFailures(person1, _firstNameBusinessObjectProperty, markAsHandled: true).ToArray();
      Assert.That(failure.Length, Is.EqualTo(1));

      var unhandledValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(true).ToArray();
      Assert.That(unhandledValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledValidationFailures[0], Is.SameAs(objectValidationFailure));

      var unhandledBusinessObjectValidationFailures = businessObjectValidationResult.GetUnhandledValidationFailures(person1, true).ToArray();
      Assert.That(unhandledBusinessObjectValidationFailures.Count, Is.EqualTo(1));
      Assert.That(unhandledBusinessObjectValidationFailures[0].ValidatedObject, Is.SameAs(person1));
      Assert.That(unhandledBusinessObjectValidationFailures[0].ValidatedProperty, Is.SameAs(_lastNameBusinessObjectProperty));
    }
  }
}
