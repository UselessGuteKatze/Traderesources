------------ user email update -----------------

update users.tbusers set flemail='-' where flemail  is null;

------------ drop some not nulls -----------------

ALTER TABLE users.tbusercorpdata
ALTER COLUMN flfirstpersoniin DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN flfirstpersoniin DROP NOT NULL;

ALTER TABLE users.tbusercorpdata
ALTER COLUMN flfirstpersonfio DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN flfirstpersonfio DROP NOT NULL;

ALTER TABLE users.tbusercorpdata
ALTER COLUMN fladrcountry DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN fladrcountry DROP NOT NULL;

ALTER TABLE users.tbusercorpdata
ALTER COLUMN fladrobl DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN fladrobl DROP NOT NULL;

ALTER TABLE users.tbusercorpdata
ALTER COLUMN fladrreg DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN fladrreg DROP NOT NULL;

ALTER TABLE users.tbusercorpdata
ALTER COLUMN fladradr DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN fladradr DROP NOT NULL;

ALTER TABLE users.tbusercorpdata
ALTER COLUMN flismobilephoneactivated DROP NOT NULL;

ALTER TABLE users.z_history_tbusercorpdata
ALTER COLUMN flismobilephoneactivated DROP NOT NULL;


------ fix corp data missing rows-------------------
select 
    nextval('public.seq_entity_id') as flEntityId,
	nextval('public.seq_request_id') as flRequestId,
	false as flIsHidden,
	0 as flRequestType,
	now()::timestamp without time zone as flRequestDate,
	-1 as flRequestUserId,	
	tbExtProfile.flbin,
	tbExtNames.flcorpname,
	null::character varying as flfirstpersoniin,
	tbExtProfile.flfirstpersonfio,
	tbExtProfile.flAdrCountry,
	''::character varying as flAdrIndex,
	tbExtProfile.flAdrObl,
	tbExtProfile.flAdrReg,
	tbExtProfile.flAdrAdr,
	tbUsers.flWorkPhone,
	tbUsers.flMobilePhone,
	tbUsers.flIsMobilePhoneConfirmed,
	tbUsers.flEmail,
	true as flisemailactivated,
	row_number () over (partition by tbExtProfile.flbin order by tbExtProfile.flrequestdate desc) as flRowNumber
into users.tbusercorpdata_missing
from users.tbusers tbUsers
left join users.tbusersexternalprofiledata tbExtProfile on tbusers.fluserid = tbExtProfile.fluserid
left join users.tbusersexternalprofiledisplaynames tbExtNames on tbusers.fluserid = tbExtNames.fluserid
where tbExtProfile.flBin is not null and flBin in (
	select t.flBin from users.tbusersexternalprofiledata t 
	left join users.tbusercorpdata t2 on t.flbin  = t2.flbin 
	where t2.flbin  is null and t.flbin is not null
);
insert into users.tbusercorpdata (
	flEntityId, flRequestId, flIsHidden, flRequestType, flRequestDate, flRequestUserId, flbin, flname, flfirstpersoniin, flfirstpersonfio, flAdrCountry,
	flAdrIndex, flAdrObl, flAdrReg, flAdrAdr, flWorkPhone, flMobilePhone, flismobilephoneactivated, flEmail, flisemailactivated
)
select 
	flEntityId, flRequestId, flIsHidden, flRequestType, flRequestDate, flRequestUserId, flbin, flcorpname, flfirstpersoniin, flfirstpersonfio, flAdrCountry,
	flAdrIndex, flAdrObl, flAdrReg, flAdrAdr, flWorkPhone, flMobilePhone, flIsMobilePhoneConfirmed, flEmail, flisemailactivated
from users.tbusercorpdata_missing where flRowNumber = 1;

insert into users.z_history_tbusercorpdata (
	flEntityId, flRequestId, flIsHidden, flRequestType, flRequestDate, flRequestUserId, flbin, flname, flfirstpersoniin, flfirstpersonfio, flAdrCountry,
	flAdrIndex, flAdrObl, flAdrReg, flAdrAdr, flWorkPhone, flMobilePhone, flismobilephoneactivated, flEmail, flisemailactivated
)
select flEntityId, flRequestId, flIsHidden, flRequestType, flRequestDate, flRequestUserId, flbin, flcorpname, flfirstpersoniin, flfirstpersonfio, flAdrCountry,
	flAdrIndex, flAdrObl, flAdrReg, flAdrAdr, flWorkPhone, flMobilePhone, flIsMobilePhoneConfirmed, flEmail, flisemailactivated
from users.tbusercorpdata_missing where flRowNumber = 1;


--------

update users.tbusercorpdata t set 
flfirstpersonfio = ' ', 
flfirstpersoniin = ' ',
flworkphone = ' ',
flmobilephone = ' ',
flemail = ' '

where flbin in (
'070740008697',
'190840900012',
'200740008785');


----------------------------------

CREATE TABLE users.TbUsersPasswordHistory (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flPassword character varying NOT NULL,
flChangeDate timestamp without time zone NOT NULL
);




CREATE TABLE users.z_history_TbUsersPasswordHistory (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flPassword character varying NOT NULL,
flChangeDate timestamp without time zone NOT NULL, 

CONSTRAINT PK_67ac3da686117d5c9eaa71adb0f7592a8ac9e80d PRIMARY KEY (
    flEntityId, flRequestId
)
);

CREATE TABLE users.TbTermsAndConditions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flVersion integer ,
flTermsAndConditions character varying , 

CONSTRAINT PK_tbTermsAndConditions_flVersion PRIMARY KEY (
    flVersion
)

);


CREATE TABLE users.z_history_TbTermsAndConditions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flVersion integer ,
flTermsAndConditions character varying , 

CONSTRAINT PK_4996b12709df1e02e3bdf46aeddbb987909cc148 PRIMARY KEY (
    flEntityId, flRequestId
)

);



CREATE TABLE users.TbUserTermsAndConditions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flVersion integer ,
flTermsAndConditionsToSign character varying ,
flSignedData character varying ,
flCertInfo character varying ,
flSignDate timestamp without time zone , 

CONSTRAINT PK_tbUserTermsAndConditions PRIMARY KEY (
    flUserId, flVersion
)

);




CREATE TABLE users.z_history_TbUserTermsAndConditions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flVersion integer ,
flTermsAndConditionsToSign character varying ,
flSignedData character varying ,
flCertInfo character varying ,
flSignDate timestamp without time zone , 

CONSTRAINT PK_650aacc089c30d6a8456c11418b24e75d9ad80b7 PRIMARY KEY (
    flEntityId, flRequestId
)

);


CREATE TABLE users.TbCorpRelatedJobs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flBin character varying(15) NOT NULL,
flGbdUlSyncJobId character varying(36) , 

CONSTRAINT PK_TbCorpRelatedJobs PRIMARY KEY (
    flBin
)

);




CREATE TABLE users.z_history_TbCorpRelatedJobs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flBin character varying(15) NOT NULL,
flGbdUlSyncJobId character varying(36) , 

CONSTRAINT PK_2db96d844aea9486dc166f0cd3a5d3e1bfcf6005 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUserRelatedJobs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flGbdFlSyncJobId character varying(36) , 

