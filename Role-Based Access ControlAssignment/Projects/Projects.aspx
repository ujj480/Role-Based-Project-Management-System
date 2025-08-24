<%@ Page Title="Projects" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Projects.aspx.cs" Inherits="Projects_Projects" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <h2>Projects</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label><br/>
     <asp:Label ID="lblId" runat="server" Visible="false" ForeColor="Red"></asp:Label><br/>
    <asp:Panel ID="pnlCreate" runat="server">
        <asp:TextBox ID="txtName" runat="server" Placeholder="Project name"></asp:TextBox>
        <asp:TextBox ID="txtDesc" runat="server" Placeholder="Description"></asp:TextBox>
        <asp:Button ID="btnCreate" runat="server" Text="Create Project" OnClick="btnCreate_Click" />
           <asp:Button ID="btnDelete" runat="server" Text="Delete Project" OnClick="btnDelete_Click" />
    </asp:Panel>
    <hr/>
   <asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="false" OnSelectedIndexChanged="gvProjects_SelectedIndexChanged" DataKeyNames="ProjectId">
    <Columns>
        <asp:BoundField DataField="ProjectId" HeaderText="ID" />
        <asp:BoundField DataField="Name" HeaderText="Name" />
        <asp:BoundField DataField="Description" HeaderText="Description" />
        <asp:HyperLinkField Text="Open" DataNavigateUrlFields="ProjectId" DataNavigateUrlFormatString="Tasks.aspx?pid={0}" />
          <asp:TemplateField HeaderStyle-Width="15px" HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:Button ID="IBtn_Show" runat="server" CommandName="Select" Height="30px"
                                                                Text="Edit" />
                                                        </ItemTemplate>
                                                        <HeaderStyle Width="25px" />
                                                        <ItemStyle Width="25px" />
                                                    </asp:TemplateField>
    </Columns>
</asp:GridView>
</asp:Content>
