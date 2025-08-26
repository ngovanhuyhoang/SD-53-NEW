// Validation cho sản phẩm - ngăn nhập số âm
document.addEventListener('DOMContentLoaded', function() {
    
    // Tìm tất cả input có liên quan đến số lượng và giá bán
    const quantityInputs = document.querySelectorAll('input[name*="SoLuong"], input[id*="SoLuong"], input[class*="quantity"]');
    const priceInputs = document.querySelectorAll('input[name*="GiaBan"], input[id*="GiaBan"], input[class*="price"]');
    
    // Hàm ngăn nhập số âm
    function preventNegativeInput(input, fieldName) {
        // Ngăn nhập ký tự âm
        input.addEventListener('keydown', function(e) {
            // Ngăn nhập dấu trừ (-)
            if (e.key === '-' || e.key === 'Minus') {
                e.preventDefault();
                showValidationMessage(input, `${fieldName} không được là số âm!`);
                return false;
            }
        });
        
        // Kiểm tra khi nhập xong
        input.addEventListener('input', function(e) {
            let value = parseFloat(this.value);
            
            if (value < 0 || this.value.includes('-')) {
                this.value = '';
                showValidationMessage(input, `${fieldName} không được là số âm!`);
            } else {
                clearValidationMessage(input);
            }
        });
        
        // Kiểm tra khi mất focus
        input.addEventListener('blur', function(e) {
            let value = parseFloat(this.value);
            
            if (fieldName === 'Giá bán' && value === 0) {
                showValidationMessage(input, 'Giá bán phải lớn hơn 0!');
            } else if (value < 0) {
                this.value = '';
                showValidationMessage(input, `${fieldName} không được là số âm!`);
            }
        });
        
        // Thêm thuộc tính HTML để ngăn số âm
        input.setAttribute('min', fieldName === 'Giá bán' ? '0.01' : '0');
        input.setAttribute('step', fieldName === 'Giá bán' ? '0.01' : '1');
    }
    
    // Hàm hiển thị thông báo lỗi
    function showValidationMessage(input, message) {
        clearValidationMessage(input);
        
        const errorDiv = document.createElement('div');
        errorDiv.className = 'validation-error text-danger';
        errorDiv.style.fontSize = '0.875em';
        errorDiv.style.marginTop = '5px';
        errorDiv.textContent = message;
        errorDiv.id = input.id + '_validation_error';
        
        input.parentNode.appendChild(errorDiv);
        input.classList.add('is-invalid');
        
        // Tự động ẩn sau 3 giây
        setTimeout(() => {
            clearValidationMessage(input);
        }, 3000);
    }
    
    // Hàm xóa thông báo lỗi
    function clearValidationMessage(input) {
        const existingError = document.getElementById(input.id + '_validation_error');
        if (existingError) {
            existingError.remove();
        }
        input.classList.remove('is-invalid');
    }
    
    // Áp dụng validation cho số lượng
    quantityInputs.forEach(input => {
        input.type = 'number';
        preventNegativeInput(input, 'Số lượng');
    });
    
    // Áp dụng validation cho giá bán
    priceInputs.forEach(input => {
        input.type = 'number';
        preventNegativeInput(input, 'Giá bán');
    });
    
    // Validation cho form submit
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            let hasError = false;
            
            // Kiểm tra tất cả input số lượng và giá bán
            const allInputs = form.querySelectorAll('input[type="number"]');
            allInputs.forEach(input => {
                const value = parseFloat(input.value);
                const isQuantity = input.name && (input.name.includes('SoLuong') || input.id.includes('SoLuong'));
                const isPrice = input.name && (input.name.includes('GiaBan') || input.id.includes('GiaBan'));
                
                if (isQuantity && value < 0) {
                    showValidationMessage(input, 'Số lượng không được là số âm!');
                    hasError = true;
                }
                
                if (isPrice && (value < 0 || value === 0)) {
                    showValidationMessage(input, value < 0 ? 'Giá bán không được là số âm!' : 'Giá bán phải lớn hơn 0!');
                    hasError = true;
                }
            });
            
            if (hasError) {
                e.preventDefault();
                alert('Vui lòng kiểm tra lại thông tin. Số lượng và giá bán không được là số âm!');
                return false;
            }
        });
    });
    
    // Thông báo toast cho validation
    function showToast(message, type = 'error') {
        // Tạo toast notification
        const toast = document.createElement('div');
        toast.className = `alert alert-${type === 'error' ? 'danger' : 'success'} alert-dismissible fade show`;
        toast.style.position = 'fixed';
        toast.style.top = '20px';
        toast.style.right = '20px';
        toast.style.zIndex = '9999';
        toast.style.minWidth = '300px';
        
        toast.innerHTML = `
            <strong>${type === 'error' ? 'Lỗi!' : 'Thành công!'}</strong> ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(toast);
        
        // Tự động ẩn sau 5 giây
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 5000);
    }
    
    console.log('Product validation loaded - Negative values prevention active');
});
