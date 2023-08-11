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
using System.Globalization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.NumericValidatorTests
{
  [TestFixture]
  public class Common : TestBase
  {
    [Test]
    public void Validate_WithInvalidNumberStyle ()
    {
      Validator.DataType = NumericValidationDataType.Double;
      Validator.NumberStyle = NumberStyles.HexNumber;
      TextBox.Text = "1";
      Assert.That(
          () => Validator.Validate(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("The combination of the flags in the 'NumberStyle' property is invalid."));
    }

    [Test]
    public void Validate_WithInvalidDataType ()
    {
      PrivateInvoke.SetNonPublicField(Validator, "_dataType", -1);
      TextBox.Text = "a";
      Assert.That(
          () => Validator.Validate(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The value '-1' of the 'DataType' property is not a valid value."));
    }

    [Test]
    public void Validate_WithSpaces ()
    {
      TextBox.Text = "   ";
      Assert.That(() => Validator.Validate(), Throws.Nothing);
      Assert.That(Validator.IsValid, Is.True);
    }
  }
}
