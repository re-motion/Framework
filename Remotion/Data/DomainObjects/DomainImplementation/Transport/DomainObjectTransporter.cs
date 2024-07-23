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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DomainImplementation.Transport
{
  /// <summary>
  /// Collects domain objects to be transported to another system.
  /// </summary>
  public class DomainObjectTransporter
  {
    /// <summary>
    /// Loads the data transported from another system into a <see cref="TransportedDomainObjects"/> container.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> from which to load the data.</param>
    /// <param name="strategy">
    /// The strategy to use when importing data. This must match the strategy being used with
    /// <see cref="Export(System.IO.Stream,Remotion.Data.DomainObjects.DomainImplementation.Transport.IExportStrategy)"/>.
    /// </param>
    /// <returns>A container holding the objects loaded from the given data.</returns>
    /// <exception cref="ObjectsNotFoundException">A referenced related object is not part of the transported data and does not exist on the
    /// target system.</exception>
    /// <remarks>
    /// Given a <see cref="DomainObjectTransporter"/>, the binary data can be retrieved from
    /// <see cref="Export(System.IO.Stream,Remotion.Data.DomainObjects.DomainImplementation.Transport.IExportStrategy)"/>.
    /// </remarks>
    public static TransportedDomainObjects LoadTransportData (Stream stream, IImportStrategy strategy)
    {
      ArgumentUtility.CheckNotNull("stream", stream);
      ArgumentUtility.CheckNotNull("strategy", strategy);

      return DomainObjectImporter.CreateImporterFromStream(stream, strategy).GetImportedObjects();
    }

    private readonly ClientTransaction _transportTransaction;
    private readonly HashSet<ObjectID> _transportedObjects = new HashSet<ObjectID>();

    public DomainObjectTransporter ()
    {
      _transportTransaction = ClientTransaction.CreateRootTransaction();
      _transportTransaction.AddListener(new TransportTransactionListener(this));
    }

    /// <summary>
    /// Gets the IDs of the objects loaded into this transporter.
    /// </summary>
    /// <value>The IDs of the loaded objects.</value>
    public ReadOnlyCollection<ObjectID> ObjectIDs
    {
      get { return new ReadOnlyCollection<ObjectID>(_transportedObjects.ToArray()); }
    }

    /// <summary>
    /// Determines whether the specified <paramref name="objectID"/> has been loaded for transportation.
    /// </summary>
    /// <param name="objectID">The object ID to check.</param>
    /// <returns>
    /// True if the specified object ID has been loaded; otherwise, false.
    /// </returns>
    public bool IsLoaded (ObjectID objectID)
    {
      return _transportedObjects.Contains(objectID);
    }

    /// <summary>
    /// Loads a new instance of a domain object for transportation.
    /// </summary>
    /// <param name="type">The domain object type to instantiate.</param>
    /// <param name="constructorParameters">A <see cref="ParamList"/> encapsulating the parameters to be passed to the constructor. Instantiate this
    /// by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new instance of <paramref name="type"/> prepared for transport.</returns>
    public DomainObject LoadNew (Type type, ParamList constructorParameters)
    {
      using (_transportTransaction.EnterNonDiscardingScope())
      {
        DomainObject domainObject = LifetimeService.NewObject(_transportTransaction, type, constructorParameters);
        Load(domainObject.ID);
        return domainObject;
      }
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> into the transporter.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object to load.</param>
    /// <returns>The loaded object, whose properties can be manipulated before it is transported.</returns>
    /// <remarks>
    /// <para>
    /// This method loads exactly the object with the given ID, it will not load any related objects.
    /// </para>
    /// <para>
    /// If an object has the foreign key side of a relationship and the related object is not loaded into this transporter, the relationship
    /// will still be transported. The related object must exist at the target system, otherwise an exception is thrown in
    /// <see cref="LoadTransportData"/>.
    /// </para>
    /// <para>
    /// If an object has the virtual side of a relationship and the related object is not loaded into this transporter, the relationship
    /// will not be transported. Its status after <see cref="LoadTransportData"/> depends on the objects at the target system. This
    /// also applies to the 1-side of a 1-to-n relationship because the n-side is the foreign key side.
    /// </para>
    /// </remarks>
    public DomainObject Load (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      DomainObject domainObject = _transportTransaction.GetObject(objectID, false);
      _transportedObjects.Add(objectID);
      return domainObject;
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly referenced by it into the transporter.
    /// Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <returns>The loaded objects, whose properties can be manipulated before they are transported.</returns>
    /// <seealso cref="PropertyIndexer.GetAllRelatedObjects"/>
    public IEnumerable<DomainObject> LoadWithRelatedObjects (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return LazyLoadWithRelatedObjects(objectID).ToArray();
    }

    private IEnumerable<DomainObject> LazyLoadWithRelatedObjects (ObjectID objectID)
    {
      IDomainObject sourceObject = _transportTransaction.GetObject(objectID, false);
      yield return Load(sourceObject.ID);
      using (_transportTransaction.EnterNonDiscardingScope())
      {
        PropertyIndexer sourceProperties = new PropertyIndexer(sourceObject);
        IEnumerable<DomainObject> relatedObjects = sourceProperties.GetAllRelatedObjects();
        foreach (DomainObject domainObject in relatedObjects)
          yield return Load(domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction
      }
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly or indirectly referenced by it into the
    /// transporter. Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <returns>The loaded objects, whose properties can be manipulated before they are transported.</returns>
    /// <seealso cref="DomainObjectGraphTraverser.GetFlattenedRelatedObjectGraph"/>
    public IEnumerable<DomainObject> LoadRecursive (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      return LoadRecursive(objectID, FullGraphTraversalStrategy.Instance);
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly or indirectly referenced by it into the
    /// transporter, as specified by the <see cref="IGraphTraversalStrategy"/>. Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <param name="strategy">An <see cref="IGraphTraversalStrategy"/> instance defining which related object links to follow and which
    /// objects to include in the set of transported objects.</param>
    /// <returns>The loaded objects, whose properties can be manipulated before they are transported.</returns>
    /// <seealso cref="DomainObjectGraphTraverser.GetFlattenedRelatedObjectGraph"/>
    public IEnumerable<DomainObject> LoadRecursive (ObjectID objectID, IGraphTraversalStrategy strategy)
    {
      ArgumentUtility.CheckNotNull("objectID", objectID);
      ArgumentUtility.CheckNotNull("strategy", strategy);

      DomainObject sourceObject = _transportTransaction.GetObject(objectID, false);
      using (_transportTransaction.EnterNonDiscardingScope())
      {
        HashSet<DomainObject> graph = new DomainObjectGraphTraverser(sourceObject, strategy).GetFlattenedRelatedObjectGraph();
        foreach (DomainObject domainObject in graph)
          Load(domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction for consistency
        return graph;
      }
    }

    /// <summary>
    /// Retrieves a loaded object so that it can be manipulated prior to it being transported.
    /// </summary>
    /// <param name="loadedObjectID">The object ID of the object to be retrieved.</param>
    /// <returns>A <see cref="DomainObject"/> representing an object to be transported. Properties of this object can be manipulated.</returns>
    public DomainObject GetTransportedObject (ObjectID loadedObjectID)
    {
      ArgumentUtility.CheckNotNull("loadedObjectID", loadedObjectID);
      if (!IsLoaded(loadedObjectID))
      {
        string message = string.Format("Object '{0}' cannot be retrieved, it hasn't been loaded yet. Load it first, then retrieve it for editing.",
            loadedObjectID);
        throw new ArgumentException(message, "loadedObjectID");
      }
      return _transportTransaction.GetObject(loadedObjectID, false);
    }

    /// <summary>
    /// Exports the objects loaded into this transporter (including their contents) in a custom format for transport to another system.
    /// At the target system, the data can be loaded via <see cref="LoadTransportData(Stream,IImportStrategy)"/>.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to which to export the loaded objects.</param>
    /// <param name="strategy">The strategy to be used for exporting data. This must match the strategy used with 
    /// <see cref="LoadTransportData(System.IO.Stream,Remotion.Data.DomainObjects.DomainImplementation.Transport.IImportStrategy)"/>.</param>
    public void Export (Stream stream, IExportStrategy strategy)
    {
      IEnumerable<DataContainer> transportedContainers = GetTransportedContainers();
      TransportItem[] transportItems = TransportItem.PackageDataContainers(transportedContainers).ToArray();
      strategy.Export(stream, transportItems);
    }

    private IEnumerable<DataContainer> GetTransportedContainers ()
    {
      foreach (ObjectID id in _transportedObjects)
      {
        var dataContainer = _transportTransaction.DataManager.DataContainers[id];
        Assertion.IsNotNull(dataContainer, "Object '{0}' is missing from the transported dataset.", id);

        yield return dataContainer;
      }
    }
  }
}
