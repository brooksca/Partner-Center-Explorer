/// <reference path="~/Scripts/_references.js" />

Microsoft.WebPortal.CustomerAddConfirmationPagePresenter = function (webPortal, feature, registrationConfirmationViewModel) {
    /// <summary>
    /// Shows the registration confirmation page.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    /// <param name="registrationConfirmationViewModel">The registration confirmation view model.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/RegistrationConfirmation/");

    var self = this;
    self.viewModel = registrationConfirmationViewModel;

    // object to pass to order API.
    self.viewModel.orderToPlace = {
        CustomerId: registrationConfirmationViewModel.MicrosoftId,             
        OperationType: Microsoft.WebPortal.CommerceOperationType.NewPurchase,
        Subscriptions: registrationConfirmationViewModel.SubscriptionsToOrder
    };

    var addressLine = self.viewModel.AddressLine1;

    if (self.viewModel.AddressLine2) {
        addressLine += " " + self.viewModel.AddressLine2;
    }

    self.viewModel.Address = [
        addressLine,
        self.viewModel.City + ", " + self.viewModel.State + " " + self.viewModel.ZipCode,
        self.viewModel.Country
    ];

    self.viewModel.ContactInformation = [
        self.viewModel.FirstName + " " + self.viewModel.LastName,
        self.viewModel.Email,
        self.viewModel.Phone
    ];

    self.onDoneClicked = function () {
        // Prepare the order
        self.processOrder();
    }

    self.processOrder = function (customerNotification, registeredCustomer) {
        /// <summary>
        /// Called when the customer has been created and hence order can be placed. 
        /// </summary>

        // order notification.        
        var orderNotification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Progress,
            self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.ProcessingOrder);
        self.webPortal.Services.Notifications.add(orderNotification);

        new Microsoft.WebPortal.Utilities.RetryableServerCall(self.webPortal.Helpers.ajaxCall("api/order/create", Microsoft.WebPortal.HttpMethod.Post, self.viewModel.orderToPlace, Microsoft.WebPortal.ContentType.Json, 120000), "RegisterCustomerOrder", []).execute()
            .done(function (result) {
                orderNotification.dismiss();

                self.webPortal.Journey.start(Microsoft.WebPortal.Feature.Home);
            })
            .fail(function (result, status, error) {
                // on failure check if customerid is returned (or check using errCode). if returned then do something to set the ClientCustomerId
                orderNotification.type(Microsoft.WebPortal.Services.Notification.NotificationType.Error);
                orderNotification.buttons([
                    // no need for retry button. user should be able to hit submit.
                    Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                        orderNotification.dismiss();
                    })
                ]);

                var errorPayload = JSON.parse(result.responseText);

                if (errorPayload) {
                    switch (errorPayload.ErrorCode) {
                        case Microsoft.WebPortal.ErrorCode.InvalidInput:
                            orderNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.InvalidInputErrorPrefix + errorPayload.Details.ErrorMessage);
                            break;
                        case Microsoft.WebPortal.ErrorCode.DownstreamServiceError:
                            orderNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.DownstreamErrorPrefix + errorPayload.Details.ErrorMessage);
                            break;
                        case Microsoft.WebPortal.ErrorCode.PaymentGatewayPaymentError:
                        case Microsoft.WebPortal.ErrorCode.PaymentGatewayIdentityFailureDuringPayment:
                        case Microsoft.WebPortal.ErrorCode.PaymentGatewayFailure:
                            orderNotification.message(errorPayload.Details.ErrorMessage);
                            break;
                        default:
                            orderNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.OrderRegistrationFailureMessage);
                            break;
                    }
                } else {
                    orderNotification.message(self.webPortal.Resources.Strings.Plugins.CustomerAddNewPage.OrderRegistrationFailureMessage);
                }

            })
            .always(function () {
                self.isPosting = false;
            });
    }
}

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.CustomerAddConfirmationPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.CustomerAddConfirmationPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is about to be rendered.
    /// </summary>

    ko.applyBindings(this, $("#RegistrationConfirmationContainer")[0]);

}

//@ sourceURL=CustomerAddConfirmationPagePresenter.js