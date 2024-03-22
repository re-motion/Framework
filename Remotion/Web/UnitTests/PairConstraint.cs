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

    public PairConstraint (Pair expected)
        : base(expected)
    {
      _expected = expected;
    }

    public override ConstraintResult ApplyTo<TActual> (TActual actual)
    {
      var actualPair = actual as Pair;
      var baseResult = base.ApplyTo(actual);
      if (baseResult.IsSuccess)
        return baseResult;

      var isSuccess = Matches(actualPair);
      return new PairConstraintResult(this, actualPair, _expected, isSuccess);
    }

    private bool Matches (Pair actual)
    {
      if (actual == null)
        return _expected == null;

      return object.Equals(_expected.First, actual.First)
             && object.Equals(_expected.Second, actual.Second);
    }
  }
}
