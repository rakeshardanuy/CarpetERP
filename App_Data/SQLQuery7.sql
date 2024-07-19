DECLARE @OrderId int=17378,@ProcessId int=13;
DECLARE @SQL NVARCHAR(MAX),
@ProcessIssueDetail NVARCHAR(250),@ProcessIssueMaster NVARCHAR(250),
@ProcessReceiveDetail NVARCHAR(250),@ProcessReceiveMaster NVARCHAR(250) 
SET @ProcessIssueMaster = 'PROCESS_ISSUE_MASTER_' + CAST(@ProcessId AS NVARCHAR) + ''    
SET @ProcessIssueDetail='PROCESS_ISSUE_DETAIL_' + CAST(@ProcessId AS NVARCHAR) + '' 
SET @ProcessReceiveMaster = 'PROCESS_RECEIVE_MASTER_' + CAST(@ProcessId AS NVARCHAR) + ''    
SET @ProcessReceiveDetail='PROCESS_RECEIVE_DETAIL_' + CAST(@ProcessId AS NVARCHAR) + '' 
SET @SQL=
'With IssueItem(IssueId,ChallanNo,DetailId,EmpId,OrderId,FinishedId,AssignDate,RequestDate,
IssueDate,ItemRate,IssueQty,RequiredQty,CancelQty,RowNo)
AS
(Select x.IssueOrderId,x.ChallanNo,y.Issue_Detail_Id,zz.Empid,y.Orderid,y.Item_Finished_Id,x.AssignDate,y.ReqByDate, 
Max(y.DATEADDED) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) IssueDate,
AVG(y.Rate) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) Rate,
SUM(y.Qty) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) IssueQty,
z.QtyRequired,
SUM(y.CancelQty) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) CancelQty,
ROW_NUMBER() OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) RowNo
from '+@ProcessIssueMaster+' x WITH (NOLOCK) Inner Join '+@ProcessIssueDetail+' y WITH (NOLOCK)
on x.IssueOrderId=y.IssueOrderId  inner join OrderDetail z  on y.Orderid=z.OrderId and  y.Item_Finished_Id=z.Item_Finished_Id
inner Join Employee_ProcessOrderNo zz on zz.IssueOrderId=x.IssueOrderId and zz.IssueDetailId=y.Issue_Detail_Id 
and zz.ProcessId='+ CAST(@ProcessId AS NVARCHAR)+' Where y.OrderId='+ CAST(@OrderId AS NVARCHAR)+'),
ReceiveItem(ReceiveId,DetailId,OrderId,IssueId,EmpId,FinishedId,ReceiveQty,ReceiveDate,RowNo)
AS
(select x.Process_Rec_Id,y.Process_Rec_Detail_Id,y.OrderId,y.IssueOrderId,x.UserId,
y.Item_Finished_Id,
SUM(y.Qty) OVER(PARTITION BY y.Orderid,y.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,y.IssueOrderId),
Max(y.DATEADDED) OVER(PARTITION BY y.Orderid,y.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,y.IssueOrderId) ReceiveDate,
ROW_NUMBER() OVER(PARTITION BY y.Orderid,y.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,y.IssueOrderId) RowNo 
from '+@ProcessReceiveMaster+' x WITH (NOLOCK) Inner Join '+@ProcessReceiveDetail+' y WITH (NOLOCK)
ON x.Process_Rec_Id=y.Process_Rec_Id and y.OrderId='+CAST(@OrderId AS NVARCHAR)+'
)
Select VF.DESIGNNAME,x.ChallanNo,x.EmpId VendorId,IsNULL(emp.EMPNAME,'''') VendorName,VF.ITEM_NAME+'' ''+VF.QUALITYNAME+'' ''+VF.COLORNAME+'' ''+VF.SHAPENAME+'' ''+VF.ShadeColorName MaterialName, 
x.IssueId,x.EmpId,x.OrderId,x.FinishedId,x.AssignDate IssDate,x.RequestDate ReqDate,x.IssueDate,y.ReceiveDate RecDate,
x.ItemRate,x.IssueQty,x.RequiredQty,x.CancelQty,y.ReceiveQty From IssueItem x Left Join ReceiveItem y 
On x.OrderId=y.OrderId and x.IssueId=y.IssueId and x.FinishedId=y.FinishedId and x.RowNo=y.RowNo
inner JOIN V_FINISHEDITEMDETAIL VF ON x.FinishedId=VF.ITEM_FINISHED_ID
inner JOIN EMPINFO emp WITH (NOLOCK)  ON x.EmpId=emp.EmpId   
Where x.OrderId='+CAST(@OrderId AS NVARCHAR)+' and x.RowNo=1
Order BY  x.IssueId'
Print @SQL

select * from PROCESS_NAME_MASTER


Select y.Orderid,x.IssueOrderId,x.ChallanNo,y.Item_Finished_Id,z.QtyRequired,
SUM(y.Qty) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) IssueByChallan,
SUM(y.Qty) OVER(PARTITION BY y.Orderid,y.Item_Finished_Id ORDER BY y.Orderid) IssueByOrder,
Max(y.DATEADDED) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) IssueDate,
AVG(y.Rate) OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) Rate,
y.Issue_Detail_Id,y.Qty,x.AssignDate,y.ReqByDate,y.dateadded,
ROW_NUMBER() OVER(PARTITION BY y.Orderid,x.IssueOrderId,y.Item_Finished_Id ORDER BY y.Orderid,x.IssueOrderId) RowNo
from PROCESS_ISSUE_MASTER_13 x WITH (NOLOCK) Inner Join PROCESS_ISSUE_DETAIL_13 y WITH (NOLOCK)
on x.IssueOrderId=y.IssueOrderId  
inner join OrderDetail z  on y.Orderid=z.OrderId and  
y.Item_Finished_Id=z.Item_Finished_Id 
Where y.OrderId=81
select top 2 * from PROCESS_ISSUE_DETAIL_13
select top 2 * from PROCESS_ISSUE_MASTER_13