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
using Remotion.Development.UnitTesting;

namespace Remotion.Development.Moq.UnitTesting
{
  public class SerializationCallbackTester<T>
  {
    private readonly Action<ISerializationEventReceiver?> _receiverSetter;
    private readonly T _instance;

    public SerializationCallbackTester (T instance, Action<ISerializationEventReceiver?> receiverSetter)
    {
      _instance = instance;
      _receiverSetter = receiverSetter;

      _receiverSetter(null);
    }

    public void Test_SerializationCallbacks ()
    {
      _receiverSetter(null);
      try
      {
        var receiver = new Mock<ISerializationEventReceiver>(MockBehavior.Strict);

        _receiverSetter(receiver.Object);
        CheckSerialization(receiver);
      }
      finally
      {
        _receiverSetter(null);
      }
    }

    public void Test_DeserializationCallbacks ()
    {
      _receiverSetter(null);
      try
      {
        byte[] bytes = Serializer.Serialize(_instance);
        var receiver = new Mock<ISerializationEventReceiver>(MockBehavior.Strict);
        _receiverSetter(receiver.Object);

        CheckDeserialization(bytes, receiver);
      }
      finally
      {
        _receiverSetter(null);
      }
    }

    private void CheckSerialization (Mock<ISerializationEventReceiver> receiver)
    {
      ExpectSerializationCallbacks(receiver);

      Serializer.Serialize(_instance);

      receiver.Verify();
    }

    private void CheckDeserialization (byte[] bytes, Mock<ISerializationEventReceiver> receiver)
    {
      ExpectDeserializationCallbacks(receiver);

      Serializer.Deserialize(bytes);

      receiver.VerifyAll();
    }

    private void ExpectSerializationCallbacks (Mock<ISerializationEventReceiver> receiver)
    {
      int sequenceCounter = 0;
      receiver.Setup(_ => _.OnSerializing(It.IsAny<StreamingContext>())).InSequence(ref sequenceCounter, 0).Verifiable();
      receiver.Setup(_ => _.OnSerialized(It.IsAny<StreamingContext>())).InSequence(ref sequenceCounter, 1).Verifiable();
    }

    private void ExpectDeserializationCallbacks (Mock<ISerializationEventReceiver> receiver)
    {
      int sequenceCounter = 0;
      receiver.Setup(_ => _.OnDeserializing(It.IsAny<StreamingContext>())).InSequence(ref sequenceCounter, 0).Verifiable();
      receiver.Setup(_ => _.OnDeserialized(It.IsAny<StreamingContext>())).InSequence(ref sequenceCounter, 1).Verifiable();
      receiver.Setup(_ => _.OnDeserialization(It.IsAny<object>())).InSequence(ref sequenceCounter, 2).Verifiable();
    }
  }
}
