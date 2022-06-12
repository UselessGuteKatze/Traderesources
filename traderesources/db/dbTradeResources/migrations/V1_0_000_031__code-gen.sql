
ALTER TABLE fishing.tbobjects
ADD COLUMN flPasportCoords character varying ;


ALTER TABLE fishing.tbobjectsrevisions
ADD COLUMN flPasportCoords character varying ;


ALTER TABLE fishing.z_history_tbobjectsrevisions
ADD COLUMN flPasportCoords character varying ;


ALTER TABLE fishing.z_history_tbobjects
ADD COLUMN flPasportCoords character varying ;