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
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.TestBasePage")]
public class TestWxeBasePage:
    WxePage,
    Remotion.Web.UI.Controls.IControl,
    IObjectWithResources //  Provides the WebForm's ResourceManager via GetResourceManager() 
    // IResourceUrlResolver //  Provides the URLs for this WebForm (e.g. to the FormGridManager)
{  
  private Button _nextButton = new Button();

  protected override void OnInit(EventArgs e)
  {
    if (! ControlHelper.IsDesignMode (this))
    {
      try
      {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}
      try
      {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(Request.UserLanguages[0]);
      }
      catch (ArgumentException)
      {}

      _nextButton.ID = "NextButton";
      _nextButton.Text = "Next";
      WxeControls.AddAt (0, _nextButton);
    }

    ShowAbortConfirmation = ShowAbortConfirmation.Always;
    EnableAbort = false;
    base.OnInit (e);
    RegisterEventHandlers();
  }

  protected override void OnPreRender(EventArgs e)
  {
    //  A call to the ResourceDispatcher to get have the automatic resources dispatched
    ResourceDispatcher.Dispatch (this, ResourceManagerUtility.GetResourceManager (this));

    base.OnPreRender (e);

    HtmlHeadAppender.Current.RegisterPageStylesheetLink ();

    var key = GetType().FullName + "_Global";
    if (! HtmlHeadAppender.Current.IsRegistered (key))
    {
      HtmlHeadAppender.Current.RegisterStylesheetLink (key, "Html/global.css");
    }


    LiteralControl stack = new LiteralControl();

    StringBuilder sb = new StringBuilder();
    sb.Append ("<br /><div>");
    sb.Append ("<b>Stack:</b><br />");
    for (WxeStep step = CurrentPageStep; step != null; step = step.ParentStep)
      sb.AppendFormat ("{0}<br />", step.ToString());      
    sb.Append ("</div>");
    stack.Text = sb.ToString();
    
    WxeControls.Add (stack);
  }

  protected virtual void RegisterEventHandlers()
  {
    _nextButton.Click += new EventHandler(NextButton_Click);
  }

  protected virtual IResourceManager GetResourceManager()
  {
    Type type = GetType();
    return GlobalizationService.GetResourceManager (type);
  }

  protected IGlobalizationService GlobalizationService
  {
    get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
  }

  IResourceManager IObjectWithResources.GetResourceManager()
  {
    return GetResourceManager();
  }

//  public string GetResourceUrl (Type definingType, ResourceType resourceType, string relativeUrl)
//  {
//    if (ControlHelper.IsDesignMode (this, this.Context))
//      return resourceType.Name + "/" + relativeUrl;
//    else
//      return Page.ResolveUrl (resourceType.Name + "/" + relativeUrl);
//  }

  private void NextButton_Click(object sender, EventArgs e)
  {
    ExecuteNextStep();
  }

  protected virtual ControlCollection WxeControls
  {
    get { return WxeForm.Controls; }
  }
}

}
