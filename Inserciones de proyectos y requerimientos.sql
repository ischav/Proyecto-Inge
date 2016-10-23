USE BD_IngeGrupo1

-- En esta sección se insertan proyectos

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (1,'Sistema de administracion de requerimientos', 'Este software tiene como finalidad
el control de distintos proyectos', 'En proceso','2016-01-01','2016-06-01','6 meses');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (2,'BAC San Jose', 'Este proyecto tiene la finalidad de permitir a los usuarios consultar
sus datos bancarios', 'En proceso','2016-06-07','2016-11-04','4 meses');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (3,'Escuela Republicana', 'La finalidad es la creación de una página web para mantener
informados a los padres de familia', 'En proceso','2017-06-07','2016-06-07','1 año');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (4,'Promociones Supermercado ARJ', 'Con este software se pretende mostrar las promociones del 
supermercado', 'En proceso','2015-01-07','2017-01-07','1 año');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (5,'Servicios Hotel La Condesa', 'Se busca que los empleados del hotel puedan solicitar
productos y servios mediante esta herramienta','En proceso','2016-03-24','2017-01-07','9 meses');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (6,'Restaurante Milori', 'Se utilizará como herramienta para distribucion de promociones',
       'En proceso','2016-05-21','2017-05-21','1 año');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (7,'Matemáticas Online', 'Con este software se pretende que los estudiantes fortalezcan sus 
habilidades para los examenes','En proceso','2016-07-27','2018-07-27','2 años');

INSERT INTO Proyecto(Id,Nombre,Descripcion,Estado,FechaInicio,FechaFinal,Duracion) 
VALUES (8,'La Cueva en tu casa', 'Con este software se pretende realizar avaluos a
articulos antes de empeñarlos','En proceso','2016-01-11','2016-10-27','10 meses');

-- En esta sección se insertan requerimientos

INSERT INTO Requerimiento(Id,Nombre,Prioridad,Esfuerzo,Estado,Descripcion,FechaInicio,FechaFinal,Sprint,
Modulo, Observaciones,IdProyecto) VALUES (1,'Insertar proyectos',1,4,'En proceso'
,'Se desea insertar proyectos en el sistema', '2016-02-01','2016-05-12',1,3,'Ninguna',1)

INSERT INTO Requerimiento(Id,Nombre,Prioridad,Esfuerzo,Estado,Descripcion,FechaInicio,FechaFinal,Sprint,
Modulo, Observaciones,IdProyecto) VALUES (2,'Insertar promociones',1,2,'En proceso'
,'Se desea insertar las promociones del restaurante', '2016-06-10','2016-06-30',1,1,'Ninguna',6)

INSERT INTO Requerimiento(Id,Nombre,Prioridad,Esfuerzo,Estado,Descripcion,FechaInicio,FechaFinal,Sprint,
Modulo, Observaciones,IdProyecto) VALUES (3,'Modificar formula',1,6,'En proceso'
,'Se desea modificar fórmulas relacionada a temáticas', '2016-08-01','2016-09-05',1,1,'Ninguna',7)
