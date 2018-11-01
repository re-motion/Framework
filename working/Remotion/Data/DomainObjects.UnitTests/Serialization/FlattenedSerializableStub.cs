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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  public class FlattenedSerializableStub : IFlattenedSerializable
  {
    public readonly string Data1;
    public readonly int Data2;
    public FlattenedSerializableStub Data3;

    public FlattenedSerializableStub (string data1, int data2)
    {
      Data1 = data1;
      Data2 = data2;
    }

    protected FlattenedSerializableStub (FlattenedDeserializationInfo info)
    {
      Data1 = info.GetValue<string> ();
      Data2 = info.GetIntValue ();
      Data3 = info.GetValueForHandle<FlattenedSerializableStub> ();
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddValue (Data1);
      info.AddIntValue (Data2);
      info.AddHandle (Data3);
    }
  }
}
