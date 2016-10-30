USE BD_IngeGrupo1;

DROP TABLE --asociado a cambios;
DROP TABLE Cambio;
DROP TABLE CriterioAceptacion;
DROP TABLE Requerimiento;


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


CREATE TABLE CriterioAceptacion(
	Escenario		VARCHAR(30),
	Descripcion		VARCHAR(200)		NOT NULL,
	IdRequerimiento VARCHAR(20),
	IdProyecto		VARCHAR(20),
	CONSTRAINT	PK_CriterioAceptacion PRIMARY KEY (Escenario, IdRequerimiento, IdProyecto),
	CONSTRAINT	FK_RequerimientoCriterioAceptacion FOREIGN KEY (IdRequerimiento, IdProyecto) REFERENCES Requerimiento(Id, IdProyecto) ON DELETE CASCADE
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
