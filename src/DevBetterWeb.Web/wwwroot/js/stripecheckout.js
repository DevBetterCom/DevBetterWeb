
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

        async function createCustomer () {
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

        var createPaymentIntent = function () {
        fetch("/create-payment-intent", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(purchase)
        })
            .then(function (result) {
                return result.json();
            })
            .then(function (data) {
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
                    document.querySelector("button").disabled = event.empty;
                    document.querySelector("#card-error").textContent = event.error ? event.error.message : "";
                });

                var form = document.getElementById("payment-form");
                form.addEventListener("submit", function (event) {
                    event.preventDefault();
                    // Complete payment when the submit button is clicked
                    payWithCard(stripe, card, data.clientSecret);
                });

            });


        }

        var createSubscription = async function({customerIdInput, paymentMethodIdInput, priceIdInput}) {

            var handlePaymentThatRequiresCustomerAction = function () {

    };

            var handleRequiresPaymentMethod = function () {

    };

            var onSubscriptionComplete = function () {

    };

            var showCardError = function (error) {

    };

            var subscriptionParams = {
        "paymentMethodId": `${paymentMethodIdInput}`,
                "customerId": `${customerIdInput}`,
                "priceId": `${priceIdInput}`
            };

            fetch('/create-subscription', {
        method: 'post',
                    headers: {
        'Content-type': 'application/json',
                    },
                body: JSON.stringify(subscriptionParams),
                })
                    .then((response) => response.json())
                    // If the card is declined, display an error to the user.
                    .then((result) => {
                        if (result.error) {
                            // The card had an error when trying to attach it to a customer.
                            throw result;
                        }
                        return result;
                    })
                    // Normalize the result to contain the object returned by Stripe.
                    // Add the additional details we need.
                    .then((result) => {
                        return {
        paymentMethodId: paymentMethodId,
                            priceId: priceId,
                            subscription: result,
                        };
                    })
                    // Some payment methods require a customer to be on session
                    // to complete the payment process. Check the status of the
                    // payment intent to handle these actions.
                    .then(handlePaymentThatRequiresCustomerAction)
                    // If attaching this card to a Customer object succeeds,
                    // but attempts to charge the customer fail, you
                    // get a requires_payment_method error.
                    .then(handleRequiresPaymentMethod)
                    // No more actions required. Provision your service for the user.
                    .then(onSubscriptionComplete)
                    .catch((error) => {
        // An error has happened. Display the failure to the user here.
        // We utilize the HTML element we created.
        showCardError(error);
                    })
        }

        async function createPayment (card, customerId, priceId, customerEmail) {

            const CustomerId = customerId;

            let PriceId = priceId;

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
        //showError(result.error.message);
        ////showError(result.error.message);
    } else {
        (async () => {
            await createSubscription({
                customerIdInput: CustomerId,
                paymentMethodIdInput: paymentResult.paymentMethod.id,
                priceIdInput: PriceId
            });
        })();
                        //orderComplete();

                    }

                    return paymentResult.paymentMethod.id;

                });

            return output;
        }

        async function getCustomerEmail (customer) {

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

        async function updatePaymentIntent(customer, paymentMethod, paymentIntent, clientSecret) {
            var paymentIntentUpdateParams = {
        "customer": `${customer}`,
                "paymentIntentSecret": `${paymentIntent}`
            };

            await fetch("/update-payment-intent", {
        method: "POST",
                headers: {
        "Content-Type": "application/json"
                },
                body: JSON.stringify(paymentIntentUpdateParams)
            })
                .then(() => payWithCard(stripe, clientSecret, paymentMethod));

        }

        var handleForm = function () {

        //createPaymentIntent();

        // errors here are occuring because, despite using async/await, promises are not resolving (unclear if they are not resolving
        // on time or if they are not resolving at all). Somehow, I need to figure out how to get them to resolve properly, or find 
        // some other way to get the required information (customer id, customer email, payment method id) into this function.

        fetch("/create-payment-intent", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(purchase)
        })
            .then(function (result) {
                return result.json();
            })
            .then(function (data) {
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
                    document.querySelector("button").disabled = event.empty;
                    document.querySelector("#card-error").textContent = event.error ? event.error.message : "";
                });

                var form = document.getElementById("payment-form");
                form.addEventListener("submit", function (event) {
                    event.preventDefault();
                    // Complete payment when the submit button is clicked

                    //const customerPromise = await createCustomer();

                    var customerEmail = document.querySelector('#email-field').value;

                    (async () => {
                        var customer = "customer not set";
                        var payment = "payment not set";

                        await createCustomer()
                            .then((customerData) => customer = customerData._customer);

                        await createPayment(card, customer, PriceId, customerEmail)
                            .then((paymentData) => payment = paymentData);

                        await updatePaymentIntent(customer, payment, data.id, data.clientSecret);



                        //await updatePaymentIntent(customer, payment, data.id);

                        // payWithCard(stripe, data.clientSecret);

                    })();



                    //getCustomerEmail(customer).then((emailData) => email = (emailData));


                    //var customerId = customerResult.result.customer.id;


                    // update paymentIntent
                    //updatePaymentIntent(customer, paymentMethod, data.id);

                });

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
                    .then(function (result) {
                        if (result.error) {
        // Show error to your customer
        showError(result.error.message);
                        } else {
        // The payment succeeded!
        // redirect to success page with email and stripe payment intent id
        orderComplete(result.paymentIntent.id);
                     }
                    });
            };

            /* ------- UI helpers ------- */
            // Shows a success message when the payment is complete
            var orderComplete = function () {
        loading(false);
                document.querySelector("result-message").classList.remove("hidden");
                document.querySelector("button").disabled = true;
                document.querySelector("#submit").classList.add("hidden");
                document.querySelector("#card-element").classList.add("hidden");
                document.querySelector("#email").classList.add("hidden");
                document.querySelector("#emailLabel").classList.add("hidden");
                document.querySelector("#cardLabel").classList.add("hidden");
            };
            // Show the customer the error from Stripe if their card fails to charge
            var showError = function (errorMsgText) {
        loading(false);
                var errorMsg = document.querySelector("#card-error");
                errorMsg.textContent = errorMsgText;
                setTimeout(function () {
        errorMsg.textContent = "";
                }, 4000);
            };
            // Show a spinner on payment submission
            var loading = function (isLoading) {
                if (isLoading) {
        // Disable the button and show a spinner
        document.querySelector("button").disabled = true;
                    document.querySelector("#spinner").classList.remove("hidden");
                    document.querySelector("#button-text").classList.add("hidden");
                } else {
        document.querySelector("button").disabled = false;
                    document.querySelector("#spinner").classList.add("hidden");
                    document.querySelector("#button-text").classList.remove("hidden");
                }
            };
