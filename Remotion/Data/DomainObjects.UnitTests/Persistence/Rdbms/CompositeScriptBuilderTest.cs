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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class CompositeScriptBuilderTest : SchemaGenerationTestBase
  {
    private Mock<IScriptBuilder> _builder1Mock;
    private Mock<IScriptBuilder> _builder2Mock;
    private ScriptElementCollection _fakeResultCollection1;
    private ScriptElementCollection _fakeResultCollection2;
    private ScriptStatement _fakeStatement1;
    private ScriptStatement _fakeStatement2;
    private ScriptStatement _fakeStatement3;

    public override void SetUp ()
    {
      base.SetUp();

      _builder1Mock = new Mock<IScriptBuilder>(MockBehavior.Strict);
      _builder2Mock = new Mock<IScriptBuilder>(MockBehavior.Strict);

      _fakeStatement1 = new ScriptStatement("Fake1");
      _fakeStatement2 = new ScriptStatement("Fake2");
      _fakeStatement3 = new ScriptStatement("Fake3");

      _fakeResultCollection1 = new ScriptElementCollection();
      _fakeResultCollection1.AddElement(_fakeStatement1);

      _fakeResultCollection2 = new ScriptElementCollection();
      _fakeResultCollection2.AddElement(_fakeStatement2);
      _fakeResultCollection2.AddElement(_fakeStatement3);
    }

    [Test]
    public void Initialization ()
    {
      var builder = new CompositeScriptBuilder(SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock.Object, _builder2Mock.Object });
      Assert.That(builder.RdbmsProviderDefinition, Is.SameAs(SchemaGenerationFirstStorageProviderDefinition));
      Assert.That(builder.ScriptBuilders, Is.EqualTo(new[]{_builder1Mock.Object, _builder2Mock.Object}));
    }

    [Test]
    public void Initialization_WithNestedCompositeScriptBuilder_InlinesNestedScriptBuilder ()
    {
      var builder = new CompositeScriptBuilder(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { new CompositeScriptBuilder(SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock.Object }), _builder2Mock.Object });

      Assert.That(builder.RdbmsProviderDefinition, Is.SameAs(SchemaGenerationFirstStorageProviderDefinition));
      Assert.That(builder.ScriptBuilders, Is.EqualTo(new[] { _builder1Mock.Object, _builder2Mock.Object }));
    }

    [Test]
    public void Initialization_WithNestedCompositeScriptBuilderHavingMismatchedStorageProviderDefinitions_ThrowsArgumentException ()
    {
      Assert.That(
          () => new CompositeScriptBuilder(
              SchemaGenerationFirstStorageProviderDefinition,
              new[] { new CompositeScriptBuilder(SchemaGenerationSecondStorageProviderDefinition, new[] { _builder1Mock.Object }), _builder2Mock.Object }),
          Throws.ArgumentException.With.Message.StartsWith(
              "The scriptBuilder sequence contains a CompositeScriptBuilder that references a different RdbmsProviderDefinition "
              + "('SchemaGenerationSecondStorageProvider') than the current CompositeScriptBuilder ('SchemaGenerationFirstStorageProvider')."));
    }

    [Test]
    public void AddEntityDefinition_CallsAllSubBuilders ()
    {
      var entityDefinition = Mock.Of<IRdbmsStorageEntityDefinition>();
      var subBuilderMocks = new[]
                            {
                                CreateSubBuilderMock(entityDefinition),
                                CreateSubBuilderMock(entityDefinition),
                                CreateSubBuilderMock(entityDefinition)
                            };
      var builder = new CompositeScriptBuilder(SchemaGenerationFirstStorageProviderDefinition, subBuilderMocks.Select(o => o.Object));
      builder.AddEntityDefinition(entityDefinition);

      subBuilderMocks[0].Verify();
      subBuilderMocks[1].Verify();
      subBuilderMocks[2].Verify();

      Mock<IScriptBuilder> CreateSubBuilderMock (IRdbmsStorageEntityDefinition storageEntityDefinition)
      {
        var subBuilderMock = new Mock<IScriptBuilder>();
        subBuilderMock.Setup(_ => _.AddEntityDefinition(storageEntityDefinition)).Verifiable();
        return subBuilderMock;
      }
    }

    [Test]
    public void AddStructuredTypeDefinition_CallsAllSubBuilders ()
    {
      Mock<IScriptBuilder> CreateSubBuilderMock ()
      {
        var subBuilderMock = new Mock<IScriptBuilder>();
        subBuilderMock.Setup(_ => _.AddStructuredTypeDefinition(It.IsAny<IRdbmsStructuredTypeDefinition>())).Verifiable();
        return subBuilderMock;
      }

      var subBuilderMocks = new[] { CreateSubBuilderMock(), CreateSubBuilderMock(), CreateSubBuilderMock() };
      var builder = new CompositeScriptBuilder(SchemaGenerationFirstStorageProviderDefinition, subBuilderMocks.Select(o => o.Object));
      builder.AddStructuredTypeDefinition(Mock.Of<IRdbmsStructuredTypeDefinition>());

      subBuilderMocks[0].Verify();
      subBuilderMocks[1].Verify();
      subBuilderMocks[2].Verify();
    }

    [Test]
    public void GetCreateScript ()
    {
      var entityDefinition1 = new Mock<IRdbmsStorageEntityDefinition>();
      var entityDefinition2 = new Mock<IRdbmsStorageEntityDefinition>();

      _builder1Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition1.Object)).Verifiable();
      _builder1Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition2.Object)).Verifiable();
      _builder1Mock.Setup(mock => mock.GetCreateScript()).Returns(_fakeResultCollection1).Verifiable();

      _builder2Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition1.Object)).Verifiable();
      _builder2Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition2.Object)).Verifiable();
      _builder2Mock.Setup(mock => mock.GetCreateScript()).Returns(_fakeResultCollection2).Verifiable();

      var builder = new CompositeScriptBuilder(SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock.Object, _builder2Mock.Object });

      builder.AddEntityDefinition(entityDefinition1.Object);
      builder.AddEntityDefinition(entityDefinition2.Object);

      var result = (ScriptElementCollection)builder.GetCreateScript();

      _builder1Mock.Verify();
      _builder2Mock.Verify();

      Assert.That(result.Elements.Count, Is.EqualTo(2));
      Assert.That(result.Elements[0], Is.SameAs(_fakeResultCollection1));
      Assert.That(result.Elements[1], Is.SameAs(_fakeResultCollection2));
    }

    [Test]
    public void GetDropScript ()
    {
      var entityDefinition1 = new Mock<IRdbmsStorageEntityDefinition>();
      var entityDefinition2 = new Mock<IRdbmsStorageEntityDefinition>();

      _builder1Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition1.Object)).Verifiable();
      _builder1Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition2.Object)).Verifiable();
      _builder1Mock.Setup(mock => mock.GetDropScript()).Returns(_fakeResultCollection1).Verifiable();

      _builder2Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition1.Object)).Verifiable();
      _builder2Mock.Setup(mock => mock.AddEntityDefinition(entityDefinition2.Object)).Verifiable();
      _builder2Mock.Setup(mock => mock.GetDropScript()).Returns(_fakeResultCollection2).Verifiable();

      var builder = new CompositeScriptBuilder(SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock.Object, _builder2Mock.Object });

      builder.AddEntityDefinition(entityDefinition1.Object);
      builder.AddEntityDefinition(entityDefinition2.Object);

      var result = (ScriptElementCollection)builder.GetDropScript();

      _builder1Mock.Verify();
      _builder2Mock.Verify();

      Assert.That(result.Elements.Count, Is.EqualTo(2));
      Assert.That(result.Elements[0], Is.SameAs(_fakeResultCollection2));
      Assert.That(result.Elements[1], Is.SameAs(_fakeResultCollection1));
    }

  }
}
