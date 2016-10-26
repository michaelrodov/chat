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
            y: 'data1',
            colors: {
                red: '#424242',
                blue: '#a5c04d'
            },
            columns: columns,
            type: 'bar'
        }
    });
}

export function getLineChart(columns, bindto) {
    return c3.generate({
        bindto: bindto,
        line: {
            expand: true,
            label: {
                format: function (value, ratio, id) {
                    return d3.round(value, 1);
                }
            }
        },
        size: {
            width: 600,
            height: 200
        },
        data: {
            colors: {
                red: '#424242',
                blue: '#a5c04d'
            },
            columns: columns,
            type: 'line'
        }
    });
}