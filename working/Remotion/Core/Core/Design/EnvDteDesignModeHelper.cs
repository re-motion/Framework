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
using System.ComponentModel.Design;
using System.Configuration;
using System.IO;
using System.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.Design
{
  /// <summary>
  /// Implementation of the <see cref="IDesignModeHelper"/> interface for the <b>EnvDTE</b> (i.e. Visual Studio) designer.
  /// </summary>
  /// <remarks>
  /// <see cref="GetConfiguration"/> is hard coded to look up a configuration file named <c>app.config</c>.
  /// </remarks>
  public abstract class EnvDteDesignModeHelper: DesignModeHelperBase
  {
    protected EnvDteDesignModeHelper (IDesignerHost designerHost)
        : base (designerHost)
    {
    }

    public override string GetProjectPath()
    {
      string projectPath = (string) GetDesignTimePropertyValue ("ActiveFileSharePath");

      if (projectPath == null)
        projectPath = (string) GetDesignTimePropertyValue ("FullPath");

      return projectPath;
    }

    public override System.Configuration.Configuration GetConfiguration ()
    {
      string projectPath = GetProjectPath();
      if (projectPath == null)
        return null;

      try
      {
        ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
        exeConfigurationFileMap.ExeConfigFilename = Path.Combine (projectPath, "app.config");
        return ConfigurationManager.OpenMappedExeConfiguration (exeConfigurationFileMap, ConfigurationUserLevel.None);
      }
      catch (Exception e)
      {
        throw new ApplicationException (string.Format ("Error while reading app.config located at {0}.", projectPath), e);
      }
    }

    public object GetDesignTimePropertyValue (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      try
      {
        //EnvDTE._DTE environment = (EnvDTE._DTE) ((IServiceProvider)site).GetService (typeof (EnvDTE._DTE));
        Type _DTEType = TypeUtility.GetType ("EnvDTE._DTE, EnvDTE", true);
        object environment = DesignerHost.GetService (_DTEType);

        if (environment != null)
        {
          //EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;
          object activeDocument = _DTEType.InvokeMember ("ActiveDocument", BindingFlags.GetProperty, null, environment, null);
          object projectItem = activeDocument.GetType().InvokeMember ("ProjectItem", BindingFlags.GetProperty, null, activeDocument, null);
          object project = projectItem.GetType().InvokeMember ("ContainingProject", BindingFlags.GetProperty, null, projectItem, null);

          ////project.Properties uses a 1-based index
          //foreach (EnvDTE.Property property in project.Properties)
          object properties = project.GetType().InvokeMember ("Properties", BindingFlags.GetProperty, null, project, null);
          foreach (object property in (IEnumerable) properties)
          {
            //if (property.Name == propertyName)
            string projectPropertyName = (string) property.GetType().InvokeMember ("Name", BindingFlags.GetProperty, null, property, null);
            if (projectPropertyName == propertyName)
            {
              //return property.Value;
              return property.GetType().InvokeMember ("Value", BindingFlags.GetProperty, null, property, null);
            }
          }
        }
      }
      catch
      {
        return null;
      }

      return null;
    }
  }
}
