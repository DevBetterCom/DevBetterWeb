IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190927233602_InitialModel')
BEGIN
    CREATE TABLE [ArchiveVideos] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(max) NULL,
        [ShowNotes] nvarchar(max) NULL,
        [DateCreated] datetimeoffset NOT NULL,
        [VideoUrl] nvarchar(max) NULL,
        CONSTRAINT [PK_ArchiveVideos] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190927233602_InitialModel')
BEGIN
    CREATE TABLE [Question] (
        [Id] int NOT NULL IDENTITY,
        [ArchiveVideoId] int NOT NULL,
        [QuestionText] nvarchar(max) NULL,
        [TimestampSeconds] int NOT NULL,
        CONSTRAINT [PK_Question] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Question_ArchiveVideos_ArchiveVideoId] FOREIGN KEY ([ArchiveVideoId]) REFERENCES [ArchiveVideos] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190927233602_InitialModel')
BEGIN
    CREATE INDEX [IX_Question_ArchiveVideoId] ON [Question] ([ArchiveVideoId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190927233602_InitialModel')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190927233602_InitialModel', N'3.1.1');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200218011305_MemberEntity')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Question]') AND [c].[name] = N'QuestionText');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Question] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Question] ALTER COLUMN [QuestionText] nvarchar(500) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200218011305_MemberEntity')
BEGIN
    CREATE TABLE [Members] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(500) NULL,
        [FirstName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NULL,
        [Address] nvarchar(500) NULL,
        [LinkedInUrl] nvarchar(200) NULL,
        [TwitterUrl] nvarchar(200) NULL,
        [GithubUrl] nvarchar(200) NULL,
        [BlogUrl] nvarchar(200) NULL,
        [TwitchUrl] nvarchar(200) NULL,
        [OtherUrl] nvarchar(200) NULL,
        [AboutInfo] nvarchar(max) NULL,
        [DateCreated] datetime2 NOT NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200218011305_MemberEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200218011305_MemberEntity', N'3.1.1');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812181103_YouTubeAddedToMember')
BEGIN
    EXEC sp_rename N'[Members].[GithubUrl]', N'GitHubUrl', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812181103_YouTubeAddedToMember')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Members]') AND [c].[name] = N'UserId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Members] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Members] ALTER COLUMN [UserId] nvarchar(500) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812181103_YouTubeAddedToMember')
BEGIN
    ALTER TABLE [Members] ADD [YouTubeUrl] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200812181103_YouTubeAddedToMember')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200812181103_YouTubeAddedToMember', N'3.1.1');
END;

GO

