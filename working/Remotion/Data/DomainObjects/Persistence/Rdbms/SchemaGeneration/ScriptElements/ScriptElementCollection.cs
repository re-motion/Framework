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
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  /// <summary>
  /// The <see cref="ScriptElementCollection"/> represents several <see cref="IScriptElement"/>s for a relational database.
  /// </summary>
  public class ScriptElementCollection : IScriptElement
  {
    private readonly List<IScriptElement> _elements;

    public ScriptElementCollection ()
    {
      _elements = new List<IScriptElement>();
    }

    public ReadOnlyCollection<IScriptElement> Elements
    {
      get { return _elements.AsReadOnly(); }
    }

    public void AppendToScript (List<ScriptStatement> script)
    {
      ArgumentUtility.CheckNotNull ("script", script);

      foreach (var element in _elements)
        element.AppendToScript (script);
    }

    public void AddElement (IScriptElement element)
    {
      ArgumentUtility.CheckNotNull ("element", element);

      _elements.Add (element);
    }
  }
}