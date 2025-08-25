// Chart.js v4 không dùng Chart.defaults.global nữa
Chart.defaults.font.family = 'Nunito, -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif';
Chart.defaults.color = '#858796';

// Hàm định dạng số
function number_format(number) {
    return new Intl.NumberFormat('vi-VN').format(number);
}

// Gọi API để vẽ biểu đồ
fetch("/api/ChartsApi/bar-chart-data")
    .then(res => res.json())
    .then(res => {
        if (!res.success) throw new Error("Lỗi khi lấy dữ liệu: " + (res.message || "Unknown error"));

        const chartData = res.data;
        const labels = chartData.labels;
        const soldData = chartData.datasets[0].data;

        const ctx = document.getElementById("myBarChart");
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: "Số lượng bán",
                    backgroundColor: "#4e73df",
                    hoverBackgroundColor: "#2e59d9",
                    borderColor: "#4e73df",
                    data: soldData,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                layout: {
                    padding: {
                        left: 10,
                        right: 25,
                        top: 25,
                        bottom: 0
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            maxRotation: 45,
                            minRotation: 0
                        },
                        grid: {
                            display: false,
                            drawBorder: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        ticks: {
                            padding: 10,
                            callback: function (value) {
                                return number_format(value);
                            }
                        },
                        grid: {
                            color: "rgb(234, 236, 244)",
                            drawBorder: false,
                            borderDash: [2],
                            zeroLineBorderDash: [2]
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: "rgb(255,255,255)",
                        titleColor: "#6e707e",
                        bodyColor: "#858796",
                        borderColor: '#dddfeb',
                        borderWidth: 1,
                        padding: 15,
                        displayColors: false,
                        callbacks: {
                            label: function (context) {
                                return 'Đã bán: ' + number_format(context.parsed.y) + ' sản phẩm';
                            }
                        }
                    },
                    title: {
                        display: false
                    }
                }
            }
        });

        console.log("Bar Chart loaded successfully");
    })
    .catch(err => {
        console.error("Lỗi khi tải biểu đồ Bar Chart:", err);
        const ctx = document.getElementById("myBarChart");
        if (ctx) {
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: ['Không có dữ liệu'],
                    datasets: [{
                        label: "Lỗi",
                        data: [0],
                        backgroundColor: "rgba(255, 99, 132, 0.8)"
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        title: {
                            display: true,
                            text: 'Không thể hiển thị biểu đồ',
                            font: {
                                size: 18,
                                weight: 'bold'
                            },
                            padding: {
                                top: 10,
                                bottom: 30
                            }
                        },
                        legend: {
                            display: false
                        }
                    }
                }
            });
        }
    });
