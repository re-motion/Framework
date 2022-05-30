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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectDefaultValueStrategyTest : ObjectBindingTestBase
  {
    [Test]
    public void IsDefaultDefault_StateIsNotNew ()
    {
      var instance = SampleBindableDomainObject.NewObject();
      LifetimeService.DeleteObject(instance.DefaultTransactionContext.ClientTransaction, instance);

      var property = GetProperty(instance);
      var strategy = (IDefaultValueStrategy)new BindableDomainObjectDefaultValueStrategy();
      Assert.That(instance.State.IsNew, Is.False);

      var result = strategy.IsDefaultValue(instance, property);

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsDefaultDefault_StateIsNewAndPropertyHasBeenTouched ()
    {
      var instance = SampleBindableDomainObject.NewObject();

      var property = GetProperty(instance);
      var strategy = (IDefaultValueStrategy)new BindableDomainObjectDefaultValueStrategy();
      instance.Name = instance.Name;
      Assert.That(instance.State.IsNew, Is.True);

      var result = strategy.IsDefaultValue(instance, property);

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsDefaultDefault_StateIsNewAndPropertyHasNotBeenTouched ()
    {
      var instance = SampleBindableDomainObject.NewObject();

      var property = GetProperty(instance);
      var strategy = (IDefaultValueStrategy)new BindableDomainObjectDefaultValueStrategy();
      Assert.That(instance.State.IsNew, Is.True);

      var result = strategy.IsDefaultValue(instance, property);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsDefaultDefault_StateIsNewAndPropertyCannotBeResolved ()
    {
      var strategy = (IDefaultValueStrategy)new BindableDomainObjectDefaultValueStrategy();
      var instance = SampleBindableDomainObject.NewObject();

      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(bool));
      propertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInformationStub.Setup(stub => stub.DeclaringType).Returns(TypeAdapter.Create(typeof(bool)));
      propertyInformationStub.Setup(stub => stub.GetOriginalDeclaringType()).Returns(TypeAdapter.Create(typeof(bool)));
      var property = CreateProperty(propertyInformationStub.Object);

      var result = strategy.IsDefaultValue(instance, property);

      Assert.That(result, Is.False);
    }

    private PropertyBase GetProperty (IBusinessObject instance)
    {
      return (PropertyBase)instance.BusinessObjectClass.GetPropertyDefinition("Name");
    }

    private BooleanProperty CreateProperty (IPropertyInformation propertyInformation)
    {
      var businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      return new BooleanProperty(GetPropertyParameters(propertyInformation, businessObjectProvider));
    }

    private BindableObjectProvider CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ()
    {
      return new BindableObjectProvider(BindableObjectMetadataFactory.Create(), new Mock<IBusinessObjectServiceFactory>().Object);
    }

    private PropertyBase.Parameters GetPropertyParameters (IPropertyInformation property, BindableObjectProvider provider)
    {
      PropertyReflector reflector = PropertyReflector.Create(property, provider);
      return (PropertyBase.Parameters)PrivateInvoke.InvokeNonPublicMethod(
          reflector, typeof(PropertyReflector), "CreateParameters", GetUnderlyingType(reflector));
    }

    private Type GetUnderlyingType (PropertyReflector reflector)
    {
      return (Type)PrivateInvoke.InvokeNonPublicMethod(reflector, typeof(PropertyReflector), "GetUnderlyingType");
    }
  }
}
