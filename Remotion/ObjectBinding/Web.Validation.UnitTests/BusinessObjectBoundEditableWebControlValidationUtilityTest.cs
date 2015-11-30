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
using FluentValidation.Results;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
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

      var failure = MockRepository.GenerateMock<ValidationFailure> ("Property", "Error");
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
      control.Expect (c => c.DataSource.BusinessObject).Return (MockRepository.GenerateMock<IBusinessObject>());

      var failure = MockRepository.GenerateMock<ValidationFailure> ("Property", "Error");
      failure.CustomState = new BindableObjectDataSource();

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
      var businessObject = MockRepository.GenerateMock<IBusinessObject>();
      control.Expect (c => c.DataSource.BusinessObject).Return (businessObject);

      var property = MockRepository.GenerateMock<IBusinessObjectStringProperty>();
      property.Expect (p => p.Identifier).Return ("PropertyIdentifier");
      control.Expect (c => c.Property).Return (property);

      var failure = MockRepository.GenerateMock<ValidationFailure> ("Property", "Error");
      failure.CustomState = businessObject;

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.False);
      control.VerifyAllExpectations();
    }

    [Test]
    [TestCase (false, false, true)]
    [TestCase (false, true, true)]
    [TestCase (true, false, false)]
    [TestCase (true, true, true)]
    public void IsMatchingControl_PropertyMatch (bool shouldBusinessObjectBeNull, bool shouldValidatedInstanceBeNull, bool isMatch)
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (true);

      var property = MockRepository.GenerateMock<IBusinessObjectStringProperty>();
      property.Expect (p => p.Identifier).Return ("Property");
      control.Expect (c => c.Property).Return (property);

      var failure = MockRepository.GenerateMock<ValidationFailure> ("Property", "Error");

      SetDataSource (shouldBusinessObjectBeNull, shouldValidatedInstanceBeNull, control, failure);

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.EqualTo (isMatch));
      control.VerifyAllExpectations();
    }

    [Test]
    [TestCase (false, false, true)]
    [TestCase (false, true, true)]
    [TestCase (true, false, false)]
    [TestCase (true, true, true)]
    public void IsMatchingControl_PropertyMatchShort (bool shouldBusinessObjectBeNull, bool shouldValidatedInstanceBeNull, bool isMatch)
    {
      var control = MockRepository.GenerateMock<IBusinessObjectBoundEditableWebControl>();
      control.Expect (c => c.HasValidBinding).Return (true);

      var property = MockRepository.GenerateMock<IBusinessObjectStringProperty>();
      property.Expect (p => p.Identifier).Return ("Property");
      control.Expect (c => c.Property).Return (property);

      var failure = MockRepository.GenerateMock<ValidationFailure> ("Object.Property", "Error");

      SetDataSource (shouldBusinessObjectBeNull, shouldValidatedInstanceBeNull, control, failure);

      var isMatching = BusinessObjectBoundEditableWebControlValidationUtility.IsMatchingControl (control, failure);

      Assert.That (isMatching, Is.EqualTo (isMatch));
      control.VerifyAllExpectations();
    }

    private static void SetDataSource (
        bool shouldBusinessObjectBeNull,
        bool shouldValidatedInstanceBeNull,
        IBusinessObjectBoundEditableWebControl control,
        ValidationFailure failure)
    {
      var businessObject = MockRepository.GenerateMock<IBusinessObject>();

      if (shouldBusinessObjectBeNull)
        control.Expect (c => c.DataSource).Return (null);
      else
      {
        var dataSource = MockRepository.GenerateMock<IBusinessObjectDataSource>();
        dataSource.Expect (c => c.BusinessObject).Return (businessObject);
        control.Expect (c => c.DataSource).Return (dataSource);
      }

      if (shouldValidatedInstanceBeNull)
        failure.CustomState = null;
      else
        failure.CustomState = businessObject;
    }
  }
}