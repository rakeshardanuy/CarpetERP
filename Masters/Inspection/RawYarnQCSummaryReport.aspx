﻿<%@ Page Title="" Language="C#" MasterPageFile="~/ERPmaster.master" AutoEventWireup="true"
    CodeFile="RawYarnQCSummaryReport.aspx.cs" Inherits="Master_Inspection_RawYarnQCSummaryReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../../Content/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <script src="../../Content/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPH_Form" runat="Server">

    <div id="title-breadcrumb-option-demo" class="page-title-breadcrumb">
        <div class="page-header pull-left">
            <div class="page-title">RAW YARN TESTING  SUMMARY REPORT</div>

            <asp:Label ID="lblMessage" ForeColor="Red" runat="server"></asp:Label>
        </div>
        <div class="clearfix"></div>
    </div>



    <div class="page-content">

        <div class="row">

            <div class="col-lg-12">
                <div class="panel panel-violet">
                    <div class="panel-heading">

                        <div class="row">

                            <div class="col-lg-4">
                                <div class="caption">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlYarnType" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Raw Yarn Inspection" Value="1" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Dyed Yarn Inspection" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Fabric Inspection" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Carton Inspection" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="Papertube Inspection" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="Polybag Inspection" Value="6"></asp:ListItem>
                                        </asp:DropDownList>

                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-8">

                                <div class="actions">
                                    <asp:Button ID="tblDownload" ValidationGroup="Report" CssClass="btn btn-primary"
                                        runat="server" Text="Download" OnClick="tblDownload_Click" />

                                    &nbsp;
                                   
                            <div class="btn-group">
                            </div>
                                </div>
                            </div>


                        </div>
                        <div class="panel-body pan">


                            <div class="row">
                                <asp:UpdatePanel runat="server" ID="upd1">
                                    <ContentTemplate>


                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <div class="input-icon right">
                                                    <%--          <i class="fa fa-calendar"></i>--%>

                                                    <asp:TextBox placeholder="From Date" class="form-control input-sm" ID="txtFrom" runat="server"
                                                        CssClass="form-control input-sm"></asp:TextBox>
                                                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtFrom"
                                                        Format="dd/MM/yyyy">
                                                    </asp:CalendarExtender>
                                                    <asp:RequiredFieldValidator ControlToValidate="txtFrom" ID="RequiredFieldValidator1"
                                                        runat="server" ErrorMessage="Select Date Range" ValidationGroup="Report"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <div class="input-icon right">
                                                    <%--               <i class="fa fa-calendar"></i>--%>

                                                    <asp:TextBox placeholder="To Date" class="form-control input-sm" ID="txtTo" runat="server"></asp:TextBox>
                                                    <asp:CalendarExtender ID="calTo" runat="server" TargetControlID="txtTo" Format="dd/MM/yyyy">
                                                    </asp:CalendarExtender>
                                                    <asp:RequiredFieldValidator ControlToValidate="txtTo" ID="RequiredFieldValidator2"
                                                        runat="server" ErrorMessage="Select Date Range" ValidationGroup="Report"></asp:RequiredFieldValidator>


                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <div class="col-lg-3">
                                    <div class="form-actions text-right pal">
                                        <asp:Button ValidationGroup="Report" ID="btnPriview" CssClass="btn btn-primary" runat="server"
                                            Text="Priview" OnClick="btnPriview_Click" />



                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12">

                                    <asp:Xml ID="Xml1" runat="server" Visible="false"></asp:Xml>
                                    <asp:Literal ID="ltlTutorial" runat="server" Visible="false"></asp:Literal>
                                </div>
                            </div>
                        </div>



                    </div>

                </div>

            </div>
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPH_SideBar" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPH_Footer" runat="Server">
</asp:Content>

