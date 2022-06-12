
ALTER TABLE forest.tbobjects 
RENAME flsallerbin TO __rm__flsallerbin; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flforestusing TO __rm__flforestusing; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flpoint TO __rm__flpoint; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flsallerid TO __rm__flsallerid; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME fllocation TO __rm__fllocation; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flcoords TO __rm__flcoords; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flwkt TO __rm__flwkt; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flcountry TO __rm__flcountry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME fldistrict TO __rm__fldistrict; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flregion TO __rm__flregion; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects 
RENAME flgeometry TO __rm__flgeometry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjects
ADD COLUMN flForestryPieces character varying NOT NULL;


ALTER TABLE forest.tbobjects
ADD COLUMN flSellerBin character varying(12) ;


ALTER TABLE forest.tbobjects
ADD COLUMN flForestUsingPeriod integer NOT NULL;


ALTER TABLE forest.tbobjects
ADD COLUMN flYearPay numeric (18, 2) NOT NULL;


ALTER TABLE forest.tbobjects
ADD COLUMN flYearReproduction numeric (18, 2) NOT NULL;


ALTER TABLE forest.tbobjects
ADD COLUMN flDocs character varying ;

CREATE UNIQUE INDEX IX_TbObjects_flId ON forest.TbObjects USING btree (flId);
CREATE UNIQUE INDEX IX_TbObjects_flRevisionId ON forest.TbObjects USING btree (flRevisionId);
CREATE INDEX IX_TbObjectsOrderNegotiations ON forest.TbObjectsOrderNegotiations USING btree (flSubjectId);
CREATE UNIQUE INDEX IX_TbObjectsOrderResult ON forest.TbObjectsOrderResult USING btree (flSubjectId);

ALTER TABLE forest.tbobjectsrevisions 
RENAME flpoint TO __rm__flpoint; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flforestusing TO __rm__flforestusing; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flsallerbin TO __rm__flsallerbin; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flgeometry TO __rm__flgeometry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flregion TO __rm__flregion; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME fldistrict TO __rm__fldistrict; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flcountry TO __rm__flcountry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flwkt TO __rm__flwkt; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flcoords TO __rm__flcoords; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME flsallerid TO __rm__flsallerid; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions 
RENAME fllocation TO __rm__fllocation; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flForestryPieces character varying NOT NULL;


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flSellerBin character varying(12) ;


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flForestUsingPeriod integer NOT NULL;


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flYearPay numeric (18, 2) NOT NULL;


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flYearReproduction numeric (18, 2) NOT NULL;


ALTER TABLE forest.tbobjectsrevisions
ADD COLUMN flDocs character varying ;

CREATE INDEX IX_TbObjectsRevisions_flId ON forest.TbObjectsRevisions USING btree (flId);
CREATE UNIQUE INDEX IX_TbObjectsRevisions_flRevisionId ON forest.TbObjectsRevisions USING btree (flRevisionId);

ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flpoint TO __rm__flpoint; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flforestusing TO __rm__flforestusing; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flsallerbin TO __rm__flsallerbin; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flcountry TO __rm__flcountry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flwkt TO __rm__flwkt; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flcoords TO __rm__flcoords; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flsallerid TO __rm__flsallerid; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME fllocation TO __rm__fllocation; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flgeometry TO __rm__flgeometry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME flregion TO __rm__flregion; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions 
RENAME fldistrict TO __rm__fldistrict; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flForestryPieces character varying NOT NULL;


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flSellerBin character varying(12) ;


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flForestUsingPeriod integer NOT NULL;


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flYearPay numeric (18, 2) NOT NULL;


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flYearReproduction numeric (18, 2) NOT NULL;


ALTER TABLE forest.z_history_tbobjectsrevisions
ADD COLUMN flDocs character varying ;


ALTER TABLE forest.z_history_tbobjects 
RENAME flgeometry TO __rm__flgeometry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flregion TO __rm__flregion; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME fldistrict TO __rm__fldistrict; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flcountry TO __rm__flcountry; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flwkt TO __rm__flwkt; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flsallerid TO __rm__flsallerid; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME fllocation TO __rm__fllocation; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flcoords TO __rm__flcoords; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flpoint TO __rm__flpoint; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flsallerbin TO __rm__flsallerbin; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects 
RENAME flforestusing TO __rm__flforestusing; -- Дабы не потерять данные, просто переименовываем колонку. Удалить можно на следующих итерациях. Либо если вы уверенны что ничего страшного не произойдет замените на DROP COLUMN самостоятельно.


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flForestryPieces character varying NOT NULL;


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flSellerBin character varying(12) ;


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flForestUsingPeriod integer NOT NULL;


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flYearPay numeric (18, 2) NOT NULL;


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flYearReproduction numeric (18, 2) NOT NULL;


ALTER TABLE forest.z_history_tbobjects
ADD COLUMN flDocs character varying ;


