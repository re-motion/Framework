using System;

namespace Remotion.Validation.IntegrationTests.TestDomain.ComponentA
{
  public class SpecialAddress : Address
  {
    public new string Street { get; set; }

    public override string PostalCode { get; set; }

    public string SpecialAddressIntroducedProperty { get; set; }
  }
}
