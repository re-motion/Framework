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
using Moq;
using NUnit.Framework;
using Remotion.Globalization;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class ResourceManagerBasedValidationMessageTest
  {
    [ResourceIdentifiers]
    private enum TestResourceIdentifiers
    {
      One,
      Two,
    }

    [Test]
    [SetCulture("it-IT")]
    [SetUICulture("fr-FR")]
    public void Format_UsesSuppliedFormatProviderInsteadOfCurrentCulture ()
    {
      var value = "{0:C}";
      CultureInfo actualCulture = null;

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock.Setup(_ => _.TryGetString("Remotion.Validation.Globalization.UnitTests.ResourceManagerBasedValidationMessageTest.One", out value))
          .Callback(() =>
          {
            actualCulture = CultureInfo.CurrentUICulture;
          })
          .Returns(true);

      var validationMessage = new ResourceManagerBasedValidationMessage(resourceManagerMock.Object, TestResourceIdentifiers.One);
      var expectedCulture = CultureInfo.GetCultureInfo("en-US");
      var formatProvider = CultureInfo.GetCultureInfo("de-AT");
      var parameters = new object[] { 3123.31};

      var result = validationMessage.Format(expectedCulture, formatProvider, parameters);

      Assert.That(result, Is.EqualTo("€ 3.123,31"));
      Assert.That(actualCulture, Is.EqualTo(expectedCulture));
    }

    [Test]
    [SetCulture("it-IT")]
    [SetUICulture("fr-FR")]
    public void Format_UsesSuppliedCultureInsteadOfCurrentUICulture ()
    {
      var value = "{0:C}";
      CultureInfo actualCulture = null;

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock.Setup(_ => _.TryGetString("Remotion.Validation.Globalization.UnitTests.ResourceManagerBasedValidationMessageTest.One", out value))
          .Callback(() =>
          {
            actualCulture = CultureInfo.CurrentUICulture;
          })
          .Returns(true);

      var validationMessage = new ResourceManagerBasedValidationMessage(resourceManagerMock.Object, TestResourceIdentifiers.One);
      var expectedCulture = CultureInfo.GetCultureInfo("en-US");
      var parameters = new object[] { 3123.31};

      var result = validationMessage.Format(expectedCulture, null, parameters);

      Assert.That(result, Is.EqualTo("¤3,123.31"));
      Assert.That(actualCulture, Is.EqualTo(expectedCulture));
    }

    [SetCulture("it-IT")]
    [SetUICulture("fr-FR")]
    [Test]
    public void ToString_ReturnsValueOfResource ()
    {
      string value = "Expected Value";
      CultureInfo actualCulture = null;

      var resourceManagerMock = new Mock<IResourceManager>();
      resourceManagerMock
          .Setup(_ => _.TryGetString("Remotion.Validation.Globalization.UnitTests.ResourceManagerBasedValidationMessageTest.Two", out value))
          .Callback(() =>
          {
            actualCulture = CultureInfo.CurrentUICulture;
          })
          .Returns(true);

      var validationMessage  = new ResourceManagerBasedValidationMessage(resourceManagerMock.Object, TestResourceIdentifiers.Two);
      var expectedCulture = CultureInfo.GetCultureInfo("fr-FR");

      var result = validationMessage.ToString();

      Assert.That(result, Is.EqualTo("Expected Value"));
      Assert.That(actualCulture, Is.EqualTo(expectedCulture));
    }
  }
}
