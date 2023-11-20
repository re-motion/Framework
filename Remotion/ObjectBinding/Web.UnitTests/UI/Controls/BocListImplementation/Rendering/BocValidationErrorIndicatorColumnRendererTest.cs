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
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Infrastructure;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocValidationErrorIndicatorColumnRendererTest : ColumnRendererTestBase<BocValidationErrorIndicatorColumnDefinition>
  {
    private BocListCssClassDefinition _bocListCssClassDefinition;
    private BocColumnRenderingContext<BocValidationErrorIndicatorColumnDefinition> _renderingContext;

    private IBocListValidationFailureRepository _validationFailureRepository;
    private IBocColumnRenderer _renderer;

    private StubValidationSummaryRenderer ValidationSummaryRenderer { get; set; }

    [SetUp]
    public override void SetUp ()
    {
      Column = new BocValidationErrorIndicatorColumnDefinition();

      base.SetUp();
      Column.OwnerControl = List.Object;

      _bocListCssClassDefinition = new BocListCssClassDefinition();

      var businessObjectWebServiceContext = BusinessObjectWebServiceContext.Create(null, null, null);
      _renderingContext = new BocColumnRenderingContext<BocValidationErrorIndicatorColumnDefinition>(
          new BocColumnRenderingContext(
              HttpContext,
              Html.Writer,
              List.Object,
              businessObjectWebServiceContext,
              Column,
              ColumnIndexProvider.Object,
              0,
              0));

      _validationFailureRepository = new BocListValidationFailureRepository();
      List.SetupGet(e => e.ValidationFailureRepository).Returns(_validationFailureRepository);

      ValidationSummaryRenderer = new StubValidationSummaryRenderer();

      _renderer = new BocValidationErrorIndicatorColumnRenderer(
          new FakeResourceUrlFactory(),
          RenderingFeatures.Default,
          _bocListCssClassDefinition,
          new FakeFallbackNavigationUrlProvider(),
          new FakeInfrastructureResourceUrlFactory(),
          ValidationSummaryRenderer);
    }

    [Test]
    public void RenderDataColumnDefinition ()
    {
      _renderer.RenderDataColumnDeclaration(_renderingContext, true);
      var document = Html.GetResultDocument();

      var col = Html.GetAssertedChildElement(document, "col", 0);
      Assert.That(col.ChildNodes.Count, Is.EqualTo(0));
      Html.AssertAttribute(col, "class", $"{_bocListCssClassDefinition.DataColumnDeclarationValidationFailureIndicator}");
      Html.AssertNoAttribute(col, "style");
    }

    [Test]
    public void RenderDataColumnDefinition_WithFixedSize_SetsCssWidthAttribute ()
    {
      Column.Width = new Unit(3, UnitType.Em);

      _renderer.RenderDataColumnDeclaration(_renderingContext, true);
      var document = Html.GetResultDocument();

      var col = Html.GetAssertedChildElement(document, "col", 0);
      Assert.That(col.ChildNodes.Count, Is.EqualTo(0));
      Html.AssertAttribute(col, "class", $"{_bocListCssClassDefinition.DataColumnDeclarationValidationFailureIndicator}");
      Html.AssertStyleAttribute(col, "width", "3em");
    }

    [Test]
    public void RenderCellWithoutValidationErrors ()
    {
      _renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", $"{_bocListCssClassDefinition.DataCell} {_bocListCssClassDefinition.DataCellValidationFailureIndicator}");
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      Assert.That(td.ChildNodes.Count, Is.EqualTo(1));
      Html.AssertTextNode(cellStructureDiv, "&nbsp;", 0);
    }

    [Test]
    public void RenderCellWithValidationErrors ()
    {
      AddListValidationFailure("List error should not be displayed");
      var rowValidationFailure = AddRowValidationFailure("Row error");

      ValidationSummaryRenderer.Callback = (BocRenderingContext<IBocList> _, in BocListValidationSummaryRenderArguments arguments) =>
      {
        Assert.That(arguments.ValidationFailures, Is.EqualTo(new [] { rowValidationFailure }));
      };

      _renderer.RenderDataCell(_renderingContext, CreateBocDataCellRenderArguments());
      var document = Html.GetResultDocument();

      var td = Html.GetAssertedChildElement(document, "td", 0);
      Html.AssertAttribute(td, "class", $"{_bocListCssClassDefinition.DataCell} {_bocListCssClassDefinition.DataCellValidationFailureIndicator}");
      Html.AssertAttribute(td, "role", "cell");

      var cellStructureDiv = Html.GetAssertedChildElement(td, "div", 0);
      Html.AssertAttribute(cellStructureDiv, "class", _bocListCssClassDefinition.CellStructureElement);

      var div = Html.GetAssertedChildElement(cellStructureDiv, "div", 0);
      Html.AssertAttribute(div, "class", _bocListCssClassDefinition.Content);
      Html.AssertChildElementCount(div, 2);

      var markerSpan = Html.GetAssertedChildElement(div, "span", 0);
      Html.AssertAttribute(markerSpan, "class", _bocListCssClassDefinition.ValidationErrorMarker);
      Html.AssertChildElementCount(markerSpan, 1);

      var img = Html.GetAssertedChildElement(markerSpan, "img", 0);
      Html.AssertAttribute(img, "src", "/sprite.svg#ValidationError");

      var span = Html.GetAssertedChildElement(div, "span", 1);
      Html.AssertAttribute(span, "class", _bocListCssClassDefinition.CssClassScreenReaderText);
      Html.GetAssertedChildElement(span, "validation-summary", 0);
    }

    private void AddListValidationFailure (string errorMessage)
    {
      var businessObjectValidationFailure = BusinessObjectValidationFailure.CreateForBusinessObject(BusinessObject, errorMessage);
      _validationFailureRepository.AddValidationFailuresForBocList(new[] { businessObjectValidationFailure });
    }

    private BocListValidationFailureWithLocationInformation AddRowValidationFailure (string errorMessage)
    {
      var typeWithReference = (TypeWithReference)BusinessObject;
      var rowObject = typeWithReference.ReferenceList[0];
      var businessObjectValidationFailure = BusinessObjectValidationFailure.CreateForBusinessObject(rowObject, errorMessage);
      _validationFailureRepository.AddValidationFailuresForDataRow(rowObject, new[] { businessObjectValidationFailure });

      return BocListValidationFailureWithLocationInformation.CreateFailureForRow(businessObjectValidationFailure, rowObject);
    }
  }
}
