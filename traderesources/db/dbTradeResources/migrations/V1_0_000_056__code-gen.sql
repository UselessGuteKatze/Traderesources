
CREATE TABLE trades.TbMsuOrgs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flBin character varying NOT NULL,
flNameRu character varying NOT NULL,
flNameKz character varying NOT NULL,
flStatus character varying(3) NOT NULL, 
CONSTRAINT PK_TbMsuOrgs PRIMARY KEY (
    flId
));


CREATE TABLE trades.z_history_TbMsuOrgs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flBin character varying NOT NULL,
flNameRu character varying NOT NULL,
flNameKz character varying NOT NULL,
flStatus character varying(3) NOT NULL, 
CONSTRAINT PK_0653857958353ad2f8d2d489ae951c3eaddcf84b PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE trades."seq_TbMsuOrgs_flId";
