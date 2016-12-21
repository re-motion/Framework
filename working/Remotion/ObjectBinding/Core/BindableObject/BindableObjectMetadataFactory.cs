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
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="BindableObjectMetadataFactory"/> implements the <see cref="IMetadataFactory"/> interface for the plain reflection based 
  /// bindable object implementation.
  /// </summary>
  public class BindableObjectMetadataFactory : IMetadataFactory
  {
    public static BindableObjectMetadataFactory Create ()
    {
      return ObjectFactory.Create<BindableObjectMetadataFactory> (true, ParamList.Empty);
    }

    private readonly BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    protected BindableObjectMetadataFactory ()
        : this (SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>())
    {
    }

    public BindableObjectMetadataFactory (BindableObjectGlobalizationService bindableObjectGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("bindableObjectGlobalizationService", bindableObjectGlobalizationService);

      _bindableObjectGlobalizationService = bindableObjectGlobalizationService;
    }

    public virtual IClassReflector CreateClassReflector (Type targetType, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new ClassReflector (targetType, businessObjectProvider, this, _bindableObjectGlobalizationService);
    }

    public virtual IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);

      return new ReflectionBasedPropertyFinder (concreteType);
    }

    public virtual PropertyReflector CreatePropertyReflector (
        Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return PropertyReflector.Create (propertyInfo, businessObjectProvider);
    }
  }
}
