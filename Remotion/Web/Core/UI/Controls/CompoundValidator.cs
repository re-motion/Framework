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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   Base class for compound validators.
/// </summary>
/// <remarks>
///   <para>
///     Compound validators are containers that automatically create child validators according to the 
///     specific requirements of a target control.
///   </para><para>
///     Inheritors should override <see cref="CreateChildValidators"/> to create specific validators. Also,
///     they can override <see cref="ControlToValidate"/> and decorate it with an attribute derived from
///     <see cref="Remotion.Web.UI.Design.ControlToStringConverter"/> in order to provide a pick list in the VS.NET property editor.
///   </para>
/// </remarks>
public abstract class CompoundValidator: WebControl, IBaseValidator
{
  private string _controlToValidate;
  private Type _targetControlType;
  private bool _childValidatorsCreated = false;
  private Style _validatorStyle = new Style();
  private bool _enableClientScript = true;
  private ValidatorDisplay _display = ValidatorDisplay.Dynamic;

  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    this.Page.Validators.Add (this);
  }

  public CompoundValidator (Type targetControlType)
    : base (HtmlTextWriterTag.Span)
  {
    _targetControlType = targetControlType;
  }

  [Category("Behavior")]
  public virtual string ControlToValidate
  {
    get { return _controlToValidate; }
    set { _controlToValidate = value; }
  }

  [Description("Indicates whether to perform validation on the client in up-level browsers.")]
  [Category("Behavior")]
  [DefaultValue (true)]
  [NotifyParentProperty (true)]
  public virtual bool EnableClientScript
  {
    get { return _enableClientScript; }
    set { _enableClientScript = value; }
  }

  [Description("How the validator is displayed.")]
  [Category("Appearance")]
  [DefaultValue (typeof (ValidatorDisplay), "Dynamic")]
  [NotifyParentProperty (true)]
  public virtual ValidatorDisplay Display
  {
    get { return _display; }
    set { _display = value; }
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender (e);

    EnsureChildValidatorsCreated ();
    foreach (Control control in Controls)
    {
      BaseValidator validator = control as BaseValidator;
      validator.EnableClientScript = this.EnableClientScript;
      validator.Display = this.Display;
      if (validator != null)
        validator.ApplyStyle (ValidatorStyle);
    }
  }

  protected override void Render (HtmlTextWriter writer)
  {
    if (this.Site != null && this.Site.DesignMode)
    {
      this.ForeColor = System.Drawing.Color.Red;
      this.RenderBeginTag (writer);
      writer.Write ("[" + this.GetType().Name + " for " + ControlToValidate + "]");
      this.RenderEndTag (writer);
      return;
    }

    Control controlToValidate = NamingContainer.FindControl (ControlToValidate);
    if (controlToValidate == null)
      return;

    if (! _targetControlType.IsAssignableFrom (controlToValidate.GetType()))
    {
      writer.Write ("<b>" + this.GetType().Name + " '" + this.ID + "': ControlToValidate must be a " + _targetControlType + " control.</b>");
      return;
    }

    this.RenderBeginTag (writer);
    RenderChildren (writer);
    this.RenderEndTag (writer);
  }

  public void EnsureChildValidatorsCreated()
  {
    if (!_childValidatorsCreated)
    {
      CreateChildValidators();
      _childValidatorsCreated = true;
    }
  }

  /// <summary>
  ///   Derived classes implement this method to create the contained validator(s).
  /// </summary>
  /// <remarks>
  ///   When implementing this method, apply the <see cref="ValidatorStyle"/> to each created validator.
  /// </remarks>
  protected abstract void CreateChildValidators ();

  [Category("Style")]
  [Description("The style that you want to apply to the contained validators.")]
  [NotifyParentProperty(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.InnerProperty)]
  public Style ValidatorStyle
  {
    get { return _validatorStyle; }
  }

  // IValidator Members

  public void Validate()
  {
    EnsureChildValidatorsCreated();
    foreach (Control control in Controls)
    {
      IValidator validator = control as IValidator;
      if (validator != null)
        validator.Validate();
    }    
  }

  [Browsable(false)]
  public bool IsValid
  {
    get
    {
      EnsureChildValidatorsCreated();
      foreach (Control control in Controls)
      {
        IValidator validator = control as IValidator;
        if (validator != null && ! validator.IsValid)
          return false;
      }    
      return true;
    }
    set { throw new NotSupportedException ("CompoundValidator.IsValid cannot be changed."); }
  }

  public string[] GetErrorMessages()
  {
    EnsureChildValidatorsCreated();
    ArrayList list = new ArrayList (Controls.Count);
    foreach (Control control in Controls)
    {
      IValidator validator = control as IValidator;
      if (validator != null && ! validator.IsValid)
        list.Add (validator.ErrorMessage);
    }    
    return (string[]) list.ToArray (typeof (string));      
  }

  string IValidator.ErrorMessage
  {
    get 
    { 
      string[] messages = GetErrorMessages();
      return string.Join ("<b>", messages);
    }
    set { throw new NotSupportedException ("CompoundValidator.ErrorMessage is not supported."); }
  }
}
}