CREATE TABLE forest.TbForestries (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) NOT NULL,
flStatus character varying(7) NOT NULL,
flDescription character varying NOT NULL,
flNote character varying NOT NULL,
flSellerBin character varying(12) NOT NULL,
flArea numeric (18, 2) NOT NULL,
flCountry character varying(10) NOT NULL,
flRegion character varying(10) NOT NULL,
flDistrict character varying(10) NOT NULL,
flLocation character varying NOT NULL,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying NOT NULL, 
CONSTRAINT PK_TbForestries PRIMARY KEY (
    flId
));

CREATE UNIQUE INDEX IX_TbForestries_flId ON forest.TbForestries USING btree (flId);
CREATE UNIQUE INDEX IX_TbForestries_flRevisionId ON forest.TbForestries USING btree (flRevisionId);

CREATE TABLE forest.z_history_TbForestries (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) NOT NULL,
flStatus character varying(7) NOT NULL,
flDescription character varying NOT NULL,
flNote character varying NOT NULL,
flSellerBin character varying(12) NOT NULL,
flArea numeric (18, 2) NOT NULL,
flCountry character varying(10) NOT NULL,
flRegion character varying(10) NOT NULL,
flDistrict character varying(10) NOT NULL,
flLocation character varying NOT NULL,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying NOT NULL, 
CONSTRAINT PK_4fd4614b9cee15e3e73b71036b13253cc62b58c1 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestriesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) NOT NULL,
flStatus character varying(7) NOT NULL,
flDescription character varying NOT NULL,
flNote character varying NOT NULL,
flSellerBin character varying(12) NOT NULL,
flArea numeric (18, 2) NOT NULL,
flCountry character varying(10) NOT NULL,
flRegion character varying(10) NOT NULL,
flDistrict character varying(10) NOT NULL,
flLocation character varying NOT NULL,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying NOT NULL, 
CONSTRAINT PK_TbForestriesRevisions PRIMARY KEY (
    flRevisionId
));

CREATE INDEX IX_TbForestriesRevisions_flId ON forest.TbForestriesRevisions USING btree (flId);
CREATE UNIQUE INDEX IX_TbForestriesRevisions_flRevisionId ON forest.TbForestriesRevisions USING btree (flRevisionId);

CREATE TABLE forest.z_history_TbForestriesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flName character varying(300) NOT NULL,
flStatus character varying(7) NOT NULL,
flDescription character varying NOT NULL,
flNote character varying NOT NULL,
flSellerBin character varying(12) NOT NULL,
flArea numeric (18, 2) NOT NULL,
flCountry character varying(10) NOT NULL,
flRegion character varying(10) NOT NULL,
flDistrict character varying(10) NOT NULL,
flLocation character varying NOT NULL,
flWKT character varying NOT NULL,
flCoords character varying NOT NULL,
flGeometry geometry(Geometry, 4326) NOT NULL,
flPoint geometry(Point, 4326) NOT NULL,
flAnotherInformation character varying NOT NULL, 
CONSTRAINT PK_50dacbe18f8c7fd339e1a15c8fcd5003ca6c380c PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestriesOrderResult (
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
CONSTRAINT PK_TbForestriesOrderResult PRIMARY KEY (
    flSubjectId
));

CREATE UNIQUE INDEX IX_TbForestriesOrderResult ON forest.TbForestriesOrderResult USING btree (flSubjectId);

