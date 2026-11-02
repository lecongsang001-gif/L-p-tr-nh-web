(function () {
    'use strict';

    // 1. Đổ bóng cho thanh menu khi cuộn xuống
    var header = document.querySelector('.mt-header');
    if (header) {
        var onScroll = function () {
            header.classList.toggle('is-scrolled', window.scrollY > 4);
        };
        onScroll();
        window.addEventListener('scroll', onScroll, { passive: true });
    }

    // 2. Ảnh xe bị lỗi / chưa có file ảnh -> dùng ảnh mặc định
    var FALLBACK = '/images/xe/no-image.svg';
    document.querySelectorAll('.mt-xe-img img').forEach(function (img) {
        var dungAnhMacDinh = function () {
            if (img.getAttribute('src') !== FALLBACK) {
                img.setAttribute('src', FALLBACK);
            }
        };
        img.addEventListener('error', dungAnhMacDinh);
        if (img.complete && img.naturalWidth === 0) {
            dungAnhMacDinh();
        }
    });

    // 3. Ô số điện thoại / CCCD chỉ nhận chữ số
    document.querySelectorAll('input[inputmode="numeric"]').forEach(function (input) {
        input.addEventListener('input', function () {
            var chiSo = input.value.replace(/\D/g, '');
            if (chiSo !== input.value) {
                input.value = chiSo;
            }
        });
    });

    // 4. Form đăng ký: ngày trả luôn sau ngày nhận ít nhất 1 ngày
    var nhan = document.getElementById('NgayNhan');
    var tra = document.getElementById('NgayTra');
    if (nhan && tra) {
        var dongBo = function () {
            if (!nhan.value) { return; }
            var d = new Date(nhan.value);               // yyyy-MM-dd được hiểu là UTC
            if (isNaN(d.getTime())) { return; }
            d.setUTCDate(d.getUTCDate() + 1);
            var toiThieu = d.toISOString().slice(0, 10);
            tra.min = toiThieu;
            if (!tra.value || tra.value < toiThieu) {
                tra.value = toiThieu;
            }
        };
        nhan.addEventListener('change', dongBo);
    }
})();