CONSTRAINT PK_TbUserRelatedJobs PRIMARY KEY (
    flUserId
)

);




CREATE TABLE users.z_history_TbUserRelatedJobs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flGbdFlSyncJobId character varying(36) , 

CONSTRAINT PK_afb4413e13519970763ad7b085afba755284f0e0 PRIMARY KEY (
    flEntityId, flRequestId
)

);





-- FIX
ALTER TABLE users.tbcorprelatedjobs
ALTER COLUMN flbin TYPE character varying(12);


ALTER TABLE users.TbUserRelatedJobs DROP CONSTRAINT "pk_tbuserrelatedjobs";
ALTER TABLE users.tbuserrelatedjobs 
DROP COLUMN fluserid;


ALTER TABLE users.tbuserrelatedjobs
ADD COLUMN flIin character varying(12) NOT NULL;


ALTER TABLE users.TbUserRelatedJobs ADD CONSTRAINT PK_TbUserRelatedJobs PRIMARY KEY (flIin);


ALTER TABLE users.z_history_tbcorprelatedjobs
ALTER COLUMN flbin TYPE character varying(12);


ALTER TABLE users.z_history_tbuserrelatedjobs 
DROP COLUMN fluserid;


ALTER TABLE users.z_history_tbuserrelatedjobs
ADD COLUMN flIin character varying(12) NOT NULL;

-- tbusers 

ALTER TABLE users.tbusers
RENAME CONSTRAINT pk_tbusers_fluserid TO __rm__pk_tbusers_fluserid;

ALTER TABLE users.z_history_tbusers
RENAME CONSTRAINT pk_55cae888b551506bbf981472b86545e1b80994d9 TO __rm__pk_55cae888b551506bbf981472b86545e1b80994d9;




ALTER TABLE users.tbusers 
RENAME TO __RM__tbusers;


ALTER TABLE users.z_history_tbusers 
RENAME TO __RM__z_history_tbusers ;



---------------------------------------------------

-- tbusersexternalprofiledata

ALTER TABLE users.tbusersexternalprofiledata
RENAME CONSTRAINT pk_tbusersexternalprofiledata TO __rm__pk_tbusersexternalprofiledata;

ALTER TABLE users.z_history_tbusersexternalprofiledata
RENAME CONSTRAINT pk_e9c0c8700bf030cd18661358e61ff519001fd647 TO __rm__pk_e9c0c8700bf030cd18661358e61ff519001fd647;


ALTER TABLE users.tbusersexternalprofiledata
RENAME TO __rm__tbusersexternalprofiledata;


ALTER TABLE users.z_history_tbusersexternalprofiledata
RENAME TO __rm__z_history_tbusersexternalprofiledata;



--------------------------------------------------

-- tbusersexternalprofiledisplaynames

ALTER TABLE users.tbusersexternalprofiledisplaynames
RENAME CONSTRAINT pk_tbusersexternalprofiledisplaynames TO __rm__pk_tbusersexternalprofiledisplaynames;

ALTER TABLE users.z_history_tbusersexternalprofiledisplaynames
RENAME CONSTRAINT pk_da92c512f8ecb44528c628fefd7447b96eb09fcb TO __rm__pk_da92c512f8ecb44528c628fefd7447b96eb09fcb;


ALTER TABLE users.tbusersexternalprofiledisplaynames
RENAME TO __rm__tbusersexternalprofiledisplaynames;

ALTER TABLE users.z_history_tbusersexternalprofiledisplaynames
RENAME TO __rm__z_history_tbusersexternalprofiledisplaynames;



------------------------------------------

-- tbusersinternalprofiledata


ALTER TABLE users.tbusersinternalprofiledata
RENAME CONSTRAINT pk_tbusersinternalprofiledata TO __rn__pk_tbusersinternalprofiledata;

ALTER TABLE users.z_history_tbusersinternalprofiledata
RENAME CONSTRAINT pk_3dda3b7a30c420bcf09c11537bc7b0c1ab80baa4 TO __rm__pk_3dda3b7a30c420bcf09c11537bc7b0c1ab80baa4;



ALTER TABLE users.tbusersinternalprofiledata
RENAME TO __rm__tbusersinternalprofiledata;

ALTER TABLE users.z_history_tbusersinternalprofiledata
RENAME TO __rm__z_history_tbusersinternalprofiledata;




----------------------------------------------------------------------



-- tbusercorpdata

ALTER TABLE users.tbusercorpdata
RENAME CONSTRAINT pk_tbusercorpdata_flbin TO __rm__pk_tbusercorpdata_flbin;

ALTER TABLE users.z_history_tbusercorpdata
RENAME CONSTRAINT pk_a591bd9ddeec2099c587b574a7322a0c71b2a656 TO __rm__pk_a591bd9ddeec2099c587b574a7322a0c71b2a656;


ALTER TABLE users.tbusercorpdata
RENAME TO __rm__tbusercorpdata;


ALTER TABLE users.z_history_tbusercorpdata
RENAME TO __rm__z_history_tbusercorpdata;


----------------------------------------------------------------------------------------------------------

ALTER TABLE users.tbuserbankdetails 
RENAME TO __rm__tbuserbankdetails;


ALTER TABLE users.z_history_tbuserbankdetails 
RENAME TO __rm__z_history_tbuserbankdetails;


ALTER SEQUENCE users."seq_TbUserBankDetails_flId" 
RENAME TO "__rm__seq_TbUserBankDetails_flId";




----------------------------------

CREATE TABLE users.tbUsers (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flLogin character varying(25) NOT NULL,
flAccountId character varying(15) NOT NULL,
flAccountType character varying(14) NOT NULL,
flXin character varying(12) NOT NULL,
flIin character varying(12) NOT NULL,
flGroupName character varying NOT NULL,
flFirstName character varying NOT NULL,
flLastName character varying NOT NULL,
flMiddleName character varying,
flFullName character varying NOT NULL,
flEmail character varying NOT NULL,
flMobilePhone character varying ,
flIsMobilePhoneConfirmed boolean NOT NULL,
flStationaryPhone character varying ,
flPassword character varying NOT NULL,
flIsBlocked boolean NOT NULL,
flBlockReason character varying ,
flAutoBlockAfter date , 

CONSTRAINT PK_tbUsers PRIMARY KEY (
    flUserId
)

);

CREATE UNIQUE INDEX IX_tbUsers_Login_Unique ON users.tbUsers USING btree (flLogin);
CREATE UNIQUE INDEX IX_tbUsers_AccountId_Iin ON users.tbUsers USING btree (flAccountId, flIin);


