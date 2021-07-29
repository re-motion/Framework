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
using Moq;
using NUnit.Framework;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithList
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private Mock<IBusinessObject> _mockBusinessObject;
    private Mock<IBusinessObjectNumericProperty> _mockProperty;
    private Mock<IFormattable>[]_mockValues;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _mockBusinessObject = new Mock<IBusinessObject> (MockBehavior.Strict);
      _mockProperty = new Mock<IBusinessObjectNumericProperty> (MockBehavior.Strict);
      _mockValues = new Mock<IFormattable>[]
          {
              new Mock<IFormattable> (MockBehavior.Strict),
              new Mock<IFormattable> (MockBehavior.Strict),
              new Mock<IFormattable> (MockBehavior.Strict)
          };
    }

    [Test]
    public void List_WithoutLineCount ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (null, null)).Returns ("First").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, null);

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithoutLineCountHavingFormatString ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString ("TheFormatString", null)).Returns ("First").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "TheFormatString");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithLineCountBelowListLength ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (null, null)).Returns ("First").Verifiable();
      _mockValues[1].Setup (_ => _.ToString (null, null)).Returns ("Second").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=2");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithLineCountAboveListLength ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (null, null)).Returns ("First").Verifiable();
      _mockValues[1].Setup (_ => _.ToString (null, null)).Returns ("Second").Verifiable();
      _mockValues[2].Setup (_ => _.ToString (null, null)).Returns ("Third").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=4");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond\r\nThird"));
    }

    [Test]
    public void List_WithLineCountAll ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (null, null)).Returns ("First").Verifiable();
      _mockValues[1].Setup (_ => _.ToString (null, null)).Returns ("Second").Verifiable();
      _mockValues[2].Setup (_ => _.ToString (null, null)).Returns ("Third").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=all");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond\r\nThird"));
    }

    [Test]
    public void List_WithInvalidLineCountHavingNonInteger ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (null, null)).Returns ("First").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=X");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithInvalidLineCountHavingMissingNumber ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (null, null)).Returns ("First").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithLineCountAndFormatString ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString ("TheFormatString", null)).Returns ("First").Verifiable();
      _mockValues[1].Setup (_ => _.ToString ("TheFormatString", null)).Returns ("Second").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=2|TheFormatString");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithLineCountAndOnlySeparator ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString (string.Empty, null)).Returns ("First").Verifiable();
      _mockValues[1].Setup (_ => _.ToString (string.Empty, null)).Returns ("Second").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=2|");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithLineCountAndFormatStringWithMultipleSeparatorCharacters ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns (_mockValues.Select (_ => _.Object).ToArray()).Verifiable();
      _mockValues[0].Setup (_ => _.ToString ("|The|FormatString", null)).Returns ("First").Verifiable();
      _mockValues[1].Setup (_ => _.ToString ("|The|FormatString", null)).Returns ("Second").Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, "lines=2||The|FormatString");

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithNull ()
    {
      _mockProperty.Setup (_ => _.IsList).Returns (true).Verifiable();
      _mockBusinessObject.Setup (_ => _.GetProperty (_mockProperty.Object)).Returns ((object) null).Verifiable();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject.Object, _mockProperty.Object, null);

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That (actual, Is.Empty);
    }
  }
}
