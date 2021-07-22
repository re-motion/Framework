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
<%@ Page Language="c#" Codebehind="UpdatePanelSutForm.aspx.cs" AutoEventWireup="True"
  Inherits="Remotion.Web.Test.MultiplePostBackCatching.UpdatePanelSutForm" SmartNavigation="False" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head" runat="server">
  <title>MultiplePostbackCatching Inside UpdatePanel</title>
  <remotion:HtmlHeadContents ID="HtmlHeadContents" runat="server" />
</head>
<body>
  <form id="MyForm" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
      <ContentTemplate>
        <asp:PlaceHolder ID="SutPlaceHolder" runat="server" />
        <br/>
        Auto Postback:
        <asp:DropDownList ID="AutoPostbackList" runat="server" AutoPostBack="True" EnableViewState="false">
          <asp:ListItem Value="1" Text="1" Selected="True" />
          <asp:ListItem Value="2" Text="2" />
          <asp:ListItem Value="3" Text="3" />
        </asp:DropDownList>
        <br/>
        Auto Postback:
        <asp:TextBox ID="AutoPostbackTextBox" runat="server" AutoPostBack="True" EnableViewState="False"/>
        <br/>
        Normal:
        <asp:TextBox ID="TextBox" runat="server" AutoPostBack="False" EnableViewState="False"/>
      </ContentTemplate>
    </asp:UpdatePanel>
  </form>
</body>
</html>
