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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Linq;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.Utilities;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Creates the components required to initialize the LINQ provider.
  /// </summary>
  [ImplementationFor(typeof(ILinqProviderComponentFactory))]
  public class LinqProviderComponentFactory : ILinqProviderComponentFactory
  {
    private readonly IMethodCallTransformerProvider _methodCallTransformerProvider;
    private readonly ResultOperatorHandlerRegistry _resultOperatorHandlerRegistry;

    public LinqProviderComponentFactory ()
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      _resultOperatorHandlerRegistry = CreateResultOperatorHandlerRegistry();
      _methodCallTransformerProvider = CreateMethodCallTransformerProvider();
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    public virtual IQueryable<T> CreateQueryable<T> (IQueryParser queryParser, IQueryExecutor executor)
    {
      ArgumentUtility.CheckNotNull("queryParser", queryParser);
      ArgumentUtility.CheckNotNull("executor", executor);

      return new DomainObjectQueryable<T>(queryParser, executor);
    }

    public virtual IQueryParser CreateQueryParser ()
    {
      var customNodeTypeRegistry = CreateCustomNodeTypeProvider();
      var expressionTreeParser = CreateExpressionTreeParser(customNodeTypeRegistry);
      return CreateQueryParser(expressionTreeParser);
    }

    public virtual IQueryExecutor CreateQueryExecutor (StorageProviderDefinition providerDefinition, string id, IReadOnlyDictionary<string, object> metadata)
    {
      ArgumentUtility.CheckNotNull("providerDefinition", providerDefinition);
      ArgumentUtility.CheckNotNullOrEmpty("id", id);
      ArgumentUtility.CheckNotNull("metadata", metadata);

      var queryGenerator = providerDefinition.Factory.CreateDomainObjectQueryGenerator(
          providerDefinition,
          _methodCallTransformerProvider,
          _resultOperatorHandlerRegistry,
          MappingConfiguration.Current);
      return new DomainObjectQueryExecutor(providerDefinition, queryGenerator, id, metadata);
    }

    protected virtual IMethodCallTransformerProvider CreateMethodCallTransformerProvider ()
    {
      var methodInfoBasedRegistry = RegistryBase<MethodInfoBasedMethodCallTransformerRegistry, MethodInfo, IMethodCallTransformer>.CreateDefault();
      var nameBasedRegistry = RegistryBase<NameBasedMethodCallTransformerRegistry, string, IMethodCallTransformer>.CreateDefault();

      return new CompoundMethodCallTransformerProvider(methodInfoBasedRegistry, nameBasedRegistry);
    }

    protected virtual ResultOperatorHandlerRegistry CreateResultOperatorHandlerRegistry ()
    {
      var resultOperatorHandlerRegistry = RegistryBase<ResultOperatorHandlerRegistry, Type, IResultOperatorHandler>.CreateDefault();

      var handler = new FetchResultOperatorHandler();
      resultOperatorHandlerRegistry.Register(handler.SupportedResultOperatorType, handler);

      return resultOperatorHandlerRegistry;
    }

    protected virtual MethodInfoBasedNodeTypeRegistry CreateCustomNodeTypeProvider ()
    {
      var customNodeTypeRegistry = new MethodInfoBasedNodeTypeRegistry();

      customNodeTypeRegistry.Register(
          new[] { MemberInfoFromExpressionUtility.GetMethod((DomainObjectCollection obj) => obj.ContainsObject(null!)) },
          typeof(ContainsExpressionNode));
      customNodeTypeRegistry.Register(
          new[] { MemberInfoFromExpressionUtility.GetProperty((DomainObjectCollection obj) => obj.Count).GetGetMethod() },
          typeof(CountExpressionNode));
      customNodeTypeRegistry.Register(
          new[] { typeof(IObjectList<>).GetRuntimeMethod("get_Count", new Type[0]) },
          typeof(CountExpressionNode));

      customNodeTypeRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("FetchOne") }, typeof(FetchOneExpressionNode));
      customNodeTypeRegistry.Register(new[] { typeof(EagerFetchingExtensionMethods).GetMethod("FetchMany") }, typeof(FetchManyExpressionNode));
      customNodeTypeRegistry.Register(
          new[] { typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetchOne") }, typeof(ThenFetchOneExpressionNode));
      customNodeTypeRegistry.Register(
          new[] { typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetchMany") }, typeof(ThenFetchManyExpressionNode));

      return customNodeTypeRegistry;
    }

    protected virtual ExpressionTreeParser CreateExpressionTreeParser (INodeTypeProvider customNodeTypeProvider)
    {
      ArgumentUtility.CheckNotNull("customNodeTypeProvider", customNodeTypeProvider);

      var nodeTypeProvider = ExpressionTreeParser.CreateDefaultNodeTypeProvider();
      nodeTypeProvider.InnerProviders.Insert(0, customNodeTypeProvider);

      var transformerRegistry = ExpressionTransformerRegistry.CreateDefault();
      var processor = ExpressionTreeParser.CreateDefaultProcessor(transformerRegistry);
      return new ExpressionTreeParser(nodeTypeProvider, processor);
    }

    protected virtual IQueryParser CreateQueryParser (ExpressionTreeParser expressionTreeParser)
    {
      ArgumentUtility.CheckNotNull("expressionTreeParser", expressionTreeParser);
      return new QueryParser(expressionTreeParser);
    }
  }
}
