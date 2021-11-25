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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class ValidationClientTransactionExtensionTest
  {
    private Mock<IValidatorProvider> _validatorProviderMock;
    private ValidationClientTransactionExtension _extension;
    private Mock<IValidator> _validatorMock1;
    private Mock<IValidator> _validatorMock2;

    [SetUp]
    public void SetUp ()
    {
      _validatorProviderMock = new Mock<IValidatorProvider>(MockBehavior.Strict);
      _validatorMock1 = new Mock<IValidator>(MockBehavior.Strict);
      _validatorMock2 = new Mock<IValidator>(MockBehavior.Strict);

      _extension = new ValidationClientTransactionExtension(_validatorProviderMock.Object);
    }

    [Test]
    public void DefaultKey ()
    {
      Assert.That(ValidationClientTransactionExtension.DefaultKey, Is.EqualTo(typeof(ValidationClientTransactionExtension).FullName));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_extension.ValidatorProvider, Is.SameAs(_validatorProviderMock.Object));
    }

    [Test]
    public void Key ()
    {
      Assert.That(_extension.Key, Is.EqualTo(ValidationClientTransactionExtension.DefaultKey));
    }

    [Test]
    public void CommitValidate_WithoutValidationFailures ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject1 = DomainObjectWithoutAnnotatedProperties.NewObject();
        var domainObject2 = TestDomainObject.NewObject();
        var domainObject3 = DomainObjectWithoutAnnotatedProperties.NewObject();

        var persistableData1 = new PersistableData(
            domainObject1,
            new DomainObjectState.Builder().SetNew().Value,
            DataContainerObjectMother.Create(domainObject1),
            new IRelationEndPoint[0]);
        var persistableData2 = new PersistableData(
            domainObject2,
            new DomainObjectState.Builder().SetChanged().Value,
            DataContainerObjectMother.Create(domainObject2),
            new IRelationEndPoint[0]);
        var persistableData3 = new PersistableData(
            domainObject3,
            new DomainObjectState.Builder().SetDeleted().Value,
            DataContainerObjectMother.Create(domainObject3),
            new IRelationEndPoint[0]);

        _validatorProviderMock
            .Setup(mock => mock.GetValidator(typeof(DomainObjectWithoutAnnotatedProperties)))
            .Returns(_validatorMock1.Object)
            .Verifiable();
        _validatorProviderMock
            .Setup(mock => mock.GetValidator(typeof(TestDomainObject)))
            .Returns(_validatorMock2.Object)
            .Verifiable();

        _validatorMock1.Setup(mock => mock.Validate(domainObject1)).Returns(new ValidationResult()).Verifiable();
        _validatorMock2.Setup(mock => mock.Validate(domainObject2)).Returns(new ValidationResult()).Verifiable();

        _extension.CommitValidate(ClientTransaction.Current, Array.AsReadOnly(new[] { persistableData1, persistableData2, persistableData3 }));

        _validatorProviderMock.Verify();
        _validatorMock1.Verify();
        _validatorMock2.Verify();
      }
    }

    [Test]
    public void CommitValidate_WithValidationFailures ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject1 = DomainObjectWithoutAnnotatedProperties.NewObject();
        var domainObject2 = TestDomainObject.NewObject();
        var domainObject3 = DomainObjectWithoutAnnotatedProperties.NewObject();

        var persistableData1 = new PersistableData(
            domainObject1,
            new DomainObjectState.Builder().SetNew().Value,
            DataContainerObjectMother.Create(domainObject1),
            new IRelationEndPoint[0]);
        var persistableData2 = new PersistableData(
            domainObject2,
            new DomainObjectState.Builder().SetChanged().Value,
            DataContainerObjectMother.Create(domainObject2),
            new IRelationEndPoint[0]);
        var persistableData3 = new PersistableData(
            domainObject3,
            new DomainObjectState.Builder().SetDeleted().Value,
            DataContainerObjectMother.Create(domainObject3),
            new IRelationEndPoint[0]);

        _validatorProviderMock
            .Setup(mock => mock.GetValidator(typeof(DomainObjectWithoutAnnotatedProperties)))
            .Returns(_validatorMock1.Object)
            .Verifiable();

        _validatorProviderMock
            .Setup(mock => mock.GetValidator(typeof(TestDomainObject)))
            .Returns(_validatorMock2.Object)
            .Verifiable();

        var propertyStub1 = new Mock<IPropertyInformation>();
        propertyStub1.Setup(_ => _.Name).Returns("PropertyStub1");
        var propertyStub2 = new Mock<IPropertyInformation>();
        propertyStub2.Setup(_ => _.Name).Returns("PropertyStub2");
        var propertyStub4 = new Mock<IPropertyInformation>();
        propertyStub4.Setup(_ => _.Name).Returns("PropertyStub4");

        var validationFailure1 = new PropertyValidationFailure(domainObject2, propertyStub1.Object, "value", "Error1", "ValidationMessage1");
        var validationFailure2 = new PropertyValidationFailure(domainObject1, propertyStub2.Object, null, "Error2", "ValidationMessage2");
        var validationFailure3 = new ObjectValidationFailure(domainObject2, "Error3", "ValidationMessage3");
        var validationFailure4 = new PropertyValidationFailure(domainObject2, propertyStub4.Object, null, "Error4", "ValidationMessage3");
        var validationFailure5 = new PropertyValidationFailure(domainObject2, propertyStub1.Object, null, "Error5", "ValidationMessage5");

        _validatorMock1.Setup(mock => mock.Validate(domainObject1)).Returns(new ValidationResult(new[] { validationFailure2 })).Verifiable();
        _validatorMock2.Setup(mock => mock.Validate(domainObject2))
            .Returns(new ValidationResult(new ValidationFailure[] { validationFailure1, validationFailure3, validationFailure4, validationFailure5 }))
            .Verifiable();

        var exception = Assert.Throws<ExtendedDomainObjectValidationException>(
            () => _extension.CommitValidate(
                ClientTransaction.Current,
                Array.AsReadOnly(new[] { persistableData1, persistableData2, persistableData3 })));

        string expectedMessage = @"One or more DomainObject contain inconsistent data:

