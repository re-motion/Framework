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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Implements <see cref="IDomainObjectCollectionEndPointCollectionManager"/>, storing the original and current <see cref="DomainObjectCollection"/> reference
  /// for a <see cref="DomainObjectCollectionEndPoint"/>.
  /// </summary>
  public class DomainObjectCollectionEndPointCollectionManager : IDomainObjectCollectionEndPointCollectionManager
  {
    private readonly RelationEndPointID _endPointID;
    private readonly IDomainObjectCollectionEndPointCollectionProvider _domainObjectCollectionProvider;
    private readonly IAssociatedDomainObjectCollectionDataStrategyFactory _dataStrategyFactory;

    private DomainObjectCollection? _originalCollectionReference;
    private DomainObjectCollection? _currentCollectionReference;

    public DomainObjectCollectionEndPointCollectionManager (
        RelationEndPointID endPointID,
        IDomainObjectCollectionEndPointCollectionProvider domainObjectCollectionProvider,
        IAssociatedDomainObjectCollectionDataStrategyFactory dataStrategyFactory)
    {
      ArgumentUtility.CheckNotNull("endPointID", endPointID);
      ArgumentUtility.CheckNotNull("domainObjectCollectionProvider", domainObjectCollectionProvider);
      ArgumentUtility.CheckNotNull("dataStrategyFactory", dataStrategyFactory);

      _endPointID = endPointID;
      _domainObjectCollectionProvider = domainObjectCollectionProvider;
      _dataStrategyFactory = dataStrategyFactory;
    }

    public RelationEndPointID EndPointID
    {
      get { return _endPointID; }
    }

    public IDomainObjectCollectionEndPointCollectionProvider DomainObjectCollectionProvider
    {
      get { return _domainObjectCollectionProvider; }
    }

    public IAssociatedDomainObjectCollectionDataStrategyFactory DataStrategyFactory
    {
      get { return _dataStrategyFactory; }
    }

    public DomainObjectCollection GetOriginalCollectionReference ()
    {
      if (_originalCollectionReference == null)
        _originalCollectionReference = _domainObjectCollectionProvider.GetCollection(_endPointID);

      return _originalCollectionReference;
    }

    public DomainObjectCollection GetCurrentCollectionReference ()
    {
      if (_currentCollectionReference == null)
        _currentCollectionReference = GetOriginalCollectionReference();

      return _currentCollectionReference;
    }

    public IDomainObjectCollectionData AssociateCollectionWithEndPoint (DomainObjectCollection newCollection)
    {
      var oldCollection = (IAssociatableDomainObjectCollection)GetCurrentCollectionReference();
      Assertion.IsTrue(oldCollection.AssociatedEndPointID == _endPointID);
      oldCollection.TransformToStandAlone();

      var oldDataStrategyOfNewCollection = ((IAssociatableDomainObjectCollection)newCollection).TransformToAssociated(_endPointID, _dataStrategyFactory);

      _currentCollectionReference = newCollection;
      return oldDataStrategyOfNewCollection;
    }

    public bool HasCollectionReferenceChanged ()
    {
      if (_originalCollectionReference == null)
      {
        Assertion.DebugAssert(_currentCollectionReference == null);
        return false;
      }

      if (_currentCollectionReference == null)
        return false;

      return _currentCollectionReference != _originalCollectionReference;
    }

    public void CommitCollectionReference ()
    {
      if (_originalCollectionReference == null)
      {
        Assertion.DebugAssert(_currentCollectionReference == null);
        return;
      }

      if (_currentCollectionReference == null)
        return;

      if (_originalCollectionReference != _currentCollectionReference)
      {
        _originalCollectionReference = _currentCollectionReference;
        _domainObjectCollectionProvider.RegisterCollection(_endPointID, _currentCollectionReference);
      }
      Assertion.DebugAssert(!HasCollectionReferenceChanged());
    }

    public void RollbackCollectionReference ()
    {
      if (_originalCollectionReference == null)
      {
        Assertion.DebugAssert(_currentCollectionReference == null);
        return;
      }

      if (_currentCollectionReference == null)
        return;

      if (_originalCollectionReference == _currentCollectionReference)
        return;

      // If the end-point's current collection is still associated with this end point, transform it to stand-alone.
      // (During rollback, the current relation might have already been associated with another end-point, we must not overwrite this!)
      var oldCollection = (IAssociatableDomainObjectCollection)GetCurrentCollectionReference();
      if (oldCollection.AssociatedEndPointID == _endPointID)
        oldCollection.TransformToStandAlone();

      // We must always associate the new collection with the end point, however - even during rollback phase,
      ((IAssociatableDomainObjectCollection)_originalCollectionReference).TransformToAssociated(_endPointID, _dataStrategyFactory);
      _currentCollectionReference = _originalCollectionReference;
      Assertion.DebugAssert(!HasCollectionReferenceChanged());
    }
  }
}
