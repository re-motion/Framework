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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PostBackFocus.aspx.cs" Inherits="Remotion.Web.Test.PostBackFocus"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager" runat="server" EnablePartialRendering="true" />
    <div>
    <asp:HyperLink ID="SelfLink" runat="server" NavigateUrl="~/PostBackFocus.aspx">Self</asp:HyperLink>
    <br />
<asp:TextBox ID="Field1" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field2" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field3" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field4" runat="server" AutoPostBack="true" />
<asp:TextBox ID="Field5" runat="server" AutoPostBack="true" />
<p></p>
        Update Panel:
      <asp:UpdatePanel runat="server">
        <ContentTemplate>
<asp:TextBox ID="UpdatePanelField1" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField2" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField3" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField4" runat="server" AutoPostBack="true" />
<asp:TextBox ID="UpdatePanelField5" runat="server" AutoPostBack="true" />
        </ContentTemplate>
      </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
