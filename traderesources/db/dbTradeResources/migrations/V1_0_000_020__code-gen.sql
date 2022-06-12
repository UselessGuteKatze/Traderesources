
ALTER TABLE hunting.tbtrades
ADD COLUMN flCostStart numeric (18, 2) ;


ALTER TABLE hunting.tbtradesrevisions
ADD COLUMN flCostStart numeric (18, 2) ;


ALTER TABLE hunting.z_history_tbtrades
ADD COLUMN flCostStart numeric (18, 2) ;


ALTER TABLE hunting.z_history_tbtradesrevisions
ADD COLUMN flCostStart numeric (18, 2) ;
