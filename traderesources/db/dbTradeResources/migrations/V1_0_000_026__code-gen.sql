
CREATE SCHEMA jobs;

CREATE TABLE jobs.tbJobs (
flEntityId bigint NOT NULL,
flRequestId bigint NOT NULL,
flIsHidden boolean NOT NULL,
flRequestType integer NOT NULL,
flRequestDate timestamp without time zone NOT NULL,
flRequestUserId integer NOT NULL,
flJobId character varying(32) NOT NULL,
flRowVersion integer NOT NULL,
flQueueName character varying(150) NOT NULL,
flMessage character varying NOT NULL,
flStatus character varying(9) NOT NULL,
flCreatedAt timestamp without time zone NOT NULL,
flStartAfter timestamp without time zone NOT NULL,
flExpireAt timestamp without time zone ,
flStartedAt timestamp without time zone ,
flFinishedAt timestamp without time zone ,
flUpdatedAt timestamp without time zone NOT NULL,
flLastSuccessAt timestamp without time zone ,
flRepeatsCount integer NOT NULL,
flRetriesCount integer NOT NULL,
flDescription character varying ,
flException character varying , 

CONSTRAINT PK_tbJobs PRIMARY KEY (
    flJobId
)

);

CREATE INDEX IX_Status_StartAfter ON jobs.tbJobs USING btree (flStatus, flStartAfter);
