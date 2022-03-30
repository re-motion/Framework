namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Reflection.MappingAttributesAreOnlyAppliedOnOriginalPropertyDeclarationsValidationRule
{
  public interface IBaseMappingAttributeInterface : IDomainObject
  {
    [StorageClassNone]
    int Property1 { get; set; }

    int Property2 { get; set; }

    int Property3 { get; set; }

    [StorageClassNone]
    int Property4 { get; set; }
  }
}
