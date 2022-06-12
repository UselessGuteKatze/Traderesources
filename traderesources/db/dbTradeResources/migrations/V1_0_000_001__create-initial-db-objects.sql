CREATE SCHEMA diagnostics;

CREATE TABLE diagnostics.TbExceptionsLog (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer ,
flException character varying NOT NULL,
flProccessed boolean ,
flUrl character varying ,

CONSTRAINT PK_TbExceptionsLog PRIMARY KEY (
    flId
)

);




CREATE SEQUENCE diagnostics."seq_TbExceptionsLog_flId";

CREATE SCHEMA files;

CREATE TABLE files.TbFiles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flFileSrcId character varying(36) NOT NULL,
flFileHashSha256 character varying(64) NOT NULL,
flFileSize bigint NOT NULL,
flContent bytea NOT NULL,

CONSTRAINT PK_TbFiles PRIMARY KEY (
    flFileSrcId
)

);

CREATE UNIQUE  INDEX "IX_TbFiles_flHash" ON files.TbFiles(flFileHashSha256 Asc);
CREATE UNIQUE  INDEX "IX_TbFiles_flHash_flSize" ON files.TbFiles(flFileHashSha256 Asc, flFileSize Asc);



CREATE TABLE files.z_history_TbFiles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flFileSrcId character varying(36) NOT NULL,
flFileHashSha256 character varying(64) NOT NULL,
flFileSize bigint NOT NULL,
flContent bytea NOT NULL,

CONSTRAINT PK_84af90d5425f276c4651e8b31b19067da2b2bbe2 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE files.tbFilesInfo (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flFileId character varying(36) NOT NULL,
flFileSrcId character varying(36) ,
flFileName character varying ,
flFileSize bigint NOT NULL,
flDescription character varying ,
flNamespace character varying ,

CONSTRAINT PK_tbFilesInfo PRIMARY KEY (
    flFileId
)

);




CREATE TABLE files.z_history_tbFilesInfo (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flFileId character varying(36) NOT NULL,
flFileSrcId character varying(36) ,
flFileName character varying ,
flFileSize bigint NOT NULL,
flDescription character varying ,
flNamespace character varying ,

CONSTRAINT PK_4e8c0655def48152cf28f0ca31f61b098459f5cc PRIMARY KEY (
    flEntityId, flRequestId
)

);



CREATE SCHEMA settings;

CREATE TABLE settings.TbUiSearchWidgetConfig (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flQueryName character varying(120) NOT NULL,
flUpdateTime timestamp without time zone ,
flFieldsOrderAndWidth character varying ,
flSelectedOutputFields character varying ,
flSelectedFilterFields character varying ,

CONSTRAINT ixSearchWidgetPrimaryKey PRIMARY KEY (
    flUserId, flQueryName
)

);




