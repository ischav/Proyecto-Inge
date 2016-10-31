USE BD_IngeGrupo1;

-- Inserción de criterios de aceptación

-- Formato para el Escenario = PRO-[IdProyecto]-RF-[IdRequerimiento]-[NumCriterio]

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-9-1','Se ingresan datos válidos y se cumple con los requisitos',9, 1);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-9-2','Se ingresan datos no válidos o no cumple con los requisitos',9, 1);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-9-3','Presiona el botón CANCELAR y debe volver a la pantalla anterior',9, 1);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-7-RF-6-1','Marcar los campos que desea exportar',6, 7);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-7-RF-6-2','Desmarcar los campos que desea exportar',6, 7);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-7-RF-6-3','Generar un documento PDF al presionar el botón EXPORTAR',6, 7);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-1-1','Agregar todos los campos requeridos y presionar el botón ACEPTAR',1, 1);