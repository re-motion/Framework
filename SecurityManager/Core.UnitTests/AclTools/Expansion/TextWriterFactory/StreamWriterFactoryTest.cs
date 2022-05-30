// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
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
      streamWriterFactory.Directory = "xyz";
      streamWriterFactory.CreateTextWriter(textWriterName);
      Assert.That(
          () => streamWriterFactory.CreateTextWriter(textWriterName),
          Throws.ArgumentException
              .With.Message.EqualTo(@"TextWriter with name ""abc"" already exists."));
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
