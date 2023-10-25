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
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Base class for renderers of <see cref="IBocRenderableControl"/> objects.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class BocRendererBase<TControl> : RendererBase<TControl>
      where TControl : IBocRenderableControl, IBusinessObjectBoundWebControl
  {
    protected BocRendererBase (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>bocTextValue</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public abstract string GetCssClassBase (TControl control);

    /// <summary>
    /// Adds class and style attributes found in the <see cref="RenderingContext{TControl}.Control"/> 
    /// to the <paramref name="renderingContext"/> so that they are rendered in the next begin tag.
    /// </summary>
    /// <param name="renderingContext">The <see cref="RenderingContext{TControl}"/>.</param>
    /// <remarks>This automatically adds the CSS classes found in <see cref="CssClassReadOnly"/>
    /// and <see cref="CssClassDisabled"/> if appropriate.</remarks>
    protected void AddAttributesToRender (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      OverrideCssClass(renderingContext, out var backUpCssClass, out var backUpAttributeCssClass);

      AddStandardAttributesToRender(renderingContext);

      RestoreClass(renderingContext, backUpCssClass, backUpAttributeCssClass);

      AddAdditionalAttributes(renderingContext);
    }

    /// <summary>
    /// Called after all attributes have been added by <see cref="AddAttributesToRender"/>.
    /// Use this to render style attributes without putting them into the control's <see cref="IBocRenderableControl.Style"/> property.
    /// </summary>
    protected virtual void AddAdditionalAttributes (RenderingContext<TControl> renderingContext)
    {
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<TControl> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      var control = renderingContext.Control;
      if (!control.DisplayName.IsEmpty)
        HtmlUtility.ExtractPlainText(control.DisplayName).AddAttributeTo(renderingContext.Writer, DiagnosticMetadataAttributesForObjectBinding.DisplayName);

      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.IsDisabled, (!control.Enabled).ToString().ToLower());
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.IsReadOnly, IsReadOnly(control).ToString().ToLower());

      var isBound = IsBoundToBusinessObject(control);
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.IsBound, isBound.ToString().ToLower());

      if (isBound)
      {
        var businessObjectClassIdentifier = GetBusinessObjectClassIdentifier(control);
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BoundType, businessObjectClassIdentifier);

        var boundProperty = control.Property?.Identifier ?? "null";
        renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.BoundProperty, boundProperty);
      }
    }

    /// <summary>
    /// Returns whether the control is bound to a business object. The default implementation checks whether the control is bound to a specific
    /// property of a business object. Derived classes may override this behavior.
    /// </summary>
    /// <param name="control">The control which is checked.</param>
    /// <returns>True if the control is bound to a business object, false otherwise.</returns>
    protected virtual bool IsBoundToBusinessObject (TControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      return control.Property != null && control.DataSource != null
             && (control.DataSource.BusinessObject != null || control.DataSource.BusinessObjectClass != null);
    }

    private string GetBusinessObjectClassIdentifier (TControl control)
    {
      // Try dynamic bound type first, afterwards static bound type. Order due to behavioral uniformity.

      Assertion.IsNotNull(control.DataSource, "control.DataSource must not be null.");

      if (control.DataSource.BusinessObject != null)
        return control.DataSource.BusinessObject.BusinessObjectClass.Identifier;

      if (control.DataSource.BusinessObjectClass != null)
        return control.DataSource.BusinessObjectClass.Identifier;

      throw new NotSupportedException("Cannot determine BusinessObjectClass.Identifier for unbound control.");
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    public virtual string CssClassReadOnly
    {
      get { return CssClassDefinition.ReadOnly; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return CssClassDefinition.Disabled; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to the <see cref="IBocRenderableControl"/> when itself and child elements
    /// that are standard browser controls (e.g. input elements) should be styled in the current theme.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>remotion-themed</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public virtual string CssClassThemed
    {
      get { return CssClassDefinition.Themed; }
    }

    /// <summary>
    /// Gets whether the control establishes a theming context which automatically styles standard browser controls (e.g. input elements)
    /// in the current theme. <see langword="false" /> by default. Derived classes may override this behavior. 
    /// </summary>
    protected virtual bool UseThemingContext
    {
      get { return false; }
    }

    private void OverrideCssClass (RenderingContext<TControl> renderingContext, out string backUpCssClass, out string? backUpAttributeCssClass)
    {
      backUpCssClass = renderingContext.Control.CssClass;
      bool hasCssClass = !string.IsNullOrEmpty(backUpCssClass);
      if (hasCssClass)
        renderingContext.Control.CssClass += GetAdditionalCssClass(renderingContext.Control);

      backUpAttributeCssClass = renderingContext.Control.Attributes["class"];
      bool hasClassAttribute = !string.IsNullOrEmpty(backUpAttributeCssClass);
      if (hasClassAttribute)
        renderingContext.Control.Attributes["class"] += GetAdditionalCssClass(renderingContext.Control);

      if (!hasCssClass && !hasClassAttribute)
        renderingContext.Control.CssClass = GetCssClassBase(renderingContext.Control) + GetAdditionalCssClass(renderingContext.Control);
    }

    private void RestoreClass (RenderingContext<TControl> renderingContext, string backUpCssClass, string? backUpAttributeCssClass)
    {
      renderingContext.Control.CssClass = backUpCssClass;
      renderingContext.Control.Attributes["class"] = backUpAttributeCssClass;
    }

    protected virtual string GetAdditionalCssClass (TControl control)
    {
      var isReadOnly = IsReadOnly(control);
      var isDisabled = !control.Enabled;

      var additionalCssClass = string.Empty;

      if (UseThemingContext)
        additionalCssClass += " " + CssClassThemed;

      if (isReadOnly)
        additionalCssClass += " " + CssClassReadOnly;
      else if (isDisabled)
        additionalCssClass += " " + CssClassDisabled;

      return additionalCssClass;
    }

    private bool IsReadOnly (TControl control)
    {
      // Note: returns always true for controls which are non-editable, effects to rendering are acceptable at the moment.

      var editableControl = control as IBusinessObjectBoundEditableWebControl;
      if (editableControl == null)
        return true;

      return editableControl.IsReadOnly;
    }
  }
}
