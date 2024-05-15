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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ScriptGeneratorTest : SchemaGenerationTestBase
  {
    private Mock<IRdbmsStorageEntityDefinition> _firstProviderStorageEntityDefinitionStub;
    private Mock<IRdbmsStorageEntityDefinition> _secondProviderStorageEntityDefinitionStub;
    private Mock<IRdbmsStorageEntityDefinition> _thirdProviderStorageEntityDefinitionStub;
    private ClassDefinition _classDefinitionForFirstStorageProvider1;
    private ClassDefinition _classDefinitionForFirstStorageProvider2;
    private ClassDefinition _classDefinitionForSecondStorageProvider;
    private ClassDefinition _classDefinitionForThirdStorageProvider;
    private Mock<IRdbmsStorageEntityDefinitionProvider> _entityDefinitionProviderMock;
    private Mock<IRdbmsStructuredTypeDefinitionProvider> _structuredTypeDefinitionProviderMock;
    private Mock<IScriptToStringConverter> _scriptToStringConverterStub;
    private ScriptGenerator _scriptGenerator;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinition1;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinition2;
    private Mock<IRdbmsStorageEntityDefinition> _fakeEntityDefinition3;
    private ScriptPair _fakeScriptResult1;
    private ScriptPair _fakeScriptResult2;
    private ScriptPair _fakeScriptResult3;
    private Mock<IScriptBuilder> _scriptBuilderForFirstStorageProviderMock;
    private Mock<IScriptBuilder> _scriptBuilderForSecondStorageProviderMock;
    private Mock<IScriptBuilder> _scriptBuilderForThirdStorageProviderMock;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinitionForFirstStorageProvider1 = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));
      _classDefinitionForFirstStorageProvider2 = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(OrderItem));
      _classDefinitionForSecondStorageProvider = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));
      _classDefinitionForThirdStorageProvider = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Customer));

      _firstProviderStorageEntityDefinitionStub = new Mock<IRdbmsStorageEntityDefinition>();
      _firstProviderStorageEntityDefinitionStub.Setup(stub => stub.StorageProviderDefinition).Returns(SchemaGenerationFirstStorageProviderDefinition);

      _secondProviderStorageEntityDefinitionStub = new Mock<IRdbmsStorageEntityDefinition>();
      _secondProviderStorageEntityDefinitionStub.Setup(stub => stub.StorageProviderDefinition).Returns(SchemaGenerationSecondStorageProviderDefinition);

      _thirdProviderStorageEntityDefinitionStub = new Mock<IRdbmsStorageEntityDefinition>();
      _thirdProviderStorageEntityDefinitionStub.Setup(stub => stub.StorageProviderDefinition).Returns(new NoRdbmsUnitTestStorageProviderStubDefinition("Test"));

      _classDefinitionForFirstStorageProvider1.SetStorageEntity(_firstProviderStorageEntityDefinitionStub.Object);
      _classDefinitionForFirstStorageProvider2.SetStorageEntity(_firstProviderStorageEntityDefinitionStub.Object);
      _classDefinitionForSecondStorageProvider.SetStorageEntity(_secondProviderStorageEntityDefinitionStub.Object);
      _classDefinitionForThirdStorageProvider.SetStorageEntity(_thirdProviderStorageEntityDefinitionStub.Object);

      _fakeScriptResult1 = new ScriptPair("CreateScript1", "DropScript1");
      _fakeScriptResult2 = new ScriptPair("CreateScript2", "DropScript2");
      _fakeScriptResult3 = new ScriptPair("CreateScript3", "DropScript3");
      _entityDefinitionProviderMock = new Mock<IRdbmsStorageEntityDefinitionProvider>(MockBehavior.Strict);
      _structuredTypeDefinitionProviderMock = new Mock<IRdbmsStructuredTypeDefinitionProvider>(MockBehavior.Strict);
      _scriptBuilderForFirstStorageProviderMock = new Mock<IScriptBuilder>(MockBehavior.Strict);
      _scriptBuilderForSecondStorageProviderMock = new Mock<IScriptBuilder>(MockBehavior.Strict);
      _scriptBuilderForThirdStorageProviderMock = new Mock<IScriptBuilder>(MockBehavior.Strict);

      _scriptToStringConverterStub = new Mock<IScriptToStringConverter>();
      _scriptToStringConverterStub.Setup(stub => stub.Convert(_scriptBuilderForFirstStorageProviderMock.Object)).Returns(_fakeScriptResult1);
      _scriptToStringConverterStub.Setup(stub => stub.Convert(_scriptBuilderForSecondStorageProviderMock.Object)).Returns(_fakeScriptResult2);
      _scriptToStringConverterStub.Setup(stub => stub.Convert(_scriptBuilderForThirdStorageProviderMock.Object)).Returns(_fakeScriptResult3);

      _scriptGenerator = new ScriptGenerator(
          pd =>
          {
            switch (pd.Name)
            {
              case "SchemaGenerationFirstStorageProvider":
                return _scriptBuilderForFirstStorageProviderMock.Object;
              case "SchemaGenerationSecondStorageProvider":
                return _scriptBuilderForSecondStorageProviderMock.Object;
              case "SchemaGenerationThirdStorageProvider":
                return _scriptBuilderForThirdStorageProviderMock.Object;
            }
            throw new InvalidOperationException("Invalid storage provider!");
          },
          _structuredTypeDefinitionProviderMock.Object,
          _entityDefinitionProviderMock.Object,
          _scriptToStringConverterStub.Object);

      _fakeEntityDefinition1 = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinition2 = new Mock<IRdbmsStorageEntityDefinition>();
      _fakeEntityDefinition3 = new Mock<IRdbmsStorageEntityDefinition>();
    }

    [Test]
    public void GetScripts_NoClassDefinitions ()
    {
      var result = _scriptGenerator.GetScripts(Array.Empty<ClassDefinition>(), Array.Empty<TupleDefinition>());

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetScript_ClassDefinitionWithoutRdbmsStorageProviderDefinition ()
    {
      var result = _scriptGenerator.GetScripts(new[] { _classDefinitionForThirdStorageProvider }, Array.Empty<TupleDefinition>());

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetScripts_OneClassDefinitionWithRdbmsStorageProviderDefinition ()
    {
      _entityDefinitionProviderMock
          .Setup(mock => mock.GetEntityDefinitions(new[] { _classDefinitionForSecondStorageProvider }))
          .Returns(new[] { _fakeEntityDefinition1.Object })
          .Verifiable();
      _structuredTypeDefinitionProviderMock
          .Setup(mock => mock.GetTypeDefinitions(Array.Empty<TupleDefinition>()))
          .Returns(Array.Empty<IRdbmsStructuredTypeDefinition>())
          .Verifiable();

      _scriptBuilderForSecondStorageProviderMock.Setup(mock => mock.AddEntityDefinition(_fakeEntityDefinition1.Object)).Verifiable();

      var result = _scriptGenerator.GetScripts(new[] { _classDefinitionForSecondStorageProvider }, Array.Empty<TupleDefinition>()).ToList();

      _entityDefinitionProviderMock.Verify();
      _structuredTypeDefinitionProviderMock.Verify();
      _scriptBuilderForSecondStorageProviderMock.Verify();
      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0].StorageProviderDefinition, Is.SameAs(SchemaGenerationSecondStorageProviderDefinition));
      Assert.That(result[0].SetUpScript, Is.EqualTo("CreateScript2"));
      Assert.That(result[0].TearDownScript, Is.EqualTo("DropScript2"));
    }

    [Test]
    public void GetScripts_SeveralClassDefinitionWithSameRdbmsStorageProviderDefinitions ()
    {
      _entityDefinitionProviderMock
          .Setup(mock => mock.GetEntityDefinitions(new[] { _classDefinitionForFirstStorageProvider1, _classDefinitionForFirstStorageProvider2 }))
          .Returns(new[] { _fakeEntityDefinition1.Object, _fakeEntityDefinition2.Object })
          .Verifiable();
      _structuredTypeDefinitionProviderMock
          .Setup(mock => mock.GetTypeDefinitions(Array.Empty<TupleDefinition>()))
          .Returns(Array.Empty<IRdbmsStructuredTypeDefinition>())
          .Verifiable();

      _scriptBuilderForFirstStorageProviderMock.Setup(mock => mock.AddEntityDefinition(_fakeEntityDefinition1.Object)).Verifiable();
      _scriptBuilderForFirstStorageProviderMock.Setup(mock => mock.AddEntityDefinition(_fakeEntityDefinition2.Object)).Verifiable();

      var result = _scriptGenerator
          .GetScripts(new[] { _classDefinitionForFirstStorageProvider1, _classDefinitionForFirstStorageProvider2 }, Array.Empty<TupleDefinition>()).ToList();

      _entityDefinitionProviderMock.Verify();
      _structuredTypeDefinitionProviderMock.Verify();
      _scriptBuilderForFirstStorageProviderMock.Verify();
      Assert.That(result.Count, Is.EqualTo(1));
      Assert.That(result[0].StorageProviderDefinition, Is.SameAs(SchemaGenerationFirstStorageProviderDefinition));
      Assert.That(result[0].SetUpScript, Is.EqualTo("CreateScript1"));
      Assert.That(result[0].TearDownScript, Is.EqualTo("DropScript1"));
    }

    [Test]
    public void GetScripts_SeveralClassDefinitionWithDifferentRdbmsStorageProviderDefinitions ()
    {
      _entityDefinitionProviderMock
          .Setup(mock => mock.GetEntityDefinitions(new[] { _classDefinitionForFirstStorageProvider1, _classDefinitionForFirstStorageProvider2 }))
          .Returns(new[] { _fakeEntityDefinition1.Object, _fakeEntityDefinition2.Object })
          .Verifiable();
      _entityDefinitionProviderMock
          .Setup(mock => mock.GetEntityDefinitions(new[] { _classDefinitionForSecondStorageProvider }))
          .Returns(new[] { _fakeEntityDefinition3.Object })
          .Verifiable();
      _structuredTypeDefinitionProviderMock
          .Setup(mock => mock.GetTypeDefinitions(Array.Empty<TupleDefinition>()))
          .Returns(Array.Empty<IRdbmsStructuredTypeDefinition>())
          .Verifiable();

      _scriptBuilderForFirstStorageProviderMock.Setup(mock => mock.AddEntityDefinition(_fakeEntityDefinition1.Object)).Verifiable();
      _scriptBuilderForFirstStorageProviderMock.Setup(mock => mock.AddEntityDefinition(_fakeEntityDefinition2.Object)).Verifiable();

      _scriptBuilderForSecondStorageProviderMock.Setup(mock => mock.AddEntityDefinition(_fakeEntityDefinition3.Object)).Verifiable();

      var result =
          _scriptGenerator.GetScripts(
              new[]
              {
                  _classDefinitionForFirstStorageProvider1,
                  _classDefinitionForFirstStorageProvider2,
                  _classDefinitionForSecondStorageProvider,
                  _classDefinitionForThirdStorageProvider
              },
              Array.Empty<TupleDefinition>()).ToList();

      _entityDefinitionProviderMock.Verify();
      _structuredTypeDefinitionProviderMock.Verify();
      _scriptBuilderForFirstStorageProviderMock.Verify();
      _scriptBuilderForSecondStorageProviderMock.Verify();
      Assert.That(result.Count, Is.EqualTo(2));
      Assert.That(result[0].StorageProviderDefinition, Is.SameAs(SchemaGenerationFirstStorageProviderDefinition));
      Assert.That(result[0].SetUpScript, Is.EqualTo("CreateScript1"));
      Assert.That(result[0].TearDownScript, Is.EqualTo("DropScript1"));
      Assert.That(result[1].StorageProviderDefinition, Is.SameAs(SchemaGenerationSecondStorageProviderDefinition));
      Assert.That(result[1].SetUpScript, Is.EqualTo("CreateScript2"));
      Assert.That(result[1].TearDownScript, Is.EqualTo("DropScript2"));
    }
  }
}
