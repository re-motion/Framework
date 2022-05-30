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
using Moq;
using NUnit.Framework;
using Remotion.Development.Moq.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.Moq.UnitTests
{
  [TestFixture]
  public class SerializationCallbackTesterTest
  {
    [Serializable]
    class OrdinaryClass : ClassWithSerializationCallbacksBase
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }
    }

    [Test]
    public void TestSerializationCallbacks_ViaOrdinaryInstance ()
    {
      new SerializationCallbackTester<OrdinaryClass>(new OrdinaryClass(), OrdinaryClass.SetReceiver)
          .Test_SerializationCallbacks();
    }

    [Test]
    public void TestDeserializationCallbacks_ViaOrdinaryInstance ()
    {
      new SerializationCallbackTester<OrdinaryClass>(new OrdinaryClass(), OrdinaryClass.SetReceiver)
          .Test_DeserializationCallbacks();
    }

    [Serializable]
    class BrokenClass : ClassWithSerializationCallbacksBase, IDeserializationCallback
    {
      private static ISerializationEventReceiver s_receiver;

      public static void SetReceiver (ISerializationEventReceiver receiver)
      {
        s_receiver = receiver;
      }

      protected override ISerializationEventReceiver StaticReceiver
      {
        get { return s_receiver; }
      }

      void IDeserializationCallback.OnDeserialization (object sender)
      {
        // suppresses receiver event
      }
    }

    [Test]
    public void TestDeserializationCallbacks_ViaBrokenInstance ()
    {
      Assert.That(
          () => new SerializationCallbackTester<BrokenClass>(new BrokenClass(), BrokenClass.SetReceiver)
              .Test_DeserializationCallbacks(),
          Throws.InstanceOf<MockException>()
              .With.Message.StartsWith(
                  "Mock<ISerializationEventReceiver:1>:\n"
                  + "This mock failed verification due to the following:\r\n"
                  + "\r\n"
                  + "   ISerializationEventReceiver _ => _.OnDeserialization(It.IsAny<object>()):\n"
                  + "   This setup was not matched."));
    }
  }
}
