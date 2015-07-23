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
using System.Globalization;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation
{
  /// <summary>
  /// Default implementation of the <seealso cref="IDateTimeFormatter"/> interface.
  /// </summary>
  [ImplementationFor (typeof (IDateTimeFormatter), Lifetime = LifetimeKind.Singleton)]
  public class DateTimeFormatter : IDateTimeFormatter
  {
    public DateTimeFormatter ()
    {
    }

    public virtual string FormatDateValue (DateTime dateValue)
    {
      return dateValue.ToString ("d");
    }

    public virtual string FormatTimeValue (DateTime timeValue, bool showSeconds)
    {
      if (showSeconds)
      {
        //  T: hh, mm, ss
        return timeValue.ToString ("T");
      }
      else
      {
        //  T: hh, mm
        return timeValue.ToString ("t");
      }
    }

    public bool Is12HourTimeFormat ()
    {
      return !string.IsNullOrEmpty (CultureInfo.CurrentCulture.DateTimeFormat.AMDesignator);
    }

    public int GetDateMaxLength ()
    {
      var maxDate = FormatDateValue (new DateTime (2000, 12, 31));
      return maxDate.Length;
    }

    public int GetTimeMaxLength (bool showSeconds)
    {
      var maxTime = FormatTimeValue (new DateTime (1, 1, 1, 23, 30, 30), showSeconds);
      return maxTime.Length;
    }
  }
}