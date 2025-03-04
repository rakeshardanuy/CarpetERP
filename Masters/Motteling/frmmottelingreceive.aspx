﻿<%@ Page Title="MOTTELING RECEIVE" Language="C#" MasterPageFile="~/ERPmaster.master"
    AutoEventWireup="true" Inherits="Masters_Motteling_frmmottelingreceive" Codebehind="frmmottelingreceive.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">
    <script src="../../Scripts/JScript.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript">
        function NewForm() {
            window.location.href = "frmmottelingreceive.aspx";
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
    </script>
    <script type="text/javascript">
        function Jscriptvalidate() {
            $(document).ready(function () {
                $("#<%=btnsave.ClientID %>").click(function () {
                    var Message = "";
                    var selectedindex = $("#<%=DDCompanyName.ClientID %>").attr('selectedIndex');
                    if (selectedindex < 0) {
                        Message = Message + "Please select Company Name!!\n";
                    }
                    selectedindex = $("#<%=DDProcessName.ClientID %>").attr('selectedIndex');
                    if (selectedindex <= 0) {
                        Message = Message + "Please Select Process. !!\n";
                    }
                    selectedindex = $("#<%=DDPartyName.ClientID %>").attr('selectedIndex');
                    if (selectedindex <= 0) {
                        Message = Message + "Please Select Vendor. !!\n";
                    }
                    selectedindex = $("#<%=DDissueno.ClientID %>").attr('selectedIndex');
                    if (selectedindex <= 0) {
                        Message = Message + "Please Select Indent No. !!\n";
                    }
                    if ($("#<%=TDBinNo.ClientID %>").is(':visible')) {
                        var selectedindex = $("#<%=DDBinNo.ClientID %>").attr('selectedIndex');
                        if (selectedindex <= 0) {
                            Message = Message + "Please select Bin No. !!\n";
                        }
                    }
                    var txtchallanNo = document.getElementById('<%=txtchallanNo.ClientID %>');
                    if (txtchallanNo.value == "") {
                        Message = Message + "Please Enter Party Challan. !!\n";
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
    <asp:UpdatePanel ID="update1" runat="server">
        <ContentTemplate>
            <script type="text/javascript" language="javascript">
                Sys.Application.add_load(Jscriptvalidate);
            </script>
            <div>
                <table>
                    <tr id="TRedit" runat="server" visible="false">
                        <td>
                            <asp:CheckBox ID="chkedit" Text="Edit" CssClass="checkboxbold" Font-Size="Small"
                                runat="server" OnCheckedChanged="chkedit_CheckedChanged" AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:CheckBox ID="chkcomplete" Text="Fill Complete Issue No." CssClass="checkboxbold"
                                Font-Size="Small" runat="server" Visible="false" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                        <td id="TRempcodescan" runat="server" visible="false">
                            <asp:Label ID="Label21" Text="Enter/Scan Emp. Code" runat="server" CssClass="labelbold" />
                            <br />
                            <asp:TextBox ID="txtWeaverIdNoscan" CssClass="textb" runat="server" Width="150px"
                                Height="20px" AutoPostBack="true" OnTextChanged="txtWeaverIdNoscan_TextChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Company Name" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList CssClass="dropdown" ID="DDCompanyName" Width="150px" runat="server">
                            </asp:DropDownList>
                        </td>
                            <td class="tdstyle">
                                <asp:Label ID="Label33" runat="server" Text="BranchName" CssClass="labelbold"></asp:Label>
                                <br />
                                <asp:DropDownList CssClass="dropdown" ID="DDBranchName" Width="150" runat="server">
                                </asp:DropDownList>
                            </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Process Name" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList CssClass="dropdown" ID="DDProcessName" Width="170px" AutoPostBack="true"
                                runat="server" OnSelectedIndexChanged="DDProcessName_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Vendor Name" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList CssClass="dropdown" ID="DDPartyName" Width="180px" AutoPostBack="true"
                                runat="server" OnSelectedIndexChanged="DDPartyName_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="lblindentno" runat="server" Text=" Issue No." CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList ID="DDissueno" runat="server" CssClass="dropdown" AutoPostBack="true"
                                Width="100px" OnSelectedIndexChanged="DDissueno_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td id="TDPartychallan" runat="server" visible="false">
                            <asp:Label ID="Label6" runat="server" Text="Party Challan No." CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:DropDownList ID="DDpartychallan" runat="server" CssClass="dropdown" AutoPostBack="true"
                                Width="110px" OnSelectedIndexChanged="DDpartychallan_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td runat="server" visible="false">
                            <asp:Label ID="Label3" runat="server" Text=" Gate In No." CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtgateinno" CssClass="textb" Width="80px" runat="server" Enabled="false" />
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Party Challan No." CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox ID="txtchallanNo" CssClass="textb" Width="100px" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="Label8" runat="server" Text="Rec. Date" CssClass="labelbold"></asp:Label>
                            <br />
                            <asp:TextBox CssClass="textb" ID="txtrecdate" runat="server" Width="80px" TabIndex="7"></asp:TextBox>
                            <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MMM-yyyy"
                                TargetControlID="txtrecdate">
                            </asp:CalendarExtender>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblcheckedby" CssClass="labelbold" Text="Checked By" runat="server" />
                            <br />
                            <asp:TextBox ID="txtcheckedby" CssClass="textb" Width="200px" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="Label39" CssClass="labelbold" Text="Approved By" runat="server" />
                            <br />
                            <asp:TextBox ID="txtapprovedby" CssClass="textb" Width="200px" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="lblgodownname" Text="Godown Name" runat="server" CssClass="labelbold" />
                            <br />
                            <asp:DropDownList ID="DDgodown" Width="200px" CssClass="dropdown" AutoPostBack="true"
                                runat="server" OnSelectedIndexChanged="DDgodown_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td id="TDBinNo" runat="server" visible="false">
                            <asp:Label ID="Label9" Text="Bin No." runat="server" CssClass="labelbold" />
                            <br />
                            <asp:DropDownList ID="DDBinNo" Width="150px" CssClass="dropdown" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="Label10" CssClass="labelbold" Text="Issued Remark" runat="server" />
                            <br />
                            <asp:TextBox ID="txtissuedremark" Enabled="false" BackColor="LightGray" CssClass="textb"
                                Width="500px" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <fieldset>
                    <legend>
                        <asp:Label ID="lblrec" Text="Receive Details" runat="server" CssClass="labelbold"
                            ForeColor="Red" />
                    </legend>
                    <div id="gride" runat="server" style="max-height: 500px; overflow: auto">
                        <asp:GridView ID="DG" AutoGenerateColumns="False" runat="server" CssClass="grid-views"
                            Width="100%" OnRowDataBound="DG_RowDataBound">
                            <HeaderStyle CssClass="gvheaders" />
                            <AlternatingRowStyle CssClass="gvalts" />
                            <RowStyle CssClass="gvrow" />
                            <EmptyDataRowStyle CssClass="gvemptytext" />
                            <Columns>
                                <asp:TemplateField HeaderText="Receive Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRitemdescription" Text='<%#Bind("Ritemdescription") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rec Lot No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblrlotno" Text='<%#Bind("LotNo") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rec Tag No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblrtagno" Text='<%#Bind("TagNo") %>' runat="server" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Issd Qty">
                                    <ItemTemplate>
                                        <asp:Label ID="lblissqty" Text='<%#Bind("issuedqty") %>' runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Recd Qty">
                                    <ItemTemplate>
                                        <asp:Label ID="lblrecqty" Text='<%#Bind("Receivedqty") %>' runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Bal. Qty">
                                    <ItemTemplate>
                                        <asp:Label ID="lblbalqty" Text='<%# System.Math.Round(Convert.ToDouble(Eval("IssuedQty")) -Convert.ToDouble(Eval("Receivedqty")),3) %>'
                                            runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rate">
                                    <ItemTemplate>
                                        <asp:Label ID="lblrate" Text='<%#Bind("rate") %>' runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rec Qty">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtrecqty" Width="80px" runat="server" BackColor="Yellow" onkeypress="return isNumberKey(this);" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Loss Qty">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtlossqty" Width="70px" runat="server" BackColor="Yellow" onkeypress="return isNumberKey(this);" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No of Cone">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtnoofcone" Width="70px" BackColor="Yellow" runat="server" onkeypress="return isNumberKey(this);" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cone Type">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DDconetype" CssClass="dropdown" runat="server" OnSelectedIndexChanged="DDConetype_SelectedIndexchanges"
                                            AutoPostBack="true">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ply Type">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DDPly" CssClass="dropdown" runat="server" AutoPostBack="true"
                                            OnSelectedIndexChanged="DDPly_SelectedIndexChanged">
                                            <asp:ListItem Text="" />
                                            <asp:ListItem Text="1 Ply" />
                                            <asp:ListItem Text="2 Ply" />
                                            <asp:ListItem Text="3 Ply" />
                                            <asp:ListItem Text="4 Ply" />
                                            <asp:ListItem Text="5 Ply" />
                                            <asp:ListItem Text="6 Ply" />
                                            <asp:ListItem Text="7 Ply" />
                                            <asp:ListItem Text="8 Ply" />
                                            <asp:ListItem Text="9 Ply" />
                                            <asp:ListItem Text="10 Ply" />
                                            <asp:ListItem Text="11 Ply" />
                                            <asp:ListItem Text="12 Ply" />
                                            <asp:ListItem Text="8-32 Ply" />
                                            <asp:ListItem Text="30 Ply" />
                                            <asp:ListItem Text="2x3 Ply" />
                                            <asp:ListItem Text="1MM" />
                                            <asp:ListItem Text="2MM" />
                                            <asp:ListItem Text="3MM" />
                                            <asp:ListItem Text="4MM" />
                                            <asp:ListItem Text="5MM" />
                                            <asp:ListItem Text="6MM" />
                                            <asp:ListItem Text="7MM" />
                                            <asp:ListItem Text="8MM" />
                                            <asp:ListItem Text="10MM" />
                                            <asp:ListItem Text="15MM" />
                                            <asp:ListItem Text="20MM" />
                                            <asp:ListItem Text="30MM" />
                                            <asp:ListItem Text="35MM" />
                                            <asp:ListItem Text="40MM" />
                                            <asp:ListItem Text="60MM" />
                                            <asp:ListItem Text="70MM" />
                                            <asp:ListItem Text="15 Ply" />
                                            <asp:ListItem Text="21 Ply" />
                                            <asp:ListItem Text="13 Ply" />
                                            <asp:ListItem Text="14 Ply" />
                                            <asp:ListItem Text="20 Ply" />
                                            <asp:ListItem Text="28 Ply" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Transport">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DDTransport" CssClass="dropdown" runat="server" AutoPostBack="true"
                                            OnSelectedIndexChanged="DDTransport_SelectedIndexChanged">
                                            <asp:ListItem Text="" />
                                            <asp:ListItem Text="Self" />
                                            <asp:ListItem Text="Company" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="PenalityRate/Kgs.">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtpenalityrate" Width="70px" BackColor="Yellow" onkeypress="return isNumberKey(this);"
                                            runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Penality" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="txtpenalityamt" BackColor="Yellow" runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Moisture">
                                    <ItemTemplate>
                                        <asp:TextBox ID="TxtMoisture" Width="70px" BackColor="Yellow" Text='<%#Bind("Moisture") %>'
                                            runat="server" onkeypress="return isNumberKey(this);" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rec Moisture">
                                    <ItemTemplate>
                                        <asp:TextBox ID="TxtRecMoisture" Width="70px" BackColor="Yellow" runat="server" onkeypress="return isNumberKey(event);" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="BellWt">
                                    <ItemTemplate>
                                        <asp:TextBox ID="TxtBellWt" Width="70px" BackColor="Yellow" runat="server" onkeypress="return isNumberKey(event);" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblissueid" Text='<%#Bind("issueId") %>' runat="server" />
                                        <asp:Label ID="lblissuedetailid" Text='<%#Bind("IssueDetailid") %>' runat="server" />
                                        <asp:Label ID="lblrfinishedid" Text='<%#Bind("Rfinishedid") %>' runat="server" />
                                        <asp:Label ID="lblunitid" Text='<%#Bind("unitid") %>' runat="server" />
                                        <asp:Label ID="lblrflagsize" Text='<%#Bind("Rflagsize") %>' runat="server" />
                                        <asp:Label ID="lblconetype" Text='<%#Bind("conetype") %>' runat="server" />
                                        <asp:Label ID="lblritemid" Text='<%#Bind("item_id") %>' runat="server" />
                                        <asp:Label ID="lblrqualityid" Text='<%#Bind("Qualityid") %>' runat="server" />
                                        <asp:Label ID="lblrshadecolorid" Text='<%#Bind("shadecolorid") %>' runat="server" />
                                        <asp:Label ID="lblPlyType" Text='<%#Bind("PlyType") %>' runat="server" />
                                        <asp:Label ID="lblTransportType" Text='<%#Bind("TransportType") %>' runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </fieldset>
            </div>
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="lblmsg" CssClass="labelbold" ForeColor="Red" runat="server" />
                    </td>
                    <td align="right">
                        <asp:Button ID="btnsave" CssClass="buttonnorm" Text="Save" runat="server" OnClick="btnsave_Click" />
                        <asp:Button ID="btnclose" CssClass="buttonnorm" Text="Close" runat="server" OnClientClick="return CloseForm();" />
                        <asp:Button ID="BtnComplete" runat="server" Visible="false" CssClass="buttonnorm"
                            Text="Complete" OnClick="BtnComplete_Click" />
                        <asp:Button ID="btnpreview" CssClass="buttonnorm" Text="Preview" runat="server" OnClick="btnpreview_Click" />
                        <asp:Button ID="btnnew" CssClass="buttonnorm" Text="New" runat="server" OnClientClick="return NewForm();" />
                    </td>
                </tr>
            </table>
            <div id="Div1" runat="server" style="max-height: 500px; overflow: auto">
                <asp:GridView ID="DGrecdetail" AutoGenerateColumns="False" runat="server" CssClass="grid-views"
                    Width="100%" OnRowEditing="DGrecdetail_RowEditing" OnRowCancelingEdit="DGrecdetail_RowCancelingEdit"
                    OnRowDeleting="DGrecdetail_RowDeleting" OnRowUpdating="DGrecdetail_RowUpdating"
                    OnRowDataBound="DGrecdetail_RowDataBound">
                    <HeaderStyle CssClass="gvheaders" />
                    <AlternatingRowStyle CssClass="gvalts" />
                    <RowStyle CssClass="gvrow" />
                    <EmptyDataRowStyle CssClass="gvemptytext" />
                    <Columns>
                        <asp:TemplateField HeaderText="ReceiveDescription">
                            <ItemTemplate>
                                <asp:Label ID="lblRitemdescription" Text='<%#Bind("RItemdescription") %>' runat="server" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Godown">
                            <ItemTemplate>
                                <asp:Label ID="lblgodownname" Text='<%#Bind("godownname") %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Lot No">
                            <ItemTemplate>
                                <asp:Label ID="lbllotno" Text='<%#Bind("Lotno") %>' runat="server" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Tag No">
                            <ItemTemplate>
                                <asp:Label ID="lbltagno" Text='<%#Bind("Tagno") %>' runat="server" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rec Qty">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtrecqty" Text='<%#Bind("recqty") %>' runat="server" Width="70px"
                                    onkeypress="return isNumberKey(this);" BackColor="Yellow" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblrecqty" Text='<%#Bind("recqty") %>' runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Loss Qty">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtlossqty" Text='<%#Bind("lossqty") %>' runat="server" Width="70px"
                                    onkeypress="return isNumberKey(this);" BackColor="Yellow" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblossqty" Text='<%#Bind("Lossqty") %>' runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Undy. Qty">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtundyedqtyedit" Text='<%#Bind("undyedqty") %>' runat="server"
                                    Width="70px" onkeypress="return isNumberKey(this);" BackColor="Yellow" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblundyedqty" Text='<%#Bind("undyedqty") %>' runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rate">
                            <ItemTemplate>
                                <asp:TextBox ID="lblrate" runat="server" CssClass="textb" Text='<%#Bind("rate") %>'
                                    Width="70px" BackColor="Yellow" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PenalityRate/Kgs.">
                            <ItemTemplate>
                                <asp:TextBox ID="txtpenalityrate_rec" runat="server" CssClass="textb" Text='<%#Bind("penalityrate") %>'
                                    Width="70px" BackColor="Yellow" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Penality Amt.">
                            <ItemTemplate>
                                <asp:Label ID="txtpenalityamtrec" runat="server" CssClass="textb" Text='<%#Bind("Penalityamt") %>'
                                    Width="70px" BackColor="Yellow" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </asp:TemplateField>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblid" Text='<%#Bind("Id") %>' runat="server" />
                                <asp:Label ID="lbldetailid" Text='<%#Bind("detailid") %>' runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkdel" runat="server" CausesValidation="False" CommandName="Delete"
                                    Text="DEL" OnClientClick="return confirm('Do you want to Delete data?')"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkeditrate" runat="server" CausesValidation="False" Text="UPDATE RATE/PENALITY"
                                    OnClick="lnkeditrate_Click"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowEditButton="false" />
                    </Columns>
                </asp:GridView>
            </div>
            <style type="text/css">
                #mask
                {
                    position: fixed;
                    left: 0px;
                    top: 0px;
                    z-index: 4;
                    opacity: 0.4;
                    -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=40)"; /* first!*/
                    filter: alpha(opacity=40); /* second!*/
                    background-color: Gray;
                    display: none;
                    width: 100%;
                    height: 100%;
                }
            </style>
            <script type="text/javascript" language="javascript">
                function ShowPopup() {
                    $('#mask').show();
                    $('#<%=pnlpopup.ClientID %>').show();
                }
                function HidePopup() {
                    $('#mask').hide();
                    $('#<%=pnlpopup.ClientID %>').hide();
                }
                $(".btnPwd").live('click', function () {
                    HidePopup();
                });
            </script>
            <div id="mask">
            </div>
            <asp:Panel ID="pnlpopup" runat="server" BackColor="White" Height="175px" Width="300px"
                Style="z-index: 111; background-color: White; position: absolute; left: 35%;
                top: 40%; border: outset 2px gray; padding: 5px; display: none">
                <table width="100%" style="width: 100%; height: 100%;" cellpadding="0" cellspacing="5">
                    <tr style="background-color: #8B7B8B; height: 1px">
                        <td colspan="2" style="color: White; font-weight: bold; font-size: 1.2em; padding: 3px"
                            align="center">
                            ENTER PASSWORD <a id="closebtn" style="color: white; float: right; text-decoration: none"
                                class="btnPwd" href="#">X</a>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            Enter Password:
                        </td>
                        <td>
                            <asp:TextBox ID="txtpwd" runat="server" TextMode="Password" Height="30px" Width="174px"
                                OnTextChanged="txtpwd_TextChanged" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td align="right">
                            <input type="button" value="Cancel" class="btnPwd" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label7" CssClass="labelbold" ForeColor="Red" Text="" runat="server" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:HiddenField ID="hnid" runat="server" Value="0" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
