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
using System.Threading;
using NUnit.Framework;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.UnitTests.Implementation
{
  [TestFixture]
  public class DelegateBasedValidationMessageTest
  {
    [Test]
    [SetCulture("en-US")]
    public void Format_WithFormatProvider_UsesSuppliedFormatProviderInsteadOfCurrentCulture ()
    {
      Func<string> validationMessageProvider = () => "{0} : {1:F}";
      var validationMessage = new DelegateBasedValidationMessage(validationMessageProvider);
      var culture = CultureInfo.GetCultureInfo("en-US");
      var formatProvider = CultureInfo.GetCultureInfo("de-AT");
      var parameters = new object[] { 3123.312, new DateTime(2023, 05, 15) };

      var result = validationMessage.Format(culture, formatProvider, parameters);

      Assert.That(result, Is.EqualTo("3123,312 : Montag, 15. Mai 2023 00:00:00"));
    }

    [Test]
    [SetCulture("it-IT")]
    public void Format_WithNullAsFormatProvider_UsesInvariantCulture ()
    {
      Func<string> validationMessageProvider = () => "{0} : {1:F}";
      var validationMessage = new DelegateBasedValidationMessage(validationMessageProvider);
      var culture = CultureInfo.GetCultureInfo("fr-FR");
      var parameters = new object[] { 3123.312, new DateTime(2023, 05, 15) };

      var result = validationMessage.Format(culture, null, parameters);

      Assert.That(result, Is.EqualTo("3123.312 : Monday, 15 May 2023 00:00:00"));
    }

    [Test]
    [SetCulture("it-IT")]
    [SetUICulture("en-US")]
    public void Format_UsesSuppliedCultureInsteadOfCurrentUICulture ()
    {
      CultureInfo actualCulture = null;
      Func<string> validationMessageProvider = () =>
      {
        actualCulture = Thread.CurrentThread.CurrentUICulture;
        return "";
      };

      var validationMessage = new DelegateBasedValidationMessage(validationMessageProvider);
      var culture = CultureInfo.GetCultureInfo("fr-FR");
      var formatProvider = CultureInfo.GetCultureInfo("de-AT");

      var result = validationMessage.Format(culture, formatProvider);

      Assert.That(result, Is.Empty);
      Assert.That(actualCulture, Is.EqualTo(CultureInfo.GetCultureInfo("fr-FR")));
    }

    [Test]
    public void ToString_ReturnsRawValidationMessage ()
    {
      Func<string> validationMessageProvider = () => "test {0} message";
      var validationMessage = new DelegateBasedValidationMessage(validationMessageProvider);

      var result = validationMessage.ToString();

      Assert.That(result, Is.EqualTo("test {0} message"));
    }
  }
}
