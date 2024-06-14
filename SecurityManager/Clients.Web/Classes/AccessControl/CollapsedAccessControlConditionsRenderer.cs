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
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.AccessControl
{
  /// <summary>
  /// The <see cref="CollapsedAccessControlConditionsRenderer"/> type is responsible for generating the collapsed view on an <see cref="AccessControlEntry"/>
  /// </summary>
  public class CollapsedAccessControlConditionsRenderer
  {
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl.AccessControlResources")]
    public enum ResourceIdentifier
    {
      TenantHierarchyCondition_This,
      TenantHierarchyCondition_ThisAndParent,
      GroupHierarchyCondition_This,
      GroupHierarchyCondition_ThisAndParent,
      GroupHierarchyCondition_ThisAndChildren,
      GroupHierarchyCondition_ThisAndParentAndChildren,
      TenantCondition_None,
      UserCondition_None,
      GroupCondition_None,
      BranchOfOwningGroupLabel,
    }

    private IResourceManager ResourceManager
    {
      get { return _globalizationService.GetResourceManager(typeof(ResourceIdentifier)); }
    }

    private readonly AccessControlEntry _accessControlEntry;
    private readonly IResourceUrlFactory _resourceUrlFactory;
    private readonly IGlobalizationService _globalizationService;

    public CollapsedAccessControlConditionsRenderer (
        AccessControlEntry accessControlEntry,
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("accessControlEntry", accessControlEntry);
      ArgumentUtility.CheckNotNull("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      _accessControlEntry = accessControlEntry;
      _resourceUrlFactory = resourceUrlFactory;
      _globalizationService = globalizationService;
    }

    public AccessControlEntry AccessControlEntry
    {
      get { return _accessControlEntry; }
    }

    public void RenderTenant (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("container", container);

      switch (_accessControlEntry.TenantCondition)
      {
        case TenantCondition.None:
          ResourceManager.GetText(ResourceIdentifier.TenantCondition_None).WriteTo(writer);
          break;
        case TenantCondition.OwningTenant:
          RenderTenantHierarchyIcon(writer, container);
          RenderPropertyPathString(writer, "TenantCondition");
          break;
        case TenantCondition.SpecificTenant:
          RenderTenantHierarchyIcon(writer, container);
          RenderLabelAfterPropertyPathString(writer, "SpecificTenant.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderGroup (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("container", container);

      switch (_accessControlEntry.GroupCondition)
      {
        case GroupCondition.None:
          ResourceManager.GetText(ResourceIdentifier.GroupCondition_None).WriteTo(writer);
          break;
        case GroupCondition.OwningGroup:
          RenderGroupHierarchyIcon(writer, container);
          RenderPropertyPathString(writer, "GroupCondition");
          break;
        case GroupCondition.SpecificGroup:
          RenderGroupHierarchyIcon(writer, container);
          RenderLabelAfterPropertyPathString(writer, "SpecificGroup.ShortName");
          break;
        case GroupCondition.BranchOfOwningGroup:
          RenderLabelBeforePropertyPathString(writer, ResourceManager.GetText(ResourceIdentifier.BranchOfOwningGroupLabel), "SpecificGroupType.DisplayName");
          break;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          RenderLabelAfterPropertyPathString(writer, "SpecificGroupType.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderUser (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("container", container);

      switch (_accessControlEntry.UserCondition)
      {
        case UserCondition.None:
          ResourceManager.GetText(ResourceIdentifier.UserCondition_None).WriteTo(writer);
          break;
        case UserCondition.Owner:
          RenderPropertyPathString(writer, "UserCondition");
          break;
        case UserCondition.SpecificUser:
          RenderLabelAfterPropertyPathString(writer, "SpecificUser.DisplayName");
          break;
        case UserCondition.SpecificPosition:
          RenderLabelAfterPropertyPathString(writer, "SpecificPosition.DisplayName");
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void RenderAbstractRole (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      ArgumentUtility.CheckNotNull("container", container);

      RenderLabelBeforePropertyPathString(writer, WebString.Empty, "SpecificAbstractRole.DisplayName");
    }

    private void RenderLabelAfterPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      writer.RenderBeginTag(HtmlTextWriterTag.Em);
      RenderPropertyPathString(writer, propertyPathIdentifier);
      writer.RenderEndTag();
      writer.Write(" (");
      string label = GetPropertyDisplayName(propertyPathIdentifier);
      writer.WriteEncodedText(label);
      writer.Write(")");
    }

    private void RenderLabelBeforePropertyPathString (HtmlTextWriter writer, WebString label, string propertyPathIdentifier)
    {
      label.WriteTo(writer);
      writer.Write(" ");
      writer.RenderBeginTag(HtmlTextWriterTag.Em);
      RenderPropertyPathString(writer, propertyPathIdentifier);
      writer.RenderEndTag();
    }

    private void RenderPropertyPathString (HtmlTextWriter writer, string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.CreateStatic(businessObject.BusinessObjectClass, propertyPathIdentifier);
      var result = propertyPath.GetResult(
          businessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);
      writer.WriteEncodedText(result.GetString(null));
    }

    private string GetPropertyDisplayName (string propertyPathIdentifier)
    {
      IBusinessObject businessObject = _accessControlEntry;
      var propertyPath = BusinessObjectPropertyPath.CreateStatic(businessObject.BusinessObjectClass, propertyPathIdentifier);
      Assertion.IsTrue(propertyPath.Properties.Count >= 2);
      return propertyPath.Properties[propertyPath.Properties.Count - 2].DisplayName;
    }

    private void RenderTenantHierarchyIcon (HtmlTextWriter writer, IControl container)
    {
      string url;
      string text;
      switch (_accessControlEntry.TenantHierarchyCondition)
      {
        case TenantHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.This:
          url = "sprite.svg#HierarchyThis";
          text = ResourceManager.GetString(ResourceIdentifier.TenantHierarchyCondition_This);
          break;
        case TenantHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case TenantHierarchyCondition.ThisAndParent:
          url = "sprite.svg#HierarchyThisAndParent";
          text = ResourceManager.GetString(ResourceIdentifier.TenantHierarchyCondition_ThisAndParent);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo(GetIconUrl(url).GetUrl()) { AlternateText = text };
      icon.Render(writer, container);
    }

    private void RenderGroupHierarchyIcon (HtmlTextWriter writer, IControl container)
    {
      string url;
      string text;
      switch (_accessControlEntry.GroupHierarchyCondition)
      {
        case GroupHierarchyCondition.Undefined:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.This:
          url = "sprite.svg#HierarchyThis";
          text = ResourceManager.GetString(ResourceIdentifier.GroupHierarchyCondition_This);
          break;
        case GroupHierarchyCondition.Parent:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.Children:
          throw new InvalidOperationException();
        case GroupHierarchyCondition.ThisAndParent:
          url = "sprite.svg#HierarchyThisAndParent";
          text = ResourceManager.GetString(ResourceIdentifier.GroupHierarchyCondition_ThisAndParent);
          break;
        case GroupHierarchyCondition.ThisAndChildren:
          url = "sprite.svg#HierarchyThisAndChildren";
          text = ResourceManager.GetString(ResourceIdentifier.GroupHierarchyCondition_ThisAndChildren);
          break;
        case GroupHierarchyCondition.ThisAndParentAndChildren:
          url = "sprite.svg#HierarchyThisAndParentAndChildren";
          text = ResourceManager.GetString(ResourceIdentifier.GroupHierarchyCondition_ThisAndParentAndChildren);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      var icon = new IconInfo(GetIconUrl(url).GetUrl()) { AlternateText = text };
      icon.Render(writer, container);
    }

    private IResourceUrl GetIconUrl (string url)
    {
      return _resourceUrlFactory.CreateThemedResourceUrl(typeof(CollapsedAccessControlConditionsRenderer), ResourceType.Image, url);
    }
  }
}