Object 'DomainObjectWithoutAnnotatedProperties' with ID '.*':
 -- PropertyStub2: Error2

Object 'TestDomainObject' with ID '.*':
 -- PropertyStub1: Error1
 -- PropertyStub1: Error5
 -- PropertyStub4: Error4
 -- Error3
";
        Assert.That(exception.Message, Does.Match(expectedMessage));

        Assert.That(
            exception.AffectedObjects,
            Is.EquivalentTo(new DomainObject[] { domainObject1, domainObject2 }));

        Assert.That(
            exception.ValidationFailures,
            Is.EquivalentTo(new ValidationFailure[] { validationFailure1, validationFailure2, validationFailure3, validationFailure4, validationFailure5 }));

        _validatorProviderMock.Verify();
        _validatorMock1.Verify();
        _validatorMock2.Verify();
      }
    }

    [Test]
    public void CommitValidate_WithValidationFailuresWithCorrectLocalization ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject1 = DomainObjectWithoutAnnotatedProperties.NewObject();
        domainObject1.Name = null;

        var persistableData1 = new PersistableData(
            domainObject1,
            new DomainObjectState.Builder().SetNew().Value,
            DataContainerObjectMother.Create(domainObject1),
            new IRelationEndPoint[0]);

        var culture = CultureInfo.GetCultureInfo("fr");
        var uiCulture = CultureInfo.GetCultureInfo("it");

        var validationMessageStub = new Mock<ValidationMessage>();
        validationMessageStub
                .Setup(
                        _ => _.Format(
                                It.IsAny<CultureInfo>(),
                                It.IsAny<IFormatProvider>(),
                                It.IsAny<object[]>()))
                .Returns(() => $"Localized Message (uiCulture: {CultureInfo.CurrentUICulture}, culture: {CultureInfo.CurrentCulture})");

        var propertyRule = new PropertyValidationRule<DomainObjectWithoutAnnotatedProperties, string>(
            PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((DomainObjectWithoutAnnotatedProperties p) => p.Name)),
            o => ((DomainObjectWithoutAnnotatedProperties)o).Name,
            _ => true,
            new[] { new NotNullValidator(validationMessageStub.Object) });

        var rules = new List<IValidationRule>();
        rules.Add(propertyRule);

        Validator validator = new Validator(rules, typeof(DomainObjectWithoutAnnotatedProperties));

        _validatorProviderMock
            .Setup(mock => mock.GetValidator(typeof(DomainObjectWithoutAnnotatedProperties)))
            .Returns(validator)
            .Verifiable();

        using (new CultureScope(culture, uiCulture))
        {
          var exception = Assert.Throws<ExtendedDomainObjectValidationException>(
              () => _extension.CommitValidate(ClientTransaction.Current, Array.AsReadOnly(new[] { persistableData1 })));


          string expectedMessage = @"One or more DomainObject contain inconsistent data:

Object 'DomainObjectWithoutAnnotatedProperties' with ID '.*':
 -- Name: The value must not be null.
";

          Assert.That(exception.Message, Does.Match(expectedMessage));

          Assert.That(exception.AffectedObjects.Length, Is.EqualTo(1));
          Assert.That(exception.AffectedObjects[0], Is.EqualTo(domainObject1));

          Assert.That(exception.ValidationFailures.Length, Is.EqualTo(1));

          var validationFailures = exception.ValidationFailures.ToArray();
          Assert.That(validationFailures, Is.All.InstanceOf<PropertyValidationFailure>());
          Assert.That(((PropertyValidationFailure)validationFailures[0]).ValidatedProperty.Name, Is.EqualTo("Name"));
          Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be null."));
          Assert.That(validationFailures[0].LocalizedValidationMessage, Is.EqualTo("Localized Message (uiCulture: it, culture: fr)"));
        }
      }
    }
  }
}
