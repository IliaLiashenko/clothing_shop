document.addEventListener("DOMContentLoaded", function () {
    const qtySelect = document.querySelector(".qty");
    const sizeSelect = document.querySelector(".form-control");
    const addToCartBtn = document.getElementById("addToCartBtn");
    const errorMessage = document.querySelector(".error-message");

    function updateErrorMessage() {
        const selectedQty = parseInt(qtySelect.value);
        const selectedSize = parseInt(sizeSelect.value);
        const availableQtyOption = document.querySelector(`option[value='${selectedSize}']:checked`);
        const availableQty = parseInt(availableQtyOption.getAttribute("data-available-qty"));

        if (selectedQty > availableQty) {
            errorMessage.textContent = "Selected quantity exceeds available quantity.";
            addToCartBtn.disabled = true;
        } else {
            errorMessage.textContent = "";
            addToCartBtn.disabled = false;
        }
    }

    qtySelect.addEventListener("change", updateErrorMessage);
    sizeSelect.addEventListener("change", updateErrorMessage);
});


$(document).ready(function () {
    let currentIndex = 0;
    const images = $('.slider-container img');
    const thumbnails = $('.gallery-thumbnail');

    function showImage(index) {
        images.removeClass('active').eq(index).addClass('active');
        thumbnails.removeClass('active').eq(index).addClass('active');
    }

    $('.gallery-thumbnail').click(function () {
        const index = $(this).data('index');
        currentIndex = index;
        showImage(index);
    });

    $('.arrow-left').click(function () {
        currentIndex = (currentIndex > 0) ? currentIndex - 1 : images.length - 1;
        showImage(currentIndex);
    });

    $('.arrow-right').click(function () {
        currentIndex = (currentIndex < images.length - 1) ? currentIndex + 1 : 0;
        showImage(currentIndex);
    });
});



