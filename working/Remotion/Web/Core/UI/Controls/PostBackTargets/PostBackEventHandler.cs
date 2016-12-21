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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Remotion.Web.UI.Controls.PostBackTargets
{
  /// <summary>
  /// A postback target with no visual representation on the page. Users may register for the <see cref="PostBack"/> event and retrieve postbacks
  /// started via JavaScript calls. This control should be used in favor of any kind of hidden "dummy" <see cref="Button"/>.
  /// </summary>
  /// <remarks>
  /// If you intend to use the PostBackEventHandler to send asynchronous postbacks, do not forget to register the <see cref="PostBackEventHandler"/>
  /// with the <see cref="ScriptManager"/> using <see cref="ScriptManager.RegisterAsyncPostBackControl"/>.
  /// </remarks>
  public class PostBackEventHandler : Control, IPostBackEventHandler
  {
    public event EventHandler<PostBackEventHandlerEventArgs> PostBack;

    public void RaisePostBackEvent (string eventArgument)
    {
      if (PostBack != null)
        PostBack (this, new PostBackEventHandlerEventArgs (eventArgument));
    }
  }
}