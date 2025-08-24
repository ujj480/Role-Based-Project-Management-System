<%@ Page Title="Tasks" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Tasks.aspx.cs" Inherits="Projects_Tasks" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h2>Tasks</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label><br/>
    <asp:HiddenField ID="hfPid" runat="server" />
    <asp:Panel ID="pnlEditor" runat="server">
        <asp:TextBox ID="txtTitle" runat="server" Placeholder="Title"></asp:TextBox>
        <asp:TextBox ID="txtTaskDesc" runat="server" Placeholder="Description"></asp:TextBox>
        <asp:DropDownList ID="ddlStatus" runat="server">
            <asp:ListItem>New</asp:ListItem>
            <asp:ListItem>InProgress</asp:ListItem>
            <asp:ListItem>Done</asp:ListItem>
        </asp:DropDownList>
        <asp:TextBox ID="txtAssignTo" runat="server" Placeholder="Assign to Username"></asp:TextBox>
        <asp:Button ID="btnAddTask" runat="server" Text="Add Task" OnClick="btnAddTask_Click" />
    </asp:Panel>
    <asp:GridView ID="gvTasks" runat="server" AutoGenerateColumns="false" DataKeyNames="TaskId">
        <Columns>
            <asp:BoundField DataField="TaskId" HeaderText="ID" />
            <asp:BoundField DataField="Title" HeaderText="Title" />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:BoundField DataField="Assignee" HeaderText="Assignee" />
        </Columns>
    </asp:GridView>
</asp:Content>
