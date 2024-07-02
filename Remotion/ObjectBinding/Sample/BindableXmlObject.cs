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
using System.Xml.Serialization;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;

namespace Remotion.ObjectBinding.Sample
{
  [BindableObjectWithIdentity]
  public class BindableXmlObject : IBusinessObjectWithIdentity
  {
    protected static T GetObject<T> (Guid id)
      where T:BindableXmlObject
    {
      return (T)XmlReflectionBusinessObjectStorageProvider.Current.GetObject(typeof(T), id);
    }

    protected static T CreateObject<T> ()
       where T : BindableXmlObject
    {
      return XmlReflectionBusinessObjectStorageProvider.Current.CreateObject<T>();
    }

    protected static T CreateObject<T> (Guid id)
         where T : BindableXmlObject
    {
      return XmlReflectionBusinessObjectStorageProvider.Current.CreateObject<T>(id);
    }

    internal Guid _id;

    protected BindableXmlObject ()
    {
    }

    [XmlIgnore]
    [ObjectBinding(Visible = false)]
    public Guid ID
    {
      get { return _id; }
    }

    [XmlIgnore]
    [OverrideMixin]
    public virtual string DisplayName
    {
      get { return GetType().GetFullNameChecked(); }
    }

    [XmlIgnore]
    [OverrideMixin]
    [ObjectBinding(Visible = false)]
    public string UniqueIdentifier
    {
      get { return _id.ToString(); }
    }

    public void SaveObject ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.SaveObject(this);
    }

    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return Mixin.Get<BindableObjectWithIdentityMixin>(this).BusinessObjectClass; }
    }

    object IBusinessObject.GetProperty (IBusinessObjectProperty property)
    {
      return Mixin.Get<BindableObjectWithIdentityMixin>(this).GetProperty(property);
    }

    string IBusinessObject.GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return Mixin.Get<BindableObjectWithIdentityMixin>(this).GetPropertyString(property, format);
    }

    void IBusinessObject.SetProperty (IBusinessObjectProperty property, object value)
    {
      Mixin.Get<BindableObjectWithIdentityMixin>(this).SetProperty(property, value);
    }
  }
}
