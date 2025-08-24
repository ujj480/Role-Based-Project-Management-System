<%@ Page Title="Users" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Admin_Users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h2>User Management (Admin)</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label><br/>
    <asp:TextBox ID="txtU" runat="server" Placeholder="Username"></asp:TextBox>
    <asp:TextBox ID="txtP" runat="server" TextMode="Password" Placeholder="Password"></asp:TextBox>
    <asp:TextBox ID="txtF" runat="server" Placeholder="Full Name"></asp:TextBox>
    <asp:TextBox ID="txtE" runat="server" Placeholder="Email"></asp:TextBox>
    <asp:DropDownList ID="ddlRole" runat="server"></asp:DropDownList>
    <asp:Button ID="btnAdd" runat="server" Text="Add User" OnClick="btnAdd_Click" />
    <hr/>
    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" DataKeyNames="UserId" OnRowCommand="gvUsers_RowCommand">
        <Columns>
            <asp:BoundField DataField="UserId" HeaderText="ID" />
            <asp:BoundField DataField="Username" HeaderText="Username" />
            <asp:BoundField DataField="FullName" HeaderText="Full Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
            <asp:TemplateField HeaderText="Roles">
                <ItemTemplate>
                    <%# string.Join(",", (string[])Eval("Roles")) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:ButtonField Text="Delete" CommandName="Del" />
        </Columns>
    </asp:GridView>
</asp:Content>
