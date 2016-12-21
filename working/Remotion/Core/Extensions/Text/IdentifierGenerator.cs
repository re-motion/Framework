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
using System.Text;

namespace Remotion.Text
{

public class IdentifierGenerator: ICloneable
{
  private static IdentifierGenerator s_cStyle = null;
  private static IdentifierGenerator s_htmlStyle = null;
  private static IdentifierGenerator s_xmlStyle = null;

  /// <summary>
  ///   Returns an identifier generator for C-style identifiers.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If you want to generate unique identifiers, use the <see cref="Clone"/> method to create a new <c>IdentifierGenerator</c>.
  ///   </para><para>
  ///     C-style identifiers start with either an english letter or an underscore. All other characters can
  ///     be english letters, digits or underscores. C-style identifiers are case-sensitive.
  ///   </para><para>
  ///     C-style identifiers can be used for the programming languages C, C++, C# and Java, among others. 
  ///     (Note that C# allows a number of unicode characters too, including languages-specific letters.)
  ///   </para>
  /// </remarks>
  public static IdentifierGenerator CStyle 
  {
    get 
    {
      if (s_cStyle == null)
      {
        IdentifierGenerator idgen = new IdentifierGenerator ();
        idgen._isTemplate = true;

        idgen.AllowEnglishLetters = true;
        idgen.AllowDigits = true;
        idgen.AllowAdditionalCharacters = "_";
        idgen.DefaultReplaceString = "_";

        idgen.TreatFirstCharacterSpecial = true;
        idgen.AllowFirstCharacterEnglishLetters = true;
        idgen.AllowAdditionalCharacters = "_";
        idgen.DefaultFirstCharacterReplaceString = "_";

        idgen.IsCaseSensitive = true;
        idgen.UniqueSeparator = "_";

        s_cStyle = idgen;
      }
      return s_cStyle;
    }
  }

  /// <summary>
  ///   Returns an identifier generator for HTML ID tokens.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If you want to generate unique identifiers, use the <see cref="Clone"/> method to create a new <c>IdentifierGenerator</c>.
  ///   </para><para>
  ///     See http://www.w3.org/TR/html4/types.html#type-id
  ///   </para>
  /// </remarks>
  public static IdentifierGenerator HtmlStyle 
  {
    get 
    {
      if (s_htmlStyle == null)
      {
        IdentifierGenerator idgen = new IdentifierGenerator ();
        idgen._isTemplate = true;

        idgen.AllowEnglishLetters = true;
        idgen.AllowDigits = true;
        idgen.AllowAdditionalCharacters = "_:-.";
        idgen.DefaultReplaceString = "_";

        idgen.TreatFirstCharacterSpecial = true;
        idgen.AllowFirstCharacterEnglishLetters = true;
        idgen.DefaultFirstCharacterReplaceString = "";

        idgen.IsCaseSensitive = false;
        idgen.UniqueSeparator = "_";
        
        s_htmlStyle = idgen;
      }
      return s_htmlStyle;
    }
  }

  /// <summary>
  ///   Returns an identifier generator for XML ID tokens.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     If you want to generate unique identifiers, use the <see cref="Clone"/> method to create a new <c>IdentifierGenerator</c>.
  ///   </para><para>
  ///     See http://www.w3.org/TR/REC-xml#id
  ///   </para>
  /// </remarks>
  public static IdentifierGenerator XmlStyle 
  {
    get 
    {
      if (s_xmlStyle == null)
      {
        IdentifierGenerator idgen = new IdentifierGenerator ();
        idgen._isTemplate = true;

        idgen.AllowEnglishLetters = true;
        idgen.AllowDigits = true;
        idgen.AllowAdditionalCharacters = "_:-.";
        idgen.DefaultReplaceString = "_";

        idgen.TreatFirstCharacterSpecial = true;
        idgen.AllowFirstCharacterEnglishLetters = true;
        idgen.AllowAdditionalCharacters = "_:";
        idgen.DefaultFirstCharacterReplaceString = "_";

        idgen.IsCaseSensitive = false; // TODO: is this correct??
        idgen.UniqueSeparator = "_";

        s_xmlStyle = idgen;
      }
      return s_xmlStyle;
    }
  }

  /// <summary> Hashtable&lt;object uniqueObject, string uniqueIdentifier&gt; </summary>
  private Hashtable _uniqueIdentfiersByObject; 
  /// <summary> Hashtable&lt;string uniqueIdentifier, null&gt; </summary>
  private Hashtable _uniqueIdentifiers;
  /// <summary> Specifies that the IdentifierGenerator must be cloned and cannot be used directly. </summary>
  private bool _isTemplate = false;

