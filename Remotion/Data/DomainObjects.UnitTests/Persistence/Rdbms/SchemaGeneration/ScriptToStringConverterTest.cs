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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ScriptToStringConverterTest : SchemaGenerationTestBase
  {
    private IScriptBuilder _scriptBuilderStub;
    private ScriptElementCollection _fakeCreateElements;
    private ScriptElementCollection _fakeDropElements;
    private ScriptToStringConverter _converter;
    private ScriptStatement _scriptElement1;
    private ScriptStatement _scriptElement2;

    public override void SetUp ()
    {
      base.SetUp();

      _converter = new ScriptToStringConverter();
      _fakeCreateElements = new ScriptElementCollection();
      _fakeDropElements = new ScriptElementCollection();
      _scriptBuilderStub = MockRepository.GenerateStub<IScriptBuilder>();
      _scriptBuilderStub.Stub (stub => stub.GetCreateScript()).Return (_fakeCreateElements);
      _scriptBuilderStub.Stub (stub => stub.GetDropScript()).Return (_fakeDropElements);

      _scriptElement1 = new ScriptStatement ("Test1");
      _scriptElement2 = new ScriptStatement ("Test2");
    }

    [Test]
    public void Convert_OneScriptElement ()
    {
      _fakeCreateElements.AddElement (_scriptElement1);
      _fakeDropElements.AddElement (_scriptElement2);

      var result = _converter.Convert (_scriptBuilderStub);

      Assert.That (result.SetUpScript, Is.EqualTo ("Test1\r\n"));
      Assert.That (result.TearDownScript, Is.EqualTo ("Test2\r\n"));
    }

    [Test]
    public void Convert_SeveralScriptElements ()
    {
      _fakeCreateElements.AddElement (_scriptElement1);
      _fakeCreateElements.AddElement (_scriptElement2);
      _fakeDropElements.AddElement (_scriptElement2);
      _fakeDropElements.AddElement (_scriptElement1);

      var result = _converter.Convert (_scriptBuilderStub);

      Assert.That (result.SetUpScript, Is.EqualTo ("Test1\r\nTest2\r\n"));
      Assert.That (result.TearDownScript, Is.EqualTo ("Test2\r\nTest1\r\n"));
    }
  }
}