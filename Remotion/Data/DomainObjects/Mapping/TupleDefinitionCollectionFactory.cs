using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping;

public class TupleDefinitionCollectionFactory
{
  public IMappingObjectFactory MappingObjectFactory { get; }

  public TupleDefinitionCollectionFactory (IMappingObjectFactory mappingObjectFactory)
  {
    MappingObjectFactory = mappingObjectFactory;
  }

  public TupleDefinition[] CreateTupleDefinitionCollection (IEnumerable<Type> types)
  {
    ArgumentUtility.CheckNotNull(nameof(types), types);

    var tupleDefinitions = new Dictionary<Type, TupleDefinition>();
    foreach (var type in types)
      GetTupleDefinition(tupleDefinitions, type);

    return tupleDefinitions.Values.ToArray();
  }

  public TupleDefinition GetTupleDefinition (IDictionary<Type, TupleDefinition> tupleDefinitions, Type tupleType)
  {
    ArgumentUtility.CheckNotNull(nameof(tupleDefinitions), tupleDefinitions);
    ArgumentUtility.CheckNotNull(nameof(tupleType), tupleType);

    if (tupleDefinitions.TryGetValue(tupleType, out var definition))
      return definition;

    var tupleDefinition = MappingObjectFactory.CreateTupleDefinition(tupleType);
    tupleDefinitions.Add(tupleDefinition.TupleType, tupleDefinition);

    return tupleDefinition;
  }
}
