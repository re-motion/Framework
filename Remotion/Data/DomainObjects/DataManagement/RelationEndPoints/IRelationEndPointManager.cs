using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provides an API to manage the <see cref="IRelationEndPoint"/> instances loaded into a <see cref="ClientTransaction"/>.
  /// </summary>
  public interface IRelationEndPointManager : IFlattenedSerializable, IRelationEndPointProvider
  {
    IRelationEndPointMapReadOnlyView RelationEndPoints { get; }

    void RegisterEndPointsForDataContainer (DataContainer dataContainer);
    IDataManagementCommand CreateUnregisterCommandForDataContainer (DataContainer dataContainer);
    IDataManagementCommand CreateUnloadVirtualEndPointsCommand (IEnumerable<RelationEndPointID> endPointIDs);

    void CommitAllEndPoints ();
    void RollbackAllEndPoints ();
    void Reset ();
  }
}