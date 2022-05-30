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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration.ScriptElements
{
  [TestFixture]
  public class ScriptElementCollectionTest
  {
    private ScriptElementCollection _elementCollection;
    private Mock<IScriptElement> _elementMock1;
    private Mock<IScriptElement> _elementMock2;
    private Mock<IScriptElement> _elementMock3;

    [SetUp]
    public void SetUp ()
    {
      _elementCollection = new ScriptElementCollection();

      _elementMock1 = new Mock<IScriptElement>(MockBehavior.Strict);
      _elementMock2 = new Mock<IScriptElement>(MockBehavior.Strict);
      _elementMock3 = new Mock<IScriptElement>(MockBehavior.Strict);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_elementCollection.Elements, Is.Empty);
    }

    [Test]
    public void AppendToScript_NoElements ()
    {
      var script = new List<ScriptStatement>();

      _elementCollection.AppendToScript(script);

      Assert.That(script, Is.Empty);
    }

    [Test]
    public void AppendToScript_OneElement ()
    {
      var script = new List<ScriptStatement>();
      _elementCollection.AddElement(_elementMock1.Object);

      _elementMock1.Setup(mock => mock.AppendToScript(script)).Verifiable();

      _elementCollection.AppendToScript(script);

      _elementMock1.Verify();
      Assert.That(script, Is.Empty);
    }

    [Test]
    public void AppendToScript_SeveralElements ()
    {
      var script = new List<ScriptStatement>();
      _elementCollection.AddElement(_elementMock1.Object);
      _elementCollection.AddElement(_elementMock2.Object);
      _elementCollection.AddElement(_elementMock3.Object);

      _elementMock1.Setup(mock => mock.AppendToScript(script)).Verifiable();
      _elementMock2.Setup(mock => mock.AppendToScript(script)).Verifiable();
      _elementMock3.Setup(mock => mock.AppendToScript(script)).Verifiable();

      _elementCollection.AppendToScript(script);

      _elementMock1.Verify();
      _elementMock2.Verify();
      _elementMock3.Verify();
      Assert.That(script, Is.Empty);
    }

    [Test]
    public void AddElement_OneElement ()
    {
      _elementCollection.AddElement(_elementMock1.Object);

      Assert.That(_elementCollection.Elements, Is.EqualTo(new[] { _elementMock1.Object }));
    }

    [Test]
    public void AddElement_SeveralElements ()
    {
      _elementCollection.AddElement(_elementMock1.Object);
      _elementCollection.AddElement(_elementMock2.Object);
      _elementCollection.AddElement(_elementMock3.Object);

      Assert.That(_elementCollection.Elements, Is.EqualTo(new[] { _elementMock1.Object, _elementMock2.Object, _elementMock3.Object }));
    }

  }
}
