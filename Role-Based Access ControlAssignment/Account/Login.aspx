<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h2>Login</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label><br />
    <asp:TextBox ID="txtUsername" runat="server" Placeholder="Username"></asp:TextBox><br />
    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Placeholder="Password"></asp:TextBox><br />
    <asp:Button ID="btnLogin" runat="server" Text="Login" OnClick="btnLogin_Click" />
</asp:Content>
