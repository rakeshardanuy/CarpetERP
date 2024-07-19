  
  
CREATE PROC [DBO].[PRO_FINISHINGISSUERECEIVESUMMARYREPORTAGNI]        
@PROCESSID INT,                              
@DATEFLAG INT,                              
@FROMDATE DATETIME,                              
@TODATE DATETIME,                              
@EMPID INT,                              
@ISSUEORDERID VARCHAR(50),                              
@WHERE  VARCHAR(1000)                                                    
AS    
    
BEGIN                            
DECLARE @STR VARCHAR(MAX), @STR_ORDERWISE VARCHAR(MAX),@VARDECPLACE INT,@FINISHINGNEWMODULEWISE INT,      
@MASTERCOMPANYID INT ,@MAXSRNO_ORDER INT,@SRNO_ORDER INT,@MainPRO_REC_ID INT ,@COMPANYNAME NVARCHAR(50),      
@AREA FLOAT ,@RATE FLOAT,@ReceiveQty INT,@IssueQty INT,@FDATE NVARCHAR(50),@TDATE NVARCHAR(50),      
@PROCESS_NAME NVARCHAR(50),@LENGTH VARCHAR(10) ,@WIDTH VARCHAR(10),@CATEGORY_NAME NVARCHAR(50),      
@ITEM_NAME NVARCHAR(50),@QUALITYNAME NVARCHAR(50),@DESIGNNAME NVARCHAR(50),@COLORNAME NVARCHAR(50),      
@SHADECOLORNAME NVARCHAR(50),@SHAPENAME NVARCHAR(50),@UNITID INT,@PENALITY INT,@DFLAG VARCHAR(10),      
@DateAdded Datetime,    
@RECEIVEDATE DATETIME,@Item_Finished_Id INT,@IssOrderID INT,@PROCESS_REC_ID nvarchar(50),      
@EMPNAME NVARCHAR(50),@CUSTOMERORDERNO NVARCHAR(50),@CUSTOMERCODE NVARCHAR(50),@DECPLACE VARCHAR(10),      
@IssueChallanNo NVARCHAR(50),@QTY INT                          
            
DECLARE @DETAIL TABLE (COMPANYNAME NVARCHAR(50),AREA FLOAT ,RATE FLOAT,ReceiveQty INT,IssueQty INT,      
FROMDATE NVARCHAR(50),TODATE NVARCHAR(50),PROCESS_NAME NVARCHAR(50),LENGTH VARCHAR(10) ,WIDTH VARCHAR(10),      
CATEGORY_NAME NVARCHAR(50),ITEM_NAME NVARCHAR(50),QUALITYNAME NVARCHAR(50),DESIGNNAME NVARCHAR(50),      
COLORNAME NVARCHAR(50),SHADECOLORNAME NVARCHAR(50),SHAPENAME NVARCHAR(50),UNITID INT,PENALITY INT,      
DATEFLAG VARCHAR,DateAdded NVARCHAR(50),RECEIVEDATE DATETIME,      
Item_Finished_Id INT,IssueOrderID INT,PROCESS_REC_ID nvarchar(50),EMPNAME NVARCHAR(50),      
DECPLACE VARCHAR(10),IssueChallanNo NVARCHAR(50), srno int,customerorderno nvarchar(20),customercode nvarchar(20))                          
            
DECLARE @FINAL_DETAIL TABLE (COMPANYNAME NVARCHAR(50),AREA FLOAT ,RATE FLOAT,ReceiveQty INT,IssueQty INT,      
FROMDATE NVARCHAR(50),TODATE NVARCHAR(50),PROCESS_NAME NVARCHAR(50),LENGTH VARCHAR(10) ,WIDTH VARCHAR(10),      
CATEGORY_NAME NVARCHAR(50),ITEM_NAME NVARCHAR(50),QUALITYNAME NVARCHAR(50),DESIGNNAME NVARCHAR(50),      
COLORNAME NVARCHAR(50),SHADECOLORNAME NVARCHAR(50),SHAPENAME NVARCHAR(50),UNITID INT,PENALITY INT,      
DATEFLAG VARCHAR,DateAdded NVARCHAR(50),RECEIVEDATE DATETIME,Item_Finished_Id INT,IssueOrderID INT,PROCESS_REC_ID nvarchar(50),      
EMPNAME NVARCHAR(50),DECPLACE VARCHAR(10),IssueChallanNo NVARCHAR(50), srno int,PENDINGQTY INT,      
customerorderno nvarchar(20),customercode nvarchar(20))                          
      
