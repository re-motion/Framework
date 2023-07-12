using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation.Factories
{
  [TestFixture]
  public class ValidationBusinessObjectBoundEditableWebControlValidatorFactoryTest
  {

    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    [TestCase(
        typeof(IBocTextValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocTextValue>),
        new[] { typeof(BocTextValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocReferenceValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocReferenceValue>),
        new[] { typeof(BocReferenceValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocAutoCompleteReferenceValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocAutoCompleteReferenceValue>),
        new[] { typeof(BocAutoCompleteReferenceValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocBooleanValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocBooleanValue>),
        new[] { typeof(BocBooleanValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocCheckBoxValidatorFactory),
        typeof(CompoundValidatorFactory<IBocCheckBox>),
        new[] { typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocDateTimeValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocDateTimeValue>),
        new[] { typeof(BocDateTimeValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocEnumValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocEnumValue>),
        new[] { typeof(BocEnumValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase(
        typeof(IBocMultilineTextValueValidatorFactory),
        typeof(CompoundValidatorFactory<IBocMultilineTextValue>),
        new[] { typeof(BocMultilineTextValueValidatorFactory), typeof(ValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    // [TestCase(
    //     typeof(IUserControlBindingValidatorFactory),
    //     typeof(CompoundValidatorFactory<UserControlBinding>),
    //     new[] { typeof(FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    public void GetInstance_Once (Type serviceType, Type targetType, Type[] expectedInnerFactoryTypesOrdered)
    {
      var instance = _serviceLocator.GetInstance(serviceType);

      Assert.That(instance, Is.InstanceOf(targetType));

      var factories = (instance.GetType().GetProperty("VlidatorFactories").GetValue(instance) as IList);
      object[] factoriesArray = new object[factories.Count];
      factories.CopyTo(factoriesArray, 0);

      Assert.That(factories, Is.Not.Null);
      Assert.That(factoriesArray.Select(o => o.GetType()), Is.EqualTo(expectedInnerFactoryTypesOrdered));
    }

    [Test]
    [TestCase(typeof(IBocTextValueValidatorFactory), typeof(CompoundValidatorFactory<IBocTextValue>))]
    [TestCase(typeof(IBocReferenceValueValidatorFactory), typeof(CompoundValidatorFactory<IBocReferenceValue>))]
    [TestCase(typeof(IBocAutoCompleteReferenceValueValidatorFactory), typeof(CompoundValidatorFactory<IBocAutoCompleteReferenceValue>))]
    [TestCase(typeof(IBocBooleanValueValidatorFactory), typeof(CompoundValidatorFactory<IBocBooleanValue>))]
    [TestCase(typeof(IBocCheckBoxValidatorFactory), typeof(CompoundValidatorFactory<IBocCheckBox>))]
    [TestCase(typeof(IBocDateTimeValueValidatorFactory), typeof(CompoundValidatorFactory<IBocDateTimeValue>))]
    [TestCase(typeof(IBocEnumValueValidatorFactory), typeof(CompoundValidatorFactory<IBocEnumValue>))]
    [TestCase(typeof(IBocMultilineTextValueValidatorFactory), typeof(CompoundValidatorFactory<IBocMultilineTextValue>))]
   // [TestCase (typeof (IUserControlBindingValidatorFactory), typeof (CompoundValidatorFactory<UserControlBinding>))]
    public void GetInstance_Twice_ReturnsSameInstance (Type serviceType, Type targetType)
    {
      var instance1 = _serviceLocator.GetInstance(serviceType);
      var instance2 = _serviceLocator.GetInstance(serviceType);

      Assert.That(instance1, Is.InstanceOf(targetType));
      Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocTextValue (bool isReadOnly)
    {
      var mock = new Mock<IBocTextValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocReferenceValue (bool isReadOnly)
    {
      var mock = new Mock<IBocReferenceValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocAutoCompleteReferenceValue (bool isReadOnly)
    {
      var mock = new Mock<IBocAutoCompleteReferenceValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocBooleanValue (bool isReadOnly)
    {
      var mock = new Mock<IBocBooleanValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocCheckBox (bool isReadOnly)
    {
      var mock = new Mock<IBocCheckBox>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocDateTimeValue (bool isReadOnly)
    {
      var mock = new Mock<IBocDateTimeValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocEnumValue (bool isReadOnly)
    {
      var mock = new Mock<IBocEnumValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void CreateValidators_IBocMultilineTextValue (bool isReadOnly)
    {
      var mock = new Mock<IBocMultilineTextValue>();
      mock.Setup(m => m.ID).Returns("ID").Verifiable();

      var factory = new ValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators(mock.Object, isReadOnly);
      CheckValidators(validators);
    }

    private void CheckValidators (IEnumerable<BaseValidator> validators)
    {
      var validatorsArray = validators.ToArray();

      Assert.That(
          validatorsArray.Select(v => v.GetType()),
          Is.EquivalentTo(new[] { typeof(BusinessObjectBoundEditableWebControlValidationResultDispatchingValidator) }));
      Assert.That(validatorsArray, Has.All.Property("EnableViewState").False);
    }
  }
}
