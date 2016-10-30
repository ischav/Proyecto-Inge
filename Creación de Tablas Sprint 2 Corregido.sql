
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
