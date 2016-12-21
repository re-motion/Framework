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
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// The <see cref="ControlWrapper"/> type is the default implementation of the <see cref="IControl"/> interface.
  /// </summary>
  public class ControlWrapper : IControl
  {
    private readonly Control _control;

    public ControlWrapper (Control control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      _control = control;
    }

    /// <summary>
    /// Gets the concrete instance wrapped by this <see cref="IControl"/> wrapper.
    /// </summary>
    /// <exception cref="NotSupportedException">This is a stub implementation which does not contain an <see cref="Control"/>. </exception>
    public Control WrappedInstance
    {
      get { return _control; }
    }

    /// <summary>
    /// When implemented by an ASP.NET server control, notifies the server control that an element, either XML or HTML, was parsed.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object"/> that was parsed. 
    /// </param>
    void IParserAccessor.AddParsedSubObject (object obj)
    {
      ((IParserAccessor) _control).AddParsedSubObject (obj);
    }

    /// <summary>
    /// Gets a collection of all data bindings on the control. This property is read-only.
    /// </summary>
    /// <returns>
    /// The collection of data bindings.
    /// </returns>
    DataBindingCollection IDataBindingsAccessor.DataBindings
    {
      get { return ((IDataBindingsAccessor) _control).DataBindings; }
    }

    /// <summary>
    /// Gets a value indicating whether the control contains any data-binding logic.
    /// </summary>
    /// <returns>
    /// true if the control contains data binding logic.
    /// </returns>
    bool IDataBindingsAccessor.HasDataBindings
    {
      get { return ((IDataBindingsAccessor) _control).HasDataBindings; }
    }

    /// <summary>
    /// Gets the control builder for this control.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.ControlBuilder"/> that built the control; otherwise, null if no builder was used.
    /// </returns>
    ControlBuilder IControlBuilderAccessor.ControlBuilder
    {
      get { return ((IControlBuilderAccessor) _control).ControlBuilder; }
    }

    /// <summary>
    /// When implemented, gets the state from the control during use on the design surface.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IDictionary"/> of the control state.
    /// </returns>
    IDictionary IControlDesignerAccessor.GetDesignModeState ()
    {
      return ((IControlDesignerAccessor) _control).GetDesignModeState();
    }

    /// <summary>
    /// When implemented, sets control state before rendering it on the design surface.
    /// </summary>
    /// <param name="data">The <see cref="T:System.Collections.IDictionary"/> containing the control state.
    /// </param>
    void IControlDesignerAccessor.SetDesignModeState (IDictionary data)
    {
      ((IControlDesignerAccessor) _control).SetDesignModeState (data);
    }

    /// <summary>
    /// When implemented, specifies the control that acts as the owner to the control implementing this method.
    /// </summary>
    /// <param name="owner">The control to act as owner.
    /// </param>
    void IControlDesignerAccessor.SetOwnerControl (Control owner)
    {
      ((IControlDesignerAccessor) _control).SetOwnerControl (owner);
    }

    /// <summary>
    /// When implemented, gets a collection of information that can be accessed by a control designer.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IDictionary"/> containing information about the control.
    /// </returns>
    IDictionary IControlDesignerAccessor.UserData
    {
      get { return ((IControlDesignerAccessor) _control).UserData; }
    }

    /// <summary>
    /// Gets a value indicating whether the instance of the class that implements this interface has any properties bound by an expression.
    /// </summary>
    /// <returns>
    /// true if the control has properties set through expressions; otherwise, false. 
    /// </returns>
    bool IExpressionsAccessor.HasExpressions
    {
      get { return ((IExpressionsAccessor) _control).HasExpressions; }
    }

    /// <summary>
    /// Gets a collection of <see cref="T:System.Web.UI.ExpressionBinding"/> objects.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Web.UI.ExpressionBindingCollection"/> containing <see cref="T:System.Web.UI.ExpressionBinding"/> objects that represent the properties and expressions for a control.
    /// </returns>
    ExpressionBindingCollection IExpressionsAccessor.Expressions
    {
      get { return ((IExpressionsAccessor) _control).Expressions; }
    }

    /// <summary>
    /// Applies the style properties defined in the page style sheet to the control.
    /// </summary>
    /// <param name="page">The <see cref="T:System.Web.UI.Page"/> containing the control.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">The style sheet is already applied.
    /// </exception>
    public void ApplyStyleSheetSkin (Page page)
    {
      _control.ApplyStyleSheetSkin (page);
    }

    /// <summary>
    /// Binds a data source to the invoked server control and all its child controls.
    /// </summary>
    public void DataBind ()
    {
      _control.DataBind();
    }

    /// <summary>
    /// Sets input focus to a control.
    /// </summary>
    public void Focus ()
    {
      _control.Focus();
    }

    /// <summary>
    /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object and stores tracing information about the control if tracing is enabled.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HTmlTextWriter"/> object that receives the control content. 
    /// </param>
    public void RenderControl (HtmlTextWriter writer)
    {
      _control.RenderControl (writer);
    }

    /// <summary>
    /// Enables a server control to perform final clean up before it is released from memory.
    /// </summary>
    public void Dispose ()
    {
      _control.Dispose();
    }

    /// <summary>
    /// Converts a URL into one that is usable on the requesting client.
    /// </summary>
    /// <returns>
    /// The converted URL.
    /// </returns>
    /// <param name="relativeUrl">The URL associated with the <see cref="P:System.Web.UI.Control.TemplateSourceDirectory"/> property. 
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">Occurs if the <paramref name="relativeUrl"/> parameter contains null. 
    /// </exception>
    public string ResolveUrl (string relativeUrl)
    {
      return _control.ResolveUrl (relativeUrl);
    }

    /// <summary>
    /// Gets a URL that can be used by the browser.
    /// </summary>
    /// <returns>
    /// A fully qualified URL to the specified resource suitable for use on the browser.
    /// </returns>
    /// <param name="relativeUrl">A URL relative to the current page.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="relativeUrl"/> is null.
    /// </exception>
    public string ResolveClientUrl (string relativeUrl)
    {
      return _control.ResolveClientUrl (relativeUrl);
    }

    /// <summary>
    /// Searches the current naming container for a server control with the specified <paramref name="id"/> parameter.
    /// </summary>
    /// <returns>
    /// The specified control, or null if the specified control does not exist.
    /// </returns>
    /// <param name="id">The identifier for the control to be found. 
    /// </param>
    public Control FindControl (string id)
    {
      return _control.FindControl (id);
    }

    /// <summary>
    /// Determines if the server control contains any child controls.
    /// </summary>
    /// <returns>
    /// true if the control contains other controls; otherwise, false.
    /// </returns>
    public bool HasControls ()
    {
      return _control.HasControls();
    }

    /// <summary>
    /// Assigns an event handler delegate to render the server control and its content into its parent control.
    /// </summary>
    /// <param name="renderMethod">The information necessary to pass to the delegate so that it can render the server control. 
    /// </param>
    public void SetRenderMethodDelegate (RenderMethod renderMethod)
    {
      _control.SetRenderMethodDelegate (renderMethod);
    }

    /// <summary>
    /// Gets the server control identifier generated by ASP.NET.
    /// </summary>
    /// <returns>
    /// The server control identifier generated by ASP.NET.
    /// </returns>
    public string ClientID
    {
      get { return _control.ClientID; }
    }

    /// <summary>
    /// Gets or sets the programmatic identifier assigned to the server control.
    /// </summary>
    /// <returns>
    /// The programmatic identifier assigned to the control.
    /// </returns>
    public string ID
    {
      get { return _control.ID; }
      set { _control.ID = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether themes apply to this control.
    /// </summary>
    /// <returns>
    /// true to use themes; otherwise, false. The default is true. 
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The Page_PreInit event has already occurred.
    ///     <para>- or -</para>
    ///     The control has already been added to the Controls collection.
    /// </exception>
    public bool EnableTheming
    {
      get { return _control.EnableTheming; }
      set { _control.EnableTheming = value; }
    }

    /// <summary>
    /// Gets or sets the skin to apply to the control.
    /// </summary>
    /// <returns>
    /// The name of the skin to apply to the control. The default is <see cref="F:System.String.Empty"/>.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The style sheet has already been applied.
    ///     <para>- or -</para>
    ///     The Page_PreInit event has already occurred.
    ///     <para>- or -</para>
    ///     The control was already added to the Controls collection.
    /// </exception>
    public string SkinID
    {
      get { return _control.SkinID; }
      set { _control.SkinID = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the server control persists its view state, and the view state of any child controls it contains, to the requesting client.
    /// </summary>
    /// <returns>
    /// true if the server control maintains its view state; otherwise false. The default is true.
    /// </returns>
    public bool EnableViewState
    {
      get { return _control.EnableViewState; }
      set { _control.EnableViewState = value; }
    }

    /// <summary>
    /// Gets a reference to the server control's naming container, which creates a unique namespace for differentiating between server controls with the same <see cref="P:System.Web.UI.Control.ID"/> property value.
    /// </summary>
    /// <returns>
    /// The server control's naming container.
    /// </returns>
    public Control NamingContainer
    {
      get { return _control.NamingContainer; }
    }

    /// <summary>
    /// Gets the control that contains this control's data binding.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Control"/> that contains this control's data binding.
    /// </returns>
    public Control BindingContainer
    {
      get { return _control.BindingContainer; }
    }

    /// <summary>
    /// Gets a reference to the <see cref="T:System.Web.UI.Page"/> instance that contains the server control.
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.Page"/> instance that contains the server control.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The control is a <see cref="T:System.Web.UI.WebControls.Substitution"/> control.
    /// </exception>
    public virtual IPage Page
    {
      get { return PageWrapper.CastOrCreate (_control.Page); }
    }

    /// <summary>
    /// Gets or sets a reference to the template that contains this control. 
    /// </summary>
    /// <returns>
    /// The <see cref="T:System.Web.UI.TemplateControl"/> instance that contains this control. 
    /// </returns>
    public TemplateControl TemplateControl
    {
      get { return _control.TemplateControl; }
      set { _control.TemplateControl = value; }
    }

    /// <summary>
    /// Gets a reference to the server control's parent control in the page control hierarchy.
    /// </summary>
    /// <returns>
    /// A reference to the server control's parent control.
    /// </returns>
    public Control Parent
    {
      get { return _control.Parent; }
    }

    /// <summary>
    /// Gets the virtual directory of the <see cref="T:System.Web.UI.Page"/> or <see cref="T:System.Web.UI.UserControl"/> that contains the current server control.
    /// </summary>
    /// <returns>
    /// The virtual directory of the page or user control that contains the server control.
    /// </returns>
    public string TemplateSourceDirectory
    {
      get { return _control.TemplateSourceDirectory; }
    }

    /// <summary>
    /// Gets or sets the application-relative virtual directory of the <see cref="T:System.Web.UI.Page"/> or <see cref="T:System.Web.UI.UserControl"/> object that contains this control.
    /// </summary>
    /// <returns>
    /// The application-relative virtual directory of the page or user control that contains this control.
    /// </returns>
    public string AppRelativeTemplateSourceDirectory
    {
      get { return _control.AppRelativeTemplateSourceDirectory; }
      set { _control.AppRelativeTemplateSourceDirectory = value; }
    }

    /// <summary>
    /// Gets information about the container that hosts the current control when rendered on a design surface.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.ComponentModel.ISite"/> that contains information about the container that the control is hosted in.
    /// </returns>
    /// <exception cref="T:System.InvalidOperationException">The control is a <see cref="T:System.Web.UI.WebControls.Substitution"/> control.
    /// </exception>
    public ISite Site
    {
      get { return _control.Site; }
      set { _control.Site = value; }
    }

    /// <summary>
    /// Gets or sets a value that indicates whether a server control is rendered as UI on the page.
    /// </summary>
    /// <returns>
    /// true if the control is visible on the page; otherwise false.
    /// </returns>
    public bool Visible
    {
      get { return _control.Visible; }
      set { _control.Visible = value; }
    }

    /// <summary>
    /// Gets the unique, hierarchically qualified identifier for the server control.
    /// </summary>
    /// <returns>
    /// The fully qualified identifier for the server control.
    /// </returns>
    public string UniqueID
    {
      get { return _control.UniqueID; }
    }

    /// <summary>
    /// Gets a <see cref="T:System.Web.UI.ControlCollection"/> object that represents the child controls for a specified server control in the UI hierarchy.
    /// </summary>
    /// <returns>
    /// The collection of child controls for the specified server control.
    /// </returns>
    public ControlCollection Controls
    {
      get { return _control.Controls; }
    }

    public event EventHandler Disposed
    {
      add { _control.Disposed += value; }
      remove { _control.Disposed -= value; }
    }

    public event EventHandler DataBinding
    {
      add { _control.DataBinding += value; }
      remove { _control.DataBinding -= value; }
    }

    public event EventHandler Init
    {
      add { _control.Init += value; }
      remove { _control.Init -= value; }
    }

    public event EventHandler Load
    {
      add { _control.Load += value; }
      remove { _control.Load -= value; }
    }

    public event EventHandler PreRender
    {
      add { _control.PreRender += value; }
      remove { _control.PreRender -= value; }
    }

    public event EventHandler Unload
    {
      add { _control.Unload += value; }
      remove { _control.Unload -= value; }
    }

    public override string ToString ()
    {
      return _control.ToString ();
    }
  }
}
