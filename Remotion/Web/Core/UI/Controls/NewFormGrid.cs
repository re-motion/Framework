using System;
using System.ComponentModel;
using System.Web.UI;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls
{
  [NonVisualControl]
  [ParseChildren(true)]
  [PersistChildren(false)]
  [ToolboxItemFilter("System.Web.UI")]
  public class NewFormGrid : Control, IControl
  {
    private const string c_formGridTag = "form-grid";

    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [Description("The rows of the form grid.")]
    [DefaultValue((string?)null)]
    public NewFormGridRowCollection Rows { get; }

    /// <summary> Enables/Disables the validation markers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ShowValidationMarkers/*' />
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables/Disables the validation markers.")]
    public bool ShowValidationMarkers { get; set; } = true;

    /// <summary> Enables/Disables the required markers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ShowRequiredMarkers/*' />
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables/Disables the required markers.")]
    public bool ShowRequiredMarkers { get; set; } = true;

    /// <summary> Enables/Disables the help providers. </summary>
    /// <include file='..\..\doc\include\UI\Controls\FormGridManager.xml' path='FormGridManager/ShowHelpProviders/*' />
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Enables/Disables the help providers.")]
    public bool ShowHelpProviders { get; set; } = true;

    public NewFormGrid ()
    {
      Rows = new NewFormGridRowCollection(this);
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    protected override void CreateChildControls ()
    {
      base.CreateChildControls();

      foreach (NewFormGridRow row in Rows)
      {
        Controls.Add(row);
      }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);
      EnsureChildControls();
    }

    protected override void Render (HtmlTextWriter writer)
    {
      writer.RenderBeginTag(c_formGridTag); // <form-grid>

      base.Render(writer);

      writer.RenderEndTag(); // </form-grid>
    }
  }
}
