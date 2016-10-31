-- Inserción de privilegios para el Sistema de Administración de Requerimientos
-- Curso de Ingeniería de Software I
-- Estudiantes:
--		Andreína Alvarado
--		Isabel Chaves
--		Adrián Madrigal
--		Josin Madrigal
--		José Sánchez

USE [BD_IngeGrupo1]
GO

-- Agregar Roles

INSERT INTO [dbo].[AspNetRoles]([Id],[Name]) VALUES ('01Admin','Administrador'); 

INSERT INTO [dbo].[AspNetRoles]([Id],[Name]) VALUES ('02Develop','Desarrollador');

INSERT INTO [dbo].[AspNetRoles]([Id],[Name]) VALUES ('03User','Usuario Final');


-- Agregar Privilegios por Módulo:

-- Modulo Gestion Usuarios

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-I', 'Agregar usuarios');

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-C', 'Consultar usuarios'); 

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-M', 'Modificar usuarios'); 

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('GUS-E', 'Eliminar usuarios');

-- Manejo de Privilegios

INSERT INTO [dbo].[Privilegio] ([Id],[Descripcion]) VALUES ('SEG-I', 'Gestionar privilegios');

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








--********************Asignacion de todos los privilegios al rol administrador

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GUS-I', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GUS-C', '01Admin'); 

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GUS-M', '01Admin'); 

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GUS-E', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('SEG-I', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('PRO-I', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('PRO-C', '01Admin'); 

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('PRO-M', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('PRO-E', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GRF-I', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GRF-C', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GRF-M', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('REP-R', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GCS-CP', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('GCS-CR', '01Admin');

INSERT INTO [dbo].[Privilegios_asociados_roles] ([Id_Privilegio],[Id_Rol]) VALUES ('REP-R', '01Admin');