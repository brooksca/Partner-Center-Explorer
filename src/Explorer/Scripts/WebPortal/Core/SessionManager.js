Microsoft.WebPortal.Core.SessionManager = function (webPortal) {
    /// <summary>
    /// Stores session information.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>

    this.webPortal = webPortal;

    // Shell, please let us know when you have finished initializing
    this.webPortal.EventSystem.subscribe(Microsoft.WebPortal.Event.PortalInitializing, this.initialize, this);

    this.webPortal.EventSystem.subscribe(Microsoft.WebPortal.Event.FeatureDeactivated, this.onFeatureDeactivated, this);

    // a hashtable that caches the HTML template for each feature
    this.featureTemplates = {};
};

Microsoft.WebPortal.Core.SessionManager.prototype.initialize = function (eventId, context, broadcaster) {
    /// <summary>
    /// Called when the portal is initializing.
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="context"></param>
    /// <param name="broadcaster"></param>

    // prefetch Microsoft offers
    this.fetchMicrosoftOffers($.Deferred());

    // assign feature presenters
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.Home, Microsoft.WebPortal.HomePagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.Customer, Microsoft.WebPortal.CustomerPagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.CustomerAddNew, Microsoft.WebPortal.CustomerAddNewPagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.CustomerAddConfirmation, Microsoft.WebPortal.CustomerAddConfirmationPagePresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.AddSubscriptions, Microsoft.WebPortal.AddSubscriptionsPresenter);
    this.webPortal.registerFeaturePresenter(Microsoft.WebPortal.Feature.Subscription, Microsoft.WebPortal.SubscriptionPagePresenter);
}

Microsoft.WebPortal.Core.SessionManager.prototype.onFeatureDeactivated = function () {
    /// <summary>
    /// Called whenever a feature is deactivated.
    /// </summary>

    // clear out the actions bar
    this.webPortal.Services.Actions.clear();
}

Microsoft.WebPortal.Core.SessionManager.prototype.fetchMicrosoftOffers = function (resolver) {
    /// <summary>
    /// Retrieves and stores Microsoft offers in memory.
    /// </summary>
    /// <param name="resolver">A JQuery deferred object which will be notified with the offers once they
    /// are available or get a rejection if there was a failure retrieving them.</param>

    if (this.MicrosoftOffers) {
        resolver.resolve(this.MicrosoftOffers);
        return;
    }

    var getMicrosoftOffersServerCall =
        new Microsoft.WebPortal.Utilities.RetryableServerCall(this.webPortal.Helpers.ajaxCall("api/offer", Microsoft.WebPortal.HttpMethod.Get))

    var self = this;

    getMicrosoftOffersServerCall.execute().done(function (microsoftOffers) {
        self.MicrosoftOffers = microsoftOffers;
        self.webPortal.Diagnostics.information("Acquired Microsoft offers");
        resolver.resolve(self.MicrosoftOffers);
    }).fail(function (result, status, error) {
        self.MicrosoftOffers = null;
        self.webPortal.Diagnostics.error("Failed to acquired Microsoft offers: " + error);
        resolver.reject();
    });
}

Microsoft.WebPortal.Core.SessionManager.prototype.fetchPortalOffers = function (resolver) {
    /// <summary>
    /// Retrieves and stores Portal offers in memory.
    /// </summary>
    /// <param name="resolver">A JQuery deferred object which will be notified with the offers once they
    /// are available or get a rejection if there was a failure retrieving them.</param>

    var getPortalOffersServerCall =
        new Microsoft.WebPortal.Utilities.RetryableServerCall(this.webPortal.Helpers.ajaxCall("api/offer", Microsoft.WebPortal.HttpMethod.Get))

    var self = this;

    getPortalOffersServerCall.execute()
        .done(function (portalOffers) {
            self.PortalOffers = portalOffers;

            //setup an Id based mapped observable array for Portal offer items.
            self.IdMappedPortalOffers = ko.utils.arrayMap(self.PortalOffers, function (offerItem) {
                return {
                    OriginalOffer: offerItem,
                    Id: offerItem.Id
                }
            });

            self.webPortal.Diagnostics.information("Acquired Portal offers");
            resolver.resolve(self.PortalOffers);
        })
        .fail(function (result, status, error) {
            self.IdMappedPortalOffers = null;
            self.webPortal.Diagnostics.error("Failed to acquired Portal offers: " + error);
            resolver.reject();
        });
}

Microsoft.WebPortal.Core.SessionManager.prototype.fetchCustomerDetails = function (resolver) {
    /// <summary>
    /// Retrieves the customer account details for use across the app.
    /// </summary>
    /// <param name="resolver">A JQuery deferred object which will be notified with the customer details once they
    /// are available or get a rejection if there was a failure retrieving them.</param>

    if (this.CustomerDetails) {
        resolver.resolve(this.CustomerDetails);
        return;
    }

    var getCustomerServerCall =
        new Microsoft.WebPortal.Utilities.RetryableServerCall(this.webPortal.Helpers.ajaxCall("api/customer", Microsoft.WebPortal.HttpMethod.Get))

    var self = this;

    getCustomerServerCall.execute()
        .done(function (customerInfo) {
            self.CustomerDetails = customerInfo;
            self.webPortal.Diagnostics.information("Acquired Customer Information.");
            resolver.resolve(self.CustomerDetails);
        })
        .fail(function (result, status, error) {
            self.CustomerDetails = null;
            self.webPortal.Diagnostics.error("Failed to acquire Customer Information: " + error);
            resolver.reject();
        });
}

//@ sourceURL=SessionManager.js
