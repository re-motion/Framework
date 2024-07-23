using System;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Indicates the state of a <see cref="DomainObject"/>.
  /// </summary>
  public struct DomainObjectState
  {
    [Flags]
    private enum Flags
    {
      /// <summary>
      /// May be set only if no other flag is set.
      /// </summary>
      Unchanged = 1 << 0,

      /// <summary>
      /// May be set when <see cref="DataChanged"/> or <see cref="RelationChanged"/> is set.
      /// </summary>
      Changed = 1 << 1,

      /// <summary>
      /// May be set when <see cref="DataChanged"/> or <see cref="RelationChanged"/> is set.
      /// </summary>
      New = 1 << 2,

      /// <summary>
      /// May be set when <see cref="DataChanged"/> or <see cref="RelationChanged"/> is set.
      /// </summary>
      Deleted = 1 << 3,

      /// <summary>
      /// May be set only if no other flag is set.
      /// </summary>
      Invalid = 1 << 4,

      /// <summary>
      /// May be set when <see cref="RelationChanged"/> is set.
      /// </summary>
      NotLoadedYet = 1 << 5,

      /// <summary>
      /// May be set when <see cref="Changed"/>, <see cref="New"/>,  or <see cref="Deleted"/> are set.
      /// Always set when <see cref="PersistentDataChanged"/> or <see cref="NonPersistentDataChanged"/> is set.
      /// Represents a <see cref="DataContainer"/> where at least one <see cref="PropertyValue"/> has <see cref="PropertyValue.Value"/> not equal to
      /// <see cref="PropertyValue.OriginalValue"/> or the <see cref="DataContainer"/>.<see cref="DataContainer.HasBeenMarkedChanged"/> flag has been set.
      /// </summary>
      DataChanged = 1 << 6,

      /// <summary>
      /// May be set when <see cref="Changed"/>, <see cref="New"/>, or <see cref="Deleted"/>, or <see cref="DataChanged"/> are also set.
      /// Represents a <see cref="DataContainer"/> where at least one <see cref="PropertyDefinition"/> with
      /// <see cref="PropertyDefinition.StorageClass"/>.<see cref="StorageClass.Persistent"/> has a <see cref="PropertyValue"/> with <see cref="PropertyValue.Value"/> not equal to
      /// <see cref="PropertyValue.OriginalValue"/> or the <see cref="DataContainer"/>.<see cref="DataContainer.HasBeenMarkedChanged"/> flag has been set for a
      /// <see cref="DomainObject"/> with the <see cref="ClassDefinition"/>.<see cref="ClassDefinitionExtensions.IsNonPersistent"/> flag set to <see langword="false" />.
      /// </summary>
      PersistentDataChanged = 1 << 7,

      /// <summary>
      /// May be set when <see cref="Changed"/>, <see cref="New"/>, or <see cref="Deleted"/>, or <see cref="DataChanged"/> are also set.
      /// Represents a <see cref="DataContainer"/> where at least one <see cref="PropertyDefinition"/> with
      /// <see cref="PropertyDefinition.StorageClass"/>.<see cref="StorageClass.Transaction"/> has a <see cref="PropertyValue"/> with <see cref="PropertyValue.Value"/> not equal to
      /// <see cref="PropertyValue.OriginalValue"/> or the <see cref="DataContainer"/>.<see cref="DataContainer.HasBeenMarkedChanged"/> flag has been set for a
      /// <see cref="DomainObject"/> with the <see cref="ClassDefinition"/>.<see cref="ClassDefinitionExtensions.IsNonPersistent"/> flag set to <see langword="true" />.
      /// </summary>
      NonPersistentDataChanged = 1 << 8,

      /// <summary>
      /// May be set when <see cref="Changed"/>, <see cref="New"/>, <see cref="Deleted"/>, or <see cref="NotLoadedYet"/> are set.
      /// </summary>
      RelationChanged = 1 << 9,

      /// <summary>
      /// May be set when <see cref="New"/>, <see cref="Deleted"/>, <see cref="Unchanged"/>, <see cref="Changed"/>,
      /// <see cref="DataChanged"/>, <see cref="PersistentDataChanged"/>, <see cref="NonPersistentDataChanged"/>, or <see cref="RelationChanged"/> is set.
      /// </summary>
      NewInHierarchy = 1 << 10,
    }

    /// <summary>
    /// Used to construct a new <see cref="DomainObjectState"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="Builder"/> is designed as a mutable value type. However, it is recommended to always use the result of the individual <b>Set...()</b> methods
    /// when constructing a new <see cref="DomainObjectState"/> to avoid lost-update mistakes.
    /// </remarks>
    public struct Builder
    {
      /// <remarks>
      /// <see cref="_flags"/> is not marked as readonly to support the builder pattern semantics even though normally,
      /// a value type would be implemented as immutable to prevent lost updates.
      /// </remarks>
      private Flags _flags;

      private Builder (Flags flags) => _flags = flags;

      /// <summary>Gets the newly constructed <see cref="DomainObjectState"/>.</summary>
      public DomainObjectState Value => new DomainObjectState(_flags);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsUnchanged"/>.</summary>
      [MustUseReturnValue]
      public Builder SetUnchanged () => SetFlag(Flags.Unchanged);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsChanged"/>.</summary>
      [MustUseReturnValue]
      public Builder SetChanged () => SetFlag(Flags.Changed);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsNotLoadedYet"/>.</summary>
      [MustUseReturnValue]
      public Builder SetNotLoadedYet () => SetFlag(Flags.NotLoadedYet);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsNew"/>.</summary>
      [MustUseReturnValue]
      public Builder SetNew () => SetFlag(Flags.New);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsNewInHierarchy"/>.</summary>
      [MustUseReturnValue]
      public Builder SetNewInHierarchy () => SetFlag(Flags.NewInHierarchy);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsDeleted"/>.</summary>
      [MustUseReturnValue]
      public Builder SetDeleted () => SetFlag(Flags.Deleted);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsInvalid"/>.</summary>
      [MustUseReturnValue]
      public Builder SetInvalid () => SetFlag(Flags.Invalid);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsDataChanged"/>.</summary>
      [MustUseReturnValue]
      public Builder SetDataChanged () => SetFlag(Flags.DataChanged);

      /// <summary>
      /// Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsPersistentDataChanged"/>
      /// and <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsDataChanged"/>.
      /// </summary>
      [MustUseReturnValue]
      public Builder SetPersistentDataChanged () => SetFlag(Flags.PersistentDataChanged | Flags.DataChanged);

      /// <summary>
      /// Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsNonPersistentDataChanged"/>
      /// and <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsDataChanged"/>.
      /// </summary>
      [MustUseReturnValue]
      public Builder SetNonPersistentDataChanged () => SetFlag(Flags.NonPersistentDataChanged | Flags.DataChanged);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsRelationChanged"/>.</summary>
      [MustUseReturnValue]
      public Builder SetRelationChanged () => SetFlag(Flags.RelationChanged);

      private Builder SetFlag (Flags flag)
      {
        _flags |= flag;
        return new Builder(_flags);
      }
    }

    private readonly Flags _flags;

    private DomainObjectState (Flags flags) => _flags = flags;

    /// <summary>
    /// The <see cref="DomainObject"/> has not changed since it was loaded.
    /// </summary>
    public bool IsUnchanged => (_flags & Flags.Unchanged) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> has been changed since it was loaded.
    /// </summary>
    /// <remarks>
    /// When this flag is set, the following other flags may also be set: <see cref="IsDataChanged"/>, <see cref="IsPersistentDataChanged"/>,
    /// <see cref="IsNonPersistentDataChanged"/>, and <see cref="IsRelationChanged"/>.
    /// </remarks>
    public bool IsChanged => (_flags & Flags.Changed) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> has been instantiated and has not been committed.
    /// </summary>
    /// <remarks>
    /// When this flag is set, the following other flags may also be set: <see cref="IsDataChanged"/>, <see cref="IsPersistentDataChanged"/>,
    /// <see cref="IsNonPersistentDataChanged"/>, and <see cref="IsRelationChanged"/>.
    /// </remarks>
    public bool IsNew => (_flags & Flags.New) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> has been instantiated in this <see cref="ClientTransaction"/> or one of its <see cref="ClientTransaction.ParentTransaction"/>s
    /// and has not been committed in the <see cref="ClientTransaction.RootTransaction"/>.
    /// </summary>
    /// <remarks>
    /// When this flag is set, the following other flags may also be set: <see cref="IsNew"/>, <see cref="IsDeleted"/>, <see cref="IsUnchanged"/>, <see cref="IsChanged"/>,
    /// <see cref="IsDataChanged"/>, <see cref="IsPersistentDataChanged"/>, <see cref="IsNonPersistentDataChanged"/>, and <see cref="IsRelationChanged"/>.
    /// </remarks>
    public bool IsNewInHierarchy => (_flags & Flags.NewInHierarchy) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> has been deleted.
    /// </summary>
    /// <remarks>
    /// When this flag is set, the following other flags may also be set: <see cref="IsDataChanged"/>, <see cref="IsPersistentDataChanged"/>,
    /// <see cref="IsNonPersistentDataChanged"/>, and <see cref="IsRelationChanged"/>.
    /// </remarks>
    public bool IsDeleted => (_flags & Flags.Deleted) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> reference is no longer or not yet valid for use in this transaction.
    /// </summary>
    /// <remarks>
    /// For more information why and when an object becomes invalid see <see cref="ObjectInvalidException"/>.
    /// </remarks>
    public bool IsInvalid => (_flags & Flags.Invalid) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/>'s data has not been loaded yet into the <see cref="ClientTransaction"/>. It will be loaded when needed,
    /// e.g. when a property value or relation is accessed, or when 
    /// <see cref="DomainObjectExtensions.EnsureDataAvailable"/> is called for the <see cref="IDomainObject"/>.
    /// </summary>
    /// <remarks>
    /// When this flag is set, the <see cref="IsRelationChanged"/> flag may also be set.
    /// </remarks>
    public bool IsNotLoadedYet => (_flags & Flags.NotLoadedYet) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/>'s data (persistent or non-persistent) has been changed.
    /// </summary>
    /// <remarks>
    /// A <see cref="DomainObject"/>'s data is classified as changed if its <see cref="DataContainer"/> takes part in the commit phase,
    /// i.e. at least one of the property values of the <see cref="DataContainer"/> has been changed
    /// or if the <see cref="DataContainer"/> has been explicitly marked as changed.
    /// </remarks>
    public bool IsDataChanged => (_flags & Flags.DataChanged) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/>'s persistent data has been changed.
    /// </summary>
    /// <remarks>
    /// A <see cref="DomainObject"/>'s persistent data is classified as changed if at least one of the property values with a <see cref="PropertyDefinition.StorageClass"/> of
    /// <see cref="StorageClass.Persistent"/> has been changed or if the <see cref="DataContainer"/> has been explicitly marked as changed for a <see cref="DomainObject"/>
    /// belonging to a <see cref="StorageProviderDefinition"/> other than <see cref="NonPersistentProviderDefinition"/>.
    /// <para>
    /// When the <see cref="IsPersistentDataChanged"/> flag is set, the <see cref="IsDataChanged"/> flag will also always be set.
    /// </para>
    /// </remarks>
    public bool IsPersistentDataChanged => (_flags & Flags.PersistentDataChanged) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/>'s non-persistent data has been changed.
    /// </summary>
    /// <remarks>
    /// A <see cref="DomainObject"/>'s non-persistent data is classified as changed if at least one of the property values with a <see cref="PropertyDefinition.StorageClass"/> of
    /// <see cref="StorageClass.Persistent"/> has been changed or if the <see cref="DataContainer"/> has been explicitly marked as changed for a <see cref="DomainObject"/>
    /// belonging to a <see cref="StorageProviderDefinition"/> other than <see cref="NonPersistentProviderDefinition"/>.
    /// <para>
    /// When the <see cref="IsNonPersistentDataChanged"/> flag is set, the <see cref="IsDataChanged"/> flag will also always be set.
    /// </para>
    /// </remarks>
    public bool IsNonPersistentDataChanged => (_flags & Flags.NonPersistentDataChanged) != 0;

    /// <summary>
    /// At least one of the <see cref="DomainObject"/>'s relations has been changed.
    /// </summary>
    /// <remarks>
    /// A <see cref="DomainObject"/>'s relation is classified as changed if its <see cref="IRelationEndPoint"/> for this <see cref="DomainObject"/> has been changed,
    /// i.e. if an item has been added or removed from the relation or, for relations based on <see cref="DomainObjectCollection"/>, the order of the items in the relations
    /// has been changed.
    /// </remarks>
    public bool IsRelationChanged => (_flags & Flags.RelationChanged) != 0;

    public override string ToString () => nameof(DomainObjectState) + " (" + _flags + ")";
  }
}
