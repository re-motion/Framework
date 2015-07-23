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
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocDateTimeValueImplementation
{
  [TestFixture]
  public class DateTimeFormatterTest
  {
    [Test]
    public void FormatDateValue_ReturnsDate ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.FormatDateValue (new DateTime (2013, 06, 20, 14, 30, 40)), Is.EqualTo ("6/20/2013"));
      }
    }

    [Test]
    public void FormatTimeValue_WithShowSecondsTrue_ReturnsTimeWithSeconds ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.FormatTimeValue (new DateTime (2013, 06, 20, 14, 30, 40), true), Is.EqualTo ("2:30:40 PM"));
      }
    }

    [Test]
    public void FormatTimeValue_WithShowSecondsFalse_ReturnsTimeWithoutSeconds ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.FormatTimeValue (new DateTime (2013, 06, 20, 5, 30, 40), false), Is.EqualTo ("5:30 AM"));
      }
    }

    [Test]
    public void Is12HourTimeFormat_WithUsCulture_ReturnsTrue ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.Is12HourTimeFormat(), Is.True);
      }
    }

    [Test]
    public void Is12HourTimeFormat_WithDeCulture_ReturnsFalse ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("de-AT"))
      {
        Assert.That (formatter.Is12HourTimeFormat(), Is.False);
      }
    }

    [Test]
    public void GetDateMaxLength_ReturnsLengthForDoubleSpacedDigits ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.GetDateMaxLength(), Is.EqualTo (10));
      }
    }


    [Test]
    public void GetTimeMaxLength_WithShowSecondsTrue_ReturnsLengthForDoubleSpacedDigits_IncludingTheSeconds ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.GetTimeMaxLength (true), Is.EqualTo (11));
      }
    }

    [Test]
    public void GetTimeMaxLength_WithShowSecondsFalse_ReturnsLengthForDoubleSpacedDigits_ExcludingTheSeconds ()
    {
      var formatter = new DateTimeFormatter();

      using (new CultureScope ("en-US"))
      {
        Assert.That (formatter.GetTimeMaxLength (false), Is.EqualTo (8));
      }
    }
  }
}