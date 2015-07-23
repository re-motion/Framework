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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Defines APIs used by <see cref="CollectionEndPoint"/> when it needs to transform a stand-alone <see cref="DomainObjectCollection"/> to
  /// an associated collection.
  /// </summary>
  public interface IAssociatableDomainObjectCollection
  {
    /// <summary>
    /// Transforms the collection to an associated collection. The collection will represent the data stored by the <see cref="ICollectionEndPoint"/>
    /// represented by <paramref name="endPointID"/>, and all modifications will be performed on that <see cref="ICollectionEndPoint"/>.
    /// After this operation, the collection's data will be that of the <see cref="RelationEndPointID"/>.
    /// This interface is used by <see cref="CollectionEndPointSetCollectionCommand"/> and should usually not be called by framework
    /// users.
    /// </summary>
    /// <param name="endPointID">The <see cref="RelationEndPointID"/> of the <see cref="ICollectionEndPoint"/> to associate with.</param>
    /// <param name="associatedCollectionDataStrategyFactory">The <see cref="IAssociatedCollectionDataStrategyFactory"/> to get the new data strategy from.</param>
    /// <returns>The <see cref="IDomainObjectCollectionData"/> strategy used by the <see cref="DomainObjectCollection"/> before it was associated.</returns>
    IDomainObjectCollectionData TransformToAssociated (
        RelationEndPointID endPointID, IAssociatedCollectionDataStrategyFactory associatedCollectionDataStrategyFactory);

    /// <summary>
    /// Transforms the collection to a stand-alone collection. The collection will get its own data store (with a fresh copy of the data that was held 
    /// by the collection) and will not be associated with an <see cref="ICollectionEndPoint"/> any longer.
    /// This interface is used by  <see cref="CollectionEndPointSetCollectionCommand"/> and should usually not be required by framework 
    /// users.
    /// </summary>
    /// <returns>The <see cref="IDomainObjectCollectionData"/> strategy used by the <see cref="DomainObjectCollection"/> before it was made stand-alone.</returns>
    IDomainObjectCollectionData TransformToStandAlone ();

    /// <summary>
    /// Gets the <see cref="ICollectionEndPoint"/> associated with this <see cref="DomainObjectCollection"/>, or <see langword="null" /> if
    /// this is a stand-alone collection.
    /// </summary>
    /// <value>The associated end point.</value>
    RelationEndPointID AssociatedEndPointID { get; }
  }
}