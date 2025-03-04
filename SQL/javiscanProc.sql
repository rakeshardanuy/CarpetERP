Alter PROC [DBO].PRO_PRODUCTIONLOOMRECEIVESTOCKNOWISE_DEVICE            
@PROCESS_REC_ID INT OUTPUT,                      
@COMPANYID INT,                  
@PRODUCTIONUNIT INT,            
@LOOMID INT,            
@ISSUEORDERID INT,            
@RECEIVENO VARCHAR(50) OUTPUT,            
@RECEIVEDATE DATETIME,            
@MSG VARCHAR(100) OUTPUT,            
@USERID INT,            
@EMPDETAIL xml,            
@CHECKEDBY VARCHAR(100)='',            
@TSTOCKNO VARCHAR(50)='',            
@WEIGHT DECIMAL (18,3)=0,            
@PENALITYAMT FLOAT=0,            
@PENALITYREMARK VARCHAR(300)='',            
@LENGTH VARCHAR(10)='',            
@WIDTH VARCHAR(10)='',            
@AREA DECIMAL(18,4)=0,            
@COMMRATE FLOAT=0,            
@QUALITYTYPE INT=1,            
@QAPERSONNAME VARCHAR(50)='',            
@ACTUALWIDTH VARCHAR(20)='',            
@ACTUALLENGTH VARCHAR(20)='',            
@MAXREJECTEDGATEPASSNO VARCHAR(50)='' OUTPUT,            
@RETURNREMARK VARCHAR(200)='',            
@qcdetail xml,            
@Bazar_Empcode varchar(100)='',            
@100_PROCESS_REC_ID INT=0 OUTPUT,            
@100_ISSUEORDERID INT=0 OUTPUT,            
@username varchar(50)=''  ,          
@PartyChallanNo Varchar(50)='' ,    
@StockStatus Int = 0,   
@BranchId int=1
AS            
BEGIN            
 DECLARE @CALTYPE INT,@UNITID INT,@ITEM_FINISHED_ID INT,            
 @RATE DECIMAL(10,2),@ISSUE_DETAIL_ID INT,@ORDERID INT,@SRNO INT,@MAXSRNO INT,@PROCESS_REC_DETAIL_ID INT,@AMOUNT DECIMAL(18,2),@COMMAMT DECIMAL(18,2),            
 @RECQTY INT,@BAZARWEIGHT DECIMAL(18,3),@PCWEIGHT DECIMAL(10,3),@PREFIX VARCHAR(10),@ITEM_CODE VARCHAR(10),            
 @RECEIVEDQTY INT,@ORDEREDQTY INT,@BAZARRECCHECK TINYINT,@MASTERCOMPANYID INT,@VARSTOCKNO BIGINT,@POSTFIX INT,@PROCESSID INT,@PROCESSDETAILID BIGINT,            
 @INTERNAL_CONSUMEDQTYCALTYPE INT,@EMPID INT,@LOOMBAZARBYPERCENTAGE INT,@WORKPERCENTAGE FLOAT=0,@MAINTAINPROCESSRECEIVESEQ INT,            
 @MAXREJECTEDID INT=0,@STOCKNO BIGINT=0,@itemId int,@AUTOISS100OFWEAVINGPROCESS INT=0,@100_PROCESSID int,@100_STREMPID int,@ROWCNT int            
 ,@StopBackDateEntryOnAllForms int=0,@CurrentDate datetime,@ReturnDate varchar(30)='',@ReturnHolidayType varchar(30)='' ,@IssRecNo varchar(100)=''           
            
 DECLARE @UNITSID INT,@100_ISSUE_DETAIL_ID INT,@100_@RECEIVEDETAILID INT,@DATE_STAMP VARCHAR(30)='',@100_RECCHALLANNO VARCHAR(30)='',@PACKINGTYPEID INT,@ARTICLENO varchar(50)='',@VARTYPEID INT=0,@BATCHNO VARCHAR(30),            
 @100_MSG VARCHAR(100)='',@ISS_RECFLAG INT, @UserType INT,@UserTypeNew int ,@CompanyWiseChallanNoGenerated int   ,@Bouns Float, @BounsAmt Float, @PURCHASEFLAG Int, @FinisherRate DECIMAL(10,2), @NewArea Float,@OnePcsConsumption DECIMAL (18,3)=0           
            
 SELECT @StopBackDateEntryOnAllForms=StopBackDateEntryOnAllForms,@MAINTAINPROCESSRECEIVESEQ=MAINTAINPROCESSRECEIVESEQ,@AUTOISS100OFWEAVINGPROCESS=[AUTOISS100%OFWEAVINGPROCESS],            
 @MASTERCOMPANYID=VARCOMPANYNO ,@CompanyWiseChallanNoGenerated=CompanyWiseChallanNoGenerated           
 FROM MASTERSETTING            
            
 --******************Stop BackDate Entry            
             
 Select @UserType = IsNull(UserType, 0) From NewUserDetail Where UserID = @USERID             
            
 IF (@StopBackDateEntryOnAllForms=1)            
 BEGIN            
  select @CurrentDate=dateadd(dd,0,DateDiff(dd,0,GETDATE()))            
            
  EXEC PRO_GETRETURNDATE_HOLIDAYNAME @COMPANYID=@COMPANYID,@DATE=@RECEIVEDATE,@MASTERCOMPANYID=@MASTERCOMPANYID,@ReturnDate=@ReturnDate Output,@ReturnHolidayType=@ReturnHolidayType output,          
  @UserTypeId=@UserType, @UserTypeNew = @UserTypeNew Output, @USERID = @USERID           
              
  IF(@UserTypeNew =1)            
  BEGIN            
   IF(CAST(@RECEIVEDATE as Datetime)<@CurrentDate)            
   BEGIN            
    SET @MSG='DATE NOT BE LESS THAN CURRENT DATE.'            
    RETURN            
   END            
  END            
            
  ----EXEC PRO_GETRETURNDATE_HOLIDAYNAME @COMPANYID=@COMPANYID,@DATE=@RECEIVEDATE,@MASTERCOMPANYID=@MASTERCOMPANYID,@ReturnDate=@ReturnDate Output,@ReturnHolidayType=@ReturnHolidayType output         
            
  IF(CAST(@RECEIVEDATE as Datetime)=@ReturnDate)            
  BEGIN            
   SET @MSG='PLEASE SELECT ANOTHER DATE DUE TO HOLIDAY.'            
   RETURN            
  END            
  ELSE            
  BEGIN            
   IF(@ReturnDate<>'')            
   BEGIN            
    Set @RECEIVEDATE = @ReturnDate           
   END            
                
  END                
 END            
