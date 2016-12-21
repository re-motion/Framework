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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.BusinessObjectPropertyPathBaseTests
{
  public class BusinessObjectPropertyPathTestHelper
  {
    // types

    // static members

    public const string NotAccessible = "Not Accessible";

    // member fields

    private MockRepository _mocks;

    private IBusinessObjectProperty _mockProperty;
    private IBusinessObjectReferenceProperty _mockReferenceProperty;
    private IBusinessObjectReferenceProperty _mockReferenceListProperty;

    private IBusinessObjectClass _mockBusinessObjectClass;
    private IBusinessObjectClassWithIdentity _mockBusinessObjectClassWithIdentity;

    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectWithIdentity _mockBusinessObjectWithIdentity;
    private IBusinessObjectWithIdentity[] _businessObjectWithIdentityList;

    private IBusinessObjectProvider _mockBusinessObjectProvider;

    // construction and disposing

    public BusinessObjectPropertyPathTestHelper ()
    {
      _mocks = new MockRepository ();

      _mockProperty = _mocks.StrictMock<IBusinessObjectProperty> ();
      _mockReferenceProperty = _mocks.StrictMock<IBusinessObjectReferenceProperty> ();
      _mockReferenceListProperty = _mocks.StrictMock<IBusinessObjectReferenceProperty> ();

      _mockBusinessObjectClass = _mocks.StrictMock<IBusinessObjectClass> ();
      _mockBusinessObjectClassWithIdentity = _mocks.StrictMock<IBusinessObjectClassWithIdentity> ();

      _mockBusinessObject = _mocks.StrictMock<IBusinessObject> ();
      _mockBusinessObjectWithIdentity = _mocks.StrictMock<IBusinessObjectWithIdentity> ();
      _businessObjectWithIdentityList = new[] { _mockBusinessObjectWithIdentity, _mocks.StrictMock<IBusinessObjectWithIdentity>() };

      _mockBusinessObjectProvider = _mocks.StrictMock<IBusinessObjectProvider> ();

      SetupResult.For (_mockBusinessObject.BusinessObjectClass).Return (_mockBusinessObjectClass);
      SetupResult.For (_mockBusinessObjectWithIdentity.BusinessObjectClass).Return (_mockBusinessObjectClassWithIdentity);
      SetupResult.For (_mockReferenceProperty.IsList).Return (false);
      SetupResult.For (_mockReferenceListProperty.IsList).Return (true);

      SetupResult.For (_mockProperty.Identifier).Return ("Property");
      SetupResult.For (_mockReferenceProperty.Identifier).Return ("ReferenceProperty");
      SetupResult.For (_mockReferenceListProperty.Identifier).Return ("ReferenceListProperty");

      SetupResult.For (_mockProperty.BusinessObjectProvider).Return (_mockBusinessObjectProvider);
      SetupResult.For (_mockReferenceProperty.BusinessObjectProvider).Return (_mockBusinessObjectProvider);
      SetupResult.For (_mockReferenceListProperty.BusinessObjectProvider).Return (_mockBusinessObjectProvider);

      SetupResult.For (_mockBusinessObjectClass.BusinessObjectProvider).Return (_mockBusinessObjectProvider);
      SetupResult.For (_mockBusinessObjectClassWithIdentity.BusinessObjectProvider).Return (_mockBusinessObjectProvider);

      SetupResult.For (_mockBusinessObjectProvider.GetPropertyPathSeparator ()).Return ('.');
      SetupResult.For (_mockBusinessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder ()).Return (BusinessObjectPropertyPathTestHelper.NotAccessible);
    }

    // methods and properties

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public IBusinessObjectProperty Property
    {
      get { return _mockProperty; }
    }

    public IBusinessObjectReferenceProperty ReferenceProperty
    {
      get { return _mockReferenceProperty; }
    }

    public IBusinessObjectReferenceProperty ReferenceListProperty
    {
      get { return _mockReferenceListProperty; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return _mockBusinessObjectClass; }
    }

    public IBusinessObjectClassWithIdentity BusinessObjectClassWithIdentity
    {
      get { return _mockBusinessObjectClassWithIdentity; }
    }

    public IBusinessObject BusinessObject
    {
      get { return _mockBusinessObject; }
    }

    public IBusinessObjectWithIdentity BusinessObjectWithIdentity
    {
      get { return _mockBusinessObjectWithIdentity; }
    }

    public IBusinessObjectWithIdentity[] BusinessObjectWithIdentityList
    {
      get { return _businessObjectWithIdentityList; }
    }

    public void ExpectOnceOnGetProperty (IBusinessObject businessObject, IBusinessObjectProperty property, object returnValue)
    {
      Expect.Call (businessObject.GetProperty (property)).Return (returnValue);
    }

    public void ExpectThrowOnGetProperty (IBusinessObject businessObject, IBusinessObjectProperty property, Exception exception)
    {
      Expect.Call (businessObject.GetProperty (property)).Throw (exception);
    }

    public void ExpectOnceOnIsAccessible (
        IBusinessObjectClass businessObjectClass, 
        IBusinessObject businessObject, 
        IBusinessObjectProperty property, 
        bool returnValue)
    {
      Expect.Call (property.IsAccessible (businessObject)).Return (returnValue);
    }
  }
}
