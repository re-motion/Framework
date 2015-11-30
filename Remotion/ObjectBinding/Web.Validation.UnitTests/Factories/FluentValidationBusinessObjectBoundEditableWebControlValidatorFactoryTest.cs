using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
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
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories
{
  [TestFixture]
  public class FluentValidationBusinessObjectBoundEditableWebControlValidatorFactoryTest
  {

    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create ();
    }

    [Test]
    [TestCase (typeof (IBocTextValueValidatorFactory), typeof (CompoundValidatorFactory<IBocTextValue>), new[] { typeof (BocTextValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocReferenceValueValidatorFactory), typeof (CompoundValidatorFactory<IBocReferenceValue>), new[] { typeof (BocReferenceValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocAutoCompleteReferenceValueValidatorFactory), typeof (CompoundValidatorFactory<IBocAutoCompleteReferenceValue>), new[] { typeof (BocAutoCompleteReferenceValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocBooleanValueValidatorFactory), typeof (CompoundValidatorFactory<IBocBooleanValue>), new[] { typeof (BocBooleanValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocCheckBoxValidatorFactory), typeof (CompoundValidatorFactory<IBocCheckBox>), new[] { typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocDateTimeValueValidatorFactory), typeof (CompoundValidatorFactory<IBocDateTimeValue>), new[] { typeof (BocDateTimeValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocEnumValueValidatorFactory), typeof (CompoundValidatorFactory<IBocEnumValue>), new[] { typeof (BocEnumValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    [TestCase (typeof (IBocMultilineTextValueValidatorFactory), typeof (CompoundValidatorFactory<IBocMultilineTextValue>), new[] { typeof (BocMultilineTextValueValidatorFactory), typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    //[TestCase (typeof (IUserControlBindingValidatorFactory), typeof (CompoundValidatorFactory<UserControlBinding>), new[] { typeof (FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory) })]
    public void GetInstance_Once (Type serviceType, Type targetType, Type[] expectedInnerFactoryTypesOrdered)
    {
      var instance = _serviceLocator.GetInstance (serviceType);

      Assert.That (instance, Is.InstanceOf (targetType));

      var factories = (instance.GetType().GetProperty ("VlidatorFactories").GetValue (instance) as IList);
      object[] factoriesArray = new object[factories.Count];
      factories.CopyTo (factoriesArray, 0);

      Assert.That (factories, Is.Not.Null);
      Assert.That (factoriesArray.Select (o => o.GetType()), Is.EqualTo (expectedInnerFactoryTypesOrdered));
    }

    [Test]
    [TestCase (typeof (IBocTextValueValidatorFactory), typeof (CompoundValidatorFactory<IBocTextValue>))]
    [TestCase (typeof (IBocReferenceValueValidatorFactory), typeof (CompoundValidatorFactory<IBocReferenceValue>))]
    [TestCase (typeof (IBocAutoCompleteReferenceValueValidatorFactory), typeof (CompoundValidatorFactory<IBocAutoCompleteReferenceValue>))]
    [TestCase (typeof (IBocBooleanValueValidatorFactory), typeof (CompoundValidatorFactory<IBocBooleanValue>))]
    [TestCase (typeof (IBocCheckBoxValidatorFactory), typeof (CompoundValidatorFactory<IBocCheckBox>))]
    [TestCase (typeof (IBocDateTimeValueValidatorFactory), typeof (CompoundValidatorFactory<IBocDateTimeValue>))]
    [TestCase (typeof (IBocEnumValueValidatorFactory), typeof (CompoundValidatorFactory<IBocEnumValue>))]
    [TestCase (typeof (IBocMultilineTextValueValidatorFactory), typeof (CompoundValidatorFactory<IBocMultilineTextValue>))]
   // [TestCase (typeof (IUserControlBindingValidatorFactory), typeof (CompoundValidatorFactory<UserControlBinding>))]
    public void GetInstance_Twice_ReturnsSameInstance (Type serviceType, Type targetType)
    {
      var instance1 = _serviceLocator.GetInstance (serviceType);
      var instance2 = _serviceLocator.GetInstance (serviceType);

      Assert.That (instance1, Is.InstanceOf (targetType));
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocTextValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocTextValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocReferenceValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocReferenceValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocAutoCompleteReferenceValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocAutoCompleteReferenceValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocBooleanValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocBooleanValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocCheckBox (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocCheckBox>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocDateTimeValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocDateTimeValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocEnumValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocEnumValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    [Test]
    [TestCase (true)]
    [TestCase (false)]
    public void CreateValidators_IBocMultilineTextValue (bool isReadOnly)
    {
      var mock = MockRepository.GenerateMock<IBocMultilineTextValue>();
      mock.Expect (m => m.ID).Return ("ID");

      var factory = new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory();
      var validators = factory.CreateValidators (mock, isReadOnly);
      CheckValidators (isReadOnly, validators);
    }

    private void CheckValidators (bool isReadOnly, IEnumerable<BaseValidator> validators)
    {
      if (isReadOnly)
        Assert.That (validators, Is.Empty);
      else
        Assert.That (validators.Select (v => v.GetType ()), Is.EquivalentTo (new[] { typeof (BusinessObjectBoundEditableWebControlValidator) }));
    }
  }
}