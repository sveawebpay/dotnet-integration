var coll = document.getElementsByClassName("collapsible");
var i;

for (i = 0; i < coll.length; i++) {
    coll[i].addEventListener("click", function () {
        this.classList.toggle("active");
        var content = this.nextElementSibling;
        if (content.style.display === "table-row-group") {
            content.style.display = "none";
        } else {
            content.style.display = "table-row-group";
        }
    });
}

var updateSettings = function(element) {
    let form = element.closest('form');
    form.find('[type="hidden"]:first').val(element.html());
    form.submit();
};

var shippingHandler = function (data) {
    console.log('event: ' + data);

    if (data.detail) {
        document.dispatchEvent(new CustomEvent("sveaCheckout:setIsLoading", { detail: { isLoading: true } }));;

        fetch('/api/svea/shippingTaxCalculation', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data.detail),
            })
            .then(response => {
                console.log(response);
            })
            .then(data => {
                console.log('Success:', data);
            })
            .catch((error) => {
                console.error('Error:', error);
            });

        document.dispatchEvent(new CustomEvent("sveaCheckout:setIsLoading", { detail: { isLoading: false } }));
    }
}

$(function () {
    document.addEventListener("sveaCheckout:shippingConfirmed", shippingHandler);
});