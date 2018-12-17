USE [mixerp]
GO
CREATE SCHEMA [config] AUTHORIZATION [public]
GO
use [mixerp]
GO
GRANT ALTER ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT CONTROL ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT CREATE SEQUENCE ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT DELETE ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT EXECUTE ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT INSERT ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT REFERENCES ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT SELECT ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT TAKE OWNERSHIP ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT UPDATE ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT VIEW CHANGE TRACKING ON SCHEMA::[config] TO [public]
GO
use [mixerp]
GO
GRANT VIEW DEFINITION ON SCHEMA::[config] TO [public]
GO


CREATE SCHEMA [localization]
GO



USE [mixerp]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [config].[mixerp](
	[key] [varchar](50) NOT NULL,
	[value] [varchar](50) NULL,
	[description] [varchar](500) NULL,
 CONSTRAINT [PK_mixerp] PRIMARY KEY CLUSTERED 
(
	[key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [config].[mixerp]
           ([key]
           ,[value]
           ,[description])
     VALUES
           ('MinimumLogLevel'
           ,'0'
           ,'Verbose = 0,Debug = 1,Information = 2,Warning = 3,Error = 4,Fatal = 5')
GO


CREATE TABLE [localization].[resources]
(
    resource_id         int IDENTITY(1,1) PRIMARY KEY,
    resource_class      varchar(50),
    [key]               varchar(50),
    value               varchar(500)
)
GO

CREATE UNIQUE NONCLUSTERED INDEX resources_path_key_uix ON localization.resources(resource_class,[key]) 
GO
CREATE UNIQUE NONCLUSTERED INDEX resources_path_inx ON localization.resources(resource_class) 
GO
CREATE UNIQUE NONCLUSTERED INDEX resources_key_inx ON localization.resources([key]) 
GO

CREATE TABLE localization.cultures
(
    culture_code        varchar(50) PRIMARY KEY,
    culture_name        Varchar(100)
);
GO

INSERT INTO localization.cultures
SELECT 'de-DE',     'German (Germany)'              UNION ALL
SELECT 'en-GB',     'English (United Kingdom)'      UNION ALL
SELECT 'es-ES',     'Spanish (Spain)'               UNION ALL
SELECT 'fil-PH',    'Filipino (Philippines)'        UNION ALL
SELECT 'fr-FR',     'French (France)'               UNION ALL
SELECT 'id-ID',     'Indonesian (Indonesia)'        UNION ALL
SELECT 'ja-JP',     'Japanese (Japan)'              UNION ALL
SELECT 'ms-MY',     'Malay (Malaysia)'              UNION ALL
SELECT 'nl-NL',     'Dutch (Netherlands)'           UNION ALL
SELECT 'pt-PT',     'Portuguese (Portugal)'         UNION ALL
SELECT 'ru-RU',     'Russian (Russia)'              UNION ALL
SELECT 'sv-SE',     'Swedish (Sweden)'              UNION ALL
SELECT 'ro-RO',     'Romanian (Romania)'            UNION ALL
SELECT 'zh-CN',     'Simplified Chinese (China)';
GO

CREATE TABLE localization.localized_resources
(
    resource_id         int IDENTITY(1,1) PRIMARY KEY,
    culture_code        varchar(50),
    [key]               varchar(50),
    value               varchar(500),
	 CONSTRAINT FK_localized_resources_cultures FOREIGN KEY(culture_code) REFERENCES localization.cultures(culture_code) ON UPDATE  NO ACTION ON DELETE  NO ACTION 
);

GO
CREATE UNIQUE NONCLUSTERED INDEX localized_resources_culture_key_uix ON localization.localized_resources(culture_code,[key]) 
GO

DROP VIEW localization.resource_view;
GO

CREATE VIEW localization.localized_resources_view
AS
SELECT
    REPLACE(localization.resources.resource_class, '.resx', '') + '.' + localization.localized_resources.culture_code + '.resx' AS resource,
    localization.resources.[key],
    localization.localized_resources.culture_code,
    localization.localized_resources.value
FROM localization.resources
INNER JOIN localization.localized_resources
ON localization.localized_resources.[key] = localization.resources.[key]
GO

DROP VIEW localization.resource_view;
GO

CREATE VIEW localization.resource_view
AS
SELECT resource_class, '' as culture, [key], value
FROM localization.resources
UNION ALL
SELECT resource_class, culture_code, [key], localization.localized_resources.value 
FROM localization.localized_resources
INNER JOIN localization.resources
ON localization.localized_resources.resource_id = localization.resources.resource_id;
GO


DROP VIEW localization.localized_resource_view;
GO

CREATE VIEW localization.localized_resource_view
AS
SELECT
    resource_class + CASE WHEN COALESCE(culture, '') = '' THEN '' ELSE '.' + culture END + '.' + [key] as [key], value 
FROM localization.resource_view;
GO

SET ANSI_PADDING OFF
GO
