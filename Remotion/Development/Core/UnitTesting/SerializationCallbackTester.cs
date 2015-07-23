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

namespace Remotion.Development.UnitTesting
{
  public class SerializationCallbackTester<T>
  {
    private readonly IMockRepository _mockRepository;
    private readonly Action<ISerializationEventReceiver> _receiverSetter;
    private readonly T _instance;

    public SerializationCallbackTester (IMockRepository mockRepository, T instance, Action<ISerializationEventReceiver> receiverSetter)
    {
      _mockRepository = mockRepository;
      _instance = instance;
      _receiverSetter = receiverSetter;

      _receiverSetter (null);
    }

    public void Test_SerializationCallbacks ()
    {
      _receiverSetter (null);
      try
      {
        ISerializationEventReceiver receiver = _mockRepository.StrictMock<ISerializationEventReceiver> ();

        _receiverSetter (receiver);
        CheckSerialization (receiver);
      }
      finally
      {
        _receiverSetter (null);
      }
    }

    public void Test_DeserializationCallbacks ()
    {
      _receiverSetter (null);
      try
      {
        byte[] bytes = Serializer.Serialize (_instance);
        ISerializationEventReceiver receiver = _mockRepository.StrictMock<ISerializationEventReceiver> ();
        _receiverSetter (receiver);

        CheckDeserialization (bytes, receiver);
      }
      finally
      {
        _receiverSetter (null);
      }
    }

    private void CheckSerialization (ISerializationEventReceiver receiver)
    {
      ExpectSerializationCallbacks (receiver);

      _mockRepository.ReplayAll();

      Serializer.Serialize (_instance);

      _mockRepository.VerifyAll();
    }

    private void CheckDeserialization (byte[] bytes, ISerializationEventReceiver receiver)
    {
      ExpectDeserializationCallbacks (receiver);

      _mockRepository.ReplayAll();

      Serializer.Deserialize (bytes);

      _mockRepository.VerifyAll();
    }

    private void ExpectSerializationCallbacks (ISerializationEventReceiver receiver)
    {
      using (_mockRepository.Ordered())
      {
        StreamingContext context = new StreamingContext();
        receiver.OnSerializing (context);
        _mockRepository.LastCall_IgnoreArguments();

        receiver.OnSerialized (context);
        _mockRepository.LastCall_IgnoreArguments ();
      }
    }

    private void ExpectDeserializationCallbacks (ISerializationEventReceiver receiver)
    {
      using (_mockRepository.Ordered())
      {
        StreamingContext context = new StreamingContext();
        receiver.OnDeserializing (context);
        _mockRepository.LastCall_IgnoreArguments ();

        receiver.OnDeserialized (context);
        _mockRepository.LastCall_IgnoreArguments ();

        receiver.OnDeserialization (null);
        _mockRepository.LastCall_IgnoreArguments ();
      }
    }
  }
}
