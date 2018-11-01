﻿// This file is part of re-strict (www.re-motion.org)
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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  /// <summary>
  /// <see cref="GroupRolesListEditableRowControlFactory"/> overrides <see cref="EditableRowAutoCompleteControlFactory"/> 
  /// and provides special logic for editing the <b>Roles</b> <see cref="BocList"/> on the <see cref="EditGroupControl"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="GroupRolesListEditableRowControlFactory"/> instance is retrieved form the <see cref="IServiceLocator"/> using the type
  /// <see cref="GroupRolesListEditableRowControlFactory"/> as key.
  /// </remarks>

  [ImplementationFor (typeof (GroupRolesListEditableRowControlFactory), Lifetime = LifetimeKind.Singleton)]
  public class GroupRolesListEditableRowControlFactory : EditableRowAutoCompleteControlFactory
  {
    public GroupRolesListEditableRowControlFactory ()
    {
    }

    protected override IBusinessObjectBoundEditableWebControl CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      if (propertyPath.Identifier == "Position")
        return CreateControlForPosition (propertyPath);
      else
        return base.CreateFromPropertyPath (propertyPath);
    }

    protected virtual BocReferenceValue CreateBocReferenceValue (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      return new BocReferenceValue();
    }

    private IBusinessObjectBoundEditableWebControl CreateControlForPosition (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      var control = CreateBocReferenceValue (propertyPath);
      control.PreRender += HandlePositionPreRender;
      control.EnableSelectStatement = false;
      return control;
    }

    private void HandlePositionPreRender (object sender, EventArgs e)
    {
      var positionReferenceValue = ArgumentUtility.CheckNotNullAndType<BocReferenceValue> ("sender", sender);

      var group = ((Role) positionReferenceValue.DataSource.BusinessObject).Group;
      var positions = positionReferenceValue.Property.SearchAvailableObjects (null, new RolePropertiesSearchArguments (group.GetHandle()));
      positionReferenceValue.SetBusinessObjectList (positions);
      if (!positions.Contains (positionReferenceValue.Value))
        positionReferenceValue.Value = null;
    }
  }
}