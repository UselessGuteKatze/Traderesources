
ALTER TABLE trades.tblandobjects
ADD COLUMN flPurpose character varying(300) ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flPurpose character varying(300) ;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flPurpose character varying(300) ;


ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flPurpose character varying(300) ;

ALTER TABLE agreements.tbagreements
ADD COLUMN flAuctionId integer ;


ALTER TABLE agreements.tbpaymentmatches
ADD COLUMN flAgreementId integer NOT NULL DEFAULT 0;


ALTER TABLE agreements.z_history_tbpaymentmatches
ADD COLUMN flAgreementId integer NOT NULL DEFAULT 0;


ALTER TABLE agreements.z_history_tbagreements
ADD COLUMN flAuctionId integer ;


CREATE TABLE agreements.TbPaymentMatchesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flPaymentId integer NOT NULL,
flAgreementId integer NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flPaymentItems character varying NOT NULL,
flStatus character varying(13) NOT NULL,
flMatchResult character varying ,
flAmount numeric (18, 2) NOT NULL,
flGuaranteeAmount numeric (18, 2) NOT NULL,
flRealAmount numeric (18, 2) NOT NULL,
flHasSendAmount boolean NOT NULL,
flSendAmount numeric (18, 2) NOT NULL,
flMatchBlockResult character varying ,
flRequisites character varying ,
flOverpayment boolean NOT NULL,
flOverpaymentAmount numeric (18, 2) ,
flSendOverpayment boolean NOT NULL,
flOverpaymentSendAmount numeric (18, 2) ,
flOverpaymentMatchBlockResult character varying ,
flOverpaymentRequisites character varying , 
CONSTRAINT PK_TbPaymentMatchesRevisions PRIMARY KEY (
    flId
));

CREATE INDEX IDX_TbPaymentMatchesRevisions_flPaymentId ON agreements.TbPaymentMatchesRevisions USING btree (flPaymentId);
CREATE INDEX IDX_TbPaymentMatchesRevisions_flStatus ON agreements.TbPaymentMatchesRevisions USING btree (flStatus);

CREATE TABLE agreements.z_history_TbPaymentMatchesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flPaymentId integer NOT NULL,
flAgreementId integer NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flPaymentItems character varying NOT NULL,
flStatus character varying(13) NOT NULL,
flMatchResult character varying ,
flAmount numeric (18, 2) NOT NULL,
flGuaranteeAmount numeric (18, 2) NOT NULL,
flRealAmount numeric (18, 2) NOT NULL,
flHasSendAmount boolean NOT NULL,
flSendAmount numeric (18, 2) NOT NULL,
flMatchBlockResult character varying ,
flRequisites character varying ,
flOverpayment boolean NOT NULL,
flOverpaymentAmount numeric (18, 2) ,
flSendOverpayment boolean NOT NULL,
flOverpaymentSendAmount numeric (18, 2) ,
flOverpaymentMatchBlockResult character varying ,
flOverpaymentRequisites character varying , 
CONSTRAINT PK_a171debdac0c3912d21de321cabf51c37bd20f54 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE agreements.TbPaymentMatchesOrderResult (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flSubjectId integer NOT NULL,
flStatus character varying(10) ,
flRegUserId integer ,
flRegDate timestamp without time zone ,
flExecUserId integer ,
flExecDate timestamp without time zone ,
flOrderNote character varying ,
flNoteNegotiations character varying ,
flType character varying(6) , 
CONSTRAINT PK_TbPaymentMatchesOrderResult PRIMARY KEY (
    flSubjectId
));


CREATE TABLE agreements.z_history_TbPaymentMatchesOrderResult (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flSubjectId integer NOT NULL,
flStatus character varying(10) ,
flRegUserId integer ,
flRegDate timestamp without time zone ,
flExecUserId integer ,
flExecDate timestamp without time zone ,
flOrderNote character varying ,
flNoteNegotiations character varying ,
flType character varying(6) , 
CONSTRAINT PK_6723e1cb7bdbbe929ce1278ae7553d5a20af2db0 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE agreements.TbPaymentMatchesOrderNegotiations (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flActionId integer ,
flSubjectId integer ,
flUserId integer ,
flRole character varying ,
flUserXin character varying ,
flUserXinWithPrefix character varying ,
flActionType character varying(10) ,
flNote character varying ,
flActionDate timestamp without time zone ,
flIsActive boolean );


CREATE TABLE agreements.z_history_TbPaymentMatchesOrderNegotiations (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flActionId integer ,
flSubjectId integer ,
flUserId integer ,
flRole character varying ,
flUserXin character varying ,
flUserXinWithPrefix character varying ,
flActionType character varying(10) ,
flNote character varying ,
flActionDate timestamp without time zone ,
flIsActive boolean , 
CONSTRAINT PK_7f4622d00bdb04841a0d2de741c60e149ac7a427 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE agreements."seq_TbPaymentMatchesRevisions_flId";


CREATE SEQUENCE agreements."seq_TbPaymentMatchesOrderResult_flSubjectId";


CREATE SEQUENCE agreements."seq_TbPaymentMatchesOrderNegotiations_flActionId";
