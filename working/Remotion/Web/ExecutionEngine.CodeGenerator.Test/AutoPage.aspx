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
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoPage.aspx.cs" Inherits="Test.AutoPage" %>
<%@ Register TagPrefix="remotion" Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" %>
<%@ Register TagPrefix="cc" TagName="AutoUserControl" Src="~/AutoUserControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Untitled Page</title>
    <remotion:HtmlHeadContents runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <table>
            <tr>
                <td>
                    In
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="InArgField" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    InOut
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="InOutArgField" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    Out
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="OutArgField" runat="server"></asp:TextBox></td>
            </tr>
        </table>
        <br />
    
        <asp:Button ID="ExecSelfButton" runat="server" Text="Execute self" OnClick="ExecSelfButton_Click" />
        <asp:Button ID="ExecCalledPageButton" runat="server" Text="Execute called page" OnClick="ExecCalledPageButton_Click" />
        <remotion:WebButton ID="ExecUserControlButton" runat="server" Text="Execute called user control" OnClick="ExecUserControlButton_Click" />
        <asp:Button ID="ReturnButton" runat="server" Text="Return" OnClick="ReturnButton_Click" />
        <asp:Button ID="NoOpButton" runat="server" Text="NoOp" />

<cc:AutoUserControl id="AutoUserControl" runat="server" />
    </div>
    </form>
</body>
</html>
