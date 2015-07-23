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

namespace Remotion.Security.UnitTests.TestDomain
{
  [PermanentGuid ("00000000-0000-0000-0001-000000000000")]
  public class File : ISecurableObject
  {
    private Confidentiality _confidentiality;
    private SomeEnum _someEnum;
	
    private string _id;

    public File ()
    {
    }

    [PermanentGuid ("00000000-0000-0000-0001-000000000001")]
    public Confidentiality Confidentiality
    {
      get { return _confidentiality; }
      set { _confidentiality = value; }
    }

    public SomeEnum SimpleEnum
    {
      get { return _someEnum; }
      set { _someEnum = value; }
    }

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    [DemandPermission (DomainAccessTypes.Journalize)]
    public void Journalize ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Type GetSecurableType ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
