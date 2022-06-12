
CREATE SCHEMA telecomoperators;

CREATE TABLE telecomoperators.TbObjects (
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
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCost numeric (18, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flDocs character varying ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbObjects PRIMARY KEY (
    flId
));

CREATE UNIQUE INDEX IX_TbObjects_flId ON telecomoperators.TbObjects USING btree (flId);
CREATE UNIQUE INDEX IX_TbObjects_flRevisionId ON telecomoperators.TbObjects USING btree (flRevisionId);

CREATE TABLE telecomoperators.z_history_TbObjects (
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
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCost numeric (18, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flDocs character varying ,
flAnotherInformation character varying , 
CONSTRAINT PK_69c1c699d4a69641e27a4241c1a63d3aa09cb9d3 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE telecomoperators.TbObjectsRevisions (
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
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCost numeric (18, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flDocs character varying ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbObjectsRevisions PRIMARY KEY (
    flRevisionId
));

CREATE INDEX IX_TbObjectsRevisions_flId ON telecomoperators.TbObjectsRevisions USING btree (flId);
CREATE UNIQUE INDEX IX_TbObjectsRevisions_flRevisionId ON telecomoperators.TbObjectsRevisions USING btree (flRevisionId);

CREATE TABLE telecomoperators.z_history_TbObjectsRevisions (
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
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCost numeric (18, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flKfsLevel1 character varying(16) ,
flKfsLevel2 character varying(16) ,
flKfsLevel3 character varying(16) ,
flKfsLevel4 character varying(16) ,
flKfsLevel5 character varying(16) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flDocs character varying ,
flAnotherInformation character varying , 
CONSTRAINT PK_e0e8e6130ea18ad4e5b4ad9721d22ecf781bdc00 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE telecomoperators.TbObjectsOrderResult (
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

CREATE UNIQUE INDEX IX_TbObjectsOrderResult ON telecomoperators.TbObjectsOrderResult USING btree (flSubjectId);

CREATE TABLE telecomoperators.z_history_TbObjectsOrderResult (
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


CREATE TABLE telecomoperators.TbObjectsOrderNegotiations (
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

CREATE INDEX IX_TbObjectsOrderNegotiations ON telecomoperators.TbObjectsOrderNegotiations USING btree (flSubjectId);

CREATE TABLE telecomoperators.z_history_TbObjectsOrderNegotiations (
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


CREATE SEQUENCE telecomoperators."seq_TbObjects_flId";


CREATE SEQUENCE telecomoperators."seq_TbObjectsRevisions_flId";


CREATE SEQUENCE telecomoperators."seq_TbObjectsOrderResult_flSubjectId";


CREATE SEQUENCE telecomoperators."seq_TbObjectsOrderNegotiations_flActionId";
