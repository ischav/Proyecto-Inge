USE [BD_IngeGrupo1]
GO

-- Agregar Roles

INSERT INTO [dbo].[AspNetRoles]([Id],[Name]) VALUES ('01Admin','Administrador'); 

INSERT INTO [dbo].[AspNetRoles]([Id],[Name]) VALUES ('02Develop','Desarrollador');

INSERT INTO [dbo].[AspNetRoles]([Id],[Name]) VALUES ('03User','Usuario Final');

-- Agregar Privilegios

-- Modulo Gestion Usuarios

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-I', 'Agregar usuarios');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-C', 'Consultar usuarios'); 

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-M', 'Modificar usuarios'); 

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-E', 'Eliminar usuarios');

-- Modulo Proyectos

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('PRO-I', 'Agregar proyectos');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('PRO-C', 'Consultar proyectos'); 

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('PRO-M', 'Modificar proyectos');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('PRO-E', 'Eliminar proyectos');


-- Modulo Gestión de Requerimientos Funcionales

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GRF-I', 'Registrar requerimientos');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GRF-C', 'Consultar requerimientos');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GRF-M', 'Modificar requerimientos');

-- Modulo Gestión de Cambios

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GCS-CP', 'Consultar requerimientos de proyecto');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GCS-CR', 'Consultar cambios en los requerimientos');

-- Módulo Reportes

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('REP-R', 'Generar reportes');