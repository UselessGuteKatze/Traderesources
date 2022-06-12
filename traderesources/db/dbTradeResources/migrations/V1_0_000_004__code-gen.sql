
CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE yoda_system.TbBanks (
flNodeId integer ,
flRevisionId integer ,
flBik character varying(8) ,
flName character varying(2048) ,
flNote character varying ,
flStatus character varying 
);

CREATE SCHEMA customconfig;

CREATE TABLE customconfig.tbtraderesourcesCustomRoles (
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




CREATE TABLE customconfig.z_history_tbtraderesourcesCustomRoles (
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

CONSTRAINT PK_3b2527694461fc8666573713158b6c3ec82b084a PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE customconfig."seq_tbtraderesourcesCustomRoles_flId";

CREATE SCHEMA trades;

CREATE TABLE trades.TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgId integer ,
flCompetentOrgBin character varying(12) ,
flSubsoilId integer ,
flType character varying(7) ,
flSubType character varying(20) ,
flAreaNumber character varying(100) ,
flName character varying(2000) ,
flDescription character varying ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (20, 2) ,
flBlocksCount integer ,
flExceptions character varying ,
flLandUrl character varying ,
flWkt character varying , 

CONSTRAINT PK_TbObjects PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgId integer ,
flCompetentOrgBin character varying(12) ,
flSubsoilId integer ,
flType character varying(7) ,
flSubType character varying(20) ,
flAreaNumber character varying(100) ,
flName character varying(2000) ,
flDescription character varying ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (18, 2) ,
flBlocksCount integer ,
flExceptions character varying ,
flLandUrl character varying ,
flWkt character varying , 

CONSTRAINT PK_69c1c699d4a69641e27a4241c1a63d3aa09cb9d3 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgId integer ,
flCompetentOrgBin character varying(12) ,
flSubsoilId integer ,
flType character varying(7) ,
flSubType character varying(20) ,
flAreaNumber character varying(100) ,
flName character varying(2000) ,
flDescription character varying ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (20, 2) ,
flBlocksCount integer ,
flExceptions character varying ,
flLandUrl character varying ,
flWkt character varying , 

CONSTRAINT PK_TbObjectsRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE trades.z_history_TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgId integer ,
flCompetentOrgBin character varying(12) ,
flSubsoilId integer ,
flType character varying(7) ,
flSubType character varying(20) ,
flAreaNumber character varying(100) ,
flName character varying(2000) ,
flDescription character varying ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (18, 2) ,
flBlocksCount integer ,
flExceptions character varying ,
flLandUrl character varying ,
flWkt character varying , 

CONSTRAINT PK_e0e8e6130ea18ad4e5b4ad9721d22ecf781bdc00 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbObjectsOrderResult (
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
)

);




CREATE TABLE trades.z_history_TbObjectsOrderResult (
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
)

);




CREATE TABLE trades.TbObjectsOrderNegotiations (
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
flIsActive boolean 
);




CREATE TABLE trades.z_history_TbObjectsOrderNegotiations (
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
)

);




