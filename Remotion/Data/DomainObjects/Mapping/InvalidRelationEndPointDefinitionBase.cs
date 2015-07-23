using System;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Holds information about a relation end point that could not be resolved.
  /// </summary>
  public class InvalidRelationEndPointDefinitionBase : IRelationEndPointDefinition
  {
    private readonly ClassDefinition _classDefinition;
    private readonly string _propertyName;
    private readonly Type _propertyType;
    private RelationDefinition _relationDefinition;
    private readonly IPropertyInformation _propertyInformation;

    public InvalidRelationEndPointDefinitionBase (ClassDefinition classDefinition, string propertyName, Type propertyType)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      _classDefinition = classDefinition;
      _propertyName = propertyName;
      _propertyType = propertyType;
      _propertyInformation = new InvalidPropertyInformation (TypeAdapter.Create (_classDefinition.ClassType), propertyName, propertyType);
    }

    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    public string PropertyName
    {
      get { return _propertyName;  }
    }

    public RelationDefinition RelationDefinition
    {
      get { return _relationDefinition; }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInformation; }
    }

    public bool IsMandatory
    {
      get { throw new NotImplementedException(); }
    }

    public CardinalityType Cardinality
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsVirtual
    {
      get { return false;  }
    }

    public bool IsAnonymous
    {
      get { return false;  }
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull ("relationDefinition", relationDefinition);

      _relationDefinition = relationDefinition;
    }
  }
}