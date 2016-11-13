$('#agregar').click(function (e) {
    var seleccion = $('#recursos option:selected');
    if (seleccion.length == 0) {
        alert("Debe seleccionar un recurso");
        e.preventDefault();
    }

    $('#equipoDesarrollo').append($(seleccion).clone());
    $(seleccion).remove();
    e.preventDefault()
});

$('#quitar').click(function (e) {
    var seleccion = $('#equipoDesarrollo option:selected');
    if (seleccion.length == 0) {
        alert("Debe seleccionar un desarrollador");
        e.preventDefault();
    }

    $('#recursos').append($(seleccion).clone());
    $(seleccion).remove();
    e.preventDefault()
});

$('#guardar').click(function (e) {
    $('#recursos option').prop('selected', true)
    $('#equipoDesarrollo option').prop('selected', true)
});