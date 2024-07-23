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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation
{
  [TestFixture]
  public class NullValueRowIDProviderTest
  {
    [Test]
    public void GetControlRowID ()
    {
      var rowIDProvider = new NullValueRowIDProvider();
      Assert.That(() => rowIDProvider.GetControlRowID(new BocListRow(0, CreateObject())), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void GetItemRowID ()
    {
      var rowIDProvider = new NullValueRowIDProvider();
      Assert.That(() => rowIDProvider.GetItemRowID(new BocListRow(0, CreateObject())), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void GetRowFromItemRowID_IndexMatches ()
    {
      var rowIDProvider = new NullValueRowIDProvider();

      Assert.That(() => rowIDProvider.GetRowFromItemRowID(new IBusinessObject[0], "1"), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void AddRow_HasNoEffect ()
    {
      var rowIDProvider = new NullValueRowIDProvider();

      Assert.That(() => rowIDProvider.AddRow(new BocListRow(0, CreateObject())), Throws.TypeOf<NotSupportedException>());
    }

    [Test]
    public void RemoveRow_HasNoEffect ()
    {
      var rowIDProvider = new NullValueRowIDProvider();

      Assert.That(() => rowIDProvider.RemoveRow(new BocListRow(0, CreateObject())), Throws.TypeOf<NotSupportedException>());
    }

    private IBusinessObject CreateObject ()
    {
      return new Mock<IBusinessObject>().Object;
    }
  }
}
