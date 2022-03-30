namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule
{
  public class ClassImplementingInterfaceWithMappingAttribute : DomainObject, IBaseMappingAttributeInterface
  {
    [StorageClassNone]
    public int Property1 { get; set; }

    public int Property2 { get; set; }

    [StorageClassNone]
    public int Property3 { get; set; }

    public int Property4 { get; set; }
  }
}
