using System;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Infrastructure.InvalidObjects
{
  /// <summary>
  /// Defines an API for classes keeping a collection of <see cref="DomainObject"/> references that were marked as invalid in a given 
  /// <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IInvalidDomainObjectManager
  {
    IEnumerable<ObjectID> InvalidObjectIDs { get; }
    
    bool IsInvalid (ObjectID id);
    DomainObject GetInvalidObjectReference (ObjectID id);

    bool MarkInvalid (DomainObject domainObject);
    bool MarkNotInvalid (ObjectID objectID);
  }
}