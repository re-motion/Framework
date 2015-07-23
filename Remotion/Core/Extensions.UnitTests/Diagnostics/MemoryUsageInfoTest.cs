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
using Remotion.Diagnostics;

namespace Remotion.Extensions.UnitTests.Diagnostics
{
  [TestFixture]
  public class MemoryUsageInfoTest
  {
    [Test]
    public new void ToString ()
    {
      MemoryUsageInfo usage = new MemoryUsageInfo("X", new ByteValue(123), new ByteValue(345), new ByteValue(789));
      Assert.That (
          usage.ToString(),
          Is.EqualTo (
              string.Format (
                  "X:{0}\tWorking set: {1}{0}\tManaged memory before collect: {2}{0}\tAfter collect: {3}",
                  Environment.NewLine,
                  new ByteValue (123),
                  new ByteValue (345),
                  new ByteValue (789)
                  )));
    }

    [Test]
    public void ToCSVString ()
    {
      MemoryUsageInfo usage = new MemoryUsageInfo ("X", new ByteValue (123), new ByteValue (345), new ByteValue (789000000));
      Assert.That (usage.ToCSVString (), Is.EqualTo ("123;345;789000000"));
    }

    [Test]
    public void ToDifferenceString ()
    {
      MemoryUsageInfo comparison = new MemoryUsageInfo ("Y", new ByteValue (999), new ByteValue (999), new ByteValue (999));
      MemoryUsageInfo usage = new MemoryUsageInfo ("X", new ByteValue (123), new ByteValue (345), new ByteValue (789));
      Assert.That (
          usage.ToDifferenceString(comparison),
          Is.EqualTo (
              string.Format (
                  "Compared to Y:{0}\tWorking set: {1}{0}\tManaged memory before collect: {2}{0}\tAfter collect: {3}",
                  Environment.NewLine,
                  (new ByteValue (123) - new ByteValue (999)).ToDifferenceString(),
                  (new ByteValue (345) - new ByteValue (999)).ToDifferenceString(),
                  (new ByteValue (789) - new ByteValue (999)).ToDifferenceString ()
                  )));
    }
  }
}