--******************END Stop BackDate Entry            
             
 ----Select @UserType = IsNull(UserType, 0) From NewUserDetail Where UserID = @USERID              
            
 ----IF (@MASTERCOMPANYID = 21 And @UserType <> 1)            
 ----BEGIN            
 ---- Set @RECEIVEDATE = GetDate();            
 ----END            
             
            
            
 --************CHECK STOCK PENDING OR NOT            
 IF NOT EXISTS(SELECT LS.TSTOCKNO FROM LOOMSTOCKNO LS WHERE LS.TSTOCKNO=@TSTOCKNO AND LS.BAZARSTATUS=0 AND LS.ProcessID = 1)            
 BEGIN            
  set @MSG='Stock No. does not exists or Pending.'            
  return            
 END            
 --*******************            
 Declare @DTEMPLOYEE as Table(EMPID int,Processid int,WorkPercentage float)            
 INSERT INTO @DTEMPLOYEE (EMPID,Processid,WorkPercentage)                       
 SELECT  p.value('@empid','INT'), p.value('@Processid','INT'), p.value('@Workpercentage','FLOAT')  FROM @empdetail.nodes('/ROOT/Stock')n(p);            
            
 SET @PROCESSID=1            
            
 --SELECT @MAINTAINPROCESSRECEIVESEQ=MAINTAINPROCESSRECEIVESEQ,@AUTOISS100OFWEAVINGPROCESS=[AUTOISS100%OFWEAVINGPROCESS] FROM MASTERSETTING            
            
 IF @LENGTH='' OR @WIDTH='' OR @AREA<=0            
 BEGIN            
  SET @MSG='LENGTH OR WIDTH OR AREA CAN NOT BE BLANK OR ZERO.'            
  RETURN            
 END            
            
 SELECT @BAZARRECCHECK=BAZARRECCHECK,@INTERNAL_CONSUMEDQTYCALTYPE=INTERNAL_CONSUMEDQTYCALTYPE,@LOOMBAZARBYPERCENTAGE=LOOMBAZARBYPERCENTAGE FROM MASTERSETTING             
 --**CHECK WORKPERCENTAGE            
 IF @LOOMBAZARBYPERCENTAGE=1            
 BEGIN            
  SELECT @WORKPERCENTAGE=ISNULL(SUM(WORKPERCENTAGE),0) FROM @DTEMPLOYEE            
  IF @WORKPERCENTAGE>100 OR @WORKPERCENTAGE<100            
  BEGIN            
   SET @MSG='WORK(%) CAN NOT BE GREATER THAN  OR LESS THAN 100 %.'            
   RETURN            
  END            
 END            
 --***RETURN CARPET            
 IF @QUALITYTYPE=4            
 BEGIN            
  SELECT @STOCKNO=STOCKNO,@ITEM_FINISHED_ID=ITEM_FINISHED_ID FROM LOOMSTOCKNO WHERE TSTOCKNO=@TSTOCKNO AND PROCESSID = 1             
  IF NOT EXISTS(SELECT GATEPASSNO FROM PRODUCTIONRECEIVEREJECTEDSTOCK WHERE GATEPASSNO=@MAXREJECTEDGATEPASSNO)            
  BEGIN             
   SELECT  @MAXREJECTEDGATEPASSNO =ISNULL(MAX(GATEPASSNO),0)+1 FROM PRODUCTIONRECEIVEREJECTEDSTOCK            
  END            
  SELECT @MAXREJECTEDID=ISNULL(MAX(ID),0)+1 FROM PRODUCTIONRECEIVEREJECTEDSTOCK            
  INSERT INTO PRODUCTIONRECEIVEREJECTEDSTOCK(ID,COMPANYID,GATEPASSNO,REJECTEDDATE,USERID,RECCHALLANNO,PROCESS_REC_ID,PROCESS_REC_DETAIL_ID,ITEM_FINISHED_ID,QTY,TSTOCKNO,MASTERCOMPANYID,STOCKNO,RETURNREMARK,ISSUEORDERID)            
  VALUES(@MAXREJECTEDID,@COMPANYID,@MAXREJECTEDGATEPASSNO,GETDATE(),@USERID,@RECEIVENO,@PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID,@ITEM_FINISHED_ID,1,'',@MASTERCOMPANYID,@STOCKNO,@RETURNREMARK,@ISSUEORDERID)            
              
  RETURN            
 END            
 --***            
 --CHECK BEAM ISSUED OR NOT            
 IF @BAZARRECCHECK=1            
 BEGIN            
  EXEC PRO_BAZARRECCHECK @QTY=1,@ISSUEORDERID=@ISSUEORDERID,@ISSUEDETAILID=@ISSUE_DETAIL_ID,@ITEM_FINISHED_ID=@ITEM_FINISHED_ID,@MSG=@MSG OUTPUT            
  IF @MSG<>''            
  BEGIN            
   SET @MSG=@MSG            
   RETURN            
  END               
 END            
