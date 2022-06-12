
ALTER TABLE trades.tbsubsoilsobjects 
DROP COLUMN flgeometry;


ALTER TABLE trades.tbsubsoilsobjects 
DROP COLUMN flpoint;


ALTER TABLE trades.tbsubsoilspolygonscoords 
DROP COLUMN flpoint;


ALTER TABLE trades.z_history_tbsubsoilspolygonscoords 
DROP COLUMN flpoint;


ALTER TABLE trades.z_history_tbsubsoilsobjects 
DROP COLUMN flgeometry;


ALTER TABLE trades.z_history_tbsubsoilsobjects 
DROP COLUMN flpoint;


CREATE SEQUENCE trades."seq_tbsubsoilsobjects_flId";


CREATE SEQUENCE trades."seq_tbsubsoilspolygons_flId";


CREATE SEQUENCE trades."seq_tbsubsoilspolygonscoords_flId";