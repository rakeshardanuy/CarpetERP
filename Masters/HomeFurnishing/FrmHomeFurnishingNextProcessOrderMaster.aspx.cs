﻿using System;using CarpetERP.Core.DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class Masters_HomeFurnishing_FrmHomeFurnishingNextProcessOrderMaster : System.Web.UI.Page
{
    static int hnEmpId = 0;
    static int varrecDetailID = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }

        if (!IsPostBack)
        {
            hnEmployeeType.Value = "0";
            hnEmpWagescalculation.Value = "";
            string str = @"Select Distinct CI.CompanyId, CI.CompanyName 
                        From Companyinfo CI(Nolock)
                        JOIN Company_Authentication CA(Nolock) ON CA.CompanyId = CI.CompanyId And CA.UserId = " + Session["varuserId"] + @" 
                        Where CI.MasterCompanyId = " + Session["varCompanyId"] + @" Order By CompanyName 
                        Select ID, BranchName 
                        From BRANCHMASTER BM(nolock) 
                        JOIN BranchUser BU(nolock) ON BU.BranchID = BM.ID And BU.UserID = " + Session["varuserId"] + @" 
                        Where BM.CompanyID = " + Session["CurrentWorkingCompanyID"] + " And BM.MasterCompanyID = " + Session["varCompanyId"] + @" 
                        Select Distinct PNM.PROCESS_NAME_ID, PNM.PROCESS_NAME  
                        From HomeFurnishingOrderMaster a(Nolock)
                        JOIN PROCESS_NAME_MASTER PNM(Nolock) ON PNM.PROCESS_NAME_ID = a.PROCESSID 
                        Where a.CompanyID = " + Session["CurrentWorkingCompanyID"] + " And a.MasterCompanyID = " + Session["varCompanyId"] + @" 
                        Order By PNM.PROCESS_NAME 
                        Select UnitId, UnitName From Unit(Nolock) Where Unitid in(1, 2, 6)";

            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

            UtilityModule.ConditionalComboFillWithDS(ref DDcompany, ds, 0, false, "");

            if (DDcompany.Items.Count > 0)
            {
                DDcompany.SelectedValue = Session["CurrentWorkingCompanyID"].ToString();
                DDcompany.Enabled = false;
            }
            UtilityModule.ConditionalComboFillWithDS(ref DDBranchName, ds, 1, false, "");
            DDBranchName.Enabled = false;
            if (DDBranchName.Items.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('Branch not define for this user!');", true);
                return;
            }

            UtilityModule.ConditionalComboFillWithDS(ref DDFromProcessName, ds, 2, true, "--Plz Select--");

            UtilityModule.ConditionalComboFillWithDS(ref ddunit, ds, 3, true, "--Plz Select--");

            txtissuedate.Text = System.DateTime.Now.ToString("dd-MMM-yyyy");
            txttargetdate.Text = System.DateTime.Now.ToString("dd-MMM-yyyy");

            hnissueorderid.Value = "0";

            if (ddunit.Items.FindByValue(variable.VarDefaultProductionunit) != null)
            {
                ddunit.SelectedValue = variable.VarDefaultProductionunit;
            }
        }
    }
    protected void DDFromProcessName_SelectedIndexChanged(object sender, EventArgs e)
    {
        string str = @"Select Distinct PNM.PROCESS_NAME_ID, PNM.PROCESS_NAME 
                    From PROCESS_NAME_MASTER PNM(Nolock) 
                    Where PNM.AddProcessName = 1 And PNM.MasterCompanyID = " + Session["varCompanyId"] + " And PNM.process_Name_ID <> " + DDFromProcessName.SelectedValue + @" 
                    Order By PNM.PROCESS_NAME 
                    Select Distinct b.ProcessRecId, b.ChallanNo 
                    From HomeFurnishingStockNo a(Nolock) 
                    JOIN HomeFurnishing_Stock_Detail HSD(Nolock) ON HSD.StockNo = a.StockNo And HSD.ToProcessID = a.CurrentProStatus 
                    JOIN HomeFurnishingReceiveMaster b(Nolock) ON b.CompanyID = HSD.CompanyID And b.ProcessRecId = HSD.Process_Rec_ID And b.ProcessID = HSD.ToProcessID 
                    Where a.Pack = 0 And a.IssRecStatus = 0  And (a.PRMID = 0 or a.PRMID is null) And a.CompanyID = " + DDcompany.SelectedValue + @" And 
                    (a.CurrentProStatus = " + DDFromProcessName.SelectedValue + " or a.CurrentProStatus is not null)   Order By b.ProcessRecId";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        UtilityModule.ConditionalComboFillWithDS(ref DDToProcess, ds, 0, true, "--Plz Select--");
        UtilityModule.ConditionalComboFillWithDS(ref DDChallanNo, ds, 1, true, "--Plz Select--");
    }
    protected void DDToProcess_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (chkEdit.Checked == true)

        {
            string str = @"Select PIM.ISSUEORDERID, PIM.CHALLANNO 
                From HomeFurnishingOrderMaster PIM(Nolock) 
                Where PIM.CompanyID = " + DDcompany.SelectedValue + @" And PIM.BranchID = " + DDBranchName.SelectedValue + @" And 
                    PIM.PROCESSID = " + DDToProcess.SelectedValue;
            if (chkcomplete.Checked == true)
            {
                str = str + @" And PIM.STATUS = 'Complete'";
            }
            else
            {
                str = str + @" And PIM.STATUS = 'Pending'";
            }
            str = str + @" Order By PIM.ISSUEORDERID Desc ";

            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
            UtilityModule.ConditionalComboFillWithDS(ref DDFolioNo, ds, 0, true, "--Plz Select--");
        }
    }
    protected void DDChallanNo_SelectedIndexChanged(object sender, EventArgs e)
    {

        string str = string.Empty, size = string.Empty ;
        if (Session["varCompanyId"].ToString() == "47")
        {
            str = @"select top(1) UnitId From HomeFurnishingReceiveMaster Where processrecid=" + DDChallanNo.SelectedValue;
            DataSet dsORDER = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

            if (dsORDER.Tables[0].Rows.Count > 0)
            {
                //if (dsORDER.Tables[0].Rows[0]["orderunitid"].ToString() == "1")
                //{
                if (ddunit.Items.FindByValue(dsORDER.Tables[0].Rows[0]["unitid"].ToString()) != null)
                {
                    ddunit.SelectedValue = dsORDER.Tables[0].Rows[0]["unitid"].ToString();
                }
                // }
            }
            if (dsORDER.Tables[0].Rows[0]["unitid"].ToString() == "1")
            {
                size = "VF.SizeMtr";
            }
            else if (dsORDER.Tables[0].Rows[0]["unitid"].ToString() == "6")
            {
                size = "VF.SizeInch";


            }
            else
            {
                size = "VF.SizeFt";
            }
        }
        else
        {
            size = "VF.SizeFt";
        }
        ViewState["size"] = size;
        Fillstockno(size);
    }
    protected void Fillstockno(string size)
    {
        DGStockDetail.DataSource = null;
        DGStockDetail.DataBind();

        string str = @"Select PRM.ProcessRecId, PRD.ProcessRecDetailId, PRD.Order_FinishedID, PRD.OrderDetailDetail_FinishedID, 
            VF.ITEM_NAME + ' ' + VF.QualityName + ' ' + VF.DesignName + ' ' + VF.ColorName + ' ' + VF.ShapeName + ' ' + "+size+@" ItemDescription, 
            PRD.Qty orderedqty
            From HomeFurnishingReceiveMaster PRM(Nolock) 
            JOIN HomeFurnishingReceiveDetail PRD(Nolock) ON PRD.ProcessRecId = PRM.ProcessRecId 
            JOIN V_FinishedItemDetail VF(Nolock) ON VF.ITEM_FINISHED_ID = PRD.OrderDetailDetail_FinishedID 
            Where PRM.CompanyId = " + DDcompany.SelectedValue + " And PRM.ProcessRecId = " + DDChallanNo.SelectedValue;
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        DG.DataSource = ds.Tables[0];
        DG.DataBind();
    }

    protected void txtfolionoedit_TextChanged(object sender, EventArgs e)
    {
        string str1 = @" Select PROCESSID, ISSUEORDERID, CHALLANNO, UNITID, CALTYPE, REPLACE(CONVERT(NVARCHAR(11), ASSIGNDATE,106),' ','-') ASSIGNDATE
            From HomeFurnishingOrderMaster PIM(Nolock) 
            Where CompanyID = " + DDcompany.SelectedValue + " And BranchID = " + DDBranchName.SelectedValue + " And CHALLANNO = '" + txtfolionoedit.Text + @"' 

            Select Top 1 FromProcessID From HomeFurnishing_Stock_Detail(Nolock)  Where IssueOrderID = " + txtfolionoedit.Text + "";

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str1);
        if (ds.Tables[0].Rows.Count > 0)
        {
            DDFromProcessName.SelectedValue = ds.Tables[1].Rows[0]["FromProcessID"].ToString();
            DDFromProcessName_SelectedIndexChanged(sender, new EventArgs());
            DDToProcess.SelectedValue = ds.Tables[0].Rows[0]["PROCESSID"].ToString();
            DDToProcess_SelectedIndexChanged(sender, new EventArgs());
            DDFolioNo.SelectedValue = ds.Tables[0].Rows[0]["ISSUEORDERID"].ToString();
            DDFolioNo_SelectedIndexChanged(sender, new EventArgs());
            ddunit.SelectedValue = ds.Tables[0].Rows[0]["UNITID"].ToString();
            DDcaltype.SelectedValue = ds.Tables[0].Rows[0]["CALTYPE"].ToString();
            txtissuedate.Text = ds.Tables[0].Rows[0]["ASSIGNDATE"].ToString();
        }
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        listWeaverName.Items.Remove(listWeaverName.SelectedItem);
        if (listWeaverName.Items.Count == 0)
        {
            hnEmpWagescalculation.Value = "";
        }
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        string StrEmpid = "";
        for (int i = 0; i < listWeaverName.Items.Count; i++)
        {
            if (StrEmpid == "")
            {
                StrEmpid = listWeaverName.Items[i].Value;
            }
            else
            {
                StrEmpid = StrEmpid + "," + listWeaverName.Items[i].Value;
            }
        }
        //Check Employee Entry
        if (StrEmpid == "")
        {
            lblmessage.Text = "Plz Enter Weaver ID No...";
            return;
        }
        string VarTStockNo = "";
        for (int i = 0; i < DGStockDetail.Rows.Count; i++)
        {
            CheckBox Chkboxitem = (CheckBox)DGStockDetail.Rows[i].FindControl("Chkboxitem");
            if (Chkboxitem.Checked == true)
            {
                if (VarTStockNo == "")
                {
                    VarTStockNo = ((Label)DGStockDetail.Rows[i].FindControl("lbltstockno")).Text + "~";
                }
                else
                {
                    VarTStockNo = VarTStockNo + ((Label)DGStockDetail.Rows[i].FindControl("lbltstockno")).Text + "~";
                }
            }
        }
        if (VarTStockNo == "")
        {
            lblmessage.Text = "Plz select atleast one stock no...";
            return;
        }
        string DetailTable = "";

        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlCommand cmd = new SqlCommand("PRO_SAVEHOMEFURNISHINGNEXTPROCESSORDER", con, Tran);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 30000;

            cmd.Parameters.Add("@ISSUEORDERID", SqlDbType.Int);
            cmd.Parameters["@ISSUEORDERID"].Direction = ParameterDirection.InputOutput;
            cmd.Parameters["@ISSUEORDERID"].Value = hnissueorderid.Value;
            cmd.Parameters.AddWithValue("@CompanyID", DDcompany.SelectedValue);
            cmd.Parameters.AddWithValue("@BranchID", DDBranchName.SelectedValue);
            cmd.Parameters.AddWithValue("@FromProcessID", DDFromProcessName.SelectedValue);
            cmd.Parameters.AddWithValue("@ToProcessID", DDToProcess.SelectedValue);
            cmd.Parameters.AddWithValue("@EMPIDS", StrEmpid);
            cmd.Parameters.AddWithValue("@ProcessRecID", DDChallanNo.SelectedValue);
            cmd.Parameters.AddWithValue("@UNITID", ddunit.SelectedValue);
            cmd.Parameters.AddWithValue("@CALTYPE", DDcaltype.SelectedValue);

            cmd.Parameters.AddWithValue("@ISSUEDATE", txtissuedate.Text);
            cmd.Parameters.AddWithValue("@TARGETDATE", txttargetdate.Text);
            cmd.Parameters.AddWithValue("@REMARKS", TxtRemarks.Text.Trim());
            cmd.Parameters.AddWithValue("@INSTRUCTION", TxtInstructions.Text.Trim());
            cmd.Parameters.AddWithValue("@FLAGFIXORWEIGHT", 1);
            cmd.Parameters.AddWithValue("@EXPORTSIZEFLAG", 0);
            cmd.Parameters.AddWithValue("@EWAYBILLNO", "");
            cmd.Parameters.Add("@ChallanNo", SqlDbType.VarChar, 100);
            cmd.Parameters["@ChallanNo"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@MSG", SqlDbType.VarChar, 100);
            cmd.Parameters["@MSG"].Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@USERID", Session["varuserid"]);
            cmd.Parameters.AddWithValue("@MASTERCOMPANYID", Session["varcompanyid"]);
            cmd.Parameters.AddWithValue("@StockNo", VarTStockNo);

            cmd.ExecuteNonQuery();
            if (cmd.Parameters["@msg"].Value.ToString() != "") //IF DATA NOT SAVED
            {
                lblmessage.Text = cmd.Parameters["@msg"].Value.ToString();
                Tran.Rollback();
            }
            else
            {
                lblmessage.Text = "Data Saved Successfully.";
                Tran.Commit();
                DDcaltype.Enabled = false;
                txtfoliono.Text = cmd.Parameters["@ISSUEORDERID"].Value.ToString(); //param[5].Value.ToString();
                hnissueorderid.Value = cmd.Parameters["@issueorderid"].Value.ToString();// param[0].Value.ToString();
                FillGrid();
                string size = string.Empty;
                if (!string.IsNullOrEmpty(ViewState["size"].ToString()))
                {
                    size = ViewState["size"].ToString();

                }
                else { size="vf.sizeft"; }
                Fillstockno(size);
                Refreshcontrol();
                disablecontrols();
            }
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblmessage.Text = ex.Message;
        }
        finally
        {
            con.Dispose();
            con.Close();
        }
    }
    protected void Refreshcontrol()
    {

    }
    protected void FillGrid()
    {
        TxtRemarks.Text = "";
        TxtInstructions.Text = "";
        string str = @"Select a.IssueOrderId, b.IssueDetailId, 
                VF.CATEGORY_NAME + ' ' + VF.ITEM_NAME + ' ' + VF.QUALITYNAME + ' ' + VF.DESIGNNAME + ' ' + VF.COLORNAME + ' ' + VF.SHADECOLORNAME + ' ' + VF.SHAPENAME + ' ' + 
                                Case When a.UNITID = 1 Then VF.SizeMtr Else Case When a.UNITID = 6 Then VF.SizeInch Else VF.SizeFt End End ItemDescription, 
                b.Width, b.Length, b.Qty, b.Rate, b.Comm, b.Area, b.Amount, REPLACE(CONVERT(NVARCHAR(11), a.assigndate, 106), ' ', '-') assigndate, 
                REPLACE(CONVERT(NVARCHAR(11), b.Reqbydate, 106), ' ', '-') Reqbydate, a.ChallanNo, a.unitid, a.caltype, a.Remarks, a.instruction, 
                (Select HSH.TStockNo + ', ' 
	                From HomeFurnishing_Stock_Detail HSD(nolock) 
	                JOIN HomeFurnishingStockNo HSH(nolock) ON HSH.StockNo = HSD.StockNo 
	                Where HSD.IssueOrderID = a.ISSUEORDERID And HSD.IssueDetailID = b.IssueDetailId And HSD.ToProcessID = a.PROCESSID For xml Path('')) TStockNo 
                From HomeFurnishingOrderMaster a(Nolock) 
                JOIN HomeFurnishingOrderDetail b(Nolock) ON b.IssueOrderID = a.IssueOrderID 
                JOIN V_FinishedItemDetail VF(Nolock) ON VF.ITEM_FINISHED_ID = b.OrderDetailDetail_FinishedID 
                Where a.ISSUEORDERID = " + hnissueorderid.Value + " And a.MasterCompanyId = " + Session["varCompanyId"] + " Order By b.IssueDetailId Desc";
        //Employeedetail
        str = str + @" Select Distinct EI.Empid, EI.EmpCode + '-' + EI.EmpName EmpName, activestatus 
                    From Employee_HomeFurnishingOrderMaster EMP(Nolock)
                    Join EmpInfo EI(Nolock) ON EI.EmpId = EMP.Empid 
                    Where Emp.ProcessId = " + DDToProcess.SelectedValue + " And Emp.IssueOrderId = " + hnissueorderid.Value;

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        DGOrderdetail.DataSource = ds.Tables[0];
        DGOrderdetail.DataBind();
        if (chkEdit.Checked == true)
        {
            //Date
            if (ds.Tables[0].Rows.Count > 0)
            {
                txtissuedate.Text = ds.Tables[0].Rows[0]["assigndate"].ToString();
                txttargetdate.Text = ds.Tables[0].Rows[0]["Reqbydate"].ToString();
                //txtfoliono.Text = ds.Tables[0].Rows[0]["issueorderid"].ToString();
                txtfoliono.Text = ds.Tables[0].Rows[0]["ChallanNo"].ToString();
                if (ddunit.Items.FindByValue(ds.Tables[0].Rows[0]["unitid"].ToString()) != null)
                {
                    ddunit.SelectedValue = ds.Tables[0].Rows[0]["unitid"].ToString();
                }
                DDcaltype.SelectedValue = ds.Tables[0].Rows[0]["caltype"].ToString();
                TxtRemarks.Text = ds.Tables[0].Rows[0]["Remarks"].ToString();
                TxtInstructions.Text = ds.Tables[0].Rows[0]["instruction"].ToString();
            }
            //Employee
            if (ds.Tables[1].Rows.Count > 0)
            {
                listWeaverName.Items.Clear();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    listWeaverName.Items.Add(new ListItem(ds.Tables[1].Rows[i]["Empname"].ToString(), ds.Tables[1].Rows[i]["Empid"].ToString()));
                    if (ds.Tables[1].Rows[i]["activestatus"].ToString() == "0")
                    {
                        listWeaverName.Items[i].Attributes.Add("style", "background-color:red;");
                    }
                    else
                    {
                        listWeaverName.Items[i].Attributes.Add("style", "background-color:white;");
                    }
                }
            }
        }
    }
    protected void btnPreview_Click(object sender, EventArgs e)
    {
        Report();
    }
    private void Report()
    {
        SqlParameter[] array = new SqlParameter[4];
        array[0] = new SqlParameter("@IssueOrderId", hnissueorderid.Value);
        array[1] = new SqlParameter("@ProcessId", DDToProcess.SelectedValue);
        array[2] = new SqlParameter("@MasterCompanyId", Session["varcompanyId"]);
        array[3] = new SqlParameter("@ReportType", 1);

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_ForProductionOrder", array);

        if (ds.Tables[0].Rows.Count > 0)
        {

            switch (Session["varcompanyid"].ToString())
            {
                case "47":
                    Session["rptFileName"] = "~\\Reports\\RptProductionOrderLoomWiseStockagni.rpt";
                    break;
                default:
                    Session["rptFileName"] = "~\\Reports\\RptProductionOrderLoomWiseStockChampo.rpt";
                    break;
            }
            Session["GetDataset"] = ds;
            Session["dsFileName"] = "~\\ReportSchema\\RptProductionOrderLoomWise.xsd";

            StringBuilder stb = new StringBuilder();
            stb.Append("<script>");
            stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true);
        }
    }
    protected void chkEdit_CheckedChanged(object sender, EventArgs e)
    {
        EditSelectedChange();

        if (Session["usertype"].ToString() != "1" && variable.VarOnlyPreviewButtonShowOnAllEditForm == "1")
        {
            DGOrderdetail.Columns[10].Visible = false;
            DGOrderdetail.Columns[11].Visible = false;
        }
    }
    protected void EditSelectedChange()
    {
        txtfolionoedit.Text = "";
        ddunit.Enabled = true;
        enablecontrols();
        if (chkEdit.Checked == true)
        {
            if (Session["varcompanyNo"].ToString() == "47")
            {
                TDupdateemp.Visible = true;
                TDactiveemployee.Visible = true;
            }
            TDFolioNo.Visible = true;
            TDFolioNotext.Visible = true;
            hnissueorderid.Value = "0";
            ddunit.Enabled = false;
            BtnUpdateConsumption.Visible = true;
        }
        else
        {
            btnsave.Visible = true;
            TDFolioNotext.Visible = false;
            TDFolioNo.Visible = false;
            hnissueorderid.Value = "0";
            TDactiveemployee.Visible = false;
            BtnUpdateConsumption.Visible = false;
        }
        DDFolioNo.Items.Clear();
        listWeaverName.Items.Clear();
        txtfoliono.Text = "";
        DGOrderdetail.DataSource = null;
        DGOrderdetail.DataBind();
    }
    protected void DDFolioNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        hnissueorderid.Value = DDFolioNo.SelectedValue;
        FillGrid();
    }
    protected void DDemployee_SelectedIndexChanged(object sender, EventArgs e)
    {
        string str = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        try
        {
            if (DDemployee.SelectedIndex > 0)
            {
                str = @"select EMp.Empid,Pm.IssueOrderId  from dbo.HomeFurnishingOrderMaster PM inner join dbo.HomeFurnishingOrderDetail PD
                        on PM.IssueOrderId=Pd.IssueOrderId  and Pd.PQty>0
                        inner join  dbo.Employee_HomeFurnishingOrderMaster EMP on EMP.IssueOrderId=PM.IssueOrderId and EMP.ProcessId = " + DDToProcess.SelectedValue + @"
                        inner join EmpInfo EI on Ei.EmpId=EMP.Empid
                        And EI.empid=" + DDemployee.SelectedValue;


                DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "AlertEmp", "alert('Folio -" + ds.Tables[0].Rows[0]["IssueOrderId"] + " Already pending at this ID No..');", true);
                    return;
                }

                //ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "select Empid,Empname from empinfo Where Empcode='" + txtWeaverIdNo.Text + "'");

                if (listWeaverName.Items.FindByValue(DDemployee.SelectedValue) == null)
                {

                    listWeaverName.Items.Add(new ListItem(DDemployee.SelectedItem.Text, DDemployee.SelectedValue));
                }

                //txtWeaverIdNo.Text = "";


                ds.Dispose();
            }
            // txtWeaverIdNo.Focus();

        }
        catch (Exception ex)
        {
            lblmessage.Visible = true;
            lblmessage.Text = ex.Message;
        }
        finally
        {
            con.Dispose();
            con.Close();
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (txtgetvalue.Text != "")
        {
            FillWeaver();
        }
    }
    protected void FillWeaver()
    {
        string str = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        try
        {
            if (txtWeaverIdNo.Text != "")
            {

                DataSet ds = null;

                if (Session["varCompanyId"].ToString() == "21")
                {
                    str = @"Select EI.Empid, EI.Empcode + '-' + EI.Empname EmpName, IsNull(EI.EmployeeType, 0) Emptype, 1 Caltype, 
                            IsNull(EID.Wagescalculation, 0) Wagescalculation 
                            From EmpInfo EI(Nolock)
                            LEFT JOIN HR_EMPLOYEEINFORMATION EID(Nolock) ON EID.EMPID = EI.EMPID 
                            Where EI.Blacklist = 0 And EI.EmpID = " + txtgetvalue.Text;
                }
                else
                {
                    str = @"Select EI.Empid, EI.Empcode + '-' + EI.Empname EmpName, IsNull(EI.EmployeeType, 0) Emptype, Case When EI.Employeetype = 1 Then 0 Else 1 End Caltype, 
                            IsNull(EID.Wagescalculation, 0) Wagescalculation 
                            From EmpInfo EI(Nolock)
                            LEFT JOIN HR_EMPLOYEEINFORMATION EID(Nolock) ON EID.EMPID = EI.EMPID 
                            Where EI.Blacklist = 0 And EI.EmpID = " + txtgetvalue.Text;
                }

                ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (hnEmployeeType.Value == "0")
                    {
                        hnEmployeeType.Value = ds.Tables[0].Rows[0]["Emptype"].ToString();
                    }
                    else if (hnEmployeeType.Value != ds.Tables[0].Rows[0]["Emptype"].ToString())
                    {
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Employee", "alert('Please select same location employee');", true);
                        return;
                    }
                    if ((Session["varCompanyId"].ToString() == "28" || Session["varCompanyId"].ToString() == "16") && ds.Tables[0].Rows[0]["emptype"].ToString() == "0")
                    {
                        SqlParameter[] param = new SqlParameter[4];
                        param[0] = new SqlParameter("@CardNo", txtWeaverIdNoscan.Text);
                        param[1] = new SqlParameter("@UserID", Session["varuserid"]);
                        param[2] = new SqlParameter("@MasterCompanyID", Session["varcompanyId"]);
                        param[3] = new SqlParameter("@Msg", SqlDbType.VarChar, 100);
                        param[3].Direction = ParameterDirection.Output;
                        //*************
                        DataSet dsnew = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_GetEmployeeIsPresentOrNot", param);

                        if (dsnew.Tables[0].Rows.Count == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Employee", "alert('Employee is absent so please process attendance');", true);
                            if (Session["varCompanyId"].ToString() == "28" && Session["varSubCompanyId"].ToString() == "281")
                            {
                                txtWeaverIdNoscan.Text = "";
                                txtWeaverIdNoscan.Focus();
                                return;
                            }
                        }
                    }
                    //***********CHECK LOCATION
                    Boolean addflag = true;

                    if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyNo"].ToString() == "28")
                    {
                        if (hnEmpWagescalculation.Value == "")
                        {
                            hnEmpWagescalculation.Value = ds.Tables[0].Rows[0]["Wagescalculation"].ToString();
                        }
                        else
                        {
                            if (hnEmpWagescalculation.Value.ToString() != ds.Tables[0].Rows[0]["Wagescalculation"].ToString())
                            {
                                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "caloutside", "alert('Employee Wages Calculation Should be same in BIO DATA ENTRY.');", true);
                                addflag = false;
                            }
                        }
                    }

                    //*********END
                    if (addflag == true)
                    {
                        if (listWeaverName.Items.FindByValue(ds.Tables[0].Rows[0]["Empid"].ToString()) == null)
                        {
                            listWeaverName.Items.Add(new ListItem(ds.Tables[0].Rows[0]["Empname"].ToString(), ds.Tables[0].Rows[0]["Empid"].ToString()));

                            if (Session["VarCompanyNo"].ToString() == "27" && hnEmployeeType.Value == "1")
                            {
                                hnEmpId = Convert.ToInt32(ds.Tables[0].Rows[0]["Empid"].ToString());
                            }
                            else
                            {
                                hnEmpId = 0;
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Employee", "alert('No Weaver found at this ID No..');", true);
                }
                ds.Dispose();
                txtWeaverIdNo.Text = "";
            }

            txtWeaverIdNo.Focus();

        }
        catch (Exception ex)
        {
            lblmessage.Visible = true;
            lblmessage.Text = ex.Message;
        }
        finally
        {
            con.Dispose();
            con.Close();
        }
    }
    protected void FillWeaverWithBarcodescan()
    {
        string str = "";
        try
        {
            if (txtWeaverIdNoscan.Text != "")
            {
                DataSet ds = null;

                str = @"Select EI.Empid, EI.Empcode + '-' + EI.Empname EmpName, IsNull(EI.EmployeeType, 0) Emptype, 
                        Case When EI.Employeetype = 1 Then 0 Else 1 End Caltype, IsNull(EID.Wagescalculation, 0) Wagescalculation 
                        From EmpInfo EI(Nolock)
                        LEFT JOIN HR_EMPLOYEEINFORMATION EID(Nolock) ON EID.EMPID = EI.EMPID 
                        Where EI.Blacklist = 0 And EI.EmpCode = '" + txtWeaverIdNoscan.Text + "'";

                ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (hnEmployeeType.Value == "0")
                    {
                        hnEmployeeType.Value = ds.Tables[0].Rows[0]["Emptype"].ToString();
                    }
                    else if (hnEmployeeType.Value != ds.Tables[0].Rows[0]["Emptype"].ToString())
                    {
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Employee", "alert('Please select same location employee');", true);
                        return;
                    }

                    if ((Session["varCompanyId"].ToString() == "28" || Session["varCompanyId"].ToString() == "16") && ds.Tables[0].Rows[0]["emptype"].ToString() == "0")
                    {
                        SqlParameter[] param = new SqlParameter[4];
                        param[0] = new SqlParameter("@CardNo", txtWeaverIdNoscan.Text);
                        param[1] = new SqlParameter("@UserID", Session["varuserid"]);
                        param[2] = new SqlParameter("@MasterCompanyID", Session["varcompanyId"]);
                        param[3] = new SqlParameter("@Msg", SqlDbType.VarChar, 100);
                        param[3].Direction = ParameterDirection.Output;
                        //*************
                        DataSet dsnew = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_GetEmployeeIsPresentOrNot", param);

                        if (dsnew.Tables[0].Rows.Count == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Employee", "alert('Employee is absent so please process attendance');", true);
                            if (Session["varCompanyId"].ToString() == "28" && Session["varSubCompanyId"].ToString() == "281")
                            {
                                txtWeaverIdNoscan.Text = "";
                                txtWeaverIdNoscan.Focus();
                                return;
                            }
                        }
                    }
                    //***********CHECK LOCATION
                    Boolean addflag = true;

                    if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyNo"].ToString() == "28")
                    {
                        if (hnEmpWagescalculation.Value == "")
                        {
                            hnEmpWagescalculation.Value = ds.Tables[0].Rows[0]["Wagescalculation"].ToString();
                        }
                        else
                        {
                            if (hnEmpWagescalculation.Value.ToString() != ds.Tables[0].Rows[0]["Wagescalculation"].ToString())
                            {
                                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "caloutside", "alert('Employee Wages Calculation Should be same in BIO DATA ENTRY.');", true);
                                addflag = false;
                            }
                        }
                    }
                    //*********END
                    if (addflag == true)
                    {
                        if (listWeaverName.Items.FindByValue(ds.Tables[0].Rows[0]["Empid"].ToString()) == null)
                        {
                            listWeaverName.Items.Add(new ListItem(ds.Tables[0].Rows[0]["Empname"].ToString(), ds.Tables[0].Rows[0]["Empid"].ToString()));
                        }
                    }
                    txtWeaverIdNoscan.Text = "";
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "Employee", "alert('No Weaver found at this ID No..');", true);
                    txtWeaverIdNoscan.Text = "";
                }
                ds.Dispose();
            }
            txtWeaverIdNoscan.Focus();
        }
        catch (Exception ex)
        {
            lblmessage.Visible = true;
            lblmessage.Text = ex.Message;
        }
    }

    protected void btnemployeesave_Click(object sender, EventArgs e)
    {
        lblpopupmsg.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            DataTable dtrecord = new DataTable();
            dtrecord.Columns.Add("empid", typeof(int));
            dtrecord.Columns.Add("activestatus", typeof(int));
            dtrecord.Columns.Add("processid", typeof(int));
            dtrecord.Columns.Add("issueorderid", typeof(int));

            for (int i = 0; i < GVDetail.Rows.Count; i++)
            {
                Label lblempid = ((Label)GVDetail.Rows[i].FindControl("lblempid"));
                Label lblactivestatus = ((Label)GVDetail.Rows[i].FindControl("lblactivestatus"));
                CheckBox Chkboxitem = ((CheckBox)GVDetail.Rows[i].FindControl("Chkboxitem"));
                DataRow dr = dtrecord.NewRow();
                dr["empid"] = lblempid.Text;
                dr["activestatus"] = Chkboxitem.Checked == true ? 0 : 1;
                if (Session["varcompanyId"].ToString() == "47")
                {
                    dr["Processid"] = DDToProcess.SelectedValue;
                }
                else
                {
                    dr["Processid"] = 1;
                
                }
                dr["issueorderid"] = DDFolioNo.SelectedValue;
                dtrecord.Rows.Add(dr);
            }
            SqlParameter[] param = new SqlParameter[6];
            param[0] = new SqlParameter("@issueorderid", DDFolioNo.SelectedValue);
            param[1] = new SqlParameter("@userid", Session["varuserid"]);
            param[2] = new SqlParameter("@dtrecord", dtrecord);
            param[3] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[3].Direction = ParameterDirection.Output;
            if (Session["varcompanyId"].ToString() == "47")
            {
                param[4] = new SqlParameter("@Processid", DDToProcess.SelectedValue);
            }
            else {
                param[4] = new SqlParameter("@Processid", 1);
            
            }
            param[5] = new SqlParameter("@Mastercompanyid", Session["varcompanyId"]);
            string sp = string.Empty;
            if (Session["varcompanyNo"].ToString() == "47")
            {
                sp = "Pro_UpdateFolioActiveStatusHome";
            }
            else
            {
                sp = "Pro_UpdateFolioActiveStatus";

            }
            //*************
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, sp, param);
            Tran.Commit();
            lblpopupmsg.Text = param[3].Value.ToString();
            FillEmployeeForDeactive();
            ModalpopupextDeactivefolio.Show();
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblpopupmsg.Text = ex.Message;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
    }
    protected void btnactiveemployee_Click(object sender, EventArgs e)
    {
        lblpopupmsg.Text = "";
        FillEmployeeForDeactive();
        ModalpopupextDeactivefolio.Show();
    }
    protected void FillEmployeeForDeactive()
    {
        string str = @"select Distinct EI.EmpName+'('+EI.EmpCode+')' as Employee,EMP.IssueOrderId,Emp.ActiveStatus,Ei.Empid 
                From Employee_HomeFurnishingOrderMaster EMP inner Join EmpInfo EI on Emp.Empid=Ei.EmpId
                   and EMP.ProcessId = " + DDToProcess.SelectedValue + " and EMP.IssueOrderId=" + DDFolioNo.SelectedValue;
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

        GVDetail.DataSource = ds.Tables[0];
        GVDetail.DataBind();
    }
    protected void GVDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox Chkboxitem = (CheckBox)e.Row.FindControl("Chkboxitem");
            Label lblactivestatus = (Label)e.Row.FindControl("lblactivestatus");
            if (lblactivestatus.Text == "1")
            {
                Chkboxitem.Checked = false;
            }
            else
            {
                Chkboxitem.Checked = true;
                e.Row.BackColor = System.Drawing.Color.Red;
            }
        }
    }
    protected void chkcomplete_CheckedChanged(object sender, EventArgs e)
    {
        DDFolioNo.SelectedIndex = -1;
        hnissueorderid.Value = "0";
        enablecontrols();
        DGOrderdetail.DataSource = null;
        DGOrderdetail.DataBind();
    }
    protected void lnkdelClick(object sender, EventArgs e)
    {
        lblmessage.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            LinkButton lnkdel = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)lnkdel.NamingContainer;

            Label lblissueorderid = (Label)gvr.FindControl("lblissueorderid");
            Label lblissuedetailid = (Label)gvr.FindControl("lblissuedetailid");
            SqlParameter[] param = new SqlParameter[6];
            param[0] = new SqlParameter("@issueorderid", lblissueorderid.Text);
            param[1] = new SqlParameter("@IssueDetailId", lblissuedetailid.Text);
            param[2] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[2].Direction = ParameterDirection.Output;
            param[3] = new SqlParameter("@MastercompanyId", Session["varcompanyNo"]);
            param[4] = new SqlParameter("@Userid", Session["varuserid"]);
            param[5] = new SqlParameter("@ProcessID", DDToProcess.SelectedValue);

            //********
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_DELETEHOMEFURNISHINGORDER", param);
            lblmessage.Text = param[2].Value.ToString();
            Tran.Commit();
            FillGrid();
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblmessage.Text = ex.Message;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
    }
    protected void ddunit_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    protected void disablecontrols()
    {
        txtWeaverIdNo.Enabled = false;
        txtWeaverIdNoscan.Enabled = false;
        btnDelete.Enabled = false;
    }
    protected void enablecontrols()
    {
        txtWeaverIdNo.Enabled = true;
        txtWeaverIdNoscan.Enabled = true;
        btnDelete.Enabled = true;
    }
    void Popup(bool isDisplay)
    {
        StringBuilder builder = new StringBuilder();
        if (isDisplay)
        {
            builder.Append("<script>");
            builder.Append("ShowPopup();</script>");
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowPopup", builder.ToString());
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "ShowPopup", builder.ToString(), false);
        }
        else
        {
            builder.Append("<script>");
            builder.Append("HidePopup();</script>");
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "HidePopup", builder.ToString(), false);
        }
    }
    protected void DGOrderdetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtqty = (TextBox)e.Row.FindControl("txtqty");
            if (txtqty != null)
            {
                if (DDcaltype.SelectedValue == "1" && variable.VarGENERATESTOCKNOONTAGGING == "1")
                {
                    txtqty.Enabled = false;
                }
            }
            if (Convert.ToInt16(Session["usertype"]) > 2)
            {
                DGOrderdetail.Columns[10].Visible = false;
                DGOrderdetail.Columns[11].Visible = false;
            }
        }
    }
    protected void btnweaveridscan_Click(object sender, EventArgs e)
    {
        //*********Check Folio Pending
        switch (Session["varcompanyid"].ToString())
        {
            case "16":
            case "28":
                string str = @"SELECT DISTINCT PIM.ISSUEORDERID, EMP.EMPID 
                    FROM HomeFurnishingOrderMaster PIM(Nolock) 
                    INNER JOIN Employee_HomeFurnishingOrderMaster EMP(Nolock) ON PIM.ISSUEORDERID=EMP.ISSUEORDERID AND EMP.PROCESSID = " + DDToProcess.SelectedValue + @" 
                    INNER JOIN EMPINFO EI(Nolock) ON EMP.EMPID=EI.EMPID 
                    WHERE PIM.STATUS = 'PENDING' AND EMP.ACTIVESTATUS = 1 AND EI.EMPCODE = '" + txtWeaverIdNoscan.Text + @"' 
                    Select UserType From NewUserDetail Where UserID = " + Session["varuserid"];

                DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string msg = "Folio already pending on these employees i.e " + ds.Tables[0].Rows[0]["issueorderid"].ToString();
                    ScriptManager.RegisterStartupScript(Page, GetType(), "altemp", "alert('" + msg + "');", true);
                    if (Convert.ToInt32(ds.Tables[1].Rows[0]["UserType"]) > 2)
                    {
                        txtWeaverIdNoscan.Text = "";
                        txtWeaverIdNoscan.Focus();
                        return;
                    }
                }
                break;
            default:
                break;
        }
        //********
        if (txtWeaverIdNoscan.Text != "")
        {
            FillWeaverWithBarcodescan();
        }
    }
    protected void DGOrderdetail_RowEditing(object sender, GridViewEditEventArgs e)
    {
        DGOrderdetail.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void DGOrderdetail_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        DGOrderdetail.EditIndex = -1;
        FillGrid();
    }
    protected void DGOrderdetail_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            Label lblissueorderid = (Label)DGOrderdetail.Rows[e.RowIndex].FindControl("lblissueorderid");
            Label lblissuedetailid = (Label)DGOrderdetail.Rows[e.RowIndex].FindControl("lblissuedetailid");
            TextBox txtrategrid = (TextBox)DGOrderdetail.Rows[e.RowIndex].FindControl("txtrategrid");
            TextBox txtcommrategrid = (TextBox)DGOrderdetail.Rows[e.RowIndex].FindControl("txtcommrategrid");

            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter("@issueorderid", lblissueorderid.Text);
            param[1] = new SqlParameter("@issuedetailid", lblissuedetailid.Text);
            param[2] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[2].Direction = ParameterDirection.Output;
            param[3] = new SqlParameter("@Userid", Session["varuserid"]);
            param[4] = new SqlParameter("@rate", txtrategrid.Text == "" ? "0" : txtrategrid.Text);
            param[5] = new SqlParameter("@commrate", txtcommrategrid.Text == "" ? "0" : txtcommrategrid.Text);
            param[6] = new SqlParameter("@ProcessID", DDToProcess.SelectedValue);

            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_UPDATE_HOMEFURNISHINGORDER", param);

            lblmessage.Text = param[2].Value.ToString();
            Tran.Commit();
            DGOrderdetail.EditIndex = -1;
            FillGrid();
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblmessage.Text = ex.Message;
        }
        finally
        {
            con.Dispose();
            con.Close();
        }
    }
    protected void BtnUpdateConsumption_Click(object sender, EventArgs e)
    {

        lblmessage.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlParameter[] param = new SqlParameter[5];
            param[0] = new SqlParameter("@issueorderid", DDFolioNo.SelectedValue);
            param[2] = new SqlParameter("@Msg", SqlDbType.VarChar, 100);
            param[2].Direction = ParameterDirection.Output;
            //********
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_UPDATEHOMEFURNISHINGPRODUCTIONORDERCONSUMPTION", param);
            if (param[2].Value.ToString() != "")
            {
                lblmessage.Text = param[2].Value.ToString();
                Tran.Rollback();
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn1", "alert('Consumption successfully updated')", true);
                Tran.Commit();
            }
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblmessage.Text = ex.Message;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
    }
    protected void DGStockDetail_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        DGStockDetail.PageIndex = e.NewPageIndex;
        string size = string.Empty;
        if (!string.IsNullOrEmpty(ViewState["size"].ToString()))
        {
            size = ViewState["size"].ToString();

        }
        else { size = "vf.sizeft"; }
        Fillstockno(size);
        //Fillstockno();
    }

    protected void DG_SelectedIndexChanged(object sender, EventArgs e)
    {
        int rowindex = DG.SelectedRow.RowIndex;

        Label lblProcessRecDetailId = (Label)DG.Rows[rowindex].FindControl("lblProcessRecDetailId");
        varrecDetailID = Convert.ToInt32(lblProcessRecDetailId.Text);
        Label lblOrder_FinishedID = (Label)DG.Rows[rowindex].FindControl("lblOrder_FinishedID");
        Label lblOrderDetailDetail_FinishedID = (Label)DG.Rows[rowindex].FindControl("lblOrderDetailDetail_FinishedID");

        string str = @"SELECT Distinct HFSN.TStockNo, HFSN.stockNo 
            FROM HomeFurnishingReceiveMaster PRM(Nolock) 
            JOIN HomeFurnishingReceiveDetail PRD(Nolock) ON PRD.ProcessRecId = PRM.ProcessRecId 
                And PRD.ProcessRecDetailId = " + lblProcessRecDetailId.Text + @"
                And PRD.Order_FinishedID = " + lblOrder_FinishedID.Text + " And PRD.OrderDetailDetail_FinishedID = " + lblOrderDetailDetail_FinishedID.Text + @" 
            JOIN HomeFurnishing_Stock_Detail PSD(Nolock) ON PSD.Process_Rec_ID = PRM.ProcessRecId 
	            And PSD.Process_Rec_Detail_ID = PRD.ProcessRecDetailId And PSD.ToProcessID = PRM.ProcessID 
            JOIN HomeFurnishingStockNo HFSN(Nolock) ON HFSN.StockNo = PSD.StockNo And HFSN.IssRecStatus = 0 And HFSN.CurrentProStatus = " + DDFromProcessName.SelectedValue + @" 
            Where PRM.CompanyId = " + DDcompany.SelectedValue + " And PRM.ProcessRecId = " + DDChallanNo.SelectedValue + @"
            Order By HFSN.stockNo";

//            SELECT HFSN.TSTOCKNO 
//            FROM HomeFurnishingReceiveMaster PRM(Nolock) 
//            JOIN HomeFurnishingStockNo HFSN(Nolock) ON HFSN.Process_Rec_ID = PRM.ProcessRecId AND HFSN.BAZARSTATUS = 1 AND HFSN.IssRecStatus = 0 AND 
//                    HFSN.CurrentProStatus = " + DDFromProcessName.SelectedValue + @" 
//            Where PRM.CompanyId = " + DDcompany.SelectedValue + " And PRM.ProcessRecId = " + DDChallanNo.SelectedValue + @"
//            Order By HFSN.stockNo";

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        DGStockDetail.DataSource = ds.Tables[0];
        DGStockDetail.DataBind();
        txttotalpcsgrid.Text = "0";

        if (ds.Tables[0].Rows.Count > 0)
        {
            Trsave.Visible = true;
            txttotalpcsgrid.Text = ds.Tables[0].Compute("count(Tstockno)", "").ToString();
        }
    }
    protected void TxtIssueQty_TextChanged(object sender, EventArgs e)
    {
        DGStockDetail.DataSource = null;
        DGStockDetail.DataBind();
        if (TxtIssueQty.Text != "" && DDChallanNo.SelectedIndex > 0)
        {
            DGStockDetail.PageSize = Convert.ToInt32(TxtIssueQty.Text);
            Fillstocknoqtywise(varrecDetailID);
        }
    }
    protected void Fillstocknoqtywise(int recDetailID)
    {
        string str = @"SELECT Distinct HFSN.TStockNo, HFSN.stockNo 
            FROM HomeFurnishingReceiveMaster PRM(Nolock) 
            JOIN HomeFurnishingReceiveDetail PRD(Nolock) ON PRD.ProcessRecId = PRM.ProcessRecId 
                And PRD.ProcessRecDetailId = " + recDetailID + @"
            JOIN HomeFurnishing_Stock_Detail PSD(Nolock) ON PSD.Process_Rec_ID = PRM.ProcessRecId 
	            And PSD.Process_Rec_Detail_ID = PRD.ProcessRecDetailId And PSD.ToProcessID = PRM.ProcessID 
            JOIN HomeFurnishingStockNo HFSN(Nolock) ON HFSN.StockNo = PSD.StockNo And HFSN.IssRecStatus = 0 And HFSN.CurrentProStatus = " + DDFromProcessName.SelectedValue + @" 
            Where PRM.CompanyId = " + DDcompany.SelectedValue + " And PRM.ProcessRecId = " + DDChallanNo.SelectedValue + @"
            Order By HFSN.stockNo";

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        DGStockDetail.DataSource = ds.Tables[0];
        DGStockDetail.DataBind();
        txttotalpcsgrid.Text = "0";
        Trsave.Visible = false;

        if (ds.Tables[0].Rows.Count > 0)
        {
            Trsave.Visible = true;
            txttotalpcsgrid.Text = ds.Tables[0].Compute("count(Tstockno)", "").ToString();
        }
    }
    protected void btnupdateemp_Click(object sender, EventArgs e)
    {
        lblmessage.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            //Employeedetail
            DataTable dtrecord = new DataTable();
            dtrecord.Columns.Add("processid", typeof(int));
            dtrecord.Columns.Add("issueorderid", typeof(int));
            dtrecord.Columns.Add("issuedetailid", typeof(int));
            dtrecord.Columns.Add("empid", typeof(int));
            for (int i = 0; i < listWeaverName.Items.Count; i++)
            {
                for (int j = 0; j < DGOrderdetail.Rows.Count; j++)
                {
                    Label lblissueorderid = ((Label)DGOrderdetail.Rows[j].FindControl("lblissueorderid"));
                    Label lblissuedetailid = ((Label)DGOrderdetail.Rows[j].FindControl("lblissuedetailid"));

                    DataRow dr = dtrecord.NewRow();
                    dr["processid"] = DDToProcess.SelectedValue;
                    dr["issueorderid"] = lblissueorderid.Text;
                    dr["issuedetailid"] = lblissuedetailid.Text;
                    dr["empid"] = listWeaverName.Items[i].Value;
                    dtrecord.Rows.Add(dr);
                }
            }
            //
            SqlParameter[] param = new SqlParameter[6];
            param[0] = new SqlParameter("@issueorderid", hnissueorderid.Value);
            param[1] = new SqlParameter("@processid", DDToProcess.SelectedValue);
            param[2] = new SqlParameter("@userid", Session["varuserid"]);
            param[3] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[3].Direction = ParameterDirection.Output;
            param[4] = new SqlParameter("@mastercompanyid", Session["varcompanyId"]);
            param[5] = new SqlParameter("@dtrecord", dtrecord);
            //*************
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "Pro_UpdateFolioEmployeehome", param);
            Tran.Commit();
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn1", "alert('" + param[3].Value.ToString() + "')", true);

        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblmessage.Text = ex.Message;
        }

        finally
        {
            con.Close();
            con.Dispose();
        }
        //********************
        #region
        //try
        //{
        //    if (DGOrderdetail.Rows.Count == 0)
        //    {
        //        Tran.Commit();
        //        return;
        //    }
        //    string str = "";
        //    //Delete And Update Existing record
        //    str = @"Delete from Employee_ProcessOrderNo Where IssueOrderId=" + hnissueorderid.Value + @" And ProcessId=1";

        //    SqlHelper.ExecuteNonQuery(Tran, CommandType.Text, str);
        //    //

        //    for (int i = 0; i < listWeaverName.Items.Count; i++)
        //    {
        //        for (int j = 0; j < DGOrderdetail.Rows.Count; j++)
        //        {
        //            str = "Insert into Employee_ProcessOrderNo (ProcessId,IssueOrderId,IssueDetailId,Empid)values(1," + ((Label)DGOrderdetail.Rows[j].FindControl("lblissueorderid")).Text + "," + ((Label)DGOrderdetail.Rows[j].FindControl("lblissuedetailid")).Text + "," + listWeaverName.Items[i].Value + ")";
        //            SqlHelper.ExecuteNonQuery(Tran, CommandType.Text, str);

        //        }
        //    }
        //    Tran.Commit();
        //    ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn1", "alert('Employee updated successfully...')", true);

        //}
        //catch (Exception ex)
        //{
        //    lblmessage.Text = ex.Message;
        //    Tran.Rollback();
        //}
        //finally
        //{
        //    con.Dispose();
        //    con.Close();
        //}
        #endregion
    }
}
