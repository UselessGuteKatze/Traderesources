
ALTER TABLE hunting.tbtrades
ADD COLUMN flWinnerData character varying ;


ALTER TABLE hunting.tbtradesrevisions
ADD COLUMN flWinnerData character varying ;


ALTER TABLE hunting.z_history_tbtradesrevisions
ADD COLUMN flWinnerData character varying ;


ALTER TABLE hunting.z_history_tbtrades
ADD COLUMN flWinnerData character varying ;

