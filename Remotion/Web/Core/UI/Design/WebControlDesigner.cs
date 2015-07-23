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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{
  /// <summary>
  ///   A desinger that requries the complete loading of the control.
  /// </summary>
  public class WebControlDesigner : ControlDesigner
  {
    private bool _hasCheckedForDuplicateAssemblies;

    public override void Initialize (IComponent component)
    {
      ArgumentUtility.CheckNotNull ("component", component);

      base.Initialize (component);
      SetViewFlags (ViewFlags.DesignTimeHtmlRequiresLoadComplete, true);

      Assertion.IsNotNull (component.Site, "The component does not have a Site.");
      IDesignerHost designerHost = (IDesignerHost) component.Site.GetService (typeof (IDesignerHost));
      Assertion.IsNotNull (designerHost, "No IDesignerHost found.");

      if (!DesignerUtility.IsDesignMode || !object.ReferenceEquals (DesignerUtility.DesignModeHelper.DesignerHost, designerHost))
        DesignerUtility.SetDesignMode (new WebDesginModeHelper (designerHost));

      EnsureCheckForDuplicateAssemblies();
    }

    public override string GetDesignTimeHtml ()
    {
      EnsureCheckForDuplicateAssemblies();

      try
      {
        IControlWithDesignTimeSupport control = Component as IControlWithDesignTimeSupport;
        if (control != null)
          control.PreRenderForDesignMode();
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.WriteLine (e.Message);
      }

      return base.GetDesignTimeHtml();
    }

    protected void EnsureCheckForDuplicateAssemblies ()
    {
      if (_hasCheckedForDuplicateAssemblies)
        return;

      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      Assembly[] remotionAssemblies = Array.FindAll (
          assemblies,
          delegate (Assembly assembly) { return assembly.FullName.StartsWith ("Remotion"); });

      Dictionary<string, Assembly> assemblyDictionary = new Dictionary<string, Assembly>();
      foreach (Assembly assembly in remotionAssemblies)
      {
        if (assemblyDictionary.ContainsKey (assembly.FullName))
        {
          throw new NotSupportedException (
              "Duplicate re-motion framework assemblies have been detected. In order to provide a consistent design time experience it is necessary"
              + " to install the re-motion framework in the global assembly cache (GAC). In addition, please ensure that the 'Copy Local' flag"
              + " is set to 'true' for all re-motion framework assemblies referenced by the web project.");
        }
        assemblyDictionary.Add (assembly.FullName, assembly);
      }

      _hasCheckedForDuplicateAssemblies = true;
    }
  }
}
