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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.EagerFetching;
using Remotion.Linq.EagerFetching.Parsing;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.SqlPreparation.MethodCallTransformers;
using Remotion.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;

namespace Remotion.Data.DomainObjects.UnitTests.Queries
{
  [TestFixture]
  public class LinqProviderComponentFactoryTest : StandardMappingTest
  {
    private LinqProviderComponentFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();
      _factory = new LinqProviderComponentFactory();
    }

    [Test]
    public void CreateQueryable ()
    {
      var executorStub = new Mock<IQueryExecutor>();
      var queryParserStub = new Mock<IQueryParser>();

      var result = _factory.CreateQueryable<Order>(queryParserStub.Object, executorStub.Object);

      Assert.That(result, Is.TypeOf(typeof(DomainObjectQueryable<Order>)));
      Assert.That(((DomainObjectQueryable<Order>)result).Provider.Executor, Is.SameAs(executorStub.Object));
      Assert.That(((DomainObjectQueryable<Order>)result).Provider.QueryParser, Is.SameAs(queryParserStub.Object));
    }

    [Test]
    public void CreateQueryParser_HasDefaultNodesAndSteps ()
    {
      var selectMethod = SelectExpressionNode.GetSupportedMethods().First();
      var queryParser = (QueryParser)_factory.CreateQueryParser();

      Assert.That(queryParser.NodeTypeProvider, Is.TypeOf(typeof(CompoundNodeTypeProvider)));
      Assert.That(((CompoundNodeTypeProvider)queryParser.NodeTypeProvider).InnerProviders[1], Is.TypeOf(typeof(MethodInfoBasedNodeTypeRegistry)));
      Assert.That(((CompoundNodeTypeProvider)queryParser.NodeTypeProvider).InnerProviders[2], Is.TypeOf(typeof(MethodNameBasedNodeTypeRegistry)));

      Assert.That(queryParser.NodeTypeProvider.GetNodeType(selectMethod), Is.SameAs(typeof(SelectExpressionNode)));
      var processingSteps = ((CompoundExpressionTreeProcessor)queryParser.Processor).InnerProcessors;
      Assert.That(processingSteps.Count,
          Is.EqualTo(ExpressionTreeParser.CreateDefaultProcessor(ExpressionTransformerRegistry.CreateDefault()).InnerProcessors.Count));
    }

    [Test]
    public void CreateQueryParser_RegistersDomainObjectCollectionContainsObject ()
    {
      var containsObjectMethod = typeof(DomainObjectCollection).GetMethod("ContainsObject");
      var queryParser = (QueryParser)_factory.CreateQueryParser();

      Assert.That(queryParser.NodeTypeProvider.GetNodeType(containsObjectMethod), Is.SameAs(typeof(ContainsExpressionNode)));
    }

    [Test]
    public void CreateQueryParser_RegistersDomainObjectCollectionCount ()
    {
      var countMethod = typeof(DomainObjectCollection).GetMethod("get_Count");
      var queryParser = (QueryParser)_factory.CreateQueryParser();

      Assert.That(queryParser.NodeTypeProvider.GetNodeType(countMethod), Is.SameAs(typeof(CountExpressionNode)));
    }

    [Test]
    public void CreateQueryParser_RegistersIObjectListCount ()
    {
      var countMethod = typeof(IObjectList<>).GetMethod("get_Count");
      var queryParser = (QueryParser)_factory.CreateQueryParser();

      Assert.That(queryParser.NodeTypeProvider.GetNodeType(countMethod), Is.SameAs(typeof(CountExpressionNode)));
    }

    [Test]
    public void CreateQueryParser_RegistersFetchObject ()
    {
      var fetchOneMethod = typeof(EagerFetchingExtensionMethods).GetMethod("FetchOne");
      var fetchManyMethod = typeof(EagerFetchingExtensionMethods).GetMethod("FetchMany");
      var thenFetchOneMethod = typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetchOne");
      var thenFetchManyMethod = typeof(EagerFetchingExtensionMethods).GetMethod("ThenFetchMany");

      var queryParser = (QueryParser)_factory.CreateQueryParser();

      Assert.That(queryParser.NodeTypeProvider.GetNodeType(fetchOneMethod), Is.SameAs(typeof(FetchOneExpressionNode)));
      Assert.That(queryParser.NodeTypeProvider.GetNodeType(fetchManyMethod), Is.SameAs(typeof(FetchManyExpressionNode)));
      Assert.That(queryParser.NodeTypeProvider.GetNodeType(thenFetchOneMethod), Is.SameAs(typeof(ThenFetchOneExpressionNode)));
      Assert.That(queryParser.NodeTypeProvider.GetNodeType(thenFetchManyMethod), Is.SameAs(typeof(ThenFetchManyExpressionNode)));
    }

