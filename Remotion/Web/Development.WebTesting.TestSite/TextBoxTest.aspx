<%-- This file is part of the re-motion Core Framework (www.re-motion.org)
 % Copyright (c) rubicon IT GmbH, www.rubicon.eu
 %
 % The re-motion Core Framework is free software; you can redistribute it 
 % and/or modify it under the terms of the GNU Lesser General Public License 
 % as published by the Free Software Foundation; either version 2.1 of the 
 % License, or (at your option) any later version.
 %
 % re-motion is distributed in the hope that it will be useful, 
 % but WITHOUT ANY WARRANTY; without even the implied warranty of 
 % MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 % GNU Lesser General Public License for more details.
 %
 % You should have received a copy of the GNU Lesser General Public License
 % along with re-motion; if not, see http://www.gnu.org/licenses.
--%>
<%@ Page Language="C#" MasterPageFile="~/Layout.Master" AutoEventWireup="true" CodeBehind="TextBoxTest.aspx.cs" Inherits="Remotion.Web.Development.WebTesting.TestSite.TextBoxTest" %>
<asp:Content ContentPlaceHolderID="body" runat="server">
  <h3>TextBox1 - re-motion IEditableTextBox</h3>
  <testsite:TestEditableTextBox ID="MyEditableTextBox" Value="MyEditableTextBoxValue" AutoPostBack="true" runat="server"/>
  <h3>TextBox2 - ASP.NET TextBox</h3>
  <asp:TextBox ID="MyAspTextBox" Text="MyAspTextBoxValue" AutoPostBack="true" runat="server"/>
  <h3>TextBox3 - HTML input[type=text]</h3>
  <input id="MyHtmlTextBox" type="Text" value="MyHtmlTextBoxValue" runat="server"/>
  <div id="scope">
    <h3>TextBox4 - ASP.NET TextBox (no auto postback)</h3>
    <asp:TextBox ID="MyAspTextBoxNoAutoPostBack" Text="MyAspTextBoxNoAutoPostBackValue" AutoPostBack="false" runat="server"/>
  </div>
</asp:Content>