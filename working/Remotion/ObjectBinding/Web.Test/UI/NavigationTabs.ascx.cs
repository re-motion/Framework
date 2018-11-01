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
using System.Reflection;
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Web;
using Remotion.Web.Configuration;
using Remotion.Web.UI.Controls;

namespace OBWTest.UI
{
  /// <summary>
  ///		Summary description for NavigationTabs.
  /// </summary>
  public class NavigationTabs : UserControl
  {
    public enum WaiConformanceLevel
    {
      Undefined = 0,
      A = 1,
      DoubleA = 3,
      TripleA = 7
    }

    protected BocEnumValue WaiConformanceLevelField;
    protected TabbedMenu TabbedMenu;
    
    public WaiConformanceLevel ConformanceLevel
    {
      get { return (WaiConformanceLevel) WebConfiguration.Current.Wcag.ConformanceLevel; }
      set { WebConfiguration.Current.Wcag.ConformanceLevel = (Remotion.Web.Configuration.WaiConformanceLevel) value; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      Type itemType = GetType();
      PropertyInfo propertyInfo = itemType.GetProperty ("ConformanceLevel");
      EnumerationProperty property = new EnumerationProperty (
          new PropertyBase.Parameters (
              (BindableObjectProvider) BusinessObjectProvider.GetProvider<BindableObjectProviderAttribute>(),
              PropertyInfoAdapter.Create (propertyInfo),
              propertyInfo.PropertyType,
              new Lazy<Type> (() => propertyInfo.PropertyType),
              null,
              false,
              false,
              new BindableObjectDefaultValueStrategy(),
              SafeServiceLocator.Current.GetInstance<IBindablePropertyReadAccessStrategy>(),
              SafeServiceLocator.Current.GetInstance<IBindablePropertyWriteAccessStrategy>(),
              SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>()));

      WaiConformanceLevelField.Property = property;
      WaiConformanceLevelField.LoadUnboundValue (ConformanceLevel, IsPostBack);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      string mode = Global.PreferQuirksModeRendering ? "Quirks" : "Standard";
      string theme = Global.PreferQuirksModeRendering ? "" : SafeServiceLocator.Current.GetInstance<ResourceTheme> ().Name;
      TabbedMenu.StatusText = mode + " " + theme;
    }

    private void WaiConformanceLevelField_SelectionChanged (object sender, EventArgs e)
    {
      ConformanceLevel = (WaiConformanceLevel) WaiConformanceLevelField.Value;
      WaiConformanceLevelField.IsDirty = false;
    }

    #region Web Form Designer generated code

    protected override void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit (e);
    }

    /// <summary>
    ///		Required method for Designer support - do not modify
    ///		the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.WaiConformanceLevelField.SelectionChanged += new System.EventHandler (this.WaiConformanceLevelField_SelectionChanged);
    }

    #endregion
  }
}
