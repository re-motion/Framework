document.getElementById("doubleInput").addEventListener("change", function() {
    const formGrid = document.getElementById("myFormGrid");
    if (this.checked) {
        formGrid.classList.add("double");
    } else {
        formGrid.classList.remove("double");
    }
});