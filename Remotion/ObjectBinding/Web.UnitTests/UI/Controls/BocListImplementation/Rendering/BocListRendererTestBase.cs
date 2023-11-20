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
using System.Web.UI.WebControls;
using Moq;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ListMenuImplementation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  public abstract class BocListRendererTestBase : RendererTestBase
  {
    protected Mock<IBocList> List { get; set; }
    protected Mock<IBocListColumnIndexProvider> ColumnIndexProvider { get; set; }
    protected IBusinessObject BusinessObject { get; set; }
    protected BocListDataRowRenderEventArgs EventArgs { get; set; }

    protected override void Initialize ()
    {
      Initialize(true);
    }

    protected void Initialize (bool withRowObjects)
    {
      base.Initialize();

      TypeWithReference businessObject;
      if (withRowObjects)
      {
        businessObject = TypeWithReference.Create(
            TypeWithReference.Create("referencedObject1"),
            TypeWithReference.Create("referencedObject2"));
        businessObject.ReferenceList = new[] { businessObject.FirstValue, businessObject.SecondValue };

      }
      else
      {
        businessObject = TypeWithReference.Create(
            TypeWithReference.Create("referencedObject1"),
            TypeWithReference.Create("referencedObject2"));
        businessObject.ReferenceList = new TypeWithReference[0];
      }

      BusinessObject = (IBusinessObject)businessObject;
      BusinessObject.BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService>(new ReflectionBusinessObjectWebUIService());

      EventArgs = new BocListDataRowRenderEventArgs(10, (IBusinessObject)businessObject.FirstValue, false, true);

      InitializeMockList();
    }

    private void InitializeMockList ()
    {
      List = new Mock<IBocList>();

      List.Setup(list => list.ClientID).Returns("MyList");
      List.Setup(mock => mock.ControlType).Returns("BocList");
      List.Setup(list => list.HasClientScript).Returns(true);
      List.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton("Label"));
      List
          .Setup(mock => mock.GetValidationErrors())
          .Returns(EnumerableUtility.Singleton(PlainTextString.CreateFromText("ValidationError")));

      var dataSourceMock = new Mock<IBusinessObjectDataSource>();
      dataSourceMock.SetupProperty(_ => _.BusinessObject);
      List.Setup(list => list.DataSource).Returns(dataSourceMock.Object);
      List.Object.DataSource.BusinessObject = BusinessObject;
      List.Setup(list => list.Property).Returns(BusinessObject.BusinessObjectClass.GetPropertyDefinition("ReferenceList"));

      var value = ((TypeWithReference)BusinessObject).ReferenceList;
      List.Setup(list => list.Value).Returns(value);
      List.Setup(list => list.HasValue).Returns(value != null && value.Length > 0);

      var listMenuStub = new Mock<IListMenu>();
      listMenuStub.SetupProperty(_ => _.Visible);
      List.Setup(list => list.ListMenu).Returns(listMenuStub.Object);

      StateBag stateBag = new StateBag();
      List.Setup(mock => mock.Attributes).Returns(new AttributeCollection(stateBag));
      List.Setup(mock => mock.Style).Returns(List.Object.Attributes.CssStyle);
      List.Setup(mock => mock.ControlStyle).Returns(new Style(stateBag));

      var page = new Mock<IPage>();
      page.Setup(_ => _.Context).Returns(HttpContext);
      List.Setup(list => list.Page).Returns(page.Object);

      var clientScriptManager = new Mock<IClientScriptManager>();
      page.Setup(pageMock => pageMock.ClientScript).Returns(clientScriptManager.Object);

      clientScriptManager.Setup(scriptManagerMock => scriptManagerMock.GetPostBackEventReference(It.IsAny<IControl>(), It.IsAny<string>())).Returns("postBackEventReference");

      clientScriptManager.Setup(scriptManagerMock => scriptManagerMock.GetPostBackEventReference(It.IsAny<PostBackOptions>())).Returns("postBackEventReference");

      var editModeController = new Mock<IEditModeController>();
      List.Setup(list => list.EditModeController).Returns(editModeController.Object);

      List.Setup(stub => stub.GetSelectorControlName()).Returns("SelectRowControl$UnqiueID");
      List.Setup(stub => stub.GetSelectAllControlName()).Returns("SelectAllControl$UniqueID");
      List.Setup(stub => stub.GetCurrentPageControlName()).Returns("CurrentPageControl$UniqueID"); // Keep the $-sign as long as the ScalarLoadPostDataTarget is used.

      List
          .Setup(list => list.GetResourceManager())
          .Returns(GlobalizationService.GetResourceManager(typeof(ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier)));

      List.Setup(stub => stub.ResolveClientUrl(It.IsAny<string>())).Returns((string url) => url.TrimStart('~'));

      var bocListValidationFailureRepository = new BocListValidationFailureRepository();
      List.Setup(_ => _.ValidationFailureRepository).Returns(bocListValidationFailureRepository);

      ColumnIndexProvider = new Mock<IBocListColumnIndexProvider>();
    }

    protected BocTitleCellRenderArguments CreateBocTitleCellRenderArguments (
        SortingDirection sortingDirection = SortingDirection.None,
        int orderIndex = -1,
        string cellID = null,
        bool isRowHeader = false)
    {
      return new BocTitleCellRenderArguments(sortingDirection, orderIndex, cellID ?? "TitleCellID", isRowHeader);
    }

    protected BocDataCellRenderArguments CreateBocDataCellRenderArguments (
        BocListDataRowRenderEventArgs dataRowRenderEventArgs = null,
        int rowIndex = 0,
        bool showIcon = false,
        string cellID = null,
        IReadOnlyCollection<string> headerIDs = null,
        bool[] columnsWithValidationFailures = null)
    {
      // If no columns are specified, we don't know how many columns are actually needed so we use 30 to be safe
      // Most tests don't care so this is fine, otherwise the tests need to provide a valid argument instead.
      var defaultColumnsWithValidationFailures = new bool[30];

      return new BocDataCellRenderArguments(
          dataRowRenderEventArgs ?? EventArgs,
          rowIndex,
          showIcon,
          cellID,
          headerIDs ?? Array.Empty<string>(),
          columnsWithValidationFailures ?? defaultColumnsWithValidationFailures);
    }
  }
}
