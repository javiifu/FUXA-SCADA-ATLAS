IF COL_LENGTH('Maquina', 'IdentificadorObjetoFuxa') IS NULL
BEGIN
    ALTER TABLE [Maquina]
    ADD [IdentificadorObjetoFuxa] NVARCHAR(100) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Maquina_IdentificadorObjetoFuxa'
      AND object_id = OBJECT_ID('Maquina')
)
BEGIN
    CREATE INDEX [IX_Maquina_IdentificadorObjetoFuxa]
        ON [Maquina]([IdentificadorObjetoFuxa]);
END;
GO
