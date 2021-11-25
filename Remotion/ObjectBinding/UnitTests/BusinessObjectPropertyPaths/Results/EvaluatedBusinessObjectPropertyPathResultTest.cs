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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Results
{
  [TestFixture]
  public class EvaluatedBusinessObjectPropertyPathResultTest
  {

    private Mock<IBusinessObjectProperty> _propertyMock;

    private Mock<IBusinessObjectWithIdentity> _businessObjectWithIdentityMock;

    private IBusinessObjectPropertyPathResult _result;
    private Mock<IBusinessObjectClassWithIdentity> _businessObjectClassWithIdentityStub;

    [SetUp]
    public void SetUp ()
    {
      var businessObjectProviderStub = new Mock<IBusinessObjectProvider>();
      businessObjectProviderStub.Setup(_=>_.GetNotAccessiblePropertyStringPlaceHolder()).Returns("X");

      _businessObjectClassWithIdentityStub = new Mock<IBusinessObjectClassWithIdentity>();
      _businessObjectClassWithIdentityStub.Setup(_=>_.BusinessObjectProvider).Returns(businessObjectProviderStub.Object);

      _businessObjectWithIdentityMock = new Mock<IBusinessObjectWithIdentity>(MockBehavior.Strict);
      _businessObjectWithIdentityMock.Setup(_=>_.BusinessObjectClass).Returns(_businessObjectClassWithIdentityStub.Object);

      _propertyMock = new Mock<IBusinessObjectProperty>(MockBehavior.Strict);
      _propertyMock.Setup(_=>_.Identifier).Returns("Property");

      _result = new EvaluatedBusinessObjectPropertyPathResult(_businessObjectWithIdentityMock.Object, _propertyMock.Object);
    }

    [Test]
    public void GetValue ()
    {
      var sequence = new MockSequence();
      ExpectOnceOnPropertyIsAccessible(true, sequence);
      ExpectOnceOnBusinessObjectWithIdentityGetProperty(100, sequence);

      object actual = _result.GetValue();

      _businessObjectWithIdentityMock.Verify();
      _propertyMock.Verify();
      Assert.That(actual, Is.EqualTo(100));
    }

    [Test]
    public void GetValue_WithAccessDenied ()
    {
      ExpectOnceOnPropertyIsAccessible(false);

      object actualObject = _result.GetValue();

      _businessObjectWithIdentityMock.Verify();
      _propertyMock.Verify();
      Assert.That(actualObject, Is.Null);
    }

    [Test]
    public void GetValue_WithBusinessObjectPropertyAccessException ()
    {
      var sequence = new MockSequence();
      ExpectOnceOnPropertyIsAccessible(true, sequence);
      ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetProperty(sequence);

      object actualObject = _result.GetValue();

      _businessObjectWithIdentityMock.Verify();
      _propertyMock.Verify();
      Assert.That(actualObject, Is.Null);
    }

    [Test]
    public void GetPropertyString ()
    {
      var sequence = new MockSequence();
      ExpectOnceOnPropertyIsAccessible(true, sequence);
      ExpectOnceOnBusinessObjectWithIdentityGetPropertyString("value", "format", sequence);

      string actual = _result.GetString("format");

      _businessObjectWithIdentityMock.Verify();
      _propertyMock.Verify();
      Assert.That(actual, Is.EqualTo("value"));
    }

    [Test]
    public void GetString_WithAccessDenied ()
    {
      ExpectOnceOnPropertyIsAccessible(false);

      string actual = _result.GetString(string.Empty);

      _businessObjectWithIdentityMock.Verify();
      _propertyMock.Verify();
      Assert.That(actual, Is.EqualTo("X"));
    }

    [Test]
    public void GetString_WithBusinessObjectPropertyAccessException ()
    {
      var sequence = new MockSequence();
      ExpectOnceOnPropertyIsAccessible(true, sequence);
      ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetPropertyString("format", sequence);

      string actual = _result.GetString("format");

      _businessObjectWithIdentityMock.Verify();
      _propertyMock.Verify();
      Assert.That(actual, Is.EqualTo("X"));
    }

    [Test]
    public void GeResultProperty ()
    {
      Assert.That(_result.ResultProperty, Is.SameAs(_propertyMock.Object));
    }

    [Test]
    public void GeResultObject ()
    {
      Assert.That(_result.ResultObject, Is.SameAs(_businessObjectWithIdentityMock.Object));
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That(_result.IsNull, Is.False);
    }

    private void ExpectOnceOnPropertyIsAccessible (bool returnValue)
    {
      _propertyMock.Setup(_ => _.IsAccessible(_businessObjectWithIdentityMock.Object))
                   .Returns(returnValue)
                   .Verifiable();
    }

    private void ExpectOnceOnPropertyIsAccessible (bool returnValue, MockSequence sequence)
    {
      _propertyMock.InSequence(sequence)
                   .Setup(_ => _.IsAccessible(_businessObjectWithIdentityMock.Object))
                   .Returns(returnValue)
                   .Verifiable();
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetProperty (int returnValue, MockSequence sequence)
    {
      _businessObjectWithIdentityMock.InSequence(sequence)
                                     .Setup(_ => _.GetProperty(_propertyMock.Object))
                                     .Returns(returnValue)
                                     .Verifiable();
    }

    private void ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetProperty (MockSequence sequence)
    {
      _businessObjectWithIdentityMock.InSequence(sequence)
                                     .Setup(_ => _.GetProperty(_propertyMock.Object))
                                     .Throws(new BusinessObjectPropertyAccessException("The Message", null))
                                     .Verifiable();
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetPropertyString (string returnValue, string format, MockSequence sequence)
    {
      _businessObjectWithIdentityMock.InSequence(sequence)
                                     .Setup(_ => _.GetPropertyString(_propertyMock.Object, format))
                                     .Returns(returnValue)
                                     .Verifiable();
    }

    private void ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetPropertyString (string format, MockSequence sequence)
    {
      _businessObjectWithIdentityMock.InSequence(sequence)
                                     .Setup(_ => _.GetPropertyString(_propertyMock.Object, format))
                                     .Throws(new BusinessObjectPropertyAccessException("The Message", null))
                                     .Verifiable();
    }
  }
}