    [Test]
    public void CreateQueryExecutor ()
    {
      var metadata = new Dictionary<string, object>
                     {
                       { "dummyKey", "dummyValue" }
                     };

      var executor = _factory.CreateQueryExecutor(TestDomainStorageProviderDefinition, "dummyID", metadata);

      Assert.That(executor, Is.TypeOf<DomainObjectQueryExecutor>());

      var domainObjectQueryExecutor = (DomainObjectQueryExecutor)executor;
      Assert.That(domainObjectQueryExecutor.StorageProviderDefinition, Is.SameAs(TestDomainStorageProviderDefinition));
      Assert.That(
          ((DomainObjectQueryGenerator)domainObjectQueryExecutor.QueryGenerator).MappingConfiguration,
          Is.SameAs(MappingConfiguration.Current));
      Assert.That(domainObjectQueryExecutor.ID, Is.EqualTo("dummyID"));
      Assert.That(domainObjectQueryExecutor.Metadata, Is.SameAs(metadata));
    }

    [Test]
    public void CreateQueryExecutor_MethodCallTransformerProvider ()
    {
      var executor = _factory.CreateQueryExecutor(TestDomainStorageProviderDefinition, "id", QueryObjectMother.EmptyMetadata);
      var provider = GetMethodCallTransformerProviderFromExecutor(executor);

      var toStringMethod = ToStringMethodCallTransformer.SupportedMethods[0];

      Assert.That(provider.Providers.Length, Is.EqualTo(2));

      Assert.That(provider.Providers[0], Is.TypeOf(typeof(MethodInfoBasedMethodCallTransformerRegistry)));
      Assert.That(provider.Providers[1], Is.TypeOf(typeof(NameBasedMethodCallTransformerRegistry)));

      Assert.That(((MethodInfoBasedMethodCallTransformerRegistry)provider.Providers[0]).GetItem(toStringMethod),
          Is.TypeOf(typeof(ToStringMethodCallTransformer)));
    }

    [Test]
    public void CreateQueryExecutor_ResultOperatorHandlerRegistry ()
    {
      var executor = _factory.CreateQueryExecutor(TestDomainStorageProviderDefinition, "id", QueryObjectMother.EmptyMetadata);
      var nodeTypeRegistry = GetResultOperatorHandlerRegistryFromExecutor(executor);

      Assert.That(nodeTypeRegistry.GetItem(typeof(CountResultOperator)), Is.TypeOf(typeof(CountResultOperatorHandler)));

      Assert.That(nodeTypeRegistry.GetItem(typeof(FetchOneRequest)), Is.TypeOf(typeof(FetchResultOperatorHandler)));
      Assert.That(nodeTypeRegistry.GetItem(typeof(FetchManyRequest)), Is.TypeOf(typeof(FetchResultOperatorHandler)));
    }

    private CompoundMethodCallTransformerProvider GetMethodCallTransformerProviderFromExecutor (IQueryExecutor executor)
    {
      var queryGenerator = (DomainObjectQueryGenerator)((DomainObjectQueryExecutor)executor).QueryGenerator;
      var sqlGenerator = (SqlQueryGenerator)queryGenerator.SqlQueryGenerator;
      var provider = ((DefaultSqlPreparationStage)sqlGenerator.PreparationStage).MethodCallTransformerProvider;
      Assert.That(provider, Is.TypeOf<CompoundMethodCallTransformerProvider>());
      return (CompoundMethodCallTransformerProvider)provider;
    }

    private ResultOperatorHandlerRegistry GetResultOperatorHandlerRegistryFromExecutor (IQueryExecutor executor)
    {
      var queryGenerator = (DomainObjectQueryGenerator)((DomainObjectQueryExecutor)executor).QueryGenerator;
      var sqlGenerator = (SqlQueryGenerator)queryGenerator.SqlQueryGenerator;
      return ((DefaultSqlPreparationStage)sqlGenerator.PreparationStage).ResultOperatorHandlerRegistry;
    }
  }
}
