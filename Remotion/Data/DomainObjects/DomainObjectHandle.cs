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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Implements <see cref="IDomainObjectHandle{T}"/>, providing a typed handle to a <see cref="DomainObject"/>.
  /// Since this class is not covariant, instances are usually accessed through the <see cref="IDomainObjectHandle{T}"/> interface instead.
  /// </summary>
  /// <typeparam name="T">The type of <see cref="IDomainObject"/> identified by this class.</typeparam>
  /// <threadsafety static="true" instance="true"/>
  public class DomainObjectHandle<T> : IDomainObjectHandle<T>
      where T : IDomainObject
  {
    private readonly ObjectID _objectID;

    public DomainObjectHandle (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      if (objectID.ClassDefinition.ClassType != typeof(T))
      {
        var message = string.Format("The class type of ObjectID '{0}' doesn't match the handle type '{1}'.", objectID, typeof(T));
        throw new ArgumentException(message, "objectID");
      }

      _objectID = objectID;
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public IDomainObjectHandle<TOther> Cast<TOther> ()
      where TOther : IDomainObject
    {
      try
      {
        return (IDomainObjectHandle<TOther>)this;
      }
      catch (InvalidCastException ex)
      {
        var message = string.Format("The handle for object '{0}' cannot be represented as a handle for type '{1}'.", _objectID, typeof(TOther));
        throw new InvalidCastException(message, ex);
      }
    }

    public bool Equals (IDomainObjectHandle<IDomainObject>? other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      if (GetType() != other.GetType())
        return false;
      return Equals(_objectID, other.ObjectID);
    }

    public override bool Equals (object? obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      return Equals(obj as IDomainObjectHandle<IDomainObject>);
    }

    public override int GetHashCode ()
    {
      return _objectID.GetHashCode();
    }

    public override string ToString ()
    {
      return _objectID + " (handle)";
    }
  }
}
