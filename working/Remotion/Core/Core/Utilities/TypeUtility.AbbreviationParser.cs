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
using System.Text.RegularExpressions;

namespace Remotion.Utilities
{
  public static partial class TypeUtility
  {
    /// <summary>
    /// The implementation of <see cref="TypeUtility.ParseAbbreviatedTypeName"/>, implemented in a nested class in order to prevent unnecessary
    /// initialization of pre-compiled regular expressions.
    /// </summary>
    private class AbbreviationParser
    {
      public static bool IsAbbreviatedTypeName (string typeName)
      {
        ArgumentUtility.DebugCheckNotNull ("typeName", typeName);
        
        return typeName.Contains ("::");
      }

      private readonly Regex _enclosedQualifiedTypeRegex;
      private readonly Regex _enclosedTypeRegex;
      private readonly Regex _typeRegex;

      public AbbreviationParser ()
      {
        const string typeNamePattern = //  <asm>::<type>
            @"(?<asm>[^\[\]\,]+)" //       <asm> is the assembly part of the type name (before ::)
            + @"::"
            + @"(?<type>[^\[\]\,]+)"; //   <type> is the partially qualified type name (after ::)

        const string bracketPattern = //   [...] (an optional pair of matching square brackets and anything in between)
            @"(?<br> \[            " //    see "Mastering Regular Expressions" (O'Reilly) for how the construct "balancing group definition" 
            + @"  (                " //    is used to match brackets: http://www.oreilly.com/catalog/regex2/chapter/ch09.pdf
            + @"      [^\[\]]      "
            + @"    |              "
            + @"      \[ (?<d>)    " //    increment nesting counter <d>
            + @"    |              "
            + @"      \] (?<-d>)   " //    decrement <d>
            + @"  )*               "
            + @"  (?(d)(?!))       " //    ensure <d> is 0 before considering next match
            + @"\] )?              ";

        const string strongNameParts = // comma-separated list of name=value pairs
            @"(?<sn> (, \s* \w+ = [^,]+ )* )";

        const string typePattern = // <asm>::<type>[...] (square brackets are optional)
            typeNamePattern
            + bracketPattern;

        const string openUnqualifiedPattern = // requires the pattern to be preceded by [ or ,
            @"(?<= [\[,] )";
        const string closeUnqualifiedPattern = // requires the pattern to be followed by ] or ,
            @"(?= [\],] )";

        const string enclosedTypePattern = // type within argument list
            openUnqualifiedPattern
            + typePattern
            + closeUnqualifiedPattern;

        const string qualifiedTypePattern = // <asm>::<type>[...], name=val, name=val ... (square brackets are optional)
            typePattern
            + strongNameParts;

        const string openQualifiedPattern = // requires the pattern to be preceded by [[ or ,[
            @"(?<= [\[,] \[)";
        const string closeQualifiedPattern = // requires the pattern to be followed by ]] or ],
            @"(?= \] [\],] )";

        const string enclosedQualifiedTypePattern = // qualified type within argument list
            openQualifiedPattern
            + qualifiedTypePattern
            + closeQualifiedPattern;

        // Do not use RegexOptions.Compiled because it takes 200ms to compile which is not offset by the calls made after cache lookups.
        // This is an issue in .NET up to at least version 4.5.1 in x64 mode.
        const RegexOptions options = RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace;
        _enclosedQualifiedTypeRegex = new Regex (enclosedQualifiedTypePattern, options);
        _enclosedTypeRegex = new Regex (enclosedTypePattern, options);
        _typeRegex = new Regex (typePattern, options);
      }

      public string ParseAbbreviatedTypeName (string abbreviatedTypeName)
      {
        ArgumentUtility.DebugCheckNotNull ("abbreviatedTypeName", abbreviatedTypeName);

        string fullTypeName = abbreviatedTypeName;
        const string replace = @"${asm}.${type}${br}, ${asm}";
        fullTypeName = ReplaceRecursive (_enclosedQualifiedTypeRegex, fullTypeName, replace + "${sn}");
        fullTypeName = ReplaceRecursive (_enclosedTypeRegex, fullTypeName, "[" + replace + "]");
        fullTypeName = _typeRegex.Replace (fullTypeName, replace);

        return fullTypeName;
      }

      private string ReplaceRecursive (Regex regex, string input, string replacement)
      {
        string result = regex.Replace (input, replacement);
        while (result != input)
        {
          input = result;
          result = regex.Replace (input, replacement);
        }
        return result;
      }
    }
  }
}
