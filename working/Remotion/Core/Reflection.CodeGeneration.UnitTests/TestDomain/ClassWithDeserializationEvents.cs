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

namespace Remotion.Reflection.CodeGeneration.UnitTests.TestDomain
{
  [Serializable]
  public class ClassWithDeserializationEvents : BaseClassWithDeserializationEvents, IDeserializationCallback
  {
    [NonSerialized]
    public bool OnDeserializationCalled;
    [NonSerialized]
    public bool OnDeserializedCalled;
    [NonSerialized]
    public bool OnDeserializingCalled;

    public void OnDeserialization (object sender)
    {
      OnDeserializationCalled = true;
    }

    [OnDeserializing]
    private void OnDeserializing (StreamingContext context)
    {
      OnDeserializingCalled = true;
    }

    [OnDeserialized]
    private void OnDeserialized (StreamingContext context)
    {
      OnDeserializedCalled = true;
    }
  }
}