CREATE TABLE users.z_history_tbUsers (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flLogin character varying(25) NOT NULL,
flAccountId character varying(15) NOT NULL,
flAccountType character varying(14) NOT NULL,
flXin character varying(12) NOT NULL,
flIin character varying(12) NOT NULL,
flGroupName character varying NOT NULL,
flFirstName character varying NOT NULL,
flLastName character varying NOT NULL,
flMiddleName character varying,
flFullName character varying NOT NULL,
flEmail character varying NOT NULL,
flMobilePhone character varying ,
flIsMobilePhoneConfirmed boolean NOT NULL,
flStationaryPhone character varying,
flPassword character varying NOT NULL,
flIsBlocked boolean NOT NULL,
flBlockReason character varying ,
flAutoBlockAfter date , 

CONSTRAINT PK_b4fdf04a65ff7665bcc586ecd8e66c3b400b301a PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbAccounts (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flAccountType character varying(14) NOT NULL,
flXin character varying(12) NOT NULL,
flNameRu character varying NOT NULL,
flNameKz character varying NOT NULL,
flIsBlocked boolean NOT NULL,
flBlockReason character varying , 

CONSTRAINT PK_TbAccounts PRIMARY KEY (
    flAccountId
)

);

CREATE UNIQUE INDEX IX_TbAccounts_AccountType_Xin ON users.TbAccounts USING btree (flAccountType, flXin);


CREATE TABLE users.z_history_TbAccounts (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flAccountType character varying(14) NOT NULL,
flXin character varying(12) NOT NULL,
flNameRu character varying NOT NULL,
flNameKz character varying NOT NULL,
flIsBlocked boolean NOT NULL,
flBlockReason character varying , 

CONSTRAINT PK_e21c7bebd191d8d7cdfab81e3f510935fe7fd454 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbAccountAddr (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flAddrType character varying(5) NOT NULL,
flAdrCountry character varying(10) NOT NULL,
flAdrIndex character varying,
flAdrObl character varying(10) NOT NULL,
flAdrReg character varying(10) NOT NULL,
flAdrAdr character varying NOT NULL,
flRkaCode character varying , 

CONSTRAINT PK_TbAccountAddr PRIMARY KEY (
    flAccountId, flAddrType
)

);




CREATE TABLE users.z_history_TbAccountAddr (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flAddrType character varying(5) NOT NULL,
flAdrCountry character varying(10) NOT NULL,
flAdrIndex character varying,
flAdrObl character varying(10) NOT NULL,
flAdrReg character varying(10) NOT NULL,
flAdrAdr character varying NOT NULL,
flRkaCode character varying , 

CONSTRAINT PK_767b0448951275f516465d879275ec5b1d900646 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbCorpData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flBin character varying(15) NOT NULL,
flFirstPersonFio character varying ,
flFirstPersonIin character varying(12) ,
flCorpEmail character varying NOT NULL,
flCorpMobilePhone character varying ,
flCorpStationaryPhone character varying , 

CONSTRAINT PK_TbCorpData PRIMARY KEY (
    flAccountId
)

);

CREATE UNIQUE INDEX IX_TbCorpData_flBin ON users.TbCorpData USING btree (flBin);


CREATE TABLE users.z_history_TbCorpData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flBin character varying(15) NOT NULL,
flFirstPersonFio character varying ,
flFirstPersonIin character varying(12) ,
flCorpEmail character varying NOT NULL,
flCorpMobilePhone character varying ,
flCorpStationaryPhone character varying , 

CONSTRAINT PK_d4907471abe0ca99aeb79792783a63ea0c2dc01f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUserInvites (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flInviteAccountId character varying NOT NULL,
flIin character varying(12) NOT NULL,
flEmail character varying NOT NULL,
flGroupName character varying NOT NULL,
flInviteExpiresAt date NOT NULL,
flAutoBlockAfter date NOT NULL,
flInviteCode character varying NOT NULL,
flUsed boolean NOT NULL, 

CONSTRAINT PK_TbUserInvites PRIMARY KEY (
    flIin
)

);




CREATE TABLE users.z_history_TbUserInvites (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flInviteAccountId character varying NOT NULL,
flIin character varying(12) NOT NULL,
flEmail character varying NOT NULL,
flGroupName character varying NOT NULL,
flInviteExpiresAt date NOT NULL,
flAutoBlockAfter date NOT NULL,
flInviteCode character varying NOT NULL,
flUsed boolean NOT NULL, 

CONSTRAINT PK_89ef2ca80d77d2123ddfee1e2afd2f5aad194cf3 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUserDocs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flGivenBy character varying NOT NULL,
flNumber character varying NOT NULL,
flDate date NOT NULL,
flDocType character varying(8) NOT NULL, 

CONSTRAINT PK_TbUserDocs PRIMARY KEY (
    flUserId
)

);




CREATE TABLE users.z_history_TbUserDocs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flGivenBy character varying NOT NULL,
flNumber character varying NOT NULL,
flDate date NOT NULL,
flDocType character varying(8) NOT NULL, 

CONSTRAINT PK_8bfefeb709d0412b74d8b127aaba16208f4709bd PRIMARY KEY (
    flEntityId, flRequestId
)

);










--------------------------------------------------
with tbUserFullName as (
    select 
        flUserId,
        coalesce(trim(tbusers.flLastName) || ' ', '') 
            || coalesce(trim(tbusers.flFirstName) || ' ', '') 
            || coalesce(trim(tbusers.flmiddlename), '') 
            
            as flFullName
    from users.__RM__tbusers tbUsers
),
tbAggUserInfo as (
    select 
        tbUsers.flUserId as flUserId,
        case 
        	when tbUsers.flLogin = 'MGR050540004455-821119400862' then '050540004455@821119400862'
        	when tbUsers.flLogin = 'MGRФЛ-880317350498' then 'ФЛ@880317350498'
        	else tbUsers.flLogin 
        end as flLogin,
        case 
            when tbExtProfile.flUserType = 'Corporate' 
                then tbExtProfile.flbin 
            when tbUsers.flLogin = 'MGRФЛ-880317350498' then 'ФЛ@880317350498'
            else tbUsers.flLogin
        end as flAccountId,

        coalesce(tbExtProfile.flUserType, 'Internal') as flAccountType,
        coalesce(tbExtProfile.flBin, tbUsers.flIin) as flXin,
        tbUsers.flIin as flIin,
        case 
        	when tbUsers.flGroupName is not null then tbUsers.flGroupName
        	when tbExtProfile.flUserType = 'Corporate' then 'БИН:'||tbCorpData.flBin
        	when tbExtProfile.flUserType = 'Individual' then 'ФЛ'
			when tbExtProfile.flUserType = 'IndividualCorp' then 'ИП'
			else 'INTERNAL'
        end as flGroupName,
        tbusers.flfirstname,
        tbusers.flLastName,
        tbusers.flMiddleName,
        tbUserFullName.flFullName as flFullName,
        tbUsers.flEmail,
        tbUsers.flMobilePhone,
        tbUsers.flIsMobilePhoneConfirmed,
        tbUsers.flWorkPhone as flStationaryPhone,
        tbUsers.flPassword,
        tbUsers.flIsBlocked,
        null as flBlockReason,
        tbIntProfile.flactiveto as flAutoBlockAfter,
        case 
            when tbExtProfile.flUserType = 'Corporate' 
                then coalesce(tbCorpData.flname, tbExtNames.flcorpname)
			when tbExtProfile.flUserType = 'IndividualCorp'
				then tbExtNames.flcorpname
            else tbUserFullName.flFullName
        end as flNameRu,
        case 
            when tbExtProfile.flUserType = 'Corporate' 
                then coalesce(tbCorpData.flname, tbExtNames.flcorpname)
			when tbExtProfile.flUserType = 'IndividualCorp'
				then tbExtNames.flcorpname
            else tbUserFullName.flFullName
        end as flNameKz,
        case 
            when tbExtProfile.flUserType = 'Corporate'
                then false
            else tbUsers.flIsBlocked
        end as flAccountIsBlocked,
        null as flAccountBlockReason,
        case 
            when tbExtProfile.flUserType is null
                then null
            when tbExtProfile.flUserType IN ('Corporate', 'Individual')
                then 'Legal'
            else 
                'Fact'
        end as flAddrType,
        coalesce(tbCorpData.flAdrCountry, tbExtProfile.flAdrCountry) as flAdrCountry,
        tbCorpData.flAdrIndex as flAdrIndex,
        coalesce(tbCorpData.flAdrObl, tbExtProfile.flAdrObl) as flAdrObl,
        coalesce(tbCorpData.flAdrReg, tbExtProfile.flAdrReg) as flAdrReg,
        coalesce(tbCorpData.flAdrAdr, tbExtProfile.flAdrAdr) as flAdrAdr,
		null as flRkaCode,
        tbCorpData.flBin as flBin,
        tbCorpData.flFirstPersonFio,
        tbCorpData.flFirstPersonIin,
        tbCorpData.flEmail as flCorpEmail,
        tbCorpData.flMobilePhone as flCorpMobilePhone,
        tbCorpData.flWorkPhone as flCorpStationaryPhone,
        tbExtProfile.fldocgivenby as flDocGivenBy,
        tbExtProfile.flDocNum as flDocNumber,
        tbExtProfile.flDocDate as flDocDate,
        tbExtProfile.flDocType as flDocType,
		tbIntProfile.flInviteSended as flInviteSended,
		tbIntProfile.flInviteDate as flInviteDate,
		tbIntProfile.flInviteCode as flInviteCode,
		tbIntProfile.flInviteAccepted as flInviteAccepted
    from users.__RM__tbusers tbUsers
    inner join tbUserFullName tbUserFullName on tbusers.fluserid = tbUserFullName.fluserid
    left join users.__rm__tbusersinternalprofiledata tbIntProfile on tbusers.fluserid = tbIntProfile.fluserid
    left join users.__rm__tbusersexternalprofiledata tbExtProfile on tbusers.fluserid = tbExtProfile.fluserid
    left join users.__rm__tbusersexternalprofiledisplaynames tbExtNames on tbusers.fluserid = tbExtNames.fluserid
    left join users.__rm__tbusercorpdata tbCorpData on tbExtProfile.flBin = tbCorpData.flBin
    where tbUsers.fllogin not in ('MGR050540004455-821119400862','MGRФЛ-880317350498')
)
--SELECT * FROM tbAggUserInfo
SELECT * INTO users.tbOldFullInfo FROM tbAggUserInfo;

SELECT DISTINCT flAccountId, 
		flAccountType,
		flXin,
		flNameRu,
		flNameKz,
		flAccountIsBlocked,
		flAccountBlockReason
INTO users.tbUniqueAccounts 
FROM users.tbOldFullInfo;

INSERT INTO users.TbAccounts (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flAccountId,
	flAccountType,
	flXin,
	flNameRu,
	flNameKz,
	flIsBlocked,
	flBlockReason
)
SELECT
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	flAccountId, 
	flAccountType,
	flXin,
	flNameRu,
	coalesce(flNameKz, flNameRu),
	flAccountIsBlocked,
	flAccountBlockReason
FROM users.tbUniqueAccounts;

INSERT INTO users.tbUsers (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flUserId,
	flLogin,
	flAccountId,
	flAccountType,
	flXin,
	flIin,
	flGroupName,
	flFirstName,
	flLastName,
	flMiddleName,
	flFullName,
	flEmail,
	flMobilePhone,
	flIsMobilePhoneConfirmed,
	flStationaryPhone,
	flPassword,
	flIsBlocked,
	flBlockReason,
	flAutoBlockAfter
)
SELECT 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	flUserId,
	flLogin,
	flAccountId,
	flAccountType,
	flXin,
	flIin,
	coalesce(flGroupName, flXin),
	coalesce(flFirstName, ''),
	flLastName,
	flMiddleName,
	flFullName,
	flEmail,
	flMobilePhone,
	coalesce(flIsMobilePhoneConfirmed, false),
	flStationaryPhone,
	coalesce(flPassword, 'a0ee-bc99-9c0b-4ef8-bb6d-6bb9-bd38-0a11'),
	flIsBlocked,
	flBlockReason,
	flAutoBlockAfter
FROM users.tbOldFullInfo;
	
INSERT INTO users.TbAccountAddr (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flAccountId,
	flAddrType,
	flAdrCountry,
	flAdrIndex,
	flAdrObl,
	flAdrReg,
	flAdrAdr,
	flRkaCode
)
SELECT 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	flAccountId,
	flAddrType,
	flAdrCountry,
	flAdrIndex,
	flAdrObl,
	flAdrReg,
	flAdrAdr,
	flRkaCode
FROM (
	SELECT DISTINCT 
		flAccountId,
		flAddrType,
		flAdrCountry,
		coalesce(flAdrIndex, '') as flAdrIndex,
		flAdrObl,
		flAdrReg,
		flAdrAdr,
		flRkaCode
	FROM users.tbOldFullInfo where flAddrType IS NOT null and flAdrCountry is not null and flAdrObl not in ('01400')
) a;
	

INSERT INTO users.TbCorpData (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flAccountId,
	flBin,
	flFirstPersonFio,
	flFirstPersonIin,
	flCorpEmail,
	flCorpMobilePhone,
	flCorpStationaryPhone
)
SELECT 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,	

	flAccountId,
	flBin,
	flFirstPersonFio,
	flFirstPersonIin,
	flCorpEmail,
	flCorpMobilePhone,
	flCorpStationaryPhone
FROM (
	SELECT DISTINCT 
		flAccountId,
		flBin,
		flFirstPersonFio,
		flFirstPersonIin,
		flCorpEmail,
		flCorpMobilePhone,
		flCorpStationaryPhone
	FROM
		users.tbOldFullInfo where flBin IS NOT NULL
) a;

INSERT INTO users.TbUserDocs (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flUserId,
	flGivenBy,
	flNumber,
	flDate,
	flDocType
)
SELECT 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	flUserId,
	flDocGivenBy,
	flDocNumber,
	flDocDate,
	flDocType
FROM users.tbOldFullInfo where flDocType IS NOT null and length(fldoctype)<=8;

INSERT INTO users.TbUserInvites (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flInviteAccountId,
	flIin,
	flEmail,
	flGroupName,
	flInviteExpiresAt,
	flAutoBlockAfter,
	flInviteCode,
	flUsed
)
SELECT 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	'-',
	flIin,
	flEmail,
	flGroupName,
	flAutoBlockAfter,
	flAutoBlockAfter,
	flInviteCode,
	flInviteAccepted
FROM users.tbOldFullInfo where flInviteSended = true;

DROP TABLE users.tbUniqueAccounts;
DROP TABLE users.tbOldFullInfo;

INSERT INTO users.z_history_TbAccounts 
SELECT * FROM users.TbAccounts;

INSERT INTO users.z_history_tbusers 
SELECT * FROM users.tbusers;

INSERT INTO users.z_history_tbcorpdata
SELECT * FROM users.tbcorpdata;

INSERT INTO users.z_history_tbaccountaddr
SELECT * FROM users.tbaccountaddr;

INSERT INTO users.z_history_tbuserdocs
SELECT * FROM users.tbuserdocs;

INSERT INTO users.z_history_tbuserinvites
SELECT * FROM users.tbuserinvites;

CREATE TABLE users.TbAccountAdmins (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flLogin character varying(25) NOT NULL, 

CONSTRAINT PK_TbAccountAdmins PRIMARY KEY (
    flAccountId, flLogin
)
);




CREATE TABLE users.z_history_TbAccountAdmins (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flLogin character varying(25) NOT NULL, 

CONSTRAINT PK_8dc76748b83685943950e695d33ad3beba196507 PRIMARY KEY (
    flEntityId, flRequestId
)
);



WITH tbAccountOwners AS (
	SELECT
		flAccountId,
		flLogin
	FROM users.tbUsers where flAccountType <> 'Corporate'
	UNION ALL
	SELECT 
		flAccountId,
		TRIM(flBin) || '-' || TRIM(flFirstPersonIin) as flLogin
	FROM users.tbcorpdata
	WHERE flFirstPersonIin is not null
)
INSERT INTO users.TbAccountAdmins (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flAccountId,
	flLogin
) 
SELECT 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	flAccountId,
	flLogin
FROM tbAccountOwners;

INSERT INTO users.z_history_TbAccountAdmins 
SELECT * FROM users.TbAccountAdmins;

WITH kato2ar AS (
	SELECT * FROM (VALUES
	('11801', '144254'),
	('11801003', '157354'),
	('11801004', '154491'),
	('11801005', '144269'),
	('11801006', '144757'),
	('11801007', '145319'),
	('11801019', '153382'),
	('11801008', '145886'),
	('11801009', '146322'),
	('11801010', '146758'),
	('11801011', '147556'),
	('11801012', '148268'),
	('11801013', '148792'),
	('11801014', '149352'),
	('11801001', '155241'),
	('11801015', '150133'),
	('11801016', '150409'),
	('11801002', '156843'),
	('11801017', '150807'),
	('11801018', '152785'),
	('11802', '68381'),
	('11802022', '70995'),
	('11802020', '68402'),
	('11802021', '70251'),
	('11802023', '71730'),
	('11802032', '78988'),
	('11802033', '251454'),
	('11802034', '251453'),
	('11802024', '72930'),
	('11802026', '74033'),
	('11802027', '74573'),
	('11802029', '76086'),
	('11802028', '75597'),
	('11802025', '73380'),
	('11802030', '77174'),
	('11802031', '77882'),
	('11803', '79492'),
	('11803036', '81407'),
	('11803037', '82961'),
	('11803038', '84428'),
	('11803039', '84918'),
	('11803049', '101057'),
	('11803040', '89720'),
	('11803051', '102366'),
	('11803034', '80696'),
	('11803044', '93701'),
	('11803043', '92656'),
	('11803041', '91333'),
	('11803042', '92053'),
	('11803045', '96676'),
	('11803046', '97307'),
	('11803047', '98161'),
	('11803048', '99524'),
	('11803033', '79497'),
	('11803035', '81062'),
	('11803050', '101761'),
	('11816', '158106'),
	('11816208', '138439'),
	('11816201', '137626'),
	('11816204', '140362'),
	('11816206', '141496'),
	('11816207', '139305'),
	('11816202', '142245'),
	('1181609', '251403'),
	('11816205', '136194'),
	('11804', '21030'),
	('11804052', '26551'),
	('11804053', '21386'),
	('11804054', '22398'),
	('11804055', '23179'),
	('11804057', '24944'),
	('11804056', '23660'),
	('11804058', '25616'),
	('11804059', '25836'),
	('11814', '31221'),
	('11814185', '35255'),
	('11814186', '36498'),
	('11814187', '37249'),
	('11814188', '37797'),
	('11814189', '38607'),
	('11814190', '40152'),
	('11814191', '41600'),
	('11814195', '46412'),
	('11814192', '43545'),
	('11814183', '32808'),
	('11814194', '45857'),
	('11814179', '32845'),
	('11814182', '33335'),
	('11814196', '47004'),
	('11814197', '48091'),
	('11814198', '48785'),
	('11814181', '31223'),
	('11814203', '50252'),
	('11806', '168532'),
	('11806074', '170722'),
	('11806075', '171635'),
	('11806076', '172641'),
	('11806077', '173461'),
	('11806079', '175089'),
	('11806080', '176088'),
	('11806081', '176469'),
	('11806082', '176903'),
	('11806073', '168533'),
	('11806078', '174443'),
	('11806083', '177478'),
	('11805', '51068'),
	('11805061', '54338'),
	('11805068', '63725'),
	('11805062', '56508'),
	('11805063', '57114'),
	('11805064', '58887'),
	('11805065', '59660'),
	('11805066', '61226'),
	('11805067', '63182'),
	('11805069', '65744'),
	('11805070', '66807'),
	('11805071', '67427'),
	('11805060', '51071'),
	('11805072', '67977'),
	('11807', '202313'),
	('11807093', '202358'),
	('11807094', '202853'),
	('11807085', '211201'),
	('11807095', '204970'),
	('11807096', '205904'),
	('11807086', '211888'),
	('11807084', '212922'),
	('11807210', '212923'),
	('11807211', '213461'),
	('11807087', '212498'),
	('11807097', '206248'),
	('11807098', '207740'),
	('11807099', '208539'),
	('11807088', '214126'),
	('11807089', '214204'),
	('11807090', '214573'),
	('11807091', '214935'),
	('11807100', '209301'),
	('11807092', '215326'),
	('11807101', '210266'),
	('11808', '125189'),
	('11808106', '127222'),
	('11808107', '127599'),
	('11808103', '125938'),
	('11808108', '127975'),
	('11808109', '128899'),
	('11808110', '129545'),
	('11808111', '129919'),
	('11808112', '130346'),
	('11808113', '130718'),
	('11808114', '131673'),
	('11808102', '125193'),
	('11808115', '132378'),
	('11808104', '126589'),
	('11808116', '133256'),
	('11808117', '133822'),
	('11808105', '126778'),
	('11808118', '134092'),
	('11808119', '134534'),
	('11808120', '134965'),
	('11808121', '135469'),
	('11809', '158628'),
	('11809123', '158641'),
	('11809124', '160076'),
	('11809125', '160572'),
	('11809126', '161561'),
	('11809127', '162679'),
	('11809122', '165288'),
	('11809128', '163602'),
	('11809129', '164334'),
	('11810', '109099'),
	('11810130', '110170'),
	('11810132', '111628'),
	('11810131', '110672'),
	('11810133', '112227'),
	('11810134', '112449'),
	('11810210', '109100'),
	('11810135', '113090'),
	('11815', '106724'),
	('11815199', '107193'),
	('11815210', '251449'),
	('11815209', '108584'),
	('11815200', '107989'),
	('11812', '2'),
	('11812153', '9'),
	('11812155', '2174'),
	('11812156', '2624'),
	('11812157', '4246'),
	('11812158', '4879'),
	('11812159', '5467'),
	('11812161', '6639'),
	('11812152', '9103'),
	('11812162', '7250'),
	('11812163', '7907'),
	('11812165', '8440'),
	('11812154', '1313'),
	('11813', '113406'),
	('11813166', '114651'),
	('11813167', '115610'),
	('11813174', '121576'),
	('11813169', '117442'),
	('11813170', '118370'),
	('11813171', '119138'),
	('11813172', '120325'),
	('11813164', '113407'),
	('11813178', '124165'),
	('11813168', '116232'),
	('11813173', '121063'),
	('11813175', '122065'),
	('11813177', '123152'),
	('11813176', '123601'),
	('11811', '178760'),
	('11811137', '180937'),
	('11811141', '197973'),
	('11811152', '251456'),
	('11811142', '183520'),
	('11811153', '251455'),
	('11811138', '181562'),
	('11811143', '184383'),
	('11811144', '199606'),
	('11811145', '188028'),
	('11811146', '189662'),
	('11811147', '192677'),
	('11811148', '195829'),
	('11811149', '198625'),
	('11811139', '182036'),
	('11811150', '196302'),
	('11811151', '197353'),
	('11817', '178771'),
	('11817001', '179926'),
	('11817002', '180370'),
	('11817003', '180628'),
	('11817004', '251415'),
	('11807212', '180370'),
	('11807213', '179926'),
	('11811136', '178771'),
	('11807214', '180628'),
	('11807215', '251415'),
	('11811140','180937'),
   	('11812160','6185'),
	('11803052', '251450'),
   	('11814193', '42534'),
    ('11814184', '36498'),
    ('11814180', '32562')
	) as t(kato, ar)
)
	UPDATE users.tbaccountaddr
	SET 
		fladrcountry = '1', 
		fladrobl = (select ar from kato2ar where kato = fladrobl limit 1),
		fladrreg = (select ar from kato2ar where kato = fladrreg limit 1)
	WHERE fladrcountry = '118';

WITH kato2ar AS (
	SELECT * FROM (VALUES
	('11801', '144254'),
	('11801003', '157354'),
	('11801004', '154491'),
	('11801005', '144269'),
	('11801006', '144757'),
	('11801007', '145319'),
	('11801019', '153382'),
	('11801008', '145886'),
	('11801009', '146322'),
	('11801010', '146758'),
	('11801011', '147556'),
	('11801012', '148268'),
	('11801013', '148792'),
	('11801014', '149352'),
	('11801001', '155241'),
	('11801015', '150133'),
	('11801016', '150409'),
	('11801002', '156843'),
	('11801017', '150807'),
	('11801018', '152785'),
	('11802', '68381'),
	('11802022', '70995'),
	('11802020', '68402'),
	('11802021', '70251'),
	('11802023', '71730'),
	('11802032', '78988'),
	('11802033', '251454'),
	('11802034', '251453'),
	('11802024', '72930'),
	('11802026', '74033'),
	('11802027', '74573'),
	('11802029', '76086'),
	('11802028', '75597'),
	('11802025', '73380'),
	('11802030', '77174'),
	('11802031', '77882'),
	('11803', '79492'),
	('11803036', '81407'),
	('11803037', '82961'),
	('11803038', '84428'),
	('11803039', '84918'),
	('11803049', '101057'),
	('11803040', '89720'),
	('11803051', '102366'),
	('11803034', '80696'),
	('11803044', '93701'),
	('11803043', '92656'),
	('11803041', '91333'),
	('11803042', '92053'),
	('11803045', '96676'),
	('11803046', '97307'),
	('11803047', '98161'),
	('11803048', '99524'),
	('11803033', '79497'),
	('11803035', '81062'),
	('11803050', '101761'),
	('11816', '158106'),
	('11816208', '138439'),
	('11816201', '137626'),
	('11816204', '140362'),
	('11816206', '141496'),
	('11816207', '139305'),
	('11816202', '142245'),
	('1181609', '251403'),
	('11816205', '136194'),
	('11804', '21030'),
	('11804052', '26551'),
	('11804053', '21386'),
	('11804054', '22398'),
	('11804055', '23179'),
	('11804057', '24944'),
	('11804056', '23660'),
	('11804058', '25616'),
	('11804059', '25836'),
	('11814', '31221'),
	('11814185', '35255'),
	('11814186', '36498'),
	('11814187', '37249'),
	('11814188', '37797'),
	('11814189', '38607'),
	('11814190', '40152'),
	('11814191', '41600'),
	('11814195', '46412'),
	('11814192', '43545'),
	('11814183', '32808'),
	('11814194', '45857'),
	('11814179', '32845'),
	('11814182', '33335'),
	('11814196', '47004'),
	('11814197', '48091'),
	('11814198', '48785'),
	('11814181', '31223'),
	('11814203', '50252'),
	('11806', '168532'),
	('11806074', '170722'),
	('11806075', '171635'),
	('11806076', '172641'),
	('11806077', '173461'),
	('11806079', '175089'),
	('11806080', '176088'),
	('11806081', '176469'),
	('11806082', '176903'),
	('11806073', '168533'),
	('11806078', '174443'),
	('11806083', '177478'),
	('11805', '51068'),
	('11805061', '54338'),
	('11805068', '63725'),
	('11805062', '56508'),
	('11805063', '57114'),
	('11805064', '58887'),
	('11805065', '59660'),
	('11805066', '61226'),
	('11805067', '63182'),
	('11805069', '65744'),
	('11805070', '66807'),
	('11805071', '67427'),
	('11805060', '51071'),
	('11805072', '67977'),
	('11807', '202313'),
	('11807093', '202358'),
	('11807094', '202853'),
	('11807085', '211201'),
	('11807095', '204970'),
	('11807096', '205904'),
	('11807086', '211888'),
	('11807084', '212922'),
	('11807210', '212923'),
	('11807211', '213461'),
	('11807087', '212498'),
	('11807097', '206248'),
	('11807098', '207740'),
	('11807099', '208539'),
	('11807088', '214126'),
	('11807089', '214204'),
	('11807090', '214573'),
	('11807091', '214935'),
	('11807100', '209301'),
	('11807092', '215326'),
	('11807101', '210266'),
	('11808', '125189'),
	('11808106', '127222'),
	('11808107', '127599'),
	('11808103', '125938'),
	('11808108', '127975'),
	('11808109', '128899'),
	('11808110', '129545'),
	('11808111', '129919'),
	('11808112', '130346'),
	('11808113', '130718'),
	('11808114', '131673'),
	('11808102', '125193'),
	('11808115', '132378'),
	('11808104', '126589'),
	('11808116', '133256'),
	('11808117', '133822'),
	('11808105', '126778'),
	('11808118', '134092'),
	('11808119', '134534'),
	('11808120', '134965'),
	('11808121', '135469'),
	('11809', '158628'),
	('11809123', '158641'),
	('11809124', '160076'),
	('11809125', '160572'),
	('11809126', '161561'),
	('11809127', '162679'),
	('11809122', '165288'),
	('11809128', '163602'),
	('11809129', '164334'),
	('11810', '109099'),
	('11810130', '110170'),
	('11810132', '111628'),
	('11810131', '110672'),
	('11810133', '112227'),
	('11810134', '112449'),
	('11810210', '109100'),
	('11810135', '113090'),
	('11815', '106724'),
	('11815199', '107193'),
	('11815210', '251449'),
	('11815209', '108584'),
	('11815200', '107989'),
	('11812', '2'),
	('11812153', '9'),
	('11812155', '2174'),
	('11812156', '2624'),
	('11812157', '4246'),
	('11812158', '4879'),
	('11812159', '5467'),
	('11812161', '6639'),
	('11812152', '9103'),
	('11812162', '7250'),
	('11812163', '7907'),
	('11812165', '8440'),
	('11812154', '1313'),
	('11813', '113406'),
	('11813166', '114651'),
	('11813167', '115610'),
	('11813174', '121576'),
	('11813169', '117442'),
	('11813170', '118370'),
	('11813171', '119138'),
	('11813172', '120325'),
	('11813164', '113407'),
	('11813178', '124165'),
	('11813168', '116232'),
	('11813173', '121063'),
	('11813175', '122065'),
	('11813177', '123152'),
	('11813176', '123601'),
	('11811', '178760'),
	('11811137', '180937'),
	('11811141', '197973'),
	('11811152', '251456'),
	('11811142', '183520'),
	('11811153', '251455'),
	('11811138', '181562'),
	('11811143', '184383'),
	('11811144', '199606'),
	('11811145', '188028'),
	('11811146', '189662'),
	('11811147', '192677'),
	('11811148', '195829'),
	('11811149', '198625'),
	('11811139', '182036'),
	('11811150', '196302'),
	('11811151', '197353'),
	('11817', '178771'),
	('11817001', '179926'),
	('11817002', '180370'),
	('11817003', '180628'),
	('11817004', '251415'),
	('11807212', '180370'),
	('11807213', '179926'),
	('11811136', '178771'),
	('11807214', '180628'),
	('11807215', '251415'),
   	('11811140','180937'),
	('11812160','6185'),
	('11803052', '251450'),
    ('11814193', '42534'),
    ('11814184', '36498'),
    ('11814180', '32562')
				   
	) as t(kato, ar)
)

UPDATE users.z_history_tbaccountaddr
SET 
	fladrcountry = '1', 
	fladrobl = (select ar from kato2ar where kato = fladrobl limit 1),
	fladrreg = (select ar from kato2ar where kato = fladrreg limit 1)
WHERE fladrcountry = '118';

	--
ALTER TABLE users.__rm__tbuserbankdetails DROP CONSTRAINT pk_tbuserbankdetails;
ALTER TABLE users.__rm__z_history_tbuserbankdetails DROP CONSTRAINT pk_65c73313cabc346a222c259565dc1d9f7c1d3d27;


ALTER TABLE users.tbusers
ADD COLUMN flRegistrationDate timestamp without time zone;

ALTER TABLE users.z_history_tbusers
ADD COLUMN flRegistrationDate timestamp without time zone;


-- Данный скрипт определяет дату регистрации пользователя только по новой схеме
-- в данном случае подходит только для новых БД проектов Dfo.
-- В Kezekte надо будет переделать скрипт.
-- Также данный скрипт пытается корректно сконвертить тестовые данные (в тестовой среде в таблице историй нету данных)

WITH tbUsersHistory AS (
	select 
		flUserId,
		flRequestDate,
		ROW_NUMBER() OVER(PARTITION BY flUserId ORDER BY flRequestDate) as flRowNum
	from users.z_history_tbusers
),
tbUsersCurr AS (
	SELECT
		flUserId,
		flRequestDate
	FROM users.tbusers
),
tbUsersFirstRequestDate AS (
	SELECT 
		users.flUserId,
		LEAST(users.flRequestDate, usershistory.flRequestDate) as flRegDate
	FROM tbUsersCurr users
	LEFT JOIN tbUsersHistory usershistory on users.fluserid = usershistory.fluserid and usershistory.flRowNum = 1
)
UPDATE users.tbUsers t1
SET flRegistrationDate = t2.flRegDate
FROM tbUsersFirstRequestDate t2 
WHERE t1.flUserId = t2.flUserId;

UPDATE users.z_history_tbUsers t1
SET flRegistrationDate = t2.flRegistrationDate
FROM users.tbUsers t2
WHERE t1.flUserId = t2.flUserId;


ALTER TABLE  users.tbUsers
ALTER COLUMN flRegistrationDate SET NOT NULL;

ALTER TABLE  users.z_history_tbUsers
ALTER COLUMN flRegistrationDate SET NOT NULL;

CREATE TABLE users.TbAccountBankAccount (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flBankBIC character varying(8) NOT NULL,
flKbe character varying(2) ,
flIik character varying(20) NOT NULL,
flNote character varying , 

CONSTRAINT PK_TbAccountBankAccount PRIMARY KEY (
    flAccountId
)

);


CREATE TABLE users.z_history_TbAccountBankAccount (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flBankBIC character varying(8) NOT NULL,
flKbe character varying(2) ,
flIik character varying(20) NOT NULL,
flNote character varying , 

CONSTRAINT PK_512b1d6624ea31d447c9da685e0c39c248b78b12 PRIMARY KEY (
    flEntityId, flRequestId
)

);



WITH tbUserBankDetailsAllPrepared AS (
    SELECT 
        tbusers.flAccountId,
        tbBankDetails.flEntityId,
        tbBankDetails.flRequestId,
        tbBankDetails.flIsHidden,
        tbBankDetails.flRequestType,
        tbBankDetails.flRequestDate,
        tbBankDetails.flRequestUserId,
        tbBankDetails.flBankBIC,
        tbBankDetails.flKbe,
        tbBankDetails.flIik,
        tbBankDetails.flNote
    FROM users.__rm__tbuserbankdetails tbBankDetails 
    INNER JOIN users.tbusers tbusers on tbBankDetails.flUserId = tbUsers.flUserId
    WHERE tbBankDetails.flRemoved = false
),
tbUserBankDetailsDescRowNum AS (
    SELECT *,
    ROW_NUMBER() OVER(PARTITION BY flAccountId ORDER BY flRequestDate DESC) as flRowNum
    FROM tbUserBankDetailsAllPrepared
)
INSERT INTO users.TbAccountBankAccount (
    flEntityId,
    flRequestId,
    flIsHidden,
    flRequestType,
    flRequestDate,
    flRequestUserId,
    flAccountId,
    flBankBIC,
    flKbe,
    flIik,
    flNote
)
SELECT 
    flEntityId,
    flRequestId,
    flIsHidden,
    flRequestType,
    flRequestDate,
    flRequestUserId,
    flAccountId,
    flBankBIC,
    flKbe,
    flIik,
    flNote
FROM tbUserBankDetailsDescRowNum WHERE flRowNum = 1;






CREATE TABLE users.TbAccountModel (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flUpdatedAt timestamp without time zone ,
flPrevUpdateVersion integer NOT NULL,
flUpdateVersion integer NOT NULL,
flAccountDataModel character varying NOT NULL, 

CONSTRAINT PK_TbAccountModel PRIMARY KEY (
    flAccountId
));




CREATE TABLE users.z_history_TbAccountModel (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flAccountId character varying(15) NOT NULL,
flUpdatedAt timestamp without time zone ,
flPrevUpdateVersion integer NOT NULL,
flUpdateVersion integer NOT NULL,
flAccountDataModel character varying NOT NULL, 

CONSTRAINT PK_f4e50377bdebb9eed917109ab1216d54551c3c26 PRIMARY KEY (
    flEntityId, flRequestId
));



CREATE TABLE users.TbUserModel (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flLogin character varying(25) NOT NULL,
flUpdatedAt timestamp without time zone ,
flPrevUpdateVersion integer NOT NULL,
flUpdateVersion integer NOT NULL,
flUserDataModel character varying NOT NULL, 

CONSTRAINT PK_TbUserModel PRIMARY KEY (
    flLogin
));


CREATE TABLE users.z_history_TbUserModel (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flLogin character varying(25) NOT NULL,
flUpdatedAt timestamp without time zone ,
flPrevUpdateVersion integer NOT NULL,
flUpdateVersion integer NOT NULL,
flUserDataModel character varying NOT NULL, 

CONSTRAINT PK_9536d5425c5d708c60851e93f5d9a4526b6ed0fa PRIMARY KEY (
    flEntityId, flRequestId
));



-- Populate TbAccountModel

WITH tbAccountAddr AS (
	SELECT 
		flAccountId as "AccountId",
		flAddrType as "AddrType",
		flAdrCountry as "AdrCountry",
		flAdrIndex as "AdrIndex",
		flAdrObl as "AdrObl",
		flAdrReg as "AdrReg",
		flAdrAdr as "AdrAdr",
		flRkaCode as "RkaCode"
	FROM users.TbAccountAddr
),
tbAccountAddrModel AS (
	SELECT 
		"AccountId",
		json_agg(t) as "AccountAddresses"
	FROM tbAccountAddr t
	GROUP BY "AccountId"
),
tbAccountAdminsModel AS (
	SELECT
		flAccountId as "AccountId",
		json_agg(json_build_object('Login', t.flLogin)) as "AccountAdmins"
	FROM users.tbAccountAdmins t
	GROUP BY flAccountId
),
tbAccountBankDetails AS (
	SELECT
		flAccountId as "AccountId",
		json_build_object(
			'Bic', flBankBic,
			'Iban', flIik,
			'Kbe', flKbe,
			'Note', flNote
		) as "BankAccount"
	FROM users.tbAccountBankAccount
),
tbCorpData AS (
	SELECT
		flAccountId as "AccountId",
		json_build_object(
			'Bin', flBin,
			'FirstPersonFio', flFirstPersonFio,
			'FirstPersonIin', flFirstPersonIin,
			'CorpEmail', flCorpEmail,
			'CorpMobilePhone', flCorpMobilePhone,
			'CorpStationaryPhone', flCorpStationaryPhone
		) as "CorpData"
	FROM users.tbcorpdata
)

insert into users.TbAccountModel (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flAccountId,
	flUpdatedAt,
	flPrevUpdateVersion,
	flUpdateVersion,
	flAccountDataModel
)

select 
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	flAccountId,
	now(),
	0,
	1,
	json_build_object(
		'Account', json_build_object(
			'AccountId', tbAccounts.flAccountId,
			'AccountType', tbAccounts.flAccountType,
			'Xin', tbAccounts.flXin,
			'NameRu', tbAccounts.flNameRu,
			'NameKz', tbAccounts.flNameKz,
			'IsBlocked', tbAccounts.flIsBlocked,
			'BlockReason', tbAccounts.flBlockReason
		),
		'AccountAddresses', tbAddrModel."AccountAddresses",
		'AccountAdmins', tbAdminsModel."AccountAdmins",
		'BankAccount', tbBankDetails."BankAccount",
		'CorpData', tbCorpData."CorpData"
	) as flModel
from users.tbaccounts tbAccounts
left join tbAccountAddrModel tbAddrModel on tbAccounts.flAccountId = tbAddrModel."AccountId"
left join tbAccountAdminsModel tbAdminsModel on tbaccounts.flAccountId = tbAdminsModel."AccountId"
left join tbAccountBankDetails tbBankDetails on tbaccounts.flAccountId = tbBankDetails."AccountId"
left join tbCorpData tbCorpData on tbaccounts.flAccountId = tbCorpData."AccountId";

INSERT INTO users.z_history_TbAccountModel SELECT * FROM users.TbAccountModel;

-- Populate TbUserModel

WITH tbuserdocs AS (
	SELECT 
		flUserId "UserId",
		flGivenBy "GivenBy",
		flNumber "Number",
		flDate "Date",
		flDocType "DocType"
	FROM users.tbuserdocs
),
tbUserDocsModel AS (
	SELECT 
		"UserId",
		json_agg(t) as "UserDocs"
	FROM tbuserdocs t
	GROUP BY "UserId"
),
tbUserRoles AS (
	SELECT
		flUserId as "UserId",
		json_agg(t.flRoleId) as "UserRoles"
	FROM users.tbuserroles t
	GROUP by flUserId
)
INSERT INTO users.tbUserModel (
	flEntityId,
	flRequestId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flLogin,
	flUpdatedAt,
	flPrevUpdateVersion,
	flUpdateVersion,
	flUserDataModel
)
SELECT
	nextval('public.seq_entity_id'),
	nextval('public.seq_request_id'),
	false,
	0,
	now(),
	-1,

	tbUsers.flLogin,
	now(),
	0,
	1,
	json_build_object(
		'AccountId', tbusers.flAccountId,
		'AccountType', tbusers.flAccountType,
		'AccountXin', tbusers.flXin,
		'UserId', tbusers.flUserId,
		'User', json_build_object(
			'Login', tbusers.flLogin,
			'Iin', tbusers.flIin,
			'GroupName', tbusers.flGroupName,
			'FirstName', tbusers.flFirstName,
			'LastName', tbusers.flLastName,
			'MiddleName', tbusers.flMiddleName,
			'FullName', tbusers.flFullName,
			'Email', tbusers.flEmail,
			'MobilePhone', tbusers.flMobilePhone,
			'IsMobilePhoneConfirmed', tbusers.flIsMobilePhoneConfirmed,
			'StationaryPhone', tbusers.flStationaryPhone,
			'IsBlocked', tbusers.flIsBlocked,
			'BlockReason', tbusers.flBlockReason,
			'AutoBlockAfter', tbusers.flAutoBlockAfter,
			'RegistrationDate', tbusers.flRegistrationDate
		),
		'UserDocs', tbDocs."UserDocs",
		'UserRoles', tbUserRoles."UserRoles"
	) as flModel
FROM users.tbusers tbusers
LEFT JOIN tbUserDocsModel tbDocs ON tbUsers.flUserId = tbDocs."UserId"
LEFT JOIN tbUserRoles tbUserRoles ON tbUsers.flUserId = tbUserRoles."UserId";


INSERT INTO users.z_history_TbUserModel SELECT * FROM users.TbUserModel;
