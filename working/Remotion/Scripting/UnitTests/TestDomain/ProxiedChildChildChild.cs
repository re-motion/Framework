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
  public class ProxiedChildChildChild : ProxiedChildChild
  {
    public ProxiedChildChildChild (string name)
      : base (name)
    {
    }



    //--------------------------------------------------------------------
    // NameProperty
    //--------------------------------------------------------------------

    public new string NameProperty
    {
      get { return "ProxiedChildChildChild::NameProperty " + _name; }
      set { _name = value + "-ProxiedChildChildChild::NameProperty"; }
    }

    public new string MutableNameProperty
    {
      //get { return "ProxiedChildChildChild::MutableNameProperty " + _name; }
      set { _name = value + "-ProxiedChildChildChild::MutableNameProperty"; }
    }

    //public new string ReadonlyNameProperty
    //{
    //  get { return "ProxiedChildChildChild::ReadonlyNameProperty " + _name; }
    //}

    public new string WriteonlyNameProperty
    {
      set { _name = value + "-ProxiedChildChildChild::WriteonlyNameProperty"; }
    }


    //--------------------------------------------------------------------
    // NamePropertyVirtual
    //--------------------------------------------------------------------

    public override string NamePropertyVirtual
    {
      get { return "ProxiedChildChildChild::NamePropertyVirtual " + _name; }
    }

    //public override string MutableNamePropertyVirtual
    //{
    //  get { return "ProxiedChildChildChild::MutableNamePropertyVirtual " + _name; }
    //  set { _name = value + "-ProxiedChildChildChild::MutableNamePropertyVirtual"; }
    //}

    public override string ReadonlyNamePropertyVirtual
    {
      get { return "ProxiedChildChildChild::ReadonlyNameVirtual " + _name; }
    }

    public override string WriteonlyNamePropertyVirtual
    {
      set { _name = value + "-ProxiedChildChildChild::WriteonlyNamePropertyVirtual"; }
    }


    public new string Name
    {
      get { return "ProxiedChildChildChild: " + _name; }
    }

    public new string MutableName
    {
      get { return "ProxiedChildChildChild: " + _name; }
      set { _name = value; }
    }

    public new string ReadonlyName
    {
      get { return "ProxiedChildChildChild: " + _name; }
    }

    public new string WriteonlyName
    {
      set { _name = value; }
    }


    public override string NameVirtual
    {
      get { return "ProxiedChildChildChild::" + _name; }
    }

    public override string MutableNameVirtual
    {
      //get { return "ProxiedChildChildChild::" + _name; }
      set { _name = value + "-ProxiedChildChildChild"; }
    }

    public override string ReadonlyNameVirtual
    {
      get { return "ProxiedChildChildChild::" + _name; }
    }

    public override string WriteonlyNameVirtual
    {
      set { _name = value + "-ProxiedChildChildChild"; }
    }


    public new string ProcessText (string s)
    {
      return "ProxiedChildChildChild: " + s.ToUpper().Replace("holla","die waldfee");
    }

    public new string PrependName (string text)
    {
      return "ProxiedChildChildChild " + Name + " " + text.ToUpper() + " " + text.ToLower();
    }

    public new string PrependName (string text, int number)
    {
      return "ProxiedChildChildChild " + Name + " " + text.ToUpper () + " " + text.ToLower () + ", number=" + number;
    }    

    public override string PrependNameVirtual (string text)
    {
      return PrependName (text);
    }

    public override string VirtualMethodNotInBaseType (string text)
    {
      return "ProxiedChildChildChild::VirtualMethodNotInBaseType " + Name + " " + text.ToUpper () + " " + text.ToLower ();
    }
  }
}
