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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Mixins;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PropertyAccessorDataCacheTest : StandardMappingTest
  {
    private PropertyAccessorDataCache _orderCache;
    private PropertyAccessorDataCache _distributorCache;
    private PropertyAccessorDataCache _closedGenericClassCache;
    private PropertyAccessorDataCache _derivedClassWithMixedPropertiesCache;

    public override void SetUp ()
    {
      base.SetUp();
      _orderCache = CreatePropertyAccessorDataCache(typeof(Order));
      _distributorCache = CreatePropertyAccessorDataCache(typeof(Distributor));
      _closedGenericClassCache = CreatePropertyAccessorDataCache(typeof(ClosedGenericClassWithManySideRelationProperties));
      _derivedClassWithMixedPropertiesCache = CreatePropertyAccessorDataCache(typeof(DerivedClassWithDifferentProperties));
    }

    [Test]
    public void GetPropertyAccessorData_ValueProperty ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order) + ".OrderNumber");

      Assert.That(data, Is.Not.Null);
      Assert.That(data.PropertyIdentifier, Is.EqualTo(typeof(Order) + ".OrderNumber"));
      Assert.That(data.Kind, Is.EqualTo(PropertyKind.PropertyValue));
    }

    [Test]
    public void GetPropertyAccessorData_RelatedObjectProperty_RealSide ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order) + ".Customer");

      Assert.That(data, Is.Not.Null);
      Assert.That(data.PropertyIdentifier, Is.EqualTo(typeof(Order) + ".Customer"));
      Assert.That(data.Kind, Is.EqualTo(PropertyKind.RelatedObject));
      Assert.That(data.RelationEndPointDefinition.IsVirtual, Is.False);
    }

    [Test]
    public void GetPropertyAccessorData_RelatedObjectProperty_VirtualSide ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order) + ".OrderTicket");

      Assert.That(data, Is.Not.Null);
      Assert.That(data.PropertyIdentifier, Is.EqualTo(typeof(Order) + ".OrderTicket"));
      Assert.That(data.Kind, Is.EqualTo(PropertyKind.RelatedObject));
      Assert.That(data.RelationEndPointDefinition.IsVirtual, Is.True);
    }

    [Test]
    public void GetPropertyAccessorData_RelatedCollectionProperty ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order) + ".OrderItems");

      Assert.That(data, Is.Not.Null);
      Assert.That(data.PropertyIdentifier, Is.EqualTo(typeof(Order) + ".OrderItems"));
      Assert.That(data.Kind, Is.EqualTo(PropertyKind.RelatedObjectCollection));
      Assert.That(data.RelationEndPointDefinition.IsVirtual, Is.True);
    }

    [Test]
    public void GetPropertyAccessorData_Unknown ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order) + ".OrderSmell");

      Assert.That(data, Is.Null);
    }

    [Test]
    public void GetPropertyAccessorData_ItemsAreCached ()
    {
      var data1 = _orderCache.GetPropertyAccessorData(typeof(Order) + ".OrderNumber");
      var data2 = _orderCache.GetPropertyAccessorData(typeof(Order) + ".OrderNumber");

      Assert.That(data1, Is.Not.Null);
      Assert.That(data1, Is.SameAs(data2));
    }

    [Test]
    public void GetPropertyAccessorData_TypeAndShortName ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order), "OrderNumber");

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void GetPropertyAccessorData_TypeAndShortName_Unknown ()
    {
      var data = _orderCache.GetPropertyAccessorData(typeof(Order), "OrderSmell");

      Assert.That(data, Is.Null);
    }

    [Test]
    public void ResolvePropertyAccessorData_PropertyInformation ()
    {
      var propertyinformation = PropertyInfoAdapter.Create(typeof(Order).GetProperty("OrderNumber"));
      var data = _orderCache.ResolvePropertyAccessorData(propertyinformation);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void ResolvePropertyAccessorData_PropertyInformation_Mixin_ViaInterface ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache(GetTypeDefinition(typeof(TargetClassForPersistentMixin)));
      var propertyinformation = PropertyInfoAdapter.Create(typeof(IMixinAddingPersistentProperties).GetProperty("PersistentProperty"));
      var data = cacheWithMixins.ResolvePropertyAccessorData(propertyinformation);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(cacheWithMixins.GetPropertyAccessorData(typeof(MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void ResolvePropertyAccessorData_PropertyInformation_Mixin_ViaMixin ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache(GetTypeDefinition(typeof(TargetClassForPersistentMixin)));
      var propertyinformation = PropertyInfoAdapter.Create(typeof(MixinAddingPersistentProperties).GetProperty("PersistentProperty"));
      var data = cacheWithMixins.ResolvePropertyAccessorData(propertyinformation);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(cacheWithMixins.GetPropertyAccessorData(typeof(MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void ResolvePropertyAccessorData_PropertyInformation_Interface ()
    {
      var propertyinformation = PropertyInfoAdapter.Create(typeof(IOrder).GetProperty("OrderNumber"));
      var data = _orderCache.ResolvePropertyAccessorData(propertyinformation);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void ResolvePropertyAccessorData_PropertyInformation_UnknownOnThisObject ()
    {
      var propertyinformation = PropertyInfoAdapter.Create(typeof(OrderItem).GetProperty("Product"));
      var data = _orderCache.ResolvePropertyAccessorData(propertyinformation);

      Assert.That(data, Is.Null);
    }

    [Test]
    public void ResolvePropertyAccessorData_PropertyInformation_Unknown ()
    {
      var propertyinformation = PropertyInfoAdapter.Create(typeof(Order).GetProperty("NotInMapping"));
      var data = _orderCache.ResolvePropertyAccessorData(propertyinformation);

      Assert.That(data, Is.Null);
    }

    [Test]
    public void ResolvePropertyAccessorData_Expression ()
    {
      var data = _orderCache.ResolvePropertyAccessorData((Order o) => o.OrderNumber);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void ResolvePropertyAccessorData_Expression_Mixin_ViaInterface ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache(GetTypeDefinition(typeof(TargetClassForPersistentMixin)));
// ReSharper disable SuspiciousTypeConversion.Global
      var data = cacheWithMixins.ResolvePropertyAccessorData((TargetClassForPersistentMixin t) => ((IMixinAddingPersistentProperties)t).PersistentProperty);
// ReSharper restore SuspiciousTypeConversion.Global

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(cacheWithMixins.GetPropertyAccessorData(typeof(MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void ResolvePropertyAccessorData_Expression_Mixin_ViaMixin ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache(GetTypeDefinition(typeof(TargetClassForPersistentMixin)));
      var data = cacheWithMixins.ResolvePropertyAccessorData((TargetClassForPersistentMixin t) => (Mixin.Get<MixinAddingPersistentProperties>(t).PersistentProperty));

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(cacheWithMixins.GetPropertyAccessorData(typeof(MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void ResolvePropertyAccessorData_Expression_Interface ()
    {
      var data = _orderCache.ResolvePropertyAccessorData((Order o) => ((IOrder)o).OrderNumber);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void ResolvePropertyAccessorData_Expression_Unknown ()
    {
      var data = _orderCache.ResolvePropertyAccessorData((Order o) => o.NotInMapping);
      Assert.That(data, Is.Null);
    }

    [Test]
    public void ResolvePropertyAccessorData_Expression_UnknownOnThisObject ()
    {
      var data = _orderCache.ResolvePropertyAccessorData((Order o) => ((OrderItem)(object)o).Product);
      Assert.That(data, Is.Null);
    }

    [Test]
    public void GetPropertyAccessorData_InvalidExpression_NoMember ()
    {
      Assert.That(
          () => _orderCache.ResolvePropertyAccessorData((Order o) => o.OrderNumber + 5),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The expression must identify a property.", "propertyAccessExpression"));
    }

    [Test]
    public void GetPropertyAccessorData_InvalidExpression_Field ()
    {
      Assert.That(
          () => _orderCache.ResolvePropertyAccessorData((Order o) => o.CtorCalled),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo("The expression must identify a property.", "propertyAccessExpression"));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_FullPropertyName ()
    {
      var data = _orderCache.GetMandatoryPropertyAccessorData(typeof(Order).FullName + ".OrderNumber");

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_FullPropertyName_Unknown ()
    {
      Assert.That(
          () => _orderCache.GetMandatoryPropertyAccessorData(typeof(Order).FullName + ".OrderSmell"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property named "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderSmell'."));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_TypeAndShortName ()
    {
      var data = _orderCache.GetMandatoryPropertyAccessorData(typeof(Order), "OrderNumber");

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void GetMandatoryPropertyAccessorData_TypeAndShortName_Unknown ()
    {
      Assert.That(
          () => _orderCache.GetMandatoryPropertyAccessorData(typeof(Order), "OrderSmell"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property named "
                  + "'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderSmell'."));
    }

    [Test]
    public void ResolveMandatoryPropertyAccessorData_Expression ()
    {
      var data = _orderCache.ResolveMandatoryPropertyAccessorData((Order o) => o.OrderNumber);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void ResolveMandatoryPropertyAccessorData_Expression_Mixin_ViaInterface ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache(GetTypeDefinition(typeof(TargetClassForPersistentMixin)));
      // ReSharper disable SuspiciousTypeConversion.Global
      var data = cacheWithMixins.ResolveMandatoryPropertyAccessorData((TargetClassForPersistentMixin t) => ((IMixinAddingPersistentProperties)t).PersistentProperty);
      // ReSharper restore SuspiciousTypeConversion.Global

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(cacheWithMixins.GetPropertyAccessorData(typeof(MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void ResolveMandatoryPropertyAccessorData_Expression_Mixin_ViaMixin ()
    {
      var cacheWithMixins = new PropertyAccessorDataCache(GetTypeDefinition(typeof(TargetClassForPersistentMixin)));
      var data = cacheWithMixins.ResolveMandatoryPropertyAccessorData((TargetClassForPersistentMixin t) => (Mixin.Get<MixinAddingPersistentProperties>(t).PersistentProperty));

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(cacheWithMixins.GetPropertyAccessorData(typeof(MixinAddingPersistentProperties).FullName + ".PersistentProperty")));
    }

    [Test]
    public void ResolveMandatoryPropertyAccessorData_Expression_Interface ()
    {
      var data = _orderCache.ResolveMandatoryPropertyAccessorData((Order o) => ((IOrder)o).OrderNumber);

      Assert.That(data, Is.Not.Null);
      Assert.That(data, Is.EqualTo(_orderCache.GetPropertyAccessorData(typeof(Order).FullName + ".OrderNumber")));
    }

    [Test]
    public void ResolveMandatoryPropertyAccessorData_Expression_Unknown ()
    {
      Assert.That(
          () => _orderCache.ResolveMandatoryPropertyAccessorData((Order o) => o.NotInMapping),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property identified by expression "
                  + "'o => o.NotInMapping'."));
    }

    [Test]
    public void ResolveMandatoryPropertyAccessorData_Expression_UnknownOnThisObject ()
    {
      Assert.That(
          () => _orderCache.ResolveMandatoryPropertyAccessorData((Order o) => ((OrderItem)(object)o).Product),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property identified by expression "
                  + "'o => Convert(Convert(o, Object), OrderItem).Product'."
                  ));
    }

    [Test]
    public void FindPropertyAccessorData_Property ()
    {
      var result = _orderCache.FindPropertyAccessorData(typeof(Order), "OrderNumber");

      var expected = _orderCache.GetMandatoryPropertyAccessorData(typeof(Order), "OrderNumber");
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FindPropertyAccessorData_VirtualRelationEndPoint ()
    {
      var result = _orderCache.FindPropertyAccessorData(typeof(Order), "OrderItems");

      var expected = _orderCache.GetMandatoryPropertyAccessorData(typeof(Order), "OrderItems");
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FindPropertyAccessorData_FromDerivedType ()
    {
      Assert.That(_distributorCache.GetPropertyAccessorData(typeof(Distributor), "ContactPerson"), Is.Null);

      var result = _distributorCache.FindPropertyAccessorData(typeof(Distributor), "ContactPerson");

      var expected = _distributorCache.GetMandatoryPropertyAccessorData(typeof(Partner), "ContactPerson");
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FindPropertyAccessorData_FromGenericType ()
    {
      Assert.That(
          _closedGenericClassCache.GetPropertyAccessorData(typeof(ClosedGenericClassWithManySideRelationProperties), "BaseUnidirectional"),
          Is.Null);

      var result = _closedGenericClassCache.FindPropertyAccessorData(typeof(ClosedGenericClassWithManySideRelationProperties), "BaseUnidirectional");

      var expected = _closedGenericClassCache.GetMandatoryPropertyAccessorData(
          typeof(GenericClassWithManySideRelationPropertiesNotInMapping<>),
          "BaseUnidirectional");
      Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void FindPropertyAccessorData_WithShadowedProperty ()
    {
      var shadowingResult = _derivedClassWithMixedPropertiesCache.FindPropertyAccessorData(typeof(DerivedClassWithDifferentProperties), "String");
      var shadowingExpected =
          _derivedClassWithMixedPropertiesCache.GetMandatoryPropertyAccessorData(typeof(DerivedClassWithDifferentProperties), "String");
      Assert.That(shadowingResult, Is.EqualTo(shadowingExpected));

      var shadowedResult = _derivedClassWithMixedPropertiesCache.FindPropertyAccessorData(typeof(ClassWithDifferentProperties), "String");
      var shadowedExpected = _derivedClassWithMixedPropertiesCache.GetMandatoryPropertyAccessorData(typeof(ClassWithDifferentProperties), "String");
      Assert.That(shadowedResult, Is.EqualTo(shadowedExpected));
    }

    [Test]
    public void FindPropertyAccessorData_NonExistingProperty ()
    {
      var result = _distributorCache.FindPropertyAccessorData(typeof(Distributor), "Frobbers");
      Assert.That(result, Is.Null);
    }

    private PropertyAccessorDataCache CreatePropertyAccessorDataCache (Type classType)
    {
      return new PropertyAccessorDataCache(MappingConfiguration.Current.GetTypeDefinition(classType));
    }
  }
}
