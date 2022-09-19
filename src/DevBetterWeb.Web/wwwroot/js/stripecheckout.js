var this_js_script = document.currentScript;

var stripeKey = this_js_script.getAttribute("data-stripe_key");
var subscriptionPlanPriceId = this_js_script.getAttribute('data-subscription_plan_price_id');

var stripe = Stripe(stripeKey);

var purchase = {
    "subscriptionpriceid": `${subscriptionPlanPriceId}`
};

var PriceId = `${subscriptionPlanPriceId}`;

// Disable the button until we have Stripe set up on the page
document.querySelector("button").disabled = true;

var handleForm = function () {

    var elements = stripe.elements();

    var style = {
        base: {
            color: "#32325d",
            fontFamily: 'Arial, sans-serif',
            fontSmoothing: "antialiased",
            fontSize: "16px",
            "::placeholder": {
                color: "#32325d"
            }
        },
        invalid: {
            fontFamily: 'Arial, sans-serif',
            color: "#fa755a",
            iconColor: "#fa755a"
        }
    };

    var card = elements.create("card", { style: style });
    // Stripe injects an iframe into the DOM
    card.mount("#card-element");

    card.on("change", function (event) {
        // Disable the Pay button if there are no card details in the Element
        var disableButton = (event.empty || event.error);

        document.querySelector("#submit").disabled = disableButton;
        document.querySelector("#card-error").textContent = event.error ? event.error.message : "";
    });

		var form = document.getElementById("payment-form");
		let submitted = false;
		form.addEventListener("submit", async (event) => {
				event.preventDefault();

				if (submitted) {
						return;
				}
			submitted = true;
			loading(true);

			var customerEmail = document.querySelector('#email-field').value;

			const { error: stripePaymentMethodError, paymentMethod } = await stripe.createPaymentMethod({
				type: 'card',
				card: card,
				billing_details: {
					email: customerEmail,
				},
			});

			if (stripePaymentMethodError) {
				showError(stripePaymentMethodError.message);

				submitted = false;
				loading(false);
				return;
			}

			// Make a call to the server to create a new
			// payment intent and store its client_secret.
			const clientSecretResult = await fetch(
				'/create-subscription',
				{
					method: 'POST',
					headers: {
						'Content-Type': 'application/json',
					},
					body: JSON.stringify({
						paymentMethodId: paymentMethod.id,
						priceId: PriceId,
						customerEmail: customerEmail,
					}),
				}
			).then((r) => r.json());

			if (clientSecretResult?.errorMessage) {
				showError(clientSecretResult.errorMessage);

				// reenable the form.
				submitted = false;
				loading(false);
				return;
			}			

			// Confirm the card payment given the clientSecret
			// from the payment intent that was just created on
			// the server.
			const { error: stripeError, paymentIntent } = await stripe.confirmCardPayment(
				clientSecretResult.latestInvoicePaymentIntentClientSecret,
				{
					payment_method: {
						type: 'card',
						card: card,
						billing_details: {
							email: customerEmail,
						},
					},
				}
			);

			if (stripeError) {
				showError(stripeError.message);

				submitted = false;
				loading(false);
				return;
			}

			submitted = false;
			orderComplete();			
    });
}

handleForm();

/* ------- UI helpers ------- */
// Shows a success message when the payment is complete
var orderComplete = function () {
    loading(false);
		document.querySelector(".result-message").classList.remove("hidden");
    document.querySelector("button").disabled = true;
    document.querySelector("#submit").classList.add("hidden");
    document.querySelector("#card-element").classList.add("hidden");
    document.querySelector("#email-field").classList.add("hidden");
    document.querySelector("#email-label").classList.add("hidden");
    document.querySelector("#cardLabel").classList.add("hidden");
    document.querySelector("#no-risk").classList.add("hidden");
};
// Show the customer the error from Stripe if their card fails to charge
var showError = function (errorMsgText) {
    loading(false);
    var errorMsg = document.querySelector("#card-error");
    errorMsg.textContent = `An error has occured: ${errorMsgText}`;
    //setTimeout(function () {
    //    errorMsg.textContent = "";
    //}, 40000);
};
// Show a spinner on payment submission
var loading = function (isLoading) {
		let buttonElement = document.getElementById('submit');
    if (isLoading) {
			// Disable the button and show a spinner
			buttonElement.classList.add('button--loading');
			buttonElement.disabled = true;
		} else {
			buttonElement.classList.remove('button--loading');
			buttonElement.disabled = false;
    }
};