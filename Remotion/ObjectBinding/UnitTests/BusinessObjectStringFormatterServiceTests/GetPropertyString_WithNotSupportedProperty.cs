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
using Moq;
using NUnit.Framework;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithNotSupportedProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private Mock<IBusinessObject> _mockBusinessObject;
    private Mock<IBusinessObjectProperty> _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService();
      _mockBusinessObject = new Mock<IBusinessObject>(MockBehavior.Strict);
      _mockProperty = new Mock<IBusinessObjectProperty>(MockBehavior.Strict);
    }

    [Test]
    public void Scalar_WithValue ()
    {
      object value = new object();
      _mockProperty.Setup(_ => _.IsList).Returns(false).Verifiable();
      _mockBusinessObject.Setup(_ => _.GetProperty(_mockProperty.Object)).Returns(value).Verifiable();

      string actual = _stringFormatterService.GetPropertyString(_mockBusinessObject.Object, _mockProperty.Object, null);

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That(actual, Is.EqualTo(value.ToString()));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      _mockProperty.Setup(_ => _.IsList).Returns(false).Verifiable();
      _mockBusinessObject.Setup(_ => _.GetProperty(_mockProperty.Object)).Returns((object)null).Verifiable();

      string actual = _stringFormatterService.GetPropertyString(_mockBusinessObject.Object, _mockProperty.Object, null);

      _mockBusinessObject.Verify();
      _mockProperty.Verify();
      Assert.That(actual, Is.Empty);
    }
  }
}
