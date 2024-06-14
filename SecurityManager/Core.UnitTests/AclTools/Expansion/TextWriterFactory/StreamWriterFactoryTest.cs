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
using System.IO;
using Moq;
using NUnit.Framework;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.TextWriterFactory
{
  [TestFixture]
  public class StreamWriterFactoryTest
  {
    [Test]
    public void NewTextWriterArgumentTest ()
    {
      var streamWriterFactory = new StreamWriterFactory();
      string directory = Path.Combine(Path.GetTempPath(), "StreamWriterFactoryTest_DirectoryTest");
      const string extension = "xyz";
      const string fileName = "someFile";
      using (StreamWriter streamWriter = (StreamWriter)streamWriterFactory.CreateTextWriter(directory, fileName, extension))
      {
        var fileStream = (FileStream)streamWriter.BaseStream;
        string filePathExpected = Path.Combine(directory, fileName + "." + extension);
        Assert.That(fileStream.Name, Is.EqualTo(filePathExpected));
      }
    }

    [Test]
    public void NewTextWriterWithNullDirectoryThrowsTest ()
    {
      var streamWriterFactory = new StreamWriterFactory();
      streamWriterFactory.Directory = null;
      Assert.That(
          () => streamWriterFactory.CreateTextWriter("whatever"),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Directory must not be null. Set using \"Directory\"-property before calling \"CreateTextWriter\""));
    }

    [Test]
    public void NewTextWriterNameAlreadyExistsTest ()
    {
      const string textWriterName = "abc";
      var streamWriterFactory = new StreamWriterFactory();
      var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      streamWriterFactory.Directory = tempDirectory;
      var textWriter = streamWriterFactory.CreateTextWriter(textWriterName);
      Assert.That(
          () => streamWriterFactory.CreateTextWriter(textWriterName),
          Throws.ArgumentException
              .With.Message.EqualTo(@"TextWriter with name ""abc"" already exists."));
      textWriter.Close();
      Directory.Delete(tempDirectory, true);
    }


    [Test]
    public void NewTextWriterOnlyNameArgumentTest ()
    {
      var streamWriterFactoryMock = new Mock<StreamWriterFactory>() { CallBase = true };
      const string directory = "the\\dir\\ect\\ory";
      streamWriterFactoryMock.Object.Directory = directory;
      const string extension = "xyz";
      streamWriterFactoryMock.Object.Extension = extension;
      const string fileName = "someFile";

      streamWriterFactoryMock.Setup(x => x.CreateTextWriter(directory, fileName, extension)).Returns(TextWriter.Null).Verifiable();

      streamWriterFactoryMock.Object.CreateTextWriter(fileName);

      streamWriterFactoryMock.Verify();
    }
  }
}
