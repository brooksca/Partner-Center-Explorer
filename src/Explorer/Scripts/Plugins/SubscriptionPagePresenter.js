Microsoft.WebPortal.SubscriptionPagePresenter = function (webPortal, feature, context) {
    /// <summary>
    /// Manages the home page experience. 
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature for which this presenter is created.</param>
    /// <param name="context">The context used to populate the page. This should be the JSON representation of a subscription.</param>
    this.base.constructor.call(this, webPortal, feature, "Subscription", "/Template/Subscription/");

    this.viewModel = {
        CustomerId: context.CustomerId,
        SubscriptionId: context.SubscriptionId,
        ShowProgress: ko.observable(true),
        IsSet: ko.observable(false),
    }

    this.webPortal = webPortal;
}

// inherit BasePresenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.SubscriptionPagePresenter, Microsoft.WebPortal.Core.TemplatePresenter);

Microsoft.WebPortal.SubscriptionPagePresenter.prototype.onRender = function () {
    /// <summary>
    /// Called when the presenter is rendered but not shown yet.
    /// </summary>

    var self = this;
    var summaryUrl = "api/subscription/summary?customerId=" + self.viewModel.CustomerId + "&subscriptionId=" + self.viewModel.SubscriptionId;

    ko.applyBindings(self, $("#SubscriptionContainer")[0]);

    var getSubscriptionSummary = function () {
        var getSubscriptionSummaryServerCall = self.webPortal.ServerCallManager.create(
            self.feature, self.webPortal.Helpers.ajaxCall(summaryUrl, Microsoft.WebPortal.HttpMethod.Get), "GetSubscriptionSummary");

        self.viewModel.IsSet(false);
        self.viewModel.ShowProgress(true);

        getSubscriptionSummaryServerCall.execute().done(function (summary) {
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

            var subscriptionNavigation = function () {
                var customerInfo = { Id: summary.Id }
                // Activate the customer page presenter and pass it the selected customer.
                self.webPortal.Journey.advance(Microsoft.WebPortal.Feature.Customer, customerInfo);
            };

            self.webPortal.Services.HeaderBar.resetBreadcrumbs();
            self.webPortal.Services.HeaderBar.addBreadcrumb(false, homeNavigation, 'Home');
            self.webPortal.Services.HeaderBar.addBreadcrumb(false, customerNavigation, summary.CompanyName);
            self.webPortal.Services.HeaderBar.addBreadcrumb(true, null, summary.FriendlyName)
        }).fail(function (result, status, error) {
            var notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error,
                "Failed to retrieve subscription summary...");

            notification.buttons([
                Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.RETRY, self.webPortal.Resources.Strings.Retry, function () {
                    notification.dismiss();

                    // retry
                    getSubscriptionSummary();
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

    getSubscriptionSummary();
};

//@ sourceURL=SubscriptionPagePresenter.js