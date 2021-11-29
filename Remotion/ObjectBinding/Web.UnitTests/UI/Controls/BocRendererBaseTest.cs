﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocRendererBaseTest : RendererTestBase
  {
    // Enables testing of abstract BocRendererBase<TControl> class
    private class TestableBocRendererBase : BocRendererBase<TestableBocControl>
    {
      public TestableBocRendererBase (
          IResourceUrlFactory resourceUrlFactory,
          IGlobalizationService globalizationService,
          IRenderingFeatures renderingFeatures)
          : base (resourceUrlFactory, globalizationService, renderingFeatures)
      {
      }

      public override string GetCssClassBase (TestableBocControl control)
      {
        return "cssClassBase";
      }

      // Enables testing of AddDiagnosticMetadataAttributes method
      public new void AddDiagnosticMetadataAttributes (RenderingContext<TestableBocControl> renderingContext)
      {
        base.AddDiagnosticMetadataAttributes(renderingContext);
      }
    }

    private class TestableBocControl : BusinessObjectBoundEditableWebControl, IBocRenderableControl
    {
      public override void LoadValue (bool interim)
      {
      }

      protected override object ValueImplementation { get; set; }

      public override bool HasValue
      {
        get { return false; }
      }

      public override string[] GetTrackedClientIDs ()
      {
        return new string[0];
      }

      public override bool UseLabel
      {
        get { return false; }
      }

      public override bool SaveValue (bool interim)
      {
        return true;
      }

      protected override IEnumerable<BaseValidator> CreateValidators (bool isReadOnly)
      {
        return Enumerable.Empty<BaseValidator>();
      }

      protected override IBusinessObjectConstraintVisitor CreateBusinessObjectConstraintVisitor ()
      {
        return NullBusinessObjectConstraintVisitor.Instance;
      }

      IEnumerable<string> IControlWithLabel.GetLabelIDs ()
      {
        return Enumerable.Empty<string>();
      }

      string IControlWithDiagnosticMetadata.ControlType
      {
        get { return "TestableBocControl"; }
      }
    }

    private TestableBocRendererBase BocRendererBase { get; set; }

    private TestableBocControl Control { get; set; }
    private IResourceUrlFactory _resourceUrlFactory;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      Control = new TestableBocControl();
      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [Test]
    public void TestDiagnosticMetadataRenderingForUnboundControl ()
    {
      Control.ReadOnly = true;
      var renderingContext = CreateRenderingContext();
      BocRendererBase = new TestableBocRendererBase(_resourceUrlFactory, GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      BocRendererBase.AddDiagnosticMetadataAttributes(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      var control = document.DocumentElement;
      control.AssertNoAttribute(DiagnosticMetadataAttributesForObjectBinding.DisplayName);
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsReadOnly, "true");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, "false");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.IsBound, "false");
      control.AssertNoAttribute(DiagnosticMetadataAttributesForObjectBinding.BoundType);
      control.AssertNoAttribute(DiagnosticMetadataAttributesForObjectBinding.BoundProperty);
    }

    [Test]
    public void TestDiagnosticMetadataRenderingForBoundControl_DataSourceWithBusinessObject ()
    {
      var businessObject = TypeWithReference.Create("MyBusinessObject");
      ((IBusinessObjectBoundControl) Control).Property =
          ((IBusinessObject) businessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceValue");

      var dataSource = new BindableObjectDataSource { Type = typeof (TypeWithReference) };
      dataSource.BusinessObject = (IBusinessObject) businessObject;
      dataSource.Register(Control);
      Control.DataSource = dataSource;

      var renderingContext = CreateRenderingContext();
      BocRendererBase = new TestableBocRendererBase(_resourceUrlFactory, GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      BocRendererBase.AddDiagnosticMetadataAttributes(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      var control = document.DocumentElement;
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.DisplayName, "ReferenceValue");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsReadOnly, "false");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributes.IsDisabled, "false");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.IsBound, "true");
      control.AssertAttributeValueEquals(
          DiagnosticMetadataAttributesForObjectBinding.BoundType,
          "Remotion.ObjectBinding.Web.UnitTests.Domain.TypeWithReference, Remotion.ObjectBinding.Web.UnitTests");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.BoundProperty, "ReferenceValue");
    }

    [Test]
    public void TestDiagnosticMetadataRenderingForBoundControl_DataSourceWithoutBusinessObject ()
    {
      var businessObject = TypeWithReference.Create("MyBusinessObject");
      ((IBusinessObjectBoundControl) Control).Property =
          ((IBusinessObject) businessObject).BusinessObjectClass.GetPropertyDefinition("ReferenceValue");

      var dataSource = new BindableObjectDataSource { Type = typeof (TypeWithReference) };
      dataSource.Register(Control);
      Control.DataSource = dataSource;

      var renderingContext = CreateRenderingContext();
      BocRendererBase = new TestableBocRendererBase(_resourceUrlFactory, GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);

      BocRendererBase.AddDiagnosticMetadataAttributes(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      renderingContext.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      var control = document.DocumentElement;
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.IsBound, "true");
      control.AssertAttributeValueEquals(
          DiagnosticMetadataAttributesForObjectBinding.BoundType,
          "Remotion.ObjectBinding.Web.UnitTests.Domain.TypeWithReference, Remotion.ObjectBinding.Web.UnitTests");
      control.AssertAttributeValueEquals(DiagnosticMetadataAttributesForObjectBinding.BoundProperty, "ReferenceValue");
    }

    private RenderingContext<TestableBocControl> CreateRenderingContext ()
    {
      return new BocRenderingContext<TestableBocControl>(HttpContext, Html.Writer, Control);
    }
  }
}