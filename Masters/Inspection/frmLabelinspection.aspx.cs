﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

public partial class Masters_Inspection_frmcartoninspection : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varcompanyid"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        if (!IsPostBack)
        {
            if (!Page.IsPostBack)
            {
                SetInitialRow();
            }
            string str = @"select CI.CompanyId,CI.CompanyName from CompanyInfo CI inner join Company_Authentication CA on Ci.CompanyId=CA.CompanyId
                    Where CI.MasterCompanyid=" + Session["varcompanyid"] + " and CA.UserId=" + Session["varuserid"] + @"  order by CompanyName 
                    Select ID, BranchName 
                    From BRANCHMASTER BM(nolock) 
                    JOIN BranchUser BU(nolock) ON BU.BranchID = BM.ID And BU.UserID = " + Session["varuserId"] + @" 
                    Where BM.CompanyID = " + Session["CurrentWorkingCompanyID"] + " And BM.MasterCompanyID = " + Session["varCompanyId"];

            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

            UtilityModule.ConditionalComboFillWithDS(ref DDcompanyName, ds, 0, true, "Plz Select--");

            if (DDcompanyName.Items.Count > 0)
            {
                DDcompanyName.SelectedValue = Session["CurrentWorkingCompanyID"].ToString();
                DDcompanyName.Enabled = false;
            }

            UtilityModule.ConditionalComboFillWithDS(ref DDBranchName, ds, 1, false, "");
            DDBranchName.Enabled = false;
            if (DDBranchName.Items.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('Branch not define for this user!');", true);
                return;
            }

            UtilityModule.ConditionalComboFill(ref DDCustomerCode, " Select customerid,customercode from customerinfo Where MasterCompanyId=" + Session["varCompanyId"] + " order by customercode", true, "--Select--");

            txtdate.Text = System.DateTime.Now.ToString("dd-MMM-yyyy");

            switch (Session["usertype"].ToString())
            {
                case "1":
                case "2":
                    btnApprove.Visible = true;
                    Changeapprovebuttoncolor(0);
                    break;
                default:
                    btnApprove.Visible = false;
                    break;
            }
        }
    }
    protected void DDCustomerCode_SelectedIndexChanged(object sender, EventArgs e)
    {
        CustomerCodeSelectedIndexChange();
    }
    private void CustomerCodeSelectedIndexChange()
    {
        if (chkedit.Checked == true)
        {
            string str3 = @"Select Distinct OM.OrderId,OM.CustomerOrderNo FROM LABELINSPECTIONMASTER RIM INNER JOIN LABELINSPECTIONDETAIL RID ON RIM.DOCID=RID.DOCID 
                        LEFT JOIN OrderMaster OM ON RIM.OrderId=OM.OrderId 
                        Where RIM.COMPANYID=" + DDcompanyName.SelectedValue + " And RIM.BranchID = " + DDBranchName.SelectedValue + "";
            if (DDCustomerCode.SelectedIndex>0)
            {
                str3 = str3 + " and RIM.CustomerId="+DDCustomerCode.SelectedValue+"";
            }            
            UtilityModule.ConditionalComboFill(ref DDCustomerOrderNo, str3, true, "--Select--");
        }
        else
        {
            string str = @"Select Distinct Orderid, CustomerOrderNo from OrderMaster Where CustomerID=" + DDCustomerCode.SelectedValue + " And CompanyId=" + DDcompanyName.SelectedValue + " and Status=0";

            str = str + "  Order by CustomerOrderNo";
            UtilityModule.ConditionalComboFill(ref DDCustomerOrderNo, str, true, "--Select--");
        }
    }
    protected void DDCustomerOrderNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        hndocid.Value = "0";
        if (chkedit.Checked == true)
        {
            fillDocno();
        }
    }
    protected void Changeapprovebuttoncolor(int approvestatus = 0)
    {
        switch (approvestatus)
        {
            case 1:
                btnApprove.BackColor = System.Drawing.Color.Green;
                break;
            default:
                btnApprove.BackColor = System.Drawing.Color.Red;
                break;
        }
    }
    private void SetInitialRow()
    {
        DataTable dt = new DataTable();
        DataRow dr = null;
        dt.Columns.Add(new DataColumn("Srno", typeof(string)));
        dt.Columns.Add(new DataColumn("Description", typeof(string)));
        dt.Columns.Add(new DataColumn("Totalqty", typeof(string)));
        dt.Columns.Add(new DataColumn("SampleSize", typeof(string)));
        dt.Columns.Add(new DataColumn("Gumming", typeof(string)));
        dt.Columns.Add(new DataColumn("Scanability", typeof(string)));
        dt.Columns.Add(new DataColumn("Color", typeof(string)));
        dt.Columns.Add(new DataColumn("Font", typeof(string)));
        dt.Columns.Add(new DataColumn("Printing", typeof(string)));
        dt.Columns.Add(new DataColumn("Bleeding", typeof(string)));
        dt.Columns.Add(new DataColumn("Cutting", typeof(string)));
        dt.Columns.Add(new DataColumn("Size", typeof(string)));
        dt.Columns.Add(new DataColumn("Found", typeof(string)));
        dt.Columns.Add(new DataColumn("Acceptance", typeof(string)));
        dt.Columns.Add(new DataColumn("Lotresult", typeof(string)));

        dr = dt.NewRow();
        dr["Srno"] = 1;
        dr["Description"] = string.Empty;
        dr["Totalqty"] = string.Empty;
        dr["Samplesize"] = string.Empty;
        dr["Gumming"] = string.Empty;
        dr["Scanability"] = string.Empty;
        dr["Color"] = string.Empty;
        dr["Font"] = string.Empty;
        dr["Printing"] = string.Empty;
        dr["Bleeding"] = string.Empty;
        dr["Cutting"] = string.Empty;
        dr["Size"] = string.Empty;
        dr["Found"] = string.Empty;
        dr["Acceptance"] = string.Empty;
        dr["Lotresult"] = "Pass";
        dt.Rows.Add(dr);
        //dr = dt.NewRow();

        //Store the DataTable in ViewState
        ViewState["CurrentTable"] = dt;

        Dgdetail.DataSource = dt;
        Dgdetail.DataBind();
    }
    private void AddNewRowToGrid()
    {
        int rowIndex = 0;

        if (ViewState["CurrentTable"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                {
                    //extract the TextBox values
                    TextBox txtsrno = (TextBox)Dgdetail.Rows[rowIndex].Cells[0].FindControl("txtsrno");
                    TextBox txtdescription = (TextBox)Dgdetail.Rows[rowIndex].Cells[1].FindControl("txtdescription");
                    TextBox txttotalqty = (TextBox)Dgdetail.Rows[rowIndex].Cells[2].FindControl("txttotalqty");
                    TextBox txtsamplesize = (TextBox)Dgdetail.Rows[rowIndex].Cells[3].FindControl("txtsamplesize");
                    TextBox txtgumming = (TextBox)Dgdetail.Rows[rowIndex].Cells[4].FindControl("txtgumming");
                    TextBox txtscanability = (TextBox)Dgdetail.Rows[rowIndex].Cells[5].FindControl("txtscanability");
                    TextBox txtcolor = (TextBox)Dgdetail.Rows[rowIndex].Cells[6].FindControl("txtcolor");
                    TextBox txtfont = (TextBox)Dgdetail.Rows[rowIndex].Cells[7].FindControl("txtfont");
                    TextBox txtprinting = (TextBox)Dgdetail.Rows[rowIndex].Cells[8].FindControl("txtprinting");
                    TextBox txtbleeding = (TextBox)Dgdetail.Rows[rowIndex].Cells[9].FindControl("txtbleeding");
                    TextBox txtcutting = (TextBox)Dgdetail.Rows[rowIndex].Cells[10].FindControl("txtcutting");
                    TextBox txtsize = (TextBox)Dgdetail.Rows[rowIndex].Cells[11].FindControl("txtsize");
                    TextBox txtfound = (TextBox)Dgdetail.Rows[rowIndex].Cells[12].FindControl("txtfound");
                    TextBox txtacceptance = (TextBox)Dgdetail.Rows[rowIndex].Cells[13].FindControl("txtacceptance");
                    DropDownList ddresult = (DropDownList)Dgdetail.Rows[rowIndex].Cells[14].FindControl("ddresult");
                    Label lbllotresult = (Label)Dgdetail.Rows[rowIndex].FindControl("lbllotresult");


                    drCurrentRow = dtCurrentTable.NewRow();
                    drCurrentRow["srno"] = i + 1;
                    drCurrentRow["Lotresult"] = "Pass";

                    dtCurrentTable.Rows[i - 1]["description"] = txtdescription.Text;
                    dtCurrentTable.Rows[i - 1]["totalqty"] = txttotalqty.Text;
                    dtCurrentTable.Rows[i - 1]["samplesize"] = txtsamplesize.Text;
                    dtCurrentTable.Rows[i - 1]["Gumming"] = txtgumming.Text;
                    dtCurrentTable.Rows[i - 1]["Scanability"] = txtscanability.Text;
                    dtCurrentTable.Rows[i - 1]["Color"] = txtcolor.Text;
                    dtCurrentTable.Rows[i - 1]["Font"] = txtfont.Text;
                    dtCurrentTable.Rows[i - 1]["Printing"] = txtprinting.Text;
                    dtCurrentTable.Rows[i - 1]["Bleeding"] = txtbleeding.Text;
                    dtCurrentTable.Rows[i - 1]["Cutting"] = txtcutting.Text;
                    dtCurrentTable.Rows[i - 1]["Size"] = txtsize.Text;
                    dtCurrentTable.Rows[i - 1]["Found"] = txtfound.Text;
                    dtCurrentTable.Rows[i - 1]["Acceptance"] = txtacceptance.Text;
                    dtCurrentTable.Rows[i - 1]["Lotresult"] = ddresult.SelectedItem.Text;


                    rowIndex++;
                }
                dtCurrentTable.Rows.Add(drCurrentRow);
                ViewState["CurrentTable"] = dtCurrentTable;

                Dgdetail.DataSource = dtCurrentTable;
                Dgdetail.DataBind();
            }
        }
        else
        {
            Response.Write("ViewState is null");
        }

        //Set Previous Data on Postbacks
        SetPreviousData();
    }
    private void SetPreviousData()
    {
        int rowIndex = 0;
        //StringCollection sc = new StringCollection();
        if (ViewState["CurrentTable"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TextBox txtsrno = (TextBox)Dgdetail.Rows[rowIndex].Cells[0].FindControl("txtsrno");
                    TextBox txtdescription = (TextBox)Dgdetail.Rows[rowIndex].Cells[1].FindControl("txtdescription");
                    TextBox txttotalqty = (TextBox)Dgdetail.Rows[rowIndex].Cells[2].FindControl("txttotalqty");
                    TextBox txtsamplesize = (TextBox)Dgdetail.Rows[rowIndex].Cells[3].FindControl("txtsamplesize");
                    TextBox txtgumming = (TextBox)Dgdetail.Rows[rowIndex].Cells[4].FindControl("txtgumming");
                    TextBox txtscanability = (TextBox)Dgdetail.Rows[rowIndex].Cells[5].FindControl("txtscanability");
                    TextBox txtcolor = (TextBox)Dgdetail.Rows[rowIndex].Cells[6].FindControl("txtcolor");
                    TextBox txtfont = (TextBox)Dgdetail.Rows[rowIndex].Cells[7].FindControl("txtfont");
                    TextBox txtprinting = (TextBox)Dgdetail.Rows[rowIndex].Cells[8].FindControl("txtprinting");
                    TextBox txtbleeding = (TextBox)Dgdetail.Rows[rowIndex].Cells[9].FindControl("txtbleeding");
                    TextBox txtcutting = (TextBox)Dgdetail.Rows[rowIndex].Cells[10].FindControl("txtcutting");
                    TextBox txtsize = (TextBox)Dgdetail.Rows[rowIndex].Cells[11].FindControl("txtsize");
                    TextBox txtfound = (TextBox)Dgdetail.Rows[rowIndex].Cells[12].FindControl("txtfound");
                    TextBox txtacceptance = (TextBox)Dgdetail.Rows[rowIndex].Cells[13].FindControl("txtacceptance");
                    DropDownList ddresult = (DropDownList)Dgdetail.Rows[rowIndex].Cells[14].FindControl("ddresult");

                    txtsrno.Text = dt.Rows[i]["srno"].ToString();
                    txtdescription.Text = dt.Rows[i]["Description"].ToString();
                    txttotalqty.Text = dt.Rows[i]["totalqty"].ToString();
                    txtsamplesize.Text = dt.Rows[i]["samplesize"].ToString();
                    txtgumming.Text = dt.Rows[i]["Gumming"].ToString();
                    txtscanability.Text = dt.Rows[i]["scanability"].ToString();
                    txtcolor.Text = dt.Rows[i]["color"].ToString();
                    txtfont.Text = dt.Rows[i]["font"].ToString();
                    txtprinting.Text = dt.Rows[i]["Printing"].ToString();
                    txtbleeding.Text = dt.Rows[i]["Bleeding"].ToString();
                    txtcutting.Text = dt.Rows[i]["Cutting"].ToString();
                    txtsize.Text = dt.Rows[i]["Size"].ToString();
                    txtfound.Text = dt.Rows[i]["found"].ToString();
                    txtacceptance.Text = dt.Rows[i]["acceptance"].ToString();
                    ddresult.SelectedItem.Text = dt.Rows[i]["lotresult"].ToString();

                    // sc.Add(box1.Text + "," + box2.Text + "," + box3.Text);
                    rowIndex++;
                }
                //InsertRecords(sc);
            }
        }
    }
    protected void ButtonAdd_Click(object sender, EventArgs e)
    {
        AddNewRowToGrid();
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        lblmsg.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlParameter[] param = new SqlParameter[14];
            param[0] = new SqlParameter("@Docid", SqlDbType.Int);
            param[0].Direction = ParameterDirection.InputOutput;
            param[0].Value = hndocid.Value;
            param[1] = new SqlParameter("@Companyid", DDcompanyName.SelectedValue);
            param[2] = new SqlParameter("@DocNo", SqlDbType.VarChar, 50);
            param[2].Value = txtdocno.Text;
            param[2].Direction = ParameterDirection.InputOutput;
            param[3] = new SqlParameter("@userid", Session["varuserid"]);
            param[4] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[4].Direction = ParameterDirection.Output;
            param[5] = new SqlParameter("@Comments", txtcomments.Text.Trim());
            param[6] = new SqlParameter("@ReportDate", txtdate.Text);
            param[7] = new SqlParameter("@standared", txtstandared.Text);
            param[8] = new SqlParameter("@suppliername", txtsuppliername.Text);
            param[9] = new SqlParameter("@challanNodate", txtchallannodate.Text);
            param[10] = new SqlParameter("@Size", txtsize.Text);
            param[11] = new SqlParameter("@BranchID", DDBranchName.SelectedValue);
            param[12] = new SqlParameter("@OrderID",DDCustomerOrderNo.SelectedValue);
            param[13] = new SqlParameter("@CustomerID",DDCustomerCode.SelectedValue);

            //*********
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "[PRO_SAVELABLEINSPECTION]", param);
            lblmsg.Text = param[4].Value.ToString();
            txtdocno.Text = param[2].Value.ToString();
            hndocid.Value = param[0].Value.ToString();

            // at the time of update delete all the data in tables
            string str1 = @"DELETE FROM LabelInspectionDetail WHERE DOCID=" + hndocid.Value;
            SqlHelper.ExecuteNonQuery(Tran, CommandType.Text, str1);

            insertrecordDetail(Tran);

            ScriptManager.RegisterStartupScript(Page, GetType(), "altsave", "alert('" + param[4].Value.ToString() + "')", true);

            Tran.Commit();
            //**********

        }
        catch (Exception ex)
        {
            lblmsg.Text = ex.Message;
            Tran.Rollback();
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
    }
    private void insertrecordDetail(SqlTransaction Tran)
    {

        string str = "";
        if (Dgdetail.Rows.Count != 0)
        {

            for (int i = 0; i < Dgdetail.Rows.Count; i++)
            {
                TextBox txtsrno = (TextBox)Dgdetail.Rows[i].Cells[0].FindControl("txtsrno");
                TextBox txtdescription = (TextBox)Dgdetail.Rows[i].Cells[1].FindControl("txtdescription");
                TextBox txttotalqty = (TextBox)Dgdetail.Rows[i].Cells[2].FindControl("txttotalqty");
                TextBox txtsamplesize = (TextBox)Dgdetail.Rows[i].Cells[3].FindControl("txtsamplesize");
                TextBox txtgumming = (TextBox)Dgdetail.Rows[i].Cells[4].FindControl("txtgumming");
                TextBox txtscanability = (TextBox)Dgdetail.Rows[i].Cells[5].FindControl("txtscanability");
                TextBox txtcolor = (TextBox)Dgdetail.Rows[i].Cells[6].FindControl("txtcolor");
                TextBox txtfont = (TextBox)Dgdetail.Rows[i].Cells[7].FindControl("txtfont");
                TextBox txtprinting = (TextBox)Dgdetail.Rows[i].Cells[8].FindControl("txtprinting");
                TextBox txtbleeding = (TextBox)Dgdetail.Rows[i].Cells[9].FindControl("txtbleeding");
                TextBox txtcutting = (TextBox)Dgdetail.Rows[i].Cells[10].FindControl("txtcutting");
                TextBox txtsizegrid = (TextBox)Dgdetail.Rows[i].Cells[11].FindControl("txtsize");
                TextBox txtfound = (TextBox)Dgdetail.Rows[i].Cells[12].FindControl("txtfound");
                TextBox txtacceptance = (TextBox)Dgdetail.Rows[i].Cells[13].FindControl("txtacceptance");
                DropDownList ddresult = (DropDownList)Dgdetail.Rows[i].Cells[14].FindControl("ddresult");

                if (txtsrno.Text != "")
                {
                    str = str + @"  Insert into LabelInspectionDetail(Docid,Srno,Description,totalqty,samplesize,Gumming,Scanability,Color,Font,Printing,Bleeding,Cutting,Size,Found,Acceptance,Lotresult)
                   values (" + hndocid.Value + ",'" + txtsrno.Text + "','" + txtdescription.Text.Replace("'", "''") + "','" + txttotalqty.Text.Replace("'", "''") + @"',
                          '" + txtsamplesize.Text.Replace("'", "''") + "','" + txtgumming.Text.Replace("'", "''") + "','" + txtscanability.Text.Replace("'", "''") + "','" + txtcolor.Text.Replace("'", "''") + "','" + txtfont.Text.Replace("'", "''") + @"'
                          ,'" + txtprinting.Text.Replace("'", "''") + "','" + txtbleeding.Text.Replace("'", "''") + "','" + txtcutting.Text.Replace("'", "''") + "','" + txtsizegrid.Text.Replace("'", "''") + "','" + txtfound.Text.Replace("'", "''") + "','" + txtacceptance.Text.Replace("'", "''") + "','" + ddresult.SelectedItem.Text.Replace("'", "''") + "')";
                }


            }
            if (str != "")
            {
                SqlHelper.ExecuteNonQuery(Tran, CommandType.Text, str);
            }
        }

    }
    protected void btnpreview_Click(object sender, EventArgs e)
    {
        lblmsg.Text = "";
        try
        {
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@Docid", hndocid.Value);

            //            string str = @"SELECT Ci.companyName,* FROM RAWYARNINSPECTIONMASTER RIM INNER JOIN RAWYARNINSPECTIONDETAIL RID ON RIM.DOCID=RID.DOCID 
            //                           inner join CompanyInfo ci on RIM.COMPANYID=ci.CompanyId Where RIM.Docid=" + hndocid.Value;

            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "[PRO_GETLABELINSPECTIONREPORT]", param);

            if (ds.Tables[0].Rows.Count > 0)
            {
                Session["rptFileName"] = "~\\Reports\\rptLabelinspection.rpt";
                Session["Getdataset"] = ds;
                Session["dsFileName"] = "~\\ReportSchema\\rptLabelinspection.xsd";
                StringBuilder stb = new StringBuilder();
                stb.Append("<script>");
                stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
            }
            else
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "alt1", "alert('No records found.')", true);
            }
        }
        catch (Exception ex)
        {
            lblmsg.Text = ex.Message;
        }
    }
    protected void btndelete_Click(object sender, EventArgs e)
    {
        lblmsg.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlParameter[] param = new SqlParameter[5];
            param[0] = new SqlParameter("@Docid", hndocid.Value);
            param[1] = new SqlParameter("@userid", Session["varuserid"]);
            param[2] = new SqlParameter("@Mastercompanyid", Session["varcompanyid"]);
            param[3] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "[PRO_DELETELABELINSPECTION]", param);
            Tran.Commit();
            if (param[3].Value.ToString() != "")
            {
                lblmsg.Text = param[3].Value.ToString();
            }
            else
            {
                lblmsg.Text = "DOC No. Deleted Successfully.";
                fillDocno();
            }

        }
        catch (Exception ex)
        {
            lblmsg.Text = ex.Message;
            Tran.Rollback();
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
    }
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        lblmsg.Text = "";
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@Docid", hndocid.Value);
            param[1] = new SqlParameter("@userid", Session["varuserid"]);
            param[2] = new SqlParameter("@mastercompanyid", Session["varcompanyid"]);
            param[3] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "[PRO_APPROVELABELINSPECTION]", param);
            Tran.Commit();
            lblmsg.Text = param[3].Value.ToString();
            ScriptManager.RegisterStartupScript(Page, GetType(), "altappv", "alert('" + param[3].Value.ToString() + "')", true);
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            lblmsg.Text = ex.Message;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
    }
    protected void chkedit_CheckedChanged(object sender, EventArgs e)
    {
        TDCustomerOrderNo.Visible = false;
        TDSupplierSearch.Visible = false;
        TDDocno.Visible = false;
        hndocid.Value = "0";
        DDDocNo.Items.Clear();
        refreshcontrol();
        if (chkedit.Checked == true)
        {
            TDDocno.Visible = true;
            TDSupplierSearch.Visible = true;
            TDCustomerOrderNo.Visible = true;
           
            fillOrderNo();
            fillDocno();
            
        }
    }
    private void refreshcontrol()
    {
        txtsuppliername.Text = "";
        txtdate.Text = System.DateTime.Now.ToString("dd-MMM-yyyy");
        txtchallannodate.Text = "";
        txtstandared.Text = "";
        txtsize.Text = "";
        SetInitialRow();
    }
    private void fillOrderNo()
    {
        string str2 = @"Select Distinct CI.customerid,CI.CustomerCode FROM LABELINSPECTIONMASTER RIM INNER JOIN LABELINSPECTIONDETAIL RID ON RIM.DOCID=RID.DOCID 
                        LEFT JOIN CustomerInfo CI ON RIM.CustomerId=CI.CustomerId
                        LEFT JOIN OrderMaster OM ON RIM.OrderId=OM.OrderID
                        Where RIM.COMPANYID=" + DDcompanyName.SelectedValue + " And RIM.BranchID = " + DDBranchName.SelectedValue + " and CI.CustomerCode<>''";
        if (txtsuppliersearch.Text != "")
        {
            str2 = str2 + " and RIM.Suppliername like '" + txtsuppliersearch.Text.Trim() + "%'";
        }
        if (txtCustomerOrderNo.Text != "")
        {
            str2 = str2 + " and OM.CustomerOrderNo like '" + txtCustomerOrderNo.Text.Trim() + "%'";
        }       
       
        UtilityModule.ConditionalComboFill(ref DDCustomerCode, str2, true, "--Select--");

        string str3 = @"Select Distinct OM.OrderId,OM.CustomerOrderNo FROM LABELINSPECTIONMASTER RIM INNER JOIN LABELINSPECTIONDETAIL RID ON RIM.DOCID=RID.DOCID 
                        LEFT JOIN OrderMaster OM ON RIM.OrderId=OM.OrderId 
                        Where RIM.COMPANYID=" + DDcompanyName.SelectedValue + " And RIM.BranchID = " + DDBranchName.SelectedValue + " and OM.CustomerOrderNo<>''";
        if (txtsuppliersearch.Text != "")
        {
            str3 = str3 + " and RIM.Suppliername like '" + txtsuppliersearch.Text.Trim() + "%'";
        }       
        if (txtCustomerOrderNo.Text != "")
        {
            str3 = str3 + " and OM.CustomerOrderNo like '" + txtCustomerOrderNo.Text.Trim() + "%'";
        }
        
        UtilityModule.ConditionalComboFill(ref DDCustomerOrderNo, str3, true, "--Select--");
        

        string str = @"SELECT Distinct isnull(RIM.OrderId,0) as OrderId,isnull(CI.CustomerId,0) as CustomerId FROM LABELINSPECTIONMASTER RIM INNER JOIN LABELINSPECTIONDETAIL RID ON RIM.DOCID=RID.DOCID                       
                       LEFT JOIN CustomerInfo CI ON RIM.CustomerID=CI.CustomerId
                       LEFT JOIN OrderMaster OM ON RIM.OrderId=OM.OrderID
                      Where RIM.COMPANYID=" + DDcompanyName.SelectedValue + " And RIM.BranchID = " + DDBranchName.SelectedValue;
        if (txtsuppliersearch.Text != "")
        {
            str = str + " and RIM.Suppliername like '" + txtsuppliersearch.Text.Trim() + "%'";
        }
        if (txtCustomerOrderNo.Text != "")
        {
            str = str + " and OM.CustomerOrderNo like '" + txtCustomerOrderNo.Text.Trim() + "%'";
        }
       
        //str = str + " order by DOCID";
         DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
         if (ds.Tables[0].Rows.Count > 0)
         {             
             DDCustomerCode.SelectedValue = ds.Tables[0].Rows[0]["CustomerId"].ToString();             
             DDCustomerOrderNo.SelectedValue = ds.Tables[0].Rows[0]["OrderId"].ToString();
         }
         else
         {
             DDCustomerCode.SelectedValue = "0";
             DDCustomerOrderNo.SelectedValue = "0";
         }

    }
    private void fillDocno()
    {
        string str = @"SELECT Distinct RIM.DOCID,RIM.DOCNO +' # ' +Replace(convert(nvarchar(11),RIM.Reportdate,106),' ','-') as DocNo 
                        FROM LABELINSPECTIONMASTER RIM INNER JOIN LABELINSPECTIONDETAIL RID ON RIM.DOCID=RID.DOCID
                        LEFT JOIN OrderMaster OM ON RIM.OrderId=OM.OrderID
                      Where RIM.COMPANYID=" + DDcompanyName.SelectedValue + " And RIM.BranchID = " + DDBranchName.SelectedValue;
        if (txtsuppliersearch.Text != "")
        {
            str = str + " and RIM.Suppliername like '" + txtsuppliersearch.Text.Trim() + "%'";
        }
        if (txtCustomerOrderNo.Text != "")
        {
            str = str + " and OM.CustomerOrderNo like '" + txtCustomerOrderNo.Text.Trim() + "%'";
        }       
         if (DDCustomerOrderNo.SelectedIndex > 0)
         {
                str = str + " and RIM.OrderId=" + DDCustomerOrderNo.SelectedValue + "";
         }
        
        
        str = str + " order by DOCID";
        UtilityModule.ConditionalComboFill(ref DDDocNo, str, true, "--Plz Select--");
    }
    protected void DDDocNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        hndocid.Value = DDDocNo.SelectedValue;
        refreshcontrol();
        FillDataback();
    }
    protected void FillDataback()
    {
        string str = @"SELECT * FROM LABELINSPECTIONMASTER RIM Where RIM.DocId=" + hndocid.Value + @"
                       SELECT * FROM LABELINSPECTIONDETAIL RID Where RID.DocId=" + hndocid.Value;
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        if (ds.Tables[0].Rows.Count > 0)
        {
            txtdocno.Text = ds.Tables[0].Rows[0]["docNo"].ToString();
            txtdate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["Reportdate"]).ToString("dd-MMM-yyyy");
            txtstandared.Text = ds.Tables[0].Rows[0]["standared"].ToString();
            txtsize.Text = ds.Tables[0].Rows[0]["Size"].ToString();
            txtsuppliername.Text = ds.Tables[0].Rows[0]["suppliername"].ToString();
            txtchallannodate.Text = ds.Tables[0].Rows[0]["challanNodate"].ToString();
            txtcomments.Text = ds.Tables[0].Rows[0]["comments"].ToString();

            Changeapprovebuttoncolor(Convert.ToInt16(ds.Tables[0].Rows[0]["approvestatus"]));
            EditRights_Button(Convert.ToInt16(Session["usertype"]), Convert.ToInt16(ds.Tables[0].Rows[0]["approvestatus"]));

            if (ds.Tables[1].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        TextBox txtsrno = (TextBox)Dgdetail.Rows[i].Cells[0].FindControl("txtsrno");
                        TextBox txtdescription = (TextBox)Dgdetail.Rows[i].Cells[1].FindControl("txtdescription");
                        TextBox txttotalqty = (TextBox)Dgdetail.Rows[i].Cells[2].FindControl("txttotalqty");
                        TextBox txtsamplesize = (TextBox)Dgdetail.Rows[i].Cells[3].FindControl("txtsamplesize");
                        TextBox txtgumming = (TextBox)Dgdetail.Rows[i].Cells[4].FindControl("txtgumming");
                        TextBox txtscanability = (TextBox)Dgdetail.Rows[i].Cells[5].FindControl("txtscanability");
                        TextBox txtcolor = (TextBox)Dgdetail.Rows[i].Cells[6].FindControl("txtcolor");
                        TextBox txtfont = (TextBox)Dgdetail.Rows[i].Cells[7].FindControl("txtfont");
                        TextBox txtprinting = (TextBox)Dgdetail.Rows[i].Cells[8].FindControl("txtprinting");
                        TextBox txtbleeding = (TextBox)Dgdetail.Rows[i].Cells[9].FindControl("txtbleeding");
                        TextBox txtcutting = (TextBox)Dgdetail.Rows[i].Cells[10].FindControl("txtcutting");
                        TextBox txtsizegrid = (TextBox)Dgdetail.Rows[i].Cells[11].FindControl("txtsize");
                        TextBox txtfound = (TextBox)Dgdetail.Rows[i].Cells[12].FindControl("txtfound");
                        TextBox txtacceptance = (TextBox)Dgdetail.Rows[i].Cells[13].FindControl("txtacceptance");
                        DropDownList ddresult = (DropDownList)Dgdetail.Rows[i].Cells[14].FindControl("ddresult");


                        txtsrno.Text = ds.Tables[1].Rows[i]["srno"].ToString();
                        txtdescription.Text = ds.Tables[1].Rows[i]["description"].ToString();
                        txttotalqty.Text = ds.Tables[1].Rows[i]["totalqty"].ToString();
                        txtsamplesize.Text = ds.Tables[1].Rows[i]["samplesize"].ToString();
                        txtgumming.Text = ds.Tables[1].Rows[i]["gumming"].ToString();
                        txtscanability.Text = ds.Tables[1].Rows[i]["scanability"].ToString();
                        txtcolor.Text = ds.Tables[1].Rows[i]["color"].ToString();
                        txtfont.Text = ds.Tables[1].Rows[i]["font"].ToString();
                        txtprinting.Text = ds.Tables[1].Rows[i]["printing"].ToString();
                        txtbleeding.Text = ds.Tables[1].Rows[i]["Bleeding"].ToString();
                        txtcutting.Text = ds.Tables[1].Rows[i]["Cutting"].ToString();
                        txtsizegrid.Text = ds.Tables[1].Rows[i]["size"].ToString();
                        txtfound.Text = ds.Tables[1].Rows[i]["found"].ToString();
                        txtacceptance.Text = ds.Tables[1].Rows[i]["acceptance"].ToString();
                        ddresult.SelectedItem.Text = ds.Tables[1].Rows[i]["lotresult"].ToString();
                    }
                    else
                    {
                        AddNewRowToGrid();

                        TextBox txtsrno = (TextBox)Dgdetail.Rows[i].Cells[0].FindControl("txtsrno");
                        TextBox txtdescription = (TextBox)Dgdetail.Rows[i].Cells[1].FindControl("txtdescription");
                        TextBox txttotalqty = (TextBox)Dgdetail.Rows[i].Cells[2].FindControl("txttotalqty");
                        TextBox txtsamplesize = (TextBox)Dgdetail.Rows[i].Cells[3].FindControl("txtsamplesize");
                        TextBox txtgumming = (TextBox)Dgdetail.Rows[i].Cells[4].FindControl("txtgumming");
                        TextBox txtscanability = (TextBox)Dgdetail.Rows[i].Cells[5].FindControl("txtscanability");
                        TextBox txtcolor = (TextBox)Dgdetail.Rows[i].Cells[6].FindControl("txtcolor");
                        TextBox txtfont = (TextBox)Dgdetail.Rows[i].Cells[7].FindControl("txtfont");
                        TextBox txtprinting = (TextBox)Dgdetail.Rows[i].Cells[8].FindControl("txtprinting");
                        TextBox txtbleeding = (TextBox)Dgdetail.Rows[i].Cells[9].FindControl("txtbleeding");
                        TextBox txtcutting = (TextBox)Dgdetail.Rows[i].Cells[10].FindControl("txtcutting");
                        TextBox txtsizegrid = (TextBox)Dgdetail.Rows[i].Cells[11].FindControl("txtsize");
                        TextBox txtfound = (TextBox)Dgdetail.Rows[i].Cells[12].FindControl("txtfound");
                        TextBox txtacceptance = (TextBox)Dgdetail.Rows[i].Cells[13].FindControl("txtacceptance");
                        DropDownList ddresult = (DropDownList)Dgdetail.Rows[i].Cells[14].FindControl("ddresult");


                        txtsrno.Text = ds.Tables[1].Rows[i]["srno"].ToString();
                        txtdescription.Text = ds.Tables[1].Rows[i]["description"].ToString();
                        txttotalqty.Text = ds.Tables[1].Rows[i]["totalqty"].ToString();
                        txtsamplesize.Text = ds.Tables[1].Rows[i]["samplesize"].ToString();
                        txtgumming.Text = ds.Tables[1].Rows[i]["gumming"].ToString();
                        txtscanability.Text = ds.Tables[1].Rows[i]["scanability"].ToString();
                        txtcolor.Text = ds.Tables[1].Rows[i]["color"].ToString();
                        txtfont.Text = ds.Tables[1].Rows[i]["font"].ToString();
                        txtprinting.Text = ds.Tables[1].Rows[i]["printing"].ToString();
                        txtbleeding.Text = ds.Tables[1].Rows[i]["Bleeding"].ToString();
                        txtcutting.Text = ds.Tables[1].Rows[i]["Cutting"].ToString();
                        txtsizegrid.Text = ds.Tables[1].Rows[i]["size"].ToString();
                        txtfound.Text = ds.Tables[1].Rows[i]["found"].ToString();
                        txtacceptance.Text = ds.Tables[1].Rows[i]["acceptance"].ToString();
                        ddresult.SelectedItem.Text = ds.Tables[1].Rows[i]["lotresult"].ToString();
                    }


                }
            }

        }
    }
    protected void EditRights_Button(int usertype, int approvestatus = 0)
    {
        switch (usertype)
        {
            case 1:
            case 2:
                btnsave.Visible = true;
                btndelete.Visible = true;
                break;
            default:
                if (approvestatus == 1)
                {
                    btnsave.Visible = false;
                    btndelete.Visible = false;
                }
                else
                {
                    btnsave.Visible = true;
                    btndelete.Visible = true;
                }
                break;
        }
    }
    protected void btnsearch_Click(object sender, EventArgs e)
    {
        fillOrderNo();
        fillDocno();        
    }
    protected void btnSearchOrderNo_Click(object sender, EventArgs e)
    {
        
        fillOrderNo();
        fillDocno();
    }
    protected void Dgdetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lbllotresult = (Label)e.Row.FindControl("lbllotresult");
            DropDownList ddresult = (DropDownList)e.Row.FindControl("ddresult");
            if (ddresult != null)
            {
                if (ddresult.Items.FindByText(lbllotresult.Text) != null)
                {
                    ddresult.SelectedItem.Text = lbllotresult.Text;
                }
            }
        }
    }
}