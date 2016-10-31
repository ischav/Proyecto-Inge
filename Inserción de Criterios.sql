USE BD_IngeGrupo1;

-- Inserci�n de criterios de aceptaci�n

-- Formato para el Escenario = PRO-[IdProyecto]-RF-[IdRequerimiento]-[NumCriterio]

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-9-1','Se ingresan datos v�lidos y se cumple con los requisitos',9, 1);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-9-2','Se ingresan datos no v�lidos o no cumple con los requisitos',9, 1);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-9-3','Presiona el bot�n CANCELAR y debe volver a la pantalla anterior',9, 1);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-7-RF-6-1','Marcar los campos que desea exportar',6, 7);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-7-RF-6-2','Desmarcar los campos que desea exportar',6, 7);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-7-RF-6-3','Generar un documento PDF al presionar el bot�n EXPORTAR',6, 7);

INSERT INTO CriterioAceptacion(Escenario, Descripcion, IdRequerimiento, IdProyecto)
VALUES ('PRO-1-RF-1-1','Agregar todos los campos requeridos y presionar el bot�n ACEPTAR',1, 1);