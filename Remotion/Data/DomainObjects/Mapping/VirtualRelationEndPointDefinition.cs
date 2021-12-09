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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Obsolete(
      "Use DomainObjectCollectionRelationEndPointDefinition or VirtualCollectionRelationEndPointDefinition or VirtualObjectRelationEndPointDefinition respectively. (Version: 3.0.0)",
      true)]
  public class VirtualRelationEndPointDefinition : IRelationEndPointDefinition
  {
    public VirtualRelationEndPointDefinition (
        TypeDefinition typeDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string sortExpressionText,
        IPropertyInformation propertyInfo)
    {
      throw new NotImplementedException(
          "Use DomainObjectCollectionRelationEndPointDefinition or VirtualCollectionRelationEndPointDefinition or VirtualObjectRelationEndPointDefinition respectively. (Version: 3.0.0)");
    }

    public RelationDefinition RelationDefinition => null!;

    public TypeDefinition TypeDefinition => null!;

    public bool IsMandatory => false;

    public CardinalityType Cardinality => CardinalityType.Many;

    public string? PropertyName => null;

    public IPropertyInformation? PropertyInfo => null;

    public bool IsVirtual => true;

    public bool IsAnonymous => false;

    public string? SortExpressionText => null;

    public SortExpressionDefinition? GetSortExpression () => null;
  }
}
