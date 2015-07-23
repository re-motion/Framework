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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a container for the persisted properties of a DomainObject.
  /// </summary>
  public sealed class DataContainer : IFlattenedSerializable
  {
    // types

    private enum DataContainerStateType
    {
      Existing = 0,
      New = 1,
      Deleted = 2
    }

    // static members and constants

    /// <summary>
    /// Creates an empty <see cref="DataContainer"/> for a new <see cref="Remotion.Data.DomainObjects.DomainObject"/>. The <see cref="DataContainer"/>
    /// contains a new <see cref="PropertyValue"/> object for every <see cref="PropertyDefinition"/> in the respective <see cref="ClassDefinition"/>.
    /// The <see cref="DataContainer"/> has be to <see cref="DataManager.RegisterDataContainer">registered</see> with a 
    /// <see cref="ClientTransaction"/> and its <see cref="DomainObject"/> must <see cref="SetDomainObject">be set</see> before it can be used.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.New"/>. All <see cref="PropertyValue"/>s for the class specified by <see cref="ObjectID.ClassID"/> are created.
    /// </remarks>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
    /// <returns>The new <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    public static DataContainer CreateNew (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var propertyValues = id.ClassDefinition.GetPropertyDefinitions().ToDictionary (pd => pd, pd => new PropertyValue (pd, pd.DefaultValue));
      return new DataContainer (id, DataContainerStateType.New, null, propertyValues);
    }

    /// <summary>
    /// Creates an empty <see cref="DataContainer"/> for an existing <see cref="Remotion.Data.DomainObjects.DomainObject"/>. The <see cref="DataContainer"/>
    /// contain all <see cref="PropertyValue"/> objects, just as if it had been created with <see cref="CreateNew"/>, but the values for its 
    /// properties are set as returned by a lookup method.
    /// The <see cref="DataContainer"/> has be to registered with a <see cref="ClientTransaction"/> via <see cref="DataManager.RegisterDataContainer"/> 
    /// and <see cref="SetDomainObject"/> must be called before it can be used.
    /// </summary>
    /// <remarks>
    /// The new <see cref="DataContainer"/> has a <see cref="State"/> of <see cref="StateType.Unchanged"/>. All <see cref="PropertyValue"/>s for the 
    /// class specified by <see cref="ObjectID.ClassID"/> are created.
    /// </remarks>
    /// <param name="id">The <see cref="ObjectID"/> of the new <see cref="DataContainer"/> to create. Must not be <see langword="null"/>.</param>
    /// <param name="timestamp">The timestamp value of the existing object in the data source.</param>
    /// <param name="valueLookup">A function object returning the value of a given property for the existing object.</param>
    /// <returns>The new <see cref="DataContainer"/>.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
    /// <exception cref="Mapping.MappingException">ClassDefinition of <paramref name="id"/> does not exist in mapping.</exception>
    public static DataContainer CreateForExisting (ObjectID id, object timestamp, Func<PropertyDefinition, object> valueLookup)
    {
      ArgumentUtility.CheckNotNull ("id", id);

      var propertyValues = id.ClassDefinition.GetPropertyDefinitions().ToDictionary (pd => pd, pd => new PropertyValue (pd, valueLookup (pd)));
      return new DataContainer (id, DataContainerStateType.Existing, timestamp, propertyValues);
    }

    private readonly ObjectID _id;
    private readonly Dictionary<PropertyDefinition, PropertyValue> _propertyValues;

    private ClientTransaction _clientTransaction;
    private IDataContainerEventListener _eventListener;
    private DomainObject _domainObject;

    private RelationEndPointID[] _associatedRelationEndPointIDs;

    private object _timestamp;
    private DataContainerStateType _state;
    private bool _isDiscarded;
    private bool _hasBeenMarkedChanged;
    private bool? _hasBeenChanged;

    // construction and disposing

    private DataContainer (ObjectID id, DataContainerStateType state, object timestamp, Dictionary<PropertyDefinition, PropertyValue> propertyValues)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("propertyValues", propertyValues);

      _id = id;
      _timestamp = timestamp;
      _state = state;

      _propertyValues = propertyValues;
    }

    public bool HasBeenMarkedChanged
    {
      get { return _hasBeenMarkedChanged; }
    }

    public object GetValue (PropertyDefinition propertyDefinition, ValueAccess valueAccess = ValueAccess.Current)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      
      RaisePropertyValueReadingNotification (propertyDefinition, valueAccess);
      object value = GetValueWithoutEvents (propertyValue, valueAccess);
     RaisePropertyValueReadNotification (propertyDefinition, value, valueAccess);
      
      return value;
    }

    public void SetValue (PropertyDefinition propertyDefinition, object value)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded();

      if (_state == DataContainerStateType.Deleted)
        throw new ObjectDeletedException (_id);

      var propertyValue = GetPropertyValue (propertyDefinition);
      if (!PropertyValue.AreValuesDifferent (propertyValue.Value, value))
      {
        propertyValue.Touch();
        return;
      }
      
      RaisePropertyValueChangingNotification (propertyDefinition, propertyValue.Value, value);

      var oldValue = propertyValue.Value;
      propertyValue.Value = value;

      // set _hasBeenChanged to true if:
      // - we were not changed before this event (now we must be - the property only fires this event when it was set to a different value)
      // - the property indicates that it doesn't have the original value ("HasChanged")
      // - recalculation of all property change states indicates another property doesn't have its original value
      if (_hasBeenChanged != null && !_hasBeenChanged.Value)
        _hasBeenChanged = true;
      else if (propertyValue.HasChanged)
        _hasBeenChanged = true;
      else
        _hasBeenChanged = null;

      RaiseStateUpdatedNotification (State);
      RaisePropertyValueChangedNotification(propertyDefinition, oldValue, value);
    }

    public object GetValueWithoutEvents (PropertyDefinition propertyDefinition, ValueAccess valueAccess = ValueAccess.Current)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      return GetValueWithoutEvents(propertyValue, valueAccess);
    }

    private object GetValueWithoutEvents (PropertyValue propertyValue, ValueAccess valueAccess)
    {
      if (valueAccess == ValueAccess.Current)
        return propertyValue.Value;
      else
        return propertyValue.OriginalValue;
    }

    public void TouchValue (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      propertyValue.Touch();
    }

    public bool HasValueBeenTouched (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      return propertyValue.HasBeenTouched;
    }

    public bool HasValueChanged (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      return propertyValue.HasChanged;
    }

    public void CommitValue (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      propertyValue.CommitState();
      
      // Invalidate state rather than recalculating it - CommitValue might be called multiple times.
      _hasBeenChanged = null;
    }

    public void RollbackValue (PropertyDefinition propertyDefinition)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      CheckNotDiscarded ();

      var propertyValue = GetPropertyValue (propertyDefinition);
      propertyValue.RollbackState();

      // Invalidate state rather than recalculating it - RollbackValue might be called multiple times.
      _hasBeenChanged = null;
    }

    public void SetValueDataFromSubTransaction (PropertyDefinition propertyDefinition, DataContainer sourceContainer)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("sourceContainer", sourceContainer);

      CheckNotDiscarded();
      sourceContainer.CheckNotDiscarded ();
      CheckSourceForSetDataFromSubTransaction (sourceContainer);

      var propertyValue = GetPropertyValue (propertyDefinition);
      var sourcePropertyValue = sourceContainer.GetPropertyValue (propertyDefinition);
      propertyValue.SetDataFromSubTransaction (sourcePropertyValue);

      // Invalidate state rather than recalculating it - SetValueDataFromSubTransaction might be called multiple times.
      _hasBeenChanged = null;
    }

    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.ClientTransaction"/> which the <see cref="DataContainer"/> is part of.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public ClientTransaction ClientTransaction
    {
      get
      {
        CheckNotDiscarded();

        if (_clientTransaction == null)
          throw new InvalidOperationException ("DataContainer has not been registered with a transaction.");

        return _clientTransaction;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has been registered with a <see cref="ClientTransaction"/>.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has been registered; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsRegistered
    {
      get 
      {
        return _clientTransaction != null;
      }
    }

    /// <summary>
    /// Gets the <see cref="IDataContainerEventListener"/> registered for this <see cref="DataContainer"/>, returning <see langword="null" /> if this 
    /// <see cref="DataContainer"/> doesn't yet have one.
    /// </summary>
    /// <value>The <see cref="IDataContainerEventListener"/> registered for this <see cref="DataContainer"/>, or <see langword="null" /> if no
    /// <see cref="IDataContainerEventListener"/> has been registered.</value>
    public IDataContainerEventListener EventListener
    {
      get { return _eventListener; }
    }

    /// <summary>
    /// Gets the <see cref="Remotion.Data.DomainObjects.DomainObject"/> associated with the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance has not been associated with a <see cref="DomainObject"/> yet.</exception>
    public DomainObject DomainObject
    {
      get
      {
        if (!HasDomainObject)
          throw new InvalidOperationException ("This DataContainer has not been associated with a DomainObject yet.");

        return _domainObject;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has been associated with a <see cref="DomainObjects.DomainObject"/>.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has a <see cref="DomainObjects.DomainObject"/>; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasDomainObject
    {
      get { return _domainObject != null; }
    }

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <remarks>
    /// This property can also be used when the <see cref="DataContainer"/> has been discarded.
    /// </remarks>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public ClassDefinition ClassDefinition
    {
      get
      {
        CheckNotDiscarded();
        return _id.ClassDefinition;
      }
    }

    /// <summary>
    /// Gets the <see cref="Type"/> of the <see cref="Remotion.Data.DomainObjects.DomainObject"/> of the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public Type DomainObjectType
    {
      get
      {
        CheckNotDiscarded();
        return _id.ClassDefinition.ClassType;
      }
    }

    /// <summary>
    /// Gets the state of the <see cref="DataContainer"/>.
    /// </summary>
    public StateType State
    {
      get
      {
        if (_isDiscarded)
          return StateType.Invalid;
        
        switch (_state)
        {
          case DataContainerStateType.New:
            return StateType.New;
          case DataContainerStateType.Deleted:
            return StateType.Deleted;
          default:
            Assertion.IsTrue (_state == DataContainerStateType.Existing);

            if (!_hasBeenChanged.HasValue)
              _hasBeenChanged = CalculatePropertyValueChangeState();

            if (_hasBeenMarkedChanged || _hasBeenChanged.Value)
              return StateType.Changed;
            else
              return StateType.Unchanged;
        }
      }
    }

    /// <summary>
    /// Marks an existing <see cref="DataContainer"/> as changed. <see cref="State"/> will return <see cref="StateType.Changed"/> after this method
    /// has been called.
    /// </summary>
    /// <exception cref="InvalidOperationException">This <see cref="DataContainer"/> is not in state <see cref="DataContainerStateType.Existing"/>.
    /// New or deleted objects cannot be marked as changed.</exception>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public void MarkAsChanged ()
    {
      CheckNotDiscarded();
      if (_state != DataContainerStateType.Existing)
        throw new InvalidOperationException ("Only existing DataContainers can be marked as changed.");

      _hasBeenMarkedChanged = true;

      RaiseStateUpdatedNotification (StateType.Changed);
    }

    /// <summary>
    /// Gets the timestamp of the last committed change of the data in the <see cref="DataContainer"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="DataContainer"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public object Timestamp
    {
      get
      {
        CheckNotDiscarded();
        return _timestamp;
      }
    }

    /// <summary>
    /// Gets a value indicating the discarded status of the <see cref="DataContainer"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when a <see cref="DataContainer"/> is discarded see <see cref="ObjectInvalidException"/>.
    /// </remarks>
    public bool IsDiscarded
    {
      get { return _isDiscarded; }
    }

    public RelationEndPointID[] AssociatedRelationEndPointIDs
    {
      get
      {
        if (_associatedRelationEndPointIDs == null)
          _associatedRelationEndPointIDs = RelationEndPointID.GetAllRelationEndPointIDs (ID).ToArray();

        return _associatedRelationEndPointIDs;
      }
    }

    public void SetTimestamp (object timestamp)
    {
      _timestamp = timestamp;
    }

    public void CommitState ()
    {
      CheckNotDiscarded ();

      if (_state == DataContainerStateType.Deleted)
        throw new InvalidOperationException ("Deleted data containers cannot be committed, they have to be discarded.");
      
      foreach (PropertyValue propertyValue in _propertyValues.Values)
        propertyValue.CommitState ();

      _hasBeenMarkedChanged = false;
      _hasBeenChanged = false;
      _state = DataContainerStateType.Existing;
      RaiseStateUpdatedNotification (StateType.Unchanged);
    }

    public void RollbackState ()
    {
      CheckNotDiscarded ();

      if (_state == DataContainerStateType.New)
        throw new InvalidOperationException ("New data containers cannot be rolled back, they have to be discarded.");

      foreach (PropertyValue propertyValue in _propertyValues.Values)
        propertyValue.RollbackState ();

      _hasBeenMarkedChanged = false;
      _hasBeenChanged = false;
      _state = DataContainerStateType.Existing;
      RaiseStateUpdatedNotification (StateType.Unchanged);
    }

    public void Delete ()
    {
      CheckNotDiscarded ();

      if (_state == DataContainerStateType.New)
        throw new InvalidOperationException ("New data containers cannot be deleted, they have to be discarded.");

      _state = DataContainerStateType.Deleted;
      RaiseStateUpdatedNotification (StateType.Deleted);
    }

    public void Discard ()
    {
      CheckNotDiscarded ();

      _isDiscarded = true;
      RaiseStateUpdatedNotification (StateType.Invalid);

      _clientTransaction = null;
      _eventListener = null;
    }

    public void SetPropertyDataFromSubTransaction (DataContainer source)
    {
      ArgumentUtility.CheckNotNull ("source", source);

      CheckNotDiscarded ();
      source.CheckNotDiscarded ();
      CheckSourceForSetDataFromSubTransaction(source);

      foreach (var kvp in _propertyValues)
      {
        var sourcePropertyValue = source.GetPropertyValue (kvp.Key);
        kvp.Value.SetDataFromSubTransaction (sourcePropertyValue);
      }

      _hasBeenChanged = null;
      RaiseStateUpdatedNotification (State);
    }

    public void SetDomainObject (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      if (domainObject.ID != null && domainObject.ID != _id)
        throw new ArgumentException ("The given DomainObject has another ID than this DataContainer.", "domainObject");
      if (_domainObject != null && _domainObject != domainObject)
        throw new InvalidOperationException ("This DataContainer has already been associated with a DomainObject.");

      _domainObject = domainObject;
    }

    public void SetEventListener (IDataContainerEventListener listener)
    {
      ArgumentUtility.CheckNotNull ("listener", listener);

      if (_eventListener != null)
        throw new InvalidOperationException ("Only one event listener can be registered for a DataContainer.");
      _eventListener = listener;
    }

    /// <summary>
    /// Creates a copy of this data container and its state.
    /// </summary>
    /// <returns>A copy of this data container with the same <see cref="ObjectID"/> and the same property values. The copy's
    /// <see cref="ClientTransaction"/> and <see cref="DomainObject"/> are not set, so the returned <see cref="DataContainer"/> cannot be 
    /// used until it is registered with a <see cref="DomainObjects.ClientTransaction"/>. Its <see cref="DomainObject"/> is set via the
    /// <see cref="SetDomainObject"/> method.</returns>
    public DataContainer Clone (ObjectID id)
    {
      CheckNotDiscarded ();

      var clonePropertyValues = _propertyValues.ToDictionary (kvp => kvp.Key, kvp => new PropertyValue (kvp.Key, kvp.Value.Value));

      var clone = new DataContainer (id, _state, _timestamp, clonePropertyValues);

      clone._hasBeenMarkedChanged = _hasBeenMarkedChanged;
      clone._hasBeenChanged = _hasBeenChanged;

      Assertion.IsNull (clone._clientTransaction);
      Assertion.IsNull (clone._domainObject);
      return clone;
    }

    internal void SetClientTransaction (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      if (_clientTransaction != null)
        throw new InvalidOperationException ("This DataContainer has already been registered with a ClientTransaction.");

      _clientTransaction = clientTransaction;
    }

    private PropertyValue GetPropertyValue (PropertyDefinition propertyDefinition)
    {
      try
      {
        return _propertyValues[propertyDefinition];
      }
      catch (KeyNotFoundException ex)
      {
        var message = string.Format ("Property '{0}' does not exist.", propertyDefinition.PropertyName);
        throw new ArgumentException (message, "propertyDefinition", ex);
      }
    }

    private void CheckNotDiscarded ()
    {
      if (_isDiscarded)
        throw new ObjectInvalidException (_id);
    }

    private bool CalculatePropertyValueChangeState ()
    {
      return _propertyValues.Values.Any (pv => pv.HasChanged);
    }

    private void RaisePropertyValueReadingNotification (PropertyDefinition propertyDefinition, ValueAccess valueAccess)
    {
      if (_eventListener != null)
        _eventListener.PropertyValueReading (this, propertyDefinition, valueAccess);
    }

    private void RaisePropertyValueReadNotification (PropertyDefinition propertyDefinition, object value, ValueAccess valueAccess)
    {
      if (_eventListener != null)
        _eventListener.PropertyValueRead (this, propertyDefinition, value, valueAccess);
    }

    private void RaisePropertyValueChangingNotification (PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      if (_eventListener != null)
        _eventListener.PropertyValueChanging (this, propertyDefinition, oldValue, newValue);
    }

    private void RaisePropertyValueChangedNotification (PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      if (_eventListener != null)
        _eventListener.PropertyValueChanged (this, propertyDefinition, oldValue, newValue);
    }

    private void RaiseStateUpdatedNotification (StateType state)
    {
      Assertion.DebugAssert (State == state);

      if (_eventListener != null)
        _eventListener.StateUpdated (this, state);
    }

    private void CheckSourceForSetDataFromSubTransaction (DataContainer source)
    {
      if (source.ClassDefinition != ClassDefinition)
      {
        var message = string.Format (
            "Cannot set this data container's property values from '{0}'; the data containers do not have the same class definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }
    }

    #region Serialization

    // ReSharper disable UnusedMember.Local
    private DataContainer (FlattenedDeserializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _id = info.GetValueForHandle<ObjectID> ();
      _timestamp = info.GetValue<object>();
      _isDiscarded = info.GetBoolValue ();

      _propertyValues = new Dictionary<PropertyDefinition, PropertyValue>();

      if (!_isDiscarded)
      {
        for (int i = 0; i < ClassDefinition.GetPropertyDefinitions ().Count (); ++i)
        {
          var propertyName = info.GetValueForHandle<string>();
          var propertyDefinition = ClassDefinition.GetPropertyDefinition (propertyName);
          var propertyValue = new PropertyValue (propertyDefinition, propertyDefinition.DefaultValue);
          propertyValue.DeserializeFromFlatStructure (info);
          _propertyValues.Add (propertyDefinition, propertyValue);
        }
      }

      _clientTransaction = info.GetValueForHandle<ClientTransaction> ();
      _eventListener = info.GetValueForHandle<IDataContainerEventListener> ();
      _state = (DataContainerStateType) info.GetIntValue ();
      _domainObject = info.GetValueForHandle<DomainObject> ();
      _hasBeenMarkedChanged = info.GetBoolValue ();
      _hasBeenChanged = info.GetValue<bool?>();
    }
    // ReSharper restore UnusedMember.Local

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddHandle (_id);
      info.AddValue (_timestamp);
      info.AddBoolValue (_isDiscarded);
      if (!_isDiscarded)
      {
        foreach (var kvp in _propertyValues)
        {
          info.AddHandle (kvp.Key.PropertyName);
          kvp.Value.SerializeIntoFlatStructure (info);
        }
      }

      info.AddHandle (_clientTransaction);
      info.AddHandle (_eventListener);
      info.AddIntValue ((int) _state);
      info.AddHandle (_domainObject);
      info.AddBoolValue(_hasBeenMarkedChanged);
      info.AddValue (_hasBeenChanged);
    }

    #endregion Serialization

    #region Obsolete
    [Obsolete ("This method is obsolete. Use Clone (ObjectID id) instead. (1.13.39)", true)]
// ReSharper disable UnusedParameter.Global
    public static DataContainer CreateAndCopyState (ObjectID id, DataContainer stateSource)
// ReSharper restore UnusedParameter.Global
    {
      throw new NotImplementedException ();
    }
    #endregion
  }
}