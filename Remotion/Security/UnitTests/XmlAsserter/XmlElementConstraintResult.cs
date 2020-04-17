using NUnit.Framework.Constraints;

namespace Remotion.Security.UnitTests.XmlAsserter
{
  public class XmlElementConstraintResult : ConstraintResult
  {
    public XmlElementConstraintResult (IConstraint constraint, object actualValue, bool isSuccess)
        : base(constraint, actualValue, isSuccess)
    {
    }
  }
}