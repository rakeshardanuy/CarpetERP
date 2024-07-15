sp_helptext PRO_SAVESAMPLEDEVELIPMENT_AGNI

select top 10 * from  SampleDevelopmentMaster
select top 10 * from  SAMPLEDEVELOPMENTITEMSPLITDETAIL
select top 10 * from  SAMPLEDEVRAWMATERIALDESCRIPTION
select top 10 * from PROCESSCONSUMPTIONMASTER
select top 10 * from PROCESSCONSUMPTIONDETAIL

select * from OrderMaster where -- CustomerOrderNo='17718' and 
LocalOrder='L152'