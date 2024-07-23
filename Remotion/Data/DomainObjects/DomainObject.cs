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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Base class for all objects that are persisted by the framework.
  /// </summary>
  [IgnoreForMappingConfiguration]
  public class DomainObject : IDomainObject
  {
    #region Creation and GetObject factory methods

    /// <summary>
    /// Returns a new instance of a concrete domain object for the current <see cref="DomainObjects.ClientTransaction"/>. The object is constructed
    /// using the default constructor in the <see cref="DomainObjects.ClientTransaction.Current"/> <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The concrete type to be implemented by the object.</typeparam>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in 
    /// management tasks at runtime.
    /// </para>
    /// <para>This method should not be directly invoked by a user, but instead by static factory methods of classes derived from
    /// <see cref="DomainObject"/>.</para>
    /// <para>For more information, also see the constructor documentation (<see cref="DomainObject()"/>).</para>
    /// </remarks>
    /// <seealso cref="DomainObject()"/>
    /// <exception cref="ArgumentException">The type <typeparamref name="T"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement the required protected
    /// constructor (see Remarks section).
    /// </exception>
    protected static T NewObject<T> () where T : DomainObject
    {
      return NewObject<T>(ParamList.Empty);
    }

    /// <summary>
    /// Returns a new instance of a concrete domain object for the current <see cref="DomainObjects.ClientTransaction"/>. The object is constructed
    /// using the supplied constructor arguments in the <see cref="DomainObjects.ClientTransaction.Current"/> <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">The concrete type to be implemented by the object.</typeparam>
    /// <param name="constructorParameters">A <see cref="ParamList"/> encapsulating the parameters to be passed to the constructor. Instantiate this
    /// by using one of the <see cref="ParamList.Create{A1,A2}"/> methods.</param>
    /// <returns>A new domain object instance.</returns>
    /// <remarks>
    /// <para>
    /// Objects created by this factory method are not directly instantiated; instead a proxy is dynamically created, which will assist in 
    /// management tasks at runtime.
    /// </para>
    /// <para>This method should not be directly invoked by a user, but instead by static factory methods of classes derived from
    /// <see cref="DomainObject"/>.</para>
    /// <para>For more information, also see the constructor documentation (<see cref="DomainObject()"/>).</para>
    /// </remarks>
    /// <seealso cref="DomainObject()"/>
    /// <exception cref="ArgumentException">The type <typeparamref name="T"/> cannot be extended to a proxy, for example because it is sealed
    /// or abstract and non-instantiable.</exception>
    /// <exception cref="MissingMethodException">The given type <typeparamref name="T"/> does not implement the required protected
    /// constructor (see Remarks section).
    /// </exception>
    protected static T NewObject<T> (ParamList constructorParameters) where T : DomainObject
    {
      ArgumentUtility.CheckNotNull("constructorParameters", constructorParameters);

      return (T)LifetimeService.NewObject(ClientTransactionScope.CurrentTransaction, typeof(T), constructorParameters);
    }

    #endregion

    /// <summary>
    /// Occurs before a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
    /// </summary>
    /// <remarks>
    /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
    /// </remarks>
    public event EventHandler<PropertyChangeEventArgs>? PropertyChanging;

    /// <summary>
    /// Occurs after a <see cref="PropertyValue"/> of the <see cref="DomainObject"/> is changed.
    /// </summary>
    /// <remarks>
    /// This event does not fire when a <see cref="PropertyValue"/> has been changed due to a relation change.
    /// </remarks>
    public event EventHandler<PropertyChangeEventArgs>? PropertyChanged;

    /// <summary>
    /// Occurs before a Relation of the <see cref="DomainObject"/> is changed.
    /// This event might be raised more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, this event is raised once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    public event EventHandler<RelationChangingEventArgs>? RelationChanging;

    /// <summary>
    /// Occurs after a Relation of the <see cref="DomainObject"/> has been changed.
    /// This event might be raised more than once for a given relation change operation. For example, when a whole related object collection is 
    /// replaced in one go, this event is raised once for each old object that is not in the new collection and once for each new object not in the 
    /// old collection.
    /// </summary>
    public event EventHandler<RelationChangedEventArgs>? RelationChanged;

    /// <summary>
    /// Occurs before the <see cref="DomainObject"/> is deleted.
    /// </summary>
    public event EventHandler? Deleting;

    /// <summary>
    /// Occurs after the <see cref="DomainObject"/> has been deleted.
    /// </summary>
    public event EventHandler? Deleted;

    /// <summary>
    /// Occurs before the changes of a <see cref="DomainObject"/> are committed.
    /// </summary>
    public event EventHandler<DomainObjectCommittingEventArgs>? Committing;

    /// <summary>
    /// Occurs after the changes of a <see cref="DomainObject"/> are successfully committed.
    /// </summary>
    public event EventHandler? Committed;

    /// <summary>
    /// Occurs before the changes of a <see cref="DomainObject"/> are rolled back.
    /// </summary>
    public event EventHandler? RollingBack;

    /// <summary>
    /// Occurs after the changes of a <see cref="DomainObject"/> are successfully rolled back.
    /// </summary>
    public event EventHandler? RolledBack;

    private ObjectID _id;
    private ClientTransaction _rootTransaction;
    private bool _needsLoadModeDataContainerOnly; // true if the object was created by a constructor call or OnLoaded has already been called once
    private DomainObjectTransactionContextImplementation _transactionContextImplementation;

    /// <summary>
    /// Initializes a new <see cref="DomainObject"/> with the current <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Any constructors implemented on concrete domain objects should delegate to this base constructor. As domain objects generally should 
    /// not be constructed via the
    /// <c>new</c> operator, these constructors must remain protected, and the concrete domain objects should have a static "NewObject" method,
    /// which delegates to <see cref="NewObject{T}(ParamList)"/>, passing it the required constructor arguments.
    /// </para>
    /// <para>
    /// It is safe to access virtual properties that are automatically implemented by the framework from constructors because this base constructor
    /// prepares everything necessary for them to work.
    /// </para>
    /// </remarks>
    protected DomainObject ()
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      PerformConstructorCheck();
      // ReSharper restore DoNotCallOverridableMethodsInConstructor

      Assertion.IsNotNull(ClientTransaction.Current, "This constructor cannot be called with a null ClientTransaction.");
      var initializationContext = ObjectInititalizationContextScope.CurrentObjectInitializationContext;
      if (initializationContext == null)
      {
        throw new InvalidOperationException(
            "The DomainObject constructor may only be called via ClientTransaction.NewObject. "
            + "If this exception occurs during a base call of a deserialization constructor, adjust the base call to call the DomainObject's "
            + "deserialization constructor instead.");
      }
      Initialize(initializationContext.ObjectID, initializationContext.RootTransaction);
      initializationContext.RegisterObject(this);

      RaiseReferenceInitializatingEvent();

      _needsLoadModeDataContainerOnly = true;
    }

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="DomainObject"/>.
    /// </summary>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the root <see cref="ClientTransaction"/> of the transaction hierarchy this <see cref="DomainObject"/> is associated with.
    /// </summary>
    /// <value>The <see cref="DomainObjects.ClientTransaction"/> this object is bound to.</value>
    /// <remarks>
    /// When a <see cref="DomainObject"/> is created, loaded, or its reference is initialized within a specific <see cref="ClientTransaction"/>, it
    /// automatically becomes associated with the hierarchy of that <see cref="ClientTransaction"/>. It can then always be used in 
    /// </remarks>
    public ClientTransaction RootTransaction
    {
      get { return _rootTransaction; }
    }

    /// <summary>
    /// Gets a <see cref="DomainObjectTransactionContextIndexer"/> object that can be used to select an <see cref="DomainObjectTransactionContext"/>
    /// for a specific <see cref="DomainObjects.ClientTransaction"/>. To obtain the default context, use <see cref="DefaultTransactionContext"/>.
    /// </summary>
    /// <value>The transaction context.</value>
    public DomainObjectTransactionContextIndexer TransactionContext => new DomainObjectTransactionContextIndexer(_transactionContextImplementation);

    /// <summary>
    /// Gets the default <see cref="DomainObjectTransactionContext"/>, i.e. the transaction context that is used when this
    /// <see cref="DomainObject"/>'s properties are accessed without specifying a <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <value>The default transaction context.</value>
    /// <remarks>
    /// The default transaction for a <see cref="DomainObject"/> is the <see cref="ClientTransaction.ActiveTransaction"/> of the associated 
    /// <see cref="RootTransaction"/>. The <see cref="ClientTransaction.ActiveTransaction"/> is usually the 
    /// <see cref="ClientTransaction.LeafTransaction"/>, but it can be changed by using <see cref="ClientTransaction"/> APIs.
    /// </remarks>
    public DomainObjectTransactionContext DefaultTransactionContext
    {
      get { return this.GetDefaultTransactionContext(); }
    }

    /// <summary>
    /// Gets the current state of the <see cref="DomainObject"/> in the <see cref="ClientTransactionScope.CurrentTransaction"/>.
    /// </summary>
    public DomainObjectState State
    {
      get { return this.GetState(); }
    }

    /// <summary>
    /// Gets a value indicating whether the object is invalid in the default transaction, ie. in its binding transaction or - if
    /// none - <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object becomes invalid see <see cref="ObjectInvalidException"/>.
    /// </remarks>
    [Obsolete("Use State.IsInvalid instead. (Version: 1.21.8)", false)]
    public bool IsInvalid
    {
      get { return State.IsInvalid; }
    }

    /// <summary>
    /// Gets the timestamp used for optimistic locking when the object is committed to the database in the default transaction, ie. in 
    /// its binding transaction or - if none - <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <value>The timestamp of the object.</value>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    public object? Timestamp
    {
      get { return this.GetTimestamp(); }
    }

    /// <summary>
    /// Ensures that <see cref="DomainObject"/> instances are not created via constructor checks.
    /// </summary>
    /// <remarks>
    /// The default implementation of this method throws an exception. When the runtime code generation invoked via <see cref="NewObject{T}()"/>
    /// generates a concrete <see cref="DomainObject"/> type, it overrides this method to disable the exception. This ensures that 
    /// <see cref="DomainObject"/> instances cannot be created simply by calling the <see cref="DomainObject"/>'s constructor.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void PerformConstructorCheck ()
    {
      throw new InvalidOperationException(
          "DomainObject constructors must not be called directly. Use DomainObject.NewObject to create DomainObject "
          + "instances.");
    }

    /// <summary>
    /// Initializes a new <see cref="DomainObject"/> during a call to <see cref="NewObject{T}()"/> or 
    /// <see cref="LifetimeService.GetObject(Remotion.Data.DomainObjects.ClientTransaction,Remotion.Data.DomainObjects.ObjectID,bool)"/>. This method
    /// is automatically called by the framework and should not normally be invoked by user code.
    /// </summary>
    /// <param name="id">The <see cref="ObjectID"/> to associate the new <see cref="DomainObject"/> with.</param>
    /// <param name="rootTransaction">The root <see cref="DomainObjects.ClientTransaction"/> to associate the new <see cref="DomainObject"/> with.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="id"/> or <paramref name="rootTransaction"/> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">This <see cref="DomainObject"/> has already been initialized.</exception>
    /// <remarks>This method is always called exactly once per <see cref="DomainObject"/> instance by the framework. It sets the object's 
    /// <see cref="ID"/> and enlists it with the given <see cref="DomainObjects.ClientTransaction"/>.</remarks>
    [MemberNotNull(nameof(_id))]
    [MemberNotNull(nameof(_rootTransaction))]
    [MemberNotNull(nameof(_transactionContextImplementation))]
    public void Initialize (ObjectID id, ClientTransaction rootTransaction)
    {
      ArgumentUtility.CheckNotNull("id", id);
      ArgumentUtility.CheckNotNull("rootTransaction", rootTransaction);

      if (rootTransaction.RootTransaction != rootTransaction)
        throw new ArgumentException("The rootTransaction parameter must be passed a root transaction.", "rootTransaction");

      if (_id != null)
        throw new InvalidOperationException("The object cannot be initialized, it already has an ID.");

      _id = id;
      _rootTransaction = rootTransaction;
      _transactionContextImplementation = new DomainObjectTransactionContextImplementation(this);
    }

    /// <summary>
    /// GetType might return a <see cref="Type"/> object for a generated class, which is usually not what is expected.
    /// <see cref="DomainObject.GetPublicDomainObjectType"/> can be used to get the Type object of the original underlying domain object type. If
    /// the <see cref="Type"/> object for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.
    /// </summary>
    [Obsolete("GetType might return a Type object for a generated class, which is usually not what is expected. "
               + "DomainObject.GetPublicDomainObjectType can be used to get the Type object of the original underlying domain object type. If the Type object"
               + "for the generated class is explicitly required, this object can be cast to 'object' before calling GetType.", true)]
    public new Type GetType ()
    {
      throw new InvalidOperationException("DomainObject.GetType should not be used.");
    }

    /// <summary>
    /// Returns the public type representation of this domain object, i.e. the type object visible to mappings, database, etc.
    /// </summary>
    /// <returns>The public type representation of this domain object.</returns>
    public Type GetPublicDomainObjectType ()
    {
      return GetPublicDomainObjectTypeImplementation();
    }

    /// <summary>
    /// Implements the functionality required by <see cref="GetPublicDomainObjectType"/>. This is a separate method to avoid having to make the 
    /// virtual call in the constructor. The implementation of this class must expect calls from the constructor of a base class.
    /// </summary>
    /// <returns>The public type representation of this domain object.</returns>
    /// <remarks>A domain object should override this method if it wants to impersonate one of its base types. The framework will handle this object
    /// as if it was of the type returned by this method and ignore its actual type.</remarks>
    protected virtual Type GetPublicDomainObjectTypeImplementation ()
    {
      return base.GetType();
    }

    /// <summary>
    /// Returns a textual representation of this object's <see cref="ID"/>.
    /// </summary>
    /// <returns>
    /// A textual representation of <see cref="ID"/>.
    /// </returns>
    public override string ToString ()
    {
      return ID.ToString();
    }

    /// <summary>
    /// Deletes the <see cref="DomainObject"/> in the default transaction, ie. in its binding transaction or - if
    /// none - <see cref="DomainObjects.ClientTransaction.Current"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The object is invalid in the transaction.</exception>
    /// <remarks>To perform custom actions when a <see cref="DomainObject"/> is deleted <see cref="OnDeleting"/> and <see cref="OnDeleted"/> should be overridden.</remarks>
    protected void Delete ()
    {
      CheckInitializeEventNotExecuting();
      LifetimeService.DeleteObject(DefaultTransactionContext.ClientTransaction, this);
    }

    private void CheckInitializeEventNotExecuting ()
    {
      _transactionContextImplementation.CheckForDomainObjectReferenceInitializing();
    }

    /// <summary>
    /// Provides simple, encapsulated access to the current property.
    /// </summary>
    /// <value>A <see cref="PropertyAccessor"/> object encapsulating the current property.</value>
    /// <remarks>
    /// The structure returned by this method allows simple access to the property's value and mapping definition objects regardless of
    /// whether it is a simple value property, a related object property, or a related object collection property.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The current property hasn't been initialized or there is no current property. Perhaps the domain 
    /// object was created with the <c>new</c> operator instead of using the <see cref="NewObject{T}()"/> method, or the property is not virtual.</exception>
    protected PropertyAccessor CurrentProperty
    {
      get
      {
        CheckInitializeEventNotExecuting();

        string propertyName = CurrentPropertyManager.GetAndCheckCurrentPropertyName();
        return Properties[propertyName];
      }
    }

    /// <summary>
    /// Provides simple, encapsulated access to the properties of this <see cref="DomainObject"/>.
    /// </summary>
    /// <returns>A <see cref="PropertyIndexer"/> object which can be used to select a specific property of this <see cref="DomainObject"/>.</returns>
    protected PropertyIndexer Properties
    {
      get
      {
        CheckInitializeEventNotExecuting();

        return new PropertyIndexer(this);
      }
    }

    /// <summary>
    /// Calls the <see cref="OnReferenceInitializing"/> method, setting a flag indicating that no mapped properties must be used.
    /// </summary>
    internal void RaiseReferenceInitializatingEvent ()
    {
      _transactionContextImplementation.BeginDomainObjectReferenceInitializing();
      try
      {
        OnReferenceInitializing();
        DomainObjectMixinCodeGenerationBridge.OnDomainObjectReferenceInitializing(this);
      }
      finally
      {
        _transactionContextImplementation.EndDomainObjectReferenceInitializing();
      }
    }

    /// <summary>
    /// Calls the <see cref="OnLoaded(LoadMode)"/> method with the right <see cref="LoadMode"/> parameter.
    /// </summary>
    internal void OnLoaded ()
    {
      LoadMode loadMode = _needsLoadModeDataContainerOnly ? LoadMode.DataContainerLoadedOnly : LoadMode.WholeDomainObjectInitialized;
      _needsLoadModeDataContainerOnly = true;

      DomainObjectMixinCodeGenerationBridge.OnDomainObjectLoaded(this, loadMode);
      OnLoaded(loadMode);
    }

    /// <summary>
    /// This method is invoked while this <see cref="DomainObject"/> is being initialized. This occurs whenever a <see cref="DomainObject"/> 
    /// is initialized, no matter whether the object is created, loaded, transported, cloned, or somehow else instantiated, and it occurs as early 
    /// as possible, after the object was enlisted with a transaction and at a point of time where it is safe to access the <see cref="ID"/> of the 
    /// object. The <see cref="OnReferenceInitializing"/> notification occurs exactly once per DomainObject, and its purpose is the initialization of 
    /// DomainObject fields that do not depend on the object's mapped data properties. 
    /// See restrictions in the Remarks section.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Override this method to initialize fields and properties of a <see cref="DomainObject"/> that do not depend on the object's mapped data
    /// properties, no matter how the object is created.
    /// </para>
    /// <para>
    /// While this method is being executed, it is not possible to access any properties or methods of the DomainObject that read or modify the state 
    /// or data of the object in a <see cref="ClientTransaction"/>. All automatically implemented properties, <see cref="CurrentProperty"/>, 
    /// <see cref="Properties"/>, <see cref="State"/>, <see cref="Timestamp"/>, <see cref="DomainObjectExtensions.RegisterForCommit"/>, 
    /// <see cref="DomainObjectExtensions.EnsureDataAvailable"/>, etc. will throw <see cref="InvalidOperationException"/>. 
    /// It is possible to inspect the <see cref="RootTransaction"/> of the object, and the object is guaranteed to be enlisted in the 
    /// <see cref="ClientTransaction.Current"/> transaction.
    /// </para>
    /// <para>The reason why it is explicitly disallowed to access mapped properties from the notification method is that 
    /// <see cref="OnReferenceInitializing"/> is usually called when no data has yet been loaded for the object. Accessing a property would cause the 
    /// data to be loaded, defeating lazy loading via object references.
    /// </para>
    /// <para>
    /// To initialize an object based on its data, use the constructor, <see cref="OnLoaded(Remotion.Data.DomainObjects.LoadMode)"/>, or the facility 
    /// callbacks. <see cref="OnLoaded(Remotion.Data.DomainObjects.LoadMode)"/> might be called more than once per object.
    /// </para>
    /// <para>
    /// When a <see cref="DomainObject"/> is newly created (usually via <see cref="NewObject{T}()"/>), this method is called while the base 
    /// <see cref="DomainObject"/> is executing. Fields initialized by a derived constructor will not be set yet.
    /// </para>
    /// </remarks>
    protected virtual void OnReferenceInitializing ()
    {
    }

    /// <summary>
    /// This method is invoked after the loading process of the object is completed.
    /// </summary>
    /// <param name="loadMode">Specifies whether the whole domain object or only the <see cref="Remotion.Data.DomainObjects.DataManagement.DataContainer"/> has been
    /// newly loaded.</param>
    /// <remarks>
    /// <para>
    /// Override this method to initialize <see cref="DomainObject"/>s that are loaded from the underlying storage.
    /// </para>
    /// <para>
    /// When a <see cref="DomainObject"/> is loaded for the first time, a new <see cref="DomainObject"/> reference will be created for it. In this
    /// case, the <see cref="OnLoaded(LoadMode)"/> method will be called with <see cref="LoadMode.WholeDomainObjectInitialized"/> being passed to the
    /// method. When, however, an additional <see cref="DataContainer"/> is loaded for an existing <see cref="DomainObject"/> reference - 
    /// in reaction to an existing <see cref="DomainObject"/> being loaded into another transaction (eg. a subtransaction), 
    /// <see cref="LoadMode.DataContainerLoadedOnly"/> is passed to the method.
    /// </para>
    /// <para>
    /// Even when an object is first loaded in a subtransaction, this method is called once with <see cref="LoadMode.WholeDomainObjectInitialized"/>,
    /// and then once with <see cref="LoadMode.DataContainerLoadedOnly"/>. <see cref="LoadMode.WholeDomainObjectInitialized"/> can thus be used to
    /// identify when the object was actually loaded from the underlying storage.
    /// </para>
    /// </remarks>
    protected virtual void OnLoaded (LoadMode loadMode)
    {
    }

    /// <summary>
    /// This method is invoked before an object's data is unloaded from the <see cref="ClientTransaction.Current"/> transaction.
    /// </summary>
    /// <remarks>
    /// <note type="inotes">Overrides of this method can throw an exception in order to stop the operation.</note>
    /// </remarks>
    protected internal virtual void OnUnloading ()
    {
    }

    /// <summary>
    /// This method is invoked after an object's data has been unloaded from the <see cref="ClientTransaction.Current"/> transaction.
    /// </summary>
    /// <remarks>
    /// <note type="inotes">Overrides of this method must not throw an exception; the operation has already been performed here.</note>
    /// </remarks>
    protected internal virtual void OnUnloaded ()
    {
    }

    /// <summary>
    /// Raises the <see cref="Committing"/> event.
    /// </summary>
    /// <param name="args">A <see cref="DomainObjectCommittingEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnCommitting (DomainObjectCommittingEventArgs args)
    {
      if (Committing != null)
        Committing(this, args);
    }

    /// <summary>
    /// Raises the <see cref="Committed"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnCommitted (EventArgs args)
    {
      if (Committed != null)
        Committed(this, args);
    }

    /// <summary>
    /// Raises the <see cref="RollingBack"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRollingBack (EventArgs args)
    {
      if (RollingBack != null)
        RollingBack(this, args);
    }

    /// <summary>
    /// Raises the <see cref="RolledBack"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRolledBack (EventArgs args)
    {
      if (RolledBack != null)
        RolledBack(this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanging"/> event.
    /// This method is invoked once per involved operation and thus might be raised more often than <see cref="OnRelationChanged"/>. For example,
    /// when a whole related object collection is replaced in one go, this method is invoked once for each old object that is not in the new collection
    /// and once for each new object not in the old collection.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangingEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRelationChanging (RelationChangingEventArgs args)
    {
      if (RelationChanging != null)
        RelationChanging(this, args);
    }

    /// <summary>
    /// Raises the <see cref="RelationChanged"/> event.
    /// This method is only invoked once per relation change and thus might be invoked less often than <see cref="OnRelationChanging"/>.
    /// </summary>
    /// <param name="args">A <see cref="RelationChangedEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnRelationChanged (RelationChangedEventArgs args)
    {
      if (RelationChanged != null)
        RelationChanged(this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanging"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnPropertyChanging (PropertyChangeEventArgs args)
    {
      if (PropertyChanging != null)
        PropertyChanging(this, args);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="args">A <see cref="PropertyChangeEventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnPropertyChanged (PropertyChangeEventArgs args)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deleting"/> event.
    /// </summary>
    /// <param name="args">A <see cref="System.EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnDeleting (EventArgs args)
    {
      if (Deleting != null)
        Deleting(this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deleted"/> event.
    /// </summary>
    /// <param name="args">A <see cref="EventArgs"/> object that contains the event data.</param>
    protected internal virtual void OnDeleted (EventArgs args)
    {
      if (Deleted != null)
        Deleted(this, args);
    }

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/> is included in the commit set of its <see cref="ClientTransaction.ActiveTransaction"/>. 
    /// The object's <see cref="State"/>.<see cref="DomainObjectState.IsInvalid"/> must not be set,
    /// and if <see cref="State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag is set, this method loads the object's data.
    /// </summary>
    /// <remarks>This method is only provided for compatibility, i.e. to make it easier to call the actual implementation.</remarks>
    /// <seealso cref="DomainObjectExtensions.RegisterForCommit"/>
    protected void RegisterForCommit ()
    {
      DomainObjectExtensions.RegisterForCommit(this);
    }

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded into the its <see cref="ClientTransaction.ActiveTransaction"/>.
    /// If it hasn't, this method causes the object's data to be loaded. If the object's data can't be found, an exception is thrown.
    /// </summary>
    /// <remarks>This method is only provided for compatibility, i.e. to make it easier to call the actual implementation.</remarks>
    /// <seealso cref="DomainObjectExtensions.EnsureDataAvailable"/>
    protected void EnsureDataAvailable ()
    {
      DomainObjectExtensions.EnsureDataAvailable(this);
    }

    /// <summary>
    /// Ensures that the <see cref="DomainObject"/>'s data has been loaded into its <see cref="ClientTransaction.ActiveTransaction"/>.
    /// If it hasn't, this method causes the object's data to be loaded. The method returns a value indicating whether the object's data was found.
    /// </summary>
    /// <remarks>This method is only provided for compatibility, i.e. to make it easier to call the actual implementation.</remarks>
    /// <seealso cref="DomainObjectExtensions.TryEnsureDataAvailable"/>
    protected bool TryEnsureDataAvailable ()
    {
      return DomainObjectExtensions.TryEnsureDataAvailable(this);
    }
  }
}
