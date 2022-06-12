
CREATE TABLE trades.TbTaxOrgs (
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

CONSTRAINT PK_TbTaxOrgs PRIMARY KEY (
    flId
)

);

CREATE TABLE trades.z_history_TbTaxOrgs (
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

CONSTRAINT PK_897dd0cb06575184018d4924b74c71f55651edae PRIMARY KEY (
    flEntityId, flRequestId
)

);

CREATE SEQUENCE trades."seq_TbTaxOrgs_flId";