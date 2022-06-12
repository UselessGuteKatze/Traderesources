

ALTER TABLE agreements.tbpaymentitems 
RENAME TO __rm__tbpaymentitems;


ALTER TABLE agreements.z_history_tbpaymentitems 
RENAME TO __rm__z_history_tbpaymentitems;


CREATE TABLE agreements.TbPaymentMatches (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flPaymentId integer NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flPaymentItems character varying NOT NULL,
flStatus character varying(13) NOT NULL,
flMatchResult character varying NOT NULL,
flAmount numeric (18, 2) NOT NULL,
flMatchBlockResult character varying ,
flOrder character varying ,
flOverpayment boolean NOT NULL,
flOverpaymentAmount numeric (18, 2) ,
flOverpaymentMatchBlockResult character varying ,
flOverpaymentOrder character varying ,
flBik character varying(8) ,
flIban character varying(32) ,
flKbe integer ,
flKnp integer ,
flKbk character varying(6) , 

CONSTRAINT PK_TbPaymentMatches PRIMARY KEY (
    flId, flPaymentId
)

);

CREATE INDEX IDX_TbPaymentMatches_flPaymentId ON agreements.TbPaymentMatches USING btree (flPaymentId);
CREATE INDEX IDX_TbPaymentMatches_flStatus ON agreements.TbPaymentMatches USING btree (flStatus);


CREATE TABLE agreements.z_history_TbPaymentMatches (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flPaymentId integer NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flPaymentItems character varying NOT NULL,
flStatus character varying(13) NOT NULL,
flMatchResult character varying NOT NULL,
flAmount numeric (18, 2) NOT NULL,
flMatchBlockResult character varying ,
flOrder character varying ,
flOverpayment boolean NOT NULL,
flOverpaymentAmount numeric (18, 2) ,
flOverpaymentMatchBlockResult character varying ,
flOverpaymentOrder character varying ,
flBik character varying(8) ,
flIban character varying(32) ,
flKbe integer ,
flKnp integer ,
flKbk character varying(6) , 

CONSTRAINT PK_d103a389c6a6d803f9c05e75e94259f58cff85de PRIMARY KEY (
    flEntityId, flRequestId
)

);




ALTER SEQUENCE agreements."seq_TbPaymentItems_flId" 
RENAME TO "__rm__seq_TbPaymentItems_flId";


CREATE SEQUENCE agreements."seq_TbPaymentMatches_flId";
