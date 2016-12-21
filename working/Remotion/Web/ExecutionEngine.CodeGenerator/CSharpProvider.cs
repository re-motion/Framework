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
  public class CSharpProvider : RegexProvider
  {
    private const string c_prefix = "//";
    private const RegexOptions c_regexOptions = RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled;

    private static Regex s_importNamespaceExpr = new Regex (@"^ \s* using \s+ (?<namespace> [\w._]+) ; \s* (//|$)", c_regexOptions);
    private static Regex s_classDeclarationExpr = new Regex (@"^ \s* ((new|public|private|protected|internal|abstract|partial) \s+)* class \s+ (?<class> [\w_]+)", c_regexOptions);    
    private static Regex s_namespaceDeclarationExpr = new Regex (@"^ \s* namespace \s+ (?<namespace> [\w_.]+) \s* (//|{|$)", c_regexOptions);                             
    private static Dictionary<string, string> s_builtInTypes = new Dictionary<string, string> ();

    private static Regex s_typeNameExpr = new Regex (@"(?<basetype> @?\w+)", c_regexOptions | RegexOptions.IgnoreCase);

    static CSharpProvider ()
    {
      s_builtInTypes.Add ("bool", "System.Boolean");
      s_builtInTypes.Add ("byte", "System.Byte");
      s_builtInTypes.Add ("sbyte", "System.SByte");
      s_builtInTypes.Add ("char", "System.Char");
      s_builtInTypes.Add ("decimal", "System.Decimal");
      s_builtInTypes.Add ("double", "System.Double");
      s_builtInTypes.Add ("float", "System.Single");
      s_builtInTypes.Add ("int", "System.Int32");
      s_builtInTypes.Add ("uint", "System.UInt32");
      s_builtInTypes.Add ("long", "System.Int64");
      s_builtInTypes.Add ("ulong", "System.UInt64");
      s_builtInTypes.Add ("object", "System.Object");
      s_builtInTypes.Add ("short", "System.Int16");
      s_builtInTypes.Add ("ushort", "System.UInt16");
      s_builtInTypes.Add ("string", "System.String");      
    }

    public override string LineComment
    {
      get { return c_prefix; }
    }

    public override Regex ImportNamespaceExpr
    {
      get { return s_importNamespaceExpr; }
    }

    public override Regex ClassDeclarationExpr
    {
      get { return s_classDeclarationExpr; }
    }

    public override Regex NamespaceDeclarationExpr
    {
      get { return s_namespaceDeclarationExpr; }
    }

    public override Dictionary<string, string> BuiltInTypes
    {
      get { return s_builtInTypes; }
    }

    public override Regex TypeNameExpr
    {
      get { return s_typeNameExpr; }
    }
  }
}
