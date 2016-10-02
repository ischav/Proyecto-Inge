USE BD_IngeGrupo1;

CREATE TABLE Usuario (
    Cedula   	 VARCHAR(16),
    Nombre   	 VARCHAR(16),
    Apellido1    VARCHAR(16),
    Apellido2    VARCHAR(16),
    FechaNac    DATE,
    Telefono1    VARCHAR(10),
    Telefono2    VARCHAR(10),
    CONSTRAINT    PK_Usuarios PRIMARY KEY (Cedula)
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
	foreign key (Id_Privilegio) references Privilegio(Id),
	foreign key (Id_Rol) references AspNetRoles(Id),
    CONSTRAINT PK_Privilegios_asociados_roles PRIMARY KEY (Id_Privilegio, Id_Rol)
);

--Correr despues de que se han creado las tablas de ASP
ALTER TABLE Usuario
ADD Id nvarchar(128) foreign key references AspNetUsers(Id)