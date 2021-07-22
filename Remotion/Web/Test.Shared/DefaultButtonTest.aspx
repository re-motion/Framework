<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultButtonTest.aspx.cs" Inherits="Remotion.Web.Test.DefaultButtonTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title></title>
</head>
<body>
  <form id="form1" runat="server">
  <div>
    <asp:HyperLink runat="server" NavigateUrl="~/DefaultButtonTest.aspx" Text="Restart" />
    <hr />
    <asp:ScriptManager runat="server" EnablePartialRendering="true" />
    <asp:Button ID="SyncNotDefaultButton" Text="Not the Default Button (sync)" runat="server" OnClick="Button_Click" />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
      <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" DefaultButton="DefaultButton1">
          <asp:TextBox ID="TextBox1" runat="server" />
          <br />
          <asp:Button ID="AsyncNotDefaultButton1" Text="Not the Default Button (async)" runat="server" OnClick="Button_Click" />
          <br />
          <asp:Button ID="DefaultButton1" Text="The Default Button" runat="server" OnClick="Button_Click" />
        </asp:Panel>
      </ContentTemplate>
    </asp:UpdatePanel>
    <hr />
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
      <ContentTemplate>
        <asp:Panel runat="server" DefaultButton="DefaultButton2">
          <asp:TextBox ID="TextBox2" runat="server" />
          <br />
          <asp:Button ID="AsyncNotDefaultButton2" Text="Not the Default Button (async)" runat="server" OnClick="Button_Click" />
          <br />
          <asp:Button ID="DefaultButton2" Text="The Default Button" runat="server" OnClick="Button_Click" />
        </asp:Panel>
      </ContentTemplate>
    </asp:UpdatePanel>
    <hr />
    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
      <ContentTemplate>
        <asp:Label ID="PostBackResult" runat="server" />
      </ContentTemplate>
    </asp:UpdatePanel>
  </div>
  </form>
</body>
</html>
