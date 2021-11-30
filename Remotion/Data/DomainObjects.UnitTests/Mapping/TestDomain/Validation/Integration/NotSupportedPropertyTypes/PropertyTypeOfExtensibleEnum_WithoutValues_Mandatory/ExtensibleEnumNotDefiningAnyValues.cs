using System;
using System.Reflection;
using Remotion.ExtensibleEnums;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Validation.Integration.NotSupportedPropertyTypes.PropertyTypeOfExtensibleEnum_WithoutValues_Mandatory
{
  public class ExtensibleEnumNotDefiningAnyValues : ExtensibleEnum<ExtensibleEnumNotDefiningAnyValues>
  {
    public ExtensibleEnumNotDefiningAnyValues (MethodBase currentMethod)
        : base(currentMethod)
    {
    }
  }
}
