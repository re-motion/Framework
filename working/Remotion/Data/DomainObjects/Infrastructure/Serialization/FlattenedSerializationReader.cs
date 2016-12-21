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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Serialization
{
  public class FlattenedSerializationReader<T>
  {
    private readonly T[] _data;
    private int _readPosition = 0;

    public FlattenedSerializationReader (T[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);

      _data = data;
    }

    public int ReadPosition
    {
      get { return _readPosition; }
    }

    public bool EndReached
    {
      get { return _readPosition == _data.Length; }
    }

    public T ReadValue ()
    {
      if (_readPosition >= _data.Length)
        throw new InvalidOperationException (string.Format ("There is no more data in the serialization stream at position {0}.", _readPosition));

      T value = _data[_readPosition];
      ++_readPosition;
      return value;
    }
  }
}
