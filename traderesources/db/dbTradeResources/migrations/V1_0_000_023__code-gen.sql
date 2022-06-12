
CREATE TABLE customconfig.tbfishingobjectsCustomRoles (
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




CREATE TABLE customconfig.z_history_tbfishingobjectsCustomRoles (
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

CONSTRAINT PK_51ebb3ebf2b383d5a16a807b097427681ee4eea4 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE customconfig."seq_tbfishingobjectsCustomRoles_flId";
