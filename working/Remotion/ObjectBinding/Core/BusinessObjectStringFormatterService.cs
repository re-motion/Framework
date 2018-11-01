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
using System.Collections;
using System.Globalization;
using System.Text;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Default implementation of the <see cref="IBusinessObjectStringFormatterService"/>. This implementation uses the .NET infrastructure 
  /// and the API's provided by the business-object interfaces to convert an <see cref="IBusinessObjectProperty"/>'s value to a <see cref="string"/>.
  /// </summary>
  public class BusinessObjectStringFormatterService : IBusinessObjectStringFormatterService
  {
    /// <summary> 
    ///   Gets the string representation of the value accessed through the specified <see cref="IBusinessObjectProperty"/> 
    ///   from the the passed <see cref="IBusinessObject"/>.
    /// </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> whose property value will be returned. </param>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="format">
    /// The format string passed to the value's <see cref="IFormattable.ToString(string,IFormatProvider)"/> method.
    /// </param>
    /// <returns> 
    ///   The string representation of the property value for the <paramref name="property"/> parameter.  
    /// <list type="table">
    ///   <listheader>
    ///     <term> Property and Property Value </term>
    ///     <description> Return Value </description>
    ///   </listheader>
    ///   <item>
    ///     <term>The <paramref name="property"/> parameter implements the <see cref="IBusinessObjectBooleanProperty"/> interface.</term>
    ///     <description><see cref="IBusinessObjectBooleanProperty.GetDisplayName"/> is evaluated for the value and the resulting string is returned.</description>
    ///   </item>
    ///   <item>
    ///     <term>The <paramref name="property"/> parameter implements the <see cref="IBusinessObjectDateTimeProperty"/> interface.</term>
    ///     <description>
    ///       The value is cast to <see cref="IFormattable"/> and the <see cref="IFormattable.ToString(string,IFormatProvider)"/> method is invoked,
    ///       passing the <paramref name="format"/> string.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>The <paramref name="property"/> parameter implements the <see cref="IBusinessObjectEnumerationProperty"/> interface.</term>
    ///     <description>The value's <see cref="IEnumerationValueInfo.DisplayName"/> is returned.</description>
    ///   </item>
    ///   <item>
    ///     <term>The <paramref name="property"/> parameter implements the <see cref="IBusinessObjectNumericProperty"/> interface.</term>
    ///     <description>
    ///       The value is cast to <see cref="IFormattable"/> and the <see cref="IFormattable.ToString(string,IFormatProvider)"/> method is invoked,
    ///       passing the <paramref name="format"/> string.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>The <paramref name="property"/> parameter implements the <see cref="IBusinessObjectReferenceProperty"/> interface.</term>
    ///     <description> 
    ///       The value's <see cref="IBusinessObjectWithIdentity.DisplayName"/> is returned (including an acess check) 
    ///       if the object implements <see cref="IBusinessObjectWithIdentity"/>, otherwise, <see cref="object.ToString()"/> is used.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>The <paramref name="property"/> parameter implements the <see cref="IBusinessObjectStringProperty"/> interface.</term>
    ///     <description>The value is cast to a string and returned.</description>
    ///   </item>
    ///   <item>
    ///     <term>The value is <see langword="null"/>.</term>
    ///     <description>An empty string is returned.</description>
    ///   </item>
    ///   <item>
    ///     <term> The value is a list. </term>
    ///     <description> 
    ///       The <paramref name="format"/> string is parsed to check whether it begins with <c>lines=&lt;n&gt;</c>, where <c>n</c> is the number of
    ///       items to render or <c>all</c> to render all items in the list. The value is then cast to <see cref="IList"/> and each item is processed
    ///       according to the <paramref name="property"/>'s formatting logic. The resulting strings are concatenated and the total number of lines is 
    ///       appended, if not all items are shown.
    ///     </description>
    ///   </item>
    /// </list>
    /// </returns>
    /// <remarks> 
    ///     Uses the <paramref name="businessObject"/>'s <see cref="IBusinessObject.GetProperty"/> method for accessing the value.
    /// </remarks>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of <paramref name="businessObject"/>'s class. 
    /// </exception>
    public string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("property", property);
      
      if (property.IsList)
      {
        Tuple<int, string> result = GetLineCountAndFormatString (format);
        return GetStringValues (businessObject, property, result.Item2, result.Item1);
      }
      else
      {
        return GetStringValue (businessObject, businessObject.GetProperty (property), property, format);
      }
    }

    private string GetStringValues (IBusinessObject businessObject, IBusinessObjectProperty property, string format, int lines)
    {
      IList list = (IList) businessObject.GetProperty (property);
      if (list == null)
        return string.Empty;

      int count = list.Count;
      StringBuilder sb = new StringBuilder (count*40);
      for (int i = 0;
           i < count && (lines == -1 || i < lines);
           ++i)
      {
        if (i > 0)
          sb.AppendLine ();
        sb.Append (GetStringValue (businessObject, list[i], property, format));
      }

      if (lines != -1 && count > lines)
        sb.Append (" ... [" + count + "]");

      return sb.ToString();
    }

    private Tuple<int, string> GetLineCountAndFormatString (string format)
    {
      Tuple<string, string> formatStrings = GetFormatStrings (format);
      return new Tuple<int, string> (ParseLineCount(formatStrings.Item1), formatStrings.Item2);
    }

    private Tuple<string, string> GetFormatStrings (string format)
    {
      if (format == null)
        return new Tuple<string, string> (null, null);

      const string lineCountPrefix = "lines=";
      if (format.StartsWith (lineCountPrefix))
      {
        string[] formatStrings = format.Split (new char[] {'|'}, 2);
        return new Tuple<string, string> (
            formatStrings[0].Substring (lineCountPrefix.Length), 
            (formatStrings.Length == 2) ? formatStrings[1] : null);
      }

      return new Tuple<string, string> (null, format);
    }

    private int ParseLineCount (string linesString)
    {
      if (linesString == "all")
        return -1;

      int lines;
      if (int.TryParse (linesString, NumberStyles.Integer, CultureInfo.InvariantCulture, out lines))
        return lines;

      return 1;
    }

    private string GetStringValue (IBusinessObject businessObject, object value, IBusinessObjectProperty property, string format)
    {
      if (property is IBusinessObjectBooleanProperty)
        return GetStringValueForBooleanProperty (value, (IBusinessObjectBooleanProperty) property);
      if (property is IBusinessObjectDateTimeProperty)
        return GetStringValueForDateTimeProperty (value, (IBusinessObjectDateTimeProperty) property, format);
      if (property is IBusinessObjectEnumerationProperty)
        return GetStringValueForEnumerationProperty (value, (IBusinessObjectEnumerationProperty) property, businessObject);
      if (property is IBusinessObjectNumericProperty)
        return GetStringValueForNumericProperty (value, format);
      if (property is IBusinessObjectReferenceProperty)
        return GetStringValueForReferenceProperty (value);
      if (property is IBusinessObjectStringProperty)
        return GetStringValueForStringProperty (value);

      return (value != null) ? value.ToString() : string.Empty;
    }

    private string GetStringValueForDateTimeProperty (object value, IBusinessObjectDateTimeProperty property, string format)
    {
      if (string.IsNullOrEmpty (format))
      {
        if (property.Type == DateTimeType.Date)
          format = "d";
        else
          format = "g";
      }

      return GetStringValueForFormattableValue ((IFormattable) value, format);
    }

    private string GetStringValueForBooleanProperty (object value, IBusinessObjectBooleanProperty property)
    {
      if (value is bool)
        return property.GetDisplayName ((bool) value);
      else
        return string.Empty;
    }

    private string GetStringValueForEnumerationProperty (object value, IBusinessObjectEnumerationProperty property, IBusinessObject businessObject)
    {
      IEnumerationValueInfo enumValueInfo = property.GetValueInfoByValue (value, businessObject);
      if (enumValueInfo == null)
        return string.Empty;
      return enumValueInfo.DisplayName;
    }

    private string GetStringValueForNumericProperty (object value, string format)
    {
      return GetStringValueForFormattableValue ((IFormattable) value, format);
    }

    private string GetStringValueForReferenceProperty (object value)
    {
      if (value == null)
        return string.Empty;

      // When using IBusinessObjectWithIdentity, the DisplayName property can be checked before accessing the string value.
      // This check is not possible with IBusinessObject. Here, the implementation must take care to check the string value before returning it.

      if (value is IBusinessObjectWithIdentity)
        return ((IBusinessObjectWithIdentity) value).GetAccessibleDisplayName();

      return value.ToString();
    }

    private string GetStringValueForStringProperty (object value)
    {
      return (string) value ?? string.Empty;
    }

    private string GetStringValueForFormattableValue (IFormattable value, string format)
    {
      if (value == null)
        return string.Empty;
      return value.ToString (format, null);
    }
  }
}
