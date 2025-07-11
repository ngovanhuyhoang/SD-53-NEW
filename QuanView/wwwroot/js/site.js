<script>
    function toggleKhachHang() {
        const isPublic = document.getElementById("LaCongKhai").checked;
    document.getElementById("chonKhachHang").style.display = isPublic ? "none" : "block";
    }
    document.addEventListener("DOMContentLoaded", toggleKhachHang);
</script>
document.querySelectorAll(".khach-row").forEach(row => {
    row.addEventListener("click", function () {
        document.querySelectorAll(".khach-row").forEach(r => r.classList.remove("table-primary"));
        row.classList.add("table-primary");
        document.getElementById("SelectedKhachHangId").value = row.dataset.id;

        // Hiện checkbox gửi email sau khi chọn khách hàng
        document.getElementById("guiEmailDiv").style.display = "block";
    });
});

