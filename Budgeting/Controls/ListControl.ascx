<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListControl.ascx.cs" Inherits="Budgeting.Controls.ListControl" %>

<ul class="list-group">
    <asp:ListView ID="ListControlList" runat="server">
        <ItemTemplate>
            <div visible='<%# !(bool)Eval("Clickable") %>' runat="server">
                <li class="<%# Eval("EntryCSS") %>"><%# Eval("Value") %></li>
            </div>

            <div visible='<%# Eval("Clickable") %>' runat="server">
                <asp:LinkButton CssClass='<%# Eval("EntryCSS") %>' runat="server" OnClick="OnItem_Click" OnClientClick="return YesNoPrompt('Delete entry?');" CommandName="OnClick" CommandArgument='<%# Eval("Argument") %>'><%# Eval("Value") %></asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:ListView>
</ul>
