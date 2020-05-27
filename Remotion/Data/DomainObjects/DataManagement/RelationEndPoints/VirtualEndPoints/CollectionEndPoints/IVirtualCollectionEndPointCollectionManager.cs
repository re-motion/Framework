using System;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Defines an interface for classes providing and keeping track of the <see cref="IObjectList"/> instances used by 
  /// a <see cref="VirtualCollectionEndPoint"/>.
  /// </summary>
  public interface IVirtualCollectionEndPointCollectionManager
  {
    IObjectList GetOriginalCollectionReference ();
    IObjectList GetCurrentCollectionReference ();

    IVirtualCollectionData AssociateCollectionWithEndPoint (IObjectList newCollection);
    bool HasCollectionReferenceChanged ();
    void CommitCollectionReference ();
    void RollbackCollectionReference ();
  }
}