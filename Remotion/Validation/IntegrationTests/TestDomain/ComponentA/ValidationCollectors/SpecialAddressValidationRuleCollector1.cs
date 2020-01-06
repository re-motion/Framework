using System;

namespace Remotion.Validation.IntegrationTests.TestDomain.ComponentA.ValidationCollectors
{
  public class SpecialAddressValidationRuleCollector1 : ValidationRuleCollectorBase<SpecialAddress>
  {
    public SpecialAddressValidationRuleCollector1 ()
    {
      AddRule (o => o.SpecialAddressIntroducedProperty).NotNull();
      AddRule (o => o.PostalCode).Matches ("1337");
    }
  }
}