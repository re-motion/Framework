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
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class InterceptedPropertyIntegrationTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      Assert.That(CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    public override void TearDown ()
    {
      base.TearDown();
      Assert.That(CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    [Test]
    public void LoadOfSimpleObjectWorks ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(WasCreatedByFactory(order), Is.True);
    }

    [Test]
    public void ConstructionOfSimpleObjectWorks ()
    {
      Order order = Order.NewObject();
      Assert.That(WasCreatedByFactory(order), Is.True);

      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That(classWithAllDataTypes, Is.Not.Null);
      Assert.That(WasCreatedByFactory(classWithAllDataTypes), Is.True);
    }

    [Test]
    public void ConstructedObjectIsDerived ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That(classWithAllDataTypes, Is.Not.TypeOf<ClassWithAllDataTypes>());
      Assert.That(classWithAllDataTypes, Is.InstanceOf<ClassWithAllDataTypes>());
    }

    [Test]
    public void GetPropertyValueWorks ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(order.OrderNumber, Is.EqualTo(1));
      Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(2005, 01, 01)));
      Assert.That(order.OrderNumber, Is.EqualTo(1));
    }

    [Test]
    public void GetPropertyValue_WithNullAndAbstractProperty ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      Assert.That(classWithAllDataTypes.StringWithNullValueProperty, Is.Null);
    }

    [Test]
    public void SetPropertyValueWorks ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();

      order.OrderNumber = 15;
      Assert.That(order.OrderNumber, Is.EqualTo(15));

      order.DeliveryDate = new DateTime(2007, 02, 03);
      Assert.That(order.DeliveryDate, Is.EqualTo(new DateTime(2007, 02, 03)));

      Assert.That(order.OrderNumber, Is.EqualTo(15));
    }

    [Test]
    public void SetPropertyValue_WithNullAndAbstractProperty ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject();
      classWithAllDataTypes.StringWithNullValueProperty = null;
      Assert.That(classWithAllDataTypes.StringWithNullValueProperty, Is.Null);
    }

    [Test]
    public void SetPropertyValue_WithCollectionSet ()
    {
      ClassWithAbstractRelatedCollectionSetter instance = ClassWithAbstractRelatedCollectionSetter.NewObject();
      var newRelatedObjects = new ObjectList<ClassWithAbstractRelatedCollectionSetter>();
      instance.RelatedObjects = newRelatedObjects;
      Assert.That(instance.RelatedObjects, Is.SameAs(newRelatedObjects));
    }


    [Test]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      Assert.That(
          () => NonInstantiableAbstractClass.NewObject(),
          NUnit.Framework.Throws.InstanceOf<NonInterceptableTypeException>()
              .With.Message.EqualTo(
                  "Cannot instantiate type Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClass "
                  + "as its member Foo (on type NonInstantiableAbstractClass) is abstract (and not an "
                  + "automatic property)."));
    }

    [Test]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      Assert.That(
          () => NonInstantiableAbstractClassWithProps.NewObject(),
          NUnit.Framework.Throws.InstanceOf<NonInterceptableTypeException>()
              .With.Message.EqualTo(
                  "Cannot instantiate type Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClassWithProps "
                  + "as its member get_Foo (on type NonInstantiableAbstractClassWithProps) is abstract (and not an automatic property)."));
    }

    [Test]
    public void ClassWithMixinWithAutoPropertiesCannotBeInstantiated ()
    {
      Assert.That(
          () => NonInstantiableClassWithMixinWithPersistentAutoProperties.NewObject(),
          NUnit.Framework.Throws.InstanceOf<NonInterceptableTypeException>()
              .With.Message.EqualTo(
                  "Cannot instantiate type "
                  + "'Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe.InterceptedPropertyIntegrationTest+NonInstantiableClassWithMixinWithPersistentAutoProperties' "
                  + "because the mixin member 'MixinWithAutoProperties.PersistentAutoProperty' is an automatic property. Mixins must implement their persistent "
                  + "members by using 'Properties' to get and set property values."));
    }

    [Test]
    public void SealedCannotBeInstantiated ()
    {
      Assert.That(
          () => NonInstantiableSealedClass.NewObject(),
          NUnit.Framework.Throws.InstanceOf<NonInterceptableTypeException>()
              .With.Message.EqualTo(
                  "Cannot instantiate type 'Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe.InterceptedPropertyIntegrationTest+NonInstantiableSealedClass' as it is sealed."));
    }

    [Test]
    public void WrongConstructorCannotBeInstantiated ()
    {
      Assert.That(
          () => LifetimeService.NewObject(TestableClientTransaction, typeof(Order), ParamList.Create("foo", "bar", "foobar", (object)null)),
          NUnit.Framework.Throws.InstanceOf<MissingMethodException>()
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.TestDomain."
                  + "Order' does not contain a constructor with the following signature: (String, String, String, Object)."));
    }

    [Test]
    public void ConstructorThrowIsPropagated ()
    {
      Assert.That(
          () => Throws.NewObject(),
          NUnit.Framework.Throws.Exception
              .With.Message.EqualTo("Thrown in ThrowException()"));
    }

    [Test]
    public void ConstructorMismatch1 ()
    {
      Assert.That(
          () => ClassWithWrongConstructor.NewObject(),
          NUnit.Framework.Throws.InstanceOf<MissingMethodException>()
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe.InterceptedPropertyIntegrationTest+ClassWithWrongConstructor' does not contain a "
                  + "constructor with the following signature: ()."));
    }

    [Test]
    public void ConstructorMismatch2 ()
    {
      Assert.That(
          () => ClassWithWrongConstructor.NewObject(3.0),
          NUnit.Framework.Throws.InstanceOf<MissingMethodException>()
              .With.Message.EqualTo(
                  "Type 'Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe.InterceptedPropertyIntegrationTest+ClassWithWrongConstructor' does not contain a "
                  + "constructor with the following signature: (Double)."));
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Customer customer = order.Customer;
      Assert.That(customer, Is.Not.Null);
      Assert.That(customer, Is.SameAs(DomainObjectIDs.Customer1.GetObject<Customer>()));

      Customer newCustomer = Customer.NewObject();
      Assert.That(newCustomer, Is.Not.Null);
      order.Customer = newCustomer;
      Assert.That(order.Customer, Is.SameAs(newCustomer));

      Assert.That(order.OriginalCustomer, Is.SameAs(customer));
    }

    [Test]
    public void GetSetRelatedObjectAndOriginal_WithNullAndAutomaticProperty ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.IsNotEmpty(order.OrderItems);
      OrderItem orderItem = order.OrderItems[0];

      orderItem.Order = null;
      Assert.That(orderItem.Order, Is.Null);

      Assert.That(orderItem.OriginalOrder, Is.SameAs(order));
    }

    [Test]
    public void GetRelatedObjects ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      DomainObjectCollection orderItems = order.OrderItems;
      Assert.That(orderItems, Is.Not.Null);
      Assert.That(orderItems.Count, Is.EqualTo(2));

      Assert.That(orderItems.Contains(DomainObjectIDs.OrderItem1), Is.True);
      Assert.That(orderItems.Contains(DomainObjectIDs.OrderItem2), Is.True);

      OrderItem newItem = OrderItem.NewObject();
      order.OrderItems.Add(newItem);

      Assert.That(order.OrderItems.ContainsObject(newItem), Is.True);
    }

    [Test]
    public void GetRelatedObjects_WithAutomaticProperties ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      Assert.That(order.OrderItems, Is.Not.Null);
      Assert.That(order.OrderItems.Count, Is.EqualTo(2));

      Assert.That(order.OrderItems.Contains(DomainObjectIDs.OrderItem1), Is.True);
      Assert.That(order.OrderItems.Contains(DomainObjectIDs.OrderItem2), Is.True);

      OrderItem newItem = OrderItem.NewObject();
      order.OrderItems.Add(newItem);

      Assert.That(order.OrderItems.ContainsObject(newItem), Is.True);
    }

    [Test]
    public void PropertyAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Assert.That(
          () => order.NotInMapping,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }

    [Test]
    public void RelatedAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Assert.That(
          () => order.NotInMappingRelated,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }

    [Test]
    public void RelatedObjectsAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Assert.That(
          () => order.NotInMappingRelatedObjects,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }


    [Test]
    public void PropertySetAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Assert.That(
          () => order.NotInMapping = 0,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }

    [Test]
    public void RelatedSetAccessWithoutBeingInMappingThrows ()
    {
      Order order = Order.NewObject();
      Assert.That(
          () => order.NotInMappingRelated = null,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }

    [Test]
    public void DefaultRelatedObject ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      OrderItem item = order.OrderItems[0];
      Assert.That(item.Order, Is.SameAs(order));

      Order newOrder = Order.NewObject();
      Assert.That(newOrder, Is.Not.Null);
      item.Order = newOrder;
      Assert.That(item.Order, Is.Not.SameAs(order));
      Assert.That(item.Order, Is.SameAs(newOrder));
    }

    [Test]
    public void CannotInstantiateReallyAbstractClass ()
    {
      Assert.That(
          () => AbstractClass.NewObject(),
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Cannot instantiate type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.AbstractClass' because it is abstract. "
                  + "For classes with automatic properties, InstantiableAttribute must be used."));
    }

    [Test]
    public void ExplicitInterfaceProperty ()
    {
      IPropertyInterface domainObject = ClassWithExplicitInterfaceProperty.NewObject();

      Assert.That(
          () => domainObject.Property = 5,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }

    [Test]
    public void CurrentPropertyThrowsWhenNotInitializes ()
    {
      Order order = Order.NewObject();

      Assert.That(
          () => order.CurrentProperty,
          NUnit.Framework.Throws.InvalidOperationException
              .With.Message.EqualTo("There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?"));
    }

    [Test]
    public void PreparePropertyAccessCorrectlySetsCurrentProperty ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      order.PreparePropertyAccess("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber");
      int orderNumber;
      try
      {
        orderNumber = order.CurrentProperty.GetValue<int>();
      }
      finally
      {
        order.PropertyAccessFinished();
      }
      Assert.That(orderNumber, Is.EqualTo(order.OrderNumber));
    }

    [Test]
    public void PreparePropertyAccess_DoesNotThrowsOnInvalidPropertyName ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      order.PreparePropertyAccess("Bla");
      order.PropertyAccessFinished();
    }

    [Test]
    public void CurrentProperty_ThrowsOnInvalidPropertyName ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      order.PreparePropertyAccess("Bla");

      Assert.That(
          () => order.CurrentProperty,
          NUnit.Framework.Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "The domain object type 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order' does not have a mapping property named 'Bla'."));

      order.PropertyAccessFinished();
    }

    [Test]
    public void AccessingInterceptedProperties_ViaReflection_GetProperty ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      var propertyInfo = ((object)order).GetType().GetProperty("OrderNumber");
      Assert.That(propertyInfo, Is.Not.Null);
      Assert.That(propertyInfo.GetValue(order, null), Is.EqualTo(1));
    }

    [Test]
    public void AccessingInterceptedProperties_ViaReflection_GetProperties ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      var propertyInfos = ((object)order).GetType().GetProperties();
      var orderNumberProperty = propertyInfos.SingleOrDefault(pi => pi.Name == "OrderNumber");

      Assert.That(orderNumberProperty, Is.Not.Null);
    }

    private bool WasCreatedByFactory (object o)
    {
      var pipeline = ((DomainObjectCreator)DomainObjectIDs.Order1.ClassDefinition.InstanceCreator).PipelineRegistry.DefaultPipeline;
      return pipeline.ReflectionService.IsAssembledType(o.GetType());
    }

    [DBTable]
    [Instantiable]
    [IncludeInMappingTestDomain]
    public abstract class ClassWithAbstractRelatedCollectionSetter : DomainObject
    {
      public static ClassWithAbstractRelatedCollectionSetter NewObject ()
      {
        return NewObject<ClassWithAbstractRelatedCollectionSetter>();
      }

      protected ClassWithAbstractRelatedCollectionSetter ()
      {
      }

      [DBBidirectionalRelation("RelatedObjects")]
      public abstract ClassWithAbstractRelatedCollectionSetter Parent { get; }

      [DBBidirectionalRelation("Parent")]
      public abstract ObjectList<ClassWithAbstractRelatedCollectionSetter> RelatedObjects { get; set; }
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public class ClassWithExplicitInterfaceProperty : DomainObject, IPropertyInterface
    {
      public static ClassWithExplicitInterfaceProperty NewObject ()
      {
        return NewObject<ClassWithExplicitInterfaceProperty>();
      }

      protected ClassWithExplicitInterfaceProperty ()
      {
      }

      int IPropertyInterface.Property
      {
        get { return CurrentProperty.GetValue<int>(); }
        set { CurrentProperty.SetValue(value); }
      }
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public class ClassWithWrongConstructor : DomainObject
    {
      public static ClassWithWrongConstructor NewObject ()
      {
        return NewObject<ClassWithWrongConstructor>();
      }

      public static ClassWithWrongConstructor NewObject (double d)
      {
        return NewObject<ClassWithWrongConstructor>(ParamList.Create(d));
      }

      [UsedImplicitly]
      public ClassWithWrongConstructor (string s)
      {
        Assert.Fail("Shouldn't be executed.");
      }
    }

    public interface IPropertyInterface
    {
      int Property { get; set; }
    }

    [DBTable]
    [Instantiable]
    [IncludeInMappingTestDomain]
    public abstract class NonInstantiableAbstractClass : DomainObject
    {
      public static NonInstantiableAbstractClass NewObject ()
      {
        return NewObject<NonInstantiableAbstractClass>();
      }

      protected NonInstantiableAbstractClass ()
      {
      }

      public abstract void Foo ();
    }

    [Instantiable]
    [DBTable]
    [IncludeInMappingTestDomain]
    public abstract class NonInstantiableAbstractClassWithProps : DomainObject
    {
      public static NonInstantiableAbstractClassWithProps NewObject ()
      {
        return NewObject<NonInstantiableAbstractClassWithProps>();
      }

      protected NonInstantiableAbstractClassWithProps ()
      {
      }

      [StorageClassNone]
      public abstract int Foo { get; }
    }

    [DBTable]
    [Uses(typeof(MixinWithAutoProperties))]
    [IncludeInMappingTestDomain]
    public class NonInstantiableClassWithMixinWithPersistentAutoProperties : DomainObject
    {
      public static NonInstantiableClassWithMixinWithPersistentAutoProperties NewObject ()
      {
        return NewObject<NonInstantiableClassWithMixinWithPersistentAutoProperties>();
      }

      public class MixinWithAutoProperties : DomainObjectMixin<NonInstantiableClassWithMixinWithPersistentAutoProperties>
      {
        public int PersistentAutoProperty { get; set; }
      }
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public sealed class NonInstantiableSealedClass : DomainObject
    {
      public static NonInstantiableSealedClass NewObject ()
      {
        return NewObject<NonInstantiableSealedClass>();
      }

      public NonInstantiableSealedClass ()
      {
      }
    }

    [DBTable]
    [IncludeInMappingTestDomain]
    public class Throws : DomainObject
    {
      public static Throws NewObject ()
      {
        return NewObject<Throws>();
      }

      public Throws ()
        : base(ThrowException(), new StreamingContext())
      {
      }

      private static SerializationInfo ThrowException ()
      {
        throw new Exception("Thrown in ThrowException()");
      }
    }
  }
}
