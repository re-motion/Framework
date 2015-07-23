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
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Remotion.Web.ExecutionEngine.CodeGenerator.Schema;

namespace Remotion.Web.ExecutionEngine.CodeGenerator.Schemas
{
  [XmlRoot (ElementName, Namespace = SchemaUri)]
  public class FunctionDeclaration
  {
    public const string ElementName = "WxeFunction";

    /// <summary> The namespace of the function declaration schema. </summary>
    /// <remarks> <c>http://www.re-motion.org/web/wxefunctiongenerator</c> </remarks>
    public const string SchemaUri = "http://www.re-motion.org/web/wxefunctiongenerator";

    public static XmlReader GetSchemaReader ()
    {
      return new XmlTextReader (Assembly.GetExecutingAssembly().GetManifestResourceStream (typeof (FunctionDeclaration), "FunctionDeclaration.xsd"));
    }

    private ParameterDeclaration[] _parameters = new ParameterDeclaration[0];
    private VariableDeclaration[] _variables = new VariableDeclaration[0];

    [XmlAttribute ("codeBehindType")]
    public string TemplateControlCodeBehindType { get; set; }

    [XmlAttribute ("markupFile")]
    public string TemplateControlMarkupFile { get; set; }

    [XmlAttribute ("mode")]
    public TemplateMode TemplateControlMode { get; set; }

    [XmlAttribute ("functionName")]
    public string FunctionName { get; set; }

    [XmlAttribute ("functionBaseType")]
    public string FunctionBaseType { get; set; }

    [XmlElement ("Parameter")]
    public ParameterDeclaration[] Parameters
    {
      get
      {
        List<ParameterDeclaration> parameters = new List<ParameterDeclaration> (_parameters);
        if (ReturnValue != null)
          parameters.Add (ReturnValue);
        return parameters.ToArray();
      }
      set
      {
        if (value == null)
          _parameters = new ParameterDeclaration[0];
        else
          _parameters = value;
      }
    }

    [XmlElement ("ReturnValue")]
    public ReturnValueDeclaration ReturnValue { get; set; }

    [XmlElement ("Variable")]
    public VariableDeclaration[] Variables
    {
      get { return _variables; }
      set
      {
        if (value == null)
          _variables = new VariableDeclaration[0];
        else
          _variables = value;
      }
    }

    [XmlIgnore]
    public VariableDeclaration[] ParametersAndVariables
    {
      get
      {
        List<VariableDeclaration> result = new List<VariableDeclaration>();
        result.AddRange (Parameters);
        result.AddRange (Variables);
        return result.ToArray();
      }
    }
  }
}
