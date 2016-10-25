-- Creación de Tablas para el Sistema de Administración de Requerimientos
-- Curso de Ingeniería de Software I
-- Estudiantes:
--		Andreína Alvarado
--		Isabel Chaves
--		Adrián Madrigal
--		Josin Madrigal
--		José Sánchez

USE BD_IngeGrupo1;

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
	CedulaUsuario	VARCHAR(16),
	IdUsuario		NVARCHAR(128),
	IdProyecto		VARCHAR(20),
	RolProyecto		VARCHAR(15)			NOT NULL,
	CONSTRAINT	PK_Usuario_asociados_proyecto PRIMARY KEY (CedulaUsuario, IdUsuario, IdProyecto), 
	CONSTRAINT	FK_UsuarioUsuario_asociados_proyecto FOREIGN KEY (CedulaUsuario, IdUsuario) REFERENCES Usuario(Cedula, Id),
	CONSTRAINT	FK_ProyectoUsuario_asociados_proyecto FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id)
);

ALTER TABLE Usuarios_asociados_proyecto DROP CONSTRAINT FK_ProyectoUsuario_asociados_proyecto;
ALTER TABLE Usuarios_asociados_proyecto ADD CONSTRAINT FK_ProyectoUsuario_asociados_proyecto FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id) ON DELETE CASCADE;

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
	CONSTRAINT	PK_Requerimiento PRIMARY KEY (Id),
	CONSTRAINT	FK_ProyectoRequerimiento FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id)
);

ALTER TABLE Requerimiento ADD Imagen image;

ALTER TABLE Requerimiento DROP CONSTRAINT FK_ProyectoRequerimiento;
ALTER TABLE Requerimiento ADD CONSTRAINT FK_ProyectoRequerimiento FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id) ON DELETE CASCADE;

CREATE TABLE Cambio(
	Fecha			DATE,
	Descripcion		VARCHAR(200)		NOT NULL,
	Justificacion	VARCHAR(200)		NOT NULL,
	IdRequerimiento	VARCHAR(20),
	CedulaUsuario	VARCHAR(16),
	IdUsuario		NVARCHAR(128),
	CONSTRAINT	PK_Cambio PRIMARY KEY (Fecha, IdRequerimiento),
	CONSTRAINT	FK_RequerimientoCambio FOREIGN KEY (IdRequerimiento) REFERENCES Requerimiento(Id),
	CONSTRAINT	FK_UsuarioCambio FOREIGN KEY (CedulaUsuario, IdUsuario) REFERENCES Usuario(Cedula, Id)
);

ALTER TABLE Cambio DROP CONSTRAINT FK_RequerimientoCambio;
ALTER TABLE Cambio ADD CONSTRAINT	FK_RequerimientoCambio FOREIGN KEY (IdRequerimiento) REFERENCES Requerimiento(Id) ON DELETE CASCADE;

CREATE TABLE CriterioAceptacion(
	Escenario		VARCHAR(30),
	Descripcion		VARCHAR(200)		NOT NULL,
	IdRequerimiento VARCHAR(20),
	CONSTRAINT	PK_CriterioAceptacion PRIMARY KEY (Escenario, IdRequerimiento),
	CONSTRAINT	FK_RequerimientoCriterioAceptacion FOREIGN KEY (IdRequerimiento) REFERENCES Requerimiento(Id)
);

ALTER TABLE CriterioAceptacion DROP CONSTRAINT FK_RequerimientoCriterioAceptacion;
ALTER TABLE CriterioAceptacion ADD CONSTRAINT	FK_RequerimientoCriterioAceptacion FOREIGN KEY (IdRequerimiento) REFERENCES Requerimiento(Id) ON DELETE CASCADE;
