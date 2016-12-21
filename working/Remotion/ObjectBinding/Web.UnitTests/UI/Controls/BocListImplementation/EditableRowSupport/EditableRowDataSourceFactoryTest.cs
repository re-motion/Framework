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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditableRowDataSourceFactoryTest
  {
    // types

    // static members and constants

    // member fields

    private IBusinessObject _value;
    private EditableRowDataSourceFactory _factory;

    // construction and disposing

    public EditableRowDataSourceFactoryTest ()
    {
    }

    // methods and properties

    [SetUp] 
    public virtual void SetUp()
    {
      _value = (IBusinessObject) TypeWithString.Create();

      _factory = new EditableRowDataSourceFactory ();
    }

    [Test]
    public void Create ()
    {
      IBusinessObjectReferenceDataSource dataSource = _factory.Create (_value);

      Assert.That (dataSource, Is.Not.Null);
      Assert.That (dataSource.BusinessObject, Is.SameAs (_value));
    }
  }
}