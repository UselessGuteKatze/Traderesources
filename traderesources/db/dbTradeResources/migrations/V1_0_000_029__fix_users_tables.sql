ALTER TABLE users.TbAccountModel ADD CONSTRAINT ch_tbaccountmodel_opt_lock CHECK (flupdateversion = (flprevupdateversion + 1));

ALTER TABLE users.tbaccounts
ADD COLUMN flBlockDate timestamp without time zone ;

ALTER TABLE users.z_history_tbaccounts
ADD COLUMN flBlockDate timestamp without time zone ;

ALTER TABLE users.tbuserinvites
ALTER COLUMN flinvitecode TYPE character varying(20);

ALTER TABLE users.TbUserInvites DROP CONSTRAINT "pk_tbuserinvites"; 
ALTER TABLE users.TbUserInvites ADD CONSTRAINT PK_TbUserInvites PRIMARY KEY (flIin, flInviteCode);
ALTER TABLE users.TbUserModel ADD CONSTRAINT ch_tbusermodel_opt_lock CHECK (flupdateversion = (flprevupdateversion + 1));
ALTER TABLE users.TbUserRoles DROP CONSTRAINT "pk_aa3d862d233176a003b1da6bdb664b7aa3ec3d9f";



ALTER TABLE users.z_history_TbUsers DROP CONSTRAINT "pk_b4fdf04a65ff7665bcc586ecd8e66c3b400b301a";
ALTER TABLE users.z_history_TbUsers ADD CONSTRAINT PK_55cae888b551506bbf981472b86545e1b80994d9 PRIMARY KEY (flEntityId, flRequestId);
ALTER TABLE users.z_history_TbAccountBankAccount DROP CONSTRAINT "pk_512b1d6624ea31d447c9da685e0c39c248b78b12";
ALTER TABLE users.z_history_TbAccountBankAccount ADD CONSTRAINT PK_dff2f1fa1e57ffab24ca85bef74b744d80c3a446 PRIMARY KEY (flEntityId, flRequestId);

ALTER TABLE users.z_history_tbuserinvites
ALTER COLUMN flinvitecode TYPE character varying(20);







ALTER TABLE users.tbcorprelatedjobs
ADD COLUMN flLastSuccessDateTime timestamp without time zone ;


ALTER TABLE users.tbcorprelatedjobs
ADD COLUMN flGbdUlModelInJson character varying ;

ALTER TABLE users.TbDataAccessSettings DROP CONSTRAINT "pk_ca50217467154e3f68d5320b82e46f3156e37211";

ALTER TABLE users.tbdataaccesssettingsimport 
RENAME TO __rm__tbdataaccesssettingsimport;

ALTER TABLE users.TbDataAccessSettingsToTablesMap DROP CONSTRAINT "pk_3591b9d86c7cd5980c5a4c6b9767632ede0eec34";
ALTER TABLE users.tbRolePermissions DROP CONSTRAINT "pk_ef4439bc04599ea8328d983a467b40b1a8ff1b33";
ALTER TABLE users.TbRoles DROP CONSTRAINT "pk_5c78aa39092f2dc2d3ae188efab556876374fc45";
ALTER TABLE users.TbRoles ADD CONSTRAINT PK_TbRoles PRIMARY KEY (flRoleId);

ALTER TABLE users.tbuserdocs
ALTER COLUMN fldoctype TYPE character varying(9);


ALTER TABLE users.tbuserrelatedjobs
ADD COLUMN flLastSuccessDateTime timestamp without time zone ;


ALTER TABLE users.tbuserrelatedjobs
ADD COLUMN flGbdFlModelInJson character varying ;


ALTER TABLE users.z_history_tbuserrelatedjobs
ADD COLUMN flLastSuccessDateTime timestamp without time zone ;


ALTER TABLE users.z_history_tbuserrelatedjobs
ADD COLUMN flGbdFlModelInJson character varying ;


ALTER TABLE users.z_history_tbuserdocs
ALTER COLUMN fldoctype TYPE character varying(9);


ALTER TABLE users.z_history_tbcorprelatedjobs
ADD COLUMN flLastSuccessDateTime timestamp without time zone ;


ALTER TABLE users.z_history_tbcorprelatedjobs
ADD COLUMN flGbdUlModelInJson character varying ;


CREATE TABLE users.TbUsersAuthorizationHistory (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flLogin character varying NOT NULL,
flRemoteAddr character varying NOT NULL,
flAuthorizationDate timestamp without time zone NOT NULL,
flSuccess boolean NOT NULL,
flReason character varying 
);




CREATE TABLE users.z_history_TbUsersAuthorizationHistory (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flLogin character varying NOT NULL,
flRemoteAddr character varying NOT NULL,
flAuthorizationDate timestamp without time zone NOT NULL,
flSuccess boolean NOT NULL,
flReason character varying , 

CONSTRAINT PK_02c19830b05321effaf7e2d36c387c1b4bf15374 PRIMARY KEY (
    flEntityId, flRequestId
)

);
