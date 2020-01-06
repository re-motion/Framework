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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.Reflection;
using Remotion.Validation.Results;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests
{
  [TestFixture]
  public class BusinessObjectBoundEditableWebControlValidationUtilityTest
  {
    [Test]
    public void IsMatchingControl_HasValidBinding_False ()
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (false);

      var validatedObject = MockRepository.GenerateMock<IBusinessObject>();

      var property = MockRepository.GenerateStub<IPropertyInformation>();
      property.Stub (_ => _.Name).Return ("PropertyStub");

      var failure = new PropertyValidationFailure (validatedObject, property, "value", "Error", "ValidationMessage");
      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.False);
      control.VerifyAllExpectations();
    }

    [Test]
    public void IsMatchingControl_NotPropertyValidationFailure_False ()
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (false);

      var validatedObject = MockRepository.GenerateMock<IBusinessObject>();

      var property = MockRepository.GenerateStub<IPropertyInformation>();
      property.Stub (_ => _.Name).Return ("PropertyStub");

      var failure = new ObjectValidationFailure (validatedObject, "Error", "ValidationMessage");
      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.False);
      control.VerifyAllExpectations();
    }

    [Test]
    public void IsMatchingControl_DifferentBusinessObject ()
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (true);
      control.DataSource = MockRepository.GenerateMock<IBusinessObjectDataSource>();
      var otherBusinessObject = MockRepository.GenerateMock<IBusinessObject>();
      control.Expect (c => c.DataSource.BusinessObject).Return (otherBusinessObject);

      var validatedObject = MockRepository.GenerateStub<IBusinessObject>();

      var property = MockRepository.GenerateStub<IPropertyInformation>();
      property.Stub (_ => _.Name).Return ("PropertyStub");

      var failure = new PropertyValidationFailure (validatedObject, property, "value", "Error", "ValidationMessage");

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.False);
      control.VerifyAllExpectations();
    }

    [Test]
    public void IsMatchingControl_DifferentProperty ()
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (true);

      control.DataSource = MockRepository.GenerateMock<IBusinessObjectDataSource>();
      var validatedObject = MockRepository.GenerateMock<IBusinessObject>();
      control.Expect (c => c.DataSource.BusinessObject).Return (validatedObject);

      var property = MockRepository.GenerateStub<IPropertyInformation>();
      property.Stub (_ => _.Name).Return ("PropertyStub");

      var businessObjectProperty = MockRepository.GenerateMock<IBusinessObjectStringProperty>();
      businessObjectProperty.Expect (p => p.Identifier).Return ("PropertyIdentifier");
      control.Expect (c => c.Property).Return (businessObjectProperty);

      var failure = new PropertyValidationFailure (validatedObject, property, "value", "Error", "ValidationMessage");

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.False);
      control.VerifyAllExpectations();
    }

    [Test]
    [TestCase (false, true)]
    [TestCase (true, false)]
    public void IsMatchingControl_PropertyMatch (bool shouldBusinessObjectBeNull, bool isMatch)
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (true);

      var validatedObject = MockRepository.GenerateMock<IBusinessObject>();

      var businessObjectProperty = MockRepository.GenerateMock<IBusinessObjectStringProperty>();
      businessObjectProperty.Expect (p => p.Identifier).Return ("PropertyStub");
      control.Expect (c => c.Property).Return (businessObjectProperty);

      var property = MockRepository.GenerateStub<IPropertyInformation>();
      property.Stub (_ => _.Name).Return ("PropertyStub");

      var failure = new PropertyValidationFailure (validatedObject, property, "value", "Error", "ValidationMessage");

      SetDataSource (shouldBusinessObjectBeNull, control, validatedObject);

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.EqualTo (isMatch));
      control.VerifyAllExpectations();
    }

    [Test]
    [TestCase (false, true)]
    [TestCase (true, false)]
    public void IsMatchingControl_PropertyMatchShort (bool shouldBusinessObjectBeNull, bool isMatch)
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (true);

      var validatedObject = MockRepository.GenerateMock<IBusinessObject>();

      var businessObjectProperty = MockRepository.GenerateMock<IBusinessObjectStringProperty>();
      businessObjectProperty.Expect (p => p.Identifier).Return ("PropertyStub");
      control.Expect (c => c.Property).Return (businessObjectProperty);

      var property = MockRepository.GenerateStub<IPropertyInformation>();
      property.Stub (_ => _.Name).Return ("PropertyStub");

      var failure = new PropertyValidationFailure (validatedObject, property, "value", "Error", "ValidationMessage");

      SetDataSource (shouldBusinessObjectBeNull, control, validatedObject);

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.EqualTo (isMatch));
      control.VerifyAllExpectations();
    }

    private static void SetDataSource (
        bool shouldBusinessObjectBeNull,
        IBusinessObjectBoundEditableWebControl control,
        IBusinessObject businessObject)
    {
      if (shouldBusinessObjectBeNull)
        control.Expect (c => c.DataSource).Return (null);
      else
      {
        var dataSource = MockRepository.GenerateMock<IBusinessObjectDataSource>();
        dataSource.Expect (c => c.BusinessObject).Return (businessObject);
        control.Expect (c => c.DataSource).Return (dataSource);
      }
    }
  }
}