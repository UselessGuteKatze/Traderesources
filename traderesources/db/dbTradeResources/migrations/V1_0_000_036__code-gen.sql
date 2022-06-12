CREATE TABLE hunting.TbSellerSigners (
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

CREATE TABLE hunting.z_history_TbSellerSigners (
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


CREATE SEQUENCE hunting."seq_TbSellerSigners_flId";


ALTER TABLE agreements.tbagreements
ADD COLUMN flAgreementCreatorBin character varying(12) ;


ALTER TABLE agreements.z_history_tbagreements
ADD COLUMN flAgreementCreatorBin character varying(12) ;