  private string _uniqueSeparator = null;

  private bool _allowEnglishLetters = false;
  private bool _allowLanguageSpecificLetters = false;
  private bool _allowDigits = false;
  private string _allowAdditionalCharacters = null;
  private string _defaultReplaceString = null;

  private bool _treatFirstCharacterSpecial = false;
  private bool _allowFirstCharacterEnglishLetters = false;
  private bool _allowFirstCharacterLanguageSpecificLetters = false;
  private bool _allowFirstCharacterDigits = false;
  private string _allowFirstCharacterAdditionalCharacters = null;
  private string _defaultFirstCharacterReplaceString = null;

  private bool _isCaseSensitive = true;
  private bool _useCaseSensitiveNames = true;

  /// <summary> IDictionary&lt;char, string&gt; </summary>
  private IDictionary _specificReplaceStrings = null;

  public void AddSpecificReplaceString (char characterToReplace, string stringToReplaceWith)
  {
    if (_specificReplaceStrings == null)
      _specificReplaceStrings = new Hashtable ();
    _specificReplaceStrings.Add (characterToReplace, stringToReplaceWith);
  }

  public IdentifierGenerator ()
  {
  }

  public string GetValidIdentifier (string str)
  {
    StringBuilder sb = new StringBuilder (str.Length);

    bool allowEnglishLetters;
    bool allowLanguageSpecificLetters;
    bool allowDigits;
    string allowAdditionalCharacters;
    string defaultReplaceString;

    if (_treatFirstCharacterSpecial)
    {
      allowEnglishLetters = _allowFirstCharacterEnglishLetters;
      allowLanguageSpecificLetters = _allowFirstCharacterLanguageSpecificLetters;
      allowDigits = _allowFirstCharacterDigits;
      allowAdditionalCharacters = _allowFirstCharacterAdditionalCharacters;
      defaultReplaceString = _defaultFirstCharacterReplaceString;
    }
    else
    {
      allowEnglishLetters = _allowEnglishLetters;
      allowLanguageSpecificLetters = _allowLanguageSpecificLetters;
      allowDigits = _allowDigits;
      allowAdditionalCharacters = _allowAdditionalCharacters;
      defaultReplaceString = _defaultReplaceString;
    }

    for (int i = 0; i < str.Length; ++i)
    {
      if (_treatFirstCharacterSpecial && i == 1)
      {
        allowEnglishLetters = _allowEnglishLetters;
        allowLanguageSpecificLetters = _allowLanguageSpecificLetters;
        allowDigits = _allowDigits;
        allowAdditionalCharacters = _allowAdditionalCharacters;
        defaultReplaceString = _defaultReplaceString;
      }

      char c = str[i];
      bool isValid = false;

      if (_specificReplaceStrings != null)
      {
        string replaceString = (string) _specificReplaceStrings[c];
        if (replaceString != null)
          isValid = true;
      }

      if (   isValid
          || (   allowLanguageSpecificLetters 
              && char.IsLetter (c))
          || (   ! allowLanguageSpecificLetters 
              && allowEnglishLetters 
              && (    (c >= 'a' && c <= 'z')
                  || (c >= 'A' && c <= 'Z')))
          || (   allowDigits 
              && char.IsDigit (c))
          || (   allowAdditionalCharacters != null 
              && allowAdditionalCharacters.IndexOf (c) >= 0))
      {
        isValid = true;
      }

      if (isValid)
        sb.Append (c);
      else
        sb.Append (defaultReplaceString);
    }

    return sb.ToString();
  }

  public string GetUniqueIdentifier (object uniqueObject, string name)
  {
    if (_isTemplate)
      throw new InvalidOperationException ("This instance of IdentifierGenerator is a template. Use the Clone method to create a new IdentifierGenerator that can be used to create unique identifieres.");

    if (_uniqueIdentifiers == null)
    {
      if (_isCaseSensitive)
        _uniqueIdentifiers = new Hashtable ();
      else
        _uniqueIdentifiers = new Hashtable (StringComparer.CurrentCultureIgnoreCase);

      if (_useCaseSensitiveNames)
        _uniqueIdentfiersByObject = new Hashtable ();
      else
        _uniqueIdentfiersByObject = new Hashtable (StringComparer.CurrentCultureIgnoreCase);
    }
    else
    {
      string uniqueIdentifier = (string) _uniqueIdentfiersByObject[uniqueObject];
      if (uniqueIdentifier != null)
        return uniqueIdentifier;
    }

    string identifier = GetValidIdentifier (name);

    if (_uniqueIdentifiers.Contains(identifier))
    {
      for (int uniqueNum = 1; ; ++uniqueNum)
      {
        string numberedIdentifier = identifier + _uniqueSeparator + uniqueNum.ToString();
        if (! _uniqueIdentifiers.Contains (numberedIdentifier))
        {
          identifier = numberedIdentifier;
          break;
        }
      }
    }

    _uniqueIdentfiersByObject.Add (uniqueObject, identifier);
    _uniqueIdentifiers.Add (identifier, null);
    return identifier;
  }

