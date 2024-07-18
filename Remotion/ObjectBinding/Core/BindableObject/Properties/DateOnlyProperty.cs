// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
#if NET6_0_OR_GREATER
  public class DateOnlyProperty : DateTimePropertyBase
  {
    public DateOnlyProperty (Parameters parameters)
        : base(parameters)
    {
    }

    public override object? ConvertFromNativePropertyType (object? nativeValue)
    {
      var dateOnly = ArgumentUtility.CheckType<DateOnly?>(nameof(nativeValue), nativeValue);
      if (dateOnly != null)
        return dateOnly.Value.ToDateTime(TimeOnly.MinValue);

      return base.ConvertFromNativePropertyType(nativeValue);
    }

    public override object? ConvertToNativePropertyType (object? publicValue)
    {
      var dateTime = ArgumentUtility.CheckType<DateTime?>(nameof(publicValue), publicValue);
      if (dateTime != null)
        return DateOnly.FromDateTime(dateTime.Value);

      return base.ConvertToNativePropertyType(publicValue);
    }

    public override DateTimeType Type
    {
      get { return DateTimeType.Date; }
    }
  }
#endif
}
