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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  /// <summary>
  /// Assists in importing data exported by a <see cref="DomainObjectTransporter"/> object. This class is used by
  /// <see cref="DomainObjectTransporter.LoadTransportData(System.IO.Stream,Remotion.Data.DomainObjects.DomainImplementation.Transport.IImportStrategy)"/> and is usually 
  /// not instantiated by users.
  /// </summary>
  public class DomainObjectImporter
  {
    public static DomainObjectImporter CreateImporterFromStream (Stream stream, IImportStrategy strategy)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);
      ArgumentUtility.CheckNotNull ("strategy", strategy);

      var transportItems = strategy.Import (stream).ToArray();
      return new DomainObjectImporter (transportItems);
    }

    private readonly TransportItem[] _transportItems;

    public DomainObjectImporter (TransportItem[] transportItems)
    {
      ArgumentUtility.CheckNotNull ("transportItems", transportItems);
      _transportItems = transportItems;
    }

    public TransportedDomainObjects GetImportedObjects ()
    {
      var targetTransaction = ClientTransaction.CreateRootTransaction ();
      var dataContainerMapping = GetTargetDataContainersForSourceObjects (targetTransaction);

      // grab enlisted objects _before_ properties are synchronized, as synchronizing might load some additional objects
      var transportedObjects = targetTransaction.GetEnlistedDomainObjects().ToList();
      SynchronizeData (targetTransaction, dataContainerMapping);

      foreach (var transportedObject in transportedObjects.OfType<IDomainObjectImporterCallback>())
      {
        transportedObject.OnDomainObjectImported (targetTransaction);
      }

      return new TransportedDomainObjects (targetTransaction, transportedObjects);
    }

    private List<Tuple<TransportItem, DataContainer>> GetTargetDataContainersForSourceObjects (ClientTransaction bindingTargetTransaction)
    {
      var result = new List<Tuple<TransportItem, DataContainer>> ();
      if (_transportItems.Length > 0)
      {
        using (bindingTargetTransaction.EnterNonDiscardingScope())
        {
          var transportedObjectIDs = GetIDs (_transportItems);

          var existingObjects = bindingTargetTransaction.TryGetObjects<DomainObject> (transportedObjectIDs);
          var existingObjectDictionary = existingObjects.Where (obj => obj != null).ToDictionary (obj => obj.ID);

          foreach (TransportItem transportItem in _transportItems)
          {
            DataContainer targetDataContainer = GetTargetDataContainer (transportItem, existingObjectDictionary, bindingTargetTransaction);
            result.Add (Tuple.Create (transportItem, targetDataContainer));
          }
        }
      }
      return result;
    }

    private DataContainer GetTargetDataContainer (TransportItem transportItem, Dictionary<ObjectID, DomainObject> existingObjects, ClientTransaction bindingTargetTransaction)
    {
      DomainObject existingObject;
      if (existingObjects.TryGetValue (transportItem.ID, out existingObject))
      {
        return bindingTargetTransaction.DataManager.GetDataContainerWithLazyLoad (existingObject.ID, throwOnNotFound: true);
      }
      else
      {
        var id = transportItem.ID;

        var instance = bindingTargetTransaction.GetInvalidObjectReference (id);
        ResurrectionService.ResurrectInvalidObject (bindingTargetTransaction, id);

        var newDataContainer = DataContainer.CreateNew (id);
        newDataContainer.SetDomainObject (instance);
        bindingTargetTransaction.DataManager.RegisterDataContainer (newDataContainer);

        return newDataContainer;
      }
    }

    private ObjectID[] GetIDs (TransportItem[] items)
    {
      return Array.ConvertAll (items, item => item.ID);
    }

    private void SynchronizeData (ClientTransaction targetTransaction, IEnumerable<Tuple<TransportItem, DataContainer>> sourceToTargetMapping)
    {
      foreach (Tuple<TransportItem, DataContainer> sourceToTargetContainer in sourceToTargetMapping)
      {
        TransportItem transportItem = sourceToTargetContainer.Item1;
        DataContainer targetContainer = sourceToTargetContainer.Item2;
        PropertyIndexer targetProperties = new PropertyIndexer (targetContainer.DomainObject);

        foreach (KeyValuePair<string, object> sourceProperty in transportItem.Properties)
        {
          PropertyAccessor targetProperty = targetProperties[sourceProperty.Key, targetTransaction];
          switch (targetProperty.PropertyData.Kind)
          {
            case PropertyKind.PropertyValue:
              targetProperty.SetValueWithoutTypeCheck (sourceProperty.Value);
              break;
            case PropertyKind.RelatedObject:
              if (!targetProperty.PropertyData.RelationEndPointDefinition.IsVirtual)
              {
                var relatedObjectID = (ObjectID) sourceProperty.Value;
                var targetRelatedObject = relatedObjectID != null ? targetTransaction.GetObject (relatedObjectID, false) : null;
                targetProperty.SetValueWithoutTypeCheck (targetRelatedObject);
              }
              break;
          }
        }
      }
    }
  }
}
