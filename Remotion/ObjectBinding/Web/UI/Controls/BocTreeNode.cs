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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

public abstract class BocTreeNode: WebTreeNode
{
  public BocTreeNode (string itemID, WebString text, PlainTextString toolTip, IconInfo? icon)
    : base(itemID, text, toolTip, icon ?? new IconInfo(string.Empty))
  {
  }

  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public override WebTreeNodeCollection Children
  {
    get { return base.Children; }
  }

  protected BocTreeView? BocTreeView
  {
    get { return (BocTreeView?)OwnerControl; }
  }
}

public class BusinessObjectTreeNode: BocTreeNode
{
  IBusinessObjectWithIdentity? _businessObject;
  IBusinessObjectReferenceProperty? _property;
  string? _propertyIdentifier;

  public BusinessObjectTreeNode (
      string itemID,
      WebString text,
      PlainTextString toolTip,
      IconInfo? icon,
      IBusinessObjectReferenceProperty? property,
      IBusinessObjectWithIdentity? businessObject)
    : base(itemID, text, toolTip, icon ?? new IconInfo(string.Empty))
  {
    Property = property;
    if (_property != null)
      _propertyIdentifier = _property.Identifier;
    BusinessObject = businessObject;
  }

  public BusinessObjectTreeNode (
      string itemID,
      WebString text,
      IBusinessObjectReferenceProperty property,
      IBusinessObjectWithIdentity businessObject)
    : this(itemID, text, PlainTextString.Empty, null, property, businessObject)
  {
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity"/> of this <see cref="BusinessObjectTreeNode"/>.
  /// </summary>
  public IBusinessObjectWithIdentity? BusinessObject
  {
    get
    {
      EnsureBusinessObject();
      return _businessObject;
    }
    set
    {
      _businessObject = value;
    }
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectReferenceProperty"/> that was used to access the 
  ///   <see cref="IBusinessObjectWithIdentity"/> of this <see cref="BusinessObjectTreeNode"/>.
  /// </summary>
  public IBusinessObjectReferenceProperty? Property
  {
    get
    {
      EnsureProperty();
      return _property;
    }
    set
    {
      _property = value;
      if (value != null)
        _propertyIdentifier = value.Identifier;
      else
        _propertyIdentifier = string.Empty;
    }
  }

  public string? PropertyIdentifier
  {
    get { return _propertyIdentifier; }
    set
    {
      _propertyIdentifier = value;
      _property = null;
    }
  }

  ///<summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "ObjectNode"; }
  }

  private void EnsureBusinessObject ()
  {
    if (_businessObject != null)
      return;

    //  Is root node?
    if (ParentNode == null)
    {
      Assertion.IsNotNull(BocTreeView, "BocTreeView must not be null.");

      if (BocTreeView.Value == null)
        throw new InvalidOperationException("Cannot evaluate the tree node hierarchy because the value collection is null.");

      for (int i = 0; i < BocTreeView.Value.Count; i++)
      {
        IBusinessObjectWithIdentity businessObject = BocTreeView.Value[i];
        if (ItemID == businessObject.UniqueIdentifier)
        {
          BusinessObject = businessObject;
          break;
        }
      }

      if (_businessObject == null)
      {
        //  Required business object has not been part of the values collection in this post back, get it from the class
        if (BocTreeView.DataSource == null)
          throw new InvalidOperationException("Cannot look-up IBusinessObjectWithIdentity '" + ItemID + "': DataSoure is null.");
        if (BocTreeView.DataSource.BusinessObjectClass == null)
          throw new InvalidOperationException("Cannot look-up IBusinessObjectWithIdentity '" + ItemID + "': DataSource.BusinessObjectClass is null.");
        if (!(BocTreeView.DataSource.BusinessObjectClass is IBusinessObjectClassWithIdentity))
        {
          throw new InvalidOperationException(
              $"Cannot look-up IBusinessObjectWithIdentity '{ItemID}': DataSource.BusinessObjectClass is of type '{BocTreeView.DataSource.BusinessObjectClass.GetType()}' "
              + $"but must be of type IBusinessObjectClassWithIdentity.");
        }

        BusinessObject =
            ((IBusinessObjectClassWithIdentity)BocTreeView.DataSource.BusinessObjectClass).GetObject(ItemID);
        if (_businessObject == null) // This test could be omitted if graceful recovery is wanted.
          throw new InvalidOperationException("Could not find IBusinessObjectWithIdentity '" + ItemID + "' via the DataSource.");
      }
    }
    else
    {
      IBusinessObjectReferenceProperty property = Assertion.IsNotNull(Property, "Property != null");
      string businessObjectID = ItemID;
      BusinessObject = ((IBusinessObjectClassWithIdentity)property.ReferenceClass).GetObject(businessObjectID);
    }
  }

  private void EnsureProperty ()
  {
    if (_property != null)
      return;

    BusinessObjectTreeNode? businessObjectParentNode = ParentNode as BusinessObjectTreeNode;
    BusinessObjectPropertyTreeNode? propertyParentNode = ParentNode as BusinessObjectPropertyTreeNode;

    if (businessObjectParentNode != null)
    {
      Assertion.IsNotNull(businessObjectParentNode.BusinessObject, "businessObjectParentNode.BusinessObject is not null.");
      Assertion.IsNotNull(_propertyIdentifier, "_propertyIdentifier must not be null.");

      IBusinessObjectProperty? property =
          businessObjectParentNode.BusinessObject.BusinessObjectClass.GetPropertyDefinition(_propertyIdentifier);
      Property = (IBusinessObjectReferenceProperty?)property;

      if (_property == null) // This test could be omitted if graceful recovery is wanted.
        throw new InvalidOperationException("Could not find IBusinessObjectReferenceProperty '" + _propertyIdentifier + "'.");
    }
    else if (propertyParentNode != null)
    {
      Property = propertyParentNode.Property;
    }
  }
}

public class BusinessObjectPropertyTreeNode: BocTreeNode
{
  IBusinessObjectReferenceProperty? _property;

