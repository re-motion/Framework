using System;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides a method to dispatch on <see cref="ILoadedObjectData"/> implementations.
  /// </summary>
  public interface ILoadedObjectVisitor
  {
    void VisitFreshlyLoadedObject (FreshlyLoadedObjectData freshlyLoadedObjectData);
    void VisitAlreadyExistingLoadedObject (AlreadyExistingLoadedObjectData alreadyExistingLoadedObjectData);
    void VisitNullLoadedObject (NullLoadedObjectData nullLoadedObjectData);
    void VisitInvalidLoadedObject (InvalidLoadedObjectData invalidLoadedObjectData);
    void VisitNotFoundLoadedObject (NotFoundLoadedObjectData notFoundLoadedObjectData);
  }
}