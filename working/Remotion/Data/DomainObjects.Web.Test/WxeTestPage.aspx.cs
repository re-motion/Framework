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
using System.Web.UI.WebControls;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Data.DomainObjects.Web.Test.WxeFunctions;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public class WxeTestPage: WxePage
  {
    protected Button WxeTransactedFunctionCreateNewButton;
    protected Label ResultLabel;
    protected Button WxeTransactedFunctionCreateNewAutoCommitButton;
    protected Button WxeTransactedFunctionCreateNewNoAutoCommitButton;
    protected Button WxeTransactedFunctionCreateNoneButton;
    protected Button WxeTransactedFunctionWithPageStepButton;

    protected WxeTestPageFunction CurrentWxeTestPageFunction
    {
      get { return (WxeTestPageFunction) CurrentFunction; }
    }

    protected HtmlHeadContents HtmlHeadContents;

    private void Page_Load (object sender, EventArgs e)
    {
      ResultLabel.Visible = false;
    }

    #region Web Form Designer generated code

    protected override void OnInit (EventArgs e)
    {
      //
      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      //
      InitializeComponent();
      base.OnInit (e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.WxeTransactedFunctionCreateNewButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNewButton_Click);
      this.WxeTransactedFunctionCreateNewAutoCommitButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNewAutoCommitButton_Click);
      this.WxeTransactedFunctionCreateNewNoAutoCommitButton.Click +=
          new System.EventHandler (this.WxeTransactedFunctionCreateNewNoAutoCommitButton_Click);
      this.WxeTransactedFunctionCreateNoneButton.Click += new System.EventHandler (this.WxeTransactedFunctionCreateNoneButton_Click);
      this.WxeTransactedFunctionWithPageStepButton.Click += new EventHandler (WxeTransactedFunctionWithPageStepButton_Click);
      this.Load += new System.EventHandler (this.Page_Load);
    }

    #endregion

    private void WxeTransactedFunctionCreateNewButton_Click (object sender, EventArgs e)
    {
      // TODO: cheange to Remember/CheckActiveClientTransactionScope
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();

        new CreateRootTestTransactedFunction (ClientTransactionScope.CurrentTransaction).Execute();

        CheckCurrentClientTransactionRestored();
      }

      ShowResultText ("Test WxeTransactedFunction (CreateNew) executed successfully.");
    }

    private void WxeTransactedFunctionCreateNewAutoCommitButton_Click (object sender, EventArgs e)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();
        SetInt32Property (5, ClientTransaction.CreateRootTransaction());

        new TestTransactedFunction (WxeTransactionMode.CreateRootWithAutoCommit, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();
        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransaction.CreateRootTransaction()) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not properly commit or set the property value.");
      }

        ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = true) executed successfully.");
    }

    private void WxeTransactedFunctionCreateNewNoAutoCommitButton_Click (object sender, EventArgs e)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();
        SetInt32Property (5, ClientTransaction.CreateRootTransaction());

        new TestTransactedFunction (WxeTransactionMode.CreateRoot, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();

        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransaction.CreateRootTransaction()) != 5)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did set and commit the property value.");
      }
      ShowResultText ("Test WxeTransactedFunction (TransactionMode = CreateNew, AutoCommit = false) executed successfully.");
    }

    private void WxeTransactedFunctionCreateNoneButton_Click (object sender, EventArgs e)
    {
      SetInt32Property (5, ClientTransaction.CreateRootTransaction());
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        RememberCurrentClientTransaction();

        new TestTransactedFunction (WxeTransactionMode.None, DomainObjectIDs.ObjectWithAllDataTypes1).Execute ();

        CheckCurrentClientTransactionRestored();

        if (GetInt32Property (ClientTransactionScope.CurrentTransaction) != 10)
          throw new TestFailureException ("The WxeTransactedFunction wrongly did not set the property value.");
      }

      if (GetInt32Property (ClientTransaction.CreateRootTransaction()) != 5)
        throw new TestFailureException ("The WxeTransactedFunction wrongly committed the property value.");

      ShowResultText ("Test WxeTransactedFunction (TransactionMode = None, AutoCommit = false) executed successfully.");
    }

    private void WxeTransactedFunctionWithPageStepButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
      {
        RememberCurrentClientTransaction();
        this.ExecuteFunction (new ParentPageStepTestTransactedFunction(), WxeCallArguments.Default);
      }
      else
      {
        if (ClientTransactionScope.ActiveScope == null)
          throw new TestFailureException ("The function did not restore the original scope.");

        CheckCurrentClientTransactionRestored();
        ShowResultText ("Test WxeTransactedFunction with nested PageStep executed successfully.");
      }
    }

    private void SetInt32Property (int value, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = DomainObjectIDs.ObjectWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();

        objectWithAllDataTypes.Int32Property = value;

        clientTransaction.Commit();
      }
    }

    private int GetInt32Property (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes objectWithAllDataTypes = DomainObjectIDs.ObjectWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();

        return objectWithAllDataTypes.Int32Property;
      }
    }

    private void RememberCurrentClientTransaction()
    {
      CurrentWxeTestPageFunction.CurrentClientTransaction = ClientTransactionScope.CurrentTransaction;
    }

    private void CheckCurrentClientTransactionRestored()
    {
      if (CurrentWxeTestPageFunction.CurrentClientTransaction != ClientTransactionScope.CurrentTransaction)
        throw new TestFailureException (
            "ClientTransactionScope.CurrentTransaction was not properly restored to the state before the WxeTransactedFunction was called.");
    }

    private void ShowResultText (string text)
    {
      ResultLabel.Visible = true;
      ResultLabel.Text = text;
    }
  }
}
