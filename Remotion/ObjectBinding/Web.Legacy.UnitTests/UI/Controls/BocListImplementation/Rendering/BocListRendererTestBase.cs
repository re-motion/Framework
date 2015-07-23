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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Legacy.UnitTests.Domain;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ListMenuImplementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  public abstract class BocListRendererTestBase : RendererTestBase
  {
    protected IBocList List { get; set; }
    protected IBusinessObject BusinessObject { get; set; }
    protected BocListDataRowRenderEventArgs EventArgs { get; set; }

    protected override void Initialize ()
    {
      Initialize (true);
    }

    protected void Initialize (bool withRowObjects)
    {
      base.Initialize ();

      TypeWithReference businessObject;
      if (withRowObjects)
      {
        businessObject = TypeWithReference.Create (
            TypeWithReference.Create ("referencedObject1"),
            TypeWithReference.Create ("referencedObject2"));
        businessObject.ReferenceList = new[] { businessObject.FirstValue, businessObject.SecondValue };
        
      }
      else
      {
        businessObject = TypeWithReference.Create();
        businessObject.ReferenceList = new TypeWithReference[0];
      }
      BusinessObject = (IBusinessObject) businessObject;
      BusinessObject.BusinessObjectClass.BusinessObjectProvider.AddService<IBusinessObjectWebUIService>
          (new ReflectionBusinessObjectWebUIService ());

      EventArgs = new BocListDataRowRenderEventArgs (10, (IBusinessObject) businessObject.FirstValue, false, true);

      InitializeMockList();
    }

    private void InitializeMockList ()
    {
      List = MockRepository.GenerateMock<IBocList>();

      List.Stub (list => list.ClientID).Return ("MyList");
      List.Stub (list => list.HasClientScript).Return (true);

      List.Stub (list => list.DataSource).Return (MockRepository.GenerateStub<IBusinessObjectDataSource>());
      List.DataSource.BusinessObject = BusinessObject;
      List.Stub (list => list.Property).Return (BusinessObject.BusinessObjectClass.GetPropertyDefinition ("ReferenceList"));

      var value = ((TypeWithReference) BusinessObject).ReferenceList;
      List.Stub (list => list.Value).Return (value);
      List.Stub (list => list.HasValue).Return (value != null && value.Length > 0);

      var listMenuStub = MockRepository.GenerateStub<IListMenu>();
      List.Stub (list => list.ListMenu).Return (listMenuStub);

      StateBag stateBag = new StateBag ();
      List.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      List.Stub (mock => mock.Style).Return (List.Attributes.CssStyle);
      List.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      var page = MockRepository.GenerateMock<IPage>();
      page.Stub (stub => page.Context).Return (HttpContext);
      List.Stub (list => list.Page).Return (page);

      var clientScriptManager = MockRepository.GenerateMock<IClientScriptManager>();
      page.Stub (pageMock => pageMock.ClientScript).Return (clientScriptManager);

      clientScriptManager.Stub (scriptManagerMock => scriptManagerMock.GetPostBackEventReference ((IControl) null, ""))
          .IgnoreArguments().Return ("postBackEventReference");

      clientScriptManager.Stub (scriptManagerMock => scriptManagerMock.GetPostBackEventReference ((PostBackOptions) null))
          .IgnoreArguments().Return ("postBackEventReference");

      var editModeController = MockRepository.GenerateMock<IEditModeController>();
      List.Stub (list => list.EditModeController).Return (editModeController);

      List.Stub (stub => stub.GetSelectorControlName ()).Return ("SelectRowControl$UnqiueID");
      List.Stub (stub => stub.GetSelectAllControlName()).Return ("SelectAllControl$UniqueID");
      List.Stub (stub => stub.GetCurrentPageControlName()).Return ("CurrentPageControl$UniqueID"); // Keep the $-sign as long as the ScalarLoadPostDataTarget is used.

      List.Stub (list => list.GetResourceManager()).Return (
          GlobalizationService.GetResourceManager (typeof (ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier)));

      List.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments ().Do ((Func<string, string>) (url => url.TrimStart ('~')));
    }
  }
}