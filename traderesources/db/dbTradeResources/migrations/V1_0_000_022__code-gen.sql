
ALTER TABLE fishing.tbobjects
ADD COLUMN flEmergentVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.tbobjects
ADD COLUMN flUnderwaterVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.tbobjects
ADD COLUMN flPhytoplanktonValue numeric (18, 2) ;


ALTER TABLE fishing.tbobjects
ADD COLUMN flHasWaterIntakes boolean ;


ALTER TABLE fishing.tbobjectsrevisions
ADD COLUMN flEmergentVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.tbobjectsrevisions
ADD COLUMN flUnderwaterVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.tbobjectsrevisions
ADD COLUMN flPhytoplanktonValue numeric (18, 2) ;


ALTER TABLE fishing.tbobjectsrevisions
ADD COLUMN flHasWaterIntakes boolean ;


ALTER TABLE fishing.z_history_tbobjects
ADD COLUMN flEmergentVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.z_history_tbobjects
ADD COLUMN flUnderwaterVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.z_history_tbobjects
ADD COLUMN flPhytoplanktonValue numeric (18, 2) ;


ALTER TABLE fishing.z_history_tbobjects
ADD COLUMN flHasWaterIntakes boolean ;


ALTER TABLE fishing.z_history_tbobjectsrevisions
ADD COLUMN flEmergentVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.z_history_tbobjectsrevisions
ADD COLUMN flUnderwaterVegetationValue numeric (18, 2) ;


ALTER TABLE fishing.z_history_tbobjectsrevisions
ADD COLUMN flPhytoplanktonValue numeric (18, 2) ;


ALTER TABLE fishing.z_history_tbobjectsrevisions
ADD COLUMN flHasWaterIntakes boolean ;
