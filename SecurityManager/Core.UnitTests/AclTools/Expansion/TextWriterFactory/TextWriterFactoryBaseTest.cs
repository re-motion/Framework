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
