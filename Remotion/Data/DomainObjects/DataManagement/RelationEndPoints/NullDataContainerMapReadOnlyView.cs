using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Null-object implementation for <see cref="IDataContainerMapReadOnlyView"/>.
  /// </summary>
  public class NullDataContainerMapReadOnlyView : IDataContainerMapReadOnlyView
  {
    public NullDataContainerMapReadOnlyView ()
    {
    }

    public int Count => 0;

    public DataContainer this [ObjectID id] => null;

    public IEnumerator<DataContainer> GetEnumerator ()
    {
      return Enumerable.Empty<DataContainer>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }


    #region Serialization

    private NullDataContainerMapReadOnlyView (FlattenedDeserializationInfo info)
    {
    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    #endregion
  }
}
