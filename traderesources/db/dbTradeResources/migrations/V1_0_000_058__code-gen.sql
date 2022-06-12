
ALTER TABLE telecomoperators.tbobjects 
RENAME flcost TO __rm__flcost; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE telecomoperators.tbobjectsrevisions 
RENAME flcost TO __rm__flcost; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE telecomoperators.z_history_tbobjects 
RENAME flcost TO __rm__flcost; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE telecomoperators.z_history_tbobjectsrevisions 
RENAME flcost TO __rm__flcost; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


CREATE TABLE telecomoperators.TbTradeChanges (
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


CREATE TABLE telecomoperators.z_history_TbTradeChanges (
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


CREATE TABLE telecomoperators.TbTrades (
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


CREATE TABLE telecomoperators.z_history_TbTrades (
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


CREATE TABLE telecomoperators.TbTradesRevisions (
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


CREATE TABLE telecomoperators.z_history_TbTradesRevisions (
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


CREATE TABLE telecomoperators.TbTradesOrderResult (
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


CREATE TABLE telecomoperators.z_history_TbTradesOrderResult (
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


CREATE TABLE telecomoperators.TbTradesOrderNegotiations (
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


CREATE TABLE telecomoperators.z_history_TbTradesOrderNegotiations (
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


CREATE SEQUENCE telecomoperators."seq_TbTradeChanges_flId";


CREATE SEQUENCE telecomoperators."seq_TbTrades_flId";


CREATE SEQUENCE telecomoperators."seq_TbTradesRevisions_flId";


CREATE SEQUENCE telecomoperators."seq_TbTradesOrderResult_flSubjectId";


CREATE SEQUENCE telecomoperators."seq_TbTradesOrderNegotiations_flActionId";
