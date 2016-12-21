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
using System.Configuration;
using System.Xml;

namespace Remotion.Reflection.CodeGeneration.TypePipe.Configuration
{
  public class TypePipeConfigurationSection : ConfigurationSection
  {
    private const string c_xmlNamespace = "http://www.re-motion.org/Reflection/CodeGeneration/TypePipe/Configuration";

    public static readonly string ExampleConfiguration =
        "<remotion.reflection.codeGeneration.typePipe xmlns=\"" + c_xmlNamespace + "\">" + Environment.NewLine +
        "  <forceStrongNaming keyFilePath=\"keyFile.snk\"/>" + Environment.NewLine +
        "  <enableSerializationWithoutAssemblySaving/>" + Environment.NewLine +
        "</remotion.reflection.codeGeneration.typePipe>";

    [ConfigurationProperty ("xmlns")]
    public string XmlNamespace
    {
      get { return c_xmlNamespace; }
    }

    [ConfigurationProperty ("forceStrongNaming")]
    public ForceStrongNamingConfigurationElement ForceStrongNaming
    {
      get { return (ForceStrongNamingConfigurationElement) this["forceStrongNaming"]; }
    }

    [ConfigurationProperty ("enableSerializationWithoutAssemblySaving")]
    public EnableSerializationWithoutAssemblySavingConfigurationElement EnableSerializationWithoutAssemblySaving
    {
      get { return (EnableSerializationWithoutAssemblySavingConfigurationElement) this["enableSerializationWithoutAssemblySaving"]; }
    }

    protected override bool OnDeserializeUnrecognizedElement (string elementName, XmlReader reader)
    {
      var message = string.Format ("Unknown element name: {0}{2}Example configuration:{2}{1}", elementName, ExampleConfiguration, Environment.NewLine);
      throw new ConfigurationErrorsException (message);
    }
  }
}