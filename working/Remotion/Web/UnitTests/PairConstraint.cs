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
using System.Web.UI;
using NUnit.Framework.Constraints;

namespace Remotion.Web.UnitTests
{
  public class PairConstraint : EqualConstraint
  {
    private readonly Pair _expected;
    private Pair _actual;

    public PairConstraint (Pair expected)
        : base(expected)
    {
      _expected = expected;
    }

    public override bool Matches (object actual)
    {
      _actual = actual as Pair;

      if (base.Matches (actual))
        return true;

      if (actual == null)
        return _expected == null;

      return object.Equals (_expected.First, ((Pair) actual).First) && object.Equals (_expected.Second, ((Pair) actual).Second);
    }

    public override void WriteMessageTo (MessageWriter writer)
    {
      if (_expected == null || _actual == null)
      {
        base.WriteMessageTo (writer);
      }
      else
      {
        writer.DisplayStringDifferences (
            string.Format ("{{ {0} , {1} }}", _expected.First, _expected.Second),
            string.Format ("{{ {0} , {1} }}", _actual.First, _actual.Second),
            -1,
            false,
            false);
      }
    }
  }
}
