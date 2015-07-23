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
  public class ProxiedChildChild : ProxiedChild, IProcessText1, IProcessText2, INotInProxied, IPrependName
  {
    public ProxiedChildChild (string name) : base (name)
    {
    }


    //--------------------------------------------------------------------
    // PropertyAmbigous
    //--------------------------------------------------------------------

    public string PropertyAmbigous
    {
      get { return "ProxiedChildChild::PropertyAmbigous " + _name; }
      set { _name = value + "-ProxiedChildChild::PropertyAmbigous"; }
    }



    //--------------------------------------------------------------------
    // NameProperty
    //--------------------------------------------------------------------

    public new string NameProperty
    {
      get { return "ProxiedChildChild::NameProperty " + _name; }
    }

    public new string MutableNameProperty
    {
      get { return "ProxiedChildChild::MutableNameProperty " + _name; }
      set { _name = value + "-ProxiedChildChild::MutableNameProperty"; }
    }

    public new string ReadonlyNameProperty
    {
      get { return "ProxiedChildChild::ReadonlyNameProperty " + _name; }
    }

    public new string WriteonlyNameProperty
    {
      set { _name = value + "-ProxiedChildChild::WriteonlyNameProperty"; }
    }


    //--------------------------------------------------------------------
    // NamePropertyVirtual
    //--------------------------------------------------------------------

    public override string NamePropertyVirtual
    {
      get { return "ProxiedChildChild::NamePropertyVirtual " + _name; }
    }

    public override string MutableNamePropertyVirtual
    {
      get { return "ProxiedChildChild::MutableNamePropertyVirtual " + _name; }
      set { _name = value + "-ProxiedChildChild::MutableNamePropertyVirtual"; }
    }

    public override string ReadonlyNamePropertyVirtual
    {
      get { return "ProxiedChildChild::ReadonlyNameVirtual " + _name; }
    }

    public override string WriteonlyNamePropertyVirtual
    {
      set { _name = value + "-ProxiedChildChild::WriteonlyNamePropertyVirtual"; }
    }




    public override string NameVirtual
    {
      get { return "ProxiedChildChild::" + _name; }
    }

    public override string MutableNameVirtual
    {
      get { return "ProxiedChildChild::" + _name; }
      set { _name = value + "-ProxiedChildChild"; }
    }

    public override string ReadonlyNameVirtual
    {
      get { return "ProxiedChildChild::" + _name; }
    }

    public override string WriteonlyNameVirtual
    {
      set { _name = value + "-ProxiedChildChild"; }
    }


    public string ProcessText (string s)
    {
      return s.ToLower().Replace("abc","xyz");
    }

    public new string PrependName (string text)
    {
      return "ProxiedChildChild " + Name + " " + text;
    }

    public override string PrependNameVirtual (string text)
    {
      return PrependName (text);
    }

    public string NotInProxied ()
    {
      return "ProxiedChildChild.NotInProxied";
    }


  }

  public interface INotInProxied
  {
    string NotInProxied ();
  }
}
