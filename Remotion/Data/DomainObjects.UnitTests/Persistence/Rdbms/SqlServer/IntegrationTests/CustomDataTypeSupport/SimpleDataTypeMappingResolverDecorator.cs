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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Linq.SqlBackend.SqlStatementModel.Unresolved;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  public class SimpleDataTypeMappingResolverDecorator : IMappingResolver
  {
    private readonly IMappingResolver _innerMappingResolver;
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public SimpleDataTypeMappingResolverDecorator (IMappingResolver innerMappingResolver, IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      _innerMappingResolver = innerMappingResolver;
      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public IResolvedTableInfo ResolveTableInfo (UnresolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      return _innerMappingResolver.ResolveTableInfo(tableInfo, generator);
    }

    public ResolvedJoinInfo ResolveJoinInfo (UnresolvedJoinInfo joinInfo, UniqueIdentifierGenerator generator)
    {
      return _innerMappingResolver.ResolveJoinInfo(joinInfo, generator);
    }

    public SqlEntityDefinitionExpression ResolveSimpleTableInfo (IResolvedTableInfo tableInfo, UniqueIdentifierGenerator generator)
    {
      return _innerMappingResolver.ResolveSimpleTableInfo(tableInfo, generator);
    }

    public Expression ResolveMemberExpression (SqlEntityExpression originatingEntity, MemberInfo memberInfo)
    {
      var propertyInfo = memberInfo as PropertyInfo;
      if (propertyInfo != null && propertyInfo.PropertyType == typeof (SimpleDataType))
      {
        var classDefinition = MappingConfiguration.Current.GetTypeDefinition(propertyInfo.DeclaringType);
        var propertyName = MappingConfiguration.Current.NameResolver.GetPropertyName(PropertyInfoAdapter.Create(propertyInfo));
        var propertyDefinition = classDefinition.GetPropertyDefinition(propertyName);
        var storagePropertyDefinition = _rdbmsPersistenceModelProvider.GetStoragePropertyDefinition(propertyDefinition);
        var stringValueColumn = storagePropertyDefinition.GetColumns().Single();
        var stringValueColumnExpression = originatingEntity.GetColumn(
            stringValueColumn.StorageTypeInfo.DotNetType,
            stringValueColumn.Name,
            stringValueColumn.IsPartOfPrimaryKey);

        var simpleDataTypeCtor = typeof (SimpleDataType).GetConstructor(new[] { typeof (string) });
        Assertion.IsNotNull(simpleDataTypeCtor);
        var stringValueMember = typeof (SimpleDataType).GetProperty("StringValue");
        var simpleDataTypeConstruction = Expression.New(
            simpleDataTypeCtor,
            new[] { stringValueColumnExpression },
            new[] { stringValueMember });

        return NamedExpression.CreateNewExpressionWithNamedArguments(simpleDataTypeConstruction);
      }
      return _innerMappingResolver.ResolveMemberExpression(originatingEntity, memberInfo);
    }

    public Expression ResolveMemberExpression (SqlColumnExpression sqlColumnExpression, MemberInfo memberInfo)
    {
      return _innerMappingResolver.ResolveMemberExpression(sqlColumnExpression, memberInfo);
    }

    public Expression ResolveConstantExpression (ConstantExpression constantExpression)
    {
      return _innerMappingResolver.ResolveConstantExpression(constantExpression);
    }

    public Expression ResolveTypeCheck (Expression expression, Type desiredType)
    {
      return _innerMappingResolver.ResolveTypeCheck(expression, desiredType);
    }

    public Expression TryResolveOptimizedIdentity (SqlEntityRefMemberExpression entityRefMemberExpression)
    {
      return _innerMappingResolver.TryResolveOptimizedIdentity(entityRefMemberExpression);
    }

    public Expression TryResolveOptimizedMemberExpression (SqlEntityRefMemberExpression entityRefMemberExpression, MemberInfo memberInfo)
    {
      return _innerMappingResolver.TryResolveOptimizedMemberExpression(entityRefMemberExpression, memberInfo);
    }
  }
}