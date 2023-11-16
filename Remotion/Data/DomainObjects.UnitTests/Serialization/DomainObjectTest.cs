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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      Assert2.IgnoreIfFeatureSerializationIsDisabled();
    }

    [Test]
    public void Serializable ()
    {
      var instance = Order.NewObject();
      instance.DeliveryDate = new DateTime(2010, 1, 13);
      Assert.That(typeof(ISerializable).IsAssignableFrom(instance.GetPublicDomainObjectType()), Is.False);

      var deserializedData = Serializer.SerializeAndDeserialize(Tuple.Create(ClientTransaction.Current, instance));
      var deserializedInstance = deserializedData.Item2;

      Assert.That(deserializedInstance.ID, Is.EqualTo(instance.ID));
      Assert.That(deserializedInstance.RootTransaction, Is.SameAs(deserializedData.Item1));

      using (deserializedData.Item1.EnterNonDiscardingScope())
      {
        Assert.That(deserializedInstance, Is.Not.SameAs(instance));
        Assert.That(deserializedInstance.DeliveryDate, Is.EqualTo(new DateTime(2010, 1, 13)));
      }
    }

    [Test]
    public void Serializable_WithISerializable ()
    {
      var instance = ClassWithAllDataTypes.NewObject();
      instance.Int32Property = 5;
      Assert.That(instance, Is.InstanceOf<ISerializable>());

      var deserializedData = Serializer.SerializeAndDeserialize(Tuple.Create(ClientTransaction.Current, instance));
      var deserializedInstance = deserializedData.Item2;

      Assert.That(deserializedInstance.ID, Is.EqualTo(instance.ID));
      Assert.That(deserializedInstance.RootTransaction, Is.SameAs(deserializedData.Item1));

      using (deserializedData.Item1.EnterNonDiscardingScope())
      {
        Assert.That(deserializedInstance, Is.Not.SameAs(instance));
        Assert.That(deserializedInstance.Int32Property, Is.EqualTo(5));
      }
    }

    [Test]
    public void Serializable_LoadCount ()
    {
      ClassWithAllDataTypes instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      Assert.That(instance.OnLoadedCalled, Is.True);
      Assert.That(instance.OnLoadedLoadMode, Is.EqualTo(LoadMode.WholeDomainObjectInitialized));

      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize(Tuple.Create(ClientTransaction.Current, instance));

      using (deserializedData.Item1.EnterNonDiscardingScope())
      {
        ClassWithAllDataTypes deserializedInstance = deserializedData.Item2;
        Assert.That(deserializedInstance.OnLoadedCalled, Is.False);
        using (ClientTransaction.Current.CreateSubTransaction().EnterNonDiscardingScope())
        {
          deserializedInstance.Int32Property = 15;
          Assert.That(deserializedInstance.OnLoadedCalled, Is.True);
          Assert.That(deserializedInstance.OnLoadedLoadMode, Is.EqualTo(LoadMode.DataContainerLoadedOnly));
        }
      }
    }

    [Test]
    public void Serialization_WithISerializable_IncludesEventHandlers ()
    {
      ClassWithAllDataTypes instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
      var eventReceiver = new DomainObjectEventReceiver(instance);

      var deserializedData = Serializer.SerializeAndDeserialize(Tuple.Create(instance, eventReceiver));
      AssertEventRegistered(deserializedData.Item1, "RelationChanging", deserializedData.Item2, GetEventHandlerMethod(instance, "RelationChanging"));
      AssertEventRegistered(deserializedData.Item1, "RelationChanged", deserializedData.Item2, GetEventHandlerMethod(instance, "RelationChanged"));
      AssertEventRegistered(deserializedData.Item1, "PropertyChanging", deserializedData.Item2, GetEventHandlerMethod(instance, "PropertyChanging"));
      AssertEventRegistered(deserializedData.Item1, "PropertyChanged", deserializedData.Item2, GetEventHandlerMethod(instance, "PropertyChanged"));
      AssertEventRegistered(deserializedData.Item1, "Deleting", deserializedData.Item2, GetEventHandlerMethod(instance, "Deleting"));
      AssertEventRegistered(deserializedData.Item1, "Deleted", deserializedData.Item2, GetEventHandlerMethod(instance, "Deleted"));
      AssertEventRegistered(deserializedData.Item1, "Committing", deserializedData.Item2, GetEventHandlerMethod(instance, "Committing"));
      AssertEventRegistered(deserializedData.Item1, "Committed", deserializedData.Item2, GetEventHandlerMethod(instance, "Committed"));
      AssertEventRegistered(deserializedData.Item1, "RollingBack", deserializedData.Item2, GetEventHandlerMethod(instance, "RollingBack"));
      AssertEventRegistered(deserializedData.Item1, "RolledBack", deserializedData.Item2, GetEventHandlerMethod(instance, "RolledBack"));
    }

    [Test]
    public void DomainObject_IDeserializationCallbackTest ()
    {
      Customer domainObject = DomainObjectIDs.Customer1.GetObject<Customer>();

      Customer deserializedDomainObject = Serializer.SerializeAndDeserialize(domainObject);
      Assert.That(deserializedDomainObject.OnDeserializationCalled, Is.True);
    }

    [Test]
    public void DomainObject_DeserializationCallbackAttributesTest ()
    {
      Customer domainObject = DomainObjectIDs.Customer1.GetObject<Customer>();

      Customer deserializedDomainObject = Serializer.SerializeAndDeserialize(domainObject);
      Assert.That(deserializedDomainObject.OnDeserializingAttributeCalled, Is.True);
      Assert.That(deserializedDomainObject.OnDeserializedAttributeCalled, Is.True);
    }

    [Test]
    public void DomainObject_CallingWrongCtorDuringDeserialization ()
    {
      var domainObject = LifetimeService.NewObject(
          TestableClientTransaction, typeof(SerializableClassCallingWrongBaseCtor), ParamList.Empty);

      Assert.That(
          () => UnwrapTargetInvocationExceptions(() => Serializer.SerializeAndDeserialize(domainObject)),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The DomainObject constructor may only be called via ClientTransaction.NewObject. "
              + "If this exception occurs during a base call of a deserialization constructor, adjust the base call to call the DomainObject's "
              + "deserialization constructor instead."));
    }

    private void AssertEventRegistered (DomainObject domainObject, string eventName, object receiver, MethodInfo receiverMethod)
    {
      var eventDelegate = (Delegate)PrivateInvoke.GetNonPublicField(domainObject, typeof(DomainObject), eventName);
      Assert.That(eventDelegate, Is.Not.Null);
      Assert.That(eventDelegate.Target, Is.SameAs(receiver));
      Assert.That(eventDelegate.Method, Is.EqualTo(receiverMethod));
    }

    private MethodInfo GetEventHandlerMethod (DomainObject instance, string eventName)
    {
      var eventDelegate = (Delegate)PrivateInvoke.GetNonPublicField(instance, typeof(DomainObject), eventName);
      return eventDelegate.Method;
    }

    private T UnwrapTargetInvocationExceptions<T> (Func<T> func)
    {
      try
      {
        return func();
      }
      catch (Exception e)
      {
        var rethrownException = e;
        while (rethrownException is TargetInvocationException)
          rethrownException = rethrownException.InnerException;
        throw rethrownException;
      }
    }

    [Serializable]
    [DBTable]
    [IncludeInMappingTestDomain]
    public class SerializableClassCallingWrongBaseCtor : DomainObject, ISerializable
    {
      public SerializableClassCallingWrongBaseCtor ()
      {
      }

      protected SerializableClassCallingWrongBaseCtor (SerializationInfo info, StreamingContext context)
      {
      }

#pragma warning disable SYSLIB0051
      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        BaseGetObjectData(info, context);
      }
#pragma warning restore SYSLIB0051
    }
  }
}
