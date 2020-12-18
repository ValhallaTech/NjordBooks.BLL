$(document).ready(function () {
    $('.datatable').DataTable();
    var table = $('#recentTransactions').DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'excel', 'pdf'
        ]
    });
    table.buttons().container()
        .appendTo($('.col-sm-6:eq(0)', table.table().container()));

    document.getElementById("years").addEventListener('change', function () {
        this.form.submit();
    });
    document.getElementById("months").addEventListener('change', function () {
        this.form.submit();
    });
});

function CanInvite() {
    swal("Good news!", "Your invitation has been sent.", "success");
}

function CantInvite() {
    swal("Sorry!", "You can't invite someone who is already in a household.", "error");
}

function CantLeave() {
    swal("Sorry!", "You can't leave while the house still has members.", "error");
}

function Overdraft() {
    swal("Overdraft!", "Details have been sent to your email.", "error");
}

function getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function hexToRgb(hex) {
    var bigint = parseInt(hex, 16);
    var r = (bigint >> 16) & 255;
    var g = (bigint >> 8) & 255;
    var b = bigint & 255;
    return r + "," + g + "," + b;
}

function NewColor(category) {   
    if (window.localStorage.getItem(category) == null) {
        let hex = getRandomColor();
        let rgb = hexToRgb(hex);
        window.localStorage.setItem(category, rgb);
    }
}

function NewHexColor(category) {
    if (window.localStorage.getItem(category) == null) {
        let hex = getRandomColor();
        window.localStorage.setItem(category, `#${hex}`);
    }
}

function BudgetBreakdownChart(Url) {
    let names = [];
    let totals = [];
    let backgroundColors = [];
    let borderColors = [];
    $.post(Url).then(function (res) {
        for (let i = 0; i < res.length; i++) {
            names.push(res[i].name);
            totals.push(res[i].total);

            let color = window.localStorage.getItem(res[i].name);
            if (color == null) {
                NewColor(res[i].name);
                color = window.localStorage.getItem(res[i].name);
            }

            backgroundColors.push(`rgba(${color},0.85)`);
            borderColors.push(`rgba(${color},1)`);
        }
    }).then(() => {
        // For a pie chart
        var ctx = document.getElementById('breakdownPieChart').getContext('2d');
        var myPieChart = new Chart(ctx, {
            type: 'pie',
            data: {
                datasets: [{
                    data: totals,
                    backgroundColor: backgroundColors,
                    borderColor: borderColors,
                    borderWidth: 1
                }],
                labels: names
            }
        });
    });
}

function CategoryItemsChart(Url) {
    let labels = [];
    let goal = [];
    let reality = [];

    let bar1Border = [];
    let bar1Background = [];
    let bar2Border = [];
    let bar2Background = [];

    $.post(Url).then(function (res) {
        for (let i = 0; i < res.length; i++) {
            labels.push(res[i].name);
            goal.push(res[i].goal);
            reality.push(res[i].reality);

            let color = window.localStorage.getItem(res[i].category);
            if (color == null) {
                NewColor(res[i].category);
                color = window.localStorage.getItem(res[i].category);
            }
            bar1Border.push(`rgba(${color},1)`);
            bar1Background.push(`rgba(${color},0.1)`);
            bar2Border.push(`rgba(${color},1)`);
            bar2Background.push(`rgba(${color},0.85)`);
        }
    }).then(() => {
        var ctx = document.getElementById('itemsBarChart').getContext('2d');
        var myChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'budget',
                    data: goal,
                    backgroundColor: bar1Background,
                    borderColor: bar1Border,
                    borderWidth: 1
                },
                {
                    label: 'spent',
                    data: reality,
                    backgroundColor: bar2Background,
                    borderColor: bar2Border,
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            }
        });
    })
}

$('#historyModal').on('shown.bs.modal', function () {
    let dates = [];
    let graphThis = [];

    $.post("/Charts/History").then(function (res) {
        dates = res.dates;
        // each line
        for (let i = 0; i < res.lines.length; i++) {

            // raw coordinates as they've come from the controller
            let xCords = [];
            let yCords = [];
            for (let j = 0; j < res.lines[i].xcords.length; j++) {
                xCords.push( res.lines[i].xcords[j] );
                yCords.push( res.lines[i].ycords[j] );
            }

            // ensure Y-val for each date
            for (let k = 0; k < dates.length; k++) {
                let date = dates[k];
                let index = dates.indexOf(date);

                // if missing a date
                if (xCords.indexOf(date) == -1) {
                    // add date
                    xCords.splice(index, 0, date);
                    // add Y value
                    if (yCords[k - 1] == null) {
                        yCords.splice(index, 0, yCords[k]);
                    }
                    else {
                        yCords.splice(index, 0, yCords[k - 1]);
                    }
                }
            }

            let color = window.localStorage.getItem(res.lines[i].name);
            if (color == null) {
                NewHexColor(res.lines[i].name);
                color = window.localStorage.getItem(res.lines[i].name);
            }

            // line graph needs array of objects
            let set = {
                data: yCords,
                label: res.lines[i].name,
                borderColor: color,
                fill: false
            };

            // graphThis is an array of objects -> [ {  } ]
            graphThis.push(set);
        }
    })

    .then(() => {
        new Chart(document.getElementById("lineChart"), {
            type: 'line',
            data: {
                labels: dates,
                datasets: graphThis
            }
        });
    });
});

function Wizard() {
    var wizard = $("#wizard");

    $("#wizardModal").modal().on('shown.bs.modal', function () {

        wizard.steps({
            cssClass: 'pill wizard',
            //stepsOrientation: "vertical",

            headerTag: "h3",
            bodyTag: "section",
            transitionEffect: "slideLeft",
            autoFocus: true,

            onFinished: function (event, currentIndex) {
                $("#accountSetup").submit();
            }
        });
    })
}
