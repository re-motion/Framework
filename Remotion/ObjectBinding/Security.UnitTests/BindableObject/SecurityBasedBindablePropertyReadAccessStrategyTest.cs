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
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.Security.BindableObject;
using Remotion.ObjectBinding.Security.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Security.UnitTests.BindableObject
{
  [TestFixture]
  public class SecurityBasedBindablePropertyReadAccessStrategyTest : TestBase
  {
    private SecurableClassWithReferenceType<string> _securableObject;
    private ServiceLocatorScope _serviceLocatorScope;
    private ISecurityProvider _securityProviderStub;
    private ISecurityPrincipal _principalStub;
    private IObjectSecurityStrategy _objectSecurityStrategyMock;
    private SecurityBasedBindablePropertyReadAccessStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp();

      _objectSecurityStrategyMock = MockRepository.GenerateStrictMock<IObjectSecurityStrategy>();

      _securableObject = new SecurableClassWithReferenceType<string> (_objectSecurityStrategyMock);

      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();

      _principalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      var principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider>();
      principalProviderStub.Stub (_ => _.GetPrincipal()).Return (_principalStub);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _securityProviderStub);
      serviceLocator.RegisterSingle (() => principalProviderStub);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      _strategy = new SecurityBasedBindablePropertyReadAccessStrategy();
    }

    public override void TearDown ()
    {
      _serviceLocatorScope.Dispose();

      base.TearDown();
    }

    [Test]
    public void CanRead_WithBusinessObjectIsNull_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var result = _strategy.CanRead (null, bindableProperty);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanRead_WithNonSecurableObject_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var result = _strategy.CanRead (new ClassWithReferenceType<string>(), bindableProperty);

      Assert.That (result, Is.True);
    }

    [Test]
    public void CanRead_WithSecurableObject_EvaluatesObjectSecurityStratey_ReturnsResult ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      ExpectHasAccessOnObjectSecurityStrategy (expectedResult, TestAccessTypes.TestRead);

      var bindableProperty = CreateBindableProperty (() => ((SecurableClassWithReferenceType<string>) null).CustomPermissisons);

      var actualResult = _strategy.CanRead (_securableObject, bindableProperty);

      Assert.That (actualResult, Is.EqualTo (expectedResult));
      _objectSecurityStrategyMock.VerifyAllExpectations();
    }

    [Test]
    public void CanRead_WithSecurableObject_WithoutSetter_UsesNullMethodInfo_ReturnsResult ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      ExpectHasAccessOnObjectSecurityStrategy (expectedResult, GeneralAccessTypes.Read);

      var bindableProperty = new StubPropertyBase (
          GetPropertyParameters (PropertyInfoAdapter.Create (typeof (ClassWithReferenceType<string>).GetProperty ("PropertyWithNoGetter"))));

      var actualResult = _strategy.CanRead (_securableObject, bindableProperty);

      Assert.That (actualResult, Is.EqualTo (expectedResult));
      _objectSecurityStrategyMock.VerifyAllExpectations();
    }

    [Test]
    public void IsPropertyAccessException_WithPermissionDeniedException_WithIBusinessObject_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var businessObjectClassStub = MockRepository.GenerateStub<IBusinessObjectClass>();
      businessObjectClassStub.Stub (_ => _.Identifier).Return ("TheClass");

      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      businessObjectStub.Stub (_ => _.BusinessObjectClass).Return (businessObjectClassStub);

      var permissionDeniedException = new PermissionDeniedException ("The Exception");
      BusinessObjectPropertyAccessException actualException;
      var actualResult = _strategy.IsPropertyAccessException (
          businessObjectStub,
          bindableProperty,
          permissionDeniedException,
          out actualException);

      Assert.That (actualResult, Is.True);

      Assert.That (actualException, Is.Not.Null);
      Assert.That (
          actualException.Message,
          Is.EqualTo (
              "A PermissionDeniedException occured while getting the value of property 'Scalar' for business object type 'TheClass'."));
      Assert.That (actualException.InnerException, Is.SameAs (permissionDeniedException));
    }

    [Test]
    public void IsPropertyAccessException_WithPermissionDeniedException_WithIBusinessObjectWithIdentity_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var businessObjectStub = MockRepository.GenerateStub<IBusinessObjectWithIdentity>();
      businessObjectStub.Stub (_ => _.UniqueIdentifier).Return ("TheIdentifier");

      var permissionDeniedException = new PermissionDeniedException ("The Exception");
      BusinessObjectPropertyAccessException actualException;
      var actualResult = _strategy.IsPropertyAccessException (
          businessObjectStub,
          bindableProperty,
          permissionDeniedException,
          out actualException);

      Assert.That (actualResult, Is.True);

      Assert.That (actualException, Is.Not.Null);
      Assert.That (
          actualException.Message,
          Is.EqualTo (
              "A PermissionDeniedException occured while getting the value of property 'Scalar' for business object with ID 'TheIdentifier'."));
      Assert.That (actualException.InnerException, Is.SameAs (permissionDeniedException));
    }

    [Test]
    public void IsPropertyAccessException_WithOtherException_ReturnsFalse ()
    {
      var bindableProperty = CreateBindableProperty ((() => ((ClassWithReferenceType<string>) null).Scalar));

      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();

      var permissionDeniedException = new Exception ("The Exception");
      BusinessObjectPropertyAccessException actualException;
      var actualResult = _strategy.IsPropertyAccessException (
          businessObjectStub,
          bindableProperty,
          permissionDeniedException,
          out actualException);

      Assert.That (actualResult, Is.False);
      Assert.That (actualException, Is.Null);
    }

    private PropertyBase CreateBindableProperty<TPropertyType> (Expression<Func<TPropertyType>> propertyExpression)
    {
      return new StubPropertyBase (
          GetPropertyParameters (PropertyInfoAdapter.Create (MemberInfoFromExpressionUtility.GetProperty (propertyExpression))));
    }

    private PropertyBase.Parameters GetPropertyParameters (IPropertyInformation propertyInformation)
    {
      return new PropertyBase.Parameters (
          CreateBindableObjectProviderWithStubBusinessObjectServiceFactory(),
          propertyInformation,
          typeof (IBusinessObject),
          new Lazy<Type> (() => typeof (IBusinessObject)),
          null,
          false,
          false,
          MockRepository.GenerateStub<IDefaultValueStrategy>(),
          MockRepository.GenerateStub<IBindablePropertyReadAccessStrategy>(),
          MockRepository.GenerateStub<IBindablePropertyWriteAccessStrategy>(),
          SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>());
    }

    private void ExpectHasAccessOnObjectSecurityStrategy (bool expectedResult, Enum accessType)
    {
      _objectSecurityStrategyMock.Expect (
          _ => _.HasAccess (
              Arg.Is (_securityProviderStub),
              Arg.Is (_principalStub),
              Arg<IReadOnlyList<AccessType>>.List.Equal (new[] { AccessType.Get (accessType) })))
          .Return (expectedResult);
    }

    protected BindableObjectProvider CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ()
    {
      return new BindableObjectProvider (BindableObjectMetadataFactory.Create(), MockRepository.GenerateStub<IBusinessObjectServiceFactory>());
    }
  }
}