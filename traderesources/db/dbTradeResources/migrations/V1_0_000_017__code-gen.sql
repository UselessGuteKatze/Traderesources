
ALTER TABLE trades.tbtrades
ADD COLUMN flNote character varying ;


ALTER TABLE trades.tbtradesrevisions
ADD COLUMN flNote character varying ;


ALTER TABLE trades.z_history_tbtrades
ADD COLUMN flNote character varying ;


ALTER TABLE trades.z_history_tbtradesrevisions
ADD COLUMN flNote character varying ;
