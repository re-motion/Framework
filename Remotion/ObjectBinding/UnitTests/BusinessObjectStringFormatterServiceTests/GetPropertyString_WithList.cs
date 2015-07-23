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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithList
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectNumericProperty _mockProperty;
    private IFormattable[] _mockValues;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _mockRepository = new MockRepository();
      _mockBusinessObject = _mockRepository.StrictMock<IBusinessObject>();
      _mockProperty = _mockRepository.StrictMock<IBusinessObjectNumericProperty>();
      _mockValues = new IFormattable[]
          {
              _mockRepository.StrictMock<IFormattable>(),
              _mockRepository.StrictMock<IFormattable>(),
              _mockRepository.StrictMock<IFormattable>()
          };
    }

    [Test]
    public void List_WithoutLineCount ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (null, null)).Return ("First");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithoutLineCountHavingFormatString ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString ("TheFormatString", null)).Return ("First");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "TheFormatString");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithLineCountBelowListLength ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (null, null)).Return ("First");
      Expect.Call (_mockValues[1].ToString (null, null)).Return ("Second");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=2");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithLineCountAboveListLength ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (null, null)).Return ("First");
      Expect.Call (_mockValues[1].ToString (null, null)).Return ("Second");
      Expect.Call (_mockValues[2].ToString (null, null)).Return ("Third");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=4");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond\r\nThird"));
    }

    [Test]
    public void List_WithLineCountAll ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (null, null)).Return ("First");
      Expect.Call (_mockValues[1].ToString (null, null)).Return ("Second");
      Expect.Call (_mockValues[2].ToString (null, null)).Return ("Third");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=all");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond\r\nThird"));
    }

    [Test]
    public void List_WithInvalidLineCountHavingNonInteger ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (null, null)).Return ("First");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=X");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithInvalidLineCountHavingMissingNumber ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (null, null)).Return ("First");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First ... [3]"));
    }

    [Test]
    public void List_WithLineCountAndFormatString ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString ("TheFormatString", null)).Return ("First");
      Expect.Call (_mockValues[1].ToString ("TheFormatString", null)).Return ("Second");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=2|TheFormatString");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithLineCountAndOnlySeparator ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString (string.Empty, null)).Return ("First");
      Expect.Call (_mockValues[1].ToString (string.Empty, null)).Return ("Second");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=2|");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithLineCountAndFormatStringWithMultipleSeparatorCharacters ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (_mockValues);
      Expect.Call (_mockValues[0].ToString ("|The|FormatString", null)).Return ("First");
      Expect.Call (_mockValues[1].ToString ("|The|FormatString", null)).Return ("Second");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "lines=2||The|FormatString");

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("First\r\nSecond ... [3]"));
    }

    [Test]
    public void List_WithNull ()
    {
      Expect.Call (_mockProperty.IsList).Return (true);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (null);
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.Empty);
    }
  }
}
