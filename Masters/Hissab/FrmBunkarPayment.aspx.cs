﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Data.OleDb;
using System.Configuration;
using System.Data.Common;

public partial class Masters_Hissab_FrmBunkarPayment : System.Web.UI.Page
{
    static int MasterCompanyId;
    static int GVBunkarCarpetReceiveCount = 0;
    static int sum = 0;
    static string name = "";
    static decimal TotalPenalityAmt = 0;
    static string hnReceiveDate = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        if (IsPostBack != true)
        {
            BindContractor();

            string str = @"select Distinct CI.CompanyId,CI.CompanyName from Companyinfo CI,Company_Authentication CA Where CI.CompanyId=CA.CompanyId And CA.UserId=" + Session["varuserId"] + "  And CI.MasterCompanyId=" + Session["varCompanyId"] + @" Order By CompanyName
                           Select VarCompanyNo,VarProdCode From MasterSetting 
                           SELECT MONTH_ID,MONTH_NAME FROM MONTHTABLE order by Month_Id
                           SELECT YEAR,YEAR AS YEAR1 FROM YEARDATA order by Year";

            DataSet ds = new DataSet();
            ds = SqlHelper.ExecuteDataset(str);
            UtilityModule.ConditionalComboFillWithDS(ref DDCompanyName, ds, 0, true, "--Select--");
            UtilityModule.ConditionalComboFillWithDS(ref DDMonth, ds, 2, true, "--Select--");
            UtilityModule.ConditionalComboFillWithDS(ref DDYear, ds, 3, true, "--Select--");
            hncomp.Value = ds.Tables[1].Rows[0]["VarCompanyNo"].ToString();
            int VarProdCode = Convert.ToInt32(ds.Tables[1].Rows[0]["VarProdCode"]);

            if (DDMonth.Items.Count > 0)
            {
                DDMonth.SelectedValue = DateTime.Now.Month.ToString();
            }
            if (DDYear.Items.Count > 0)
            {
                DDYear.SelectedValue = DateTime.Now.Year.ToString();
            }

            //TxtRecDate.Text = DateTime.Now.ToString("dd-MMM-yyyy");

            if (DDCompanyName.Items.Count > 0)
            {
                DDCompanyName.SelectedValue = Session["CurrentWorkingCompanyID"].ToString();
                DDCompanyName.Enabled = false;
                BindContractor();
                DDItemName.Items.Clear();
                DDBunkarName.Items.Clear();
                ClearAfterCompanyChange();
            }
        }
    }
    
    private void ClearAfterCompanyChange()
    {
        //DDEmployeeNamee.Focus();
        GVBazaarDetail.DataSource = null;
        GVBazaarDetail.DataBind();

        //GVPenalty.DataSource = null;
        //GVPenalty.DataBind();       

        ViewState["BunkarCarpetReceive"] = null;
        ViewState["BunkarCarpetReceivePen"] = null;
    }

    protected void BindContractor()
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        try
        {
            using (con)
            {
                using (SqlCommand cmd = new SqlCommand("PRO_DDBINDCONTRACTOR", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            DDContractorName.DataSource = dr;
                            DDContractorName.DataTextField = "EmpName";
                            DDContractorName.DataValueField = "EmpId";
                            DDContractorName.DataBind();
                            DDContractorName.Items.Insert(0, new ListItem("--Select--", "0"));
                        }
                    }
                    con.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write("Error:" + ex.Message.ToString());
        }
    }
    protected void DDCompanyName_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindContractor();
        DDItemName.Items.Clear();
        DDBunkarName.Items.Clear();
        ClearAfterCompanyChange();
    }
    private void BindItemName()
    {
        string str = "";

        str = @" select Distinct VF.Item_Id,VF.Item_Name from Process_Receive_Master_1 PRM JOIN Process_Receive_Detail_1 PRD ON PRM.Process_Rec_Id=PRD.Process_Rec_ID 
                JOIN V_finishedItemDetailNew VF ON PRD.Item_Finished_Id=VF.Item_Finished_Id Where PRM.EmpID=" + DDContractorName.SelectedValue + " order by VF.Item_Name";

        UtilityModule.ConditionalComboFill(ref DDItemName, str, true, "-----------Select------");
    }
    protected void DDContractorName_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindItemName();
        DDBunkarName.Items.Clear();
    }
    private void BindBunkarName()
    {
        string str = "";

        str = @" select BMID,BunkarName from bunkarmaster Where Status=0 and ContractorId=" + DDContractorName.SelectedValue + " Order by BunkarName";

        UtilityModule.ConditionalComboFill(ref DDBunkarName, str, true, "-----------Select------");
    }
    protected void DDItemName_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindBunkarName();
    }
    private DataSet FillBazaarDetail()
    {
        string sql = "";
        if (variable.VarNewQualitySize == "1")
        {
            sql = @"select *,BazaarQty-BunkarQty as PQty from V_BazarDetailWithBunkar Where CompanyId=" + DDCompanyName.SelectedValue + " and Item_ID=" + DDItemName.SelectedValue + " and empid=" + DDContractorName.SelectedValue + " and BazaarQty-BunkarQty>0 ";

        }

        sql = sql + "order by Process_Rec_ID";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, sql);
        if (ds.Tables[0].Rows.Count > 0)
        {
            GVBazaarDetail.DataSource = ds;
            GVBazaarDetail.DataBind();
            GVBazaarDetail.Visible = true;
        }
        else
        {
            GVBazaarDetail.Visible = false;
            ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true);
        }
        return ds;
    }
    protected void GVBazaarDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "javascript:setMouseOverColor(this);";
            e.Row.Attributes["onmouseout"] = "javascript:setMouseOutColor(this);";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.GVBazaarDetail, "Select$" + e.Row.RowIndex);
        }
    }
    private void FillBunkarPaymentDetail()
    {
        string sql = "";
        if (variable.VarNewQualitySize == "1")
        {
            sql = @"select * from V_BunkarEntryDetail Where CompanyId=" + DDCompanyName.SelectedValue + " and Item_ID=" + DDItemName.SelectedValue + " and ContractorId=" + DDContractorName.SelectedValue + " and BunkarId=" + DDBunkarName.SelectedValue + " and MonthId=" + DDMonth.SelectedValue + " and Year=" + DDYear.SelectedValue + "";

        }

        sql = sql + "order by Process_Rec_ID";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, sql);
        if (ds.Tables[0].Rows.Count > 0)
        {
            GVBunkarPaymentDetail.DataSource = ds;
            GVBunkarPaymentDetail.DataBind();
            GVBunkarPaymentDetail.Visible = true;
        }
        //else
        //{
        //    GVBunkarPaymentDetail.Visible = false;
        //    ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true);
        //}
    }
    protected void GVBunkarPaymentDetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "javascript:setMouseOverColor(this);";
            e.Row.Attributes["onmouseout"] = "javascript:setMouseOutColor(this);";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.GVBunkarPaymentDetail, "select$" + e.Row.RowIndex);
        }
    }
    public string getgiven(string strval, string strval1, string strval2, string strval3)
    {
        string val = "0";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "select isnull(sum(Qty),0) from BunkarpaymentDetail where Item_Finished_Id=" + strval1 + "  and Process_Rec_Id=" + strval2 + " and Process_Rec_Detail_Id=" + strval3 + "");
        if (ds.Tables[0].Rows.Count > 0)
        {
            val = Convert.ToString(Convert.ToInt32(strval) - Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()));
        }
        return val;
    }

    decimal TotalQty2 = 0;
    decimal TotalArea2 = 0;
    decimal TotalWeight = 0;
    protected void CalQtyTotalArea()
    {

        for (int i = 0; i < GVBunkarCarpetReceive.Rows.Count; i++)
        {
            TextBox TxtRecDate = ((TextBox)GVBunkarCarpetReceive.Rows[i].FindControl("TxtRecDate"));
            TextBox txtBZWeight = ((TextBox)GVBunkarCarpetReceive.Rows[i].FindControl("txtBZWeight"));
            Label lblRate = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblRate"));
            Label lblBunkarQty = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblBunkarQty"));


            TextBox txtBunkarQty = ((TextBox)GVBunkarCarpetReceive.Rows[i].FindControl("txtBunkarQty"));
            TotalQty2 += Convert.ToDecimal(txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text);
            Label lblArea = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblArea"));
            TotalArea2 += Convert.ToDecimal(lblArea.Text) * Convert.ToDecimal(txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text);

            //Label lblFinalWt = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblFinalWt"));
            //TotalWeight += Convert.ToDecimal(lblFinalWt.Text);

            Label lblTArea = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblTArea"));
            lblTArea.Text = Convert.ToString(Convert.ToDecimal(lblArea.Text) * Convert.ToDecimal(txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text));

            Label lblLagat = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblLagat"));

            Label lblStWeight = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblStWeight"));
            lblStWeight.Text = Convert.ToString(Convert.ToDecimal(lblArea.Text) * Convert.ToDecimal(txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text) * Convert.ToDecimal(lblLagat.Text));

            Label lblDefWeight = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblDefWeight"));
            lblDefWeight.Text = Convert.ToString(Convert.ToDecimal(txtBZWeight.Text == "" ? "0" : txtBZWeight.Text) - Convert.ToDecimal(lblStWeight.Text));

            Label lblPenalityName = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblPenalityName"));

            Label lblSrNo = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblSrNo"));
            Label lblExtraWeight = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblExtraWeight"));

            if (Convert.ToDouble(lblDefWeight.Text) < 0)
            {
                lblExtraWeight.Text = "0";
            }
            else
            {
                lblExtraWeight.Text = lblDefWeight.Text;
            }
            ////Actual perc
            Label lblActualPercentage = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblActualPercentage"));
            Label lblLessPercentage = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblLessPercentage"));
            if (Convert.ToDouble(lblStWeight.Text) == 0)
            {
                lblActualPercentage.Text = "0";
            }
            else
            {
                lblActualPercentage.Text = Convert.ToString(Math.Round(Convert.ToDouble(lblDefWeight.Text) / Convert.ToDouble(lblStWeight.Text) * 100, 2));
            }


            ////Less Perc
            if (Convert.ToDouble(lblActualPercentage.Text) < 0)
            {
                lblLessPercentage.Text = "0";
            }
            else
            {
                lblLessPercentage.Text = lblActualPercentage.Text;
            }

            ////Wamount
            Label lblWAmount = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblWAmount"));
            lblWAmount.Text = Convert.ToString(Convert.ToDecimal(lblTArea.Text) * Convert.ToDecimal(lblRate.Text));

            Label lblItemId = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblItemId"));
            Label lblWeightPRate = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblWeightPRate"));
            Label lblWeightPenality = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblWeightPenality"));
            if (Convert.ToDouble(lblLessPercentage.Text) > 0 && TxtRecDate.Text != "")
            {
                lblWeightPRate.Text = Convert.ToString(UtilityModule.Get_WeightPenalty(Convert.ToInt32(lblItemId.Text), Convert.ToDouble(lblLessPercentage.Text), TxtRecDate.Text));
                lblWeightPenality.Text = Convert.ToString(Convert.ToDecimal(lblWeightPRate.Text) * Convert.ToDecimal(lblTArea.Text));
            }
            else
            {
                lblWeightPRate.Text = "0";
                lblWeightPenality.Text = "0";
            }
            Label lblPaidAmount = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblPaidAmount"));
            Label lblPenalityAmount = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblPenalityAmount"));
            lblPaidAmount.Text = Convert.ToString(Convert.ToDecimal(lblWAmount.Text) - Convert.ToDecimal(lblPenalityAmount.Text) - Convert.ToDecimal(lblWeightPenality.Text));

            DataTable dt3 = new DataTable();
            dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

            if (dt3.Rows.Count > 0)
            {
                foreach (DataRow row in dt3.Rows)
                {
                    if (row["SrNo"].ToString() == lblSrNo.Text)
                    {
                        row["ExtraWeight"] = lblExtraWeight.Text; ;
                        row["ActualPercentage"] = lblActualPercentage.Text;
                        row["LessPercentage"] = lblLessPercentage.Text;
                        row["WeightPRate"] = lblWeightPRate.Text;
                        row["PenalityAmount"] = lblPenalityAmount.Text;
                        row["WeightPenality"] = lblWeightPenality.Text;
                        row["PaidAmount"] = lblPaidAmount.Text;
                    }
                    dt3.AcceptChanges();
                    row.SetModified();
                    ViewState["BunkarCarpetReceive"] = dt3;
                }
            }

        }

    }
    public Boolean BQty(int ProcessRecDetailId, int rowindex)
    {
        llMessageBox.Visible = false;
        llMessageBox.Text = "";

        int Balqty = 0;
        int Issuedqty = 0;
        int Count2 = GVBazaarDetail.Rows.Count;
        int Count3 = GVBunkarCarpetReceive.Rows.Count;

        if (GVBazaarDetail.Rows.Count > 0)
        {
            for (int k = 0; k < Count2; k++)
            {
                string GLQualityId = ((Label)(GVBazaarDetail.Rows[k].FindControl("lblQualityid"))).Text;
                string GLItemId = ((Label)(GVBazaarDetail.Rows[k].FindControl("lblitem_id"))).Text;
                string lblProcessRecDetailId2 = ((Label)(GVBazaarDetail.Rows[k].FindControl("lblProcess_Rec_Detail_Id"))).Text;
                string lblbalnce2 = ((Label)(GVBazaarDetail.Rows[k].FindControl("lblbalnce"))).Text;
                string hnBalanceQty = ((Label)(GVBazaarDetail.Rows[k].FindControl("hnBalanceQty"))).Text;

                if (ProcessRecDetailId == Convert.ToInt32(lblProcessRecDetailId2))
                {
                    lblbalnce2 = Convert.ToString(Convert.ToInt32(hnBalanceQty) - sum);
                    Balqty = Convert.ToInt32(hnBalanceQty);
                }
            }
        }
        if (GVBunkarCarpetReceive.Rows.Count > 0)
        {
            for (int k = 0; k < Count3; k++)
            {
                string Process_Rec_Detail_Id = ((Label)(GVBunkarCarpetReceive.Rows[k].FindControl("lblProcess_Rec_Detail_ID"))).Text;
                string txtBunkarQty = ((TextBox)(GVBunkarCarpetReceive.Rows[k].FindControl("txtBunkarQty"))).Text;

                if (ProcessRecDetailId == Convert.ToInt32(Process_Rec_Detail_Id))
                {
                    Issuedqty += Convert.ToInt32(txtBunkarQty);
                }
            }
        }
        if (Issuedqty > Balqty)
        {
            llMessageBox.Visible = true;
            llMessageBox.Text = "Receive Qty Cannot greater than order qty!!";
            TextBox RecQty = (TextBox)GVBunkarCarpetReceive.Rows[rowindex].FindControl("txtBunkarQty");
            RecQty.Text = "0";
            return false;
        }
        else
        {
            if (GVBazaarDetail.Rows.Count > 0)
            {
                for (int k = 0; k < Count2; k++)
                {
                    string lblProcessRecDetailId2 = ((Label)(GVBazaarDetail.Rows[k].FindControl("lblProcess_Rec_Detail_Id"))).Text;
                    Label lblbalnce2 = ((Label)(GVBazaarDetail.Rows[k].FindControl("lblbalnce")));

                    if (ProcessRecDetailId == Convert.ToInt32(lblProcessRecDetailId2))
                    {
                        lblbalnce2.Text = Convert.ToString(Balqty - Issuedqty);
                    }
                }
            }
        }
        return true;
    }
    protected void txtBunkarQty_TextChanged(object sender, EventArgs e)
    {
        llMessageBox.Visible = false;
        llMessageBox.Text = "";

        TextBox txt = (TextBox)sender;
        GridViewRow gvRow = (GridViewRow)txt.Parent.Parent;
        txt.Focus();

        Label lblSrNo = (Label)gvRow.FindControl("lblSrNo");


        TextBox txtBunkarQty = (TextBox)gvRow.FindControl("txtBunkarQty");

        Label Item = (Label)gvRow.FindControl("lblItemId");
        Label lblQualityid = (Label)gvRow.FindControl("lblQualityId");
        Label ProcessRecDetailId = (Label)gvRow.FindControl("lblProcess_Rec_Detail_ID");
        Label hnBalQty = (Label)gvRow.FindControl("hnBalQty");
        Label lblArea = (Label)gvRow.FindControl("lblArea");
        Label lblPenalityName = (Label)gvRow.FindControl("lblPenalityName");
        lblPenalityName.Text = "";
        Label lblCalType = (Label)gvRow.FindControl("lblCalType");
        Label lblPenalityAmount = (Label)gvRow.FindControl("lblPenalityAmount");
        lblPenalityAmount.Text = "0";


        if (Convert.ToInt32(txtBunkarQty.Text) > 0)
        {

            DataTable dt3 = new DataTable();
            dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

            // int sum = 0;
            //sum = Convert.ToInt32(dt3.Compute("SUM(Qty)", "ItemId=" + Item.Text + " and QualityId=" + lblQualityid.Text + " and Issue_Detail_Id=" + IssueDetailId.Text));

            if (BQty(Convert.ToInt32(ProcessRecDetailId.Text), gvRow.RowIndex) == true)
            {
                if (dt3.Rows.Count > 0)
                {
                    foreach (DataRow row in dt3.Rows)
                    {
                        if (row["SrNo"].ToString() == lblSrNo.Text)
                        {
                            row["BunkarQty"] = txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text;


                            if (lblCalType.Text == "0" || lblCalType.Text == "2" || lblCalType.Text == "3" || lblCalType.Text == "4")
                            {
                                row["WAmount"] = Convert.ToDecimal(lblArea.Text) * Convert.ToInt32(txtBunkarQty.Text) * Convert.ToDecimal(row["Rate"].ToString());
                                //row["CommAmt"] = Convert.ToDecimal(lblArea.Text) * Convert.ToInt32(txtBunkarQty.Text) * Convert.ToDecimal(row["Comm"].ToString());
                            }
                            if (lblCalType.Text == "1")
                            {
                                row["WAmount"] = Convert.ToInt32(txtBunkarQty.Text) * Convert.ToDecimal(row["Rate"].ToString());
                                // row["CommAmt"] = Convert.ToInt32(txtReqQty.Text) * Convert.ToDecimal(row["Comm"].ToString());
                            }

                            row["TArea"] = Convert.ToDecimal(lblArea.Text) * Convert.ToInt32(txtBunkarQty.Text);
                        }
                        dt3.AcceptChanges();
                        row.SetModified();
                        ViewState["BunkarCarpetReceive"] = dt3;
                    }
                }
                CalQtyTotalArea();
                CarpetPenalityDel(Convert.ToInt32(lblSrNo.Text));
            }



        }
        else
        {
            llMessageBox.Visible = true;
            llMessageBox.Text = "Sorry Less Than Equal Zero Not Allowed !!";
        }

    }
    protected void GVBazaarDetail_SelectedIndexChanged(object sender, EventArgs e)
    {
        //****************sql Table Type 

        /// tbsample.ActiveTabIndex = 1;

        llMessageBox.Visible = false;
        llMessageBox.Text = "";
        int max = 0;
        int srno;
        int TypeId = 1;

        DataTable dtrecords = new DataTable();

        if (ViewState["BunkarCarpetReceive"] == null)
        {
            //dtrecords = new DataTable();
            dtrecords.Columns.Add("SrNo", typeof(int));
            dtrecords.Columns.Add("BazaarChallanNo", typeof(string));
            dtrecords.Columns.Add("ReceiveDate", typeof(DateTime));

            dtrecords.Columns.Add("ItemName", typeof(string));
            dtrecords.Columns.Add("Quality", typeof(string));
            dtrecords.Columns.Add("Design", typeof(string));
            dtrecords.Columns.Add("Color", typeof(string));
            dtrecords.Columns.Add("Shape", typeof(string));
            dtrecords.Columns.Add("Size", typeof(string));
            dtrecords.Columns.Add("BunkarQty", typeof(int));
            dtrecords.Columns.Add("BZWeight", typeof(float));
            dtrecords.Columns.Add("StWeight", typeof(float));
            dtrecords.Columns.Add("Penality", typeof(string));
            dtrecords.Columns.Add("Area", typeof(float));
            dtrecords.Columns.Add("TArea", typeof(float));
            dtrecords.Columns.Add("Lagat", typeof(float));
            dtrecords.Columns.Add("DefWeight", typeof(float));
            dtrecords.Columns.Add("ExtraWeight", typeof(float));
            dtrecords.Columns.Add("ActualPercentage", typeof(float));
            dtrecords.Columns.Add("LessPercentage", typeof(float));
            dtrecords.Columns.Add("Rate", typeof(float));
            dtrecords.Columns.Add("WeightPRate", typeof(float));
            dtrecords.Columns.Add("WAmount", typeof(float));
            dtrecords.Columns.Add("PenalityAmount", typeof(float));
            dtrecords.Columns.Add("WeightPenality", typeof(float));
            dtrecords.Columns.Add("PaidAmount", typeof(float));
            dtrecords.Columns.Add("Item_Finished_Id", typeof(int));
            dtrecords.Columns.Add("Process_Rec_Id", typeof(int));
            dtrecords.Columns.Add("Process_Rec_Detail_Id", typeof(int));
            dtrecords.Columns.Add("BazaarQty", typeof(int));
            dtrecords.Columns.Add("UnitId", typeof(int));
            dtrecords.Columns.Add("BazaarReceiveDate", typeof(DateTime));
            dtrecords.Columns.Add("ItemId", typeof(int));
            dtrecords.Columns.Add("QualityId", typeof(int));
            dtrecords.Columns.Add("ItemDescription", typeof(string));
            dtrecords.Columns.Add("CalType", typeof(int));
            dtrecords.Columns.Add("BalQty", typeof(int));

            ViewState["BunkarCarpetReceive"] = dtrecords;
        }
        else
        {
            dtrecords = (DataTable)ViewState["BunkarCarpetReceive"];
        }


        int r = Convert.ToInt32(GVBazaarDetail.SelectedIndex.ToString());



        if (r >= 0)
        {
            string lblbalnce = ((Label)GVBazaarDetail.Rows[r].FindControl("lblbalnce")).Text;
            if (Convert.ToInt32(lblbalnce) > 0 && lblbalnce != "")
            {
                string lblChallanNo = ((Label)GVBazaarDetail.Rows[r].FindControl("lblChallanNo")).Text;

                string lblItemName = ((Label)GVBazaarDetail.Rows[r].FindControl("lblItemName")).Text;
                string lblQuality = ((Label)GVBazaarDetail.Rows[r].FindControl("lblQuality")).Text;
                string lblDesign = ((Label)GVBazaarDetail.Rows[r].FindControl("lblDesign")).Text;
                string lblColor = ((Label)GVBazaarDetail.Rows[r].FindControl("lblColor")).Text;
                string lblShape = ((Label)GVBazaarDetail.Rows[r].FindControl("lblShape")).Text;
                string lblSize = ((Label)GVBazaarDetail.Rows[r].FindControl("lblSize")).Text;
                string lblBazaarQty = ((Label)GVBazaarDetail.Rows[r].FindControl("lblBazaarQty")).Text;


                string lblArea = ((Label)GVBazaarDetail.Rows[r].FindControl("lblArea")).Text;
                string lblLagat = ((Label)GVBazaarDetail.Rows[r].FindControl("lblLagat")).Text;
                string lblRate = ((Label)GVBazaarDetail.Rows[r].FindControl("lblRate")).Text;
                string lblItemFinishedId = ((Label)GVBazaarDetail.Rows[r].FindControl("lblItemFinishedId")).Text;
                string lblProcess_Rec_Id = ((Label)GVBazaarDetail.Rows[r].FindControl("lblProcess_Rec_Id")).Text;
                string lblProcess_Rec_Detail_Id = ((Label)GVBazaarDetail.Rows[r].FindControl("lblProcess_Rec_Detail_Id")).Text;
                string lblUnitId = ((Label)GVBazaarDetail.Rows[r].FindControl("lblUnitId")).Text;
                string lblBazaarReceiveDate = ((Label)GVBazaarDetail.Rows[r].FindControl("lblReceiveDate")).Text;
                string lblitem_id = ((Label)GVBazaarDetail.Rows[r].FindControl("lblitem_id")).Text;
                string lblQualityId = ((Label)GVBazaarDetail.Rows[r].FindControl("lblQualityId")).Text;
                string lblDescription = ((Label)GVBazaarDetail.Rows[r].FindControl("lblDescription")).Text;
                string lblCalType = ((Label)GVBazaarDetail.Rows[r].FindControl("lblCalType")).Text;



                //ChkItemFinishedId(Convert.ToInt32(lblItemFinishedId));

                if (Convert.ToInt32(lblbalnce) == 0)
                {
                    //ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('Receive Qty Cannot greater than order qty!');", true);
                    llMessageBox.Visible = true;
                    llMessageBox.Text = "Receive Qty Cannot greater than order qty!!";
                }
                else
                {
                    if (dtrecords.Rows.Count > 0)
                    {
                        max = (int)dtrecords.Compute("MAX(SrNo)", "");
                    }

                    if (max == 0)
                    {
                        srno = 1;
                    }
                    else
                    {
                        srno = max + 1;
                    }


                    if (llMessageBox.Text == "")
                    {

                        //**************
                        DataRow dr = dtrecords.NewRow();
                        dr["SrNo"] = srno;
                        dr["BazaarChallanNo"] = lblChallanNo;
                        if (hnReceiveDate != "")
                        {
                            dr["ReceiveDate"] = hnReceiveDate;
                        }
                        else
                        {
                            dr["ReceiveDate"] = DateTime.Now.ToString("dd-MMM-yyyy");
                        }

                        dr["ItemName"] = lblItemName;
                        dr["Quality"] = lblQuality;
                        dr["Design"] = lblDesign;
                        dr["Color"] = lblColor;
                        dr["Shape"] = lblShape;
                        dr["Size"] = lblSize;
                        dr["BunkarQty"] = Convert.ToInt32(lblbalnce);
                        dr["StWeight"] = Math.Round(Convert.ToDouble(lblbalnce) * Convert.ToDouble(lblArea) * Convert.ToDouble(lblLagat), 4);
                        dr["Penality"] = null;
                        dr["Area"] = lblArea;
                        dr["TArea"] = Convert.ToDouble(lblbalnce) * Convert.ToDouble(lblArea);
                        dr["Lagat"] = lblLagat;
                        dr["DefWeight"] = 0;
                        dr["ExtraWeight"] = 0;
                        dr["ActualPercentage"] = 0;
                        dr["LessPercentage"] = 0;
                        dr["Rate"] = lblRate;
                        dr["WeightPRate"] = 0;
                        dr["WAmount"] = Convert.ToDouble(lblbalnce) * Convert.ToDouble(lblArea) * Convert.ToDouble(lblRate);
                        dr["PenalityAmount"] = 0;
                        dr["WeightPenality"] = 0;
                        dr["PaidAmount"] = 0;
                        dr["Item_Finished_Id"] = lblItemFinishedId;
                        dr["Process_Rec_Id"] = lblProcess_Rec_Id;
                        dr["Process_Rec_Detail_Id"] = lblProcess_Rec_Detail_Id;
                        dr["BazaarQty"] = lblBazaarQty;
                        dr["UnitId"] = lblUnitId;
                        dr["BazaarReceiveDate"] = lblBazaarReceiveDate;
                        dr["ItemId"] = lblitem_id;
                        dr["QualityId"] = lblQualityId;
                        dr["ItemDescription"] = lblDescription;
                        dr["CalType"] = lblCalType;
                        dr["BalQty"] = Convert.ToInt32(lblbalnce) - sum;



                        dtrecords.Rows.Add(dr);
                        ViewState["BunkarCarpetReceive"] = dtrecords;

                        //lblBalanceQty.Text = "0";

                    }

                }
            }
            else
            {
                llMessageBox.Text = "No Pending Qty";
                llMessageBox.Visible = true;
            }
        }
        DataTable dt5 = new DataTable();
        dt5 = (DataTable)ViewState["BunkarCarpetReceive"];

        GVBunkarCarpetReceiveCount = dt5.Rows.Count;

        GVBunkarCarpetReceive.DataSource = dt5;
        GVBunkarCarpetReceive.DataBind();

        BQty(Convert.ToInt32(((Label)GVBazaarDetail.Rows[r].FindControl("lblProcess_Rec_Detail_Id")).Text), GVBazaarDetail.SelectedIndex);



        ////txtBunkarQty_TextChanged(sender, new EventArgs());
    }
    protected void txtBZWeight_TextChanged(object sender, EventArgs e)
    {

        TextBox txt = (TextBox)sender;
        GridViewRow gvRow = (GridViewRow)txt.Parent.Parent;
        txt.Focus();

        //GridViewRow myRow = ((Control)sender).Parent.Parent as GridViewRow;
        //myRow.FindControl("txtUnit").Focus();

        Label lblSrNo = (Label)gvRow.FindControl("lblSrNo");


        TextBox txtBunkarQty = (TextBox)gvRow.FindControl("txtBunkarQty");
        TextBox txtBZWeight = (TextBox)gvRow.FindControl("txtBZWeight");

        Label Item = (Label)gvRow.FindControl("lblItemId");
        Label lblQualityid = (Label)gvRow.FindControl("lblQualityId");
        Label ProcessRecDetailId = (Label)gvRow.FindControl("lblProcess_Rec_Detail_ID");
        Label hnBalQty = (Label)gvRow.FindControl("hnBalQty");
        Label lblArea = (Label)gvRow.FindControl("lblArea");
        //Label lblPenalityName = (Label)gvRow.FindControl("lblPenalityName");
        //lblPenalityName.Text = "";
        Label lblCalType = (Label)gvRow.FindControl("lblCalType");
        Label lblLagat = (Label)gvRow.FindControl("lblLagat");




        DataTable dt3 = new DataTable();
        dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

        // int sum = 0;
        //sum = Convert.ToInt32(dt3.Compute("SUM(Qty)", "ItemId=" + Item.Text + " and QualityId=" + lblQualityid.Text + " and Issue_Detail_Id=" + IssueDetailId.Text));


        if (dt3.Rows.Count > 0)
        {
            foreach (DataRow row in dt3.Rows)
            {
                if (row["SrNo"].ToString() == lblSrNo.Text)
                {
                    row["BZWeight"] = txtBZWeight.Text == "" ? "0" : txtBZWeight.Text;
                    row["DefWeight"] = Math.Round(Convert.ToDecimal(txtBZWeight.Text == "" ? "0" : txtBZWeight.Text) - (Convert.ToDecimal(Convert.ToDecimal(lblArea.Text) * Convert.ToInt32(txtBunkarQty.Text) * Convert.ToDecimal(lblLagat.Text))), 4);

                }
                dt3.AcceptChanges();
                row.SetModified();
                ViewState["BunkarCarpetReceive"] = dt3;
            }
        }


        CalQtyTotalArea();
        ////CarpetPenalityDel(Convert.ToInt32(lblSrNo.Text));        

        int rowcount = GVBunkarCarpetReceive.Rows.Count;
        int rowIndex = gvRow.RowIndex;
        if (rowIndex < rowcount - 1)
        {
            ((TextBox)GVBunkarCarpetReceive.Rows[rowIndex + 1].FindControl("txtBZWeight")).Focus();
        }


    }
    protected void TxtRecDate_TextChanged(object sender, EventArgs e)
    {

        TextBox txt = (TextBox)sender;
        GridViewRow gvRow = (GridViewRow)txt.Parent.Parent;
        txt.Focus();

        Label lblSrNo = (Label)gvRow.FindControl("lblSrNo");

        TextBox TxtRecDate = (TextBox)gvRow.FindControl("TxtRecDate");


        DataTable dt3 = new DataTable();
        dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

        // int sum = 0;
        //sum = Convert.ToInt32(dt3.Compute("SUM(Qty)", "ItemId=" + Item.Text + " and QualityId=" + lblQualityid.Text + " and Issue_Detail_Id=" + IssueDetailId.Text));


        if (dt3.Rows.Count > 0)
        {
            foreach (DataRow row in dt3.Rows)
            {
                if (row["SrNo"].ToString() == lblSrNo.Text)
                {
                    row["ReceiveDate"] = TxtRecDate.Text;

                    hnReceiveDate = TxtRecDate.Text;

                }
                dt3.AcceptChanges();
                row.SetModified();
                ViewState["BunkarCarpetReceive"] = dt3;
            }
        }

        CalQtyTotalArea();

        ////CarpetPenalityDel(Convert.ToInt32(lblSrNo.Text));
    }

    protected void BtnShow_Click(object sender, EventArgs e)
    {
        FillBazaarDetail();
        FillBunkarPaymentDetail();
    }
    protected void GVBunkarCarpetReceive_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int GDCount = GVBunkarCarpetReceiveCount - 1;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.FindControl("txtKhapWidth").Focus();
            TextBox TxtRecDate = (TextBox)e.Row.FindControl("TxtRecDate");

            TextBox txtBunkarQty = (TextBox)e.Row.FindControl("txtBunkarQty");
            TextBox txtBZWeight = (TextBox)e.Row.FindControl("txtBZWeight");
            Label lblRate = (Label)e.Row.FindControl("lblRate");
            Label lblBunkarQty = (Label)e.Row.FindControl("lblBunkarQty");
            //TotalQty += Convert.ToDecimal(lblBunkarQty.Text);
            Label lblArea = (Label)e.Row.FindControl("lblArea");
            //TotalArea += Convert.ToDecimal(lblArea.Text) * Convert.ToDecimal(lblBunkarQty.Text);

            Label lblTArea = (Label)e.Row.FindControl("lblTArea");
            lblTArea.Text = Convert.ToString(Convert.ToDecimal(lblArea.Text) * Convert.ToDecimal(txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text));

            Label lblLagat = (Label)e.Row.FindControl("lblLagat");

            Label lblStWeight = (Label)e.Row.FindControl("lblStWeight");
            lblStWeight.Text = Convert.ToString(Convert.ToDecimal(lblArea.Text) * Convert.ToDecimal(txtBunkarQty.Text == "" ? "0" : txtBunkarQty.Text) * Convert.ToDecimal(lblLagat.Text));

            Label lblDefWeight = (Label)e.Row.FindControl("lblDefWeight");
            lblDefWeight.Text = Convert.ToString(Convert.ToDecimal(txtBZWeight.Text == "" ? "0" : txtBZWeight.Text) - Convert.ToDecimal(lblStWeight.Text));

            Label lblPenalityName = (Label)e.Row.FindControl("lblPenalityName");

            Label lblSrNo = (Label)e.Row.FindControl("lblSrNo");
            Label lblExtraWeight = (Label)e.Row.FindControl("lblExtraWeight");

            if (Convert.ToDouble(lblDefWeight.Text) < 0)
            {
                lblExtraWeight.Text = "0";
            }
            else
            {
                lblExtraWeight.Text = lblDefWeight.Text;
            }
            ////Actual perc
            Label lblActualPercentage = (Label)e.Row.FindControl("lblActualPercentage");
            Label lblLessPercentage = (Label)e.Row.FindControl("lblLessPercentage");
            if (Convert.ToDouble(lblStWeight.Text) == 0)
            {
                lblActualPercentage.Text = "0";
            }
            else
            {

                lblActualPercentage.Text = Convert.ToString(Math.Round(Convert.ToDouble(lblDefWeight.Text) / Convert.ToDouble(lblStWeight.Text) * 100, 2));
            }

            ////Less Perc
            if (Convert.ToDouble(lblActualPercentage.Text) < 0)
            {
                lblLessPercentage.Text = "0";
            }
            else
            {
                lblLessPercentage.Text = lblActualPercentage.Text == "" ? "0" : lblActualPercentage.Text;
            }

            ////Wamount
            Label lblWAmount = (Label)e.Row.FindControl("lblWAmount");
            lblWAmount.Text = Convert.ToString(Convert.ToDecimal(lblTArea.Text) * Convert.ToDecimal(lblRate.Text));

            Label lblItemId = (Label)e.Row.FindControl("lblItemId");
            Label lblWeightPRate = (Label)e.Row.FindControl("lblWeightPRate");
            Label lblWeightPenality = (Label)e.Row.FindControl("lblWeightPenality");
            if (Convert.ToDouble(lblLessPercentage.Text) > 0 && TxtRecDate.Text != "")
            {
                lblWeightPRate.Text = Convert.ToString(UtilityModule.Get_WeightPenalty(Convert.ToInt32(lblItemId.Text), Convert.ToDouble(lblLessPercentage.Text), TxtRecDate.Text));
                lblWeightPenality.Text = Convert.ToString(Convert.ToDecimal(lblWeightPRate.Text) * Convert.ToDecimal(lblTArea.Text));
            }
            else
            {
                lblWeightPRate.Text = "0";
                lblWeightPenality.Text = "0";
            }
            Label lblPaidAmount = (Label)e.Row.FindControl("lblPaidAmount");
            Label lblPenalityAmount = (Label)e.Row.FindControl("lblPenalityAmount");
            //string PenalityAmt = ((Label)GVBunkarCarpetReceive.Rows[e.Row.RowIndex].FindControl("lblPenalityAmount")).Text;
            //lblPenalityAmount.Text = PenalityAmt;
            lblPaidAmount.Text = Convert.ToString(Convert.ToDecimal(lblWAmount.Text) - Convert.ToDecimal(lblPenalityAmount.Text) - Convert.ToDecimal(lblWeightPenality.Text));


            if (e.Row.RowIndex == GDCount)
            {
                if (e.Row.RowIndex > 0)
                {
                    string RecDate2 = ((TextBox)GVBunkarCarpetReceive.Rows[e.Row.RowIndex - 1].FindControl("TxtRecDate")).Text;
                    TxtRecDate.Text = RecDate2;

                    DataTable dt3 = new DataTable();
                    dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

                    if (dt3.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt3.Rows)
                        {
                            if (row["SrNo"].ToString() == lblSrNo.Text)
                            {

                                TxtRecDate.Text = Convert.ToDateTime(row["ReceiveDate"]).ToString("dd-MMM-yyyy");

                            }
                            dt3.AcceptChanges();
                            row.SetModified();
                            ViewState["BunkarCarpetReceive"] = dt3;
                        }
                    }
                }
            }


        }

        //if (e.Row.RowType == DataControlRowType.Footer)
        //{
        //    Label lblGrandTQty = (Label)e.Row.FindControl("lblGrandTQty");
        //    lblGrandTQty.Text = TotalQty.ToString();
        //    Label lblGrandTArea = (Label)e.Row.FindControl("lblGrandTArea");
        //    lblGrandTArea.Text = TotalArea.ToString();
        //}
    }
    protected void GVBunkarCarpetReceive_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int Issuedqty = 0;
        string txtBunkarQty = ((TextBox)GVBunkarCarpetReceive.Rows[e.RowIndex].FindControl("txtBunkarQty")).Text;

        string lblSrNo = ((Label)GVBunkarCarpetReceive.Rows[e.RowIndex].FindControl("lblSrNo")).Text;
        string lblItemId = ((Label)GVBunkarCarpetReceive.Rows[e.RowIndex].FindControl("lblItemId")).Text;
        string lblQualityId = ((Label)GVBunkarCarpetReceive.Rows[e.RowIndex].FindControl("lblQualityId")).Text;
        string Process_Rec_Detail_ID = ((Label)GVBunkarCarpetReceive.Rows[e.RowIndex].FindControl("lblProcess_Rec_Detail_ID")).Text;
        string hnBalQty = ((Label)GVBunkarCarpetReceive.Rows[e.RowIndex].FindControl("hnBalQty")).Text;

        int rowindex = e.RowIndex;

        DataTable dt3 = new DataTable();
        dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

        if (dt3.Rows.Count > 0)
        {
            dt3.Rows.Remove(dt3.Rows[rowindex]);
            dt3.AcceptChanges();
            ViewState["BunkarCarpetReceive"] = dt3;
        }

        if (dt3.Rows.Count <= 0)
        {
            ViewState["CarpetReceivePen"] = null;
        }

        GVBunkarCarpetReceive.DataSource = ViewState["BunkarCarpetReceive"];
        GVBunkarCarpetReceive.DataBind();

        //txtTotalWeight.Text = Convert.ToString(Convert.ToDouble(txtTotalWeight.Text == "" ? "0" : txtTotalWeight.Text) - Convert.ToDouble(lblFinalWt));

        CarpetPenalityDel(Convert.ToInt32(lblSrNo));

        BQty(Convert.ToInt32(Process_Rec_Detail_ID), 0);
    }
    protected void GVPenalty_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox chkoutItem = ((CheckBox)e.Row.FindControl("Chkboxitem"));
            TextBox txtQty = ((TextBox)e.Row.FindControl("txtQty"));
            TextBox txtAmt = ((TextBox)e.Row.FindControl("txtAmt"));
            //Label lblAmt = ((Label)e.Row.FindControl("lblAmt"));

            Label lblRate = ((Label)e.Row.FindControl("lblRate"));
            Label lblPenalityName = ((Label)e.Row.FindControl("lblPenalityName"));
            Label lblPenalityId = ((Label)e.Row.FindControl("lblPenalityId"));
            Label lblPenalityType = ((Label)e.Row.FindControl("lblPenalityType"));

            if (lblPenalityId.Text == "0")
            {
                //GVPenalty.Rows[currentRow.RowIndex].Enabled = false;
                txtQty.Enabled = false;
                txtQty.Text = "0";
                txtAmt.Enabled = true;
                //txtAmt.Text = "0";
                //cb.Checked = false;
            }
            else
            {
                //GVPenalty.Rows[currentRow.RowIndex].Enabled = true;
                txtAmt.Enabled = false;
            }

            if (ViewState["BunkarCarpetReceivePen"] != null)
            {
                DataTable dt = (DataTable)ViewState["BunkarCarpetReceivePen"];

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (ViewState["SrNo"].ToString() == dt.Rows[i]["SrNo"].ToString() && lblPenalityId.Text == dt.Rows[i]["PenalityId"].ToString())
                    {
                        chkoutItem.Checked = true;
                        txtQty.Text = dt.Rows[i]["Qty"].ToString();
                        txtAmt.Text = dt.Rows[i]["Amt"].ToString();

                        //DataView dv = new DataView(dt);
                        //dv.RowFilter = "SrNo =" + ViewState["SrNo"].ToString();
                        //GVPenalty.DataSource = dv;
                        //GVPenalty.DataBind();

                    }
                }
            }
        }

    }

    protected void popup_Click(object sender, EventArgs e)
    {
        ViewState["RQty"] = "";
        ViewState["Area"] = "";
        ViewState["SrNo"] = "";

        // Show ModalPopUpExtender.
        ModalPopupExtender1.Show();

        GridViewRow gvRow = (GridViewRow)(sender as Control).Parent.Parent;
        int index = gvRow.RowIndex;
        LinkButton lnk = (LinkButton)gvRow.FindControl("popup");
        string myScript = lnk.Text;

        Label lblSrNo = (Label)gvRow.FindControl("lblSrNo");
        Label lblQualityId = (Label)gvRow.FindControl("lblQualityId");
        TextBox txtBunkarQty = (TextBox)gvRow.FindControl("txtBunkarQty");
        Label lblArea = (Label)gvRow.FindControl("lblArea");
        Label lblItemId = (Label)gvRow.FindControl("lblItemId");

        int Qualityid = Convert.ToInt32(lblQualityId.Text);
        int ItemId = Convert.ToInt32(lblItemId.Text);
        ViewState["RQty"] = txtBunkarQty.Text;
        ViewState["Area"] = lblArea.Text;
        ViewState["SrNo"] = lblSrNo.Text;
        FillPenalityGrid(ItemId);
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        ModalPopupExtender1.Hide();
        name = "";
        TotalPenalityAmt = 0;

        DataTable dtrecordsPen = new DataTable();

        if (ViewState["BunkarCarpetReceivePen"] == null)
        {
            //DataTable dtrecords = new DataTable();
            dtrecordsPen.Columns.Add("SrNo", typeof(int));
            dtrecordsPen.Columns.Add("PenalityId", typeof(int));
            dtrecordsPen.Columns.Add("PenalityName", typeof(string));
            dtrecordsPen.Columns.Add("Qty", typeof(int));
            dtrecordsPen.Columns.Add("Rate", typeof(float));
            dtrecordsPen.Columns.Add("Amt", typeof(float));
            dtrecordsPen.Columns.Add("PenalityType", typeof(string));

            ViewState["BunkarCarpetReceivePen"] = dtrecordsPen;
        }
        else
        {
            dtrecordsPen = (DataTable)ViewState["BunkarCarpetReceivePen"];
        }

        for (int i = 0; i < GVPenalty.Rows.Count; i++)
        {
            CheckBox chkoutItem = ((CheckBox)GVPenalty.Rows[i].FindControl("Chkboxitem"));
            TextBox txtQty = ((TextBox)GVPenalty.Rows[i].FindControl("txtQty"));
            TextBox txtAmt = ((TextBox)GVPenalty.Rows[i].FindControl("txtAmt"));
            //Label lblAmt = ((Label)GVPenalty.Rows[i].FindControl("lblAmt"));

            if ((chkoutItem.Checked == true) && (Convert.ToDouble(txtQty.Text == "" ? "0" : txtQty.Text) >= 0) && Convert.ToDouble(txtAmt.Text == "" ? "0" : txtAmt.Text) > 0)
            {
                Label lblRate = ((Label)GVPenalty.Rows[i].FindControl("lblRate"));
                Label lblPenalityName = ((Label)GVPenalty.Rows[i].FindControl("lblPenalityName"));
                Label lblPenalityId = ((Label)GVPenalty.Rows[i].FindControl("lblPenalityId"));
                Label lblPenalityType = ((Label)GVPenalty.Rows[i].FindControl("lblPenalityType"));

                int m = 0;
                for (int n = 0; n < dtrecordsPen.Rows.Count; n++)
                {
                    if (dtrecordsPen.Rows[n]["SrNo"].ToString() == ViewState["SrNo"].ToString() && dtrecordsPen.Rows[n]["PenalityId"].ToString() == lblPenalityId.Text)
                    {
                        m = 1;
                        dtrecordsPen.Rows.RemoveAt(n);
                        dtrecordsPen.AcceptChanges();
                        break;
                    }
                }

                DataRow dr = dtrecordsPen.NewRow();
                dr["SrNo"] = ViewState["SrNo"];
                dr["PenalityId"] = lblPenalityId.Text;
                dr["PenalityName"] = lblPenalityName.Text;
                dr["Qty"] = txtQty.Text;
                dr["Rate"] = lblRate.Text == "" ? "0" : lblRate.Text;
                dr["Amt"] = txtAmt.Text == "" ? "0" : txtAmt.Text;
                dr["PenalityType"] = lblPenalityType.Text;

                dtrecordsPen.Rows.Add(dr);
                ViewState["BunkarCarpetReceivePen"] = dtrecordsPen;
                name += lblPenalityName.Text + " " + txtQty.Text + "-" + txtAmt.Text + ",";
                TotalPenalityAmt = TotalPenalityAmt + Convert.ToDecimal(txtAmt.Text);
                //name += lblPenalityId.Text + "_" + lblPenalityName.Text + " " + txtQty.Text + "-" + txtAmt.Text + ",";
            }
        }

        DataTable dt3 = new DataTable();
        dt3 = (DataTable)ViewState["BunkarCarpetReceive"];

        if (dt3.Rows.Count > 0)
        {
            foreach (DataRow row2 in dt3.Rows)
            {
                if (row2["SrNo"].ToString() == ViewState["SrNo"].ToString())
                {
                    row2["Penality"] = name;
                    row2["PenalityAmount"] = TotalPenalityAmt;
                }
                dt3.AcceptChanges();
                row2.SetModified();
                ViewState["BunkarCarpetReceive"] = dt3;
            }
        }

        for (int i = 0; i < GVBunkarCarpetReceive.Rows.Count; i++)
        {
            Label lblPenalityName = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblPenalityName"));
            Label lblSrNo = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblSrNo"));
            Label lblPenalityAmount = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblPenalityAmount"));

            Label lblPaidAmount = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblPaidAmount"));
            Label lblWAmount = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblWAmount"));
            Label lblWeightPenality = ((Label)GVBunkarCarpetReceive.Rows[i].FindControl("lblWeightPenality"));


            if (lblSrNo.Text == ViewState["SrNo"].ToString())
            {
                lblPenalityName.Text = name;
                lblPenalityName.ToolTip = name;
                lblPenalityAmount.Text = Convert.ToString(Math.Round(TotalPenalityAmt, 4));
                lblPaidAmount.Text = Convert.ToString(Convert.ToDecimal(lblWAmount.Text) - Convert.ToDecimal(lblPenalityAmount.Text) - Convert.ToDecimal(lblWeightPenality.Text));
            }
        }

        //GVCarpetReceive.DataSource = ViewState["CarpetReceive"];
        //GVCarpetReceive.DataBind();        

    }
    protected void CarpetPenalityDel(int SrNo)
    {
        if (ViewState["BunkarCarpetReceivePen"] != null)
        {
            DataTable dtrecordsPen = (DataTable)ViewState["BunkarCarpetReceivePen"];

            DataView dv = new DataView(dtrecordsPen);
            dv.RowFilter = "SrNo not in('" + SrNo + "')";
            ViewState["BunkarCarpetReceivePen"] = dv.ToTable();
        }
    }
    protected void Chkboxitem_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chk = (CheckBox)sender;
        GridViewRow gr = (GridViewRow)chk.Parent.Parent;
        CheckBox cb = (CheckBox)gr.FindControl("Chkboxitem");

        TextBox txtQty = (TextBox)gr.FindControl("txtQty");
        TextBox txtAmt = (TextBox)gr.FindControl("txtAmt");
        //Label lblAmt = (Label)gr.FindControl("lblAmt");
        Label lblPenalityType = (Label)gr.FindControl("lblPenalityType");
        Label lblPenalityId = (Label)gr.FindControl("lblPenalityId");
        Label lblRate = (Label)gr.FindControl("lblRate");

        if (cb.Checked == false)
        {
            txtQty.Text = "0";
            txtAmt.Text = "0";

            if (ViewState["BunkarCarpetReceivePen"] != null)
            {
                DataTable dt4 = new DataTable();
                dt4 = (DataTable)ViewState["BunkarCarpetReceivePen"];

                int m = 0;
                for (int n = 0; n < dt4.Rows.Count; n++)
                {
                    if (dt4.Rows[n]["SrNo"].ToString() == ViewState["SrNo"].ToString() && dt4.Rows[n]["PenalityId"].ToString() == lblPenalityId.Text)
                    {
                        m = 1;

                        dt4.Rows.RemoveAt(n);
                        dt4.AcceptChanges();
                        break;
                    }
                }
            }
        }

        ModalPopupExtender1.Show();
    }
    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        if (GVPenalty.Rows.Count > 0)
        {
            GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;
            //int id =Convert.ToInt32(currentRow);

            String lblPenalityId = ((Label)currentRow.FindControl("lblPenalityId")).Text;

            TextBox txtQty = (TextBox)currentRow.FindControl("txtQty");
            // Label lblAmt = (Label)currentRow.FindControl("lblAmt");
            TextBox txtAmt = (TextBox)currentRow.FindControl("txtAmt");
            Label lblPenalityType = (Label)currentRow.FindControl("lblPenalityType");
            Label lblRate = (Label)currentRow.FindControl("lblRate");

            CheckBox cb = (CheckBox)currentRow.FindControl("Chkboxitem");

            if (lblPenalityId == "0")
            {
                //GVPenalty.Rows[currentRow.RowIndex].Enabled = false;
                txtQty.Enabled = false;
                txtQty.Text = "0";
                txtAmt.Enabled = true;
                //txtAmt.Text = "0";
                //cb.Checked = false;
            }
            else
            {
                //GVPenalty.Rows[currentRow.RowIndex].Enabled = true;
                txtAmt.Enabled = false;
            }

            if (cb.Checked == false)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('Plz Check First Penality !!');", true);
                txtQty.Text = "0";
            }
            if (Convert.ToInt32(txtQty.Text) > Convert.ToInt32(ViewState["RQty"]))
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('Sorry You Can't Enter More Than Receive Quantity !!');", true);
                txtQty.Text = "0";
                //lblAmt.Text = "0";
                txtAmt.Text = "0";
            }
            else
            {
                if (Convert.ToInt32(lblPenalityId) > 0)
                {
                    if (lblPenalityType.Text == "A")
                    {
                        double TAmt = Convert.ToDouble(txtQty.Text) * Convert.ToDouble(lblRate.Text);
                        //lblAmt.Text = Convert.ToString(TAmt);
                        txtAmt.Text = Convert.ToString(TAmt);
                    }
                    else
                    {
                        double TAmt = Convert.ToDouble(ViewState["Area"]) * Convert.ToDouble(txtQty.Text) * Convert.ToDouble(lblRate.Text);
                        // lblAmt.Text = Convert.ToString(TAmt);
                        txtAmt.Text = Convert.ToString(TAmt);
                    }
                }
            }

            ModalPopupExtender1.Show();
        }
    }
    protected void txtAmt_TextChanged(object sender, EventArgs e)
    {
        GridViewRow currentRow = (GridViewRow)((TextBox)sender).Parent.Parent;
        //int id =Convert.ToInt32(currentRow);

        String lblPenalityId = ((Label)currentRow.FindControl("lblPenalityId")).Text;

        TextBox txtQty = (TextBox)currentRow.FindControl("txtQty");
        // Label lblAmt = (Label)currentRow.FindControl("lblAmt");
        TextBox txtAmt = (TextBox)currentRow.FindControl("txtAmt");
        Label lblPenalityType = (Label)currentRow.FindControl("lblPenalityType");
        Label lblRate = (Label)currentRow.FindControl("lblRate");

        CheckBox cb = (CheckBox)currentRow.FindControl("Chkboxitem");

        if (cb.Checked == true)
        {
            if (ViewState["BunkarCarpetReceivePen"] != null)
            {
                DataTable dt4 = new DataTable();
                dt4 = (DataTable)ViewState["BunkarCarpetReceivePen"];

                int m = 0;
                for (int n = 0; n < dt4.Rows.Count; n++)
                {
                    if (dt4.Rows[n]["SrNo"].ToString() == ViewState["SrNo"].ToString() && dt4.Rows[n]["PenalityId"].ToString() == lblPenalityId)
                    {
                        m = 1;
                        dt4.Rows[n]["Amt"] = txtAmt.Text;

                        dt4.AcceptChanges();
                        ViewState["BunkarCarpetReceivePen"] = dt4;
                    }
                }
            }
        }
        ModalPopupExtender1.Show();
    }
    protected void FillPenalityGrid(int ItemId)
    {
        string sql = "";
        if (variable.VarNewQualitySize == "1")
        {
            sql = @"select PenalityId,PenalityName,rate,PenalityType,QualityId from PenalityMaster where (Qualityid=" + ItemId + " or Qualityid=0) and PenalityWF='W' And PenalityId<>-1 ";
        }
        sql = sql + "Order By PenalityName Desc";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, sql);
        if (ds.Tables[0].Rows.Count > 0)
        {
            GVPenalty.DataSource = ds;
            GVPenalty.DataBind();
            GVPenalty.Visible = true;
        }
        else
        {
            GVPenalty.Visible = false;
            ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true);
        }
    }
    protected void BtnSave_Click(object sender, EventArgs e)
    {
        CHECKVALIDCONTROL();
        if (llMessageBox.Text == "")
        {
            ProcessIssue();
        }
    }
    private void CHECKVALIDCONTROL()
    {
        llMessageBox.Text = "";
        if (UtilityModule.VALIDDROPDOWNLIST(DDCompanyName) == false)
        {
            goto a;
        }
        if (UtilityModule.VALIDDROPDOWNLIST(DDContractorName) == false)
        {
            goto a;
        }
        if (UtilityModule.VALIDDROPDOWNLIST(DDItemName) == false)
        {
            goto a;
        }
        //if (UtilityModule.VALIDTEXTBOX(TxtRecDate) == false)
        //{
        //    goto a;
        //}

        else
        {
            goto B;
        }
    a:
        llMessageBox.Visible = true;
        UtilityModule.SHOWMSG(llMessageBox);
    B: ;
    }
    private void ChkGVDDValidation()
    {
        llMessageBox.Text = "";
        int count = GVBunkarCarpetReceive.Rows.Count;

        if (GVBunkarCarpetReceive.Rows.Count > 0)
        {
            for (int k = 0; k < count; k++)
            {
                int Qty = Convert.ToInt32(((TextBox)(GVBunkarCarpetReceive.Rows[k].FindControl("txtBunkarQty"))).Text);
                TextBox TxtRecDate = ((TextBox)GVBunkarCarpetReceive.Rows[k].FindControl("TxtRecDate"));

                if (Qty <= 0 || Qty == 0)
                {
                    llMessageBox.Visible = true;
                    llMessageBox.Text = "Please fill qty";
                }
                else if (TxtRecDate.Text == "")
                {
                    llMessageBox.Visible = true;
                    llMessageBox.Text = "Please select Receive Date";
                }
            }
        }
    }
    //*********************************************Process Issue**************************************************************************
    private void ProcessIssue()
    {
        string HKStrWL = "";
        string RoundFullArea = "";
        double BZW = 0, BZL = 0;
        ChkGVDDValidation();
        if (llMessageBox.Text == "")
        {
            SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
            con.Open();

            DataTable dtrecords = new DataTable();
            if (ViewState["BunkarCarpetReceive"] != null)
            {
                dtrecords = (DataTable)ViewState["BunkarCarpetReceive"];
            }

            dtrecords = (DataTable)ViewState["BunkarCarpetReceive"];

            DataTable dtrecordsPenSave = new DataTable();
            if (ViewState["BunkarCarpetReceivePen"] != null)
            {
                dtrecordsPenSave = (DataTable)ViewState["BunkarCarpetReceivePen"];
            }
            else if (ViewState["BunkarCarpetReceivePen"] == null)
            {
                dtrecordsPenSave.Columns.Add("SrNo", typeof(int));
                dtrecordsPenSave.Columns.Add("PenalityId", typeof(int));
                dtrecordsPenSave.Columns.Add("PenalityName", typeof(string));
                dtrecordsPenSave.Columns.Add("Qty", typeof(int));
                dtrecordsPenSave.Columns.Add("Rate", typeof(float));
                dtrecordsPenSave.Columns.Add("Amt", typeof(float));
                dtrecordsPenSave.Columns.Add("PenalityType", typeof(string));

                DataRow dr = dtrecordsPenSave.NewRow();
                dr["SrNo"] = 0;
                dr["PenalityId"] = 0;
                dr["PenalityName"] = "";
                dr["Qty"] = 0;
                dr["Rate"] = 0;
                dr["Amt"] = 0;
                dr["PenalityType"] = null;

                dtrecordsPenSave.Rows.Add(dr);
                ViewState["BunkarCarpetReceivePen"] = dtrecordsPenSave;
            }


            if (dtrecords.Rows.Count > 0)
            {
                SqlTransaction Tran = con.BeginTransaction();
                try
                {

                    SqlParameter[] _arrpara = new SqlParameter[16];

                    if (ViewState["ID"] == null)
                    {
                        ViewState["ID"] = 0;
                    }
                    _arrpara[0] = new SqlParameter("@ID", SqlDbType.Int);
                    _arrpara[0].Direction = ParameterDirection.InputOutput;
                    _arrpara[0].Value = ViewState["ID"];
                    _arrpara[1] = new SqlParameter("@CompanyId", DDCompanyName.SelectedValue);
                    _arrpara[2] = new SqlParameter("@Item_Id", DDItemName.SelectedValue);
                    _arrpara[3] = new SqlParameter("@ContractorId", DDContractorName.SelectedValue);
                    _arrpara[4] = new SqlParameter("@BunkarId", DDBunkarName.SelectedValue);
                    _arrpara[5] = new SqlParameter("@MonthId", DDMonth.SelectedValue);
                    _arrpara[6] = new SqlParameter("@Year", DDYear.SelectedValue);
                    _arrpara[7] = new SqlParameter("@PaymentDate", DateTime.Now.ToShortDateString());
                    _arrpara[8] = new SqlParameter("@FinalFlag", "0");
                    _arrpara[9] = new SqlParameter("@ChallanNo", SqlDbType.VarChar, 50);
                    _arrpara[9].Direction = ParameterDirection.InputOutput;
                    _arrpara[9].Value = TxtChallanNo.Text;
                    _arrpara[10] = new SqlParameter("@DetailId", SqlDbType.Int);
                    _arrpara[10].Direction = ParameterDirection.Output;
                    _arrpara[10].Value = 0;
                    _arrpara[11] = new SqlParameter("@UserId", Session["varUserId"]);
                    _arrpara[12] = new SqlParameter("@SaveFlag", "0");
                    _arrpara[13] = new SqlParameter("@dtrecords", dtrecords);
                    _arrpara[14] = new SqlParameter("@dtrecordsPen", dtrecordsPenSave);
                    _arrpara[15] = new SqlParameter("@Msgflag", SqlDbType.VarChar, 100);
                    _arrpara[15].Direction = ParameterDirection.Output;

                    SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "[Pro_SaveBunkarPayment]", _arrpara);

                    llMessageBox.Visible = true;
                    llMessageBox.Text = _arrpara[15].Value.ToString();
                    //ViewState["PurchaseReceiveDetailId"] = _arrpara[8].Value;
                    llMessageBox.Text = "Data Successfully Saved.......";
                    ViewState["ID"] = 0;
                    //lblChallanNo.Text = _arrpara[5].Value.ToString();
                    //lblProcessRecID.Text = _arrpara[0].Value.ToString();
                    Tran.Commit();

                    TxtChallanNo.Text = _arrpara[9].Value.ToString();

                    ClearAfterSave();
                    FillBunkarPaymentDetail();

                }
                catch (Exception ex)
                {
                    UtilityModule.MessageAlert(ex.Message, "Master/Hissab/FrmBunkarPayment.aspx");
                    Tran.Rollback();
                    ViewState["ID"] = 0;
                    llMessageBox.Visible = true;
                    llMessageBox.Text = ex.Message;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
    }
    private void ClearAfterSave()
    {

        //DDEmployeeNamee.Focus();
        //fillorderdetail();
        GVBunkarCarpetReceive.DataSource = null;
        GVBunkarCarpetReceive.DataBind();

        GVPenalty.DataSource = null;
        GVPenalty.DataBind();

        ViewState["BunkarCarpetReceive"] = null;
        ViewState["BunkarCarpetReceivePen"] = null;

    }
    public SortDirection dir
    {
        get
        {
            if (ViewState["dirState"] == null)
            {
                ViewState["dirState"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["dirState"];
        }
        set
        {
            ViewState["dirState"] = value;
        }

    }
    protected void GVBazaarDetail_Sorting(object sender, GridViewSortEventArgs e)
    {
        DataSet ddd = new DataSet();
        ddd = FillBazaarDetail();
        DataTable dt = ddd.Tables[0];
        string sortingDirection = string.Empty;
        if (dir == SortDirection.Ascending)
        {
            dir = SortDirection.Descending;
            sortingDirection = "Desc";
        }
        else
        {
            dir = SortDirection.Ascending;
            sortingDirection = "Asc";
        }
        DataView sortedView = new DataView(dt);
        sortedView.Sort = e.SortExpression + " " + sortingDirection;
        //Session["objects"] = sortedView;
        GVBazaarDetail.DataSource = sortedView;
        GVBazaarDetail.DataBind();
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        // this is required for avoid error (control must be placed inside form tag)
    }

}