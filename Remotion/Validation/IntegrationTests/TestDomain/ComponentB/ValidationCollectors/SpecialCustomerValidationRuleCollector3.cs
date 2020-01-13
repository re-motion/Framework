using System;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentA.ValidationCollectors;
using Remotion.Validation.IntegrationTests.TestDomain.Validators;

namespace Remotion.Validation.IntegrationTests.TestDomain.ComponentB.ValidationCollectors
{
  public class SpecialCustomerValidationRuleCollector3 : ValidationRuleCollectorBase<SpecialCustomer3>
  {
    public SpecialCustomerValidationRuleCollector3 ()
    {
      RemoveRule().Validator<FakeCustomerValidator, CustomerValidationRuleCollector1>();
    }
  }
}