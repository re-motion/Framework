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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Mixins;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  using SubTransactionFactory =
      Func<ClientTransaction, IInvalidDomainObjectManager, IEnlistedDomainObjectManager, ITransactionHierarchyManager, IClientTransactionEventSink, ClientTransaction>;

/// <summary>
/// Represents an in-memory transaction.
/// </summary>
/// <remarks>
/// <para>
/// When a <see cref="ClientTransaction"/> is manually instantiated, it has to be activated for the current thread by using a
/// <see cref="ClientTransactionScope"/>, e.g. via calling <see cref="EnterDiscardingScope"/> or <see cref="EnterNonDiscardingScope"/>. The current transaction
/// for a thread can be retrieved via <see cref="Current"/> or <see cref="ClientTransactionScope.ActiveScope"/>.
/// </para>
/// <para>
/// <see cref="ClientTransaction">ClientTransaction's</see> methods temporarily set the <see cref="ClientTransactionScope"/> to this instance to
/// ensure they are executed in the right context.
/// </para>
/// </remarks>
#if FEATURE_SERIALIZATION
[Serializable]
#endif
public class ClientTransaction
{
  /// <summary>
  /// Creates a new root <see cref="ClientTransaction"/>, a transaction which uses a <see cref="RootPersistenceStrategy"/>.
  /// </summary>
  /// <returns>A new root <see cref="ClientTransaction"/> instance.</returns>
  /// <remarks>The object returned by this method can be extended with <b>Mixins</b> by configuring the <see cref="MixinConfiguration.ActiveConfiguration"/>
  /// to include a mixin for type <see cref="RootPersistenceStrategy"/>. Declaratively, this can be achieved by attaching an
  /// <see cref="ExtendsAttribute"/> instance for <see cref="ClientTransaction"/> or <see cref="RootPersistenceStrategy"/> to a mixin class.</remarks>
  public static ClientTransaction CreateRootTransaction ()
  {
    var componentFactory = RootClientTransactionComponentFactory.Create();
    return ObjectFactory.Create<ClientTransaction>(true, ParamList.Create(componentFactory));
  }

  /// <summary>
  /// Gets the <see cref="ClientTransaction"/> currently associated with this thread, or <see langword="null"/> if no such transaction exists.
  /// </summary>
  /// <value>The current <see cref="ClientTransaction"/> for the active thread, or <see langword="null"/> if no transaction is associated with it.</value>
  /// <remarks>This method is a shortcut for calling <see cref="ClientTransactionScope.CurrentTransaction"/>, but it doesn't throw an exception but
  /// return <see langword="null"/> if no transaction exists for the current thread.
  /// </remarks>
  public static ClientTransaction? Current
  {
    get
    {
      // Performance: In order to reduce SafeContext calls, we do not use HasCurrentTransaction/CurrentTransaction here
      var activeScope = ClientTransactionScope.ActiveScope;

      if (activeScope != null && activeScope.ScopedTransaction != null)
        return activeScope.ScopedTransaction;

      return null;
    }
  }

  // member fields

  /// <summary>
  /// Occurs when the <b>ClientTransaction</b> has created a subtransaction.
  /// </summary>
  public event EventHandler<SubTransactionCreatedEventArgs>? SubTransactionCreated;

  /// <summary>
  /// Occurs after the <b>ClientTransaction</b> has loaded a new object.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs>? Loaded;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Commit"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionCommittingEventArgs>? Committing;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Commit"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs>? Committed;

  /// <summary>
  /// Occurs immediately before the <b>ClientTransaction</b> performs a <see cref="Rollback"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs>? RollingBack;

  /// <summary>
  /// Occurs immediately after the <b>ClientTransaction</b> has successfully performed a <see cref="Rollback"/> operation.
  /// </summary>
  public event EventHandler<ClientTransactionEventArgs>? RolledBack;

  private readonly IDictionary<Enum, object> _applicationData;
  private readonly ITransactionHierarchyManager _hierarchyManager;
  private readonly IClientTransactionEventBroker _eventBroker;

  private readonly IEnlistedDomainObjectManager _enlistedDomainObjectManager;
  private readonly IInvalidDomainObjectManager _invalidDomainObjectManager;
  private readonly IDataManager _dataManager;
  private readonly IObjectLifetimeAgent _objectLifetimeAgent;
  private readonly IPersistenceStrategy _persistenceStrategy;
  private readonly IQueryManager _queryManager;
  private readonly ICommitRollbackAgent _commitRollbackAgent;

  private bool _isDiscarded;

  private readonly Guid _id = Guid.NewGuid();

  protected ClientTransaction (IClientTransactionComponentFactory componentFactory)
  {
    ArgumentUtility.CheckNotNull("componentFactory", componentFactory);

    _applicationData = componentFactory.CreateApplicationData(this);
    _eventBroker = componentFactory.CreateEventBroker(this);
    _hierarchyManager = componentFactory.CreateTransactionHierarchyManager(this, _eventBroker);
    _hierarchyManager.InstallListeners(_eventBroker);
    _enlistedDomainObjectManager = componentFactory.CreateEnlistedObjectManager(this);
    _invalidDomainObjectManager = componentFactory.CreateInvalidDomainObjectManager(this, _eventBroker);
    _persistenceStrategy = componentFactory.CreatePersistenceStrategy(this);
    _dataManager = componentFactory.CreateDataManager(this, _eventBroker, _invalidDomainObjectManager, _persistenceStrategy, _hierarchyManager);
    _objectLifetimeAgent = componentFactory.CreateObjectLifetimeAgent(
        this, _eventBroker, _invalidDomainObjectManager, _dataManager, _enlistedDomainObjectManager, _persistenceStrategy);
    _queryManager = componentFactory.CreateQueryManager(this, _eventBroker, _invalidDomainObjectManager, _persistenceStrategy, _dataManager, _hierarchyManager);
    _commitRollbackAgent = componentFactory.CreateCommitRollbackAgent(this, _eventBroker, _persistenceStrategy, _dataManager);

    var extensions = componentFactory.CreateExtensions(this);
    foreach (var extension in extensions)
      _eventBroker.Extensions.Add(extension);

    _hierarchyManager.OnBeforeTransactionInitialize();
    _eventBroker.RaiseTransactionInitializeEvent();
  }

  internal ITransactionHierarchyManager HierarchyManager
  {
    get { return _hierarchyManager; }
  }

  /// <summary>
  /// Gets the parent transaction for this <see cref="ClientTransaction"/>, or <see langword="null" /> if this transaction is a root transaction.
  /// </summary>
  /// <value>The parent transaction, or <see langword="null" /> if this transaction is a root transaction.</value>
  public ClientTransaction? ParentTransaction
  {
    get { return _hierarchyManager.ParentTransaction; }
  }

