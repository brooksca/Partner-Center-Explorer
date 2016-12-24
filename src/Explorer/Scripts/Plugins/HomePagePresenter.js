/// <reference path="~/Scripts/_references.js" />

Microsoft.WebPortal.HomePagePresenter = function (webPortal, feature) {
    /// <summary>
    /// Manages the home page experience. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    this.base.constructor.call(this, webPortal, feature, "Home", "/Template/Home/");

    this.viewModel = {
        ShowProgress: ko.observable(true),
        IsSet: ko.observable(false),
    }

    this.onCustomerClicked = function (data) {
        var customerInfo = { Id: data.Id }
        // Activate the customer page presenter and pass it the selected customer.
        webPortal.Journey.advance(Microsoft.WebPortal.Feature.Customer, customerInfo);
    };

    webPortal.Services.HeaderBar.resetBreadcrumbs();
    webPortal.Services.HeaderBar.addBreadcrumb(true, null, 'Home');
}

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.HomePagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.HomePagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is rendered but not shown yet.
    /// </summary>

    var self = this;
    ko.applyBindings(self, $("#CustomersContainer")[0]);

    var getCustomers = function () {
        var getCustomersServerCall = self.webPortal.ServerCallManager.create(
            self.feature, self.webPortal.Helpers.ajaxCall("api/customer", Microsoft.WebPortal.HttpMethod.Get), "GetCustomers");

        self.viewModel.IsSet(false);
        self.viewModel.ShowProgress(true);

        getCustomersServerCall.execute().done(function (customers) {
            self.viewModel.Details = ko.observable(customers);
            self.viewModel.IsSet(true);
        }).fail(function (result, status, error) {
            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error,
                "Failed to retrieve customers...");

            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.RETRY, self.webPortal.Resources.Strings.Retry, function () {
                    notification.dismiss();

                    // retry
                    getCustomers();
                }),
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.CANCEL, self.webPortal.Resources.Strings.Cancel, function () {
                    notification.dismiss();
                })
            ]);

            self.webPortal.Services.Notifications.add(notification);
        }).always(function () {
            // stop showing progress
            self.viewModel.ShowProgress(false);
        });
    }

    getCustomers();
}

//@ sourceURL=HomePagePresenter.js