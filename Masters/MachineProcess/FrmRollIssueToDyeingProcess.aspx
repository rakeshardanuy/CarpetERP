﻿<%@ Page Title="Roll Issue to Dyeing" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" CodeFile="FrmRollIssueToDyeingProcess.aspx.cs"
    Inherits="Masters_MachineProcess_FrmRollIssueToDyeingProcess" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="page" ContentPlaceHolderID="CPH_Form" runat="server">
    <script src="../../Scripts/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../Scripts/FixFocus2.js"></script>
    <script type="text/javascript">
        function CloseForm() {
            window.location.href = "../../main.aspx";
        }
        function ClickNew() {
            window.location.href = "FrmRollIssueToDyeingProcess.aspx";
        }

        function isNumber(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
                alert('numeric only');
                return false;
            }

            else {
                return true;
            }
        }
        function Addsize() {
            var answer = confirm("Do you want to ADD?")
            if (answer) {
                var Shapeid = 0;
                if (document.getElementById('CPH_Form_ddshape')) {
                    var e = document.getElementById("CPH_Form_ddshape");
                    if (e.options.length > 0) {
                        var Shapeid = e.options[e.selectedIndex].value;
                    }
                }
                e = document.getElementById("CPH_Form_DDUnit");
                var VarUnitID = e.options[e.selectedIndex].value;
                window.open('../Carpet/AddSize.aspx?ShapeID=' + Shapeid + '&UnitID=' + VarUnitID + '', '', 'width=1000px,Height=501px');
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

        function CheckBoxClick(objref) {

            var row = objref.parentNode.parentNode;
            if (objref.checked) {
                row.style.backgroundColor = "Orange";
            }
            else {
                row.style.backgroundColor = "White";
            }
        }
        function Validation() {
            if (document.getElementById("<%=ddCompName.ClientID %>").value <= "0") {
                alert("Pls Select Company Name");
                document.getElementById("<%=ddCompName.ClientID %>").focus();
                return false;
            }
            if (document.getElementById("<%=DDProcessName.ClientID %>").value <= "0") {
                alert("Pls Select Process Name");
                document.getElementById("<%=DDProcessName.ClientID %>").focus();
                return false;
            }

            if (document.getElementById("<%=Td3.ClientID %>")) {
                if (document.getElementById('CPH_Form_DDIssueNo').value <= "0") {
                    alert("Please select Issue no....!");
                    document.getElementById("CPH_Form_DDIssueNo").focus();
                    return false;
                }
            }
            if (document.getElementById("<%=TxtIssueDate.ClientID %>").value == "") {
                alert("Pls Select Issue Date");
                document.getElementById("<%=TxtIssueDate.ClientID %>").focus();
                return false;
            }
            if (document.getElementById("<%=DDIssueNo.ClientID %>")) {
                if (document.getElementById('CPH_Form_DDIssueNo').value <= "0") {
                    alert("Please Select Issue No....!");
                    document.getElementById("CPH_Form_DDIssueNo").focus();
                    return false;
                }
            }
            if (document.getElementById("<%=txtEwayBillNo.ClientID %>").value == "") {
                alert("Pls Insert EWayBill No");
                document.getElementById("<%=txtEwayBillNo.ClientID %>").focus();
                return false;
            }
            if (document.getElementById("<%=txtVehicleNo.ClientID %>").value == "") {
                alert("Pls Insert Vehicle No");
                document.getElementById("<%=txtVehicleNo.ClientID %>").focus();
                return false;
            }
            if (document.getElementById("<%=txtHSNCode.ClientID %>").value == "") {
                alert("Pls Insert HSNCode No");
                document.getElementById("<%=txtHSNCode.ClientID %>").focus();
                return false;
            }
            else {
                return confirm('Do you want to save data?')
            }
        }
    </script>
    <asp:UpdatePanel ID="up1" runat="server">
        <ContentTemplate>
            <div>
                <table>
                    <tr id="Tr1" runat="server">
                        <td id="Td1" colspan="2" class="tdstyle">
                            <asp:Label ID="Label1" runat="server" Text="Company Name" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList ID="ddCompName" runat="server" Width="150px" CssClass="dropdown">
                            </asp:DropDownList>
                        </td>
                        <td id="Td4" class="tdstyle">
                            <asp:Label ID="Label2" runat="server" Text=" Process Name" CssClass="labelbold"></asp:Label>
                            &nbsp;
                            <asp:CheckBox ID="ChKForEdit" runat="server" Text=" For Edit" CssClass="checkboxbold"
                                OnCheckedChanged="ChKForEdit_CheckedChanged" AutoPostBack="true" />
                            <br />
                            <asp:DropDownList ID="DDProcessName" runat="server" Width="150px" AutoPostBack="True"
                                CssClass="dropdown">
                            </asp:DropDownList>
                        </td>
                        <td id="Td2" class="tdstyle">
                            <asp:Label ID="Label3" runat="server" Text="Machine No" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList ID="DDEmployeeName" runat="server" Width="150px" CssClass="dropdown"
                                AutoPostBack="True" OnSelectedIndexChanged="DDEmployeeName_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td id="Td3" runat="server" visible="false">
                            <asp:Label ID="Label17" runat="server" Text="Issue No" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList ID="DDIssueNo" runat="server" Width="150px" CssClass="dropdown"
                                AutoPostBack="True" OnSelectedIndexChanged="DDIssue_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td id="Td5" class="tdstyle">
                            <asp:Label ID="Label4" runat="server" Text="Issue Date" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="TxtIssueDate" runat="server" Width="100px" CssClass="textb"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MMM-yyyy"
                                TargetControlID="TxtIssueDate">
                            </asp:CalendarExtender>
                        </td>
                        <td id="Td6" class="tdstyle">
                            <asp:Label ID="Label5" runat="server" Text=" Issue No." CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="TxtIssueNo" Width="100px" runat="server" CssClass="textb" ReadOnly="true"
                                onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>
                         <td id="Td7" class="tdstyle">
                            <asp:Label ID="Label6" runat="server" Text=" Eway BillNo" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtEwayBillNo" Width="100px" runat="server" CssClass="textb" ReadOnly="false"
                                onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>
                        <td id="Td8" class="tdstyle">
                            <asp:Label ID="Label7" runat="server" Text=" Vehicle No" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtVehicleNo" Width="100px" runat="server" CssClass="textb" ReadOnly="false"
                                onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>
                        <td id="Td9" class="tdstyle">
                            <asp:Label ID="Label8" runat="server" Text=" HSN Code" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtHSNCode" Width="100px" runat="server" CssClass="textb" ReadOnly="false"
                                onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <div id="gride" runat="server" style="max-height: 400px; overflow: auto">
                                <asp:GridView ID="DG" AutoGenerateColumns="False" runat="server" CssClass="grid-views"
                                    OnRowDataBound="DG_RowDataBound">
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
                                         
                                           <asp:TemplateField HeaderText="Sub Roll No">
                                            <ItemTemplate>
                                                <asp:Label ID="LblMaterialReceiveInPcsDetailID" Text='<%#Bind("MaterialReceiveInPcsDetailID") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sub Sub Roll No">
                                            <ItemTemplate>
                                                <asp:Label ID="LblRollNo" Text='<%#Bind("RollReceiveToNextDetailID") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Order No">
                                            <ItemTemplate>
                                                <asp:Label ID="LblOrderNo" Text='<%#Bind("OrderNo") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Description">
                                            <ItemTemplate>
                                                <asp:Label ID="LblItemDescription" Text='<%#Bind("ItemDescription") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Unit">
                                            <ItemTemplate>
                                                <asp:Label ID="LblUnit" Text='<%#Bind("UnitName")%>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qty">
                                            <ItemTemplate>
                                                <asp:Label ID="LblQty" Text='<%#Bind("Qty")%>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="LblRollReceiveToNextID" Text='<%#Bind("RollReceiveToNextID") %>' runat="server" />
                                                <asp:Label ID="LblRollReceiveToNextDetailID" Text='<%#Bind("RollReceiveToNextDetailID") %>'
                                                    runat="server" />
                                                <asp:Label ID="LblUnitID" Text='<%#Bind("UnitID") %>' runat="server" />
                                                <asp:Label ID="lblRollReceiveOtherProcessId" Text='<%#Bind("RollReceiveOtherProcessId") %>' runat="server" />
                                                 <asp:Label ID="lblRollReceiveOtherProcessDetailId" Text='<%#Bind("RollReceiveOtherProcessDetailId") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
                <table style="width: 80%;">

                 <tr>
                  <td id="Td11" class="tdstyle" align="left" style="padding-left:20px">
                            <asp:Label ID="Label10" runat="server" Text=" Amount" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtAmount" Width="100px" runat="server" CssClass="textb" ReadOnly="false" onkeypress="return isNumber(event);"
                               onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>
                         <td id="Td10" class="tdstyle">
                            <asp:Label ID="Label9" runat="server" Text=" Remarks" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtRemarks" Width="350px" runat="server" Height="50px" TextMode="MultiLine" CssClass="textb" ReadOnly="false"
                                onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>

                        <td id="Td12" class="tdstyle">
                            <asp:Label ID="Label11" runat="server" Text=" Total Qty" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtTotalQty" Width="100px" runat="server" CssClass="textb" ReadOnly="true"
                                onkeydown="return (event.keyCode!=13);"></asp:TextBox>
                        </td>
                       
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lblmessage" Text="" runat="server" CssClass="labelbold" ForeColor="Red" />
                        </td>
                        <td colspan="8" align="right">
                            <asp:Button ID="btnnew" runat="server" Text="New" OnClientClick="return ClickNew()"
                                CssClass="buttonnorm" />
                            <asp:Button ID="btnsave" runat="server" Text="Save" OnClick="btnsave_Click" OnClientClick="return Validation();"
                                CssClass="buttonnorm" />
                            <asp:Button ID="btnPreview" runat="server" Text="Preview" CssClass="buttonnorm preview_width"
                                OnClick="btnPreview_Click" Visible="false" />
                            <asp:Button ID="btnclose" runat="server" Text="Close" OnClientClick="return CloseForm(); "
                                CssClass="buttonnorm" />
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <div id="Div1" runat="server" style="max-height: 400px; overflow: auto">
                                <asp:GridView ID="gvdetail" AutoGenerateColumns="False" runat="server" CssClass="grid-views"
                                    OnRowDataBound="gvdetail_RowDataBound" OnRowDeleting="gvdetail_RowDeleting">
                                    <HeaderStyle CssClass="gvheaders" />
                                    <AlternatingRowStyle CssClass="gvalts" />
                                    <RowStyle CssClass="gvrow" />
                                    <EmptyDataRowStyle CssClass="gvemptytext" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sub Sub Roll No">
                                            <ItemTemplate>
                                                <asp:Label ID="LblRollNo" Text='<%#Bind("RollReceiveToNextDetailID") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Description">
                                            <ItemTemplate>
                                                <asp:Label ID="LblItemDescription" Text='<%#Bind("ItemDescription") %>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Unit">
                                            <ItemTemplate>
                                                <asp:Label ID="LblUnit" Text='<%#Bind("UnitName")%>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qty">
                                            <ItemTemplate>
                                                <asp:Label ID="LblQty" Text='<%#Bind("Qty")%>' runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                         <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"
                                                    Text="Del" OnClientClick="return confirm('Do You Want To Delete Row ?')"></asp:LinkButton>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" Width="50px" />
                                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRollIssueDyeingProcessID" Text='<%#Bind("RollIssueDyeingProcessID") %>' runat="server" />
                                                <asp:Label ID="lblRollIssueDyeingProcessDetailID" Text='<%#Bind("RollIssueDyeingProcessDetailID") %>'
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                       
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:HiddenField ID="HnIssueID" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>