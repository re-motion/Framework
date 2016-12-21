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
using System.Linq;

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class ProxiedChild : Proxied, IAmbigous1, IAmbigous2, IAmbigous3, IAmbigous4, IProperty, IPropertyAmbigous1, IPropertyAmbigous2
  {
    public ProxiedChild ()
    {
    }

    public ProxiedChild (string name)
        : base (name)
    {
    }



    //--------------------------------------------------------------------
    // PropertyAmbigous
    //--------------------------------------------------------------------

    string IPropertyAmbigous1.PropertyAmbigous
    {
      get { return "ProxiedChildChild::IPropertyAmbigous1::PropertyAmbigous " + _name; }
      set { _name = value + "-ProxiedChildChild::IPropertyAmbigous1::PropertyAmbigous"; }
    }

    string IPropertyAmbigous2.PropertyAmbigous
    {
      get { return "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous " + _name; }
      set { _name = value + "-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous"; }
    }

    //public string PropertyAmbigous
    //{
    //  get { return "ProxiedChildChild::PropertyAmbigous " + _name; }
    //  set { _name = value + "-ProxiedChildChild::PropertyAmbigous"; }
    //}


    //--------------------------------------------------------------------
    // NameProperty
    //--------------------------------------------------------------------

    string IProperty.MutableNameProperty
    {
      get { return "ProxiedChild::IAmbigous1::MutableNameProperty " + _name; }
      set { _name = value + "-ProxiedChild::IAmbigous1::MutableNameProperty"; }
    }

    public new string NameProperty
    {
      get { return "ProxiedChild::NameProperty " + _name; }
      set { _name = value + "-ProxiedChild::NameProperty"; }
    }

    public new string MutableNameProperty
    {
      get { return "ProxiedChild::MutableNameProperty " + _name; }
      set { _name = value + "-ProxiedChild::MutableNameProperty"; }
    }

    public new string ReadonlyNameProperty
    {
      get { return "ProxiedChild::ReadonlyNameProperty " + _name; }
    }

    public new string WriteonlyNameProperty
    {
      set { _name = value + "-ProxiedChild::WriteonlyNameProperty"; }
    }


    //--------------------------------------------------------------------
    // NamePropertyVirtual
    //--------------------------------------------------------------------

    public override string NamePropertyVirtual
    {
      get { return "ProxiedChild::NamePropertyVirtual " + _name; }
    }

    public override string MutableNamePropertyVirtual
    {
      get { return "ProxiedChild::MutableNamePropertyVirtual " + _name; }
      set { _name = value + "-ProxiedChild::MutableNamePropertyVirtual"; }
    }

    public override string ReadonlyNamePropertyVirtual
    {
      get { return "ProxiedChild::ReadonlyNameVirtual " + _name; }
    }

    public override string WriteonlyNamePropertyVirtual
    {
      set { _name = value + "-ProxiedChild::WriteonlyNamePropertyVirtual"; }
    }



    public new string Name
    {
      get { return "ProxiedChild: " +_name; }
    }

    public new string MutableName
    {
      get { return "ProxiedChild: " + _name; }
      set { _name = value; }
    }

    public new string ReadonlyName
    {
      get { return "ProxiedChild: " + _name; }
    }

    public new string WriteonlyName
    {
      set { _name = value; }
    }




    public string BraKet (string bra, string it, string ket)
    {
      return bra + it + ket;
    }

    public string BraKet (string it)
    {
      return BraKet ("<", it, ">");
    }

    public string BraKet (string it, int number)
    {
      return StringTimes (BraKet (it), number);
    }


    public string BraKet ()
    {
      return BraKet ("<", "", ">");
    }

    public string OverloadedGenericToString (int t0, int t1)
    {
      return "OverloadedGenericToString" + t0 + t1;
    }

    public override string OverrideMe (string s)
    {
      return "ProxiedChild: " + s;
    }


    private string StringTimes (string text, int number)
    {
      return text.ToSequence (number).Aggregate ((sa, s) => sa + s);
    }

    string IAmbigous2.StringTimes (string text, int number)
    {
      return StringTimes (text, number);
    }

    string IAmbigous1.StringTimes (string text, int number)
    {
      return StringTimes (text, number);
    }



    public string StringTimes2 (string text, int number)
    {
      return text.ToSequence (number).Aggregate ((sa, s) => sa + s);
    }

    string IAmbigous3.StringTimes2 (string text, int number)
    {
      return StringTimes2 (text, number);
    }

    string IAmbigous4.StringTimes2 (string text, int number)
    {
      return StringTimes2 (text, number);
    }

    public string AnotherMethod (string text, int number)
    {
      return text + number;
    }

    string IAmbigous3.AnotherMethod (string text, int number)
    {
      return AnotherMethod (text, number);
    }

    string IAmbigous4.AnotherMethod (string text, int number)
    {
      return AnotherMethod (text, number);
    }


    public string PrependName (string text, int number)
    {
      return "ProxiedChild " + Name + " " + text + ", THE NUMBER=" + number;
    }

    public virtual string VirtualMethodNotInBaseType (string text)
    {
      return "ProxiedChild::VirtualMethodNotInBaseType " + Name + " " + text;
    }
  }
}
