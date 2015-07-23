// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.UI;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  /// <summary>
  /// <see cref="BocListInlineEditingConfigurator"/> is used to set up <see cref="BocList"/> instances for inline editing when used in the 
  /// <see cref="Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure"/> editors.
  /// </summary>
  /// <remarks>
  /// The <see cref="BocListInlineEditingConfigurator"/> instance is retrieved form the <see cref="IServiceLocator"/> using the type
  /// <see cref="BocListInlineEditingConfigurator"/> as key.
  /// </remarks>
  [ImplementationFor(typeof (BocListInlineEditingConfigurator), Lifetime = LifetimeKind.Singleton)]
  public class BocListInlineEditingConfigurator
  {
    private readonly IResourceUrlFactory _resourceUrlFactory;
    private readonly IGlobalizationService _globalizationService;

    public BocListInlineEditingConfigurator (IResourceUrlFactory resourceUrlFactory, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      _resourceUrlFactory = resourceUrlFactory;
      _globalizationService = globalizationService;
    }

    public virtual void Configure<TBusinessObject> (BocList bocList, Func<TBusinessObject> newObjectFactory)
        where TBusinessObject: BaseSecurityManagerObject
    {
      ArgumentUtility.CheckNotNull ("bocList", bocList);
      ArgumentUtility.CheckNotNull ("newObjectFactory", newObjectFactory);

      if (!ControlHelper.IsDesignMode (bocList))
      {
        bocList.FixedColumns.Insert (
            0,
            new BocRowEditModeColumnDefinition
            {
                Width = Unit.Pixel (40),
                EditIcon = GetIcon ("EditItem.gif", GlobalResourcesHelper.GetString (_globalizationService, GlobalResources.Edit)),
                SaveIcon = GetIcon("ApplyButton.gif", GlobalResourcesHelper.GetString (_globalizationService, GlobalResources.Apply)),
                CancelIcon = GetIcon("CancelButton.gif", GlobalResourcesHelper.GetString (_globalizationService, GlobalResources.Cancel))
            });

        bocList.EditableRowChangesCanceled += HandleEditableRowChangesCanceled;

        bocList.ListMenuItems.Add (
            new BocMenuItem
            {
              Text = GlobalResourcesHelper.GetString (_globalizationService, GlobalResources.New),
                Command = new InlineEditingNewItemMenuItemCommand<TBusinessObject> (newObjectFactory)
            });
        bocList.ListMenuItems.Add (
            new BocMenuItem
            {
              Text = GlobalResourcesHelper.GetString (_globalizationService, GlobalResources.Delete),
                RequiredSelection = RequiredSelection.OneOrMore,
                Command = new InlineEditingDeleteItemMenuItemCommand<TBusinessObject>()
            });
      }
    }

    private void HandleEditableRowChangesCanceled (object sender, BocListItemEventArgs e)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      ArgumentUtility.CheckNotNull ("e", e);

      var businessObject = (BaseSecurityManagerObject) e.BusinessObject;
      if (businessObject.State == StateType.New)
        businessObject.Delete();
    }

    private IconInfo GetIcon (string resourceUrl, string alternateText)
    {
      var url = _resourceUrlFactory.CreateThemedResourceUrl (typeof (BocListInlineEditingConfigurator), ResourceType.Image, resourceUrl).GetUrl();
      return new IconInfo (url) { AlternateText = alternateText };
    }
  }
}