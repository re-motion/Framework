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
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.SampleDomain
{
  public class SecurableObject : ISecurableObject, IInterfaceWithProperty
  {
    public static void CheckPermissions ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Create)]
    public static SecurableObject CreateForSpecialCase ()
    {
      return new SecurableObject();
    }

    public static bool IsValid ()
    {
      return false;
    }

    [DemandPermission (GeneralAccessTypes.Read)]
    public static bool IsValid (SecurableObject securableClass)
    {
      return true;
    }

    [DemandPermission (GeneralAccessTypes.Read)]
    public static string GetObjectName (SecurableObject securableObject)
    {
      return null;
    }

    private readonly IObjectSecurityStrategy _securityStrategy;

    public SecurableObject ()
    {
    }

    public SecurableObject (IObjectSecurityStrategy objectSecurityStrategy)
    {
      _securityStrategy = objectSecurityStrategy;
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetType();
    }

    [DemandPermission (GeneralAccessTypes.Edit, GeneralAccessTypes.Create)]
    public void Show ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Edit)]
    public virtual void Record ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Delete)]
    public void Load ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Create)]
    public void Load (string filename)
    {
    }

    [DemandPermission (GeneralAccessTypes.Find)]
    public virtual void Print ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Delete)]
    public void Send ()
    {
    }

    public void Save ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Create)]
    public virtual void Make ()
    {
    }

    public void Delete ()
    {
    }

    [DemandPermission (GeneralAccessTypes.Delete)]
    public void Delete (int count)
    {
    }

    [DemandPermission (GeneralAccessTypes.Edit, GeneralAccessTypes.Find, GeneralAccessTypes.Edit)]
    public void Close ()
    {
    }

    public bool IsEnabled
    {
      get { return true; }
    }

    public bool IsVisible
    {
      get { return true; }
      [DemandPermission (TestAccessTypes.Fourth)]
      set { Dev.Null = value; }
    }

    private object NonPublicProperty
    {
      [DemandPermission (TestAccessTypes.First)]
      get { return null; }
      [DemandPermission (TestAccessTypes.Second)]
      set { Dev.Null = value; }
    }

    object IInterfaceWithProperty.InterfaceProperty
    {
      [DemandPermission (TestAccessTypes.First)]
      get { return null; }
      [DemandPermission (TestAccessTypes.Second)]
      set { Dev.Null = value; }
    }
  }
}