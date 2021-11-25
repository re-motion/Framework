using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class SortExpressionDefinitionProviderTest : MappingReflectionTestBase
  {
    [Test]
    public void GetSortExpression_WithValidSortExpressionText_ReturnsNull ()
    {
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var referencedClassDefinition = Configuration.GetTypeDefinition(typeof (OrderItem));
      var referencePropertyInfo = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Order o) => o.OrderItems));

      var sortExpressionDefinition = sortExpressionDefinitionProvider.GetSortExpression(referencePropertyInfo, referencedClassDefinition, "Product asc");
      Assert.That(sortExpressionDefinition, Is.Not.Null);
      Assert.That(sortExpressionDefinition.ToString(), Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem.Product ASC"));
    }

    [Test]
    public void GetSortExpression_WithEmptySortExpressionText_ReturnsNull ()
    {
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var referencedClassDefinition = Configuration.GetTypeDefinition(typeof (OrderItem));
      var referencePropertyInfo = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Order o) => o.OrderItems));

      Assert.That(sortExpressionDefinitionProvider.GetSortExpression(referencePropertyInfo, referencedClassDefinition, ""), Is.Null);
    }

    [Test]
    public void GetSortExpression_WithNullSortExpressionText_ReturnsNull ()
    {
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var referencedClassDefinition = Configuration.GetTypeDefinition(typeof (OrderItem));
      var referencePropertyInfo = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Order o) => o.OrderItems));

      Assert.That(sortExpressionDefinitionProvider.GetSortExpression(referencePropertyInfo, referencedClassDefinition, null), Is.Null);
    }

    [Test]
    public void GetSortExpression_WithInvalidSortExpressionText_WrapsExceptionWithPropertyMetadata ()
    {
      var sortExpressionDefinitionProvider = new SortExpressionDefinitionProvider();
      var referencedClassDefinition = Configuration.GetTypeDefinition(typeof (OrderItem));
      var referencePropertyInfo = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Order o) => o.OrderItems));

      Assert.That(
          () => sortExpressionDefinitionProvider.GetSortExpression(referencePropertyInfo, referencedClassDefinition, "Product asc asc"),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "SortExpression 'Product asc asc' cannot be parsed: Expected 1 or 2 parts (a property name and an optional identifier), found 3 parts instead.\r\n\r\n" +
                  "Declaring type: Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order\r\nProperty: OrderItems"));
    }
  }
}