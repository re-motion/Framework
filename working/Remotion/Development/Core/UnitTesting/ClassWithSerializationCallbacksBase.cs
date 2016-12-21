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
  [Serializable]
  public abstract class ClassWithSerializationCallbacksBase : IDeserializationCallback
  {
    protected abstract ISerializationEventReceiver StaticReceiver { get; }

    public void OnDeserialization (object sender)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnDeserialization (sender);
    }

    [OnDeserialized]
    public void OnDeserialized (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnDeserialized (context);
    }

    [OnDeserializing]
    public void OnDeserializing (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnDeserializing (context);
    }

    [OnSerialized]
    public void OnSerialized (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnSerialized (context);
    }

    [OnSerializing]
    public void OnSerializing (StreamingContext context)
    {
      if (StaticReceiver != null)
        StaticReceiver.OnSerializing (context);
    }
  }
}
