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
    private IBusinessObjectProperty _firstNameBusinessObjectProperty;
    private IBusinessObjectProperty _lastNameBusinessObjectProperty;
    private IBusinessObjectProperty _phoneNumberBusinessObjectProperty;
    private IBusinessObjectProperty _customerNumberBusinessObjectProperty;
    private PropertyInfoAdapter _firstNameProperty;
    private PropertyInfoAdapter _lastNameProperty;
    private PropertyInfoAdapter _phoneNumberProperty;
    private PropertyInfoAdapter _phoneNumberInterfaceProperty;
    private PropertyInfoAdapter _customerNumberInterfaceProperty;

    [SetUp]
    public void SetUp ()
    {
      _validatorBuilder = SafeServiceLocator.Current.GetInstance<IValidatorBuilder>();
      _personValidator = _validatorBuilder.BuildValidator(typeof (Person));

      _personClass = BindableObjectProvider.GetProviderForBindableObjectType(typeof (Person)).GetBindableObjectClass(typeof (Person));
      _firstNameBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("FirstName"));
      _lastNameBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("LastName"));
      _phoneNumberBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("PhoneNumber"));
      _customerNumberBusinessObjectProperty = Assertion.IsNotNull(_personClass.GetPropertyDefinition("CustomerNumber"));

      _firstNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.FirstName));
      _lastNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.LastName));
      _phoneNumberProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.PhoneNumber));
      _phoneNumberInterfaceProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((ICustomer _) => _.PhoneNumber));
      _customerNumberInterfaceProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((ICustomer _) => _.CustomerNumber));
    }

    [Test]
    public void GetValidationFailures_ReturnsMatchingValidationFailures ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer) person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.OfType<PropertyValidationFailure>().Select(f => f.ValidatedProperty).Distinct().Count(),
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
      Assert.That(lastNameValidationFailures[0].ErrorMessage, Is.EqualTo("NotEmptyValidator: Validation error."));

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
      Assert.That(customerNumberValidationFailures[0].ErrorMessage, Is.EqualTo("NotEmptyValidator: Validation error."));
    }

    [Test]
    public void GetValidationFailures_WithMarkHasHandledTrueOrFalse_ReturnsMatchingValidationFailuresOnEachCall ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer) person).CustomerNumber = "";

      var validationResult = _personValidator.Validate(person);
      Assert.That(validationResult.IsValid, Is.False);
      Assert.That(validationResult.Errors.Count, Is.EqualTo(4));
      Assert.That(
          validationResult.Errors.OfType<PropertyValidationFailure>().Select(f => f.ValidatedProperty).Distinct().Count(),
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
    public void GetUnhandledValidationFailures_ForBusinessObject_ReturnsFilteredResult ()
    {
      var person1 = new Person();
      person1.FirstName = null;
      person1.LastName = "";
      person1.PhoneNumber = null;
      ((ICustomer) person1).CustomerNumber = "";

      var person2 = new Person();
      person2.FirstName = null;
      person2.LastName = "LastName";
      person2.PhoneNumber = "PhoneNumber";
      ((ICustomer) person2).CustomerNumber = "CustomerNumber";

      var validationResult1 = _personValidator.Validate(person1);
      Assert.That(validationResult1.IsValid, Is.False);
      Assert.That(validationResult1.Errors.Count, Is.EqualTo(4));

      var validationResult2 = _personValidator.Validate(person2);
      Assert.That(validationResult2.IsValid, Is.False);
      Assert.That(validationResult2.Errors.Count, Is.EqualTo(1));

      var combinedValidationResult = new ValidationResult(
          validationResult1.Errors
              .Concat(validationResult2.Errors)
              .Concat(new[] { new ObjectValidationFailure(person1, "Object Validation Error", "Localized Object Validation Error") })
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
      var sortedUnhandledValidationFailures = unhandledValidationFailures.OrderBy(f=> f.ValidatedProperty?.Identifier).ToArray();

      Assert.That(sortedUnhandledValidationFailures[0].ValidatedProperty, Is.Null);
      Assert.That(sortedUnhandledValidationFailures[0].ErrorMessage, Is.EqualTo("Localized Object Validation Error"));

      Assert.That(sortedUnhandledValidationFailures[1].ValidatedProperty, Is.SameAs(_customerNumberBusinessObjectProperty));
      Assert.That(sortedUnhandledValidationFailures[1].ErrorMessage, Is.EqualTo("NotEmptyValidator: Validation error."));

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
      ((ICustomer) person).CustomerNumber = "";

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
      ((ICustomer) person1).CustomerNumber = "";

      var person2 = new Person();
      person2.FirstName = null;
      person2.LastName = "LastName";
      person2.PhoneNumber = "PhoneNumber";
      ((ICustomer) person2).CustomerNumber = "CustomerNumber";

      var validationResult1 = _personValidator.Validate(person1);
      Assert.That(validationResult1.IsValid, Is.False);
      Assert.That(validationResult1.Errors.Count, Is.EqualTo(4));

      var validationResult2 = _personValidator.Validate(person2);
      Assert.That(validationResult2.IsValid, Is.False);
      Assert.That(validationResult2.Errors.Count, Is.EqualTo(1));

      var combinedValidationResult = new ValidationResult(
          validationResult1.Errors
              .Concat(validationResult2.Errors)
              .Concat(new[] { new ObjectValidationFailure(person1, "Object Validation Error", "Localized Object Validation Error") })
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

      var unhandledValidationFailuresForResult1 = validationResult1.Errors.OfType<PropertyValidationFailure>()
          .Where(f => f.ValidatedProperty.Equals(_firstNameProperty) || f.ValidatedProperty.Equals(_lastNameProperty));

      Assert.That(unhandledValidationFailures, Is.EquivalentTo(combinedValidationResult.Errors.Except(unhandledValidationFailuresForResult1)));
    }
  }
}