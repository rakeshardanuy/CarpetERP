﻿<%@ Page Title="FOLIO" Language="C#" MasterPageFile="~/ERPmaster.master" AutoEventWireup="true" Inherits="Masters_Hissab_frmFolioDetail" Codebehind="frmFolioDetail.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <script src="../../Scripts/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        function CloseForm() {
            window.location.href = "../../main.aspx";
        }
    </script>
    <script type="text/javascript">
        function Jscriptvalidate() {
            $(document).ready(function () {
                $("#<%=btnprint.ClientID %>").click(function () {
                    var Message = "";
                    var selectedindex = $("#<%=DDCompanyName.ClientID %>").attr('selectedIndex');
                    if (selectedindex < 0) {
                        Message = Message + "Please select Company Name!!\n";
                    }
                    selectedindex = $("#<%=DDweaver.ClientID %>").attr('selectedIndex');
                    if (selectedindex <= 0) {
                        Message = Message + "Please Select Weaver. !!\n";
                    }
                    selectedindex = $("#<%=DDFolioNo.ClientID %>").attr('selectedIndex');
                    if (selectedindex <= 0) {
                        Message = Message + "Please Select Folio. !!\n";
                    }

                    if (Message == "") {
                        return true;
                    }
                    else {
                        alert(Message);
                        return false;
                    }
                });

            });
        }
    </script>
    <asp:UpdatePanel ID="upda1" runat="server">
        <ContentTemplate>
            <script type="text/javascript" language="javascript">
                Sys.Application.add_load(Jscriptvalidate);
            </script>
            <div style="margin: 1% 20% 0% 20%">
                <table style="width: 100%" border="1" cellspacing="0">
                    <tr>
                        <td style="width: 30%; border-style: dotted">
                            <asp:Label ID="lblfoliono" Text="Enter Folio No." CssClass="labelbold" runat="server" />
                        </td>
                        <td style="width: 50%; border-style: dotted">
                            <asp:TextBox ID="txtfoliono" CssClass="textb" Width="95%" runat="server" AutoPostBack="true"
                                OnTextChanged="txtfoliono_TextChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 30%; border-style: dotted">
                            <asp:Label Text="Company Name" runat="server" CssClass="labelbold" />
                        </td>
                        <td style="width: 50%; border-style: dotted">
                            <asp:DropDownList CssClass="dropdown" ID="DDCompanyName" Width="95%" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 30%; border-style: dotted">
                            <asp:Label ID="Label1" Text="Weaver Name" runat="server" CssClass="labelbold" />
                        </td>
                        <td style="width: 50%; border-style: dotted">
                            <asp:DropDownList CssClass="dropdown" ID="DDweaver" Width="95%" runat="server" OnSelectedIndexChanged="DDweaver_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 20%; border-style: dotted">
                            <asp:Label ID="Label2" Text="Folio No." runat="server" CssClass="labelbold" />
                        </td>
                        <td style="width: 20%; border-style: dotted">
                            <asp:DropDownList CssClass="dropdown" ID="DDFolioNo" Width="95%" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 50%; border-style: dotted">
                            <asp:CheckBox Text="Final Folio" ID="chkfinalfolio" CssClass="checkboxbold" AutoPostBack="true"
                                runat="server" OnCheckedChanged="chkfinalfolio_CheckedChanged" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:CheckBox Text="Pouf Folio" ID="ChkPoufFolio" CssClass="checkboxbold" runat="server"
                                Visible="false" /><br />
                            <asp:CheckBox Text="For Department Report" ID="ChkForDepartmentReport" CssClass="checkboxbold"
                                runat="server" Visible="false" AutoPostBack="true" OnCheckedChanged="ChkForDepartmentReport_CheckedChanged" /><br />
                            &nbsp;<asp:CheckBox Text="Hindi Format Folio" ID="ChkHindiFormat" CssClass="checkboxbold"
                                runat="server" Visible="false" AutoPostBack="true" OnCheckedChanged="ChkHindiFormat_CheckedChanged" /><br />
                            &nbsp;<asp:CheckBox Text="Folio Material Detail" ID="ChkFolioMaterialDetail" CssClass="checkboxbold"
                                runat="server" Visible="false" AutoPostBack="true" OnCheckedChanged="ChkFolioMaterialDetail_CheckedChanged" /><br />
                            &nbsp;<asp:CheckBox Text="Folio Material Detail New" ID="ChkFolioMaterialDetailNew"
                                CssClass="checkboxbold" runat="server" Visible="false" AutoPostBack="true" OnCheckedChanged="ChkFolioMaterialDetailNew_CheckedChanged" /><br />
                            &nbsp;<asp:CheckBox Text="Print Without BarCode" ID="ChkWithoutBarCode" CssClass="checkboxbold" runat="server" Visible="true"/>
                        </td>
                        <td align="right">
                            <asp:Button ID="BtnStockNoStatus" Text="StockNo Status" CssClass="buttonnorm" runat="server"
                                OnClick="BtnStockNoStatus_Click" />
                            <asp:Button ID="BtnPrintSummaryFolio" Text="Folio Summary" CssClass="buttonnorm"
                                runat="server" OnClick="BtnPrintSummaryFolio_Click" />
                            <asp:Button ID="btnprint" Text="Preview" CssClass="buttonnorm" runat="server" OnClick="btnprint_Click" />
                            <asp:Button ID="btnfinalfolio" Text="Final Folio" CssClass="buttonnorm" runat="server"
                                OnClick="btnfinalfolio_Click" Visible="false" OnClientClick="return confirm('Do you want to final this Folio No.?')" />
                            <asp:Button ID="btnclose" Text="Close" CssClass="buttonnorm" runat="server" OnClientClick="return CloseForm();" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="lblmsg" CssClass="labelbold" ForeColor="Red" Text="" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnprint" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