DECLARE @DETAIL_ORDERWISE TABLE (COMPANYNAME NVARCHAR(50),RATE FLOAT,ReceiveQty INT,IssueQty INT,      
FROMDATE NVARCHAR(50),TODATE NVARCHAR(50),PROCESS_NAME NVARCHAR(50),      
LENGTH VARCHAR(10) ,WIDTH VARCHAR(10),CATEGORY_NAME NVARCHAR(50),ITEM_NAME NVARCHAR(50),      
QUALITYNAME NVARCHAR(50),DESIGNNAME NVARCHAR(50),COLORNAME NVARCHAR(50),SHADECOLORNAME NVARCHAR(50),      
SHAPENAME NVARCHAR(50),UNITID INT,PENALITY INT,    
DATEFLAG VARCHAR,DateAdded NVARCHAR(50),RECEIVEDATE DATETIME,EMPNAME NVARCHAR(50), srno int,      
customerorderno nvarchar(20))       
      
DECLARE @FINAL_DETAIL_ORDERWISE TABLE (COMPANYNAME NVARCHAR(50),RATE FLOAT,ReceiveQty INT,IssueQty INT,      
FROMDATE NVARCHAR(50),TODATE NVARCHAR(50),PROCESS_NAME NVARCHAR(50),      
LENGTH VARCHAR(10) ,WIDTH VARCHAR(10),CATEGORY_NAME NVARCHAR(50),ITEM_NAME NVARCHAR(50),      
QUALITYNAME NVARCHAR(50),DESIGNNAME NVARCHAR(50),COLORNAME NVARCHAR(50),      
SHADECOLORNAME NVARCHAR(50),SHAPENAME NVARCHAR(50),UNITID INT,PENALITY INT,    
DATEFLAG VARCHAR,DateAdded NVARCHAR(50),RECEIVEDATE DATETIME,     
EMPNAME NVARCHAR(50), srno int,PENDINGQTY INT,customerorderno nvarchar(20))      
      
SELECT @VARDECPLACE=CASE WHEN VARFINISHINGUNIT=1 THEN ROUNDMTRFLAG ELSE ROUNDFTFLAG END,                            
@FINISHINGNEWMODULEWISE=FINISHINGNEWMODULEWISE,@MASTERCOMPANYID=VarCompanyNo FROM MASTERSETTING(Nolock)                            
                                               
                           
      IF @FINISHINGNEWMODULEWISE=1                            
      BEGIN                            
BEGIN                              
                                     
 SET @STR='SELECT  CI.COMPANYNAME,sum(ISD.AREA*ISD.QTY) as  AREA,ISD.RATE,                            
