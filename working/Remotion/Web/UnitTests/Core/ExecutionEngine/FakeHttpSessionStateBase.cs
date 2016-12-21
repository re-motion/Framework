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

using System.Web;
using System.Web.SessionState;
using Remotion.Collections;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  public class FakeHttpSessionStateBase : HttpSessionStateBase
  {
    private readonly object _lockObject = new object();
    private readonly NameObjectCollection _items = new NameObjectCollection();
    private readonly SessionStateMode _mode;

    public FakeHttpSessionStateBase (SessionStateMode mode = SessionStateMode.Off)
    {
      _mode = mode;
    }

    public sealed override object SyncRoot
    {
      get { return _lockObject; }
    }

    public sealed override object this [string name]
    {
      get { return _items[name]; }
      set { _items[name] = value; }
    }

    public override void Add (string name, object value)
    {
      _items.Add (name, value);
    }

    public override void Remove (string name)
    {
      _items.Remove (name);
    }

    public override SessionStateMode Mode
    {
      get { return _mode; }
    }
  }
}