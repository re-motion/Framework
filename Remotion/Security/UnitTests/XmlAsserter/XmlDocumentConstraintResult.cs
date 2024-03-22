using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace Remotion.Security.UnitTests.XmlAsserter
{
  public class XmlDocumentConstraintResult : ConstraintResult
  {
    private readonly IReadOnlyCollection<string> _messages;

    public XmlDocumentConstraintResult (
        IConstraint constraint,
        object actualValue,
        bool isSuccess,
        IReadOnlyCollection<string> messages)
        : base(constraint, actualValue, isSuccess)
    {
      _messages = messages;
    }


    public override void WriteMessageTo (MessageWriter writer)
    {
      writer.Write(String.Join("\n", _messages));
    }
  }
}
