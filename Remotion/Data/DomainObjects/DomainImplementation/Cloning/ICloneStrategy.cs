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

namespace Remotion.Data.DomainObjects.DomainImplementation.Cloning
{
  /// <summary>
  /// Provides an interface for classes determining the details about how <see cref="DomainObjectCloner"/> clones a <see cref="DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Implement this interface do determine how <see cref="DomainObject"/> graphs are cloned. When <see cref="DomainObjectCloner"/> clones a single
  /// <see cref="DomainObject"/>, it needs to determine what to do with the objects referenced by that <see cref="DomainObject"/>. This decision
  /// is delegated to an instance of <see cref="ICloneStrategy"/>, whose <see cref="HandleReference"/> is called. From within <see cref="HandleReference"/>,
  /// the strategy can call <see cref="CloneContext.GetCloneFor{T}"/> to obtain clones for the referenced objects.
  /// </para>
  /// </remarks>
  /// <example>
  /// The following is an example of how to implement a cloning strategy that sees 1:n relations as aggregations; therefore, when one of the 
  /// "n" objects is cloned, the strategy inserts the clone into the original "1" object's collection rather than cloning the "1" object as well.
  /// <code>
  /// public class SmartCloneStrategy : ICloneStrategy
  /// {
  ///   public void HandleReference (
  ///       PropertyAccessor sourceReference,
  ///       ClientTransaction sourceTransaction,
  ///       PropertyAccessor cloneReference,
  ///       ClientTransaction cloneTransaction,
  ///       CloneContext context)
  ///   {
  ///     if (sourceReference.Kind == PropertyKind.RelatedObject)
  ///     {
  ///       if (sourceReference.RelationEndPointDefinition.RelationDefinition.RelationKind == RelationKindType.OneToMany)
  ///       {
  ///       ///    do not clone referenced object, but insert object into original collection
  ///         DomainObject originalRelated = (DomainObject) sourceReference.GetValueWithoutTypeCheckTx (sourceTransaction);
  ///         cloneReference.SetValueWithoutTypeCheckTx (cloneTransaction, originalRelated);
  ///       }
  ///       else
  ///       {
  ///         DomainObject originalRelated = (DomainObject) sourceReference.GetValueWithoutTypeCheckTx (sourceTransaction);
  ///         DomainObject cloneRelated = originalRelated != null ? context.GetCloneFor (originalRelated) : null;
  ///         cloneReference.SetValueWithoutTypeCheckTx (cloneTransaction, cloneRelated);
  ///       }
  ///     }
  ///     else
  ///     {
  ///       Assertion.IsTrue (sourceReference.Kind == PropertyKind.RelatedObjectCollection);
  ///       DomainObjectCollection originalRelatedCollection = (DomainObjectCollection) sourceReference.GetValueWithoutTypeCheckTx (sourceTransaction);
  ///       DomainObjectCollection cloneRelatedCollection = (DomainObjectCollection) cloneReference.GetValueWithoutTypeCheckTx (cloneTransaction);
  ///       foreach (DomainObject originalRelated in originalRelatedCollection)
  ///       {
  ///         DomainObject cloneRelated = context.GetCloneFor (originalRelated);
  ///         cloneRelatedCollection.Add (cloneRelated);
  ///       }
  ///     }
  ///   }
  /// }
  /// </code>
  /// </example>
  public interface ICloneStrategy
  {
    /// <summary>
    /// Called when <see cref="DomainObjectCloner"/> encounters a reference that might need to be cloned.
    /// </summary>
    /// <param name="sourceReference">The reference on the source object.</param>
    /// <param name="cloneReference">The reference on the cloned object.</param>
    /// <param name="context">A <see cref="CloneContext"/> that should be used to obtain clones of objects held by <paramref name="sourceReference"/>.</param>
    /// <remarks>
    /// <para>
    /// Implementers can check the <paramref name="sourceReference"/> and set the <paramref name="cloneReference"/> to clones,
    /// original, or empty as needed. In order to get the right clone for a referenced object, the <paramref name="context"/> can be used.
    /// </para>
    /// <para>
    /// Note that <see cref="HandleReference"/> is only called for references yet untouched. Therefore, for bidirectional references, it will 
    /// only be called for one side of the relation even if both  sides are cloned.
    /// </para>
    /// <para>
    /// When the  <paramref name="context"/> is used to obtain the clones, no object will be cloned twice.
    /// </para>
    /// </remarks>
    void HandleReference (PropertyAccessor sourceReference, PropertyAccessor cloneReference, CloneContext context);
  }
}