  /// <summary>
  /// Gets the active sub-transaction of this <see cref="ClientTransaction"/>, or <see langword="null" /> if this transaction has no sub-transaction.
  /// </summary>
  /// <value>The active sub-transaction, or <see langword="null" /> if this transaction has no sub-transaction.</value>
  /// <remarks>When the <see cref="SubTransaction"/> is discarded, this property is automatically set to <see langword="null" />.</remarks>
  public ClientTransaction? SubTransaction
  {
    get { return _hierarchyManager.SubTransaction; }
  }

  /// <summary>
  /// Gets the root transaction of this <see cref="ClientTransaction"/>, that is, the top-level transaction in a row of sub-transactions.
  /// If this <see cref="ClientTransaction"/> is itself a root transaction (i.e, it has no <see cref="ParentTransaction"/>), it is returned.
  /// </summary>
  /// <value>The root transaction of this <see cref="ClientTransaction"/>.</value>
  public ClientTransaction RootTransaction
  {
    get { return _hierarchyManager.TransactionHierarchy.RootTransaction; }
  }

  /// <summary>
  /// Gets the lowest sub-transaction of this <see cref="ClientTransaction"/>, that is, the bottom-most transaction in a row of sub-transactions.
  /// If this <see cref="ClientTransaction"/> is itself the leaf transaction (i.e, it has no <see cref="SubTransaction"/>), it itself is 
  /// returned.
  /// </summary>
  /// <value>The leaf transaction of this <see cref="ClientTransaction"/>.</value>
  public ClientTransaction LeafTransaction
  {
    get { return _hierarchyManager.TransactionHierarchy.LeafTransaction; }
  }

  /// <summary>
  /// Gets the active transaction in the associated <see cref="ClientTransaction"/> hierarchy, i.e., the transaction that is currently being used
  /// to execute <see cref="DomainObject"/> operations. The active transaction is controlled by the APIs such as <see cref="EnterNonDiscardingScope"/>,
  /// <see cref="EnterDiscardingScope"/>, or <see cref="ClientTransactionExtensions.ExecuteInScope"/>.
  /// </summary>
  public ClientTransaction ActiveTransaction
  {
    get { return _hierarchyManager.TransactionHierarchy.ActiveTransaction; }
  }

  /// <summary>
  /// Gets the persistence strategy associated with this <see cref="ClientTransaction"/>. The <see cref="PersistenceStrategy"/> is used to load
  /// data from the underlying data source without actually registering the data in this transaction, and it can be used to store data in the
  /// underlying data source.
  /// </summary>
  /// <value>The persistence strategy associated with this <see cref="ClientTransaction"/>.</value>
  protected IPersistenceStrategy PersistenceStrategy
  {
    get { return _persistenceStrategy; }
  }

  /// <summary>
  /// Returns a <see cref="Guid"/> that uniquely identifies this <see cref="ClientTransaction"/>.
  /// </summary>
  public Guid ID
  {
    get { return _id; }
  }

  /// <summary>
  /// Indicates whether this transaction can written, i.e., it has no <see cref="SubTransaction"/>.
  /// Transactions with an open <see cref="SubTransaction"/> can only be used to read and load data, not change it.
  /// </summary>
  /// <value><see langword="true" /> if this instance is writeable; otherwise, <see langword="false" />.</value>
  /// <remarks>
  /// <para>
  /// Transactions are made read-only while there exist open subtransactions for them. Such a transaction can only be used for
  /// operations that do not cause any change of transaction state. Reading, loading, and querying objects is allowed, but any method that would 
  /// cause a state change will throw an exception.
  /// </para>
  /// <para>
  /// Most of the time, this property returns <see langword="true" /> as long as <see cref="SubTransaction"/> is <see langword="null" />. However,
  /// while 
  /// <see cref="CreateSubTransaction()"/>
  /// is executing, there is a small time frame where this property already returns <see langword="false" /> while <see cref="SubTransaction"/> is 
  /// still <see langword="null" />. In addition, infrastructure code might temporarily set a transaction active for internal operations even though
  /// a <see cref="SubTransaction"/> exists.
  /// </para>
  /// </remarks>
  public bool IsWriteable
  {
    get { return _hierarchyManager.IsWriteable; }
  }

  /// <summary>
  /// Returns whether this <see cref="ClientTransaction"/> has been discarded. A transaction is discarded when its <see cref="Discard"/> or
  /// <see cref="ITransaction.Release"/> methods are called or when it has been used in a discarding scope.
  /// </summary>
  /// <value>True if this transaction has been discarded.</value>
  public bool IsDiscarded
  {
    get { return _isDiscarded; }
  }

  /// <summary>
  /// Gets the collection of <see cref="IClientTransactionExtension"/>s of this <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   Use <see cref="ClientTransactionExtensionCollection.Add"/> and <see cref="ClientTransactionExtensionCollection.Remove"/> 
  ///   to register and unregister an extension.
  /// </para>
  /// <para>
  ///   The order of the extensions in this collection is the order in which they are notified.
  /// </para>
  /// <para>
  /// The collection of extensions is the same for a parent transactions and all of its (direct and indirect) substransactions.
  /// </para>
  /// </remarks>
  public ClientTransactionExtensionCollection Extensions
  {
    get { return _eventBroker.Extensions; }
  }

  /// <summary>
  /// Gets the <see cref="IQueryManager"/> of the <see cref="ClientTransaction"/>.
  /// </summary>
  public IQueryManager QueryManager
  {
    get { return _queryManager; }
  }

  public override string ToString ()
  {
    string rootOrSub = ParentTransaction == null ? "root" : "sub";
    string leafOrParent = SubTransaction == null ? "leaf" : "parent";
    return string.Format("ClientTransaction ({0}, {1}) {2}", rootOrSub, leafOrParent, ID);
  }

  protected internal void AddListener (IClientTransactionListener listener)
  {
    ArgumentUtility.CheckNotNull("listener", listener);
    _eventBroker.AddListener(listener);
  }

  protected void RemoveListener (IClientTransactionListener listener)
  {
    ArgumentUtility.CheckNotNull("listener", listener);
    _eventBroker.RemoveListener(listener);
  }

