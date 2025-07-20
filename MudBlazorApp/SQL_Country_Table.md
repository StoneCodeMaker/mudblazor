# Country Table SQL Script

## Create Table Script

```sql
USE [fd-ng-local]
GO

-- Drop the table if it already exists to avoid errors
IF OBJECT_ID('dbo.Country', 'U') IS NOT NULL
    DROP TABLE dbo.Country
GO

-- Create the Country table
CREATE TABLE dbo.Country (
    Country_ID INT NOT NULL PRIMARY KEY,
    Name VARCHAR(100),
    Verfied_Mode_ID SMALLINT,  -- Note the typo in 'Verfied' - keeping as specified
    Sort_Key SMALLINT NOT NULL,
    Date_Added DATETIME NULL,
    Date_Modified DATETIME NULL,
    Updated_By INT NULL,
    Transaction_ID INT NULL
)
GO

-- Add some comments for documentation
EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Stores country information',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Country'
GO
```

## Insert Statements

```sql
-- Insert 20 sample countries
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (1, 'Afghanistan', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (2, 'Albania', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (3, 'Algeria', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (4, 'Andorra', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (5, 'Angola', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (6, 'Argentina', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (7, 'Australia', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (8, 'Austria', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (9, 'Belgium', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (10, 'Brazil', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (11, 'Canada', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (12, 'China', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (13, 'Denmark', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (14, 'France', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (15, 'Germany', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (16, 'India', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (17, 'Italy', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (18, 'Japan', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (19, 'Mexico', 1, 0);
INSERT INTO dbo.Country (Country_ID, Name, Verfied_Mode_ID, Sort_Key) VALUES (20, 'United Kingdom', 1, 0);
```
