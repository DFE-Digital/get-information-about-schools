/*
GIAS Test public table schema

This  script creates schemas, empty table structures, and table relationships.
It includes column definitions, identity columns, computed-column expressions, primary keys, unique constraints, default constraints and foreign keys.
It excludes users, permissions, roles, certificates, symmetric keys, views, stored procedures, functions, triggers, standalone indexes and full-text indexes/catalogues.
Review before running.
*/

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF SCHEMA_ID(N'DataOpsJobs') IS NULL
    EXEC(N'CREATE SCHEMA [DataOpsJobs]');
GO

IF SCHEMA_ID(N'FrontEnd') IS NULL
    EXEC(N'CREATE SCHEMA [FrontEnd]');
GO

IF SCHEMA_ID(N'gias_sharing') IS NULL
    EXEC(N'CREATE SCHEMA [gias_sharing]');
GO

IF SCHEMA_ID(N'MasterProvider') IS NULL
    EXEC(N'CREATE SCHEMA [MasterProvider]');
GO

CREATE TABLE [DataOpsJobs].[Ofsted_IndependentSchools] (
    [URN]                         NUMERIC (19)  NULL,
    [LastDayOfInspection]         DATE          NULL,
    [LastDayOfInspection_Import]  VARCHAR (255) NULL,
    [OverallEffectiveness]        NUMERIC (4)   NULL,
    [OverallEffectiveness_Import] VARCHAR (50)  NULL
);


GO

CREATE TABLE [DataOpsJobs].[Ofsted_StateFunded] (
    [URN]                                                         NUMERIC (19)  NULL,
    [Doesthesection8inspectionrelatetotheURNofthecurrentschool]   VARCHAR (255) NULL,
    [URNattimeofthesection8inspection]                            NUMERIC (19)  NULL,
    [URNattimeofthesection8inspection_Import]                     VARCHAR (255) NULL,
    [Dateoflatestsection8inspection]                              DATE          NULL,
    [Dateoflatestsection8inspection_IMPORT]                       VARCHAR (255) NULL,
    [Inspectionenddate]                                           DATE          NULL,
    [Inspectionenddate_Import]                                    VARCHAR (255) NULL,
    [DoesthelatestfullinspectionrelatetotheURNofthecurrentschool] VARCHAR (255) NULL,
    [URNattimeoflatestfullinspection]                             NUMERIC (19)  NULL,
    [URNattimeoflatestfullinspection_Import]                      VARCHAR (255) NULL,
    [OverallEffectiveness]                                        VARCHAR (255) NULL,
    [CategoryOfConcern]                                           VARCHAR (255) NULL,
    [Check]                                                       VARCHAR (255) NULL,
    [URN_Amended]                                                 NUMERIC (19)  NULL,
    [Date]                                                        DATE          NULL
);


GO

CREATE TABLE [DataOpsJobs].[DistrictAdministrative_Import] (
    [code] NVARCHAR (64)  NOT NULL,
    [name] NVARCHAR (510) NOT NULL,
    CONSTRAINT [PK__DistrictAdministrative_Import] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[gias_dsi_account_sync] (
    [SyncId]          INT            IDENTITY (1, 1) NOT NULL,
    [Data]            NVARCHAR (MAX) NULL,
    [numberOfRecords] INT            NOT NULL,
    [page]            INT            NOT NULL,
    [numberOfPages]   INT            NOT NULL,
    [SyncDateTime]    DATETIME       NOT NULL,
    CONSTRAINT [PK_DataOpsJobs.gias_dsi_account_sync] PRIMARY KEY CLUSTERED ([SyncId] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[ParliamentaryConstituency_Import] (
    [code] NVARCHAR (64)  NOT NULL,
    [name] NVARCHAR (510) NOT NULL,
    CONSTRAINT [PK__ParliamentaryConstituency_Import] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[MSOA_Import] (
    [code] NVARCHAR (64)  NOT NULL,
    [name] NVARCHAR (510) NOT NULL,
    CONSTRAINT [PK__MSOA_Import] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[GIAS_SchoolCensus_Updates] (
    [Id]           INT          IDENTITY (1, 1) NOT NULL,
    [URN]          NUMERIC (19) NOT NULL,
    [UpdatedField] VARCHAR (50) NOT NULL,
    [UpdatedValue] VARCHAR (50) NOT NULL,
    [RN]           INT          NOT NULL,
    [FileNo]       TINYINT      NULL,
    CONSTRAINT [PK__GIAS_SchoolCensus_Updates] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[Ofsted_FurtherEducation] (
    [URN]                                NUMERIC (19)  NULL,
    [DateOfLatestShortInspection_Import] VARCHAR (255) NULL,
    [DateOfLatestShortInspection]        DATE          NULL,
    [LastDayOfInspection]                DATE          NULL,
    [LastDayOfInspection_Import]         VARCHAR (50)  NULL,
    [OverallEffectiveness]               NUMERIC (4)   NULL,
    [OverallEffectiveness_Import]        VARCHAR (50)  NULL,
    [InspectionDate]                     DATE          NULL
);


GO

CREATE TABLE [DataOpsJobs].[GIAS_UpdatedExclusionList_2024_Nov] (
    [URN]         NUMERIC (19)  NOT NULL,
    [ArcMap_PCON] NVARCHAR (64) NOT NULL,
    CONSTRAINT [PK_GIAS_UpdatedExclusionList_2024_Nov] PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[LSOA_Import] (
    [code] NVARCHAR (64)  NOT NULL,
    [name] NVARCHAR (510) NOT NULL,
    CONSTRAINT [PK__LSOA_Import] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[GeoData_Import] (
    [postcode]                       NVARCHAR (255) NOT NULL,
    [administrativeWard_code]        NVARCHAR (32)  NULL,
    [casWard_code]                   NVARCHAR (32)  NULL,
    [lsoa_code]                      NVARCHAR (32)  NULL,
    [msoa_code]                      NVARCHAR (32)  NULL,
    [parliamentaryConstituency_code] NVARCHAR (32)  NULL,
    [districtAdministrative_code]    NVARCHAR (32)  NULL,
    [urbanRural_code]                NVARCHAR (32)  NULL,
    [lsoa_code2021]                  NVARCHAR (32)  NULL,
    [msoa_code2021]                  NVARCHAR (32)  NULL,
    [urbanRural_code2021]            NVARCHAR (32)  NULL,
    CONSTRAINT [PK__GeoData_Import] PRIMARY KEY CLUSTERED ([postcode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[ONSPD_URN_ExclusionList] (
    [URN]       NUMERIC (19) NOT NULL,
    [DateAdded] DATETIME     NOT NULL,
    CONSTRAINT [PK__ONSPD_URN_ExclusionList] PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[GeoData_AUD] (
    [ImportDate] DATETIME      NOT NULL,
    [URN]        NUMERIC (19)  NOT NULL,
    [Postcode]   NVARCHAR (8)  NOT NULL,
    [DataChange] NVARCHAR (32) NOT NULL,
    [OLD_Value]  NVARCHAR (32) NULL,
    [NEW_Value]  NVARCHAR (32) NULL,
    CONSTRAINT [PK__GeoData_AUD] PRIMARY KEY CLUSTERED ([ImportDate] ASC, [URN] ASC, [DataChange] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[AdministrativeWard_Import] (
    [code] NVARCHAR (64)  NOT NULL,
    [name] NVARCHAR (510) NOT NULL,
    CONSTRAINT [PK__AdministrativeWard_Import] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[Ofsted_GIASProdExtract] (
    [URN]                  NUMERIC (19) NULL,
    [OfstedLastInspection] DATE         NULL,
    [OfstedRating]         VARCHAR (64) NULL,
    [GIAS_OfstedDate]      DATE         NULL,
    [GIAS_OfstedRating]    VARCHAR (64) NULL,
    [CheckDate]            BIT          NULL,
    [CheckRating]          BIT          NULL,
    [CheckBoth]            BIT          NULL
);


GO

CREATE TABLE [DataOpsJobs].[GIAS_IEBTSchoolCensus] (
    [URN]                     NUMERIC (19)   NOT NULL,
    [AccomChange]             NVARCHAR (32)  NULL,
    [Street]                  NVARCHAR (100) NULL,
    [Locality]                NVARCHAR (100) NULL,
    [Address3]                NVARCHAR (100) NULL,
    [Town]                    NVARCHAR (100) NULL,
    [County]                  NVARCHAR (32)  NULL,
    [Postcode]                NVARCHAR (8)   NULL,
    [CharityName]             NVARCHAR (255) NULL,
    [CharityNum]              INT            NULL,
    [EstablishmentName]       NVARCHAR (100) NULL,
    [HeadFirstName]           NVARCHAR (100) NULL,
    [HeadLastName]            NVARCHAR (100) NULL,
    [HeadTitle]               NVARCHAR (32)  NULL,
    [HighBoardFee]            INT            NULL,
    [HighDayFee]              INT            NULL,
    [LowBoardFee]             INT            NULL,
    [LowDayFee]               INT            NULL,
    [MainEmail]               NVARCHAR (100) NULL,
    [NumberOfPupils]          NVARCHAR (10)  NULL,
    [SENNoStat]               INT            NULL,
    [SENStat]                 INT            NULL,
    [PTBoys2andUnder]         INT            NULL,
    [PTBoys3]                 INT            NULL,
    [PTBoys4a]                INT            NULL,
    [PTBoys4b]                INT            NULL,
    [PTBoys4c]                INT            NULL,
    [PTGirls2andUnder]        INT            NULL,
    [PTGirls3]                INT            NULL,
    [PTGirls4a]               INT            NULL,
    [PTGirls4b]               INT            NULL,
    [PTGirls4c]               INT            NULL,
    [TotalBoyBoarders]        INT            NULL,
    [TotalPupilsFullTime]     INT            NULL,
    [TotalStaffFullTime]      INT            NULL,
    [TotalGirlBoarders]       INT            NULL,
    [TotalPupilsPartTime]     INT            NULL,
    [TotalStaffPartTime]      INT            NULL,
    [CompSchoolAge]           INT            NULL,
    [TotalPupilsInPublicCare] INT            NULL,
    CONSTRAINT [PK__GIAS_IEBTSchoolCensus] PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[GIAS_IEBTSchoolCensus_Updates] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [URN]          NUMERIC (19)  NOT NULL,
    [UpdatedField] VARCHAR (50)  NOT NULL,
    [UpdatedValue] VARCHAR (500) NOT NULL,
    [RN]           INT           NOT NULL,
    [FileNo]       TINYINT       NULL,
    CONSTRAINT [PK__GIAS_IEBTSchoolCensus_Updates] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[pcd_pcon_uk_Import] (
    [postcode] NVARCHAR (255) NOT NULL,
    [code]     NVARCHAR (64)  NULL,
    [name]     NVARCHAR (510) NULL,
    CONSTRAINT [PK__pcd_pcon_uk_Import] PRIMARY KEY CLUSTERED ([postcode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[GIAS_SchoolCensus] (
    [URN]            NUMERIC (19)  NOT NULL,
    [CensusDate]     DATE          NOT NULL,
    [NumberOfGirls]  INT           NOT NULL,
    [NumberOfBoys]   INT           NOT NULL,
    [NumberOfPupils] INT           NOT NULL,
    [FSM]            NVARCHAR (20) NULL,
    [PercentageFSM]  NVARCHAR (20) NULL,
    CONSTRAINT [PK__GIAS_SchoolCensus] PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [DataOpsJobs].[gias_dsi_account_sync_Log] (
    [LogId]              INT           IDENTITY (1, 1) NOT NULL,
    [SyncId]             INT           NOT NULL,
    [userId]             VARCHAR (200) NULL,
    [roleName]           VARCHAR (200) NULL,
    [email]              VARCHAR (200) NULL,
    [givenName]          VARCHAR (200) NULL,
    [familyName]         VARCHAR (200) NULL,
    [ProcessingDateTime] DATETIME      NOT NULL,
    [DisabledInGIAS]     BIT           NOT NULL,
    CONSTRAINT [PK_DataOpsJobs.gias_dsi_account_sync_Log] PRIMARY KEY CLUSTERED ([LogId] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[NotificationTemplates] (
    [PartitionKey] VARCHAR (64)   NOT NULL,
    [RowKey]       VARCHAR (64)   NOT NULL,
    [Timestamp]    DATETIME2 (7)  NOT NULL,
    [Content]      VARCHAR (2048) NOT NULL,
    CONSTRAINT [PK_FrontEnd_NotificationTemplates] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[GlossaryItems] (
    [PartitionKey] VARCHAR (64)   NOT NULL,
    [RowKey]       VARCHAR (64)   NOT NULL,
    [Timestamp]    DATETIME2 (7)  NOT NULL,
    [Title]        VARCHAR (512)  NOT NULL,
    [Content]      VARCHAR (4096) NOT NULL,
    CONSTRAINT [PK_FrontEnd_GlossaryItems] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[Tokens] (
    [PartitionKey] VARCHAR (64)   NOT NULL,
    [RowKey]       VARCHAR (64)   NOT NULL,
    [Timestamp]    DATETIME2 (7)  NOT NULL,
    [Data]         VARCHAR (2048) NOT NULL,
    CONSTRAINT [PK_FrontEnd_Tokens] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[UserPreferences] (
    [PartitionKey]     VARCHAR (64)  NOT NULL,
    [RowKey]           VARCHAR (64)  NOT NULL,
    [Timestamp]        DATETIME2 (7) NOT NULL,
    [SavedSearchToken] VARCHAR (256) NULL,
    CONSTRAINT [PK_FrontEnd_UserPreferences] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[FaqItems] (
    [PartitionKey] VARCHAR (64)   NOT NULL,
    [RowKey]       VARCHAR (64)   NOT NULL,
    [Timestamp]    DATETIME2 (7)  NOT NULL,
    [Content]      VARCHAR (4096) NOT NULL,
    [DisplayOrder] INT            NOT NULL,
    [GroupId]      VARCHAR (128)  NOT NULL,
    [Title]        VARCHAR (512)  NOT NULL,
    CONSTRAINT [PK_FrontEnd_FaqItems] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[AZTLoggerMessages] (
    [PartitionKey]    VARCHAR (64)   NOT NULL,
    [RowKey]          VARCHAR (64)   NOT NULL,
    [Timestamp]       DATETIME2 (7)  NOT NULL,
    [ClientIpAddress] VARCHAR (32)   NULL,
    [HttpMethod]      VARCHAR (16)   NULL,
    [ReferrerUrl]     VARCHAR (2048) NULL,
    [RequestJsonBody] VARCHAR (MAX)  NULL,
    [Url]             VARCHAR (2048) NULL,
    [UserAgent]       VARCHAR (1024) NULL,
    [DateUtc]         DATETIME2 (7)  NOT NULL,
    [Environment]     VARCHAR (32)   NULL,
    [Exception]       VARCHAR (MAX)  NULL,
    [Message]         VARCHAR (MAX)  NOT NULL,
    [UserId]          VARCHAR (128)  NULL,
    [Username]        VARCHAR (256)  NULL,
    CONSTRAINT [PK_FrontEnd_AZTLoggerMessages] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[FaqGroups] (
    [PartitionKey] VARCHAR (64)  NOT NULL,
    [RowKey]       VARCHAR (64)  NOT NULL,
    [Timestamp]    DATETIME2 (7) NOT NULL,
    [DisplayOrder] INT           NOT NULL,
    [GroupName]    VARCHAR (128) NOT NULL,
    CONSTRAINT [PK_FrontEnd_FaqGroups] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[DataQualityStatus] (
    [PartitionKey] VARCHAR (64)  NOT NULL,
    [RowKey]       VARCHAR (64)  NOT NULL,
    [Timestamp]    DATETIME2 (7) NOT NULL,
    [DataOwner]    VARCHAR (512) NOT NULL,
    [Email]        VARCHAR (512) NOT NULL,
    [LastUpdated]  DATE          NOT NULL,
    CONSTRAINT [PK_FrontEnd_DataQualityStatus] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[NewsArticles] (
    [PartitionKey]   VARCHAR (64)  NOT NULL,
    [RowKey]         VARCHAR (64)  NOT NULL,
    [Timestamp]      DATETIME2 (7) NOT NULL,
    [Title]          VARCHAR (200) NOT NULL,
    [ArticleDate]    DATETIME2 (7) NOT NULL,
    [ShowDate]       BIT           NOT NULL,
    [Content]        VARCHAR (MAX) NOT NULL,
    [Version]        TINYINT       NOT NULL,
    [Tracker]        VARCHAR (64)  NOT NULL,
    [AuditUser]      INT           NOT NULL,
    [AuditEvent]     VARCHAR (32)  NOT NULL,
    [AuditTimestamp] DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_FrontEnd_NewsArticles] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[LocalAuthoritySets] (
    [PartitionKey] VARCHAR (64)  NOT NULL,
    [RowKey]       VARCHAR (64)  NOT NULL,
    [Timestamp]    DATETIME2 (7) NOT NULL,
    [IdData]       VARCHAR (512) NOT NULL,
    [Title]        VARCHAR (64)  NOT NULL,
    CONSTRAINT [PK_FrontEnd_LocalAuthoritySets] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[NotificationBanners] (
    [PartitionKey]   VARCHAR (64)   NOT NULL,
    [RowKey]         VARCHAR (64)   NOT NULL,
    [Timestamp]      DATETIME2 (7)  NOT NULL,
    [Importance]     TINYINT        NOT NULL,
    [Content]        VARCHAR (2048) NOT NULL,
    [Start]          DATETIME2 (7)  NOT NULL,
    [End]            DATETIME2 (7)  NOT NULL,
    [Version]        TINYINT        NOT NULL,
    [Tracker]        VARCHAR (64)   NOT NULL,
    [AuditUser]      INT            NOT NULL,
    [AuditEvent]     VARCHAR (32)   NOT NULL,
    [AuditTimestamp] DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_FrontEnd_NotificationBanners] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [FrontEnd].[ApiRecorderSessionItems] (
    [PartitionKey]    VARCHAR (64)   NOT NULL,
    [RowKey]          VARCHAR (256)  NOT NULL,
    [Timestamp]       DATETIME2 (7)  NOT NULL,
    [HttpMethod]      VARCHAR (10)   NULL,
    [Path]            VARCHAR (1024) NULL,
    [RequestHeaders]  VARCHAR (MAX)  NULL,
    [ResponseHeaders] VARCHAR (MAX)  NULL,
    [RawRequestBody]  VARCHAR (MAX)  NULL,
    [RawResponseBody] VARCHAR (MAX)  NULL,
    [ElapsedTimeSpan] VARCHAR (50)   NULL,
    [ElapsedMS]       FLOAT (53)     NULL,
    CONSTRAINT [PK_FrontEnd_ApiRecorderSessionItems] PRIMARY KEY CLUSTERED ([PartitionKey] ASC, [RowKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[establishment_change_history_cache] (
    [request_type]                            NVARCHAR (64)   NULL,
    [id]                                      NUMERIC (19)    NOT NULL,
    [change_date]                             DATETIME        NULL,
    [due_date]                                DATETIME        NULL,
    [last_change_date]                        DATETIME        NULL,
    [status]                                  NVARCHAR (255)  NULL,
    [string_new_value]                        NVARCHAR (4000) NULL,
    [string_old_value]                        NVARCHAR (4000) NULL,
    [numeric_new_value]                       NUMERIC (19)    NULL,
    [numeric_old_value]                       NUMERIC (19)    NULL,
    [date_new_value]                          DATETIME        NULL,
    [date_old_value]                          DATETIME        NULL,
    [approved_or_rejected_by_user_username]   NVARCHAR (255)  NULL,
    [data_owner_code]                         NVARCHAR (32)   NULL,
    [establishment_urn]                       NUMERIC (19)    NULL,
    [field_short_name]                        NVARCHAR (32)   NULL,
    [proposed_by_user_username]               NVARCHAR (255)  NULL,
    [proposer_id]                             NUMERIC (19)    NULL,
    [change_reason]                           NVARCHAR (255)  NULL,
    [changed_by_bo]                           TINYINT         NULL,
    [changed_in_error]                        TINYINT         NULL,
    [delay_reason]                            NVARCHAR (255)  NULL,
    [proposed_by_external_user]               TINYINT         NULL,
    [reject_reason]                           NVARCHAR (1024) NULL,
    [link_urn]                                NUMERIC (19)    NULL,
    [created_date]                            DATETIME        NULL,
    [version]                                 INT             NOT NULL,
    [effective_date]                          DATETIME        NULL,
    [approved_or_rejected_by_user_group_code] NVARCHAR (32)   NULL,
    [proposed_by_user_group_code]             NVARCHAR (32)   NULL,
    [same_user]                               CHAR (18)       NULL,
    [additional_address_number]               INT             NULL,
    [additional_proprietor_number]            INT             NULL,
    [data_owner_name]                         NVARCHAR (255)  NULL,
    [proposed_by_user_group_name]             NVARCHAR (255)  NULL,
    [approved_or_rejected_by_user_group_name] NVARCHAR (255)  NULL,
    [row_inserted_datetime]                   DATETIME        NULL,
    [row_inserted_by]                         NVARCHAR (255)  NULL,
    [row_updated_datetime]                    DATETIME        NULL,
    [row_updated_by]                          NVARCHAR (255)  NULL,
    CONSTRAINT [PK_establishment_change_history_cache] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[staff_record_change_history_cache] (
    [request_type]                     NVARCHAR (31)   NULL,
    [id]                               NUMERIC (19)    NOT NULL,
    [created_date]                     DATETIME        NULL,
    [effective_date]                   DATETIME        NULL,
    [last_change_date]                 DATETIME        NULL,
    [reject_reason]                    NVARCHAR (2048) NULL,
    [status]                           NVARCHAR (255)  NULL,
    [version]                          INT             NOT NULL,
    [date_new_value]                   DATETIME        NULL,
    [date_old_value]                   DATETIME        NULL,
    [lookup_new_value]                 NVARCHAR (255)  NULL,
    [lookup_old_value]                 NVARCHAR (255)  NULL,
    [string_new_value]                 NVARCHAR (255)  NULL,
    [string_old_value]                 NVARCHAR (255)  NULL,
    [approved_or_rejected_by_username] NVARCHAR (255)  NULL,
    [proposed_by_username]             NVARCHAR (255)  NULL,
    [field_short_name]                 NVARCHAR (32)   NULL,
    [staff_record_uid]                 NUMERIC (19)    NULL,
    [long_new_value]                   NUMERIC (19)    NULL,
    [long_old_value]                   NUMERIC (19)    NULL,
    [urn]                              NUMERIC (19)    NULL,
    [gid]                              NUMERIC (19)    NULL,
    [group_id]                         NVARCHAR (255)  NULL,
    [row_inserted_datetime]            DATETIME        NULL,
    [row_inserted_by]                  NVARCHAR (255)  NULL,
    [row_updated_datetime]             DATETIME        NULL,
    [row_updated_by]                   NVARCHAR (255)  NULL,
    CONSTRAINT [PK_staff_record_change_history_cache] PRIMARY KEY CLUSTERED ([id] ASC, [version] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[establishment_group_cache] (
    [gid]                               NUMERIC (19)    NOT NULL,
    [name]                              NVARCHAR (255)  NULL,
    [open_date]                         DATETIME        NULL,
    [group_type_code]                   NVARCHAR (32)   NULL,
    [group_fax_number]                  NVARCHAR (255)  NULL,
    [group_fax_std]                     NVARCHAR (255)  NULL,
    [head_first_name]                   NVARCHAR (255)  NULL,
    [head_honours]                      NVARCHAR (255)  NULL,
    [head_last_name]                    NVARCHAR (255)  NULL,
    [head_preferred_job_title]          NVARCHAR (255)  NULL,
    [group_telephone_number]            NVARCHAR (255)  NULL,
    [group_telephone_std]               NVARCHAR (255)  NULL,
    [closed_date]                       DATETIME        NULL,
    [group_address_3]                   NVARCHAR (100)  NULL,
    [group_locality]                    NVARCHAR (255)  NULL,
    [group_postcode]                    NVARCHAR (255)  NULL,
    [group_street]                      NVARCHAR (255)  NULL,
    [group_town]                        NVARCHAR (255)  NULL,
    [group_contact_fax_number]          NVARCHAR (255)  NULL,
    [group_contact_fax_std]             NVARCHAR (255)  NULL,
    [group_contact_first_name]          NVARCHAR (255)  NULL,
    [group_contact_honours]             NVARCHAR (255)  NULL,
    [group_contact_last_name]           NVARCHAR (255)  NULL,
    [group_contact_preferred_job_title] NVARCHAR (255)  NULL,
    [group_contact_telephone_number]    NVARCHAR (255)  NULL,
    [group_contact_telephone_std]       NVARCHAR (255)  NULL,
    [group_contact_address_3]           NVARCHAR (100)  NULL,
    [group_contact_locality]            NVARCHAR (255)  NULL,
    [group_contact_postcode]            NVARCHAR (255)  NULL,
    [group_contact_street]              NVARCHAR (255)  NULL,
    [group_contact_town]                NVARCHAR (255)  NULL,
    [head_title_code]                   NVARCHAR (32)   NULL,
    [group_county_code]                 NVARCHAR (32)   NULL,
    [group_contact_title_code]          NVARCHAR (32)   NULL,
    [group_contact_county_code]         NVARCHAR (32)   NULL,
    [group_contact_type_code]           NVARCHAR (32)   NULL,
    [email]                             NVARCHAR (255)  NULL,
    [created_in_error]                  TINYINT         NULL,
    [shared_head_teacher]               TINYINT         NULL,
    [companies_house_number]            NVARCHAR (255)  NULL,
    [group_id]                          NVARCHAR (255)  NULL,
    [group_contact_email]               NVARCHAR (255)  NULL,
    [local_authority_code]              NVARCHAR (32)   NULL,
    [sa_org_id]                         NUMERIC (19)    NULL,
    [governance_last_check]             DATETIME        NULL,
    [delegate_auth_details]             NVARCHAR (1000) NULL,
    [group_country_code]                NVARCHAR (32)   NULL,
    [group_contact_country_code]        NVARCHAR (32)   NULL,
    [group_post_uprn]                   NVARCHAR (255)  NULL,
    [group_contact_uprn]                NVARCHAR (255)  NULL,
    [group_post_site_name]              NVARCHAR (255)  NULL,
    [group_contact_site_name]           NVARCHAR (255)  NULL,
    [corporate_contact]                 NVARCHAR (255)  NULL,
    [ukprn]                             NUMERIC (19)    NULL,
    [group_type_name]                   NVARCHAR (255)  NULL,
    [local_authority_name]              NVARCHAR (255)  NULL,
    [group_country_name]                NVARCHAR (255)  NULL,
    [group_county_name]                 NVARCHAR (255)  NULL,
    [group_contact_title_name]          NVARCHAR (255)  NULL,
    [group_contact_county_name]         NVARCHAR (255)  NULL,
    [group_contact_country_name]        NVARCHAR (255)  NULL,
    [group_contact_type_name]           NVARCHAR (255)  NULL,
    [head_title_name]                   NVARCHAR (255)  NULL,
    [version]                           INT             NOT NULL,
    [last_changed_date]                 DATETIME        NULL,
    [incorporated_on_date]              DATETIME        NULL,
    [group_status_code]                 NVARCHAR (32)   NULL,
    [group_status_name]                 NVARCHAR (255)  NULL,
    [row_inserted_datetime]             DATETIME        NULL,
    [row_inserted_by]                   NVARCHAR (255)  NULL,
    [row_updated_datetime]              DATETIME        NULL,
    [row_updated_by]                    NVARCHAR (255)  NULL,
    CONSTRAINT [PK_establishment_group_cache] PRIMARY KEY CLUSTERED ([gid] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[staff_record_cache] (
    [appointment_date]                  DATETIME       NULL,
    [forename_1]                        NVARCHAR (50)  NULL,
    [forename_2]                        NVARCHAR (50)  NULL,
    [phone]                             NVARCHAR (40)  NULL,
    [postcode]                          NVARCHAR (8)   NULL,
    [stepdown_date]                     DATETIME       NULL,
    [surname]                           NVARCHAR (50)  NULL,
    [establishment_urn]                 NUMERIC (19)   NULL,
    [gid]                               NUMERIC (19)   NULL,
    [staff_appointing_body_code]        NVARCHAR (32)  NULL,
    [staff_role_code]                   NVARCHAR (32)  NULL,
    [title_code]                        NVARCHAR (32)  NULL,
    [deleted]                           TINYINT        NULL,
    [forced_archived]                   TINYINT        NULL,
    [status]                            CHAR (18)      NULL,
    [shared]                            CHAR (18)      NULL,
    [staff_role_name]                   NVARCHAR (255) NULL,
    [staff_appointing_body_name]        NVARCHAR (255) NULL,
    [title_name]                        NVARCHAR (255) NULL,
    [status_name]                       NVARCHAR (255) NULL,
    [uid]                               NUMERIC (19)   NOT NULL,
    [parent_establishment_urn]          NUMERIC (19)   NULL,
    [parent_gid]                        NUMERIC (19)   NULL,
    [parent_appointment_date]           DATETIME       NULL,
    [parent_stepdown_date]              DATETIME       NULL,
    [parent_status]                     CHAR (18)      NULL,
    [parent_status_name]                NVARCHAR (255) NULL,
    [parent_staff_role_code]            NVARCHAR (32)  NULL,
    [parent_staff_role_name]            NVARCHAR (255) NULL,
    [parent_staff_appointing_body_code] NVARCHAR (32)  NULL,
    [parent_staff_appointing_body_name] NVARCHAR (255) NULL,
    [parent_title_code]                 NVARCHAR (32)  NULL,
    [parent_title_name]                 NVARCHAR (255) NULL,
    [parent_forename_1]                 NVARCHAR (50)  NULL,
    [parent_forename_2]                 NVARCHAR (50)  NULL,
    [parent_phone]                      NVARCHAR (40)  NULL,
    [parent_postcode]                   NVARCHAR (8)   NULL,
    [parent_surname]                    NVARCHAR (50)  NULL,
    [parent_deleted]                    TINYINT        NULL,
    [parent_shared]                     CHAR (18)      NULL,
    [parent_forced_archived]            TINYINT        NULL,
    [birth_date]                        DATETIME       NULL,
    [direct_email]                      NVARCHAR (255) NULL,
    [prev_title_code]                   NVARCHAR (32)  NULL,
    [prev_forename_1]                   NVARCHAR (255) NULL,
    [prev_forename_2]                   NVARCHAR (255) NULL,
    [prev_surname]                      NVARCHAR (255) NULL,
    [prev_title_name]                   NVARCHAR (255) NULL,
    [parent_birth_date]                 DATETIME       NULL,
    [parent_direct_email]               NVARCHAR (255) NULL,
    [parent_prev_title_code]            NVARCHAR (32)  NULL,
    [parent_prev_title_name]            NVARCHAR (255) NULL,
    [parent_prev_forename_1]            NVARCHAR (255) NULL,
    [parent_prev_forename_2]            NVARCHAR (255) NULL,
    [parent_prev_surname]               NVARCHAR (255) NULL,
    [version]                           INT            NOT NULL,
    [row_inserted_datetime]             DATETIME       NULL,
    [row_inserted_by]                   NVARCHAR (255) NULL,
    [row_updated_datetime]              DATETIME       NULL,
    [row_updated_by]                    NVARCHAR (255) NULL,
    CONSTRAINT [PK_staff_record_cache] PRIMARY KEY CLUSTERED ([uid] ASC, [version] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[establishment_cache] (
    [last_changed_date]                              DATETIME        NULL,
    [ukprn]                                          NUMERIC (19)    NULL,
    [establishment_number]                           INT             NULL,
    [previous_establishment_number]                  INT             NULL,
    [fehe_identifier]                                INT             NULL,
    [version]                                        INT             NOT NULL,
    [establishment_name]                             NVARCHAR (100)  NULL,
    [created_date]                                   DATETIME        NULL,
    [open_date]                                      DATETIME        NULL,
    [close_date]                                     DATETIME        NULL,
    [reason_establishment_opened_code]               NVARCHAR (32)   NULL,
    [establishment_type_code]                        NVARCHAR (32)   NULL,
    [status_code]                                    NVARCHAR (32)   NULL,
    [change_reason_code]                             NVARCHAR (32)   NULL,
    [reason_establishment_closed_code]               NVARCHAR (32)   NULL,
    [education_phase_code]                           NVARCHAR (32)   NULL,
    [gender_code]                                    NVARCHAR (32)   NULL,
    [further_education_type_code]                    NVARCHAR (32)   NULL,
    [statutory_low_age]                              INT             NULL,
    [statutory_high_age]                             INT             NULL,
    [school_capacity]                                INT             NULL,
    [street]                                         NVARCHAR (100)  NULL,
    [town]                                           NVARCHAR (100)  NULL,
    [address_3]                                      NVARCHAR (100)  NULL,
    [postcode]                                       NVARCHAR (8)    NULL,
    [locality]                                       NVARCHAR (100)  NULL,
    [easting]                                        INT             NULL,
    [northing]                                       INT             NULL,
    [fax_std]                                        NVARCHAR (20)   NULL,
    [fax_number]                                     NVARCHAR (40)   NULL,
    [s_2_s_email]                                    NVARCHAR (100)  NULL,
    [scu_preferred_email]                            NVARCHAR (100)  NULL,
    [scu_alternative_email]                          NVARCHAR (100)  NULL,
    [website_address]                                NVARCHAR (200)  NULL,
    [telephone_std]                                  NVARCHAR (20)   NULL,
    [telephone_number]                               NVARCHAR (40)   NULL,
    [number_of_boys]                                 NVARCHAR (10)   NULL,
    [number_of_pupils]                               NVARCHAR (10)   NULL,
    [number_of_girls]                                NVARCHAR (10)   NULL,
    [lowest_full_time_age]                           INT             NULL,
    [edubase_last_update]                            DATETIME        NULL,
    [s_2_s_login_code]                               NVARCHAR (32)   NULL,
    [highest_full_time_age]                          INT             NULL,
    [timestamp]                                      ROWVERSION      NULL,
    [converted]                                      TINYINT         NULL,
    [establishment_name_index]                       NVARCHAR (100)  NULL,
    [special_pupils]                                 INT             NULL,
    [ey_government_funded_children]                  INT             NULL,
    [approved_number_boarders_special]               INT             NULL,
    [teen_mothers_places]                            INT             NULL,
    [resourced_provision_capacity]                   INT             NULL,
    [resourced_provision_on_roll]                    INT             NULL,
    [sen_unit_capacity]                              INT             NULL,
    [sen_unit_on_roll]                               INT             NULL,
    [number_of_under_5s]                             INT             NULL,
    [head_first_name]                                NVARCHAR (100)  NULL,
    [head_last_name]                                 NVARCHAR (100)  NULL,
    [head_preferred_job_title]                       NVARCHAR (50)   NULL,
    [head_honours]                                   NVARCHAR (20)   NULL,
    [head_appointment_date]                          DATETIME        NULL,
    [head_email]                                     NVARCHAR (100)  NULL,
    [total_pupils_three_month]                       NUMERIC (19)    NULL,
    [uprn]                                           NVARCHAR (255)  NULL,
    [lsoa_code]                                      NVARCHAR (32)   NULL,
    [llsc_code]                                      NVARCHAR (32)   NULL,
    [urban_rural_code]                               NVARCHAR (32)   NULL,
    [la_code]                                        NVARCHAR (32)   NULL,
    [msoa_code]                                      NVARCHAR (32)   NULL,
    [county_code]                                    NVARCHAR (32)   NULL,
    [head_title_code]                                NVARCHAR (32)   NULL,
    [cas_ward_code]                                  NVARCHAR (32)   NULL,
    [ttwa_code]                                      NVARCHAR (32)   NULL,
    [previous_la_code]                               NVARCHAR (32)   NULL,
    [administrative_ward_code]                       NVARCHAR (32)   NULL,
    [district_administrative_code]                   NVARCHAR (32)   NULL,
    [parliamentary_constituency_code]                NVARCHAR (32)   NULL,
    [gor_code]                                       NVARCHAR (32)   NULL,
    [trust_school_flag_name]                         NVARCHAR (255)  NULL,
    [trust_school_code]                              NVARCHAR (32)   NULL,
    [gss_la_code_code]                               NVARCHAR (32)   NULL,
    [country_code]                                   NVARCHAR (32)   NULL,
    [last_inspection_visit]                          DATETIME        NULL,
    [next_inspection_visit]                          DATETIME        NULL,
    [ofsted_last_inspection]                         DATETIME        NULL,
    [special_educational_needs_code]                 NVARCHAR (32)   NULL,
    [training_establishment_code]                    NVARCHAR (32)   NULL,
    [second_main_specialism_code]                    NVARCHAR (32)   NULL,
    [first_main_specialism_code]                     NVARCHAR (32)   NULL,
    [first_secondary_specialism_code]                NVARCHAR (32)   NULL,
    [second_secondary_specialism_code]               NVARCHAR (32)   NULL,
    [emotional_and_behavioural_difficulties_code]    NVARCHAR (32)   NULL,
    [leading_option_1_code]                          NVARCHAR (32)   NULL,
    [leading_option_3_code]                          NVARCHAR (32)   NULL,
    [excellence_in_cities_action_zone_code]          NVARCHAR (32)   NULL,
    [special_classes_code]                           NVARCHAR (32)   NULL,
    [leadership_incentive_code]                      NVARCHAR (32)   NULL,
    [learning_support_unit_code]                     NVARCHAR (32)   NULL,
    [private_finance_initiative_code]                NVARCHAR (32)   NULL,
    [official_sixth_form_code]                       NVARCHAR (32)   NULL,
    [education_action_zone_code]                     NVARCHAR (32)   NULL,
    [nursery_provision_code]                         NVARCHAR (32)   NULL,
    [early_excellence_code]                          NVARCHAR (32)   NULL,
    [boarders_code]                                  NVARCHAR (32)   NULL,
    [registered_early_years_code]                    NVARCHAR (32)   NULL,
    [excellence_in_cities_group_code]                NVARCHAR (32)   NULL,
    [leading_option_2_code]                          NVARCHAR (32)   NULL,
    [excellence_in_cities_city_learning_centre_code] NVARCHAR (32)   NULL,
    [excellence_in_cities_code]                      NVARCHAR (32)   NULL,
    [education_by_others_code]                       NVARCHAR (32)   NULL,
    [teenaged_mothers_code]                          NVARCHAR (32)   NULL,
    [child_care_facilities_code]                     NVARCHAR (32)   NULL,
    [investor_in_people_code]                        NVARCHAR (32)   NULL,
    [ft_provision_code]                              NVARCHAR (32)   NULL,
    [beacon_code]                                    NVARCHAR (32)   NULL,
    [religious_ethos_code]                           NVARCHAR (32)   NULL,
    [childrens_centres_phase_type_code]              NVARCHAR (32)   NULL,
    [direct_provision_of_early_years_code]           NVARCHAR (32)   NULL,
    [disadvantaged_area_code]                        NVARCHAR (32)   NULL,
    [governance_code]                                NVARCHAR (32)   NULL,
    [operational_hours_code]                         NVARCHAR (32)   NULL,
    [governors_flag_code]                            NVARCHAR (32)   NULL,
    [governance_detail]                              NVARCHAR (255)  NULL,
    [governance_last_check]                          DATETIME        NULL,
    [marketing_opt_in_out_code]                      NVARCHAR (32)   NULL,
    [burnham_report_code]                            NVARCHAR (32)   NULL,
    [contact_preference_code]                        NVARCHAR (32)   NULL,
    [superannuation_category_code]                   NVARCHAR (32)   NULL,
    [ofsted_special_measures_code]                   NVARCHAR (32)   NULL,
    [fresh_start_code]                               NVARCHAR (32)   NULL,
    [admissions_policy_code]                         NVARCHAR (32)   NULL,
    [religious_character_code]                       NVARCHAR (32)   NULL,
    [diocese_code]                                   NVARCHAR (32)   NULL,
    [free_school_meals]                              NVARCHAR (10)   NULL,
    [moved_to_edubase_login_code]                    NVARCHAR (32)   NULL,
    [type_of_reserved_provision_code]                NVARCHAR (32)   NULL,
    [contact_type_code]                              NVARCHAR (32)   NULL,
    [inspectorate_name_code]                         NVARCHAR (32)   NULL,
    [studio_school_indicator_code]                   NVARCHAR (32)   NULL,
    [eyfs_exemption_code]                            NVARCHAR (32)   NULL,
    [section_41_approved_code]                       NVARCHAR (32)   NULL,
    [quality_assurance_body_name_code]               VARCHAR (32)    NULL,
    [establishment_accredited_code]                  VARCHAR (32)    NULL,
    [ofsted_rating_code]                             NVARCHAR (32)   NULL,
    [edubase_trigger_1_code]                         NVARCHAR (32)   NULL,
    [percentage_of_pupils_receiving_fsm]             NVARCHAR (10)   NULL,
    [places_pru]                                     INT             NULL,
    [correspondence_address]                         NVARCHAR (100)  NULL,
    [sa_org_id]                                      NUMERIC (19)    NULL,
    [date_next_welfare_inspection]                   DATETIME        NULL,
    [secure_access_pin]                              NUMERIC (19)    NULL,
    [ch_number]                                      VARCHAR (20)    NULL,
    [qab_report]                                     VARCHAR (200)   NULL,
    [site_name]                                      NVARCHAR (255)  NULL,
    [inspectorate_report]                            NVARCHAR (300)  NULL,
    [checked_zero_days]                              TINYINT         NULL,
    [checked_thirty_days]                            TINYINT         NULL,
    [checked_sixty_days]                             TINYINT         NULL,
    [burnham_group]                                  NVARCHAR (20)   NULL,
    [boys_2_and_under]                               NVARCHAR (10)   NULL,
    [boys_3]                                         NVARCHAR (10)   NULL,
    [boys_4a]                                        NVARCHAR (10)   NULL,
    [boys_4b]                                        NVARCHAR (10)   NULL,
    [boys_4c]                                        NVARCHAR (10)   NULL,
    [boys_5]                                         NVARCHAR (10)   NULL,
    [boys_6]                                         NVARCHAR (10)   NULL,
    [boys_7]                                         NVARCHAR (10)   NULL,
    [boys_8]                                         NVARCHAR (10)   NULL,
    [boys_9]                                         NVARCHAR (10)   NULL,
    [boys_10]                                        NVARCHAR (10)   NULL,
    [boys_11]                                        NVARCHAR (10)   NULL,
    [boys_12]                                        NVARCHAR (10)   NULL,
    [boys_13]                                        NVARCHAR (10)   NULL,
    [boys_14]                                        NVARCHAR (10)   NULL,
    [boys_15]                                        NVARCHAR (10)   NULL,
    [boys_16]                                        NVARCHAR (10)   NULL,
    [boys_17]                                        NVARCHAR (10)   NULL,
    [boys_18]                                        NVARCHAR (10)   NULL,
    [boys_19_plus]                                   NVARCHAR (10)   NULL,
    [girls_2_and_under]                              NVARCHAR (10)   NULL,
    [girls_3]                                        NVARCHAR (10)   NULL,
    [girls_4a]                                       NVARCHAR (10)   NULL,
    [girls_4b]                                       NVARCHAR (10)   NULL,
    [girls_4c]                                       NVARCHAR (10)   NULL,
    [girls_5]                                        NVARCHAR (10)   NULL,
    [girls_6]                                        NVARCHAR (10)   NULL,
    [girls_7]                                        NVARCHAR (10)   NULL,
    [girls_8]                                        NVARCHAR (10)   NULL,
    [girls_9]                                        NVARCHAR (10)   NULL,
    [girls_10]                                       NVARCHAR (10)   NULL,
    [girls_11]                                       NVARCHAR (10)   NULL,
    [girls_12]                                       NVARCHAR (10)   NULL,
    [girls_13]                                       NVARCHAR (10)   NULL,
    [girls_14]                                       NVARCHAR (10)   NULL,
    [girls_15]                                       NVARCHAR (10)   NULL,
    [girls_16]                                       NVARCHAR (10)   NULL,
    [girls_17]                                       NVARCHAR (10)   NULL,
    [girls_18]                                       NVARCHAR (10)   NULL,
    [girls_19_plus]                                  NVARCHAR (10)   NULL,
    [helpdesk_notes]                                 NVARCHAR (255)  NULL,
    [ch_address_3]                                   NVARCHAR (255)  NULL,
    [ch_locality]                                    NVARCHAR (255)  NULL,
    [ch_name]                                        NVARCHAR (255)  NULL,
    [ch_postcode]                                    NVARCHAR (255)  NULL,
    [ch_street]                                      NVARCHAR (255)  NULL,
    [ch_town]                                        NVARCHAR (255)  NULL,
    [ch_county_code]                                 NVARCHAR (32)   NULL,
    [con_title_code]                                 NVARCHAR (32)   NULL,
    [con_first_name]                                 NVARCHAR (100)  NULL,
    [con_last_name]                                  NVARCHAR (100)  NULL,
    [con_preferred_job_title]                        NVARCHAR (50)   NULL,
    [con_honours]                                    NVARCHAR (20)   NULL,
    [con_street]                                     NVARCHAR (100)  NULL,
    [con_fax_number]                                 NVARCHAR (40)   NULL,
    [con_town]                                       NVARCHAR (100)  NULL,
    [con_address3]                                   NVARCHAR (100)  NULL,
    [con_locality]                                   NVARCHAR (100)  NULL,
    [con_postcode]                                   NVARCHAR (8)    NULL,
    [con_county_code]                                NVARCHAR (32)   NULL,
    [con_easting]                                    INT             NULL,
    [con_northing]                                   INT             NULL,
    [con_uprn]                                       NVARCHAR (255)  NULL,
    [con_llsc_code]                                  NVARCHAR (32)   NULL,
    [con_urban_rural_code]                           NVARCHAR (32)   NULL,
    [con_la_code]                                    NVARCHAR (32)   NULL,
    [con_previous_la_code]                           NVARCHAR (32)   NULL,
    [con_gss_la_code_code]                           NVARCHAR (32)   NULL,
    [con_cas_ward_code]                              NVARCHAR (32)   NULL,
    [con_gor_code]                                   NVARCHAR (32)   NULL,
    [con_rsc_region_code]                            NVARCHAR (32)   NULL,
    [con_country_code]                               NVARCHAR (32)   NULL,
    [con_telephone_std]                              NVARCHAR (20)   NULL,
    [con_telephone_number]                           NVARCHAR (40)   NULL,
    [con_fax_std]                                    NVARCHAR (20)   NULL,
    [con_email]                                      NVARCHAR (100)  NULL,
    [con_site_name]                                  NVARCHAR (255)  NULL,
    [con_country_name]                               NVARCHAR (255)  NULL,
    [contact_preference_name]                        NVARCHAR (255)  NULL,
    [marketing_opt_in_out_name]                      NVARCHAR (255)  NULL,
    [establishment_type_name]                        NVARCHAR (255)  NULL,
    [reason_establishment_opened_name]               NVARCHAR (255)  NULL,
    [reason_establishment_closed_name]               NVARCHAR (255)  NULL,
    [gender_name]                                    NVARCHAR (255)  NULL,
    [status_name]                                    NVARCHAR (255)  NULL,
    [trust_school_name]                              NVARCHAR (255)  NULL,
    [change_reason_name]                             NVARCHAR (255)  NULL,
    [education_phase_name]                           NVARCHAR (255)  NULL,
    [further_education_type_name]                    NVARCHAR (255)  NULL,
    [lsoa_name]                                      NVARCHAR (255)  NULL,
    [msoa_name]                                      NVARCHAR (255)  NULL,
    [llsc_name]                                      NVARCHAR (255)  NULL,
    [ttwa_name]                                      NVARCHAR (255)  NULL,
    [gor_name]                                       NVARCHAR (255)  NULL,
    [urban_rural_name]                               NVARCHAR (255)  NULL,
    [la_name]                                        NVARCHAR (255)  NULL,
    [previous_la_name]                               NVARCHAR (255)  NULL,
    [gss_la_code_name]                               NVARCHAR (255)  NULL,
    [country_name]                                   NVARCHAR (255)  NULL,
    [cas_ward_name]                                  NVARCHAR (255)  NULL,
    [administrative_ward_name]                       NVARCHAR (255)  NULL,
    [district_administrative_name]                   NVARCHAR (255)  NULL,
    [parliamentary_constituency_name]                NVARCHAR (255)  NULL,
    [county_name]                                    NVARCHAR (255)  NULL,
    [special_educational_needs_name]                 NVARCHAR (255)  NULL,
    [training_establishment_name]                    NVARCHAR (255)  NULL,
    [first_main_specialism_name]                     NVARCHAR (255)  NULL,
    [second_main_specialism_name]                    NVARCHAR (255)  NULL,
    [first_secondary_specialism_name]                NVARCHAR (255)  NULL,
    [second_secondary_specialism_name]               NVARCHAR (255)  NULL,
    [emotional_and_behavioural_difficulties_name]    NVARCHAR (255)  NULL,
    [leading_option_1_name]                          NVARCHAR (255)  NULL,
    [leading_option_2_name]                          NVARCHAR (255)  NULL,
    [leading_option_3_name]                          NVARCHAR (255)  NULL,
    [special_classes_name]                           NVARCHAR (255)  NULL,
    [leadership_incentive_name]                      NVARCHAR (255)  NULL,
    [learning_support_unit_name]                     NVARCHAR (255)  NULL,
    [private_finance_initiative_name]                NVARCHAR (255)  NULL,
    [admissions_policy_name]                         NVARCHAR (255)  NULL,
    [official_sixth_form_name]                       NVARCHAR (255)  NULL,
    [education_action_zone_name]                     NVARCHAR (255)  NULL,
    [nursery_provision_name]                         NVARCHAR (255)  NULL,
    [early_excellence_name]                          NVARCHAR (255)  NULL,
    [boarders_name]                                  NVARCHAR (255)  NULL,
    [registered_early_years_name]                    NVARCHAR (255)  NULL,
    [excellence_in_cities_city_learning_centre_name] NVARCHAR (255)  NULL,
    [excellence_in_cities_action_zone_name]          NVARCHAR (255)  NULL,
    [excellence_in_cities_name]                      NVARCHAR (255)  NULL,
    [excellence_in_cities_group_name]                NVARCHAR (255)  NULL,
    [education_by_others_name]                       NVARCHAR (255)  NULL,
    [teenaged_mothers_name]                          NVARCHAR (255)  NULL,
    [child_care_facilities_name]                     NVARCHAR (255)  NULL,
    [investor_in_people_name]                        NVARCHAR (255)  NULL,
    [ft_provision_name]                              NVARCHAR (255)  NULL,
    [beacon_name]                                    NVARCHAR (255)  NULL,
    [religious_ethos_name]                           NVARCHAR (255)  NULL,
    [religious_character_name]                       NVARCHAR (255)  NULL,
    [diocese_name]                                   NVARCHAR (255)  NULL,
    [childrens_centres_phase_type_name]              NVARCHAR (255)  NULL,
    [direct_provision_of_early_years_name]           NVARCHAR (255)  NULL,
    [disadvantaged_area_name]                        NVARCHAR (255)  NULL,
    [governance_name]                                NVARCHAR (255)  NULL,
    [governors_flag_name]                            NVARCHAR (255)  NULL,
    [operational_hours_name]                         NVARCHAR (255)  NULL,
    [burnham_report_name]                            NVARCHAR (255)  NULL,
    [ofsted_rating_name]                             NVARCHAR (255)  NULL,
    [fresh_start_name]                               NVARCHAR (255)  NULL,
    [moved_to_edubase_login_name]                    NVARCHAR (255)  NULL,
    [type_of_reserved_provision_name]                NVARCHAR (255)  NULL,
    [contact_type_name]                              NVARCHAR (255)  NULL,
    [inspectorate_name_name]                         NVARCHAR (255)  NULL,
    [studio_school_indicator_name]                   NVARCHAR (255)  NULL,
    [eyfs_exemption_name]                            NVARCHAR (255)  NULL,
    [section_41_approved_name]                       NVARCHAR (255)  NULL,
    [quality_assurance_body_name_name]               NVARCHAR (255)  NULL,
    [establishment_accredited_name]                  NVARCHAR (255)  NULL,
    [rsc_region_code]                                NVARCHAR (32)   NULL,
    [rsc_region_name]                                NVARCHAR (255)  NULL,
    [edubase_trigger_1_name]                         NVARCHAR (255)  NULL,
    [superannuation_category_name]                   NVARCHAR (255)  NULL,
    [ofsted_special_measures_name]                   NVARCHAR (255)  NULL,
    [s_2_s_login_name]                               NVARCHAR (255)  NULL,
    [head_title_name]                                NVARCHAR (255)  NULL,
    [ch_county_name]                                 NVARCHAR (255)  NULL,
    [con_title_name]                                 NVARCHAR (255)  NULL,
    [con_llsc_name]                                  NVARCHAR (255)  NULL,
    [con_gor_name]                                   NVARCHAR (255)  NULL,
    [con_urban_rural_name]                           NVARCHAR (255)  NULL,
    [con_la_name]                                    NVARCHAR (255)  NULL,
    [con_previous_la_name]                           NVARCHAR (255)  NULL,
    [con_gss_la_code_name]                           NVARCHAR (255)  NULL,
    [con_county_name]                                NVARCHAR (255)  NULL,
    [con_cas_ward_name]                              NVARCHAR (255)  NULL,
    [con_rsc_region_name]                            NVARCHAR (255)  NULL,
    [urn]                                            NUMERIC (19)    NOT NULL,
    [establishment_type_group_code]                  NVARCHAR (32)   NULL,
    [establishment_type_group_name]                  NVARCHAR (255)  NULL,
    [ischool_timestamp]                              BINARY (8)      NULL,
    [ischool_notes]                                  NVARCHAR (4000) NULL,
    [ischool_date_of_last_ofsted_visit]              DATETIME        NULL,
    [ischool_date_of_last_isi_visit]                 DATETIME        NULL,
    [ischool_date_of_last_welfare_visit]             DATETIME        NULL,
    [ischool_date_of_last_fp_visit]                  DATETIME        NULL,
    [ischool_date_of_last_sis_visit]                 DATETIME        NULL,
    [ischool_date_of_last_bridge_visit]              DATETIME        NULL,
    [ischool_action_required_wel]                    DATETIME        NULL,
    [ischool_action_required_fp]                     DATETIME        NULL,
    [ischool_next_general_action_required]           DATETIME        NULL,
    [ischool_census_date]                            DATETIME        NULL,
    [ischool_next_ofsted_visit]                      DATETIME        NULL,
    [ischool_charity_name]                           NVARCHAR (255)  NULL,
    [ischool_charity_number]                         INT             NULL,
    [ischool_type_code]                              NVARCHAR (32)   NULL,
    [ischool_type_name]                              NVARCHAR (255)  NULL,
    [ischool_iebt_associations]                      NVARCHAR (1000) NULL,
    [ischool_total_pupils_full_time]                 INT             NULL,
    [ischool_total_pupils_part_time]                 INT             NULL,
    [ischool_total_pupils_public_care]               INT             NULL,
    [ischool_total_number_ft_staff]                  INT             NULL,
    [ischool_total_number_pt_staff]                  INT             NULL,
    [ischool_total_weekly_hours_pt_staff]            INT             NULL,
    [ischool_pupil_to_teacher_ratio]                 NVARCHAR (255)  NULL,
    [ischool_compulsory_school_age]                  INT             NULL,
    [ischool_lowest_day_fee]                         INT             NULL,
    [ischool_highest_day_fee]                        INT             NULL,
    [ischool_lowest_board_fee]                       INT             NULL,
    [ischool_highest_board_fee]                      INT             NULL,
    [ischool_sen_stat]                               INT             NULL,
    [ischool_no_stat]                                INT             NULL,
    [ischool_total_boy_boarders]                     INT             NULL,
    [ischool_total_girl_boarders]                    INT             NULL,
    [ischool_total_ftt_teachers_tutors]              INT             NULL,
    [ischool_total_ptt_teachers_tutors]              INT             NULL,
    [ischool_accom_change_code]                      NVARCHAR (32)   NULL,
    [ischool_accom_change_name]                      NVARCHAR (255)  NULL,
    [ischool_inspectorate_code]                      NVARCHAR (32)   NULL,
    [ischool_inspectorate_name]                      NVARCHAR (255)  NULL,
    [ischool_boarding_establishment_code]            NVARCHAR (32)   NULL,
    [ischool_boarding_establishment_name]            NVARCHAR (255)  NULL,
    [ischool_association_code]                       NVARCHAR (32)   NULL,
    [ischool_association_name]                       NVARCHAR (255)  NULL,
    [ischool_proprietor_type_code]                   NVARCHAR (32)   NULL,
    [ischool_proprietor_type_name]                   NVARCHAR (255)  NULL,
    [ischool_props_title_code]                       NVARCHAR (32)   NULL,
    [ischool_props_title_name]                       NVARCHAR (255)  NULL,
    [ischool_props_first_name]                       NVARCHAR (100)  NULL,
    [ischool_props_last_name]                        NVARCHAR (100)  NULL,
    [ischool_props_preferred_job_title]              NVARCHAR (25)   NULL,
    [ischool_props_honours]                          NVARCHAR (20)   NULL,
    [ischool_props_street]                           NVARCHAR (100)  NULL,
    [ischool_props_town]                             NVARCHAR (100)  NULL,
    [ischool_props_address_3]                        NVARCHAR (100)  NULL,
    [ischool_props_locality]                         NVARCHAR (100)  NULL,
    [ischool_props_postcode]                         NVARCHAR (8)    NULL,
    [ischool_props_county_code]                      NVARCHAR (32)   NULL,
    [ischool_props_county_name]                      NVARCHAR (255)  NULL,
    [ischool_props_country_code]                     NVARCHAR (32)   NULL,
    [ischool_props_country_name]                     NVARCHAR (255)  NULL,
    [ischool_props_uprn]                             NVARCHAR (255)  NULL,
    [ischool_props_site_name]                        NVARCHAR (255)  NULL,
    [ischool_props_telephone_std]                    NVARCHAR (20)   NULL,
    [ischool_props_telephone_number]                 NVARCHAR (40)   NULL,
    [ischool_props_fax_std]                          NVARCHAR (20)   NULL,
    [ischool_props_fax_number]                       NVARCHAR (40)   NULL,
    [ischool_props_email]                            NVARCHAR (100)  NULL,
    [ischool_chairman_title_code]                    NVARCHAR (32)   NULL,
    [ischool_chairman_title_name]                    NVARCHAR (255)  NULL,
    [ischool_chairman_first_name]                    NVARCHAR (100)  NULL,
    [ischool_chairman_last_name]                     NVARCHAR (100)  NULL,
    [ischool_chairman_preferred_job_title]           NVARCHAR (25)   NULL,
    [ischool_chairman_honours]                       NVARCHAR (20)   NULL,
    [ischool_chairman_street]                        NVARCHAR (100)  NULL,
    [ischool_chairman_town]                          NVARCHAR (100)  NULL,
    [ischool_chairman_address_3]                     NVARCHAR (100)  NULL,
    [ischool_chairman_locality]                      NVARCHAR (100)  NULL,
    [ischool_chairman_postcode]                      NVARCHAR (8)    NULL,
    [ischool_chairman_county_code]                   NVARCHAR (32)   NULL,
    [ischool_chairman_county_name]                   NVARCHAR (255)  NULL,
    [ischool_chairman_country_code]                  NVARCHAR (32)   NULL,
    [ischool_chairman_country_name]                  NVARCHAR (255)  NULL,
    [ischool_chairman_uprn]                          NVARCHAR (255)  NULL,
    [ischool_chairman_site_name]                     NVARCHAR (255)  NULL,
    [ischool_chairman_telephone_std]                 NVARCHAR (20)   NULL,
    [ischool_chairman_telephone_number]              NVARCHAR (40)   NULL,
    [ischool_chairman_fax_std]                       NVARCHAR (20)   NULL,
    [ischool_chairman_fax_number]                    NVARCHAR (40)   NULL,
    [ischool_chairman_email]                         NVARCHAR (100)  NULL,
    [ischool_pt_boys_2_and_under]                    INT             NULL,
    [ischool_pt_boys_3]                              INT             NULL,
    [ischool_pt_boys_4a]                             INT             NULL,
    [ischool_pt_boys_4b]                             INT             NULL,
    [ischool_pt_boys_4c]                             INT             NULL,
    [ischool_pt_girls_2_and_under]                   INT             NULL,
    [ischool_pt_girls_3]                             INT             NULL,
    [ischool_pt_girls_4a]                            INT             NULL,
    [ischool_pt_girls_4b]                            INT             NULL,
    [ischool_pt_girls_4c]                            INT             NULL,
    [trust_school_flag_code]                         NVARCHAR (32)   NULL,
    [school_sponsor_flag_code]                       NVARCHAR (32)   NULL,
    [school_sponsor_flag_name]                       NVARCHAR (255)  NULL,
    [school_sponsor_code]                            NVARCHAR (32)   NULL,
    [school_sponsor_name]                            NVARCHAR (255)  NULL,
    [federation_flag_code]                           NVARCHAR (32)   NULL,
    [federation_flag_name]                           NVARCHAR (255)  NULL,
    [federation_code]                                NVARCHAR (32)   NULL,
    [federation_name]                                NVARCHAR (255)  NULL,
    [sen_code_1]                                     NVARCHAR (32)   NULL,
    [sen_name_1]                                     NVARCHAR (255)  NULL,
    [sen_code_2]                                     NVARCHAR (32)   NULL,
    [sen_name_2]                                     NVARCHAR (255)  NULL,
    [sen_code_3]                                     NVARCHAR (32)   NULL,
    [sen_name_3]                                     NVARCHAR (255)  NULL,
    [sen_code_4]                                     NVARCHAR (32)   NULL,
    [sen_name_4]                                     NVARCHAR (255)  NULL,
    [sen_code_5]                                     NVARCHAR (32)   NULL,
    [sen_name_5]                                     NVARCHAR (255)  NULL,
    [sen_code_6]                                     NVARCHAR (32)   NULL,
    [sen_name_6]                                     NVARCHAR (255)  NULL,
    [sen_code_7]                                     NVARCHAR (32)   NULL,
    [sen_name_7]                                     NVARCHAR (255)  NULL,
    [sen_code_8]                                     NVARCHAR (32)   NULL,
    [sen_name_8]                                     NVARCHAR (255)  NULL,
    [sen_code_9]                                     NVARCHAR (32)   NULL,
    [sen_name_9]                                     NVARCHAR (255)  NULL,
    [sen_code_10]                                    NVARCHAR (32)   NULL,
    [sen_name_10]                                    NVARCHAR (255)  NULL,
    [sen_code_11]                                    NVARCHAR (32)   NULL,
    [sen_name_11]                                    NVARCHAR (255)  NULL,
    [sen_code_12]                                    NVARCHAR (32)   NULL,
    [sen_name_12]                                    NVARCHAR (255)  NULL,
    [sen_code_13]                                    NVARCHAR (32)   NULL,
    [sen_name_13]                                    NVARCHAR (255)  NULL,
    [row_inserted_datetime]                          DATETIME        NULL,
    [row_inserted_by]                                NVARCHAR (255)  NULL,
    [row_updated_datetime]                           DATETIME        NULL,
    [row_updated_by]                                 NVARCHAR (255)  NULL,
    CONSTRAINT [PK_establishment_cache] PRIMARY KEY CLUSTERED ([urn] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[group_link_cache] (
    [id]                    NUMERIC (19)   NOT NULL,
    [gid]                   NUMERIC (19)   NOT NULL,
    [archived]              TINYINT        NULL,
    [link_type]             NVARCHAR (255) NULL,
    [urn_version]           INT            NOT NULL,
    [urn]                   NUMERIC (19)   NOT NULL,
    [effective_date]        DATETIME       NULL,
    [cc_link_type]          NVARCHAR (255) NULL,
    [row_inserted_datetime] DATETIME       NULL,
    [row_inserted_by]       NVARCHAR (255) NULL,
    [row_updated_datetime]  DATETIME       NULL,
    [row_updated_by]        NVARCHAR (255) NULL,
    [gid_version]           INT            NOT NULL,
    CONSTRAINT [PK_group_link_cache] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[public_columns] (
    [public_columns_record_id] INT            IDENTITY (1, 1) NOT NULL,
    [source_table_name]        NVARCHAR (50)  NOT NULL,
    [target_table_name]        NVARCHAR (50)  NOT NULL,
    [source_column_name]       NVARCHAR (100) NOT NULL,
    [target_column_name]       NVARCHAR (100) NOT NULL,
    [row_inserted_by]          NVARCHAR (255) NULL,
    [row_inserted_datetime]    DATETIME       NULL,
    [row_updated_by]           NVARCHAR (255) NULL,
    [row_updated_datetime]     DATETIME       NULL,
    CONSTRAINT [PK_public_columns] PRIMARY KEY CLUSTERED ([public_columns_record_id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[cache_update_log] (
    [cache_update_log_id]   BIGINT          IDENTITY (1, 1) NOT NULL,
    [start_time]            DATETIME        NOT NULL,
    [end_time]              DATETIME        NULL,
    [success_flag]          TINYINT         NOT NULL,
    [row_inserted_datetime] DATETIME        NULL,
    [row_inserted_by]       NVARCHAR (255)  NULL,
    [row_updated_datetime]  DATETIME        NULL,
    [row_updated_by]        NVARCHAR (255)  NULL,
    [log_error_code]        NVARCHAR (10)   NULL,
    [log_narration]         NVARCHAR (2000) NULL,
    CONSTRAINT [PK_cache_update_log] PRIMARY KEY CLUSTERED ([cache_update_log_id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[cache_update_table_log] (
    [cache_update_log_id]     BIGINT         NOT NULL,
    [number_of_rows_inserted] INT            NOT NULL,
    [number_of_rows_updated]  INT            NOT NULL,
    [row_inserted_datetime]   DATETIME       NULL,
    [row_inserted_by]         NVARCHAR (255) NULL,
    [row_updated_datetime]    DATETIME       NULL,
    [row_updated_by]          NVARCHAR (255) NULL,
    [table_name]              NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_cache_update_table_log] PRIMARY KEY CLUSTERED ([cache_update_log_id] ASC, [table_name] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [gias_sharing].[establishment_group_change_history_cache] (
    [request_type]                     NVARCHAR (31)   NULL,
    [id]                               NUMERIC (19)    NOT NULL,
    [created_date]                     DATETIME        NULL,
    [effective_date]                   DATETIME        NULL,
    [last_change_date]                 DATETIME        NULL,
    [reject_reason]                    NVARCHAR (2048) NULL,
    [status]                           NVARCHAR (255)  NULL,
    [version]                          INT             NOT NULL,
    [date_new_value]                   DATETIME        NULL,
    [date_old_value]                   DATETIME        NULL,
    [lookup_new_value]                 NVARCHAR (255)  NULL,
    [lookup_old_value]                 NVARCHAR (255)  NULL,
    [group_name]                       NVARCHAR (255)  NULL,
    [group_open_date]                  DATETIME        NULL,
    [link_type]                        NVARCHAR (255)  NULL,
    [shared_head_teacher]              TINYINT         NULL,
    [numeric_new_value]                NUMERIC (19)    NULL,
    [numeric_old_value]                NUMERIC (19)    NULL,
    [string_new_value]                 NVARCHAR (1024) NULL,
    [string_old_value]                 NVARCHAR (1024) NULL,
    [approved_or_rejected_by_username] NVARCHAR (255)  NULL,
    [proposed_by_username]             NVARCHAR (255)  NULL,
    [field_short_name]                 NVARCHAR (32)   NULL,
    [gid]                              NUMERIC (19)    NULL,
    [group_type_code]                  NVARCHAR (32)   NULL,
    [urn]                              NUMERIC (19)    NULL,
    [has_joint_committee_code]         NVARCHAR (32)   NULL,
    [group_link]                       NUMERIC (19)    NULL,
    [boolean_new_value]                TINYINT         NULL,
    [boolean_old_value]                TINYINT         NULL,
    [cc_link_type]                     NVARCHAR (255)  NULL,
    [group_closed_date]                DATETIME2 (7)   NULL,
    [companies_house_number]           NVARCHAR (255)  NULL,
    [group_address_3]                  NVARCHAR (255)  NULL,
    [group_contact_email]              NVARCHAR (255)  NULL,
    [group_id]                         NVARCHAR (255)  NULL,
    [group_locality]                   NVARCHAR (255)  NULL,
    [group_postcode]                   NVARCHAR (255)  NULL,
    [group_street]                     NVARCHAR (255)  NULL,
    [group_town]                       NVARCHAR (255)  NULL,
    [group_county_code]                NVARCHAR (32)   NULL,
    [delegation_information]           NVARCHAR (255)  NULL,
    [head_first_name]                  NVARCHAR (255)  NULL,
    [head_last_name]                   NVARCHAR (255)  NULL,
    [head_title_id_code]               NVARCHAR (32)   NULL,
    [corporate_contact]                NVARCHAR (255)  NULL,
    [group_relations_link]             NUMERIC (19)    NULL,
    [group_type_name]                  NVARCHAR (255)  NULL,
    [has_joint_committee_name]         NVARCHAR (255)  NULL,
    [group_county_name]                NVARCHAR (255)  NULL,
    [head_title_id_name]               NVARCHAR (255)  NULL,
    [row_inserted_datetime]            DATETIME        NULL,
    [row_inserted_by]                  NVARCHAR (255)  NULL,
    [row_updated_datetime]             DATETIME        NULL,
    [row_updated_by]                   NVARCHAR (255)  NULL,
    CONSTRAINT [PK_establishment_group_change_history_cache] PRIMARY KEY CLUSTERED ([id] ASC, [version] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [MasterProvider].[DSI_Provider_Extract] (
    [MasterProviderCode]           BIGINT         NULL,
    [MasterURN]                    NVARCHAR (500) NULL,
    [GIAS_URN]                     NVARCHAR (500) NULL,
    [Ofsted_URN]                   INT            NULL,
    [MasterUKPRN]                  NVARCHAR (500) NULL,
    [MasterUPIN]                   VARCHAR (20)   NULL,
    [MasterProviderName]           NVARCHAR (500) NULL,
    [MasterEdubaseUID]             VARCHAR (20)   NULL,
    [MasterCRMID]                  VARCHAR (50)   NULL,
    [MasterCRMAccountID]           VARCHAR (100)  NULL,
    [MasterDfEEstabNo]             NVARCHAR (500) NULL,
    [MasterDfELANumber]            NVARCHAR (500) NULL,
    [MasterDfELAEstabNo]           VARCHAR (61)   NULL,
    [MasterCompanyHouseNumber]     NVARCHAR (100) NULL,
    [MasterLocalAuthorityCode]     NVARCHAR (500) NULL,
    [MasterLocalAuthorityName]     NVARCHAR (500) NULL,
    [MasterProviderLegalName]      NVARCHAR (500) NULL,
    [MasterDateOpened]             DATETIME       NULL,
    [MasterDateClosed]             DATETIME       NULL,
    [MasterProviderTypeCode]       NVARCHAR (250) NULL,
    [MasterProviderTypeName]       NVARCHAR (250) NULL,
    [MasterUpperAge]               NVARCHAR (500) NULL,
    [MasterLowerAge]               NVARCHAR (500) NULL,
    [MasterFlagSchoolhasSixthForm] INT            NULL,
    [MasterProviderStatusCode]     NVARCHAR (250) NULL,
    [MasterProviderStatusName]     NVARCHAR (250) NULL,
    [MasterPhaseofEducationCode]   NVARCHAR (250) NULL,
    [MasterPhaseofEducationName]   NVARCHAR (250) NULL,
    [MasterAddress]                NVARCHAR (500) NULL,
    [MasterPostcode]               NVARCHAR (500) NULL,
    [MasterProvisionTypeCode]      NVARCHAR (250) NULL,
    [MasterProvisionTypeName]      NVARCHAR (250) NULL,
    [MasterGenderCode]             NVARCHAR (250) NULL,
    [MasterGenderName]             NVARCHAR (250) NULL,
    [MasterReligiousCharacterCode] NVARCHAR (250) NULL,
    [MasterReligiousCharacterName] NVARCHAR (250) NULL,
    [MasterAdmissionPolicyCode]    NVARCHAR (250) NULL,
    [MasterAdmissionPolicyName]    NVARCHAR (250) NULL,
    [HeadTitleName]                NVARCHAR (250) NULL,
    [HeadFirstName]                NVARCHAR (250) NULL,
    [HeadLastName]                 NVARCHAR (250) NULL,
    [IsActive]                     INT            NOT NULL,
    [IsNotActiveFrom]              DATETIME       NULL,
    [CreatedDatetime]              DATETIME       NULL,
    [MasterNavVendorNo]            VARCHAR (10)   NULL,
    [Type_of_Provider]             VARCHAR (100)  NOT NULL,
    [Source_System]                VARCHAR (4)    NOT NULL,
    [GIAS_Provider_Type_code]      NVARCHAR (MAX) NULL,
    [GIAS_Provider_Type]           NVARCHAR (MAX) NULL,
    [PIMS_Provider_Type_Code]      NUMERIC (9)    NULL,
    [PIMS_Provider_Type]           NVARCHAR (100) NULL,
    [GIAS_Status_Code]             INT            NULL,
    [GIAS_Status]                  NVARCHAR (MAX) NULL,
    [PIMS_Status]                  NVARCHAR (100) NULL,
    [DistrictAdministrativeCode]   NVARCHAR (500) NULL,
    [DistrictAdministrativeName]   NVARCHAR (500) NULL,
    [TelephoneNum]                 NVARCHAR (500) NULL,
    [RegionCode]                   NVARCHAR (500) NULL,
    [IsOnAPAR]                     NVARCHAR (50)  NULL
);


GO

CREATE TABLE [MasterProvider].[DSI_Links_Provider_Extract] (
    [MasterProviderCode]            BIGINT      NULL,
    [Associated_MasterProviderCode] BIGINT      NULL,
    [link_type]                     VARCHAR (7) NOT NULL
);


GO

CREATE TABLE [dbo].[$__schema_journal] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ScriptName] NVARCHAR (255) NOT NULL,
    [Applied]    DATETIME       NOT NULL,
    CONSTRAINT [PK_$__schema_journal_Id] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AccommodationChanged] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__AccommodationCha__59FA5E80] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AdditionalDatasetOrder] (
    [id]                    NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [createdDate]           DATETIME        NULL,
    [sum]                   NUMERIC (19, 2) NULL,
    [datasetName_code]      NVARCHAR (255)  NULL,
    [subscription_order_id] NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AdministrativeWard] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__AdministrativeWa__5BE2A6F2] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AdmissionsPolicy] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__AdmissionsPolicy__060DEAE8] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnalysisCriterion] (
    [id]            NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [code]          NVARCHAR (255) NOT NULL,
    [criterionType] INT            NOT NULL,
    [sortOrder]     INT            NOT NULL,
    [report_id]     NUMERIC (19)   NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnalysisCriterionValue] (
    [id]                   NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [value]                NVARCHAR (512) NULL,
    [analysisCriterion_id] NUMERIC (19)   NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnalysisReport] (
    [id]                            NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [creationDate]                  DATETIME        NULL,
    [datasetName]                   NVARCHAR (128)  NOT NULL,
    [description]                   NVARCHAR (1024) NULL,
    [isSimple]                      TINYINT         NOT NULL,
    [isVisibleToProviderUsers]      TINYINT         NULL,
    [isVisibleToProviders]          TINYINT         NULL,
    [isVisibleToStakeholders]       TINYINT         NULL,
    [name]                          NVARCHAR (256)  NOT NULL,
    [creatorUser_username]          NVARCHAR (255)  NULL,
    [lastModifiedUser_username]     NVARCHAR (255)  NULL,
    [predefinedReportCategory_code] NVARCHAR (32)   NULL,
    [filter_id]                     NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnalysisReportToMeasureType] (
    [report_id]    NUMERIC (19)  NOT NULL,
    [measure_code] NVARCHAR (32) NOT NULL
);


GO

CREATE TABLE [dbo].[AnnouncementCollection] (
    [id]                        NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [announcementType]          NVARCHAR (255) NOT NULL,
    [datePublished]             DATETIME       NULL,
    [dateRemoved]               DATETIME       NULL,
    [publish]                   TINYINT        NULL,
    [published]                 TINYINT        NULL,
    [remove]                    TINYINT        NULL,
    [removed]                   TINYINT        NULL,
    [saOrgId]                   NUMERIC (19)   NULL,
    [urn]                       NUMERIC (19)   NULL,
    [template_id]               NUMERIC (19)   NOT NULL,
    [ts]                        DATETIME       NULL,
    [publishAttemptCount]       INT            NULL,
    [publishLastErrorMessage]   NVARCHAR (255) NULL,
    [unPublishAttemptCount]     INT            NULL,
    [unPublishLastErrorMessage] NVARCHAR (255) NULL,
    [uid]                       NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnnouncementCollectionValues] (
    [announcementId] NUMERIC (19)   NOT NULL,
    [fieldVal]       NVARCHAR (100) NULL,
    [fieldKey]       NVARCHAR (50)  NOT NULL,
    PRIMARY KEY CLUSTERED ([announcementId] ASC, [fieldKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnnouncementTemplate] (
    [id]                          NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [announcementBody]            VARCHAR (3750)  NULL,
    [announcementSummary]         VARCHAR (340)   NULL,
    [announcementTitle]           VARCHAR (75)    NULL,
    [announcementType]            NVARCHAR (255)  NOT NULL,
    [applicationId]               NVARCHAR (255)  NOT NULL,
    [enabled]                     TINYINT         NULL,
    [expirationAfterDays]         INT             NULL,
    [organisationAnnouncementKey] NVARCHAR (4000) NULL,
    [organisationId]              NUMERIC (19)    NULL,
    [announcementAction_code]     NVARCHAR (32)   NOT NULL,
    [canBeSentManually]           TINYINT         NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnnouncementTemplate_AUD] (
    [id]                          NUMERIC (19)    NOT NULL,
    [ver_rev]                     NUMERIC (19)    NOT NULL,
    [REVTYPE]                     TINYINT         NULL,
    [announcementBody]            NVARCHAR (4000) NULL,
    [announcementSummary]         NVARCHAR (4000) NULL,
    [announcementTitle]           NVARCHAR (255)  NULL,
    [announcementType]            NVARCHAR (255)  NULL,
    [applicationId]               NVARCHAR (255)  NULL,
    [enabled]                     TINYINT         NULL,
    [expirationAfterDays]         INT             NULL,
    [organisationAnnouncementKey] NVARCHAR (4000) NULL,
    [organisationId]              NUMERIC (19)    NULL,
    [announcementAction_code]     NVARCHAR (32)   NULL,
    [canBeSentManually]           TINYINT         NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnnouncementTemplate_UserGroup] (
    [AnnouncementTemplate_id] NUMERIC (19)  NOT NULL,
    [groups_code]             NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([AnnouncementTemplate_id] ASC, [groups_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AnnouncementTemplate_UserGroup_AUD] (
    [ver_rev]                 NUMERIC (19)  NOT NULL,
    [AnnouncementTemplate_id] NUMERIC (19)  NOT NULL,
    [groups_code]             NVARCHAR (32) NOT NULL,
    [REVTYPE]                 TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [AnnouncementTemplate_id] ASC, [groups_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ApplicationHistory] (
    [id]     NVARCHAR (512) NOT NULL,
    [action] NVARCHAR (64)  NOT NULL,
    [ts]     DATETIME       NOT NULL,
    CONSTRAINT [PK__ApplicationHisto__7C8480AE] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ApplicationSchema] (
    [installed_rank] INT             NOT NULL,
    [version]        NVARCHAR (50)   NULL,
    [description]    NVARCHAR (200)  NULL,
    [type]           NVARCHAR (20)   NOT NULL,
    [script]         NVARCHAR (1000) NOT NULL,
    [checksum]       INT             NULL,
    [installed_by]   NVARCHAR (100)  NOT NULL,
    [installed_on]   DATETIME        NOT NULL,
    [execution_time] INT             NOT NULL,
    [success]        BIT             NOT NULL,
    CONSTRAINT [ApplicationSchema_pk] PRIMARY KEY CLUSTERED ([installed_rank] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Association] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Association__48CFD27E] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Authority] (
    [code]        NVARCHAR (100)  NOT NULL,
    [description] NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AutomatedEmailTemplate] (
    [id]                   NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [approved]             TINYINT        NOT NULL,
    [body]                 NVARCHAR (MAX) NULL,
    [code]                 NVARCHAR (255) NULL,
    [description]          NVARCHAR (MAX) NULL,
    [help]                 NVARCHAR (MAX) NULL,
    [subject]              NVARCHAR (255) NOT NULL,
    [version]              INT            NOT NULL,
    [signOffUser_username] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AutomatedEmailTemplate_AUD] (
    [id]                   NUMERIC (19)   NOT NULL,
    [ver_rev]              NUMERIC (19)   NOT NULL,
    [REVTYPE]              TINYINT        NULL,
    [approved]             TINYINT        NULL,
    [body]                 NVARCHAR (MAX) NULL,
    [code]                 NVARCHAR (255) NULL,
    [description]          NVARCHAR (MAX) NULL,
    [help]                 NVARCHAR (MAX) NULL,
    [subject]              NVARCHAR (255) NULL,
    [signOffUser_username] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[AzureSQLMaintenanceLog] (
    [id]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [OperationTime] DATETIME2 (7)  NULL,
    [command]       VARCHAR (4000) NULL,
    [ExtraInfo]     VARCHAR (4000) NULL,
    [StartTime]     DATETIME2 (7)  NULL,
    [EndTime]       DATETIME2 (7)  NULL,
    [StatusMessage] VARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BATCH_JOB_EXECUTION] (
    [JOB_EXECUTION_ID]           BIGINT         NOT NULL,
    [VERSION]                    BIGINT         NULL,
    [JOB_INSTANCE_ID]            BIGINT         NOT NULL,
    [CREATE_TIME]                DATETIME       NOT NULL,
    [START_TIME]                 DATETIME       NULL,
    [END_TIME]                   DATETIME       NULL,
    [STATUS]                     VARCHAR (10)   NULL,
    [EXIT_CODE]                  VARCHAR (2500) NULL,
    [EXIT_MESSAGE]               VARCHAR (2500) NULL,
    [LAST_UPDATED]               DATETIME       NULL,
    [JOB_CONFIGURATION_LOCATION] VARCHAR (2500) NULL,
    PRIMARY KEY CLUSTERED ([JOB_EXECUTION_ID] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BATCH_JOB_EXECUTION_CONTEXT] (
    [JOB_EXECUTION_ID]   BIGINT         NOT NULL,
    [SHORT_CONTEXT]      VARCHAR (2500) NOT NULL,
    [SERIALIZED_CONTEXT] TEXT           NULL,
    PRIMARY KEY CLUSTERED ([JOB_EXECUTION_ID] ASC)
);


GO

CREATE TABLE [dbo].[BATCH_JOB_EXECUTION_PARAMS] (
    [JOB_EXECUTION_ID] BIGINT        NOT NULL,
    [TYPE_CD]          VARCHAR (6)   NOT NULL,
    [KEY_NAME]         VARCHAR (100) NOT NULL,
    [STRING_VAL]       VARCHAR (250) NULL,
    [DATE_VAL]         DATETIME      NULL,
    [LONG_VAL]         BIGINT        NULL,
    [DOUBLE_VAL]       FLOAT (53)    NULL,
    [IDENTIFYING]      CHAR (1)      NOT NULL
);


GO

CREATE TABLE [dbo].[BATCH_JOB_EXECUTION_SEQ] (
    [ID] BIGINT IDENTITY (1, 1) NOT NULL
);


GO

CREATE TABLE [dbo].[BATCH_JOB_INSTANCE] (
    [JOB_INSTANCE_ID] BIGINT        NOT NULL,
    [VERSION]         BIGINT        NULL,
    [JOB_NAME]        VARCHAR (100) NOT NULL,
    [JOB_KEY]         VARCHAR (32)  NOT NULL,
    PRIMARY KEY CLUSTERED ([JOB_INSTANCE_ID] ASC) WITH (DATA_COMPRESSION = PAGE),
    CONSTRAINT [JOB_INST_UN] UNIQUE NONCLUSTERED ([JOB_NAME] ASC, [JOB_KEY] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BATCH_JOB_SEQ] (
    [ID] BIGINT IDENTITY (1, 1) NOT NULL
);


GO

CREATE TABLE [dbo].[BATCH_STEP_EXECUTION] (
    [STEP_EXECUTION_ID]  BIGINT         NOT NULL,
    [VERSION]            BIGINT         NOT NULL,
    [STEP_NAME]          VARCHAR (100)  NOT NULL,
    [JOB_EXECUTION_ID]   BIGINT         NOT NULL,
    [START_TIME]         DATETIME       NOT NULL,
    [END_TIME]           DATETIME       NULL,
    [STATUS]             VARCHAR (10)   NULL,
    [COMMIT_COUNT]       BIGINT         NULL,
    [READ_COUNT]         BIGINT         NULL,
    [FILTER_COUNT]       BIGINT         NULL,
    [WRITE_COUNT]        BIGINT         NULL,
    [READ_SKIP_COUNT]    BIGINT         NULL,
    [WRITE_SKIP_COUNT]   BIGINT         NULL,
    [PROCESS_SKIP_COUNT] BIGINT         NULL,
    [ROLLBACK_COUNT]     BIGINT         NULL,
    [EXIT_CODE]          VARCHAR (2500) NULL,
    [EXIT_MESSAGE]       VARCHAR (2500) NULL,
    [LAST_UPDATED]       DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([STEP_EXECUTION_ID] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BATCH_STEP_EXECUTION_CONTEXT] (
    [STEP_EXECUTION_ID]  BIGINT         NOT NULL,
    [SHORT_CONTEXT]      VARCHAR (2500) NOT NULL,
    [SERIALIZED_CONTEXT] TEXT           NULL,
    PRIMARY KEY CLUSTERED ([STEP_EXECUTION_ID] ASC)
);


GO

CREATE TABLE [dbo].[BATCH_STEP_EXECUTION_SEQ] (
    [ID] BIGINT IDENTITY (1, 1) NOT NULL
);


GO

CREATE TABLE [dbo].[Beacon] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Beacon__4AB81AF0] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Boarders] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Boarders__117F9D94] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BoardingEstablishment] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__BoardingEstablis__5DCAEF64] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BulkUpdate] (
    [id]                 NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [createTime]         DATETIME        NOT NULL,
    [effectiveDate]      DATETIME        NULL,
    [endTime]            DATETIME        NULL,
    [fileName]           NVARCHAR (2048) NOT NULL,
    [originalFileName]   NVARCHAR (2048) NOT NULL,
    [overrideCR]         TINYINT         NOT NULL,
    [startTime]          DATETIME        NULL,
    [status]             INT             NOT NULL,
    [asUser_username]    NVARCHAR (255)  NULL,
    [user_username]      NVARCHAR (255)  NULL,
    [fileFormat]         INT             NULL,
    [createFreeSchools]  TINYINT         NOT NULL,
    [createAcademyLinks] TINYINT         NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BulkUpdateMessage] (
    [id]            NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [message]       NVARCHAR (2048) NOT NULL,
    [time]          DATETIME        NOT NULL,
    [bulkUpdate_id] NUMERIC (19)    NOT NULL,
    [skipped]       TINYINT         NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BulkUpdateStatus] (
    [id]              NUMERIC (19)    NOT NULL,
    [tempId]          NVARCHAR (255)  NULL,
    [type]            NVARCHAR (31)   NOT NULL,
    [total]           NUMERIC (19)    NULL,
    [processed]       NUMERIC (19)    NULL,
    [errorsCount]     INT             NULL,
    [rowsCount]       INT             NULL,
    [status]          NVARCHAR (255)  NULL,
    [freeSchoolsUrns] NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([tempId] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[BurnhamReport] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__BurnhamReport__35BCFE0A] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ByTypeFieldPermission] (
    [id]             NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [field]          NVARCHAR (32) NOT NULL,
    [type]           NVARCHAR (32) NOT NULL,
    [userGroup_code] NVARCHAR (32) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([field] ASC, [type] ASC, [userGroup_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[CacheAvailability] (
    [id]          NUMERIC (5)  IDENTITY (1, 1) NOT NULL,
    [available]   BIT          NULL,
    [update_date] DATETIME     NOT NULL,
    [updated_by]  VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Callback] (
    [id]           NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [uid]          NVARCHAR (255)  NOT NULL,
    [name]         NVARCHAR (255)  NULL,
    [description]  NVARCHAR (1000) NULL,
    [type]         NVARCHAR (32)   NOT NULL,
    [format]       NVARCHAR (32)   NOT NULL,
    [isLegacy]     TINYINT         NOT NULL,
    [isTemporary]  TINYINT         NOT NULL,
    [isPublic]     TINYINT         NOT NULL,
    [created]      DATETIME        NOT NULL,
    [creator]      NVARCHAR (255)  NULL,
    [creatorGroup] NVARCHAR (32)   NOT NULL,
    [started]      DATETIME        NULL,
    [finished]     DATETIME        NULL,
    [status]       NVARCHAR (32)   NOT NULL,
    [failureCause] NVARCHAR (4000) NULL,
    [total]        INT             NULL,
    [processed]    INT             NULL,
    [dataSize]     NUMERIC (19)    NULL,
    [storageType]  NVARCHAR (32)   NOT NULL,
    [storageKey]   NVARCHAR (255)  NOT NULL,
    [storageUri]   NVARCHAR (512)  NOT NULL,
    [lastSuccess]  DATETIME        NULL,
    [parent_id]    NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([uid] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[CallbackHistory] (
    [id]                   NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [uid]                  NVARCHAR (255)  NOT NULL,
    [name]                 NVARCHAR (255)  NULL,
    [description]          NVARCHAR (1000) NULL,
    [type]                 NVARCHAR (32)   NOT NULL,
    [format]               NVARCHAR (32)   NOT NULL,
    [isLegacy]             TINYINT         NOT NULL,
    [isTemporary]          TINYINT         NOT NULL,
    [isPublic]             TINYINT         NOT NULL,
    [created]              DATETIME        NOT NULL,
    [creator]              NVARCHAR (255)  NULL,
    [creatorGroup]         NVARCHAR (32)   NOT NULL,
    [started]              DATETIME        NULL,
    [finished]             DATETIME        NULL,
    [status]               NVARCHAR (32)   NOT NULL,
    [failureCause]         NVARCHAR (4000) NULL,
    [total]                INT             NULL,
    [processed]            INT             NULL,
    [dataSize]             NUMERIC (19)    NULL,
    [storageType]          NVARCHAR (32)   NOT NULL,
    [storageKey]           NVARCHAR (255)  NOT NULL,
    [storageUri]           NVARCHAR (512)  NULL,
    [lastSuccess]          DATETIME        NULL,
    [scheduled_extract_id] NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[CasWard] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__CasWard__5070F446] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeHistoryNewValues] (
    [EstablishmentChangeHistory_id] NUMERIC (19)   NOT NULL,
    [value]                         NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK__ChangeHistoryNew__1881A0DE] PRIMARY KEY CLUSTERED ([EstablishmentChangeHistory_id] ASC, [value] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeHistoryNewValues_AUD] (
    [ver_rev]                       NUMERIC (19)   NOT NULL,
    [EstablishmentChangeHistory_id] NUMERIC (19)   NOT NULL,
    [value]                         NVARCHAR (255) NOT NULL,
    [REVTYPE]                       TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [EstablishmentChangeHistory_id] ASC, [value] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeHistoryNewValuesArchive1] (
    [EstablishmentChangeHistory_id] NUMERIC (19)   NOT NULL,
    [value]                         NVARCHAR (255) NOT NULL
);


GO

CREATE TABLE [dbo].[ChangeHistoryOldValues] (
    [EstablishmentChangeHistory_id] NUMERIC (19)   NOT NULL,
    [value]                         NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK__ChangeHistoryOld__1A69E950] PRIMARY KEY CLUSTERED ([EstablishmentChangeHistory_id] ASC, [value] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeHistoryOldValues_AUD] (
    [ver_rev]                       NUMERIC (19)   NOT NULL,
    [EstablishmentChangeHistory_id] NUMERIC (19)   NOT NULL,
    [value]                         NVARCHAR (255) NOT NULL,
    [REVTYPE]                       TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [EstablishmentChangeHistory_id] ASC, [value] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeHistoryOldValuesArchive1] (
    [EstablishmentChangeHistory_id] NUMERIC (19)   NOT NULL,
    [value]                         NVARCHAR (255) NOT NULL
);


GO

CREATE TABLE [dbo].[ChangeReason] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ChangeReason__46E78A0C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeRequestMailingHistory] (
    [addresseeType]      NVARCHAR (31)  NOT NULL,
    [id]                 NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [date]               DATETIME       NULL,
    [mailType]           NVARCHAR (255) NULL,
    [changeRequest_id]   NUMERIC (19)   NULL,
    [addressee_username] NVARCHAR (255) NULL,
    [addressee_id]       NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeRequestMailingHistoryArchive1] (
    [addresseeType]      NVARCHAR (31)  NOT NULL,
    [id]                 NUMERIC (19)   NOT NULL,
    [date]               DATETIME       NULL,
    [mailType]           NVARCHAR (255) NULL,
    [changeRequest_id]   NUMERIC (19)   NULL,
    [addressee_username] NVARCHAR (255) NULL,
    [addressee_id]       NUMERIC (19)   NULL
);


GO

CREATE TABLE [dbo].[ChangeRequestProposer] (
    [id]        NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [address]   NVARCHAR (255) NULL,
    [email]     NVARCHAR (255) NULL,
    [fax]       NVARCHAR (255) NULL,
    [name]      NVARCHAR (255) NULL,
    [postcode]  NVARCHAR (255) NULL,
    [surname]   NVARCHAR (255) NULL,
    [telephone] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChangeRequestProposer_AUD] (
    [id]        NUMERIC (19)   NOT NULL,
    [ver_rev]   NUMERIC (19)   NOT NULL,
    [REVTYPE]   TINYINT        NULL,
    [address]   NVARCHAR (255) NULL,
    [email]     NVARCHAR (255) NULL,
    [fax]       NVARCHAR (255) NULL,
    [name]      NVARCHAR (255) NULL,
    [postcode]  NVARCHAR (255) NULL,
    [surname]   NVARCHAR (255) NULL,
    [telephone] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChildCareFacilities] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ChildCareFacilit__5FB337D6] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChildrensCentresGroupFlag] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChildrensCentresLeadFlag] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ChildrensCentresPhaseType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[CompaniesHouseDownloadHolder] (
    [companyNumber] NVARCHAR (255) NOT NULL,
    [closureDate]   NVARCHAR (255) NULL,
    [companyStatus] NVARCHAR (255) NULL,
    [edubaseId]     NUMERIC (19)   NOT NULL,
    [edubaseName]   NVARCHAR (255) NOT NULL,
    [edubaseOpen]   TINYINT        NOT NULL,
    [status]        NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([companyNumber] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[CompaniesHouseUpdates] (
    [id]            NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [hasFailed]     TINYINT       NOT NULL,
    [runDate]       DATETIME2 (7) NOT NULL,
    [recordsToLoad] NUMERIC (19)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ContactPreference] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ContactPreferenc__24927208] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ContactType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ContactType__44FF419A] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ContentDocument] (
    [id]      NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [title]   NVARCHAR (255) NOT NULL,
    [version] INT            NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ContentDocument_AUD] (
    [id]      NUMERIC (19)   NOT NULL,
    [ver_rev] NUMERIC (19)   NOT NULL,
    [REVTYPE] TINYINT        NULL,
    [title]   NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ConvertAcademyStatus] (
    [id]     NUMERIC (18)   IDENTITY (1, 1) NOT NULL,
    [tempId] NVARCHAR (255) NULL,
    [data]   NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([tempId] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[County] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NULL,
    [archived] TINYINT        NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__County__3B75D760] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DataDictionary] (
    [shortName]       NVARCHAR (32)   NOT NULL,
    [version]         INT             NOT NULL,
    [description]     NVARCHAR (2000) NULL,
    [fieldUpdated]    NVARCHAR (2000) NULL,
    [notes]           NVARCHAR (2000) NULL,
    [validationRules] NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DataDictionary_aud] (
    [ver_rev]         NUMERIC (19)    NOT NULL,
    [revtype]         SMALLINT        NOT NULL,
    [shortName]       NVARCHAR (32)   NOT NULL,
    [description]     NVARCHAR (2000) NULL,
    [fieldUpdated]    NVARCHAR (2000) NULL,
    [notes]           NVARCHAR (2000) NULL,
    [validationRules] NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DataDictionary_EstablishmentField] (
    [DataDictionary_shortName] NVARCHAR (32) NOT NULL,
    [dependencies_shortName]   NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__DataDictionary_E__0579B962] PRIMARY KEY CLUSTERED ([DataDictionary_shortName] ASC, [dependencies_shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DataDictionary_EstablishmentField_aud] (
    [ver_rev]                  NUMERIC (19) NOT NULL,
    [revtype]                  SMALLINT     NOT NULL,
    [DataDictionary_shortName] VARCHAR (32) NOT NULL,
    [dependencies_shortName]   VARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [DataDictionary_shortName] ASC, [dependencies_shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DatasetName] (
    [code]        NVARCHAR (255) NOT NULL,
    [displayName] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DbSensitiveColumns] (
    [tableName]            NVARCHAR (128)  NOT NULL,
    [columnName]           NVARCHAR (128)  NOT NULL,
    [encrypted]            TINYINT         NOT NULL,
    [encryptedValue]       VARBINARY (MAX) NULL,
    [defaultStringValue]   NVARCHAR (256)  NULL,
    [defaultNumericValue]  INT             NULL,
    [defaultDatetimeValue] DATETIME        NULL,
    CONSTRAINT [PK_DbSensitiveColumns] PRIMARY KEY CLUSTERED ([tableName] ASC, [columnName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Diocese] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Diocese__1ED998B2] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DirectProvisionOfEarlyYears] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DisadvantagedArea] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DistrictAdministrative] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__DistrictAdminist__619B8048] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DistrictAdministrative_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Document] (
    [id]                     NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [archived]               TINYINT         NULL,
    [comments]               NVARCHAR (2048) NULL,
    [createdAt]              DATETIME        NOT NULL,
    [documentName]           NVARCHAR (255)  NULL,
    [fileName]               NVARCHAR (2048) NOT NULL,
    [fileSize]               NUMERIC (19)    NULL,
    [originalFileName]       NVARCHAR (2048) NOT NULL,
    [published]              TINYINT         NULL,
    [status]                 NVARCHAR (255)  NULL,
    [updatedAt]              DATETIME        NULL,
    [uploadedDate]           DATETIME        NULL,
    [version]                INT             NULL,
    [versionNumber]          INT             NULL,
    [createdBy_username]     NVARCHAR (255)  NOT NULL,
    [section]                NVARCHAR (31)   NULL,
    [expirationDate]         DATETIME        NULL,
    [documentPermissionMode] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Document_AUD] (
    [id]                     NUMERIC (19)    NOT NULL,
    [ver_rev]                NUMERIC (19)    NOT NULL,
    [REVTYPE]                TINYINT         NULL,
    [archived]               TINYINT         NULL,
    [comments]               NVARCHAR (2048) NULL,
    [createdAt]              DATETIME        NULL,
    [documentName]           NVARCHAR (255)  NULL,
    [expirationDate]         DATETIME        NULL,
    [fileName]               NVARCHAR (2048) NULL,
    [fileSize]               NUMERIC (19)    NULL,
    [originalFileName]       NVARCHAR (2048) NULL,
    [published]              TINYINT         NULL,
    [status]                 NVARCHAR (255)  NULL,
    [updatedAt]              DATETIME        NULL,
    [uploadedDate]           DATETIME        NULL,
    [versionNumber]          INT             NULL,
    [createdBy_username]     NVARCHAR (255)  NULL,
    [section]                NVARCHAR (31)   NULL,
    [documentPermissionMode] INT             NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Document_UserGroup] (
    [Document_id] NUMERIC (19)  NOT NULL,
    [groups_code] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([Document_id] ASC, [groups_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Document_UserGroup_AUD] (
    [ver_rev]     NUMERIC (19)  NOT NULL,
    [Document_id] NUMERIC (19)  NOT NULL,
    [groups_code] NVARCHAR (32) NOT NULL,
    [REVTYPE]     TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [Document_id] ASC, [groups_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DocumentationSection] (
    [code]        NVARCHAR (31)  NOT NULL,
    [displayName] NVARCHAR (255) NOT NULL,
    [isPermanent] TINYINT        NOT NULL,
    [weight]      INT            NOT NULL,
    [version]     INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[DocumentationSection_AUD] (
    [code]        NVARCHAR (31)  NOT NULL,
    [ver_rev]     NUMERIC (19)   NOT NULL,
    [REVTYPE]     TINYINT        NULL,
    [displayName] NVARCHAR (255) NULL,
    [isPermanent] TINYINT        NULL,
    [weight]      INT            NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EarlyExcellence] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EarlyExcellence__6383C8BA] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Edubase_locks] (
    [id]               NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [lock_name]        NVARCHAR (255)  NOT NULL,
    [lock_description] NVARCHAR (1000) NULL,
    [acquired]         TINYINT         NOT NULL,
    [acquired_since]   DATETIME2 (7)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EdubaseNote] (
    [id]          NVARCHAR (255)  NOT NULL,
    [description] NVARCHAR (255)  NULL,
    [text]        NVARCHAR (4000) NULL,
    CONSTRAINT [PK__EdubaseNote__278EDA44] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EdubaseNote_AUD] (
    [id]          NVARCHAR (255)  NOT NULL,
    [ver_rev]     NUMERIC (19)    NOT NULL,
    [REVTYPE]     TINYINT         NULL,
    [description] NVARCHAR (255)  NULL,
    [text]        NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EduBaseTrigger] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EdubaseTriggerSettings] (
    [id]                        NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [higherThreshold]           NUMERIC (19)  NULL,
    [lowerThreshold]            NUMERIC (19)  NULL,
    [saAccessRestrictions_code] NVARCHAR (32) NULL,
    [governanceThreshold]       NUMERIC (19)  NULL,
    [saSyncInitial]             TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EdubaseTriggerSettings_AUD] (
    [id]                        NUMERIC (19)  NOT NULL,
    [ver_rev]                   NUMERIC (19)  NOT NULL,
    [REVTYPE]                   TINYINT       NULL,
    [higherThreshold]           NUMERIC (19)  NULL,
    [lowerThreshold]            NUMERIC (19)  NULL,
    [saAccessRestrictions_code] NVARCHAR (32) NULL,
    [governanceThreshold]       NUMERIC (19)  NULL,
    [saSyncInitial]             TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EducationActionZone] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EducationActionZ__267ABA7A] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EducationByOthers] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EducationByOther__656C112C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EducationPhase] (
    [code]       NVARCHAR (32)  NOT NULL,
    [group_code] NVARCHAR (32)  NOT NULL,
    [name]       NVARCHAR (255) NOT NULL,
    [archived]   TINYINT        NULL,
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EducationPhase__0425A276] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EducationPhaseGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EducationPhaseGr__023D5A04] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EmailAttachment] (
    [id]       NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [fileName] NVARCHAR (255) NULL,
    [fileSize] NUMERIC (19)   NULL,
    [email_id] NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Establishment] (
    [URN]                                       NUMERIC (19)   NOT NULL,
    [lastChangedDate]                           DATETIME       NULL,
    [UKPRN]                                     NUMERIC (19)   NULL,
    [EstablishmentNumber]                       INT            NULL,
    [PreviousEstablishmentNumber]               INT            NULL,
    [FEHEIdentifier]                            INT            NULL,
    [EstablishmentName]                         NVARCHAR (100) NULL,
    [OpenDate]                                  DATETIME       NULL,
    [CloseDate]                                 DATETIME       NULL,
    [S2SEmail]                                  NVARCHAR (100) NULL,
    [WebsiteAddress]                            NVARCHAR (200) NULL,
    [OffstedLastInspection]                     DATETIME       NULL,
    [SCUAlternativeEmail]                       NVARCHAR (100) NULL,
    [SCUPreferredEmail]                         NVARCHAR (100) NULL,
    [contactPreference_code]                    NVARCHAR (32)  NOT NULL,
    [MarketingOptInOut_code]                    NVARCHAR (32)  NOT NULL,
    [type_code]                                 NVARCHAR (32)  NOT NULL,
    [reasonEstablishmentOpened_code]            NVARCHAR (32)  NOT NULL,
    [burnhamReport_code]                        NVARCHAR (32)  NOT NULL,
    [reasonEstablishmentClosed_code]            NVARCHAR (32)  NOT NULL,
    [superannuationCategory_code]               NVARCHAR (32)  NOT NULL,
    [ofstedSpecialMeasures_code]                NVARCHAR (32)  NOT NULL,
    [educationPhase_code]                       NVARCHAR (32)  NOT NULL,
    [status_code]                               NVARCHAR (32)  NOT NULL,
    [changeReason_code]                         NVARCHAR (32)  NULL,
    [BurnhamGroup]                              INT            NULL,
    [createdDate]                               DATETIME       NULL,
    [SpecialPupils]                             INT            NULL,
    [SchoolCapacity]                            INT            NULL,
    [freshStart_code]                           NVARCHAR (32)  NOT NULL,
    [admissionsPolicy_code]                     NVARCHAR (32)  NOT NULL,
    [gender_code]                               NVARCHAR (32)  NOT NULL,
    [furtherEducationType_code]                 NVARCHAR (32)  NOT NULL,
    [religiousCharacter_code]                   NVARCHAR (32)  NOT NULL,
    [diocese_code]                              NVARCHAR (32)  NOT NULL,
    [StatutoryLowAge]                           INT            NULL,
    [StatutoryHighAge]                          INT            NULL,
    [EYGovernmentFundedChildren]                INT            NULL,
    [Boys2andUnder]                             NVARCHAR (10)  NULL,
    [Boys3]                                     NVARCHAR (10)  NULL,
    [Boys4a]                                    NVARCHAR (10)  NULL,
    [Boys4b]                                    NVARCHAR (10)  NULL,
    [Boys4c]                                    NVARCHAR (10)  NULL,
    [Boys5]                                     NVARCHAR (10)  NULL,
    [Boys6]                                     NVARCHAR (10)  NULL,
    [Boys7]                                     NVARCHAR (10)  NULL,
    [Boys8]                                     NVARCHAR (10)  NULL,
    [Boys9]                                     NVARCHAR (10)  NULL,
    [Boys10]                                    NVARCHAR (10)  NULL,
    [Boys11]                                    NVARCHAR (10)  NULL,
    [Boys12]                                    NVARCHAR (10)  NULL,
    [Boys13]                                    NVARCHAR (10)  NULL,
    [Boys14]                                    NVARCHAR (10)  NULL,
    [Boys15]                                    NVARCHAR (10)  NULL,
    [Boys16]                                    NVARCHAR (10)  NULL,
    [Boys17]                                    NVARCHAR (10)  NULL,
    [Boys18]                                    NVARCHAR (10)  NULL,
    [Boys19plus]                                NVARCHAR (10)  NULL,
    [Girls2andUnder]                            NVARCHAR (10)  NULL,
    [Girls3]                                    NVARCHAR (10)  NULL,
    [Girls4a]                                   NVARCHAR (10)  NULL,
    [Girls4b]                                   NVARCHAR (10)  NULL,
    [Girls4c]                                   NVARCHAR (10)  NULL,
    [Girls5]                                    NVARCHAR (10)  NULL,
    [Girls6]                                    NVARCHAR (10)  NULL,
    [Girls7]                                    NVARCHAR (10)  NULL,
    [Girls8]                                    NVARCHAR (10)  NULL,
    [Girls9]                                    NVARCHAR (10)  NULL,
    [Girls10]                                   NVARCHAR (10)  NULL,
    [Girls11]                                   NVARCHAR (10)  NULL,
    [Girls12]                                   NVARCHAR (10)  NULL,
    [Girls13]                                   NVARCHAR (10)  NULL,
    [Girls14]                                   NVARCHAR (10)  NULL,
    [Girls15]                                   NVARCHAR (10)  NULL,
    [Girls16]                                   NVARCHAR (10)  NULL,
    [Girls17]                                   NVARCHAR (10)  NULL,
    [Girls18]                                   NVARCHAR (10)  NULL,
    [Girls19plus]                               NVARCHAR (10)  NULL,
    [freeSchoolMeals]                           NVARCHAR (10)  NULL,
    [TeenMothersPlaces]                         INT            NULL,
    [PlacesPRU]                                 INT            NULL,
    [ApprovedNumberBoardersSpecial]             INT            NULL,
    [educationByOthers_code]                    NVARCHAR (32)  NOT NULL,
    [excellenceInCitiesCityLearningCentre_code] NVARCHAR (32)  NOT NULL,
    [excellenceInCities_code]                   NVARCHAR (32)  NOT NULL,
    [leadingOption3_code]                       NVARCHAR (32)  NOT NULL,
    [teenagedMothers_code]                      NVARCHAR (32)  NOT NULL,
    [childCareFacilities_code]                  NVARCHAR (32)  NOT NULL,
    [excellenceInCitiesActionZone_code]         NVARCHAR (32)  NOT NULL,
    [specialClasses_code]                       NVARCHAR (32)  NOT NULL,
    [leadershipIncentive_code]                  NVARCHAR (32)  NOT NULL,
    [learningSupportUnit_code]                  NVARCHAR (32)  NOT NULL,
    [privateFinanceInitiative_code]             NVARCHAR (32)  NOT NULL,
    [leadingOption1_code]                       NVARCHAR (32)  NOT NULL,
    [officialSixthForm_code]                    NVARCHAR (32)  NOT NULL,
    [specialEducationalNeeds_code]              NVARCHAR (32)  NOT NULL,
    [educationActionZone_code]                  NVARCHAR (32)  NOT NULL,
    [nurseryProvision_code]                     NVARCHAR (32)  NOT NULL,
    [earlyExcellence_code]                      NVARCHAR (32)  NOT NULL,
    [boarders_code]                             NVARCHAR (32)  NOT NULL,
    [trainingEstablishment_code]                NVARCHAR (32)  NOT NULL,
    [investorInPeople_code]                     NVARCHAR (32)  NOT NULL,
    [ftProvision_code]                          NVARCHAR (32)  NOT NULL,
    [leadingOption2_code]                       NVARCHAR (32)  NOT NULL,
    [registeredEarlyYears_code]                 NVARCHAR (32)  NOT NULL,
    [excellenceInCitiesGroup_code]              NVARCHAR (32)  NOT NULL,
    [emotionalAndBehaviouralDifficulties_code]  NVARCHAR (32)  NOT NULL,
    [TrustName]                                 NVARCHAR (100) NULL,
    [trustSchool_code]                          NVARCHAR (32)  NOT NULL,
    [beacon_code]                               NVARCHAR (32)  NOT NULL,
    [firstMainSpecialism_code]                  NVARCHAR (32)  NOT NULL,
    [firstSecondarySpecialism_code]             NVARCHAR (32)  NOT NULL,
    [secondMainSpecialism_code]                 NVARCHAR (32)  NOT NULL,
    [secondSecondarySpecialism_code]            NVARCHAR (32)  NOT NULL,
    [ConEmail]                                  NVARCHAR (100) NULL,
    [Address3]                                  NVARCHAR (100) NULL,
    [Locality]                                  NVARCHAR (100) NULL,
    [Postcode]                                  NVARCHAR (8)   NULL,
    [Street]                                    NVARCHAR (100) NULL,
    [Town]                                      NVARCHAR (100) NULL,
    [FaxNumber]                                 NVARCHAR (40)  NULL,
    [FaxStd]                                    NVARCHAR (20)  NULL,
    [HeadFirstName]                             NVARCHAR (100) NULL,
    [HeadHonours]                               NVARCHAR (20)  NULL,
    [HeadLastName]                              NVARCHAR (100) NULL,
    [HeadPreferredJobTitle]                     NVARCHAR (50)  NULL,
    [TelephoneNumber]                           NVARCHAR (40)  NULL,
    [TelephoneStd]                              NVARCHAR (20)  NULL,
    [Easting]                                   INT            NULL,
    [Northing]                                  INT            NULL,
    [ConAddress3]                               NVARCHAR (100) NULL,
    [ConLocality]                               NVARCHAR (100) NULL,
    [ConPostcode]                               NVARCHAR (8)   NULL,
    [ConStreet]                                 NVARCHAR (100) NULL,
    [ConTown]                                   NVARCHAR (100) NULL,
    [ConFaxNumber]                              NVARCHAR (40)  NULL,
    [ConFaxStd]                                 NVARCHAR (20)  NULL,
    [ConFirstName]                              NVARCHAR (100) NULL,
    [ConHonours]                                NVARCHAR (20)  NULL,
    [ConLastName]                               NVARCHAR (100) NULL,
    [ConPreferredJobtitle]                      NVARCHAR (50)  NULL,
    [ConTelephoneNumber]                        NVARCHAR (40)  NULL,
    [ConTelephoneStd]                           NVARCHAR (20)  NULL,
    [ConEasting]                                INT            NULL,
    [ConNorthing]                               INT            NULL,
    [administrativeWard_code]                   NVARCHAR (32)  NOT NULL,
    [districtAdministrative_code]               NVARCHAR (32)  NOT NULL,
    [lsoa_code]                                 NVARCHAR (32)  NOT NULL,
    [msoa_code]                                 NVARCHAR (32)  NOT NULL,
    [parliamentaryConstituency_code]            NVARCHAR (32)  NOT NULL,
    [ttwa_code]                                 NVARCHAR (32)  NOT NULL,
    [contactType_code]                          NVARCHAR (32)  NOT NULL,
    [County_code]                               NVARCHAR (32)  NOT NULL,
    [HeadTitle_code]                            NVARCHAR (32)  NOT NULL,
    [CasWard_code]                              NVARCHAR (32)  NOT NULL,
    [LLSC_code]                                 NVARCHAR (32)  NOT NULL,
    [LA_code]                                   NVARCHAR (32)  NOT NULL,
    [PreviousLA_code]                           NVARCHAR (32)  NOT NULL,
    [GOR_code]                                  NVARCHAR (32)  NOT NULL,
    [UrbanRural_code]                           NVARCHAR (32)  NOT NULL,
    [ConCounty_code]                            NVARCHAR (32)  NOT NULL,
    [ConTitle_code]                             NVARCHAR (32)  NOT NULL,
    [ConCasWard_code]                           NVARCHAR (32)  NOT NULL,
    [ConLLSC_code]                              NVARCHAR (32)  NOT NULL,
    [ConLA_code]                                NVARCHAR (32)  NOT NULL,
    [ConPreviousLA_code]                        NVARCHAR (32)  NOT NULL,
    [ConGOR_code]                               NVARCHAR (32)  NOT NULL,
    [ConUrbanRural_code]                        NVARCHAR (32)  NOT NULL,
    [timestamp]                                 ROWVERSION     NOT NULL,
    [movedToEdubaseLogin_code]                  NVARCHAR (32)  NULL,
    [typeOfReservedProvision_code]              NVARCHAR (32)  NULL,
    [helpdeskNotes]                             NVARCHAR (255) NULL,
    [resourcedProvisionCapacity]                INT            NULL,
    [resourcedProvisionOnRoll]                  INT            NULL,
    [senUnitCapacity]                           INT            NULL,
    [senUnitOnRoll]                             INT            NULL,
    [s2sLogin_code]                             NVARCHAR (32)  NULL,
    [percentageOfPupilsReceivingFsm]            NVARCHAR (10)  NULL,
    [version]                                   INT            NOT NULL,
    [checkedSixtyDays]                          TINYINT        NULL,
    [checkedThirtyDays]                         TINYINT        NULL,
    [checkedZeroDays]                           TINYINT        NULL,
    [inspectorateReport]                        NVARCHAR (300) NULL,
    [lastInspectionVisit]                       DATETIME       NULL,
    [nextInspectionVisit]                       DATETIME       NULL,
    [inspectorateName_code]                     NVARCHAR (32)  NULL,
    [GssLaCode_code]                            NVARCHAR (32)  NOT NULL,
    [ConGssLaCode_code]                         NVARCHAR (32)  NOT NULL,
    [studioSchoolIndicator_code]                NVARCHAR (32)  NOT NULL,
    [secureAccessPIN]                           NUMERIC (19)   NULL,
    [eyfsExemption_code]                        NVARCHAR (32)  NULL,
    [converted]                                 TINYINT        NULL,
    [NumberOfBoys]                              NVARCHAR (10)  NULL,
    [NumberOfGirls]                             NVARCHAR (10)  NULL,
    [NumberOfPupils]                            NVARCHAR (10)  NULL,
    [LowestFullTimeAge]                         INT            NULL,
    [HighestFullTimeAge]                        INT            NULL,
    [section41Approved_code]                    NVARCHAR (32)  NULL,
    [EstablishmentNameIndex]                    AS             (CASE WHEN lower(LEFT(rtrim(ltrim([EstablishmentName])), (3))) = 'st.'
                                                                          OR lower(LEFT(rtrim(ltrim([EstablishmentName])), (3))) = 'st ' THEN stuff(rtrim(ltrim([EstablishmentName])), (1), (3), 'Saint ') + ' (St)' WHEN lower(LEFT(rtrim(ltrim([EstablishmentName])), (5))) = 'the  ' THEN stuff(rtrim(ltrim([EstablishmentName])), (1), (5), '') WHEN lower(LEFT(rtrim(ltrim([EstablishmentName])), (4))) = 'the ' THEN stuff(rtrim(ltrim([EstablishmentName])), (1), (4), '') WHEN LEFT(rtrim(ltrim([EstablishmentName])), (2)) = 'Mc' THEN stuff(rtrim(ltrim([EstablishmentName])), (1), (2), 'Mac') ELSE rtrim(ltrim([EstablishmentName])) END) PERSISTED,
    [CorrespondenceAddress]                     NVARCHAR (100) NULL,
    [dateNextWelfareInspection]                 DATETIME       NULL,
    [eduBaseLastUpdate]                         DATETIME       NULL,
    [saOrgId]                                   NUMERIC (19)   NULL,
    [eduBaseTrigger1_code]                      NVARCHAR (32)  NULL,
    [CHAddress3]                                NVARCHAR (255) NULL,
    [CHLocality]                                NVARCHAR (255) NULL,
    [CHName]                                    NVARCHAR (255) NULL,
    [CHPostcode]                                NVARCHAR (255) NULL,
    [CHStreet]                                  NVARCHAR (255) NULL,
    [CHTown]                                    NVARCHAR (255) NULL,
    [TotalPupilsThreeMonth]                     NUMERIC (19)   NULL,
    [CHCounty_code]                             NVARCHAR (32)  NULL,
    [religiousEthos_code]                       NVARCHAR (32)  NULL,
    [governanceDetail]                          NVARCHAR (255) NULL,
    [numberOfUnder5s]                           INT            NULL,
    [childrensCentresPhaseType_code]            NVARCHAR (32)  NULL,
    [directProvisionOfEarlyYears_code]          NVARCHAR (32)  NULL,
    [disadvantagedArea_code]                    NVARCHAR (32)  NULL,
    [governance_code]                           NVARCHAR (32)  NULL,
    [operationalHours_code]                     NVARCHAR (32)  NULL,
    [HeadAppointmentDate]                       DATETIME       NULL,
    [HeadEmail]                                 NVARCHAR (100) NULL,
    [governorsFlag_code]                        NVARCHAR (32)  NULL,
    [governanceLastCheck]                       DATETIME       NULL,
    [ofstedRating_code]                         NVARCHAR (32)  NOT NULL,
    [RSCRegion_code]                            NVARCHAR (32)  NOT NULL,
    [ConRSCRegion_code]                         NVARCHAR (32)  NOT NULL,
    [Country_code]                              NVARCHAR (32)  NOT NULL,
    [ConCountry_code]                           NVARCHAR (32)  NOT NULL,
    [UPRN]                                      NVARCHAR (255) NULL,
    [ConUPRN]                                   NVARCHAR (255) NULL,
    [siteName]                                  NVARCHAR (255) NULL,
    [ConSiteName]                               NVARCHAR (255) NULL,
    [qualityAssuranceBodyName_code]             VARCHAR (32)   NULL,
    [establishmentAccredited_code]              VARCHAR (32)   NULL,
    [QABReport]                                 VARCHAR (200)  NULL,
    [CHNumber]                                  VARCHAR (20)   NULL,
    [accreditationExpiryDate]                   DATETIME       NULL,
    CONSTRAINT [PK_Establishment] PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Establishment_AdditionalAddress_AUD] (
    [ver_rev] NUMERIC (19) NOT NULL,
    [URN]     NUMERIC (19) NOT NULL,
    [id]      NUMERIC (19) NOT NULL,
    [REVTYPE] TINYINT      NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Establishment_AUD] (
    [URN]                                       NUMERIC (19)   NOT NULL,
    [ver_rev]                                   NUMERIC (19)   NOT NULL,
    [REVTYPE]                                   TINYINT        NULL,
    [FEHEIdentifier]                            INT            NULL,
    [Boys10]                                    NVARCHAR (10)  NULL,
    [Boys11]                                    NVARCHAR (10)  NULL,
    [Boys12]                                    NVARCHAR (10)  NULL,
    [Boys13]                                    NVARCHAR (10)  NULL,
    [Boys14]                                    NVARCHAR (10)  NULL,
    [Boys15]                                    NVARCHAR (10)  NULL,
    [Boys16]                                    NVARCHAR (10)  NULL,
    [Boys17]                                    NVARCHAR (10)  NULL,
    [Boys18]                                    NVARCHAR (10)  NULL,
    [Boys19plus]                                NVARCHAR (10)  NULL,
    [Boys2andUnder]                             NVARCHAR (10)  NULL,
    [Boys3]                                     NVARCHAR (10)  NULL,
    [Boys4a]                                    NVARCHAR (10)  NULL,
    [Boys4b]                                    NVARCHAR (10)  NULL,
    [Boys4c]                                    NVARCHAR (10)  NULL,
    [Boys5]                                     NVARCHAR (10)  NULL,
    [Boys6]                                     NVARCHAR (10)  NULL,
    [Boys7]                                     NVARCHAR (10)  NULL,
    [Boys8]                                     NVARCHAR (10)  NULL,
    [Boys9]                                     NVARCHAR (10)  NULL,
    [EYGovernmentFundedChildren]                INT            NULL,
    [Girls10]                                   NVARCHAR (10)  NULL,
    [Girls11]                                   NVARCHAR (10)  NULL,
    [Girls12]                                   NVARCHAR (10)  NULL,
    [Girls13]                                   NVARCHAR (10)  NULL,
    [Girls14]                                   NVARCHAR (10)  NULL,
    [Girls15]                                   NVARCHAR (10)  NULL,
    [Girls16]                                   NVARCHAR (10)  NULL,
    [Girls17]                                   NVARCHAR (10)  NULL,
    [Girls18]                                   NVARCHAR (10)  NULL,
    [Girls19plus]                               NVARCHAR (10)  NULL,
    [Girls2andUnder]                            NVARCHAR (10)  NULL,
    [Girls3]                                    NVARCHAR (10)  NULL,
    [Girls4a]                                   NVARCHAR (10)  NULL,
    [Girls4b]                                   NVARCHAR (10)  NULL,
    [Girls4c]                                   NVARCHAR (10)  NULL,
    [Girls5]                                    NVARCHAR (10)  NULL,
    [Girls6]                                    NVARCHAR (10)  NULL,
    [Girls7]                                    NVARCHAR (10)  NULL,
    [Girls8]                                    NVARCHAR (10)  NULL,
    [Girls9]                                    NVARCHAR (10)  NULL,
    [BurnhamGroup]                              INT            NULL,
    [SchoolCapacity]                            INT            NULL,
    [SpecialPupils]                             INT            NULL,
    [StatutoryHighAge]                          INT            NULL,
    [StatutoryLowAge]                           INT            NULL,
    [checked]                                   TINYINT        NULL,
    [CloseDate]                                 DATETIME       NULL,
    [ConEmail]                                  NVARCHAR (100) NULL,
    [createdDate]                               DATETIME       NULL,
    [EstablishmentNumber]                       INT            NULL,
    [lastChangedDate]                           DATETIME       NULL,
    [EstablishmentName]                         NVARCHAR (100) NULL,
    [EstablishmentNameIndex]                    NVARCHAR (108) NULL,
    [Address3]                                  NVARCHAR (100) NULL,
    [Locality]                                  NVARCHAR (255) NULL,
    [Postcode]                                  NVARCHAR (255) NULL,
    [Street]                                    NVARCHAR (255) NULL,
    [Town]                                      NVARCHAR (255) NULL,
    [FaxNumber]                                 NVARCHAR (255) NULL,
    [FaxStd]                                    NVARCHAR (255) NULL,
    [HeadFirstName]                             NVARCHAR (255) NULL,
    [HeadHonours]                               NVARCHAR (255) NULL,
    [HeadLastName]                              NVARCHAR (255) NULL,
    [HeadPreferredJobTitle]                     NVARCHAR (255) NULL,
    [TelephoneNumber]                           NVARCHAR (255) NULL,
    [TelephoneStd]                              NVARCHAR (255) NULL,
    [Easting]                                   INT            NULL,
    [Northing]                                  INT            NULL,
    [OffstedLastInspection]                     DATETIME       NULL,
    [OpenDate]                                  DATETIME       NULL,
    [ConAddress3]                               NVARCHAR (100) NULL,
    [ConLocality]                               NVARCHAR (255) NULL,
    [ConPostcode]                               NVARCHAR (255) NULL,
    [ConStreet]                                 NVARCHAR (255) NULL,
    [ConTown]                                   NVARCHAR (255) NULL,
    [ConFaxNumber]                              NVARCHAR (255) NULL,
    [ConFaxStd]                                 NVARCHAR (255) NULL,
    [ConFirstName]                              NVARCHAR (255) NULL,
    [ConHonours]                                NVARCHAR (255) NULL,
    [ConLastName]                               NVARCHAR (255) NULL,
    [ConPreferredJobtitle]                      NVARCHAR (255) NULL,
    [ConTelephoneNumber]                        NVARCHAR (255) NULL,
    [ConTelephoneStd]                           NVARCHAR (255) NULL,
    [ConEasting]                                INT            NULL,
    [ConNorthing]                               INT            NULL,
    [PreviousEstablishmentNumber]               INT            NULL,
    [S2SEmail]                                  NVARCHAR (100) NULL,
    [SCUAlternativeEmail]                       NVARCHAR (100) NULL,
    [SCUPreferredEmail]                         NVARCHAR (100) NULL,
    [ApprovedNumberBoardersSpecial]             INT            NULL,
    [freeSchoolMeals]                           NVARCHAR (10)  NULL,
    [PlacesPRU]                                 INT            NULL,
    [TeenMothersPlaces]                         INT            NULL,
    [TrustName]                                 NVARCHAR (100) NULL,
    [UKPRN]                                     INT            NULL,
    [WebsiteAddress]                            NVARCHAR (200) NULL,
    [administrativeWard_code]                   NVARCHAR (32)  NULL,
    [districtAdministrative_code]               NVARCHAR (32)  NULL,
    [lsoa_code]                                 NVARCHAR (32)  NULL,
    [msoa_code]                                 NVARCHAR (32)  NULL,
    [parliamentaryConstituency_code]            NVARCHAR (32)  NULL,
    [ttwa_code]                                 NVARCHAR (32)  NULL,
    [burnhamReport_code]                        NVARCHAR (32)  NULL,
    [changeReason_code]                         NVARCHAR (32)  NULL,
    [admissionsPolicy_code]                     NVARCHAR (32)  NULL,
    [diocese_code]                              NVARCHAR (32)  NULL,
    [freshStart_code]                           NVARCHAR (32)  NULL,
    [furtherEducationType_code]                 NVARCHAR (32)  NULL,
    [gender_code]                               NVARCHAR (32)  NULL,
    [religiousCharacter_code]                   NVARCHAR (32)  NULL,
    [contactPreference_code]                    NVARCHAR (32)  NULL,
    [contactType_code]                          NVARCHAR (32)  NULL,
    [educationPhase_code]                       NVARCHAR (32)  NULL,
    [MarketingOptInOut_code]                    NVARCHAR (32)  NULL,
    [movedToEdubaseLogin_code]                  NVARCHAR (32)  NULL,
    [County_code]                               NVARCHAR (32)  NULL,
    [HeadTitle_code]                            NVARCHAR (32)  NULL,
    [CasWard_code]                              NVARCHAR (32)  NULL,
    [LLSC_code]                                 NVARCHAR (32)  NULL,
    [LA_code]                                   NVARCHAR (32)  NULL,
    [PreviousLA_code]                           NVARCHAR (32)  NULL,
    [GOR_code]                                  NVARCHAR (32)  NULL,
    [UrbanRural_code]                           NVARCHAR (32)  NULL,
    [ofstedSpecialMeasures_code]                NVARCHAR (32)  NULL,
    [ConCounty_code]                            NVARCHAR (32)  NULL,
    [ConTitle_code]                             NVARCHAR (32)  NULL,
    [ConCasWard_code]                           NVARCHAR (32)  NULL,
    [ConLLSC_code]                              NVARCHAR (32)  NULL,
    [ConLA_code]                                NVARCHAR (32)  NULL,
    [ConPreviousLA_code]                        NVARCHAR (32)  NULL,
    [ConGOR_code]                               NVARCHAR (32)  NULL,
    [ConUrbanRural_code]                        NVARCHAR (32)  NULL,
    [reasonEstablishmentClosed_code]            NVARCHAR (32)  NULL,
    [reasonEstablishmentOpened_code]            NVARCHAR (32)  NULL,
    [beacon_code]                               NVARCHAR (32)  NULL,
    [boarders_code]                             NVARCHAR (32)  NULL,
    [childCareFacilities_code]                  NVARCHAR (32)  NULL,
    [earlyExcellence_code]                      NVARCHAR (32)  NULL,
    [educationActionZone_code]                  NVARCHAR (32)  NULL,
    [educationByOthers_code]                    NVARCHAR (32)  NULL,
    [emotionalAndBehaviouralDifficulties_code]  NVARCHAR (32)  NULL,
    [excellenceInCities_code]                   NVARCHAR (32)  NULL,
    [excellenceInCitiesActionZone_code]         NVARCHAR (32)  NULL,
    [excellenceInCitiesCityLearningCentre_code] NVARCHAR (32)  NULL,
    [excellenceInCitiesGroup_code]              NVARCHAR (32)  NULL,
    [firstMainSpecialism_code]                  NVARCHAR (32)  NULL,
    [firstSecondarySpecialism_code]             NVARCHAR (32)  NULL,
    [ftProvision_code]                          NVARCHAR (32)  NULL,
    [investorInPeople_code]                     NVARCHAR (32)  NULL,
    [leadershipIncentive_code]                  NVARCHAR (32)  NULL,
    [leadingOption1_code]                       NVARCHAR (32)  NULL,
    [leadingOption2_code]                       NVARCHAR (32)  NULL,
    [leadingOption3_code]                       NVARCHAR (32)  NULL,
    [learningSupportUnit_code]                  NVARCHAR (32)  NULL,
    [nurseryProvision_code]                     NVARCHAR (32)  NULL,
    [officialSixthForm_code]                    NVARCHAR (32)  NULL,
    [privateFinanceInitiative_code]             NVARCHAR (32)  NULL,
    [registeredEarlyYears_code]                 NVARCHAR (32)  NULL,
    [secondMainSpecialism_code]                 NVARCHAR (32)  NULL,
    [secondSecondarySpecialism_code]            NVARCHAR (32)  NULL,
    [specialClasses_code]                       NVARCHAR (32)  NULL,
    [specialEducationalNeeds_code]              NVARCHAR (32)  NULL,
    [teenagedMothers_code]                      NVARCHAR (32)  NULL,
    [trainingEstablishment_code]                NVARCHAR (32)  NULL,
    [trustSchool_code]                          NVARCHAR (32)  NULL,
    [status_code]                               NVARCHAR (32)  NULL,
    [superannuationCategory_code]               NVARCHAR (32)  NULL,
    [type_code]                                 NVARCHAR (32)  NULL,
    [helpdeskNotes]                             NVARCHAR (255) NULL,
    [resourcedProvisionCapacity]                INT            NULL,
    [resourcedProvisionOnRoll]                  INT            NULL,
    [senUnitCapacity]                           INT            NULL,
    [senUnitOnRoll]                             INT            NULL,
    [typeOfReservedProvision_code]              NVARCHAR (32)  NULL,
    [percentageOfPupilsReceivingFsm]            NVARCHAR (10)  NULL,
    [s2sLogin_code]                             NVARCHAR (32)  NULL,
    [checkedSixtyDays]                          TINYINT        NULL,
    [checkedThirtyDays]                         TINYINT        NULL,
    [checkedZeroDays]                           TINYINT        NULL,
    [inspectorateReport]                        NVARCHAR (300) NULL,
    [lastInspectionVisit]                       DATETIME       NULL,
    [nextInspectionVisit]                       DATETIME       NULL,
    [inspectorateName_code]                     NVARCHAR (32)  NULL,
    [GssLaCode_code]                            NVARCHAR (32)  NULL,
    [ConGssLaCode_code]                         NVARCHAR (32)  NULL,
    [studioSchoolIndicator_code]                NVARCHAR (32)  NULL,
    [secureAccessPIN]                           NUMERIC (19)   NULL,
    [eyfsExemption_code]                        NVARCHAR (32)  NULL,
    [converted]                                 TINYINT        NULL,
    [NumberOfBoys]                              NVARCHAR (10)  NULL,
    [NumberOfGirls]                             NVARCHAR (10)  NULL,
    [NumberOfPupils]                            NVARCHAR (10)  NULL,
    [LowestFullTimeAge]                         INT            NULL,
    [HighestFullTimeAge]                        INT            NULL,
    [section41Approved_code]                    NVARCHAR (32)  NULL,
    [CorrespondenceAddress]                     NVARCHAR (100) NULL,
    [dateNextWelfareInspection]                 DATETIME       NULL,
    [eduBaseLastUpdate]                         DATETIME       NULL,
    [saOrgId]                                   NUMERIC (19)   NULL,
    [eduBaseTrigger1_code]                      NVARCHAR (32)  NULL,
    [CHAddress3]                                NVARCHAR (255) NULL,
    [CHLocality]                                NVARCHAR (255) NULL,
    [CHName]                                    NVARCHAR (255) NULL,
    [CHPostcode]                                NVARCHAR (255) NULL,
    [CHStreet]                                  NVARCHAR (255) NULL,
    [CHTown]                                    NVARCHAR (255) NULL,
    [TotalPupilsThreeMonth]                     NUMERIC (19)   NULL,
    [CHCounty_code]                             NVARCHAR (32)  NULL,
    [religiousEthos_code]                       NVARCHAR (32)  NULL,
    [governanceDetail]                          NVARCHAR (255) NULL,
    [numberOfUnder5s]                           INT            NULL,
    [childrensCentresPhaseType_code]            NVARCHAR (32)  NULL,
    [directProvisionOfEarlyYears_code]          NVARCHAR (32)  NULL,
    [disadvantagedArea_code]                    NVARCHAR (32)  NULL,
    [governance_code]                           NVARCHAR (32)  NULL,
    [operationalHours_code]                     NVARCHAR (32)  NULL,
    [HeadAppointmentDate]                       DATETIME       NULL,
    [HeadEmail]                                 NVARCHAR (100) NULL,
    [governorsFlag_code]                        NVARCHAR (32)  NULL,
    [governanceLastCheck]                       DATETIME       NULL,
    [ofstedRating_code]                         NVARCHAR (32)  NOT NULL,
    [RSCRegion_code]                            NVARCHAR (32)  NULL,
    [ConRSCRegion_code]                         NVARCHAR (32)  NULL,
    [Country_code]                              NVARCHAR (32)  NULL,
    [ConCountry_code]                           NVARCHAR (32)  NULL,
    [UPRN]                                      NVARCHAR (255) NULL,
    [ConUPRN]                                   NVARCHAR (255) NULL,
    [siteName]                                  NVARCHAR (255) NULL,
    [ConSiteName]                               NVARCHAR (255) NULL,
    [establishmentAccredited_code]              VARCHAR (32)   NULL,
    [qualityAssuranceBodyName_code]             VARCHAR (32)   NULL,
    [CHNumber]                                  VARCHAR (20)   NULL,
    [QABReport]                                 VARCHAR (200)  NULL,
    [accreditationExpiryDate]                   DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([URN] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Establishment_EstablishmentLink_AUD] (
    [ver_rev] NUMERIC (19) NOT NULL,
    [URN]     NUMERIC (19) NOT NULL,
    [id]      NUMERIC (19) NOT NULL,
    [REVTYPE] TINYINT      NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentAccredited] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [code]     VARCHAR (32)  NOT NULL,
    [name]     VARCHAR (255) NOT NULL,
    [archived] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentAdditionalAddresses] (
    [id]            NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [urn]           NUMERIC (19)   NOT NULL,
    [record_number] NUMERIC (19)   NOT NULL,
    [address3]      NVARCHAR (255) NULL,
    [locality]      NVARCHAR (100) NULL,
    [postcode]      NVARCHAR (8)   NULL,
    [siteName]      NVARCHAR (255) NULL,
    [street]        NVARCHAR (100) NULL,
    [town]          NVARCHAR (100) NULL,
    [uprn]          NVARCHAR (255) NULL,
    [country_code]  NVARCHAR (32)  NOT NULL,
    [county_code]   NVARCHAR (32)  NOT NULL,
    [easting]       INT            NULL,
    [northing]      INT            NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([urn] ASC, [record_number] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentAdditionalAddresses_AUD] (
    [id]            NUMERIC (19)   NOT NULL,
    [ver_rev]       NUMERIC (19)   NOT NULL,
    [REVTYPE]       TINYINT        NULL,
    [urn]           NUMERIC (19)   NULL,
    [record_number] NUMERIC (19)   NULL,
    [address3]      NVARCHAR (255) NULL,
    [locality]      NVARCHAR (100) NULL,
    [postcode]      NVARCHAR (8)   NULL,
    [siteName]      NVARCHAR (255) NULL,
    [street]        NVARCHAR (100) NULL,
    [town]          NVARCHAR (100) NULL,
    [uprn]          NVARCHAR (255) NULL,
    [country_code]  NVARCHAR (32)  NULL,
    [county_code]   NVARCHAR (32)  NULL,
    [easting]       INT            NULL,
    [northing]      INT            NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentChangeHistory] (
    [type]                               NVARCHAR (64)   NOT NULL,
    [id]                                 NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [changeDate]                         DATETIME        NULL,
    [dueDate]                            DATETIME        NULL,
    [lastChangeDate]                     DATETIME        NULL,
    [status]                             NVARCHAR (255)  NULL,
    [newValue]                           NVARCHAR (4000) NULL,
    [oldValue]                           NVARCHAR (4000) NULL,
    [numericNewValue]                    NUMERIC (19)    NULL,
    [numericOldValue]                    NUMERIC (19)    NULL,
    [dateNewValue]                       DATETIME        NULL,
    [dateOldValue]                       DATETIME        NULL,
    [approvedOrRejectedBy_username]      NVARCHAR (255)  NULL,
    [dataOwner_code]                     NVARCHAR (32)   NULL,
    [establishment_URN]                  NUMERIC (19)    NOT NULL,
    [field_shortName]                    NVARCHAR (32)   NOT NULL,
    [proposedByUser_username]            NVARCHAR (255)  NULL,
    [proposer_id]                        NUMERIC (19)    NULL,
    [changeReason]                       NVARCHAR (255)  NULL,
    [changedByBO]                        TINYINT         NULL,
    [changedInError]                     TINYINT         NULL,
    [delayReason]                        NVARCHAR (255)  NULL,
    [proposedByExternalUser]             TINYINT         NULL,
    [rejectReason]                       NVARCHAR (1024) NULL,
    [linkURN]                            NUMERIC (19)    NULL,
    [createdDate]                        DATETIME        NULL,
    [version]                            INT             NULL,
    [effectiveDate]                      DATETIME        NULL,
    [approvedOrRejectedByUserGroup_code] NVARCHAR (32)   NULL,
    [proposedByUserGroup_code]           NVARCHAR (32)   NULL,
    [sameUser]                           AS              (CONVERT (TINYINT, CASE WHEN [proposedByUser_username] IS NULL
                                                                                      AND [approvedOrRejectedBy_username] IS NULL THEN (1) WHEN [proposedByUser_username] IS NULL
                                                                                                                                                OR [approvedOrRejectedBy_username] IS NULL THEN (0) WHEN [proposedByUser_username] <> [approvedOrRejectedBy_username] THEN (0) ELSE (1) END, (0))) PERSISTED NOT NULL,
    [additionalAddressNumber]            INT             NULL,
    [additionalProprietorNumber]         INT             NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentChangeHistory_aud] (
    [ver_rev]                            NUMERIC (19)    NOT NULL,
    [REVTYPE]                            TINYINT         NULL,
    [type]                               NVARCHAR (64)   NOT NULL,
    [id]                                 NUMERIC (19)    NOT NULL,
    [changeDate]                         DATETIME        NULL,
    [dueDate]                            DATETIME        NULL,
    [lastChangeDate]                     DATETIME        NULL,
    [status]                             NVARCHAR (255)  NULL,
    [newValue]                           NVARCHAR (4000) NULL,
    [oldValue]                           NVARCHAR (4000) NULL,
    [numericNewValue]                    NUMERIC (19)    NULL,
    [numericOldValue]                    NUMERIC (19)    NULL,
    [dateNewValue]                       DATETIME        NULL,
    [dateOldValue]                       DATETIME        NULL,
    [approvedOrRejectedBy_username]      NVARCHAR (255)  NULL,
    [dataOwner_code]                     NVARCHAR (32)   NULL,
    [establishment_URN]                  NUMERIC (19)    NULL,
    [field_shortName]                    NVARCHAR (32)   NULL,
    [proposedByUser_username]            NVARCHAR (255)  NULL,
    [proposer_id]                        NUMERIC (19)    NULL,
    [changeReason]                       NVARCHAR (255)  NULL,
    [changedByBO]                        TINYINT         NULL,
    [changedInError]                     TINYINT         NULL,
    [delayReason]                        NVARCHAR (255)  NULL,
    [proposedByExternalUser]             TINYINT         NULL,
    [rejectReason]                       NVARCHAR (1024) NULL,
    [linkURN]                            NUMERIC (19)    NULL,
    [createdDate]                        DATETIME        NULL,
    [version]                            INT             NULL,
    [effectiveDate]                      DATETIME        NULL,
    [approvedOrRejectedByUserGroup_code] NVARCHAR (32)   NULL,
    [proposedByUserGroup_code]           NVARCHAR (32)   NULL,
    [sameUser]                           TINYINT         NULL,
    [additionalAddressNumber]            INT             NULL,
    [additionalProprietorNumber]         INT             NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentChangeHistory_AUDArchive1] (
    [ver_rev]                            NUMERIC (19)    NOT NULL,
    [REVTYPE]                            TINYINT         NULL,
    [type]                               NVARCHAR (64)   NOT NULL,
    [id]                                 NUMERIC (19)    NOT NULL,
    [changeDate]                         DATETIME        NULL,
    [dueDate]                            DATETIME        NULL,
    [lastChangeDate]                     DATETIME        NULL,
    [status]                             NVARCHAR (255)  NULL,
    [newValue]                           NVARCHAR (4000) NULL,
    [oldValue]                           NVARCHAR (4000) NULL,
    [numericNewValue]                    NUMERIC (19)    NULL,
    [numericOldValue]                    NUMERIC (19)    NULL,
    [dateNewValue]                       DATETIME        NULL,
    [dateOldValue]                       DATETIME        NULL,
    [approvedOrRejectedBy_username]      NVARCHAR (255)  NULL,
    [dataOwner_code]                     NVARCHAR (32)   NULL,
    [establishment_URN]                  NUMERIC (19)    NULL,
    [field_shortName]                    NVARCHAR (32)   NULL,
    [proposedByUser_username]            NVARCHAR (255)  NULL,
    [proposer_id]                        NUMERIC (19)    NULL,
    [changeReason]                       NVARCHAR (255)  NULL,
    [changedByBO]                        TINYINT         NULL,
    [changedInError]                     TINYINT         NULL,
    [delayReason]                        NVARCHAR (255)  NULL,
    [proposedByExternalUser]             TINYINT         NULL,
    [rejectReason]                       NVARCHAR (1024) NULL,
    [linkURN]                            NUMERIC (19)    NULL,
    [createdDate]                        DATETIME        NULL,
    [version]                            INT             NULL,
    [effectiveDate]                      DATETIME        NULL,
    [approvedOrRejectedByUserGroup_code] NVARCHAR (32)   NULL,
    [proposedByUserGroup_code]           NVARCHAR (32)   NULL,
    [sameUser]                           TINYINT         NULL,
    [additionalAddressNumber]            INT             NULL,
    [additionalProprietorNumber]         INT             NULL
);


GO

CREATE TABLE [dbo].[EstablishmentChangeHistoryArchive1] (
    [type]                               NVARCHAR (64)   NOT NULL,
    [id]                                 NUMERIC (19)    NOT NULL,
    [changeDate]                         DATETIME        NULL,
    [dueDate]                            DATETIME        NULL,
    [lastChangeDate]                     DATETIME        NULL,
    [status]                             NVARCHAR (255)  NULL,
    [newValue]                           NVARCHAR (4000) NULL,
    [oldValue]                           NVARCHAR (4000) NULL,
    [numericNewValue]                    NUMERIC (19)    NULL,
    [numericOldValue]                    NUMERIC (19)    NULL,
    [dateNewValue]                       DATETIME        NULL,
    [dateOldValue]                       DATETIME        NULL,
    [approvedOrRejectedBy_username]      NVARCHAR (255)  NULL,
    [dataOwner_code]                     NVARCHAR (32)   NULL,
    [establishment_URN]                  NUMERIC (19)    NOT NULL,
    [field_shortName]                    NVARCHAR (32)   NOT NULL,
    [proposedByUser_username]            NVARCHAR (255)  NULL,
    [proposer_id]                        NUMERIC (19)    NULL,
    [changeReason]                       NVARCHAR (255)  NULL,
    [changedByBO]                        TINYINT         NULL,
    [changedInError]                     TINYINT         NULL,
    [delayReason]                        NVARCHAR (255)  NULL,
    [proposedByExternalUser]             TINYINT         NULL,
    [rejectReason]                       NVARCHAR (1024) NULL,
    [linkURN]                            NUMERIC (19)    NULL,
    [createdDate]                        DATETIME        NULL,
    [version]                            INT             NULL,
    [effectiveDate]                      DATETIME        NULL,
    [approvedOrRejectedByUserGroup_code] NVARCHAR (32)   NULL,
    [proposedByUserGroup_code]           NVARCHAR (32)   NULL,
    [sameUser]                           TINYINT         NULL,
    [additionalAddressNumber]            INT             NULL,
    [additionalProprietorNumber]         INT             NULL
);


GO

CREATE TABLE [dbo].[EstablishmentField] (
    [shortName]                 NVARCHAR (32)  NOT NULL,
    [oldShortName]              NVARCHAR (32)  NULL,
    [displayName]               NVARCHAR (255) NULL,
    [oldDisplayName]            NVARCHAR (255) NULL,
    [fieldtype]                 NVARCHAR (31)  NOT NULL,
    [path]                      NVARCHAR (64)  NULL,
    [legacy]                    TINYINT        NULL,
    [readonly]                  TINYINT        NULL,
    [lookupClass]               NVARCHAR (255) NULL,
    [filterability]             TINYINT        NULL,
    [defaultOptionForFilter]    TINYINT        NULL,
    [group_code]                NVARCHAR (32)  NULL,
    [toeGroup_code]             NVARCHAR (32)  NULL,
    [type]                      NVARCHAR (255) NULL,
    [sen_code]                  NVARCHAR (32)  NULL,
    [completenessThreshold]     FLOAT (53)     NULL,
    [consistencyThreshold]      INT            NULL,
    [consistencyCheckedDate]    DATETIME       NULL,
    [tableName]                 NVARCHAR (32)  NULL,
    [columnName]                NVARCHAR (64)  NULL,
    [length]                    INT            NULL,
    [lookupTable]               NVARCHAR (64)  NULL,
    [shortDisplayName]          NVARCHAR (64)  NULL,
    [childrenCentreDisplayName] NVARCHAR (64)  NULL,
    [orderInExtracts]           INT            NULL,
    [archived]                  BIT            NULL,
    CONSTRAINT [PK__EstablishmentFie__6ABAD62E] PRIMARY KEY CLUSTERED ([shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentField_AUD] (
    [fieldtype]                 NVARCHAR (31)  NOT NULL,
    [shortName]                 NVARCHAR (32)  NOT NULL,
    [ver_rev]                   NUMERIC (19)   NOT NULL,
    [REVTYPE]                   TINYINT        NULL,
    [columnName]                NVARCHAR (255) NULL,
    [completenessThreshold]     FLOAT (53)     NULL,
    [consistencyCheckedDate]    DATETIME       NULL,
    [consistencyThreshold]      FLOAT (53)     NULL,
    [defaultOptionForFilter]    TINYINT        NULL,
    [displayName]               NVARCHAR (255) NULL,
    [filterability]             TINYINT        NULL,
    [legacy]                    TINYINT        NULL,
    [oldDisplayName]            NVARCHAR (255) NULL,
    [oldShortName]              NVARCHAR (32)  NULL,
    [path]                      NVARCHAR (64)  NULL,
    [readonly]                  TINYINT        NULL,
    [tableName]                 NVARCHAR (255) NULL,
    [type]                      NVARCHAR (255) NULL,
    [length]                    INT            NULL,
    [group_code]                NVARCHAR (32)  NULL,
    [toeGroup_code]             NVARCHAR (32)  NULL,
    [sen_code]                  NVARCHAR (32)  NULL,
    [shortDisplayName]          NVARCHAR (64)  NULL,
    [childrenCentreDisplayName] NVARCHAR (64)  NULL,
    [orderInExtracts]           INT            NULL,
    [archived]                  BIT            NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFieldBase_ByTypeFieldPermission_AUD] (
    [ver_rev] NUMERIC (19)  NOT NULL,
    [field]   NVARCHAR (32) NOT NULL,
    [id]      NUMERIC (19)  NOT NULL,
    [REVTYPE] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [field] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFieldBase_EstablishmentFieldToTypeMapping_AUD] (
    [ver_rev] NUMERIC (19)  NOT NULL,
    [field]   NVARCHAR (32) NOT NULL,
    [id]      NUMERIC (19)  NOT NULL,
    [REVTYPE] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [field] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFieldBase_FieldPosition_AUD] (
    [ver_rev]   NUMERIC (19)  NOT NULL,
    [shortName] NVARCHAR (32) NOT NULL,
    [id]        NUMERIC (19)  NOT NULL,
    [REVTYPE]   TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [shortName] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFieldToTypeMapping] (
    [id]    NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [field] NVARCHAR (32) NOT NULL,
    [type]  NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([field] ASC, [type] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFieldToTypeMappingForExtracts] (
    [id]    NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [field] NVARCHAR (32) NOT NULL,
    [type]  NVARCHAR (32) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([field] ASC, [type] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFieldValidation] (
    [id]                 NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [shortName]          NVARCHAR (32) NOT NULL,
    [validationRuleCode] VARCHAR (32)  NOT NULL,
    [validationParam]    VARCHAR (512) NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([shortName] ASC, [validationRuleCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentFilter] (
    [id]                       NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [creationDate]             DATETIME       NOT NULL,
    [creatorUsername]          NVARCHAR (255) NOT NULL,
    [name]                     NVARCHAR (255) NOT NULL,
    [creatorGroup_code]        NVARCHAR (32)  NOT NULL,
    [bringUpField]             NVARCHAR (32)  NULL,
    [isExcludeChildrenCentres] TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentGroup] (
    [id]                            NUMERIC (19)    NOT NULL,
    [name]                          NVARCHAR (255)  NULL,
    [openDate]                      DATETIME        NOT NULL,
    [type_code]                     NVARCHAR (32)   NOT NULL,
    [GroupFaxNumber]                NVARCHAR (255)  NULL,
    [GroupFaxStd]                   NVARCHAR (255)  NULL,
    [HeadFirstName]                 NVARCHAR (255)  NULL,
    [HeadHonours]                   NVARCHAR (255)  NULL,
    [HeadLastName]                  NVARCHAR (255)  NULL,
    [HeadPreferredJobTitle]         NVARCHAR (255)  NULL,
    [GroupTelephoneNumber]          NVARCHAR (255)  NULL,
    [GroupTelephoneStd]             NVARCHAR (255)  NULL,
    [closedDate]                    DATETIME        NULL,
    [GroupAddress3]                 NVARCHAR (100)  NULL,
    [GroupLocality]                 NVARCHAR (255)  NULL,
    [GroupPostcode]                 NVARCHAR (255)  NULL,
    [GroupStreet]                   NVARCHAR (255)  NULL,
    [GroupTown]                     NVARCHAR (255)  NULL,
    [GroupContactFaxNumber]         NVARCHAR (255)  NULL,
    [GroupContactFaxStd]            NVARCHAR (255)  NULL,
    [GroupContactFirstName]         NVARCHAR (255)  NULL,
    [GroupContactHonours]           NVARCHAR (255)  NULL,
    [GroupContactLastName]          NVARCHAR (255)  NULL,
    [GroupContactPreferredJobTitle] NVARCHAR (255)  NULL,
    [GroupContactTelephoneNumber]   NVARCHAR (255)  NULL,
    [GroupContactTelephoneStd]      NVARCHAR (255)  NULL,
    [GroupContactAddress3]          NVARCHAR (100)  NULL,
    [GroupContactLocality]          NVARCHAR (255)  NULL,
    [GroupContactPostcode]          NVARCHAR (255)  NULL,
    [GroupContactStreet]            NVARCHAR (255)  NULL,
    [GroupContactTown]              NVARCHAR (255)  NULL,
    [HeadTitle_code]                NVARCHAR (32)   NOT NULL,
    [GroupCounty_code]              NVARCHAR (32)   NOT NULL,
    [GroupContactTitle_code]        NVARCHAR (32)   NOT NULL,
    [GroupContactCounty_code]       NVARCHAR (32)   NOT NULL,
    [groupContactType_code]         NVARCHAR (32)   NOT NULL,
    [email]                         NVARCHAR (255)  NULL,
    [createdInError]                TINYINT         NOT NULL,
    [sharedHeadTeacher]             TINYINT         NULL,
    [companiesHouseNumber]          NVARCHAR (255)  NULL,
    [groupId]                       NVARCHAR (255)  NULL,
    [groupContactEmail]             NVARCHAR (255)  NULL,
    [localAuthority_code]           NVARCHAR (32)   NULL,
    [saOrgId]                       NUMERIC (19)    NULL,
    [governanceLastCheck]           DATETIME        NULL,
    [delegateAuthDetails]           NVARCHAR (1000) NULL,
    [GroupCountry_code]             NVARCHAR (32)   NOT NULL,
    [GroupContactCountry_code]      NVARCHAR (32)   NOT NULL,
    [GroupPostUPRN]                 NVARCHAR (255)  NULL,
    [GroupContactUPRN]              NVARCHAR (255)  NULL,
    [GroupPostSiteName]             NVARCHAR (255)  NULL,
    [GroupContactSiteName]          NVARCHAR (255)  NULL,
    [corporateContact]              NVARCHAR (255)  NULL,
    [UKPRN]                         NUMERIC (19)    NULL,
    CONSTRAINT [PK__EstablishmentGro__54F67D98] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentGroup_AUD] (
    [id]                            NUMERIC (19)    NOT NULL,
    [ver_rev]                       NUMERIC (19)    NOT NULL,
    [REVTYPE]                       TINYINT         NULL,
    [closedDate]                    DATETIME        NULL,
    [email]                         NVARCHAR (255)  NULL,
    [GroupAddress3]                 NVARCHAR (100)  NULL,
    [GroupLocality]                 NVARCHAR (255)  NULL,
    [GroupPostcode]                 NVARCHAR (255)  NULL,
    [GroupStreet]                   NVARCHAR (255)  NULL,
    [GroupTown]                     NVARCHAR (255)  NULL,
    [GroupContactFaxNumber]         NVARCHAR (255)  NULL,
    [GroupContactFaxStd]            NVARCHAR (255)  NULL,
    [GroupContactFirstName]         NVARCHAR (255)  NULL,
    [GroupContactHonours]           NVARCHAR (255)  NULL,
    [GroupContactLastName]          NVARCHAR (255)  NULL,
    [GroupContactPreferredJobTitle] NVARCHAR (255)  NULL,
    [GroupContactTelephoneNumber]   NVARCHAR (255)  NULL,
    [GroupContactTelephoneStd]      NVARCHAR (255)  NULL,
    [GroupContactAddress3]          NVARCHAR (100)  NULL,
    [GroupContactLocality]          NVARCHAR (255)  NULL,
    [GroupContactPostcode]          NVARCHAR (255)  NULL,
    [GroupContactStreet]            NVARCHAR (255)  NULL,
    [GroupContactTown]              NVARCHAR (255)  NULL,
    [name]                          NVARCHAR (255)  NULL,
    [GroupFaxNumber]                NVARCHAR (255)  NULL,
    [GroupFaxStd]                   NVARCHAR (255)  NULL,
    [HeadFirstName]                 NVARCHAR (255)  NULL,
    [HeadHonours]                   NVARCHAR (255)  NULL,
    [HeadLastName]                  NVARCHAR (255)  NULL,
    [HeadPreferredJobTitle]         NVARCHAR (255)  NULL,
    [GroupTelephoneNumber]          NVARCHAR (255)  NULL,
    [GroupTelephoneStd]             NVARCHAR (255)  NULL,
    [openDate]                      DATETIME        NULL,
    [GroupCounty_code]              NVARCHAR (32)   NULL,
    [GroupContactTitle_code]        NVARCHAR (32)   NULL,
    [GroupContactCounty_code]       NVARCHAR (32)   NULL,
    [groupContactType_code]         NVARCHAR (32)   NULL,
    [HeadTitle_code]                NVARCHAR (32)   NULL,
    [type_code]                     NVARCHAR (32)   NULL,
    [createdInError]                TINYINT         NULL,
    [sharedHeadTeacher]             TINYINT         NULL,
    [companiesHouseNumber]          NVARCHAR (255)  NULL,
    [groupId]                       NVARCHAR (255)  NULL,
    [groupContactEmail]             NVARCHAR (255)  NULL,
    [localAuthority_code]           NVARCHAR (32)   NULL,
    [saOrgId]                       NUMERIC (19)    NULL,
    [governanceLastCheck]           DATETIME        NULL,
    [delegateAuthDetails]           NVARCHAR (1000) NULL,
    [GroupCountry_code]             NVARCHAR (32)   NULL,
    [GroupContactCountry_code]      NVARCHAR (32)   NULL,
    [GroupPostUPRN]                 NVARCHAR (255)  NULL,
    [GroupContactUPRN]              NVARCHAR (255)  NULL,
    [GroupPostSiteName]             NVARCHAR (255)  NULL,
    [GroupContactSiteName]          NVARCHAR (255)  NULL,
    [corporateContact]              NVARCHAR (255)  NULL,
    [UKPRN]                         NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentGroupType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentGroupType_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentLink] (
    [id]              NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [establishedDate] DATETIME       NULL,
    [oldFieldName]    NVARCHAR (255) NULL,
    [linkURN]         NUMERIC (19)   NOT NULL,
    [type_code]       NVARCHAR (32)  NOT NULL,
    [URN]             NUMERIC (19)   NULL,
    [systemLinkDate]  DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentLink_AUD] (
    [id]              NUMERIC (19)   NOT NULL,
    [ver_rev]         NUMERIC (19)   NOT NULL,
    [REVTYPE]         TINYINT        NULL,
    [establishedDate] DATETIME       NULL,
    [oldFieldName]    NVARCHAR (255) NULL,
    [systemLinkDate]  DATETIME       NULL,
    [linkURN]         NUMERIC (19)   NULL,
    [type_code]       NVARCHAR (32)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentPartnership] (
    [URN]         NUMERIC (19)   NOT NULL,
    [partnership] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK__EstablishmentPar__5F492382] PRIMARY KEY CLUSTERED ([URN] ASC, [partnership] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentPartnership_AUD] (
    [ver_rev]     NUMERIC (19)   NOT NULL,
    [URN]         NUMERIC (19)   NOT NULL,
    [partnership] NVARCHAR (255) NOT NULL,
    [REVTYPE]     TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [partnership] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentProprietors] (
    [id]              NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [urn]             NUMERIC (19)   NOT NULL,
    [record_number]   NUMERIC (19)   NOT NULL,
    [name]            NVARCHAR (255) NULL,
    [street]          NVARCHAR (100) NULL,
    [locality]        NVARCHAR (100) NULL,
    [address3]        NVARCHAR (255) NULL,
    [town]            NVARCHAR (100) NULL,
    [county_code]     NVARCHAR (32)  NULL,
    [postcode]        NVARCHAR (8)   NULL,
    [telephoneNumber] NVARCHAR (100) NULL,
    [email]           NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([urn] ASC, [record_number] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentProprietors_AUD] (
    [id]              NUMERIC (19)   NOT NULL,
    [ver_rev]         NUMERIC (19)   NOT NULL,
    [REVTYPE]         TINYINT        NULL,
    [urn]             NUMERIC (19)   NULL,
    [record_number]   NUMERIC (19)   NULL,
    [name]            NVARCHAR (255) NULL,
    [street]          NVARCHAR (100) NULL,
    [locality]        NVARCHAR (100) NULL,
    [address3]        NVARCHAR (255) NULL,
    [town]            NVARCHAR (100) NULL,
    [county_code]     NVARCHAR (32)  NULL,
    [postcode]        NVARCHAR (8)   NULL,
    [telephoneNumber] NVARCHAR (100) NULL,
    [email]           NVARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentSixthFormSchool] (
    [URN]             NUMERIC (19)   NOT NULL,
    [sixthFormSchool] NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK__EstablishmentSix__5C6CB6D7] PRIMARY KEY CLUSTERED ([URN] ASC, [sixthFormSchool] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentSixthFormSchool_AUD] (
    [ver_rev]         NUMERIC (19)   NOT NULL,
    [URN]             NUMERIC (19)   NOT NULL,
    [sixthFormSchool] NVARCHAR (255) NOT NULL,
    [REVTYPE]         TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [sixthFormSchool] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentStatus] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EstablishmentSta__20C1E124] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentStatus_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToFederation] (
    [URN]            NUMERIC (19)  NOT NULL,
    [federationCode] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__EstablishmentToF__6225902D] PRIMARY KEY CLUSTERED ([URN] ASC, [federationCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToFederation_AUD] (
    [ver_rev]        NUMERIC (19)  NOT NULL,
    [URN]            NUMERIC (19)  NOT NULL,
    [federationCode] NVARCHAR (32) NOT NULL,
    [REVTYPE]        TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [federationCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen1] (
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__EstablishmentToS__1B9317B3] PRIMARY KEY CLUSTERED ([URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen1_AUD] (
    [ver_rev] NUMERIC (19)  NOT NULL,
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    [REVTYPE] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen2] (
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__EstablishmentToS__1F63A897] PRIMARY KEY CLUSTERED ([URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen2_AUD] (
    [ver_rev] NUMERIC (19)  NOT NULL,
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    [REVTYPE] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen3] (
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__EstablishmentToS__2334397B] PRIMARY KEY CLUSTERED ([URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen3_AUD] (
    [ver_rev] NUMERIC (19)  NOT NULL,
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    [REVTYPE] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen4] (
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentToSen4_AUD] (
    [ver_rev] NUMERIC (19)  NOT NULL,
    [URN]     NUMERIC (19)  NOT NULL,
    [senCode] NVARCHAR (32) NOT NULL,
    [REVTYPE] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [senCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentType] (
    [code]       NVARCHAR (32)  NOT NULL,
    [group_code] NVARCHAR (32)  NOT NULL,
    [name]       NVARCHAR (255) NOT NULL,
    [archived]   TINYINT        NULL,
    [id]         INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EstablishmentTyp__1B0907CE] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentType_AUD] (
    [code]       NVARCHAR (32)  NOT NULL,
    [ver_rev]    NUMERIC (19)   NOT NULL,
    [REVTYPE]    TINYINT        NULL,
    [archived]   TINYINT        NULL,
    [name]       NVARCHAR (255) NULL,
    [group_code] NVARCHAR (32)  NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EstablishmentTypeGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__EstablishmentTyp__1920BF5C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExcellenceInCities] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ExcellenceInCiti__286302EC] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExcellenceInCitiesActionZone] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ExcellenceInCiti__00551192] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExcellenceInCitiesCityLearningCentre] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ExcellenceInCiti__0F975522] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExcellenceInCitiesGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ExcellenceInCiti__300424B4] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExtractFieldWithParams] (
    [id]                  NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [showLookupName]      TINYINT       NOT NULL,
    [field_shortName]     NVARCHAR (32) NULL,
    [scheduledExtract_id] NUMERIC (19)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExtractionLog] (
    [id]            NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [date]          DATETIME       NULL,
    [fileName]      NVARCHAR (255) NULL,
    [free]          TINYINT        NULL,
    [type]          NVARCHAR (255) NULL,
    [user_username] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ExtractsPostRunVerifierAudit] (
    [id]                 NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [callbackId]         NUMERIC (19)    NOT NULL,
    [scheduledExtractId] NUMERIC (19)    NULL,
    [extractName]        NVARCHAR (512)  NULL,
    [retry_attempt]      INT             NOT NULL,
    [retryed_at]         DATETIME        NOT NULL,
    [finishedAt]         DATETIME        NULL,
    [status]             NVARCHAR (64)   NOT NULL,
    [failureReason]      NVARCHAR (4000) NULL,
    [correlationId]      NVARCHAR (64)   NULL,
    CONSTRAINT [PK_ExtractsPostRunVerifierAudit] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[EYFSExemption] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FAQ] (
    [id]                   NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [body]                 NVARCHAR (MAX) NOT NULL,
    [createdAt]            DATETIME       NOT NULL,
    [published]            TINYINT        NOT NULL,
    [title]                NVARCHAR (512) NOT NULL,
    [updatedAt]            DATETIME       NOT NULL,
    [version]              INT            NOT NULL,
    [weight]               INT            NULL,
    [createdBy_username]   NVARCHAR (255) NOT NULL,
    [signOffUser_username] NVARCHAR (255) NOT NULL,
    [subject_code]         NVARCHAR (32)  NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FAQ_FAQGroup] (
    [FAQ_id]      NUMERIC (19)  NOT NULL,
    [groups_code] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__FAQ_FAQGroup__38B96646] PRIMARY KEY CLUSTERED ([FAQ_id] ASC, [groups_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FAQGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FAQGroup__34E8D562] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FAQSubject] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FAQSubject__36D11DD4] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Favourite] (
    [type]           NVARCHAR (31)  NOT NULL,
    [id]             NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [lastAccessDate] DATETIME       NULL,
    [user_username]  NVARCHAR (255) NOT NULL,
    [extract_id]     NUMERIC (19)   NULL,
    [filter_id]      NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FeatureFlags] (
    [flagName]    VARCHAR (100) NOT NULL,
    [description] VARCHAR (255) NULL,
    [enabled]     BIT           NOT NULL,
    [updatedAt]   DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_FeatureFlags] PRIMARY KEY CLUSTERED ([flagName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Federation] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Federation__4CA06362] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FederationFlag] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FederationType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Feedback] (
    [id]    NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [email] NVARCHAR (255)  NULL,
    [name]  NVARCHAR (255)  NULL,
    [text]  NVARCHAR (4000) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FieldCriterion] (
    [criterionType]       NVARCHAR (31)  NOT NULL,
    [id]                  NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [condition]           NVARCHAR (255) NULL,
    [rawValue]            IMAGE          NULL,
    [field_shortName]     NVARCHAR (32)  NOT NULL,
    [betweenFrom]         DATETIME       NULL,
    [betweenTo]           DATETIME       NULL,
    [subset]              NVARCHAR (32)  NULL,
    [radiusSearchData_id] NUMERIC (19)   NULL,
    [includePrevious]     TINYINT        NULL,
    [filter_id]           NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);


GO

CREATE TABLE [dbo].[FieldGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FieldGroup__5629CD9C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FieldPosition] (
    [id]                INT           IDENTITY (1, 1) NOT NULL,
    [shortName]         NVARCHAR (32) NULL,
    [tab_code]          NVARCHAR (32) NULL,
    [orderBy]           INT           NULL,
    [showInRightColumn] TINYINT       NULL,
    CONSTRAINT [PK__FieldPosition__6F7F8B4B] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FieldPosition_AUD] (
    [id]                NUMERIC (19)  NOT NULL,
    [ver_rev]           NUMERIC (19)  NOT NULL,
    [REVTYPE]           TINYINT       NULL,
    [orderBy]           INT           NULL,
    [showInRightColumn] TINYINT       NULL,
    [tab_code]          NVARCHAR (32) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FieldStatistics] (
    [shortName]       NVARCHAR (32) NOT NULL,
    [numberEmpty]     INT           NULL,
    [numberUpdated]   INT           NOT NULL,
    [totalNumber]     INT           NOT NULL,
    [updateFrequency] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__FieldStatistics__119F9925] PRIMARY KEY CLUSTERED ([shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FieldTab] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FieldTab__5812160E] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FreshStart] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FreshStart__6754599E] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FullTimeProvision] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FullTimeProvisio__693CA210] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FurtherEducationType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__FurtherEducation__1CF15040] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[FyiStakeholder] (
    [type]           NVARCHAR (32) NOT NULL,
    [field]          NVARCHAR (32) NOT NULL,
    [userGroup_code] NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__FyiStakeholder__047AA831] PRIMARY KEY CLUSTERED ([field] ASC, [type] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Gender] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Gender__0DAF0CB0] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GeoData] (
    [postcode]                       NVARCHAR (255) NOT NULL,
    [administrativeWard_code]        NVARCHAR (32)  NULL,
    [casWard_code]                   NVARCHAR (32)  NULL,
    [lsoa_code]                      NVARCHAR (32)  NULL,
    [msoa_code]                      NVARCHAR (32)  NULL,
    [parliamentaryConstituency_code] NVARCHAR (32)  NULL,
    [districtAdministrative_code]    NVARCHAR (32)  NULL,
    [urbanRural_code]                NVARCHAR (32)  NULL,
    CONSTRAINT [PK__GeoData__6E565CE8] PRIMARY KEY CLUSTERED ([postcode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GeoData_AUD] (
    [postcode]                       NVARCHAR (255) NOT NULL,
    [ver_rev]                        NUMERIC (19)   NOT NULL,
    [REVTYPE]                        TINYINT        NULL,
    [administrativeWard_code]        NVARCHAR (32)  NULL,
    [casWard_code]                   NVARCHAR (32)  NULL,
    [districtAdministrative_code]    NVARCHAR (32)  NULL,
    [lsoa_code]                      NVARCHAR (32)  NULL,
    [msoa_code]                      NVARCHAR (32)  NULL,
    [parliamentaryConstituency_code] NVARCHAR (32)  NULL,
    [urbanRural_code]                NVARCHAR (32)  NULL,
    PRIMARY KEY CLUSTERED ([postcode] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GeoLocationAverages] (
    [id]                      NUMERIC (19) IDENTITY (1, 1) NOT NULL,
    [locationName]            VARCHAR (40) NOT NULL,
    [easting]                 INT          NOT NULL,
    [northing]                INT          NOT NULL,
    [county]                  VARCHAR (32) NULL,
    [town]                    VARCHAR (32) NULL,
    [dependentLocality]       VARCHAR (40) NULL,
    [doubleDependentLocality] VARCHAR (40) NULL,
    CONSTRAINT [PK_LocationAverage] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GeoPostcodeAverages] (
    [postcode] VARCHAR (8) NOT NULL,
    [easting]  INT         NOT NULL,
    [northing] INT         NOT NULL,
    CONSTRAINT [PK_PostcodeAverage] PRIMARY KEY CLUSTERED ([postcode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GlossaryItem] (
    [id]            NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [content]       NVARCHAR (MAX) NOT NULL,
    [lastModified]  DATETIME       NULL,
    [title]         NVARCHAR (256) NOT NULL,
    [user_username] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([title] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GorLookup] (
    [la]      NVARCHAR (32) NOT NULL,
    [gor]     NVARCHAR (32) NULL,
    [gorname] NVARCHAR (64) NULL
);


GO

CREATE TABLE [dbo].[Governance] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GovernmentOfficeRegion] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__GovernmentOffice__31EC6D26] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GovernorsFlag] (
    [code]               NVARCHAR (32)  NOT NULL,
    [archived]           TINYINT        NULL,
    [name]               NVARCHAR (255) NOT NULL,
    [orderBy]            INT            NULL,
    [isCanHaveNonShared] TINYINT        NULL,
    [isCanHaveShared]    TINYINT        NULL,
    [id]                 INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GovUserRequest] (
    [id]                NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [created]           DATETIME       NULL,
    [departmentName]    NVARCHAR (255) NULL,
    [email]             NVARCHAR (255) NULL,
    [fax]               NVARCHAR (255) NULL,
    [firstName]         NVARCHAR (255) NULL,
    [lastName]          NVARCHAR (255) NULL,
    [login]             NVARCHAR (255) NULL,
    [phone]             NVARCHAR (255) NULL,
    [slaUaStatus]       NVARCHAR (255) NULL,
    [status]            NVARCHAR (255) NULL,
    [group_code]        NVARCHAR (32)  NULL,
    [company]           NVARCHAR (255) NOT NULL,
    [positionInCompany] NVARCHAR (255) NOT NULL,
    [typeOfAccess]      NVARCHAR (255) NULL,
    [lengthOfAccess]    NUMERIC (19)   NULL,
    [superUser]         TINYINT        NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupChangeRequest] (
    [requestType]                   NVARCHAR (31)   NOT NULL,
    [id]                            NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [createdDate]                   DATETIME        NULL,
    [effectiveDate]                 DATETIME        NULL,
    [lastChangeDate]                DATETIME        NOT NULL,
    [rejectReason]                  NVARCHAR (2048) NULL,
    [status]                        NVARCHAR (255)  NOT NULL,
    [version]                       INT             NULL,
    [dateNewValue]                  DATETIME        NULL,
    [dateOldValue]                  DATETIME        NULL,
    [lookupNewValue]                NVARCHAR (255)  NULL,
    [lookupOldValue]                NVARCHAR (255)  NULL,
    [groupName]                     NVARCHAR (255)  NULL,
    [groupOpenDate]                 DATETIME        NULL,
    [linkType]                      NVARCHAR (255)  NULL,
    [sharedHeadTeacher]             TINYINT         NULL,
    [numericNewValue]               NUMERIC (19)    NULL,
    [numericOldValue]               NUMERIC (19)    NULL,
    [stringNewValue]                NVARCHAR (1024) NULL,
    [stringOldValue]                NVARCHAR (1024) NULL,
    [approvedOrRejectedBy_username] NVARCHAR (255)  NULL,
    [proposedBy_username]           NVARCHAR (255)  NOT NULL,
    [field_shortName]               NVARCHAR (32)   NULL,
    [group_id]                      NUMERIC (19)    NULL,
    [groupType]                     NVARCHAR (32)   NULL,
    [urn]                           NUMERIC (19)    NULL,
    [hasJointCommittee_code]        NVARCHAR (32)   NULL,
    [groupLink]                     NUMERIC (19)    NULL,
    [booleanNewValue]               TINYINT         NULL,
    [booleanOldValue]               TINYINT         NULL,
    [ccLinkType]                    NVARCHAR (255)  NULL,
    [groupClosedDate]               DATETIME2 (7)   NULL,
    [companiesHouseNumber]          NVARCHAR (255)  NULL,
    [groupAddress3]                 NVARCHAR (255)  NULL,
    [groupContactEmail]             NVARCHAR (255)  NULL,
    [groupId]                       NVARCHAR (255)  NULL,
    [groupLocality]                 NVARCHAR (255)  NULL,
    [groupPostcode]                 NVARCHAR (255)  NULL,
    [groupStreet]                   NVARCHAR (255)  NULL,
    [groupTown]                     NVARCHAR (255)  NULL,
    [groupCounty_code]              NVARCHAR (32)   NULL,
    [delegationInformation]         NVARCHAR (255)  NULL,
    [headFirstName]                 NVARCHAR (255)  NULL,
    [headLastName]                  NVARCHAR (255)  NULL,
    [headTitleId_code]              NVARCHAR (32)   NULL,
    [corporateContact]              NVARCHAR (255)  NULL,
    [groupRelationsLink]            NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupChangeRequest_AUD] (
    [requestType]                   NVARCHAR (31)   NOT NULL,
    [id]                            NUMERIC (19)    NOT NULL,
    [ver_rev]                       NUMERIC (19)    NOT NULL,
    [REVTYPE]                       TINYINT         NULL,
    [createdDate]                   DATETIME        NULL,
    [effectiveDate]                 DATETIME        NULL,
    [lastChangeDate]                DATETIME        NULL,
    [rejectReason]                  NVARCHAR (2048) NULL,
    [status]                        NVARCHAR (255)  NULL,
    [approvedOrRejectedBy_username] NVARCHAR (255)  NULL,
    [proposedBy_username]           NVARCHAR (255)  NULL,
    [linkType]                      NVARCHAR (255)  NULL,
    [sharedHeadTeacher]             TINYINT         NULL,
    [urn]                           NUMERIC (19)    NULL,
    [group_id]                      NUMERIC (19)    NULL,
    [hasJointCommittee_code]        NVARCHAR (32)   NULL,
    [groupName]                     NVARCHAR (255)  NULL,
    [groupOpenDate]                 DATETIME        NULL,
    [groupType]                     NVARCHAR (32)   NULL,
    [field_shortName]               NVARCHAR (32)   NULL,
    [stringNewValue]                NVARCHAR (1024) NULL,
    [stringOldValue]                NVARCHAR (1024) NULL,
    [lookupNewValue]                NVARCHAR (255)  NULL,
    [lookupOldValue]                NVARCHAR (255)  NULL,
    [numericNewValue]               NUMERIC (19)    NULL,
    [numericOldValue]               NUMERIC (19)    NULL,
    [dateNewValue]                  DATETIME        NULL,
    [dateOldValue]                  DATETIME        NULL,
    [groupLink]                     NUMERIC (19)    NULL,
    [booleanNewValue]               TINYINT         NULL,
    [booleanOldValue]               TINYINT         NULL,
    [ccLinkType]                    NVARCHAR (255)  NULL,
    [groupClosedDate]               DATETIME2 (7)   NULL,
    [companiesHouseNumber]          NVARCHAR (255)  NULL,
    [groupAddress3]                 NVARCHAR (255)  NULL,
    [groupContactEmail]             NVARCHAR (255)  NULL,
    [groupId]                       NVARCHAR (255)  NULL,
    [groupLocality]                 NVARCHAR (255)  NULL,
    [groupPostcode]                 NVARCHAR (255)  NULL,
    [groupStreet]                   NVARCHAR (255)  NULL,
    [groupTown]                     NVARCHAR (255)  NULL,
    [groupCounty_code]              NVARCHAR (32)   NULL,
    [delegationInformation]         NVARCHAR (255)  NULL,
    [headFirstName]                 NVARCHAR (255)  NULL,
    [headLastName]                  NVARCHAR (255)  NULL,
    [headTitleId_code]              NVARCHAR (32)   NULL,
    [corporateContact]              NVARCHAR (255)  NULL,
    [groupRelationsLink]            NUMERIC (19)    NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupExtractLog] (
    [id]           NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [isWebService] TINYINT        NULL,
    [systemUser]   NVARCHAR (255) NULL,
    [timestamp]    DATETIME       NULL,
    [type]         NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupField] (
    [shortName]                 NVARCHAR (32)  NOT NULL,
    [displayName]               NVARCHAR (255) NOT NULL,
    [fieldType]                 NVARCHAR (32)  NOT NULL,
    [path]                      NVARCHAR (64)  NULL,
    [lookupClass]               NVARCHAR (64)  NULL,
    [readonly]                  TINYINT        NULL,
    [shortDisplayName]          NVARCHAR (64)  NULL,
    [childrenCentreDisplayName] NVARCHAR (64)  NULL,
    [archived]                  TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupField_AUD] (
    [fieldType]                 NVARCHAR (32)  NOT NULL,
    [shortName]                 NVARCHAR (32)  NOT NULL,
    [ver_rev]                   NUMERIC (19)   NOT NULL,
    [REVTYPE]                   TINYINT        NULL,
    [displayName]               NVARCHAR (255) NULL,
    [path]                      NVARCHAR (64)  NULL,
    [readonly]                  TINYINT        NULL,
    [shortDisplayName]          NVARCHAR (64)  NULL,
    [childrenCentreDisplayName] NVARCHAR (64)  NULL,
    [archived]                  TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupField_GroupFieldPosition_AUD] (
    [ver_rev]   NUMERIC (19)  NOT NULL,
    [shortName] NVARCHAR (32) NOT NULL,
    [id]        NUMERIC (19)  NOT NULL,
    [REVTYPE]   TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [shortName] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldPermission] (
    [groupCode]  NVARCHAR (32) NOT NULL,
    [shortName]  NVARCHAR (32) NOT NULL,
    [permission] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([groupCode] ASC, [shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldPermission_AUD] (
    [ver_rev]    NUMERIC (19)   NOT NULL,
    [groupCode]  NVARCHAR (32)  NOT NULL,
    [permission] NVARCHAR (255) NOT NULL,
    [shortName]  NVARCHAR (32)  NOT NULL,
    [REVTYPE]    TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [groupCode] ASC, [permission] ASC, [shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldPosition] (
    [id]                NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [tab_code]          NVARCHAR (32) NOT NULL,
    [shortName]         NVARCHAR (32) NOT NULL,
    [orderBy]           INT           NOT NULL,
    [showInRightColumn] TINYINT       NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldPosition_AUD] (
    [id]                NUMERIC (19)  NOT NULL,
    [ver_rev]           NUMERIC (19)  NOT NULL,
    [REVTYPE]           TINYINT       NULL,
    [orderBy]           INT           NULL,
    [showInRightColumn] TINYINT       NULL,
    [tab_code]          NVARCHAR (32) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldTab] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldTab_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    [orderBy]  INT            NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupFieldValidation] (
    [id]                 NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [shortName]          NVARCHAR (32) NOT NULL,
    [validationRuleCode] VARCHAR (32)  NOT NULL,
    [validationParam]    VARCHAR (512) NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([shortName] ASC, [validationRuleCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupLink] (
    [id]            NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [archived]      TINYINT        NOT NULL,
    [linkType]      NVARCHAR (255) NULL,
    [version]       INT            NULL,
    [urn]           NUMERIC (19)   NULL,
    [group_id]      NUMERIC (19)   NULL,
    [effectiveDate] DATETIME       NULL,
    [ccLinkType]    NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupLink_aud] (
    [ver_rev]                NUMERIC (19)   NOT NULL,
    [urn]                    NUMERIC (19)   NOT NULL,
    [group_id]               NUMERIC (19)   NOT NULL,
    [REVTYPE]                TINYINT        NULL,
    [id]                     NUMERIC (19)   NOT NULL,
    [archived]               TINYINT        NULL,
    [linkType]               NVARCHAR (255) NULL,
    [sharedHeadTeacher]      TINYINT        NULL,
    [hasJointCommittee_code] NVARCHAR (32)  NULL,
    [effectiveDate]          DATETIME       NULL,
    [ccLinkType]             NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupLinkType] (
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupRelationsLink] (
    [id]              NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [establishedDate] DATETIME      NULL,
    [systemLinkDate]  DATETIME      NULL,
    [linked_group]    NUMERIC (19)  NOT NULL,
    [type_code]       NVARCHAR (32) NOT NULL,
    [linking_group]   NUMERIC (19)  NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GroupRelationsLink_AUD] (
    [id]              NUMERIC (19)  NOT NULL,
    [ver_rev]         NUMERIC (19)  NOT NULL,
    [REVTYPE]         TINYINT       NULL,
    [establishedDate] DATETIME      NULL,
    [systemLinkDate]  DATETIME      NULL,
    [linked_group]    NUMERIC (19)  NULL,
    [type_code]       NVARCHAR (32) NULL,
    [linking_group]   NUMERIC (19)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[GSSLACode] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[HighPerformanceOption] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__HighPerformanceO__398D8EEE] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[HighPerformanceOption_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[InboxMessage] (
    [type]              NVARCHAR (31)  NOT NULL,
    [id]                NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [body]              NVARCHAR (MAX) NULL,
    [created]           DATETIME       NULL,
    [subject]           NVARCHAR (255) NULL,
    [imported]          TINYINT        NULL,
    [messageId]         NVARCHAR (255) NULL,
    [loggedBy_username] NVARCHAR (255) NULL,
    [timestamp]         ROWVERSION     NOT NULL,
    [hasAttachments]    TINYINT        NULL,
    CONSTRAINT [PK_InboxMessage] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[IndependentSchools] (
    [URN]                        NUMERIC (19)    NOT NULL,
    [Notes]                      NVARCHAR (4000) NULL,
    [DateOfLastOfstedVisit]      DATETIME        NULL,
    [DateOfLastISIVisit]         DATETIME        NULL,
    [DateOfLastWelfareVisit]     DATETIME        NULL,
    [DateOfLastFPVisit]          DATETIME        NULL,
    [DateOfLastSISVisit]         DATETIME        NULL,
    [DateOfLastBridgeVisit]      DATETIME        NULL,
    [NextGeneralActionRequired]  DATETIME        NULL,
    [ActionRequiredWEL]          DATETIME        NULL,
    [ActionRequiredFP]           DATETIME        NULL,
    [CensusDate]                 DATETIME        NULL,
    [TotalPupilsFullTime]        INT             NULL,
    [TotalPupilsPartTime]        INT             NULL,
    [CompulsorySchoolAge]        INT             NULL,
    [SENstat]                    INT             NULL,
    [SENnoStat]                  INT             NULL,
    [TotalNumberFTStaff]         INT             NULL,
    [TotalNumberPTStaff]         INT             NULL,
    [TotalWeeklyHoursPTStaff]    INT             NULL,
    [CharityName]                NVARCHAR (255)  NULL,
    [CharityNumber]              INT             NULL,
    [LowestDayFee]               INT             NULL,
    [LowestBoardFee]             INT             NULL,
    [HighestDayFee]              INT             NULL,
    [TotalBoyBoarders]           INT             NULL,
    [TotalGirlBoarders]          INT             NULL,
    [PTBoys2andUnder]            INT             NULL,
    [PTBoys3]                    INT             NULL,
    [PTBoys4a]                   INT             NULL,
    [PTBoys4b]                   INT             NULL,
    [PTBoys4c]                   INT             NULL,
    [PTGirls2andUnder]           INT             NULL,
    [PTGirls3]                   INT             NULL,
    [PTGirls4a]                  INT             NULL,
    [PTGirls4b]                  INT             NULL,
    [PTGirls4c]                  INT             NULL,
    [TotalPupilsPublicCare]      INT             NULL,
    [type_code]                  NVARCHAR (32)   NOT NULL,
    [accomChange_code]           NVARCHAR (32)   NOT NULL,
    [inspectorate_code]          NVARCHAR (32)   NOT NULL,
    [boardingEstablishment_code] NVARCHAR (32)   NOT NULL,
    [HighestBoardFee]            INT             NULL,
    [association_code]           NVARCHAR (32)   NOT NULL,
    [NextOfstedVisit]            DATETIME        NULL,
    [PupilToTeacherRatio]        NVARCHAR (255)  NULL,
    [ChairmanAddress3]           NVARCHAR (100)  NULL,
    [ChairmanLocality]           NVARCHAR (100)  NULL,
    [ChairmanPostcode]           NVARCHAR (8)    NULL,
    [ChairmanStreet]             NVARCHAR (100)  NULL,
    [ChairmanTown]               NVARCHAR (100)  NULL,
    [ChairmanFaxNumber]          NVARCHAR (40)   NULL,
    [ChairmanFaxStd]             NVARCHAR (20)   NULL,
    [ChairmanFirstName]          NVARCHAR (100)  NULL,
    [ChairmanHonours]            NVARCHAR (20)   NULL,
    [ChairmanLastName]           NVARCHAR (100)  NULL,
    [ChairmanPreferredJobTitle]  NVARCHAR (25)   NULL,
    [ChairmanTelephoneNumber]    NVARCHAR (40)   NULL,
    [ChairmanTelephoneStd]       NVARCHAR (20)   NULL,
    [ChairmanEmail]              NVARCHAR (100)  NULL,
    [PropsAddress3]              NVARCHAR (100)  NULL,
    [PropsLocality]              NVARCHAR (100)  NULL,
    [PropsPostcode]              NVARCHAR (8)    NULL,
    [PropsStreet]                NVARCHAR (100)  NULL,
    [PropsTown]                  NVARCHAR (100)  NULL,
    [PropsFaxNumber]             NVARCHAR (40)   NULL,
    [PropsFaxStd]                NVARCHAR (20)   NULL,
    [PropsFirstName]             NVARCHAR (100)  NULL,
    [PropsHonours]               NVARCHAR (20)   NULL,
    [PropsLastName]              NVARCHAR (100)  NULL,
    [PropsPreferredJobTitle]     NVARCHAR (25)   NULL,
    [PropsTelephoneNumber]       NVARCHAR (40)   NULL,
    [PropsTelephoneStd]          NVARCHAR (20)   NULL,
    [PropsEmail]                 NVARCHAR (100)  NULL,
    [ChairmanCounty_code]        NVARCHAR (32)   NOT NULL,
    [ChairmanTitle_code]         NVARCHAR (32)   NOT NULL,
    [PropsCounty_code]           NVARCHAR (32)   NOT NULL,
    [PropsTitle_code]            NVARCHAR (32)   NOT NULL,
    [timestamp]                  ROWVERSION      NOT NULL,
    [ChairmanCountry_code]       NVARCHAR (32)   NOT NULL,
    [PropsCountry_code]          NVARCHAR (32)   NOT NULL,
    [ChairmanUPRN]               NVARCHAR (255)  NULL,
    [PropsUPRN]                  NVARCHAR (255)  NULL,
    [ChairmanSiteName]           NVARCHAR (255)  NULL,
    [PropsSiteName]              NVARCHAR (255)  NULL,
    [IEBTAssociations]           NVARCHAR (1000) NULL,
    [proprietorType_code]        NVARCHAR (32)   NULL,
    [TotalFTTeachersTutors]      INT             NULL,
    [TotalPTTeachersTutors]      INT             NULL,
    [registrationSuspended_code] VARCHAR (32)    NOT NULL,
    PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[IndependentSchools_AUD] (
    [URN]                        NUMERIC (19)    NOT NULL,
    [ver_rev]                    NUMERIC (19)    NOT NULL,
    [REVTYPE]                    TINYINT         NULL,
    [ActionRequiredFP]           DATETIME        NULL,
    [ActionRequiredWEL]          DATETIME        NULL,
    [CensusDate]                 DATETIME        NULL,
    [ChairmanAddress3]           NVARCHAR (100)  NULL,
    [ChairmanLocality]           NVARCHAR (255)  NULL,
    [ChairmanPostcode]           NVARCHAR (255)  NULL,
    [ChairmanStreet]             NVARCHAR (255)  NULL,
    [ChairmanTown]               NVARCHAR (255)  NULL,
    [ChairmanFaxNumber]          NVARCHAR (255)  NULL,
    [ChairmanFaxStd]             NVARCHAR (255)  NULL,
    [ChairmanFirstName]          NVARCHAR (255)  NULL,
    [ChairmanHonours]            NVARCHAR (255)  NULL,
    [ChairmanLastName]           NVARCHAR (255)  NULL,
    [ChairmanPreferredJobTitle]  NVARCHAR (255)  NULL,
    [ChairmanTelephoneNumber]    NVARCHAR (255)  NULL,
    [ChairmanTelephoneStd]       NVARCHAR (255)  NULL,
    [ChairmanEmail]              NVARCHAR (100)  NULL,
    [CharityName]                NVARCHAR (255)  NULL,
    [CharityNumber]              INT             NULL,
    [CompulsorySchoolAge]        INT             NULL,
    [DateOfLastBridgeVisit]      DATETIME        NULL,
    [DateOfLastFPVisit]          DATETIME        NULL,
    [DateOfLastISIVisit]         DATETIME        NULL,
    [DateOfLastOfstedVisit]      DATETIME        NULL,
    [DateOfLastSISVisit]         DATETIME        NULL,
    [DateOfLastWelfareVisit]     DATETIME        NULL,
    [HighestBoardFee]            INT             NULL,
    [HighestDayFee]              INT             NULL,
    [LowestBoardFee]             INT             NULL,
    [LowestDayFee]               INT             NULL,
    [NextGeneralActionRequired]  DATETIME        NULL,
    [NextOfstedVisit]            DATETIME        NULL,
    [Notes]                      NVARCHAR (4000) NULL,
    [PropsAddress3]              NVARCHAR (100)  NULL,
    [PropsLocality]              NVARCHAR (255)  NULL,
    [PropsPostcode]              NVARCHAR (255)  NULL,
    [PropsStreet]                NVARCHAR (255)  NULL,
    [PropsTown]                  NVARCHAR (255)  NULL,
    [PropsFaxNumber]             NVARCHAR (255)  NULL,
    [PropsFaxStd]                NVARCHAR (255)  NULL,
    [PropsFirstName]             NVARCHAR (255)  NULL,
    [PropsHonours]               NVARCHAR (255)  NULL,
    [PropsLastName]              NVARCHAR (255)  NULL,
    [PropsPreferredJobTitle]     NVARCHAR (255)  NULL,
    [PropsTelephoneNumber]       NVARCHAR (255)  NULL,
    [PropsTelephoneStd]          NVARCHAR (255)  NULL,
    [PropsEmail]                 NVARCHAR (100)  NULL,
    [PTBoys2andUnder]            INT             NULL,
    [PTBoys3]                    INT             NULL,
    [PTBoys4a]                   INT             NULL,
    [PTBoys4b]                   INT             NULL,
    [PTBoys4c]                   INT             NULL,
    [PTGirls2andUnder]           INT             NULL,
    [PTGirls3]                   INT             NULL,
    [PTGirls4a]                  INT             NULL,
    [PTGirls4b]                  INT             NULL,
    [PTGirls4c]                  INT             NULL,
    [PupilToTeacherRatio]        NVARCHAR (255)  NULL,
    [SENnoStat]                  INT             NULL,
    [SENstat]                    INT             NULL,
    [TotalBoyBoarders]           INT             NULL,
    [TotalGirlBoarders]          INT             NULL,
    [TotalNumberFTStaff]         INT             NULL,
    [TotalNumberPTStaff]         INT             NULL,
    [TotalPupilsFullTime]        INT             NULL,
    [TotalPupilsPartTime]        INT             NULL,
    [TotalPupilsPublicCare]      INT             NULL,
    [TotalWeeklyHoursPTStaff]    INT             NULL,
    [accomChange_code]           NVARCHAR (32)   NULL,
    [association_code]           NVARCHAR (32)   NULL,
    [boardingEstablishment_code] NVARCHAR (32)   NULL,
    [ChairmanCounty_code]        NVARCHAR (32)   NULL,
    [ChairmanTitle_code]         NVARCHAR (32)   NULL,
    [inspectorate_code]          NVARCHAR (32)   NULL,
    [PropsCounty_code]           NVARCHAR (32)   NULL,
    [PropsTitle_code]            NVARCHAR (32)   NULL,
    [type_code]                  NVARCHAR (32)   NULL,
    [ChairmanCountry_code]       NVARCHAR (32)   NULL,
    [PropsCountry_code]          NVARCHAR (32)   NULL,
    [ChairmanUPRN]               NVARCHAR (255)  NULL,
    [PropsUPRN]                  NVARCHAR (255)  NULL,
    [ChairmanSiteName]           NVARCHAR (255)  NULL,
    [PropsSiteName]              NVARCHAR (255)  NULL,
    [IEBTAssociations]           NVARCHAR (1000) NULL,
    [proprietorType_code]        NVARCHAR (32)   NULL,
    [TotalFTTeachersTutors]      INT             NULL,
    [TotalPTTeachersTutors]      INT             NULL,
    [registrationSuspended]      VARCHAR (100)   NULL,
    [registrationSuspended_code] VARCHAR (32)    NOT NULL,
    PRIMARY KEY CLUSTERED ([URN] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[IndependentSchools_EstablishmentProprietor_AUD] (
    [ver_rev] NUMERIC (19) NOT NULL,
    [URN]     NUMERIC (19) NOT NULL,
    [id]      NUMERIC (19) NOT NULL,
    [REVTYPE] TINYINT      NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [URN] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[IndependentSchoolType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__IndependentSchoo__412EB0B6] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Inspection] (
    [id]               NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [dateValue]        DATETIME       NULL,
    [inspectionDate]   DATETIME       NOT NULL,
    [numericValue]     NUMERIC (19)   NULL,
    [value]            NVARCHAR (255) NULL,
    [changeRequest_id] NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[InspectionArchive1] (
    [id]               NUMERIC (19)   NOT NULL,
    [dateValue]        DATETIME       NULL,
    [inspectionDate]   DATETIME       NOT NULL,
    [numericValue]     NUMERIC (19)   NULL,
    [value]            NVARCHAR (255) NULL,
    [changeRequest_id] NUMERIC (19)   NULL
);


GO

CREATE TABLE [dbo].[InspectionUpdates] (
    [id]        NUMERIC (18)    IDENTITY (1, 1) NOT NULL,
    [runDate]   DATETIME        NOT NULL,
    [errors]    NVARCHAR (4000) NULL,
    [hasFailed] TINYINT         NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Inspectorate] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Inspectorate__4316F928] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[InspectorateName] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[InvestorInPeople] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__InvestorInPeople__6B24EA82] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[InvoiceDocument] (
    [id]     NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [number] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ip_quotas] (
    [ip]    NVARCHAR (40) NOT NULL,
    [quota] INT           NOT NULL,
    PRIMARY KEY CLUSTERED ([ip] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ip_stats] (
    [ip]        NVARCHAR (40) NOT NULL,
    [timestamp] BIGINT        NOT NULL
);


GO

CREATE TABLE [dbo].[LaGorMapping] (
    [id]                  NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [localAuthority_code] NVARCHAR (32) NOT NULL,
    [region_code]         NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([localAuthority_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LaGssMapping] (
    [id]                  NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [gssLaCode_code]      NVARCHAR (32) NULL,
    [localAuthority_code] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LdapProfile] (
    [id]         NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [profileKey] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([profileKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LeadershipIncentive] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LeadershipIncent__22AA2996] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LearningSupportUnit] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LearningSupportU__6D0D32F4] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LegacyData] (
    [URN]     NUMERIC (19)   NOT NULL,
    [toeCode] NVARCHAR (255) NULL,
    [toeName] NVARCHAR (255) NULL,
    [ptFlag]  NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([URN] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LegacyData_AUD] (
    [URN]     NUMERIC (19)   NOT NULL,
    [ver_rev] NUMERIC (19)   NOT NULL,
    [REVTYPE] TINYINT        NULL,
    [ptFlag]  NVARCHAR (255) NULL,
    [toeCode] NVARCHAR (255) NULL,
    [toeName] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([URN] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LinkType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LinkType__52593CB8] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LLSC] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LLSC__4E88ABD4] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LlscLookup] (
    [la]       NVARCHAR (32) NOT NULL,
    [llsc]     NVARCHAR (32) NULL,
    [llscname] NVARCHAR (64) NULL
);


GO

CREATE TABLE [dbo].[LocalAuthority] (
    [code]             NVARCHAR (32)  NOT NULL,
    [group_code]       NVARCHAR (32)  NOT NULL,
    [name]             NVARCHAR (255) NULL,
    [orderBy]          INT            NULL,
    [archived]         TINYINT        NULL,
    [contactEmail]     NVARCHAR (100) NULL,
    [contactFirstName] NVARCHAR (50)  NULL,
    [contactLastName]  NVARCHAR (50)  NULL,
    [contactPhone]     NVARCHAR (50)  NULL,
    [contactTitle]     NVARCHAR (50)  NULL,
    [id]               INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LocalAuthority__2C3393D0] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LocalAuthority_AUD] (
    [code]       NVARCHAR (32)  NOT NULL,
    [ver_rev]    NUMERIC (19)   NOT NULL,
    [REVTYPE]    TINYINT        NULL,
    [archived]   TINYINT        NULL,
    [name]       NVARCHAR (255) NULL,
    [orderBy]    INT            NULL,
    [group_code] NVARCHAR (32)  NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LocalAuthorityContactDetails] (
    [id]                  NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [email]               NVARCHAR (255) NULL,
    [firstName]           NVARCHAR (255) NULL,
    [lastName]            NVARCHAR (255) NULL,
    [phone]               NVARCHAR (255) NULL,
    [position]            NVARCHAR (255) NULL,
    [version]             INT            NULL,
    [localAuthority_code] NVARCHAR (32)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LocalAuthorityGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LocalAuthorityGr__2A4B4B5E] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[LSOA] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__LSOA__05D8E0BE] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[MarketingOptOut] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__MarketingOptOut__74AE54BC] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[MarketingOptOut_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[MeasureType] (
    [code]             NVARCHAR (32)  NOT NULL,
    [archived]         TINYINT        NULL,
    [name]             NVARCHAR (255) NOT NULL,
    [affectsSensitive] TINYINT        NOT NULL,
    [id]               INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[MovedToEdubaseLogin] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[MSOA] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__MSOA__07C12930] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Nationality] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[NewsStory] (
    [id]                   NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [version]              INT             NULL,
    [summary]              NVARCHAR (255)  NOT NULL,
    [text]                 NVARCHAR (4000) NOT NULL,
    [title]                NVARCHAR (255)  NOT NULL,
    [approved]             TINYINT         NULL,
    [effectiveDate]        DATETIME        NOT NULL,
    [createdAt]            DATETIME        NULL,
    [updatedAt]            DATETIME        NULL,
    [createdBy_username]   NVARCHAR (255)  NOT NULL,
    [signOffUser_username] NVARCHAR (255)  NOT NULL,
    [signedOffBy_username] NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[NewsStory_UserGroup] (
    [NewsStory_id] NUMERIC (19)  NOT NULL,
    [groups_code]  NVARCHAR (32) NOT NULL,
    CONSTRAINT [PK__NewsStory_UserGr__5555A4F4] PRIMARY KEY CLUSTERED ([NewsStory_id] ASC, [groups_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[NurseryProvision] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__NurseryProvision__6EF57B66] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OfficialSixthForm] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__OfficialSixthFor__70DDC3D8] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OfstedRating] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OfstedSpecialMeasures] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__OfstedSpecialMea__72C60C4A] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OlapTable] (
    [URN]                                 NUMERIC (19)   NOT NULL,
    [LocalAuthority]                      NVARCHAR (32)  NOT NULL,
    [GSSLACode]                           NVARCHAR (32)  NOT NULL,
    [EstablishmentName]                   NVARCHAR (100) NULL,
    [EstablishmentType]                   NVARCHAR (32)  NOT NULL,
    [EstablishmentTypeMiddle]             VARCHAR (2)    NOT NULL,
    [EstablishmentTypeUpper]              VARCHAR (2)    NOT NULL,
    [EstablishmentStatus]                 NVARCHAR (32)  NOT NULL,
    [OpenDateYear]                        VARCHAR (64)   NOT NULL,
    [OpenDateTerm]                        VARCHAR (9)    NOT NULL,
    [CloseDateYear]                       VARCHAR (64)   NOT NULL,
    [CloseDateTerm]                       VARCHAR (9)    NOT NULL,
    [LAGroup]                             VARCHAR (1)    NOT NULL,
    [AgeRange]                            VARCHAR (1)    NOT NULL,
    [statutoryHighAge]                    VARCHAR (30)   NOT NULL,
    [statutoryLowAge]                     VARCHAR (30)   NOT NULL,
    [Boarders]                            NVARCHAR (32)  NOT NULL,
    [OfficialSixthForm]                   NVARCHAR (32)  NOT NULL,
    [ReasonEstablishmentOpened]           NVARCHAR (32)  NOT NULL,
    [ReasonEstablishmentClosed]           NVARCHAR (32)  NOT NULL,
    [FurtherEducationType]                NVARCHAR (32)  NOT NULL,
    [NurseryProvision]                    NVARCHAR (32)  NOT NULL,
    [EducationPhase]                      NVARCHAR (32)  NOT NULL,
    [EducationPhaseGroup]                 VARCHAR (1)    NOT NULL,
    [Gender]                              NVARCHAR (32)  NOT NULL,
    [ReligiousCharacter]                  NVARCHAR (32)  NOT NULL,
    [Diocese]                             NVARCHAR (32)  NOT NULL,
    [AdmissionsPolicy]                    NVARCHAR (32)  NOT NULL,
    [TrustSchool]                         NVARCHAR (32)  NOT NULL,
    [FreeMeals]                           VARCHAR (9)    NOT NULL,
    [PercentageFSM]                       VARCHAR (9)    NOT NULL,
    [SchoolCapacity]                      VARCHAR (14)   NOT NULL,
    [GonvertmentFundedTotal]              VARCHAR (14)   NOT NULL,
    [SpecialClasses]                      NVARCHAR (32)  NOT NULL,
    [FreshStart]                          NVARCHAR (32)  NOT NULL,
    [OffstedLastInspection]               VARCHAR (61)   NULL,
    [RegisteredEarlyYears]                NVARCHAR (32)  NOT NULL,
    [EarlyExcellence]                     NVARCHAR (32)  NOT NULL,
    [SpecialPupils]                       VARCHAR (14)   NOT NULL,
    [SpecialBoarders]                     VARCHAR (14)   NOT NULL,
    [TeenagedMothers]                     NVARCHAR (32)  NOT NULL,
    [TeenagedMothersPlaces]               VARCHAR (12)   NOT NULL,
    [ChildCareFacilities]                 NVARCHAR (32)  NOT NULL,
    [SpecialEducationalNeeds]             NVARCHAR (32)  NOT NULL,
    [EmotionalAndBehaviouralDifficulties] NVARCHAR (32)  NOT NULL,
    [PlacesPRU]                           VARCHAR (12)   NOT NULL,
    [FtProvision]                         NVARCHAR (32)  NOT NULL,
    [EducationByOthers]                   NVARCHAR (32)  NOT NULL,
    [FirstMainSpecialism]                 NVARCHAR (32)  NOT NULL,
    [SecondMainSpecialism]                NVARCHAR (32)  NOT NULL,
    [FirstSecondarySpecialism]            NVARCHAR (32)  NOT NULL,
    [SecondSecondarySpecialism]           NVARCHAR (32)  NOT NULL,
    [leadingOption1]                      NVARCHAR (32)  NOT NULL,
    [leadingOption2]                      NVARCHAR (32)  NOT NULL,
    [leadingOption3]                      NVARCHAR (32)  NOT NULL,
    [OfstedSpecialMeasures]               NVARCHAR (32)  NOT NULL,
    [InvestorInPeople]                    NVARCHAR (32)  NOT NULL,
    [PrivateFinanceInitiative]            NVARCHAR (32)  NOT NULL,
    [trainingEstablishment_code]          NVARCHAR (32)  NOT NULL,
    [County]                              NVARCHAR (32)  NOT NULL,
    [ContactCounty]                       NVARCHAR (32)  NOT NULL,
    [BoysNumber]                          VARCHAR (14)   NULL,
    [GirlsNumber]                         VARCHAR (14)   NULL,
    [censusHighestAge]                    INT            NULL,
    [censusHighestAgeGroup]               VARCHAR (9)    NOT NULL,
    [censusLowestAge]                     INT            NULL,
    [censusLowestAgeGroup]                VARCHAR (9)    NOT NULL,
    [Boys2]                               VARCHAR (9)    NOT NULL,
    [Boys3]                               VARCHAR (9)    NOT NULL,
    [Boys4a]                              VARCHAR (9)    NOT NULL,
    [Boys4b]                              VARCHAR (9)    NOT NULL,
    [Boys4c]                              VARCHAR (9)    NOT NULL,
    [Boys5]                               VARCHAR (9)    NOT NULL,
    [Boys6]                               VARCHAR (9)    NOT NULL,
    [Boys7]                               VARCHAR (9)    NOT NULL,
    [Boys8]                               VARCHAR (9)    NOT NULL,
    [Boys9]                               VARCHAR (9)    NOT NULL,
    [Boys10]                              VARCHAR (9)    NOT NULL,
    [Boys11]                              VARCHAR (9)    NOT NULL,
    [Boys12]                              VARCHAR (9)    NOT NULL,
    [Boys13]                              VARCHAR (9)    NOT NULL,
    [Boys14]                              VARCHAR (9)    NOT NULL,
    [Boys15]                              VARCHAR (9)    NOT NULL,
    [Boys16]                              VARCHAR (9)    NOT NULL,
    [Boys17]                              VARCHAR (9)    NOT NULL,
    [Boys18]                              VARCHAR (9)    NOT NULL,
    [Boys19]                              VARCHAR (9)    NOT NULL,
    [Girls2]                              VARCHAR (9)    NOT NULL,
    [Girls3]                              VARCHAR (9)    NOT NULL,
    [Girls4a]                             VARCHAR (9)    NOT NULL,
    [Girls4b]                             VARCHAR (9)    NOT NULL,
    [Girls4c]                             VARCHAR (9)    NOT NULL,
    [Girls5]                              VARCHAR (9)    NOT NULL,
    [Girls6]                              VARCHAR (9)    NOT NULL,
    [Girls7]                              VARCHAR (9)    NOT NULL,
    [Girls8]                              VARCHAR (9)    NOT NULL,
    [Girls9]                              VARCHAR (9)    NOT NULL,
    [Girls10]                             VARCHAR (9)    NOT NULL,
    [Girls11]                             VARCHAR (9)    NOT NULL,
    [Girls12]                             VARCHAR (9)    NOT NULL,
    [Girls13]                             VARCHAR (9)    NOT NULL,
    [Girls14]                             VARCHAR (9)    NOT NULL,
    [Girls15]                             VARCHAR (9)    NOT NULL,
    [Girls16]                             VARCHAR (9)    NOT NULL,
    [Girls17]                             VARCHAR (9)    NOT NULL,
    [Girls18]                             VARCHAR (9)    NOT NULL,
    [Girls19]                             VARCHAR (9)    NOT NULL,
    [BoysNursery]                         INT            NOT NULL,
    [BoysPrimary]                         INT            NOT NULL,
    [BoysSecondary]                       INT            NOT NULL,
    [BoysPost16]                          INT            NOT NULL,
    [GirlsNursery]                        INT            NOT NULL,
    [GirlsPrimary]                        INT            NOT NULL,
    [GirlsSecondary]                      INT            NOT NULL,
    [GirlsPost16]                         INT            NOT NULL,
    [UrbanRural]                          NVARCHAR (32)  NOT NULL,
    [UrbanRuralGroup]                     VARCHAR (1)    NOT NULL,
    [GOR]                                 NVARCHAR (32)  NOT NULL,
    [AccomChange]                         NVARCHAR (32)  NOT NULL,
    [Association]                         NVARCHAR (32)  NOT NULL,
    [CharityName]                         NVARCHAR (255) NOT NULL,
    [MonthOfLastBridgeVisit]              VARCHAR (30)   NOT NULL,
    [MonthOfLastFPVisit]                  VARCHAR (30)   NOT NULL,
    [MonthOfLastISIVisit]                 VARCHAR (30)   NOT NULL,
    [MonthOfLastOfstedVisit]              VARCHAR (30)   NOT NULL,
    [MonthOfLastSISVisit]                 VARCHAR (30)   NOT NULL,
    [MonthOfLastWelfareVisit]             VARCHAR (30)   NOT NULL,
    [IndependentSchoolType]               NVARCHAR (32)  NOT NULL,
    [inspectorate]                        NVARCHAR (32)  NOT NULL,
    [MonthOfactionRequiredFP]             VARCHAR (30)   NOT NULL,
    [MonthOfactionRequiredWEL]            VARCHAR (30)   NOT NULL,
    [MonthOfnextGeneralActionRequired]    VARCHAR (30)   NOT NULL,
    [MonthOfnextOfstedVisit]              VARCHAR (30)   NOT NULL,
    [PTBoys2]                             VARCHAR (14)   NOT NULL,
    [PTBoys3]                             VARCHAR (14)   NOT NULL,
    [PTBoys4a]                            VARCHAR (14)   NOT NULL,
    [PTBoys4b]                            VARCHAR (14)   NOT NULL,
    [PTBoys4c]                            VARCHAR (14)   NOT NULL,
    [PTGirls2]                            VARCHAR (14)   NOT NULL,
    [PTGirls3]                            VARCHAR (14)   NOT NULL,
    [PTGirls4a]                           VARCHAR (14)   NOT NULL,
    [PTGirls4b]                           VARCHAR (14)   NOT NULL,
    [PTGirls4c]                           VARCHAR (14)   NOT NULL,
    [SENnoStat]                           VARCHAR (14)   NOT NULL,
    [SENstat]                             VARCHAR (14)   NOT NULL,
    [ResourcedProvisionOnRoll]            VARCHAR (13)   NOT NULL,
    [ResourcedProvisionCapacity]          VARCHAR (13)   NOT NULL,
    [SenUnitOnRoll]                       VARCHAR (13)   NOT NULL,
    [SenUnitCapacity]                     VARCHAR (13)   NOT NULL,
    [PupilToTeacherRatio]                 NVARCHAR (255) NOT NULL,
    [TotalWeeklyHoursPTStaff]             VARCHAR (30)   NOT NULL,
    [TotalBoyBoarders]                    VARCHAR (30)   NOT NULL,
    [TotalPupilsFullTime]                 VARCHAR (30)   NOT NULL,
    [TotalNumberFTStaff]                  VARCHAR (30)   NOT NULL,
    [TotalGirlBoarders]                   VARCHAR (30)   NOT NULL,
    [TotalPupilsPartTime]                 VARCHAR (30)   NOT NULL,
    [TotalNumberPTStaff]                  VARCHAR (30)   NOT NULL,
    [CompulsorySchoolAge]                 VARCHAR (30)   NOT NULL,
    [TotalPupilsPublicCare]               VARCHAR (30)   NOT NULL,
    [BoardingEstablishment]               NVARCHAR (32)  NOT NULL,
    [Specialist]                          VARCHAR (1)    NOT NULL,
    [HighestBoardFee]                     VARCHAR (19)   NOT NULL,
    [HighestDayFee]                       VARCHAR (19)   NOT NULL,
    [LowestBoardFee]                      VARCHAR (19)   NOT NULL,
    [LowestDayFee]                        VARCHAR (19)   NOT NULL,
    [EYFSExemption]                       NVARCHAR (32)  NOT NULL
);


GO

CREATE TABLE [dbo].[OldDictionary] (
    [code]  NVARCHAR (32)  NOT NULL,
    [name]  NVARCHAR (255) NULL,
    [field] NVARCHAR (32)  NOT NULL,
    CONSTRAINT [PK__OldDictionary__15A53433] PRIMARY KEY CLUSTERED ([code] ASC, [field] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OperationalHours] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OwningRule] (
    [id]             NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [field]          NVARCHAR (32) NOT NULL,
    [type]           NVARCHAR (32) NOT NULL,
    [status]         NVARCHAR (32) NOT NULL,
    [userGroup_code] NVARCHAR (32) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([field] ASC, [type] ASC, [status] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[OwningRule_aud] (
    [ver_rev]        NUMERIC (19)  NOT NULL,
    [REVTYPE]        TINYINT       NULL,
    [id]             NUMERIC (19)  NOT NULL,
    [field]          NVARCHAR (32) NULL,
    [status]         NVARCHAR (32) NULL,
    [type]           NVARCHAR (32) NULL,
    [userGroup_code] NVARCHAR (32) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ParliamentaryConstituency] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ParliamentaryCon__76969D2E] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[PaymentTerm] (
    [code] NVARCHAR (255) NOT NULL,
    [name] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[PersonalSettings] (
    [id]             NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [showFavourites] TINYINT        NOT NULL,
    [showNews]       TINYINT        NOT NULL,
    [showReminders]  TINYINT        NOT NULL,
    [startPage]      NVARCHAR (255) NOT NULL,
    [username]       NVARCHAR (255) NULL,
    [userGroup]      NVARCHAR (32)  NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[PersonalSettings_DefaultColumns] (
    [PersonalSettings_id] NUMERIC (19)   NOT NULL,
    [element]             NVARCHAR (255) NULL
);


GO

CREATE TABLE [dbo].[PersonalSettings_DefaultExtractFields] (
    [id]             NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [showLookupName] TINYINT       NOT NULL,
    [field]          NVARCHAR (32) NOT NULL,
    [username]       NUMERIC (19)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[postcodes] (
    [Post_Code] NVARCHAR (50) NULL,
    [Age]       NVARCHAR (50) NULL,
    [Gender]    NVARCHAR (50) NULL,
    [Ethnicity] NVARCHAR (50) NULL
);


GO

CREATE TABLE [dbo].[postcodes_import] (
    [Post_Code] VARCHAR (255) NULL,
    [Age]       VARCHAR (255) NULL,
    [Gender]    VARCHAR (255) NULL,
    [Ethnicity] VARCHAR (255) NULL
);


GO

CREATE TABLE [dbo].[PredefinedReportCategory] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[PrivateFinanceInitiative] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__PrivateFinanceIn__787EE5A0] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ProprietorType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ProvisionEbd] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ProvisionEbd__7A672E12] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ProvisionSen] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ProvisionSen__7C4F7684] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_BLOB_TRIGGERS] (
    [SCHED_NAME]    VARCHAR (120) NOT NULL,
    [TRIGGER_NAME]  VARCHAR (200) NOT NULL,
    [TRIGGER_GROUP] VARCHAR (200) NOT NULL,
    [BLOB_DATA]     IMAGE         NULL
);


GO

CREATE TABLE [dbo].[QRTZ_CALENDARS] (
    [SCHED_NAME]    VARCHAR (120)   NOT NULL,
    [CALENDAR_NAME] VARCHAR (200)   NOT NULL,
    [CALENDAR]      VARBINARY (MAX) NULL,
    CONSTRAINT [PK_QRTZ_CALENDARS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [CALENDAR_NAME] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_CRON_TRIGGERS] (
    [SCHED_NAME]      VARCHAR (120) NOT NULL,
    [TRIGGER_NAME]    VARCHAR (200) NOT NULL,
    [TRIGGER_GROUP]   VARCHAR (200) NOT NULL,
    [CRON_EXPRESSION] VARCHAR (120) NOT NULL,
    [TIME_ZONE_ID]    VARCHAR (80)  NULL,
    CONSTRAINT [PK_QRTZ_CRON_TRIGGERS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_NAME] ASC, [TRIGGER_GROUP] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_FIRED_TRIGGERS] (
    [SCHED_NAME]        VARCHAR (120) NOT NULL,
    [ENTRY_ID]          VARCHAR (95)  NOT NULL,
    [TRIGGER_NAME]      VARCHAR (200) NOT NULL,
    [TRIGGER_GROUP]     VARCHAR (200) NOT NULL,
    [INSTANCE_NAME]     VARCHAR (200) NOT NULL,
    [FIRED_TIME]        BIGINT        NOT NULL,
    [SCHED_TIME]        BIGINT        NOT NULL,
    [PRIORITY]          INT           NOT NULL,
    [STATE]             VARCHAR (16)  NOT NULL,
    [JOB_NAME]          VARCHAR (200) NULL,
    [JOB_GROUP]         VARCHAR (200) NULL,
    [IS_NONCONCURRENT]  VARCHAR (1)   NULL,
    [REQUESTS_RECOVERY] VARCHAR (1)   NULL,
    CONSTRAINT [PK_QRTZ_FIRED_TRIGGERS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [ENTRY_ID] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_JOB_DETAILS] (
    [SCHED_NAME]        VARCHAR (120) NOT NULL,
    [JOB_NAME]          VARCHAR (200) NOT NULL,
    [JOB_GROUP]         VARCHAR (200) NOT NULL,
    [DESCRIPTION]       VARCHAR (250) NULL,
    [JOB_CLASS_NAME]    VARCHAR (250) NOT NULL,
    [IS_DURABLE]        VARCHAR (1)   NOT NULL,
    [IS_NONCONCURRENT]  VARCHAR (1)   NOT NULL,
    [IS_UPDATE_DATA]    VARCHAR (1)   NOT NULL,
    [REQUESTS_RECOVERY] VARCHAR (1)   NOT NULL,
    [JOB_DATA]          IMAGE         NULL,
    CONSTRAINT [PK_QRTZ_JOB_DETAILS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [JOB_NAME] ASC, [JOB_GROUP] ASC)
);


GO

CREATE TABLE [dbo].[QRTZ_LOCKS] (
    [SCHED_NAME] VARCHAR (120) NOT NULL,
    [LOCK_NAME]  VARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_QRTZ_LOCKS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [LOCK_NAME] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS] (
    [SCHED_NAME]    VARCHAR (120) NOT NULL,
    [TRIGGER_GROUP] VARCHAR (200) NOT NULL,
    CONSTRAINT [PK_QRTZ_PAUSED_TRIGGER_GRPS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_GROUP] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_SCHEDULER_STATE] (
    [SCHED_NAME]        VARCHAR (120) NOT NULL,
    [INSTANCE_NAME]     VARCHAR (200) NOT NULL,
    [LAST_CHECKIN_TIME] BIGINT        NOT NULL,
    [CHECKIN_INTERVAL]  BIGINT        NOT NULL,
    CONSTRAINT [PK_QRTZ_SCHEDULER_STATE] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [INSTANCE_NAME] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] (
    [SCHED_NAME]      VARCHAR (120) NOT NULL,
    [TRIGGER_NAME]    VARCHAR (200) NOT NULL,
    [TRIGGER_GROUP]   VARCHAR (200) NOT NULL,
    [REPEAT_COUNT]    BIGINT        NOT NULL,
    [REPEAT_INTERVAL] BIGINT        NOT NULL,
    [TIMES_TRIGGERED] BIGINT        NOT NULL,
    CONSTRAINT [PK_QRTZ_SIMPLE_TRIGGERS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_NAME] ASC, [TRIGGER_GROUP] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] (
    [SCHED_NAME]    VARCHAR (120)   NOT NULL,
    [TRIGGER_NAME]  VARCHAR (200)   NOT NULL,
    [TRIGGER_GROUP] VARCHAR (200)   NOT NULL,
    [STR_PROP_1]    VARCHAR (512)   NULL,
    [STR_PROP_2]    VARCHAR (512)   NULL,
    [STR_PROP_3]    VARCHAR (512)   NULL,
    [INT_PROP_1]    INT             NULL,
    [INT_PROP_2]    INT             NULL,
    [LONG_PROP_1]   BIGINT          NULL,
    [LONG_PROP_2]   BIGINT          NULL,
    [DEC_PROP_1]    NUMERIC (13, 4) NULL,
    [DEC_PROP_2]    NUMERIC (13, 4) NULL,
    [BOOL_PROP_1]   VARCHAR (1)     NULL,
    [BOOL_PROP_2]   VARCHAR (1)     NULL,
    CONSTRAINT [PK_QRTZ_SIMPROP_TRIGGERS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_NAME] ASC, [TRIGGER_GROUP] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[QRTZ_TRIGGERS] (
    [SCHED_NAME]     VARCHAR (120) NOT NULL,
    [TRIGGER_NAME]   VARCHAR (200) NOT NULL,
    [TRIGGER_GROUP]  VARCHAR (200) NOT NULL,
    [JOB_NAME]       VARCHAR (200) NOT NULL,
    [JOB_GROUP]      VARCHAR (200) NOT NULL,
    [DESCRIPTION]    VARCHAR (250) NULL,
    [NEXT_FIRE_TIME] BIGINT        NULL,
    [PREV_FIRE_TIME] BIGINT        NULL,
    [PRIORITY]       INT           NULL,
    [TRIGGER_STATE]  VARCHAR (16)  NOT NULL,
    [TRIGGER_TYPE]   VARCHAR (8)   NOT NULL,
    [START_TIME]     BIGINT        NOT NULL,
    [END_TIME]       BIGINT        NULL,
    [CALENDAR_NAME]  VARCHAR (200) NULL,
    [MISFIRE_INSTR]  SMALLINT      NULL,
    [JOB_DATA]       IMAGE         NULL,
    CONSTRAINT [PK_QRTZ_TRIGGERS] PRIMARY KEY CLUSTERED ([SCHED_NAME] ASC, [TRIGGER_NAME] ASC, [TRIGGER_GROUP] ASC)
);


GO

CREATE TABLE [dbo].[QualityAssuranceBodyName] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [code]     VARCHAR (32)  NOT NULL,
    [name]     VARCHAR (255) NOT NULL,
    [archived] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[RadiusSearchData] (
    [id]       NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [easting]  INT            NOT NULL,
    [northing] INT            NOT NULL,
    [radius]   NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReasonEstablishmentClosed] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ReasonEstablishm__07F6335A] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReasonEstablishmentClosed_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReasonEstablishmentOpened] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ReasonEstablishm__1367E606] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReasonEstablishmentOpened_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Recipient] (
    [id]                  NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [info]                NVARCHAR (255) NULL,
    [type]                NVARCHAR (255) NOT NULL,
    [establishment_URN]   NUMERIC (19)   NULL,
    [user_username]       NVARCHAR (255) NULL,
    [messageId]           NUMERIC (19)   NULL,
    [localAuthority_code] NVARCHAR (32)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[RecordStatus] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[RegisteredEarlyYears] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__RegisteredEarlyY__173876EA] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[RegistrationSuspended] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [code]     VARCHAR (32)  NOT NULL,
    [name]     VARCHAR (255) NOT NULL,
    [orderBy]  INT           NULL,
    [archived] BIT           NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReligiousCharacter] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ReligiousCharact__0BC6C43E] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReligiousCharacter_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReligiousEthos] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Reminder] (
    [id]               NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [creatorUsername]  NVARCHAR (255)  NULL,
    [details]          NVARCHAR (4000) NULL,
    [dueDate]          DATETIME        NOT NULL,
    [notificationDate] DATETIME        NULL,
    [title]            NVARCHAR (255)  NOT NULL,
    [creatorUserGroup] NVARCHAR (32)   NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReminderInstance] (
    [id]          NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [complete]    TINYINT        NOT NULL,
    [URN]         NUMERIC (19)   NULL,
    [reminder_id] NUMERIC (19)   NULL,
    [username]    NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReportFile] (
    [id]                NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [reportKey]         NVARCHAR (128) NOT NULL,
    [lastGeneratedDate] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([reportKey] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReportFile_AUD] (
    [id]                NUMERIC (19)   NOT NULL,
    [ver_rev]           NUMERIC (19)   NOT NULL,
    [REVTYPE]           TINYINT        NULL,
    [implClass]         NVARCHAR (255) NULL,
    [reportKey]         NVARCHAR (128) NULL,
    [lastGeneratedDate] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ReportFilePermission] (
    [reportKey] NUMERIC (19)  NOT NULL,
    [groupCode] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([reportKey] ASC, [groupCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[RevisionInfo] (
    [id]                  NUMERIC (19)   IDENTITY (0, 1) NOT NULL,
    [timestamp]           NUMERIC (19)   NOT NULL,
    [modifiedBy_username] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[RSCRegion] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[S2SLogin] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SAAccessRestrictions] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SanityCheckLog] (
    [id]              NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [lostChanges]     INT            NULL,
    [sanityCheckDate] DATETIME       NOT NULL,
    [status]          NVARCHAR (255) NULL,
    [timestamp]       ROWVERSION     NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ScheduledExtract] (
    [id]                    NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [lastExtractionDate]    DATETIME       NULL,
    [name]                  NVARCHAR (255) NOT NULL,
    [endDate]               DATETIME       NULL,
    [frequency]             NVARCHAR (255) NULL,
    [startDate]             DATETIME       NULL,
    [type]                  NVARCHAR (255) NULL,
    [filter_id]             NUMERIC (19)   NOT NULL,
    [includeLinksFeed]      TINYINT        NOT NULL,
    [maskNumericValues]     TINYINT        NOT NULL,
    [creatorUsername]       NVARCHAR (255) NULL,
    [creatorGroup]          NVARCHAR (32)  NULL,
    [includeGroupLinksFeed] TINYINT        NULL,
    [includeGroupsFeed]     TINYINT        NULL,
    [topPriority]           TINYINT        NULL,
    [publicAccessible]      TINYINT        NULL,
    [asUserUsername]        NVARCHAR (255) NULL,
    [asGroup]               NVARCHAR (32)  NULL,
    [callback_id]           NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ScheduledExtractLog] (
    [id]                  NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [timestamp]           DATETIME       NULL,
    [systemUser]          NVARCHAR (255) NULL,
    [scheduledExtract]    NUMERIC (19)   NULL,
    [isGeneratedOnFly]    TINYINT        NULL,
    [establishmentFilter] NUMERIC (19)   NULL,
    [success]             TINYINT        NULL,
    [timeFinished]        DATETIME       NULL,
    [timeStarted]         DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SchoolSponsorFlag] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SchoolUser] (
    [username] NVARCHAR (255) NOT NULL,
    [password] NVARCHAR (255) NULL,
    [URN]      NUMERIC (19)   NOT NULL,
    [version]  INT            NULL,
    [enabled]  TINYINT        NOT NULL,
    CONSTRAINT [PK__SchoolUser__093F5D4E] PRIMARY KEY CLUSTERED ([username] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SchoolUser_AUD] (
    [username] NVARCHAR (255) NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [enabled]  TINYINT        NULL,
    [password] NVARCHAR (255) NULL,
    [URN]      NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([username] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SecondMainSpecialism] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__SecondMainSpecia__62E4AA3C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SecondSpecialism] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__SecondSpecialism__65C116E7] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Section] (
    [id]          NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [text]        NVARCHAR (MAX) NOT NULL,
    [title]       NVARCHAR (255) NULL,
    [version]     INT            NULL,
    [weight]      NUMERIC (19)   NULL,
    [document_id] NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Section_AUD] (
    [id]          NUMERIC (19)   NOT NULL,
    [ver_rev]     NUMERIC (19)   NOT NULL,
    [REVTYPE]     TINYINT        NULL,
    [text]        NVARCHAR (MAX) NULL,
    [title]       NVARCHAR (255) NULL,
    [weight]      NUMERIC (19)   NULL,
    [document_id] NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Section41Approved] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Sen] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Sen__2E1BDC42] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Sen_AUD] (
    [code]     NVARCHAR (32)  NOT NULL,
    [ver_rev]  NUMERIC (19)   NOT NULL,
    [REVTYPE]  TINYINT        NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SingleClickQuery] (
    [id]          NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [description] NVARCHAR (1024) NULL,
    [query]       NVARCHAR (MAX)  NULL,
    [title]       NVARCHAR (255)  NULL,
    [code]        NVARCHAR (10)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SingleClickQuery_AUD] (
    [id]          NUMERIC (19)    NOT NULL,
    [ver_rev]     NUMERIC (19)    NOT NULL,
    [REVTYPE]     TINYINT         NULL,
    [description] NVARCHAR (1024) NULL,
    [query]       NVARCHAR (MAX)  NULL,
    [title]       NVARCHAR (255)  NULL,
    [code]        NVARCHAR (10)   NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SixtyDayNotUpdatedReminder] (
    [id]           NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [urn]          NUMERIC (19)  NOT NULL,
    [sending_date] DATETIME      NOT NULL,
    [addressee]    VARCHAR (100) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([urn] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SixtyDayNotUpdatedReminderFlag] (
    [id]          NUMERIC (5)  IDENTITY (1, 1) NOT NULL,
    [enabled]     BIT          NULL,
    [update_date] DATETIME     NOT NULL,
    [updated_by]  VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SnacLocalAuthority] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__SnacLocalAuthori__37A5467C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SpecialClasses] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__SpecialClasses__7E37BEF6] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Specialism] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Specialism__3D5E1FD2] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffAppointingBody] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffChangeRequest] (
    [type]                          NVARCHAR (31)   NOT NULL,
    [id]                            NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [createdDate]                   DATETIME        NULL,
    [effectiveDate]                 DATETIME        NULL,
    [lastChangeDate]                DATETIME        NOT NULL,
    [rejectReason]                  NVARCHAR (2048) NULL,
    [status]                        NVARCHAR (255)  NOT NULL,
    [version]                       INT             NULL,
    [encrypted]                     TINYINT         NULL,
    [dateNewValueEnc]               VARBINARY (MAX) NULL,
    [dateNewValue]                  DATETIME        NULL,
    [dateOldValueEnc]               VARBINARY (MAX) NULL,
    [dateOldValue]                  DATETIME        NULL,
    [lookupNewValueEnc]             VARBINARY (MAX) NULL,
    [lookupNewValue]                NVARCHAR (255)  NULL,
    [lookupOldValueEnc]             VARBINARY (MAX) NULL,
    [lookupOldValue]                NVARCHAR (255)  NULL,
    [stringNewValueEnc]             VARBINARY (MAX) NULL,
    [stringNewValue]                NVARCHAR (255)  NULL,
    [stringOldValueEnc]             VARBINARY (MAX) NULL,
    [stringOldValue]                NVARCHAR (255)  NULL,
    [approvedOrRejectedBy_username] NVARCHAR (255)  NULL,
    [proposedBy_username]           NVARCHAR (255)  NOT NULL,
    [field_shortName]               NVARCHAR (32)   NULL,
    [staffRecord_uid]               NUMERIC (19)    NULL,
    [longNewValue]                  NUMERIC (19)    NULL,
    [longOldValue]                  NUMERIC (19)    NULL,
    [booleanNewValue]               TINYINT         NULL,
    [booleanOldValue]               TINYINT         NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffField] (
    [fieldType]        NVARCHAR (31)  NOT NULL,
    [shortName]        NVARCHAR (32)  NOT NULL,
    [displayName]      NVARCHAR (255) NOT NULL,
    [encrypted]        TINYINT        NULL,
    [forPublic]        TINYINT        NULL,
    [path]             NVARCHAR (64)  NULL,
    [readonly]         TINYINT        NULL,
    [shortDisplayName] NVARCHAR (64)  NULL,
    [lookupClass]      NVARCHAR (64)  NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([shortDisplayName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffField_AUD] (
    [fieldType]        NVARCHAR (31)  NOT NULL,
    [shortName]        NVARCHAR (32)  NOT NULL,
    [ver_rev]          NUMERIC (19)   NOT NULL,
    [REVTYPE]          TINYINT        NULL,
    [displayName]      NVARCHAR (255) NULL,
    [encrypted]        TINYINT        NULL,
    [forPublic]        TINYINT        NULL,
    [path]             NVARCHAR (64)  NULL,
    [readonly]         TINYINT        NULL,
    [shortDisplayName] NVARCHAR (64)  NULL,
    PRIMARY KEY CLUSTERED ([shortName] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffField_StaffFieldPosition_AUD] (
    [ver_rev]   NUMERIC (19)  NOT NULL,
    [shortName] NVARCHAR (32) NOT NULL,
    [id]        NUMERIC (19)  NOT NULL,
    [REVTYPE]   TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [shortName] ASC, [id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffFieldPermission] (
    [shortName]  NVARCHAR (32)  NOT NULL,
    [groupCode]  NVARCHAR (32)  NOT NULL,
    [permission] NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([groupCode] ASC, [shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffFieldPermission_AUD] (
    [ver_rev]    NUMERIC (19)   NOT NULL,
    [groupCode]  NVARCHAR (32)  NOT NULL,
    [permission] NVARCHAR (255) NOT NULL,
    [shortName]  NVARCHAR (32)  NOT NULL,
    [REVTYPE]    TINYINT        NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [groupCode] ASC, [permission] ASC, [shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffFieldPosition] (
    [id]                NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [orderBy]           INT           NULL,
    [showInRightColumn] TINYINT       NULL,
    [shortName]         NVARCHAR (32) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffFieldPosition_AUD] (
    [id]                NUMERIC (19) NOT NULL,
    [ver_rev]           NUMERIC (19) NOT NULL,
    [REVTYPE]           TINYINT      NULL,
    [orderBy]           INT          NULL,
    [showInRightColumn] TINYINT      NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffFieldRole] (
    [id]          NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [isNotNeeded] TINYINT       NULL,
    [isRequired]  TINYINT       NULL,
    [isWarning]   TINYINT       NULL,
    [staffField]  NVARCHAR (32) NOT NULL,
    [staffRole]   NVARCHAR (32) NOT NULL,
    [isReflected] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([staffField] ASC, [staffRole] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffFieldValidation] (
    [id]                 NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [shortName]          NVARCHAR (32) NOT NULL,
    [validationRuleCode] VARCHAR (32)  NOT NULL,
    [validationParam]    VARCHAR (512) NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([shortName] ASC, [validationRuleCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffImportRecord] (
    [id]               BIGINT         IDENTITY (1, 1) NOT NULL,
    [isRealProcessing] BIT            NULL,
    [originalFileName] NVARCHAR (255) NULL,
    [templateFilePath] NVARCHAR (255) NULL,
    [importFilePath]   NVARCHAR (255) NULL,
    [resultFilePath]   NVARCHAR (255) NULL,
    [importUUID]       NVARCHAR (255) NULL,
    [status]           NVARCHAR (255) NULL,
    [timestamp]        DATETIME       NULL
);


GO

CREATE TABLE [dbo].[StaffRecord] (
    [uid]                       NUMERIC (19)  NOT NULL,
    [appointmentDate]           DATETIME      NULL,
    [forename1]                 NVARCHAR (50) NULL,
    [forename2]                 NVARCHAR (50) NULL,
    [phone]                     NVARCHAR (40) NULL,
    [postcode]                  NVARCHAR (8)  NULL,
    [stepdownDate]              DATETIME      NULL,
    [surname]                   NVARCHAR (50) NULL,
    [establishment_URN]         NUMERIC (19)  NULL,
    [group_id]                  NUMERIC (19)  NULL,
    [staffAppointingBody_code]  NVARCHAR (32) NOT NULL,
    [staffRecordEnc_id]         NUMERIC (19)  NOT NULL,
    [staffRole_code]            NVARCHAR (32) NOT NULL,
    [title_code]                NVARCHAR (32) NOT NULL,
    [deleted]                   TINYINT       NULL,
    [forcedArchived]            TINYINT       NULL,
    [parentStaffRecord_uid]     NUMERIC (19)  NULL,
    [status]                    INT           NOT NULL,
    [isOriginalSignatoryMember] BIT           NULL,
    [isOriginalChairOfTrustees] BIT           NULL,
    [shared]                    AS            (CONVERT (TINYINT, CASE WHEN [staffRole_code] = '20'
                                                                           OR [staffRole_code] = '19'
                                                                           OR [staffRole_code] = '14'
                                                                           OR [staffRole_code] = '13'
                                                                           OR [staffRole_code] = '12'
                                                                           OR [staffRole_code] = '11' THEN (1) ELSE (0) END)) PERSISTED,
    PRIMARY KEY CLUSTERED ([uid] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([staffRecordEnc_id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRecord_AUD] (
    [uid]                       NUMERIC (19)  NOT NULL,
    [ver_rev]                   NUMERIC (19)  NOT NULL,
    [REVTYPE]                   TINYINT       NULL,
    [appointmentDate]           DATETIME      NULL,
    [forename1]                 NVARCHAR (50) NULL,
    [forename2]                 NVARCHAR (50) NULL,
    [phone]                     NVARCHAR (40) NULL,
    [postcode]                  NVARCHAR (8)  NULL,
    [status]                    INT           NULL,
    [stepdownDate]              DATETIME      NULL,
    [surname]                   NVARCHAR (50) NULL,
    [establishment_URN]         NUMERIC (19)  NULL,
    [group_id]                  NUMERIC (19)  NULL,
    [staffAppointingBody_code]  NVARCHAR (32) NULL,
    [staffRecordEnc_id]         NUMERIC (19)  NULL,
    [staffRole_code]            NVARCHAR (32) NULL,
    [title_code]                NVARCHAR (32) NULL,
    [deleted]                   TINYINT       NULL,
    [forcedArchived]            TINYINT       NULL,
    [shared]                    TINYINT       NULL,
    [parentStaffRecord_uid]     NUMERIC (19)  NULL,
    [isOriginalSignatoryMember] BIT           NULL,
    [isOriginalChairOfTrustees] BIT           NULL,
    PRIMARY KEY CLUSTERED ([uid] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRecordEnc] (
    [id]            NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [birthDate]     VARBINARY (MAX) NULL,
    [directEmail]   VARBINARY (MAX) NULL,
    [prevForename1] VARBINARY (MAX) NULL,
    [prevForename2] VARBINARY (MAX) NULL,
    [prevSurname]   VARBINARY (MAX) NULL,
    [prevTitle]     VARBINARY (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRecordEnc_AUD] (
    [id]            NUMERIC (19)    NOT NULL,
    [ver_rev]       NUMERIC (19)    NOT NULL,
    [REVTYPE]       TINYINT         NULL,
    [birthDate]     VARBINARY (MAX) NULL,
    [directEmail]   VARBINARY (MAX) NULL,
    [prevForename1] VARBINARY (MAX) NULL,
    [prevForename2] VARBINARY (MAX) NULL,
    [prevSurname]   VARBINARY (MAX) NULL,
    [prevTitle]     VARBINARY (MAX) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRecordsOutdated] (
    [id]                  NUMERIC (19) IDENTITY (1, 1) NOT NULL,
    [announcementId]      NUMERIC (19) NULL,
    [created]             DATETIME     NULL,
    [resolved]            DATETIME     NULL,
    [saOrgId]             NUMERIC (19) NULL,
    [uid]                 NUMERIC (19) NULL,
    [urn]                 NUMERIC (19) NULL,
    [governanceLastCheck] DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRole] (
    [code]            NVARCHAR (32)  NOT NULL,
    [archived]        TINYINT        NULL,
    [name]            NVARCHAR (255) NOT NULL,
    [orderBy]         INT            NULL,
    [isOnePersonRole] TINYINT        NOT NULL,
    [isSharing]       TINYINT        NOT NULL,
    [isShared]        TINYINT        NOT NULL,
    [id]              INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRoleExpiration] (
    [id]             NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [announcementId] NUMERIC (19)  NULL,
    [created]        DATETIME      NULL,
    [resolved]       DATETIME      NULL,
    [saOrgId]        NUMERIC (19)  NULL,
    [uid]            NUMERIC (19)  NULL,
    [urn]            NUMERIC (19)  NULL,
    [gid]            NUMERIC (19)  NOT NULL,
    [stepdownDate]   DATETIME      NULL,
    [staffRole]      NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffRoleType] (
    [typeCode] NVARCHAR (32) NOT NULL,
    [roleCode] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([typeCode] ASC, [roleCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffSharedViewCached] (
    [id]             VARCHAR (61)   NOT NULL,
    [UID]            NUMERIC (19)   NOT NULL,
    [matName]        NVARCHAR (255) NULL,
    [matGID]         NUMERIC (19)   NOT NULL,
    [roleCode]       NVARCHAR (32)  NOT NULL,
    [matStatus]      INT            NOT NULL,
    [childrenTotal]  INT            NOT NULL,
    [childrenActive] INT            NOT NULL,
    [URN]            NUMERIC (19)   NULL,
    [estName]        NVARCHAR (138) NULL,
    [estGID]         NUMERIC (19)   NULL,
    [estStatus]      INT            NULL,
    CONSTRAINT [PK_StaffSharedViewCached] PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StaffType] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[StudioSchoolIndicator] (
    [code]     NVARCHAR (32)  NOT NULL,
    [archived] TINYINT        NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [orderBy]  INT            NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SubscriptionInfo] (
    [id]              NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [buildingName]    NVARCHAR (255)  NULL,
    [notForMarketing] TINYINT         NOT NULL,
    [postcode]        NVARCHAR (255)  NULL,
    [privateUse]      TINYINT         NOT NULL,
    [purpose]         NVARCHAR (1000) NULL,
    [streetName]      NVARCHAR (255)  NULL,
    [streetNumber]    NVARCHAR (255)  NULL,
    [town]            NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SubscriptionInfo_aud] (
    [ver_rev]         NUMERIC (19)    NOT NULL,
    [REVTYPE]         TINYINT         NULL,
    [id]              NUMERIC (19)    NOT NULL,
    [buildingName]    NVARCHAR (255)  NULL,
    [notForMarketing] TINYINT         NULL,
    [postcode]        NVARCHAR (255)  NULL,
    [privateUse]      TINYINT         NULL,
    [purpose]         NVARCHAR (1000) NULL,
    [streetName]      NVARCHAR (255)  NULL,
    [streetNumber]    NVARCHAR (255)  NULL,
    [town]            NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SubscriptionInvoice] (
    [id]                   NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [amount]               NUMERIC (19, 2) NULL,
    [createdDate]          DATETIME        NULL,
    [instalment]           INT             NULL,
    [invoiceStatus]        NVARCHAR (255)  NULL,
    [paidDate]             DATETIME        NULL,
    [total]                NUMERIC (19, 2) NULL,
    [vatAmount]            NUMERIC (19, 2) NULL,
    [invoiceDocument_id]   NUMERIC (19)    NULL,
    [proformaDocument_id]  NUMERIC (19)    NOT NULL,
    [subscriptionOrder_id] NUMERIC (19)    NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SubscriptionOrder] (
    [id]                       NUMERIC (19)    IDENTITY (1, 1) NOT NULL,
    [createdDate]              DATETIME        NULL,
    [endSubscriptionDate]      DATETIME        NULL,
    [expiredDate]              DATETIME        NULL,
    [extractType]              NVARCHAR (255)  NULL,
    [sum]                      NUMERIC (19, 2) NULL,
    [paymentTerm_code]         NVARCHAR (255)  NULL,
    [schedule_id]              NUMERIC (19)    NULL,
    [subscriptionPackage_code] NVARCHAR (255)  NULL,
    [user_username]            NVARCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SubscriptionPackage] (
    [code]  NVARCHAR (255)  NOT NULL,
    [name]  NVARCHAR (255)  NULL,
    [price] NUMERIC (19, 2) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SubscriptionSchedule] (
    [type]       NVARCHAR (31) NOT NULL,
    [id]         NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [dayOfMonth] INT           NULL,
    [date1]      DATETIME      NULL,
    [date2]      DATETIME      NULL,
    [date3]      DATETIME      NULL,
    [dayOfWeek]  INT           NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SuperannuationCategory] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__SuperannuationCa__09DE7BCC] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SystemUser] (
    [username]                       NVARCHAR (255) NOT NULL,
    [company]                        NVARCHAR (255) NULL,
    [email]                          NVARCHAR (255) NULL,
    [enabled]                        TINYINT        NULL,
    [expiredDate]                    DATETIME       NULL,
    [fax]                            NVARCHAR (255) NULL,
    [firstName]                      NVARCHAR (255) NULL,
    [lastName]                       NVARCHAR (255) NULL,
    [neverExpired]                   TINYINT        NULL,
    [password]                       NVARCHAR (255) NULL,
    [phone]                          NVARCHAR (255) NULL,
    [version]                        INT            NULL,
    [URN]                            NUMERIC (19)   NULL,
    [UserGroupCode]                  NVARCHAR (32)  NOT NULL,
    [LocalAuthority]                 NVARCHAR (32)  NULL,
    [sendChangeRequestNotifications] TINYINT        NOT NULL,
    [superuser]                      TINYINT        NOT NULL,
    [subscriptionInfo_id]            NUMERIC (19)   NULL,
    [activated]                      TINYINT        NOT NULL,
    [agreementStatus]                NVARCHAR (255) NULL,
    [agreement_id]                   NUMERIC (19)   NULL,
    [activationCode]                 NVARCHAR (16)  NULL,
    [importedFromLdap]               TINYINT        NOT NULL,
    [positionInCompany]              NVARCHAR (255) NULL,
    [ldapProfile_id]                 NUMERIC (19)   NULL,
    [sendDataOwnerNotifications]     TINYINT        NOT NULL,
    [createdAt]                      DATETIME       NULL,
    [wsEnabled]                      TINYINT        NULL,
    [saUserId]                       NUMERIC (19)   NULL,
    [wsPassword]                     NVARCHAR (255) NULL,
    [wsUsername]                     NVARCHAR (255) NULL,
    [FullNameAlias]                  AS             ((CASE WHEN [saUserId] IS NOT NULL THEN CONVERT (VARCHAR, [saUserId], (0)) ELSE [username] END + ' ') + [UserGroupCode]) PERSISTED,
    [UID]                            NUMERIC (19)   NULL,
    CONSTRAINT [PK__SystemUser__1A34DF26] PRIMARY KEY CLUSTERED ([username] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[SystemUser_aud] (
    [ver_rev]                        NUMERIC (19)   NOT NULL,
    [revtype]                        SMALLINT       NOT NULL,
    [username]                       NVARCHAR (255) NOT NULL,
    [company]                        NVARCHAR (255) NULL,
    [email]                          NVARCHAR (255) NULL,
    [enabled]                        TINYINT        NULL,
    [expiredDate]                    DATETIME       NULL,
    [fax]                            NVARCHAR (255) NULL,
    [firstName]                      NVARCHAR (255) NULL,
    [lastName]                       NVARCHAR (255) NULL,
    [neverExpired]                   TINYINT        NULL,
    [password]                       NVARCHAR (255) NULL,
    [phone]                          NVARCHAR (255) NULL,
    [version]                        INT            NULL,
    [URN]                            NUMERIC (19)   NULL,
    [UserGroupCode]                  NVARCHAR (32)  NOT NULL,
    [LocalAuthority]                 NVARCHAR (32)  NULL,
    [sendChangeRequestNotifications] TINYINT        NOT NULL,
    [superuser]                      TINYINT        NOT NULL,
    [subscriptionInfo_id]            NUMERIC (19)   NULL,
    [activated]                      TINYINT        NULL,
    [agreementStatus]                NVARCHAR (255) NULL,
    [agreement_id]                   NUMERIC (19)   NULL,
    [activationCode]                 NVARCHAR (255) NULL,
    [importedFromLdap]               TINYINT        NOT NULL,
    [positionInCompany]              NVARCHAR (255) NULL,
    [sendDataOwnerNotifications]     TINYINT        NULL,
    [createdAt]                      DATETIME       NULL,
    [FullNameAlias]                  NVARCHAR (100) NULL,
    [saUserId]                       NUMERIC (19)   NULL,
    [wsEnabled]                      TINYINT        NULL,
    [wsPassword]                     NVARCHAR (255) NULL,
    [wsUsername]                     NVARCHAR (255) NULL,
    [UID]                            NUMERIC (19)   NULL,
    PRIMARY KEY CLUSTERED ([username] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TeenagedMothers] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__TeenagedMothers__00200768] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TelephoneUpdateProcess] (
    [id]                  NUMERIC (19) IDENTITY (1, 1) NOT NULL,
    [updated_items_count] NUMERIC (19) NULL,
    [process_status]      VARCHAR (30) NULL,
    [date_start]          DATETIME     NULL,
    [date_end]            DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TestEncTable] (
    [stroka]    VARCHAR (11)    NOT NULL,
    [strokaEnc] VARBINARY (MAX) NULL
);


GO

CREATE TABLE [dbo].[Title] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Title__15502E78] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ToeFieldGroup] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__ToeFieldGroup__5441852A] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Tool] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [code]     VARCHAR (32)  NOT NULL,
    [name]     VARCHAR (255) NOT NULL,
    [archived] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ToolPermission] (
    [id]         NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [groupCode]  NVARCHAR (32) NOT NULL,
    [toolCode]   VARCHAR (32)  NOT NULL,
    [permission] VARCHAR (32)  NOT NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([groupCode] ASC, [toolCode] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TrainingSchool] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__TrainingSchool__02084FDA] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TrueFalse] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__TrueFalse__33D4B598] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TrustedSource] (
    [id]             NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [field]          NVARCHAR (32) NOT NULL,
    [type]           NVARCHAR (32) NOT NULL,
    [userGroup_code] NVARCHAR (32) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE CLUSTERED ([field] ASC, [type] ASC, [userGroup_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TrustedSource_AUD] (
    [id]             NUMERIC (19)  NOT NULL,
    [ver_rev]        NUMERIC (19)  NOT NULL,
    [REVTYPE]        TINYINT       NULL,
    [field]          NVARCHAR (32) NULL,
    [type]           NVARCHAR (32) NULL,
    [userGroup_code] NVARCHAR (32) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TrustSchool] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__TrustSchool__03F0984C] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[Ttwa] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__Ttwa__09A971A2] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TypeOfReservedProvision] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[TypeOfSite] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__TypeOfSite__3F466844] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UkprnUpdateProcess] (
    [id]                  NUMERIC (19) IDENTITY (1, 1) NOT NULL,
    [since_date]          DATETIME     NOT NULL,
    [item_type]           VARCHAR (30) NULL,
    [updated_items_count] NUMERIC (19) NULL,
    [process_status]      VARCHAR (30) NULL,
    [date_start]          DATETIME     NULL,
    [date_end]            DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UrbanRural] (
    [code]     NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (255) NOT NULL,
    [archived] TINYINT        NULL,
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK__UrbanRural__7E6CC920] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UrnSequence] (
    [schoolType] NVARCHAR (32) NOT NULL,
    [maxURN]     BIGINT        NOT NULL,
    CONSTRAINT [PK_UrnSequence] PRIMARY KEY CLUSTERED ([schoolType] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UsedEstablishmentNumber] (
    [id]                  NUMERIC (19)  IDENTITY (1, 1) NOT NULL,
    [EstablishmentNumber] INT           NULL,
    [LA_code]             NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE),
    UNIQUE NONCLUSTERED ([LA_code] ASC, [EstablishmentNumber] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserActivity] (
    [id]            NUMERIC (19)   IDENTITY (1, 1) NOT NULL,
    [date]          DATETIME       NOT NULL,
    [type]          NVARCHAR (255) NOT NULL,
    [user_username] NVARCHAR (255) NOT NULL,
    [details]       NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserGroup] (
    [code]                    NVARCHAR (32)  NOT NULL,
    [name]                    NVARCHAR (255) NULL,
    [specialGroup]            TINYINT        NULL,
    [externalGroup]           TINYINT        NULL,
    [slaDays]                 INT            NULL,
    [role]                    NVARCHAR (32)  NULL,
    [recordStatus_code]       NVARCHAR (32)  NOT NULL,
    [sla_id]                  NUMERIC (19)   NULL,
    [allowedForAccessRequest] TINYINT        NOT NULL,
    [expiredDate]             DATETIME       NULL,
    [saUserGroupCode]         NVARCHAR (255) NULL,
    CONSTRAINT [PK__UserGroup__395884C4] PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserGroup_aud] (
    [ver_rev]                 NUMERIC (19)   NOT NULL,
    [REVTYPE]                 TINYINT        NULL,
    [code]                    NVARCHAR (32)  NOT NULL,
    [name]                    NVARCHAR (255) NULL,
    [specialGroup]            TINYINT        NULL,
    [externalGroup]           TINYINT        NULL,
    [slaDays]                 INT            NULL,
    [role]                    NVARCHAR (32)  NULL,
    [recordStatus_code]       NVARCHAR (32)  NULL,
    [UserGroupCode]           NVARCHAR (32)  NULL,
    [sla_id]                  NUMERIC (19)   NULL,
    [allowedForAccessRequest] TINYINT        NOT NULL,
    [expiredDate]             DATETIME       NULL,
    [saUserGroupCode]         NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([code] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserGroup_Authority] (
    [UserGroup_code]   NVARCHAR (32)  NOT NULL,
    [authorities_code] NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserGroup_code] ASC, [authorities_code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserGroupPermission] (
    [groupCode]  NVARCHAR (32) NOT NULL,
    [shortName]  NVARCHAR (32) NOT NULL,
    [permission] NVARCHAR (32) NOT NULL,
    PRIMARY KEY CLUSTERED ([groupCode] ASC, [shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserGroupPermission_aud] (
    [ver_rev]    NUMERIC (19)   NOT NULL,
    [REVTYPE]    TINYINT        NULL,
    [groupCode]  NVARCHAR (32)  NOT NULL,
    [permission] NVARCHAR (255) NOT NULL,
    [shortName]  NVARCHAR (32)  NOT NULL,
    PRIMARY KEY CLUSTERED ([ver_rev] ASC, [groupCode] ASC, [permission] ASC, [shortName] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserRole] (
    [authority] NVARCHAR (32)  NOT NULL,
    [caption]   NVARCHAR (255) NULL,
    CONSTRAINT [PK__UserRole__37703C52] PRIMARY KEY CLUSTERED ([authority] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[UserRole_AUD] (
    [authority] NVARCHAR (32)  NOT NULL,
    [ver_rev]   NUMERIC (19)   NOT NULL,
    [REVTYPE]   TINYINT        NULL,
    [caption]   NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([authority] ASC, [ver_rev] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[ValidationRule] (
    [id]       INT           IDENTITY (1, 1) NOT NULL,
    [code]     VARCHAR (32)  NOT NULL,
    [name]     VARCHAR (255) NOT NULL,
    [archived] TINYINT       NULL,
    PRIMARY KEY CLUSTERED ([code] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

CREATE TABLE [dbo].[WebAddressUpdateProcess] (
    [id]                  NUMERIC (19) IDENTITY (1, 1) NOT NULL,
    [updated_items_count] NUMERIC (19) NULL,
    [process_status]      VARCHAR (30) NULL,
    [date_start]          DATETIME     NULL,
    [date_end]            DATETIME     NULL,
    PRIMARY KEY CLUSTERED ([id] ASC) WITH (DATA_COMPRESSION = PAGE)
);


GO

ALTER TABLE [DataOpsJobs].[gias_dsi_account_sync]
    ADD CONSTRAINT [DF_gias_dsi_account_sync_DateTime] DEFAULT (getdate()) FOR [SyncDateTime];


GO

ALTER TABLE [DataOpsJobs].[ONSPD_URN_ExclusionList]
    ADD CONSTRAINT [DF_ONSPD_URN_ExclusionList__DateAdded] DEFAULT (getdate()) FOR [DateAdded];


GO

ALTER TABLE [DataOpsJobs].[gias_dsi_account_sync_Log]
    ADD CONSTRAINT [DF_gias_dsi_account_sync_Log_DateTime] DEFAULT (getdate()) FOR [ProcessingDateTime];


GO

ALTER TABLE [FrontEnd].[NotificationTemplates]
    ADD CONSTRAINT [DF_NotificationTemplates_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[GlossaryItems]
    ADD CONSTRAINT [DF_GlossaryItems_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[Tokens]
    ADD CONSTRAINT [DF_Tokens_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[UserPreferences]
    ADD CONSTRAINT [DF_UserPreferences_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[FaqItems]
    ADD CONSTRAINT [DF_FaqItems_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[AZTLoggerMessages]
    ADD CONSTRAINT [DF_AZTLoggerMessages_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[FaqGroups]
    ADD CONSTRAINT [DF_FaqGroups_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[DataQualityStatus]
    ADD CONSTRAINT [DF_DataQualityStatus_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[NewsArticles]
    ADD CONSTRAINT [DF_NewsArticles_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[LocalAuthoritySets]
    ADD CONSTRAINT [DF_LocalAuthoritySets_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[NotificationBanners]
    ADD CONSTRAINT [DF_NotificationBanners_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [FrontEnd].[ApiRecorderSessionItems]
    ADD CONSTRAINT [DF_ApiRecorderSessionItems_Timestamp] DEFAULT (sysutcdatetime()) FOR [Timestamp];


GO

ALTER TABLE [gias_sharing].[establishment_change_history_cache]
    ADD CONSTRAINT [df_establishment_change_history_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_change_history_cache]
    ADD CONSTRAINT [df_establishment_change_history_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[establishment_change_history_cache]
    ADD CONSTRAINT [df_establishment_change_history_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[establishment_change_history_cache]
    ADD CONSTRAINT [df_establishment_change_history_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[staff_record_change_history_cache]
    ADD CONSTRAINT [df_staff_record_change_history_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[staff_record_change_history_cache]
    ADD CONSTRAINT [df_staff_record_change_history_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[staff_record_change_history_cache]
    ADD CONSTRAINT [df_staff_record_change_history_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[staff_record_change_history_cache]
    ADD CONSTRAINT [df_staff_record_change_history_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_group_cache]
    ADD CONSTRAINT [df_establishment_group_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_group_cache]
    ADD CONSTRAINT [df_establishment_group_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_group_cache]
    ADD CONSTRAINT [df_establishment_group_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[establishment_group_cache]
    ADD CONSTRAINT [df_establishment_group_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[staff_record_cache]
    ADD CONSTRAINT [df_staff_record_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[staff_record_cache]
    ADD CONSTRAINT [df_staff_record_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[staff_record_cache]
    ADD CONSTRAINT [df_staff_record_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[staff_record_cache]
    ADD CONSTRAINT [df_staff_record_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_cache]
    ADD CONSTRAINT [df_establishment_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[establishment_cache]
    ADD CONSTRAINT [df_establishment_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[establishment_cache]
    ADD CONSTRAINT [df_establishment_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_cache]
    ADD CONSTRAINT [df_establishment_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[group_link_cache]
    ADD CONSTRAINT [df_group_link_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[group_link_cache]
    ADD CONSTRAINT [df_group_link_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[group_link_cache]
    ADD CONSTRAINT [df_group_link_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[group_link_cache]
    ADD CONSTRAINT [df_group_link_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[public_columns]
    ADD CONSTRAINT [df_public_columns_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[public_columns]
    ADD CONSTRAINT [df_public_columns_row_update_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[public_columns]
    ADD CONSTRAINT [df_public_columns_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[public_columns]
    ADD CONSTRAINT [df_public_columns_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[cache_update_log]
    ADD CONSTRAINT [df_cache_update_log_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[cache_update_log]
    ADD CONSTRAINT [df_cache_update_log_start_time] DEFAULT (getdate()) FOR [start_time];


GO

ALTER TABLE [gias_sharing].[cache_update_log]
    ADD CONSTRAINT [df_cache_update_log_row_update_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[cache_update_log]
    ADD CONSTRAINT [df_cache_update_log_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[cache_update_log]
    ADD CONSTRAINT [df_cache_update_log_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[cache_update_table_log]
    ADD CONSTRAINT [df_cache_update_table_log_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[cache_update_table_log]
    ADD CONSTRAINT [df_cache_update_table_log_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [gias_sharing].[cache_update_table_log]
    ADD CONSTRAINT [df_cache_update_table_log_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[cache_update_table_log]
    ADD CONSTRAINT [df_cache_update_table_log_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_group_change_history_cache]
    ADD CONSTRAINT [df_establishment_group_change_history_cache_row_inserted_datetime] DEFAULT (getdate()) FOR [row_inserted_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_group_change_history_cache]
    ADD CONSTRAINT [df_establishment_group_change_history_cache_row_updated_datetime] DEFAULT (getdate()) FOR [row_updated_datetime];


GO

ALTER TABLE [gias_sharing].[establishment_group_change_history_cache]
    ADD CONSTRAINT [df_establishment_group_change_history_cache_row_inserted_by] DEFAULT (user_name()) FOR [row_inserted_by];


GO

ALTER TABLE [gias_sharing].[establishment_group_change_history_cache]
    ADD CONSTRAINT [df_establishment_group_change_history_cache_row_updated_by] DEFAULT (user_name()) FOR [row_updated_by];


GO

ALTER TABLE [dbo].[EstablishmentGroup]
    ADD CONSTRAINT [DF__Establish__creat__0A14514D] DEFAULT ((0)) FOR [createdInError];


GO

ALTER TABLE [dbo].[FeatureFlags]
    ADD CONSTRAINT [DF_FeatureFlags_UpdatedAt] DEFAULT (sysutcdatetime()) FOR [updatedAt];


GO

ALTER TABLE [dbo].[FeatureFlags]
    ADD CONSTRAINT [DF_FeatureFlags_Enabled] DEFAULT ((0)) FOR [enabled];


GO

ALTER TABLE [dbo].[FieldStatistics]
    ADD CONSTRAINT [DF__FieldStat__updat__52793849] DEFAULT ('NEVER') FOR [updateFrequency];


GO

ALTER TABLE [dbo].[StaffRecord]
    ADD CONSTRAINT [DF_StaffRecord_deleted] DEFAULT ((0)) FOR [deleted];


GO

ALTER TABLE [DataOpsJobs].[gias_dsi_account_sync_Log] WITH NOCHECK
    ADD CONSTRAINT [FK_gias_dsi_account_sync_Log_SyncId_gias_dsi_account_sync_SyncId] FOREIGN KEY ([SyncId]) REFERENCES [DataOpsJobs].[gias_dsi_account_sync] ([SyncId]);


GO

ALTER TABLE [gias_sharing].[establishment_change_history_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_establishment_change_history_cache__EstablishmentChangeHistory] FOREIGN KEY ([id]) REFERENCES [dbo].[EstablishmentChangeHistory] ([id]);


GO

ALTER TABLE [gias_sharing].[staff_record_change_history_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_staff_record_change_history_cache__StaffChangeRequest] FOREIGN KEY ([id]) REFERENCES [dbo].[StaffChangeRequest] ([id]);


GO

ALTER TABLE [gias_sharing].[establishment_group_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_establishment_group_cache__EstablishmentGroup] FOREIGN KEY ([gid]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [gias_sharing].[staff_record_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_staff_record_cache__StaffRecord] FOREIGN KEY ([uid]) REFERENCES [dbo].[StaffRecord] ([uid]);


GO

ALTER TABLE [gias_sharing].[establishment_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_establishment_cache__establishment] FOREIGN KEY ([urn]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [gias_sharing].[group_link_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_group_link_cache__establishment_cache] FOREIGN KEY ([urn]) REFERENCES [gias_sharing].[establishment_cache] ([urn]);


GO

ALTER TABLE [gias_sharing].[group_link_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_group_link_cache__establishment_group_cache] FOREIGN KEY ([gid]) REFERENCES [gias_sharing].[establishment_group_cache] ([gid]);


GO

ALTER TABLE [gias_sharing].[group_link_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_group_link_cache__GroupLink] FOREIGN KEY ([id]) REFERENCES [dbo].[GroupLink] ([id]);


GO

ALTER TABLE [gias_sharing].[cache_update_table_log] WITH NOCHECK
    ADD CONSTRAINT [FK_cache_update_table_log_cache_update_log_id_cache_update_log_cache_update_log_id] FOREIGN KEY ([cache_update_log_id]) REFERENCES [gias_sharing].[cache_update_log] ([cache_update_log_id]);


GO

ALTER TABLE [gias_sharing].[establishment_group_change_history_cache] WITH NOCHECK
    ADD CONSTRAINT [FK_establishment_group_change_history_cache__GroupChangeRequest] FOREIGN KEY ([id]) REFERENCES [dbo].[GroupChangeRequest] ([id]);


GO

ALTER TABLE [dbo].[AdditionalDatasetOrder] WITH NOCHECK
    ADD CONSTRAINT [FK_AdditionalDatasetOrder_subscription_order_id_SubscriptionOrder_id] FOREIGN KEY ([subscription_order_id]) REFERENCES [dbo].[SubscriptionOrder] ([id]);


GO

ALTER TABLE [dbo].[AdditionalDatasetOrder] WITH NOCHECK
    ADD CONSTRAINT [FK_AdditionalDatasetOrder_datasetName_code_DatasetName_code] FOREIGN KEY ([datasetName_code]) REFERENCES [dbo].[DatasetName] ([code]);


GO

ALTER TABLE [dbo].[AnalysisCriterion] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisCriterion_report_id_AnalysisReport_id] FOREIGN KEY ([report_id]) REFERENCES [dbo].[AnalysisReport] ([id]);


GO

ALTER TABLE [dbo].[AnalysisCriterionValue] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisCriterionValue_analysisCriterion_id_AnalysisCriterion_id] FOREIGN KEY ([analysisCriterion_id]) REFERENCES [dbo].[AnalysisCriterion] ([id]);


GO

ALTER TABLE [dbo].[AnalysisReport] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisReport_creatorUser_username_SystemUser_username] FOREIGN KEY ([creatorUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[AnalysisReport] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisReport_filter_id_EstablishmentFilter_id] FOREIGN KEY ([filter_id]) REFERENCES [dbo].[EstablishmentFilter] ([id]);


GO

ALTER TABLE [dbo].[AnalysisReport] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisReport_lastModifiedUser_username_SystemUser_username] FOREIGN KEY ([lastModifiedUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[AnalysisReport] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisReport_predefinedReportCategory_code_PredefinedReportCategory_code] FOREIGN KEY ([predefinedReportCategory_code]) REFERENCES [dbo].[PredefinedReportCategory] ([code]);


GO

ALTER TABLE [dbo].[AnalysisReportToMeasureType] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisReportToMeasureType_report_id_AnalysisReport_id] FOREIGN KEY ([report_id]) REFERENCES [dbo].[AnalysisReport] ([id]);


GO

ALTER TABLE [dbo].[AnalysisReportToMeasureType] WITH NOCHECK
    ADD CONSTRAINT [FK_AnalysisReportToMeasureType_measure_code_MeasureType_code] FOREIGN KEY ([measure_code]) REFERENCES [dbo].[MeasureType] ([code]);


GO

ALTER TABLE [dbo].[AnnouncementCollection] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementCollection_template_id_AnnouncementTemplate_id] FOREIGN KEY ([template_id]) REFERENCES [dbo].[AnnouncementTemplate] ([id]);


GO

ALTER TABLE [dbo].[AnnouncementCollectionValues] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementCollectionValues_announcementId_AnnouncementCollection_id] FOREIGN KEY ([announcementId]) REFERENCES [dbo].[AnnouncementCollection] ([id]);


GO

ALTER TABLE [dbo].[AnnouncementTemplate] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementTemplate_announcementAction_code_EduBaseTrigger_code] FOREIGN KEY ([announcementAction_code]) REFERENCES [dbo].[EduBaseTrigger] ([code]);


GO

ALTER TABLE [dbo].[AnnouncementTemplate_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementTemplate_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[AnnouncementTemplate_UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementTemplate_UserGroup_groups_code_UserGroup_code] FOREIGN KEY ([groups_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[AnnouncementTemplate_UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementTemplate_UserGroup_AnnouncementTemplate_id_AnnouncementTemplate_id] FOREIGN KEY ([AnnouncementTemplate_id]) REFERENCES [dbo].[AnnouncementTemplate] ([id]);


GO

ALTER TABLE [dbo].[AnnouncementTemplate_UserGroup_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_AnnouncementTemplate_UserGroup_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[AutomatedEmailTemplate] WITH NOCHECK
    ADD CONSTRAINT [FK_AutomatedEmailTemplate_signOffUser_username_SystemUser_username] FOREIGN KEY ([signOffUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[AutomatedEmailTemplate_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_AutomatedEmailTemplate_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[BATCH_JOB_EXECUTION] WITH NOCHECK
    ADD CONSTRAINT [JOB_INST_EXEC_FK] FOREIGN KEY ([JOB_INSTANCE_ID]) REFERENCES [dbo].[BATCH_JOB_INSTANCE] ([JOB_INSTANCE_ID]);


GO

ALTER TABLE [dbo].[BATCH_JOB_EXECUTION_CONTEXT] WITH NOCHECK
    ADD CONSTRAINT [JOB_EXEC_CTX_FK] FOREIGN KEY ([JOB_EXECUTION_ID]) REFERENCES [dbo].[BATCH_JOB_EXECUTION] ([JOB_EXECUTION_ID]);


GO

ALTER TABLE [dbo].[BATCH_JOB_EXECUTION_PARAMS] WITH NOCHECK
    ADD CONSTRAINT [JOB_EXEC_PARAMS_FK] FOREIGN KEY ([JOB_EXECUTION_ID]) REFERENCES [dbo].[BATCH_JOB_EXECUTION] ([JOB_EXECUTION_ID]);


GO

ALTER TABLE [dbo].[BATCH_STEP_EXECUTION] WITH NOCHECK
    ADD CONSTRAINT [JOB_EXEC_STEP_FK] FOREIGN KEY ([JOB_EXECUTION_ID]) REFERENCES [dbo].[BATCH_JOB_EXECUTION] ([JOB_EXECUTION_ID]);


GO

ALTER TABLE [dbo].[BATCH_STEP_EXECUTION_CONTEXT] WITH NOCHECK
    ADD CONSTRAINT [STEP_EXEC_CTX_FK] FOREIGN KEY ([STEP_EXECUTION_ID]) REFERENCES [dbo].[BATCH_STEP_EXECUTION] ([STEP_EXECUTION_ID]);


GO

ALTER TABLE [dbo].[BulkUpdate] WITH NOCHECK
    ADD CONSTRAINT [FK_BulkUpdate_user_username_SystemUser_username] FOREIGN KEY ([user_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[BulkUpdate] WITH NOCHECK
    ADD CONSTRAINT [FK_BulkUpdate_asUser_username_SystemUser_username] FOREIGN KEY ([asUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[BulkUpdateMessage] WITH NOCHECK
    ADD CONSTRAINT [FK_BulkUpdateMessage_bulkUpdate_id_BulkUpdate_id] FOREIGN KEY ([bulkUpdate_id]) REFERENCES [dbo].[BulkUpdate] ([id]);


GO

ALTER TABLE [dbo].[BulkUpdateStatus] WITH NOCHECK
    ADD CONSTRAINT [FK_BulkUpdateStatus_id_BulkUpdate_id] FOREIGN KEY ([id]) REFERENCES [dbo].[BulkUpdate] ([id]);


GO

ALTER TABLE [dbo].[ByTypeFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ByTypeFieldPermission_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[ByTypeFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ByTypeFieldPermission_userGroup_code_UserGroup_code] FOREIGN KEY ([userGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[ByTypeFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ByTypeFieldPermission_type_EstablishmentType_code] FOREIGN KEY ([type]) REFERENCES [dbo].[EstablishmentType] ([code]);


GO

ALTER TABLE [dbo].[Callback] WITH NOCHECK
    ADD CONSTRAINT [FK_Callback_creatorGroup_UserGroup_code] FOREIGN KEY ([creatorGroup]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[Callback] WITH NOCHECK
    ADD CONSTRAINT [FK_Callback_parent_id_Callback_id] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[Callback] ([id]);


GO

ALTER TABLE [dbo].[CallbackHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_CallbackHistory_scheduled_extract_id_ScheduledExtract_id] FOREIGN KEY ([scheduled_extract_id]) REFERENCES [dbo].[ScheduledExtract] ([id]);


GO

ALTER TABLE [dbo].[CallbackHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_CallbackHistory_creatorGroup_UserGroup_code] FOREIGN KEY ([creatorGroup]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[ChangeHistoryNewValues] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeHistoryNewValues_EstablishmentChangeHistory_id_EstablishmentChangeHistory_id] FOREIGN KEY ([EstablishmentChangeHistory_id]) REFERENCES [dbo].[EstablishmentChangeHistory] ([id]);


GO

ALTER TABLE [dbo].[ChangeHistoryNewValues_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeHistoryNewValues_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ChangeHistoryOldValues] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeHistoryOldValues_EstablishmentChangeHistory_id_EstablishmentChangeHistory_id] FOREIGN KEY ([EstablishmentChangeHistory_id]) REFERENCES [dbo].[EstablishmentChangeHistory] ([id]);


GO

ALTER TABLE [dbo].[ChangeHistoryOldValues_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeHistoryOldValues_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ChangeRequestMailingHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeRequestMailingHistory_addressee_username_SystemUser_username] FOREIGN KEY ([addressee_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[ChangeRequestMailingHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeRequestMailingHistory_addressee_id_ChangeRequestProposer_id] FOREIGN KEY ([addressee_id]) REFERENCES [dbo].[ChangeRequestProposer] ([id]);


GO

ALTER TABLE [dbo].[ChangeRequestMailingHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeRequestMailingHistory_changeRequest_id_EstablishmentChangeHistory_id] FOREIGN KEY ([changeRequest_id]) REFERENCES [dbo].[EstablishmentChangeHistory] ([id]);


GO

ALTER TABLE [dbo].[ChangeRequestProposer_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ChangeRequestProposer_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ContentDocument_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ContentDocument_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[DataDictionary_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_DataDictionary_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[DataDictionary_EstablishmentField] WITH NOCHECK
    ADD CONSTRAINT [FK_DataDictionary_EstablishmentField_dependencies_shortName_EstablishmentField_shortName] FOREIGN KEY ([dependencies_shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[DataDictionary_EstablishmentField] WITH NOCHECK
    ADD CONSTRAINT [FK_DataDictionary_EstablishmentField_DataDictionary_shortName_DataDictionary_shortName] FOREIGN KEY ([DataDictionary_shortName]) REFERENCES [dbo].[DataDictionary] ([shortName]);


GO

ALTER TABLE [dbo].[DataDictionary_EstablishmentField_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_DataDictionary_EstablishmentField_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[DistrictAdministrative_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_DistrictAdministrative_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Document] WITH NOCHECK
    ADD CONSTRAINT [FK_Document_section_DocumentationSection_code] FOREIGN KEY ([section]) REFERENCES [dbo].[DocumentationSection] ([code]);


GO

ALTER TABLE [dbo].[Document] WITH NOCHECK
    ADD CONSTRAINT [FK_Document_createdBy_username_SystemUser_username] FOREIGN KEY ([createdBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[Document_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Document_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Document_UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_Document_UserGroup_Document_id_Document_id] FOREIGN KEY ([Document_id]) REFERENCES [dbo].[Document] ([id]);


GO

ALTER TABLE [dbo].[Document_UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_Document_UserGroup_groups_code_UserGroup_code] FOREIGN KEY ([groups_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[Document_UserGroup_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Document_UserGroup_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[DocumentationSection_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_DocumentationSection_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EdubaseNote_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EdubaseNote_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EdubaseTriggerSettings] WITH NOCHECK
    ADD CONSTRAINT [FK_EdubaseTriggerSettings_saAccessRestrictions_code_SAAccessRestrictions_code] FOREIGN KEY ([saAccessRestrictions_code]) REFERENCES [dbo].[SAAccessRestrictions] ([code]);


GO

ALTER TABLE [dbo].[EdubaseTriggerSettings_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EdubaseTriggerSettings_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EducationPhase] WITH NOCHECK
    ADD CONSTRAINT [FK_EducationPhase_group_code_EducationPhaseGroup_code] FOREIGN KEY ([group_code]) REFERENCES [dbo].[EducationPhaseGroup] ([code]);


GO

ALTER TABLE [dbo].[EmailAttachment] WITH NOCHECK
    ADD CONSTRAINT [FK_EmailAttachment_email_id_InboxMessage_id] FOREIGN KEY ([email_id]) REFERENCES [dbo].[InboxMessage] ([id]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_reasonEstablishmentClosed_code_ReasonEstablishmentClosed_code] FOREIGN KEY ([reasonEstablishmentClosed_code]) REFERENCES [dbo].[ReasonEstablishmentClosed] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_disadvantagedArea_code_DisadvantagedArea_code] FOREIGN KEY ([disadvantagedArea_code]) REFERENCES [dbo].[DisadvantagedArea] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_GssLaCode_code_GSSLACode_code] FOREIGN KEY ([GssLaCode_code]) REFERENCES [dbo].[GSSLACode] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_burnhamReport_code_BurnhamReport_code] FOREIGN KEY ([burnhamReport_code]) REFERENCES [dbo].[BurnhamReport] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConRSCRegion_code_RSCRegion_code] FOREIGN KEY ([ConRSCRegion_code]) REFERENCES [dbo].[RSCRegion] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_movedToEdubaseLogin_code_MovedToEdubaseLogin_code] FOREIGN KEY ([movedToEdubaseLogin_code]) REFERENCES [dbo].[MovedToEdubaseLogin] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_boarders_code_Boarders_code] FOREIGN KEY ([boarders_code]) REFERENCES [dbo].[Boarders] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_emotionalAndBehaviouralDifficulties_code_ProvisionEbd_code] FOREIGN KEY ([emotionalAndBehaviouralDifficulties_code]) REFERENCES [dbo].[ProvisionEbd] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_studioSchoolIndicator_code_StudioSchoolIndicator_code] FOREIGN KEY ([studioSchoolIndicator_code]) REFERENCES [dbo].[StudioSchoolIndicator] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_CasWard_code_CasWard_code] FOREIGN KEY ([CasWard_code]) REFERENCES [dbo].[CasWard] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_specialClasses_code_SpecialClasses_code] FOREIGN KEY ([specialClasses_code]) REFERENCES [dbo].[SpecialClasses] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_trainingEstablishment_code_TrainingSchool_code] FOREIGN KEY ([trainingEstablishment_code]) REFERENCES [dbo].[TrainingSchool] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_learningSupportUnit_code_LearningSupportUnit_code] FOREIGN KEY ([learningSupportUnit_code]) REFERENCES [dbo].[LearningSupportUnit] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_furtherEducationType_code_FurtherEducationType_code] FOREIGN KEY ([furtherEducationType_code]) REFERENCES [dbo].[FurtherEducationType] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ofstedRating_code_OfstedRating_code] FOREIGN KEY ([ofstedRating_code]) REFERENCES [dbo].[OfstedRating] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_LA_code_LocalAuthority_code] FOREIGN KEY ([LA_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_directProvisionOfEarlyYears_code_DirectProvisionOfEarlyYears_code] FOREIGN KEY ([directProvisionOfEarlyYears_code]) REFERENCES [dbo].[DirectProvisionOfEarlyYears] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_religiousCharacter_code_ReligiousCharacter_code] FOREIGN KEY ([religiousCharacter_code]) REFERENCES [dbo].[ReligiousCharacter] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ftProvision_code_FullTimeProvision_code] FOREIGN KEY ([ftProvision_code]) REFERENCES [dbo].[FullTimeProvision] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_UrbanRural_code_UrbanRural_code] FOREIGN KEY ([UrbanRural_code]) REFERENCES [dbo].[UrbanRural] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_admissionsPolicy_code_AdmissionsPolicy_code] FOREIGN KEY ([admissionsPolicy_code]) REFERENCES [dbo].[AdmissionsPolicy] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_parliamentaryConstituency_code_ParliamentaryConstituency_code] FOREIGN KEY ([parliamentaryConstituency_code]) REFERENCES [dbo].[ParliamentaryConstituency] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_beacon_code_Beacon_code] FOREIGN KEY ([beacon_code]) REFERENCES [dbo].[Beacon] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_excellenceInCitiesGroup_code_ExcellenceInCitiesGroup_code] FOREIGN KEY ([excellenceInCitiesGroup_code]) REFERENCES [dbo].[ExcellenceInCitiesGroup] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConPreviousLA_code_LocalAuthority_code] FOREIGN KEY ([ConPreviousLA_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_msoa_code_MSOA_code] FOREIGN KEY ([msoa_code]) REFERENCES [dbo].[MSOA] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_contactType_code_ContactType_code] FOREIGN KEY ([contactType_code]) REFERENCES [dbo].[ContactType] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_gender_code_Gender_code] FOREIGN KEY ([gender_code]) REFERENCES [dbo].[Gender] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_HeadTitle_code_Title_code] FOREIGN KEY ([HeadTitle_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_childCareFacilities_code_ChildCareFacilities_code] FOREIGN KEY ([childCareFacilities_code]) REFERENCES [dbo].[ChildCareFacilities] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_leadingOption1_code_HighPerformanceOption_code] FOREIGN KEY ([leadingOption1_code]) REFERENCES [dbo].[HighPerformanceOption] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConTitle_code_Title_code] FOREIGN KEY ([ConTitle_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ofstedSpecialMeasures_code_OfstedSpecialMeasures_code] FOREIGN KEY ([ofstedSpecialMeasures_code]) REFERENCES [dbo].[OfstedSpecialMeasures] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_earlyExcellence_code_EarlyExcellence_code] FOREIGN KEY ([earlyExcellence_code]) REFERENCES [dbo].[EarlyExcellence] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_firstMainSpecialism_code_Specialism_code] FOREIGN KEY ([firstMainSpecialism_code]) REFERENCES [dbo].[Specialism] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_LLSC_code_LLSC_code] FOREIGN KEY ([LLSC_code]) REFERENCES [dbo].[LLSC] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_reasonEstablishmentOpened_code_ReasonEstablishmentOpened_code] FOREIGN KEY ([reasonEstablishmentOpened_code]) REFERENCES [dbo].[ReasonEstablishmentOpened] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_childrensCentresPhaseType_code_ChildrensCentresPhaseType_code] FOREIGN KEY ([childrensCentresPhaseType_code]) REFERENCES [dbo].[ChildrensCentresPhaseType] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_changeReason_code_ChangeReason_code] FOREIGN KEY ([changeReason_code]) REFERENCES [dbo].[ChangeReason] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_type_code_EstablishmentType_code] FOREIGN KEY ([type_code]) REFERENCES [dbo].[EstablishmentType] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_establishmentAccredited_code_EstablishmentAccredited_code] FOREIGN KEY ([establishmentAccredited_code]) REFERENCES [dbo].[EstablishmentAccredited] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_PreviousLA_code_LocalAuthority_code] FOREIGN KEY ([PreviousLA_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_districtAdministrative_code_DistrictAdministrative_code] FOREIGN KEY ([districtAdministrative_code]) REFERENCES [dbo].[DistrictAdministrative] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_secondMainSpecialism_code_SecondMainSpecialism_code] FOREIGN KEY ([secondMainSpecialism_code]) REFERENCES [dbo].[SecondMainSpecialism] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_contactPreference_code_ContactPreference_code] FOREIGN KEY ([contactPreference_code]) REFERENCES [dbo].[ContactPreference] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_leadershipIncentive_code_LeadershipIncentive_code] FOREIGN KEY ([leadershipIncentive_code]) REFERENCES [dbo].[LeadershipIncentive] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_eyfsExemption_code_EYFSExemption_code] FOREIGN KEY ([eyfsExemption_code]) REFERENCES [dbo].[EYFSExemption] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_religiousEthos_code_ReligiousEthos_code] FOREIGN KEY ([religiousEthos_code]) REFERENCES [dbo].[ReligiousEthos] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_CHCounty_code_County_code] FOREIGN KEY ([CHCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_lsoa_code_LSOA_code] FOREIGN KEY ([lsoa_code]) REFERENCES [dbo].[LSOA] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConCounty_code_County_code] FOREIGN KEY ([ConCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_firstSecondarySpecialism_code_SecondSpecialism_code] FOREIGN KEY ([firstSecondarySpecialism_code]) REFERENCES [dbo].[SecondSpecialism] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_RSCRegion_code_RSCRegion_code] FOREIGN KEY ([RSCRegion_code]) REFERENCES [dbo].[RSCRegion] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_typeOfReservedProvision_code_TypeOfReservedProvision_code] FOREIGN KEY ([typeOfReservedProvision_code]) REFERENCES [dbo].[TypeOfReservedProvision] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_leadingOption2_code_HighPerformanceOption_code] FOREIGN KEY ([leadingOption2_code]) REFERENCES [dbo].[HighPerformanceOption] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_Country_code_Nationality_code] FOREIGN KEY ([Country_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_nurseryProvision_code_NurseryProvision_code] FOREIGN KEY ([nurseryProvision_code]) REFERENCES [dbo].[NurseryProvision] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_educationActionZone_code_EducationActionZone_code] FOREIGN KEY ([educationActionZone_code]) REFERENCES [dbo].[EducationActionZone] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_superannuationCategory_code_SuperannuationCategory_code] FOREIGN KEY ([superannuationCategory_code]) REFERENCES [dbo].[SuperannuationCategory] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_specialEducationalNeeds_code_ProvisionSen_code] FOREIGN KEY ([specialEducationalNeeds_code]) REFERENCES [dbo].[ProvisionSen] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConCasWard_code_CasWard_code] FOREIGN KEY ([ConCasWard_code]) REFERENCES [dbo].[CasWard] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConCountry_code_Nationality_code] FOREIGN KEY ([ConCountry_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_excellenceInCities_code_ExcellenceInCities_code] FOREIGN KEY ([excellenceInCities_code]) REFERENCES [dbo].[ExcellenceInCities] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_inspectorateName_code_InspectorateName_code] FOREIGN KEY ([inspectorateName_code]) REFERENCES [dbo].[InspectorateName] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_GOR_code_GovernmentOfficeRegion_code] FOREIGN KEY ([GOR_code]) REFERENCES [dbo].[GovernmentOfficeRegion] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_eduBaseTrigger1_code_EduBaseTrigger_code] FOREIGN KEY ([eduBaseTrigger1_code]) REFERENCES [dbo].[EduBaseTrigger] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_trustSchool_code_TrustSchool_code] FOREIGN KEY ([trustSchool_code]) REFERENCES [dbo].[TrustSchool] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_secondSecondarySpecialism_code_SecondSpecialism_code] FOREIGN KEY ([secondSecondarySpecialism_code]) REFERENCES [dbo].[SecondSpecialism] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_privateFinanceInitiative_code_PrivateFinanceInitiative_code] FOREIGN KEY ([privateFinanceInitiative_code]) REFERENCES [dbo].[PrivateFinanceInitiative] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConUrbanRural_code_UrbanRural_code] FOREIGN KEY ([ConUrbanRural_code]) REFERENCES [dbo].[UrbanRural] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConLA_code_LocalAuthority_code] FOREIGN KEY ([ConLA_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_section41Approved_code_Section41Approved_code] FOREIGN KEY ([section41Approved_code]) REFERENCES [dbo].[Section41Approved] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_excellenceInCitiesCityLearningCentre_code_ExcellenceInCitiesCityLearningCentre_code] FOREIGN KEY ([excellenceInCitiesCityLearningCentre_code]) REFERENCES [dbo].[ExcellenceInCitiesCityLearningCentre] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_diocese_code_Diocese_code] FOREIGN KEY ([diocese_code]) REFERENCES [dbo].[Diocese] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_MarketingOptInOut_code_MarketingOptOut_code] FOREIGN KEY ([MarketingOptInOut_code]) REFERENCES [dbo].[MarketingOptOut] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_status_code_EstablishmentStatus_code] FOREIGN KEY ([status_code]) REFERENCES [dbo].[EstablishmentStatus] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_teenagedMothers_code_TeenagedMothers_code] FOREIGN KEY ([teenagedMothers_code]) REFERENCES [dbo].[TeenagedMothers] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_governorsFlag_code_GovernorsFlag_code] FOREIGN KEY ([governorsFlag_code]) REFERENCES [dbo].[GovernorsFlag] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_administrativeWard_code_AdministrativeWard_code] FOREIGN KEY ([administrativeWard_code]) REFERENCES [dbo].[AdministrativeWard] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConLLSC_code_LLSC_code] FOREIGN KEY ([ConLLSC_code]) REFERENCES [dbo].[LLSC] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_educationPhase_code_EducationPhase_code] FOREIGN KEY ([educationPhase_code]) REFERENCES [dbo].[EducationPhase] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_officialSixthForm_code_OfficialSixthForm_code] FOREIGN KEY ([officialSixthForm_code]) REFERENCES [dbo].[OfficialSixthForm] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ttwa_code_Ttwa_code] FOREIGN KEY ([ttwa_code]) REFERENCES [dbo].[Ttwa] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_s2sLogin_code_S2SLogin_code] FOREIGN KEY ([s2sLogin_code]) REFERENCES [dbo].[S2SLogin] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_leadingOption3_code_HighPerformanceOption_code] FOREIGN KEY ([leadingOption3_code]) REFERENCES [dbo].[HighPerformanceOption] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConGOR_code_GovernmentOfficeRegion_code] FOREIGN KEY ([ConGOR_code]) REFERENCES [dbo].[GovernmentOfficeRegion] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_registeredEarlyYears_code_RegisteredEarlyYears_code] FOREIGN KEY ([registeredEarlyYears_code]) REFERENCES [dbo].[RegisteredEarlyYears] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_operationalHours_code_OperationalHours_code] FOREIGN KEY ([operationalHours_code]) REFERENCES [dbo].[OperationalHours] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_excellenceInCitiesActionZone_code_ExcellenceInCitiesActionZone_code] FOREIGN KEY ([excellenceInCitiesActionZone_code]) REFERENCES [dbo].[ExcellenceInCitiesActionZone] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_governance_code_Governance_code] FOREIGN KEY ([governance_code]) REFERENCES [dbo].[Governance] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_educationByOthers_code_EducationByOthers_code] FOREIGN KEY ([educationByOthers_code]) REFERENCES [dbo].[EducationByOthers] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_qualityAssuranceBodyName_code_QualityAssuranceBodyName_code] FOREIGN KEY ([qualityAssuranceBodyName_code]) REFERENCES [dbo].[QualityAssuranceBodyName] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_investorInPeople_code_InvestorInPeople_code] FOREIGN KEY ([investorInPeople_code]) REFERENCES [dbo].[InvestorInPeople] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_County_code_County_code] FOREIGN KEY ([County_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_freshStart_code_FreshStart_code] FOREIGN KEY ([freshStart_code]) REFERENCES [dbo].[FreshStart] ([code]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_ConGssLaCode_code_GSSLACode_code] FOREIGN KEY ([ConGssLaCode_code]) REFERENCES [dbo].[GSSLACode] ([code]);


GO

ALTER TABLE [dbo].[Establishment_AdditionalAddress_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_AdditionalAddress_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Establishment_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Establishment_EstablishmentLink_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Establishment_EstablishmentLink_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentAdditionalAddresses] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentAdditionalAddresses_county_code_County_code] FOREIGN KEY ([county_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentAdditionalAddresses] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentAdditionalAddresses_urn_Establishment_URN] FOREIGN KEY ([urn]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentAdditionalAddresses] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentAdditionalAddresses_country_code_Nationality_code] FOREIGN KEY ([country_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentAdditionalAddresses_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentAdditionalAddresses_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_approvedOrRejectedByUserGroup_code_UserGroup_code] FOREIGN KEY ([approvedOrRejectedByUserGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_proposedByUser_username_SystemUser_username] FOREIGN KEY ([proposedByUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_dataOwner_code_UserGroup_code] FOREIGN KEY ([dataOwner_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_proposer_id_ChangeRequestProposer_id] FOREIGN KEY ([proposer_id]) REFERENCES [dbo].[ChangeRequestProposer] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_establishment_URN_Establishment_URN] FOREIGN KEY ([establishment_URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_proposedByUserGroup_code_UserGroup_code] FOREIGN KEY ([proposedByUserGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_field_shortName_EstablishmentField_shortName] FOREIGN KEY ([field_shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_approvedOrRejectedBy_username_SystemUser_username] FOREIGN KEY ([approvedOrRejectedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[EstablishmentChangeHistory_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentChangeHistory_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentField] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentField_sen_code_Sen_code] FOREIGN KEY ([sen_code]) REFERENCES [dbo].[Sen] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentField] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentField_toeGroup_code_ToeFieldGroup_code] FOREIGN KEY ([toeGroup_code]) REFERENCES [dbo].[ToeFieldGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentField] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentField_group_code_FieldGroup_code] FOREIGN KEY ([group_code]) REFERENCES [dbo].[FieldGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentField_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentField_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentFieldBase_ByTypeFieldPermission_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldBase_ByTypeFieldPermission_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentFieldBase_EstablishmentFieldToTypeMapping_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldBase_EstablishmentFieldToTypeMapping_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentFieldBase_FieldPosition_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldBase_FieldPosition_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentFieldToTypeMapping] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldToTypeMapping_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[EstablishmentFieldToTypeMapping] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldToTypeMapping_type_EstablishmentType_code] FOREIGN KEY ([type]) REFERENCES [dbo].[EstablishmentType] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentFieldValidation] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldValidation_shortName_EstablishmentField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[EstablishmentFieldValidation] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFieldValidation_validationRuleCode_ValidationRule_code] FOREIGN KEY ([validationRuleCode]) REFERENCES [dbo].[ValidationRule] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentFilter] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFilter_bringUpField_EstablishmentField_shortName] FOREIGN KEY ([bringUpField]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[EstablishmentFilter] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentFilter_creatorGroup_code_UserGroup_code] FOREIGN KEY ([creatorGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_type_code_EstablishmentGroupType_code] FOREIGN KEY ([type_code]) REFERENCES [dbo].[EstablishmentGroupType] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_GroupContactTitle_code_Title_code] FOREIGN KEY ([GroupContactTitle_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_GroupContactCountry_code_Nationality_code] FOREIGN KEY ([GroupContactCountry_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_HeadTitle_code_Title_code] FOREIGN KEY ([HeadTitle_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_GroupCountry_code_Nationality_code] FOREIGN KEY ([GroupCountry_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_GroupContactCounty_code_County_code] FOREIGN KEY ([GroupContactCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_GroupCounty_code_County_code] FOREIGN KEY ([GroupCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_localAuthority_code_LocalAuthority_code] FOREIGN KEY ([localAuthority_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_groupContactType_code_ContactType_code] FOREIGN KEY ([groupContactType_code]) REFERENCES [dbo].[ContactType] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentGroup_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroup_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentGroupType_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentGroupType_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentLink] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentLink_type_code_LinkType_code] FOREIGN KEY ([type_code]) REFERENCES [dbo].[LinkType] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentLink] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentLink_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentLink] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentLink_linkURN_Establishment_URN] FOREIGN KEY ([linkURN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentLink_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentLink_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentPartnership] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentPartnership_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentPartnership_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentPartnership_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentProprietors] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentProprietors_urn_IndependentSchools_URN] FOREIGN KEY ([urn]) REFERENCES [dbo].[IndependentSchools] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentProprietors] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentProprietors_county_code_County_code] FOREIGN KEY ([county_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentProprietors_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentProprietors_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentSixthFormSchool] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentSixthFormSchool_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentSixthFormSchool_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentSixthFormSchool_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentStatus_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentStatus_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentToFederation] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToFederation_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentToFederation] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToFederation_federationCode_Federation_code] FOREIGN KEY ([federationCode]) REFERENCES [dbo].[Federation] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentToFederation_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToFederation_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentToSen1] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen1_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentToSen1] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen1_senCode_Sen_code] FOREIGN KEY ([senCode]) REFERENCES [dbo].[Sen] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentToSen1_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen1_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentToSen2] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen2_senCode_Sen_code] FOREIGN KEY ([senCode]) REFERENCES [dbo].[Sen] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentToSen2] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen2_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentToSen2_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen2_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentToSen3] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen3_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentToSen3] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen3_senCode_Sen_code] FOREIGN KEY ([senCode]) REFERENCES [dbo].[Sen] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentToSen3_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen3_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentToSen4] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen4_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[EstablishmentToSen4] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen4_senCode_Sen_code] FOREIGN KEY ([senCode]) REFERENCES [dbo].[Sen] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentToSen4_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentToSen4_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[EstablishmentType] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentType_group_code_EstablishmentTypeGroup_code] FOREIGN KEY ([group_code]) REFERENCES [dbo].[EstablishmentTypeGroup] ([code]);


GO

ALTER TABLE [dbo].[EstablishmentType_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_EstablishmentType_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ExtractFieldWithParams] WITH NOCHECK
    ADD CONSTRAINT [FK_ExtractFieldWithParams_scheduledExtract_id_ScheduledExtract_id] FOREIGN KEY ([scheduledExtract_id]) REFERENCES [dbo].[ScheduledExtract] ([id]);


GO

ALTER TABLE [dbo].[ExtractFieldWithParams] WITH NOCHECK
    ADD CONSTRAINT [FK_ExtractFieldWithParams_field_shortName_EstablishmentField_shortName] FOREIGN KEY ([field_shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[ExtractionLog] WITH NOCHECK
    ADD CONSTRAINT [FK_ExtractionLog_user_username_SystemUser_username] FOREIGN KEY ([user_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[FAQ] WITH NOCHECK
    ADD CONSTRAINT [FK_FAQ_subject_code_FAQSubject_code] FOREIGN KEY ([subject_code]) REFERENCES [dbo].[FAQSubject] ([code]);


GO

ALTER TABLE [dbo].[FAQ] WITH NOCHECK
    ADD CONSTRAINT [FK_FAQ_createdBy_username_SystemUser_username] FOREIGN KEY ([createdBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[FAQ] WITH NOCHECK
    ADD CONSTRAINT [FK_FAQ_signOffUser_username_SystemUser_username] FOREIGN KEY ([signOffUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[FAQ_FAQGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_FAQ_FAQGroup_FAQ_id_FAQ_id] FOREIGN KEY ([FAQ_id]) REFERENCES [dbo].[FAQ] ([id]);


GO

ALTER TABLE [dbo].[FAQ_FAQGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_FAQ_FAQGroup_groups_code_FAQGroup_code] FOREIGN KEY ([groups_code]) REFERENCES [dbo].[FAQGroup] ([code]);


GO

ALTER TABLE [dbo].[Favourite] WITH NOCHECK
    ADD CONSTRAINT [FK_Favourite_user_username_SystemUser_username] FOREIGN KEY ([user_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[Favourite] WITH NOCHECK
    ADD CONSTRAINT [FK_Favourite_filter_id_EstablishmentFilter_id] FOREIGN KEY ([filter_id]) REFERENCES [dbo].[EstablishmentFilter] ([id]);


GO

ALTER TABLE [dbo].[Favourite] WITH NOCHECK
    ADD CONSTRAINT [FK_Favourite_extract_id_ScheduledExtract_id] FOREIGN KEY ([extract_id]) REFERENCES [dbo].[ScheduledExtract] ([id]);


GO

ALTER TABLE [dbo].[FieldCriterion] WITH NOCHECK
    ADD CONSTRAINT [FK_FieldCriterion_filter_id_EstablishmentFilter_id] FOREIGN KEY ([filter_id]) REFERENCES [dbo].[EstablishmentFilter] ([id]);


GO

ALTER TABLE [dbo].[FieldCriterion] WITH NOCHECK
    ADD CONSTRAINT [FK_FieldCriterion_field_shortName_EstablishmentField_shortName] FOREIGN KEY ([field_shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[FieldCriterion] WITH NOCHECK
    ADD CONSTRAINT [FK_FieldCriterion_radiusSearchData_id_RadiusSearchData_id] FOREIGN KEY ([radiusSearchData_id]) REFERENCES [dbo].[RadiusSearchData] ([id]);


GO

ALTER TABLE [dbo].[FieldPosition] WITH NOCHECK
    ADD CONSTRAINT [FK_FieldPosition_shortName_EstablishmentField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[FieldPosition] WITH NOCHECK
    ADD CONSTRAINT [FK_FieldPosition_tab_code_FieldTab_code] FOREIGN KEY ([tab_code]) REFERENCES [dbo].[FieldTab] ([code]);


GO

ALTER TABLE [dbo].[FieldPosition_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_FieldPosition_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[FyiStakeholder] WITH NOCHECK
    ADD CONSTRAINT [FK_FyiStakeholder_type_EstablishmentType_code] FOREIGN KEY ([type]) REFERENCES [dbo].[EstablishmentType] ([code]);


GO

ALTER TABLE [dbo].[FyiStakeholder] WITH NOCHECK
    ADD CONSTRAINT [FK_FyiStakeholder_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[FyiStakeholder] WITH NOCHECK
    ADD CONSTRAINT [FK_FyiStakeholder_userGroup_code_UserGroup_code] FOREIGN KEY ([userGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_urbanRural_code_UrbanRural_code] FOREIGN KEY ([urbanRural_code]) REFERENCES [dbo].[UrbanRural] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_lsoa_code_LSOA_code] FOREIGN KEY ([lsoa_code]) REFERENCES [dbo].[LSOA] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_districtAdministrative_code_DistrictAdministrative_code] FOREIGN KEY ([districtAdministrative_code]) REFERENCES [dbo].[DistrictAdministrative] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_administrativeWard_code_AdministrativeWard_code] FOREIGN KEY ([administrativeWard_code]) REFERENCES [dbo].[AdministrativeWard] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_parliamentaryConstituency_code_ParliamentaryConstituency_code] FOREIGN KEY ([parliamentaryConstituency_code]) REFERENCES [dbo].[ParliamentaryConstituency] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_casWard_code_CasWard_code] FOREIGN KEY ([casWard_code]) REFERENCES [dbo].[CasWard] ([code]);


GO

ALTER TABLE [dbo].[GeoData] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_msoa_code_MSOA_code] FOREIGN KEY ([msoa_code]) REFERENCES [dbo].[MSOA] ([code]);


GO

ALTER TABLE [dbo].[GeoData_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GeoData_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GlossaryItem] WITH NOCHECK
    ADD CONSTRAINT [FK_GlossaryItem_user_username_SystemUser_username] FOREIGN KEY ([user_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[GovUserRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GovUserRequest_group_code_UserGroup_code] FOREIGN KEY ([group_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_approvedOrRejectedBy_username_SystemUser_username] FOREIGN KEY ([approvedOrRejectedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_urn_Establishment_URN] FOREIGN KEY ([urn]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_groupType_EstablishmentGroupType_code] FOREIGN KEY ([groupType]) REFERENCES [dbo].[EstablishmentGroupType] ([code]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_groupRelationsLink_GroupRelationsLink_id] FOREIGN KEY ([groupRelationsLink]) REFERENCES [dbo].[GroupRelationsLink] ([id]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_proposedBy_username_SystemUser_username] FOREIGN KEY ([proposedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_group_id_EstablishmentGroup_id] FOREIGN KEY ([group_id]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_groupLink_GroupLink_id] FOREIGN KEY ([groupLink]) REFERENCES [dbo].[GroupLink] ([id]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_headTitleId_code_Title_code] FOREIGN KEY ([headTitleId_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_field_shortName_GroupField_shortName] FOREIGN KEY ([field_shortName]) REFERENCES [dbo].[GroupField] ([shortName]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_groupCounty_code_County_code] FOREIGN KEY ([groupCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[GroupChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_hasJointCommittee_code_TrueFalse_code] FOREIGN KEY ([hasJointCommittee_code]) REFERENCES [dbo].[TrueFalse] ([code]);


GO

ALTER TABLE [dbo].[GroupChangeRequest_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupChangeRequest_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupField_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupField_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupField_GroupFieldPosition_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupField_GroupFieldPosition_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldPermission_shortName_GroupField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[GroupField] ([shortName]);


GO

ALTER TABLE [dbo].[GroupFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldPermission_groupCode_UserGroup_code] FOREIGN KEY ([groupCode]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[GroupFieldPermission_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldPermission_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupFieldPosition] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldPosition_shortName_GroupField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[GroupField] ([shortName]);


GO

ALTER TABLE [dbo].[GroupFieldPosition] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldPosition_tab_code_GroupFieldTab_code] FOREIGN KEY ([tab_code]) REFERENCES [dbo].[GroupFieldTab] ([code]);


GO

ALTER TABLE [dbo].[GroupFieldPosition_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldPosition_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupFieldTab_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldTab_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupFieldValidation] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldValidation_shortName_GroupField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[GroupField] ([shortName]);


GO

ALTER TABLE [dbo].[GroupFieldValidation] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupFieldValidation_validationRuleCode_ValidationRule_code] FOREIGN KEY ([validationRuleCode]) REFERENCES [dbo].[ValidationRule] ([code]);


GO

ALTER TABLE [dbo].[GroupLink] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupLink_urn_Establishment_URN] FOREIGN KEY ([urn]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[GroupLink] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupLink_group_id_EstablishmentGroup_id] FOREIGN KEY ([group_id]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [dbo].[GroupLink_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupLink_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[GroupRelationsLink] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupRelationsLink_linked_group_EstablishmentGroup_id] FOREIGN KEY ([linked_group]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [dbo].[GroupRelationsLink] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupRelationsLink_type_code_GroupLinkType_code] FOREIGN KEY ([type_code]) REFERENCES [dbo].[GroupLinkType] ([code]);


GO

ALTER TABLE [dbo].[GroupRelationsLink] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupRelationsLink_linking_group_EstablishmentGroup_id] FOREIGN KEY ([linking_group]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [dbo].[GroupRelationsLink_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_GroupRelationsLink_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[HighPerformanceOption_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_HighPerformanceOption_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[InboxMessage] WITH NOCHECK
    ADD CONSTRAINT [FK_InboxMessage_loggedBy_username_SystemUser_username] FOREIGN KEY ([loggedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_ChairmanCounty_code_County_code] FOREIGN KEY ([ChairmanCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_PropsCounty_code_County_code] FOREIGN KEY ([PropsCounty_code]) REFERENCES [dbo].[County] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_PropsTitle_code_Title_code] FOREIGN KEY ([PropsTitle_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_accomChange_code_AccommodationChanged_code] FOREIGN KEY ([accomChange_code]) REFERENCES [dbo].[AccommodationChanged] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_proprietorType_code_ProprietorType_code] FOREIGN KEY ([proprietorType_code]) REFERENCES [dbo].[ProprietorType] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_ChairmanCountry_code_Nationality_code] FOREIGN KEY ([ChairmanCountry_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_PropsCountry_code_Nationality_code] FOREIGN KEY ([PropsCountry_code]) REFERENCES [dbo].[Nationality] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_inspectorate_code_Inspectorate_code] FOREIGN KEY ([inspectorate_code]) REFERENCES [dbo].[Inspectorate] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_type_code_IndependentSchoolType_code] FOREIGN KEY ([type_code]) REFERENCES [dbo].[IndependentSchoolType] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_boardingEstablishment_code_BoardingEstablishment_code] FOREIGN KEY ([boardingEstablishment_code]) REFERENCES [dbo].[BoardingEstablishment] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_association_code_Association_code] FOREIGN KEY ([association_code]) REFERENCES [dbo].[Association] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_ChairmanTitle_code_Title_code] FOREIGN KEY ([ChairmanTitle_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_registrationSuspended_code_RegistrationSuspended_code] FOREIGN KEY ([registrationSuspended_code]) REFERENCES [dbo].[RegistrationSuspended] ([code]);


GO

ALTER TABLE [dbo].[IndependentSchools_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[IndependentSchools_EstablishmentProprietor_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_IndependentSchools_EstablishmentProprietor_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Inspection] WITH NOCHECK
    ADD CONSTRAINT [FK_Inspection_changeRequest_id_EstablishmentChangeHistory_id] FOREIGN KEY ([changeRequest_id]) REFERENCES [dbo].[EstablishmentChangeHistory] ([id]);


GO

ALTER TABLE [dbo].[LaGorMapping] WITH NOCHECK
    ADD CONSTRAINT [FK_LaGorMapping_localAuthority_code_LocalAuthority_code] FOREIGN KEY ([localAuthority_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[LaGorMapping] WITH NOCHECK
    ADD CONSTRAINT [FK_LaGorMapping_region_code_GovernmentOfficeRegion_code] FOREIGN KEY ([region_code]) REFERENCES [dbo].[GovernmentOfficeRegion] ([code]);


GO

ALTER TABLE [dbo].[LaGssMapping] WITH NOCHECK
    ADD CONSTRAINT [FK_LaGssMapping_gssLaCode_code_GSSLACode_code] FOREIGN KEY ([gssLaCode_code]) REFERENCES [dbo].[GSSLACode] ([code]);


GO

ALTER TABLE [dbo].[LaGssMapping] WITH NOCHECK
    ADD CONSTRAINT [FK_LaGssMapping_localAuthority_code_LocalAuthority_code] FOREIGN KEY ([localAuthority_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[LegacyData_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_LegacyData_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[LocalAuthority] WITH NOCHECK
    ADD CONSTRAINT [FK_LocalAuthority_group_code_LocalAuthorityGroup_code] FOREIGN KEY ([group_code]) REFERENCES [dbo].[LocalAuthorityGroup] ([code]);


GO

ALTER TABLE [dbo].[LocalAuthority_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_LocalAuthority_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[LocalAuthorityContactDetails] WITH NOCHECK
    ADD CONSTRAINT [FK_LocalAuthorityContactDetails_localAuthority_code_LocalAuthority_code] FOREIGN KEY ([localAuthority_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[MarketingOptOut_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_MarketingOptOut_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[NewsStory] WITH NOCHECK
    ADD CONSTRAINT [FK_NewsStory_signedOffBy_username_SystemUser_username] FOREIGN KEY ([signedOffBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[NewsStory] WITH NOCHECK
    ADD CONSTRAINT [FK_NewsStory_signOffUser_username_SystemUser_username] FOREIGN KEY ([signOffUser_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[NewsStory] WITH NOCHECK
    ADD CONSTRAINT [FK_NewsStory_createdBy_username_SystemUser_username] FOREIGN KEY ([createdBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[NewsStory_UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_NewsStory_UserGroup_groups_code_UserGroup_code] FOREIGN KEY ([groups_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[NewsStory_UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_NewsStory_UserGroup_NewsStory_id_NewsStory_id] FOREIGN KEY ([NewsStory_id]) REFERENCES [dbo].[NewsStory] ([id]);


GO

ALTER TABLE [dbo].[OldDictionary] WITH NOCHECK
    ADD CONSTRAINT [FK_OldDictionary_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[OwningRule] WITH NOCHECK
    ADD CONSTRAINT [FK_OwningRule_type_EstablishmentType_code] FOREIGN KEY ([type]) REFERENCES [dbo].[EstablishmentType] ([code]);


GO

ALTER TABLE [dbo].[OwningRule] WITH NOCHECK
    ADD CONSTRAINT [FK_OwningRule_status_EstablishmentStatus_code] FOREIGN KEY ([status]) REFERENCES [dbo].[EstablishmentStatus] ([code]);


GO

ALTER TABLE [dbo].[OwningRule] WITH NOCHECK
    ADD CONSTRAINT [FK_OwningRule_userGroup_code_UserGroup_code] FOREIGN KEY ([userGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[OwningRule] WITH NOCHECK
    ADD CONSTRAINT [FK_OwningRule_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[OwningRule_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_OwningRule_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[PersonalSettings] WITH NOCHECK
    ADD CONSTRAINT [FK_PersonalSettings_userGroup_UserGroup_code] FOREIGN KEY ([userGroup]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[PersonalSettings_DefaultColumns] WITH NOCHECK
    ADD CONSTRAINT [FK_PersonalSettings_DefaultColumns_PersonalSettings_id_PersonalSettings_id] FOREIGN KEY ([PersonalSettings_id]) REFERENCES [dbo].[PersonalSettings] ([id]);


GO

ALTER TABLE [dbo].[PersonalSettings_DefaultExtractFields] WITH NOCHECK
    ADD CONSTRAINT [FK_PersonalSettings_DefaultExtractFields_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[PersonalSettings_DefaultExtractFields] WITH NOCHECK
    ADD CONSTRAINT [FK_PersonalSettings_DefaultExtractFields_username_PersonalSettings_id] FOREIGN KEY ([username]) REFERENCES [dbo].[PersonalSettings] ([id]);


GO

ALTER TABLE [dbo].[QRTZ_CRON_TRIGGERS] WITH NOCHECK
    ADD CONSTRAINT [FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY ([SCHED_NAME], [TRIGGER_NAME], [TRIGGER_GROUP]) REFERENCES [dbo].[QRTZ_TRIGGERS] ([SCHED_NAME], [TRIGGER_NAME], [TRIGGER_GROUP]) ON DELETE CASCADE;


GO

ALTER TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] WITH NOCHECK
    ADD CONSTRAINT [FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY ([SCHED_NAME], [TRIGGER_NAME], [TRIGGER_GROUP]) REFERENCES [dbo].[QRTZ_TRIGGERS] ([SCHED_NAME], [TRIGGER_NAME], [TRIGGER_GROUP]) ON DELETE CASCADE;


GO

ALTER TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] WITH NOCHECK
    ADD CONSTRAINT [FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY ([SCHED_NAME], [TRIGGER_NAME], [TRIGGER_GROUP]) REFERENCES [dbo].[QRTZ_TRIGGERS] ([SCHED_NAME], [TRIGGER_NAME], [TRIGGER_GROUP]) ON DELETE CASCADE;


GO

ALTER TABLE [dbo].[QRTZ_TRIGGERS] WITH NOCHECK
    ADD CONSTRAINT [FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS] FOREIGN KEY ([SCHED_NAME], [JOB_NAME], [JOB_GROUP]) REFERENCES [dbo].[QRTZ_JOB_DETAILS] ([SCHED_NAME], [JOB_NAME], [JOB_GROUP]);


GO

ALTER TABLE [dbo].[ReasonEstablishmentClosed_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ReasonEstablishmentClosed_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ReasonEstablishmentOpened_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ReasonEstablishmentOpened_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Recipient] WITH NOCHECK
    ADD CONSTRAINT [FK_Recipient_messageId_InboxMessage_id] FOREIGN KEY ([messageId]) REFERENCES [dbo].[InboxMessage] ([id]);


GO

ALTER TABLE [dbo].[Recipient] WITH NOCHECK
    ADD CONSTRAINT [FK_Recipient_user_username_SystemUser_username] FOREIGN KEY ([user_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[Recipient] WITH NOCHECK
    ADD CONSTRAINT [FK_Recipient_establishment_URN_Establishment_URN] FOREIGN KEY ([establishment_URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[Recipient] WITH NOCHECK
    ADD CONSTRAINT [FK_Recipient_localAuthority_code_LocalAuthority_code] FOREIGN KEY ([localAuthority_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[ReligiousCharacter_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ReligiousCharacter_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Reminder] WITH NOCHECK
    ADD CONSTRAINT [FK_Reminder_creatorUserGroup_UserGroup_code] FOREIGN KEY ([creatorUserGroup]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[ReminderInstance] WITH NOCHECK
    ADD CONSTRAINT [FK_ReminderInstance_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[ReminderInstance] WITH NOCHECK
    ADD CONSTRAINT [FK_ReminderInstance_username_SystemUser_username] FOREIGN KEY ([username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[ReminderInstance] WITH NOCHECK
    ADD CONSTRAINT [FK_ReminderInstance_reminder_id_Reminder_id] FOREIGN KEY ([reminder_id]) REFERENCES [dbo].[Reminder] ([id]);


GO

ALTER TABLE [dbo].[ReportFile_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_ReportFile_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ReportFilePermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ReportFilePermission_groupCode_UserGroup_code] FOREIGN KEY ([groupCode]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[ReportFilePermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ReportFilePermission_reportKey_ReportFile_id] FOREIGN KEY ([reportKey]) REFERENCES [dbo].[ReportFile] ([id]);


GO

ALTER TABLE [dbo].[RevisionInfo] WITH NOCHECK
    ADD CONSTRAINT [FK_RevisionInfo_modifiedBy_username_SystemUser_username] FOREIGN KEY ([modifiedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[ScheduledExtract] WITH NOCHECK
    ADD CONSTRAINT [FK_ScheduledExtract_callback_id_Callback_id] FOREIGN KEY ([callback_id]) REFERENCES [dbo].[Callback] ([id]);


GO

ALTER TABLE [dbo].[ScheduledExtract] WITH NOCHECK
    ADD CONSTRAINT [FK_ScheduledExtract_asGroup_UserGroup_code] FOREIGN KEY ([asGroup]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[ScheduledExtract] WITH NOCHECK
    ADD CONSTRAINT [FK_ScheduledExtract_filter_id_EstablishmentFilter_id] FOREIGN KEY ([filter_id]) REFERENCES [dbo].[EstablishmentFilter] ([id]);


GO

ALTER TABLE [dbo].[ScheduledExtract] WITH NOCHECK
    ADD CONSTRAINT [FK_ScheduledExtract_creatorGroup_UserGroup_code] FOREIGN KEY ([creatorGroup]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[SchoolUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SchoolUser_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[SchoolUser_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_SchoolUser_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Section] WITH NOCHECK
    ADD CONSTRAINT [FK_Section_document_id_ContentDocument_id] FOREIGN KEY ([document_id]) REFERENCES [dbo].[ContentDocument] ([id]);


GO

ALTER TABLE [dbo].[Section_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Section_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Sen_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_Sen_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[SingleClickQuery_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_SingleClickQuery_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffChangeRequest_staffRecord_uid_StaffRecord_uid] FOREIGN KEY ([staffRecord_uid]) REFERENCES [dbo].[StaffRecord] ([uid]);


GO

ALTER TABLE [dbo].[StaffChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffChangeRequest_field_shortName_StaffField_shortName] FOREIGN KEY ([field_shortName]) REFERENCES [dbo].[StaffField] ([shortName]);


GO

ALTER TABLE [dbo].[StaffChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffChangeRequest_proposedBy_username_SystemUser_username] FOREIGN KEY ([proposedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[StaffChangeRequest] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffChangeRequest_approvedOrRejectedBy_username_SystemUser_username] FOREIGN KEY ([approvedOrRejectedBy_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[StaffField_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffField_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffField_StaffFieldPosition_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffField_StaffFieldPosition_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldPermission_groupCode_UserGroup_code] FOREIGN KEY ([groupCode]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[StaffFieldPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldPermission_shortName_StaffField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[StaffField] ([shortName]);


GO

ALTER TABLE [dbo].[StaffFieldPermission_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldPermission_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffFieldPosition] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldPosition_shortName_StaffField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[StaffField] ([shortName]);


GO

ALTER TABLE [dbo].[StaffFieldPosition_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldPosition_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffFieldRole] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldRole_staffRole_StaffRole_code] FOREIGN KEY ([staffRole]) REFERENCES [dbo].[StaffRole] ([code]);


GO

ALTER TABLE [dbo].[StaffFieldRole] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldRole_staffField_StaffField_shortName] FOREIGN KEY ([staffField]) REFERENCES [dbo].[StaffField] ([shortName]);


GO

ALTER TABLE [dbo].[StaffFieldValidation] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldValidation_validationRuleCode_ValidationRule_code] FOREIGN KEY ([validationRuleCode]) REFERENCES [dbo].[ValidationRule] ([code]);


GO

ALTER TABLE [dbo].[StaffFieldValidation] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffFieldValidation_shortName_StaffField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[StaffField] ([shortName]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_group_id_EstablishmentGroup_id] FOREIGN KEY ([group_id]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_establishment_URN_Establishment_URN] FOREIGN KEY ([establishment_URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_staffAppointingBody_code_StaffAppointingBody_code] FOREIGN KEY ([staffAppointingBody_code]) REFERENCES [dbo].[StaffAppointingBody] ([code]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_parentStaffRecord_uid_StaffRecord_uid] FOREIGN KEY ([parentStaffRecord_uid]) REFERENCES [dbo].[StaffRecord] ([uid]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_staffRole_code_StaffRole_code] FOREIGN KEY ([staffRole_code]) REFERENCES [dbo].[StaffRole] ([code]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_staffRecordEnc_id_StaffRecordEnc_id] FOREIGN KEY ([staffRecordEnc_id]) REFERENCES [dbo].[StaffRecordEnc] ([id]);


GO

ALTER TABLE [dbo].[StaffRecord] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_title_code_Title_code] FOREIGN KEY ([title_code]) REFERENCES [dbo].[Title] ([code]);


GO

ALTER TABLE [dbo].[StaffRecord_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecord_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffRecordEnc_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRecordEnc_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[StaffRoleExpiration] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRoleExpiration_staffRole_StaffRole_code] FOREIGN KEY ([staffRole]) REFERENCES [dbo].[StaffRole] ([code]);


GO

ALTER TABLE [dbo].[StaffRoleType] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRoleType_roleCode_StaffRole_code] FOREIGN KEY ([roleCode]) REFERENCES [dbo].[StaffRole] ([code]);


GO

ALTER TABLE [dbo].[StaffRoleType] WITH NOCHECK
    ADD CONSTRAINT [FK_StaffRoleType_typeCode_StaffType_code] FOREIGN KEY ([typeCode]) REFERENCES [dbo].[StaffType] ([code]);


GO

ALTER TABLE [dbo].[SubscriptionInfo_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionInfo_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[SubscriptionInvoice] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionInvoice_invoiceDocument_id_InvoiceDocument_id] FOREIGN KEY ([invoiceDocument_id]) REFERENCES [dbo].[InvoiceDocument] ([id]);


GO

ALTER TABLE [dbo].[SubscriptionInvoice] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionInvoice_subscriptionOrder_id_SubscriptionOrder_id] FOREIGN KEY ([subscriptionOrder_id]) REFERENCES [dbo].[SubscriptionOrder] ([id]);


GO

ALTER TABLE [dbo].[SubscriptionInvoice] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionInvoice_proformaDocument_id_InvoiceDocument_id] FOREIGN KEY ([proformaDocument_id]) REFERENCES [dbo].[InvoiceDocument] ([id]);


GO

ALTER TABLE [dbo].[SubscriptionOrder] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionOrder_schedule_id_SubscriptionSchedule_id] FOREIGN KEY ([schedule_id]) REFERENCES [dbo].[SubscriptionSchedule] ([id]);


GO

ALTER TABLE [dbo].[SubscriptionOrder] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionOrder_paymentTerm_code_PaymentTerm_code] FOREIGN KEY ([paymentTerm_code]) REFERENCES [dbo].[PaymentTerm] ([code]);


GO

ALTER TABLE [dbo].[SubscriptionOrder] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionOrder_subscriptionPackage_code_SubscriptionPackage_code] FOREIGN KEY ([subscriptionPackage_code]) REFERENCES [dbo].[SubscriptionPackage] ([code]);


GO

ALTER TABLE [dbo].[SubscriptionOrder] WITH NOCHECK
    ADD CONSTRAINT [FK_SubscriptionOrder_user_username_SystemUser_username] FOREIGN KEY ([user_username]) REFERENCES [dbo].[SystemUser] ([username]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_UserGroupCode_UserGroup_code] FOREIGN KEY ([UserGroupCode]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_ldapProfile_id_LdapProfile_id] FOREIGN KEY ([ldapProfile_id]) REFERENCES [dbo].[LdapProfile] ([id]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_subscriptionInfo_id_SubscriptionInfo_id] FOREIGN KEY ([subscriptionInfo_id]) REFERENCES [dbo].[SubscriptionInfo] ([id]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_LocalAuthority_LocalAuthority_code] FOREIGN KEY ([LocalAuthority]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_UID_EstablishmentGroup_id] FOREIGN KEY ([UID]) REFERENCES [dbo].[EstablishmentGroup] ([id]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_agreement_id_Document_id] FOREIGN KEY ([agreement_id]) REFERENCES [dbo].[Document] ([id]);


GO

ALTER TABLE [dbo].[SystemUser] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_URN_Establishment_URN] FOREIGN KEY ([URN]) REFERENCES [dbo].[Establishment] ([URN]);


GO

ALTER TABLE [dbo].[SystemUser_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_SystemUser_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[ToolPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ToolPermission_groupCode_UserGroup_code] FOREIGN KEY ([groupCode]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[ToolPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_ToolPermission_toolCode_Tool_code] FOREIGN KEY ([toolCode]) REFERENCES [dbo].[Tool] ([code]);


GO

ALTER TABLE [dbo].[TrustedSource] WITH NOCHECK
    ADD CONSTRAINT [FK_TrustedSource_type_EstablishmentType_code] FOREIGN KEY ([type]) REFERENCES [dbo].[EstablishmentType] ([code]);


GO

ALTER TABLE [dbo].[TrustedSource] WITH NOCHECK
    ADD CONSTRAINT [FK_TrustedSource_userGroup_code_UserGroup_code] FOREIGN KEY ([userGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[TrustedSource] WITH NOCHECK
    ADD CONSTRAINT [FK_TrustedSource_field_EstablishmentField_shortName] FOREIGN KEY ([field]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[TrustedSource_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_TrustedSource_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[UsedEstablishmentNumber] WITH NOCHECK
    ADD CONSTRAINT [FK_UsedEstablishmentNumber_LA_code_LocalAuthority_code] FOREIGN KEY ([LA_code]) REFERENCES [dbo].[LocalAuthority] ([code]);


GO

ALTER TABLE [dbo].[UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroup_recordStatus_code_RecordStatus_code] FOREIGN KEY ([recordStatus_code]) REFERENCES [dbo].[RecordStatus] ([code]);


GO

ALTER TABLE [dbo].[UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroup_role_UserRole_authority] FOREIGN KEY ([role]) REFERENCES [dbo].[UserRole] ([authority]);


GO

ALTER TABLE [dbo].[UserGroup] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroup_sla_id_Document_id] FOREIGN KEY ([sla_id]) REFERENCES [dbo].[Document] ([id]);


GO

ALTER TABLE [dbo].[UserGroup_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroup_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[UserGroup_Authority] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroup_Authority_UserGroup_code_UserGroup_code] FOREIGN KEY ([UserGroup_code]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[UserGroup_Authority] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroup_Authority_authorities_code_Authority_code] FOREIGN KEY ([authorities_code]) REFERENCES [dbo].[Authority] ([code]);


GO

ALTER TABLE [dbo].[UserGroupPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroupPermission_groupCode_UserGroup_code] FOREIGN KEY ([groupCode]) REFERENCES [dbo].[UserGroup] ([code]);


GO

ALTER TABLE [dbo].[UserGroupPermission] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroupPermission_shortName_EstablishmentField_shortName] FOREIGN KEY ([shortName]) REFERENCES [dbo].[EstablishmentField] ([shortName]);


GO

ALTER TABLE [dbo].[UserGroupPermission_aud] WITH NOCHECK
    ADD CONSTRAINT [FK_UserGroupPermission_aud_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[UserRole_AUD] WITH NOCHECK
    ADD CONSTRAINT [FK_UserRole_AUD_ver_rev_RevisionInfo_id] FOREIGN KEY ([ver_rev]) REFERENCES [dbo].[RevisionInfo] ([id]);


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK__Establish__FEHEI__32AB8735] CHECK ([FEHEIdentifier]>=(1) AND [FEHEIdentifier]<=(999999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK_Establishment_TeenMothersPlaces] CHECK ([TeenMothersPlaces]>=(0) AND [TeenMothersPlaces]<=(9999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK__Establish__Previ__339FAB6E] CHECK ([PreviousEstablishmentNumber]>=(1) AND [PreviousEstablishmentNumber]<=(9999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK__Establish__Burnh__3587F3E0] CHECK ([BurnhamGroup]>=(0) AND [BurnhamGroup]<=(6));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK_Census_EYGovernmentFundedChildren] CHECK ([EYGovernmentFundedChildren]>=(0) AND [EYGovernmentFundedChildren]<=(9999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK_Establishment_ApprovedNumberBoardersSpecial] CHECK ([ApprovedNumberBoardersSpecial]>=(0) AND [ApprovedNumberBoardersSpecial]<=(9999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK_Census_HighestFullTimeAge] CHECK ([HighestFullTimeAge]>=(0) AND [HighestFullTimeAge]<=(99));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK__Establish__Estab__31B762FC] CHECK ([EstablishmentNumber]>=(1) AND [EstablishmentNumber]<=(9999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK__Establish__UKPRN__3493CFA7] CHECK ([UKPRN]>=(10000000) AND [UKPRN]<=(99999999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK_Establishment_PlacesPRU] CHECK ([PlacesPRU]>=(0) AND [PlacesPRU]<=(9999));


GO

ALTER TABLE [dbo].[Establishment] WITH NOCHECK
    ADD CONSTRAINT [CK_Census_LowestFullTimeAge] CHECK ([LowestFullTimeAge]>=(0) AND [LowestFullTimeAge]<=(99));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTBoy__4865BE2A] CHECK ([PTBoys4a]>=(0) AND [PTBoys4a]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__53D770D6] CHECK ([TotalPupilsPartTime]>=(0) AND [TotalPupilsPartTime]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__SENst__50FB042B] CHECK ([SENstat]>=(0) AND [SENstat]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTGir__4E1E9780] CHECK ([PTGirls4b]>=(0) AND [PTGirls4b]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTGir__4B422AD5] CHECK ([PTGirls2andUnder]>=(0) AND [PTGirls2andUnder]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__56B3DD81] CHECK ([TotalWeeklyHoursPTStaff]>=(0) AND [TotalWeeklyHoursPTStaff]<=(99));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Lowes__4589517F] CHECK ([LowestDayFee]>=(0) AND [LowestDayFee]<=(999999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Highe__42ACE4D4] CHECK ([HighestBoardFee]>=(0) AND [HighestBoardFee]<=(999999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__57A801BA] CHECK ([TotalBoyBoarders]>=(0) AND [TotalBoyBoarders]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTBoy__467D75B8] CHECK ([PTBoys2andUnder]>=(0) AND [PTBoys2andUnder]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTBoy__4959E263] CHECK ([PTBoys4b]>=(0) AND [PTBoys4b]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__54CB950F] CHECK ([TotalNumberFTStaff]>=(0) AND [TotalNumberFTStaff]<=(999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Highe__43A1090D] CHECK ([HighestDayFee]>=(0) AND [HighestDayFee]<=(999999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__51EF2864] CHECK ([TotalPupilsPublicCare]>=(0) AND [TotalPupilsPublicCare]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTGir__4C364F0E] CHECK ([PTGirls3]>=(0) AND [PTGirls3]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTGir__4F12BBB9] CHECK ([PTGirls4c]>=(0) AND [PTGirls4c]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Compu__41B8C09B] CHECK ([CompulsorySchoolAge]>=(0) AND [CompulsorySchoolAge]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Lowes__44952D46] CHECK ([LowestBoardFee]>=(0) AND [LowestBoardFee]<=(999999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTBoy__4A4E069C] CHECK ([PTBoys4c]>=(0) AND [PTBoys4c]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__52E34C9D] CHECK ([TotalPupilsFullTime]>=(0) AND [TotalPupilsFullTime]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTGir__4D2A7347] CHECK ([PTGirls4a]>=(0) AND [PTGirls4a]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__SENno__5006DFF2] CHECK ([SENnoStat]>=(0) AND [SENnoStat]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__589C25F3] CHECK ([TotalGirlBoarders]>=(0) AND [TotalGirlBoarders]<=(9999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__Total__55BFB948] CHECK ([TotalNumberPTStaff]>=(0) AND [TotalNumberPTStaff]<=(999));


GO

ALTER TABLE [dbo].[IndependentSchools] WITH NOCHECK
    ADD CONSTRAINT [CK__Independe__PTBoy__477199F1] CHECK ([PTBoys3]>=(0) AND [PTBoys3]<=(9999));


GO

