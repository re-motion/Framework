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
<%@ Page language="c#" Codebehind="WxeTestPage.aspx.cs" AutoEventWireup="false" Inherits="Remotion.Data.DomainObjects.Web.Test.WxeTestPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <HEAD>
    <title>FirstPage</title>
    <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
  </HEAD>
<body>
<form id=Form1 method=post runat="server">
<P>
<asp:Label id="ResultLabel" runat="server">ResultLabel</asp:Label></P>
<h2>WXE-TransactionMode</h2>
<TABLE id="Table1" cellSpacing="1" cellPadding="10" border="1">
  <TR>
    <TD>WxeTransactionMode = CreateNew:</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewButton" runat="server" Text="Run Test"></asp:Button></TD></TR>
</TABLE><BR>

<h2>Write and read data with different values for TransactionMode and AutoCommit</h2>
<TABLE id="Table2" cellSpacing="1" cellPadding="10" border="1">
  <TR>
    <TD>
      <P align=right>TransactionMode:</P></TD>
    <TD>CreateNew</TD>
    <TD>None</TD></TR>
  <TR>
    <TD>AutoCommit = true</TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD>
    <TD></TD></TR>
  <TR>
    <TD>
      <P>AutoCommit = false</P></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNewNoAutoCommitButton" runat="server" Text="Run Test"></asp:Button></TD>
    <TD>
<asp:Button id="WxeTransactedFunctionCreateNoneButton" runat="server" Text="Run Test"></asp:Button></TD></TR></TABLE>
<h2>Page step in nested transacted functions</h2>
<p><asp:Button id="WxeTransactedFunctionWithPageStepButton" runat="server" Text="Run Test"></asp:Button></p>
<P></P></form>
	
  </body>
</HTML>
