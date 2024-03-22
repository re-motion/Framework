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
using System.Collections.Specialized;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ServiceLocation;
using Remotion.Text;
using Remotion.Validation;
using Remotion.Validation.Results;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace OBWTest
{

public class TestTabbedForm : TestWxeBasePage
{
  protected HtmlHeadContents HtmlHeadContents;
  private IDataEditControl[] _dataEditControls;
  protected WebTabStrip PagesTabStrip;
  protected TabbedMenu NavigationTabs;
  protected ValidationStateViewer ValidationStateViewer;
  protected TabbedMultiView MultiView;
  private PlaceHolder _wxeControlsPlaceHolder;
  protected SmartHyperLink SmartHyperLink1;
  private bool _currentObjectSaved = false;
  protected WebUpdatePanel UpdatePanel;

  public TestTabbedForm ()
  {
  }

  protected override void OnPreInit (EventArgs e)
  {
    MasterPageFile = Global.PreferQuirksModeRendering ? "~/QuirksMode.Master" : "~/StandardMode.Master";
    base.OnPreInit(e);
  }

  protected TestTabbedFormWxeFunction Function
  {
    get { return (TestTabbedFormWxeFunction)CurrentFunction; }
  }

  protected override void OnLoad (EventArgs e)
  {
    base.OnLoad(e);

    int activeViewIndex = 0;
    TabView activeView = (TabView)MultiView.Views[activeViewIndex];
    if (activeViewIndex > 0)
      MultiView.SetActiveView(activeView);
  }

  private void LoadUserControls ()
  {
    // add tabs 
    AddTab("1", "Test Tab 1", null);
    AddTab("2", "Test Tab 2 foo bar", null);
    AddTab("3", "Test Tab 3 foo", null);
    AddTab("4", "Test Tab 4 foo foo bar", null);
    AddTab("5", "Test Tab 5", null);
    AddTab("6", "Test Tab 6 foo", null);
    AddTab("7", "Test Tab 7 foo foo bar", null);

    //    AddMainMenuTab ("1", "Main Tab 1", null);
    //    AddMainMenuTab ("2", "Main Tab 2 foo bar", null);
    //    AddMainMenuTab ("3", "Main Tab 3 foo", null);
    //    AddMainMenuTab ("4", "Main Tab 4 foo foo bar", null);
    //    AddMainMenuTab ("5", "Main Tab 5", null);
    //    AddMainMenuTab ("6", "Main Tab 6 foo", null);
    //    AddMainMenuTab ("7", "Main Tab 7 foo foo bar", null);

    var resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();
    List<IDataEditControl> dataEditControls = new List<IDataEditControl>();
    // load editor pages
    IDataEditControl dataEditControl;
    dataEditControl = AddPage(
        "TestTabbedPersonDetailsUserControl",
        WebString.CreateFromHtml("<u>P</u>erson <b>Details</b>"),
        "p",
        new IconInfo(resourceUrlFactory.CreateResourceUrl(typeof(Person), ResourceType.Image, "Remotion.ObjectBinding.Sample.Person.gif").GetUrl()),
        "TestTabbedPersonDetailsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add(dataEditControl);
    dataEditControl = AddPage(
        "TestTabbedPersonJobsUserControl",
        WebString.CreateFromText("&Jobs"),
        "",
        new IconInfo(resourceUrlFactory.CreateResourceUrl(typeof(Job), ResourceType.Image, "Remotion.ObjectBinding.Sample.Job.gif").GetUrl()),
        "TestTabbedPersonJobsUserControl.ascx");
    if (dataEditControl != null)
      dataEditControls.Add(dataEditControl);
    _dataEditControls = (IDataEditControl[])dataEditControls.ToArray();
  }

  private void AddTab (string id, string text, IconInfo icon)
  {
    WebTab tab = new WebTab();
    tab.Text = WebString.CreateFromText("text");
    tab.ItemID = id ;
    tab.Icon = icon;
    PagesTabStrip.Tabs.Add(tab);
  }

  private void AddMainMenuTab (string id, WebString text, IconInfo icon)
  {
    WebTab tab = new WebTab();
    tab.Text = WebString.CreateFromText("text");
    tab.ItemID = id ;
    tab.Icon = icon;
    NavigationTabs.Tabs.Add(tab);
  }

  private IDataEditControl AddPage (string id, WebString title, string accessKey, IconInfo icon, string path)
  {
    TabView view = new TabView();
    view.ID = id+ "_View";
    view.Title = title;
    view.AccessKey = accessKey;
    view.Icon = icon;

    UserControl control = (UserControl)this.LoadControl(path);
    control.ID = IdentifierGenerator.HtmlStyle.GetValidIdentifier(Path.GetFileNameWithoutExtension(path));

    //EgoFormPageUserControl formPageControl = control as EgoFormPageUserControl;
    //if (formPageControl != null)
    //  formPageControl.FormPageObject = formPage;

    view.LazyControls.Add(control);
    MultiView.Views.Add(view);

    IDataEditControl dataEditControl = control as IDataEditControl;
    if (dataEditControl != null)
      dataEditControl.Load += new EventHandler(DataEditControl_Load);

    return dataEditControl;
  }

  private void DataEditControl_Load (object sender, EventArgs e)
  {
    IDataEditControl dataEditControl = (IDataEditControl)sender;
    dataEditControl.BusinessObject = (IBusinessObject)Function.Object;
    dataEditControl.LoadValues(IsPostBackAfterEnsure(dataEditControl.ID));
    dataEditControl.Mode = Function.ReadOnly ? DataSourceMode.Read : DataSourceMode.Edit;
  }

  private bool IsPostBackAfterEnsure (string id)
  {
    const string key = "EnsuredPostBacks";
    StringCollection ensuredPostBacks = (StringCollection)ViewState[key];
    if (ensuredPostBacks == null)
    {
      ensuredPostBacks = new StringCollection();
      ViewState[key] = ensuredPostBacks;
    }

    if (ensuredPostBacks.Contains(id))
    {
      return true;
    }
    else
    {
      ensuredPostBacks.Add(id);
      return false;
    }
  }

	override protected void OnInit (EventArgs e)
	{
		//
		// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		//
		InitializeComponent();

    WebButton saveButton = new WebButton();
    saveButton.ID = "SaveButton";
    saveButton.Text = WebString.CreateFromText("Save");
    saveButton.Style["margin-right"] = "10pt";
    saveButton.Click += new EventHandler(SaveButton_Click);
    MultiView.TopControls.Add(saveButton);

    WebButton cancelButton = new WebButton();
    cancelButton.ID = "CancelButton";
    cancelButton.Text = WebString.CreateFromText("Cancel");
    cancelButton.Style["margin-right"] = "10pt";
    cancelButton.Click += new EventHandler(CancelButton_Click);
    MultiView.TopControls.Add(cancelButton);

    WebButton postBackButton = new WebButton();
    postBackButton.ID = "PostBackButton";
    postBackButton.Text = WebString.CreateFromText("Postback");
    postBackButton.Style["margin-right"] = "10pt";
    MultiView.BottomControls.Add(postBackButton);

    _wxeControlsPlaceHolder = new PlaceHolder();
    MultiView.BottomControls.Add(_wxeControlsPlaceHolder);

    base.OnInit(e);

    this.EnableAbort = true;
    this.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;

	  LoadUserControls();

	  new BocList().RegisterHtmlHeadContents(HtmlHeadAppender.Current);
	  new DropDownMenu().RegisterHtmlHeadContents(HtmlHeadAppender.Current);
	  new ListMenu().RegisterHtmlHeadContents(HtmlHeadAppender.Current);
	}
	#region Web Form Designer generated code


	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent ()
	{
    MultiView.ActiveViewChanged += new EventHandler(MultiView_ActiveViewChanged);
  }
	#endregion

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);
    string mode = Global.PreferQuirksModeRendering ? "Quirks" : "Standard";
    string theme = Global.PreferQuirksModeRendering ? "" : SafeServiceLocator.Current.GetInstance<ResourceTheme>().Name;
    NavigationTabs.StatusText = WebString.CreateFromText(mode + " " + theme);

    if (Global.PreferQuirksModeRendering)
      UpdatePanel.Style.Clear();
  }

  protected override object SaveControlState ()
  {
    if (!_currentObjectSaved)
    {
      foreach (IDataEditControl control in _dataEditControls)
        control.DataSource.SaveValues(true);
    }

    return base.SaveControlState();
  }

  private void CancelButton_Click (object sender, EventArgs e)
  {
    ExecuteNextStep();
  }

  private void SaveButton_Click (object sender, EventArgs e)
  {
    MultiView.EnsureAllLazyLoadedViews();
    PrepareValidation();

    // prepare validation for all tabs
    foreach (IDataEditControl control in _dataEditControls)
      control.PrepareValidation();

    // validate all tabs
    foreach (IDataEditControl control in _dataEditControls)
    {
      bool isValid = control.Validate();
      if (! isValid)
        return;
    }

    // save all tabs
    foreach (IDataEditControl control in _dataEditControls)
      control.DataSource.SaveValues(false);

    if (!PerformDomainValidation())
      return;

    ExecuteNextStep();
  }

  private bool PerformDomainValidation ()
  {
    var businessObject = Function.Object;
    var validator = ValidatorProvider.GetValidator(businessObject.GetType());
    var validationResult = validator.Validate(businessObject);

    var combinedValidationResult = new ValidationResult(validationResult.Errors);

    if (!combinedValidationResult.IsValid)
    {
      var businessObjectValidationResult = BusinessObjectValidationResult.Create(combinedValidationResult);
      foreach (var control in _dataEditControls)
      {

        BindableObjectDataSourceControlValidationResultDispatchingValidator dataEditControlDispatchingValidator;
        if (control is TestTabbedPersonDetailsUserControl detailsUserControl)
          dataEditControlDispatchingValidator = detailsUserControl.DataSourceValidationResultDispatchingValidator;
        else if (control is TestTabbedPersonJobsUserControl jobsUserControl)
          dataEditControlDispatchingValidator = jobsUserControl.DataSourceValidationResultDispatchingValidator;
        else
          dataEditControlDispatchingValidator = null;

        dataEditControlDispatchingValidator?.DispatchValidationFailures(businessObjectValidationResult);
        dataEditControlDispatchingValidator?.Validate();
      }
    }

    return combinedValidationResult.IsValid;
  }

  private void MultiView_ActiveViewChanged (object sender, EventArgs e)
  {

  }

  protected override ControlCollection WxeControls
  {
    get { return _wxeControlsPlaceHolder.Controls; }
  }
  private IValidatorProvider ValidatorProvider
  {
    get { return SafeServiceLocator.Current.GetInstance<IValidatorProvider>(); }
  }
}

}
