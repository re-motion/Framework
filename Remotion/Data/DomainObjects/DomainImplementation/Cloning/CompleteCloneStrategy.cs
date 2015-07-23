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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Cloning
{
  /// <summary>
  /// Assists <see cref="DomainObjectCloner"/> by cloning all objects referenced by a cloned source object as well. This ensures deep cloning
  /// of a whole object graph.
  /// </summary>
  public class CompleteCloneStrategy : ICloneStrategy
  {
    /// <summary>
    /// Sets the <paramref name="cloneReference"/> to hold clones of the objects referenced by <paramref name="sourceReference"/>.
    /// </summary>
    /// <param name="sourceReference">The reference on the source object.</param>
    /// <param name="cloneReference">The reference on the cloned object.</param>
    /// <param name="context">The <see cref="CloneContext"/> that is used to obtain clones of objects held by <paramref name="sourceReference"/>.</param>
    public void HandleReference (PropertyAccessor sourceReference, PropertyAccessor cloneReference, CloneContext context)
    {
      if (sourceReference.PropertyData.Kind == PropertyKind.RelatedObject)
      {
        var originalRelated = (DomainObject) sourceReference.GetValueWithoutTypeCheck ();
        DomainObject cloneRelated = originalRelated != null ? context.GetCloneFor (originalRelated) : null;
        cloneReference.SetValueWithoutTypeCheck (cloneRelated);
      }
      else
      {
        Assertion.IsTrue (sourceReference.PropertyData.Kind == PropertyKind.RelatedObjectCollection);
        var originalRelatedCollection = (DomainObjectCollection) sourceReference.GetValueWithoutTypeCheck ();
        var cloneRelatedCollection = (DomainObjectCollection) cloneReference.GetValueWithoutTypeCheck ();

        foreach (DomainObject originalRelated in originalRelatedCollection)
        {
          DomainObject cloneRelated = context.GetCloneFor (originalRelated);
          cloneRelatedCollection.Add (cloneRelated);
        }
      }
    }
  }
}
