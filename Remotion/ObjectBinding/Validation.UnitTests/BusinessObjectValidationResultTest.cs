using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;
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
      _personValidator = _validatorBuilder.BuildValidator (typeof (Person));

      _personClass = BindableObjectProvider.GetProviderForBindableObjectType (typeof (Person)).GetBindableObjectClass (typeof (Person));
      _firstNameBusinessObjectProperty = Assertion.IsNotNull (_personClass.GetPropertyDefinition ("FirstName"));
      _lastNameBusinessObjectProperty = Assertion.IsNotNull (_personClass.GetPropertyDefinition ("LastName"));
      _phoneNumberBusinessObjectProperty = Assertion.IsNotNull (_personClass.GetPropertyDefinition ("PhoneNumber"));
      _customerNumberBusinessObjectProperty = Assertion.IsNotNull (_personClass.GetPropertyDefinition ("CustomerNumber"));

      _firstNameProperty = PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((Person _) => _.FirstName));
      _lastNameProperty = PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((Person _) => _.LastName));
      _phoneNumberProperty = PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((Person _) => _.PhoneNumber));
      _phoneNumberInterfaceProperty = PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((ICustomer _) => _.PhoneNumber));
      _customerNumberInterfaceProperty = PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty ((ICustomer _) => _.CustomerNumber));
    }

    [Test]
    public void Create ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "";
      person.PhoneNumber = null;
      ((ICustomer) person).CustomerNumber = "";

      var validationResult = _personValidator.Validate (person);
      Assert.That (validationResult.IsValid, Is.False);
      Assert.That (validationResult.Errors.Count, Is.EqualTo (4));
      Assert.That (
          validationResult.Errors.OfType<PropertyValidationFailure>().Select (f => f.ValidatedProperty).Distinct().Count(),
          Is.EqualTo (4));

      var businessObjectValidationResult = BusinessObjectValidationResult.Create (validationResult);

      var firstNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures (person, _firstNameBusinessObjectProperty, true).ToArray();
      Assert.That (firstNameValidationFailures.Length, Is.EqualTo (1));
      Assert.That (firstNameValidationFailures[0].ValidatedObject, Is.SameAs (person));
      Assert.That (firstNameValidationFailures[0].ValidatedProperty, Is.SameAs (_firstNameBusinessObjectProperty));
      Assert.That (firstNameValidationFailures[0].ErrorMessage, Is.EqualTo ("NotNullValidator: Validation error."));

      var lastNameValidationFailures =
          businessObjectValidationResult.GetValidationFailures (person, _lastNameBusinessObjectProperty, true).ToArray();
      Assert.That (lastNameValidationFailures.Length, Is.EqualTo (1));
      Assert.That (lastNameValidationFailures[0].ValidatedObject, Is.SameAs (person));
      Assert.That (lastNameValidationFailures[0].ValidatedProperty, Is.SameAs (_lastNameBusinessObjectProperty));
      Assert.That (lastNameValidationFailures[0].ErrorMessage, Is.EqualTo ("NotEmptyValidator: Validation error."));

      var phoneNumberValidationFailures =
          businessObjectValidationResult.GetValidationFailures (person, _phoneNumberBusinessObjectProperty, true).ToArray();
      Assert.That (phoneNumberValidationFailures.Length, Is.EqualTo (1));
      Assert.That (phoneNumberValidationFailures[0].ValidatedObject, Is.SameAs (person));
      Assert.That (phoneNumberValidationFailures[0].ValidatedProperty, Is.SameAs (_phoneNumberBusinessObjectProperty));
      Assert.That (phoneNumberValidationFailures[0].ErrorMessage, Is.EqualTo ("NotNullValidator: Validation error."));

      var customerNumberValidationFailures =
          businessObjectValidationResult.GetValidationFailures (person, _customerNumberBusinessObjectProperty, true).ToArray();
      Assert.That (customerNumberValidationFailures.Length, Is.EqualTo (1));
      Assert.That (customerNumberValidationFailures[0].ValidatedObject, Is.SameAs (person));
      Assert.That (customerNumberValidationFailures[0].ValidatedProperty, Is.SameAs (_customerNumberBusinessObjectProperty));
      Assert.That (customerNumberValidationFailures[0].ErrorMessage, Is.EqualTo ("NotEmptyValidator: Validation error."));
    }
  }
}