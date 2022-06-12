
CREATE SCHEMA agreements;

CREATE TABLE agreements.TbAgreements (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flAgreementNumber character varying(20) NOT NULL,
flAgreementRevisionId integer NOT NULL,
flAgreementType character varying NOT NULL,
flAgreementStatus character varying(12) NOT NULL,
flAgreementCreateDate timestamp without time zone NOT NULL,
flAgreementSignDate timestamp without time zone ,
flObjectId integer NOT NULL,
flObjectType character varying NOT NULL,
flTradeId integer ,
flTradeType character varying ,
flSellerBin character varying(12) ,
flWinnerXin character varying(12) , 

CONSTRAINT PK_TbAgreements PRIMARY KEY (
    flAgreementId, flAgreementRevisionId
)

);

CREATE UNIQUE INDEX IDX_TbAgreements_flAgreementId ON agreements.TbAgreements USING btree (flAgreementId, flAgreementNumber);


CREATE TABLE agreements.z_history_TbAgreements (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flAgreementNumber character varying(20) NOT NULL,
flAgreementRevisionId integer NOT NULL,
flAgreementType character varying NOT NULL,
flAgreementStatus character varying(12) NOT NULL,
flAgreementCreateDate timestamp without time zone NOT NULL,
flAgreementSignDate timestamp without time zone ,
flObjectId integer NOT NULL,
flObjectType character varying NOT NULL,
flTradeId integer ,
flTradeType character varying ,
flSellerBin character varying(12) ,
flWinnerXin character varying(12) , 

CONSTRAINT PK_63ba28053fc8da99e2df3b10596aadb2dc8745f9 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE agreements.TbAgreementModels (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flAgreementRevisionId integer NOT NULL,
flAgreementStatus character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flComment character varying ,
flCommentDateTime timestamp without time zone ,
flModels character varying NOT NULL,
flContent character varying NOT NULL
);

CREATE UNIQUE INDEX IDX_TbAgreementModels_flAgreementId_flAgreementRevisionId ON agreements.TbAgreementModels USING btree (flAgreementId, flAgreementRevisionId);


CREATE TABLE agreements.z_history_TbAgreementModels (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flAgreementRevisionId integer NOT NULL,
flAgreementStatus character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flComment character varying ,
flCommentDateTime timestamp without time zone ,
flModels character varying NOT NULL,
flContent character varying NOT NULL, 

CONSTRAINT PK_0e8870fca89009215d43109a8d05f52a3ab76eed PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE agreements.TbAgreementPdfs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flPdf bytea NOT NULL,
flPdfWithSigns bytea , 

CONSTRAINT PK_TbAgreementPdfs PRIMARY KEY (
    flAgreementId
)

);

CREATE UNIQUE INDEX IDX_TbAgreementPdfs_flAgreementId ON agreements.TbAgreementPdfs USING btree (flAgreementId);


CREATE TABLE agreements.z_history_TbAgreementPdfs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flPdf bytea NOT NULL,
flPdfWithSigns bytea , 

CONSTRAINT PK_d5496824afbfbfce8de32d1f7af7252cd2fec8ee PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE agreements.TbAgreementSigns (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flDataToSign character varying NOT NULL,
flCertInfo character varying NOT NULL,
flSignedData character varying NOT NULL,
flSignDate timestamp without time zone NOT NULL,
flUserId integer NOT NULL,
flSignerRole character varying(6) NOT NULL, 

CONSTRAINT PK_TbAgreementSigns PRIMARY KEY (
    flAgreementId, flSignerRole
)

);

CREATE UNIQUE INDEX IDX_TbAgreementSigns_flAgreementId ON agreements.TbAgreementSigns USING btree (flAgreementId, flSignerRole);


CREATE TABLE agreements.z_history_TbAgreementSigns (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAgreementId integer NOT NULL,
flDataToSign character varying NOT NULL,
flCertInfo character varying NOT NULL,
flSignedData character varying NOT NULL,
flSignDate timestamp without time zone NOT NULL,
flUserId integer NOT NULL,
flSignerRole character varying(6) NOT NULL, 

CONSTRAINT PK_867fceadf1c68308f2c5fe903fd6966d751bfe01 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE agreements."seq_TbAgreements_flAgreementId";