CREATE TABLE forest.z_history_TbForestriesOrderResult (
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
CONSTRAINT PK_9f0e06276bbd2b64aab929c01f764ac7f686abf5 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestriesOrderNegotiations (
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

CREATE INDEX IX_TbForestriesOrderNegotiations ON forest.TbForestriesOrderNegotiations USING btree (flSubjectId);

CREATE TABLE forest.z_history_TbForestriesOrderNegotiations (
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
CONSTRAINT PK_280f24d9c2dc26dc6faf703757c7dddfbb52c90f PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbQuarters (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flForestry integer NOT NULL,
flNumber character varying(300) ,
flStatus character varying(7) NOT NULL,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flArea numeric (18, 2) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbQuarters PRIMARY KEY (
    flId
));

CREATE UNIQUE INDEX IX_TbQuarters_flId ON forest.TbQuarters USING btree (flId);
CREATE UNIQUE INDEX IX_TbQuarters_flRevisionId ON forest.TbQuarters USING btree (flRevisionId);

CREATE TABLE forest.z_history_TbQuarters (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flForestry integer NOT NULL,
flNumber character varying(300) ,
flStatus character varying(7) NOT NULL,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flArea numeric (18, 2) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_022a51cc0adabd87e79b7330894a3036d83f6fce PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbQuartersRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flForestry integer NOT NULL,
flNumber character varying(300) ,
flStatus character varying(7) NOT NULL,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flArea numeric (18, 2) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbQuartersRevisions PRIMARY KEY (
    flRevisionId
));

CREATE INDEX IX_TbQuartersRevisions_flId ON forest.TbQuartersRevisions USING btree (flId);
CREATE UNIQUE INDEX IX_TbQuartersRevisions_flRevisionId ON forest.TbQuartersRevisions USING btree (flRevisionId);

CREATE TABLE forest.z_history_TbQuartersRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flForestry integer NOT NULL,
flNumber character varying(300) ,
flStatus character varying(7) NOT NULL,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flArea numeric (18, 2) ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_58f254f3ad9b6a9b4ed8c32dad9ca125790cc989 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbQuartersOrderResult (
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
CONSTRAINT PK_TbQuartersOrderResult PRIMARY KEY (
    flSubjectId
));

CREATE UNIQUE INDEX IX_TbQuartersOrderResult ON forest.TbQuartersOrderResult USING btree (flSubjectId);

CREATE TABLE forest.z_history_TbQuartersOrderResult (
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
CONSTRAINT PK_10ac4a01c70f321aab6161375792f6f452b252c3 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbQuartersOrderNegotiations (
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

CREATE INDEX IX_TbQuartersOrderNegotiations ON forest.TbQuartersOrderNegotiations USING btree (flSubjectId);

CREATE TABLE forest.z_history_TbQuartersOrderNegotiations (
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
CONSTRAINT PK_b05e94d52ebfa34cb41600220c1e5f75ba3f2546 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestryPieces (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flQuarter integer NOT NULL,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (18, 2) ,
flForestFundCategory character varying(61) ,
flRestrictions character varying(42) ,
flFellingValue character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbForestryPieces PRIMARY KEY (
    flId
));

CREATE UNIQUE INDEX IX_TbForestryPieces_flId ON forest.TbForestryPieces USING btree (flId);
CREATE UNIQUE INDEX IX_TbForestryPieces_flRevisionId ON forest.TbForestryPieces USING btree (flRevisionId);

CREATE TABLE forest.z_history_TbForestryPieces (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flQuarter integer NOT NULL,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (18, 2) ,
flForestFundCategory character varying(61) ,
flRestrictions character varying(42) ,
flFellingValue character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_380bd7f4cd47cab91844358abe537b41ac5f90c5 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestryPiecesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flQuarter integer NOT NULL,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (18, 2) ,
flForestFundCategory character varying(61) ,
flRestrictions character varying(42) ,
flFellingValue character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_TbForestryPiecesRevisions PRIMARY KEY (
    flRevisionId
));

CREATE INDEX IX_TbForestryPiecesRevisions_flId ON forest.TbForestryPiecesRevisions USING btree (flId);
CREATE UNIQUE INDEX IX_TbForestryPiecesRevisions_flRevisionId ON forest.TbForestryPiecesRevisions USING btree (flRevisionId);

CREATE TABLE forest.z_history_TbForestryPiecesRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flRevisionId integer NOT NULL,
flQuarter integer NOT NULL,
flNumber character varying(300) ,
flDescription character varying ,
flNote character varying ,
flSellerBin character varying(12) ,
flStatus character varying(7) ,
flBlock character varying(20) ,
flArea numeric (18, 2) ,
flForestFundCategory character varying(61) ,
flRestrictions character varying(42) ,
flFellingValue character varying ,
flWKT character varying ,
flCoords character varying ,
flGeometry geometry(Geometry, 4326) ,
flPoint geometry(Point, 4326) ,
flAnotherInformation character varying , 
CONSTRAINT PK_18f68e20c10991803675de1d4a2295d66e5e5a88 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestryPiecesOrderResult (
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
CONSTRAINT PK_TbForestryPiecesOrderResult PRIMARY KEY (
    flSubjectId
));

CREATE UNIQUE INDEX IX_TbForestryPiecesOrderResult ON forest.TbForestryPiecesOrderResult USING btree (flSubjectId);

CREATE TABLE forest.z_history_TbForestryPiecesOrderResult (
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
CONSTRAINT PK_7202fd57642ee1e37a46b4deb0a720ae0289678a PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE TABLE forest.TbForestryPiecesOrderNegotiations (
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

CREATE INDEX IX_TbForestryPiecesOrderNegotiations ON forest.TbForestryPiecesOrderNegotiations USING btree (flSubjectId);

CREATE TABLE forest.z_history_TbForestryPiecesOrderNegotiations (
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
CONSTRAINT PK_46b589548ffb5c9e4d7ab9ee5dd56de8e79bd07c PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE forest."seq_TbForestries_flId";


CREATE SEQUENCE forest."seq_TbForestriesRevisions_flId";


CREATE SEQUENCE forest."seq_TbForestriesOrderResult_flSubjectId";


CREATE SEQUENCE forest."seq_TbForestriesOrderNegotiations_flActionId";


CREATE SEQUENCE forest."seq_TbQuarters_flId";


CREATE SEQUENCE forest."seq_TbQuartersRevisions_flId";


CREATE SEQUENCE forest."seq_TbQuartersOrderResult_flSubjectId";


CREATE SEQUENCE forest."seq_TbQuartersOrderNegotiations_flActionId";


CREATE SEQUENCE forest."seq_TbForestryPieces_flId";


CREATE SEQUENCE forest."seq_TbForestryPiecesRevisions_flId";


CREATE SEQUENCE forest."seq_TbForestryPiecesOrderResult_flSubjectId";


CREATE SEQUENCE forest."seq_TbForestryPiecesOrderNegotiations_flActionId";

