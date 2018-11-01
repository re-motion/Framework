using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Remotion.Validation.IntegrationTests.TestDomain.ComponentA.ValidationCollectors
{
  public class SpecialAddressValidationCollector1 : ComponentValidationCollector<SpecialAddress>
  {
    public SpecialAddressValidationCollector1 ()
    {
      AddRule (o => o.SpecialAddressIntroducedProperty).NotNull();
      AddRule (o => o.PostalCode).Matches ("1337");
    }
  }
}