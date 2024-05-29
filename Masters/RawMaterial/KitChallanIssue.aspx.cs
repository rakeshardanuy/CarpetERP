﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public partial class Masters_process_KitChallanIssue : System.Web.UI.Page
{
    static int MasterCompanyId;
    protected void Page_Load(object sender, EventArgs e)
    {
        MasterCompanyId = Convert.ToInt16(Session["varCompanyId"]);
        if (Session["varCompanyId"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        DataSet DSQ = null; string Qry = "";
        if (!IsPostBack)
        {
            ViewState["Prmid"] = 0;
            Qry = @" select Distinct CI.CompanyId,Companyname from Companyinfo CI,Company_Authentication CA Where CA.CompanyId=CI.CompanyId And CA.UserId=" + Session["varuserId"] + " And CI.MasterCompanyId=" + Session["varCompanyId"] + @" Order By Companyname
                     select PNM.PROCESS_NAME_ID,PNM.PROCESS_NAME From PROCESS_NAME_MASTER PNM inner join UserRightsProcess URP on PNM.PROCESS_NAME_ID=URP.ProcessId and URP.Userid=" + Session["varuserid"] + @"
                     WHere PNM.ProcessType=1  order by PROCESS_NAME
                     Select VarProdCode,VarCompanyNo From MasterSetting 
                     Select ID, BranchName 
                        From BRANCHMASTER BM(nolock) 
                        JOIN BranchUser BU(nolock) ON BU.BranchID = BM.ID And BU.UserID = " + Session["varuserId"] + @" 
                        Where BM.CompanyID = " + Session["CurrentWorkingCompanyID"] + " And BM.MasterCompanyID = " + Session["varCompanyId"] + @"
                    Select ConeType, ConeType From ConeMaster Order By SrNo ";

            DSQ = SqlHelper.ExecuteDataset(Qry);
            UtilityModule.ConditionalComboFillWithDS(ref ddCompName, DSQ, 0, true, "--Select--");
            if (ddCompName.Items.Count > 0)
            {
                ddCompName.SelectedValue = Session["CurrentWorkingCompanyID"].ToString();
                ddCompName.Enabled = false;
            }

            UtilityModule.ConditionalComboFillWithDS(ref DDBranchName, DSQ, 3, false, "");
            DDBranchName.Enabled = false;
            if (DDBranchName.Items.Count == 0)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('Branch not define for this user!');", true);
                return;
            }

            UtilityModule.ConditionalComboFillWithDS(ref ddProcessName, DSQ, 1, true, "--Select--");
            //UtilityModule.ConditionalComboFillWithDS(ref DDconetype, DSQ, 4, false, "");

            int VarProdCode = Convert.ToInt32(DSQ.Tables[2].Rows[0]["VarProdCode"]);
            int VarCompanyId = Convert.ToInt32(DSQ.Tables[2].Rows[0]["VarCompanyNo"]);
            txtdate.Text = DateTime.Now.ToString("dd-MMM-yyyy");
            //ddCompName.SelectedIndex = 1;
            //switch (VarProdCode)
            //{
            //    case 0:
            //        procode.Visible = false;
            //        break;
            //    case 1:
            //        procode.Visible = true;
            //        break;
            //}
            lablechange();
            //Fill_Temp_OrderNo();
            switch (Session["varcompanyid"].ToString())
            {
               
            }
            //*************
            if (variable.Carpetcompany == "1")
            {
                TRempcodescan.Visible = true;
            }
            if (variable.VarBINNOWISE == "1")
            {
                //TDBinNo.Visible = true;
            }
            if (MySession.TagNowise == "1")
            {
               // TDTagno.Visible = true;
            }

            if (variable.VarCompanyWiseChallanNoGenerated == "1")
            {
                //txtchalanno.Enabled = false;
            }
        }
    }
    public void lablechange()
    {
        String[] ParameterList = new String[8];
        ParameterList = UtilityModule.ParameteLabel(Convert.ToInt32(Session["varCompanyId"]));
        //lblqualityname.Text = ParameterList[0];
        //lbldesignname.Text = ParameterList[1];
        //lblcolorname.Text = ParameterList[2];
        //lblshapename.Text = ParameterList[3];
        //LblSize.Text = ParameterList[4];
        //lblcategoryname.Text = ParameterList[5];
        //lblitemname.Text = ParameterList[6];
        //lblshadecolor.Text = ParameterList[7];
    }
    protected void ddProcessName_SelectedIndexChanged(object sender, EventArgs e)
    {
        ProcessNameSelectedIndexChange();
    }
    private void ProcessNameSelectedIndexChange()
    {
        ViewState["Prmid"] = 0;
        //Fill_Grid();
        string str = "";
        if (variable.VarFinishingNewModuleWise == "1" && ddProcessName.SelectedValue != "1")
        {
            str = @"select EI.EmpId,EI.EmpName+' '+case when isnull(EI.empcode,'')='' Then '' ELse '['+ EI.empcode+']' End 
                    From Empinfo EI 
                    join EmpProcess EMP on EI.EmpId=EMP.EmpId Where EMP.Processid=" + ddProcessName.SelectedValue;
        }
        else
        {
            str = @"SELECT distinct e.EmpId, e.EmpName  
                        FROM  PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + @" PIM 
                                            JOIN  EmpInfo e ON pim.Empid = e.EmpId And e.MasterCompanyId = " + Session["varCompanyId"] + @" 
                        UNION 
                        Select EI.EmpId, EI.EmpName 
                        From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + @" PIM(Nolock) 
                        JOIN Employee_ProcessOrderNo EPO(Nolock) ON EPO.IssueOrderId = PIM.IssueOrderId And ProcessID = " + ddProcessName.SelectedValue + @" 
                        JOIN EmpInfo EI(Nolock) ON EI.EmpID = EPO.Empid ";
            //if (Session["varcompanyid"].ToString() == "28")
            //{
            //    str = str + " Where PIM.FlagStockNoAttachWithoutRawMaterialIssue = 0";
            //}
            //else
            //{
              //  str = str + " Where PIM.FlagStockNoAttachWithoutRawMaterialIssue = 1";
            //}
            str = str + " Order By EmpName";
        }
        UtilityModule.ConditionalComboFill(ref ddempname, str, true, "--Select--");
    }
    protected void ddempname_SelectedIndexChanged(object sender, EventArgs e)
    {
       // txtchalanno.Text = "";
        EmpNameSelectedIndexChange();
    }
    private void EmpNameSelectedIndexChange()
    {
        ViewState["Prmid"] = 0;
        //Fill_Grid();
        string str = "";
        if (Session["varcompanyId"].ToString() == "9")
        {
            UtilityModule.ConditionalComboFill(ref ddOrderNo, @"select distinct PM.issueorderid,om.localOrder+'/'+cast(PM.ChallanNo as varchar(100)) as IssueOrderid1 
            From process_issue_master_" + ddProcessName.SelectedValue + @" PM 
            Join Process_issue_detail_" + ddProcessName.SelectedValue + @" PD on PM.issueOrderid=PD.issueOrderid 
            Join ordermaster OM on OM.orderid=PD.orderid 
            Where PM.empid=" + ddempname.SelectedValue + " And PM.CompanyId=" + ddCompName.SelectedValue + " and isnull(SampleNumber,'')='' order by issueorderid1", true, "Select Order No");
        }
        else
        {
            str = @"select Distinct PIM.IssueOrderId,Pim.ChallanNo as Issueorderid1 
                From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + @" PIM 
                join EmpInfo ei on pim.Empid=ei.EmpId 
                WHERE --IsNull(PIM.DEPARTMENTTYPE, 0) = 0 And 
                PIm.Companyid=" + ddCompName.SelectedValue + " and PIM.Empid=" + ddempname.SelectedValue + " and isnull(pim.FOLIOSTATUS,0)=0 and isnull(PIM.samplenumber,'')=''";

            if (ChkForCompleteStatus.Checked == true)
            {
                str = str + @" And PIM.Status = 'Complete'";
            }
            else
            {
                str = str + @" And PIM.Status <> 'Complete'";
            }
            str = str + " UNION  ";
            str = str + @" select Distinct pim.IssueOrderId,pim.ChallanNo as Issueorderid1 
            From Process_issue_Master_" + ddProcessName.SelectedValue + @" pim 
            join employee_processorderno emp on pim.issueorderid=emp.issueorderid and emp.ProcessId=" + ddProcessName.SelectedValue + @" And pim.Empid=0 
            Where --IsNull(PIM.DEPARTMENTTYPE, 0) = 0 And 
            PIm.Companyid=" + ddCompName.SelectedValue + " and EMP.Empid=" + ddempname.SelectedValue + " and isnull(pim.FOLIOSTATUS,0)=0 and isnull(PIM.samplenumber,'')=''";
            
            if (ChkForCompleteStatus.Checked == true)
            {
                str = str + @" And PIM.Status = 'Complete'";
            }
            else
            {
                str = str + @" And PIM.Status <> 'Complete'";
            }
            UtilityModule.ConditionalComboFill(ref ddOrderNo, str, true, "Select order no");
        }
    }
    protected void ddOrderNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        OrderNoSelectedIndexChange();
    }
    protected void DDItemDesignName_SelectedIndexChanged(object sender, EventArgs e)
    {
        ItemDesignNameSelectedChanged();
    }
    private void ItemDesignNameSelectedChanged()
    {
        SqlParameter[] parparam = new SqlParameter[4];
        parparam[0] = new SqlParameter("@ISSUEORDERID", ddOrderNo.SelectedValue);
        parparam[1] = new SqlParameter("@FLAGSTOCKAVGORNOT", SqlDbType.Int);
        parparam[1].Direction = ParameterDirection.Output;
        parparam[2] = new SqlParameter("@ITEMDESCRIPTION", SqlDbType.NVarChar, 4000);
        parparam[2].Direction = ParameterDirection.Output;
        parparam[3] = new SqlParameter("@DesignID", DDItemDesignName.SelectedValue);

        SqlHelper.ExecuteNonQuery(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "[PRO_CHECKFOLIOSTOCKCONSMPQTY]", parparam);
        if (parparam[1].Value.ToString() == "1")
        {
            string Str = "50% Stock Not available " + parparam[2].Value.ToString();

            ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('" + Str + "');", true);
            DDItemDesignName.SelectedIndex = 0;
            return;
        }
    }
    private void OrderNoSelectedIndexChange()
    {
        if (Session["varcompanyid"].ToString() == "16" || Session["varcompanyid"].ToString() == "28")
        {
            TdDDItemDesignName.Visible = true;

            UtilityModule.ConditionalComboFill(ref DDItemDesignName, @"Select Distinct VF.DesignID, VF.DesignName 
            From PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PID(Nolock) 
            JOIN V_FinishedItemDetail VF(Nolock) ON VF.ITEM_FINISHED_ID = PID.Item_Finished_Id 
            Where PID.IssueOrderId = " + ddOrderNo.SelectedValue + @" 
            Order BY VF.DesignName ", true, "Select Design Name");
        }

        ViewState["Prmid"] = 0;
        if (ChKForEdit.Checked == false)
        {
            //Fill_Grid();
        }
        else
        {
            if (Session["varcompanyId"].ToString() == "9")
            {
                UtilityModule.ConditionalComboFill(ref DDChallanNo, @"Select PrmId,ChalanNo from ProcessRawMaster Where TypeFlag = 0 And TranType=0 And PrOrderId=" + ddOrderNo.SelectedValue + " And Processid=" + ddProcessName.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Challan No");
            }
            else if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyId"].ToString() == "28")
            {
                UtilityModule.ConditionalComboFill(ref DDChallanNo, @"Select PrmId, ChalanNo + ' / ' + REPLACE(CONVERT(NVARCHAR(11), Date, 106), ' ', '-') Challan from ProcessRawMaster Where TypeFlag = 0 And TranType=0 And PrOrderId=" + ddOrderNo.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Challan No");
            }
            else
            {
                UtilityModule.ConditionalComboFill(ref DDChallanNo, @"Select PrmId,Replace(Str(cast(PrmId as varchar))+'/'+ChalanNo ,' ','') from ProcessRawMaster Where TypeFlag = 0 And TranType=0 And PrOrderId=" + ddOrderNo.SelectedValue + " And Processid=" + ddProcessName.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Challan No");
            }

        }
        //*****sample order
//        if (hnissampleorder.Value == "1")
//        {
//            UtilityModule.ConditionalComboFill(ref ddCatagory, @"select ICM.Category_id,ICM.CATEGORY_NAME From Item_category_Master  ICM 
//                                                               inner join CategorySeparate CS on ICM.CATEGORY_ID=Cs.Categoryid and Cs.id=1 and ICM.MasterCompanyid=" + Session["varcompanyid"] + "", true, "Select Category Name");
//        }
//        else
//        {
//            UtilityModule.ConditionalComboFill(ref ddCatagory, @"SELECT distinct ITEM_CATEGORY_MASTER.CATEGORY_ID, ITEM_CATEGORY_MASTER.CATEGORY_NAME
//            FROM ITEM_PARAMETER_MASTER INNER JOIN ITEM_MASTER ON ITEM_PARAMETER_MASTER.ITEM_ID = ITEM_MASTER.ITEM_ID 
//            INNER JOIN ITEM_CATEGORY_MASTER ON ITEM_MASTER.CATEGORY_ID = ITEM_CATEGORY_MASTER.CATEGORY_ID 
//            INNER JOIN CategorySeparate CS on ITEM_CATEGORY_MASTER.Category_id=cs.categoryid and cs.id=1
//            INNER JOIN PROCESS_CONSUMPTION_DETAIL ON ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFINISHEDID
//            WHERE PROCESS_CONSUMPTION_DETAIL.ISSUEORDERID =" + ddOrderNo.SelectedValue + " and Processid=" + ddProcessName.SelectedValue + "   And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Category Name");
//        }
//        if (ddCatagory.Items.Count > 0)
//        {
//            ddCatagory.SelectedIndex = 1;
//        }
     //   Fill_Category_SelectedChange();
       // fill_Grid_ShowConsmption();
        FillCustomerCodeAndOrderNo();
    }
    private void FillCustomerCodeAndOrderNo()
    {
        if (ddProcessName.SelectedIndex > 0 && ddOrderNo.SelectedIndex > 0)
        {
            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, @"Select Distinct CI.CustomerCode + ' / ' + OM.CustomerOrderNo + ', ' 
                    From PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PID(Nolock) 
                    JOIN OrderMaster OM(Nolock) ON OM.OrderID = PID.OrderID 
                    JOIN Customerinfo CI(Nolock) ON CI.CustomerId = OM.CustomerId 
                    Where PID.IssueOrderId = " + ddOrderNo.SelectedValue + @" 
                    For XML Path('')");
            if (ds.Tables[0].Rows.Count > 0)
            {
                LblCustomerCodeAndOrderNo.Text = ds.Tables[0].Rows[0][0].ToString();
            }
        }
    }
    protected void ChKForEdit_CheckedChanged(object sender, EventArgs e)
    {
        EditCheckedChanged();

        if (Session["usertype"].ToString() != "1" && variable.VarOnlyPreviewButtonShowOnAllEditForm == "1")
        {
            btnsave.Visible = false;
            //BtnUpdateRemark.Visible = false;
            gvdetail.Columns[8].Visible = false;

        }
    }
    private void EditCheckedChanged()
    {
        if (ChKForEdit.Checked == true)
        {
            if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyId"].ToString() == "28")
            {
                TDForCompleteStatus.Visible = true;
            }
            Td7.Visible = true;
            if (ddOrderNo.Items.Count > 0)
            {
                if (Session["varcompanyId"].ToString() == "9")
                {
                    UtilityModule.ConditionalComboFill(ref DDChallanNo, @"Select PrmId,ChalanNo 
                    from ProcessRawMaster 
                    Where TypeFlag = 0 And TranType=0 And PrOrderId=" + ddOrderNo.SelectedValue + " And CompanyID = " + ddCompName.SelectedValue + " And BranchID = " + DDBranchName.SelectedValue + @" And 
                    MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Challan No");
                }
                else if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyId"].ToString() == "28")
                {
                    TDForCompleteStatus.Visible = true;
                    UtilityModule.ConditionalComboFill(ref DDChallanNo, @"Select PrmId, ChalanNo + ' / ' + REPLACE(CONVERT(NVARCHAR(11), Date, 106), ' ', '-') Challan 
                    from ProcessRawMaster 
                    Where TypeFlag = 0 And TranType=0 And CompanyID = " + ddCompName.SelectedValue + " And BranchID = " + DDBranchName.SelectedValue + @" And 
                    PrOrderId=" + ddOrderNo.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"], true, "Select Challan No");
                }
                else
                {
                    UtilityModule.ConditionalComboFill(ref DDChallanNo, @"Select PrmId,Replace(Str(PrmId)+' /'+Str(ChalanNo),' ','') 
                    from ProcessRawMaster 
                    Where TypeFlag = 0 And TranType=0 And PrOrderId=" + ddOrderNo.SelectedValue + " And CompanyID = " + ddCompName.SelectedValue + @" And 
                    BranchID = " + DDBranchName.SelectedValue + @" And MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Challan No");
                }
            }
        }
        else
        {
            TDForCompleteStatus.Visible = false;
            Td7.Visible = false;
        }
    }
    protected void DDChallanNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        ChallanNoSelectedIndexChange();
    }
    private void ChallanNoSelectedIndexChange()
    {
        ViewState["Prmid"] = 0;
        //txtchalanno.Text = "";
        if (DDChallanNo.SelectedIndex > 0)
        {
            ViewState["Prmid"] = DDChallanNo.SelectedValue;

            string strsql2 = "select PRMID,ChalanNo from ProcessRawMaster PRM where PRM.TypeFlag = 0 And PRM.Prmid=" + DDChallanNo.SelectedValue + " And PRM.MasterCompanyId=" + Session["varCompanyId"];
            DataSet ds2 = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql2);

            if (ds2.Tables[0].Rows.Count > 0)
            {
                txtchalanno.Text = ds2.Tables[0].Rows[0]["ChalanNo"].ToString();
            }

            if (DDChallanNo.SelectedItem.Text.Split('/').Length > 1)
            {
                txtchalanno.Text = DDChallanNo.SelectedItem.Text.Split('/')[1];
            }
            else
            {
                txtchalanno.Text = DDChallanNo.SelectedItem.Text;
            }


            //string strsql = "select TransportName,BiltyNo,VehicleNo,Remark,isnull(EWayBillNo,'') as EWayBillNo from ProcessRawMaster PRM where PRM.TypeFlag = 0 And PRM.Prmid=" + DDChallanNo.SelectedValue + " And PRM.MasterCompanyId=" + Session["varCompanyId"];
            //DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);

            //if (ds.Tables[0].Rows.Count > 0)
            //{
                //txtTransportName.Text = ds.Tables[0].Rows[0]["TransportName"].ToString();
                //txtBiltyNo.Text = ds.Tables[0].Rows[0]["BiltyNo"].ToString();
                //txtVehicleNo.Text = ds.Tables[0].Rows[0]["VehicleNo"].ToString();
            //    txtremark.Text = ds.Tables[0].Rows[0]["Remark"].ToString();
               // txtEWayBillNo.Text = ds.Tables[0].Rows[0]["EWayBillNo"].ToString();
          //  }



        }
        Fill_Grid();
    }
    protected void ddCatagory_SelectedIndexChanged(object sender, EventArgs e)
    {
        Fill_Category_SelectedChange();
    }
    private void ddlcategorycange()
    {
        //ql.Visible = false;
        //clr.Visible = false;
        //dsn.Visible = false;
        //shp.Visible = false;
        //sz.Visible = false;
        //shd.Visible = false;
//        string strsql = "SELECT [CATEGORY_PARAMETERS_ID],[CATEGORY_ID],IPM.[PARAMETER_ID],PARAMETER_NAME " +
//                      " FROM [ITEM_CATEGORY_PARAMETERS] IPM inner join PARAMETER_MASTER PM on " +
//                      " IPM.[PARAMETER_ID]=PM.[PARAMETER_ID] where [CATEGORY_ID]=" + ddCatagory.SelectedValue + " And PM.MasterCompanyId=" + Session["varCompanyId"];
//        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);
//        if (ds.Tables[0].Rows.Count > 0)
//        {
//            foreach (DataRow dr in ds.Tables[0].Rows)
//            {
//                switch (dr["PARAMETER_ID"].ToString())
//                {
//                    case "1":
//                        ql.Visible = true;
//                        break;
//                    case "3":
//                        clr.Visible = true;
//                        if (hnissampleorder.Value == "1")
//                        {
//                            UtilityModule.ConditionalComboFill(ref ddcolor, "select Colorid,Colorname From Color Where MasterCompanyId=" + ddcolor.SelectedValue + " order by ColorName", true, "--Select Color--");
//                        }
//                        else
//                        {
//                            UtilityModule.ConditionalComboFill(ref ddcolor, @"SELECT DISTINCT dbo.color.ColorId, dbo.color.ColorName FROm dbo.ITEM_PARAMETER_MASTER INNER JOIN
//                        PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId INNER JOIN
//                        dbo.color ON dbo.ITEM_PARAMETER_MASTER.COLOR_ID = dbo.color.ColorId
//                        where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "--Select Color--");
//                        }
//                        break;
//                    case "2":
//                        dsn.Visible = true;
//                        if (hnissampleorder.Value == "1")
//                        {
//                            UtilityModule.ConditionalComboFill(ref dddesign, "select designId,designName From design  Where MasterCompanyid=" + Session["varcompanyid"] + " order by designName", true, "--Select Design--");
//                        }
//                        else
//                        {
//                            UtilityModule.ConditionalComboFill(ref dddesign, @"SELECT DISTINCT dbo.Design.designId, dbo.Design.designName  FROM dbo.ITEM_PARAMETER_MASTER INNER JOIN
//                        PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId INNER JOIN
//                        dbo.Design ON dbo.ITEM_PARAMETER_MASTER.DESIGN_ID = dbo.Design.designId
//                        where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "--Select Design--");
//                        }
//                        break;
//                    case "4":
//                        shp.Visible = true;
//                        if (hnissampleorder.Value == "1")
//                        {
//                            UtilityModule.ConditionalComboFill(ref ddshape, "select Shapeid,ShapeName From Shape Where MasterCompanyid=" + ddshape.SelectedValue + " order by ShapeId", true, "--Select Shape--");
//                        }
//                        else
//                        {
//                            UtilityModule.ConditionalComboFill(ref ddshape, @"SELECT DISTINCT dbo.Shape.ShapeId, dbo.Shape.ShapeName  FROM dbo.ITEM_PARAMETER_MASTER INNER JOIN
//                        PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId INNER JOIN
//                        dbo.Shape ON dbo.ITEM_PARAMETER_MASTER.SHAPE_ID = dbo.Shape.ShapeId
//                        where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "--Select Shape--");
//                        }
//                        if (ddshape.Items.Count > 0)
//                        {
//                            ddshape.SelectedIndex = 1;
//                        }
//                        break;
//                    case "5":
//                        sz.Visible = true;
//                        ChkForMtr.Checked = false;
//                        if (hnissampleorder.Value == "1")
//                        {
//                            UtilityModule.ConditionalComboFill(ref ddsize, "select SizeId,SizeFt From Size WHere shapeid=" + ddshape.SelectedValue + "  and MasterCompanyid=" + Session["varcompanyid"] + " ", true, "Size in Ft");
//                        }
//                        else
//                        {
//                            UtilityModule.ConditionalComboFill(ref ddsize, @"SELECT DISTINCT dbo.Size.SizeId, dbo.Size.SizeFt FROM dbo.ITEM_PARAMETER_MASTER INNER JOIN
//                        PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId INNER JOIN
//                        dbo.Size ON dbo.ITEM_PARAMETER_MASTER.SIZE_ID = dbo.Size.SizeId
//                        where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Size in Ft");
//                        }
//                        break;
//                    case "6":
//                        shd.Visible = true;
//                        break;
//                }
//            }
//        }
    }
    protected void dditemname_SelectedIndexChanged(object sender, EventArgs e)
    {
        //ItemName_SelectChange();
    }
//    private void ItemName_SelectChange()
//    {
//        if (dditemname.SelectedIndex >= 0)
//        {
//            string Qry = "";
//            if (hnissampleorder.Value == "1")
//            {
//                Qry = @" SELECT u.UnitId,u.UnitName  FROM ITEM_MASTER i INNER JOIN  Unit u ON i.UnitTypeID = u.UnitTypeID where item_id=" + dditemname.SelectedValue + " And i.MasterCompanyId=" + Session["varCompanyId"] + @"
//                         select QualityId,QualityName From Quality Where Item_Id=" + dditemname.SelectedItem + " and MasterCompanyid=" + Session["varcompanyId"] + " order by QualityName";
//            }
//            else
//            {
//                Qry = @" SELECT u.UnitId,u.UnitName  FROM ITEM_MASTER i INNER JOIN  Unit u ON i.UnitTypeID = u.UnitTypeID where item_id=" + dditemname.SelectedValue + " And i.MasterCompanyId=" + Session["varCompanyId"];
//                Qry = Qry + @" SELECT DISTINCT dbo.Quality.QualityId,dbo.Quality.QualityName
//                        FROM  dbo.ITEM_PARAMETER_MASTER INNER JOIN
//                        PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId
//                        INNER JOIN dbo.Quality ON dbo.ITEM_PARAMETER_MASTER.QUALITY_ID = dbo.Quality.QualityId
//                        where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " and Processid=" + ddProcessName.SelectedValue + " and quality.item_id=" + dditemname.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"];
//            }
//            DataSet DSQ = SqlHelper.ExecuteDataset(Qry);
//            UtilityModule.ConditionalComboFillWithDS(ref ddlunit, DSQ, 0, true, "Select Unit");
//            UtilityModule.ConditionalComboFillWithDS(ref dquality, DSQ, 1, true, "Select Quallity");
//            if (dquality.Items.Count > 0)
//            {
//                if (dquality.Items.Count == 1)
//                {
//                    dquality.SelectedIndex = 0;
//                }
//                else
//                {
//                    dquality.SelectedIndex = 1;
//                }

//                QualitySelectedIndexChange();
//                if (shd.Visible == true)
//                {

//                    if (hnissampleorder.Value == "1")
//                    {
//                        UtilityModule.ConditionalComboFill(ref ddlshade, "select ShadecolorId,ShadeColorName From shadecolor  Where MasterCompanyid=" + Session["varcompanyid"] + " order by ShadeColorName", true, "Select Shadecolor");
//                    }
//                    else
//                    {
//                        UtilityModule.ConditionalComboFill(ref ddlshade, @"SELECT DISTINCT SC.ShadecolorId,SC.ShadeColorName FROM ITEM_PARAMETER_MASTER IPM INNER JOIN
//                PROCESS_CONSUMPTION_DETAIL PCD ON IPM.ITEM_FINISHED_ID=PCD.IFinishedId INNER JOIN ShadeColor SC ON IPM.SHADECOLOR_ID=SC.ShadecolorId 
//                Where PCD.Issueorderid=" + ddOrderNo.SelectedValue + " and Processid=" + ddProcessName.SelectedValue + " And IPM.Quality_Id=" + dquality.SelectedValue + " And IPM.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Shadecolor");
//                    }

//                }
//            }
//            if (ddlunit.Items.Count > 0)
//            {
//                ddlunit.SelectedIndex = 1;
//            }
//        }
//    }
    protected void dquality_SelectedIndexChanged(object sender, EventArgs e)
    {
       // QualitySelectedIndexChange();
    }
//    private void QualitySelectedIndexChange()
//    {
//        UtilityModule.ConditionalComboFill(ref ddlshade, @"SELECT DISTINCT SC.ShadecolorId,SC.ShadeColorName FROM ITEM_PARAMETER_MASTER IPM INNER JOIN
//        PROCESS_CONSUMPTION_DETAIL PCD ON IPM.ITEM_FINISHED_ID=PCD.IFinishedId INNER JOIN ShadeColor SC ON IPM.SHADECOLOR_ID=SC.ShadecolorId 
//        Where PCD.Issueorderid=" + ddOrderNo.SelectedValue + " and Processid=" + ddProcessName.SelectedValue + " And IPM.Quality_Id=" + dquality.SelectedValue + " And IPM.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Shadecolor");
//        fill_qty();
//    }
    protected void dddesign_SelectedIndexChanged(object sender, EventArgs e)
    {
        //fill_qty();
    }
    protected void ddcolor_SelectedIndexChanged(object sender, EventArgs e)
    {
       // fill_qty();
    }
    protected void ddshape_SelectedIndexChanged(object sender, EventArgs e)
    {
       // fill_qty();
    }
    protected void ddsize_SelectedIndexChanged(object sender, EventArgs e)
    {
       // fill_qty();
    }
    protected void ddlshade_SelectedIndexChanged(object sender, EventArgs e)
    {
        //fill_qty(sender);
    }
    protected void ddgodown_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (TDBinNo.Visible == true)
        //{
        //    FillBinNo(sender);
        //}
        //else
        //{
        //    Fill_GodownSelectedChange(sender);
        //    Fill_LotNoSelectedChange();
        //}
       // Fill_GodownSelectedChange(sender);
        //Fill_LotNoSelectedChange();
    }
    //protected void FillBinNo(object sender = null)
    //{
    //    txtstock.Text = "";
    //    int color = 0;
    //    int quality = 0;
    //    int design = 0;
    //    int shape = 0;
    //    int size = 0;
    //    int shadeColor = 0;
    //    if ((ql.Visible == true && dquality.SelectedIndex > 0) || ql.Visible != true)
    //    {
    //        quality = 1;
    //    }
    //    if (dsn.Visible == true && dddesign.SelectedIndex > 0 || dsn.Visible != true)
    //    {
    //        design = 1;
    //    }
    //    if (clr.Visible == true && ddcolor.SelectedIndex > 0 || clr.Visible != true)
    //    {
    //        color = 1;
    //    }
    //    if (shp.Visible == true && ddshape.SelectedIndex > 0 || shp.Visible != true)
    //    {
    //        shape = 1;
    //    }
    //    if (sz.Visible == true && ddsize.SelectedIndex > 0 || sz.Visible != true)
    //    {
    //        size = 1;
    //    }
    //    if (shd.Visible == true && ddlshade.SelectedIndex > 0 || shd.Visible != true)
    //    {
    //        shadeColor = 1;
    //    }
    //    if (quality == 1 && design == 1 && color == 1 && shape == 1 && size == 1 && shadeColor == 1)
    //    {
    //        int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "", Convert.ToInt32(Session["varCompanyId"]));
    //        string str = "Select Distinct BinNo,BinNo from stock Where CompanyId=" + ddCompName.SelectedValue + " And Godownid=" + ddgodown.SelectedValue + "   and item_finished_id=" + Varfinishedid + " and LotNo='" + ddlotno.SelectedItem.Text + "'";
    //        if (MySession.Stockapply == "True" && ChKForEdit.Checked == false)
    //        {
    //            str = str + "  And QtyInHand>0";
    //        }
    //        if (TDTagno.Visible == true)
    //        {
    //            str = str + "  And TagNo='" + DDTagno.SelectedItem.Text + "'";
    //        }
    //        UtilityModule.ConditionalComboFill(ref DDBinNo, str, true, "--Select--");
    //        if (DDBinNo.Items.Count > 0)
    //        {
    //            DDBinNo.SelectedIndex = 1;
    //            if (sender != null)
    //            {
    //                DDBinNo_SelectedIndexChanged(sender, new EventArgs());
    //            }
    //        }
    //    }
    //}
    //protected void FillTagNo(object sender = null)
    //{
    //    txtstock.Text = "";
    //    int color = 0;
    //    int quality = 0;
    //    int design = 0;
    //    int shape = 0;
    //    int size = 0;
    //    int shadeColor = 0;
    //    if ((ql.Visible == true && dquality.SelectedIndex > 0) || ql.Visible != true)
    //    {
    //        quality = 1;
    //    }
    //    if (dsn.Visible == true && dddesign.SelectedIndex > 0 || dsn.Visible != true)
    //    {
    //        design = 1;
    //    }
    //    if (clr.Visible == true && ddcolor.SelectedIndex > 0 || clr.Visible != true)
    //    {
    //        color = 1;
    //    }
    //    if (shp.Visible == true && ddshape.SelectedIndex > 0 || shp.Visible != true)
    //    {
    //        shape = 1;
    //    }
    //    if (sz.Visible == true && ddsize.SelectedIndex > 0 || sz.Visible != true)
    //    {
    //        size = 1;
    //    }
    //    if (shd.Visible == true && ddlshade.SelectedIndex > 0 || shd.Visible != true)
    //    {
    //        shadeColor = 1;
    //    }
    //    if (quality == 1 && design == 1 && color == 1 && shape == 1 && size == 1 && shadeColor == 1)
    //    {
    //        int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "", Convert.ToInt32(Session["varCompanyId"]));
    //        string str = "Select Distinct TagNo,Tagno from stock Where CompanyId=" + ddCompName.SelectedValue + " And Godownid=" + ddgodown.SelectedValue + "   and item_finished_id=" + Varfinishedid + " and LotNo='" + ddlotno.SelectedItem.Text + "'";
    //        if (MySession.Stockapply == "True" && ChKForEdit.Checked == false)
    //        {
    //            str = str + " and Round(Qtyinhand,3)>0";
    //        }

    //        UtilityModule.ConditionalComboFill(ref DDTagno, str, true, "--Select--");
    //        if (DDTagno.Items.Count > 0)
    //        {

    //            DDTagno.SelectedIndex = 1;
    //            if (sender != null)
    //            {
    //                DDTagno_SelectedIndexChanged(sender, new EventArgs());
    //            }
    //        }
    //    }
    //}
    //private void Fill_GodownSelectedChange(object sender = null)
    //{
    //    txtstock.Text = "";
    //    int color = 0;
    //    int quality = 0;
    //    int design = 0;
    //    int shape = 0;
    //    int size = 0;
    //    int shadeColor = 0;
    //    if ((ql.Visible == true && dquality.SelectedIndex > 0) || ql.Visible != true)
    //    {
    //        quality = 1;
    //    }
    //    if (dsn.Visible == true && dddesign.SelectedIndex > 0 || dsn.Visible != true)
    //    {
    //        design = 1;
    //    }
    //    if (clr.Visible == true && ddcolor.SelectedIndex > 0 || clr.Visible != true)
    //    {
    //        color = 1;
    //    }
    //    if (shp.Visible == true && ddshape.SelectedIndex > 0 || shp.Visible != true)
    //    {
    //        shape = 1;
    //    }
    //    if (sz.Visible == true && ddsize.SelectedIndex > 0 || sz.Visible != true)
    //    {
    //        size = 1;
    //    }
    //    if (shd.Visible == true && ddlshade.SelectedIndex > 0 || shd.Visible != true)
    //    {
    //        shadeColor = 1;
    //    }
    //    if (quality == 1 && design == 1 && color == 1 && shape == 1 && size == 1 && shadeColor == 1)
    //    {
    //        int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "", Convert.ToInt32(Session["varCompanyId"]));
    //        string str = "Select Distinct lotno,lotno from stock Where CompanyId=" + ddCompName.SelectedValue + " And Godownid=" + ddgodown.SelectedValue + " and item_finished_id=" + Varfinishedid;
    //        if (MySession.Stockapply == "True" && ChKForEdit.Checked == false)
    //        {
    //            str = str + " and Round(QtyInHand,3)>0";
    //        }

    //        UtilityModule.ConditionalComboFill(ref ddlotno, str, true, "--Select--");
    //        ddlotno.SelectedIndex = -1;
    //        if (ddlotno.Items.Count > 0)
    //        {
    //            ddlotno.SelectedIndex = 1;

    //            if (sender != null && ddlotno.SelectedIndex > 0)
    //            {
    //                ddlotno_SelectedIndexChanged(sender, new EventArgs());
    //            }
    //        }
    //    }
    //}
    protected void ddlotno_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (TDTagno.Visible == true)
        //{
        //    DDTagno.SelectedIndex = -1;
        //  //  FillTagNo(sender);
        //}
        //else
        //{
        //    //Fill_LotNoSelectedChange();
        //}
    }
    //private void Fill_LotNoSelectedChange()
    //{
    //    if (ddlotno.Items.Count > 0)
    //    {
    //        txtstock.Text = "0";
    //        int color = 0;
    //        int quality = 0;
    //        int design = 0;
    //        int shape = 0;
    //        int size = 0;
    //        int shadeColor = 0;
    //        if ((ql.Visible == true && dquality.SelectedIndex > 0) || ql.Visible != true)
    //        {
    //            quality = 1;
    //        }
    //        if (dsn.Visible == true && dddesign.SelectedIndex > 0 || dsn.Visible != true)
    //        {
    //            design = 1;
    //        }
    //        if (clr.Visible == true && ddcolor.SelectedIndex > 0 || clr.Visible != true)
    //        {
    //            color = 1;
    //        }
    //        if (shp.Visible == true && ddshape.SelectedIndex > 0 || shp.Visible != true)
    //        {
    //            shape = 1;
    //        }
    //        if (sz.Visible == true && ddsize.SelectedIndex > 0 || sz.Visible != true)
    //        {
    //            size = 1;
    //        }
    //        if (shd.Visible == true && ddlshade.SelectedIndex > 0 || shd.Visible != true)
    //        {
    //            shadeColor = 1;
    //        }
    //        if (quality == 1 && design == 1 && color == 1 && shape == 1 && size == 1 && shadeColor == 1)
    //        {

    //            int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "", Convert.ToInt32(Session["varCompanyId"]));
    //            ViewState["FinishedID"] = Varfinishedid;
    //            string TagNo = "Without Tag No";
    //            string BinNo = "";
    //            if (TDTagno.Visible == true)
    //            {
    //                TagNo = DDTagno.SelectedIndex > 0 ? DDTagno.SelectedItem.Text : "Without Tag No";
    //            }
    //            if (TDBinNo.Visible == true)
    //            {
    //                BinNo = DDBinNo.SelectedIndex > 0 ? DDBinNo.SelectedItem.Text : "";
    //            }
    //            txtstock.Text = UtilityModule.getstockQty(ddCompName.SelectedValue, ddgodown.SelectedValue, ddlotno.SelectedItem.Text, Varfinishedid, TagNo: TagNo, BinNo: BinNo).ToString();

    //        }
    //    }
    //}
    private string CheckStockQty()
    {
        string str = "";
        try
        {
            //int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "");
            SqlParameter[] parparam = new SqlParameter[4];

            int OrderID = Convert.ToInt32(SqlHelper.ExecuteScalar(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "Select Orderid From PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + " Where IssueOrderId=" + ddOrderNo.SelectedValue));
            parparam[0] = new SqlParameter("@OrderId", OrderID);
            parparam[1] = new SqlParameter("@ProcessNo", ddProcessName.SelectedValue);
            parparam[2] = new SqlParameter("@FinishedID", ViewState["FinishedID"]);
            parparam[3] = new SqlParameter("@TxtQty", "0");
            parparam[4] = new SqlParameter("@Message", SqlDbType.NVarChar, 2);
            parparam[4].Direction = ParameterDirection.Output;
            parparam[5] = new SqlParameter("@PrtIdFlag", ddProcessName.SelectedValue);

            string Str = @"Select Select ISNULL(Sum(IssueQuantity),0) From ProcessRawMaster PRM,ProcessRawTran PRT Where PRM.PRMid=PRT.PRMid And PRM.trantype=0 And PRM.TypeFlag = 0 And 
            PRM.Processid=" + ddProcessName.SelectedValue + " And PRM.Prorderid in (Select IssueOrderid From PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" 
            Where OrderId in (Select Orderid From PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + " Where IssueOrderId=" + ddOrderNo.SelectedValue + ")) And PRTid=0 And PRM.MasterCompanyId=" + Session["varCompanyId"];

            SqlHelper.ExecuteNonQuery(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_CheckStockQtyForProcessIssue", parparam);
            if (parparam[4].Value.ToString() == "G")
            {
                LblError.Visible = true;
                LblError.Text = "IssueQty should not be greater than stock";
                str = "G";
            }
        }
        catch (Exception ex)
        {
            UtilityModule.MessageAlert(ex.Message, "Masters/RawMaterial/IndentRawIssue");
        }
        return str;
    }
    protected void DuplicateChallanNo()
    {
        //LblError.Text = "";
        //LblError.Visible = true;
        //SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        //if (con.State == ConnectionState.Closed)
        //{
        //    con.Open();
        //}
        //try
        //{
        //    //if (txtchalanno.Text != "")
        //    //{
        //    //    string str = "Select ChalanNo From ProcessRawMaster Where ChalanNo<>'' And TranType=0 And TypeFlag = 0 And ChalanNo='" + txtchalanno.Text + "' and Empid>0 And PRMID<>" + ViewState["Prmid"] + " And MasterCompanyId=" + Session["varCompanyId"];
        //    //    DataSet ds = SqlHelper.ExecuteDataset(con, CommandType.Text, str);
        //    //    if (ds.Tables[0].Rows.Count > 0)
        //    //    {
        //    //        LblError.Text = "Challan no. already exists.....";
        //    //    }
        //    //}
        //}
        //catch (Exception ex)
        //{
        //    LblError.Visible = true;
        //    LblError.Text = ex.Message;
        //}
        //finally
        //{
        //    con.Close();
        //    con.Dispose();
        //}
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {

        if (Session["varcompanyid"].ToString() == "21")
        {
            string status = "";
            if (txtkitno.Text == "")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "save1", "alert('Please enter Kit No.');", true);
                txtkitno.Focus();
                return;
            }

        }
        if (!string.IsNullOrEmpty(hdnfinishedid.Value.ToString()) && !string.IsNullOrEmpty(hdnchallanfinishedid.Value.ToString()))
        {
            if (hdnfinishedid.Value.ToString() != hdnchallanfinishedid.Value.ToString())
            {
                LblError.Text = "Kit desciption not matched with folio description.";
                LblError.Visible = true;
                return;
            }
            else
            {
                LblError.Visible = true;
            }
        
        }
        //********sql table Type
        DataTable dtrecords = new DataTable();
        dtrecords.Columns.Add("ifinishedid", typeof(int));
        dtrecords.Columns.Add("IUnitid", typeof(int));
        dtrecords.Columns.Add("Isizeflag", typeof(int));
        dtrecords.Columns.Add("Godownid", typeof(int));
        dtrecords.Columns.Add("Lotno", typeof(string));
        dtrecords.Columns.Add("TagNo", typeof(string));
        dtrecords.Columns.Add("issueqty", typeof(float));
        //dtrecords.Columns.Add("Noofcone", typeof(int));
        dtrecords.Columns.Add("Prorderid", typeof(int));
        dtrecords.Columns.Add("ConsmpQty", typeof(float));
        dtrecords.Columns.Add("BinNo", typeof(string));
        Label lblprmid = new Label();
        //*******************
        for (int i = 0; i < gvkit.Rows.Count; i++)
        {
            //CheckBox Chkboxitem = ((CheckBox)DG.Rows[i].FindControl("Chkboxitem"));
            //TextBox txtissueqty = ((TextBox)DG.Rows[i].FindControl("txtissueqty"));
            //DropDownList DDGodown = ((DropDownList)gvdetail.Rows[i].FindControl("DDGodown"));
            //DropDownList DDLotNo = ((DropDownList)gvdetail.Rows[i].FindControl("DDLotNo"));
            //DropDownList DDTagNo = ((DropDownList)  gvdetail.Rows[i].FindControl("DDTagNo"));
            //DropDownList DDBinNo = ((DropDownList)gvdetail.Rows[i].FindControl("DDBinNo"));

            //if (Chkboxitem.Checked == true && (txtissueqty.Text != "") && DDGodown.SelectedIndex > 0 && DDLotNo.SelectedIndex > 0 && DDTagNo.SelectedIndex > 0)
            //{
            Label lblitemfinishedid = ((Label)gvkit.Rows[i].FindControl("lblfinishedid"));
            Label lblunitid = ((Label)gvkit.Rows[i].FindControl("lblunitid"));
            Label lblitemid = ((Label)gvkit.Rows[i].FindControl("lblitemid"));
            Label lblgodowinid = ((Label)gvkit.Rows[i].FindControl("lblgodownid"));
            Label lbllotno = ((Label)gvkit.Rows[i].FindControl("lblLotno"));
            Label lbltagno = ((Label)gvkit.Rows[i].FindControl("lbltagno"));
            Label lblissueqty = ((Label)gvkit.Rows[i].FindControl("lblissueqty"));
            lblprmid = ((Label)gvkit.Rows[i].FindControl("lblprmid"));
               
                //TextBox txtnoofcone = ((TextBox)DG.Rows[i].FindControl("txtnoofcone"));
                //Label lblissueorderid = ((Label)DG.Rows[i].FindControl("lblissueorderid"));
                //Label lblconsmpqty = ((Label)DG.Rows[i].FindControl("lblconsmpqty"));
                //*********************
                DataRow dr = dtrecords.NewRow();
                dr["ifinishedid"] = lblitemfinishedid.Text;
                dr["IUnitid"] = lblunitid.Text;
                dr["Isizeflag"] = 0;
                dr["Godownid"] = lblgodowinid.Text;
                dr["Lotno"] = lbllotno.Text;
                dr["TagNo"] = lbltagno.Text;
                dr["IssueQty"] = lblissueqty.Text == "" ? "0" : lblissueqty.Text;
                //dr["Noofcone"] = txtnoofcone.Text == "" ? "0" : txtnoofcone.Text;
                dr["Prorderid"] = lblitemid.Text;
                dr["consmpqty"] = 0;
                dr["BinNo"] = "";
                dtrecords.Rows.Add(dr);
          //  }
        }
        //if (Session["varcompanyid"].ToString() == "21")
        //{
        //    if (chkEdit.Checked == false)     // Change when Updated Completed
        //    {
        //        if (dtrecords.Rows.Count != DG.Rows.Count)
        //        {
        //            ScriptManager.RegisterStartupScript(Page, GetType(), "save1", "alert('Please check stock qty in all rows');", true);
        //            return;
        //        }
        //    }
        //}

        if (Session["varcompanyid"].ToString() == "22")
        {
            DateTime IssueDate = Convert.ToDateTime(txtdate.Text.ToString());
           // DateTime FolioDate = Convert.ToDateTime(txtFolioIssueDate.Text.ToString());

            //if (IssueDate < FolioDate)
            //{
            //    ScriptManager.RegisterStartupScript(Page, GetType(), "save1", "alert('Please select date greater then folio date');", true);
            //    return;
            //}

            //if (chkEdit.Checked == false)     // Change when Updated Completed
            //{
            //    if (dtrecords.Rows.Count != DG.Rows.Count)
            //    {
            //        ScriptManager.RegisterStartupScript(Page, GetType(), "save1", "alert('Please check stock qty in all rows');", true);
            //        return;
            //    }
            //}
        }

        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        if (dtrecords.Rows.Count > 0)
        {
            SqlTransaction Tran = con.BeginTransaction();
            try
            {
                SqlParameter[] param = new SqlParameter[16];
                param[0] = new SqlParameter("@PrmId", SqlDbType.Int);
                //if (chkEdit.Checked == true && Session["varcompanyid"].ToString() == "21")
                //{
                //    param[0].Value = DDissueno.SelectedValue;
                //}
                //else
                //{
                    param[0].Value = 0;
              //  }

                param[0].Direction = ParameterDirection.InputOutput;
                param[1] = new SqlParameter("@companyid", ddCompName.SelectedValue);
                param[2] = new SqlParameter("@Processid", ddProcessName.SelectedValue);
                param[3] = new SqlParameter("@Prorderid", ddOrderNo.SelectedValue);
                param[4] = new SqlParameter("@issueDate", txtdate.Text);
                param[5] = new SqlParameter("@userid", Session["varuserid"]);
                param[6] = new SqlParameter("@Mastercompanyid", Session["varcompanyid"]);
                param[7] = new SqlParameter("@dtrecords", dtrecords);
                param[8] = new SqlParameter("@TranType", SqlDbType.TinyInt);
                param[8].Value = 0;
                param[9] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
                param[9].Direction = ParameterDirection.Output;
                param[10] = new SqlParameter("@EWayBillNo", string.Empty);
                param[11] = new SqlParameter("@StockNoQty", "0" );
                param[12] = new SqlParameter("@FolioChallanNo", ddOrderNo.SelectedIndex > 0 ? ddOrderNo.SelectedItem.Text : "");
                param[13] = new SqlParameter("@CHALANNO", SqlDbType.VarChar, 50);
                param[13].Value = "";
                param[13].Direction = ParameterDirection.InputOutput;
                param[14] = new SqlParameter("@Kitid",lblprmid.Text);
                param[15] = new SqlParameter("@Kitno", txtkitno.Text);

                ///**********
                SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "Pro_SaveWeftIssueonLoom_kit", param);
                //*******************
                ViewState["reportid"] = param[0].Value.ToString();
                string result = param[13].Value.ToString();
               // hnissueid.Value = param[0].Value.ToString();
                Tran.Commit();
                if (param[9].Value.ToString() != "")
                {
                    LblError.Text = param[9].Value.ToString();
                }
                else
                {
                    LblError.Visible = true;
                    LblError.Text = "DATA SAVED SUCCESSFULLY.";
                    Fill_Grid();
                    //FillissueGrid();
                }
              //  TxtTotalPcs.Text = "";

            }
            catch (Exception ex)
            {
                Tran.Rollback();
                LblError.Text = ex.Message;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(Page, GetType(), "save1", "alert('Please select atleast one check box to save data.');", true);
        }
    }
    //protected void btnsave_Click(object sender, EventArgs e)
    //{
    //    //CHECKVALIDCONTROL();
    //    //if (LblError.Text == "")
    //    //{
    //    //    if (variable.VarMANYFOLIORAWISSUE_SINGLECHALAN == "0")
    //    //    {
    //    //        DuplicateChallanNo();
    //    //    }
    //    //}

    //    if (LblError.Text == "")
    //    {
    //        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
    //        con.Open();
    //        SqlTransaction Tran = con.BeginTransaction();
    //        try
    //        {
    //            SqlParameter[] arr = new SqlParameter[38];

    //            arr[0] = new SqlParameter("@PrmID", SqlDbType.Int);
    //            arr[1] = new SqlParameter("@CompanyId", SqlDbType.Int);
    //            arr[2] = new SqlParameter("@EmpId", SqlDbType.Int);
    //            arr[3] = new SqlParameter("@ProcessId", SqlDbType.Int);
    //            arr[4] = new SqlParameter("@OrderId", SqlDbType.Int);
    //            arr[5] = new SqlParameter("@IssueDate", SqlDbType.SmallDateTime);
    //            arr[6] = new SqlParameter("@ChalanNo", SqlDbType.NVarChar, 50);
    //            arr[7] = new SqlParameter("@TranType", SqlDbType.Int);
    //            arr[8] = new SqlParameter("@userid", SqlDbType.Int);
    //            arr[9] = new SqlParameter("@mastercompanyid", SqlDbType.Int);
    //            arr[10] = new SqlParameter("@Prtid", SqlDbType.Int);
    //            arr[11] = new SqlParameter("@CategoryId", SqlDbType.Int);
    //            arr[12] = new SqlParameter("@Itemid", SqlDbType.Int);
    //            arr[13] = new SqlParameter("@FinishedId", SqlDbType.Int);
    //            arr[14] = new SqlParameter("@GodownId", SqlDbType.Int);
    //            arr[15] = new SqlParameter("@IssueQuantity", SqlDbType.Float);
    //            arr[16] = new SqlParameter("@lotNo", SqlDbType.NVarChar, 50);
    //            arr[17] = new SqlParameter("@UnitId", SqlDbType.Int);
    //            arr[18] = new SqlParameter("@PrmIdOutPut", SqlDbType.Int);
    //            arr[19] = new SqlParameter("@PrtIdOutPut", SqlDbType.Int);
    //            arr[20] = new SqlParameter("@UpdateFlag", SqlDbType.Int);
    //            arr[21] = new SqlParameter("@BinNo", SqlDbType.VarChar, 50);
    //            arr[22] = new SqlParameter("@TagNo", SqlDbType.VarChar, 50);
    //            arr[23] = new SqlParameter("@Msg", SqlDbType.VarChar, 100);
    //            arr[24] = new SqlParameter("@TanaBana", SqlDbType.VarChar, 50);
    //            arr[25] = new SqlParameter("@TransportName", SqlDbType.VarChar, 50);
    //            arr[26] = new SqlParameter("@BiltyNo", SqlDbType.VarChar, 50);
    //            arr[27] = new SqlParameter("@VehicleNo", SqlDbType.VarChar, 20);
    //            arr[28] = new SqlParameter("@EstimatedRate", SqlDbType.Float);
    //            arr[29] = new SqlParameter("@Conetype", SqlDbType.VarChar, 50);
    //            arr[30] = new SqlParameter("@Noofcone", SqlDbType.Int);
    //            arr[31] = new SqlParameter("@Remark", txtremarks.Text);
    //            arr[32] = new SqlParameter("@FolioChallanNo", SqlDbType.VarChar, 50);
    //            arr[33] = new SqlParameter("@BellWt", SqlDbType.Float);
    //            arr[34] = new SqlParameter("@CGSTSGST", SqlDbType.Float);
    //            arr[35] = new SqlParameter("@ItemDesignID", SqlDbType.Int);
    //            arr[36] = new SqlParameter("@BranchID", SqlDbType.Int);
    //            arr[37] = new SqlParameter("@EWayBillNo", SqlDbType.VarChar, 50);

    //            int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, Tran, ddlshade, "", Convert.ToInt32(Session["varCompanyId"]));

    //            arr[0].Value = ViewState["Prmid"];
    //            arr[1].Value = ddCompName.SelectedValue;
    //            arr[2].Value = ddempname.SelectedValue;
    //            arr[3].Value = ddProcessName.SelectedValue;
    //            arr[4].Value = ddOrderNo.SelectedValue;
    //            arr[5].Value = txtdate.Text;
    //            arr[6].Value = txtchalanno.Text;
    //            arr[6].Direction = ParameterDirection.InputOutput;
    //            arr[7].Value = 0;
    //            arr[8].Value = Session["varuserid"].ToString();
    //            arr[9].Value = Session["varCompanyId"].ToString();
    //            arr[10].Value = 0;
    //            arr[20].Value = 0;
    //            if (btnsave.Text == "Update")
    //            {
    //                arr[10].Value = gvdetail.SelectedDataKey.Value;
    //                arr[20].Value = 1;
    //            }
    //            arr[11].Value = ddCatagory.SelectedValue;
    //            arr[12].Value = dditemname.SelectedValue;
    //            arr[13].Value = Varfinishedid;
    //            arr[14].Value = ddgodown.SelectedValue;
    //            arr[15].Value = txtissue.Text;
    //            arr[16].Value = ddlotno.SelectedItem.Text;
    //            arr[17].Value = ddlunit.SelectedValue;
    //            arr[18].Direction = ParameterDirection.Output;
    //            arr[19].Direction = ParameterDirection.Output;
    //            string BinNo = TDBinNo.Visible == false ? "" : (DDBinNo.SelectedIndex > 0 ? DDBinNo.SelectedItem.Text : "");
    //            arr[21].Value = BinNo;
    //            arr[22].Value = TDTagno.Visible == false ? "Without Tag No" : DDTagno.SelectedItem.Text;
    //            arr[23].Direction = ParameterDirection.Output;
    //            arr[24].Value = txtTanaBana.Text;
    //            arr[25].Value = txtTransportName.Text;
    //            arr[26].Value = txtBiltyNo.Text;
    //            arr[27].Value = txtVehicleNo.Text;
    //            arr[28].Value = txtEstimatedRate.Text == "" ? "0" : txtEstimatedRate.Text;
    //            arr[29].Value = DDconetype.SelectedItem.Text;
    //            arr[30].Value = txtnoofcone.Text == "" ? "0" : txtnoofcone.Text;
    //            arr[32].Value = ddOrderNo.SelectedIndex > 0 ? ddOrderNo.SelectedItem.Text : "";
    //            arr[33].Value = TxtBellWt.Text == "" ? "0" : TxtBellWt.Text;
    //            arr[34].Value = txtCGSTSGST.Text == "" ? "0" : txtCGSTSGST.Text;

    //            if (TdDDItemDesignName.Visible == true)
    //            {
    //                arr[35].Value = DDItemDesignName.SelectedValue;
    //            }
    //            else
    //            {
    //                arr[35].Value = 0;
    //            }
    //            arr[36].Value = DDBranchName.SelectedValue;
    //            arr[37].Value = TDEWayBillNo.Visible == true ? txtEWayBillNo.Text : "";

    //            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_PROCESS_RAW_ISSUE", arr);

    //            Tran.Commit();
    //            txtchalanno.Text = arr[6].Value.ToString();
    //            ViewState["Prmid"] = arr[18].Value;
    //            LblError.Visible = true;
    //            LblError.Text = arr[23].Value.ToString();
    //            Fill_Grid();
    //            fill_Grid_ShowConsmption();
    //            SaveReferece();
    //            btnsave.Text = "Save";
    //        }
    //        catch (Exception ex)
    //        {
    //            Tran.Rollback();
    //            LblError.Visible = true;
    //            LblError.Text = ex.Message;
    //        }
    //        finally
    //        {
    //            con.Close();
    //            con.Dispose();
    //        }
    //    }
    //}
    private void SaveReferece()
    {
        //if (ddlshade.Items.Count > 0 && shd.Visible == true)
        //{
        //    ddlshade.SelectedIndex = 0;
        //}
        //txtstock.Text = "";
        //txtconqty.Text = "";
        //TxtPendQty.Text = "";
        //txtissue.Text = "";
        //txtTanaBana.Text = "";
        //txtEstimatedRate.Text = "";
        //txtnoofcone.Text = "";
        //txtCGSTSGST.Text = "";


    }
    private void Fill_Grid()
    {
        gvdetail.DataSource = fill_Data_grid();
        gvdetail.DataBind();
    }
    private DataSet fill_Data_grid()
    {
        DataSet ds = null;
        string strsql = "";
        try
        {
            if (variable.VarMANYFOLIORAWISSUE_SINGLECHALAN == "1")
            {
                strsql = @"Select PrtId,CATEGORY_NAME,ITEM_NAME,QualityName+ Space(2)+DesignName+ Space(2)+ColorName+ Space(2)+ShapeName+ Space(2)+SizeFt+ Space(2)+ShadeColorName DESCRIPTION,
                             IssueQuantity Qty,LotNo,GodownName,Pt.BinNo,PT.TagNo From Processrawmaster Pm,ProcessRawTran PT,V_FinishedItemDetail VF,GodownMaster GM Where pm.prmid=pt.prmid and  PT.Finishedid=VF.Item_Finished_id And 
                             PT.GodownId=GM.GodownId And PM.chalanno='" + txtchalanno.Text + "' and PM.Trantype=0 And PM.TypeFlag = 0 And VF.MasterCompanyId=" + Session["varCompanyId"];
                if (ChKForEdit.Checked == true && ddOrderNo.SelectedIndex > 0)
                {

                    strsql = strsql + " and PM.Prorderid=" + ddOrderNo.SelectedValue + "";
                }
            }
            else
            {
                strsql = @"Select PrtId,CATEGORY_NAME,ITEM_NAME,QualityName+ Space(2)+DesignName+ Space(2)+ColorName+ Space(2)+ShapeName+ Space(2)+SizeFt+ Space(2)+ShadeColorName DESCRIPTION,
                             IssueQuantity Qty,LotNo,GodownName,Pt.BinNo,PT.TagNo From ProcessRawTran PT,V_FinishedItemDetail VF,GodownMaster GM Where PT.Finishedid=VF.Item_Finished_id And 
                             PT.GodownId=GM.GodownId And PT.PrmID=" + ViewState["Prmid"] + " And VF.MasterCompanyId=" + Session["varCompanyId"];
                strsql = strsql + " Order By PrtId";
            }
            ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);
        }
        catch (Exception ex)
        {
            LblError.Visible = true;
            LblError.Text = ex.Message;
            Logs.WriteErrorLog("Masters_process_ProcessRawIssue|fill_Data_grid|" + ex.Message);
        }
        return ds;
    }
    protected void txtchalan_ontextchange(object sender, EventArgs e)
    {
        //string ChalanNo = Convert.ToString(SqlHelper.ExecuteScalar(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "select isnull(ChalanNo,0) asd from ProcessRawMaster where TypeFlag = 0 And ChalanNo='" + txtchalanno.Text + "' And MasterCompanyId=" + Session["varCompanyId"]));
        //if (ChalanNo != "")
        //{
        //    txtchalanno.Text = "";
        //    txtchalanno.Focus();
        //    LblError.Visible = true;
        //    LblError.Text = "Challan No already exist";
        //}
        //else
        //{
        //    LblError.Visible = false;
        //}
    }
    protected void gvdetail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Attributes["onmouseover"] = "javascript:setMouseOverColor(this);";
            e.Row.Attributes["onmouseout"] = "javascript:setMouseOutColor(this);";
            e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gvdetail, "Select$" + e.Row.RowIndex);
            for (int i = 0; i < gvdetail.Columns.Count; i++)
            {
                if (variable.VarBINNOWISE == "1")
                {
                    if (gvdetail.Columns[i].HeaderText.ToUpper() == "BIN NO.")
                    {
                        gvdetail.Columns[i].Visible = true;
                    }
                }
                else
                {
                    if (gvdetail.Columns[i].HeaderText.ToUpper() == "BIN NO.")
                    {
                        gvdetail.Columns[i].Visible = false;
                    }
                }
            }
        }
    }
    protected void gvkit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    e.Row.Attributes["onmouseover"] = "javascript:setMouseOverColor(this);";
        //    e.Row.Attributes["onmouseout"] = "javascript:setMouseOutColor(this);";
        //    e.Row.Attributes["onclick"] = ClientScript.GetPostBackClientHyperlink(this.gvdetail, "Select$" + e.Row.RowIndex);
        //    for (int i = 0; i < gvdetail.Columns.Count; i++)
        //    {
        //        if (variable.VarBINNOWISE == "1")
        //        {
        //            if (gvkit.Columns[i].HeaderText.ToUpper() == "BIN NO.")
        //            {
        //                gvkit.Columns[i].Visible = true;
        //            }
        //        }
        //        else
        //        {
        //            if (gvkit.Columns[i].HeaderText.ToUpper() == "BIN NO.")
        //            {
        //                gvkit.Columns[i].Visible = false;
        //            }
        //        }
        //    }
        //}
    }
    protected void gvdetail_SelectedIndexChanged(object sender, EventArgs e)
    {
//        LblError.Text = "";
//        DataSet ds = null;
//        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
//        try
//        {
//            con.Open();
//            ds = null;
//            string sql = @"SELECT DISTINCT PRM.PRMid,PRM.Companyid,PRM.Processid,PRM.Prorderid,PRM.Empid,PRM.Date,PRM.ChalanNo,PRT.PRTid,PRT.Finishedid,PRT.IssueQuantity+prt.coneweight as Issuequantity,
//                           PRT.Godownid,PRT.Lotno,PRT.UnitId,CATEGORY_ID,ITEM_ID,QualityId,ColorId,designId,ShapeId,SizeId,ShadecolorId,isnull(Prt.BinNo,'') as BinNo,Prt.Tagno,
//                           isnull(PRT.TanaBana,'') as TanaBana,isnull(PRT.EstimatedRate,0) as EstimatedRate,prt.conetype,prt.noofcone,prt.coneweight,isnull(PRT.CGSTSGST,0) as CGSTSGST
//                           FROM ProcessRawMaster PRM,ProcessRawTran PRT,V_FinishedItemDetail IPM
//                           Where PRM.TypeFlag = 0 And PRM.PRMid=PRT.PRMid And PRT.Finishedid=IPM.ITEM_FINISHED_ID And PRT.PRTid=" + gvdetail.SelectedValue + " ANd PRM.MasterCompanyId=" + Session["varCompanyId"];
//            ds = SqlHelper.ExecuteDataset(con, CommandType.Text, sql);

//            ddCatagory.SelectedValue = ds.Tables[0].Rows[0]["CATEGORY_ID"].ToString();
//            Fill_Category_SelectedChange();
//            dditemname.SelectedValue = ds.Tables[0].Rows[0]["ITEM_ID"].ToString();

//            ItemName_SelectChange();
//            dquality.SelectedValue = ds.Tables[0].Rows[0]["QualityId"].ToString();
//            //ItemName_SelectChange();

//            UtilityModule.ConditionalComboFill(ref ddlunit, "SELECT u.UnitId,u.UnitName  FROM ITEM_MASTER i INNER JOIN  Unit u ON i.UnitTypeID = u.UnitTypeID where item_id=" + dditemname.SelectedValue + " And i.MasterCompanyId=" + Session["varCompanyId"], true, "Select Unit");
//            ddlunit.SelectedValue = ds.Tables[0].Rows[0]["UnitId"].ToString();

//            //            if (ql.Visible == true)
//            //            {
//            //                UtilityModule.ConditionalComboFill(ref dquality, @"SELECT DISTINCT dbo.Quality.QualityId,dbo.Quality.QualityName
//            //                FROM  dbo.ITEM_PARAMETER_MASTER INNER JOIN PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId
//            //                INNER JOIN dbo.Quality ON dbo.ITEM_PARAMETER_MASTER.QUALITY_ID = dbo.Quality.QualityId
//            //                Where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " and quality.item_id=" + dditemname.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Quallity");

//            //                dquality.SelectedValue = ds.Tables[0].Rows[0]["QualityId"].ToString();
//            //                QualitySelectedIndexChange();
//            //            }
//            if (dsn.Visible == true)
//            {
//                dddesign.SelectedValue = ds.Tables[0].Rows[0]["designId"].ToString();
//            }
//            if (clr.Visible == true)
//            {
//                ddcolor.SelectedValue = ds.Tables[0].Rows[0]["ColorId"].ToString();
//            }
//            if (shp.Visible == true)
//            {
//                ddshape.SelectedValue = ds.Tables[0].Rows[0]["ShapeId"].ToString();
//            }
//            if (shd.Visible == true)
//            {
//                ddlshade.SelectedValue = ds.Tables[0].Rows[0]["ShadecolorId"].ToString();
//            }
//            if (procode.Visible == true)
//            {
//                TxtProdCode.Text = ds.Tables[0].Rows[0]["productcode"].ToString();
//            }
//            if (sz.Visible == true)
//            {
//                if (ddlunit.SelectedValue == "1")
//                {
//                    UtilityModule.ConditionalComboFill(ref ddsize, "select sizeid,sizemtr from size where Shapeid=" + ddshape.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"], true, "select size");
//                }
//                else if (ddlunit.SelectedValue == "2")
//                {
//                    UtilityModule.ConditionalComboFill(ref ddsize, "select sizeid,sizeft from size where Shapeid=" + ddshape.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"], true, "select size");
//                }
//                ddsize.SelectedValue = ds.Tables[0].Rows[0]["SizeId"].ToString();
//            }
//            string Str;
//            switch (Session["varcompanyid"].ToString())
//            {
//                case "9":
//                    Str = @"SELECT ROUND(SUM(CASE WHEN CalType=0 or Caltype=2 Then Case When UnitId=1 Then (PD.Qty-isnull(CancelQty,0))*PD.Area*(PCD.IQTY+PCD.ILoss) Else (PD.Qty-Isnull(CancelQty,0))*PD.Area*(PCD.IQTY+PCD.ILOss)/10.76391 End Else 
//                    Case When UnitId=1 Then (PD.Qty-Isnull(CancelQty,0))*(PCD.IQTY+PCD.ILoss) Else (PD.Qty-isnull(CancelQty,0))*(PCD.IQTY+PCD.ILoss) End End),3) Qty,[dbo].[Get_ProcessIssueQty] (PCD.IFinishedid,PM.IssueOrderId) IssQty 
//                    From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
//                    PROCESS_CONSUMPTION_DETAIL PCD Where PM.IssueOrderId=PD.IssueOrderId And PD.Issue_Detail_Id=PCD.Issue_Detail_Id And 
//                    PCD.ProcessId=" + ddProcessName.SelectedValue + " And PCD.IssueOrderID=" + ddOrderNo.SelectedValue + @" And PCD.IFinishedid=" + ds.Tables[0].Rows[0]["Finishedid"] + @"
//                    Group By PM.IssueOrderId,PCD.IFinishedid";
//                    break;
//                case "15":
//                    Str = @"SELECT ROUND(SUM(Case When UnitId=1 Then (PD.Qty-isnull(CancelQty,0))*PD.Area*PCD.IQTY*1.196 Else (PD.Qty-isnull(CancelQty,0))*PD.Area*PCD.IQTY End ),3)  Qty,[dbo].[Get_ProcessIssueQty] (PCD.IFinishedid,PM.IssueOrderId) IssQty 
//                    From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
//                    PROCESS_CONSUMPTION_DETAIL PCD Where PM.IssueOrderId=PD.IssueOrderId And PD.Issue_Detail_Id=PCD.Issue_Detail_Id And 
//                    PCD.ProcessId=" + ddProcessName.SelectedValue + " And PCD.IssueOrderID=" + ddOrderNo.SelectedValue + @" And PCD.IFinishedid=" + ds.Tables[0].Rows[0]["Finishedid"] + @"
//                    Group By PM.IssueOrderId,PCD.IFinishedid";
//                    break;
//                default:
//                    Str = @"SELECT ROUND(SUM(CASE WHEN CalType=0 or Caltype=2 Then Case When UnitId=1 Then (PD.Qty-isnull(CancelQty,0))*PD.Area*PCD.IQTY*1.196 Else (PD.Qty-isnull(CancelQty,0))*PD.Area*PCD.IQTY End Else 
//                    Case When UnitId=1 Then (PD.Qty-isnull(CancelQty,0))*PCD.IQTY*1.196 Else (PD.Qty-isnull(CancelQty,0))*PCD.IQTY End End),3) Qty,[dbo].[Get_ProcessIssueQty] (PCD.IFinishedid,PM.IssueOrderId) IssQty 
//                    From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
//                    PROCESS_CONSUMPTION_DETAIL PCD Where PM.IssueOrderId=PD.IssueOrderId And PD.Issue_Detail_Id=PCD.Issue_Detail_Id And 
//                    PCD.ProcessId=" + ddProcessName.SelectedValue + " And PCD.IssueOrderID=" + ddOrderNo.SelectedValue + @" And PCD.IFinishedid=" + ds.Tables[0].Rows[0]["Finishedid"] + @"
//                    Group By PM.IssueOrderId,PCD.IFinishedid";
//                    break;
//            }

//            DataSet ds1 = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str);
//            if (ds1.Tables[0].Rows.Count > 0)
//            {
//                txtconqty.Text = (ds1.Tables[0].Rows[0]["qty"].ToString());
//                TxtPendQty.Text = (Math.Round(Convert.ToDouble(ds1.Tables[0].Rows[0]["qty"]) - Convert.ToDouble(ds1.Tables[0].Rows[0]["IssQty"]), 3)).ToString();
//            }

//            ////UtilityModule.ConditionalComboFill(ref ddgodown, "Select Distinct GM.GodownID,GM.GodownName From GodownMaster GM,Stock S Where GM.GodownID=S.GodownID And QtyInHand>0 And CompanyId=" + ddCompName.SelectedValue + " And item_finished_id=" + ds.Tables[0].Rows[0]["Finishedid"] + " And GM.MasterCompanyId=" + Session["varCompanyId"] + " Order By GodownName", true, "--Select--");

//            string str1 = "";
//            str1 = "Select Distinct GM.GodownID,GM.GodownName From GodownMaster GM JOIN Godown_Authentication GA ON GM.GoDownID=GA.GodownID and GA.UserID=" + Session["varUserId"] + " and GA.MasterCompanyId=" + Session["varCompanyId"] + @"
//                    JOIN Stock S ON GM.GodownID=S.GodownID
//                    Where QtyInHand>0 And CompanyId=" + ddCompName.SelectedValue + " And item_finished_id=" + ds.Tables[0].Rows[0]["Finishedid"] + " And GM.MasterCompanyId=" + Session["varCompanyId"] + @"
//                    Order By GodownName";

//            UtilityModule.ConditionalComboFill(ref ddgodown, str1, true, "--Select--");
//            ddgodown.SelectedValue = ds.Tables[0].Rows[0]["Godownid"].ToString();
//            ddgodown_SelectedIndexChanged(sender, new EventArgs());
//            if (ddlotno.Items.FindByValue(ds.Tables[0].Rows[0]["Lotno"].ToString()) != null)
//            {
//                ddlotno.SelectedValue = ds.Tables[0].Rows[0]["Lotno"].ToString();
//                ddlotno_SelectedIndexChanged(sender, new EventArgs());
//            }
//            if (TDTagno.Visible == true)
//            {
//                if (DDTagno.Items.FindByValue(ds.Tables[0].Rows[0]["Tagno"].ToString()) != null)
//                {
//                    DDTagno.SelectedValue = ds.Tables[0].Rows[0]["Tagno"].ToString();
//                    DDTagno_SelectedIndexChanged(sender, new EventArgs());
//                }
//            }
//            if (TDBinNo.Visible == true)
//            {
//                if (DDBinNo.Items.FindByText(ds.Tables[0].Rows[0]["BinNo"].ToString()) != null)
//                {
//                    DDBinNo.SelectedValue = ds.Tables[0].Rows[0]["BinNo"].ToString();
//                    DDBinNo_SelectedIndexChanged(sender, new EventArgs());
//                }
//            }

//            if (TDTanaBana.Visible == true)
//            {
//                txtTanaBana.Text = ds.Tables[0].Rows[0]["TanaBana"].ToString();
//                txtEstimatedRate.Text = ds.Tables[0].Rows[0]["EstimatedRate"].ToString();
//            }
//            if (DDconetype.Items.FindByText(ds.Tables[0].Rows[0]["Conetype"].ToString()) != null)
//            {
//                DDconetype.SelectedValue = ds.Tables[0].Rows[0]["Conetype"].ToString();
//            }
//            txtnoofcone.Text = ds.Tables[0].Rows[0]["noofcone"].ToString();
//            txtissue.Text = ds.Tables[0].Rows[0]["issuequantity"].ToString();
//            TxtPendQty.Text = (Convert.ToDouble(TxtPendQty.Text) + Convert.ToDouble(txtissue.Text)).ToString();
//            txtstock.Text = (Convert.ToDouble(txtstock.Text) + Convert.ToDouble(txtissue.Text)).ToString();

//            if (TDCGSTSGST.Visible == true)
//            {
//                txtCGSTSGST.Text = ds.Tables[0].Rows[0]["CGSTSGST"].ToString();
//            }
//        }
//        catch (Exception ex)
//        {
//            LblError.Visible = true;
//            LblError.Text = ex.Message;
//        }
//        finally
//        {
//            if (con.State == ConnectionState.Open)
//            {
//                con.Close();
//            }
//        }
//        btnsave.Text = "Update";
    }

    protected void TxtProdCode_TextChanged(object sender, EventArgs e)
    {
        DataSet ds;
        string Str;
        //if (TxtProdCode.Text != "" && ddOrderNo.SelectedIndex > 0)
        //{

        //    Str = "select IPM.*,IM.CATEGORY_ID  from ITEM_PARAMETER_MASTER IPM,ITEM_MASTER IM,PROCESS_CONSUMPTION_DETAIL PCD  WHERE IPM.ITEM_FINISHED_ID = PCD.IFINISHEDID and PCD.ISSUEORDERID =" + ddOrderNo.SelectedValue + " and IPM.ITEM_ID=IM.ITEM_ID and ProductCode='" + TxtProdCode.Text + "' And IPM.MasterCompanyId=" + Session["varCompanyId"];
        //    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        string Qry = @"select category_id,category_name from item_category_master Where MasterCompanyId=" + Session["varCompanyId"];
        //        Qry = Qry + " Select Distinct Item_Id,Item_Name from Item_Master where MasterCompanyId=" + Session["varCompanyId"] + " And Category_Id=" + Convert.ToInt32(ds.Tables[0].Rows[0]["CATEGORY_ID"].ToString());
        //        Qry = Qry + "  select qualityid,qualityname from quality where MasterCompanyId=" + Session["varCompanyId"] + " And item_id=" + Convert.ToInt32(ds.Tables[0].Rows[0]["ITEM_ID"].ToString());
        //        Qry = Qry + "  select distinct Designid,DesignName from Design Where MasterCompanyId=" + Session["varCompanyId"] + " Order  by DesignName ";
        //        Qry = Qry + "  SELECT ColorId,ColorName FROM Color Where MasterCompanyId=" + Session["varCompanyId"] + " order by colorid";
        //        Qry = Qry + "  select Shapeid,ShapeName from Shape Where MasterCompanyId=" + Session["varCompanyId"] + " Order by Shapeid  ";
        //        Qry = Qry + "  SELECT SIZEID,SIZEFT fROM SIZE WhERE MasterCompanyId=" + Session["varCompanyId"] + " ANd SHAPEID=" + ddshape.SelectedValue + "";
        //        DataSet DSQ = SqlHelper.ExecuteDataset(Qry);
        //        UtilityModule.ConditionalComboFillWithDS(ref ddCatagory, DSQ, 0, true, "select");
        //        ddCatagory.SelectedValue = ds.Tables[0].Rows[0]["CATEGORY_ID"].ToString();
        //        UtilityModule.ConditionalComboFillWithDS(ref dditemname, DSQ, 1, true, "--Select Item--");
        //        dditemname.SelectedValue = ds.Tables[0].Rows[0]["ITEM_ID"].ToString();
        //        UtilityModule.ConditionalComboFillWithDS(ref dquality, DSQ, 2, true, "Select Quallity");
        //        dquality.SelectedValue = ds.Tables[0].Rows[0]["QUALITY_ID"].ToString();
        //        UtilityModule.ConditionalComboFillWithDS(ref dddesign, DSQ, 3, true, "--Select Design--");
        //        dddesign.SelectedValue = ds.Tables[0].Rows[0]["DESIGN_ID"].ToString();
        //        UtilityModule.ConditionalComboFillWithDS(ref ddcolor, DSQ, 4, true, "--Select Color--");
        //        ddcolor.SelectedValue = ds.Tables[0].Rows[0]["COLOR_ID"].ToString();
        //        UtilityModule.ConditionalComboFillWithDS(ref ddshape, DSQ, 5, true, "--Select Shape--");
        //        ddshape.SelectedValue = ds.Tables[0].Rows[0]["SHAPE_ID"].ToString();
        //        UtilityModule.ConditionalComboFillWithDS(ref ddsize, DSQ, 6, true, "--SELECT SIZE--");
        //        ddsize.SelectedValue = ds.Tables[0].Rows[0]["SIZE_ID"].ToString();

        //        Session["finishedid"] = ds.Tables[0].Rows[0]["Item_Finished_id"].ToString();
        //        if (Convert.ToInt32(dquality.SelectedValue) > 0)
        //        {
        //            ql.Visible = true;

        //        }
        //        else
        //        {
        //            ql.Visible = false;

        //        }
        //        if (Convert.ToInt32(dddesign.SelectedValue) > 0)
        //        {
        //            dsn.Visible = true;
        //        }
        //        else
        //        {

        //            dsn.Visible = false;
        //        }
        //        int c = (ddcolor.SelectedIndex > 0 ? Convert.ToInt32(ddcolor.SelectedValue) : 0);
        //        if (c > 0)
        //        {
        //            clr.Visible = true;

        //        }
        //        else
        //        {
        //            clr.Visible = false;
        //        }


        //        int s = (ddshape.SelectedIndex > 0 ? Convert.ToInt32(ddshape.SelectedValue) : 0);
        //        if (s > 0)
        //        {
        //            shp.Visible = true;
        //        }

        //        else
        //        {
        //            shp.Visible = false;
        //        }

        //        int si = (ddsize.SelectedIndex > 0 ? Convert.ToInt32(ddsize.SelectedValue) : 0);
        //        if (si > 0)
        //        {
        //            sz.Visible = true;
        //        }
        //        else
        //        {
        //            sz.Visible = false;
        //        }
        //        UtilityModule.ConditionalComboFill(ref ddlunit, "SELECT u.UnitId,u.UnitName  FROM ITEM_MASTER i INNER JOIN  Unit u ON i.UnitTypeID = u.UnitTypeID where item_id=" + dditemname.SelectedValue + " And i.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Unit");
        //    }
        //    else
        //    {
        //        TxtProdCode.Text = "";
        //        TxtProdCode.Focus();
        //    }
        //}
        //else
        //{
        //    ddCatagory.Items.Clear();
        //}
        //fill_qty();
    }
    [System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
    public static string[] GetQuality(string prefixText, int count)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        con.Open();
        string strQuery = "Select ProductCode from ITEM_PARAMETER_MASTER IPM inner join item_Master IM on IM.Item_Id=IPM.Item_Id inner join CategorySeparate CS on CS.CategoryId=IM.Category_Id  where ProductCode Like  '" + prefixText + "%' And IPM.MasterCompanyId=" + MasterCompanyId;
        //string strQuery = "Select ProductCode from ITEM_PARAMETER_MASTER  where ProductCode Like  '" + prefixText + "%'";
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter(strQuery, con);
        da.Fill(ds);
        count = ds.Tables[0].Rows.Count;
        con.Close();
        List<string> ProductCode = new List<string>();
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            ProductCode.Add(ds.Tables[0].Rows[i][0].ToString());
        }
        con.Close();
        return ProductCode.ToArray();
    }
    private void Validated()
    {

    }
    protected void btnclose_Click(object sender, EventArgs e)
    {
        Session.Remove("prmid");
        Session.Remove("finishedid");
        Session.Remove("inhand");
        Session.Remove("stocktranid");
        Session.Remove("stockid");
    }
    protected void txtissue_TextChanged(object sender, EventArgs e)
    {
        //if (Session["VarcompanyNo"].ToString() == "5")
        //{
        //    if (ddProcessName.SelectedValue == "2" || ddProcessName.SelectedValue == "6")
        //    {
        //        string ChkMsg = CheckOderStockAssign();
        //        if (ChkMsg == "G")
        //        {
        //            LblError.Visible = true;
        //            LblError.Text = "Qty should not be greater than assigned stock";
        //            txtissue.Text = "";
        //            txtissue.Focus();
        //            return;
        //        }
        //    }
        //}

        //double stockqty = Convert.ToDouble(txtstock.Text == "" ? "0" : txtstock.Text);
        //double totalQty = Convert.ToDouble(txtconqty.Text == "" ? "0" : txtconqty.Text);
        //double PreQty = Math.Round(totalQty - Convert.ToDouble(TxtPendQty.Text == "" ? "0" : TxtPendQty.Text), 3);
        //double VarExcessQty = 0;
        if (Session["varcompanyNo"].ToString() == "16" && ddProcessName.SelectedValue != "1")
        {
           // VarExcessQty = 0;
        }
        else
        {
//            string Str = "Select PercentageExecssQtyForProcessIss From MasterSetting(Nolock)";

//            if (Session["varcompanyNo"].ToString() == "27")
//            {
//                Str = @"Select ExtraPercentage PercentageExecssQtyForProcessIss From ProcessConsumptionExtraPercentage(Nolock) 
//                Where CompanyID = " + ddCompName.SelectedValue + " And ProcessID = " + ddProcessName.SelectedValue + @" And 
//                IssueOrderID = " + ddOrderNo.SelectedValue + " And MasterCompanyID = " + Session["varCompanyId"];
//            }
//            VarExcessQty = Convert.ToDouble(SqlHelper.ExecuteScalar(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str));
        }
//        int VarEmployeeType = 0;
//        if (Session["varcompanyNo"].ToString() == "16" || Session["varcompanyNo"].ToString() == "28")
//        {
//            VarEmployeeType = Convert.ToInt32(SqlHelper.ExecuteScalar(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, @"Select Distinct EI.EmployeeType 
//                From Employee_ProcessOrderNo EPO(Nolock) 
//                JOIN Empinfo EI(Nolock) ON EI.EmpID = EPO.EmpID 
//                Where EPO.ProcessID = " + ddProcessName.SelectedValue + " And EPO.IssueOrderId = " + ddOrderNo.SelectedValue));

//            if (VarEmployeeType == 1)
//            {
//                VarExcessQty = 0;
//            }
//        }
        //totalQty = (totalQty * (100.0 + VarExcessQty) / 100);

        //double Qty = Convert.ToDouble(txtissue.Text == "" ? "0" : txtissue.Text);
        //double coneweight = UtilityModule.Getconeweight(DDconetype.SelectedItem.Text, Convert.ToInt16(txtnoofcone.Text == "" ? "0" : txtnoofcone.Text));
        //Qty = Qty - coneweight;
        //if (Qty + PreQty > totalQty || Qty > stockqty)
        //{
        //    txtissue.Text = "";
        //    LblError.Text = "Pls Enter Correct Qty ";
        //    LblError.Visible = true;
        //    txtissue.Focus();
        //    return;
        //}
        //else
        //{
        //    LblError.Visible = false;
        //}

    }
    private string CheckOderStockAssign()
    {
        string str = "";
        //try
        //{
        //    int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "", Convert.ToInt32(Session["varCompanyId"]));
        //    SqlParameter[] parparam = new SqlParameter[7];
        //    parparam[0] = new SqlParameter("@PrOrderid", ddOrderNo.SelectedValue);
        //    parparam[1] = new SqlParameter("@FinishedID", Varfinishedid);
        //    parparam[2] = new SqlParameter("@TxtQty", txtissue.Text);
        //    if (btnsave.Text == "Update")
        //    {
        //        parparam[3] = new SqlParameter("@PrtId", gvdetail.SelectedDataKey.Value);
        //    }
        //    else
        //    {
        //        parparam[3] = new SqlParameter("@PrtId", 0);
        //    }
        //    parparam[4] = new SqlParameter("@Message", SqlDbType.NVarChar, 2);
        //    parparam[5] = new SqlParameter("@LotNo", ddlotno.SelectedItem.Text);
        //    parparam[6] = new SqlParameter("@CompanyId", ddCompName.SelectedValue);
        //    parparam[4].Direction = ParameterDirection.Output;

        //    SqlHelper.ExecuteNonQuery(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "[Pro_CheckOrderStockAssign_Weaver]", parparam);
        //    str = parparam[4].Value.ToString();
        //}
        //catch (Exception ex)
        //{
        //    UtilityModule.MessageAlert(ex.Message, "Masters/RawMaterial/IndentRawIssue");
        //}
        return str;
    }
//    private void fill_qty(object sender = null)
//    {
//        txtconqty.Text = "0";
//        TxtPendQty.Text = "0";
//        int color = 0;
//        int quality = 0;
//        int design = 0;
//        int shape = 0;
//        int size = 0;
//        int shadeColor = 0;
//        if ((ql.Visible == true && dquality.SelectedIndex > 0) || ql.Visible != true)
//        {
//            quality = 1;
//        }
//        if (dsn.Visible == true && dddesign.SelectedIndex > 0 || dsn.Visible != true)
//        {
//            design = 1;
//        }
//        if (clr.Visible == true && ddcolor.SelectedIndex > 0 || clr.Visible != true)
//        {
//            color = 1;
//        }
//        if (shp.Visible == true && ddshape.SelectedIndex > 0 || shp.Visible != true)
//        {
//            shape = 1;
//        }
//        if (sz.Visible == true && ddsize.SelectedIndex > 0 || sz.Visible != true)
//        {
//            size = 1;
//        }
//        if (shd.Visible == true && ddlshade.SelectedIndex > 0 || shd.Visible != true)
//        {
//            shadeColor = 1;
//        }
//        //*************************
//        if (quality == 1 && design == 1 && color == 1 && shape == 1 && size == 1 && shadeColor == 1)
//        {
//            ddlotno.Items.Clear();
//            txtstock.Text = "";

//            int Varfinishedid = UtilityModule.getItemFinishedId(dditemname, dquality, dddesign, ddcolor, ddshape, ddsize, TxtProdCode, ddlshade, 0, "", Convert.ToInt32(Session["varCompanyId"]));
//            DataSet ds = null;
//            string Str;
//            switch (Session["varcompanyNo"].ToString())
//            {
//                case "9":
//                    // Str = @"SELECT ROUND(SUM(CASE WHEN CalType=0 or Caltype=2 Then Case When UnitId=1 Then (PD.Qty-isnull(CancelQty,0))*PD.Area*(PCD.IQTY+PCD.ILoss) Else (PD.Qty-Isnull(CancelQty,0))*PD.Area*(PCD.IQTY+PCD.ILOss)/10.76391 End Else 
//                    // Case When UnitId=1 Then (PD.Qty-Isnull(CancelQty,0))*(PCD.IQTY+PCD.ILoss) Else (PD.Qty-isnull(CancelQty,0))*(PCD.IQTY+PCD.ILoss) End End),3) Qty,[dbo].[Get_ProcessIssueQty] (PCD.IFinishedid,PM.IssueOrderId) IssQty 
//                    // From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
//                    // PROCESS_CONSUMPTION_DETAIL PCD Where PM.IssueOrderId=PD.IssueOrderId And PM.issueOrderId=PCD.IssueOrderId And PD.Issue_Detail_Id=PCD.Issue_Detail_Id And 
//                    // PCD.ProcessId=" + ddProcessName.SelectedValue + " And PCD.IssueOrderID=" + ddOrderNo.SelectedValue + @" And PCD.IFinishedid=" + Varfinishedid + @"
//                    // Group By PM.IssueOrderId,PCD.IFinishedid";
//                    // ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str);


//                    Str = @"SELECT Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN PM.UnitId=1 Then (PD.QTY-ISNULL(CANCELQTY,0))*PD.Area*(PCD.IQTY+PCD.ILoss) * 1.196
//                    Else CASE WHEN PM.UnitId=6 Then (PD.QTY-ISNULL(CANCELQTY,0))*PD.Area*(PCD.IQTY+PCD.ILoss)* 1.196/10.76391 else (PD.QTY-ISNULL(CANCELQTY,0))*PD.Area*(PCD.IQTY+PCD.ILoss)* 1.196/10.76391 END END ELSE 
//                    CASE WHEN PM.UnitId=1 Then (PD.QTY-ISNULL(CANCELQTY,0))*(PCD.IQTY+PCD.ILoss) else (PD.QTY-ISNULL(CANCELQTY,0))*(PCD.IQTY+PCD.ILoss) END END),3),0) Qty,[dbo].[Get_ProcessIssueQty] (PCD.IFinishedid,PM.IssueOrderId) IssQty 
//                    From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
//                    PROCESS_CONSUMPTION_DETAIL PCD Where PM.IssueOrderId=PD.IssueOrderId And PM.issueOrderId=PCD.IssueOrderId And PD.Issue_Detail_Id=PCD.Issue_Detail_Id And 
//                    PCD.ProcessId=" + ddProcessName.SelectedValue + " And PCD.IssueOrderID=" + ddOrderNo.SelectedValue + @" And PCD.IFinishedid=" + Varfinishedid + @"
//                    Group By PM.IssueOrderId,PCD.IFinishedid";
//                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str);
//                    break;
//                case "15":
//                    Str = @"SELECT ROUND(SUM(CASE WHEN UNITID=1 THEN (PD.QTY-ISNULL(CANCELQTY,0))*(case when vf.Katiwithexportsize=1 Then vf.Areamtr Else PD.Area End)*PCD.IQTY*1.196 
//                    ELSE (PD.QTY-ISNULL(CANCELQTY,0))*(case when vf.Katiwithexportsize=1 Then vf.Actualfullareasqyd Else PD.Area End)*PCD.IQTY END),3) QTY,[dbo].[Get_ProcessIssueQty] (PCD.IFinishedid,PM.IssueOrderId) IssQty 
//                    From PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
//                    PROCESS_CONSUMPTION_DETAIL PCD,V_FinishedItemDetail vf Where PM.IssueOrderId=PD.IssueOrderId And PM.issueOrderId=PCD.IssueOrderId And PD.Issue_Detail_Id=PCD.Issue_Detail_Id And 
//                    PD.Item_Finished_Id=vf.ITEM_FINISHED_ID  and
//                    PCD.ProcessId=" + ddProcessName.SelectedValue + " And PCD.IssueOrderID=" + ddOrderNo.SelectedValue + @" And PCD.IFinishedid=" + Varfinishedid + @"
//                    Group By PM.IssueOrderId,PCD.IFinishedid";
//                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, Str);
//                    break;
//                default:
//                    SqlParameter[] param = new SqlParameter[4];
//                    param[0] = new SqlParameter("@Processid", ddProcessName.SelectedValue);
//                    param[1] = new SqlParameter("@Issueorderid", ddOrderNo.SelectedValue);
//                    param[2] = new SqlParameter("@Mastercompanyid", Session["varcompanyid"]);
//                    param[3] = new SqlParameter("@Item_finished_id", Varfinishedid);
//                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "PRO_GETWEAVER_FINISHERCONSUMPTIONQTY", param);

//                    break;
//            }
//            if (ds.Tables[0].Rows.Count > 0)
//            {
//                txtconqty.Text = (ds.Tables[0].Rows[0]["qty"].ToString());
//                TxtPendQty.Text = (Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0]["qty"]) - Convert.ToDouble(ds.Tables[0].Rows[0]["IssQty"]), 3)).ToString();
//            }
//            // ////****************************
//            ////string str = @"Select Distinct GM.GodownID,GM.GodownName From GodownMaster GM,Stock S Where GM.GodownID=S.GodownID And QtyInHand>0 And CompanyId=" + ddCompName.SelectedValue + " And item_finished_id=" + Varfinishedid + " And GM.MasterCompanyId=" + Session["varCompanyId"] + @" Order By GodownName
//            //// select godownid From Modulewisegodown Where ModuleName='" + Page.Title + "'";

//            string str = @"Select Distinct GM.GodownID,GM.GodownName From GodownMaster GM JOIN Godown_Authentication GA ON GM.GodownId=GA.GodownId and GA.UserId=" + Session["varUserId"] + " and GA.MasterCompanyId=" + Session["varCompanyId"] + @"
//                            JOIN Stock S ON GM.GodownID=S.GodownID  Where S.QtyInHand>0 And S.CompanyId=" + ddCompName.SelectedValue + " And S.item_finished_id=" + Varfinishedid + " And GM.MasterCompanyId=" + Session["varCompanyId"] + @" Order By GM.GodownName
//                           select godownid From Modulewisegodown Where ModuleName='" + Page.Title + "'";

//            ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

//            UtilityModule.ConditionalComboFillWithDS(ref ddgodown, ds, 0, true, "--Select--");

//            if (ddgodown.Items.Count > 0)
//            {
//                if (ds.Tables[1].Rows.Count > 0)
//                {
//                    if (ddgodown.Items.FindByValue(ds.Tables[1].Rows[0]["godownid"].ToString()) != null)
//                    {
//                        ddgodown.SelectedValue = ds.Tables[1].Rows[0]["godownid"].ToString();
//                        if (sender != null)
//                        {
//                            ddgodown_SelectedIndexChanged(sender, new EventArgs());
//                        }

//                    }
//                }
//                else
//                {
//                    ddgodown.SelectedIndex = 1;
//                    if (sender != null)
//                    {
//                        ddgodown_SelectedIndexChanged(sender, new EventArgs());
//                    }
//                }
//                //Fill_GodownSelectedChange();
//                //Fill_LotNoSelectedChange();
//            }

//        }
//    }
    //protected void ChkForMtr_CheckedChanged(object sender, EventArgs e)
    //{
    //    if (ChkForMtr.Checked == false)
    //    {
    //        UtilityModule.ConditionalComboFill(ref ddsize, "select sizeid,sizeft from size where Shapeid=" + ddshape.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"] + "", true, "Size in Ft");
    //    }
    //    else
    //    {
    //        UtilityModule.ConditionalComboFill(ref ddsize, "select sizeid,sizemtr from size where Shapeid=" + ddshape.SelectedValue + " And MasterCompanyId=" + Session["varCompanyId"] + "", true, "Size in Mtr");
    //    }
    //}
    private void CHECKVALIDCONTROL()
    {
        LblError.Text = "";
        if (UtilityModule.VALIDDROPDOWNLIST(ddCompName) == false)
        {
            goto a;
        }
        if (UtilityModule.VALIDDROPDOWNLIST(ddProcessName) == false)
        {
            goto a;
        }
        if (UtilityModule.VALIDDROPDOWNLIST(ddempname) == false)
        {
            goto a;
        }
        if (UtilityModule.VALIDDROPDOWNLIST(ddOrderNo) == false)
        {
            goto a;
        }
        if (TdDDItemDesignName.Visible == true)
        {
            if (UtilityModule.VALIDDROPDOWNLIST(DDItemDesignName) == false)
            {
                goto a;
            }
        }
        if (UtilityModule.VALIDTEXTBOX(txtdate) == false)
        {
            goto a;
        }
        //if (UtilityModule.VALIDDROPDOWNLIST(ddCatagory) == false)
        //{
        //    goto a;
        //}
        //if (UtilityModule.VALIDDROPDOWNLIST(dditemname) == false)
        //{
        //    goto a;
        //}
        //if (ql.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(dquality) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (dsn.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(dddesign) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (clr.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(ddcolor) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (shp.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(ddshape) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (sz.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(ddsize) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (shd.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(ddlshade) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (UtilityModule.VALIDDROPDOWNLIST(ddlunit) == false)
        //{
        //    goto a;
        //}
        //if (UtilityModule.VALIDDROPDOWNLIST(ddgodown) == false)
        //{
        //    goto a;
        //}
        //if (UtilityModule.VALIDDROPDOWNLIST(ddlotno) == false)
        //{
        //    goto a;
        //}
        //if (TDBinNo.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(DDBinNo) == false)
        //    {
        //        goto a;
        //    }
        //}
        //if (TDTagno.Visible == true)
        //{
        //    if (UtilityModule.VALIDDROPDOWNLIST(DDTagno) == false)
        //    {
        //        goto a;
        //    }

        //}

        //if (UtilityModule.VALIDTEXTBOX(txtstock) == false)
        //{
        //    goto a;
        //}
        //if (UtilityModule.VALIDTEXTBOX(txtconqty) == false)
        //{
        //    goto a;
        //}
        //if (UtilityModule.VALIDTEXTBOX(txtissue) == false)
        //{
        //    goto a;
        //}
        else
        {
            goto B;
        }
    a:
        UtilityModule.SHOWMSG(LblError);
    B: ;
    }
    private void WayChallanFormatReport()
    {
        SqlParameter[] _array = new SqlParameter[3];
        _array[0] = new SqlParameter("@prmId", SqlDbType.Int);
        _array[1] = new SqlParameter("@ProcessId", SqlDbType.Int);
        _array[2] = new SqlParameter("@Trantype", SqlDbType.Int);

        _array[0].Value = ViewState["Prmid"];
        _array[1].Value = ddProcessName.SelectedValue;
        _array[2].Value = 0; //For Issue

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_RawMaterialIssuedDetail_Hafizia", _array);

        if (ds.Tables[0].Rows.Count > 0)
        {
            Session["rptFileName"] = "~\\Reports\\RptRawMaterialIssueWayChallanHafizia.rpt";
            Session["GetDataset"] = ds;
            Session["dsFileName"] = "~\\ReportSchema\\RptRawMaterialIssueWayChallanHafizia.xsd";

            StringBuilder stb = new StringBuilder();
            stb.Append("<script>");
            stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
        }
        else { ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true); }
    }
    private void WayChallanFormatBackReport()
    {
        SqlParameter[] _array = new SqlParameter[3];
        _array[0] = new SqlParameter("@prmId", SqlDbType.Int);
        _array[1] = new SqlParameter("@ProcessId", SqlDbType.Int);
        _array[2] = new SqlParameter("@Trantype", SqlDbType.Int);

        _array[0].Value = ViewState["Prmid"];
        _array[1].Value = ddProcessName.SelectedValue;
        _array[2].Value = 0; //For Issue

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_RawMaterialIssuedWayChallanBackFormat_Hafizia", _array);

        if (ds.Tables[1].Rows.Count > 0)
        {
            Session["rptFileName"] = "~\\Reports\\RptRawMaterialIssueWayChallanBackFormatHafizia.rpt";
            Session["GetDataset"] = ds;
            Session["dsFileName"] = "~\\ReportSchema\\RptRawMaterialIssueWayChallanBackFormatHafizia.xsd";

            StringBuilder stb = new StringBuilder();
            stb.Append("<script>");
            stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
        }
        else { ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true); }

    }
    private void ModRaziFormatReport()
    {
        SqlParameter[] _array = new SqlParameter[3];
        _array[0] = new SqlParameter("@prmId", SqlDbType.Int);
        _array[1] = new SqlParameter("@ProcessId", SqlDbType.Int);
        _array[2] = new SqlParameter("@Trantype", SqlDbType.Int);

        _array[0].Value = ViewState["Prmid"];
        _array[1].Value = ddProcessName.SelectedValue;
        _array[2].Value = 0; //For Issue

        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_RawMaterialIssuedDetail_MohdRazi", _array);

        if (ds.Tables[0].Rows.Count > 0)
        {
            Session["rptFileName"] = "~\\Reports\\RptRawMaterialIssueDetailMohdRazi.rpt";
            Session["GetDataset"] = ds;
            Session["dsFileName"] = "~\\ReportSchema\\RptRawMaterialIssueDetailMohdRazi.xsd";

            StringBuilder stb = new StringBuilder();
            stb.Append("<script>");
            stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
            ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
        }
        else { ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true); }
    }
    protected void btnpreview_Click(object sender, EventArgs e)
    {
        if (Session["varCompanyId"].ToString() == "9")
        {
            WayChallanFormatReport();
        }
        else if (Session["varCompanyId"].ToString() == "9" )
        {
            WayChallanFormatBackReport();
        }
        else if (Session["varCompanyId"].ToString() == "41")
        {
            ModRaziFormatReport();
        }
        else if (Session["varCompanyId"].ToString() == "8")
        {
            SqlParameter[] _array = new SqlParameter[4];
            _array[0] = new SqlParameter("@IssueOrderId", SqlDbType.Int);
            _array[1] = new SqlParameter("@prmId", SqlDbType.Int);
            _array[2] = new SqlParameter("@ProcessId", SqlDbType.Int);
            _array[3] = new SqlParameter("@Trantype", SqlDbType.Int);

            _array[0].Value = ddOrderNo.SelectedValue;
            _array[1].Value = ViewState["Prmid"];
            _array[2].Value = ddProcessName.SelectedValue;
            _array[3].Value = 0; //For Issue

            DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "Pro_RawMaterialIssuedSlip", _array);

            if (ds.Tables[0].Rows.Count > 0)
            {
                Session["rptFileName"] = "~\\Reports\\RptRawMaterialIssueRecSlip.rpt";
                Session["GetDataset"] = ds;
                Session["dsFileName"] = "~\\ReportSchema\\RptRawMaterialIssueRecSlip.xsd";

                StringBuilder stb = new StringBuilder();
                stb.Append("<script>");
                stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
            }
            else { ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true); }

        }
        else
        {
            string str = "";
            switch (Session["varcompanyId"].ToString())
            {
                case "9":

                    str = @"SELECT PRM.Date, PRM.ChalanNo,PRM.trantype, PRT.IssueQuantity,PRT.Lotno, GM.GodownName, EI.EmpName,
                        EI.Address, CI.CompanyName, CI.CompAddr1, CI.CompAddr2, CI.CompAddr3, CI.CompTel,
                        Vf.ITEM_NAME, Vf.QualityName, Vf.designName, Vf.ColorName,Vf.ShadeColorName, Vf.ShapeName,Vf.SizeMtr, PNM.PROCESS_NAME,
                        PRM.Prorderid,U.UnitName,Om.LocalOrder,CI.GSTNO,EI.GSTNO as EmpGstno ,isnull(PRT.TagNo,'') as TagNo,
                        isnull(PRM.Remark,'') as Remark
                        FROM  ProcessRawMaster PRM  INNER JOIN  ProcessRawTran PRT ON PRM.PRMid=PRT.PRMid 
                        INNER JOIN GodownMaster GM ON GM.GoDownID=PRT.Godownid 
                        INNER JOIN EmpInfo EI ON PRM.Empid=EI.EmpId 
                        INNER JOIN CompanyInfo CI ON PRM.Companyid=CI.CompanyId 
                        INNER JOIN PROCESS_NAME_MASTER PNM ON PRM.Processid=PNM.PROCESS_NAME_ID
                        INNER JOIN V_FinishedItemDetail Vf ON PRT.Finishedid=Vf.ITEM_FINISHED_ID 
                        INNER JOIN Unit U ON U.UnitId=PRT.UnitId
                        inner join(select  Distinct IssueOrderId,Orderid from PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PID)as Process_issue_Detail
                        on Process_issue_Detail.IssueOrderId=prm.Prorderid
                        inner join OrderMaster OM on OM.OrderId=Process_issue_Detail.Orderid
                        Where PRM.TypeFlag = 0 And PRM.PrmId=" + ViewState["Prmid"];
                    DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        Session["rptFileName"] = "~\\Reports\\RptRawMaterialSlipforHafiziaNew.rpt";
                        Session["GetDataset"] = ds;
                        Session["dsFileName"] = "~\\ReportSchema\\RptRawMaterialSlipforHafizia.xsd";

                        StringBuilder stb = new StringBuilder();
                        stb.Append("<script>");
                        stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);
                    }
                    else { ScriptManager.RegisterStartupScript(Page, GetType(), "opn1", "alert('No Record Found!');", true); }

                    break;
                case "16":
                case "28":
                    Boolean Istqueryflag = true;
                    if (Session["varcompanyNo"].ToString() == "9")
                    {
                        Istqueryflag = true;

                    }
                    else
                    {
                        if (ddProcessName.SelectedValue == "1")
                        {
                            Istqueryflag = true;
                        }
                        else
                        {
                            if (variable.VarFinishingNewModuleWise == "1")
                            {
                                Istqueryflag = false;
                            }
                            else
                            {
                                Istqueryflag = true;
                            }
                        }
                    }
                    if (Istqueryflag == true)
                    {
                        str = @" Select PM.Date, PM.ChalanNo, PM.trantype, PT.IssueQuantity, 
                                PT.Lotno, GM.GodownName, Case When IsNull(EI.EmpName, '') = '' Then 
	                                (Select Distinct EII.EmpName + ', ' 
		                                From Employee_ProcessOrderNo EPO(Nolock) 
		                                JOIN Empinfo EII(Nolock) ON EII.EmpID = EPO.EmpID And EPO.IssueOrderId = PM.Prorderid And EPO.ProcessId = PM.Processid) 
                                Else EI.EmpName End EmpName, 
                                Case When IsNull(EI.Address, '') = '' Then 
	                                (Select Distinct EII.Address + ', ' 
		                                From Employee_ProcessOrderNo EPO(Nolock) 
		                                JOIN Empinfo EII(Nolock) ON EII.EmpID = EPO.EmpID And EPO.IssueOrderId = PM.Prorderid And EPO.ProcessId = PM.Processid) 
                                Else EI.Address End Address,
                                CI.CompanyName, BM.BranchAddress CompAddr1, '' CompAddr2, 
                                '' CompAddr3, CI.CompTel, vf.ITEM_NAME, vf.QualityName, vf.designName, 
                                vf.ColorName, vf.ShadeColorName, vf.ShapeName, vf.SizeMtr, PNM.PROCESS_NAME, 
                                (Select Distinct Cast(Prorderid As Nvarchar) + ', ' 
									From ProcessRawMaster PMS(Nolock) Where PMS.TranType = PM.TranType And PMS.chalanno = PM.chalanno For XML Path('')) Prorderid, 
                                Case When IsNull(EI.GSTNo, '') = '' Then 
	                                (Select Distinct EII.GSTNo + ', ' 
		                                From Employee_ProcessOrderNo EPO(Nolock) 
		                                JOIN Empinfo EII(Nolock) ON EII.EmpID = EPO.EmpID And EPO.IssueOrderId = PM.Prorderid And EPO.ProcessId = PM.Processid) 
                                Else EI.GSTNo End empgstin,                               
                                CI.GSTNo,PT.TAGNO,PT.BINNO, 
                                (Select Distinct CII.CustomerCode + ', '
		                                From PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PID(Nolock) 
		                                JOIN OrderMaster OM(Nolock) ON OM.OrderiD = PID.OrderiD 
                                        JOIN CustomerInfo CII(Nolock) ON CII.CustomerID = OM.CustomerID 
                                        Where PID.IssueOrderId = PM.Prorderid For XML Path('')) OrderNo, BM.GstNo BranchGstNo, 
                                0 ReportType, PM.Prorderid IssueOrderID 
                                From ProcessRawMaster PM 
                                join ProcessRawTran PT on PM.PRMid=PT.PRMid 
                                JOIN BranchMaster BM ON BM.ID = PM.BranchID 
                                join CompanyInfo ci on PM.Companyid=ci.CompanyId 
                                join V_FinishedItemDetail vf on PT.Finishedid=vf.ITEM_FINISHED_ID 
                                join GodownMaster GM on PT.Godownid=GM.GoDownID 
                                LEFT join EmpInfo Ei on PM.Empid=ei.EmpId 
                                join PROCESS_NAME_MASTER PNM on PM.Processid=PNM.PROCESS_NAME_ID 
                                Where PM.TypeFlag = 0 ";
                    }
                    else
                    {
                        str = @" select PM.Date, PM.ChalanNo, PM.trantype, PT.IssueQuantity, 
                           PT.Lotno, GM.GodownName, EI.EmpName, '' Address, CI.CompanyName, BM.BranchAddress CompAddr1, '' CompAddr2, 
                           '' CompAddr3, CI.CompTel, vf.ITEM_NAME, vf.QualityName, vf.designName, 
                           vf.ColorName, vf.ShadeColorName, vf.ShapeName, vf.SizeMtr, PNM.PROCESS_NAME, 
                           PM.Prorderid, '' as empgstin, CI.GSTNo,PT.TAGNO,PT.BINNO, BM.GstNo BranchGstNo, 
                           0 ReportType, PM.Prorderid IssueOrderID 
                           From ProcessRawMaster PM 
                           JOIN BranchMaster BM ON BM.ID = PM.BranchID 
                           inner join ProcessRawTran PT on PM.PRMid=PT.PRMid 
                           inner join CompanyInfo ci on PM.Companyid=ci.CompanyId 
                           inner join V_FinishedItemDetail vf on PT.Finishedid=vf.ITEM_FINISHED_ID 
                           inner join GodownMaster GM on PT.Godownid=GM.GoDownID 
                           inner join V_GetCommaSeparateEmployee Ei on PM.Prorderid=ei.Issueorderid and ei.Processid=" + ddProcessName.SelectedValue + @" 
                           inner join PROCESS_NAME_MASTER PNM on PM.Processid=PNM.PROCESS_NAME_ID 
                           Where PM.TypeFlag = 0 and PM.Processid=" + ddProcessName.SelectedValue;
                    }
                    //if (variable.VarMANYFOLIORAWISSUE_SINGLECHALAN == "1")
                    //{
                    //    str = str + @" And PM.TranType = 0 And PM.chalanno='" + txtchalanno.Text + "'";
                    //}
                    //else
                    //{
                        str = str + @" And PM.Prmid=" + ViewState["Prmid"];
                  //  }
                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Session["rptFileName"] = "~\\Reports\\RptRawIssueRecDuplicateNew.rpt";
                        if (Convert.ToInt32(Session["varSubCompanyId"]) == 283 || Convert.ToInt32(Session["varSubCompanyId"]) == 282)
                        {
                            Session["rptFileName"] = "~\\Reports\\RptRawIssueRecDuplicateNew2.rpt";
                        }
                        Session["GetDataset"] = ds;
                        Session["dsFileName"] = "~\\ReportSchema\\RptRawIssueRecDuplicateNew.xsd";

                        StringBuilder stb = new StringBuilder();
                        stb.Append("<script>");
                        stb.Append("window.open('../../ViewReport.aspx', 'nwwin', 'toolbar=0, titlebar=1,  top=0px, left=0px, scrollbars=1, resizable = yes');</script>");
                        ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn", stb.ToString(), false);

                    }
                    else { ScriptManager.RegisterStartupScript(Page, GetType(), "opn2", "alert('No Record Found!');", true); }

                    break;
                case "42":
                    if (variable.VarMANYFOLIORAWISSUE_SINGLECHALAN == "1")
                    {
                        //Session["ReportPath"] = "Reports/RptRawIssueRecDuplicateManyfolioonsinglechalan.rpt";
                        //Session["CommanFormula"] = "{ProcessRawMaster.chalanno}='" + txtchalanno.Text + "' and {ProcessRawMaster.trantype}=0";
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Preview();", true);
                    }
                    else
                    {
                        Session["ReportPath"] = "Reports/RptRawIssueRecDuplicateVikramMirzapur.rpt";
                        Session["CommanFormula"] = "{ProcessRawMaster.PrmId}=" + ViewState["Prmid"];
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Preview();", true);
                    }
                    break;
                default:
                    if (variable.VarMANYFOLIORAWISSUE_SINGLECHALAN == "1")
                    {
                        //Session["ReportPath"] = "Reports/RptRawIssueRecDuplicateManyfolioonsinglechalan.rpt";
                        //Session["CommanFormula"] = "{ProcessRawMaster.chalanno}='" + txtchalanno.Text + "' and {ProcessRawMaster.trantype}=0";
                        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Preview();", true);
                    }
                    else
                    {
                        Session["ReportPath"] = "Reports/RptRawIssueRecDuplicate.rpt";
                        Session["CommanFormula"] = "{ProcessRawMaster.PrmId}=" + ViewState["Prmid"];
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Preview();", true);
                    }
                    break;
            }
        }
        //Session["ReportPath"] = "Reports/RptRawIssueRec.rpt";
        //Session["CommanFormula"] = "{ProcessRawMaster.PrmId}=" + ViewState["Prmid"];
        //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Preview();", true);
    }
    protected void TxtPOrderNo_TextChanged(object sender, EventArgs e)
    {
        LblError.Text = "";
        String VarPOrderNo = TxtPOrderNo.Text == "" ? "0" : TxtPOrderNo.Text;

        ////        //DataSet Ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "Select * from TEMP_PROCESS_ISSUE_MASTER_NEW Where IssueOrderId='" + VarPOrderNo + "'");
        //        string str = @"SELECT VIM.COMPANYID,OM.CUSTOMERID,VID.ORDERID,VIM.PROCESSID,VIM.EMPID,VIM.ISSUEORDERID
        //                      FROM VIEW_PROCESS_ISSUE_MASTER VIM INNER JOIN VIEW_PROCESS_ISSUE_DETAIL VID ON VIM.ISSUEORDERID=VID.ISSUEORDERID
        //                      AND VIM.PROCESSID=VID.PROCESS_NAME_ID AND VIM.STATUS<>'CANCELED'
        //                      INNER JOIN ORDERMASTER OM ON VID.ORDERID=OM.ORDERID WHere VIm.issueorderid='" + VarPOrderNo + "'";


        string str = @"SELECT DISTINCT VIM.COMPANYID, OM.CUSTOMERID, VID.ORDERID, VIM.PROCESSID, 
            CASE WHEN VIM.EMPID = 0 THEN (Select Top 1 EmpID From Employee_ProcessOrderNo EPO(Nolock) 
                                                Where EPO.IssueOrderId = VIM.IssueOrderId And EPO.ProcessId = VIM.PROCESSID
                                         ) ELSE VIM.EMPID END EMPID, 
            VIM.ISSUEORDERID, IsNull(VIM.CHALLANNO, 0) CHALLANNO,vid.Item_Finished_Id 
            FROM VIEW_PROCESS_ISSUE_MASTER VIM 
            INNER JOIN VIEW_PROCESS_ISSUE_DETAIL VID ON VIM.ISSUEORDERID=VID.ISSUEORDERID AND VIM.PROCESSID=VID.PROCESS_NAME_ID AND VIM.STATUS <> 'CANCELED'
            INNER JOIN ORDERMASTER OM ON VID.ORDERID = OM.ORDERID 
            Where --IsNull(VIM.DEPARTMENTTYPE, 0) = 0 And 
            VIM.COMPANYID = " + ddCompName.SelectedValue + " And VIm.ChallanNo = '" + VarPOrderNo + "'";

        if (Session["varcompanyId"].ToString() == "16" || Session["varcompanyId"].ToString() == "28")
        {
            str = str + @" And VIM.Status <> 'Complete'";
        }
        if (Session["varcompanyid"].ToString() == "16")
        {
            str = str + " and vim.processid=1";
        }
        if (ddProcessName.SelectedIndex > 0)
        {
            str = str + " and vim.processid=" + ddProcessName.SelectedValue;
        }

        DataSet Ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

        if (Ds.Tables[0].Rows.Count > 0)
        {
            hdnchallanfinishedid.Value = Ds.Tables[0].Rows[0]["Item_Finished_Id"].ToString();
            ddProcessName.SelectedValue = Ds.Tables[0].Rows[0]["ProcessId"].ToString();
            ProcessNameSelectedIndexChange();
            ddempname.SelectedValue = Ds.Tables[0].Rows[0]["EmpId"].ToString();
            EmpNameSelectedIndexChange();
            if (ddOrderNo.Items.FindByValue(Ds.Tables[0].Rows[0]["ISSUEOrderId"].ToString()) != null)
            {
                ddOrderNo.SelectedValue = Ds.Tables[0].Rows[0]["IssueOrderId"].ToString();
            }
            else
            {
                LblError.Text = "This Folio No. does not exists!!!!!";
                LblError.Visible = true;
                return;

            }
            OrderNoSelectedIndexChange();
            if (DDChallanNo.Items.Count > 0)
            {
                DDChallanNo.SelectedIndex = 1;
               
                ChallanNoSelectedIndexChange();
            }
//            if (dquality.Items.Count > 0)
//            {
//                UtilityModule.ConditionalComboFill(ref ddlshade, @"SELECT DISTINCT SC.ShadecolorId,SC.ShadeColorName FROM ITEM_PARAMETER_MASTER IPM INNER JOIN
//                PROCESS_CONSUMPTION_DETAIL PCD ON IPM.ITEM_FINISHED_ID=PCD.IFinishedId INNER JOIN ShadeColor SC ON IPM.SHADECOLOR_ID=SC.ShadecolorId 
//                Where PCD.Issueorderid=" + ddOrderNo.SelectedValue + " And IPM.Quality_Id=" + dquality.SelectedValue + " And IPM.MasterCompanyId=" + Session["varCompanyId"] + "", true, "Select Shadecolor");
//            }
            //fill_qty();
            //Fill_GodownSelectedChange();
            //Fill_LotNoSelectedChange();
        }
        else
        {
            if (ddOrderNo.Items.Count > 0)
            {
                ddOrderNo.SelectedIndex = 0;
               // fill_Grid_ShowConsmption();
            }
            //if (ddCatagory.Items.Count > 0)
            //{
            //    ddCatagory.SelectedIndex = 0;
            //}
            TxtPOrderNo.Text = "";
            TxtPOrderNo.Focus();
        }
    }
    private void fill_Grid_ShowConsmption()
    {
        DataSet ds = null;
        string strsql = "";

        try
        {
            switch (Session["varcompanyId"].ToString())
            {
                case "9":
                    ////                    strsql = @"SELECT  VF1.Category_Name,VF1.Item_Name,VF1.QualityName+Space(2)+VF1.DesignName+Space(2)+VF1.ColorName+Space(2)+VF1.ShapeName+Space(2)+
                    ////                            CASE WHEN PM.UnitId=1 Then VF1.SizeMtr else VF1.SizeFt END+Space(2)+VF1.ShadeColorName Description,
                    ////                            Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN PM.UnitId=1 Then PD.QTY*PD.Area*(OCD.IQTY+OCD.ILoss) * 1.196
                    ////                            Else CASE WHEN PM.UnitId=6 Then PD.QTY*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196/10.76391 else PD.QTY*PD.Area*(OCD.IQTY+OCD.ILoss) END END ELSE 
                    ////                            CASE WHEN PM.UnitId=1 Then PD.QTY*(OCD.IQTY+OCD.ILoss) else PD.QTY*(OCD.IQTY+OCD.ILoss) END END),3),0) ConsmpQTY,
                    ////                            [dbo].[Get_ProcessIssueQty2] (OCD.IFINISHEDID,PM.Issueorderid," + ddProcessName.SelectedValue + @") IssQty,
                    ////                            Round(Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN PM.UnitId=1 Then PD.QTY*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196
                    ////                            Else CASE WHEN PM.UnitId=6 Then PD.QTY*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196/10.76391 else PD.QTY*PD.Area*(OCD.IQTY+OCD.ILoss) END END ELSE CASE WHEN PM.UnitId=1 Then PD.QTY*(OCD.IQTY+OCD.ILoss) else 
                    ////                            PD.QTY*(OCD.IQTY+OCD.ILoss) END END),3),0)-[dbo].[Get_ProcessIssueQty2] (OCD.IFINISHEDID,PM.Issueorderid," + ddProcessName.SelectedValue + @"),3) PendQty 
                    ////                            FROM PROCESS_CONSUMPTION_DETAIL OCD,PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
                    ////                            V_FinishedItemDetail VF1 Where PM.IssueOrderid=PD.IssueOrderid And OCD.Issueorderid=PD.Issueorderid And OCD.Issue_Detail_Id=PD.Issue_Detail_Id And 
                    ////                            VF1.ITEM_FINISHED_ID=OCD.IFINISHEDID And PM.Issueorderid=" + ddOrderNo.SelectedValue + " and Ocd.PROCESSID=" + ddProcessName.SelectedValue + " And VF1.MasterCompanyId=" + Session["varCompanyId"] + @"
                    ////                            Group By VF1.Category_Name,VF1.Item_Name,VF1.QualityName,VF1.DesignName,VF1.ColorName,VF1.ShapeName,PM.UnitId,VF1.SizeMtr,VF1.SizeFt,
                    ////                            VF1.ShadeColorName,OCD.IFINISHEDID,PM.Issueorderid";

                    strsql = @"SELECT  VF1.Category_Name,VF1.Item_Name,VF1.QualityName+Space(2)+VF1.DesignName+Space(2)+VF1.ColorName+Space(2)+VF1.ShapeName+Space(2)+
                            CASE WHEN PM.UnitId=1 Then VF1.SizeMtr else VF1.SizeFt END+Space(2)+VF1.ShadeColorName Description,
                            Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN PM.UnitId=1 Then (PD.QTY-ISNULL(PD.CANCELQTY,0))*PD.Area*(OCD.IQTY+OCD.ILoss) * 1.196
                            Else CASE WHEN PM.UnitId=6 Then (PD.QTY-ISNULL(PD.CANCELQTY,0))*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196/10.76391 else (PD.QTY-ISNULL(PD.CANCELQTY,0))*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196/10.76391 END END ELSE 
                            CASE WHEN PM.UnitId=1 Then (PD.QTY-ISNULL(PD.CANCELQTY,0))*(OCD.IQTY+OCD.ILoss) else (PD.QTY-ISNULL(PD.CANCELQTY,0))*(OCD.IQTY+OCD.ILoss) END END),3),0) ConsmpQTY,
                            [dbo].[Get_ProcessIssueQty2] (OCD.IFINISHEDID,PM.Issueorderid," + ddProcessName.SelectedValue + @") IssQty,
                            Round(Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN PM.UnitId=1 Then (PD.QTY-ISNULL(PD.CANCELQTY,0))*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196
                            Else CASE WHEN PM.UnitId=6 Then (PD.QTY-ISNULL(PD.CANCELQTY,0))*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196/10.76391 else (PD.QTY-ISNULL(PD.CANCELQTY,0))*PD.Area*(OCD.IQTY+OCD.ILoss)* 1.196/10.76391 END END ELSE CASE WHEN PM.UnitId=1 Then (PD.QTY-ISNULL(PD.CANCELQTY,0))*(OCD.IQTY+OCD.ILoss) else 
                            (PD.QTY-ISNULL(PD.CANCELQTY,0))*(OCD.IQTY+OCD.ILoss) END END),3),0)-[dbo].[Get_ProcessIssueQty2] (OCD.IFINISHEDID,PM.Issueorderid," + ddProcessName.SelectedValue + @"),3) PendQty 
                            FROM PROCESS_CONSUMPTION_DETAIL OCD,PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
                            V_FinishedItemDetail VF1 Where PM.IssueOrderid=PD.IssueOrderid And OCD.Issueorderid=PD.Issueorderid And OCD.Issue_Detail_Id=PD.Issue_Detail_Id And 
                            VF1.ITEM_FINISHED_ID=OCD.IFINISHEDID And PM.Issueorderid=" + ddOrderNo.SelectedValue + " and Ocd.PROCESSID=" + ddProcessName.SelectedValue + " And VF1.MasterCompanyId=" + Session["varCompanyId"] + @"
                            Group By VF1.Category_Name,VF1.Item_Name,VF1.QualityName,VF1.DesignName,VF1.ColorName,VF1.ShapeName,PM.UnitId,VF1.SizeMtr,VF1.SizeFt,
                            VF1.ShadeColorName,OCD.IFINISHEDID,PM.Issueorderid";
                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);
                    break;
                case "15":
                    strsql = @"SELECT VF1.Category_Name,VF1.Item_Name,VF1.QualityName+Space(2)+VF1.DesignName+Space(2)+VF1.ColorName+Space(2)+VF1.ShapeName+Space(2)+
                            CASE WHEN PM.UnitId=1 Then VF1.SizeMtr else VF1.SizeFt END+Space(2)+VF1.ShadeColorName Description,
                            Isnull(Round(Sum(CASE WHEN PM.UnitId=1 Then PD.Qty*(case when vf.Katiwithexportsize=1 Then vf.Areamtr Else PD.Area End)*OCD.IQTY*1.196 else PD.Qty*(case when vf.Katiwithexportsize=1 Then vf.Actualfullareasqyd Else PD.Area End)*OCD.IQTY END),3),0) ConsmpQTY,
                            [dbo].[Get_ProcessIssueQty2] (OCD.IFINISHEDID,PM.Issueorderid," + ddProcessName.SelectedValue + ") IssQty,Round(Isnull(Round(Sum(CASE WHEN PM.UnitId=1 Then PD.Qty*(case when vf.Katiwithexportsize=1 Then vf.Areamtr Else PD.Area End)*OCD.IQTY*1.196 else PD.Qty*(case when vf.Katiwithexportsize=1 Then vf.Actualfullareasqyd Else PD.Area End)*OCD.IQTY END),3),0)-[dbo].[Get_ProcessIssueQty2] (OCD.IFINISHEDID,PM.Issueorderid," + ddProcessName.SelectedValue + @"),3) PendQty 
                            FROM PROCESS_CONSUMPTION_DETAIL OCD,PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
                            V_FinishedItemDetail VF1,V_Finisheditemdetail vf Where PM.IssueOrderid=PD.IssueOrderid And OCD.Issueorderid=PD.Issueorderid And OCD.Issue_Detail_Id=PD.Issue_Detail_Id And 
                            VF1.ITEM_FINISHED_ID=OCD.IFINISHEDID and PD.Item_finished_id=vf.item_finished_id And PM.Issueorderid=" + ddOrderNo.SelectedValue + " and  Ocd.PROCESSID=" + ddProcessName.SelectedValue + " And VF1.MasterCompanyId=" + Session["varCompanyId"] + @"
                            Group By VF1.Category_Name,VF1.Item_Name,VF1.QualityName,VF1.DesignName,VF1.ColorName,VF1.ShapeName,PM.UnitId,VF1.SizeMtr,VF1.SizeFt,
                            VF1.ShadeColorName,OCD.IFINISHEDID,PM.Issueorderid";
                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);
                    break;
                default:
                    SqlParameter[] param = new SqlParameter[3];
                    param[0] = new SqlParameter("@Processid", ddProcessName.SelectedValue);
                    param[1] = new SqlParameter("@ISSUEORDERID", ddOrderNo.SelectedValue);
                    param[2] = new SqlParameter("@MASTERCOMPANYID", Session["varcompanyId"]);
                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.StoredProcedure, "PRO_FILLWEAVER_FINISHERCONSUMPTION", param);
                    #region
                    //                    strsql = @"SELECT VF1.Category_Name,VF1.Item_Name,VF1.QualityName+Space(2)+VF1.DesignName+Space(2)+VF1.ColorName+Space(2)+VF1.ShapeName+Space(2)+
                    //                            CASE WHEN PM.UnitId=1 Then VF1.SizeMtr else VF1.SizeFt END+Space(2)+VF1.ShadeColorName Description,
                    //                            Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN PM.UnitId=1 Then PD.Qty*PD.Area*OCD.IQTY*1.196 else PD.Qty*PD.Area*OCD.IQTY END ELSE 
                    //                            CASE WHEN PM.UnitId=1 Then PD.Qty*OCD.IQTY*1.196 else PD.Qty*OCD.IQTY END END),3),0) ConsmpQTY,
                    //                            [dbo].[Get_ProcessIssueQty] (OCD.IFINISHEDID,PM.Issueorderid) IssQty,Round(Isnull(Round(Sum(CASE WHEN PM.CalType=0 or PM.Caltype=2 THEN CASE WHEN 
                    //                            PM.UnitId=1 Then PD.Qty*PD.Area*OCD.IQTY*1.196 else PD.Qty*PD.Area*OCD.IQTY END ELSE CASE WHEN PM.UnitId=1 Then PD.Qty*OCD.IQTY*1.196 else 
                    //                            PD.Qty*OCD.IQTY END END),3),0)-[dbo].[Get_ProcessIssueQty] (OCD.IFINISHEDID,PM.Issueorderid),3) PendQty 
                    //                            FROM PROCESS_CONSUMPTION_DETAIL OCD,PROCESS_ISSUE_MASTER_" + ddProcessName.SelectedValue + " PM,PROCESS_ISSUE_DETAIL_" + ddProcessName.SelectedValue + @" PD,
                    //                            V_FinishedItemDetail VF1,CategorySeparate Cs Where PM.IssueOrderid=PD.IssueOrderid And OCD.Issueorderid=PD.Issueorderid And OCD.Issue_Detail_Id=PD.Issue_Detail_Id And 
                    //                            VF1.ITEM_FINISHED_ID=OCD.IFINISHEDID And PM.Issueorderid=" + ddOrderNo.SelectedValue + " And VF1.CATEGORY_ID=Cs.CategoryId and Cs.id=1 and Ocd.PROCESSID=" + ddProcessName.SelectedValue + " And VF1.MasterCompanyId=" + Session["varCompanyId"] + @"
                    //                            Group By VF1.Category_Name,VF1.Item_Name,VF1.QualityName,VF1.DesignName,VF1.ColorName,VF1.ShapeName,PM.UnitId,VF1.SizeMtr,VF1.SizeFt,
                    //                            VF1.ShadeColorName,OCD.IFINISHEDID,PM.Issueorderid";
                    //                    ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, strsql);
                    #endregion
                    break;
            }
            //GDGridShow.DataSource = ds;
            //GDGridShow.DataBind();
        }
        catch (Exception ex)
        {
            LblError.Visible = true;
            LblError.Text = ex.Message;
            Logs.WriteErrorLog("Masters_process_ProcessRawIssue|fill_Data_grid|" + ex.Message);
        }

    }
    private void Fill_Category_SelectedChange()
    {
//        if (ddCatagory.SelectedIndex >= 0)
//        {
//            ddlcategorycange();
//            //***********Sample
//            if (hnissampleorder.Value == "1")
//            {
//                UtilityModule.ConditionalComboFill(ref dditemname, "select ITEM_ID,ITEM_NAME From Item_Master Where CATEGORY_ID=" + ddCatagory.SelectedValue + " and Mastercompanyid=" + Session["varcompanyId"] + " order by ITEM_NAME", true, "--Select Item--");
//            }
//            else
//            {
//                UtilityModule.ConditionalComboFill(ref dditemname, @"SELECT DISTINCT dbo.ITEM_MASTER.ITEM_ID, dbo.ITEM_MASTER.ITEM_NAME FROM 
//            dbo.ITEM_PARAMETER_MASTER INNER JOIN PROCESS_CONSUMPTION_DETAIL ON dbo.ITEM_PARAMETER_MASTER.ITEM_FINISHED_ID = PROCESS_CONSUMPTION_DETAIL.IFinishedId INNER JOIN
//            dbo.ITEM_MASTER ON dbo.ITEM_PARAMETER_MASTER.ITEM_ID = dbo.ITEM_MASTER.ITEM_ID
//            Where PROCESS_CONSUMPTION_DETAIL.issueorderid=" + ddOrderNo.SelectedValue + " and Processid=" + ddProcessName.SelectedValue + " and item_master.category_id=" + ddCatagory.SelectedValue + " And ITEM_PARAMETER_MASTER.MasterCompanyId=" + Session["varCompanyId"] + "", true, "--Select Item--");
//            }

//            if (dditemname.Items.Count > 0)
//            {
//                dditemname.SelectedIndex = 1;
//                ItemName_SelectChange();
//            }
//        }
    }
    protected void gvdetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        con.Open();
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            int VarPrtID = Convert.ToInt32(gvdetail.DataKeys[e.RowIndex].Value);
            ViewState["Prmid"] = SqlHelper.ExecuteScalar(Tran, CommandType.Text, "Select PrmId from ProcessRawTran Where PrtId=" + VarPrtID);
            SqlParameter[] arr = new SqlParameter[6];

            arr[0] = new SqlParameter("@PrtID", SqlDbType.Int);
            arr[1] = new SqlParameter("@RowCount", SqlDbType.Int);
            arr[2] = new SqlParameter("@TranType", SqlDbType.Int);
            arr[3] = new SqlParameter("@Msg", SqlDbType.VarChar, 100);
            arr[4] = new SqlParameter("@userid", Session["varuserid"]);
            arr[5] = new SqlParameter("@Mastercompanyid", Session["varcompanyid"]);

            arr[0].Value = VarPrtID;
            arr[1].Value = 2;
            arr[2].Value = 0;
            if (gvdetail.Rows.Count == 1)
            {
                arr[1].Value = 1;
            }
            arr[3].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_PROCESS_RAW_ISSUE_RECEIVE_DELETE", arr);
            if (arr[3].Value.ToString() != "")
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "altdel", "alert('" + arr[3].Value.ToString() + "');", true);
            }
            else
            {
                LblError.Text = "Row Item Deleted successfully.";
            }
            Tran.Commit();
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            LblError.Visible = true;
            LblError.Text = ex.Message;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
        Fill_Grid();
    }
    private void Fill_Temp_OrderNo()
    {
        SqlHelper.ExecuteNonQuery(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "DELETE TEMP_PROCESS_ISSUE_MASTER_NEW");
        DataSet Ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "Select * from PROCESS_NAME_MASTER");
        if (Ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
            {
                SqlHelper.ExecuteNonQuery(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, "Insert into TEMP_PROCESS_ISSUE_MASTER_NEW SELECT PM.Companyid,OM.Customerid,PD.Orderid," + Ds.Tables[0].Rows[i]["Process_Name_Id"] + " ProcessId,PM.Empid,PM.IssueOrderid FROM PROCESS_ISSUE_MASTER_" + Ds.Tables[0].Rows[i]["Process_Name_Id"] + " PM,PROCESS_ISSUE_DETAIL_" + Ds.Tables[0].Rows[i]["Process_Name_Id"] + " PD,OrderMaster OM Where PM.IssueOrderid=PD.IssueOrderid And PD.Orderid=OM.Orderid");
            }
        }
    }
    protected void txtWeaverIdNoscan_TextChanged(object sender, EventArgs e)
    {
        FillProcess_Employee(sender);
    }
    protected void FillProcess_Employee(object sender = null)
    {
        string str = @"SELECT Top(1) EMP.ProcessId,EI.EmpId FROM EMPLOYEE_PROCESSORDERNO EMP INNER JOIN EMPINFO EI ON EMP.EMPID=EI.EMPID WHERE EI.EMPCODE='" + txtWeaverIdNoscan.Text + "'";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);
        if (ds.Tables[0].Rows.Count > 0)
        {
            if (ddProcessName.Items.FindByValue(ds.Tables[0].Rows[0]["Processid"].ToString()) != null)
            {
                ddProcessName.SelectedValue = ds.Tables[0].Rows[0]["Processid"].ToString();
                if (sender != null)
                {
                    ddProcessName_SelectedIndexChanged(sender, new EventArgs());
                }

            }
            if (ddempname.Items.FindByValue(ds.Tables[0].Rows[0]["Empid"].ToString()) != null)
            {
                ddempname.SelectedValue = ds.Tables[0].Rows[0]["Empid"].ToString();
                if (sender != null)
                {
                    ddempname_SelectedIndexChanged(sender, new EventArgs());
                }
            }
            ddOrderNo.Focus();
        }
        else
        {
            ddProcessName.SelectedIndex = -1;
            ddOrderNo.SelectedIndex = -1;
            ScriptManager.RegisterStartupScript(Page, GetType(), "fillemp", "alert('Please Enter correct Emp. Code or No entry from this employee')", true);

        }
    }
    protected void DDBinNo_SelectedIndexChanged(object sender, EventArgs e)
    {
       // Fill_LotNoSelectedChange();
    }
    protected void DDTagno_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (TDBinNo.Visible == true)
        //{
        //    DDBinNo.SelectedIndex = -1;
           // FillBinNo(sender);
        //}
        //else
        //{
           // Fill_LotNoSelectedChange();
       // }
    }
    protected void BtnUpdateRemark_Click(object sender, EventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        con.Open();
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            SqlParameter[] arr = new SqlParameter[7];

            arr[0] = new SqlParameter("@PRMID", SqlDbType.Int);
            arr[1] = new SqlParameter("@TranType", SqlDbType.Int);
            arr[2] = new SqlParameter("@Msg", SqlDbType.VarChar, 100);
            arr[3] = new SqlParameter("@userid", Session["varuserid"]);
            arr[4] = new SqlParameter("@Mastercompanyid", Session["varcompanyid"]);
            arr[5] = new SqlParameter("@Remark", "");
            arr[6] = new SqlParameter("@ProcessId", ddProcessName.SelectedValue);

            arr[0].Value = DDChallanNo.SelectedValue;
            arr[1].Value = 0;
            arr[2].Direction = ParameterDirection.Output;

            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_PROCESS_RAW_ISSUE_UPDATE_REMARK", arr);

            LblError.Text = arr[3].Value.ToString();
            //if (arr[3].Value.ToString() != "")
            //{
            //    ScriptManager.RegisterStartupScript(Page, GetType(), "altdel", "alert('" + arr[3].Value.ToString() + "');", true);
            //}
            //else
            //{
            //    LblError.Text = arr[3].Value.ToString();
            //}
            Tran.Commit();
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            LblError.Visible = true;
            LblError.Text = ex.Message;
        }
        finally
        {
            con.Close();
            con.Dispose();
        }
       // Fill_Grid();
    }
    protected void BtnOrderProcessToChampoPanipatPNM2_Click(object sender, EventArgs e)
    {
        if (Convert.ToInt32(ViewState["Prmid"]) > 0)
        {
            SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlTransaction Tran = con.BeginTransaction();
            try
            {
                SqlParameter[] param = new SqlParameter[6];
                param[0] = new SqlParameter("@PRMID", ViewState["Prmid"]);
                param[1] = new SqlParameter("@PROCESSID", ddProcessName.SelectedValue);
                param[2] = new SqlParameter("@USERID", 1);
                param[3] = new SqlParameter("@MASTERCOMPANYID", 28);
                param[4] = new SqlParameter("@TYPEFLAG", 1);
                param[5] = new SqlParameter("@MSG", SqlDbType.VarChar, 100);
                param[5].Direction = ParameterDirection.Output;

                SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_GETINTERNALBAZAARDETAIL_CHAMPOPANIPAT", param);
                if (param[5].Value == "Data successfully process")
                {
                    Tran.Commit();
                }
                else
                {
                    Tran.Rollback();
                }
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn1", "alert('" + param[5].Value + "')", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(Page, GetType(), "opn1", "alert('" + ex.Message + "')", true);
                Tran.Rollback();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }
    }
    protected void ChkForCompleteStatus_CheckedChanged(object sender, EventArgs e)
    {
        if (ddempname.Items.Count > 0)
        {
            ddempname.SelectedIndex = -1;
            ddOrderNo.Items.Clear();
        }
    }
    protected void txtkitno_TextChanged(object sender, EventArgs e)
    {
        FillEditGrid();

    }
    protected void FillEditGrid()
    {
        string str = @"select dbo.F_getItemDescription(PT.Finishedid,0) as ItemDescription,PT.Finishedid,PM.Finishedid as item_finished_id ,pt.godownid,pt.itemid,pt.unitid,
                    PT.Lotno,PT.TagNo,PT.IssueQuantity,PM.qty,PM.chalanNo,Replace(CONVERT(nvarchar(11),PM.date,106),' ','-') as IssueDate,PM.prmid,PT.Prtid,PM.processid,VF.ITEM_NAME,VF.DesignName,VF.SizeMtr
                    from ProcessRawMaster_KM PM inner join ProcessRawTran_KM PT on PM.PRMid=PT.PRMid
 JOIN V_FinishedItemDetail VF(Nolock) ON VF.ITEM_FINISHED_ID = PM.Finishedid 
                     and PM.kitno='" + txtkitno.Text+"'";
        DataSet ds = SqlHelper.ExecuteDataset(ErpGlobal.DBCONNECTIONSTRING, CommandType.Text, str);

        if (ds != null)
        {

            if (ds.Tables[0].Rows.Count > 0)
            {
                hdnfinishedid.Value=ds.Tables[0].Rows[0]["item_finished_id"].ToString();

                lblkitarticle.Text = Convert.ToString(ds.Tables[0].Rows[0]["DesignName"]);
                lblkitsize.Text = Convert.ToString(ds.Tables[0].Rows[0]["SizeMtr"]);
                lblkitpcs.Text = Convert.ToString(ds.Tables[0].Rows[0]["qty"]);

                gvkit.DataSource = ds.Tables[0];
                gvkit.DataBind();
            }
            LblError.Visible = false;
        }
        else {
            LblError.Visible = true;
            LblError.Text = "No Record Found!!";
        
        }
        //if (chkEdit.Checked == true)
        //{
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        txtissueno.Text = ds.Tables[0].Rows[0]["chalanno"].ToString();
        //        txtissuedate.Text = ds.Tables[0].Rows[0]["issuedate"].ToString();
        //        TxtEWayBillNo.Text = ds.Tables[0].Rows[0]["EWayBillNo"].ToString();
        //    }
        //    else
        //    {
        //        txtissueno.Text = "";
        //        txtissuedate.Text = System.DateTime.Now.ToString("dd-MMM-yyyy");
        //    }

        //}

    }

    protected void gvdetail_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvdetail.EditIndex = e.NewEditIndex;
        //if (chkEdit.Checked)
        //{
            Fill_Grid();

        //}
        //else
        //{
        //    FillissueGrid();

        //}
    }
    protected void gvdetail_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvdetail.EditIndex = -1;
        Fill_Grid();
        //if (chkEdit.Checked)
        //{
        //    FillEditGrid();

        //}
        //else
        //{

        //    FillissueGrid();
        //}
    }
    protected void gvdetail_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }
        SqlTransaction Tran = con.BeginTransaction();
        try
        {
            int VarPrtID = Convert.ToInt32(gvdetail.DataKeys[e.RowIndex].Value);
            ViewState["Prmid"] = SqlHelper.ExecuteScalar(Tran, CommandType.Text, "Select PrmId from ProcessRawTran Where PrtId=" + VarPrtID);

            //Label lblhqty = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblhqty");
            //Label lblprmid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprmid");
            //Label lblprtid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprtid");
            //Label lblprorderid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprorderid");
            TextBox txtqty = (TextBox)gvdetail.Rows[e.RowIndex].FindControl("txtqty");
          //  Label lblprocessid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprocessid");
            //**************
            SqlParameter[] param = new SqlParameter[8];
            param[0] = new SqlParameter("@prmid",Convert.ToInt32(ViewState["Prmid"]));
            param[1] = new SqlParameter("@prtid", VarPrtID);
            param[2] = new SqlParameter("@hqty", 0);
            param[3] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
            param[3].Direction = ParameterDirection.Output;
            param[4] = new SqlParameter("@qty", txtqty.Text == "" ? "0" : txtqty.Text);
            param[5] = new SqlParameter("@processid", ddProcessName.SelectedValue);
            param[6] = new SqlParameter("@userid", Session["varuserid"]);
            param[7] = new SqlParameter("@EWayBillNo", "");
            //*************
            SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "PRO_UPDATEKITISSUE", param);
            LblError.Text = param[3].Value.ToString();
            Tran.Commit();
          
           
            if (ChKForEdit.Checked)
            {
                Fill_Grid();

            }
            gvdetail.EditIndex = -1;
            //else
            //{
            //    FillissueGrid();

            //}
            // FillissueGrid();
        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;
            Tran.Rollback();
        }
        finally
        {
            con.Dispose();
            con.Close();
        }
    }
    //protected void gvdetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
    //{
    //    SqlConnection con = new SqlConnection(ErpGlobal.DBCONNECTIONSTRING);
    //    if (con.State == ConnectionState.Closed)
    //    {
    //        con.Open();
    //    }
    //    SqlTransaction Tran = con.BeginTransaction();
    //    try
    //    {
    //        Label lblprmid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprmid");
    //        Label lblprtid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprtid");
    //        Label lblprorderid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprorderid");
    //        Label lblprocessid = (Label)gvdetail.Rows[e.RowIndex].FindControl("lblprocessid");

    //        SqlParameter[] param = new SqlParameter[4];
    //        param[0] = new SqlParameter("@prmid", lblprmid.Text);
    //        param[1] = new SqlParameter("@prtid", lblprtid.Text);
    //        param[2] = new SqlParameter("@Processid", lblprocessid.Text);
    //        param[3] = new SqlParameter("@msg", SqlDbType.VarChar, 100);
    //        param[3].Direction = ParameterDirection.Output;
    //        //****************
    //        SqlHelper.ExecuteNonQuery(Tran, CommandType.StoredProcedure, "Pro_deleteweftissue", param);
    //        LblError.Text = param[3].Value.ToString();
    //        Tran.Commit();
    //        Fill_Grid();
    //        //if (chkEdit.Checked)
    //        //{
    //        //    FillEditGrid();

    //        //}
    //        //else
    //        //{
    //        //    FillissueGrid();

    //        //}
    //        //***************
    //    }
    //    catch (Exception ex)
    //    {
    //        LblError.Text = ex.Message;
    //        Tran.Rollback();
    //    }
    //    finally
    //    {
    //        con.Dispose();
    //        con.Close();
    //    }
    //}


}