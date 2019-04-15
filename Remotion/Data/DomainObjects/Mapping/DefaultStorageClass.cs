using System;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>The storage class options available in the persistence framework.</summary>
  /// <seealso cref="StorageClass"/>
  public enum DefaultStorageClass
  {
    /// <summary>The property is persisted into the data store.</summary>
    Persistent,
    /// <summary>The property is managed by the <see cref="ClientTransaction"/>, but not persisted.</summary>
    Transaction,
  }
}