--**************GET STOCK DETAIL            
 EXEC PRO_CHECKLOOMSTOCKNO  @ISSUEORDERID=@ISSUEORDERID,@TSTOCKNO=@TSTOCKNO,@MSG=@MSG OUTPUT            
 IF @MSG='' OR @MSG IS NULL            
 BEGIN            
  IF EXISTS(SELECT TSTOCKNO FROM CARPETNUMBER WHERE TSTOCKNO=@TSTOCKNO)            
  BEGIN            
   SET @MSG='BAZAR ALREADY DONE FOR THIS STOCK NO.'            
   RETURN            
  END            
 END            
 ELSE            
 BEGIN            
  SET @MSG=@MSG            
  RETURN            
 END            
 --**************            
 --****************GET DETAIL STOCK DETAIL            
 SELECT @ITEM_FINISHED_ID=PID.ITEM_FINISHED_ID,@RATE=PID.RATE,            
 @ISSUE_DETAIL_ID=PID.ISSUE_DETAIL_ID,@ORDERID=PID.ORDERID,@RECQTY=1,@BAZARWEIGHT=@WEIGHT,@PCWEIGHT=@WEIGHT,            
 @ITEM_CODE=IM.ITEM_CODE,@RECEIVEDQTY=ISNULL(DBO.F_GETPRODUCTIONRECQTY(@ISSUEORDERID,PID.ITEM_FINISHED_ID,PID.ISSUE_DETAIL_ID),0)            
 ,@ORDEREDQTY=(PID.QTY-ISNULL(PID.CANCELQTY,0)),@PREFIX=LT.PREFIX,@POSTFIX=LT.POSTFIX,@CALTYPE=PIM.CALTYPE,@UNITID=PIM.UNITID ,@EMPID=PIM.EMPID,            
 @itemId=vf.ITEM_ID,@UNITSID=Pim.Units , @Bouns = PID.Bonus, @BounsAmt = PID.BonusAmt, @FinisherRate = PID.RATE2, @PURCHASEFLAG = PIM.PURCHASEFLAG, @NewArea = PID.AREA              
 FROM LOOMSTOCKNO  LT             
 INNER JOIN PROCESS_ISSUE_DETAIL_1 PID ON LT.ISSUEORDERID=PID.ISSUEORDERID AND LT.ISSUEDETAILID=PID.ISSUE_DETAIL_ID             
 INNER JOIN PROCESS_ISSUE_MASTER_1 PIM ON PID.ISSUEORDERID=PIM.ISSUEORDERID             
 INNER JOIN V_FINISHEDITEMDETAIL VF ON PID.ITEM_FINISHED_ID=VF.ITEM_FINISHED_ID             
 INNER JOIN ITEM_MASTER IM ON VF.ITEM_ID=IM.ITEM_ID             
 WHERE LT.TSTOCKNO=@TSTOCKNO AND LT.ISSUEORDERID=@ISSUEORDERID AND LT.ProcessID = 1             
 --****************            
 IF @BAZARRECCHECK=1            
 BEGIN            
  EXEC PRO_BAZARRECCHECK @QTY=1,@ISSUEORDERID=@ISSUEORDERID,@ISSUEDETAILID=@ISSUE_DETAIL_ID,@ITEM_FINISHED_ID=@ITEM_FINISHED_ID,@MSG=@MSG OUTPUT            
  IF @MSG<>''            
  BEGIN            
   SET @MSG=@MSG            
   RETURN            
  END               
 END            
 --****************CHECK EMPLOYEE            
 IF @Bazar_Empcode<>''            
 Begin            
  IF @EMPID=0            
  begin            
   IF not exists(select dt.EMPID From @DTEMPLOYEE dt inner join EmpInfo ei on dt.EMPID=ei.EmpId            
   and ei.EmpCode in(select Items from dbo.Split(@Bazar_Empcode,',')))            
   begin            
    set @MSG='Invalid emp code for this stock No.'            
    return            
   end            
  end            
 end            
 --****************            
 IF @PROCESS_REC_ID=0 OR @PROCESS_REC_ID IS NULL            
 BEGIN            
  IF @MAINTAINPROCESSRECEIVESEQ=1            
  BEGIN            
   SELECT @PROCESS_REC_ID=ISNULL(MAX(PROCESS_REC_ID),0)+1  FROM MASTERSETTING            
  END            
  ELSE            
  BEGIN            
   SELECT @PROCESS_REC_ID=ISNULL(MAX(PROCESS_REC_ID),0)+1 FROM PROCESS_RECEIVE_MASTER_1            
  END             
          
  select @RECEIVENO as receiveno        
  IF @RECEIVENO=''  OR @RECEIVENO IS NULL            
  BEGIN            
   SET @RECEIVENO=@PROCESS_REC_ID            
  END            
      IF(@CompanyWiseChallanNoGenerated=1)          
    BEGIN              
     EXEC [DBO].[PRO_GatePassNew] @TRANTYPE=1,@ISSUERECNO=@IssRecNo OUTPUT,@TABLENAME='PROCESS_RECEIVE_MASTER_1',@TABLEID=@PROCESS_REC_ID,@CompanyId=@COMPANYID,@VarCompanyId=@MASTERCOMPANYID          
     SET @RECEIVENO=@IssRecNo           
    END          
         
  INSERT INTO PROCESS_RECEIVE_MASTER_1(PROCESS_REC_ID,EMPID,RECEIVEDATE,UNITID,USERID,CHALLANNO,COMPANYID,REMARKS,CALTYPE,BranchId,PartyChallanNo)                              
  VALUES(@PROCESS_REC_ID,@EMPID,@RECEIVEDATE,@UNITID,@USERID,@RECEIVENO,@COMPANYID,'',@CALTYPE,@BranchId,@PartyChallanNo)            
            
  IF @MAINTAINPROCESSRECEIVESEQ=1            
  BEGIN             
   UPDATE MASTERSETTING SET PROCESS_REC_ID =@PROCESS_REC_ID                    
  END             
 END            
 ELSE            
 BEGIN            
  SELECT @RECEIVENO=CHALLANNO FROM PROCESS_RECEIVE_MASTER_1 WHERE PROCESS_REC_ID=@PROCESS_REC_ID            
            
  IF @@ROWCOUNT=0            
  BEGIN            
   IF @RECEIVENO=''  OR @RECEIVENO IS NULL            
   BEGIN            
    SET @RECEIVENO=@PROCESS_REC_ID            
   END            
      IF(@CompanyWiseChallanNoGenerated=1)          
    BEGIN              
     EXEC [DBO].[PRO_GatePassNew] @TRANTYPE=1,@ISSUERECNO=@IssRecNo OUTPUT,@TABLENAME='PROCESS_RECEIVE_MASTER_1',@TABLEID=@PROCESS_REC_ID,@CompanyId=@COMPANYID,@VarCompanyId=@MASTERCOMPANYID          
     SET @RECEIVENO=@IssRecNo           
    END          
         
   INSERT INTO PROCESS_RECEIVE_MASTER_1(PROCESS_REC_ID,EMPID,RECEIVEDATE,UNITID,USERID,CHALLANNO,COMPANYID,REMARKS,CALTYPE,BranchId,PartyChallanNo)                              
   VALUES(@PROCESS_REC_ID,@EMPID,@RECEIVEDATE,@UNITID,@USERID,@RECEIVENO,@COMPANYID,'',@CALTYPE,@BranchId,@PartyChallanNo)            
  END             
  BEGIN            
   SELECT COMPANYID FROM PROCESS_RECEIVE_MASTER_1 WHERE PROCESS_REC_ID = @PROCESS_REC_ID AND COMPANYID = @COMPANYID             
   IF @@ROWCOUNT = 0            
   BEGIN            
    Set @MSG = 'THIS STOCK NO BELONGS TO OTHER COMPANY'            
    RETURN            
   END             
  END            
 END            
 ---DETAIL            
             
 ---CHECK PENDING QTY            
 IF((@ORDEREDQTY-@RECEIVEDQTY)<@RECQTY)                  
 BEGIN            
  SET @MSG='RECEIVE QTY CAN NOT BE GREATER THAN PENDING QTY.'            
  RETURN            
 END             
    
 IF(@MASTERCOMPANYID = 42)    
    BEGIN    
     IF(@Area > @NewArea)    
     BEGIN    
    Set @Area = @NewArea         
     END    
    
     Set @Area= round(Cast(@Area as decimal(28, 3)),2,3) -----Area Two Digit Without Round    
    
     IF(@PURCHASEFLAG = 1 And @StockStatus = 2)    
     Begin    
    Set @Rate = @Rate + @FinisherRate     
     End    
    END    
    
    
         
 WHILE(@RECQTY>0)            
 BEGIN            
  IF @CALTYPE=0            
  BEGIN            
   SET @AMOUNT=@RATE*@AREA            
   SET @COMMAMT=@COMMRATE*@AREA            
  END            
  ELSE            
  BEGIN            
   SET @AMOUNT=@RATE            
   SET @COMMAMT=@COMMRATE            
  END            
 IF @QUALITYTYPE=3            
  BEGIN            
   SELECT @AMOUNT=0,@COMMAMT=0            
  END            
      IF(@MASTERCOMPANYID = 42)    
   BEGIN       
    
     Select @OnePcsConsumption=Round(ISNULL(sum(ROUND(CASE WHEN OCD.ICALTYPE=0 OR OCD.ICALTYPE=2 THEN CASE WHEN PM.UNITID=1 THEN PD.QTY*PD.AREA*OCD.IQTY*1.196     
     ELSE PD.QTY * Round(VF1.AreaFt * 144.0 / 1296.0, 2, 2) * OCD.IQTY END ELSE     
     CASE WHEN PM.UNITID=1 THEN PD.QTY*OCD.IQTY*1.196 ELSE PD.QTY*OCD.IQTY END END,3)),0)/isnull((PD.Qty),0),4)     
    FROM PROCESS_ISSUE_MASTER_1 PM(Nolock)     
    JOIN PROCESS_ISSUE_DETAIL_1 PD(Nolock) ON PD.IssueOrderID = PM.ISSUEORDERID     
    JOIN V_FINISHEDITEMDETAIL VF1 ON VF1.ITEM_FINISHED_ID = PD.ITEM_FINISHED_ID     
    JOIN PROCESS_CONSUMPTION_DETAIL OCD(Nolock) ON OCD.IssueOrderID = PM.IssueOrderID And OCD.ISSUE_DETAIL_ID = PD.ISSUE_DETAIL_ID And OCD.ProcessID = 1     
    JOIN UNIT U(Nolock) ON U.UNITID = OCD.IUNITID     
    Where PM.ISSUEORDERID = @ISSUEORDERID and PD.ISSUE_DETAIL_ID=@ISSUE_DETAIL_ID    
    group by PD.Qty    
    
    SET @OnePcsConsumption=@OnePcsConsumption*@RECQTY    
    
   END    
       
    
    
    
  SELECT @PROCESS_REC_DETAIL_ID=ISNULL(MAX(PROCESS_REC_DETAIL_ID),0)+1 FROM PROCESS_RECEIVE_DETAIL_1            
            
  INSERT INTO PROCESS_RECEIVE_DETAIL_1(PROCESS_REC_DETAIL_ID,PROCESS_REC_ID,ITEM_FINISHED_ID,LENGTH,WIDTH,AREA,RATE,AMOUNT,QTY,            
  WEIGHT,COMM,COMMAMT,ISSUEORDERID,ISSUE_DETAIL_ID,ORDERID,PENALITY,PREMARKS,QUALITYTYPE,GATEPASSNO,FLAGFIXORWEIGHT,CHECKEDBY,QAPERSONNAME,ACTUALWIDTH,ACTUALLENGTH,FRRATE2,ConsumptionWeight)            
  VALUES(@PROCESS_REC_DETAIL_ID,@PROCESS_REC_ID,@ITEM_FINISHED_ID,@LENGTH,@WIDTH,@AREA,@RATE,@AMOUNT,1,@PCWEIGHT,@COMMRATE,@COMMAMT,@ISSUEORDERID,@ISSUE_DETAIL_ID,            
  @ORDERID,@PENALITYAMT,@PENALITYREMARK,@QUALITYTYPE,@PROCESS_REC_ID,1,@CHECKEDBY,@QAPERSONNAME,@ACTUALWIDTH,@ACTUALLENGTH,@FinisherRate,@OnePcsConsumption)            
            
 --INSERT INTO REJECTED STOCKNO             
             
  UPDATE PROCESS_ISSUE_DETAIL_1 SET PQTY=PQTY-@RECQTY WHERE ISSUEORDERID=@ISSUEORDERID AND ISSUE_DETAIL_ID=@ISSUE_DETAIL_ID            
  UPDATE LOOMSTOCKNO SET PROCESS_REC_ID=@PROCESS_REC_ID,PROCESS_REC_DETAIL_ID=@PROCESS_REC_DETAIL_ID,BAZARSTATUS=1             
  WHERE TSTOCKNO=@TSTOCKNO AND ISSUEORDERID=@ISSUEORDERID AND PROCESSID = 1             
           
        
     IF(@MASTERCOMPANYID = 42)    
   BEGIN    
    IF(@PCWEIGHT>@OnePcsConsumption)    
    BEGIN    
     SET @PCWEIGHT=@OnePcsConsumption    
    END    
   END    
    
    
  EXEC PRO_PROCESS_RECEIVECONSUMPTION @PROCESS_REC_ID=@PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID=@PROCESS_REC_DETAIL_ID,@FINISHEDID=@ITEM_FINISHED_ID,@PROCESSID=1,@AREA=@AREA,@WEIGHT=@PCWEIGHT,@UNITID=@UNITID,@ISSUE_ORDER_ID=@ISSUEORDERID,        
  @ISSUE_DETAIL_ID=@ISSUE_DETAIL_ID,@FLAGFIXORWEIGHT=1,@QTY=1,@CALTYPE=@INTERNAL_CONSUMEDQTYCALTYPE            
            
 --INSERT INTO CARPETNUMBER              
            
  IF EXISTS(SELECT TSTOCKNO FROM CARPETNUMBER WHERE TSTOCKNO=@TSTOCKNO)            
  BEGIN            
   SET @MSG='STOCK NUMBER ALREADY EXISTS IN TABLE.'            
   RETURN            
  END            
            
  SELECT @VARSTOCKNO=ISNULL(MAX(STOCKNO),0)+1 FROM CARPETNUMBER            
            
  INSERT INTO CARPETNUMBER(STOCKNO,ITEM_FINISHED_ID,TYPEID,PACK,ORDERID,TSTOCKNO,PREFIX,POSTFIX,COMPANYID,PROCESS_REC_ID,PROCESS_REC_DETAIL_ID,REC_DATE,CURRENTPROSTATUS,ISSRECSTATUS)            
       VALUES(@VARSTOCKNO,@ITEM_FINISHED_ID,1,0,@ORDERID,@TSTOCKNO,@PREFIX,@POSTFIX,@COMPANYID,@PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID,@RECEIVEDATE,@PROCESSID,0)            
            
  SELECT @PROCESSDETAILID=ISNULL(MAX(PROCESSDETAILID),0)+1 FROM  PROCESS_STOCK_DETAIL            
            
  INSERT INTO PROCESS_STOCK_DETAIL(PROCESSDETAILID,STOCKNO,FROMPROCESSID,TOPROCESSID,ORDERID,RECEIVEDATE,COMPANYID,RECEIVEDETAILID,USERID,ISSUEDETAILID,HISSABFLAG)            
  VALUES (@PROCESSDETAILID,@VARSTOCKNO,0,@PROCESSID,@ORDERID,@RECEIVEDATE,@COMPANYID,@PROCESS_REC_DETAIL_ID,@USERID,@ISSUE_DETAIL_ID,0)            
            
  IF(@MasterCompanyId=21)            
  BEGIN               
               
   EXEC Pro_GenerateBazaarPacking_Seq_No_Weekly @Item_Finished_Id=@ITEM_FINISHED_ID,@CompanyId=@CompanyId, @PROCESS_REC_ID = @PROCESS_REC_ID,            
   @PROCESS_REC_DETAIL_ID = @PROCESS_REC_DETAIL_ID, @ReceiveDate = @RECEIVEDATE, @StockNo = 0, @FROMPROCESSID = 1, @ToProcessId = 0, @PackingStatus = 0,            
   @PackingReceiveDate = '', @UserId = @USERID, @MasterCompanyId = @MasterCompanyID,@MSG=@MSG OUTPUT             
                 
  END            
            
  --INSERT INTO EMPLOYEE_PROCESSRECEIVENO            
  IF @Bazar_Empcode=''            
  BEGIN            
   INSERT INTO EMPLOYEE_PROCESSRECEIVENO (PROCESSID,ISSUEORDERID,ISSUEDETAILID,PROCESS_REC_ID,PROCESS_REC_DETAIL_ID,EMPID,WORKPERCENTAGE)           
   SELECT @PROCESSID,@ISSUEORDERID,@ISSUE_DETAIL_ID,@PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID,EMPID,WORKPERCENTAGE            
   FROM @DTEMPLOYEE WHERE PROCESSID=@PROCESSID             
  END            
  ELSE            
  BEGIN            
   INSERT INTO EMPLOYEE_PROCESSRECEIVENO (PROCESSID,ISSUEORDERID,ISSUEDETAILID,PROCESS_REC_ID,PROCESS_REC_DETAIL_ID,EMPID,WORKPERCENTAGE)            
   SELECT @PROCESSID,@ISSUEORDERID,@ISSUE_DETAIL_ID,@PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID,dt.EMPID,dt.WORKPERCENTAGE            
   FROM @DTEMPLOYEE dt inner join EmpInfo ei on dt.EMPID=ei.EmpId and ei.EmpCode in(select Items from dbo.Split(@Bazar_Empcode,',')) WHERE dt.PROCESSID=@PROCESSID             
  END            
  --EXEC PRO_SAVEEMPLOYEE_RECEIVENO_LOOM @PROCESSID=@PROCESSID,@ISSUEORDERID=@ISSUEORDERID,@ISSUEDETAILID=@ISSUE_DETAIL_ID,@PROCESS_REC_ID=@PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID=@PROCESS_REC_DETAIL_ID,@DTEMPLOYEE=@DTEMPLOYEE            