CREATE TABLE trades.TbTrades (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgBin character varying(12) ,
flType character varying(7) ,
flStatus character varying(15) ,
flObjectId integer ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationTypes character varying(18) ,
flExploringPeriod character varying ,
flRequiredDocuments character varying ,
flRequiredConditions character varying ,
flParticipantConditions character varying ,
flAdditionalConditions character varying(26) ,
flComissionMembers character varying ,
flTaxOrganBin character varying(12) ,
flIik character varying(32) ,
flBik character varying(8) ,
flKnp integer ,
flKbe integer ,
flKbk integer , 

CONSTRAINT PK_TbTrades PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbTrades (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgBin character varying(12) ,
flType character varying(7) ,
flStatus character varying(15) ,
flObjectId integer ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationTypes character varying(18) ,
flExploringPeriod character varying ,
flRequiredDocuments character varying ,
flRequiredConditions character varying ,
flParticipantConditions character varying ,
flAdditionalConditions character varying(26) ,
flComissionMembers character varying ,
flTaxOrganBin character varying(12) ,
flIik character varying(32) ,
flBik character varying(8) ,
flKnp integer ,
flKbe integer ,
flKbk integer , 

CONSTRAINT PK_2bf44c20805121f5b8fe4d5e9fb25321ebc98edd PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgBin character varying(12) ,
flType character varying(7) ,
flStatus character varying(15) ,
flObjectId integer ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationTypes character varying(18) ,
flExploringPeriod character varying ,
flRequiredDocuments character varying ,
flRequiredConditions character varying ,
flParticipantConditions character varying ,
flAdditionalConditions character varying(26) ,
flComissionMembers character varying ,
flTaxOrganBin character varying(12) ,
flIik character varying(32) ,
flBik character varying(8) ,
flKnp integer ,
flKbe integer ,
flKbk integer , 

CONSTRAINT PK_TbTradesRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE trades.z_history_TbTradesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flCompetentOrgBin character varying(12) ,
flType character varying(7) ,
flStatus character varying(15) ,
flObjectId integer ,
flAppDateTime timestamp without time zone ,
flDateTime timestamp without time zone ,
flStartRate numeric (18, 2) ,
flGarPay numeric (18, 2) ,
flTradeOperationTypes character varying(18) ,
flExploringPeriod character varying ,
flRequiredDocuments character varying ,
flRequiredConditions character varying ,
flParticipantConditions character varying ,
flAdditionalConditions character varying(26) ,
flComissionMembers character varying ,
flTaxOrganBin character varying(12) ,
flIik character varying(32) ,
flBik character varying(8) ,
flKnp integer ,
flKbe integer ,
flKbk integer , 

CONSTRAINT PK_9fad9f0ae53373d857605cd226dea7f7bbfdfee9 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradesOrderResult (
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
flType character varying(7) , 

CONSTRAINT PK_TbTradesOrderResult PRIMARY KEY (
    flSubjectId
)

);




CREATE TABLE trades.z_history_TbTradesOrderResult (
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
flType character varying(7) , 

CONSTRAINT PK_a47a8dc2aebb25c2f84fd81cbda950afaa7dfe5e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradesOrderNegotiations (
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
flIsActive boolean 
);




CREATE TABLE trades.z_history_TbTradesOrderNegotiations (
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
)

);




CREATE TABLE trades.TbTradesRequiredDocuments (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTradeId integer ,
flRevisionId integer ,
flId character varying ,
flText character varying ,
flType character varying 
);




CREATE TABLE trades.z_history_TbTradesRequiredDocuments (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTradeId integer ,
flRevisionId integer ,
flId character varying ,
flText character varying ,
flType character varying , 

CONSTRAINT PK_d8a2a470158d4227508f513f9b0e29bc320a139b PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradeParticipantConditions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTradeId integer ,
flRevisionId integer ,
flId character varying ,
flText character varying ,
flType character varying 
);




CREATE TABLE trades.z_history_TbTradeParticipantConditions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTradeId integer ,
flRevisionId integer ,
flId character varying ,
flText character varying ,
flType character varying , 

CONSTRAINT PK_4c05c22f911ba783522c3bdf70dc772a16f0de13 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradeChanges (
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
)

);




CREATE TABLE trades.z_history_TbTradeChanges (
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
)

);




