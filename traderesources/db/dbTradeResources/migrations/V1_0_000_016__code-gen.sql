
CREATE TABLE customconfig.tblandobjectsCustomRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flNamespace character varying NOT NULL,
flBin character varying NOT NULL,
flOrgName character varying ,
flIin character varying NOT NULL,
flFio character varying ,
flPosition character varying ,
flRole character varying NOT NULL,
flFrom date ,
flTo date 
);




CREATE TABLE customconfig.z_history_tblandobjectsCustomRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flNamespace character varying NOT NULL,
flBin character varying NOT NULL,
flOrgName character varying ,
flIin character varying NOT NULL,
flFio character varying ,
flPosition character varying ,
flRole character varying NOT NULL,
flFrom date ,
flTo date , 

CONSTRAINT PK_bfc61167f1b5c694fc2634dd16da125e1319511e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE customconfig."seq_tblandobjectsCustomRoles_flId";