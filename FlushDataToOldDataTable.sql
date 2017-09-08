DECLARE @lastQuarter bigint
DECLARE @nextQuarter bigint
DECLARE @newQuarter varchar(50)
DECLARE @year int
SELECT @lastQuarter = Substring(Quarter,2,1)
FROM
(SELECT TOP 1 *
FROM EmployeeDB.dbo.OldGameData
ORDER by SUBSTRING(Quarter,3,4) DESC,SUBSTRING(Quarter,2,1) DESC) AS DataSoFar
SELECT CONVERT(bigint,@lastQuarter)
SET @year=YEAR(GetDate())
if @lastQuarter<4
BEGIN
SET @nextQuarter = @lastQuarter +1
END
ELSE
BEGIN
SET @nextQuarter=1
END
SET @newQuarter= CONCAT('Q',@nextQuarter,@year);
INSERT INTO  EmployeeDB.dbo.OldGameData 
SELECT @newQuarter,MoveId,GameId,EmployeePK,UnguessedEmployees,GuessedEmployees,PointsSoFar
FROM EmployeeDB.dbo.Games	
WHERE GameId in
(SELECT GameId
FROM [EmployeeDB].[dbo].[Games]
WHERE UnguessedEmployees='[]')
ORDER by MoveId

DELETE 
FROM EmployeeDB.dbo.Games
WHERE GameId in
(SELECT GameId
FROM [EmployeeDB].[dbo].[Games]
WHERE UnguessedEmployees='[]')