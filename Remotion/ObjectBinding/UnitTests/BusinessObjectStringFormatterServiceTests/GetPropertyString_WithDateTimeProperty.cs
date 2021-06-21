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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithDateTimeProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private Mock<IBusinessObject> _mockBusinessObject;
    private Mock<IBusinessObjectDateTimeProperty> _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _mockBusinessObject = new Mock<IBusinessObject> (MockBehavior.Strict);
      _mockProperty = new Mock<IBusinessObjectDateTimeProperty> (MockBehavior.Strict);
    }

    [Test]
    public void Scalar_WithValu_AndDateTimeProperty ()
    {
      _mockProperty.Setup (_ => _.Type).Returns (DateTimeType.DateTime).Verifiable();
      _mockProperty.Setup (_ => _.IsList).Returns (false).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (new DateTime (2000, 4, 14, 3, 45, 10)).Verifiable();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, null);

        _mockBusinessObject.Verify();
        _mockProperty.Verify();
        Assert.That (actual, Is.EqualTo ("14.04.2000 03:45"));
      }
    }

    [Test]
    public void Scalar_WithValu_AndDateProperty ()
    {
      _mockProperty.Setup (_ => _.Type).Returns (DateTimeType.Date).Verifiable();
      _mockProperty.Setup (_ => _.IsList).Returns (false).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (new DateTime (2000, 6, 17, 1, 1, 1)).Verifiable();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, null);

        _mockBusinessObject.Verify();
        _mockProperty.Verify();
        Assert.That (actual, Is.EqualTo ("17.06.2000"));
      }
    }

    [Test]
    public void Scalar_WithValu_AndDateProperty_AndExplicitFormatString ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (false).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (new DateTime (2000, 6, 17, 1, 2, 3)).Verifiable();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "yyyy-dd-MM HH:mm:ss");

        _mockBusinessObject.Verify();
        _mockProperty.Verify();
        Assert.That (actual, Is.EqualTo ("2000-17-06 01:02:03"));
      }
    }

    [Test]
    public void Scalar_WithFormattableValue ()
    {
      var mockValue = new Mock<IFormattable> (MockBehavior.Strict);
      _mockProperty.Setup (_ => _.IsList).Returns (false).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (mockValue.Object).Verifiable();
      mockValue.Setup (_ => _.ToString ("FormatString", null)).Returns ("ExpectedStringValue").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "FormatString");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      mockValue.Verify();
      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (false).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns ((object) null).Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "FormatString");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.Empty);
    }
  }
}
