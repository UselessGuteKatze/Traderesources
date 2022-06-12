
ALTER TABLE hydrocarbon.tbtrades
ALTER COLUMN flstatus TYPE character varying(26);


ALTER TABLE hydrocarbon.tbtradesrevisions
ALTER COLUMN flstatus TYPE character varying(26);


ALTER TABLE hydrocarbon.z_history_tbtrades
ALTER COLUMN flstatus TYPE character varying(26);


ALTER TABLE hydrocarbon.z_history_tbtradesrevisions
ALTER COLUMN flstatus TYPE character varying(26);

