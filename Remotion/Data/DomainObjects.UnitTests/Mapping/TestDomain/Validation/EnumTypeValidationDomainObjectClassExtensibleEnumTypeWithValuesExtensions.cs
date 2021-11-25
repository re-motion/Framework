using System;
using System.Reflection;
using Remotion.ExtensibleEnums;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation
{
  public static class EnumTypeValidationDomainObjectClassExtensibleEnumTypeWithValuesExtensions
  {
    public static EnumTypeValidationDomainObjectClass.ExtensibleEnumTypeWithValues Value (this ExtensibleEnumDefinition<EnumTypeValidationDomainObjectClass.ExtensibleEnumTypeWithValues> definition)
    {
      return new EnumTypeValidationDomainObjectClass.ExtensibleEnumTypeWithValues(MethodBase.GetCurrentMethod());
    }
  }
}