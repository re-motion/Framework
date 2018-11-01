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
using System.Text.RegularExpressions;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public abstract class RegexProvider: LanguageProvider
  {
    public abstract string LineComment { get; }
    public abstract Regex ImportNamespaceExpr { get; }
    public abstract Regex ClassDeclarationExpr { get; }
    public abstract Regex NamespaceDeclarationExpr { get; }
    public abstract Dictionary<string, string> BuiltInTypes { get; } 
    public abstract Regex TypeNameExpr { get; }

    public override CodeLineType ParseLine (string line, out string argument)
    {
      string trimmedLine = line.TrimStart ();
      Match match;
      if (trimmedLine.StartsWith (LineComment))
      {
        argument = trimmedLine.Substring (LineComment.Length);
        return CodeLineType.LineComment;
      }
      else if ((match = ImportNamespaceExpr.Match (line)).Success)
      {
        argument = match.Groups["namespace"].Value;
        return CodeLineType.NamespaceImport;
      }
      else if ((match = NamespaceDeclarationExpr.Match (line)).Success)
      {
        argument = match.Groups["namespace"].Value;
        return CodeLineType.NamespaceDeclaration;
      }
      else if ((match = ClassDeclarationExpr.Match (line)).Success)
      {
        argument = match.Groups["class"].Value;
        return CodeLineType.ClassDeclaration;
      }
      else
      {
        argument = null;
        return CodeLineType.Other;
      }
    }

    public override string ConvertTypeName (string type)
    {
      type = TypeNameExpr.Replace (
          type,
          delegate (Match match)
          {
            string basetype = match.Groups["basetype"].Value;
            string result;
            if (BuiltInTypes.TryGetValue (basetype, out result))
              return result;
            else
              return basetype;
          });

      return type.Replace ('{', '<').Replace ('}', '>');
    }
  }
}