  public string GetUniqueIdentifier (string uniqueName)
  {
    return GetUniqueIdentifier (uniqueName, uniqueName);
  }

  /// <summary>
  ///   If a unique number is appended to string, <c>UniqueSeparator</c> is inserted between the identifier 
  ///   and the number.
  /// </summary>
  /// <example>
  ///   If <c>UniqueSeparator</c> is an underscore ("_"), an identifier "id" is generated and has to be 
  ///   appended with the number 2 to be unique, the resulting unique identifier is "id_2".
  /// </example>
  public string UniqueSeparator
  {
    get { return _uniqueSeparator; }
    set { _uniqueSeparator = value; }
  }

  /// <summary>
  /// States that all english letters (upper case and lower case) are valid for identifiers, while accented
  /// letters and umlauts are not.
  /// </summary>
  public bool AllowEnglishLetters
  {
    get { return _allowEnglishLetters; }
    set { _allowEnglishLetters = value; }
  }

  /// <summary>
  /// States that all letters (upper case and lower case) are valid for identifiers, including accented
  /// letters and umlauts.
  /// </summary>
  public bool AllowLanguageSpecificLetters
  {
    get { return _allowLanguageSpecificLetters; }
    set { _allowLanguageSpecificLetters = value; }
  }

  /// <summary>
  /// States that numeric digits are valid for identifiers.
  /// </summary>
  public bool AllowDigits
  {
    get { return _allowDigits; }
    set { _allowDigits = value; }
  }

  /// <summary>
  /// Provides a list of characters that are valid as a string.
  /// </summary>
  public string AllowAdditionalCharacters
  {
    get { return _allowAdditionalCharacters; }
    set { _allowAdditionalCharacters = value; }
  }

  /// <summary>
  /// Characters that are not valid and have no special replace string defined are replaced with this value.
  /// </summary>
  public string DefaultReplaceString
  {
    get { return _defaultReplaceString; }
    set { _defaultReplaceString = value; }
  }

  /// <summary>
  /// If this property is true, the special properties for the first character are considered for
  /// the first character instead of the normal properties.
  /// </summary>
  /// <remarks>
  ///   If <c>TreatFirstCharacterSpecial</c> is true,
  ///   <list type="bullets">
  ///     <item><see cref="AllowFirstCharacterEnglishLetters"/></item> is considered instead of <see cref="AllowEnglishLetters"/>
  ///     <item><see cref="AllowFirstCharacterLanguageSpecificLetters"/></item> is considered instead of <see cref="AllowLanguageSpecificLetters"/>
  ///     <item><see cref="AllowFirstCharacterDigits"/></item> is considered instead of <see cref="AllowDigits"/>
  ///     <item><see cref="AllowFirstCharacterAdditionalCharacters"/></item> is considered instead of <see cref="AllowAdditionalCharacters"/>
  ///     <item><see cref="DefaultFirstCharacterReplaceString"/></item> is considered instead of <see cref="DefaultReplaceString"/>
  ///   </list>
  ///   for the first character.
  /// </remarks>
  public bool TreatFirstCharacterSpecial
  {
    get { return _treatFirstCharacterSpecial; }
    set { _treatFirstCharacterSpecial = value; }
  }

  /// <summary>
  /// States that all english letters (upper case and lower case) are valid for the first character of an 
  /// identifier, while accented letters and umlauts are not.
  /// </summary>
  /// <remarks>
  /// This property is only considered if <see cref="TreatFirstCharacterSpecial"/> is <see langword="true" />.
  /// </remarks>
  public bool AllowFirstCharacterEnglishLetters
  {
    get { return _allowFirstCharacterEnglishLetters; }
    set { _allowFirstCharacterEnglishLetters = value; }
  }

