using NUnit.Framework;
using Remotion.Validation.Rules;
using Remotion.Validation.UnitTests.TestDomain;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class PropertyValidationRuleTest
  {
    [Test]
    [Ignore ("TODO RM-5960")]
    public void Validate ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5960")]
    public void ToString_Overridden ()
    {
      PropertyValidationRule<Customer, string> validationRule = null;
      Assert.That (
          validationRule.ToString(),
          Is.EqualTo ("PropertyValidationRule: Remotion.Validation.IntegrationTests.TestDomain.ComponentA.Address#PostalCode"));
    }
  }
}