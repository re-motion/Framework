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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Results
{
  [TestFixture]
  public class EvaluatedBusinessObjectPropertyPathResultTest
  {
    private MockRepository _mockRepository;

    private IBusinessObjectProperty _propertyMock;

    private IBusinessObjectWithIdentity _businessObjectWithIdentityMock;

    private IBusinessObjectPropertyPathResult _result;
    private IBusinessObjectClassWithIdentity _businessObjectClassWithIdentityStub;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      var businessObjectProviderStub = MockRepository.GenerateStub<IBusinessObjectProvider>();
      businessObjectProviderStub.Stub (_=>_.GetNotAccessiblePropertyStringPlaceHolder ()).Return ("X");

      _businessObjectClassWithIdentityStub = MockRepository.GenerateStub<IBusinessObjectClassWithIdentity>();
      _businessObjectClassWithIdentityStub.Stub (_=>_.BusinessObjectProvider).Return (businessObjectProviderStub);

      _businessObjectWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectWithIdentity>();
      _businessObjectWithIdentityMock.Stub (_=>_.BusinessObjectClass).Return (_businessObjectClassWithIdentityStub);

      _propertyMock = _mockRepository.StrictMock<IBusinessObjectProperty>();
      _propertyMock.Stub (_=>_.Identifier).Return ("Property");

      _result = new EvaluatedBusinessObjectPropertyPathResult (_businessObjectWithIdentityMock, _propertyMock);
    }

    [Test]
    public void GetValue ()
    {
      using (_mockRepository.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectWithIdentityGetProperty (100);
      }
      _mockRepository.ReplayAll();

      object actual = _result.GetValue();

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo (100));
    }

    [Test]
    public void GetValue_WithAccessDenied ()
    {
      ExpectOnceOnPropertyIsAccessible (false);
      _mockRepository.ReplayAll();

      object actualObject = _result.GetValue();

      _mockRepository.VerifyAll();
      Assert.That (actualObject, Is.Null);
    }

    [Test]
    public void GetValue_WithBusinessObjectPropertyAccessException ()
    {
      using (_mockRepository.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetProperty();
      }
      _mockRepository.ReplayAll();

      object actualObject = _result.GetValue();

      _mockRepository.VerifyAll();
      Assert.That (actualObject, Is.Null);
    }

    [Test]
    public void GetPropertyString ()
    {
      using (_mockRepository.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectOnceOnBusinessObjectWithIdentityGetPropertyString ("value", "format");
      }
      _mockRepository.ReplayAll();

      string actual = _result.GetString ("format");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("value"));
    }

    [Test]
    public void GetString_WithAccessDenied ()
    {
      ExpectOnceOnPropertyIsAccessible (false);
      _mockRepository.ReplayAll();

      string actual = _result.GetString (string.Empty);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("X"));
    }

    [Test]
    public void GetString_WithBusinessObjectPropertyAccessException ()
    {
      using (_mockRepository.Ordered())
      {
        ExpectOnceOnPropertyIsAccessible (true);
        ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetPropertyString ("format");
      }
      _mockRepository.ReplayAll();

      string actual = _result.GetString ("format");

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("X"));
    }

    [Test]
    public void GeResultProperty ()
    {
      Assert.That (_result.ResultProperty, Is.SameAs (_propertyMock));
    }

    [Test]
    public void GeResultObject ()
    {
      Assert.That (_result.ResultObject, Is.SameAs (_businessObjectWithIdentityMock));
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.That (_result.IsNull, Is.False);
    }

    private void ExpectOnceOnPropertyIsAccessible (bool returnValue)
    {
      _propertyMock.Expect (_ => _.IsAccessible (_businessObjectWithIdentityMock))
                   .Return (returnValue);
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetProperty (int returnValue)
    {
      _businessObjectWithIdentityMock.Expect (_ => _.GetProperty (_propertyMock))
                                     .Return (returnValue);
    }

    private void ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetProperty ()
    {
      _businessObjectWithIdentityMock.Expect (_ => _.GetProperty (_propertyMock))
                                     .Throw (new BusinessObjectPropertyAccessException("The Message", null));
    }

    private void ExpectOnceOnBusinessObjectWithIdentityGetPropertyString (string returnValue, string format)
    {
      _businessObjectWithIdentityMock.Expect (_ => _.GetPropertyString (_propertyMock, format))
                                     .Return (returnValue);
    }

    private void ExpectThrowBusinessObjectPropertyAccessExceptionOnBusinessObjectWithIdentityGetPropertyString (string format)
    {
      _businessObjectWithIdentityMock.Expect (_ => _.GetPropertyString (_propertyMock, format))
                                     .Throw (new BusinessObjectPropertyAccessException("The Message", null));
    }
  }
}