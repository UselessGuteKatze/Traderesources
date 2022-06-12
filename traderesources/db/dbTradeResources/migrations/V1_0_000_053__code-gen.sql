
CREATE TABLE customconfig.tbhydrocarbonCustomRoles (
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
flTo date );


CREATE TABLE customconfig.z_history_tbhydrocarbonCustomRoles (
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
CONSTRAINT PK_c77db8d10b768908df67734fa50592dbb81ca562 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE customconfig."seq_tbhydrocarbonCustomRoles_flId";


CREATE SCHEMA hydrocarbon;

CREATE TABLE hydrocarbon.TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(25) ,
flRegion character varying ,
flArea numeric (20, 6) ,
flBlocksCount integer ,
flExceptions character varying ,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying , 
CONSTRAINT PK_TbObjects PRIMARY KEY (
    flId
));

CREATE UNIQUE INDEX IX_TbObjects_flId ON hydrocarbon.TbObjects USING btree (flId);
CREATE UNIQUE INDEX IX_TbObjects_flRevisionId ON hydrocarbon.TbObjects USING btree (flRevisionId);

CREATE TABLE hydrocarbon.z_history_TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(25) ,
flRegion character varying ,
flArea numeric (18, 6) ,
flBlocksCount integer ,
flExceptions character varying ,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying , 
CONSTRAINT PK_69c1c699d4a69641e27a4241c1a63d3aa09cb9d3 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(25) ,
flRegion character varying ,
flArea numeric (20, 6) ,
flBlocksCount integer ,
flExceptions character varying ,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying , 
CONSTRAINT PK_TbObjectsRevisions PRIMARY KEY (
    flRevisionId
));

CREATE INDEX IX_TbObjectsRevisions_flId ON hydrocarbon.TbObjectsRevisions USING btree (flId);
CREATE UNIQUE INDEX IX_TbObjectsRevisions_flRevisionId ON hydrocarbon.TbObjectsRevisions USING btree (flRevisionId);

CREATE TABLE hydrocarbon.z_history_TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(25) ,
flRegion character varying ,
flArea numeric (18, 6) ,
flBlocksCount integer ,
flExceptions character varying ,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying , 
CONSTRAINT PK_e0e8e6130ea18ad4e5b4ad9721d22ecf781bdc00 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbObjectsOrderResult (
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
CONSTRAINT PK_TbObjectsOrderResult PRIMARY KEY (
    flSubjectId
));

CREATE UNIQUE INDEX IX_TbObjectsOrderResult ON hydrocarbon.TbObjectsOrderResult USING btree (flSubjectId);

