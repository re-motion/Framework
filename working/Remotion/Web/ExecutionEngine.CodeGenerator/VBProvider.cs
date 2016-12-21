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
  public class VBProvider : RegexProvider
  {
    private const string c_prefix = "'";
    private const RegexOptions c_regexOptions = RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase;

    private static Regex s_importNamespaceExpr = new Regex (@"^ \s* Imports \s+ (?<namespace> [\w._]+) \s+ ('|$)", c_regexOptions);
    private static Regex s_classDeclarationExpr = new Regex (@"^ \s* ((Public|Private|Protected|Friend|Shadows|MustInherit|Partial) \s+)* Class \s+ (?<class> [\w_]+)", c_regexOptions);
    private static Regex s_namespaceDeclarationExpr = new Regex (@"^ \s* Namespace \s+ (?<namespace> [\w_.]+) \s* ('|$)", c_regexOptions);
    private static Dictionary<string, string> s_builtInTypes = new Dictionary<string, string> (new CaseInsensitiveStringComparer());

    private static Regex s_typeNameExpr = new Regex (@"(?<basetype> (\w+ | \[\w+\]))", c_regexOptions | RegexOptions.IgnoreCase);

    static VBProvider ()
    {
      s_builtInTypes.Add ("Boolean", "System.Boolean");
      s_builtInTypes.Add ("Byte", "System.Byte");
      s_builtInTypes.Add ("Char", "System.Char");
      s_builtInTypes.Add ("Date", "System.DateTime");
      s_builtInTypes.Add ("Decimal", "System.Decimal");
      s_builtInTypes.Add ("Double", "System.Double");
      s_builtInTypes.Add ("Integer", "System.Int32");
      s_builtInTypes.Add ("Long", "System.Int64");
      s_builtInTypes.Add ("Object", "System.Object");
      s_builtInTypes.Add ("SByte", "System.SByte");
      s_builtInTypes.Add ("Short", "System.Int16");
      s_builtInTypes.Add ("Single", "System.Single");
      s_builtInTypes.Add ("String", "System.String");
      s_builtInTypes.Add ("UInteger", "System.UInt32");
      s_builtInTypes.Add ("ULong", "System.UInt64");
      s_builtInTypes.Add ("UShort", "System.UInt16");
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
