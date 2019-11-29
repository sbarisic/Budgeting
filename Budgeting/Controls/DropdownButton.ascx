<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownButton.ascx.cs" Inherits="Budgeting.Controls.DropdownButton" %>

<button id="btnDropdown" class="btn btn-outline-secondary dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" runat="server"></button>
<div class="dropdown-menu">
    <asp:Repeater ID="repDropdownItems" ItemType="Budgeting.Controls.DropdownEntry" runat="server">
        <ItemTemplate>
            <asp:Button UseSubmitBehavior="false" CssClass="dropdown-item " Text="<%# Item.Name %>" OnClick="Dropdown_Click" CommandName="<%# Item.ID %>" runat="server" />
        </ItemTemplate>
    </asp:Repeater>
</div>
