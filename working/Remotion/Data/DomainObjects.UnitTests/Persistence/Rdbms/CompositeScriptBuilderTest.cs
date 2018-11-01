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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class CompositeScriptBuilderTest : SchemaGenerationTestBase
  {
    private IScriptBuilder _builder1Mock;
    private IScriptBuilder _builder2Mock;
    private ScriptElementCollection _fakeResultCollection1;
    private ScriptElementCollection _fakeResultCollection2;
    private ScriptStatement _fakeStatement1;
    private ScriptStatement _fakeStatement2;
    private ScriptStatement _fakeStatement3;

    public override void SetUp ()
    {
      base.SetUp ();

      _builder1Mock = MockRepository.GenerateStrictMock<IScriptBuilder>();
      _builder2Mock = MockRepository.GenerateStrictMock<IScriptBuilder>();

      _fakeStatement1 = new ScriptStatement ("Fake1");
      _fakeStatement2 = new ScriptStatement ("Fake2");
      _fakeStatement3 = new ScriptStatement ("Fake3");

      _fakeResultCollection1 = new ScriptElementCollection();
      _fakeResultCollection1.AddElement (_fakeStatement1);

      _fakeResultCollection2 = new ScriptElementCollection ();
      _fakeResultCollection2.AddElement (_fakeStatement2);
      _fakeResultCollection2.AddElement (_fakeStatement3);
    }

    [Test]
    public void Initialization ()
    {
      var builder = new CompositeScriptBuilder (SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock, _builder2Mock });
      Assert.That (builder.RdbmsProviderDefinition, Is.SameAs (SchemaGenerationFirstStorageProviderDefinition));
      Assert.That (builder.ScriptBuilders, Is.EqualTo(new[]{_builder1Mock, _builder2Mock}));
    }

    [Test]
    public void Initialization_WithNestedCompositeScriptBuilder_InlinesNestedScriptBuilder ()
    {
      var builder = new CompositeScriptBuilder (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { new CompositeScriptBuilder (SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock }), _builder2Mock });

      Assert.That (builder.RdbmsProviderDefinition, Is.SameAs (SchemaGenerationFirstStorageProviderDefinition));
      Assert.That (builder.ScriptBuilders, Is.EqualTo (new[] { _builder1Mock, _builder2Mock }));
    }

    [Test]
    public void Initialization_WithNestedCompositeScriptBuilderHavingMismatchedStorageProviderDefinitions_ThrowsArgumentException ()
    {
      Assert.That (
          () => new CompositeScriptBuilder (
              SchemaGenerationFirstStorageProviderDefinition,
              new[] { new CompositeScriptBuilder (SchemaGenerationSecondStorageProviderDefinition, new[] { _builder1Mock }), _builder2Mock }),
          Throws.ArgumentException.With.Message.StringStarting (
              "The scriptBuilder sequence contains a CompositeScriptBuilder that references a different RdbmsProviderDefinition "
              + "('SchemaGenerationSecondStorageProvider') than the current CompositeScriptBuilder ('SchemaGenerationFirstStorageProvider')."));
    }

    [Test]
    public void GetCreateScript ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition> ();
      var entityDefinition2 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition> ();

      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder1Mock.Expect (mock => mock.GetCreateScript ()).Return (_fakeResultCollection1);
      _builder1Mock.Replay ();

      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder2Mock.Expect (mock => mock.GetCreateScript ()).Return (_fakeResultCollection2);
      _builder2Mock.Replay ();

      var builder = new CompositeScriptBuilder (SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock, _builder2Mock });

      builder.AddEntityDefinition (entityDefinition1);
      builder.AddEntityDefinition (entityDefinition2);

      var result = (ScriptElementCollection) builder.GetCreateScript ();

      _builder1Mock.VerifyAllExpectations ();
      _builder2Mock.VerifyAllExpectations ();
      
      Assert.That (result.Elements.Count, Is.EqualTo (2));
      Assert.That (result.Elements[0], Is.SameAs (_fakeResultCollection1));
      Assert.That (result.Elements[1], Is.SameAs (_fakeResultCollection2));
    }
    
    [Test]
    public void GetDropScript ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition> ();
      var entityDefinition2 = MockRepository.GenerateStub<IRdbmsStorageEntityDefinition> ();

      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder1Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder1Mock.Expect (mock => mock.GetDropScript ()).Return (_fakeResultCollection1);
      _builder1Mock.Replay ();

      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition1));
      _builder2Mock.Expect (mock => mock.AddEntityDefinition (entityDefinition2));
      _builder2Mock.Expect (mock => mock.GetDropScript ()).Return (_fakeResultCollection2);
      _builder2Mock.Replay ();

      var builder = new CompositeScriptBuilder (SchemaGenerationFirstStorageProviderDefinition, new[] { _builder1Mock, _builder2Mock });

      builder.AddEntityDefinition (entityDefinition1);
      builder.AddEntityDefinition (entityDefinition2);

      var result = (ScriptElementCollection) builder.GetDropScript ();

      _builder1Mock.VerifyAllExpectations ();
      _builder2Mock.VerifyAllExpectations ();

      Assert.That (result.Elements.Count, Is.EqualTo (2));
      Assert.That (result.Elements[0], Is.SameAs (_fakeResultCollection2));
      Assert.That (result.Elements[1], Is.SameAs (_fakeResultCollection1));
    }

  }
}