  /// <summary>
  /// States that all letters (upper case and lower case) are valid for the first character of an identifier, 
  /// including accented letters and umlauts.
  /// </summary>
  /// <remarks>
  /// This property is only considered if <see cref="TreatFirstCharacterSpecial"/> is <see langword="true" />.
  /// </remarks>
  public bool AllowFirstCharacterLanguageSpecificLetters
  {
    get { return _allowFirstCharacterLanguageSpecificLetters; }
    set { _allowFirstCharacterLanguageSpecificLetters = value; }
  }

  /// <summary>
  /// States that numeric digits are valid for the first character of an identifier.
  /// </summary>
  /// <remarks>
  /// This property is only considered if <see cref="TreatFirstCharacterSpecial"/> is <see langword="true" />.
  /// </remarks>
  public bool AllowFirstCharacterDigits
  {
    get { return _allowFirstCharacterDigits; }
    set { _allowFirstCharacterDigits = value; }
  }

  /// <summary>
  /// Provides a list of characters that are valid for the first character of an identifier.
  /// </summary>
  /// <remarks>
  /// This property is only considered if <see cref="TreatFirstCharacterSpecial"/> is <see langword="true" />.
  /// </remarks>
  public string AllowFirstCharacterAdditionalCharacters
  {
    get { return _allowFirstCharacterAdditionalCharacters; }
    set { _allowFirstCharacterAdditionalCharacters = value; }
  }

  /// <summary>
  /// If the first character is not valid and has no special replace string defined, it isreplaced with this value.
  /// </summary>
  /// <remarks>
  /// This property is only considered if <see cref="TreatFirstCharacterSpecial"/> is <see langword="true" />.
  /// </remarks>
  public string DefaultFirstCharacterReplaceString
  {
    get { return _defaultFirstCharacterReplaceString; }
    set { _defaultFirstCharacterReplaceString = value; }
  }
  /// <summary>
  ///   Specifies whether the resulting identifiers are case-sensitive.
  /// </summary>
  /// <remarks>
  ///   For generating unique identifiers, this property determines which identifiers are considered equal.
  /// </remarks>
  public bool IsCaseSensitive
  {
    get { return _isCaseSensitive; }
    set 
    { 
      if (_uniqueIdentifiers != null)
        throw new InvalidOperationException ("Cannot change property IsCaseSensitive after unique identifiers have been produced.");
      _isCaseSensitive = value; 
    }
  }

  /// <summary>
  ///   Secifies whether the provided unique names are considered to be case sensitive.
  /// </summary>
  /// <remarks>
  ///   For generating unique identifiers using unique names, this property determines whether two unique names that
  ///   differ only in case are considered equal.
  /// </remarks>
  public bool UseCaseSensitiveNames
  {
    get { return _useCaseSensitiveNames; }
    set 
    { 
      if (_uniqueIdentfiersByObject != null)
        throw new InvalidOperationException ("Cannot change property UseCaseSensitiveNames after unique identifiers have been produced.");
      _useCaseSensitiveNames = value; 
    }
  }

  object ICloneable.Clone()
  {
    return this.Clone();
  }

  /// <summary>
  ///   Creates a copy of an IdentifierGenerator.
  /// </summary>
  /// <remarks>
  ///   No records of generated identifiers are copied to the clone.
  /// </remarks>
  public IdentifierGenerator Clone ()
  {
    IdentifierGenerator clone = new IdentifierGenerator ();

    clone._uniqueSeparator = this._uniqueSeparator;

    clone._allowEnglishLetters = this._allowEnglishLetters;
    clone._allowLanguageSpecificLetters = this._allowLanguageSpecificLetters;
    clone._allowDigits = this._allowDigits;
    clone._allowAdditionalCharacters = this._allowAdditionalCharacters;
    clone._defaultReplaceString = this._defaultReplaceString;

    clone._treatFirstCharacterSpecial = this._treatFirstCharacterSpecial;
    clone._allowFirstCharacterEnglishLetters = this._allowFirstCharacterEnglishLetters;
    clone._allowFirstCharacterLanguageSpecificLetters = this._allowFirstCharacterLanguageSpecificLetters;
    clone._allowFirstCharacterDigits = this._allowFirstCharacterDigits;
    clone._allowFirstCharacterAdditionalCharacters = this._allowFirstCharacterAdditionalCharacters;
    clone._defaultFirstCharacterReplaceString = this._defaultFirstCharacterReplaceString;

    clone._isCaseSensitive = this._isCaseSensitive;
    clone._useCaseSensitiveNames = this._useCaseSensitiveNames;
    // do not copy this._isTemplate !

    return clone;
  }
}

}
