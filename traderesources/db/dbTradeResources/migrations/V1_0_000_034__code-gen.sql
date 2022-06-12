
ALTER TABLE trades.tblandobjectstrades
ADD COLUMN flWinnerData character varying ;


ALTER TABLE trades.tblandobjectstradesrevisions
ADD COLUMN flWinnerData character varying ;


ALTER TABLE trades.z_history_tblandobjectstradesrevisions
ADD COLUMN flWinnerData character varying ;


ALTER TABLE trades.z_history_tblandobjectstrades
ADD COLUMN flWinnerData character varying ;
