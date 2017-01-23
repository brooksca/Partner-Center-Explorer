/// <reference path="~/Scripts/_references.js" />

Microsoft.WebPortal.CustomerPagePresenter = function (webPortal, feature, context) {
    /// <summary>
    /// Manages the home page experience. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    /// <param name="context">The context used to populate the page. This should be the JSON representation of a customer.</param>
    this.base.constructor.call(this, webPortal, feature, "Customer", "/Template/Customer/");

    this.viewModel = {
        CustomerId: context.Id,
        ShowProgress: ko.observable(true),
        IsSet: ko.observable(false),
    };

    this.onSubscriptionClicked = function (data) {
        var subscriptionInfo = { CustomerId: data.CustomerId, SubscriptionId: data.SubscriptionId }
        // Activate the subscription page presenter and pass it the selected subscription.
        webPortal.Journey.advance(Microsoft.WebPortal.Feature.Subscription, subscriptionInfo);
    };

    this.webPortal = webPortal;
};

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.CustomerPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.CustomerPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is rendered but not shown yet.
    /// </summary>

    var self = this;
    var summaryUrl = "api/customer/summary?customerId=" + self.viewModel.CustomerId;

    ko.applyBindings(self, $("#CustomerContainer")[0]);

    var getCustomerSummary = function () {
        var getCustomerSummaryServerCall = self.webPortal.ServerCallManager.create(
            self.feature, self.webPortal.Helpers.ajaxCall(summaryUrl, Microsoft.WebPortal.HttpMethod.Get), "GetCustomerSummary");

        self.viewModel.IsSet(false);
        self.viewModel.ShowProgress(true);

        getCustomerSummaryServerCall.execute().done(function (summary) {
            self.viewModel.Summary = ko.observable(summary);
            self.viewModel.IsSet(true);

            var homeNavigation = function () {
                // Activate the home page presenter.
                self.webPortal.Journey.advance(Microsoft.WebPortal.Feature.Home);
            };

            var customerNavigation = function () {
                var customerInfo = { Id: summary.Id }
                // Activate the customer page presenter and pass it the selected customer.
                self.webPortal.Journey.advance(Microsoft.WebPortal.Feature.Customer, customerInfo);
            };

            self.webPortal.Services.HeaderBar.resetBreadcrumbs();
            self.webPortal.Services.HeaderBar.addBreadcrumb(false, homeNavigation, 'Home');
            self.webPortal.Services.HeaderBar.addBreadcrumb(true, customerNavigation, summary.CompanyProfile.CompanyName);
        }).fail(function (result, status, error) {
            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error,
                "Failed to retrieve customer summary...");

            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.RETRY, self.webPortal.Resources.Strings.Retry, function () {
                    notification.dismiss();

                    // retry
                    getCustomerSummary();
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

    getCustomerSummary();
};

//@ sourceURL=CustomerPagePresenter.js