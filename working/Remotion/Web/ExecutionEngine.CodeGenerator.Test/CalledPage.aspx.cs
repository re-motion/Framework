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
using System.Collections.Generic;
using Remotion.Web.ExecutionEngine;

namespace Test
{
  // <WxeFunction>
  //   <Parameter name="input" type="string" />
  //   <Parameter name="other" type="List{int[,][]}" />
  //   <Parameter name="output" type="string" direction="Out" />
  //   <Parameter name="bothways" type="string" direction="InOut" />
  //   <ReturnValue type="string" />
	// </WxeFunction>
  public partial class CalledPage: WxePage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

		protected void Button1_Click (object sender, EventArgs e)
		{
			ReturnValue = "thank you";
			Return ();
		}

    public static void Call (IWxePage page, WxeArgument<string> input, WxeArgument<List<int[,][]>> other, WxeArgument<string> ReturnValue)
    {

    }

    public static void Call (IWxePage page, string input, List<int[,][]> other, string ReturnValue)
    {
    }

    public static void foo()
    {
      WxeArgument<List<int[,][]>> list = null;
      IWxePage page = null;
      Call (page, "", list, (WxeArgument<string>) null);
      Call (page, "", new List<int[,][]>(), "");
    }
  }

  public interface IWxeArgument<T>
  {
    
  }

  public class WxeArgument<T>
  {
    private T _value;

    public static implicit operator WxeArgument<T> (T value)
    {
      return new WxeArgument<T> (value);
    }

    public WxeArgument (T value)
    {
      _value = value;
    }
  }

}
