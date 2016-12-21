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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.SecurityManager.Clients.Web.UI;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  /// <summary>
  /// <see cref="EditableRowAutoCompleteControlFactory"/> overrides <see cref="EditableRowControlFactory"/> and instantiates 
  /// <see cref="BocAutoCompleteReferenceValue"/> controls instead of <see cref="BocReferenceValue"/> controls for property paths ending in.
  /// <see cref="IBusinessObjectReferenceProperty"/>
  /// </summary>
   /// <remarks>
  /// The <see cref="EditableRowAutoCompleteControlFactory"/> instance is retrieved form the <see cref="IServiceLocator"/> using the type
  /// <see cref="EditableRowAutoCompleteControlFactory"/> as key.
  /// </remarks>

  [ImplementationFor(typeof (EditableRowAutoCompleteControlFactory), Lifetime = LifetimeKind.Singleton)]
  public class EditableRowAutoCompleteControlFactory : EditableRowControlFactory
  {
    public EditableRowAutoCompleteControlFactory ()
    {
    }

    public override void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterHtmlHeadContents (htmlHeadAppender);

      var bocAutoCompleteReferenceValue = new BocAutoCompleteReferenceValue();
      bocAutoCompleteReferenceValue.RegisterHtmlHeadContents (htmlHeadAppender);
    }

    protected override IBusinessObjectBoundEditableWebControl CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      if (IsAutoCompleteReferenceValueRequired (propertyPath))
        return CreateBocAutoCompleteReferenceValue(propertyPath);

      return base.CreateFromPropertyPath (propertyPath);
    }

    protected virtual IBusinessObjectBoundEditableWebControl CreateBocAutoCompleteReferenceValue (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      var control = new BocAutoCompleteReferenceValue();
      SecurityManagerSearchWebService.BindServiceToControl (control);

      if (Is<User> (propertyPath) || Is<Group> (propertyPath))
      {
        control.PreRender += delegate
        {
          BasePage page = (BasePage) control.Page;
          control.Args = page.CurrentFunction.TenantHandle.AsArgument();
        };
      }

      return control;
    }

    protected virtual bool IsAutoCompleteReferenceValueRequired (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      var lastProperty = propertyPath.Properties.Last();
      bool isScalarReferenceProperty = !lastProperty.IsList && lastProperty is IBusinessObjectReferenceProperty;
      if (isScalarReferenceProperty)
        return true;

      return false;
    }

    private bool Is<T> (IBusinessObjectPropertyPath propertyPath)
        where T: OrganizationalStructureObject
    {
      return typeof (T).IsAssignableFrom (propertyPath.Properties.Last().PropertyType);
    }
  }
}