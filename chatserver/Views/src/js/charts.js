var lineChartObject;

export function generateLineChart(columns, bindto) {
    return c3.generate({
        bindto: bindto,
        axis: {
            y: {
                show: false
            }
        },
        legend: {
            position: 'right'
        },
        size: {
            width: 600,
            height: 100
        },
        data: {
            colors: {
                red: '#424242',
                blue: '#a5c04d'
            },
            columns: columns,
            type: 'bar',
            axes: {
                mph: 'y',
                wph: 'y2'
            }
        }
    });
}

