<%@ Page Title="Seat Master" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="SeatMaster.aspx.cs" Inherits="IPLProject.SeatMasterPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2 style="color: #1E40AF;">Admin - Seat Management</h2>
    <asp:Label ID="lblMsg" runat="server" ForeColor="Green"></asp:Label>
    
    <div style="background:#F8FAFC; padding:15px; border-radius:6px; margin-bottom:20px; border:1px solid #BFDBFE;">
        <h3>Add / Edit Seat</h3>
        <asp:HiddenField ID="hfSeatId" runat="server" />
        <label>Select Stand 
        <asp:RequiredFieldValidator ID="rfvStand" runat="server" ControlToValidate="ddlStand" InitialValue="" ErrorMessage="*" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator></label>
        <asp:DropDownList ID="ddlStand" runat="server"></asp:DropDownList>
        
        <label>Seat Number <asp:RequiredFieldValidator ID="rfvSeatNumber" runat="server" ControlToValidate="txtSeatNumber" ErrorMessage="*" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator></label>
        <asp:TextBox ID="txtSeatNumber" runat="server" placeholder="e.g. N-107"></asp:TextBox>
        
        <label>Price (Rs) <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ControlToValidate="txtPrice" ErrorMessage="*" ForeColor="Red" Display="Dynamic"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="revPrice" runat="server" ControlToValidate="txtPrice" ErrorMessage="Enter valid number" ValidationExpression="^\d+(\.\d{1,2})?$" ForeColor="Red" Display="Dynamic"></asp:RegularExpressionValidator></label>
        <asp:TextBox ID="txtPrice" runat="server" placeholder="e.g. 3500"></asp:TextBox>
        
        <div style="margin-top:10px;">
            <asp:Button ID="btnSave" runat="server" Text="Save Seat" CssClass="btn-green" OnClick="btnSave_Click" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-red" OnClick="btnCancel_Click" CausesValidation="false" />
        </div>
    </div>

    <asp:GridView ID="gvSeats" runat="server" AutoGenerateColumns="False" CssClass="grid-table" OnRowCommand="gvSeats_RowCommand">
        <Columns>
            <asp:BoundField DataField="SeatId" HeaderText="ID" />
            <asp:BoundField DataField="StandName" HeaderText="Stand" />
            <asp:BoundField DataField="SeatNumber" HeaderText="Seat No" />
            <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="Rs : {0:N2}" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditSeat" CommandArgument='<%# Eval("SeatId") %>' CssClass="btn-green" CausesValidation="false" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteSeat" CommandArgument='<%# Eval("SeatId") %>' CssClass="btn-red" OnClientClick="return confirm('Delete seat?');" CausesValidation="false" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