--                            
  SET @RECQTY=@RECQTY-1            
 END                
 SET @SRNO=@SRNO+1            
--********INSERT INTO QC PARAMETER DETAIL            
 DECLARE @TT_QCCHECK TT_QCCHECKFIRSTPROCESS            
 DECLARE @TABLEQCPARAMETER AS TABLE(PARAID INT,Remark varchar(100))             
 Declare @Qcmsg varchar(100)            
 INSERT INTO @TABLEQCPARAMETER (PARAID,Remark)                       
 SELECT  p.value('@qccode','INT'), p.value('@remark','varchar(100)')  FROM @qcdetail.nodes('/ROOT/qc')n(p);            
            
 INSERT INTO @TT_QCCHECK            
 SELECT DISTINCT @PROCESS_REC_ID,@PROCESS_REC_DETAIL_ID,QP.PARANAME,CASE WHEN TC.PARAID IS NULL THEN 1 ELSE 0 END AS PARAMVALUE,CASE WHEN TC.PARAID IS NULL THEN '' ELSE TC.REMARK ENd AS REASON            
 FROM QCMASTER QM INNER JOIN QCPARAMETER QP ON QM.ParaID=QP.PARAID             
 INNER JOIN ITEM_MASTER VF ON QM.ITEMID=VF.ITEM_ID AND QP.CATEGORYID=VF.CATEGORY_ID              
 LEFT JOIN @TABLEQCPARAMETER TC ON QP.PARAID=TC.PARAID            
 WHERE VF.ITEM_ID=@ITEMID AND QP.PROCESSID=@PROCESSID            
            
 Exec Pro_saveQc @DTRECORD=@TT_QCCHECK,@msg=@qcmsg output            
 --********               
 --*******AUTO ENTRY IN 100% OF PROCESS            
 IF @AUTOISS100OFWEAVINGPROCESS=1            
 BEGIN            
  SELECT @100_PROCESSID=PROCESS_NAME_ID FROM PROCESS_NAME_MASTER WHERE PROCESS_NAME='100% '+'WEAVING'            
  IF @@ROWCOUNT>0            
  BEGIN            
   SELECT @100_STREMPID=EI.EMPID FROM EMPINFO EI INNER JOIN EMPPROCESS EMP ON EI.EMPID=EMP.EMPID AND EMP.PROCESSID=@100_PROCESSID AND EI.EMPCODE<>''            
   AND EI.EMPNAME=@USERNAME            
            
   SET @ROWCNT=@@ROWCOUNT            
            
   IF @ROWCNT=0            
   BEGIN            
    SELECT @100_STREMPID=EI.EMPID FROM EMPINFO EI INNER JOIN EMPPROCESS EMP ON EI.EMPID=EMP.EMPID AND EMP.PROCESSID=@100_PROCESSID AND EI.EMPCODE<>''                    
    SET @ROWCNT=@@ROWCOUNT            
   END            
            
   IF @ROWCNT>0            
   BEGIN                    
    SELECT @100_ISSUE_DETAIL_ID=0,@100_@RECEIVEDETAILID=0,@WEIGHT=0,@RATE=0,@DATE_STAMP='',@100_RECCHALLANNO='',@PACKINGTYPEID=0,@ARTICLENO='',@BATCHNO='',@VARTYPEID=1,@ISS_RECFLAG=1            
                      
    EXEC PRO_NEXTPROCESSISSUEFOROTHER @ISSUEORDERID=@100_ISSUEORDERID OUTPUT, @EMPID=0, @ASSIGN_DATE=@RECEIVEDATE, @STATUS='COMPLETE', @UNITID=2,             
    @USER_ID=@USERID, @REMARKS='', @INSTRUCTION='', @COMPANYID=@COMPANYID, @ISSUE_DETAIL_ID=@100_ISSUE_DETAIL_ID, @ITEM_FINISHED_ID=@ITEM_FINISHED_ID,             
    @LENGTH=@LENGTH, @WIDTH=@WIDTH, @AREA=@AREA, @RATE=@RATE, @AMOUNT=0, @QTY=1, @REQBYDATE=@RECEIVEDATE, @PQTY=1, @COMM=0, @COMMAMT=0, @ORDERID=@ORDERID, @CALTYPE=1,            
    @STOCKNO=@VARSTOCKNO, @FROMPROCESSID=@PROCESSID, @TOPROCESSID=@100_PROCESSID, @ORDERDATE=@RECEIVEDATE, @RECEIVEDATE=@RECEIVEDATE, @RECEIVEDETAILID=@100_@RECEIVEDETAILID,            
    @VARTYPEID=1, @ISS_RECFLAG=@ISS_RECFLAG, @WEIGHT=@WEIGHT, @PROCESS_REC_ID=@100_PROCESS_REC_ID OUTPUT, @STREMPID=@100_STREMPID, @UNITSID=@UNITSID,            
    @MSG=@100_MSG OUTPUT, @DATE_STAMP='', @RECCHALLANNO=@100_RECCHALLANNO OUTPUT, @PACKINGTYPEID=0, @ARTICLENO='', @BATCHNO='',            
    @MASTERCOMPANYID=@MASTERCOMPANYID,@ISSUEWITHOUTRATE=1             
   END            
  END            
 END            
--*******END                            
            
 --UPDATE STATUS            
 EXEC PRO_UPDATELOOMORDERSTATUS  @ISSUEORDERID=@ISSUEORDERID            
 ----            
            
END 