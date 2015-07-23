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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  [TestFixture]
  public class ScriptStatementTest
  {
    private ScriptStatement _statement;

    [SetUp]
    public void SetUp ()
    {
      _statement = new ScriptStatement ("Test");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_statement.Statement, Is.EqualTo ("Test"));
    }

    [Test]
    public void AppendToScript_ScriptNotEmpty ()
    {
      var scriptStatement = new ScriptStatement("Test");
      var script = new List<ScriptStatement> { scriptStatement };
      Assert.That (script.Count, Is.EqualTo (1));

      _statement.AppendToScript (script);

      Assert.That (script, Is.EqualTo (new[] { scriptStatement, _statement }));
    }

    [Test]
    public void AppendToScript_ScriptEmpty ()
    {
      var script = new List<ScriptStatement> ();
      Assert.That (script.Count, Is.EqualTo (0));

      _statement.AppendToScript (script);

      Assert.That (script, Is.EqualTo (new[] { _statement }));
    }
  }
}