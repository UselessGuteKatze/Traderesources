$(document).ready(function () {

    var targetId = 'object-coords-viewer';
    var coordsString = $(`#${targetId}`).attr("coords");
    var coords = JSON.parse(coordsString);

    var $target = $(`#${targetId}`);

    $target.html(`
        <div id="coords-panel">
            <table>
                <thead>
                <tr>
                    <td class="text-h-center white-text">Долгота</td>
                    <td class="text-h-center white-text">Широта</td>
                </tr>
                </thead>
                <tbody id="coords-table-body">
                </tbody>
            </table>
        </div>
    `);

    coords.forEach(function (coord) {
        $('#coords-table-body').append(`
	      	<tr class="coord-row">
		        <td>${coord.appropriateX}</td>
		        <td>${coord.appropriateY}</td>
	    	</tr>
		`);
    });

});