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
  public class GetPropertyString_WithNotSupportedProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectProperty _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService ();
      _mockRepository = new MockRepository ();
      _mockBusinessObject = _mockRepository.StrictMock<IBusinessObject> ();
      _mockProperty = _mockRepository.StrictMock<IBusinessObjectProperty> ();
    }

    [Test]
    public void Scalar_WithValue ()
    {
      object value = new object();
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (value);
      _mockRepository.ReplayAll();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo (value.ToString()));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (null);
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.Empty);
    }
  }
}
