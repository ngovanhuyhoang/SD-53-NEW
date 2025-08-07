// Sử dụng jQuery để đồng bộ với code khác
$(document).ready(function () {
    // Kiểm tra xem Chart.js đã load chưa
    if (typeof Chart === 'undefined') {
        console.error('Chart.js chưa được load!');
        return;
    }

    // Kiểm tra element tồn tại
    const canvasElement = document.getElementById('donHangChart');
    if (!canvasElement) {
        console.error('Không tìm thấy element với id "donHangChart"');
        return;
    }

    fetch('/api/ChartsApi/trang-thai-don-hang-trong-thang')
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            if (!data || data.length === 0) {
                console.warn('Không có dữ liệu trạng thái đơn hàng');
                return;
            }

            const labels = data.map(item => item.trangThai);
            const values = data.map(item => item.soLuong);
            const backgroundColors = [
                '#36A2EB', // Xanh dương
                '#FF6384', // Hồng
                '#FFCE56', // Vàng
                '#4BC0C0', // Xanh ngọc
                '#9966FF', // Tím
                '#FF9F40'  // Cam
            ];

            const ctx = canvasElement.getContext('2d');

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: labels,
                    datasets: [{
                        data: values,
                        backgroundColor: backgroundColors,
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false, // Thêm để tránh lỗi resize
                    plugins: {
                        legend: {
                            position: 'bottom',
                        },
                        //title: {
                        //    display: true,
                        //    text: 'Thống kê trạng thái đơn hàng trong tháng'
                        //},
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    const total = context.chart.data.datasets[0].data.reduce((sum, val) => sum + val, 0);
                                    const percentage = ((value / total) * 100).toFixed(1);
                                    return `${context.label}: ${value} đơn hàng (${percentage}%)`;
                                }
                            },
                            backgroundColor: "rgb(255,255,255)",
                            bodyColor: "#858796",
                            borderColor: '#dddfeb',
                            borderWidth: 1,
                            padding: 12,
                            displayColors: false
                        }
                    },
                    cutout: '50%'
                }
            });
        })
        .catch(error => {
            console.error('Lỗi khi lấy dữ liệu thống kê:', error);
        });
});