  public BusinessObjectPropertyTreeNode (
      string itemID,
      WebString text,
      PlainTextString toolTip,
      IconInfo? icon,
      IBusinessObjectReferenceProperty? property)
    : base(itemID, text, toolTip, icon)
  {
    Property = property;
  }

  public BusinessObjectPropertyTreeNode (
      string itemID,
      WebString text,
      IBusinessObjectReferenceProperty property)
    : this(itemID, text, PlainTextString.Empty, null, property)
  {
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectReferenceProperty"/> of this <see cref="BusinessObjectPropertyTreeNode"/>.
  /// </summary>
  [AllowNull]
  public IBusinessObjectReferenceProperty Property
  {
    [MemberNotNull(nameof(_property))]
    get
    {
      EnsureProperty();
      return _property;
    }
    set
    {
      _property = value;
    }
  }

  ///<summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "PropertyNode"; }
  }

  [MemberNotNull(nameof(_property))]
  private void EnsureProperty ()
  {
    if (_property != null)
      return;

    BusinessObjectTreeNode? parentNode = (BusinessObjectTreeNode?)ParentNode;
    if (parentNode == null)
      throw new InvalidOperationException("BusinessObjectPropertyTreeNode with ItemID '" + ItemID + "' has no parent node but property nodes cannot be used as root nodes.");

    Assertion.IsNotNull(parentNode.BusinessObject, "parentNode.BusinessObject must not be null.");

    IBusinessObjectProperty? property = parentNode.BusinessObject.BusinessObjectClass.GetPropertyDefinition(ItemID);
    _property = (IBusinessObjectReferenceProperty)Assertion.IsNotNull(
        property,
        "Property '{0}' does not exist on class '{1}'.",
        ItemID,
        parentNode.BusinessObject.BusinessObjectClass.Identifier);
  }
}

}
