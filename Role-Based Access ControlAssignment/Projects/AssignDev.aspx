<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AssignDev.aspx.cs" Inherits="Projects_AssignDev" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
        <h2>Assign Developers to Project</h2>

    <asp:DropDownList ID="ddlProjects" runat="server" AutoPostBack="true" 
        OnSelectedIndexChanged="ddlProjects_SelectedIndexChanged"></asp:DropDownList>
    <br /><br />

    <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="UserId" HeaderText="ID" />
            <asp:BoundField DataField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:TemplateField HeaderText="Assign">
                <ItemTemplate>
                 <asp:CheckBox ID="chkAssign" runat="server" Checked='<%# Convert.ToBoolean(Eval("IsAssigned")) %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <asp:Button ID="btnSave" runat="server" Text="Save Assignments" OnClick="btnSave_Click" />
</asp:Content>

