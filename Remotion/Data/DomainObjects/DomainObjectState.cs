using System;
using JetBrains.Annotations;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Indicates the state of a <see cref="DomainObject"/>.
  /// </summary>
  [Serializable]
  public struct DomainObjectState
  {
    [Flags]
    private enum Flags
    {
      Unchanged = 1,
      Changed = 1 << 1,
      New = 1 << 2,
      Deleted = 1 << 3,
      Invalid = 1 << 4,
      NotLoadedYet = 1 << 5,
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
      public DomainObjectState Value => new DomainObjectState (_flags);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsUnchanged"/></summary>
      [MustUseReturnValue]
      public Builder SetUnchanged () => SetFlag (Flags.Unchanged);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsChanged"/></summary>
      [MustUseReturnValue]
      public Builder SetChanged () => SetFlag (Flags.Changed);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsNotLoadedYet"/></summary>
      [MustUseReturnValue]
      public Builder SetNotLoadedYet () => SetFlag (Flags.NotLoadedYet);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsNew"/></summary>
      [MustUseReturnValue]
      public Builder SetNew () => SetFlag (Flags.New);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsDeleted"/></summary>
      [MustUseReturnValue]
      public Builder SetDeleted () => SetFlag (Flags.Deleted);

      /// <summary>Sets <see cref="DomainObjectState"/>.<see cref="DomainObjectState.IsInvalid"/></summary>
      [MustUseReturnValue]
      public Builder SetInvalid () => SetFlag (Flags.Invalid);

      private Builder SetFlag (Flags flag)
      {
        _flags |= flag;
        return new Builder (_flags);
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
    public bool IsChanged => (_flags & Flags.Changed) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> has been instantiated and has not been committed.
    /// </summary>
    public bool IsNew => (_flags & Flags.New) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> has been deleted.
    /// </summary>
    public bool IsDeleted => (_flags & Flags.Deleted) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/> reference is no longer or not yet valid for use in this transaction.
    /// </summary>
    public bool IsInvalid => (_flags & Flags.Invalid) != 0;

    /// <summary>
    /// The <see cref="DomainObject"/>'s data has not been loaded yet into the <see cref="ClientTransaction"/>. It will be loaded when needed,
    /// e.g. when a property value or relation is accessed, or when 
    /// <see cref="DomainObjectExtensions.EnsureDataAvailable"/> is called for the <see cref="IDomainObject"/>.
    /// </summary>
    public bool IsNotLoadedYet => (_flags & Flags.NotLoadedYet) != 0;

    public override string ToString () => nameof (DomainObjectState) + " (" + _flags + ")";
  }
}