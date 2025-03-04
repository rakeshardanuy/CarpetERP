﻿<%@ Page Title="Punch Card Receive On Production Order" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true"
    Inherits="Masters_PunchCardIndent_FrmPunchCardIndentReceive_OnProductionOrder" Codebehind="FrmPunchCardIndentReceive_OnProductionOrder.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <script src="../../Scripts/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <%--<script src="../../Scripts/Fixfocus.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        function NewForm() {
            window.location.href = "FrmPunchCardIndentReceive_OnProductionOrder.aspx";
        }
        function CloseForm() {
            window.location.href = "../../main.aspx";
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            else {
                return true;
            }
        }
        function CheckBoxClick(objref) {

            var row = objref.parentNode.parentNode;
            if (objref.checked) {
                row.style.backgroundColor = "Orange";
            }
            else {
                row.style.backgroundColor = "White";
            }
        }
        function CheckAll(objref) {
            var gridview = objref.parentNode.parentNode.parentNode;
            var inputlist = gridview.getElementsByTagName("input");
            for (var i = 0; i < inputlist.length; i++) {
                var row = inputlist[i].parentNode.parentNode;
                if (inputlist[i].type == "checkbox" && objref != inputlist[i]) {
                    if (objref.checked) {

                        inputlist[i].checked = true;
                        row.style.backgroundColor = "Orange";
                    }
                    else {
                        inputlist[i].checked = false;
                        row.style.backgroundColor = "White";

                    }
                }
            }

        }

    </script>
    <div>
        <asp:UpdatePanel ID="upd2" runat="server">
            <ContentTemplate>
                <fieldset>
                    <legend>
                        <asp:Label ID="lblMasterDetail" runat="server" Text="Master Details" CssClass="labelbold"
                            ForeColor="Red" Font-Bold="true"></asp:Label></legend>
                    <table>
                        <tr>
                            <td id="TDEdit" runat="server" visible="false">
                                <asp:CheckBox ID="chkEdit" Text="For Edit" CssClass="labelbold" runat="server" AutoPostBack="true"
                                    OnCheckedChanged="chkEdit_CheckedChanged" />
                            </td>
                        </tr>
                        <tr>
                            <%-- <td id="TDEMPEDIT" runat="server" visible="true">
                                    <asp:Label ID="lblempcodeedit" CssClass="labelbold" Text="Emp. Code." runat="server" />
                                    <br />
                                    <asp:TextBox ID="txteditempcode" CssClass="textb" runat="server" Width="80px" />
                                    <asp:TextBox ID="txteditempid" runat="server" Style="display: none"></asp:TextBox>
                                    <asp:Button ID="btnsearchedit" runat="server" Text="Button" OnClick="btnsearchedit_Click"
                                        Style="display: none;" />
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" BehaviorID="SrchAutoComplete1"
                                        CompletionInterval="20" Enabled="True" ServiceMethod="GetEmployeeAll" EnableCaching="true"
                                        CompletionSetCount="20" OnClientItemSelected="EmpSelectedEdit" ServicePath="~/Autocomplete.asmx"
                                        TargetControlID="txteditempcode" UseContextKey="True" ContextKey="0#0#0" MinimumPrefixLength="2">
                                    </asp:AutoCompleteExtender>
                                </td>--%>
                            <td id="TDFolioNotext" runat="server" visible="true">
                                <asp:Label ID="Label1" CssClass="labelbold" Text="Folio No." runat="server" />
                                <br />
                                <asp:TextBox ID="txtfolionoedit" CssClass="textb" Width="150px" runat="server" AutoPostBack="true"
                                    OnTextChanged="txtfolionoedit_TextChanged" />
                            </td>
                            <%--  <td id="TDChallanNo" runat="server" visible="true">
                                    <asp:Label ID="Label10" CssClass="labelbold" Text="Challan No." runat="server" />
                                    <br />
                                    <asp:TextBox ID="txtChallanNo" CssClass="textb" Width="150px" runat="server" AutoPostBack="true"
                                        OnTextChanged="txtChallanNo_TextChanged" />
                                </td>--%>
                            <td>
                                <asp:Label ID="Label9" runat="server" Text="Company Name" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList ID="DDcompany" runat="server" CssClass="dropdown" Width="150px">
                                </asp:DropDownList>
                            </td>
                            <td id="Tddropdowncustcode" runat="server">
                                <asp:Label ID="Label2" runat="server" Text="Customer Code" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDCustomerCode" Width="150px" runat="server"
                                    AutoPostBack="True" OnSelectedIndexChanged="DDCustomerCode_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label3" runat="server" Text="Order No" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDCustomerOrderNumber" Width="150px" runat="server"
                                    AutoPostBack="True" OnSelectedIndexChanged="DDCustomerOrderNumber_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text="Process Name" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDProcessName" runat="server" AutoPostBack="True"
                                    Width="150px" OnSelectedIndexChanged="DDProcessName_SelectedIndexChanged1">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label11" runat="server" Text="Emp Name" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDEmployeeName" Width="150px" runat="server"
                                    OnSelectedIndexChanged="DDEmployeeName_SelectedIndexChanged" AutoPostBack="True">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label12" runat="server" Text="Folio No" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDPOrderNo" runat="server" AutoPostBack="True"
                                    Width="150px" Visible="true" OnSelectedIndexChanged="DDPOrderNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="TRDescription" runat="server" visible="false">
                            <td id="tdUnitId" runat="server" visible="false">
                                <asp:Label ID="Label13" runat="server" Text="Unit" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDunit" runat="server" Width="100px" OnSelectedIndexChanged="DDunit_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                            <td id="TDItemCode1" runat="server" visible="false">
                                <asp:Label ID="Label14" runat="server" Text="Item Code" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:TextBox ID="TxtItemCode" CssClass="textb" runat="server" Width="90px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="Label15" runat="server" Text="Category Name" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDCategoryName" Width="150px" runat="server"
                                    AutoPostBack="True" OnSelectedIndexChanged="DDCategoryName_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label16" runat="server" Text="Item Name" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDItemName" Width="150px" runat="server"
                                    AutoPostBack="True" OnSelectedIndexChanged="DDItemName_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td colspan="3">
                                <asp:Label ID="Label17" runat="server" Text="Description" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDDescription" runat="server" Width="500px"
                                    AutoPostBack="True" OnSelectedIndexChanged="DDDescription_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <%-- <td id="TDFolioNo" runat="server" visible="true">
                                <asp:Label ID="Label7" runat="server" Text="Folio No." CssClass="labelbold"></asp:Label><br />
                                <asp:DropDownList ID="DDFolioNo" runat="server" CssClass="dropdown" AutoPostBack="true"
                                    Width="150px" OnSelectedIndexChanged="DDFolioNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td id="TDMapStencilType" runat="server" visible="true">
                                <asp:Label ID="Label3" runat="server" Text="Map/Trace Type" CssClass="labelbold"></asp:Label><br />
                                <asp:DropDownList ID="DDMapStencilType" runat="server" CssClass="dropdown" Width="150px"
                                    Enabled="true" AutoPostBack="true" OnSelectedIndexChanged="DDMapStencilType_SelectedIndexChanged">
                                    <asp:ListItem Value="1" Text="Map" Selected="True">Map</asp:ListItem>
                                    <asp:ListItem Value="2" Text="Trace">Trace</asp:ListItem>
                                </asp:DropDownList>
                            </td>--%>
                            <td id="TDIssueNo" runat="server" visible="true">
                                <asp:Label ID="Label5" runat="server" Text="Issue No." CssClass="labelbold"></asp:Label><br />
                                <asp:DropDownList ID="DDIssueNo" runat="server" CssClass="dropdown" AutoPostBack="true"
                                    Width="150px" OnSelectedIndexChanged="DDIssueNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label4" runat="server" Text="Receive Date" CssClass="labelbold"></asp:Label><br />
                                <asp:TextBox ID="txtReceiveDate" CssClass="textb" Width="150px" runat="server" />
                                <asp:CalendarExtender ID="cal1" TargetControlID="txtReceiveDate" Format="dd-MMM-yyyy"
                                    runat="server">
                                </asp:CalendarExtender>
                            </td>
                            <td id="TDReceiveNo" runat="server" visible="false">
                                <asp:Label ID="Label10" runat="server" Text="Receive No." CssClass="labelbold"></asp:Label><br />
                                <asp:DropDownList ID="DDReceiveNo" runat="server" CssClass="dropdown" AutoPostBack="true"
                                    Width="150px" OnSelectedIndexChanged="DDReceiveNo_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:Label ID="Label8" runat="server" Text="Receive No" CssClass="labelbold"></asp:Label><br />
                                <asp:TextBox ID="txtReceiveNo" CssClass="textb" Width="61px" runat="server" Enabled="false" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <fieldset>
                    <legend>
                        <asp:Label ID="Label6" runat="server" Text="Issue Details" CssClass="labelbold" ForeColor="Red"
                            Font-Bold="true"></asp:Label></legend>
                    <table>
                        <tr>
                            <td>
                                <div id="gride" runat="server" style="max-height: 500px; overflow: auto">
                                    <asp:GridView ID="DG" AutoGenerateColumns="False" runat="server" CssClass="grid-views"
                                        EmptyDataText="No. Records found." OnRowDataBound="DG_RowDataBound">
                                        <HeaderStyle CssClass="gvheaders" />
                                        <AlternatingRowStyle CssClass="gvalts" />
                                        <RowStyle CssClass="gvrow" />
                                        <EmptyDataRowStyle CssClass="gvemptytext" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="">
                                                <HeaderTemplate>
                                                    <asp:CheckBox ID="ChkAllItem" runat="server" onclick="return CheckAll(this);" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="Chkboxitem" runat="server" onclick="return CheckBoxClick(this);" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" Width="10px" />
                                            </asp:TemplateField>
                                            <%-- <asp:TemplateField HeaderText="Raw Material Description">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblitemdescription" Text='<%#Bind("ItemDescription") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField HeaderText="ItemName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblItemName" Text='<%#Bind("Item_Name")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="QualityName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQualityName" Text='<%#Bind("QualityName")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Design">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDesignName" Text='<%#Bind("DesignName")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Color">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblColorName" Text='<%#Bind("ColorName")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Shape">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblShapeName" Text='<%#Bind("ShapeName")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Size">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSize" Text='<%#Bind("Size")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="NoOfSet">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNoOfSet" Text='<%#Bind("NoOfSet")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="PerSet Qty">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPerSetQty" Text='<%#Bind("PerSetQty")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="StockNoSeries">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStockNoSeries" Text='<%#Bind("StockNoSeries")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            
                                            <%-- <asp:TemplateField HeaderText="Pre ReceiveNoOf Set">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPreReceiveQty" Text='<%#Bind("PreReceiveQty")%>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField> 
                                         <asp:TemplateField HeaderText="Receive NoOf Set">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtReceiveNoOfSet" Width="70px" BackColor="Yellow" runat="server"
                                                        onkeypress="return isNumberKey(event);" />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                            <asp:TemplateField Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblItemFinishedId" Text='<%#Bind("ItemFinishedId") %>' runat="server" />
                                                    <asp:Label ID="lblOrderId" Text='<%#Bind("OrderID") %>' runat="server" />
                                                    <asp:Label ID="lblPCIIssueId" Text='<%#Bind("PCIIssueId") %>' runat="server" />
                                                    <asp:Label ID="lblPCIIssueDetailId" Text='<%#Bind("PCIIssueDetailId") %>' runat="server" />
                                                    <asp:Label ID="lblPunchCardIndentType" Text='<%#Bind("PunchCardIndentType") %>' runat="server" />
                                                    <asp:Label ID="lblSNSID" Text='<%#Bind("SNSID") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </fieldset>
                <%--<fieldset>
                    <legend>
                        <asp:Label ID="Label5" Text="Order Description" runat="server" CssClass="labelbold"
                            ForeColor="Red" />
                    </legend>
                    <table>
                        <tr>
                            <td id="Tdstockno" runat="server" visible="true">
                                <table border="1" cellspacing="5">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label6" Text="Enter Map/Trace No." CssClass="labelbold" Font-Size="Small"
                                                runat="server" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtMapStencilstockno" CssClass="textb" Height="40px" Width="250px"
                                                runat="server" onKeypress="KeyDownHandler(event);" />
                                            <asp:Button ID="btnMapStencilStockNo" runat="server" Style="display: none" OnClick="txtMapStencilstockno_TextChanged" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                       
                    </table>
                </fieldset>--%>
                <div>
                    <table width="100%">
                        <tr>
                            <td>
                                <asp:Label ID="lblmessage" Text="" runat="server" CssClass="labelbold" ForeColor="Red" />
                            </td>
                            <td align="right">
                                <asp:Button ID="btnnew" runat="server" Text="New" CssClass="buttonnorm" OnClientClick="return NewForm();" />
                                <asp:Button ID="btnsave" runat="server" Text="Save" CssClass="buttonnorm" OnClick="btnsave_Click" />
                                <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="buttonnorm" OnClick="btnPreview_Click" />
                                <asp:Button ID="btnclose" runat="server" Text="Close" CssClass="buttonnorm" OnClientClick="return CloseForm();" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div style="max-height: 300px; overflow: auto;">
                   <asp:GridView ID="gvdetail" runat="server" AutoGenerateColumns="False" EmptyDataText="No records found.."
                        OnRowDataBound="RowDataBound" OnRowDeleting="gvdetail_RowDeleting">
                        <HeaderStyle CssClass="gvheaders" />
                        <AlternatingRowStyle CssClass="gvalts" />
                        <Columns>
                            <asp:TemplateField HeaderText="ItemName">
                                <ItemTemplate>
                                    <asp:Label ID="lblItemName" Text='<%#Bind("Item_Name")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="QualityName">
                                <ItemTemplate>
                                    <asp:Label ID="lblQualityName" Text='<%#Bind("QualityName")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Design">
                                <ItemTemplate>
                                    <asp:Label ID="lblDesignName" Text='<%#Bind("DesignName")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Color">
                                <ItemTemplate>
                                    <asp:Label ID="lblColorName" Text='<%#Bind("ColorName")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Shape">
                                <ItemTemplate>
                                    <asp:Label ID="lblShapeName" Text='<%#Bind("ShapeName")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Size">
                                <ItemTemplate>
                                    <asp:Label ID="lblSize" Text='<%#Bind("Size")%>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="NoOFSet">
                                <ItemTemplate>
                                    <asp:Label ID="lblRecNoOfSet" Text='<%#Bind("RecNoOfSet") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PerSet Qty">
                                <ItemTemplate>
                                    <asp:Label ID="lblRecPerSetQty" Text='<%#Bind("RecPerSetQty") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="StockNo Series">
                                <ItemTemplate>
                                    <asp:Label ID="lblStockNoSeries" Text='<%#Bind("StockNoSeries") %>' runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="StockNo" Visible="true">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkPCStockNo" runat="server" CausesValidation="False" Text="PCStockNo"
                                                        OnClientClick="return confirm('Do you want to add missing no?')" OnClick="lnkPCStockNo_Click"></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                            <asp:TemplateField Visible="false">
                                <ItemTemplate>
                                <asp:Label ID="lblPCIReceiveId" Text='<%#Bind("PCIReceiveId") %>' runat="server" />
                                <asp:Label ID="lblPCIReceiveDetailId" Text='<%#Bind("PCIReceiveDetailId") %>' runat="server" />
                                    <asp:Label ID="lblPCIIssueId" Text='<%#Bind("PCIIssueId") %>' runat="server" />
                                    <asp:Label ID="lblPCIIssueDetailId" Text='<%#Bind("PCIIssueDetailId") %>' runat="server" />
                                    <asp:Label ID="lblSNSID" Text='<%#Bind("SNSID") %>' runat="server" />
                                    <asp:Label ID="lblItemFinishedID" Text='<%#Bind("ItemFinishedID") %>' runat="server" />
                                   
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="DEL" OnClientClick="return confirm('Do you want to Delete data?')"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                        </Columns>
                        <RowStyle CssClass="gvrow" />
                        <EmptyDataRowStyle CssClass="gvemptytext" />
                    </asp:GridView>
                </div>
                <asp:HiddenField ID="hnReceiveid" runat="server" />
                <asp:HiddenField ID="hnissueorderid" runat="server" />
                <div>
                    <asp:Button runat="server" ID="btnModalPopUp" Style="display: none" />
                    <asp:ModalPopupExtender ID="Modalpopupext" runat="server" PopupControlID="pnModelPopup"
                        TargetControlID="btnModalPopUp" BackgroundCssClass="modalBackground" CancelControlID="btnqcclose">
                    </asp:ModalPopupExtender>
                    <asp:Panel ID="pnModelPopup" runat="server" CssClass="modalPopup" Style="display: none">
                        <fieldset>
                            <legend>
                                <asp:Label ID="lblqc" Text="Punch Card StockNo" runat="server" ForeColor="Black" CssClass="labelbold" />
                            </legend>
                            <table>
                                <tr>
                                    <td>
                                        <div style="max-height: 250px; overflow: scroll; width: 350px" id="divqc">
                                            <asp:GridView ID="GVPunchCardStockNo" CssClass="grid-views" runat="server" AutoGenerateColumns="False"
                                                OnRowDataBound="GVPunchCardStockNo_RowDataBound">
                                                <HeaderStyle CssClass="gvheaders" />
                                                <AlternatingRowStyle CssClass="gvalts1" />
                                                <RowStyle CssClass="gvrow1" />
                                                <EmptyDataRowStyle CssClass="gvemptytext" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="">
                                                        <%--<HeaderTemplate>
                                                    <asp:CheckBox ID="ChkAllItem" runat="server" onclick="return CheckAll(this);" />
                                                </HeaderTemplate>--%>
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="Chkboxitem" runat="server" />
                                                            <%--<asp:CheckBox ID="Chkboxitem" runat="server" AutoPostBack="true" OnCheckedChanged="Chkboxitem_CheckedChanged" />--%>
                                                            <%--onclick="return CheckBoxClick(this);"--%>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="50px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="PC StockNo">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPCStockNo" Text='<%#Bind("PCStockNo") %>' runat="server" />
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="250px" />
                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Rate">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtRate" runat="server" Width="100px" Text='<%#Bind("rate") %>' />                                                            
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" Width="150px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblSNSID" Text='<%#Bind("SNSID") %>' runat="server" Visible="false" />
                                                            <asp:Label ID="lblSNID" Text='<%#Bind("SNID") %>' runat="server" Visible="false" />
                                                            <asp:Label ID="lblPCIReceiveID" Text='<%#Bind("PCIReceiveID") %>' runat="server" Visible="false" />
                                                            <asp:Label ID="lblPCIReceiveDetailId" Text='<%#Bind("PCIReceiveDetailId") %>' runat="server" Visible="false" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Button ID="BtnMissingPCStockNo" Text="Save" runat="server" CssClass="buttonnorm"
                                            OnClick="BtnMissingPCStockNo_Click" />
                                        <asp:Button ID="btnqcclose" Text="Close" runat="server" CssClass="buttonnorm" />

                                        <asp:Label ID="lblMissingPCIReceiveID" runat="server" CssClass="labelbold" ForeColor="Red"
                                Visible="false"></asp:Label>
                            <asp:Label ID="lblMissingPCIReceiveDetailId" runat="server" CssClass="labelbold"
                                ForeColor="Red" Visible="false"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblqcmsg" runat="server" CssClass="labelbold" ForeColor="Red"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </asp:Panel>
                </div>
                <asp:HiddenField ID="HnMissingID" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