CREATE TABLE hydrocarbon.z_history_TbObjectsOrderResult (
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
CONSTRAINT PK_cecfb14a8749f710138290df08da664bf3a5f473 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbObjectsOrderNegotiations (
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

CREATE INDEX IX_TbObjectsOrderNegotiations ON hydrocarbon.TbObjectsOrderNegotiations USING btree (flSubjectId);

CREATE TABLE hydrocarbon.z_history_TbObjectsOrderNegotiations (
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
CONSTRAINT PK_6c86d20c272967475f3e2738d6744ffb893fc564 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradeChanges (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flTradeId integer ,
flRevisionId integer ,
flAuctionId integer ,
flIsTradeLoaded boolean ,
flDateTime timestamp without time zone ,
flMessage character varying , 
CONSTRAINT PK_TbTradeChanges PRIMARY KEY (
    flId
));


CREATE TABLE hydrocarbon.z_history_TbTradeChanges (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flTradeId integer ,
flRevisionId integer ,
flAuctionId integer ,
flIsTradeLoaded boolean ,
flDateTime timestamp without time zone ,
flMessage character varying , 
CONSTRAINT PK_8f1623c03321dbb0b0cd012340e1edf6c8214a04 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTrades (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flStatus character varying(15) NOT NULL,
flType character varying(7) ,
flCompetentOrgBin character varying(12) ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationType character varying(18) ,
flExploringPeriod character varying ,
flAdditionalConditions character varying(26) ,
flParticipantConditions character varying ,
flRequiredDocuments character varying ,
flCommissionMembers character varying ,
flNote character varying ,
flWinnerXin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flTaxAuthorityBin character varying(12) ,
flBik character varying(8) ,
flIik character varying(32) ,
flKnp integer ,
flKbk character varying(6) ,
flKbe integer , 
CONSTRAINT PK_TbTrades PRIMARY KEY (
    flId
));


CREATE TABLE hydrocarbon.z_history_TbTrades (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flStatus character varying(15) NOT NULL,
flType character varying(7) ,
flCompetentOrgBin character varying(12) ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationType character varying(18) ,
flExploringPeriod character varying ,
flAdditionalConditions character varying(26) ,
flParticipantConditions character varying ,
flRequiredDocuments character varying ,
flCommissionMembers character varying ,
flNote character varying ,
flWinnerXin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flTaxAuthorityBin character varying(12) ,
flBik character varying(8) ,
flIik character varying(32) ,
flKnp integer ,
flKbk character varying(6) ,
flKbe integer , 
CONSTRAINT PK_2bf44c20805121f5b8fe4d5e9fb25321ebc98edd PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flStatus character varying(15) NOT NULL,
flType character varying(7) ,
flCompetentOrgBin character varying(12) ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationType character varying(18) ,
flExploringPeriod character varying ,
flAdditionalConditions character varying(26) ,
flParticipantConditions character varying ,
flRequiredDocuments character varying ,
flCommissionMembers character varying ,
flNote character varying ,
flWinnerXin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flTaxAuthorityBin character varying(12) ,
flBik character varying(8) ,
flIik character varying(32) ,
flKnp integer ,
flKbk character varying(6) ,
flKbe integer , 
CONSTRAINT PK_TbTradesRevisions PRIMARY KEY (
    flRevisionId
));


CREATE TABLE hydrocarbon.z_history_TbTradesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flStatus character varying(15) NOT NULL,
flType character varying(7) ,
flCompetentOrgBin character varying(12) ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationType character varying(18) ,
flExploringPeriod character varying ,
flAdditionalConditions character varying(26) ,
flParticipantConditions character varying ,
flRequiredDocuments character varying ,
flCommissionMembers character varying ,
flNote character varying ,
flWinnerXin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flTaxAuthorityBin character varying(12) ,
flBik character varying(8) ,
flIik character varying(32) ,
flKnp integer ,
flKbk character varying(6) ,
flKbe integer , 
CONSTRAINT PK_9fad9f0ae53373d857605cd226dea7f7bbfdfee9 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradesOrderResult (
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
flType character varying(8) , 
CONSTRAINT PK_TbTradesOrderResult PRIMARY KEY (
    flSubjectId
));


CREATE TABLE hydrocarbon.z_history_TbTradesOrderResult (
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
flType character varying(8) , 
CONSTRAINT PK_a47a8dc2aebb25c2f84fd81cbda950afaa7dfe5e PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradesOrderNegotiations (
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


CREATE TABLE hydrocarbon.z_history_TbTradesOrderNegotiations (
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
CONSTRAINT PK_856f5b10366343f0f9e7ddaeed80cdc491c84728 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradePreApplication (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAppId integer NOT NULL,
flAppGuidId character varying(32) NOT NULL,
flAppNumber character varying(20) NOT NULL,
flRegByUserId integer ,
flRegDate timestamp without time zone ,
flAcceptDate timestamp without time zone ,
flRejectDate timestamp without time zone ,
flRejectReason character varying ,
flApplicantType character varying(12) ,
flApplicantXin character varying ,
flApplicantName character varying ,
flApplicantAddress character varying ,
flApplicantPhoneNumber character varying ,
flSubsoilsObjectId integer NOT NULL,
flSubsoilsName character varying ,
flSubsoilsCoordinates character varying ,
flControlingCountry character varying(50) ,
flControlingPerson character varying ,
flCitizenship character varying(50) ,
flStatus character varying(10) ,
flAppContent character varying ,
flAppModel character varying ,
flDocuments character varying , 
CONSTRAINT PK_TbTradePreApplication PRIMARY KEY (
    flAppId
));

CREATE UNIQUE INDEX IX_AppTbTradePreApplication_AppGuidId ON hydrocarbon.TbTradePreApplication USING btree (flAppGuidId);

CREATE TABLE hydrocarbon.z_history_TbTradePreApplication (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAppId integer NOT NULL,
flAppGuidId character varying(32) NOT NULL,
flAppNumber character varying(20) NOT NULL,
flRegByUserId integer ,
flRegDate timestamp without time zone ,
flAcceptDate timestamp without time zone ,
flRejectDate timestamp without time zone ,
flRejectReason character varying ,
flApplicantType character varying(12) ,
flApplicantXin character varying ,
flApplicantName character varying ,
flApplicantAddress character varying ,
flApplicantPhoneNumber character varying ,
flSubsoilsObjectId integer NOT NULL,
flSubsoilsName character varying ,
flSubsoilsCoordinates character varying ,
flControlingCountry character varying(50) ,
flControlingPerson character varying ,
flCitizenship character varying(50) ,
flStatus character varying(10) ,
flAppContent character varying ,
flAppModel character varying ,
flDocuments character varying , 
CONSTRAINT PK_eeb32754cc03768d82da39babd2bd91354563a66 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradePreApplicationApproovals (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flAppId integer ,
flSessionId integer ,
flDateTime timestamp without time zone ,
flIsSessionClosed boolean ,
flStatus character varying(8) ,
flStep character varying(6) ,
flUserId integer ,
flUserIin character varying(12) ,
flNote character varying , 
CONSTRAINT PK_TbTradePreApplicationApproovals PRIMARY KEY (
    flId
));


CREATE TABLE hydrocarbon.z_history_TbTradePreApplicationApproovals (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flAppId integer ,
flSessionId integer ,
flDateTime timestamp without time zone ,
flIsSessionClosed boolean ,
flStatus character varying(8) ,
flStep character varying(6) ,
flUserId integer ,
flUserIin character varying(12) ,
flNote character varying , 
CONSTRAINT PK_a1c89b3b714b985586947f7f835e4e896b07d16a PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE hydrocarbon.TbTradePreApplicationSigns (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAppId integer NOT NULL,
flDataToSign character varying NOT NULL,
flCertInfo character varying NOT NULL,
flSignedData character varying NOT NULL,
flSignDate timestamp without time zone NOT NULL,
flUserId integer NOT NULL,
flSignerType character varying(9) NOT NULL, 
CONSTRAINT PK_TbTradePreApplicationSigns PRIMARY KEY (
    flAppId, flSignerType
));


CREATE TABLE hydrocarbon.z_history_TbTradePreApplicationSigns (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAppId integer NOT NULL,
flDataToSign character varying NOT NULL,
flCertInfo character varying NOT NULL,
flSignedData character varying NOT NULL,
flSignDate timestamp without time zone NOT NULL,
flUserId integer NOT NULL,
flSignerType character varying(9) NOT NULL, 
CONSTRAINT PK_cf72828c56b9eddca95edc7047f6cdaad621b5d1 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE hydrocarbon."seq_TbObjects_flId";


CREATE SEQUENCE hydrocarbon."seq_TbObjectsRevisions_flId";


CREATE SEQUENCE hydrocarbon."seq_TbObjectsOrderResult_flSubjectId";


CREATE SEQUENCE hydrocarbon."seq_TbObjectsOrderNegotiations_flActionId";


CREATE SEQUENCE hydrocarbon."seq_TbTradeChanges_flId";


CREATE SEQUENCE hydrocarbon."seq_TbTrades_flId";


CREATE SEQUENCE hydrocarbon."seq_TbTradesRevisions_flId";


CREATE SEQUENCE hydrocarbon."seq_TbTradesOrderResult_flSubjectId";


CREATE SEQUENCE hydrocarbon."seq_TbTradesOrderNegotiations_flActionId";


CREATE SEQUENCE hydrocarbon."seq_TbTradePreApplication_flAppId";


CREATE SEQUENCE hydrocarbon."seq_TbTradePreApplicationApproovals_flId";
