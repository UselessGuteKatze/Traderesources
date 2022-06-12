
ALTER TABLE fishing.tbtrades
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE fishing.tbtradesrevisions
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE fishing.z_history_tbtradesrevisions
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE fishing.z_history_tbtrades
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE hunting.tbtrades
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE hunting.tbtradesrevisions
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE hunting.z_history_tbtrades
ADD COLUMN flAdditionalCondition character varying(10) ;


ALTER TABLE hunting.z_history_tbtradesrevisions
ADD COLUMN flAdditionalCondition character varying(10) ;