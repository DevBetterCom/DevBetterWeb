IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [ArchiveVideos] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NULL,
        [ShowNotes] nvarchar(max) NULL,
        [DateCreated] datetimeoffset NOT NULL,
        [VideoUrl] nvarchar(200) NULL,
        CONSTRAINT [PK_ArchiveVideos] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [Books] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(500) NULL,
        [Author] nvarchar(100) NULL,
        [Details] nvarchar(1000) NULL,
        [PurchaseUrl] nvarchar(200) NULL,
        CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [Members] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(500) NOT NULL,
        [FirstName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NULL,
        [AboutInfo] nvarchar(max) NULL,
        [Address] nvarchar(500) NULL,
        [PEFriendCode] nvarchar(100) NULL,
        [PEUsername] nvarchar(100) NULL,
        [BlogUrl] nvarchar(200) NULL,
        [GitHubUrl] nvarchar(200) NULL,
        [LinkedInUrl] nvarchar(200) NULL,
        [OtherUrl] nvarchar(200) NULL,
        [TwitchUrl] nvarchar(200) NULL,
        [YouTubeUrl] nvarchar(200) NULL,
        [TwitterUrl] nvarchar(200) NULL,
        [DateCreated] datetime2 NOT NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [Question] (
        [Id] int NOT NULL IDENTITY,
        [ArchiveVideoId] int NOT NULL,
        [QuestionText] nvarchar(500) NULL,
        [TimestampSeconds] int NOT NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Question_ArchiveVideos_ArchiveVideoId] FOREIGN KEY ([ArchiveVideoId]) REFERENCES [ArchiveVideos] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [BookMember] (
        [BooksReadId] int NOT NULL,
        [MembersWhoHaveReadId] int NOT NULL,
        CONSTRAINT [PK_BookMember] PRIMARY KEY ([BooksReadId], [MembersWhoHaveReadId]),
        CONSTRAINT [FK_BookMember_Books_BooksReadId] FOREIGN KEY ([BooksReadId]) REFERENCES [Books] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookMember_Members_MembersWhoHaveReadId] FOREIGN KEY ([MembersWhoHaveReadId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [Subscriptions] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        CONSTRAINT [PK_Subscriptions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Subscriptions_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE TABLE [SubscriptionDates] (
        [SubscriptionId] int NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NULL,
        CONSTRAINT [PK_SubscriptionDates] PRIMARY KEY ([SubscriptionId]),
        CONSTRAINT [FK_SubscriptionDates_Subscriptions_SubscriptionId] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscriptions] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE INDEX [IX_BookMember_MembersWhoHaveReadId] ON [BookMember] ([MembersWhoHaveReadId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE INDEX [IX_Question_ArchiveVideoId] ON [Question] ([ArchiveVideoId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    CREATE INDEX [IX_Subscriptions_MemberId] ON [Subscriptions] ([MemberId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016195014_InitialModel')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201016195014_InitialModel', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016200028_CodinGameUrl')
BEGIN
    ALTER TABLE [Members] ADD [CodinGameUrl] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201016200028_CodinGameUrl')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201016200028_CodinGameUrl', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201104211836_DiscordUsername')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Members]') AND [c].[name] = N'CodinGameUrl');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Members] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Members] ALTER COLUMN [CodinGameUrl] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201104211836_DiscordUsername')
BEGIN
    ALTER TABLE [Members] ADD [DiscordUsername] nvarchar(200) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201104211836_DiscordUsername')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201104211836_DiscordUsername', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210118212122_MapCoordinates')
BEGIN
    ALTER TABLE [Members] ADD [CityLatitude] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210118212122_MapCoordinates')
BEGIN
    ALTER TABLE [Members] ADD [CityLongitude] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210118212122_MapCoordinates')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210118212122_MapCoordinates', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [CityLocation_Latitude] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [CityLocation_Longitude] decimal(18,2) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [ShippingAddress_City] nvarchar(100) NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [ShippingAddress_Country] nvarchar(100) NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [ShippingAddress_PostalCode] nvarchar(12) NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [ShippingAddress_State] nvarchar(100) NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    ALTER TABLE [Members] ADD [ShippingAddress_Street] nvarchar(500) NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210207162757_AddShippingAddress')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210207162757_AddShippingAddress', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210209190217_InviteEntity')
BEGIN
    CREATE TABLE [Invitations] (
        [Id] int NOT NULL IDENTITY,
        [Email] nvarchar(max) NULL,
        [InviteCode] nvarchar(max) NULL,
        [StripeSubscriptionId] nvarchar(max) NULL,
        CONSTRAINT [PK_Invitations] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210209190217_InviteEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210209190217_InviteEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210209190650_InviteEntityActiveBool')
BEGIN
    ALTER TABLE [Invitations] ADD [Active] bit NOT NULL DEFAULT CAST(0 AS bit);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210209190650_InviteEntityActiveBool')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210209190650_InviteEntityActiveBool', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210218184116_InviteEntityConfig')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Invitations]') AND [c].[name] = N'StripeSubscriptionId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Invitations] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Invitations] DROP COLUMN [StripeSubscriptionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210218184116_InviteEntityConfig')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Invitations]') AND [c].[name] = N'InviteCode');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Invitations] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Invitations] ALTER COLUMN [InviteCode] nvarchar(500) NOT NULL;
    ALTER TABLE [Invitations] ADD DEFAULT N'' FOR [InviteCode];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210218184116_InviteEntityConfig')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Invitations]') AND [c].[name] = N'Email');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Invitations] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Invitations] ALTER COLUMN [Email] nvarchar(200) NOT NULL;
    ALTER TABLE [Invitations] ADD DEFAULT N'' FOR [Email];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210218184116_InviteEntityConfig')
