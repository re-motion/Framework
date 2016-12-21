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

namespace Remotion.Web
{
  public class ResourceType
  {
    public static readonly ResourceType Image = new ResourceType ("Image");
    public static readonly ResourceType Html = new ResourceType ("Html");
    public static readonly ResourceType UI = new ResourceType ("UI");
    public static readonly ResourceType HelpPage = new ResourceType ("HelpPage");

    private readonly string _name;

    public ResourceType (string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }
  }
}
