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
using System.Collections;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.TypePipe;

namespace Remotion.ObjectBinding.Web.UnitTests.Domain
{
  [BindableObjectWithIdentity]
  public class TypeWithReference : IBusinessObjectWithIdentity
  {
    public static TypeWithReference Create ()
    {
      return ObjectFactory.Create<TypeWithReference>(true, ParamList.Empty);
    }

    public static TypeWithReference Create (TypeWithReference firstValue, TypeWithReference secondValue)
    {
      return ObjectFactory.Create<TypeWithReference>(true, ParamList.Create(firstValue, secondValue));
    }

    public static TypeWithReference Create (string displayName)
    {
      return ObjectFactory.Create<TypeWithReference>(true, ParamList.Create(displayName));
    }

    private TypeWithReference _referenceValue;
    private TypeWithReference[] _referenceList;
    private IList _referenceListAsList;
    private TypeWithReference _firstValue;
    private TypeWithReference _secondValue;
    private string _displayName;
    private Guid _id = Guid.NewGuid();

    protected TypeWithReference ()
    {
    }

    protected TypeWithReference (TypeWithReference firstValue, TypeWithReference secondValue)
    {
      _firstValue = firstValue;
      _secondValue = secondValue;
    }

    protected TypeWithReference (string displayName)
    {
      _displayName = displayName;
    }

    public TypeWithReference ReferenceValue
    {
      get { return _referenceValue; }
      set { _referenceValue = value; }
    }

    public TypeWithReference[] ReferenceList
    {
      get { return _referenceList; }
      set { _referenceList = value; }
    }

    [ItemType(typeof(TypeWithReference))]
    public IList ReferenceListAsList
    {
      get { return _referenceListAsList; }
      set { _referenceListAsList = value; }
    }

    public TypeWithReference FirstValue
    {
      get { return _firstValue; }
      set { _firstValue = value; }
    }

    public TypeWithReference SecondValue
    {
      get { return _secondValue; }
      set { _secondValue = value; }
    }

    [OverrideMixin]
    public string DisplayName
    {
      get { return _displayName ?? UniqueIdentifier; }
    }

    [OverrideMixin]
    public string UniqueIdentifier
    {
      get { return _id.ToString(); }
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
