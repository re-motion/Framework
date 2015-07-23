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
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  [TestFixture]
  public class ScriptElementCollectionTest
  {
    private ScriptElementCollection _elementCollection;
    private IScriptElement _elementMock1;
    private IScriptElement _elementMock2;
    private IScriptElement _elementMock3;

    [SetUp]
    public void SetUp ()
    {
      _elementCollection = new ScriptElementCollection();

      _elementMock1 = MockRepository.GenerateStrictMock<IScriptElement> ();
      _elementMock2 = MockRepository.GenerateStrictMock<IScriptElement> ();
      _elementMock3 = MockRepository.GenerateStrictMock<IScriptElement> ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_elementCollection.Elements, Is.Empty);
    }

    [Test]
    public void AppendToScript_NoElements ()
    {
      var script = new List<ScriptStatement>();

      _elementCollection.AppendToScript (script);

      Assert.That (script, Is.Empty);
    }

    [Test]
    public void AppendToScript_OneElement ()
    {
      var script = new List<ScriptStatement> ();
      _elementCollection.AddElement (_elementMock1);

      _elementMock1.Expect (mock => mock.AppendToScript (script));
      _elementMock1.Replay();

      _elementCollection.AppendToScript (script);

      _elementMock1.VerifyAllExpectations ();
      Assert.That (script, Is.Empty);
    }

    [Test]
    public void AppendToScript_SeveralElements ()
    {
      var script = new List<ScriptStatement> ();
      _elementCollection.AddElement (_elementMock1);
      _elementCollection.AddElement (_elementMock2);
      _elementCollection.AddElement (_elementMock3);

      _elementMock1.Expect (mock => mock.AppendToScript (script));
      _elementMock2.Expect (mock => mock.AppendToScript (script));
      _elementMock3.Expect (mock => mock.AppendToScript (script));
      _elementMock1.Replay ();
      _elementMock2.Replay ();
      _elementMock3.Replay ();
      
      _elementCollection.AppendToScript (script);

      _elementMock1.VerifyAllExpectations ();
      _elementMock2.VerifyAllExpectations ();
      _elementMock3.VerifyAllExpectations ();
      Assert.That (script, Is.Empty);
    }

    [Test]
    public void AddElement_OneElement ()
    {
      _elementCollection.AddElement (_elementMock1);

      Assert.That (_elementCollection.Elements, Is.EqualTo (new[] { _elementMock1 }));
    }

    [Test]
    public void AddElement_SeveralElements ()
    {
      _elementCollection.AddElement (_elementMock1);
      _elementCollection.AddElement (_elementMock2);
      _elementCollection.AddElement (_elementMock3);
      
      Assert.That (_elementCollection.Elements, Is.EqualTo (new[] { _elementMock1, _elementMock2, _elementMock3 }));
    }

  }
}