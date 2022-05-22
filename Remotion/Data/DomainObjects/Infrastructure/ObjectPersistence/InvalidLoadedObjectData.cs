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

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents an object that already exists in the target <see cref="ClientTransaction"/> as an invalid object.
  /// </summary>
  public class InvalidLoadedObjectData : ILoadedObjectData
  {
    private readonly IDomainObject _invalidObjectReference;

    public InvalidLoadedObjectData (IDomainObject invalidObjectReference)
    {
      ArgumentUtility.CheckNotNull("invalidObjectReference", invalidObjectReference);

      _invalidObjectReference = invalidObjectReference;
    }

    public ObjectID ObjectID
    {
      get { return _invalidObjectReference.ID; }
    }

    public IDomainObject InvalidObjectReference
    {
      get { return _invalidObjectReference; }
    }

    public IDomainObject? GetDomainObjectReference ()
    {
      return _invalidObjectReference;
    }

    public void Accept (ILoadedObjectVisitor visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);
      visitor.VisitInvalidLoadedObject(this);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
