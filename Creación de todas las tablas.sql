-- Creación de Tablas para el Sistema de Administración de Requerimientos
-- Curso de Ingeniería de Software I
-- Estudiantes:
--		Andreína Alvarado
--		Isabel Chaves
--		Adrián Madrigal
--		Josin Madrigal
--		José Sánchez

USE BD_IngeGrupo1;

DROP TABLE Usuarios_asociados_proyecto;
DROP TABLE Privilegios_asociados_roles;
DROP TABLE CriterioAceptacion;
DROP TABLE Cambio;
DROP TABLE Requerimiento;
DROP TABLE Usuario;
DROP TABLE Proyecto;
DROP TABLE Privilegio

CREATE TABLE Usuario (
    Cedula   	VARCHAR(16)	UNIQUE,
    Nombre   	VARCHAR(16)	NOT NULL,
    Apellido1   VARCHAR(16)	NOT NULL,
    Apellido2   VARCHAR(16),
    FechaNac	DATE,
    Telefono1   VARCHAR(10),
    Telefono2   VARCHAR(10),
	Sexo		CHAR(1),
	Id			NVARCHAR(128),
	CONSTRAINT	PK_Usuarios PRIMARY KEY (Id),
	CONSTRAINT	FK_ASPUsers FOREIGN KEY (Id) REFERENCES AspNetUsers(Id)
);

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

CREATE TABLE Proyecto (
	Id				VARCHAR(20),
	Nombre   		VARCHAR(50) 		NOT NULL,
    Descripcion		VARCHAR(100),
	Estado			VARCHAR(20)			NOT NULL,
    FechaInicio		DATE,
    FechaFinal		DATE,
	Duracion		VARCHAR(10),
	CONSTRAINT	PK_Proyecto PRIMARY KEY (Id)
);

CREATE TABLE Usuarios_asociados_proyecto(
	IdUsuario		NVARCHAR(128),
	IdProyecto		VARCHAR(20),
	RolProyecto		VARCHAR(15)			NOT NULL,
	CONSTRAINT	PK_Usuario_asociados_proyecto PRIMARY KEY (IdUsuario, IdProyecto), 
	CONSTRAINT	FK_UsuarioUsuario_asociados_proyecto FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id) ON UPDATE CASCADE,
	CONSTRAINT	FK_ProyectoUsuario_asociados_proyecto FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id) ON DELETE CASCADE
);

CREATE TABLE Requerimiento(
	Id				VARCHAR(20),
	Nombre			VARCHAR(40)			NOT NULL,
	Prioridad		VARCHAR(2),
	Esfuerzo		VARCHAR(2),
	Estado			VARCHAR(20),
	Descripcion		VARCHAR(100),
	FechaInicio		DATE,
	FechaFinal		DATE,
	Sprint			VARCHAR(2),
	Modulo			VARCHAR(30),
	Observaciones	VARCHAR(150),
	IdProyecto		VARCHAR(20),
	Imagen			IMAGE,
	IdResponsable	NVARCHAR(128),
	CONSTRAINT	PK_Requerimiento PRIMARY KEY (Id, IdProyecto),
	CONSTRAINT	FK_Usuarios FOREIGN KEY (IdResponsable) REFERENCES Usuario(Id) ON UPDATE CASCADE,
	CONSTRAINT	FK_ProyectoRequerimiento FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id) ON DELETE CASCADE
);


CREATE TABLE Cambio(
	Fecha			DATE,
	Descripcion		VARCHAR(200)		NOT NULL,
	Justificacion	VARCHAR(200)		NOT NULL,
	IdRequerimiento	VARCHAR(20),
	IdProyecto		VARCHAR(20),
	IdUsuario		NVARCHAR(128),
	CONSTRAINT	PK_Cambio PRIMARY KEY (Fecha, IdRequerimiento, IdProyecto, IdUsuario),
	CONSTRAINT	FK_RequerimientoCambio FOREIGN KEY (IdRequerimiento, IdProyecto) REFERENCES Requerimiento(Id, IdProyecto) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT	FK_UsuarioCambio FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id)
);

CREATE TABLE CriterioAceptacion(
	Escenario		VARCHAR(30),
	Descripcion		VARCHAR(200)		NOT NULL,
	IdRequerimiento VARCHAR(20),
	IdProyecto		VARCHAR(20),
	CONSTRAINT	PK_CriterioAceptacion PRIMARY KEY (Escenario, IdRequerimiento, IdProyecto),
	CONSTRAINT	FK_RequerimientoCriterioAceptacion FOREIGN KEY (IdRequerimiento, IdProyecto) REFERENCES Requerimiento(Id, IdProyecto) ON DELETE CASCADE
);
