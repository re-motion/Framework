using System;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents an object loaded via an implementation of <see cref="IPersistenceStrategy"/>.
  /// </summary>
  public interface ILoadedObjectData : INullObject
  {
    ObjectID ObjectID { get; }
    DomainObject GetDomainObjectReference ();

    void Accept (ILoadedObjectVisitor visitor);
  }
}