  /// <summary>
  /// Discards this transaction (rendering it unusable) and, if this transaction is a subtransaction, returns control to the parent transaction.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created via <see cref="CreateSubTransaction()"/>, the parent transaction is made read-only and cannot be
  /// used in potentially modifying operations until the subtransaction returns control to the parent transaction by calling this method.
  /// </para>
  /// <para>
  /// Note that this method only affects writeability of the transactions, it does not influence the active <see cref="ClientTransactionScope"/> and
  /// <see cref="ClientTransaction.Current"/> transaction. However, by default, the scope created by <see cref="EnterDiscardingScope"/> will automatically
  /// execute this method when the scope is left (see <see cref="AutoRollbackBehavior.Discard"/>). In most cases,
  /// <see cref="Discard"/> therefore doesn't have to be called explicity; leaving the scopes suffices.
  /// </para>
  /// <para>
  /// Use <see cref="EnterNonDiscardingScope"/> instead of <see cref="EnterDiscardingScope"/> to avoid this method being called at the end of a scope.
  /// </para>
  /// </remarks>
  public virtual void Discard ()
  {
    if (!_isDiscarded)
    {
      _eventBroker.RaiseTransactionDiscardEvent();
      _hierarchyManager.OnTransactionDiscard();

      _isDiscarded = true;
      AddListener(new InvalidatedTransactionListener());
    }
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction and enters it, 
  /// making this <see cref="ClientTransaction"/> the <see cref="ClientTransaction.Current"/> transaction for the calling thread and the 
  /// <see cref="ActiveTransaction"/> within its transaction hierarchy. 
  /// When the scope is left, this transaction will be discarded. 
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> for this transaction with an automatic <see cref="AutoRollbackBehavior.Discard"/>
  /// behavior.</returns>
  /// <remarks>
  /// <para>
  /// The created scope will not perform any automatic rollback, but it will return control to the parent transaction at its end if this
  /// transaction is a subtransaction.
  /// </para>
  /// <para>
  /// The new <see cref="ClientTransactionScope"/> stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
  /// <see cref="ClientTransactionScope.Leave"/> method is called or the scope is disposed of, the previous scope is reactivated, and the 
  /// <see cref="ClientTransaction.Current"/> property is restored to its previous value.
  /// </para>
  /// <para>
  /// When a <see cref="DomainObject"/> is accessed, it will by default always use the <see cref="ActiveTransaction"/> of the associated 
  /// <see cref="DomainObject.RootTransaction"/>. This method makes this <see cref="ClientTransaction"/> the <see cref="ActiveTransaction"/>,
  /// causing <see cref="DomainObject"/> instances bound to its hierarchy to be accessed in the context of this transaction. When the scope is left, 
  /// the <see cref="ActiveTransaction"/> is reverted to its previous value.
  /// </para>
  /// </remarks>
  public virtual ClientTransactionScope EnterDiscardingScope ()
  {
    return EnterScope(AutoRollbackBehavior.Discard);
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction with the given automatic rollback behavior and enters it, 
  /// making this <see cref="ClientTransaction"/> the <see cref="ClientTransaction.Current"/> transaction for the calling thread and the 
  /// <see cref="ActiveTransaction"/> within its transaction hierarchy. 
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> for this transaction.</returns>
  /// <param name="rollbackBehavior">The automatic rollback behavior to be performed when the scope's <see cref="ClientTransactionScope.Leave"/>
  /// method is called.</param>
  /// <remarks>
  /// <para>
  /// The new <see cref="ClientTransactionScope"/> stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
  /// <see cref="ClientTransactionScope.Leave"/> method is called or the scope is disposed of, the previous scope is reactivated, and the 
  /// <see cref="ClientTransaction.Current"/> property is restored to its previous value.
  /// </para>
  /// <para>
  /// When a <see cref="DomainObject"/> is accessed, it will by default always use the <see cref="ActiveTransaction"/> of the associated 
  /// <see cref="DomainObject.RootTransaction"/>. This method makes this <see cref="ClientTransaction"/> the <see cref="ActiveTransaction"/>,
  /// causing <see cref="DomainObject"/> instances bound to its hierarchy to be accessed in the context of this transaction. When the scope is left, 
  /// the <see cref="ActiveTransaction"/> is reverted to its previous value.
  /// </para>
  /// </remarks>
  public virtual ClientTransactionScope EnterScope (AutoRollbackBehavior rollbackBehavior)
  {
    var activationScope = _hierarchyManager.TransactionHierarchy.ActivateTransaction(this);
    return new ClientTransactionScope(this, rollbackBehavior, activationScope);
  }

  /// <summary>
  /// Creates a new <see cref="ClientTransactionScope"/> for this transaction and enters it, 
  /// making this <see cref="ClientTransaction"/> the <see cref="ClientTransaction.Current"/> transaction for the calling thread and the 
  /// <see cref="ActiveTransaction"/> within its transaction hierarchy. 
  /// When the scope is left, this transaction is not discarded.
  /// </summary>
  /// <returns>A new <see cref="ClientTransactionScope"/> for this transaction with no automatic rollback behavior.</returns>
  /// <remarks>
  /// <para>
  /// The created scope will not perform any automatic rollback and it will not return control to the parent transaction at its end if this
  /// transaction is a subtransaction. You must explicitly call <see cref="Discard"/> if you want to continue working with
  /// the parent transaction. This method is useful if you want to temporarily open a scope for a transaction, then open a scope for another
  /// transaction, then open a new scope for the first transaction again. In this case, the first scope must be a non-discarding scope, otherwise the
  /// transaction will be discarded and cannot be used a second time.
  /// </para>
  /// <para>
  /// The new <see cref="ClientTransactionScope"/> stores the previous <see cref="ClientTransactionScope.ActiveScope"/>. When this scope's
  /// <see cref="ClientTransactionScope.Leave"/> method is called or the scope is disposed of, the previous scope is reactivated, and the 
  /// <see cref="ClientTransaction.Current"/> property is restored to its previous value.
  /// </para>
  /// <para>
  /// When a <see cref="DomainObject"/> is accessed, it will by default always use the <see cref="ActiveTransaction"/> of the associated 
  /// <see cref="DomainObject.RootTransaction"/>. This method makes this <see cref="ClientTransaction"/> the <see cref="ActiveTransaction"/>,
  /// causing <see cref="DomainObject"/> instances bound to its hierarchy to be accessed in the context of this transaction. When the scope is left, 
  /// the <see cref="ActiveTransaction"/> is reverted to its previous value.
  /// </para>
  /// </remarks>
  public virtual ClientTransactionScope EnterNonDiscardingScope ()
  {
    return EnterScope(AutoRollbackBehavior.None);
  }

  /// <summary>
  /// Gets the number of domain objects enlisted in this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The number of domain objects enlisted in this <see cref="ClientTransaction"/>.</value>
  public int EnlistedDomainObjectCount
  {
    get { return _enlistedDomainObjectManager.EnlistedDomainObjectCount; }
  }

  /// <summary>
  /// Gets all domain objects enlisted in this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <value>The domain objects enlisted in this transaction.</value>
  /// <remarks>
  /// The <see cref="DataContainer"/>s of the returned objects might not have been loaded yet. In that case, they will be loaded on first
  /// access of the respective objects' properties, and this might trigger an <see cref="ObjectsNotFoundException"/> if the container cannot be loaded.
  /// </remarks>
  public IEnumerable<DomainObject> GetEnlistedDomainObjects ()
  {
    return _enlistedDomainObjectManager.GetEnlistedDomainObjects();
  }

  /// <summary>
  /// Returns the <see cref="DomainObject"/> registered for the given <paramref name="objectID"/> in this <see cref="ClientTransaction"/>, or 
  /// <see langword="null"/> if no such object exists.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> for which to retrieve a <see cref="DomainObject"/>.</param>
  /// <returns>
  /// A <see cref="DomainObject"/> with the given <paramref name="objectID"/> previously registered in this <see cref="ClientTransaction"/>,
  /// or <see langword="null"/> if no such object exists.
  /// </returns>
  /// <remarks>
  /// The <see cref="DataContainer"/> of the returned object might not have been loaded yet. In that case, it will be loaded on first
  /// access of the object's properties, and this might trigger an <see cref="ObjectsNotFoundException"/> if the container cannot be loaded.
  /// </remarks>
  public DomainObject? GetEnlistedDomainObject (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);
    return _enlistedDomainObjectManager.GetEnlistedDomainObject(objectID);
  }

  /// <summary>
  /// Determines whether the specified <paramref name="domainObject"/> is enlisted in this transaction.
  /// </summary>
  /// <param name="domainObject">The domain object to be checked.</param>
  /// <returns>
  /// <see langword="true" /> if the specified domain object can be used in the context of this transaction; otherwise, <see langword="false" />.
  /// </returns>
  /// <remarks>
  /// Note that comparing the <see cref="RootTransaction"/> to the <see cref="DomainObject.RootTransaction"/> of <paramref name="domainObject"/> is 
  /// faster and usually yields the same result as this method. Only infrastructure code needing to guard against incorrectly setup 
  /// <see cref="DomainObject"/> instances needs to use <see cref="IsEnlisted"/>.
  /// </remarks>
  public bool IsEnlisted (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull("domainObject", domainObject);
    return _enlistedDomainObjectManager.IsEnlisted(domainObject);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the object's data. If the object's data can't be found, an exception is thrown,
  /// the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set, and the object becomes <b>invalid</b>
  /// in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The domain object whose data must be loaded.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ObjectInvalidException">The given <paramref name="objectID"/> is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found object's
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>
  /// results in a <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  public void EnsureDataAvailable (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);

    _dataManager.GetDataContainerWithLazyLoad(objectID, throwOnNotFound: true);
  }

  /// <summary>
  /// Ensures that the data for the <see cref="DomainObject"/>s with the given <see cref="ObjectID"/> values has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the objects' data, performing a bulk load operation.
  /// If an object's data can't be found, an exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
  /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectIDs">The <see cref="ObjectID"/> values whose data must be loaded.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ClientTransactionsDifferException">One of the given <paramref name="objectIDs"/> cannot be used in this 
  /// <see cref="ClientTransaction"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the given <paramref name="objectIDs"/> is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found objects'
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>s
  /// results in a <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  public void EnsureDataAvailable (IEnumerable<ObjectID> objectIDs)
  {
    ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

    DataManager.GetDataContainersWithLazyLoad(objectIDs, throwOnNotFound: true);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="DomainObject"/> with the given <see cref="ObjectID"/> has been loaded into this
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the object's data. The method returns a value indicating whether the
  /// object's data was found. If the object's data can't be found, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
  /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The domain object whose data must be loaded.</param>
  /// <returns><see langword="true" /> if the object's data is now available in the <see cref="ClientTransaction"/>, <see langword="false" /> if the 
  /// data couldn't be found.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="ObjectInvalidException">The given <paramref name="objectID"/> is invalid in this transaction.</exception>
  public bool TryEnsureDataAvailable (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);

    var dataContainer = DataManager.GetDataContainerWithLazyLoad(objectID, throwOnNotFound: false);
    return dataContainer != null;
  }

  /// <summary>
  /// Ensures that the data for the <see cref="DomainObject"/>s with the given <see cref="ObjectID"/> values has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the objects' data, performing a bulk load operation.
  /// The method returns a value indicating whether the data of all the objects was found.
  /// If an object's data can't be found, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
  /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectIDs">The <see cref="ObjectID"/> values whose data must be loaded.</param>
  /// <returns><see langword="true" /> if the data is now available in the <see cref="ClientTransaction"/> for all objects, <see langword="false" /> 
  /// if the data couldn't be found for one or more objects.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null" />.</exception>
  /// <exception cref="ClientTransactionsDifferException">One of the given <paramref name="objectIDs"/> cannot be used in this 
  /// <see cref="ClientTransaction"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the given <paramref name="objectIDs"/> is invalid in this transaction.</exception>
  public bool TryEnsureDataAvailable (IEnumerable<ObjectID> objectIDs)
  {
    ArgumentUtility.CheckNotNull("objectIDs", objectIDs);

    var dataContainers = DataManager.GetDataContainersWithLazyLoad(objectIDs, false);
    return dataContainers.All(dc => dc != null);
  }

  /// <summary>
  /// Creates a new <see cref="ObjectID"/> for the given class definition.
  /// </summary>
  /// <param name="classDefinition">The class definition to create a new <see cref="ObjectID"/> for.</param>
  /// <returns></returns>
  protected internal ObjectID CreateNewObjectID (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull("classDefinition", classDefinition);

    return _persistenceStrategy.CreateNewObjectID(classDefinition);
  }

  /// <summary>
  /// Ensures that the data of the <see cref="IRelationEndPoint"/> with the given <see cref="RelationEndPointID"/> has been loaded into this 
  /// <see cref="ClientTransaction"/>. If it hasn't, this method loads the relation end point's data.
  /// </summary>
  /// <param name="endPointID">The <see cref="RelationEndPointID"/> of the end point whose data must be loaded. In order to force a collection-valued 
  /// relation property to be loaded, pass the <see cref="DomainObjectCollection.AssociatedEndPointID"/>.</param>
  /// <exception cref="ArgumentNullException">The <paramref name="endPointID"/> parameter is <see langword="null" />.</exception>
  public void EnsureDataComplete (RelationEndPointID endPointID)
  {
    var endPoint = DataManager.GetRelationEndPointWithLazyLoad(endPointID);
    endPoint.EnsureDataComplete();

    Assertion.IsTrue(endPoint.IsDataComplete);
  }

  /// <summary>
  /// Initializes a new subtransaction with this <see cref="ClientTransaction"/> as its <see cref="ParentTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created, the parent transaction is automatically made read-only and cannot be modified until the subtransaction
  /// returns control to it via <see cref="Discard"/>. <see cref="Discard"/> is automatically called when a
  /// scope created by <see cref="EnterDiscardingScope"/> is left.
  /// </para>
  /// </remarks>
  public virtual ClientTransaction CreateSubTransaction ()
  {
    return CreateSubTransaction((parentTx, invalidDomainObjectManager, enlistedDomainObjectManager, hierarchyManager, eventSink) =>
    {
      var componentFactory = SubClientTransactionComponentFactory.Create(parentTx, invalidDomainObjectManager, enlistedDomainObjectManager, hierarchyManager, eventSink);
      return ObjectFactory.Create<ClientTransaction>(true, ParamList.Create(componentFactory));
    });
  }

  /// <summary>
  /// Initializes a new subtransaction with this <see cref="ClientTransaction"/> as its <see cref="ParentTransaction"/>. A custom transaction
  /// factory is used to instantiate the subtransaction. This allows subtransactions of types derived from <see cref="ClientTransaction"/>
  /// to be created. The factory must create a subtransaction whose <see cref="ParentTransaction"/> is this <see cref="ClientTransaction"/>, otherwise
  /// this method throws an exception.
  /// </summary>
  /// <param name="subTransactionFactory">A custom implementation of <see cref="IClientTransactionComponentFactory"/> to use when instantiating
  /// the subtransaction.</param>
  /// <remarks>
  /// <para>
  /// When a subtransaction is created, the parent transaction is automatically made read-only and cannot be modified until the subtransaction
  /// returns control to it via <see cref="Discard"/>. <see cref="Discard"/> is automatically called when a
  /// scope created by <see cref="EnterDiscardingScope"/> is left.
  /// </para>
  /// </remarks>
  public virtual ClientTransaction CreateSubTransaction (SubTransactionFactory subTransactionFactory)
  {
    ArgumentUtility.CheckNotNull("subTransactionFactory", subTransactionFactory);

    return _hierarchyManager.CreateSubTransaction(
        tx => subTransactionFactory(tx, _invalidDomainObjectManager, _enlistedDomainObjectManager, _hierarchyManager, _eventBroker));
  }

  /// <summary>
  /// Returns whether at least one <see cref="DomainObject"/> in this <see cref="ClassDefinition"/> matches the supplied <see cref="DomainObjectState"/> predicate.
  /// </summary>
  /// <returns>
  /// <see langword="true"/> if at least one <see cref="DomainObject"/> in this <see cref="ClassDefinition"/> matches the supplied <see cref="DomainObjectState"/> predicate;
  /// otherwise, <see langword="false"/>.
  /// </returns>
  public bool HasObjectsWithState (Predicate<DomainObjectState> predicate)
  {
    ArgumentUtility.CheckNotNull("predicate", predicate);

    return _commitRollbackAgent.HasData(predicate);
  }

  /// <summary>
  /// Commits all changes within the <b>ClientTransaction</b> to the underlying data source.
  /// </summary>
  /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
  /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the data source.</exception>
  /// <remarks>
  /// <para>
  /// Committing a <see cref="ClientTransaction"/> raises a number of events:
  /// <list type="number">
  /// <item><description>
  /// First, a chain of Committing events is raised. Each Committing event can cancel the <see cref="Commit"/> operation by throwing an exception 
  /// (which, after canceling the operation, will be propagated to the caller). Committing event handlers can also modify each 
  /// <see cref="DomainObject"/> being committed, and they can add or remove objects to or from the commit set. For example, if a Committing event
  /// handler modifies a changed object so that the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> flag is set,
  /// that object will be removed from the commit set. Or, if a handler modifies an unchanged object so that it's
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag is set, it will become part of the commit set.
  /// When a set of objects (a, b) is committed, the Committing event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitting"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committing"/> for (a, b)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committing"/> for (a, b)</description></item>
  /// <item><description>a.<see cref="DomainObject.Committing"/>, b.<see cref="DomainObject.Committing"/> (order undefined)</description></item>
  /// </list>
  /// Usually, every event handler in the Committing event chain receives each object in the commit set exactly once. (See 
  /// <see cref="ICommittingEventRegistrar.RegisterForAdditionalCommittingEvents"/> in order to repeat the Committing events for an object.)
  /// If any event handler adds an object c to the commit set (e.g., by changing or creating it), the whole chain is repeated, but only for c.
  /// </description></item>
  /// <item><description>
  /// Then, <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitValidate"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.CommitValidate"/> are raised for the commit set.
  /// The event handlers for those events get the commit set exactly as it is saved to the underlying data store (or parent transaction) and are 
  /// allowed to cancel the operation by throwing an exception, e.g., if a validation rule fails. The event handlers must not modify the 
  /// <see cref="ClientTransaction"/>'s state (including that of any <see cref="DomainObject"/> in the transaction) in any way.
  /// </description></item>
  /// <item><description>
  /// Then, the data is saved to the underlying data store or parent transaction. The data store or parent transaction may cancel the operation
  /// by throwing an exception (e.g., a <see cref="ConcurrencyViolationException"/> or a database-level exception).
  /// </description></item>
  /// <item><description>
  /// Finally, if the <see cref="Commit"/> operation was completed successfully, a chain of Committed events is raised. Committed event handlers must
  /// not throw an exception. When a set of objects (a, b) was committed, the Committed event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>a.<see cref="DomainObject.Committed"/>, b.<see cref="DomainObject.Committed"/> (order undefined)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.Committed"/> for (a, b)</description></item>
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionCommitted"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.Committed"/> for (a, b)
  /// </description></item>
  /// </list>
  /// </description></item>
  /// </list>
  /// </para>
  /// </remarks>
  public virtual void Commit ()
  {
    _commitRollbackAgent.CommitData();
  }

  /// <summary>
  /// Performs a rollback of all changes within the <b>ClientTransaction</b>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Rolling back a <see cref="ClientTransaction"/> raises a number of events:
  /// <list type="number">
  /// <item><description>
  /// First, a chain of RollingBack events is raised. Each RollingBack event can cancel the <see cref="Rollback"/> operation by throwing an exception 
  /// (which, after canceling the operation, will be propagated to the caller). RollingBack event handlers can also modify each 
  /// <see cref="DomainObject"/> being rolled back, and they can add or remove objects to or from the rollback set. For example, if a RollingBack event
  /// handler modifies a changed object so that it's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsUnchanged"/> flag is set,
  /// that object will no longer need to be rolled back. Or, if a handler modifies an unchanged object so that it's
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsChanged"/> flag is set, it will become part of the rollback set.
  /// When a set of objects (a, b) is rolled back, the RollingBack event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionRollingBack"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.RollingBack"/> for (a, b)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.RollingBack"/> for (a, b)</description></item>
  /// <item><description>a.<see cref="DomainObject.RollingBack"/>, b.<see cref="DomainObject.RollingBack"/> (order undefined)</description></item>
  /// </list>
  /// Every event handler in the RollingBack event chain receives each object in the rollback set exactly once.
  /// If any event handler adds an object c to the rollback set (e.g., by changing or creating it), the whole chain is repeated, but only for c.
  /// </description></item>
  /// <item><description>
  /// Then, the data is rolled back.
  /// </description></item>
  /// <item><description>
  /// Finally, if the <see cref="Rollback"/> operation was completed successfully, a chain of RolledBack events is raised. RolledBack event handlers must
  /// not throw an exception. When a set of objects (a, b) was rolled back, the RolledBack event chain consists of the following events, raised in order:
  /// <list type="number">
  /// <item><description>a.<see cref="DomainObject.RolledBack"/>, b.<see cref="DomainObject.RolledBack"/> (order undefined)</description></item>
  /// <item><description><see cref="ClientTransaction"/>.<see cref="ClientTransaction.RolledBack"/> for (a, b)</description></item>
  /// <item><description>
  /// <see cref="IClientTransactionListener"/>.<see cref="IClientTransactionListener.TransactionRolledBack"/> and 
  /// <see cref="IClientTransactionExtension"/>.<see cref="IClientTransactionExtension.RolledBack"/> for (a, b)
  /// </description></item>
  /// </list>
  /// </description></item>
  /// </list>
  /// </para>
  /// </remarks>
  public virtual void Rollback ()
  {
    _commitRollbackAgent.RollbackData();
  }

  /// <summary>
  /// Gets a <see cref="DomainObject"/> that is already loaded or attempts to load it from the data source. If the object's data can't be found, an 
  /// exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set, and the object becomes
  /// <b>invalid</b> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="id">The <see cref="ObjectID"/> of the <see cref="DomainObject"/> that should be loaded. Must not be <see langword="null"/>.</param>
  /// <param name="includeDeleted">Indicates if the method should return <see cref="DomainObject"/>s that are already deleted.</param>
  /// <returns>The <see cref="DomainObject"/> with the specified <paramref name="id"/>.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// The object could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found object's
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>
  /// results in a <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  /// <exception cref="ObjectInvalidException">The object is invalid in this transaction.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="id"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the data source.
  /// </exception>
  /// <exception cref="ObjectDeletedException">The object has already been deleted and the <paramref name="includeDeleted"/> flag is 
  /// <see langword="false" />.</exception>
  protected internal virtual DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    ArgumentUtility.CheckNotNull("id", id);

    return _objectLifetimeAgent.GetObject(id, includeDeleted);
  }

