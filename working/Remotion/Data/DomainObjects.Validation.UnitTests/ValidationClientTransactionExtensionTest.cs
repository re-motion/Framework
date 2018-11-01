﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using FluentValidation.Validators;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Utilities;
using Remotion.Validation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class ValidationClientTransactionExtensionTest
  {
    private IValidatorBuilder _validatorBuilderMock;
    private ValidationClientTransactionExtension _extension;
    private IValidator _validatorMock1;
    private IValidator _validatorMock2;

    [SetUp]
    public void SetUp ()
    {
      _validatorBuilderMock = MockRepository.GenerateStrictMock<IValidatorBuilder>();
      _validatorMock1 = MockRepository.GenerateStrictMock<IValidator>();
      _validatorMock2 = MockRepository.GenerateStrictMock<IValidator>();

      _extension = new ValidationClientTransactionExtension (_validatorBuilderMock);
    }

    [Test]
    public void DefaultKey ()
    {
      Assert.That (ValidationClientTransactionExtension.DefaultKey, Is.EqualTo (typeof (ValidationClientTransactionExtension).FullName));
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_extension.ValidatorBuilder, Is.SameAs (_validatorBuilderMock));
    }

    [Test]
    public void Key ()
    {
      Assert.That (_extension.Key, Is.EqualTo (ValidationClientTransactionExtension.DefaultKey));
    }

    [Test]
    public void CommitValidate_WithoutValidationFailures ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject1 = DomainObjectWithoutAnnotatedProperties.NewObject();
        var domainObject2 = TestDomainObject.NewObject();
        var domainObject3 = DomainObjectWithoutAnnotatedProperties.NewObject();

        var persistableData1 = new PersistableData (
            domainObject1,
            StateType.New,
            DataContainerObjectMother.Create (domainObject1),
            new IRelationEndPoint[0]);
        var persistableData2 = new PersistableData (
            domainObject2,
            StateType.Changed,
            DataContainerObjectMother.Create (domainObject2),
            new IRelationEndPoint[0]);
        var persistableData3 = new PersistableData (
            domainObject3,
            StateType.Deleted,
            DataContainerObjectMother.Create (domainObject3),
            new IRelationEndPoint[0]);

        _validatorBuilderMock
            .Expect (mock => mock.BuildValidator (typeof (DomainObjectWithoutAnnotatedProperties)))
            .Return (_validatorMock1);
        _validatorBuilderMock
            .Expect (mock => mock.BuildValidator (typeof (TestDomainObject)))
            .Return (_validatorMock2);

        _validatorMock1.Expect (mock => mock.Validate (domainObject1)).Return (new ValidationResult());
        _validatorMock2.Expect (mock => mock.Validate (domainObject2)).Return (new ValidationResult());

        _extension.CommitValidate (ClientTransaction.Current, Array.AsReadOnly (new[] { persistableData1, persistableData2, persistableData3 }));

        _validatorBuilderMock.VerifyAllExpectations();
        _validatorMock1.VerifyAllExpectations();
        _validatorMock2.VerifyAllExpectations();
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

        var persistableData1 = new PersistableData (
            domainObject1,
            StateType.New,
            DataContainerObjectMother.Create (domainObject1),
            new IRelationEndPoint[0]);
        var persistableData2 = new PersistableData (
            domainObject2,
            StateType.Changed,
            DataContainerObjectMother.Create (domainObject2),
            new IRelationEndPoint[0]);
        var persistableData3 = new PersistableData (
            domainObject3,
            StateType.Deleted,
            DataContainerObjectMother.Create (domainObject3),
            new IRelationEndPoint[0]);

        _validatorBuilderMock
            .Expect (mock => mock.BuildValidator (typeof (DomainObjectWithoutAnnotatedProperties)))
            .Return (_validatorMock1);

        _validatorBuilderMock
            .Expect (mock => mock.BuildValidator (typeof (TestDomainObject)))
            .Return (_validatorMock2);

        var validationFailure1 = new ValidationFailure ("Test1", "Error1");
        validationFailure1.SetValidatedInstance (domainObject1);
        var validationFailure2 = new ValidationFailure ("Test2", "Error2");
        validationFailure2.SetValidatedInstance (domainObject1);
        var validationFailure3 = new ValidationFailure ("Test3", "Error3");

        _validatorMock1.Expect (mock => mock.Validate (domainObject1)).Repeat.Twice().Return (new ValidationResult (new[] { validationFailure1 }));
        _validatorMock2.Expect (mock => mock.Validate (domainObject2))
            .Repeat.Twice()
            .Return (new ValidationResult (new[] { validationFailure2, validationFailure3 }));

        var exception = Assert.Throws<DomainObjectFluentValidationException> (
            () => _extension.CommitValidate (
                ClientTransaction.Current,
                Array.AsReadOnly (new[] { persistableData1, persistableData2, persistableData3 })));

        string expectedMessage = @"One or more DomainObject contain inconsistent data:

Object 'DomainObjectWithoutAnnotatedProperties' with ID '.*':
 -- Test1: Error1
 -- Test2: Error2

Object 'TestDomainObject' with ID '.*':
 -- Test3: Error3

";
        Assert.That (exception.Message, Is.StringMatching (expectedMessage));

        Assert.That (new DomainObject[] { domainObject1, domainObject2 }, Is.EqualTo (exception.AffectedObjects));
        
        Assert.That (new[] { validationFailure1, validationFailure2, validationFailure3}, Is.EqualTo (exception.ValidationFailures.ToArray ()));

        _validatorBuilderMock.VerifyAllExpectations();
        _validatorMock1.VerifyAllExpectations();
        _validatorMock2.VerifyAllExpectations();
      }
    }

    [Test]
    public void CommitValidate_WithValidationFailuresWithCorrectLocalization ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject1 = DomainObjectWithoutAnnotatedProperties.NewObject();
        domainObject1.Name = null;

        var persistableData1 = new PersistableData (
            domainObject1,
            StateType.New,
            DataContainerObjectMother.Create (domainObject1),
            new IRelationEndPoint[0]);

        var propertyRule = PropertyRule.Create<DomainObjectWithoutAnnotatedProperties, string> (p => p.Name);
        propertyRule.AddValidator (new NotNullValidator());

        var rules = new List<IValidationRule>();
        rules.Add (propertyRule);

        Validator validator = new Validator (rules, typeof (DomainObjectWithoutAnnotatedProperties));

        _validatorBuilderMock
            .Expect (mock => mock.BuildValidator (typeof (DomainObjectWithoutAnnotatedProperties))).Repeat.Twice()
            .Return (validator);

        using (new CultureScope ("de"))
        {

          var exception = Assert.Throws<DomainObjectFluentValidationException> (
              () => _extension.CommitValidate (ClientTransaction.Current, Array.AsReadOnly (new[] { persistableData1 })));


          string expectedMessage = @"One or more DomainObject contain inconsistent data:

Object 'DomainObjectWithoutAnnotatedProperties' with ID '.*':
 -- Name: 'Name' must not be empty.
";

          Assert.That (exception.Message, Is.StringMatching (expectedMessage));

          Assert.That (exception.AffectedObjects.Length, Is.EqualTo (1));
          Assert.That (exception.AffectedObjects[0], Is.EqualTo (domainObject1));

          Assert.That (exception.ValidationFailures.Count, Is.EqualTo (1));
          Assert.That (exception.ValidationFailures.ToArray()[0].ErrorMessage, Is.EqualTo ("'Name' darf keinen Null-Wert aufweisen."));
        }
      }
    }
  }
}