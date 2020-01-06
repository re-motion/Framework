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
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories.Decorators;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocEnumValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    [TestCase (true, true, new[] { typeof (BusinessObjectBoundEditableWebControlValidator), typeof (RequiredFieldValidator) })]
    [TestCase (true, false, new[] { typeof (BusinessObjectBoundEditableWebControlValidator) })]
    [TestCase (false, true, new[] { typeof (BusinessObjectBoundEditableWebControlValidator) })]
    [TestCase (false, false, new[] { typeof (BusinessObjectBoundEditableWebControlValidator) })]
    public void CreateValidators (bool required, bool isValueType, Type[] expectedValidatorTypes)
    {
      var compoundFactory =
          new CompoundBocEnumValueValidatorFactory (
              new IBocEnumValueValidatorFactory[]
              { new BocEnumValueValidatorFactory(), new ValidationBusinessObjectBoundEditableWebControlValidatorFactory() });
      var factory = new FilteringBocEnumValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocEnumValue>();
      control.Expect (c => c.IsRequired).Return (true);
      control.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });
      control.Expect (c => c.Property).Return (GetPropertyStub (required, isValueType));
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (expectedValidatorTypes));
    }
  }
}