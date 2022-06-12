
CREATE TABLE agreements.TbPayments (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flPaymentId integer NOT NULL,
flAgreementId integer NOT NULL,
flPaymentStatus character varying(6) NOT NULL,
flPayAmount numeric (18, 2) NOT NULL,
flPaidAmount numeric (18, 2) NOT NULL, 

CONSTRAINT PK_TbPayments PRIMARY KEY (
    flPaymentId, flAgreementId
)

);

CREATE UNIQUE INDEX IDX_TbPayments_flPaymentId ON agreements.TbPayments USING btree (flPaymentId);
CREATE UNIQUE INDEX IDX_TbPayments_flAgreementId ON agreements.TbPayments USING btree (flAgreementId);


CREATE TABLE agreements.z_history_TbPayments (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flPaymentId integer NOT NULL,
flAgreementId integer NOT NULL,
flPaymentStatus character varying(6) NOT NULL,
flPayAmount numeric (18, 2) NOT NULL,
flPaidAmount numeric (18, 2) NOT NULL, 

CONSTRAINT PK_8bc8debd4e880407acd919d5af7ff1e100b461e1 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE agreements.TbPaymentItems (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flPaymentId integer NOT NULL,
flPaymentItemId character varying NOT NULL,
flPaymentItemStatus character varying(13) NOT NULL,
flAmount numeric (18, 2) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flPurpose character varying NOT NULL, 

CONSTRAINT PK_TbPaymentItems PRIMARY KEY (
    flId, flPaymentId
)

);

CREATE INDEX IDX_TbPaymentItems_flPaymentId ON agreements.TbPaymentItems USING btree (flPaymentId);
CREATE INDEX IDX_TbPaymentItems_flPaymentItemStatus ON agreements.TbPaymentItems USING btree (flPaymentItemStatus);


CREATE TABLE agreements.z_history_TbPaymentItems (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flPaymentId integer NOT NULL,
flPaymentItemId character varying NOT NULL,
flPaymentItemStatus character varying(13) NOT NULL,
flAmount numeric (18, 2) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flPurpose character varying NOT NULL, 

CONSTRAINT PK_141d62c958d6861b670251acb189e28ef5b2dcdc PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE agreements."seq_TbPayments_flPaymentId";


CREATE SEQUENCE agreements."seq_TbPaymentItems_flId";
