ALTER TABLE users.tbdataaccesssettingsimport
ALTER COLUMN fldataaccesssettingsid SET NOT NULL;


ALTER TABLE users.tbdataaccesssettingsimport
ALTER COLUMN flsettingsimportid TYPE character varying(128),
ALTER COLUMN flsettingsimportid SET NOT NULL;


ALTER TABLE users.tbroles
ALTER COLUMN flproject TYPE character varying(128);


ALTER TABLE users.z_history_tbroles
ALTER COLUMN flproject TYPE character varying(128);


CREATE TABLE files.tbFileStoreFiles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flFileId character varying ,
flFileName character varying ,
flDescription character varying ,
flFileHash character varying ,
flFileSize bigint ,
flNamespace character varying ,
flContent bytea 
);


