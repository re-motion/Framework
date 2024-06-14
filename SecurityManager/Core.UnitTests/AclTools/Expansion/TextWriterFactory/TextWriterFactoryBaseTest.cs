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
using Moq;
using NUnit.Framework;
using Remotion.SecurityManager.AclTools.Expansion.TextWriterFactory;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.TextWriterFactory
{
  [TestFixture]
  public class TextWriterFactoryBaseTest
  {
    [Test]
    public void AppendExtensionTest ()
    {
      const string name = "huizilipochtli";
      const string extension = "ext";
      Assert.That(TextWriterFactoryBase.AppendExtension(name, extension),Is.EqualTo(name + "." + extension));
      Assert.That(TextWriterFactoryBase.AppendExtension(name, null), Is.EqualTo(name));
      Assert.That(TextWriterFactoryBase.AppendExtension(name, ""), Is.EqualTo(name));
    }


    [Test]
    public void GetRelativePathTest ()
    {
      var textWriterFactoryBaseMock = new Mock<TextWriterFactoryBase>() { CallBase = true };
      textWriterFactoryBaseMock.Setup(x => x.TextWriterExists("yang")).Returns(true).Verifiable();
      textWriterFactoryBaseMock.Object.Extension = "dat";
      var result = textWriterFactoryBaseMock.Object.GetRelativePath("yin", "yang");
      Assert.That(result, Is.EqualTo(@".\yang.dat"));
      textWriterFactoryBaseMock.Verify();
    }


    [Test]
    public void GetRelativePathNoEntryWithNameExistsTest ()
    {
      var textWriterFactoryBaseMock = new Mock<TextWriterFactoryBase>() { CallBase = true };

      textWriterFactoryBaseMock.Setup(x => x.TextWriterExists("yang")).Returns(false).Verifiable();

      Assert.That(
          () => textWriterFactoryBaseMock.Object.GetRelativePath("yin", "yang"),
          Throws.ArgumentException
              .With.Message.EqualTo(@"No TextWriter with name ""yang"" registered => no relative path exists."));

      textWriterFactoryBaseMock.Verify();
    }

  }
}
