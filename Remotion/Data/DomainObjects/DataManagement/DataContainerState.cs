using System;
using JetBrains.Annotations;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Indicates the state of a <see cref="DataContainer"/>.
  /// </summary>
  public struct DataContainerState
  {
    [Flags]
    private enum Flags
    {
      Unchanged = 1,
      Changed = 1 << 1,
      New = 1 << 2,
      Deleted = 1 << 3,
      Discarded = 1 << 4,
    }

    /// <summary>
    /// Used to construct a new <see cref="DataContainerState"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="Builder"/> is designed as a mutable value type. However, it is recommended to always use the result of the individual <b>Set...()</b> methods
    /// when constructing a new <see cref="DataContainerState"/> to avoid lost-update mistakes.
    /// </remarks>
    public struct Builder
    {
      /// <remarks>
      /// <see cref="_flags"/> is not marked as readonly to support the builder pattern semantics even though normally,
      /// a value type would be implemented as immutable to prevent lost updates.
      /// </remarks>
      private Flags _flags;

      private Builder (Flags flags) => _flags = flags;

      /// <summary>Gets the newly constructed <see cref="DataContainerState"/>.</summary>
      public DataContainerState Value => new DataContainerState(_flags);

      /// <summary>Sets <see cref="DataContainerState"/>.<see cref="DataContainerState.IsUnchanged"/></summary>
      [MustUseReturnValue]
      public Builder SetUnchanged () => SetFlag(Flags.Unchanged);

      /// <summary>Sets <see cref="DataContainerState"/>.<see cref="DataContainerState.IsChanged"/></summary>
      [MustUseReturnValue]
      public Builder SetChanged () => SetFlag(Flags.Changed);

      /// <summary>Sets <see cref="DataContainerState"/>.<see cref="DataContainerState.IsNew"/></summary>
      [MustUseReturnValue]
      public Builder SetNew () => SetFlag(Flags.New);

      /// <summary>Sets <see cref="DataContainerState"/>.<see cref="DataContainerState.IsDeleted"/></summary>
      [MustUseReturnValue]
      public Builder SetDeleted () => SetFlag(Flags.Deleted);

      /// <summary>Sets <see cref="DataContainerState"/>.<see cref="DataContainerState.IsDiscarded"/></summary>
      [MustUseReturnValue]
      public Builder SetDiscarded () => SetFlag(Flags.Discarded);

      private Builder SetFlag (Flags flag)
      {
        _flags |= flag;
        return new Builder(_flags);
      }
    }

    private readonly Flags _flags;

    private DataContainerState (Flags flags) => _flags = flags;

    /// <summary>
    /// The <see cref="DataContainer"/> has not changed since it was loaded.
    /// </summary>
    public bool IsUnchanged => (_flags & Flags.Unchanged) != 0;

    /// <summary>
    /// The <see cref="DataContainer"/> has been changed since it was loaded.
    /// </summary>
    public bool IsChanged => (_flags & Flags.Changed) != 0;

    /// <summary>
    /// The <see cref="DataContainer"/> has been instantiated and has not been committed.
    /// </summary>
    public bool IsNew => (_flags & Flags.New) != 0;

    /// <summary>
    /// The <see cref="DataContainer"/> has been deleted.
    /// </summary>
    public bool IsDeleted => (_flags & Flags.Deleted) != 0;

    /// <summary>
    /// The <see cref="DataContainerState"/> reference is no longer or not yet valid for use in this transaction.
    /// </summary>
    public bool IsDiscarded => (_flags & Flags.Discarded) != 0;

    public override string ToString () => nameof(DataContainerState) + " (" + _flags + ")";
  }
}