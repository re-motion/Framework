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
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;

namespace Remotion.Web.UI.Controls.PostBackTargets
{
  /// <summary>
  /// Accepts postdata and raises the <see cref="DataChanged"/> event if the <see cref="Value"/> was changed by the postback.
  /// </summary>
  public class ScalarLoadPostDataTarget : Control, IPostBackDataHandler
  {
    public ScalarLoadPostDataTarget ()
    {
    }

    public event EventHandler DataChanged;

    public string Value { get; set; }

    public bool LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      var newValues = postCollection.GetValues (postDataKey);
      var oldValue = Value;
      if (newValues != null)
        Value = newValues.FirstOrDefault();

      return oldValue != Value;
    }

    public void RaisePostDataChangedEvent ()
    {
      if (DataChanged != null)
        DataChanged (this, EventArgs.Empty);
    }
  }
}