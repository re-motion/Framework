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
using Remotion.FunctionalProgramming;
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
  /// <see cref="UserRolesListEditableRowControlFactory"/> overrides <see cref="EditableRowAutoCompleteControlFactory"/> 
  /// and provides special logic for editing the <b>Roles</b> <see cref="BocList"/> on the <see cref="EditUserControl"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="UserRolesListEditableRowControlFactory"/> instance is retrieved form the <see cref="IServiceLocator"/> using the type
  /// <see cref="UserRolesListEditableRowControlFactory"/> as key.
  /// </remarks>
  [ImplementationFor (typeof (UserRolesListEditableRowControlFactory), Lifetime = LifetimeKind.Singleton)]
  public class UserRolesListEditableRowControlFactory : EditableRowAutoCompleteControlFactory
  {
    public UserRolesListEditableRowControlFactory ()
    {
    }

    protected override IBusinessObjectBoundEditableWebControl CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      if (propertyPath.Identifier == "Group")
        return CreateControlForGroup (propertyPath);
      else if (propertyPath.Identifier == "Position")
        return CreateControlForPosition (propertyPath);
      else
        return base.CreateFromPropertyPath (propertyPath);
    }

    protected virtual BocReferenceValue CreateBocReferenceValue (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      return new BocReferenceValue();
    }

    private IBusinessObjectBoundEditableWebControl CreateControlForGroup (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      var control = base.CreateFromPropertyPath (propertyPath);
      if (control is BocAutoCompleteReferenceValue)
      {
        var referenceValue = (BocAutoCompleteReferenceValue) control;
        referenceValue.TextBoxStyle.AutoPostBack = true;
        return referenceValue;
      }
      else if (control is BocReferenceValue)
      {
        var referenceValue = (BocReferenceValue) control;
        referenceValue.DropDownListStyle.AutoPostBack = true;
        return referenceValue;
      }
      else
        throw new InvalidOperationException (string.Format ("Control type '{0}' is not supported for property 'Group'", control.GetType()));
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

      var groupReferenceValue = GetGroupReferenceValue (positionReferenceValue.DataSource);

      var group = (Group) groupReferenceValue.Value;
      if (group == null)
      {
        positionReferenceValue.ClearBusinessObjectList();
        positionReferenceValue.Enabled = false;
      }
      else
      {
        var positions = positionReferenceValue.Property.SearchAvailableObjects (null, new RolePropertiesSearchArguments (group.GetHandle()));
        positionReferenceValue.SetBusinessObjectList (positions);
        positionReferenceValue.Enabled = true;
      }
    }

    private IBusinessObjectBoundControl GetGroupReferenceValue (IBusinessObjectDataSource dataSource)
    {
      var groupProperty = dataSource.BusinessObject.BusinessObjectClass.GetPropertyDefinition ("Group");
      return dataSource.GetBoundControlsWithValidBinding()
          .Where (c => c.Property == groupProperty)
          .Single (() => new InvalidOperationException ("Expected control bound to property 'Group' was not found"));
    }
  }
}