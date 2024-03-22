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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Security.BindableObject;
using Remotion.ObjectBinding.Security.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Security.UnitTests.BindableObject
{
  [TestFixture]
  public class SecurityBasedBindablePropertyReadAccessStrategyTest : TestBase
  {
    private SecurableClassWithReferenceType<string> _securableObject;
    private ServiceLocatorScope _serviceLocatorScope;
    private Mock<ISecurityProvider> _securityProviderStub;
    private Mock<ISecurityPrincipal> _principalStub;
    private Mock<IObjectSecurityStrategy> _objectSecurityStrategyMock;
    private SecurityBasedBindablePropertyReadAccessStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp();

      _objectSecurityStrategyMock = new Mock<IObjectSecurityStrategy>(MockBehavior.Strict);

      _securableObject = new SecurableClassWithReferenceType<string>(_objectSecurityStrategyMock.Object);

      _securityProviderStub = new Mock<ISecurityProvider>();

      _principalStub = new Mock<ISecurityPrincipal>();
      var principalProviderStub = new Mock<IPrincipalProvider>();
      principalProviderStub.Setup(_ => _.GetPrincipal()).Returns(_principalStub.Object);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _securityProviderStub.Object);
      serviceLocator.RegisterSingle(() => principalProviderStub.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

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
      var bindableProperty = CreateBindableProperty((() => ((ClassWithReferenceType<string>)null).Scalar));

      var result = _strategy.CanRead(null, bindableProperty);

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanRead_WithNonSecurableObject_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty((() => ((ClassWithReferenceType<string>)null).Scalar));

      var result = _strategy.CanRead(new ClassWithReferenceType<string>(), bindableProperty);

      Assert.That(result, Is.True);
    }

    [Test]
    public void CanRead_WithSecurableObject_EvaluatesObjectSecurityStratey_ReturnsResult ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      ExpectHasAccessOnObjectSecurityStrategy(expectedResult, TestAccessTypes.TestRead);

      var bindableProperty = CreateBindableProperty(() => ((SecurableClassWithReferenceType<string>)null).CustomPermissisons);

      var actualResult = _strategy.CanRead(_securableObject, bindableProperty);

      Assert.That(actualResult, Is.EqualTo(expectedResult));
      _objectSecurityStrategyMock.Verify();
    }

    [Test]
    public void CanRead_WithSecurableObject_WithoutSetter_UsesNullMethodInfo_ReturnsResult ()
    {
      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      ExpectHasAccessOnObjectSecurityStrategy(expectedResult, GeneralAccessTypes.Read);

      var bindableProperty = new StubPropertyBase(
          GetPropertyParameters(PropertyInfoAdapter.Create(typeof(ClassWithReferenceType<string>).GetProperty("PropertyWithNoGetter"))));

      var actualResult = _strategy.CanRead(_securableObject, bindableProperty);

      Assert.That(actualResult, Is.EqualTo(expectedResult));
      _objectSecurityStrategyMock.Verify();
    }

    [Test]
    public void IsPropertyAccessException_WithPermissionDeniedException_WithIBusinessObject_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty((() => ((ClassWithReferenceType<string>)null).Scalar));

      var businessObjectClassStub = new Mock<IBusinessObjectClass>();
      businessObjectClassStub.Setup(_ => _.Identifier).Returns("TheClass");

      var businessObjectStub = new Mock<IBusinessObject>();
      businessObjectStub.Setup(_ => _.BusinessObjectClass).Returns(businessObjectClassStub.Object);

      var permissionDeniedException = new PermissionDeniedException("The Exception");
      BusinessObjectPropertyAccessException actualException;
      var actualResult = _strategy.IsPropertyAccessException(
          businessObjectStub.Object,
          bindableProperty,
          permissionDeniedException,
          out actualException);

      Assert.That(actualResult, Is.True);

      Assert.That(actualException, Is.Not.Null);
      Assert.That(
          actualException.Message,
          Is.EqualTo(
              "A PermissionDeniedException occured while getting the value of property 'Scalar' for business object type 'TheClass'."));
      Assert.That(actualException.InnerException, Is.SameAs(permissionDeniedException));
    }

    [Test]
    public void IsPropertyAccessException_WithPermissionDeniedException_WithIBusinessObjectWithIdentity_ReturnsTrue ()
    {
      var bindableProperty = CreateBindableProperty((() => ((ClassWithReferenceType<string>)null).Scalar));

      var businessObjectStub = new Mock<IBusinessObjectWithIdentity>();
      businessObjectStub.Setup(_ => _.UniqueIdentifier).Returns("TheIdentifier");

      var permissionDeniedException = new PermissionDeniedException("The Exception");
      BusinessObjectPropertyAccessException actualException;
      var actualResult = _strategy.IsPropertyAccessException(
          businessObjectStub.Object,
          bindableProperty,
          permissionDeniedException,
          out actualException);

      Assert.That(actualResult, Is.True);

      Assert.That(actualException, Is.Not.Null);
      Assert.That(
          actualException.Message,
          Is.EqualTo(
              "A PermissionDeniedException occured while getting the value of property 'Scalar' for business object with ID 'TheIdentifier'."));
      Assert.That(actualException.InnerException, Is.SameAs(permissionDeniedException));
    }

    [Test]
    public void IsPropertyAccessException_WithOtherException_ReturnsFalse ()
    {
      var bindableProperty = CreateBindableProperty((() => ((ClassWithReferenceType<string>)null).Scalar));

      var businessObjectStub = new Mock<IBusinessObject>();

      var permissionDeniedException = new Exception("The Exception");
      BusinessObjectPropertyAccessException actualException;
      var actualResult = _strategy.IsPropertyAccessException(
          businessObjectStub.Object,
          bindableProperty,
          permissionDeniedException,
          out actualException);

      Assert.That(actualResult, Is.False);
      Assert.That(actualException, Is.Null);
    }

    private PropertyBase CreateBindableProperty<TPropertyType> (Expression<Func<TPropertyType>> propertyExpression)
    {
      return new StubPropertyBase(
          GetPropertyParameters(PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty(propertyExpression))));
    }

    private PropertyBase.Parameters GetPropertyParameters (IPropertyInformation propertyInformation)
    {
      return new PropertyBase.Parameters(
          CreateBindableObjectProviderWithStubBusinessObjectServiceFactory(),
          propertyInformation,
          typeof(IBusinessObject),
          new Lazy<Type>(() => typeof(IBusinessObject)),
          null,
          true,
          false,
          false,
          new Mock<IDefaultValueStrategy>().Object,
          new Mock<IBindablePropertyReadAccessStrategy>().Object,
          new Mock<IBindablePropertyWriteAccessStrategy>().Object,
          SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
          new Mock<IBusinessObjectPropertyConstraintProvider>().Object);
    }

    private void ExpectHasAccessOnObjectSecurityStrategy (bool expectedResult, Enum accessType)
    {
      _objectSecurityStrategyMock
          .Setup(_ => _.HasAccess(_securityProviderStub.Object, _principalStub.Object, new[] { AccessType.Get(accessType) }))
          .Returns(expectedResult)
          .Verifiable();
    }

    protected BindableObjectProvider CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ()
    {
      return new BindableObjectProvider(BindableObjectMetadataFactory.Create(), new Mock<IBusinessObjectServiceFactory>().Object);
    }
  }
}
