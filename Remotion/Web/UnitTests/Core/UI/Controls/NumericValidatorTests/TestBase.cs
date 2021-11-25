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
using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Moq;
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.NumericValidatorTests
{
  public class TestBase
  {
    private NumericValidator _validator;
    private TextBox _textBox;
    private CultureInfo _cultureBackup;


    [SetUp]
    public virtual void SetUp ()
    {
      _textBox = new TextBox();
      _textBox.ID = "TextBox";
      var namingContainer = new Mock<Control>(MockBehavior.Strict);
      namingContainer.Setup(_ => _.FindControl("TextBox")).Returns(_textBox).Verifiable();
      _validator = new NumericValidatorMock((Control) namingContainer.As<INamingContainer>().Object);
      _validator.ControlToValidate = _textBox.ID;

      _cultureBackup = Thread.CurrentThread.CurrentCulture;
      Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [TearDown]
    public virtual void TearDown ()
    {
      Thread.CurrentThread.CurrentCulture = _cultureBackup;
    }

    protected NumericValidator Validator
    {
      get { return _validator; }
    }

    protected TextBox TextBox
    {
      get { return _textBox; }
    }
  }
}
