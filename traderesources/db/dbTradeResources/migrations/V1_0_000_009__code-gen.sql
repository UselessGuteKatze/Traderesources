
ALTER TABLE trades.tblandobjects
ALTER COLUMN fllandarea TYPE numeric (22, 4);


ALTER TABLE trades.tblandobjectsrevisions
ALTER COLUMN fllandarea TYPE numeric (22, 4);


ALTER TABLE trades.z_history_tblandobjects
ALTER COLUMN fllandarea TYPE numeric (18, 4);


ALTER TABLE trades.z_history_tblandobjectsrevisions
ALTER COLUMN fllandarea TYPE numeric (18, 4);
