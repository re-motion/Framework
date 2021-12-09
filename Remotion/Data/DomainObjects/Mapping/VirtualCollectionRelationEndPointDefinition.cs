// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Represents the many-side of a bidirectional one-to-many relationship based on <see cref="IObjectList{TDomainObject}"/>.
  /// </summary>
  [DebuggerDisplay("{GetType().Name}: {PropertyName}, Cardinality: {Cardinality}")]
  public class VirtualCollectionRelationEndPointDefinition : IRelationEndPointDefinition, IRelationEndPointDefinitionSetter
  {
    private readonly string _propertyName;
    private RelationDefinition? _relationDefinition;
    private readonly TypeDefinition _typeDefinition;
    private readonly bool _isMandatory;
    private readonly Lazy<SortExpressionDefinition?> _sortExpression;
    private readonly IPropertyInformation _propertyInfo;

    public VirtualCollectionRelationEndPointDefinition (
        [NotNull] TypeDefinition typeDefinition,
        string propertyName,
        bool isMandatory,
        [NotNull] Lazy<SortExpressionDefinition?> sortExpression,
        [NotNull] IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull("typeDefinition", typeDefinition);
      ArgumentUtility.CheckNotNullOrEmpty("propertyName", propertyName);
      ArgumentUtility.CheckNotNull("sortExpression", sortExpression);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);

      _typeDefinition = typeDefinition;
      _isMandatory = isMandatory;
      _propertyName = propertyName;
      _sortExpression = sortExpression;
      _propertyInfo = propertyInfo;
    }

    public void SetRelationDefinition (RelationDefinition relationDefinition)
    {
      ArgumentUtility.CheckNotNull("relationDefinition", relationDefinition);
      _relationDefinition = relationDefinition;
    }

    public bool HasRelationDefinitionBeenSet
    {
      get { return _relationDefinition != null; }
    }

    public RelationDefinition RelationDefinition
    {
      get
      {
        Assertion.IsNotNull(_relationDefinition, "RelationDefinition has not been set for this relation end point.");
        return _relationDefinition;
      }
    }

    public TypeDefinition TypeDefinition
    {
      get { return _typeDefinition; }
    }

    public bool IsMandatory
    {
      get { return _isMandatory; }
    }

    public CardinalityType Cardinality
    {
      get { return CardinalityType.Many; }
    }

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public bool IsVirtual
    {
      get { return true; }
    }

    public bool IsAnonymous
    {
      get { return false; }
    }

    [CanBeNull]
    public SortExpressionDefinition? GetSortExpression ()
    {
      return _sortExpression.Value;
    }
  }
}
