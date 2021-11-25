using System.Web.UI;
using NUnit.Framework.Constraints;

namespace Remotion.Web.UnitTests
{
  public class PairConstraintResult : ConstraintResult
  {
    private readonly Pair _expected;
    private readonly Pair _actual;

    public PairConstraintResult (PairConstraint constraint, Pair actualValue, Pair expected, bool isSuccess)
        : base(constraint, actualValue, isSuccess)
    {
      _actual = actualValue;
      _expected = expected;
    }

    public override void WriteMessageTo (MessageWriter writer)
    {
      if (_expected == null || _actual == null)
      {
        base.WriteMessageTo(writer);
      }
      else
      {
        writer.DisplayStringDifferences(
            string.Format("{{ {0} , {1} }}", _expected.First, _expected.Second),
            string.Format("{{ {0} , {1} }}", _actual.First, _actual.Second),
            -1,
            false,
            false);
      }
    }
  }
}