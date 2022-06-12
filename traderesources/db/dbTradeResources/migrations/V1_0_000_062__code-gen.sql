
ALTER TABLE trades.tblandobjects
ADD COLUMN flEgknId character varying ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flEgknId character varying ;


ALTER TABLE trades.tbobjects 
RENAME TO __rm__tbobjects;


ALTER TABLE trades.tbobjectsordernegotiations 
RENAME TO __rm__tbobjectsordernegotiations;


ALTER TABLE trades.tbobjectsorderresult 
RENAME TO __rm__tbobjectsorderresult;


ALTER TABLE trades.tbobjectsrevisions 
RENAME TO __rm__tbobjectsrevisions;


ALTER TABLE trades.tbtradechanges 
RENAME TO __rm__tbtradechanges;


ALTER TABLE trades.tbtradeparticipantconditions 
RENAME TO __rm__tbtradeparticipantconditions;


ALTER TABLE trades.tbtrades 
RENAME TO __rm__tbtrades;


ALTER TABLE trades.tbtradesrequireddocuments 
RENAME TO __rm__tbtradesrequireddocuments;


ALTER TABLE trades.tbtradesrevisions 
RENAME TO __rm__tbtradesrevisions;


ALTER TABLE trades.z_history_tbtradesrevisions 
RENAME TO __rm__z_history_tbtradesrevisions;


ALTER TABLE trades.z_history_tbtradechanges 
RENAME TO __rm__z_history_tbtradechanges;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flEgknId character varying ;


ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flEgknId character varying ;


ALTER TABLE trades.z_history_tbtradesrequireddocuments 
RENAME TO __rm__z_history_tbtradesrequireddocuments;


ALTER TABLE trades.z_history_tbtrades 
RENAME TO __rm__z_history_tbtrades;


ALTER TABLE trades.z_history_tbobjectsordernegotiations 
RENAME TO __rm__z_history_tbobjectsordernegotiations;


ALTER TABLE trades.z_history_tbobjects 
RENAME TO __rm__z_history_tbobjects;


ALTER TABLE trades.z_history_tbtradeparticipantconditions 
RENAME TO __rm__z_history_tbtradeparticipantconditions;


ALTER TABLE trades.z_history_tbobjectsrevisions 
RENAME TO __rm__z_history_tbobjectsrevisions;


ALTER TABLE trades.z_history_tbobjectsorderresult 
RENAME TO __rm__z_history_tbobjectsorderresult;


ALTER SEQUENCE trades."seq_TbObjects_flId" 
RENAME TO "__rm__seq_TbObjects_flId";


ALTER SEQUENCE trades."seq_TbObjectsRevisions_flId" 
RENAME TO "__rm__seq_TbObjectsRevisions_flId";


ALTER SEQUENCE trades."seq_TbObjectsOrderResult_flSubjectId" 
RENAME TO "__rm__seq_TbObjectsOrderResult_flSubjectId";


ALTER SEQUENCE trades."seq_TbObjectsOrderNegotiations_flActionId" 
RENAME TO "__rm__seq_TbObjectsOrderNegotiations_flActionId";


ALTER SEQUENCE trades."seq_TbTrades_flId" 
RENAME TO "__rm__seq_TbTrades_flId";


ALTER SEQUENCE trades."seq_TbTradesRevisions_flId" 
RENAME TO "__rm__seq_TbTradesRevisions_flId";


ALTER SEQUENCE trades."seq_TbTradeChanges_flId" 
RENAME TO "__rm__seq_TbTradeChanges_flId";
