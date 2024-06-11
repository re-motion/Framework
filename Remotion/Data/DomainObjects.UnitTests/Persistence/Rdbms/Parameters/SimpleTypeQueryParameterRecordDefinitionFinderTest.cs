using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Parameters;

[TestFixture]
public class SimpleTypeQueryParameterRecordDefinitionFinderTest
{
  [Test]
  public void Initialize ()
  {
    var structuredTypeDefinitionFinder = Mock.Of<IRdbmsStructuredTypeDefinitionFinder>();

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinder);

    Assert.That(simpleTypeQueryParameterRecordDefinitionFinder.StructuredTypeDefinitionFinder, Is.SameAs(structuredTypeDefinitionFinder));
  }

  [Test]
  public void GetRecordDefinition_ForNullParameterValue_ReturnsNull ()
  {
    var queryParameter = new QueryParameter("Null", null);
    var query = Mock.Of<IQuery>();
    var structuredTypeDefinitionFinder = Mock.Of<IRdbmsStructuredTypeDefinitionFinder>();

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinder);

    var result = simpleTypeQueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void GetRecordDefinition_FindsSingleColumnStructuredTypeDefinition_ReturnsRecordDefinition ()
  {
    var queryParameter = new QueryParameter("Test", Array.Empty<Guid>());
    var query = Mock.Of<IQuery>();
    var storagePropertyDefinition = new SimpleStoragePropertyDefinition(
        typeof(Guid),
        new ColumnDefinition("Value", StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation(), false));
    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "Test"),
        [ storagePropertyDefinition ],
        Array.Empty<ITableConstraintDefinition>());

    var structuredTypeDefinitionFinderStub = new Mock<IRdbmsStructuredTypeDefinitionFinder>();
    structuredTypeDefinitionFinderStub.Setup(_ => _.GetTypeDefinition(queryParameter, query)).Returns(tableTypeDefinition);

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinderStub.Object);

    var result = simpleTypeQueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.RecordName, Is.EqualTo("Guid"));
    Assert.That(result.StructuredTypeDefinition, Is.SameAs(tableTypeDefinition));
    Assert.That(result.PropertyDefinitions.Count, Is.EqualTo(1));
    Assert.That(result.PropertyDefinitions.Single().StoragePropertyDefinition, Is.SameAs(storagePropertyDefinition));
  }

  [Test]
  public void GetRecordDefinition_FindsMultiColumnStructuredTypeDefinition_ThrowsInvalidOperationException ()
  {
    var queryParameter = new QueryParameter("Test", Array.Empty<ObjectID>());
    var query = Mock.Of<IQuery>();
    var idValueStoragePropertyDefinition = new SimpleStoragePropertyDefinition(
        typeof(Guid),
        new ColumnDefinition("ID", StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation(), false));
    var classIdStoragePropertyDefinition = new SimpleStoragePropertyDefinition(
        typeof(string),
        new ColumnDefinition("ClassID", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(), false));
    var storagePropertyDefinition = new CompoundStoragePropertyDefinition(
        typeof(ObjectID),
        [
            new CompoundStoragePropertyDefinition.NestedPropertyInfo(idValueStoragePropertyDefinition, o => ((ObjectID)o).Value),
            new CompoundStoragePropertyDefinition.NestedPropertyInfo(classIdStoragePropertyDefinition, o => ((ObjectID)o).ClassID)
        ],
        parts => new ObjectID((string)parts[1], parts[0])
    );
    var tableTypeDefinition = new TableTypeDefinition(new EntityNameDefinition(null, "Test"), [ storagePropertyDefinition ], Array.Empty<ITableConstraintDefinition>());

    var structuredTypeDefinitionFinderStub = new Mock<IRdbmsStructuredTypeDefinitionFinder>();
    structuredTypeDefinitionFinderStub.Setup(_ => _.GetTypeDefinition(queryParameter, query)).Returns(tableTypeDefinition);

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinderStub.Object);

    Assert.That(
        () => simpleTypeQueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query),
        Throws.InstanceOf<InvalidOperationException>()
            .With.Message.EqualTo($"The structured type 'Test' cannot be mapped by {typeof(SimpleTypeQueryParameterRecordDefinitionFinder)}."));
  }

  [Test]
  public void GetRecordDefinition_FindsMultiPropertyStructuredTypeDefinition_ThrowsInvalidOperationException ()
  {
    var queryParameter = new QueryParameter("Test", Array.Empty<ObjectID>());
    var query = Mock.Of<IQuery>();
    var idValueStoragePropertyDefinition = new SimpleStoragePropertyDefinition(
        typeof(Guid),
        new ColumnDefinition("ID", StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation(), false));
    var classIdStoragePropertyDefinition = new SimpleStoragePropertyDefinition(
        typeof(string),
        new ColumnDefinition("ClassID", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation(), false));

    var tableTypeDefinition = new TableTypeDefinition(
        new EntityNameDefinition(null, "Test"),
        [ idValueStoragePropertyDefinition, classIdStoragePropertyDefinition ],
        Array.Empty<ITableConstraintDefinition>());

    var structuredTypeDefinitionFinderStub = new Mock<IRdbmsStructuredTypeDefinitionFinder>();
    structuredTypeDefinitionFinderStub.Setup(_ => _.GetTypeDefinition(queryParameter, query)).Returns(tableTypeDefinition);

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinderStub.Object);

    Assert.That(
        () => simpleTypeQueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query),
        Throws.InstanceOf<InvalidOperationException>()
            .With.Message.EqualTo($"The structured type 'Test' cannot be mapped by {typeof(SimpleTypeQueryParameterRecordDefinitionFinder)}."));
  }

  [Test]
  public void GetRecordDefinition_DoesNotFindStructuredTypeDefinition_ReturnsNull ()
  {
    var queryParameter = new QueryParameter("Test", Guid.Empty);
    var query = Mock.Of<IQuery>();

    var structuredTypeDefinitionFinderStub = new Mock<IRdbmsStructuredTypeDefinitionFinder>();
    structuredTypeDefinitionFinderStub.Setup(_ => _.GetTypeDefinition(queryParameter, query)).Returns((IRdbmsStructuredTypeDefinition)null);

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinderStub.Object);

    var result = simpleTypeQueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query);

    Assert.That(result, Is.Null);
  }

  [Test]
  public void GetRecordDefinition_NotSupportedStructuredTypeDefinition_ThrowsInvalidOperationException ()
  {
    var queryParameter = new QueryParameter("Test", Array.Empty<ObjectID>());
    var query = Mock.Of<IQuery>();
    var exception = new NotSupportedException("Dummy");

    var structuredTypeDefinitionFinderStub = new Mock<IRdbmsStructuredTypeDefinitionFinder>();
    structuredTypeDefinitionFinderStub.Setup(_ => _.GetTypeDefinition(queryParameter, query)).Throws(exception);

    var simpleTypeQueryParameterRecordDefinitionFinder = new SimpleTypeQueryParameterRecordDefinitionFinder(structuredTypeDefinitionFinderStub.Object);

    Assert.That(
        () => simpleTypeQueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query),
        Throws.InstanceOf<InvalidOperationException>()
            .With.InnerException.SameAs(exception)
            .With.Message.EqualTo($"The parameter value's type '{typeof(ObjectID[])}' cannot be mapped by {typeof(SimpleTypeQueryParameterRecordDefinitionFinder)}."));
  }
}
