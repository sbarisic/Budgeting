<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListControl.ascx.cs" Inherits="Budgeting.Controls.ListControl" %>

<ul class="list-group">
    <asp:ListView ID="ListControlList" runat="server">
        <ItemTemplate>
            <li class="<%# Eval("EntryCSS") %>"><%# Eval("Value") %></li>
        </ItemTemplate>
    </asp:ListView>
</ul>
