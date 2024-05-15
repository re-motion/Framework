using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlTableTypeScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private SqlTableTypeScriptElementFactory _factory;
    private TableTypeDefinition _tableTypeDefinitionWithClusteredPrimaryKeyConstraint;
    private TableTypeDefinition _tableTypeDefinitionWithoutPrimaryKeyConstraint;
    private TableTypeDefinition _tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = new SqlTableTypeScriptElementFactory();


      var column1 = new ColumnDefinition("Column1", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(false), false);
      var column2 = new ColumnDefinition("Column2", StorageTypeInformationObjectMother.CreateBitStorageTypeInformation(true), false);
      var property1 = new SimpleStoragePropertyDefinition(typeof(string), column1);
      var property2 = new SimpleStoragePropertyDefinition(typeof(bool), column2);

      _tableTypeDefinitionWithoutPrimaryKeyConstraint = TableTypeDefinitionObjectMother.Create(
          StorageSettings.GetDefaultStorageProviderDefinition(),
          typeName: "TypeName",
          schemaName: "SchemaName",
          propertyDefinitions: new[] { property1, property2 }
      );

      _tableTypeDefinitionWithClusteredPrimaryKeyConstraint = TableTypeDefinitionObjectMother.Create(
          StorageSettings.GetDefaultStorageProviderDefinition(),
          typeName: "TypeName",
          propertyDefinitions: new[] { property1, property2 },

          constraints: new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition("PKName", true, new[] { column1 }) });

      _tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint = TableTypeDefinitionObjectMother.Create(
          StorageSettings.GetDefaultStorageProviderDefinition(),
          typeName: "TypeName",
          propertyDefinitions: new[] { property1, property2 },

          constraints: new ITableConstraintDefinition[] { new PrimaryKeyConstraintDefinition("PKName", false, new[] { column1, column2 }) });
    }

    [Test]
    public void GetCreateElement_TableTypeDefinitionWithoutPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithoutPrimaryKeyConstraint);

      var expectedResult =
          "IF TYPE_ID('[SchemaName].[TypeName]') IS NULL CREATE TYPE [SchemaName].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableTypeDefinitionWithClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF TYPE_ID('[dbo].[TypeName]') IS NULL CREATE TYPE [dbo].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL,\r\n"
          + "  CONSTRAINT [PKName] PRIMARY KEY CLUSTERED ([Column1])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetCreateElement_TableTypeDefinitionWithNonClusteredPrimaryKeyConstraint ()
    {
      var result = _factory.GetCreateElement(_tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "IF TYPE_ID('[dbo].[TypeName]') IS NULL CREATE TYPE [dbo].[TypeName] AS TABLE\r\n"
          + "(\r\n"
          + "  [Column1] varchar(100) NOT NULL,\r\n"
          + "  [Column2] bit NULL,\r\n"
          + "  CONSTRAINT [PKName] PRIMARY KEY NONCLUSTERED ([Column1], [Column2])\r\n"
          + ")";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement ()
    {
      var result = _factory.GetDropElement(_tableTypeDefinitionWithoutPrimaryKeyConstraint);

      var expectedResult =
          "DROP TYPE IF EXISTS [SchemaName].[TypeName]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetDropElement_DefaultSchema ()
    {
      var result = _factory.GetDropElement(_tableTypeDefinitionWithNonClusteredPrimaryKeyConstraint);

      var expectedResult =
          "DROP TYPE IF EXISTS [dbo].[TypeName]";

      Assert.That(result, Is.TypeOf(typeof(ScriptStatement)));
      Assert.That(((ScriptStatement)result).Statement, Is.EqualTo(expectedResult));
    }
  }
}
