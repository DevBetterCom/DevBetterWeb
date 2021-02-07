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
    VALUES (N'20201016195014_InitialModel', N'5.0.2');
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
    VALUES (N'20201016200028_CodinGameUrl', N'5.0.2');
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
    VALUES (N'20201104211836_DiscordUsername', N'5.0.2');
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
    VALUES (N'20210118212122_MapCoordinates', N'5.0.2');
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
    VALUES (N'20210207162757_AddShippingAddress', N'5.0.2');
END;
GO

COMMIT;
GO

