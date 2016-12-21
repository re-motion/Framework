// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework.Constraints;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Metadata
{
  public class EnumValueInfoListContentsConstraint : Constraint
  {
    private string _expectedName;

    public EnumValueInfoListContentsConstraint (string expectedName)
    {
      _expectedName = expectedName;
    }

    public override bool Matches (object actual)
    {
      base.actual = actual;
      var actualAsEnumValueInfoList = actual as List<EnumValueInfo>;
      if (actualAsEnumValueInfoList != null)
      {
        foreach (var value in actualAsEnumValueInfoList)
        {
          if (string.Equals (value.Name, _expectedName, StringComparison.Ordinal))
            return true;
        }
      }

      return false;
    }

    public override void WriteDescriptionTo (MessageWriter writer)
    {
      throw new NotImplementedException();
    }

    public override void WriteMessageTo (MessageWriter writer)
    {
      var message = new StringBuilder();
      message.Append ("Expected: ");
      message.Append (_expectedName);
      message.Append("\t but was: ");
      message.Append (String.Join(", ", ExtractNames (((IList<EnumValueInfo>) actual)).ToArray()));

      writer.Write (message.ToString());
    }

    private List<string> ExtractNames (IList<EnumValueInfo> list)
    {
      var actualAsEnumValueInfoList = base.actual as IList<EnumValueInfo>;
      if (actualAsEnumValueInfoList == null)
        return null;

      List<string> actualNames = new List<string> ();
      foreach (EnumValueInfo value in list)
        actualNames.Add (value.Name);

      return actualNames;
    }
  }
}