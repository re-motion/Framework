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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class GetReferenceClass : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;
    private BindableObjectProvider _bindableObjectWithIdentityProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectWithIdentityProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProvider);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute>(_bindableObjectWithIdentityProvider);
    }

    [Test]
    public void UseBindableObjectProvider ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty(
          new PropertyBase.Parameters(
              _bindableObjectProvider,
              GetPropertyInfo(typeof(ClassWithReferenceType<ClassWithIdentity>), "Scalar"),
              typeof(ClassWithIdentity),
              new Lazy<Type>(() => TypeFactory.GetConcreteType(typeof(ClassWithIdentity))),
              null,
              true,
              false,
              false,
              new BindableObjectDefaultValueStrategy(),
              new Mock<IBindablePropertyReadAccessStrategy>().Object,
              new Mock<IBindablePropertyWriteAccessStrategy>().Object,
              SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
              new Mock<IBusinessObjectPropertyConstraintProvider>().Object));

      Assert.That(property.ReferenceClass, Is.SameAs(BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(ClassWithIdentity))));
      Assert.That(
          property.BusinessObjectProvider,
          Is.SameAs(BindableObjectProvider.GetProviderForBindableObjectType(typeof(ClassWithReferenceType<ClassWithIdentity>))));
      Assert.That(
          property.ReferenceClass.BusinessObjectProvider,
          Is.SameAs(BindableObjectProvider.GetProviderForBindableObjectType(typeof(ClassWithIdentity))));
      Assert.That(property.ReferenceClass.BusinessObjectProvider, Is.Not.SameAs(property.BusinessObjectProvider));
    }

    [Test]
    public void UseBindableObjectProvider_WithBaseClass ()
    {
      IBusinessObjectReferenceProperty property = new ReferenceProperty(
          new PropertyBase.Parameters(
              _bindableObjectProvider,
              GetPropertyInfo(typeof(ClassWithReferenceToClassDerivedFromBindableObjectBase), "ScalarReference"),
              typeof(ClassDerivedFromBindableObjectBase),
              new Lazy<Type>(() => typeof(ClassDerivedFromBindableObjectBase)),
              null,
              true,
              false,
              false,
              new BindableObjectDefaultValueStrategy(),
              new Mock<IBindablePropertyReadAccessStrategy>().Object,
              new Mock<IBindablePropertyWriteAccessStrategy>().Object,
              SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(),
              new Mock<IBusinessObjectPropertyConstraintProvider>().Object));

      Assert.That(property.ReferenceClass, Is.SameAs(BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(ClassDerivedFromBindableObjectBase))));
      Assert.That(
          property.BusinessObjectProvider,
          Is.SameAs(BindableObjectProvider.GetProviderForBindableObjectType(typeof(ClassWithReferenceToClassDerivedFromBindableObjectBase))));
      Assert.That(
          property.ReferenceClass.BusinessObjectProvider,
          Is.SameAs(BindableObjectProvider.GetProviderForBindableObjectType(typeof(ClassDerivedFromBindableObjectBase))));
      Assert.That(property.ReferenceClass.BusinessObjectProvider, Is.SameAs(property.BusinessObjectProvider));
    }

    [Test]
    public void UseBusinessObjectClassService ()
    {
      var mockService = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var expectedClass = new Mock<IBusinessObjectClass>();
      var businessObjectFromOtherBusinessObjectProvider = new Mock<IBusinessObject>();
      Type typeFromOtherBusinessObjectProvider = businessObjectFromOtherBusinessObjectProvider.Object.GetType();
      IBusinessObjectReferenceProperty property = CreateProperty("Scalar", typeFromOtherBusinessObjectProvider);

      mockService.Setup(_ => _.GetBusinessObjectClass(typeFromOtherBusinessObjectProvider)).Returns(expectedClass.Object).Verifiable();

      _bindableObjectProvider.AddService(typeof(IBusinessObjectClassService), mockService.Object);
      IBusinessObjectClass actualClass = property.ReferenceClass;

      mockService.Verify();
      Assert.That(actualClass, Is.SameAs(expectedClass.Object));
    }

    [Test]
    public void UseBusinessObjectClassService_WithoutService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty("Scalar", typeof(ClassFromOtherBusinessObjectImplementation));
      Assert.That(
          () => property.ReferenceClass,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The 'Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain.ClassFromOtherBusinessObjectImplementation' "
                  + "type does not use the 'Remotion.ObjectBinding.BindableObject' implementation of 'Remotion.ObjectBinding.IBusinessObject' and there is no "
                  + "'Remotion.ObjectBinding.IBusinessObjectClassService' registered with the 'Remotion.ObjectBinding.BusinessObjectProvider' associated with this type."));
    }

    [Test]
    public void UseBusinessObjectClassService_WithServiceReturningNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty("Scalar", typeof(ClassFromOtherBusinessObjectImplementation));

      _bindableObjectProvider.AddService(typeof(IBusinessObjectClassService), new StubBusinessObjectClassService());
      Assert.That(
          () => property.ReferenceClass,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The GetBusinessObjectClass method of 'Remotion.ObjectBinding.UnitTests.TestDomain.StubBusinessObjectClassService', registered "
                  + "with the 'Remotion.ObjectBinding.BindableObject.BindableObjectProvider', failed to return an 'Remotion.ObjectBinding.IBusinessObjectClass' "
                  + "for type 'Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain.ClassFromOtherBusinessObjectImplementation'."));
    }

    private ReferenceProperty CreateProperty (string propertyName, Type propertyType)
    {
      return new ReferenceProperty(
          GetPropertyParameters(
              GetPropertyInfo(typeof(ClassWithReferenceType<>).MakeGenericType(propertyType), propertyName), _bindableObjectProvider));
    }
  }
}
