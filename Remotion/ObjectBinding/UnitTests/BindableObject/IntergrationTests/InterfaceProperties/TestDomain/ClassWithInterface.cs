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

namespace Remotion.ObjectBinding.UnitTests.BindableObject.IntergrationTests.InterfaceProperties.TestDomain
{
  [BindableObject]
  public class ClassWithInterface : IInterface
  {
    private string _implicitPropertyWithGetter;
    private string _implicitPropertyWithGetterAndSetter;
    private string _implicitPropertyWithGetterAndImplementationOnlySetter;
    private string _explicitPropertyWithGetter;
    private string _explicitPropertyWithGetterAndSetter;

    public string ImplicitPropertyWithGetter
    {
      get { return _implicitPropertyWithGetter; }
    }

    public void SetImplicitPropertyWithGetter (string value)
    {
      _implicitPropertyWithGetter = value;
    }

    public string ImplicitPropertyWithGetterAndSetter
    {
      get { return _implicitPropertyWithGetterAndSetter; }
      set { _implicitPropertyWithGetterAndSetter = value; }
    }

    public string ImplicitPropertyWithGetterAndImplementationOnlySetter
    {
      get { return _implicitPropertyWithGetterAndImplementationOnlySetter; }
      set { _implicitPropertyWithGetterAndImplementationOnlySetter = value; }
    }

    string IInterface.ExplicitPropertyWithGetter
    {
      get { return _explicitPropertyWithGetter; }
    }
    
    public void SetExplicitPropertyWithGetter (string value)
    {
      _explicitPropertyWithGetter = value;
    }

    string IInterface.ExplicitPropertyWithGetterAndSetter
    {
      get { return _explicitPropertyWithGetterAndSetter; }
      set { _explicitPropertyWithGetterAndSetter = value; }
    }
  }
}