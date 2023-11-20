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

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public abstract class SerializableMappingObject
#pragma warning disable SYSLIB0050
      : IObjectReference
#pragma warning restore SYSLIB0050
  {
    public abstract object GetRealObject (StreamingContext context);
    protected abstract bool IsPartOfMapping { get; }
    protected abstract string IDForExceptions { get; }

    [OnSerializing]
    private void CheckWhenSerializing (StreamingContext context)
    {
      if (!IsPartOfMapping)
      {
        string message = string.Format("The {0} '{1}' cannot be serialized because is is not part of the current mapping.",
            GetType().Name, IDForExceptions);
        throw new SerializationException(message);
      }
    }
  }
}
