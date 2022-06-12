
CREATE TABLE trades.TbOfficialOrg (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flOrgId integer NOT NULL,
flRevisionId integer NOT NULL,
flBin character varying(12) ,
flNameRu character varying(1024) ,
flNameKz character varying(1024) ,
flRegistrationDate date ,
flRegistrationNum character varying(50) ,
flOpf character varying(3) ,
flState character varying(3) ,
flBlock character varying(6) ,
flNote character varying ,
flFirstPerson character varying(256) ,
flFirstPersonIin character varying(12) ,
flAccountant character varying(256) ,
flAccountantIin character varying(12) ,
flAdrCountry character varying(3) ,
flAdrIndex character varying(6) ,
flAdrRka character varying(20) ,
flAdrObl character varying(5) ,
flAdrReg character varying(8) ,
flAdrAdr character varying(1024) ,
flAdrMobile character varying(128) ,
flAdrPhone character varying(128) ,
flAdrFax character varying(128) ,
flAdrMail character varying(128) ,
flAdrWeb character varying(128) ,
flAdrNote character varying(1024) ,
flActivityTypes character varying , 

CONSTRAINT PK_TbOfficialOrg PRIMARY KEY (
    flOrgId
)

);




CREATE TABLE trades.z_history_TbOfficialOrg (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flOrgId integer NOT NULL,
flRevisionId integer NOT NULL,
flBin character varying(12) ,
flNameRu character varying(1024) ,
flNameKz character varying(1024) ,
flRegistrationDate date ,
flRegistrationNum character varying(50) ,
flOpf character varying(3) ,
flState character varying(3) ,
flBlock character varying(6) ,
flNote character varying ,
flFirstPerson character varying(256) ,
flFirstPersonIin character varying(12) ,
flAccountant character varying(256) ,
flAccountantIin character varying(12) ,
flAdrCountry character varying(3) ,
flAdrIndex character varying(6) ,
flAdrRka character varying(20) ,
flAdrObl character varying(5) ,
flAdrReg character varying(8) ,
flAdrAdr character varying(1024) ,
flAdrMobile character varying(128) ,
flAdrPhone character varying(128) ,
flAdrFax character varying(128) ,
flAdrMail character varying(128) ,
flAdrWeb character varying(128) ,
flAdrNote character varying(1024) ,
flActivityTypes character varying , 

CONSTRAINT PK_60251a3fd50148657db4ceb2b8715b8b11834f77 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbOfficialOrgRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flOrgId integer NOT NULL,
flRevisionId integer NOT NULL,
flBin character varying(12) ,
flNameRu character varying(1024) ,
flNameKz character varying(1024) ,
flRegistrationDate date ,
flRegistrationNum character varying(50) ,
flOpf character varying(3) ,
flState character varying(3) ,
flBlock character varying(6) ,
flNote character varying ,
flFirstPerson character varying(256) ,
flFirstPersonIin character varying(12) ,
flAccountant character varying(256) ,
flAccountantIin character varying(12) ,
flAdrCountry character varying(3) ,
flAdrIndex character varying(6) ,
flAdrRka character varying(20) ,
flAdrObl character varying(5) ,
flAdrReg character varying(8) ,
flAdrAdr character varying(1024) ,
flAdrMobile character varying(128) ,
flAdrPhone character varying(128) ,
flAdrFax character varying(128) ,
flAdrMail character varying(128) ,
flAdrWeb character varying(128) ,
flAdrNote character varying(1024) ,
flActivityTypes character varying , 

CONSTRAINT PK_TbOfficialOrgRevisions PRIMARY KEY (
    flRevisionId
)

);




CREATE TABLE trades.z_history_TbOfficialOrgRevisions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flOrgId integer NOT NULL,
flRevisionId integer NOT NULL,
flBin character varying(12) ,
flNameRu character varying(1024) ,
flNameKz character varying(1024) ,
flRegistrationDate date ,
flRegistrationNum character varying(50) ,
flOpf character varying(3) ,
flState character varying(3) ,
flBlock character varying(6) ,
flNote character varying ,
flFirstPerson character varying(256) ,
flFirstPersonIin character varying(12) ,
flAccountant character varying(256) ,
flAccountantIin character varying(12) ,
flAdrCountry character varying(3) ,
flAdrIndex character varying(6) ,
flAdrRka character varying(20) ,
flAdrObl character varying(5) ,
flAdrReg character varying(8) ,
flAdrAdr character varying(1024) ,
flAdrMobile character varying(128) ,
flAdrPhone character varying(128) ,
flAdrFax character varying(128) ,
flAdrMail character varying(128) ,
flAdrWeb character varying(128) ,
flAdrNote character varying(1024) ,
flActivityTypes character varying , 

CONSTRAINT PK_276f6fb2c92783725b768f206a2aa67e78efa561 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbOfficialOrgOrderResult (
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
flType character varying(10) , 

CONSTRAINT PK_TbOfficialOrgOrderResult PRIMARY KEY (
    flSubjectId
)

);




CREATE TABLE trades.z_history_TbOfficialOrgOrderResult (
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
flType character varying(10) , 

CONSTRAINT PK_f3472a8faa0590a8d20be7bbec92a8e8b45ac44a PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbOfficialOrgOrderNegotiations (
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




CREATE TABLE trades.z_history_TbOfficialOrgOrderNegotiations (
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

CONSTRAINT PK_65757f4d932f6b2ccfc3e9305451b2fe19d2cb73 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE trades.TbOfficialOrgActivityTypes (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flOfficialOrgId integer NOT NULL,
flActivityType character varying(20) NOT NULL,
flFrom date ,
flTo date , 

CONSTRAINT PK_TbOfficialOrgActivityTypes PRIMARY KEY (
    flOfficialOrgId, flActivityType
)

);




CREATE TABLE trades.z_history_TbOfficialOrgActivityTypes (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flOfficialOrgId integer NOT NULL,
flActivityType character varying(20) NOT NULL,
flFrom date ,
flTo date , 

CONSTRAINT PK_fd81834deefc23277ebbe0097713b5b6c8be4ff2 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE trades."seq_TbOfficialOrg_flOrgId";


CREATE SEQUENCE trades."seq_TbOfficialOrgOrderResult_flSubjectId";


CREATE SEQUENCE trades."seq_TbOfficialOrgOrderNegotiations_flActionId";

