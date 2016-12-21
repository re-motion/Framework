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

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class Document 
  {
    public string Name { get; set; }
    public int Number { get; set; }

    public Document (string name) : this (name,0)
    {
    }

    public Document (string name, int number)
    {
      Name = name;
      Number = number;
    }

    public Document (Document doc)
    {
      Name = doc.Name;
      Number = doc.Number;
    }
  }
}
