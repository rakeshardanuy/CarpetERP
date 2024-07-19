<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/ERPmaster.master"
    EnableEventValidation="false" Inherits="main" Codebehind="main.aspx.cs" %>

<asp:Content ID="page" ContentPlaceHolderID="CPH_Form" runat="server">            
    <div style="background-color: #edf3fe; height: 591px">
        <div align="right">
            <table>
                <tr>
                    <td id="TDDg" runat="server" visible="false">
                        <asp:GridView ID="DG" runat="server" AutoGenerateColumns="false" OnSelectedIndexChanged="DG_SelectedIndexChanged">
                            <Columns>
                                <asp:TemplateField HeaderText="Order No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblOrderNo" runat="server" ForeColor="Red" Font-Underline="true" Enabled="false"
                                            Text='<%# Bind("OrderNo") %>'> </asp:Label>
                                        <asp:Label ID="lblOrderId" runat="server" ForeColor="Red" Enabled="false" Visible="false"
                                            Text='<%# Bind("OrderId") %>'> </asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
