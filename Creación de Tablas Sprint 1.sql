-- Creación de Tablas para el Sistema de Administración de Requerimientos
-- Curso de Ingeniería de Software I
-- Estudiantes:
--		Andreína Alvarado
--		Isabel Chaves
--		Adrián Madrigal
--		Josin Madrigal
--		José Sánchez

USE BD_IngeGrupo1;

CREATE TABLE Usuario (
    Cedula   	VARCHAR(16),
    Nombre   	VARCHAR(16),
    Apellido1   VARCHAR(16),
    Apellido2   VARCHAR(16),
    FechaNac	DATE,
    Telefono1   VARCHAR(10),
    Telefono2   VARCHAR(10),
	Sexo		CHAR(1),
	Id			NVARCHAR(128),
	CONSTRAINT	PK_Usuarios PRIMARY KEY (Cedula, Id),
	CONSTRAINT	FK_ASPUsers FOREIGN KEY (Id) REFERENCES AspNetUsers(Id)
);

ALTER TABLE Usuario DROP CONSTRAINT FK_ASPUsers;
ALTER TABLE Usuario ADD CONSTRAINT FK_ASPUsers FOREIGN KEY (Id) REFERENCES AspNetUsers(Id) ON DELETE CASCADE; 

CREATE TABLE Privilegio (
    Id   		 NVARCHAR(128),
    Descripcion    VARCHAR(80),
    CONSTRAINT    PK_Privilegio PRIMARY KEY (Id)
);

--Correr despues de que se han creado las tablas de ASP
CREATE TABLE Privilegios_asociados_roles (
    Id_Privilegio	NVARCHAR(128),
	Id_Rol			NVARCHAR(128),
	CONSTRAINT	FK_Privilegio FOREIGN KEY (Id_Privilegio) REFERENCES Privilegio(Id),
	CONSTRAINT	FK_AspRoles FOREIGN KEY	(Id_Rol) REFERENCES AspNetRoles(Id),
    CONSTRAINT	PK_Privilegios_asociados_roles PRIMARY KEY (Id_Privilegio, Id_Rol)
);

ALTER TABLE Privilegios_asociados_roles DROP CONSTRAINT FK_Privilegio;
ALTER TABLE Privilegios_asociados_roles ADD CONSTRAINT FK_Privilegio FOREIGN KEY (Id_Privilegio) REFERENCES Privilegio(Id) ON DELETE CASCADE; 

ALTER TABLE Privilegios_asociados_roles DROP CONSTRAINT FK_AspRoles;
ALTER TABLE Privilegios_asociados_roles ADD CONSTRAINT	FK_AspRoles FOREIGN KEY	(Id_Rol) REFERENCES AspNetRoles(Id);