CREATE TABLE settings.z_history_TbUiSearchWidgetConfig (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flQueryName character varying(120) NOT NULL,
flUpdateTime timestamp without time zone ,
flFieldsOrderAndWidth character varying ,
flSelectedOutputFields character varying ,
flSelectedFilterFields character varying ,

CONSTRAINT PK_8c46b02992ca2addf2d45bbaa08b909e6e1ed5f0 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.TbUiSearchWidgetConfigDefault (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(120) NOT NULL,
flUpdateTime timestamp without time zone ,
flFieldsOrderAndWidth character varying ,
flSelectedOutputFields character varying ,
flSelectedFilterFields character varying ,

CONSTRAINT ixDefaultSearchWidgetPrimaryKey PRIMARY KEY (
    flQueryName
)

);




CREATE TABLE settings.z_history_TbUiSearchWidgetConfigDefault (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(120) NOT NULL,
flUpdateTime timestamp without time zone ,
flFieldsOrderAndWidth character varying ,
flSelectedOutputFields character varying ,
flSelectedFilterFields character varying ,

CONSTRAINT PK_6cb9f439d1f7437ce58bc596f7629c276f976d93 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchOutputFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_df994cc4be528aa825c46238b7860d326985ca5f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchOutputFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_6130b28a73cd9d012d408c90b4afd57bafd36738 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchFieldsOrderSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWeight numeric (18, 2) ,

CONSTRAINT PK_476448a060b868428340e1e04aff0b21ce64ce71 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchFieldsOrderSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWeight numeric (18, 2) ,

CONSTRAINT PK_79a4f790f2ad97bc2044c691f0d6b675a937073b PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchFieldsWidthSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWidth integer ,

CONSTRAINT PK_6024a27e3c441ba9b6d59de9b9cc99ba60aefd9f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchFieldsWidthSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWidth integer ,

CONSTRAINT PK_8f80a193bb33a39d484196f9123fcfb05d5fc824 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchFilterFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_231fd5babba87e3f0f62aca8eb78d55ae73e1320 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchFilterFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_ce34877be71d0a5f39aa6db12c91de43234b641e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchDefaultOutputFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_af24cfe2c2dbb3e02c11b24b847af14897236a9d PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchDefaultOutputFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_4cfe177a9b7578c0c982ad9bb136ec9b6fcd1c72 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchDefaultFieldsOrderSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWeight numeric (18, 2) ,

CONSTRAINT PK_ed064f58871724e7f1ee8516a4f4b2cf02cdfa07 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchDefaultFieldsOrderSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWeight numeric (18, 2) ,

CONSTRAINT PK_35d7355b3dda6b3d7b6bdb0287ad38ed96a0ac96 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchDefaultFieldsWidthSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWidth integer ,

CONSTRAINT PK_f2c25c8bca80278b459e947524c8f6b486ae5fb7 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchDefaultFieldsWidthSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,
flWidth integer ,

CONSTRAINT PK_d229ef8091aa1870133c42abebd4502e4c76a327 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.tbSearchDefaultFilterFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_df9a8b498a9626f3f67e4fd4c2e8dc95e1416f2f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE settings.z_history_tbSearchDefaultFilterFieldsSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flQueryName character varying(250) ,
flFieldName character varying(100) ,

CONSTRAINT PK_878ddc266653602d0094270259c4c2647fbf561a PRIMARY KEY (
    flEntityId, flRequestId
)

);



CREATE SCHEMA yoda_system;

CREATE TABLE yoda_system.tbAccessToReferencesRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flReferenceAccessRoleId integer ,
flRoleText character varying ,
flReferenceValues character varying ,

CONSTRAINT PK_3c9ab0d701d98f34ee054a602724e48edb72f0fa PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_tbAccessToReferencesRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flReferenceAccessRoleId integer ,
flRoleText character varying ,
flReferenceValues character varying ,

CONSTRAINT PK_14f86d49f39229aef9932be4beed5861cec516f4 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.TbCsSavedQueriesSchemas (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying ,
flQueryXmlSchema character varying ,

CONSTRAINT PK_8cb48a156d5298799ac25553a3ccfb58a4748389 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_TbCsSavedQueriesSchemas (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flQueryName character varying ,
flQueryXmlSchema character varying ,

CONSTRAINT PK_91487ce96b624ca25c232c6ca7ff41afb60e8062 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.tbMrp (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flYear integer ,
flMrp numeric (18, 2) ,

CONSTRAINT PK_c61f0e29f7fa1b1767aa818b6e7d95c44b9aa3b4 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_tbMrp (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flYear integer ,
flMrp numeric (18, 2) ,

CONSTRAINT PK_31ce122eac7b8dda35a1a18e35a0ba0c1a56a926 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.tbTextTranslations2 (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTextId integer ,
flLang character varying(10) ,
flTranslatedText character varying ,

CONSTRAINT PK_f15dc8c427f36b7861644155fdcd1851ee7fd24a PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_tbTextTranslations2 (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTextId integer ,
flLang character varying(10) ,
flTranslatedText character varying ,

CONSTRAINT PK_c073a99d214eeb84707d6e54ed2929af8c810427 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.tbTranslationsAllRegistredTexts (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flRowId integer ,
flText character varying ,
flCodeStack character varying ,
flUrl character varying ,

CONSTRAINT PK_1a4ce3a2a295dc958454c57305dad39a372577fc PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_tbTranslationsAllRegistredTexts (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flRowId integer ,
flText character varying ,
flCodeStack character varying ,
flUrl character varying ,

CONSTRAINT PK_af138dbab2c77803676c356d16f39094bb015dfc PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.TbTextSources2 (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTextId integer ,
flSource character varying ,

CONSTRAINT PK_a6b381db47ef9b21099f873b665b2ba363fc278a PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_TbTextSources2 (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTextId integer ,
flSource character varying ,

CONSTRAINT PK_aa558209bad93295c859f04423775a6f2a92cf9f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.TbTextTranslationsCategories (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flCategoryId integer ,
flText character varying ,

CONSTRAINT PK_5f1a4e8be8b8c205448d5520c3b7b641fc20ad29 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_TbTextTranslationsCategories (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flCategoryId integer ,
flText character varying ,

CONSTRAINT PK_7dc7f047a756db2e23b569a29239bd41de265efe PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.TbTextSourceCategories (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flCategoryId integer ,
flSource character varying(500) ,

CONSTRAINT PK_65d7352094427eed99e57d2252ccaee461757c82 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_TbTextSourceCategories (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flCategoryId integer ,
flSource character varying(500) ,

CONSTRAINT PK_b38b5e9eb8bcc9d0362f15e76d53ea34f9e252f2 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.TbTextsAll (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flText character varying NOT NULL,
flExportToJavascript boolean ,

CONSTRAINT PK_TbTextsAll PRIMARY KEY (
    flId
)

);




CREATE TABLE yoda_system.z_history_TbTextsAll (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flText character varying NOT NULL,
flExportToJavascript boolean ,

CONSTRAINT PK_f73fab33ea028cdad0ab1ce7cbd2b48e2fbc40c1 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.tbTextsIgnore (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTextId integer ,

CONSTRAINT PK_17315d296aeb903a991fa5b380dc8305ede27ba3 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.tbNotWorkingDaysCalendar (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flDate date ,

CONSTRAINT PK_a31c3275886a974b5ec56ef24b5233bff36ea22d PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_tbNotWorkingDaysCalendar (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flDate date ,

CONSTRAINT PK_66da3e7dd07a6296e887f4f3d7c4fbf0fc4a54d9 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.tbBlock (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTheme character varying(100) ,
flModule character varying(200) ,
flBlock character varying(100) ,
flRegion character varying(50) ,
flWeight numeric (18, 2) ,

CONSTRAINT PK_b01bdce7d64d18543432a1e0c9626926ff71d1b8 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE yoda_system.z_history_tbBlock (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flTheme character varying(100) ,
flModule character varying(200) ,
flBlock character varying(100) ,
flRegion character varying(50) ,
flWeight numeric (18, 2) ,

CONSTRAINT PK_96db1046206d00d8f396572d3760606c78929a72 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE yoda_system."seq_TbTextTranslationsCategories_flCategoryId";


CREATE SEQUENCE yoda_system."seq_TbTextsAll_flId";

CREATE SCHEMA users;

CREATE TABLE users.TbDataAccessSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flSettingsId integer ,
flTitle character varying ,
flReferenceName character varying(200) ,
flUserProfile character varying(100) ,

CONSTRAINT PK_ca50217467154e3f68d5320b82e46f3156e37211 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.z_history_TbDataAccessSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flSettingsId integer ,
flTitle character varying ,
flReferenceName character varying(200) ,
flUserProfile character varying(100) ,

CONSTRAINT PK_e94f51aef8b435df6100fa120d312fd9df83a9fe PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbDataAccessSettingsToTablesMap (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flMapId integer ,
flTableName character varying(200) ,
flSettingsId integer ,

CONSTRAINT PK_3591b9d86c7cd5980c5a4c6b9767632ede0eec34 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.z_history_TbDataAccessSettingsToTablesMap (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flMapId integer ,
flTableName character varying(200) ,
flSettingsId integer ,

CONSTRAINT PK_0cbb56ea49ffdc513f55fab5ee5bf50556d55bac PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbDataAccessUserSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flSettingsId integer NOT NULL,
flUserId integer NOT NULL,
flValue character varying NOT NULL,

CONSTRAINT PK_TbDataAccessUserSettings PRIMARY KEY (
    flId
)

);




CREATE TABLE users.z_history_TbDataAccessUserSettings (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flSettingsId integer NOT NULL,
flUserId integer NOT NULL,
flValue character varying NOT NULL,

CONSTRAINT PK_29da563ea899011899c0363c2c8440fc70233417 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbDataAccessSettingsImport (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flDataAccessSettingsId integer ,
flSettingsImportId character varying ,

CONSTRAINT PK_50a3f99f44c4543ee5f3aa7a368b6ae296540d4e PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flRoleId integer NOT NULL,
flRoleName character varying NOT NULL,
flRoleDescription character varying ,
flProject character varying(20) ,
flUserProfile character varying(20) ,
flAutoAssigned boolean ,

CONSTRAINT PK_5c78aa39092f2dc2d3ae188efab556876374fc45 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.z_history_TbRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flRoleId integer NOT NULL,
flRoleName character varying NOT NULL,
flRoleDescription character varying ,
flProject character varying(20) ,
flUserProfile character varying(20) ,
flAutoAssigned boolean ,

CONSTRAINT PK_13e08298465414a31c7f9fbc42ee15f0083d5155 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.tbRolePermissions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flRoleId integer ,
flControllerName character varying ,
flPermission character varying ,

CONSTRAINT PK_ef4439bc04599ea8328d983a467b40b1a8ff1b33 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.z_history_tbRolePermissions (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flRoleId integer ,
flControllerName character varying ,
flPermission character varying ,

CONSTRAINT PK_de8003b8061964dc0e1a00b3cb54616fc201297f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUserRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flRoleId integer ,

CONSTRAINT PK_aa3d862d233176a003b1da6bdb664b7aa3ec3d9f PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.z_history_TbUserRoles (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer ,
flRoleId integer ,

CONSTRAINT PK_5f7e43289bd4fe7721eb9cf98a6acdace6b73b58 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUserBankDetails (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flUserId integer NOT NULL,
flBankBIC character varying(8) NOT NULL,
flKbe character varying(20) ,
flIik character varying(20) NOT NULL,
flNote character varying ,
flRemoved boolean NOT NULL,

CONSTRAINT PK_TbUserBankDetails PRIMARY KEY (
    flId
)

);

CREATE  INDEX "IX_TbUserBankDetails_flUserId" ON users.TbUserBankDetails(flUserId Asc);



CREATE TABLE users.z_history_TbUserBankDetails (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flId integer NOT NULL,
flUserId integer NOT NULL,
flBankBIC character varying(8) NOT NULL,
flKbe character varying(20) ,
flIik character varying(20) NOT NULL,
flNote character varying ,
flRemoved boolean NOT NULL,

CONSTRAINT PK_65c73313cabc346a222c259565dc1d9f7c1d3d27 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUsersExternalProfileData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flBin character varying(12) ,
flGlobalObjectId integer ,
flActivationCode character varying(10) ,
flActivated boolean ,
flPhoneActivationCode character varying(10) ,
flPhoneActivated boolean ,
flUserType character varying(30) ,
flFirstPersonFio character varying(500) ,
flAdrCountry character varying(10) ,
flAdrObl character varying(10) ,
flAdrReg character varying(10) ,
flAdrAdr character varying ,
flDocType character varying(10) ,
flDocNum character varying(30) ,
flDocDate date ,
flDocGivenBy character varying ,

CONSTRAINT PK_TbUsersExternalProfileData PRIMARY KEY (
    flUserId
)

);




CREATE TABLE users.z_history_TbUsersExternalProfileData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flBin character varying(12) ,
flGlobalObjectId integer ,
flActivationCode character varying(10) ,
flActivated boolean ,
flPhoneActivationCode character varying(10) ,
flPhoneActivated boolean ,
flUserType character varying(30) ,
flFirstPersonFio character varying(500) ,
flAdrCountry character varying(10) ,
flAdrObl character varying(10) ,
flAdrReg character varying(10) ,
flAdrAdr character varying ,
flDocType character varying(10) ,
flDocNum character varying(30) ,
flDocDate date ,
flDocGivenBy character varying ,

CONSTRAINT PK_e9c0c8700bf030cd18661358e61ff519001fd647 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.tbUsersExternalProfileDisplayNames (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flUserFullName character varying NOT NULL,
flCorpName character varying ,
flCorpNameKz character varying ,
flDisplayText character varying NOT NULL,

CONSTRAINT PK_tbUsersExternalProfileDisplayNames PRIMARY KEY (
    flUserId
)

);




CREATE TABLE users.z_history_tbUsersExternalProfileDisplayNames (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flUserFullName character varying NOT NULL,
flCorpName character varying ,
flCorpNameKz character varying ,
flDisplayText character varying NOT NULL,

CONSTRAINT PK_da92c512f8ecb44528c628fefd7447b96eb09fcb PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUserCorpData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flBin character varying(12) NOT NULL,
flName character varying NOT NULL,
flFirstPersonIin character varying(12) NOT NULL,
flFirstPersonFio character varying NOT NULL,
flAdrCountry character varying(10) NOT NULL,
flAdrIndex character varying ,
flAdrObl character varying(10) NOT NULL,
flAdrReg character varying(10) NOT NULL,
flAdrAdr character varying NOT NULL,
flWorkPhone character varying ,
flMobilePhone character varying NOT NULL,
flIsMobilePhoneActivated boolean NOT NULL,
flEmail character varying NOT NULL,
flIsEmailActivated boolean NOT NULL,

CONSTRAINT PK_TbUserCorpData_flBin PRIMARY KEY (
    flBin
)

);




CREATE TABLE users.z_history_TbUserCorpData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flBin character varying(12) NOT NULL,
flName character varying NOT NULL,
flFirstPersonIin character varying(12) NOT NULL,
flFirstPersonFio character varying NOT NULL,
flAdrCountry character varying(10) NOT NULL,
flAdrIndex character varying ,
flAdrObl character varying(10) NOT NULL,
flAdrReg character varying(10) NOT NULL,
flAdrAdr character varying NOT NULL,
flWorkPhone character varying ,
flMobilePhone character varying NOT NULL,
flIsMobilePhoneActivated boolean NOT NULL,
flEmail character varying NOT NULL,
flIsEmailActivated boolean NOT NULL,

CONSTRAINT PK_a591bd9ddeec2099c587b574a7322a0c71b2a656 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUsersInternalProfileData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flOrgBin character varying(12) ,
flWorkPost character varying(25) ,
flWorkPostNote character varying ,
flInviteSended boolean NOT NULL,
flInviteDate timestamp without time zone ,
flInviteCode character varying ,
flInviteAccepted boolean ,
flActiveTo date ,

CONSTRAINT PK_TbUsersInternalProfileData PRIMARY KEY (
    flUserId
)

);




CREATE TABLE users.z_history_TbUsersInternalProfileData (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flOrgBin character varying(12) ,
flWorkPost character varying(25) ,
flWorkPostNote character varying ,
flInviteSended boolean NOT NULL,
flInviteDate timestamp without time zone ,
flInviteCode character varying ,
flInviteAccepted boolean ,
flActiveTo date ,

CONSTRAINT PK_3dda3b7a30c420bcf09c11537bc7b0c1ab80baa4 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE TABLE users.TbUsers (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flLogin character varying(30) NOT NULL,
flIin character varying(12) NOT NULL,
flLastName character varying ,
flFirstName character varying ,
flMiddleName character varying ,
flGroupName character varying ,
flWorkPhone character varying ,
flMobilePhone character varying ,
flIsMobilePhoneConfirmed boolean ,
flEmail character varying NOT NULL,
flPassword character varying ,
flIsBlocked boolean NOT NULL,
flIsDeleted boolean NOT NULL,
flProfile character varying(100) NOT NULL,

CONSTRAINT PK_tbUsers_flUserId PRIMARY KEY (
    flUserId
)

);

CREATE UNIQUE  INDEX "IX_tbUsers_flLoginUnique" ON users.TbUsers(flLogin Asc);



CREATE TABLE users.z_history_TbUsers (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flUserId integer NOT NULL,
flLogin character varying(30) NOT NULL,
flIin character varying(12) NOT NULL,
flLastName character varying ,
flFirstName character varying ,
flMiddleName character varying ,
flGroupName character varying ,
flWorkPhone character varying ,
flMobilePhone character varying ,
flIsMobilePhoneConfirmed boolean ,
flEmail character varying NOT NULL,
flPassword character varying ,
flIsBlocked boolean NOT NULL,
flIsDeleted boolean NOT NULL,
flProfile character varying(100) NOT NULL,

CONSTRAINT PK_55cae888b551506bbf981472b86545e1b80994d9 PRIMARY KEY (
    flEntityId, flRequestId
)

);




CREATE SEQUENCE users."seq_TbDataAccessSettings_flSettingsId";


CREATE SEQUENCE users."seq_TbDataAccessSettingsToTablesMap_flMapId";


CREATE SEQUENCE users."seq_TbDataAccessUserSettings_flId";


CREATE SEQUENCE users."seq_TbRoles_flRoleId";


CREATE SEQUENCE users."seq_TbUserBankDetails_flId";


CREATE SEQUENCE users."seq_TbUsers_flUserId";
