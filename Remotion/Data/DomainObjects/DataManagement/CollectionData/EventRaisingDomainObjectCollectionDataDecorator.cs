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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Decorates <see cref="IDomainObjectCollectionData"/> by raising events whenever the inner collection is modified. The events are raised via
  /// an <see cref="IDomainObjectCollectionEventRaiser"/> instance before and after the modification.
  /// </summary>
  /// <remarks>
  /// This decorator is used to get events with stand-alone <see cref="DomainObjectCollection"/> instances.
  /// </remarks>
  [Serializable]
  public class EventRaisingDomainObjectCollectionDataDecorator : ObservableDomainObjectCollectionDataDecoratorBase
  {
    private readonly IDomainObjectCollectionEventRaiser _eventRaiser;

    public EventRaisingDomainObjectCollectionDataDecorator (IDomainObjectCollectionEventRaiser eventRaiser, IDomainObjectCollectionData wrappedData)
      : base(wrappedData)
    {
      ArgumentUtility.CheckNotNull("eventRaiser", eventRaiser);
      _eventRaiser = eventRaiser;
    }

    public IDomainObjectCollectionEventRaiser EventRaiser
    {
      get { return _eventRaiser; }
    }

    protected override void OnDataChanging (OperationKind operation, IDomainObject? affectedObject, int index)
    {
      if (operation != OperationKind.Sort)
        ArgumentUtility.CheckNotNull("affectedObject", affectedObject!);

      switch (operation)
      {
        case OperationKind.Insert:
          _eventRaiser.BeginAdd(index, affectedObject!);
          break;
        case OperationKind.Remove:
          _eventRaiser.BeginRemove(index, affectedObject!);
          break;
        case OperationKind.Sort:
          break;
        default:
          throw new InvalidOperationException("Invalid operation: " + operation);
      }
    }

    protected override void OnDataChanged (OperationKind operation, IDomainObject? affectedObject, int index)
    {
      if (operation != OperationKind.Sort)
        ArgumentUtility.CheckNotNull("affectedObject", affectedObject!);

      switch (operation)
      {
        case OperationKind.Insert:
          _eventRaiser.EndAdd(index, affectedObject!);
          break;
        case OperationKind.Remove:
          _eventRaiser.EndRemove(index, affectedObject!);
          break;
        case OperationKind.Sort:
          _eventRaiser.WithinReplaceData();
          break;
        default:
          throw new InvalidOperationException("Invalid operation: " + operation);
      }
    }
  }
}
