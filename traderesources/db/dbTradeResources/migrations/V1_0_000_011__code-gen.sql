
ALTER TABLE trades.tblandobjects
ADD COLUMN flConsolidatedStatementDateTime timestamp without time zone ;


ALTER TABLE trades.tblandobjects
ADD COLUMN flConsolidatedStatementReconciliationDateTime timestamp without time zone ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flConsolidatedStatementDateTime timestamp without time zone ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flConsolidatedStatementReconciliationDateTime timestamp without time zone ;


ALTER TABLE trades.tblandobjectstrades
ALTER COLUMN flcoststart DROP NOT NULL;


ALTER TABLE trades.tblandobjectstrades
ALTER COLUMN fldeposit DROP NOT NULL;


ALTER TABLE trades.tblandobjectstrades
ALTER COLUMN flinitialcost DROP NOT NULL;


ALTER TABLE trades.tblandobjectstrades
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE trades.tblandobjectstrades
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE trades.tblandobjectstrades
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE trades.tblandobjectstradesrevisions
ALTER COLUMN flinitialcost DROP NOT NULL;


ALTER TABLE trades.tblandobjectstradesrevisions
ALTER COLUMN fldeposit DROP NOT NULL;


ALTER TABLE trades.tblandobjectstradesrevisions
ALTER COLUMN flcoststart DROP NOT NULL;


ALTER TABLE trades.tblandobjectstradesrevisions
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE trades.tblandobjectstradesrevisions
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE trades.tblandobjectstradesrevisions
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flConsolidatedStatementDateTime timestamp without time zone ;


ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flConsolidatedStatementReconciliationDateTime timestamp without time zone ;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flConsolidatedStatementDateTime timestamp without time zone ;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flConsolidatedStatementReconciliationDateTime timestamp without time zone ;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ALTER COLUMN flcoststart DROP NOT NULL;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ALTER COLUMN flinitialcost DROP NOT NULL;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ALTER COLUMN fldeposit DROP NOT NULL;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE trades.z_history_tblandobjectstrades
ALTER COLUMN flcoststart DROP NOT NULL;


ALTER TABLE trades.z_history_tblandobjectstrades
ALTER COLUMN fldeposit DROP NOT NULL;


ALTER TABLE trades.z_history_tblandobjectstrades
ALTER COLUMN flinitialcost DROP NOT NULL;


ALTER TABLE trades.z_history_tblandobjectstrades
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE trades.z_history_tblandobjectstrades
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE trades.z_history_tblandobjectstrades
ADD COLUMN flCommissionMembers character varying ;
