

CREATE TABLE customconfig.tbforestobjectsCustomRoles (
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


CREATE TABLE customconfig.z_history_tbforestobjectsCustomRoles (
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
CONSTRAINT PK_308c4c9315c306fc7e31f0157d5bb3b43555ea68 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE customconfig."seq_tbforestobjectsCustomRoles_flId";


CREATE SCHEMA forest;

CREATE TABLE forest.TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flForestUsing character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbObjects PRIMARY KEY (
    flId
));


CREATE TABLE forest.z_history_TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flForestUsing character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_69c1c699d4a69641e27a4241c1a63d3aa09cb9d3 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flForestUsing character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbObjectsRevisions PRIMARY KEY (
    flRevisionId
));


CREATE TABLE forest.z_history_TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flForestUsing character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_e0e8e6130ea18ad4e5b4ad9721d22ecf781bdc00 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbObjectsOrderResult (
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


CREATE TABLE forest.z_history_TbObjectsOrderResult (
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


CREATE TABLE forest.TbObjectsOrderNegotiations (
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


CREATE TABLE forest.z_history_TbObjectsOrderNegotiations (
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


CREATE TABLE forest.TbTradeChanges (
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


CREATE TABLE forest.z_history_TbTradeChanges (
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


CREATE TABLE forest.TbTrades (
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
flCostStart numeric (18, 2) ,
flDeposit numeric (18, 2) ,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flWinnerBin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flWinnerId integer ,
flSaleCondition character varying ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flSaleProtocolDate date ,
flIsSaleProtocolExists boolean ,
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


CREATE TABLE forest.z_history_TbTrades (
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
flCostStart numeric (18, 2) ,
flDeposit numeric (18, 2) ,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flWinnerBin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flWinnerId integer ,
flSaleCondition character varying ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flSaleProtocolDate date ,
flIsSaleProtocolExists boolean ,
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


CREATE TABLE forest.TbTradesRevisions (
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
flCostStart numeric (18, 2) ,
flDeposit numeric (18, 2) ,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flWinnerBin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flWinnerId integer ,
flSaleCondition character varying ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flSaleProtocolDate date ,
flIsSaleProtocolExists boolean ,
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


CREATE TABLE forest.z_history_TbTradesRevisions (
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
flCostStart numeric (18, 2) ,
flDeposit numeric (18, 2) ,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flWinnerBin character varying(12) ,
flWinnerData character varying ,
flCost numeric (18, 2) ,
flWinnerId integer ,
flSaleCondition character varying ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flSaleProtocolDate date ,
flIsSaleProtocolExists boolean ,
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


CREATE TABLE forest.TbTradesOrderResult (
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


CREATE TABLE forest.z_history_TbTradesOrderResult (
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


CREATE TABLE forest.TbTradesOrderNegotiations (
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


CREATE TABLE forest.z_history_TbTradesOrderNegotiations (
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


CREATE SEQUENCE forest."seq_TbObjects_flId";


CREATE SEQUENCE forest."seq_TbObjectsRevisions_flId";


CREATE SEQUENCE forest."seq_TbObjectsOrderResult_flSubjectId";


CREATE SEQUENCE forest."seq_TbObjectsOrderNegotiations_flActionId";


CREATE SEQUENCE forest."seq_TbTradeChanges_flId";


CREATE SEQUENCE forest."seq_TbTrades_flId";


CREATE SEQUENCE forest."seq_TbTradesRevisions_flId";


CREATE SEQUENCE forest."seq_TbTradesOrderResult_flSubjectId";


CREATE SEQUENCE forest."seq_TbTradesOrderNegotiations_flActionId";
