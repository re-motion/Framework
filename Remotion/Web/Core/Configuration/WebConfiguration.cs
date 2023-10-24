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
using System.Xml.Serialization;
using Remotion.Web.Schemas;
using Remotion.Xml;

namespace Remotion.Web.Configuration
{

/// <summary> The configuration section for <b>Remotion.Web</b>. </summary>
/// <include file='..\doc\include\Configuration\WebConfiguration.xml' path='WebConfiguration/Class/*' />
[XmlType(WebConfiguration.ElementName, Namespace = WebConfiguration.SchemaUri)]
public class WebConfiguration: IConfigurationSectionHandler
{
  /// <summary> The name of the configuration section in the configuration file. </summary>
  /// <remarks> <c>remotion.web</c> </remarks>
  public const string ElementName = "remotion.web";

  /// <summary> The namespace of the configuration section schema. </summary>
  /// <remarks> <c>http://www.re-motion.org/web/configuration</c> </remarks>
  public const string SchemaUri = "http://www.re-motion.org/web/configuration";

  private static readonly DoubleCheckedLockingContainer<WebConfiguration> s_current =
      new DoubleCheckedLockingContainer<WebConfiguration>(CreateConfig);

  /// <summary> Gets the <see cref="WebConfiguration"/>. </summary>
  public static WebConfiguration Current
  {
    get { return s_current.Value; }
  }

  protected static void SetCurrent (WebConfiguration configuration)
  {
    s_current.Value = configuration;
  }

  private static WebConfiguration CreateConfig ()
  {
    XmlNode section = (XmlNode)ConfigurationManager.GetSection(ElementName);
    if (section == null)
      return new WebConfiguration();

    var schema = new WebConfigurationSchema();
    return (WebConfiguration)XmlSerializationUtility.DeserializeUsingSchema(
        new XmlNodeReader(section),
        // "web.config/configuration/" + ElementName,  // TODO: context is no longer supported, verify that node has correct BaseURI
        typeof(WebConfiguration),
        SchemaUri,
        schema.LoadSchemaSet());
  }

  private ExecutionEngineConfiguration _executionEngine = new ExecutionEngineConfiguration();
  private WcagConfiguration _wcag = new WcagConfiguration();

  /// <summary> Gets or sets the <see cref="ExecutionEngineConfiguration"/> entry. </summary>
  [XmlElement("executionEngine")]
  public ExecutionEngineConfiguration ExecutionEngine
  {
    get { return _executionEngine; }
    set { _executionEngine = value; }
  }

  /// <summary> Gets or sets the <see cref="WcagConfiguration"/> entry. </summary>
  [XmlElement("wcag")]
  public WcagConfiguration Wcag
  {
    get { return _wcag; }
    set { _wcag = value; }
  }

  object IConfigurationSectionHandler.Create (object parent, object configContext, XmlNode section)
  {
    // instead of the WebConfiguration instance, the xml node is returned. this prevents version 
    // conflicts when multiple versions of this assembly are loaded within one AppDomain.
    return section;
  }
}


}
