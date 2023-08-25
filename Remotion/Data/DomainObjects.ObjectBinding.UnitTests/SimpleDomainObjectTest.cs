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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class SimpleDomainObjectTest : TestBase
  {
    [Test]
    public void NewObject ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject();
      Assert.That(instance, Is.Not.Null);
      Assert.That(instance.IntProperty, Is.EqualTo(0));
      instance.IntProperty = 5;
      Assert.That(instance.IntProperty, Is.EqualTo(5));
    }

    [Test]
    public void SimpleDomainObject_SupportsGetObjectViaHandle_AndObjectID ()
    {
      var instance = ClassDerivedFromSimpleDomainObject.NewObject();
      var handle = instance.GetHandle();

      var gottenInstance1 = handle.GetObject();
      Assert.That(gottenInstance1, Is.SameAs(instance));

      var gottenInstance2 = instance.ID.GetObject<ClassDerivedFromSimpleDomainObject>();
      Assert.That(gottenInstance2, Is.SameAs(instance));
    }

    [Test]
    public void Serializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      var instance = ClassDerivedFromSimpleDomainObject.NewObject();
      instance.IntProperty = 7;

      var deserializedData = Serializer.SerializeAndDeserialize(Tuple.Create(ClientTransaction.Current, instance));
      var deserializedInstance = deserializedData.Item2;

      Assert.That(deserializedInstance.ID, Is.EqualTo(instance.ID));
      Assert.That(deserializedInstance.RootTransaction, Is.SameAs(deserializedData.Item1));

      using (deserializedData.Item1.EnterNonDiscardingScope())
      {
        Assert.That(deserializedInstance, Is.Not.SameAs(instance));
        Assert.That(deserializedInstance.IntProperty, Is.EqualTo(7));
      }
    }


#pragma warning disable SYSLIB0050
    [Test]
    public void DeserializationConstructor_CallsBase ()
    {
      var serializable = ClassDerivedFromSimpleDomainObject_ImplementingISerializable.NewObject();

      var info = new SerializationInfo(typeof(ClassDerivedFromSimpleDomainObject_ImplementingISerializable), new FormatterConverter());
      var context = new StreamingContext();

      serializable.GetObjectData(info, context);
      Assert.That(info.MemberCount, Is.GreaterThan(0));

      var deserialized =
          (ClassDerivedFromSimpleDomainObject_ImplementingISerializable)Activator.CreateInstance(((object)serializable).GetType(), info, context);
      Assert.That(deserialized.ID, Is.EqualTo(serializable.ID));
    }
#pragma warning restore SYSLIB0050
  }
}
