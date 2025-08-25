// Cấu hình font mặc định cho Chart.js v4 - CÁCH ĐÚNG
$(document).ready(function () {
    // Kiểm tra Chart.js đã load chưa
    if (typeof Chart === 'undefined') {
        console.error('Chart.js chưa được load!');
        return;
    }

    // Cấu hình defaults cho Chart.js v4
    try {
        Chart.defaults.font = Chart.defaults.font || {};
        Chart.defaults.font.family = 'Nunito, -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif';
        Chart.defaults.color = '#858796';
    } catch (e) {
        console.warn('Không thể set Chart defaults:', e);
    }

    // Hàm định dạng số
    function number_format(number, decimals, dec_point, thousands_sep) {
        number = (number + '').replace(',', '').replace(' ', '');
        var n = !isFinite(+number) ? 0 : +number,
            prec = !isFinite(+decimals) ? 0 : Math.abs(decimals),
            sep = (typeof thousands_sep === 'undefined') ? ',' : thousands_sep,
            dec = (typeof dec_point === 'undefined') ? '.' : dec_point,
            s = '',
            toFixedFix = function (n, prec) {
                var k = Math.pow(10, prec);
                return '' + Math.round(n * k) / k;
            };
        s = (prec ? toFixedFix(n, prec) : '' + Math.round(n)).split('.');
        if (s[0].length > 3) {
            s[0] = s[0].replace(/\B(?=(?:\d{3})+(?!\d))/g, sep);
        }
        if ((s[1] || '').length < prec) {
            s[1] = s[1] || '';
            s[1] += new Array(prec - s[1].length + 1).join('0');
        }
        return s.join(dec);
    }

    // Kiểm tra element tồn tại
    const canvasElement = document.getElementById("myAreaChart");
    if (!canvasElement) {
        console.error('Không tìm thấy element với id "myAreaChart"');
        return;
    }

    // Destroy chart cũ nếu tồn tại
    const existingChart = Chart.getChart(canvasElement);
    if (existingChart) {
        existingChart.destroy();
        console.log('Destroyed existing Area Chart');
    }

    // Gọi API
    fetch("/api/ChartsApi/area-chart-data")
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(res => {
            if (!res.success) throw new Error("Lỗi khi lấy dữ liệu: " + (res.message || "Unknown error"));

            const chartData = res.data;
            const labels = chartData.labels.map(label => label.replace("Tháng ", ""));
            const revenues = chartData.datasets[0].data;

            const ctx = canvasElement.getContext('2d');

            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: "Doanh thu",
                        data: revenues,
                        tension: 0.3,
                        backgroundColor: "rgba(78, 115, 223, 0.05)",
                        borderColor: "rgba(78, 115, 223, 1)",
                        pointRadius: 3,
                        pointBackgroundColor: "rgba(78, 115, 223, 1)",
                        pointBorderColor: "rgba(78, 115, 223, 1)",
                        pointHoverRadius: 3,
                        pointHoverBackgroundColor: "rgba(78, 115, 223, 1)",
                        pointHoverBorderColor: "rgba(78, 115, 223, 1)",
                        pointHitRadius: 10,
                        pointBorderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    layout: {
                        padding: { left: 10, right: 25, top: 25, bottom: 0 }
                    },
                    plugins: {
                        legend: {
                            display: true,
                            labels: {
                                color: '#333',
                                font: {
                                    family: 'Nunito, sans-serif'
                                }
                            }
                        },
                        tooltip: {
                            backgroundColor: "rgb(255,255,255)",
                            bodyColor: "#858796",
                            titleColor: '#6e707e',
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            xPadding: 15,
                            yPadding: 15,
                            displayColors: false,
                            mode: 'index',
                            intersect: false,
                            callbacks: {
                                label: function (context) {
                                    return 'Doanh thu: ' + number_format(context.parsed.y) + ' đ';
                                }
                            }
                        },
                        title: {
                            display: false
                        }
                    },
                    scales: {
                        x: {
                            ticks: {
                                maxRotation: 45,
                                minRotation: 45,
                                maxTicksLimit: 12,
                                color: '#858796',
                                font: {
                                    family: 'Nunito, sans-serif'
                                }
                            },
                            grid: {
                                display: false,
                                drawBorder: false
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: 'Doanh thu (VNĐ)',
                                color: '#858796',
                                font: {
                                    family: 'Nunito, sans-serif'
                                }
                            },
                            ticks: {
                                callback: function (value) {
                                    return number_format(value) + ' đ';
                                },
                                padding: 10,
                                maxTicksLimit: 5,
                                color: '#858796',
                                font: {
                                    family: 'Nunito, sans-serif'
                                }
                            },
                            grid: {
                                color: "rgb(234, 236, 244)",
                                borderDash: [2],
                                drawBorder: false
                            }
                        }
                    }
                }
            });

            console.log("Area Chart loaded successfully");
        })
        .catch(err => {
            console.error("Lỗi khi tải biểu đồ Area Chart:", err);
            const ctx = canvasElement.getContext('2d');
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: ['Không có dữ liệu'],
                    datasets: [{
                        label: "Lỗi",
                        data: [0],
                        backgroundColor: "rgba(255, 99, 132, 0.1)",
                        borderColor: "rgba(255, 99, 132, 1)",
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false },
                        title: { display: false }
                    }
                }
            });
        });
});