using System;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Provides an interface to clients requiring to raise <see cref="IObjectList{T}"/> events from outside of the collection.
  /// </summary>
  public interface IVirtualCollectionEventRaiser
  {
    //TOOD RM-7294: Check which APIs are actually needed and drop the rest

    void BeginAdd (int index, DomainObject domainObject);
    void EndAdd (int index, DomainObject domainObject);

    void BeginRemove (int index, DomainObject domainObject);
    void EndRemove (int index, DomainObject domainObject);

    void BeginDelete ();
    void EndDelete ();

    void WithinReplaceData ();
  }
}