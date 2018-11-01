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
using System.Runtime.Serialization;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
/// <summary>
/// Represents the definition of a query.
/// </summary>
/// <remarks>
/// During the serialization process the object determines if it is part of <see cref="Query"/> 
/// and serializes this information. If it was then the deserialized object will be the reference from 
/// <see cref="Query"/> with the same <see cref="ID"/> again. Otherwise, a new object will be instantiated.
/// </remarks>
[Serializable]
public class QueryDefinition : ISerializable, IObjectReference
{
  // types

  // static members and constants

  // member fields

  private readonly string _id;

  private readonly string _statement;
  private readonly QueryType _queryType;
  private readonly Type _collectionType;
  private readonly StorageProviderDefinition _storageProviderDefinition;
  
  // Note: _ispartOfQueryConfiguration is used only during the deserialization process. 
  // It is set only in the deserialization constructor and is used in IObjectReference.GetRealObject.
  private readonly bool _ispartOfQueryConfiguration;


  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <paramref name="queryID"/> to be associated with this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="storageProviderDefinition">The <see cref="StorageProviderDefinition"/> used for executing instances of this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="statement">The <paramref name="statement"/> of the <b>QueryDefinition</b>. The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> specified through <paramref name="storageProviderDefinition"/> must understand the syntax of the <paramref name="statement"/>. Must not be <see langword="null"/>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="queryID"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="storageProviderDefinition"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="statement"/> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <paramref name="queryID"/> is an empty string.<br /> -or- <br />
  ///   <paramref name="statement"/> is an empty string.
  /// </exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="queryType"/> is not a valid enum value.</exception>
  public QueryDefinition (string queryID, StorageProviderDefinition storageProviderDefinition, string statement, QueryType queryType)
    : this (queryID, storageProviderDefinition, statement, queryType, null)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <b>QueryDefinition</b> class.
  /// </summary>
  /// <param name="queryID">The <paramref name="queryID"/> to be associated with this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="storageProviderDefinition">The <see cref="StorageProviderDefinition"/> used for executing instances of this <b>QueryDefinition</b>. Must not be <see langword="null"/>.</param>
  /// <param name="statement">The <paramref name="statement"/> of the <b>QueryDefinition</b>. The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> specified through <paramref name="storageProviderDefinition"/> must understand the syntax of the <paramref name="statement"/>. Must not be <see langword="null"/>.</param>
  /// <param name="queryType">One of the <see cref="QueryType"/> enumeration constants.</param>
  /// <param name="collectionType">If <paramref name="queryType"/> specifies a collection to be returned, <paramref name="collectionType"/> specifies the type of the collection. If <paramref name="queryType"/> is <see langword="null"/>, <see cref="DomainObjectCollection"/> is used.</param>
  /// <exception cref="System.ArgumentNullException">
  ///   <paramref name="queryID"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="storageProviderDefinition"/> is <see langword="null"/>.<br /> -or- <br />
  ///   <paramref name="statement"/> is <see langword="null"/>.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  ///   <paramref name="queryID"/> is an empty string.<br /> -or- <br />
  ///   <paramref name="statement"/> is an empty string.
  /// </exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="queryType"/> is not a valid enum value.</exception>
  public QueryDefinition (
      string queryID, 
      StorageProviderDefinition storageProviderDefinition,
      string statement, 
      QueryType queryType, 
      Type collectionType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("queryID", queryID);
    ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("statement", statement);
    ArgumentUtility.CheckValidEnumValue ("queryType", queryType);

    if (queryType == QueryType.Scalar && collectionType != null)
      throw new ArgumentException (string.Format ("The scalar query '{0}' must not specify a collectionType.", queryID), "collectionType");

    if (queryType == QueryType.Collection && collectionType == null)
      collectionType = typeof (DomainObjectCollection);

    if (collectionType != null 
        && !collectionType.Equals (typeof (DomainObjectCollection)) 
        && !collectionType.IsSubclassOf (typeof (DomainObjectCollection)))
    {
      throw new ArgumentException (string.Format (
          "The collectionType of query '{0}' must be 'Remotion.Data.DomainObjects.DomainObjectCollection' or derived from it.", queryID), "collectionType");
    }

    _id = queryID;
    _storageProviderDefinition = storageProviderDefinition;
    _statement = statement;
    _queryType = queryType;
    _collectionType = collectionType;
  }

