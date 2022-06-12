
CREATE SCHEMA fishing;

CREATE TABLE fishing.TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flReservoirId integer ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flLength numeric (22, 2) ,
flWidth numeric (22, 2) ,
flArea numeric (22, 2) ,
flMaxDepth numeric (22, 2) ,
flAvgDepth numeric (22, 2) ,
flEmergentVegetation character varying(10) ,
flUnderwaterVegetation character varying(10) ,
flPhytoplankton character varying(10) ,
flFauna character varying ,
flСommercialFauna character varying ,
flFishProductivity character varying ,
flPollution character varying ,
flWaterIntakes character varying ,
flPermanentUsingPlacesCount integer ,
flAnotherInformation character varying , 

CONSTRAINT PK_TbObjects PRIMARY KEY (
    flId
)

);




CREATE TABLE fishing.z_history_TbObjects (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flReservoirId integer ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flLength numeric (18, 2) ,
flWidth numeric (18, 2) ,
flArea numeric (18, 2) ,
flMaxDepth numeric (18, 2) ,
flAvgDepth numeric (18, 2) ,
flEmergentVegetation character varying(10) ,
flUnderwaterVegetation character varying(10) ,
flPhytoplankton character varying(10) ,
flFauna character varying ,
flСommercialFauna character varying ,
flFishProductivity character varying ,
flPollution character varying ,
flWaterIntakes character varying ,
flPermanentUsingPlacesCount integer ,
flAnotherInformation character varying , 

CONSTRAINT PK_69c1c699d4a69641e27a4241c1a63d3aa09cb9d3 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE fishing.TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flReservoirId integer ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flLength numeric (22, 2) ,
flWidth numeric (22, 2) ,
flArea numeric (22, 2) ,
flMaxDepth numeric (22, 2) ,
flAvgDepth numeric (22, 2) ,
flEmergentVegetation character varying(10) ,
flUnderwaterVegetation character varying(10) ,
flPhytoplankton character varying(10) ,
flFauna character varying ,
flСommercialFauna character varying ,
flFishProductivity character varying ,
flPollution character varying ,
flWaterIntakes character varying ,
flPermanentUsingPlacesCount integer ,
flAnotherInformation character varying , 

CONSTRAINT PK_TbObjectsRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE fishing.z_history_TbObjectsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flReservoirId integer ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flLength numeric (18, 2) ,
flWidth numeric (18, 2) ,
flArea numeric (18, 2) ,
flMaxDepth numeric (18, 2) ,
flAvgDepth numeric (18, 2) ,
flEmergentVegetation character varying(10) ,
flUnderwaterVegetation character varying(10) ,
flPhytoplankton character varying(10) ,
flFauna character varying ,
flСommercialFauna character varying ,
flFishProductivity character varying ,
flPollution character varying ,
flWaterIntakes character varying ,
flPermanentUsingPlacesCount integer ,
flAnotherInformation character varying , 

CONSTRAINT PK_e0e8e6130ea18ad4e5b4ad9721d22ecf781bdc00 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE fishing.TbObjectsOrderResult (
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




CREATE TABLE fishing.z_history_TbObjectsOrderResult (
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




CREATE TABLE fishing.TbObjectsOrderNegotiations (
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




CREATE TABLE fishing.z_history_TbObjectsOrderNegotiations (
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




CREATE TABLE fishing.TbReservoirs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flStatus character varying(7) ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flArea numeric (22, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_TbReservoirs PRIMARY KEY (
    flId
)

);




CREATE TABLE fishing.z_history_TbReservoirs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flStatus character varying(7) ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flArea numeric (18, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_99544716bd01f6ef77c2c97b22d6e85fd4aac74e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE fishing.TbReservoirsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flStatus character varying(7) ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flArea numeric (22, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_TbReservoirsRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE fishing.z_history_TbReservoirsRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flStatus character varying(7) ,
flName character varying(300) ,
flDescription character varying ,
flNote character varying ,
flArea numeric (18, 2) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) , 

CONSTRAINT PK_af9dec357829af212a3dd2c2dc31978a8396e2d2 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE fishing.TbReservoirOrderResult (
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

CONSTRAINT PK_TbReservoirOrderResult PRIMARY KEY (
    flSubjectId
)

);




CREATE TABLE fishing.z_history_TbReservoirOrderResult (
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

CONSTRAINT PK_aedf72e2e58baa60ce173e14bd17647701e36a20 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE fishing.TbReservoirOrdersNegotiations (
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




CREATE TABLE fishing.z_history_TbReservoirOrdersNegotiations (
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

CONSTRAINT PK_cfbd5cb7ba1750dd9555a7040d6439860ade2278 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE fishing."seq_TbObjects_flId";


CREATE SEQUENCE fishing."seq_TbObjectsRevisions_flId";


CREATE SEQUENCE fishing."seq_TbObjectsOrderResult_flSubjectId";


CREATE SEQUENCE fishing."seq_TbObjectsOrderNegotiations_flActionId";


CREATE SEQUENCE fishing."seq_TbReservoirs_flId";


CREATE SEQUENCE fishing."seq_TbReservoirsRevisions_flId";


CREATE SEQUENCE fishing."seq_TbReservoirOrderResult_flSubjectId";


CREATE SEQUENCE fishing."seq_TbReservoirOrdersNegotiations_flActionId";





CREATE SCHEMA hunting;

CREATE TABLE hunting.TbObjects (
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
flYear integer ,
flCategory character varying(20) ,
flWildfowlThroughput character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flHuntArea numeric (22, 2) ,
flForestArea numeric (22, 2) ,
flAgriArea numeric (22, 2) ,
flWaterArea numeric (22, 2) ,
flLandReserveArea numeric (22, 2) ,
flOtherArea numeric (22, 2) ,
flRangerSites integer ,
flInsideOrganization character varying ,
flHuntingState character varying ,
flHarmfulAnimals character varying ,
flAnimalsAdmission character varying ,
flBiotechnicalActivities character varying ,
flConsideredAnimals character varying ,
flWildAnimalsFeed character varying ,
flInvestedWork character varying ,
flAntipoaching character varying ,
flHuntingDogsAndDecoyDucks character varying ,
flBuildings character varying ,
flTransport character varying , 

CONSTRAINT PK_TbObjects PRIMARY KEY (
    flId
)

);




CREATE TABLE hunting.z_history_TbObjects (
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
flYear integer ,
flCategory character varying(20) ,
flWildfowlThroughput character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flHuntArea numeric (18, 2) ,
flForestArea numeric (18, 2) ,
flAgriArea numeric (18, 2) ,
flWaterArea numeric (18, 2) ,
flLandReserveArea numeric (18, 2) ,
flOtherArea numeric (18, 2) ,
flRangerSites integer ,
flInsideOrganization character varying ,
flHuntingState character varying ,
flHarmfulAnimals character varying ,
flAnimalsAdmission character varying ,
flBiotechnicalActivities character varying ,
flConsideredAnimals character varying ,
flWildAnimalsFeed character varying ,
flInvestedWork character varying ,
flAntipoaching character varying ,
flHuntingDogsAndDecoyDucks character varying ,
flBuildings character varying ,
flTransport character varying , 

CONSTRAINT PK_69c1c699d4a69641e27a4241c1a63d3aa09cb9d3 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE hunting.TbObjectsRevisions (
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
flYear integer ,
flCategory character varying(20) ,
flWildfowlThroughput character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flHuntArea numeric (22, 2) ,
flForestArea numeric (22, 2) ,
flAgriArea numeric (22, 2) ,
flWaterArea numeric (22, 2) ,
flLandReserveArea numeric (22, 2) ,
flOtherArea numeric (22, 2) ,
flRangerSites integer ,
flInsideOrganization character varying ,
flHuntingState character varying ,
flHarmfulAnimals character varying ,
flAnimalsAdmission character varying ,
flBiotechnicalActivities character varying ,
flConsideredAnimals character varying ,
flWildAnimalsFeed character varying ,
flInvestedWork character varying ,
flAntipoaching character varying ,
flHuntingDogsAndDecoyDucks character varying ,
flBuildings character varying ,
flTransport character varying , 

CONSTRAINT PK_TbObjectsRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE hunting.z_history_TbObjectsRevisions (
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
flYear integer ,
flCategory character varying(20) ,
flWildfowlThroughput character varying ,
flSallerId integer ,
flSallerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flLocation character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flHuntArea numeric (18, 2) ,
flForestArea numeric (18, 2) ,
flAgriArea numeric (18, 2) ,
flWaterArea numeric (18, 2) ,
flLandReserveArea numeric (18, 2) ,
flOtherArea numeric (18, 2) ,
flRangerSites integer ,
flInsideOrganization character varying ,
flHuntingState character varying ,
flHarmfulAnimals character varying ,
flAnimalsAdmission character varying ,
flBiotechnicalActivities character varying ,
flConsideredAnimals character varying ,
flWildAnimalsFeed character varying ,
flInvestedWork character varying ,
flAntipoaching character varying ,
flHuntingDogsAndDecoyDucks character varying ,
flBuildings character varying ,
flTransport character varying , 

CONSTRAINT PK_e0e8e6130ea18ad4e5b4ad9721d22ecf781bdc00 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE hunting.TbObjectsOrderResult (
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




CREATE TABLE hunting.z_history_TbObjectsOrderResult (
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




CREATE TABLE hunting.TbObjectsOrderNegotiations (
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




CREATE TABLE hunting.z_history_TbObjectsOrderNegotiations (
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




CREATE SEQUENCE hunting."seq_TbObjects_flId";


CREATE SEQUENCE hunting."seq_TbObjectsRevisions_flId";


CREATE SEQUENCE hunting."seq_TbObjectsOrderResult_flSubjectId";


CREATE SEQUENCE hunting."seq_TbObjectsOrderNegotiations_flActionId";
