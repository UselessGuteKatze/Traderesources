
ALTER TABLE trades.tbobjects
ALTER COLUMN flarea TYPE numeric (20, 6);


ALTER TABLE trades.tbobjectsrevisions
ALTER COLUMN flarea TYPE numeric (20, 6);


ALTER TABLE trades.z_history_tbobjects
ALTER COLUMN flarea TYPE numeric (18, 6);


ALTER TABLE trades.z_history_tbobjectsrevisions
ALTER COLUMN flarea TYPE numeric (18, 6);
