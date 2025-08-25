// Chart.js v4.4.1 - Cấu hình và tạo Donut Chart từ API
$(document).ready(function () {
    // Kiểm tra Chart.js đã load chưa
    if (typeof Chart === 'undefined') {
        console.error('Chart.js chưa được load!');
        return;
    }

    // Cấu hình defaults cho Chart.js v4 - CÁCH AN TOÀN
    try {
        Chart.defaults.font = Chart.defaults.font || {};
        Chart.defaults.font.family = 'Nunito, -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif';
        Chart.defaults.color = '#858796';
    } catch (e) {
        console.warn('Không thể set Chart defaults:', e);
    }

    // Hàm hỗ trợ định dạng số
    function number_format(number) {
        return number.toLocaleString('vi-VN');
    }

    // Kiểm tra element tồn tại
    const canvasElement = document.getElementById("myPieChart");
    if (!canvasElement) {
        console.error('Không tìm thấy element với id "myPieChart"');
        return;
    }

    // Destroy chart cũ nếu tồn tại
    const existingChart = Chart.getChart(canvasElement);
    if (existingChart) {
        existingChart.destroy();
        console.log('Destroyed existing Pie Chart');
    }

    // Fetch dữ liệu từ API và tạo biểu đồ
    fetch("/api/ChartsApi/pie-chart-data")
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(res => {
            if (!res.success) throw new Error("Lỗi khi lấy dữ liệu: " + (res.message || "Unknown error"));

            const chartData = res.data;
            const labels = chartData.labels;
            const data = chartData.datasets[0].data;
            const backgroundColor = chartData.datasets[0].backgroundColor;

            const ctx = canvasElement.getContext('2d');

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: data,
                        backgroundColor: backgroundColor,
                        hoverOffset: 10
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom',
                            labels: {
                                usePointStyle: true,
                                padding: 20,
                                color: '#858796',
                                font: {
                                    family: 'Nunito, sans-serif'
                                }
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const dataset = context.dataset;
                                    const total = dataset.data.reduce((acc, val) => acc + val, 0);
                                    const value = dataset.data[context.dataIndex];
                                    const percent = ((value / total) * 100).toFixed(0);
                                    return `${context.label}: ${number_format(value)} đ (${percent}%)`;
                                }
                            },
                            backgroundColor: "rgb(255,255,255)",
                            bodyColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            padding: 15,
                            displayColors: false
                        },
                        title: {
                            display: false
                        }
                    },
                    cutout: '50%'
                }
            });

            console.log("Pie Chart loaded successfully");
        })
        .catch(err => {
            console.error("Lỗi khi tải biểu đồ Pie Chart:", err);
            // Hiển thị biểu đồ lỗi
            const ctx = canvasElement.getContext('2d');
            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Không có dữ liệu'],
                    datasets: [{
                        data: [1],
                        backgroundColor: ["rgba(255, 99, 132, 0.8)"]
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false },
                        title: { display: false }
                    },
                    cutout: '50%'
                }
            });
        });
});