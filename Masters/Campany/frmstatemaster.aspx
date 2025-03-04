﻿<%@ Page Language="C#" AutoEventWireup="true" Inherits="Masters_Campany_frmstatemaster"
    EnableEventValidation="false" Codebehind="frmstatemaster.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="head1" runat="server">
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Enter" />
    <meta content="blendTrans(Duration=0.5)" http-equiv="Page-Exit" />
    <title></title>
    <script src="../../Scripts/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <link href="../../Styles/vijay.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function closesstate() {
            window.opener.document.getElementById('CPH_Form_State').click();
            self.close();
        }
    </script>
    <script type="text/javascript">


        function getbacktostepone() {
            window.location = "frmstatemaster.aspx";
        }
        function onSuccess() {
            ta
            setTimeout(okay, 200);
        }
        function onError() {
            setTimeout(getbacktostepone, 200);
        }
        function okay() {
            window.parent.document.getElementById('btnOkay').click();
        }
        function cancel() {
            window.parent.document.getElementById('btnCancel').click();
        }
        function priview() {
            window.open('../../ReportViewer.aspx', '');

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptmanager1" runat="server">
    </asp:ScriptManager>
    <div style="margin-left: 15%; margin-right: 15%">
        <asp:UpdatePanel ID="updatepanel1" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td class="tdstyle">
                            <asp:Label ID="lbl" Text="Country Name" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:DropDownList ID="ddCountry" runat="server" CssClass="dropdown" Width="150px">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdstyle">
                            <asp:Label ID="Label1" Text="State Name" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:TextBox Width="150px" ID="txtStateName" runat="server" CssClass="textb">
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please Enter State Name"
                                ControlToValidate="txtStateName" ValidationGroup="f1" ForeColor="Red">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="tdstyle">
                            <asp:Label ID="Label2" Text="State Code" runat="server" Font-Bold="true" />
                        </td>
                        <td>
                            <asp:TextBox Width="150px" ID="txtStateCode" runat="server" CssClass="textb">
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please Enter State Code"
                                ControlToValidate="txtStateCode" ValidationGroup="f1" ForeColor="Red">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2" colspan="2">
                            <div style="width: 100%; height: 200px; overflow: auto">
                                <asp:Label ID="lblerr" runat="server" ForeColor="Red"></asp:Label>
                                <asp:GridView ID="DGState" runat="server" Width="280px" DataKeyNames="Stateid" CssClass="grid-views"
                                    AutoGenerateColumns="False" OnRowCreated="DGState_RowCreated" OnRowDataBound="DGState_RowDataBound"
                                    OnSelectedIndexChanged="DGState_SelectedIndexChanged">
                                    <HeaderStyle CssClass="gvheaders" />
                                    <AlternatingRowStyle CssClass="gvalts" />
                                    <RowStyle CssClass="gvrow" />
                                    <Columns>
                                        <asp:BoundField DataField="StateName" HeaderText="StateName">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="StateCode" HeaderText="StateCode">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="CountryName" HeaderText="CountryName">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                    </Columns>
                                    <EmptyDataRowStyle CssClass="gvemptytext" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <asp:Button ID="btnsave" runat="server" CssClass="buttonnorm" Text="Save" ValidationGroup="f1"
                                OnClick="btnsave_Click" />
                            <asp:Button ID="btnclose" runat="server" CssClass="buttonnorm" Text="Close" ValidationGroup="f1"
                                CausesValidation="False" OnClientClick="return closesstate();" />
                            <asp:Button ID="btndelete" runat="server" Text="Delete" CssClass="buttonnorm" OnClientClick="return confirm('Do you want to Delete data?')"
                                Visible="False" OnClick="btndelete_Click1" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
