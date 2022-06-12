
CREATE TABLE trades.TbLandObjectTradeChanges (
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

CONSTRAINT PK_TbLandObjectTradeChanges PRIMARY KEY (
    flId
)

);

CREATE TABLE trades.z_history_TbLandObjectTradeChanges (
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

CONSTRAINT PK_86873ddc46db6a10fbb3f53b8a442968bb238029 PRIMARY KEY (
    flEntityId, flRequestId
)

);

CREATE SEQUENCE trades."seq_TbLandObjectTradeChanges_flId";
