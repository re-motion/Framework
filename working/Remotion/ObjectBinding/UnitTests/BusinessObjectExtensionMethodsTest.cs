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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectExtensionMethodsTest
  {
    [Test]
    public void GetProperty ()
    {
      object exptected = new object();
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();

      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      classStub.Stub (_ => _.GetPropertyDefinition ("TheProperty")).Return (propertyStub);
      businessObjectStub.Stub (_ => _.GetProperty (propertyStub)).Return (exptected);

      object actual = businessObjectStub.GetProperty ("TheProperty");

      Assert.That (actual, Is.EqualTo (exptected));
    }

    [Test]
    public void SetProperty ()
    {
      object exptected = new object();
      var businessObjectMock = MockRepository.GenerateMock<IBusinessObject>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();

      businessObjectMock.Stub (_ => _.BusinessObjectClass).Return (classStub);
      classStub.Stub (_ => _.GetPropertyDefinition ("TheProperty")).Return (propertyStub);
      businessObjectMock.Expect (_ => _.SetProperty (propertyStub, exptected));

      businessObjectMock.SetProperty ("TheProperty", exptected);

      businessObjectMock.VerifyAllExpectations();
    }

    [Test]
    public void GetPropertyString ()
    {
      string exptected = "TheValue";
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyStub = MockRepository.GenerateStub<IBusinessObjectProperty>();

      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      classStub.Stub (_ => _.GetPropertyDefinition ("TheProperty")).Return (propertyStub);
      businessObjectStub.Stub (_ => _.GetPropertyString (propertyStub, null)).Return (exptected);

      string actual = businessObjectStub.GetPropertyString ("TheProperty");

      Assert.That (actual, Is.EqualTo (exptected));
    }

    [Test]
    public void GetProperty_WithInvalidPropertyIdentifier ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();

      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      classStub.Stub (_ => _.GetPropertyDefinition ("InvalidProperty")).Return (null);
      classStub.Stub (_ => _.Identifier).Return ("TheClass");

      Assert.That (
          () => businessObjectStub.GetProperty ("InvalidProperty"),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "The business object's class ('TheClass') does not contain a property named 'InvalidProperty'."));
    }

    [Test]
    public void GetAccessibleDisplayName_WithoutBusinessObjectProperty_ReturnsDisplayNamePropertyFromInterfaceImplementation ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();

      classStub.Stub (_ => _.GetPropertyDefinition ("DisplayName")).Return (null);
      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      businessObjectStub.Stub (_ => _.DisplayName).Return ("The DisplayName");

      string actual = businessObjectStub.GetAccessibleDisplayName();

      Assert.That (actual, Is.StringStarting ("The DisplayName"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndAccessGranted_ReturnsValueViaBusinessObjectProperty ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyMock = MockRepository.GenerateMock<IBusinessObjectProperty>();

      classStub.Stub (_ => _.GetPropertyDefinition ("DisplayName")).Return (propertyMock);
      propertyMock.Expect (_ => _.IsAccessible (businessObjectStub)).Return (true);
      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      businessObjectStub.Stub (_ => _.DisplayName).Throw (new AssertionException ("Should not be called."));
      businessObjectStub.Stub (_ => _.GetProperty (propertyMock)).Return ("The DisplayName");

      string actual = businessObjectStub.GetAccessibleDisplayName();

      propertyMock.VerifyAllExpectations();
      Assert.That (actual, Is.EqualTo ("The DisplayName"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndAccessDenied_ReturnsNotAccessiblePlaceHolder ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyMock = MockRepository.GenerateMock<IBusinessObjectProperty>();
      var businessObjectProviderStub = MockRepository.GenerateStub<IBusinessObjectProvider>();

      classStub.Stub (_ => _.GetPropertyDefinition ("DisplayName")).Return (propertyMock);
      propertyMock.Expect (_ => _.IsAccessible (businessObjectStub)).Return (false);
      propertyMock.Stub (_ => _.BusinessObjectProvider).Return (businessObjectProviderStub);
      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      businessObjectStub.Stub (_ => _.DisplayName).Throw (new AssertionException ("Should not be called."));
      businessObjectStub.Stub (_ => _.GetProperty (propertyMock)).Throw (new AssertionException ("Should not be called."));
      businessObjectProviderStub.Stub (_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Return ("N/A");
      string actual = businessObjectStub.GetAccessibleDisplayName();

      propertyMock.VerifyAllExpectations();
      Assert.That (actual, Is.EqualTo ("N/A"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndBusinessObjectPropertyAccessException_ReturnsNotAccessiblePlaceHolder ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyMock = MockRepository.GenerateMock<IBusinessObjectProperty>();
      var businessObjectProviderStub = MockRepository.GenerateStub<IBusinessObjectProvider>();

      classStub.Stub (_ => _.GetPropertyDefinition ("DisplayName")).Return (propertyMock);
      propertyMock.Expect (_ => _.IsAccessible (businessObjectStub)).Return (true);
      propertyMock.Stub (_ => _.BusinessObjectProvider).Return (businessObjectProviderStub);
      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      businessObjectStub.Stub (_ => _.DisplayName).Throw (new AssertionException ("Should not be called."));
      businessObjectStub.Stub (_ => _.GetProperty (propertyMock)).Throw (new BusinessObjectPropertyAccessException ("The Message", null));
      businessObjectProviderStub.Stub (_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Return ("N/A");
      string actual = businessObjectStub.GetAccessibleDisplayName();

      propertyMock.VerifyAllExpectations();
      Assert.That (actual, Is.EqualTo ("N/A"));
    }

    [Test]
    public void GetAccessibleDisplayName_WithBusinessObjectProperty_AndOtherException_ReThrowsOriginalException ()
    {
      var expectedException = new Exception ("The Message", null);

      var businessObjectStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      var classStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      var propertyMock = MockRepository.GenerateMock<IBusinessObjectProperty>();
      var businessObjectProviderStub = MockRepository.GenerateStub<IBusinessObjectProvider>();

      classStub.Stub (_ => _.GetPropertyDefinition ("DisplayName")).Return (propertyMock);
      propertyMock.Expect (_ => _.IsAccessible (businessObjectStub)).Return (true);
      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (classStub);
      businessObjectStub.Stub (_ => _.DisplayName).Throw (new AssertionException ("Should not be called."));
      businessObjectStub.Stub (_ => _.GetProperty (propertyMock)).Throw (expectedException);
      businessObjectProviderStub.Stub (_ => _.GetNotAccessiblePropertyStringPlaceHolder()).Return ("N/A");
      Assert.That (() => businessObjectStub.GetAccessibleDisplayName(), Throws.Exception.SameAs (expectedException));

      propertyMock.VerifyAllExpectations();
    }
  }
}