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
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.BindableObjectMixinTests
{
  [TestFixture]
  public class GetBusinessObjectClass : TestBase
  {
    private SimpleBusinessObjectClass _bindableObject;
    private BindableObjectMixin _bindableObjectMixin;
    private IBusinessObject _businessObject;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObject = ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty);
      _bindableObjectMixin = Mixin.Get<BindableObjectMixin>(_bindableObject);
      _businessObject = _bindableObjectMixin;
    }

    [Test]
    public void FromClass ()
    {
      Assert.That(_bindableObjectMixin.BusinessObjectClass, Is.Not.Null);
      Assert.That(_bindableObjectMixin.BusinessObjectClass.TargetType, Is.SameAs(typeof(SimpleBusinessObjectClass)));
      Assert.That(
          _bindableObjectMixin.BusinessObjectClass.BusinessObjectProvider,
          Is.SameAs(BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass))));
    }

    [Test]
    public void FromInterface ()
    {
      Assert.That(_businessObject.BusinessObjectClass, Is.Not.Null);
      Assert.That(_businessObject.BusinessObjectClass, Is.SameAs(_bindableObjectMixin.BusinessObjectClass));
      Assert.That(
          _businessObject.BusinessObjectClass.BusinessObjectProvider,
          Is.SameAs(BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass))));
    }

    [Test]
    public void LazyInitialization_KeepsProviderOfConstructionTime ()
    {
      var objectCreatedBefore = ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty);
      var providerBefore = BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass));

      BindableObjectProvider.SetProvider(typeof(BindableObjectProviderAttribute), null);

      var providerAfter = BindableObjectProvider.GetProviderForBindableObjectType(typeof(SimpleBusinessObjectClass));
      Assert.That(providerAfter, Is.Not.SameAs(providerBefore));
      var objectCreatedAfter = ObjectFactory.Create<SimpleBusinessObjectClass>(ParamList.Empty);

      Assert.That(((IBusinessObject)objectCreatedAfter).BusinessObjectClass.BusinessObjectProvider, Is.SameAs(providerAfter));
      Assert.That(((IBusinessObject)objectCreatedBefore).BusinessObjectClass.BusinessObjectProvider, Is.SameAs(providerBefore));
    }

    [Test]
    public void LazyInitialization_KeepsMixinConfigurationOfConstructionTime ()
    {
      IBusinessObject businessObject;
      using (MixinConfiguration.BuildNew()
          .ForClass<BaseBusinessObjectClass>().AddMixin<MixinAddingProperty>()
          .ForClass<BaseBusinessObjectClass>().AddMixin<BindableObjectMixin>()
          .EnterScope())
      {
        businessObject = (IBusinessObject)ObjectFactory.Create<DerivedBusinessObjectClass>(ParamList.Empty);
        Assert.That(businessObject, Is.InstanceOf(typeof(IMixinAddingProperty)));
      }

      // lazy initialization here - should use mixin configuration from above
      var businessObjectClass = (BindableObjectClass)businessObject.BusinessObjectClass;
      Assert.That(businessObjectClass.GetPropertyDefinition("MixedProperty"), Is.Not.Null);
    }
  }
}
