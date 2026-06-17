CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
ALTER DATABASE CHARACTER SET utf8mb4;

CREATE TABLE `Location` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `SourceApiId` bigint NULL,
    `Latitude` double NULL,
    `Longitude` double NULL,
    `Altitude` double NULL,
    `Country` longtext CHARACTER SET utf8mb4 NULL,
    `Indoor` tinyint(1) NULL,
    `ExactLocation` tinyint(1) NULL,
    CONSTRAINT `PK_Location` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `SensorType` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `SourceApiId` bigint NULL,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Manufacturer` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_SensorType` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `Sensor` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `SourceApiId` bigint NULL,
    `Pin` int NULL,
    `SensorTypeId` bigint NULL,
    CONSTRAINT `PK_Sensor` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Sensor_SensorType_SensorTypeId` FOREIGN KEY (`SensorTypeId`) REFERENCES `SensorType` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `SensorModel` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `SourceApiId` bigint NULL,
    `Device` varchar(50) CHARACTER SET utf8mb4 NULL,
    `Pm1` decimal(18,2) NULL,
    `Pm25` decimal(18,2) NULL,
    `Pm10` decimal(18,2) NULL,
    `Timestamp` datetime NULL,
    `Latitude` decimal(18,8) NULL,
    `Longitude` decimal(18,8) NULL,
    `Ijp` decimal(18,8) NULL,
    `IjpStringEn` varchar(255) CHARACTER SET utf8mb4 NULL,
    `IjpString` varchar(255) CHARACTER SET utf8mb4 NULL,
    `IjpDescription` varchar(512) CHARACTER SET utf8mb4 NULL,
    `IjpDescriptionEn` varchar(512) CHARACTER SET utf8mb4 NULL,
    `Color` varchar(20) CHARACTER SET utf8mb4 NULL,
    `Temperature` decimal(18,2) NULL,
    `Humidity` decimal(18,2) NULL,
    `AveragePm1` decimal(18,2) NULL,
    `AveragePm25` decimal(18,2) NULL,
    `AveragePm10` decimal(18,2) NULL,
    `Name` varchar(100) CHARACTER SET utf8mb4 NULL,
    `Indoor` bit NULL,
    `PreviousIjp` varchar(50) CHARACTER SET utf8mb4 NULL,
    `Hcho` decimal(18,2) NULL,
    `AverageHcho` decimal(18,2) NULL,
    `LocationName` varchar(50) CHARACTER SET utf8mb4 NULL,
    `SamplingRate` decimal(18,2) NULL,
    `LocationId` bigint NULL,
    `SensorId` bigint NULL,
    `SensorDataValuesIds` longtext CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_SensorModel` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SensorModel_Location_LocationId` FOREIGN KEY (`LocationId`) REFERENCES `Location` (`Id`),
    CONSTRAINT `FK_SensorModel_Sensor_SensorId` FOREIGN KEY (`SensorId`) REFERENCES `Sensor` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `SensorDataValues` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `SourceApiId` bigint NULL,
    `Value` double NULL,
    `ValueType` longtext CHARACTER SET utf8mb4 NOT NULL,
    `SensorModelId` bigint NULL,
    CONSTRAINT `PK_SensorDataValues` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SensorDataValues_SensorModel_SensorModelId` FOREIGN KEY (`SensorModelId`) REFERENCES `SensorModel` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Sensor_SensorTypeId` ON `Sensor` (`SensorTypeId`);

CREATE INDEX `IX_SensorDataValues_SensorModelId` ON `SensorDataValues` (`SensorModelId`);

CREATE UNIQUE INDEX `IX_SensorModel_Device` ON `SensorModel` (`Device`);

CREATE UNIQUE INDEX `IX_SensorModel_Id` ON `SensorModel` (`Id`);

CREATE INDEX `IX_SensorModel_LocationId` ON `SensorModel` (`LocationId`);

CREATE INDEX `IX_SensorModel_SensorId` ON `SensorModel` (`SensorId`);

CREATE UNIQUE INDEX `IX_SensorModel_SourceApiId` ON `SensorModel` (`SourceApiId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260617185423_InitialMariaDb', '9.0.0');

COMMIT;

