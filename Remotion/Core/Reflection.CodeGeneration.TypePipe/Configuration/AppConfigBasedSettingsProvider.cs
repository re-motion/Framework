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
using Remotion.TypePipe;

namespace Remotion.Reflection.CodeGeneration.TypePipe.Configuration
{
  /// <summary>
  /// An <c>App.config</c>-based configuration provider.<br/>
  /// <example>
  /// Example of an <c>App.config</c> configuration file.
  /// <code>
  /// &lt;configuration&gt;
  ///   &lt;configSections&gt;
  ///     &lt;section name="remotion.reflection.codeGeneration.typePipe" type="Remotion.Reflection.CodeGeneration.TypePipe.Configuration.TypePipeConfigurationSection, Remotion.Reflection.CodeGeneration.TypePipe"/&gt;
  ///     &lt;!-- ... --&gt;
  ///   &lt;/configSections&gt;
  ///   
  ///   &lt;remotion.reflection.codeGeneration.typePipe xmlns="http://www.re-motion.org/Reflection/CodeGeneration/TypePipe/Configuration"&gt;
  ///     &lt;forceStrongNaming keyFilePath="keyFile.snk"/&gt;
  ///     &lt;enableSerializationWithoutAssemblySaving/&gt;
  ///   &lt;/remotion.reflection.codeGeneration.typePipe&gt;
  ///   &lt;!-- ... --&gt;
  /// &lt;/configuration&gt;
  /// </code>
  /// </example>
  /// </summary>
  public class AppConfigBasedSettingsProvider
  {
    private readonly TypePipeConfigurationSection _section;

    public AppConfigBasedSettingsProvider ()
    {
      _section = (TypePipeConfigurationSection) ConfigurationManager.GetSection ("remotion.reflection.codeGeneration.typePipe") ?? new TypePipeConfigurationSection();
    }

    public bool ForceStrongNaming
    {
      get { return _section.ForceStrongNaming.ElementInformation.IsPresent; }
    }

    public string KeyFilePath
    {
      get { return _section.ForceStrongNaming.KeyFilePath; }
    }

    public bool EnableSerializationWithoutAssemblySaving
    {
      get { return _section.EnableSerializationWithoutAssemblySaving.ElementInformation.IsPresent; }
    }

    public PipelineSettings GetSettings ()
    {
      return PipelineSettings
          .New()
          .SetForceStrongNaming (ForceStrongNaming)
          .SetKeyFilePath (KeyFilePath)
          .SetEnableSerializationWithoutAssemblySaving (EnableSerializationWithoutAssemblySaving)
          .Build();
    }
  }
}