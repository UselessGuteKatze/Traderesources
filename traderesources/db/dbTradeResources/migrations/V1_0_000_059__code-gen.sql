
ALTER TABLE forest.tbobjects
ADD COLUMN flForestUsingType character varying(31) NOT NULL DEFAULT 'ПобочноеЛесопользование';


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flForestUsingType character varying(31) NOT NULL DEFAULT 'ПобочноеЛесопользование';


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flForestUsingType character varying(31) NOT NULL DEFAULT 'ПобочноеЛесопользование';


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flForestUsingType character varying(31) NOT NULL DEFAULT 'ПобочноеЛесопользование';