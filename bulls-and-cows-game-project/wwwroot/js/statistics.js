$(document).ready(function () {
    const statisticsTableBody = $('#statisticsTableBody');
    const statisticsTableContainer = $('#statisticsTableContainer');
    let currentSortColumn = 'startTime';
    let currentSortDirection = 'desc';

    //aktualni filtry
    let currentFilters = {
        year: $('#filterYear').val(),
        month: $('#filterMonth').val(),
        day: $('#filterDay').val(),
        status: $('#filterStatus').val(),
        difficulty: $('#filterDifficulty').val()
    };

    function loadGameStatistics(filters, sortColumn, sortDirection) {
        currentFilters = filters; 
        currentSortColumn = sortColumn; 
        currentSortDirection = sortDirection;

        const allFilters = { ...filters, sortBy: sortColumn, sortDir: sortDirection };
        const queryParams = new URLSearchParams(allFilters).toString();

        fetch(`/api/Statistics?${queryParams}`)
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw new Error(errorData.message || 'Error fetching statistics');
                    });
                }
                return response.json();
            })
            .then(games => {
                statisticsTableBody.empty();

                if (games.length === 0) {
                    statisticsTableBody.append('<tr><td colspan="8" class="text-center text-muted">No games found with selected filters.</td></tr>');
                    return;
                }

                function getSortArrow(column) {
                    if (currentSortColumn === column) {
                        return currentSortDirection === 'asc' ? ' &#9650;' : ' &#9660;';
                    }
                    return '';
                }

                // Aktualizujte hlavicky tabulky s indikátory razeni
                $('#statisticsTableContainer').find('.sortable-header').each(function () {
                    const headerColumn = $(this).data('sort-by');
                    let originalText = $(this).text().replace(/[\s\u25B2\u25BC]/g, '');
                    $(this).html(originalText + getSortArrow(headerColumn));
                });

                games.forEach((game, index) => {
                    const status = game.isSolved ? 'Won' : 'Lost';
                    const date = new Date(game.startTime);
                    const formattedDate = date.toLocaleDateString('cs-CZ').replace(/\s/g, "");
                    const formattedTime = date.toLocaleTimeString('cs-CZ', {
                        hour: '2-digit',
                        minute: '2-digit'
                    });

                    const newRow = `
                        <tr>
                            <td>${index + 1}</td>
                            <td>${formattedDate}</td>
                            <td>${formattedTime}</td>
                            <td>${game.duration}</td>
                            <td>${status}</td>
                            <td>${game.targetCode}</td>
                            <td>${game.totalGuesses}</td>
                            <td>${game.difficulties.join(', ')}</td> </tr>
                    `;
                    statisticsTableBody.append(newRow);
                });

                // kliknuti na hlavicku sloupcu
                statisticsTableContainer.find('.sortable-header').off('click').on('click', function () {
                    const column = $(this).data('sort-by');
                    if (currentSortColumn === column) {
                        currentSortDirection = (currentSortDirection === 'asc' ? 'desc' : 'asc');
                    } else {
                        currentSortColumn = column;
                        currentSortDirection = 'desc';
                    }
                    loadGameStatistics(currentFilters, currentSortColumn, currentSortDirection);
                });

            });
    }

    $('#filterForm').on('submit', function (e) {
        e.preventDefault();
        currentFilters = { 
            year: $('#filterYear').val(),
            month: $('#filterMonth').val(),
            day: $('#filterDay').val(),
            status: $('#filterStatus').val(),
            difficulty: $('#filterDifficulty').val()
        };
        loadGameStatistics(currentFilters, currentSortColumn, currentSortDirection);
    });


    loadGameStatistics(currentFilters, currentSortColumn, currentSortDirection); 

    const downloadButton = $('#downloadStatisticsButton');
    downloadButton.on('click', async function () {
        if (confirm('Do you want to download statistics?')) { 
            const allDownloadFilters = { ...currentFilters, sortBy: currentSortColumn, sortDir: currentSortDirection, download: true};
            const queryParams = new URLSearchParams(allDownloadFilters).toString();

            try {
                const response = await fetch(`/api/Statistics?${queryParams}`, {
                    method: 'GET',
                    headers: {
                        'Accept': 'application/xml' 
                    }
                });

                if (response.ok) {
                    const blob = await response.blob();
                    const downloadUrl = window.URL.createObjectURL(blob);

                    //docastny odkaz
                    const a = document.createElement('a');
                    a.href = downloadUrl;
                    a.download = 'statistics.xml'; 
                    document.body.appendChild(a);
                    a.click(); 

                    document.body.removeChild(a);
                    window.URL.revokeObjectURL(downloadUrl);
                } else {
                    const errorText = await response.text();
                    alert('Failed to download statistics. Error: ' + errorText);
                }
            } catch (error) {
                alert('An error occurred while downloading the file.');
            }
       
        }
    });
});