  /// <summary>
  /// Gets an object that is already loaded (even if its <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set)
  /// or attempts to load them from the data source. If an object cannot be found, it's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
  /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The ID of the object to be retrieved.</param>
  /// <returns>
  /// The <see cref="DomainObject"/> with the specified <paramref name="objectID"/>, or <see langword="null" /> if it couldn't be found.
  /// </returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="Persistence.StorageProviderException">
  ///   The Mapping does not contain a class definition for the given <paramref name="objectID"/>.<br /> -or- <br />
  ///   An error occurred while reading a <see cref="PropertyValue"/>.<br /> -or- <br />
  ///   An error occurred while accessing the data source.
  /// </exception>
  protected internal virtual DomainObject? TryGetObject (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);
    return _objectLifetimeAgent.TryGetObject(objectID);
  }

  /// <summary>
  /// Gets a reference to a <see cref="DomainObject"/> with the given <see cref="ObjectID"/> from this <see cref="ClientTransaction"/>. If the
  /// transaction does not currently hold an object with this <see cref="ObjectID"/>, an object reference representing that <see cref="ObjectID"/> 
  /// is created without calling a constructor and without loading the object's data from the data source. This method does not check whether an
  /// object with the given <see cref="ObjectID"/> actually exists in the data source, and it will also return invalid or deleted objects.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> to get an object reference for.</param>
  /// <returns>
  /// An object with the given <see cref="ObjectID"/>, possibly with the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/>,
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsDeleted"/>, or <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
  /// flags set.
  /// </returns>
  /// <remarks>
  /// <para>
  /// When an object with the given <paramref name="objectID"/> has already been enlisted in the transaction, that object is returned. Otherwise,
  /// an object with the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsNotLoadedYet"/> flag set is created and enlisted
  /// without loading its data from the data source. In such a case, the object's data is loaded when it's first needed; e.g., when one of its
  /// properties is accessed or when <see cref="EnsureDataAvailable(Remotion.Data.DomainObjects.ObjectID)"/> is called for its <see cref="ObjectID"/>.
  /// At that point, an <see cref="ObjectsNotFoundException"/> may be triggered when the object's data cannot be found.
  /// </para>
  /// </remarks>
  /// <exception cref="ArgumentNullException">The <paramref name="objectID"/> parameter is <see langword="null" />.</exception>
  protected internal virtual DomainObject GetObjectReference (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);
    return _objectLifetimeAgent.GetObjectReference(objectID);
  }

  /// <summary>
  /// Gets a reference to a <see cref="DomainObject"/> that has <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag set at this time.
  /// If the object is not actually invalid (check with <see cref="ClientTransaction"/>.<see cref="IsInvalid"/>), an exception is throws.
  /// </summary>
  /// <param name="objectID">The object ID to get the <see cref="DomainObject"/> reference for.</param>
  /// <returns>
  /// An object with the given <see cref="ObjectID"/> that has <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag set.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// The object does not have the <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag set at this time.
  /// </exception>
  protected internal virtual DomainObject GetInvalidObjectReference (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);
    return _invalidDomainObjectManager.GetInvalidObjectReference(objectID);
  }

  /// <summary>
  /// Determines whether the specified <see cref="ObjectID"/> has been marked invalid in the scope of this <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="objectID">The <see cref="ObjectID"/> to check.</param>
  /// <returns>
  /// 	<see langword="true"/> if the specified <paramref name="objectID"/> is invalid; otherwise, <see langword="false"/>.
  /// </returns>
  public bool IsInvalid (ObjectID objectID)
  {
    ArgumentUtility.CheckNotNull("objectID", objectID);
    return _invalidDomainObjectManager.IsInvalid(objectID);
  }

  protected internal virtual DomainObject NewObject (Type domainObjectType, ParamList constructorParameters)
  {
    ArgumentUtility.CheckNotNull("domainObjectType", domainObjectType);
    ArgumentUtility.CheckNotNull("constructorParameters", constructorParameters);

    var classDefinition = MappingConfiguration.Current.GetClassDefinition(domainObjectType);
    return _objectLifetimeAgent.NewObject(classDefinition, constructorParameters);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded or attempts to load them from the data source.
  /// If an object's data can't be found, an exception is thrown, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/>
  /// flag is set, and the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
  /// <paramref name="objectIDs"/>. This list might include deleted objects.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the expected type <typeparamref name="T"/>.</exception>
  /// <exception cref="ObjectInvalidException">One of the retrieved objects is invalid in this transaction.</exception>
  /// <exception cref="ObjectsNotFoundException">
  /// One or more objects could not be found in the data source. Note that the <see cref="ClientTransaction"/> sets the not found objects'
  /// <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag, so calling this API again with the same <see cref="ObjectID"/>
  /// results in a <see cref="ObjectInvalidException"/> being thrown.
  /// </exception>
  protected internal T[] GetObjects<T> (IEnumerable<ObjectID> objectIDs)
      where T : DomainObject
  {
    ArgumentUtility.CheckNotNull("objectIDs", objectIDs);
    return _objectLifetimeAgent.GetObjects<T>(objectIDs);
  }

  /// <summary>
  /// Gets a number of objects that are already loaded (including invalid objects) or attempts to load them from the data source. 
  /// If an object cannot be found, the object's <see cref="DomainObject.State"/>.<see cref="DomainObjectState.IsInvalid"/> flag is set,
  /// the object becomes <b>invalid</b> in the <see cref="ClientTransaction"/>, and the result array will contain a <see langword="null" />
  /// reference in its place.
  /// </summary>
  /// <typeparam name="T">The type of objects expected to be returned. Specify <see cref="DomainObject"/> if no specific type is expected.</typeparam>
  /// <param name="objectIDs">The IDs of the objects to be retrieved.</param>
  /// <returns>A list of objects of type <typeparamref name="T"/> corresponding to (and in the same order as) the IDs specified in 
  /// <paramref name="objectIDs"/>. This list can contain invalid and <see langword="null" /> <see cref="DomainObject"/> references.</returns>
  /// <exception cref="ArgumentNullException">The <paramref name="objectIDs"/> parameter is <see langword="null"/>.</exception>
  /// <exception cref="InvalidCastException">One of the retrieved objects doesn't fit the specified type <typeparamref name="T"/>.</exception>
  protected internal T?[] TryGetObjects<T> (IEnumerable<ObjectID> objectIDs)
      where T : DomainObject
  {
    ArgumentUtility.CheckNotNull("objectIDs", objectIDs);
    return _objectLifetimeAgent.TryGetObjects<T>(objectIDs);
  }

  /// <summary>
  /// Gets the related object of a given <see cref="RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the current related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="ObjectEndPoint"/></exception>
  protected internal virtual DomainObject? GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
      throw new ArgumentException("The given end-point ID does not denote a related object (cardinality one).", "relationEndPointID");

    var domainObject = GetOriginatingObjectForRelationAccess(relationEndPointID);

    _eventBroker.RaiseRelationReadingEvent(domainObject, relationEndPointID.Definition, ValueAccess.Current);

    var objectEndPoint = (IObjectEndPoint)DataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
    DomainObject? relatedObject = objectEndPoint.GetOppositeObject();

    _eventBroker.RaiseRelationReadEvent(domainObject, relationEndPointID.Definition, relatedObject, ValueAccess.Current);

    return relatedObject;
  }

  /// <summary>
  /// Gets the original related object of a given <see cref="RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="ObjectEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>The <see cref="DomainObject"/> that is the original related object.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to an <see cref="ObjectEndPoint"/></exception>
  protected internal virtual DomainObject? GetOriginalRelatedObject (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.One)
      throw new ArgumentException("The given end-point ID does not denote a related object (cardinality one).", "relationEndPointID");

    var domainObject = GetOriginatingObjectForRelationAccess(relationEndPointID);

    _eventBroker.RaiseRelationReadingEvent(domainObject, relationEndPointID.Definition, ValueAccess.Original);

    var objectEndPoint = (IObjectEndPoint)_dataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
    DomainObject? relatedObject = objectEndPoint.GetOriginalOppositeObject();

    _eventBroker.RaiseRelationReadEvent(domainObject, relationEndPointID.Definition, relatedObject, ValueAccess.Original);

    return relatedObject;
  }

  /// <summary>
  /// Gets the related objects of a given <see cref="RelationEndPointID"/>.
  /// </summary>
  /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="DomainObjectCollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
  /// <returns>An <see cref="IObjectList{IDomainObject}"/> or a <see cref="DomainObjectCollection"/> containing the current related objects.</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="DomainObjectCollectionEndPoint"/></exception>
  protected internal virtual IReadOnlyList<IDomainObject> GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
      throw new ArgumentException("The given end-point ID does not denote a related object collection (cardinality many).", "relationEndPointID");

    var domainObject = GetOriginatingObjectForRelationAccess(relationEndPointID);

    _eventBroker.RaiseRelationReadingEvent(domainObject, relationEndPointID.Definition, ValueAccess.Current);

    IReadOnlyCollectionData<IDomainObject> readOnlyRelatedObjects;
    IReadOnlyList<IDomainObject> relatedObjects;
    var collectionEndPoint = _dataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
    if (collectionEndPoint is IDomainObjectCollectionEndPoint domainObjectCollectionEndPoint)
    {
      var domainObjectCollection = domainObjectCollectionEndPoint.Collection;
      relatedObjects = (IReadOnlyList<IDomainObject>)domainObjectCollection;
      readOnlyRelatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(domainObjectCollection);
    }
    else
    {
      var virtualCollectionEndPoint = (IVirtualCollectionEndPoint)collectionEndPoint;
      relatedObjects = virtualCollectionEndPoint.Collection;
      readOnlyRelatedObjects = (IReadOnlyCollectionData<IDomainObject>)relatedObjects;
    }

    _eventBroker.RaiseRelationReadEvent(domainObject, relationEndPointID.Definition, readOnlyRelatedObjects, ValueAccess.Current);

    return relatedObjects;
  }

    /// <summary>
    /// Gets the original related objects of a given <see cref="RelationEndPointID"/> at the point of instantiation, loading, commit or rollback.
    /// </summary>
    /// <param name="relationEndPointID">The <see cref="RelationEndPointID"/> to evaluate. It must refer to a <see cref="DomainObjectCollectionEndPoint"/>. Must not be <see langword="null"/>.</param>
    /// <returns>An <see cref="IObjectList{IDomainObject}"/> or a <see cref="DomainObjectCollection"/> containing the original related objects.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="relationEndPointID"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="relationEndPointID"/> does not refer to a <see cref="DomainObjectCollectionEndPoint"/></exception>
    protected internal virtual IReadOnlyList<IDomainObject> GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    ArgumentUtility.CheckNotNull("relationEndPointID", relationEndPointID);

    if (relationEndPointID.Definition.Cardinality != CardinalityType.Many)
      throw new ArgumentException("The given end-point ID does not denote a related object collection (cardinality many).", "relationEndPointID");

    var domainObject = GetOriginatingObjectForRelationAccess(relationEndPointID);

    _eventBroker.RaiseRelationReadingEvent(domainObject, relationEndPointID.Definition, ValueAccess.Original);

    IReadOnlyCollectionData<DomainObject> readOnlyRelatedObjects;
    IReadOnlyList<IDomainObject> relatedObjects;
    var collectionEndPoint = _dataManager.GetRelationEndPointWithLazyLoad(relationEndPointID);
    if (collectionEndPoint is IDomainObjectCollectionEndPoint domainObjectCollectionEndPoint)
    {
      var domainObjectCollection = domainObjectCollectionEndPoint.GetCollectionWithOriginalData();
      relatedObjects = (IReadOnlyList<IDomainObject>)domainObjectCollection;
      readOnlyRelatedObjects = new ReadOnlyDomainObjectCollectionAdapter<DomainObject>(domainObjectCollection);
    }
    else
    {
      var virtualCollectionEndPoint = (IVirtualCollectionEndPoint)collectionEndPoint;
      relatedObjects = virtualCollectionEndPoint.GetCollectionWithOriginalData();
      readOnlyRelatedObjects = (IReadOnlyCollectionData<DomainObject>)relatedObjects;
    }

    _eventBroker.RaiseRelationReadEvent(domainObject, relationEndPointID.Definition, readOnlyRelatedObjects, ValueAccess.Original);

    return relatedObjects;
  }

  /// <summary>
  /// Deletes a <see cref="DomainObject"/>.
  /// </summary>
  /// <param name="domainObject">The <see cref="DomainObject"/> to delete. Must not be <see langword="null"/>.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="domainObject"/> is <see langword="null"/>.</exception>
  /// <exception cref="DataManagement.ClientTransactionsDifferException">
  ///   <paramref name="domainObject"/> belongs to a different <see cref="ClientTransaction"/>. 
  /// </exception>
  protected internal virtual void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull("domainObject", domainObject);
    _objectLifetimeAgent.Delete(domainObject);
  }

  /// <summary>
  /// Raises the <see cref="Loaded"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnLoaded (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull("args", args);

    if (Loaded != null)
      Loaded(this, args);
  }

  /// <summary>
  /// Raises the <see cref="Committing"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnCommitting (ClientTransactionCommittingEventArgs args)
  {
    ArgumentUtility.CheckNotNull("args", args);

    if (Committing != null)
      Committing(this, args);
  }


  /// <summary>
  /// Raises the <see cref="Committed"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnCommitted (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull("args", args);

    if (Committed != null)
      Committed(this, args);
  }

  /// <summary>
  /// Raises the <see cref="RollingBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnRollingBack (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull("args", args);

    if (RollingBack != null)
      RollingBack(this, args);
  }

  /// <summary>
  /// Raises the <see cref="RolledBack"/> event.
  /// </summary>
  /// <param name="args">A <see cref="ClientTransactionEventArgs"/> object that contains the event data.</param>
  protected internal virtual void OnRolledBack (ClientTransactionEventArgs args)
  {
    ArgumentUtility.CheckNotNull("args", args);

    if (RolledBack != null)
      RolledBack(this, args);
  }

  /// <summary>
  /// Raises the <see cref="SubTransactionCreated"/> event.
  /// </summary>
  /// <param name="eventArgs">A <see cref="Remotion.Data.DomainObjects.SubTransactionCreatedEventArgs"/> instance containing the event data.</param>
  protected internal virtual void OnSubTransactionCreated (SubTransactionCreatedEventArgs eventArgs)
  {
    ArgumentUtility.CheckNotNull("eventArgs", eventArgs);

    if (SubTransactionCreated != null)
      SubTransactionCreated(this, eventArgs);
  }

  /// <summary>
  /// Gets the <see cref="IDataManager"/> of this <see cref="ClientTransaction"/>.
  /// </summary>
  protected internal IDataManager DataManager
  {
    get { return _dataManager; }
  }

  /// <summary>
  /// Gets a <see cref="System.Collections.Generic.Dictionary {TKey, TValue}"/> to store application specific objects 
  /// within the <see cref="ClientTransaction"/> hierarchy.
  /// </summary>
  /// <remarks>
  /// <para>
  /// To store and access values create project specific <see cref="System.Enum"/>(s) which ensure namespace separation of keys in the dictionary.
  /// </para>
  /// <para>
  /// Note that the application data collection is not managed in a transactional way. Also, it is the same for a parent transactions and all of
  /// its (direct and indirect) substransactions.
  /// </para>
  /// </remarks>
  public IDictionary<Enum, object> ApplicationData
  {
    get { return _applicationData; }
  }

  public virtual ITransaction ToITransaction ()
  {
    // See  RM-5278 when thinking about removing the ToITransaction method.

    return new ClientTransactionWrapper(this);
  }

  private DomainObject GetOriginatingObjectForRelationAccess (RelationEndPointID relationEndPointID)
  {
    // We always want the originating object to be loaded (even for virtual end-points) because we want that:
    // - the user can rely on property access triggering OnLoaded for initialization (e.g., for registering event handlers),
    // - the user gets an ObjectNotFoundException for the originating object rather than a "mandatory relation not set in database" exception (or a
    //   null result) when the originating object doesn't exist.
    Assertion.IsNotNull(relationEndPointID.ObjectID, "RelationEndPointID must indicate an existing endpoint at this point.");
    DomainObject domainObject = GetObject(relationEndPointID.ObjectID, true);
    return domainObject;
  }
  // ReSharper restore UnusedParameter.Global
}
}
