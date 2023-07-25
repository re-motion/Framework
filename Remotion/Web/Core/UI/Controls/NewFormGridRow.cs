using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Web.UI.Controls
{
  [NonVisualControl]
  [ParseChildren(true)]
  [PersistChildren(false)]
  [ToolboxItemFilter("System.Web.UI")]
  public class NewFormGridRow : Control, IControlItem
  {
    private const string c_formGridRowTag = "form-grid-row";
    private const string c_formGridLabelTag = "form-grid-label";
    private const string c_formGridIconTag = "form-grid-icon";
    private const string c_formGridIconsTag = "form-grid-icons";
    private const string c_formGridValueTag = "form-grid-value";
    private const string c_formGridErrorTag = "form-grid-error";

    private readonly PlaceHolder _labelControlsPlaceHolder;
    private readonly PlaceHolder _valueControlsPlaceHolder;
    private readonly PlaceHolder _iconsControls;
    private readonly PlaceHolder _helpIconPlaceHolder;
    private readonly PlaceHolder _requiredIconPlaceHolder;
    private readonly PlaceHolder _validatorControls;

    private NewFormGrid? _formGrid;

    [PersistenceMode(PersistenceMode.Attribute)]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public WebString Label { get; set; } = WebString.Empty;

    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ControlCollection IconControls => _iconsControls.Controls;

    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ControlCollection LabelControls => _labelControlsPlaceHolder.Controls;

    [PersistenceMode(PersistenceMode.InnerProperty)]
    public ControlCollection ValueControls => _valueControlsPlaceHolder.Controls;

    [PersistenceMode(PersistenceMode.Attribute)]
    public NewFormGridRowStyle Style { get; set; }

    public NewFormGridRow ()
    {
      _labelControlsPlaceHolder = new PlaceHolder();
      _valueControlsPlaceHolder = new PlaceHolder();
      _iconsControls = new PlaceHolder();
      _helpIconPlaceHolder = new PlaceHolder();
      _requiredIconPlaceHolder = new PlaceHolder();
      _validatorControls = new PlaceHolder();
    }

    private NewFormGrid FormGrid => _formGrid ??= (NewFormGrid)((IControlItem)this).OwnerControl!;

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      EnsureChildControls();
    }

    protected override void CreateChildControls ()
    {
      base.CreateChildControls();

      Controls.Add(_labelControlsPlaceHolder);
      Controls.Add(_valueControlsPlaceHolder);
      Controls.Add(_iconsControls);
      Controls.Add(_helpIconPlaceHolder);
      Controls.Add(_requiredIconPlaceHolder);
      Controls.Add(_validatorControls);

      var controlWithHelpInfo = FilterForVisibleSmartControls(_labelControlsPlaceHolder.Controls)
          .Concat(FilterForVisibleSmartControls(_valueControlsPlaceHolder.Controls))
          .FirstOrDefault(e => e.HelpInfo != null);
      if (controlWithHelpInfo != null)
      {
        var helpInfo = CreateHelpMarker(controlWithHelpInfo.HelpInfo!);
        helpInfo.ID += "_" + controlWithHelpInfo.ID;
        _helpIconPlaceHolder.Controls.Add(helpInfo);
      }

      var isRequired = FilterForVisibleSmartControls(_labelControlsPlaceHolder.Controls)
          .Concat(FilterForVisibleSmartControls(_valueControlsPlaceHolder.Controls))
          .Any(e => e.IsRequired);
      if (isRequired)
      {
        _requiredIconPlaceHolder.Controls.Add(CreateRequiredMarker());
      }

      var firstControl = _valueControlsPlaceHolder.Controls.Cast<Control>()
          .Concat(_labelControlsPlaceHolder.Controls.Cast<Control>())
          .FirstOrDefault();
      if (firstControl != null)
        ID = $"NewFormGridRow_{firstControl.ID}";
    }

    protected override void Render (HtmlTextWriter writer)
    {
      var className = Style switch
      {
          NewFormGridRowStyle.Default => string.Empty,
          NewFormGridRowStyle.SeparateValue => "separate",
          NewFormGridRowStyle.Header => "header",
          _ => throw new ArgumentOutOfRangeException()
      };
      if (!string.IsNullOrEmpty(className))
        writer.AddAttribute(HtmlTextWriterAttribute.Class, className);
      writer.RenderBeginTag(c_formGridRowTag); // <form-grid-row>

      // Label
      writer.RenderBeginTag(c_formGridLabelTag); // <form-grid-label>
      Label.WriteTo(writer); // fixit: default text when none is set
      writer.RenderEndTag(); // </form-grid-label>

      // Custom icons
      writer.RenderBeginTag(c_formGridIconsTag); // <form-grid-icons>
      foreach (Control icon in _iconsControls.Controls)
        icon.RenderControl(writer);
      writer.RenderEndTag(); // </form-grid-icons>

      // Help icon
      writer.RenderBeginTag(c_formGridIconTag); // <form-grid-icon>
      if (FormGrid.ShowHelpProviders)
      {
        foreach (Control helpIcon in _helpIconPlaceHolder.Controls)
          helpIcon.RenderControl(writer);
      }
      writer.RenderEndTag(); // </form-grid-icon>

      // Required icon
      writer.RenderBeginTag(c_formGridIconTag); // <form-grid-icon>
      if (FormGrid.ShowRequiredMarkers) // fixit: Handle special case of validation icon (since they share the column with required)
      {
        foreach (Control requiredIcon in _requiredIconPlaceHolder.Controls)
          requiredIcon.RenderControl(writer);
      }
      writer.RenderEndTag(); // </form-grid-icon>

      // Value controls
      writer.RenderBeginTag(c_formGridValueTag); // <form-grid-value>
      foreach (Control control in ValueControls)
        control.RenderControl(writer);
      writer.RenderEndTag(); // </form-grid-value>

      // fixit: render errors if there are any

      writer.RenderEndTag(); // </form-grid-row>
    }

    private IEnumerable<ISmartControl> FilterForVisibleSmartControls (ControlCollection controls)
    {
      return controls
          .Cast<Control>()
          .Where(e => e.Visible)
          .OfType<ISmartControl>();
    }

    private Control CreateHelpMarker (HelpInfo helpInfo)
    {
      // fixit: localization + image resolving
      ArgumentUtility.CheckNotNull("helpInfo", helpInfo);

      Image helpIcon = new Image();
      helpIcon.ImageUrl = "/git/ObjectBinding.Web.Test/res/Remotion.Web/Themes/NovaViso/Image/sprite.svg#RequiredField";

      helpIcon.AlternateText = "?";
      helpIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      HtmlAnchor helpAnchor = new HtmlAnchor();
      helpAnchor.ID = "HelpLink";
      helpAnchor.Controls.Add(helpIcon);
      helpAnchor.HRef = ResolveClientUrl(helpInfo.NavigateUrl);
      helpAnchor.Target = helpInfo.Target;
      if (!string.IsNullOrEmpty(helpInfo.OnClick))
        helpAnchor.Attributes.Add("onclick", helpInfo.OnClick);

      if (helpInfo.ToolTip == null)
        helpAnchor.Title = "Help";
      else
        helpAnchor.Attributes.Add(HtmlTextWriterAttribute2.AriaLabel, helpInfo.ToolTip);

      return helpAnchor;
    }

    private Control CreateRequiredMarker ()
    {
      // fixit: localization + image resolving
      Image requiredIcon = new Image();
      requiredIcon.ImageUrl = "/git/ObjectBinding.Web.Test/res/Remotion.Web/Themes/NovaViso/Image/sprite.svg#RequiredField";

      requiredIcon.AlternateText = "*";
      requiredIcon.ToolTip = "Input required";
      requiredIcon.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      return requiredIcon;
    }

    IControl? IControlItem.OwnerControl { get; set; } = null;

    string? IControlItem.ItemID { get; } = null;

    void IControlItem.LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
    }
  }
}
