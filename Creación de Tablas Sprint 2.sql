USE BD_IngeGrupo1;

CREATE TABLE Proyecto (
	Id				VARCHAR(20),
    Nombre   		VARCHAR(50),
    Descripcion		VARCHAR(100),
	Estado			VARCHAR(20),
    FechaInicio		DATE,
    FechaFinal		DATE,
	Duracion		VARCHAR(10),
	CONSTRAINT	PK_Proyecto PRIMARY KEY (Id)
);

CREATE TABLE Usuarios_asociados_proyecto(
	CedulaUsuario	VARCHAR(16),
	IdUsuario		NVARCHAR(128),
	IdProyecto		VARCHAR(20),
	RolProyecto		VARCHAR(15),
	CONSTRAINT	PK_Usuario_asociados_proyecto PRIMARY KEY (CedulaUsuario, IdUsuario, IdProyecto), 
	CONSTRAINT	FK_Usuario FOREIGN KEY (CedulaUsuario, IdUsuario) REFERENCES Usuario(Cedula, Id),
	CONSTRAINT	FK_Proyecto FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id)
);

CREATE TABLE Requerimiento(
	Id				VARCHAR(20),
	Nombre			VARCHAR(40),
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
	CONSTRAINT	FK_Proyecto FOREIGN KEY (IdProyecto) REFERENCES Proyecto(Id)
);

CREATE TABLE Cambio(
	Fecha			DATE,
	Descripcion		VARCHAR(200),
	Justificacion	VARCHAR(200),
	IdRequerimiento	VARCHAR(20),
	CONSTRAINT	PK_Cambio PRIMARY KEY (Fecha, IdRequerimiento),
	CONSTRAINT	FK_Cambio FOREIGN KEY (IdRequerimiento) REFERENCES Requerimiento(Id) 
);

CREATE TABLE Usuarios_asociados_cambios(
	CedulaUsuario	VARCHAR(16),
	IdUsuario		NVARCHAR(128),
	FechaCambio		DATE,
	CONSTRAINT  PK_Usuarios_asociados_cambios PRIMARY KEY (CedulaUsuario, IdUsuario, FechaCambio),
	CONSTRAINT	FK_Usuario FOREIGN KEY (CedulaUsuario, IdUsuario) REFERENCES Usuario(Cedula, Id),
	CONSTRAINT  FK_Cambio FOREIGN KEY (FechaCambio) REFERENCES Cambio(Fecha)
);

CREATE TABLE CriterioAceptacion(
	Escenario		VARCHAR(30),
	Descripcion		VARCHAR(200),
	IdRequerimiento VARCHAR(20)
	CONSTRAINT	PK_CriterioAceptacion PRIMARY KEY (Escenario, IdRequerimiento),
	CONSTRAINT	FK_Requerimiento FOREIGN KEY (IdRequerimiento) REFERENCES Requerimiento(Id)
);