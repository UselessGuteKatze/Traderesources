
ALTER TABLE forest.tbtrades
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE forest.tbtrades
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE forest.tbtrades
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE forest.tbtradesrevisions
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE forest.tbtradesrevisions
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE forest.tbtradesrevisions
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE forest.z_history_tbtrades
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE forest.z_history_tbtrades
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE forest.z_history_tbtrades
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE forest.z_history_tbtradesrevisions
ADD COLUMN flParticipantConditions character varying ;


ALTER TABLE forest.z_history_tbtradesrevisions
ADD COLUMN flRequiredDocuments character varying ;


ALTER TABLE forest.z_history_tbtradesrevisions
ADD COLUMN flCommissionMembers character varying ;


ALTER TABLE trades.tboilsectororganizations 
RENAME TO __rm__tboilsectororganizations;


ALTER TABLE trades.tbsubsoilsobjects 
RENAME TO __rm__tbsubsoilsobjects;


ALTER TABLE trades.tbsubsoilspolygons 
RENAME TO __rm__tbsubsoilspolygons;


ALTER TABLE trades.tbsubsoilspolygonscoords 
RENAME TO __rm__tbsubsoilspolygonscoords;


ALTER TABLE trades.tbtradepreapplication 
RENAME TO __rm__tbtradepreapplication;


ALTER TABLE trades.tbtradepreapplicationapproovals 
RENAME TO __rm__tbtradepreapplicationapproovals;


ALTER TABLE trades.tbtradepreapplicationsigns 
RENAME TO __rm__tbtradepreapplicationsigns;


ALTER TABLE trades.tbtradepreapplicationssenttoesedo 
RENAME TO __rm__tbtradepreapplicationssenttoesedo;


ALTER TABLE trades.tbtradesordernegotiations 
RENAME TO __rm__tbtradesordernegotiations;


ALTER TABLE trades.tbtradesorderresult 
RENAME TO __rm__tbtradesorderresult;


ALTER TABLE trades.z_history_tbtradepreapplicationssenttoesedo 
RENAME TO __rm__z_history_tbtradepreapplicationssenttoesedo;


ALTER TABLE trades.z_history_tbsubsoilspolygonscoords 
RENAME TO __rm__z_history_tbsubsoilspolygonscoords;


ALTER TABLE trades.z_history_tbsubsoilspolygons 
RENAME TO __rm__z_history_tbsubsoilspolygons;


ALTER TABLE trades.z_history_tboilsectororganizations 
RENAME TO __rm__z_history_tboilsectororganizations;


ALTER TABLE trades.z_history_tbtradepreapplicationapproovals 
RENAME TO __rm__z_history_tbtradepreapplicationapproovals;


ALTER TABLE trades.z_history_tbtradepreapplicationsigns 
RENAME TO __rm__z_history_tbtradepreapplicationsigns;


ALTER TABLE trades.z_history_tbtradepreapplication 
RENAME TO __rm__z_history_tbtradepreapplication;


ALTER TABLE trades.z_history_tbsubsoilsobjects 
RENAME TO __rm__z_history_tbsubsoilsobjects;


ALTER TABLE trades.z_history_tbtradesordernegotiations 
RENAME TO __rm__z_history_tbtradesordernegotiations;


ALTER TABLE trades.z_history_tbtradesorderresult 
RENAME TO __rm__z_history_tbtradesorderresult;


ALTER SEQUENCE trades."seq_TbTradesOrderResult_flSubjectId" 
RENAME TO "__rm__seq_TbTradesOrderResult_flSubjectId";


ALTER SEQUENCE trades."seq_TbTradesOrderNegotiations_flActionId" 
RENAME TO "__rm__seq_TbTradesOrderNegotiations_flActionId";


ALTER SEQUENCE trades."seq_TbTradePreApplication_flAppId" 
RENAME TO "__rm__seq_TbTradePreApplication_flAppId";


ALTER SEQUENCE trades."seq_TbTradePreApplicationApproovals_flId" 
RENAME TO "__rm__seq_TbTradePreApplicationApproovals_flId";


ALTER SEQUENCE trades."seq_TbOilSectorOrganizations_flId" 
RENAME TO "__rm__seq_TbOilSectorOrganizations_flId";


ALTER SEQUENCE trades."seq_tbsubsoilsobjects_flId" 
RENAME TO "__rm__seq_tbsubsoilsobjects_flId";


ALTER SEQUENCE trades."seq_tbsubsoilspolygons_flId" 
RENAME TO "__rm__seq_tbsubsoilspolygons_flId";


ALTER SEQUENCE trades."seq_tbsubsoilspolygonscoords_flId" 
RENAME TO "__rm__seq_tbsubsoilspolygonscoords_flId";