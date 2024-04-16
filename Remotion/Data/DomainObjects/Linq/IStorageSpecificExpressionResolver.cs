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
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// <see cref="IStorageSpecificExpressionResolver"/> defines the API for classes that evaluate the re-store persistence model when resolving
  /// re-linq expressions.
  /// </summary>
  public interface IStorageSpecificExpressionResolver
  {
    SqlEntityDefinitionExpression ResolveEntity (TypeDefinition typeDefinition, string tableAlias);
    Expression ResolveProperty (SqlEntityExpression originatingEntity, PropertyDefinition propertyDefinition);
    Expression ResolveIDProperty (SqlEntityExpression originatingEntity, TypeDefinition typeDefinition);

    IResolvedTableInfo ResolveTable (TypeDefinition typeDefinition, string tableAlias);

    ResolvedJoinInfo ResolveJoin (
        SqlEntityExpression originatingEntity, IRelationEndPointDefinition leftEndPoint, IRelationEndPointDefinition rightEndPoint, string tableAlias);

    Expression ResolveEntityIdentityViaForeignKey (SqlEntityExpression originatingEntity, RelationEndPointDefinition foreignKeyEndPoint);
    Expression? ResolveIDPropertyViaForeignKey (SqlEntityExpression originatingEntity, RelationEndPointDefinition foreignKeyEndPoint);
  }
}