CREATE TABLE trades.TbComissionMembers (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flCompetentOrgBin character varying(12) ,
flIin character varying(12) ,
flFio character varying ,
flInfo character varying ,
flStart date ,
flEnd date ,
flStatus character varying(7) , 

CONSTRAINT PK_TbComissionMembers PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbComissionMembers (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flCompetentOrgBin character varying(12) ,
flIin character varying(12) ,
flFio character varying ,
flInfo character varying ,
flStart date ,
flEnd date ,
flStatus character varying(7) , 

CONSTRAINT PK_9dce0f20e411d442cdec104a319316bf927cf6ec PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flType character varying(20) ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flKadNumber character varying ,
flBalanceHolderId integer ,
flBalanceHolderBin character varying(12) ,
flName character varying(300) ,
flDescription character varying ,
flLandArea numeric (18, 2) ,
flLandCategory character varying(10) ,
flCost numeric (18, 2) ,
flFuncPurpose1 character varying(30) ,
flFuncPurpose2 character varying(30) ,
flFuncPurpose3 character varying(30) ,
flFuncPurpose4 character varying(30) ,
flUsageAim character varying(2) ,
flPropertyType character varying(30) ,
flIsDivisible character varying(10) ,
flNote character varying ,
flPhotos character varying ,
flDocs character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flChargePresence character varying(20) ,
flCharge character varying ,
flRestriction character varying ,
flArest character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_TbLandObjects PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbLandObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flType character varying(20) ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flKadNumber character varying ,
flBalanceHolderId integer ,
flBalanceHolderBin character varying(12) ,
flName character varying(300) ,
flDescription character varying ,
flLandArea numeric (18, 2) ,
flLandCategory character varying(10) ,
flCost numeric (18, 2) ,
flFuncPurpose1 character varying(30) ,
flFuncPurpose2 character varying(30) ,
flFuncPurpose3 character varying(30) ,
flFuncPurpose4 character varying(30) ,
flUsageAim character varying(2) ,
flPropertyType character varying(30) ,
flIsDivisible character varying(10) ,
flNote character varying ,
flPhotos character varying ,
flDocs character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flChargePresence character varying(20) ,
flCharge character varying ,
flRestriction character varying ,
flArest character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_0051e63f90f9cd88a21ec447f0bcd6af863b9355 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flType character varying(20) ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flKadNumber character varying ,
flBalanceHolderId integer ,
flBalanceHolderBin character varying(12) ,
flName character varying(300) ,
flDescription character varying ,
flLandArea numeric (18, 2) ,
flLandCategory character varying(10) ,
flCost numeric (18, 2) ,
flFuncPurpose1 character varying(30) ,
flFuncPurpose2 character varying(30) ,
flFuncPurpose3 character varying(30) ,
flFuncPurpose4 character varying(30) ,
flUsageAim character varying(2) ,
flPropertyType character varying(30) ,
flIsDivisible character varying(10) ,
flNote character varying ,
flPhotos character varying ,
flDocs character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flChargePresence character varying(20) ,
flCharge character varying ,
flRestriction character varying ,
flArest character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_TbLandObjectsRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE trades.z_history_TbLandObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flType character varying(20) ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flKadNumber character varying ,
flBalanceHolderId integer ,
flBalanceHolderBin character varying(12) ,
flName character varying(300) ,
flDescription character varying ,
flLandArea numeric (18, 2) ,
flLandCategory character varying(10) ,
flCost numeric (18, 2) ,
flFuncPurpose1 character varying(30) ,
flFuncPurpose2 character varying(30) ,
flFuncPurpose3 character varying(30) ,
flFuncPurpose4 character varying(30) ,
flUsageAim character varying(2) ,
flPropertyType character varying(30) ,
flIsDivisible character varying(10) ,
flNote character varying ,
flPhotos character varying ,
flDocs character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flChargePresence character varying(20) ,
flCharge character varying ,
flRestriction character varying ,
flArest character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_93ee95ee864075a9e2474e56fb991145e4c1081e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsOrderResult (
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

CONSTRAINT PK_TbLandObjectsOrderResult PRIMARY KEY (
    flSubjectId
)

);




CREATE TABLE trades.z_history_TbLandObjectsOrderResult (
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

CONSTRAINT PK_608e00a6f415d0e85223967764732075b736df43 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsOrderNegotiations (
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
flIsActive boolean 
);




CREATE TABLE trades.z_history_TbLandObjectsOrderNegotiations (
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

CONSTRAINT PK_dc7aacba54a0c801acc02193e6a55626c33499a0 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsTrades (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flForm character varying(10) NOT NULL,
flMethod character varying(10) NOT NULL,
flType character varying(10) NOT NULL,
flStatus character varying(10) NOT NULL,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flAdditionalCondition character varying(10) ,
flUpFactor numeric (18, 2) ,
flCostMinCondition character varying(10) ,
flOwnershipMonths integer ,
flLandLeaseAvailable character varying(10) ,
flLandLeaseLength integer ,
flInitialCost numeric (18, 2) NOT NULL,
flDeposit numeric (18, 2) NOT NULL,
flCostStart numeric (18, 2) NOT NULL,
flCostMin numeric (18, 2) ,
flWinnerId integer ,
flWinnerBin character varying(12) ,
flSaleCondition character varying ,
flCost numeric (18, 2) ,
flPostControl character varying(10) ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flControlDateStart date ,
flControlDateEnd date ,
flControlFrequency character varying(10) ,
flControlCondition character varying ,
flTaxAuthorityBin character varying(12) NOT NULL,
flBik character varying(8) NOT NULL,
flIik character varying(32) NOT NULL,
flKnp integer NOT NULL,
flKbk character varying(6) NOT NULL,
flKbe integer NOT NULL, 

CONSTRAINT PK_TbLandObjectsTrades PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbLandObjectsTrades (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flForm character varying(10) NOT NULL,
flMethod character varying(10) NOT NULL,
flType character varying(10) NOT NULL,
flStatus character varying(10) NOT NULL,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flAdditionalCondition character varying(10) ,
flUpFactor numeric (18, 2) ,
flCostMinCondition character varying(10) ,
flOwnershipMonths integer ,
flLandLeaseAvailable character varying(10) ,
flLandLeaseLength integer ,
flInitialCost numeric (18, 2) NOT NULL,
flDeposit numeric (18, 2) NOT NULL,
flCostStart numeric (18, 2) NOT NULL,
flCostMin numeric (18, 2) ,
flWinnerId integer ,
flWinnerBin character varying(12) ,
flSaleCondition character varying ,
flCost numeric (18, 2) ,
flPostControl character varying(10) ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flControlDateStart date ,
flControlDateEnd date ,
flControlFrequency character varying(10) ,
flControlCondition character varying ,
flTaxAuthorityBin character varying(12) NOT NULL,
flBik character varying(8) NOT NULL,
flIik character varying(32) NOT NULL,
flKnp integer NOT NULL,
flKbk character varying(6) NOT NULL,
flKbe integer NOT NULL, 

CONSTRAINT PK_18c190c12857edb6c2a312857c9c00c162dbebe6 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsTradesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flForm character varying(10) NOT NULL,
flMethod character varying(10) NOT NULL,
flType character varying(10) NOT NULL,
flStatus character varying(10) NOT NULL,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flAdditionalCondition character varying(10) ,
flUpFactor numeric (18, 2) ,
flCostMinCondition character varying(10) ,
flOwnershipMonths integer ,
flLandLeaseAvailable character varying(10) ,
flLandLeaseLength integer ,
flInitialCost numeric (18, 2) NOT NULL,
flDeposit numeric (18, 2) NOT NULL,
flCostStart numeric (18, 2) NOT NULL,
flCostMin numeric (18, 2) ,
flWinnerId integer ,
flWinnerBin character varying(12) ,
flSaleCondition character varying ,
flCost numeric (18, 2) ,
flPostControl character varying(10) ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flControlDateStart date ,
flControlDateEnd date ,
flControlFrequency character varying(10) ,
flControlCondition character varying ,
flTaxAuthorityBin character varying(12) NOT NULL,
flBik character varying(8) NOT NULL,
flIik character varying(32) NOT NULL,
flKnp integer NOT NULL,
flKbk character varying(6) NOT NULL,
flKbe integer NOT NULL, 

CONSTRAINT PK_TbLandObjectsTradesRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE trades.z_history_TbLandObjectsTradesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flObjectId integer NOT NULL,
flForm character varying(10) NOT NULL,
flMethod character varying(10) NOT NULL,
flType character varying(10) NOT NULL,
flStatus character varying(10) NOT NULL,
flCompetentOrgBin character varying(12) NOT NULL,
flDateTime timestamp without time zone NOT NULL,
flNote character varying NOT NULL,
flAdditionalCondition character varying(10) ,
flUpFactor numeric (18, 2) ,
flCostMinCondition character varying(10) ,
flOwnershipMonths integer ,
flLandLeaseAvailable character varying(10) ,
flLandLeaseLength integer ,
flInitialCost numeric (18, 2) NOT NULL,
flDeposit numeric (18, 2) NOT NULL,
flCostStart numeric (18, 2) NOT NULL,
flCostMin numeric (18, 2) ,
flWinnerId integer ,
flWinnerBin character varying(12) ,
flSaleCondition character varying ,
flCost numeric (18, 2) ,
flPostControl character varying(10) ,
flIsSaleAgreementExists boolean ,
flSaleAgreementDate date ,
flSaleAgreementNumber character varying ,
flPrepayment numeric (18, 2) ,
flRemainingSum numeric (18, 2) ,
flRemainingSumDate date ,
flIsSaleProtocolExists boolean ,
flSaleProtocolDate date ,
flSaleProtocolNumber character varying ,
flControlDateStart date ,
flControlDateEnd date ,
flControlFrequency character varying(10) ,
flControlCondition character varying ,
flTaxAuthorityBin character varying(12) NOT NULL,
flBik character varying(8) NOT NULL,
flIik character varying(32) NOT NULL,
flKnp integer NOT NULL,
flKbk character varying(6) NOT NULL,
flKbe integer NOT NULL, 

CONSTRAINT PK_e33e7494940c733fce614b972df352cd24d10057 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsTradesOrderResult (
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

CONSTRAINT PK_TbLandObjectsTradesOrderResult PRIMARY KEY (
    flSubjectId
)

);




CREATE TABLE trades.z_history_TbLandObjectsTradesOrderResult (
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

CONSTRAINT PK_f90182ea126b4683b26fbdf2d0a18965120c79eb PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbLandObjectsTradesOrderNegotiations (
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
flIsActive boolean 
);




CREATE TABLE trades.z_history_TbLandObjectsTradesOrderNegotiations (
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

CONSTRAINT PK_568e216e3a589b6cc43a81adf6dc605ce936c1ac PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.tbsubsoilsobjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flName character varying(500) NOT NULL,
flNumber character varying ,
flInfo character varying ,
flType character varying(11) ,
flBlockCount integer ,
flArea numeric (18, 6) ,
flExceptions character varying ,
flRegion character varying ,
flDistrict character varying ,
flStatus character varying(19) ,
flWKT character varying ,
flGeometry character varying ,
flPoint character varying , 

CONSTRAINT PK_tbsubsoilsobjects PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_tbsubsoilsobjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flName character varying(500) NOT NULL,
flNumber character varying ,
flInfo character varying ,
flType character varying(11) ,
flBlockCount integer ,
flArea numeric (18, 6) ,
flExceptions character varying ,
flRegion character varying ,
flDistrict character varying ,
flStatus character varying(19) ,
flWKT character varying ,
flGeometry character varying ,
flPoint character varying , 

CONSTRAINT PK_8cfb09584ff23329444f2c9344bf3febd61ce888 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradePreApplication (
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
)

);

CREATE UNIQUE INDEX IX_AppTbTradePreApplication_AppGuidId ON trades.TbTradePreApplication USING btree (flAppGuidId);


CREATE TABLE trades.z_history_TbTradePreApplication (
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
)

);




CREATE TABLE trades.TbTradePreApplicationSigns (
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
)

);




CREATE TABLE trades.z_history_TbTradePreApplicationSigns (
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
)

);




CREATE TABLE trades.TbDeprivedPersons (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flFio character varying NOT NULL,
flXin character varying(12) NOT NULL,
flReasonForInclusion character varying NOT NULL,
flNote character varying NOT NULL,
flDateOfInclusion date NOT NULL,
flDateOfExclusion date ,
flStatus character varying(7) NOT NULL, 

CONSTRAINT PK_TbDeprivedPersons PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbDeprivedPersons (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flFio character varying NOT NULL,
flXin character varying(12) NOT NULL,
flReasonForInclusion character varying NOT NULL,
flNote character varying NOT NULL,
flDateOfInclusion date NOT NULL,
flDateOfExclusion date ,
flStatus character varying(7) NOT NULL, 

CONSTRAINT PK_e3e71487277e6e4592412bd3f63e9d766fdaf095 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradePreApplicationApproovals (
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
)

);




CREATE TABLE trades.z_history_TbTradePreApplicationApproovals (
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
)

);




CREATE TABLE trades.TbOilSectorOrganizations (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flName character varying NOT NULL,
flCountry character varying(20) NOT NULL,
flRegion character varying(100) NOT NULL,
flStatus character varying(7) NOT NULL,
flRnn character varying(12) NOT NULL,
flBin character varying(12) NOT NULL, 

CONSTRAINT PK_TbOilSectorOrganizations PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbOilSectorOrganizations (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flName character varying NOT NULL,
flCountry character varying(20) NOT NULL,
flRegion character varying(100) NOT NULL,
flStatus character varying(7) NOT NULL,
flRnn character varying(12) NOT NULL,
flBin character varying(12) NOT NULL, 

CONSTRAINT PK_507f5507d755da823a902d1bb99ffb5988d152fe PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.tbsubsoilspolygons (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer ,
flSubsoilNumber character varying ,
flGeomNumber character varying ,
flGeomCode character varying ,
flGeomType character varying 
);




CREATE TABLE trades.z_history_tbsubsoilspolygons (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer ,
flSubsoilNumber character varying ,
flGeomNumber character varying ,
flGeomCode character varying ,
flGeomType character varying , 

CONSTRAINT PK_19a0c8590d6aad042bac20b8cf13fe536549679e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.tbsubsoilspolygonscoords (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer ,
flGeomCode character varying ,
flCoordNumber character varying ,
flLongitute character varying ,
flLatitude character varying ,
flxdecimal character varying ,
flydecimal character varying ,
flWkt character varying ,
flPoint character varying 
);




CREATE TABLE trades.z_history_tbsubsoilspolygonscoords (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer ,
flGeomCode character varying ,
flCoordNumber character varying ,
flLongitute character varying ,
flLatitude character varying ,
flxdecimal character varying ,
flydecimal character varying ,
flWkt character varying ,
flPoint character varying , 

CONSTRAINT PK_44649afcbb0400059496b25e30cda01b4de76844 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbTradePreApplicationsSentToEsedo (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAppId integer NOT NULL,
flSendDate timestamp without time zone , 

CONSTRAINT PK_TbTradePreApplicationsSentToEsedo PRIMARY KEY (
    flAppId
)

);




CREATE TABLE trades.z_history_TbTradePreApplicationsSentToEsedo (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAppId integer NOT NULL,
flSendDate timestamp without time zone , 

CONSTRAINT PK_fd172e82b50f455d21c72aee41d77c0f3f419690 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE trades."seq_TbObjects_flId";


CREATE SEQUENCE trades."seq_TbObjectsRevisions_flId";


CREATE SEQUENCE trades."seq_TbObjectsOrderResult_flSubjectId";


CREATE SEQUENCE trades."seq_TbObjectsOrderNegotiations_flActionId";


CREATE SEQUENCE trades."seq_TbTrades_flId";


CREATE SEQUENCE trades."seq_TbTradesRevisions_flId";


CREATE SEQUENCE trades."seq_TbTradesOrderResult_flSubjectId";


CREATE SEQUENCE trades."seq_TbTradesOrderNegotiations_flActionId";


CREATE SEQUENCE trades."seq_TbTradeChanges_flId";


CREATE SEQUENCE trades."seq_TbComissionMembers_flId";


CREATE SEQUENCE trades."seq_TbLandObjects_flId";


CREATE SEQUENCE trades."seq_TbLandObjectsRevisions_flId";


CREATE SEQUENCE trades."seq_TbLandObjectsOrderResult_flSubjectId";


CREATE SEQUENCE trades."seq_TbLandObjectsOrderNegotiations_flActionId";


CREATE SEQUENCE trades."seq_TbLandObjectsTrades_flId";


CREATE SEQUENCE trades."seq_TbLandObjectsTradesRevisions_flId";


CREATE SEQUENCE trades."seq_TbLandObjectsTradesOrderResult_flSubjectId";


CREATE SEQUENCE trades."seq_TbLandObjectsTradesOrderNegotiations_flActionId";


CREATE SEQUENCE trades."seq_TbTradePreApplication_flAppId";


CREATE SEQUENCE trades."seq_TbDeprivedPersons_flId";


CREATE SEQUENCE trades."seq_TbTradePreApplicationApproovals_flId";


CREATE SEQUENCE trades."seq_TbOilSectorOrganizations_flId";
