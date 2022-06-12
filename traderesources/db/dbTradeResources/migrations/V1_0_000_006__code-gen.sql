
CREATE TABLE trades.TbLandObjectsTradesPublications (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flLandObjectTradeId integer NOT NULL,
flLandObjectTradeRevisionId integer NOT NULL,
flNewspaperLang character varying(10) ,
flNewspaperId character varying(10) ,
flNumber character varying ,
flDate date ,
flText character varying ,
flWebDate date , 

CONSTRAINT PK_TbLandObjectsTradesPublications PRIMARY KEY (
    flId
)

);

CREATE TABLE trades.z_history_TbLandObjectsTradesPublications (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flLandObjectTradeId integer NOT NULL,
flLandObjectTradeRevisionId integer NOT NULL,
flNewspaperLang character varying(10) ,
flNewspaperId character varying(10) ,
flNumber character varying ,
flDate date ,
flText character varying ,
flWebDate date , 

CONSTRAINT PK_e34cdcbcdcffbf9717e473381c669344c644fb55 PRIMARY KEY (
    flEntityId, flRequestId
)

);

CREATE SEQUENCE trades."seq_TbLandObjectsTradesPublications_flId";