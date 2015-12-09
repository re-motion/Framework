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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories.Decorators;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocDateTimeValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    [TestCase (true, true, new[] { typeof (BusinessObjectBoundEditableWebControlValidator), typeof (BocDateTimeRequiredValidator), typeof (BocDateTimeFormatValidator) })]
    [TestCase (true, false, new[] { typeof (BusinessObjectBoundEditableWebControlValidator), typeof (BocDateTimeFormatValidator) })]
    [TestCase (false, true, new[] { typeof (BusinessObjectBoundEditableWebControlValidator), typeof (BocDateTimeFormatValidator) })]
    [TestCase (false, false, new[] { typeof (BusinessObjectBoundEditableWebControlValidator), typeof (BocDateTimeFormatValidator) })]
    public void CreateValidators (bool required, bool isValueType, Type[] expectedValidatorTypes)
    {
      var compoundFactory =
          new CompoundBocDateTimeValueValidatorFactory (
              new IBocDateTimeValueValidatorFactory[]
              { new BocDateTimeValueValidatorFactory(), new FluentValidationBusinessObjectBoundEditableWebControlValidatorFactory() });
      var factory = new FilteringBocDateTimeValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocDateTimeValue>();
      control.Expect (c => c.IsRequired).Return (true);
      control.Expect (c => c.Property).Return (GetPropertyStub (required, isValueType));
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (
              expectedValidatorTypes));
    }
  }
}