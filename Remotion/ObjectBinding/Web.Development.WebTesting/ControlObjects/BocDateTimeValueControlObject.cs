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
using Coypu;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the <see cref="T:Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValue"/> control.
  /// </summary>
  public class BocDateTimeValueControlObject : BocControlObject, IControlObjectWithFormElements
  {
    private readonly bool _hasTimeField;

    public BocDateTimeValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
      _hasTimeField = Scope[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueHasTimeField] == "true";
    }

    /// <summary>
    /// Returns whether the control has a time field as well.
    /// </summary>
    public bool HasTimeField ()
    {
      return _hasTimeField;
    }

    /// <summary>
    /// Returns the current <see cref="DateTime"/> respresented by the control or null if an invalid <see cref="DateTime"/> is displayed.
    /// </summary>
    public DateTime GetDateTime ()
    {
      var dateTimeString = GetDateTimeAsString();
      return DateTime.Parse (dateTimeString);
    }

    /// <summary>
    /// Returns the current date and time (string) values separated by space.
    /// </summary>
    public string GetDateTimeAsString ()
    {
      string dateTimeString;

      if (IsReadOnly())
      {
        dateTimeString = Scope.FindCss ("span:nth-child(1)").Text;
        if (_hasTimeField)
          dateTimeString += " " + Scope.FindCss ("span:nth-child(2)").Text;
      }
      else
      {
        dateTimeString = GetDateScope().Value;
        if (_hasTimeField)
          dateTimeString += " " + GetTimeScope().Value;
      }

      return dateTimeString;
    }

    /// <summary>
    /// Sets the date component of the control to <paramref name="newDate"/>.
    /// </summary>
    public UnspecifiedPageObject SetDate (DateTime newDate, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var newDateString = newDate.ToShortDateString();
      return SetDate (newDateString, actionOptions);
    }

    /// <summary>
    /// Sets the date component of the control to <paramref name="newDateString"/>.
    /// </summary>
    public UnspecifiedPageObject SetDate ([NotNull] string newDateString, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("newDateString", newDateString);

      var dateScope = GetDateScope();

      var actualActionOptions = MergeWithDefaultActionOptions (dateScope, actionOptions);
      new FillWithAction (this, dateScope, newDateString, FinishInput.WithTab).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Sets the time component of the control to <paramref name="newTime"/>.
    /// </summary>
    public UnspecifiedPageObject SetTime (TimeSpan newTime, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var newTimeAsDateTime = DateTime.MinValue.Add (newTime);

      string newTimeString;

      var timeScope = GetTimeScope();
      if (timeScope[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeFieldHasSeconds] == "true")
        newTimeString = newTimeAsDateTime.ToLongTimeString();
      else
        newTimeString = newTimeAsDateTime.ToShortTimeString();

      return SetTime (newTimeString, actionOptions);
    }

    /// <summary>
    /// Sets the time component of the control to <paramref name="newTimeString"/>.
    /// </summary>
    public UnspecifiedPageObject SetTime ([NotNull] string newTimeString, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ArgumentUtility.CheckNotNull ("newTimeString", newTimeString);

      var timeScope = GetTimeScope();

      var actualActionOptions = MergeWithDefaultActionOptions (timeScope, actionOptions);
      new FillWithAction (this, timeScope, newTimeString, FinishInput.WithTab).Execute (actualActionOptions);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Sets the date component and the time component of the control to <paramref name="newDateTime"/>.
    /// </summary>
    public UnspecifiedPageObject SetDateTime (DateTime newDateTime, [CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      SetDate (newDateTime, actionOptions);
      if (_hasTimeField)
        SetTime (newDateTime.TimeOfDay, actionOptions);
      return UnspecifiedPage();
    }

    /// <summary>
    /// See <see cref="IControlObjectWithFormElements.GetFormElementNames"/>. Returns the input[type=text] (date value) as first element, the
    /// input[type=text] (time value) as second element. Make sure to use the time value only if <see cref="HasTimeField"/> returns
    /// <see langword="true" />.
    /// </summary>
    ICollection<string> IControlObjectWithFormElements.GetFormElementNames ()
    {
      var htmlID = GetHtmlID();
      return new[] { string.Format ("{0}_DateValue", htmlID), string.Format ("{0}_TimeValue", htmlID) };
    }

    private ElementScope GetDateScope ()
    {
      return Scope.FindTagWithAttribute ("input", DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueDateField, "true");
    }

    private ElementScope GetTimeScope ()
    {
      return Scope.FindTagWithAttribute ("input", DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeField, "true");
    }
  }
}