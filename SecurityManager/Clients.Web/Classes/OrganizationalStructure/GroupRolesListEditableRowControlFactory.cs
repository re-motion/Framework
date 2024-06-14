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
using System.Linq;
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

  [ImplementationFor(typeof(GroupRolesListEditableRowControlFactory), Lifetime = LifetimeKind.Singleton)]
  public class GroupRolesListEditableRowControlFactory : EditableRowAutoCompleteControlFactory
  {
    public GroupRolesListEditableRowControlFactory ()
    {
    }

    protected override IBusinessObjectBoundEditableWebControl? CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      if (propertyPath.Identifier == "Position")
        return CreateControlForPosition(propertyPath);
      else
        return base.CreateFromPropertyPath(propertyPath);
    }

    protected virtual BocReferenceValue CreateBocReferenceValue (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      return new BocReferenceValue();
    }

    private IBusinessObjectBoundEditableWebControl CreateControlForPosition (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull("propertyPath", propertyPath);

      var control = CreateBocReferenceValue(propertyPath);
      control.PreRender += HandlePositionPreRender;
      control.EnableSelectStatement = false;
      return control;
    }

    private void HandlePositionPreRender (object? sender, EventArgs e)
    {
      var positionReferenceValue = ArgumentUtility.CheckNotNullAndType<BocReferenceValue>("sender", sender!);

      Assertion.IsNotNull(positionReferenceValue.DataSource, "BocReferenceValue{{{0}}}.DataSource != null", positionReferenceValue.ID);
      Assertion.IsNotNull(positionReferenceValue.DataSource.BusinessObject, "BocReferenceValue{{{0}}}.DataSource.BusinessObject != null", positionReferenceValue.ID);
      Assertion.IsNotNull(positionReferenceValue.Property, "BocReferenceValue{{{0}}}.Property != null", positionReferenceValue.ID);

      var role = (Role)positionReferenceValue.DataSource.BusinessObject;
      var group = role.Group;
      Assertion.IsNotNull(group, "Role{{{0}}}.Group != null", role.ID);

      var positions = positionReferenceValue.Property.SearchAvailableObjects(null, new RolePropertiesSearchArguments(group.GetHandle()));
      positionReferenceValue.SetBusinessObjectList(positions);
      if (!positions.Contains(positionReferenceValue.Value))
        positionReferenceValue.Value = null;
    }
  }
}
