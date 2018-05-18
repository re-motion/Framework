using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization
{
  /// <summary>
  /// Adds logic for unescaping the <see cref="ObjectIDStringSerializer"/>.<see cref="ObjectIDStringSerializer.Delimiter"/>
  /// when parsing a serialized <see cref="ObjectID"/>.
  /// </summary>
  /// <remarks>
  /// Note that this logic is only intended to provide backwards compatiblity for serialized <see cref="ObjectID"/>s of type <see cref="String"/> 
  /// to <see cref="ObjectIDStringSerializer"/>.
  /// </remarks>
  internal class UnescapingStringObjectIDValueParser : IObjectIDValueParser
  {
    public static readonly UnescapingStringObjectIDValueParser Instance = new UnescapingStringObjectIDValueParser();
    private const string c_escapedDelimiter = "&pipe;";
    private const string c_escapedDelimiterPlaceholder = "&amp;pipe;";

    private UnescapingStringObjectIDValueParser ()
    {
    }

    public bool TryParse (string stringValue, out object resultValue)
    {
      ArgumentUtility.CheckNotNull ("stringValue", stringValue);

      if (StringObjectIDValueParser.Instance.TryParse (stringValue, out resultValue))
      {
        resultValue = Unescape ((string) resultValue);
        return true;
      }

      resultValue = null;
      return false;
    }

    private static string Unescape (string value)
    {
      if (value.IndexOf (c_escapedDelimiter) >= 0)
        value = value.Replace (c_escapedDelimiter, ObjectIDStringSerializer.DelimiterAsString);

      if (value.IndexOf (c_escapedDelimiterPlaceholder) >= 0)
        value = value.Replace (c_escapedDelimiterPlaceholder, c_escapedDelimiter);

      return value;
    }
  }
}