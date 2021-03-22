﻿
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

async function createCustomer() {
    try {
        var emailJSON = {
            "email": `${document.querySelector('#email-field').value}`
        };
        const output = await fetch('/create-customer', {
            method: 'post',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(emailJSON)
        })
        const customerData = await output.json();
        return customerData;
    } catch (error) {
        console.error(error);
    }

}

var createSubscription = async function ({ customerIdInput, paymentMethodIdInput, priceIdInput }) {

    var handlePaymentThatRequiresCustomerAction = async function ({
        subscription,
        invoiceStatus,
        priceId,
        paymentMethodId,
    }) {
        {
            if (subscription && subscription._status === 'active') {
                // Subscription is active, no customer actions required.
                return { subscription, priceId, paymentMethodId };
            }

            // If it's a first payment attempt, the payment intent is on the subscription latest invoice.
            // If it's a retry, the payment intent will be on the invoice itself.

            let paymentIntentStatus = invoiceStatus ? invoiceStatus : subscription._latestInvoicePaymentIntentStatus;

            if (
                paymentIntentStatus === 'requires_action'
            ) {
                await stripe
                    .confirmCardPayment(subscription._latestInvoicePaymentIntentClientSecret, {
                        payment_method: paymentMethodId
                    })
                    .then((x) => {
                        if (x.error) {
                            // Start code flow to handle updating the payment details.
                            // Display error message in your UI.
                            // The card was declined (i.e. insufficient funds, card has expired, etc).
                            throw x.error.message;
                        } else {
                            if (x._paymentIntentStatus === 'succeeded') {
                                // Show a success message to your customer.
                                subscription._status = "active";
                                return {
                                    priceId: priceId,
                                    subscription: subscription,
                                    invoice: invoice,
                                    paymentMethodId: paymentMethodId,
                                };
                            }
                            // authentication (if any was attempted) failed
                            if (x.paymentIntentStatus === 'requires_payment_method') {
                                return {
                                    priceId: priceId,
                                    subscription: subscription,
                                    invoice: invoice,
                                    paymentMethodId: paymentMethodId,
                                };
                            }
                        }
                    })
                    .catch((error) => {
                        showError(error.message);
                    });
            } else {
                // No customer action needed.
                return { subscription, priceId, paymentMethodId };
            }
        }
    };

    var handleRequiresPaymentMethod = async function ({
        subscription,
        paymentMethodId,
        priceId,
    }) {
        try {
            if (subscription._status === 'active') {
                // subscription is active, no customer actions required.
                return { subscription, priceId, paymentMethodId };
            }
            else if (subscription._latestInvoicePaymentIntentStatus === 'requires_payment_method') {
                var message = 'Invalid payment method. Please try again.';

                throw new Error(message);
            }
            else {
                return { subscription, priceId, paymentMethodId };
            }
        } catch (error) {
            showError(error.message);
        }

    };

    var onSubscriptionComplete = function (result) {

        if (result.subscription._status === 'active') {
            orderComplete();
        }
        else if (loading) {
            showError('Something went wrong. Please try again.');
        }

    };


    var subscriptionParams = {
        "paymentMethodId": `${paymentMethodIdInput}`,
        "customerId": `${customerIdInput}`,
        "priceId": `${priceIdInput}`
    };

    await fetch('/create-subscription', {
        method: 'post',
        headers: {
            'Content-type': 'application/json',
        },
        body: JSON.stringify(subscriptionParams),
    })
        .then((response) => {
            return response.json()
        })

        // If the card is declined, display an error to the user.
        .then((x) => {
            if (x._errorMessage) {
                showError(x._errorMessage);
                
                throw x._errorMessage;
            }
            return x;
        })
        // Normalize the result to contain the object returned by Stripe.
        // Add the additional details we need.

        .then((output) => {
            return {
                paymentMethodId: paymentMethodIdInput,
                priceId: priceIdInput,
                subscription: output,
            };

        })

        // Some payment methods require a customer to be on session
        // to complete the payment process. Check the status of the
        // payment intent to handle these actions.
        .then((value) => {
            (async () => {
                await handlePaymentThatRequiresCustomerAction({
                    subscription: value.subscription,
                    invoiceStatus: value.subscription._latestInvoicePaymentIntentStatus,
                    priceId: value.priceId,
                    paymentMethodId: value.paymentMethodId,
                });

                // If attaching this card to a Customer object succeeds,
                // but attempts to charge the customer fail, you
                // get a requires_payment_method error.
                await handleRequiresPaymentMethod({
                    subscription: value.subscription,
                    paymentMethodId: value.paymentMethodId,
                    priceId: value.priceId,
                });

                // No more actions required. Provision your service for the user.
                await onSubscriptionComplete(value);
            })();
        })
        .catch((error) => {
            // An error has happened. Display the failure to the user here.
            // We utilize the HTML element we created.
            //showError(error.message);
        });
}

async function createPayment(card, customerId, priceId, customerEmail) {

    var CustomerId = customerId;

    var PriceId = priceId;

    const output = stripe
        .createPaymentMethod({
            type: 'card',
            card: card,
            billing_details: {
                email: customerEmail,
            },
        })
        .then((paymentResult) => {
            if (paymentResult.error) {
                showError(paymentResult.error.message);
            } else {
                (async () => {
                    await createSubscription({
                        customerIdInput: CustomerId,
                        paymentMethodIdInput: paymentResult.paymentMethod.id,
                        priceIdInput: PriceId
                    });
                })();

            }

            return paymentResult.paymentMethod.id;

        });

    return output;
}

async function getCustomerEmail(customer) {

    var customerJSON = {
        "customerId": `${customer}`
    };

    var output = await fetch('/get-email', {
        method: 'post',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(customerJSON)
    })
    const emailData = await output.json();

    return emailData;
}

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
    document.querySelector("#submit").disabled = true;

    card.on("change", function (event) {
        // Disable the Pay button if there are no card details in the Element
        var disableButton = (event.empty || event.error);

        document.querySelector("#submit").disabled = disableButton;
        document.querySelector("#card-error").textContent = event.error ? event.error.message : "";
    });

    var form = document.getElementById("payment-form");
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        // Complete payment when the submit button is clicked
        loading(true);
        var customerEmail = document.querySelector('#email-field').value;

        (async () => {
            var customer = "customer not set";

            await createCustomer()
                .then((customerData) => customer = customerData._customer);

            await createPayment(card, customer, PriceId, customerEmail)
                .then((paymentData) => {
                    payment = paymentData;
                });

        })();


    });


}

handleForm();



// Calls stripe.confirmCardPayment
// If the card requires authentication Stripe shows a pop-up modal to
// prompt the user to enter authentication details without leaving your page.
var payWithCard = function (stripe, clientSecret, paymentMethod) {
    loading(true);

    stripe
        .confirmCardPayment(clientSecret, {
            payment_method: paymentMethod,
        })
        .then((results) => {
            if (results.error) {
                // Show error to your customer
                showError(results.error.message);
            } else {
                // The payment succeeded!

                orderComplete(results.paymentIntent.id);
            }
        });
};

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
    if (isLoading) {
        // Disable the button and show a spinner
        document.querySelector("#submit").disabled = true;
        document.querySelector("#spinner").classList.remove("hidden");
        document.querySelector("#button-text").classList.add("hidden");
    } else {
        document.querySelector("#submit").disabled = false;
        document.querySelector("#spinner").classList.add("hidden");
        document.querySelector("#button-text").classList.remove("hidden");
    }
};
