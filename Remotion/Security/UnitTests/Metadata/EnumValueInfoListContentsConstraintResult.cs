using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Metadata
{
  public class EnumValueInfoListContentsConstraintResult: ConstraintResult
  {
    private readonly string _expectedName;
    private readonly object _actual;

    public EnumValueInfoListContentsConstraintResult (IConstraint constraint, object actualValue, string expectedName, bool isSuccess)
        : base(constraint, actualValue, isSuccess)
    {
      _actual = actualValue;
      _expectedName = expectedName;
    }

    public override void WriteMessageTo (MessageWriter writer)
    {
      var message = new StringBuilder();
      message.Append ("Expected: ");
      message.Append (_expectedName);
      message.Append ("\t but was: ");
      message.Append (String.Join (", ", ExtractNames (((IList<EnumValueInfo>) _actual)).ToArray()));

      writer.Write (message.ToString());
    }

    private List<string> ExtractNames (IList<EnumValueInfo> list)
    {
      var actualAsEnumValueInfoList = _actual as IList<EnumValueInfo>;
      if (actualAsEnumValueInfoList == null)
        return null;

      List<string> actualNames = new List<string> ();
      foreach (EnumValueInfo value in list)
        actualNames.Add (value.Name);

      return actualNames;
    }
  }
}