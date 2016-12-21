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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectIDStringSerialization
{
  [TestFixture]
  public class Int32ObjectIDValueParserTest
  {
    [Test]
    public void TryParse ()
    {
      object resultValue;
      var success = Int32ObjectIDValueParser.Instance.TryParse ("12", out resultValue);

      Assert.That (success, Is.True);
      Assert.That (resultValue, Is.EqualTo (12));
    }

    [Test]
    public void TryParse_EmptyValue ()
    {
      object resultValue;
      var success = Int32ObjectIDValueParser.Instance.TryParse ("", out resultValue);

      Assert.That (success, Is.False);
      Assert.That (resultValue, Is.Null);
    }

    [Test]
    public void TryParse_NonInt ()
    {
      object resultValue;
      var success = Int32ObjectIDValueParser.Instance.TryParse ("a", out resultValue);

      Assert.That (success, Is.False);
      Assert.That (resultValue, Is.Null);
    }
  }
}
