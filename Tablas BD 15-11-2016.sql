-- Creación de Tablas para el Sistema de Administración de Requerimientos
-- Curso de Ingeniería de Software I
-- Estudiantes:
--		Andreína Alvarado
--		Isabel Chaves
--		Adrián Madrigal
--		Josin Madrigal
--		José Sánchez

USE BD_IngeGrupo1;

--------------
-- USUARIOS --
--------------
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

-----------------
-- PRIVILEGIOS --
-----------------
CREATE TABLE Privilegio (
    Id   		 NVARCHAR(128),
    Descripcion    VARCHAR(80),
    CONSTRAINT    PK_Privilegio PRIMARY KEY (Id)
);


-- Correr despues de que se han creado las tablas de ASP

--------------------------
-- PRIVILEGIOS -> ROLES --
--------------------------
CREATE TABLE Privilegios_asociados_roles (
    Id_Privilegio	NVARCHAR(128),
	Id_Rol			NVARCHAR(128),
	CONSTRAINT	FK_Privilegio FOREIGN KEY (Id_Privilegio) REFERENCES Privilegio(Id),
	CONSTRAINT	FK_AspRoles FOREIGN KEY	(Id_Rol) REFERENCES AspNetRoles(Id),
    CONSTRAINT	PK_Privilegios_asociados_roles PRIMARY KEY (Id_Privilegio, Id_Rol)
);

---------------
-- PROYECTOS --
---------------
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

--------------------------
-- USUARIOS -> PROYECTO --
--------------------------
CREATE TABLE Usuarios_asociados_proyecto(
	IdUsuario		NVARCHAR(128),
	IdProyecto		VARCHAR(20),
	RolProyecto		VARCHAR(15)			NOT NULL,
	CONSTRAINT	PK_Usuario_asociados_proyecto PRIMARY KEY (IdUsuario, IdProyecto), 
	CONSTRAINT	FK_UsuarioUsuario_asociados_proyecto FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id) ON UPDATE CASCADE,
	CONSTRAINT	FK_ProyectoUsuario_asociados_proyecto FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id) ON DELETE CASCADE
);

--------------------
-- REQUERIMIENTOS --
--------------------
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
ALTER TABLE Requerimiento ADD IdSolicitante NVARCHAR(128); 
ALTER TABLE Requerimiento DROP CONSTRAINT FK_Usuarios;
ALTER TABLE Requerimiento ADD CONSTRAINT FK_UsuarioResponsable FOREIGN KEY (IdResponsable) REFERENCES Usuario(Id);
ALTER TABLE Requerimiento ADD CONSTRAINT FK_UsuarioSolicitante FOREIGN KEY (IdSolicitante) REFERENCES Usuario(Id);

------------------------------------------------------
--	CAMBIO REALIZADO A REQUERIMIENTOS EL 5-11-2016 --
------------------------------------------------------
ALTER TABLE Requerimiento ADD Version int DEFAULT 1;

-----------------------------
-- CRITERIOS DE ACEPTACION --
-----------------------------
CREATE TABLE CriterioAceptacion(
	Escenario		VARCHAR(30),
	Descripcion		VARCHAR(200)		NOT NULL,
	IdRequerimiento VARCHAR(20),
	IdProyecto		VARCHAR(20),
	CONSTRAINT	PK_CriterioAceptacion PRIMARY KEY (Escenario, IdRequerimiento, IdProyecto),
	CONSTRAINT	FK_RequerimientoCriterioAceptacion FOREIGN KEY (IdRequerimiento, IdProyecto) REFERENCES Requerimiento(Id, IdProyecto) ON DELETE CASCADE
);


--------------------------
-- HISTORIAL DE CAMBIOS --
--------------------------
CREATE TABLE Cambio(
	IdSolicitud			INT		IDENTITY(1,1),
	-- Este IdSolicitud debe manejarse internamente, no se muestra. 
	-- Además, inicia en 1 y va incrementando automáticamente de 1 en 1.
	IdRequerimiento		VARCHAR(20),
	IdProyecto			VARCHAR(20),
	Nombre				VARCHAR(40)			NOT NULL,
	Prioridad			VARCHAR(2),
	Esfuerzo			VARCHAR(2),
	Estado				VARCHAR(20),
	Descripcion			VARCHAR(100),
	FechaInicio			DATE,
	FechaFinal			DATE,
	Sprint				VARCHAR(2),
	Modulo				VARCHAR(30),
	Observaciones		VARCHAR(150),
	Imagen				IMAGE,
	IdResponsable		NVARCHAR(128),
	IdSolicitante		NVARCHAR(128),
	Version				INT,
	DescripcionCambio	VARCHAR(200)		NOT NULL,
	JustificacionCambio VARCHAR(200)		NOT NULL,
	SolicitanteCambio	NVARCHAR(128)		NOT NULL,
	FechaCambio			DATE,
	EstadoSolicitud		VARCHAR(15)			NOT NULL,-- Válidos: Pendiente, En revisión, Aceptado, Rechazado.
	CONSTRAINT PK_Cambios PRIMARY KEY(IdSolicitud, IdRequerimiento, IdProyecto),
	CONSTRAINT FK_UsuarioCambio FOREIGN KEY(IdResponsable) REFERENCES Usuario(Id),
	CONSTRAINT FK_UsuarioCambioSolicitanteReq FOREIGN KEY(IdSolicitante) REFERENCES Usuario(Id),
	CONSTRAINT FK_UsuarioCambiosSolicitante FOREIGN KEY(SolicitanteCambio) REFERENCES Usuario(Id),
	CONSTRAINT FK_IdRequerimientoCambios FOREIGN KEY(IdRequerimiento, IdProyecto) REFERENCES Requerimiento(Id, IdProyecto)
);

ALTER TABLE Cambio ADD ObservacionesSolicitud VARCHAR(400);
ALTER TABLE Cambio ADD VersionCambio INT;

CREATE TABLE CriterioAceptacionHistorial(
	Escenario		VARCHAR(30),
	Descripcion		VARCHAR(200)		NOT NULL,
	IdRequerimiento VARCHAR(20),
	IdProyecto		VARCHAR(20),
	IdSolicitud		INT,
	CONSTRAINT	PK_CriterioAceptacionHistorial PRIMARY KEY (Escenario,IdSolicitud, IdRequerimiento, IdProyecto),
	CONSTRAINT	FK_CriterioAceptacionCambio FOREIGN KEY (IdSolicitud, IdRequerimiento, IdProyecto) REFERENCES Cambio(IdSolicitud, IdRequerimiento, IdProyecto) ON DELETE CASCADE
);