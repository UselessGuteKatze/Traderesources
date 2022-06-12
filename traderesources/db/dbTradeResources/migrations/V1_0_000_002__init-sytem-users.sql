insert into users.tbusers (
	flRequestId,
	flEntityId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flUserid,
	flLogin,
	flFirstName,
	flLastName,
	flMiddleName,
	flIin, 
	flEmail,
	flIsBlocked, 
	flIsDeleted, 
	flProfile, 
	flIsMobilePhoneConfirmed
) values (
	-1,		-- flRequestId,
	-1,		-- flEntityId,
	false,	-- flIsHidden,
	1,		-- flRequestType,
	now(),	-- flRequestDate,
	-1,		-- flRequestUserId,

	-2,			-- flUserid,
	'guest',	-- flLogin,
	'guest',	-- flFirstName,
	'guest',	-- flLastName,
	'guest',	-- flMiddleName,
	'guest',	-- flIin, 
	'-',		-- flEmail
	false,		-- flIsBlocked, 
	false,		-- flIsDeleted, 
	'internal',	-- flProfile, 
	false		-- flIsMobilePhoneConfirmed
);

insert into users.z_history_tbusers (
	flRequestId,
	flEntityId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flUserid,
	flLogin,
	flFirstName,
	flLastName,
	flMiddleName,
	flIin, 
	flEmail,
	flIsBlocked, 
	flIsDeleted, 
	flProfile, 
	flIsMobilePhoneConfirmed
) values (
	-1,		-- flRequestId,
	-1,		-- flEntityId,
	false,	-- flIsHidden,
	1,		-- flRequestType,
	now(),	-- flRequestDate,
	-1,		-- flRequestUserId,

	-2,			-- flUserid,
	'guest',	-- flLogin,
	'guest',	-- flFirstName,
	'guest',	-- flLastName,
	'guest',	-- flMiddleName,
	'guest',	-- flIin, 
	'-',		-- flEmail
	false,		-- flIsBlocked, 
	false,		-- flIsDeleted, 
	'internal',	-- flProfile, 
	false		-- flIsMobilePhoneConfirmed
);



insert into users.TbUsersInternalProfileData (
	flRequestId,
	flEntityId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flUserId,
	flWorkPost,
	flInviteSended,
	flInviteAccepted
) values (
	-1,		-- flRequestId,
	-1,		-- flEntityId,
	false,	-- flIsHidden,
	1,		-- flRequestType,
	now(),	-- flRequestDate,
	-1,		-- flRequestUserId,

	-2,		-- flUserId,
	'',		-- flWorkPost,
	false,
	false
);


insert into users.z_history_TbUsersInternalProfileData (
	flRequestId,
	flEntityId,
	flIsHidden,
	flRequestType,
	flRequestDate,
	flRequestUserId,

	flUserId,
	flWorkPost,
	flInviteSended,
	flInviteAccepted
) values (
	-1,		-- flRequestId,
	-1,		-- flEntityId,
	false,	-- flIsHidden,
	1,		-- flRequestType,
	now(),	-- flRequestDate,
	-1,		-- flRequestUserId,

	-2,		-- flUserId,
	'',		-- flWorkPost,
	false,
	false
);