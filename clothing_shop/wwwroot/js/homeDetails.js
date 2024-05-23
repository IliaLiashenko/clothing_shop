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
