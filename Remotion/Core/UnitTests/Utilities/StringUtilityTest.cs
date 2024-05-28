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
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class StringUtilityTest
{
  private enum TestEnum
  {
    Value1
  }

  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeDe;

  private readonly Type _int32 = typeof(int);
  private readonly Type _nullableInt32 = typeof(int?);
  private readonly Type _double = typeof(double);
  private readonly Type _string = typeof(string);
  private readonly Type _object = typeof(object);
  private readonly Type _guid = typeof(Guid);
  private readonly Type _nullableGuid = typeof(Guid?);
  private readonly Type _enum = typeof(TestEnum);
  private readonly Type _nullableEnum = typeof(TestEnum?);
  private readonly Type _dbNull = typeof(DBNull);
  private readonly Type _doubleArray = typeof(double[]);
  private readonly Type _stringArray = typeof(string[]);

  [SetUp]
  public void SetUp ()
  {
    _cultureEnUs = new CultureInfo("en-US");
    _cultureDeDe = new CultureInfo("de-DE");

    _cultureBackup = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

    StubStringUtility.ClearCache();
  }

  [TearDown]
  public void TearDown ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureBackup;
  }

  [Test]
	public void NullToEmpty ()
	{
    Assert.That(StringUtility.NullToEmpty(null), Is.EqualTo(string.Empty));
    Assert.That(StringUtility.NullToEmpty("1"), Is.EqualTo("1"));
	}

  [Test]
  public void IsNullOrEmpty ()
  {
    Assert.That(StringUtility.IsNullOrEmpty(null), Is.EqualTo(true));
    Assert.That(StringUtility.IsNullOrEmpty(string.Empty), Is.EqualTo(true));
    Assert.That(StringUtility.IsNullOrEmpty(" "), Is.EqualTo(false));
  }

  [Test]
  public void AreEqual ()
  {
    Assert.That(StringUtility.AreEqual("test1", "test1", false), Is.EqualTo(true));
    Assert.That(StringUtility.AreEqual("test1", "test1", true), Is.EqualTo(true));
    Assert.That(StringUtility.AreEqual("test1", "TEST1", false), Is.EqualTo(false));
    Assert.That(StringUtility.AreEqual("test1", "TEST1", true), Is.EqualTo(true));
    Assert.That(StringUtility.AreEqual("täst1", "TÄST1", false), Is.EqualTo(false));
    Assert.That(StringUtility.AreEqual("täst1", "TÄST1", true), Is.EqualTo(true));
  }

  [Test]
  public void CanParseInt32 ()
  {
    Assert.That(StringUtility.CanParse(_int32), Is.True);
  }

  [Test]
  public void CanParseNullableInt32 ()
  {
    Assert.That(StringUtility.CanParse(_nullableInt32), Is.True);
  }

  [Test]
  public void CanParseEnum ()
  {
    Assert.That(StringUtility.CanParse(_enum), Is.True);
  }

  [Test]
  public void CanParseNullableEnum ()
  {
    Assert.That(StringUtility.CanParse(_nullableEnum), Is.True);
  }

  [Test]
  public void GetParseMethodForInt32 ()
  {
    MethodInfo parseMethod = StubStringUtility.GetParseMethod(_int32, true);
    Assert.That(parseMethod, Is.Not.Null);
    Assert.That(parseMethod.Name, Is.EqualTo("Parse"));
    Assert.That(parseMethod.GetParameters().Length, Is.EqualTo(2));
    Assert.That(parseMethod.GetParameters()[0].ParameterType, Is.EqualTo(typeof(string)));
    Assert.That(parseMethod.GetParameters()[1].ParameterType, Is.EqualTo(typeof(IFormatProvider)));
    Assert.That(parseMethod.ReturnType, Is.EqualTo(typeof(int)));
    Assert.That(parseMethod.IsPublic, Is.True);
    Assert.That(parseMethod.IsStatic, Is.True);
  }

  [Test]
  public void GetParseMethodFromTypeForInt32 ()
  {
    MethodInfo parseMethod = StubStringUtility.GetParseMethodFromType(_int32);
    Assert.That(parseMethod, Is.Not.Null);
    Assert.That(parseMethod.Name, Is.EqualTo("Parse"));
    Assert.That(parseMethod.GetParameters().Length, Is.EqualTo(1));
    Assert.That(parseMethod.GetParameters()[0].ParameterType, Is.EqualTo(typeof(string)));
    Assert.That(parseMethod.ReturnType, Is.EqualTo(typeof(int)));
    Assert.That(parseMethod.IsPublic, Is.True);
    Assert.That(parseMethod.IsStatic, Is.True);
  }

  [Test]
  public void GetParseMethodWithFormatProviderFromTypeForInt32 ()
  {
    MethodInfo parseMethod = StubStringUtility.GetParseMethodWithFormatProviderFromType(_int32);
    Assert.That(parseMethod, Is.Not.Null);
    Assert.That(parseMethod.Name, Is.EqualTo("Parse"));
    Assert.That(parseMethod.GetParameters().Length, Is.EqualTo(2));
    Assert.That(parseMethod.GetParameters()[0].ParameterType, Is.EqualTo(typeof(string)));
    Assert.That(parseMethod.GetParameters()[1].ParameterType, Is.EqualTo(typeof(IFormatProvider)));
    Assert.That(parseMethod.ReturnType, Is.EqualTo(typeof(int)));
    Assert.That(parseMethod.IsPublic, Is.True);
    Assert.That(parseMethod.IsStatic, Is.True);
  }

  [Test]
  public void CanParseObject ()
  {
    Assert.That(StringUtility.CanParse(_object), Is.False);
  }

  [Test]
  public void GetParseMethodForObjectWithException ()
  {
    Assert.That(
        () => StubStringUtility.GetParseMethod(_object, true),
        Throws.InstanceOf<ParseException>());
  }

  [Test]
  public void GetParseMethodForObjectWithoutException ()
  {
    Assert.That(StubStringUtility.GetParseMethod(_object, false), Is.Null);
  }

  [Test]
  public void GetParseMethodFromTypeForObject ()
  {
    Assert.That(StubStringUtility.GetParseMethodFromType(_object), Is.Null);
  }

  [Test]
  public void GetParseMethodWithFormatProviderFromTypeForObject ()
  {
    Assert.That(StubStringUtility.GetParseMethodWithFormatProviderFromType(_object), Is.Null);
  }

  [Test]
  public void ParseInt32 ()
  {
    object value = StringUtility.Parse(_int32, "1", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_int32));
    Assert.That(value, Is.EqualTo(1));
  }

  [Test]
  public void ParseInt32WithEmpty ()
  {
    Assert.That(
        () => StringUtility.Parse(_int32, string.Empty, CultureInfo.InvariantCulture),
        Throws.InstanceOf<ParseException>());
  }

  [Test]
  public void ParseInt32WithNull ()
  {
    Assert.That(
        () => StringUtility.Parse(_int32, null, CultureInfo.InvariantCulture),
        Throws.InstanceOf<ParseException>());
  }

  [Test]
  public void ParseNullableInt32 ()
  {
    object value = StringUtility.Parse(_nullableInt32, "1", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_int32));
    Assert.That(value, Is.EqualTo(1));
  }

  [Test]
  public void ParseNullableInt32WithEmpty ()
  {
    Assert.That(StringUtility.Parse(_nullableInt32, string.Empty, CultureInfo.InvariantCulture), Is.Null);
  }

  [Test]
  public void ParseNullableInt32WithNull ()
  {
    Assert.That(StringUtility.Parse(_nullableInt32, null, CultureInfo.InvariantCulture), Is.Null);
  }

  [Test]
  public void ParseEnum ()
  {
    object value = StringUtility.Parse(_enum, "Value1", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_enum));
    Assert.That(value, Is.EqualTo(TestEnum.Value1));
  }

  [Test]
  public void ParseEnumWithEmpty ()
  {
    Assert.That(
        () => StringUtility.Parse(_enum, string.Empty, CultureInfo.InvariantCulture),
        Throws.InstanceOf<ParseException>()
            .With.Message.EqualTo(" is not a valid value for TestEnum."));
  }

  [Test]
  public void ParseEnumWithNull ()
  {
    Assert.That(
        () => StringUtility.Parse(_enum, null, CultureInfo.InvariantCulture),
        Throws.InstanceOf<ParseException>()
            .With.Message.EqualTo(" is not a valid value for TestEnum."));
  }

  [Test]
  public void ParseNullableEnum ()
  {
    object value = StringUtility.Parse(_nullableEnum, "Value1", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_enum));
    Assert.That(value, Is.EqualTo(TestEnum.Value1));
  }

  [Test]
  public void ParseNullableEnumWithEmpty ()
  {
    Assert.That(StringUtility.Parse(_nullableEnum, string.Empty, CultureInfo.InvariantCulture), Is.Null);
  }

  [Test]
  public void ParseNullableEnumWithNull ()
  {
    Assert.That(StringUtility.Parse(_nullableEnum, null, CultureInfo.InvariantCulture), Is.Null);
  }

  [Test]
  public void ParseDoubleWithCultureInvariant ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse(_double, "4,321.123", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_double));
    Assert.That(value, Is.EqualTo(4321.123));
  }

  [Test]
  public void ParseDoubleWithCultureEnUs ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse(_double, "4,321.123", _cultureEnUs);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_double));
    Assert.That(value, Is.EqualTo(4321.123));
  }

  [Test]
  public void ParseDoubleEnUsWithCultureDeDe ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    Assert.That(
        () => StringUtility.Parse(_double, "4,321.123", _cultureDeDe),
        Throws.InstanceOf<ParseException>());
  }

  [Test]
  public void ParseDoubleWithCultureDeDe ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtility.Parse(_double, "4.321,123", _cultureDeDe);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_double));
    Assert.That(value, Is.EqualTo(4321.123));
  }

  [Test]
  public void ParseDoubleDeDeWithCultureEnUs ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    Assert.That(
        () => StringUtility.Parse(_double, "4.321,123", _cultureEnUs),
        Throws.InstanceOf<ParseException>());
  }

  [Test]
  [Ignore(@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureInvariant ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse(_doubleArray, @"6\,543.123,5\,432.123,4\,321.123", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_doubleArray));
    double[] values = (double[])value;
    Assert.That(values.Length, Is.EqualTo(3));
    Assert.That(values[0], Is.EqualTo(6543.123));
    Assert.That(values[1], Is.EqualTo(5432.123));
    Assert.That(values[2], Is.EqualTo(4321.123));
  }

  [Test]
  public void ParseDoubleArrayWithCultureInvariantNoThousands ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse(_doubleArray, @"6543.123,5432.123,4321.123", CultureInfo.InvariantCulture);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_doubleArray));
    double[] values = (double[])value;
    Assert.That(values.Length, Is.EqualTo(3));
    Assert.That(values[0], Is.EqualTo(6543.123));
    Assert.That(values[1], Is.EqualTo(5432.123));
    Assert.That(values[2], Is.EqualTo(4321.123));
  }

  [Test]
  [Ignore(@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureEnUs ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse(_doubleArray, @"6\,543.123,5\,432.123,4\,321.123", _cultureEnUs);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_doubleArray));
    double[] values = (double[])value;
    Assert.That(values.Length, Is.EqualTo(3));
    Assert.That(values[0], Is.EqualTo(6543.123));
    Assert.That(values[1], Is.EqualTo(5432.123));
    Assert.That(values[2], Is.EqualTo(4321.123));
  }

  [Test]
  public void ParseDoubleArrayWithCultureEnUsNoThousands ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    object value = StringUtility.Parse(_doubleArray, @"6543.123,5432.123,4321.123", _cultureEnUs);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_doubleArray));
    double[] values = (double[])value;
    Assert.That(values.Length, Is.EqualTo(3));
    Assert.That(values[0], Is.EqualTo(6543.123));
    Assert.That(values[1], Is.EqualTo(5432.123));
    Assert.That(values[2], Is.EqualTo(4321.123));
  }

  [Test]
  [Ignore(@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureDeDe ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtility.Parse(_doubleArray, @"6.543\,123,5.432\,123,4.321\,123", _cultureDeDe);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_doubleArray));
    double[] values = (double[])value;
    Assert.That(values.Length, Is.EqualTo(3));
    Assert.That(values[0], Is.EqualTo(6543.123));
    Assert.That(values[1], Is.EqualTo(5432.123));
    Assert.That(values[2], Is.EqualTo(4321.123));
  }

  [Test]
  public void ParseStringArray ()
  {
    object value = StringUtility.Parse(_stringArray, "\"a\",\"b\",\"c\",\"d\"", null);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_stringArray));
    string[] values = (string[])value;
    Assert.That(values.Length, Is.EqualTo(4));
    Assert.That(values[0], Is.EqualTo("a"));
    Assert.That(values[1], Is.EqualTo("b"));
    Assert.That(values[2], Is.EqualTo("c"));
    Assert.That(values[3], Is.EqualTo("d"));
  }

  [Test]
  public void ParseArrayOfDoubleArrays ()
  {
    Assert.That(
        () => StringUtility.Parse(typeof(double[][]), "1,2,3", null),
        Throws.InstanceOf<ParseException>());
  }

  [Test]
  public void CanParseDoubleArray ()
  {
    Assert.That(StringUtility.CanParse(_doubleArray), Is.True);
  }

  [Test]
  public void CanParseArrayDoubleArray ()
  {
    Assert.That(StringUtility.CanParse(typeof(double[][])), Is.False);
  }

  [Test]
  public void CanParseString ()
  {
    Assert.That(StringUtility.CanParse(_string), Is.True);
  }

  [Test]
  public void CanParseDBNull ()
  {
    Assert.That(StringUtility.CanParse(_dbNull), Is.True);
  }

  [Test]
  public void ParseDBNull ()
  {
    object value = StringUtility.Parse(_dbNull, DBNull.Value.ToString(), null);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_dbNull));
    Assert.That(value, Is.EqualTo(DBNull.Value));
  }

  [Test]
  public void CanParseGuid ()
  {
    Assert.That(StringUtility.CanParse(_guid), Is.True);
  }

  [Test]
  public void ParseGuid ()
  {
    Guid guid = Guid.NewGuid();
    object value = StringUtility.Parse(_guid, guid.ToString(), null);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_guid));
    Assert.That(value, Is.EqualTo(guid));
  }

  [Test]
  public void ParseGuidWithEmpty ()
  {
    Assert.That(
        () => StringUtility.Parse(_guid, string.Empty, CultureInfo.InvariantCulture),
        Throws.InstanceOf<FormatException>());
  }

  [Test]
  public void ParseGuidWithNull ()
  {
    Assert.That(
        () => StringUtility.Parse(_guid, null, CultureInfo.InvariantCulture),
        Throws.InstanceOf<ArgumentNullException>());
  }

  [Test]
  public void ParseEmptyGuid ()
  {
    Guid guid = Guid.Empty;
    object value = StringUtility.Parse(_guid, guid.ToString(), null);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_guid));
    Assert.That(value, Is.EqualTo(guid));
  }

  [Test]
  public void ParseNullableGuid ()
  {
    Guid? guid = Guid.NewGuid();
    object value = StringUtility.Parse(_nullableGuid, guid.ToString(), null);
    Assert.That(value, Is.Not.Null);
    Assert.That(value.GetType(), Is.EqualTo(_guid));
    Assert.That(value, Is.EqualTo(guid));
  }

  [Test]
  public void ParseNullableGuidWithEmpty ()
  {
    Assert.That(StringUtility.Parse(_nullableGuid, string.Empty, null), Is.Null);
  }

  [Test]
  public void ParseNullableGuidWithNull ()
  {
    Assert.That(StringUtility.Parse(_nullableGuid, null, null), Is.Null);
  }

  [Test]
  public void FormatNull ()
  {
    Assert.That(StringUtility.Format(null, null), Is.EqualTo(string.Empty));
  }

  [Test]
  public void FormatString ()
  {
    const string value = "Hello World!";
    Assert.That(StringUtility.Format(value, null), Is.EqualTo(value));
  }

  [Test]
  public void FormatDoubleWithCultureEnUs ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    Assert.That(StringUtility.Format(4321.123, _cultureEnUs), Is.EqualTo("4321.123"));
  }

  [Test]
  public void FormatDoubleWithCultureDeDe ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    Assert.That(StringUtility.Format(4321.123, _cultureDeDe), Is.EqualTo("4321,123"));
  }

  [Test]
  public void FormatGuid ()
  {
    Guid guid = Guid.Empty;
    Assert.That(StringUtility.Format(guid, null), Is.EqualTo(guid.ToString()));
  }

  [Test]
  public void FormatDoubleArrayWithCultureEnUsNoThousands ()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeDe;
    double[] values = new double[] {6543.123, 5432.123, 4321.123};
    Assert.That(StringUtility.Format(values, _cultureEnUs), Is.EqualTo(@"6543.123,5432.123,4321.123"));
  }


  [Test]
  public void GetFileNameTimestampTest ()
  {
    var dt = new DateTime(2008, 12, 24, 23, 59, 59, 999);
    string result = StringUtility.GetFileNameTimestamp(dt);
    Assert.That(result, Is.EqualTo("2008_12_24__23_59_59_999"));
  }

  [Test]
  public void GetFileNameTimestampNowTest ()
  {
    string result = StringUtility.GetFileNameTimestampNow();
    DateTime dateTimeNow = DateTime.Now;
    DateTime dateTimeResult = DateTime.ParseExact(result, "yyyy_M_d__H_m_s_FFF", CultureInfo.InvariantCulture.NumberFormat);
    Assert.That(dateTimeNow - dateTimeResult, Is.LessThanOrEqualTo(new TimeSpan(0,0,0,0,50)));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithCRLF_SplitsAtCRLF ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("First\r\nSecond\r\nThird"), Is.EqualTo(new[] { "First", "Second", "Third" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithLF_SplitsAtLF ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("First\r\nSecond\nThird"), Is.EqualTo(new[] { "First", "Second", "Third" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithEmptyItemInSequence_ReturnsEmptyItem ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("First\r\n\r\nThird"), Is.EqualTo(new[] { "First", "", "Third" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithLeadingCRLF_BeginsWithEmptyItem ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("First\r\nSecond\r\n").ToArray(), Is.EqualTo(new[] { "First", "Second", "" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithTrailingCRLF_EndsWithEmptyItem ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("\r\nSecond\r\nThird").ToArray(), Is.EqualTo(new[] { "", "Second", "Third" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithMultipleCRLF_ReturnsMultipleEmptyItems ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("First\r\n\r\n\r\nFourth").ToArray(), Is.EqualTo(new[] { "First", "", "", "Fourth" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithString_ReturnsOneItem ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString("First"), Is.EqualTo(new[] { "First" }));
  }

  [Test]
  public void ParseNewLineSeparatedString_WithEmptyString_ReturnsOneEmptyItem ()
  {
    Assert.That(StringUtility.ParseNewLineSeparatedString(""), Is.EqualTo(new[] { "" }));
  }
}

[TestFixture]
public class StringUtility_ParseSeparatedListTest
{
  [Test]
  public void TestParseSeparatedList ()
  {
    // char doubleq = '\"';
    const char singleq = '\'';
    const char backsl = '\\';
    const char comma = ',';
    const string whitespace = " ";

    Check("1", comma, singleq, singleq, backsl, whitespace, true,
           unquoted("1"));
    Check("1,2", comma, singleq, singleq, backsl, whitespace, true,
           unquoted("1"), unquoted("2"));
    Check("'1', '2'", comma, singleq, singleq, backsl, whitespace, true,
           quoted("1"), quoted("2"));
    Check("<1>, <2>", comma, '<', '>', backsl, whitespace, true,
           quoted("1"), quoted("2"));
    Check("a='A', b='B'", comma, singleq, singleq, backsl, whitespace, true,
           unquoted("a='A'"), unquoted("b='B'"));
    Check("a='A', b='B,B\\'B\\''", comma, singleq, singleq, backsl, whitespace, true,
           unquoted("a='A'"), unquoted("b='B,B'B''"));
    Check("a b c = 'd,e' f 'g,h'", comma, singleq, singleq, backsl, whitespace, true,
           unquoted("a b c = 'd,e' f 'g,h'"));
    Check("a <a ,<a,> a, <b", comma, '<', '>', backsl, whitespace, true,
           unquoted("a <a ,<a,> a"), quoted("b"));
  }

  private StringUtility.ParsedItem quoted (string value)
  {
    return new StringUtility.ParsedItem(value, true);
  }

  private StringUtility.ParsedItem unquoted (string value)
  {
    return new StringUtility.ParsedItem(value, false);
  }

  private void Check (
      string value,
      char delimiter, char openingQuote, char closingQuote, char escapingChar, string whitespaceCharacters,
      bool interpretSpecialCharacters,
      params StringUtility.ParsedItem[] expectedItems)
  {
    StringUtility.ParsedItem[] actualItems = StringUtility.ParseSeparatedList(
      value, delimiter, openingQuote, closingQuote, escapingChar, whitespaceCharacters, interpretSpecialCharacters);

    Assert.That(actualItems.Length, Is.EqualTo(expectedItems.Length));
    for (int i = 0; i < expectedItems.Length; ++i)
    {
      Assert.That(actualItems[i].Value, Is.EqualTo(expectedItems[i].Value), string.Format("[{0}].Value", i));
      Assert.That(actualItems[i].IsQuoted, Is.EqualTo(expectedItems[i].IsQuoted), string.Format("[{0}].IsQuoted", i));
    }
  }
}

}
