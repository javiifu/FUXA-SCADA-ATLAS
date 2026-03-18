IF COL_LENGTH('Maquina', 'IdentificadorObjetoFuxa') IS NULL
BEGIN
    ALTER TABLE [Maquina]
    ADD [IdentificadorObjetoFuxa] NVARCHAR(100) NULL;
END;
GO

IF EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Maquina_IdentificadorObjetoFuxa'
      AND object_id = OBJECT_ID('Maquina')
      AND is_unique = 0
)
BEGIN
    DROP INDEX [IX_Maquina_IdentificadorObjetoFuxa] ON [Maquina];
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Maquina_IdentificadorObjetoFuxa'
      AND object_id = OBJECT_ID('Maquina')
      AND is_unique = 1
)
BEGIN
    CREATE UNIQUE INDEX [IX_Maquina_IdentificadorObjetoFuxa]
        ON [Maquina]([IdentificadorObjetoFuxa])
        WHERE [IdentificadorObjetoFuxa] IS NOT NULL;
END;
GO
