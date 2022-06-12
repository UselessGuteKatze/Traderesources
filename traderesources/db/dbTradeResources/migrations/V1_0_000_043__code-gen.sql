
CREATE TABLE fishing.TbSellerSigners (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flSellerBin character varying(12) ,
flSignerBin character varying(12) , 

CONSTRAINT PK_TbSellerSigners PRIMARY KEY (
    flSellerBin
)

);


CREATE TABLE fishing.z_history_TbSellerSigners (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flSellerBin character varying(12) ,
flSignerBin character varying(12) , 

CONSTRAINT PK_720d2486a32aedb6c61cac4485146f9d77dd5ef0 PRIMARY KEY (
    flEntityId, flRequestId
)

);


CREATE SEQUENCE fishing."seq_TbSellerSigners_flId";