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
using ParamList = Remotion.TypePipe.ParamList;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DefaultValueTest : ObjectBindingTestBase
  {
    private SampleBindableMixinDomainObject _loadedObject;
    private SampleBindableMixinDomainObject _newObject;
    private IBusinessObject _loadedBusinessObject;
    private IBusinessObject _newBusinessOrder;

    private ClientTransactionScope _subTxScope;

    public override void SetUp ()
    {
      base.SetUp();

      var objectID = SampleBindableMixinDomainObject.NewObject().ID;

      var subTx = TestableClientTransaction.CreateSubTransaction();
      _subTxScope = subTx.EnterDiscardingScope();

      _loadedObject = objectID.GetObject<SampleBindableMixinDomainObject>();
      _loadedBusinessObject = (IBusinessObject)_loadedObject;

      _newObject = SampleBindableMixinDomainObject.NewObject();
      _newBusinessOrder = (IBusinessObject)_newObject;
    }

    public override void TearDown ()
    {
      _subTxScope.Leave();
      base.TearDown();
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultValue_OnExistingObject ()
    {
      Assert.That(_loadedBusinessObject.GetProperty("Int32"), Is.Not.Null);
      Assert.That(_loadedBusinessObject.GetProperty("Int32"), Is.EqualTo(_loadedObject.Int32));
    }

    [Test]
    public void GetPropertyReturnsNullIfDefaultValue_OnNewObject ()
    {
      Assert.That(_newBusinessOrder.GetProperty("Int32"), Is.Null);
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultListValue_OnNewObject ()
    {
      Assert.That(_newBusinessOrder.GetProperty("List"), Is.Not.Null);
    }

    [Test]
    public void GetPropertyReturnsNonNullIfDefaultValue_OnNewObjectInSubtransaction ()
    {
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That(_newBusinessOrder.GetProperty("Int32"), Is.Not.Null);
        Assert.That(_newBusinessOrder.GetProperty("Int32"), Is.EqualTo(_newObject.Int32));
      }
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue_OnExistingObject ()
    {
      _loadedObject.Int32 = _loadedObject.Int32;
      Assert.That(_loadedBusinessObject.GetProperty("Int32"), Is.Not.Null);
    }

    [Test]
    public void GetPropertyReturnsNonNullIfNonDefaultValue_OnNewObject ()
    {
      _newObject.Int32 = _newObject.Int32;
      Assert.That(_newBusinessOrder.GetProperty("Int32"), Is.Not.Null);
    }

    [Test]
    public void GetPropertyDefaultForNonMappingProperties ()
    {
      var businessObject = (IBusinessObject)LifetimeService.NewObject(ClientTransaction.Current, typeof(BindableDomainObjectWithProperties), ParamList.Empty);
      Assert.That(businessObject.GetProperty("RequiredPropertyNotInMapping"), Is.Not.Null);
      Assert.That(businessObject.GetProperty("RequiredPropertyNotInMapping"), Is.EqualTo(true));
    }

    [Test]
    public void GetProperty_CustomIPropertyInformationImplementation ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(bool));
      propertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns(MethodInfoAdapter.Create(typeof(object).GetMethod("ToString")));

      var booleanProperty = CreateProperty(propertyInformationStub.Object);

      var result = _newBusinessOrder.GetProperty(booleanProperty);

      Assert.That(result.ToString().StartsWith("SampleBindableMixinDomainObject"), Is.True);
    }

    [Test]
    public void PropertyWithNoGetter ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(bool));
      propertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInformationStub.Setup(stub => stub.GetGetMethod(true)).Returns((IMethodInformation)null);

      var booleanProperty = CreateProperty(propertyInformationStub.Object);
      Assert.That(
          () => _newBusinessOrder.GetProperty(booleanProperty),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Property has no getter."));
    }

    [Test]
    public void PropertyWithNoSetter ()
    {
      var propertyInformationStub = new Mock<IPropertyInformation>();
      propertyInformationStub.Setup(stub => stub.PropertyType).Returns(typeof(bool));
      propertyInformationStub.Setup(stub => stub.GetIndexParameters()).Returns(new ParameterInfo[0]);
      propertyInformationStub.Setup(stub => stub.GetSetMethod(true)).Returns((IMethodInformation)null);

      var booleanProperty = CreateProperty(propertyInformationStub.Object);
      Assert.That(
          () => _newBusinessOrder.SetProperty(booleanProperty, new object()),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Property has no setter."));
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
