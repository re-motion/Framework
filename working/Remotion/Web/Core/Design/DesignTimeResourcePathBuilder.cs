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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Resources;

namespace Remotion.Web.Design
{
  /// <summary>
  /// Design-time implementation of the <see cref="IResourcePathBuilder"/> interface.
  /// </summary>
  [UsedImplicitly]
  public class DesignTimeResourcePathBuilder : ResourcePathBuilderBase
  {
    private const string c_designTimeRootDefault = "C:\\Remotion.Resources";
    private const string c_designTimeRootEnvironmentVaribaleName = "REMOTIONRESOURCES";

    public DesignTimeResourcePathBuilder ()
    {
    }

    protected override string BuildPath (string[] completePath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("completePath", completePath);

      return completePath.Aggregate (Path.Combine);
    }

    protected override string GetResourceRoot ()
    {
      string root = Environment.GetEnvironmentVariable (c_designTimeRootEnvironmentVaribaleName);
      if (string.IsNullOrEmpty (root))
        root = c_designTimeRootDefault;
      return Path.GetPathRoot (root);

      //EnvDTE._DTE environment = (EnvDTE._DTE) site.GetService (typeof (EnvDTE._DTE));
      //if(environment != null)
      //{
      //  EnvDTE.Project project = environment.ActiveDocument.ProjectItem.ContainingProject;          
      //  //  project.Properties uses a 1-based index
      //  for (int i = 1; i <= project.Properties.Count; i++)
      //  {
      //    if(project.Properties.Item (i).Name == "ActiveFileSharePath")
      //      return project.Properties.Item (i).Value.ToString();
      //  }
      //}
    }
  }
}