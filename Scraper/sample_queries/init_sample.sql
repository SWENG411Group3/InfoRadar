/*
    Run on SQL server to initialize the DB and Lighthouse for the lighthouse "example"
*/

USE info_radar;

CREATE TABLE Sample_Lighthouse (
	Id INT PRIMARY KEY IDENTITY,
	Created DATETIME NOT NULL,
	Field_Value1 INT,
	Field_Value2 INT,
);

INSERT INTO Lighthouses (InternalName, Title, Frequency, Created)
	VALUES ('sample', 'Sample Lighthouse', 86400, GETDATE());
