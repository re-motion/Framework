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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocDateTimeValueValidatorTest
  {
    private const string ValidDate = "01.02.2003";
    private const string ValidTime = "13:37";

    private const string MissingDateAndTimeErrorMessage = "MissingDateAndTimeErrorMessage";
    private const string MissingDateOrTimeErrorMessage = "MissingDateOrTimeErrorMessage";
    private const string MissingDateErrorMessage = "MissingDateErrorMessage";
    private const string MissingTimeErrorMessage = "MissingTimeErrorMessage";
    private const string InvalidDateAndTimeErrorMessage = "InvalidDateAndTimeErrorMessage";
    private const string InvalidDateErrorMessage = "InvalidDateErrorMessage";
    private const string InvalidTimeErrorMessage = "InvalidTimeErrorMessage";

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { MissingDateOrTimeErrorMessage })]
    [TestCase (BocDateTimeValueType.Date, new[] { MissingDateErrorMessage })]
    [TestCase (BocDateTimeValueType.DateTime, new[] { MissingDateAndTimeErrorMessage })]
    public void Validate_Required_DateAndTimeMissing (BocDateTimeValueType valueType, string[] expectedError)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue ("", "", true, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.False);
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new string[0], true)]
    [TestCase (BocDateTimeValueType.Date, new[] { MissingDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { MissingDateErrorMessage }, false)]
    public void Validate_Required_DateMissing (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue ("", ValidTime, true, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new string[0], true)]
    [TestCase (BocDateTimeValueType.Date, new[] { MissingDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { MissingDateErrorMessage }, false)]
    public void Validate_NotRequired_DateMissing (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue ("", ValidTime, false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new string[0], true)]
    [TestCase (BocDateTimeValueType.Date, new string[0], true)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { MissingTimeErrorMessage }, false)]
    public void Validate_Required_TimeMissing (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue (ValidDate, "", true, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidDateAndTimeErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidDateAndTimeErrorMessage }, false)]
    public void Validate_InvalidDateAndTime (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue ("invalid", "invalid", false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (-2, new string[0], true)]
    public void Validate_InvalidDate (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue ("invalid", ValidTime, false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidDateErrorMessage }, false)]
    public void Validate_InvalidDate2 (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        // ValidDate + " " + ValidTime would be a valid date but is entered in date field -> invalid
        var bocDateTimeValue = CreateBocDateTimeValue (ValidDate + " " + ValidTime, ValidTime, false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidDateErrorMessage }, false)]
    public void Validate_InvalidDate3 (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        // ValidDate + " " + ValidTime would be a valid date but is entered in date field -> invalid
        var bocDateTimeValue = CreateBocDateTimeValue ("01.01.10000", ValidTime, false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidTimeErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new string[0], true)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidTimeErrorMessage }, false)]
    public void Validate_InvalidTime (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue (ValidDate, "invalid", false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidTimeErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new string[0], true)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidTimeErrorMessage }, false)]
    public void Validate_InvalidTime2 (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue (ValidDate, ValidDate + " " + ValidTime, false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    [Test]
    public void Validate_NoBocDateTimeValue ()
    {
      var control = new BocTextValue();
      var validators = CreateValidator (control);

      List<string> errors;
      Assert.Throws<HttpException> (() => Validate (validators, out errors));
    }

    [Test]
    [TestCase (BocDateTimeValueType.Undefined, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.Date, new[] { InvalidDateErrorMessage }, false)]
    [TestCase (BocDateTimeValueType.DateTime, new[] { InvalidDateErrorMessage }, false)]
    public void Validate_EmptyDate (BocDateTimeValueType valueType, string[] expectedError, bool valid)
    {
      using (new CultureScope ("de-AT"))
      {
        var bocDateTimeValue = CreateBocDateTimeValue ("00:00", ValidTime, false, valueType);
        var validators = CreateValidator (bocDateTimeValue);

        List<string> errors;
        bool isValid = Validate (validators, out errors);

        Assert.That (isValid, Is.EqualTo (valid));
        Assert.That (errors, Is.EqualTo (expectedError));
      }
    }

    private bool Validate (List<BaseValidator> validators, out List<string> errors)
    {
      bool isValid = true;
      errors = new List<string>();
      foreach (var validator in validators)
      {
        validator.Validate();
        isValid &= validator.IsValid;
        if (!validator.IsValid)
          errors.Add (validator.ErrorMessage);
      }
      return isValid;
    }

    private BocDateTimeValue CreateBocDateTimeValue (string dateValue, string timeValue, bool isRequired, BocDateTimeValueType valueType)
    {
      var bocDateTimeValue = MockRepository.GeneratePartialMock<BocDateTimeValue>();
      bocDateTimeValue.Stub (c => c.DateString).Return (dateValue);
      bocDateTimeValue.Stub (c => c.TimeString).Return (timeValue);
      bocDateTimeValue.Required = isRequired;
      bocDateTimeValue.ValueType = valueType;
      return bocDateTimeValue;
    }

    private List<BaseValidator> CreateValidator (BusinessObjectBoundEditableWebControl bocDateTimeValue)
    {
      var validator = MockRepository.GeneratePartialMock<BocDateTimeValueValidator>();
      validator.ID = "Validator";
      validator.ControlToValidate = "Control";

      var namingContainer = MockRepository.GenerateMock<Control>();
      namingContainer.Expect (c => c.FindControl ("Control")).Return (bocDateTimeValue).Repeat.Any();
      validator.Stub (c => c.NamingContainer).Return (namingContainer);

      validator.MissingDateAndTimeErrorMessage = MissingDateAndTimeErrorMessage;
      validator.MissingDateOrTimeErrorMessage = MissingDateOrTimeErrorMessage;
      validator.MissingDateErrorMessage = MissingDateErrorMessage;
      validator.MissingTimeErrorMessage = MissingTimeErrorMessage;
      validator.InvalidDateAndTimeErrorMessage = InvalidDateAndTimeErrorMessage;
      validator.InvalidDateErrorMessage = InvalidDateErrorMessage;
      validator.InvalidTimeErrorMessage = InvalidTimeErrorMessage;

      return new List<BaseValidator>() { validator };
    }
  }
}