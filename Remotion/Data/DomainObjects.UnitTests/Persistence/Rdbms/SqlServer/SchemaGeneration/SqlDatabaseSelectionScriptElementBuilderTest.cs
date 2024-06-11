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
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlDatabaseSelectionScriptElementBuilderTest : SchemaGenerationTestBase
  {
    private Mock<IScriptBuilder> _innerScriptBuilderMock;
    private SqlDatabaseSelectionScriptElementBuilder _builder;

    public override void SetUp ()
    {
      base.SetUp();

      var connectionString = "Data Source=myServerAddress;Initial Catalog=MyDataBase;User Id=myUsername;Password=myPassword;";
      _innerScriptBuilderMock = new Mock<IScriptBuilder>();
      _builder = new SqlDatabaseSelectionScriptElementBuilder(_innerScriptBuilderMock.Object, connectionString);
    }

    [Test]
    public void AddStructuredTypeDefinition_CallsInnerBuilder ()
    {
      var structuredTypeDefinition = Mock.Of<IRdbmsStructuredTypeDefinition>();
      _innerScriptBuilderMock.Setup(mock => mock.AddStructuredTypeDefinition(structuredTypeDefinition)).Verifiable();

      Assert.That(() => _builder.AddStructuredTypeDefinition(structuredTypeDefinition), Throws.Nothing);

      _innerScriptBuilderMock.Verify();
    }

    [Test]
    public void AddEntityDefinition ()
    {
      var entityDefinitionStub = new Mock<IRdbmsStorageEntityDefinition>();

      _innerScriptBuilderMock.Setup(mock => mock.AddEntityDefinition(entityDefinitionStub.Object)).Verifiable();

      _builder.AddEntityDefinition(entityDefinitionStub.Object);

      _innerScriptBuilderMock.Verify();
    }

    [Test]
    public void GetCreateScript_GetDropScript_ValidConnectionString ()
    {
      var statement1 = new ScriptStatement("Test1");
      var statement2 = new ScriptStatement("Test2");
      var fakeCreateResult = new ScriptElementCollection();
      fakeCreateResult.AddElement(statement1);
      fakeCreateResult.AddElement(statement2);
      var fakeDropResult = new ScriptElementCollection();
      fakeDropResult.AddElement(statement2);
      fakeDropResult.AddElement(statement1);

      _innerScriptBuilderMock.Setup(mock => mock.GetCreateScript()).Returns(fakeCreateResult).Verifiable();
      _innerScriptBuilderMock.Setup(mock => mock.GetDropScript()).Returns(fakeDropResult).Verifiable();

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement, Is.EqualTo("USE MyDataBase"));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[1], Is.SameAs(fakeCreateResult));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("USE MyDataBase"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[1], Is.SameAs(fakeDropResult));
    }

    [Test]
    public void GetCreateScript_WithConnectionStringMissingInitialCatalog_ThrowsInvalidOperationException ()
    {
      var builder = new SqlDatabaseSelectionScriptElementBuilder(
          _innerScriptBuilderMock.Object,
          new SqlConnectionStringBuilder { DataSource = "localhost" }.ToString());

      Assert.That(
          () => builder.GetCreateScript(),
          Throws.InvalidOperationException
                .With.Message.EqualTo("No database name could be found in the given connection string 'Data Source=localhost'."));
    }

    [Test]
    public void GetDropScript_WithConnectionStringMissingInitialCatalog_ThrowsInvalidOperationException ()
    {
      var builder = new SqlDatabaseSelectionScriptElementBuilder(
          _innerScriptBuilderMock.Object,
          new SqlConnectionStringBuilder { DataSource = "localhost" }.ToString());

      Assert.That(
          () => builder.GetDropScript(),
          Throws.InvalidOperationException
                .With.Message.EqualTo("No database name could be found in the given connection string 'Data Source=localhost'."));
    }
  }
}