BEGIN
    ALTER TABLE [Invitations] ADD [PaymentHandlerSubscriptionId] nvarchar(500) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210218184116_InviteEntityConfig')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210218184116_InviteEntityConfig', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210325205032_BillingActivity')
BEGIN
    CREATE TABLE [BillingActivities] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [Date] datetime2 NOT NULL,
        [Details_Message] nvarchar(500) NOT NULL DEFAULT N'',
        [Details_Amount] decimal(18,2) NOT NULL DEFAULT 0.0,
        CONSTRAINT [PK_BillingActivities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BillingActivities_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210325205032_BillingActivity')
BEGIN
    CREATE INDEX [IX_BillingActivities_MemberId] ON [BillingActivities] ([MemberId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210325205032_BillingActivity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210325205032_BillingActivity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BillingActivities]') AND [c].[name] = N'Details_Message');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [BillingActivities] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [BillingActivities] DROP COLUMN [Details_Message];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    ALTER TABLE [Subscriptions] ADD [SubscriptionPlanId] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    ALTER TABLE [BillingActivities] ADD [Details_ActionVerbPastTense] nvarchar(100) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    ALTER TABLE [BillingActivities] ADD [Details_BillingPeriod] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    ALTER TABLE [BillingActivities] ADD [Details_MemberName] nvarchar(100) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    ALTER TABLE [BillingActivities] ADD [Details_SubscriptionPlanName] nvarchar(100) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    CREATE TABLE [SubscriptionPlan] (
        [Id] int NOT NULL IDENTITY,
        [Details_Name] nvarchar(100) NULL,
        [Details_PricePerBillingPeriod] decimal(18,2) NULL,
        [Details_BillingPeriod] int NULL,
        CONSTRAINT [PK_SubscriptionPlan] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412190046_SubscriptionPlanAndBillingDetails')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210412190046_SubscriptionPlanAndBillingDetails', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412195027_DateToBillingDetailsFromBillingActivity')
BEGIN
    EXEC sp_rename N'[BillingActivities].[Date]', N'Details_Date', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210412195027_DateToBillingDetailsFromBillingActivity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210412195027_DateToBillingDetailsFromBillingActivity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    ALTER TABLE [SubscriptionDates] DROP CONSTRAINT [FK_SubscriptionDates_Subscriptions_SubscriptionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    EXEC sp_rename N'[SubscriptionDates].[SubscriptionId]', N'MemberSubscriptionId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SubscriptionPlan]') AND [c].[name] = N'Details_PricePerBillingPeriod');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [SubscriptionPlan] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [SubscriptionPlan] ALTER COLUMN [Details_PricePerBillingPeriod] decimal(18,2) NOT NULL;
    ALTER TABLE [SubscriptionPlan] ADD DEFAULT 0.0 FOR [Details_PricePerBillingPeriod];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SubscriptionPlan]') AND [c].[name] = N'Details_Name');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [SubscriptionPlan] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [SubscriptionPlan] ALTER COLUMN [Details_Name] nvarchar(100) NOT NULL;
    ALTER TABLE [SubscriptionPlan] ADD DEFAULT N'' FOR [Details_Name];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SubscriptionPlan]') AND [c].[name] = N'Details_BillingPeriod');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [SubscriptionPlan] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [SubscriptionPlan] ALTER COLUMN [Details_BillingPeriod] int NOT NULL;
    ALTER TABLE [SubscriptionPlan] ADD DEFAULT 0 FOR [Details_BillingPeriod];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BillingActivities]') AND [c].[name] = N'Details_ActionVerbPastTense');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [BillingActivities] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [BillingActivities] ALTER COLUMN [Details_ActionVerbPastTense] int NOT NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    ALTER TABLE [SubscriptionDates] ADD CONSTRAINT [FK_SubscriptionDates_Subscriptions_MemberSubscriptionId] FOREIGN KEY ([MemberSubscriptionId]) REFERENCES [Subscriptions] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210614175320_RenameSubscriptionToMemberSubscription')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210614175320_RenameSubscriptionToMemberSubscription', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [SubscriptionDates] DROP CONSTRAINT [FK_SubscriptionDates_Subscriptions_MemberSubscriptionId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [Subscriptions] DROP CONSTRAINT [FK_Subscriptions_Members_MemberId];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    DROP TABLE [SubscriptionPlan];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [Subscriptions] DROP CONSTRAINT [PK_Subscriptions];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [SubscriptionDates] DROP CONSTRAINT [PK_SubscriptionDates];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    EXEC sp_rename N'[Subscriptions]', N'MemberSubscriptions';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    EXEC sp_rename N'[SubscriptionDates]', N'MemberSubscriptionDates';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    EXEC sp_rename N'[MemberSubscriptions].[SubscriptionPlanId]', N'MemberSubscriptionPlanId', N'COLUMN';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    EXEC sp_rename N'[MemberSubscriptions].[IX_Subscriptions_MemberId]', N'IX_MemberSubscriptions_MemberId', N'INDEX';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [MemberSubscriptions] ADD CONSTRAINT [PK_MemberSubscriptions] PRIMARY KEY ([Id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [MemberSubscriptionDates] ADD CONSTRAINT [PK_MemberSubscriptionDates] PRIMARY KEY ([MemberSubscriptionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    CREATE TABLE [MemberSubscriptionPlan] (
        [Id] int NOT NULL IDENTITY,
        [Details_Name] nvarchar(100) NOT NULL,
        [Details_PricePerBillingPeriod] decimal(18,2) NOT NULL,
        [Details_BillingPeriod] int NOT NULL,
        CONSTRAINT [PK_MemberSubscriptionPlan] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [MemberSubscriptionDates] ADD CONSTRAINT [FK_MemberSubscriptionDates_MemberSubscriptions_MemberSubscriptionId] FOREIGN KEY ([MemberSubscriptionId]) REFERENCES [MemberSubscriptions] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    ALTER TABLE [MemberSubscriptions] ADD CONSTRAINT [FK_MemberSubscriptions_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210615133924_SubscriptionFullRename')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210615133924_SubscriptionFullRename', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210617200829_DailyCheckEntity')
BEGIN
    CREATE TABLE [DailyChecks] (
        [Id] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [TasksCompleted] nvarchar(500) NULL,
        CONSTRAINT [PK_DailyChecks] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210617200829_DailyCheckEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210617200829_DailyCheckEntity', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210713141727_InviteEntityDateProperties')
BEGIN
    ALTER TABLE [Invitations] ADD [DateCreated] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210713141727_InviteEntityDateProperties')
BEGIN
    ALTER TABLE [Invitations] ADD [DateOfLastAdminPing] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210713141727_InviteEntityDateProperties')
BEGIN
    ALTER TABLE [Invitations] ADD [DateOfUserPing] datetime2 NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210713141727_InviteEntityDateProperties')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210713141727_InviteEntityDateProperties', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [DateUploaded] datetimeoffset NOT NULL DEFAULT '0001-01-01T00:00:00.0000000+00:00';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [Description] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [Duration] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [Status] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [VideoId] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    CREATE TABLE [MemberVideo] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [ArchiveVideoId] int NOT NULL,
        [SecondWatched] int NOT NULL,
        [IsCompleted] bit NOT NULL,
        CONSTRAINT [PK_MemberVideo] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MemberVideo_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    CREATE INDEX [IX_MemberVideo_MemberId] ON [MemberVideo] ([MemberId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210914184658_MIGRATIONNAME')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210914184658_MIGRATIONNAME', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210916044929_VideoProgress')
BEGIN
    DROP TABLE [MemberVideo];
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210916044929_VideoProgress')
BEGIN
    CREATE TABLE [MemberVideoProgress] (
        [Id] int NOT NULL IDENTITY,
        [MemberId] int NOT NULL,
        [ArchiveVideoId] int NOT NULL,
        [SecondWatched] int NOT NULL,
        [IsCompleted] bit NOT NULL,
        CONSTRAINT [PK_MemberVideoProgress] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MemberVideoProgress_ArchiveVideos_ArchiveVideoId] FOREIGN KEY ([ArchiveVideoId]) REFERENCES [ArchiveVideos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MemberVideoProgress_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210916044929_VideoProgress')
BEGIN
    CREATE INDEX [IX_MemberVideoProgress_ArchiveVideoId] ON [MemberVideoProgress] ([ArchiveVideoId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210916044929_VideoProgress')
BEGIN
    CREATE INDEX [IX_MemberVideoProgress_MemberId] ON [MemberVideoProgress] ([MemberId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210916044929_VideoProgress')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210916044929_VideoProgress', N'5.0.11');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211006222227_MemberVideoList')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [AnimatedThumbnailUri] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211006222227_MemberVideoList')
BEGIN
    ALTER TABLE [ArchiveVideos] ADD [Views] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20211006222227_MemberVideoList')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20211006222227_MemberVideoList', N'5.0.11');
END;
GO

COMMIT;
GO

