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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implements <see cref="IVirtualCollectionEndPointCollectionManager"/>, storing the original and current <see cref="IObjectList{IDomainObject}"/> reference
  /// for a <see cref="VirtualCollectionEndPoint"/>.
  /// </summary>
  public class VirtualCollectionEndPointCollectionManager : IVirtualCollectionEndPointCollectionManager
  {
    private readonly RelationEndPointID _endPointID;
    private readonly IVirtualCollectionEndPointCollectionProvider _collectionProvider;

    private IObjectList<IDomainObject>? _originalCollectionReference;
    private IObjectList<IDomainObject>? _currentCollectionReference;

    public VirtualCollectionEndPointCollectionManager (
        RelationEndPointID endPointID,
        IVirtualCollectionEndPointCollectionProvider collectionProvider)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      ArgumentUtility.CheckNotNull("collectionProvider", collectionProvider);

      _endPointID = endPointID;
      _collectionProvider = collectionProvider;
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public IVirtualCollectionEndPointCollectionProvider CollectionProvider
    {
      get { return _collectionProvider; }
    }

    public IObjectList<IDomainObject> GetOriginalCollectionReference ()
    {
      if (_originalCollectionReference == null)
        _originalCollectionReference = _collectionProvider.GetCollection(_endPointID);

      return _originalCollectionReference;
    }

    public IObjectList<IDomainObject> GetCurrentCollectionReference ()
    {
      if (_currentCollectionReference == null)
        _currentCollectionReference = GetOriginalCollectionReference();

      return _currentCollectionReference;
    }
  }
}
