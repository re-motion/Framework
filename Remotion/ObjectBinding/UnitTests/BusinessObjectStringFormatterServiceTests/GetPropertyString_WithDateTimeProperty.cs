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
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithDateTimeProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectDateTimeProperty _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _mockRepository = new MockRepository();
      _mockBusinessObject = _mockRepository.StrictMock<IBusinessObject>();
      _mockProperty = _mockRepository.StrictMock<IBusinessObjectDateTimeProperty>();
    }

    [Test]
    public void Scalar_WithValu_AndDateTimeProperty ()
    {
      Expect.Call (_mockProperty.Type).Return (DateTimeType.DateTime);
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (new DateTime (2000, 4, 14, 3, 45, 10));
      _mockRepository.ReplayAll();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

        _mockRepository.VerifyAll();
        Assert.That (actual, Is.EqualTo ("14.04.2000 03:45"));
      }
    }

    [Test]
    public void Scalar_WithValu_AndDateProperty ()
    {
      Expect.Call (_mockProperty.Type).Return (DateTimeType.Date);
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (new DateTime (2000, 6, 17, 1, 1, 1));
      _mockRepository.ReplayAll();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

        _mockRepository.VerifyAll();
        Assert.That (actual, Is.EqualTo ("17.06.2000"));
      }
    }

    [Test]
    public void Scalar_WithValu_AndDateProperty_AndExplicitFormatString ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (new DateTime (2000, 6, 17, 1, 2, 3));
      _mockRepository.ReplayAll();

      using (new CultureScope (new CultureInfo ("de-de"), new CultureInfo ("de-de")))
      {
        string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "yyyy-dd-MM HH:mm:ss");

        _mockRepository.VerifyAll();
        Assert.That (actual, Is.EqualTo ("2000-17-06 01:02:03"));
      }
    }

    [Test]
    public void Scalar_WithFormattableValue ()
    {
      IFormattable mockValue = _mockRepository.StrictMock<IFormattable>();
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (mockValue);
      Expect.Call (mockValue.ToString ("FormatString", null)).Return ("ExpectedStringValue");
      _mockRepository.ReplayAll();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "FormatString");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (null);
      _mockRepository.ReplayAll();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, "FormatString");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.Empty);
    }
  }
}