isnull((Select sum(ISNULL(PRD.Qty,0)) from PROCESS_RECEIVE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+' PRD(Nolock)                            
Where ISD.IssueOrderID=PRD.IssueOrderID  AND PD.Process_Rec_Id=PRD.Process_Rec_Id  And ISD.OrderID = PRD.OrderID And ISD.Item_Finished_Id=PRD.Item_Finished_Id),0) as recQty,                      
            isnull((Select sum(ISNULL(PRD.Qty,0)) from PROCESS_ISSUE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+' PRD(Nolock)                      
Where ISD.IssueOrderID=PRD.IssueOrderID   And ISD.OrderID = PRD.OrderID And ISD.Item_Finished_Id=PRD.Item_Finished_Id),0) as ISSUEQTY,                            
                       
            '''+REPLACE(CONVERT(NVARCHAR(11),@FROMDATE,106),' ','-') + ''' AS FROMDATE,'''+REPLACE(CONVERT(NVARCHAR(11),@TODATE,106),' ','-') + ''' AS TODATE,                            
            (SELECT PROCESS_NAME FROM PROCESS_NAME_MASTER(Nolock) WHERE PROCESS_NAME_ID='+CAST(@PROCESSID AS VARCHAR) +') PROCESS_NAME,                            
            ISD.LENGTH,ISD.WIDTH,VF.CATEGORY_NAME,VF.ITEM_NAME,VF.QUALITYNAME,VF.DESIGNNAME,VF.COLORNAME,VF.SHADECOLORNAME,                            
            VF.SHAPENAME,IM.UNITID,0 AS PENALITY,'+CAST(@DATEFLAG AS VARCHAR)+' AS DATEFLAG, CONVERT(NVARCHAR(11),ISD.Dateadded,106) as Dateadded,PM.RECEIVEDATE,ISD.Item_Finished_Id,ISD.IssueOrderID,pm.challanno as PROCESS_REC_ID,                         
  
   
            DBO.F_GETFOLIOEMPLOYEENEW(ISD.ISSUEORDERID,'+CAST(@PROCESSID AS VARCHAR)+') AS EMPNAME,'+CAST(@VARDECPLACE AS VARCHAR)+' AS DECPLACE,                            
            isnull(IM.ChallanNo,'''') as IssueChallanNo,row_number() over(order by pd.PROCESS_REC_ID desc) as srno ,om.CustomerOrderNo,cm.customercode                            
            FROM PROCESS_ISSUE_MASTER_'+CAST(@PROCESSID AS VARCHAR)+' IM(Nolock)                            
INNER JOIN PROCESS_ISSUE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+'(Nolock)  ISD ON ISD.ISSUEORDERID =IM.ISSUEORDERID                            
                  LEFT JOIN PROCESS_RECEIVE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+'(Nolock) PD ON ISD.ISSUE_DETAIL_ID=PD.ISSUE_DETAIL_ID                          
LEFT JOIN PROCESS_RECEIVE_MASTER_'+CAST(@PROCESSID AS VARCHAR)+'(Nolock) PM ON PM.PROCESS_REC_ID=PD.PROCESS_REC_ID                          
            INNER JOIN COMPANYINFO CI(Nolock) ON IM.COMPANYID=CI.COMPANYID                            
            INNER JOIN V_FINISHEDITEMDETAIL VF(Nolock) ON ISD.ITEM_FINISHED_ID=VF.ITEM_FINISHED_ID                            
            Left join ordermaster om(Nolock) on ISD.orderid=Om.orderid                  
Left join customerinfo cm(Nolock) on om.customerid=cm.customerid                  
            WHERE 1=1'                            
            IF @WHERE<>''                            
            BEGIN                            
                  SET @STR=@STR + @WHERE                            
            END                            
            IF @ISSUEORDERID<>''                            
            BEGIN                            
                  SET @STR=@STR + ' AND ISD.ISSUEORDERID='+CAST(@ISSUEORDERID AS VARCHAR) +''                            
            END                            
            IF @EMPID>0                            
            BEGIN                            
                 SET @STR=@STR + ' AND PD.ISSUE_DETAIL_ID IN(SELECT EMP.ISSUEDETAILID FROM EMPLOYEE_PROCESSORDERNO EMP(Nolock)                            
                              INNER JOIN PROCESS_ISSUE_MASTER_'+CAST(@PROCESSID AS VARCHAR) +' PRM(Nolock) ON EMP.ISSUEORDERID=PRM.ISSUEORDERID                            
                              AND EMP.PROCESSID='+CAST(@PROCESSID AS VARCHAR) +' AND EMP.EMPID='+CAST(@EMPID AS VARCHAR) +''                            
                  IF @DATEFLAG=1                            
                  BEGIN                            
                        SET @STR=@STR +  ' AND  IM.ASSIgndate>='''+REPLACE(CONVERT(NVARCHAR(11),@FROMDATE,106),' ','-')+''' AND  IM.ASSIgndate<='''+REPLACE(CONVERT(NVARCHAR(11),@TODATE,106),' ','-')+''''                            
                  END                            
                  SET @STR=@STR + ')'                    
            END                            
            SET @STR=@STR + ' Group by CI.COMPANYNAME,ISD.RATE,ISD.LENGTH,ISD.WIDTH,VF.CATEGORY_NAME,VF.ITEM_NAME,VF.QUALITYNAME,VF.DESIGNNAME,VF.COLORNAME,VF.SHADECOLORNAME,                            
                                          VF.SHAPENAME,IM.UNITID,IM.ChallanNo,PM.ChallanNo,ISD.Item_Finished_Id,ISD.IssueOrderID,pd.PROCESS_REC_ID, ISD.OrderID,om.CustomerOrderNo,cm.customercode,CONVERT(NVARCHAR(11),ISD.Dateadded,106),PM.RECEIVEDATE'     
 
                    
     
         
            SET @STR=@STR + ' Order by   CONVERT(NVARCHAR(11),ISD.Dateadded,106),PM.RECEIVEDATE'                            
print(@STR)                      
                     
    
INSERT  @DETAIL EXEC(@STR);      
    
SELECT @MAXSRNO_ORDER=MAX(srno),@SRNO_ORDER=MIN(srno) FROM @DETAIL    
    
    
WHILE(@SRNO_ORDER<=@MAXSRNO_ORDER)                                                              
BEGIN                            
declare @PENDINGQTY INT=0                          
Select @COMPANYNAME=COMPANYNAME,@AREA =AREA ,@RATE=RATE,@ReceiveQty=ReceiveQty,@IssueQty=IssueQty,@FDATE=FROMDATE,@TDATE=TODATE,@PROCESS_NAME=PROCESS_NAME,                          
      @LENGTH=LENGTH,@WIDTH=WIDTH,@CATEGORY_NAME=CATEGORY_NAME,@ITEM_NAME=ITEM_NAME,@QUALITYNAME=QUALITYNAME,@DESIGNNAME=DESIGNNAME,                          
      @COLORNAME=COLORNAME,@SHADECOLORNAME=SHADECOLORNAME,@SHAPENAME=SHAPENAME,@UNITID=UNITID,@PENALITY=PENALITY,    
@DFLAG=DATEFLAG,@DateAdded=DateAdded,@RECEIVEDATE=RECEIVEDATE,                          
      @Item_Finished_Id=Item_Finished_Id,@IssOrderID=IssueOrderID,@PROCESS_REC_ID=PROCESS_REC_ID,@EMPNAME=EMPNAME,@DECPLACE= DECPLACE ,                          
      @IssueChallanNo=IssueChallanNo,@CUSTOMERORDERNO=customerorderno,@CUSTOMERCODE=customercode,@DateAdded=DateAdded From @DETAIL Where SrNo = @SRNO_ORDER ORDER BY IssueOrderID DESC                            
                                
      select @PENDINGQTY=isnull(PENDINGQTY,0) from @FINAL_DETAIL where IssueOrderID=@IssOrderID and  Item_Finished_Id=@Item_Finished_Id                        
                         
                                
            IF(@PENDINGQTY>0)                          
            SET @PENDINGQTY=@PENDINGQTY-@ReceiveQty;                          
            ELSE                          
            SET @PENDINGQTY=@IssueQty-@ReceiveQty;                          
                                      
                                
                                
                         
      INSERT INTO @FINAL_DETAIL VALUES(@COMPANYNAME,@AREA,@RATE,@ReceiveQty,@IssueQty,@FDATE,@TDATE,@PROCESS_NAME,                          
      @LENGTH,@WIDTH,@CATEGORY_NAME,@ITEM_NAME,@QUALITYNAME,@DESIGNNAME,                          
      @COLORNAME,@SHADECOLORNAME,@SHAPENAME,@UNITID,@PENALITY,cast(@DFLAG as varchar),@DateAdded,@RECEIVEDATE,                          
      @Item_Finished_Id,@IssOrderID,@PROCESS_REC_ID,@EMPNAME,@DECPLACE,                          
      @IssueChallanNo,@SRNO_ORDER,@PENDINGQTY,@CUSTOMERORDERNO,@CUSTOMERCODE)                          
                           
SET @SRNO_ORDER+=1;                          
                         
END                          
    --        EXEC(@STR)                            
SELECT * FROM @FINAL_DETAIL                          
                
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
END                        
      END                            
      ELSE                            
      BEGIN                            
                     
            SET @STR='SELECT  CI.COMPANYNAME,sum(PD.AREA*PD.QTY) as  AREA,PD.RATE,                            
            sum(PD.QTY) as IssueQty,                            
            isnull((Select sum(PRD.Qty) from Process_Receive_Detail_'+CAST(@PROCESSID AS VARCHAR) +' PRD(Nolock)                            
            Where PD.IssueOrderID=PRD.IssueOrderID And PD.OrderID = PRD.OrderID And PD.Item_Finished_Id=PRD.Item_Finished_Id),0) as ReceiveQty,                            
            '''+REPLACE(CONVERT(NVARCHAR(11),@FROMDATE,106),' ','-') + ''' AS FROMDATE,'''+REPLACE(CONVERT(NVARCHAR(11),@TODATE,106),' ','-') + ''' AS TODATE,                            
            (SELECT PROCESS_NAME FROM PROCESS_NAME_MASTER(Nolock) WHERE PROCESS_NAME_ID='+CAST(@PROCESSID AS VARCHAR) +') PROCESS_NAME,                            
            PD.LENGTH,PD.WIDTH,VF.CATEGORY_NAME,VF.ITEM_NAME,VF.QUALITYNAME,VF.DESIGNNAME,VF.COLORNAME,VF.SHADECOLORNAME,                            
            VF.SHAPENAME,PM.UNITID,0 AS PENALITY,'+CAST(@DATEFLAG AS VARCHAR)+' AS DATEFLAG,CONVERT(NVARCHAR(11),PD.Dateadded,106),PM.ASSIGNDATE,PD.Item_Finished_Id,PD.IssueOrderID,                            
            EI.EMPNAME,'+CAST(@VARDECPLACE AS VARCHAR)+' AS DECPLACE,isnull(PM.ChallanNo,PM.Issueorderid) as IssueChallanNo                            
            FROM PROCESS_ISSUE_MASTER_'+CAST(@PROCESSID AS VARCHAR) +' PM(Nolock)                            
            INNER JOIN PROCESS_ISSUE_DETAIL_'+CAST(@PROCESSID AS VARCHAR) +' PD(Nolock) ON PM.ISSUEORDERID=PD.ISSUEORDERID                            
            INNER JOIN COMPANYINFO CI(Nolock) ON PM.COMPANYID=CI.COMPANYID                            
            INNER JOIN V_FINISHEDITEMDETAIL VF(Nolock) ON PD.ITEM_FINISHED_ID=VF.ITEM_FINISHED_ID                            
            Left join ordermaster om(Nolock) on Pd.orderid=Om.orderid                            
            INNER JOIN EMPINFO EI(Nolock) ON PM.EMPID=EI.EMPID                            
            WHERE 1=1'                            
            IF @WHERE<>''                            
            BEGIN                            
                  SET @STR=@STR + @WHERE                            
            END                            
            IF @ISSUEORDERID<>''                            
            BEGIN                            
                  SET @STR=@STR + ' AND PM.ISSUEORDERID='+CAST(@ISSUEORDERID AS VARCHAR) +''                            
            END                            
            IF @EMPID>0                            
            BEGIN                            
                  SET @STR=@STR + ' AND PM.EMPID='+CAST(@EMPID AS VARCHAR) +''                                  
            END                            
            IF @DATEFLAG=1                            
            BEGIN                            
                  SET @STR=@STR +  ' AND PM.ASSIGNDATE>='''+REPLACE(CONVERT(NVARCHAR(11),@FROMDATE,106),' ','-')+''' AND PM.ASSIGNDATE<='''+REPLACE(CONVERT(NVARCHAR(11),@TODATE,106),' ','-')+''''                            
            END                            
            SET @STR=@STR + ' Group by CI.COMPANYNAME,PD.RATE,PD.LENGTH,PD.WIDTH,VF.CATEGORY_NAME,VF.ITEM_NAME,VF.QUALITYNAME,VF.DESIGNNAME,VF.COLORNAME,VF.SHADECOLORNAME,                            
                                          VF.SHAPENAME,PM.UNITID,PM.ASSIGNDATE,PM.ISSUEORDERID,PM.ChallanNo,PD.Item_Finished_Id,PD.IssueOrderID, PD.OrderID,CONVERT(NVARCHAR(11),PD.Dateadded,106)'                            
            SET @STR=@STR + ' Order by  PM.ASSIGNDATE'                            
            EXEC(@STR)                            
            --PRINT(@STR)                            
      END                            
 IF (@MASTERCOMPANYID=47)                              
            BEGIN                                                                      
           SET @STR_ORDERWISE='SELECT  CI.COMPANYNAME,ISD.RATE,                              
isnull((Select sum(ISNULL(PRD.Qty,0)) from PROCESS_RECEIVE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+' PRD(Nolock)                              
Where  ISD.OrderID = PRD.OrderID and isd.ITEM_FINISHED_ID=prd.ITEM_FINISHED_ID ),0) as recQty,                        
            isnull((Select sum(ISNULL(PRD.Qty,0)) from PROCESS_ISSUE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+' PRD(Nolock)                              
Where ISD.OrderID = PRD.OrderID and isd.ITEM_FINISHED_ID=prd.ITEM_FINISHED_ID ),0) as ISSUEQTY,                              
                                               
            '''+REPLACE(CONVERT(NVARCHAR(11),@FROMDATE,106),' ','-') + ''' AS FROMDATE,'''+REPLACE(CONVERT(NVARCHAR(11),@TODATE,106),' ','-') + ''' AS TODATE,                              
            (SELECT PROCESS_NAME FROM PROCESS_NAME_MASTER(Nolock) WHERE PROCESS_NAME_ID='+CAST(@PROCESSID AS VARCHAR) +') PROCESS_NAME,                              
            ISD.LENGTH,ISD.WIDTH,VF.CATEGORY_NAME,VF.ITEM_NAME,VF.QUALITYNAME,VF.DESIGNNAME,VF.COLORNAME,VF.SHADECOLORNAME,                              
            VF.SHAPENAME,max(IM.UNITID) UNITID,0 AS PENALITY,'+CAST(@DATEFLAG AS VARCHAR)+' AS DATEFLAG,     
CONVERT(NVARCHAR(11),ISD.Dateadded,106) DateAdded,PM.RECEIVEDATE,                            
            '''' AS EMPNAME,                              
            row_number() over(order by ISD.OrderID desc) as srno ,om.CustomerOrderNo                        
            FROM PROCESS_ISSUE_MASTER_'+CAST(@PROCESSID AS VARCHAR)+' IM(Nolock)                              
INNER JOIN PROCESS_ISSUE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+'(Nolock)  ISD ON ISD.ISSUEORDERID =IM.ISSUEORDERID                              
                  LEFT JOIN PROCESS_RECEIVE_DETAIL_'+CAST(@PROCESSID AS VARCHAR)+'(Nolock) PD ON ISD.ISSUE_DETAIL_ID=PD.ISSUE_DETAIL_ID                            
LEFT JOIN PROCESS_RECEIVE_MASTER_'+CAST(@PROCESSID AS VARCHAR)+'(Nolock) PM ON PM.PROCESS_REC_ID=PD.PROCESS_REC_ID                    
            INNER JOIN COMPANYINFO CI(Nolock) ON IM.COMPANYID=CI.COMPANYID                              
            INNER JOIN V_FINISHEDITEMDETAIL VF(Nolock) ON ISD.ITEM_FINISHED_ID=VF.ITEM_FINISHED_ID                              
            Left join ordermaster om(Nolock) on ISD.orderid=Om.orderid                              
            WHERE 1=1'                              
            IF @WHERE<>''                              
            BEGIN                              
                  SET @STR_ORDERWISE=@STR_ORDERWISE + @WHERE                              
            END                              
            IF @ISSUEORDERID<>''                              
            BEGIN                              
                  SET @STR_ORDERWISE=@STR_ORDERWISE + ' AND ISD.ISSUEORDERID='+CAST(@ISSUEORDERID AS VARCHAR) +''                              
            END                              
            IF @EMPID>0                              
            BEGIN                              
                 SET @STR_ORDERWISE=@STR_ORDERWISE + ' AND PD.ISSUE_DETAIL_ID IN(SELECT EMP.ISSUEDETAILID FROM EMPLOYEE_PROCESSORDERNO EMP(Nolock)                              
                              INNER JOIN PROCESS_ISSUE_MASTER_'+CAST(@PROCESSID AS VARCHAR) +' PRM(Nolock) ON EMP.ISSUEORDERID=PRM.ISSUEORDERID                              
                              AND EMP.PROCESSID='+CAST(@PROCESSID AS VARCHAR) +' AND EMP.EMPID='+CAST(@EMPID AS VARCHAR) +''                              
                  IF @DATEFLAG=1                              
                  BEGIN                              
                        SET @STR_ORDERWISE=@STR_ORDERWISE +  ' AND  IM.ASSIgndate>='''+REPLACE(CONVERT(NVARCHAR(11),@FROMDATE,106),' ','-')+''' AND  IM.ASSIgndate<='''+REPLACE(CONVERT(NVARCHAR(11),@TODATE,106),' ','-')+''''                              
                  END                              
                  SET @STR_ORDERWISE=@STR_ORDERWISE + ')'                              
            END                              
            SET @STR_ORDERWISE=@STR_ORDERWISE + ' Group by CI.COMPANYNAME,ISD.RATE,ISD.LENGTH,ISD.WIDTH,VF.CATEGORY_NAME,VF.ITEM_NAME,VF.QUALITYNAME,VF.DESIGNNAME,VF.COLORNAME,VF.SHADECOLORNAME,                              
                                          VF.SHAPENAME,ISD.OrderID,isd.ITEM_FINISHED_ID,om.CustomerOrderNo,CONVERT(NVARCHAR(11),ISD.Dateadded,106),PM.RECEIVEDATE'                              
            SET @STR_ORDERWISE=@STR_ORDERWISE + ' Order by   CONVERT(NVARCHAR(11),ISD.Dateadded,106),PM.RECEIVEDATE'                                
                               
    
    
print(@STR_ORDERWISE)                        
  INSERT  @DETAIL_ORDERWISE EXEC(@STR_ORDERWISE);                          
                       
SELECT @MAXSRNO_ORDER=MAX(srno),@SRNO_ORDER=MIN(srno) FROM @DETAIL_ORDERWISE                                                      
WHILE(@SRNO_ORDER<=@MAXSRNO_ORDER)                                                          
BEGIN                              
declare @PENDQTY INT=0                            
Select @COMPANYNAME=COMPANYNAME,@RATE=RATE,@ReceiveQty=ReceiveQty,@IssueQty=IssueQty,@FDATE=FROMDATE,@TDATE=TODATE,@PROCESS_NAME=PROCESS_NAME,                            
      @LENGTH=LENGTH,@WIDTH=WIDTH,@CATEGORY_NAME=CATEGORY_NAME,@ITEM_NAME=ITEM_NAME,@QUALITYNAME=QUALITYNAME,@DESIGNNAME=DESIGNNAME,                            
      @COLORNAME=COLORNAME,@SHADECOLORNAME=SHADECOLORNAME,@SHAPENAME=SHAPENAME,@UNITID=UNITID,@PENALITY=PENALITY,    
@DFLAG=DATEFLAG,@DateAdded=DateAdded,@RECEIVEDATE=RECEIVEDATE,      
      @EMPNAME=EMPNAME,@CUSTOMERORDERNO=customerorderno From @DETAIL_ORDERWISE Where SrNo = @SRNO_ORDER                              
                                  
      select @PENDQTY=isnull(@PENDQTY,0) from @FINAL_DETAIL_ORDERWISE                          
                           
                                  
            IF(@PENDQTY>0)                            
            SET @PENDQTY=@PENDQTY-@ReceiveQty;                            
            ELSE                            
            SET @PENDQTY=@IssueQty-@ReceiveQty;                            
                                        
                                  
                                  
                           
      INSERT INTO @FINAL_DETAIL_ORDERWISE VALUES(@COMPANYNAME,@RATE,@ReceiveQty,@IssueQty,@FDATE,@TDATE,@PROCESS_NAME,                            
      @LENGTH,@WIDTH,@CATEGORY_NAME,@ITEM_NAME,@QUALITYNAME,@DESIGNNAME,    
@COLORNAME,@SHADECOLORNAME,@SHAPENAME,@UNITID,@PENALITY,cast(@DFLAG as varchar),@DateAdded,@RECEIVEDATE,         
@EMPNAME,@SRNO_ORDER,@PENDQTY,@CUSTOMERORDERNO)                            
         
      
    
                         
      
    
    
    
    
    
    
    
    
    
SET @SRNO_ORDER+=1;                            
                           
END                          
--print(@STR_ORDERWISE)                        
            --EXEC(@STR_ORDERWISE)                              
SELECT * FROM @FINAL_DETAIL_ORDERWISE                            
END           
    
    
END 