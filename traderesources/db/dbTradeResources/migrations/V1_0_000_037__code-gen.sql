
ALTER TABLE trades.tblandobjects
ADD COLUMN flApplicationId integer ;


ALTER TABLE trades.tblandobjects
ADD COLUMN flTaxSum numeric (18, 2) ;


ALTER TABLE trades.tblandobjects
ADD COLUMN flRentSum numeric (18, 2) ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flApplicationId integer ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flTaxSum numeric (18, 2) ;


ALTER TABLE trades.tblandobjectsrevisions
ADD COLUMN flRentSum numeric (18, 2) ;



ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flApplicationId integer ;


ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flTaxSum numeric (18, 2) ;


ALTER TABLE trades.z_history_tblandobjectsrevisions
ADD COLUMN flRentSum numeric (18, 2) ;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flApplicationId integer ;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flTaxSum numeric (18, 2) ;


ALTER TABLE trades.z_history_tblandobjects
ADD COLUMN flRentSum numeric (18, 2) ;


CREATE TABLE trades.TbLandApplications (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flLandObjectId integer ,
flStatus character varying(24) ,
flCompetentOrgBin character varying(12) NOT NULL,
flRegDate timestamp without time zone ,
flSignSendDate timestamp without time zone ,
flAcceptDate timestamp without time zone ,
flRejectDate timestamp without time zone ,
flRejectReason character varying ,
flRegByUserId integer ,
flApplicantType character varying(12) ,
flApplicantXin character varying ,
flApplicantName character varying ,
flApplicantAddress character varying ,
flApplicantPhoneNumber character varying ,
flApplicantInfo character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flWKT character varying ,
flCoords character varying ,
flUsageAim character varying(2) ,
flDataToSign character varying NOT NULL,
flCertInfo character varying NOT NULL,
flSignedData character varying NOT NULL,
flPdf bytea NOT NULL, 

CONSTRAINT PK_TbLandApplications PRIMARY KEY (
    flId
)

);




CREATE TABLE trades.z_history_TbLandApplications (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flLandObjectId integer ,
flStatus character varying(24) ,
flCompetentOrgBin character varying(12) NOT NULL,
flRegDate timestamp without time zone ,
flSignSendDate timestamp without time zone ,
flAcceptDate timestamp without time zone ,
flRejectDate timestamp without time zone ,
flRejectReason character varying ,
flRegByUserId integer ,
flApplicantType character varying(12) ,
flApplicantXin character varying ,
flApplicantName character varying ,
flApplicantAddress character varying ,
flApplicantPhoneNumber character varying ,
flApplicantInfo character varying ,
flCountry character varying(10) ,
flRegion character varying(10) ,
flDistrict character varying(10) ,
flAddress character varying ,
flWKT character varying ,
flCoords character varying ,
flUsageAim character varying(2) ,
flDataToSign character varying NOT NULL,
flCertInfo character varying NOT NULL,
flSignedData character varying NOT NULL,
flPdf bytea NOT NULL, 

CONSTRAINT PK_5dbc1a7e2fdbce1a8ddf9c95d3b897f8598a75a8 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE trades."seq_TbLandApplications_flId";