  /// <summary>
  /// This constructor is used for deserializing the object and is not intended to be used directly from code.
  /// </summary>
  /// <param name="info">The data needed to serialize or deserialize an object. </param>
  /// <param name="context">The source and destination of a given serialized stream.</param>
  protected QueryDefinition (SerializationInfo info, StreamingContext context)
  {
    _id = info.GetString ("ID");
    _ispartOfQueryConfiguration = info.GetBoolean ("IsPartOfQueryConfiguration");

    if (!_ispartOfQueryConfiguration)
    {
       var storageProviderID = info.GetString ("StorageProviderID");
       _storageProviderDefinition = DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory (storageProviderID);

      _statement = info.GetString ("Statement");
      _queryType = (QueryType) info.GetValue ("QueryType", typeof (QueryType));
      _collectionType = (Type) info.GetValue ("CollectionType", typeof (Type));
    }
  }

  // methods and properties

  /// <summary>
  /// Gets the unique ID for this <b>QueryDefinition</b>.
  /// </summary>
  public string ID
  {
    get { return _id; }
  }

  /// <summary>
  /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> used for executing instances of this <b>QueryDefinition</b>.
  /// </summary>
  public StorageProviderDefinition StorageProviderDefinition
  {
    get { return _storageProviderDefinition; }
  }

  /// <summary>
  /// Gets the statement-text of the <b>QueryDefinition</b>. The <see cref="Remotion.Data.DomainObjects.Persistence.StorageProvider"/> specified through <see cref="StorageProviderDefinition"/> must understand the syntax of the <b>Statement</b>.
  /// </summary>
  public string Statement
  {
    get { return _statement; }
  }

  /// <summary>
  /// Gets the <see cref="QueryType"/> of this <b>QueryDefinition</b>.
  /// </summary>
  public QueryType QueryType
  {
    get { return _queryType; }
  }

  /// <summary>
  /// If <see cref="QueryType"/> specifies a collection to be returned, <b>CollectionType</b> specifies the type of the collection. The default is <see cref="DomainObjectCollection"/>. 
  /// </summary>
  public Type CollectionType
  {
    get { return _collectionType; }
  }

  #region ISerializable Members

  /// <summary>
  /// Populates a specified <see cref="System.Runtime.Serialization.SerializationInfo"/> with the 
  /// data needed to serialize the current <see cref="QueryDefinition"/> instance. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
  /// <param name="context">The contextual information about the source or destination of the serialization.</param>
  void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
  {
    GetObjectData (info, context);
  }

  /// <summary>
  /// Populates a specified <see cref="System.Runtime.Serialization.SerializationInfo"/> with the 
  /// data needed to serialize the current <see cref="QueryDefinition"/> instance. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
  /// <param name="context">The contextual information about the source or destination of the serialization.</param>
  /// <note type="inheritinfo">Overwrite this method to support serialization of derived classes.</note>
  protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
  {
    info.AddValue ("ID", _id);

    bool isPartOfQueryConfiguration = DomainObjectsConfiguration.Current.Query.QueryDefinitions.Contains (this);
    info.AddValue ("IsPartOfQueryConfiguration", isPartOfQueryConfiguration);

    if (!isPartOfQueryConfiguration)
    {
      info.AddValue ("StorageProviderID", StorageProviderDefinition.Name);
      info.AddValue ("Statement", _statement);
      info.AddValue ("QueryType", _queryType);
      info.AddValue ("CollectionType", _collectionType);
    }
  }

  #endregion

  #region IObjectReference Members

  /// <summary>
  /// Returns a reference to the real object that should be deserialized. See remarks 
  /// on <see cref="QueryDefinition"/> for further details.
  /// </summary>
  /// <param name="context">The source and destination of a given serialized stream.</param>
  /// <returns>Returns the actual <see cref="QueryDefinition"/>.</returns>
  object IObjectReference.GetRealObject (StreamingContext context)
  {
    if (_ispartOfQueryConfiguration)
      return DomainObjectsConfiguration.Current.Query.QueryDefinitions.GetMandatory (_id);
    else
      return this;
  }

  #endregion
}
}
