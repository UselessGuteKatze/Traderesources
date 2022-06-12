
ALTER TABLE trades.tblandobjectstrades
ALTER COLUMN flstatus TYPE character varying(15);


ALTER TABLE trades.tblandobjectstradesrevisions
ALTER COLUMN flstatus TYPE character varying(15);


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ALTER COLUMN flstatus TYPE character varying(15);


ALTER TABLE trades.z_history_tblandobjectstrades
ALTER COLUMN flstatus TYPE character varying(15);
