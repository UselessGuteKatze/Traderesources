
CREATE TABLE forest.TbSellerCreators (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flSellerBin character varying(12) NOT NULL,
flCreatorBin character varying(12) NOT NULL);


CREATE TABLE forest.z_history_TbSellerCreators (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flSellerBin character varying(12) NOT NULL,
flCreatorBin character varying(12) NOT NULL, 
CONSTRAINT PK_3b6b87eb71bb1bbe7552b474c4b9697ea8830115 PRIMARY KEY (
    flEntityId, flRequestId
));


CREATE SEQUENCE forest."seq_TbSellerCreators_flId";


ALTER TABLE customconfig.tbfishingobjectscustomroles 
RENAME TO __rm__tbfishingobjectscustomroles;


ALTER TABLE customconfig.tbforestobjectscustomroles 
RENAME TO __rm__tbforestobjectscustomroles;


ALTER TABLE customconfig.tbhuntingobjectscustomroles 
RENAME TO __rm__tbhuntingobjectscustomroles;


ALTER TABLE customconfig.tbhydrocarboncustomroles 
RENAME TO __rm__tbhydrocarboncustomroles;


ALTER TABLE customconfig.tblandobjectscustomroles 
RENAME TO __rm__tblandobjectscustomroles;


ALTER TABLE customconfig.tbtraderesourcescustomroles 
RENAME TO __rm__tbtraderesourcescustomroles;


ALTER TABLE customconfig.z_history_tbfishingobjectscustomroles 
RENAME TO __rm__z_history_tbfishingobjectscustomroles;


ALTER TABLE customconfig.z_history_tbforestobjectscustomroles 
RENAME TO __rm__z_history_tbforestobjectscustomroles;


ALTER TABLE customconfig.z_history_tbhuntingobjectscustomroles 
RENAME TO __rm__z_history_tbhuntingobjectscustomroles;


ALTER TABLE customconfig.z_history_tbhydrocarboncustomroles 
RENAME TO __rm__z_history_tbhydrocarboncustomroles;


ALTER TABLE customconfig.z_history_tblandobjectscustomroles 
RENAME TO __rm__z_history_tblandobjectscustomroles;


ALTER TABLE customconfig.z_history_tbtraderesourcescustomroles 
RENAME TO __rm__z_history_tbtraderesourcescustomroles;


ALTER SEQUENCE customconfig."seq_tbhydrocarbonCustomRoles_flId" 
RENAME TO "__rm__seq_tbhydrocarbonCustomRoles_flId";


ALTER SEQUENCE customconfig."seq_tbtraderesourcesCustomRoles_flId" 
RENAME TO "__rm__seq_tbtraderesourcesCustomRoles_flId";


ALTER SEQUENCE customconfig."seq_tblandobjectsCustomRoles_flId" 
RENAME TO "__rm__seq_tblandobjectsCustomRoles_flId";


ALTER SEQUENCE customconfig."seq_tbhuntingobjectsCustomRoles_flId" 
RENAME TO "__rm__seq_tbhuntingobjectsCustomRoles_flId";


ALTER SEQUENCE customconfig."seq_tbfishingobjectsCustomRoles_flId" 
RENAME TO "__rm__seq_tbfishingobjectsCustomRoles_flId";


ALTER SEQUENCE customconfig."seq_tbforestobjectsCustomRoles_flId" 
RENAME TO "__rm__seq_tbforestobjectsCustomRoles_flId";