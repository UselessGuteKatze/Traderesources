
CREATE TABLE files.TbGuidIds (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flGuid character varying NOT NULL,
flContent character varying NOT NULL,
flContentHash character varying NOT NULL,
flContentHashType character varying NOT NULL,
flParts character varying NOT NULL, 

CONSTRAINT PK_TbGuidIds PRIMARY KEY (
    flId
)

);

CREATE TABLE files.z_history_TbGuidIds (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flGuid character varying NOT NULL,
flContent character varying NOT NULL,
flContentHash character varying NOT NULL,
flContentHashType character varying NOT NULL,
flParts character varying NOT NULL, 

CONSTRAINT PK_e8f6a060c9cf3d06e9d4ad7fdd8e5967a9463805 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE files."seq_TbGuidIds_flId";
