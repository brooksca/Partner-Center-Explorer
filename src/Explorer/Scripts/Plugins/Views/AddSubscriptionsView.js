/// <reference path="~/Scripts/_references.js" />

Microsoft.WebPortal.Views.AddSubscriptionsView = function (webPortal, elementSelector, defaultOffer, isShown, animation) {
    /// <summary>
    /// A view that renders UX showing a list of subscriptions to be added from a drop down list.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="elementSelector">The JQuery selector for the HTML element this view will own.</param>
    /// <param name="defaultOffer">An optional default offer to add to the subscriptions list.</param>
    /// <param name="isShown">The initial show state. Optional. Default is false.</param>
    /// <param name="animation">Optional animation to use for showing and hiding the view.</param>

    this.base.constructor.call(this, webPortal, elementSelector, isShown, null, animation);
    this.template = "addSubscriptions-template";
    var self = this;
    Globalize.culture(self.webPortal.Resources.Strings.CurrentLocale);

    // configure the subscriptions list
    self.subscriptionsList = new Microsoft.WebPortal.Views.List(this.webPortal, elementSelector + " #SubscriptionsList", this);

    self.subscriptionsList.setColumns([
        new Microsoft.WebPortal.Views.List.Column("Name", null, false, false, null, null, null, "subscriptionEntry-template")
    ]);

    self.subscriptionsList.showHeader(false);
    self.subscriptionsList.setEmptyListUI(this.webPortal.Resources.Strings.Plugins.AddSubscriptionsView.EmptyListCaption);
    self.subscriptionsList.enableStatusBar(false);
    self.subscriptionsList.setSelectionMode(Microsoft.WebPortal.Views.List.SelectionMode.None);

    self.AddOfferItemToView = function (offerItem) {
        // add this portalOffer to subcriptionList. 
        var quantity = ko.observable(1);

        self.subscriptionsList.append([{
            offer: offerItem,
            quantity: quantity
        }]);

        quantity.subscribe(function (newValue) {
            if (isNaN(parseInt(newValue))) {
                quantity(0);
            } else {
                quantity(parseInt(newValue));
            }
        }, self);

        $(elementSelector + " #SubscriptionsList").height($(elementSelector + " #SubscriptionsList table").height());
        webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResizing);
        webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResized);
    }
}

// extend the base view
$WebPortal.Helpers.inherit(Microsoft.WebPortal.Views.AddSubscriptionsView, Microsoft.WebPortal.Core.View);

Microsoft.WebPortal.Views.AddSubscriptionsView.prototype.onRender = function () {
    /// <summary>
    /// Called when the view is rendered.
    /// </summary>

    $(this.elementSelector).attr("data-bind", "template: { name: '" + this.template + "'}");
    ko.applyBindings(this, $(this.elementSelector)[0]);
}

Microsoft.WebPortal.Views.AddSubscriptionsView.prototype.onShowing = function (isShowing) {
    /// <summary>
    /// Called when the view is about to be shown or hidden.
    /// </summary>
    /// <param name="isShowing">true if showing, false if hiding.</param>

    if (isShowing) {
        this.subscriptionsList.show();
    }
    else {
        this.subscriptionsList.hide();
    }
}

Microsoft.WebPortal.Views.AddSubscriptionsView.prototype.onShown = function (isShown) {
    /// <summary>
    /// Called when the view is shown or hidden.
    /// </summary>
    /// <param name="isShown">true if shown, false if hidden.</param>

    if (isShown) {
        // resize the list to fit its content
        $(this.elementSelector + " #SubscriptionsList").height($(this.elementSelector + " #SubscriptionsList table").height());

        // force a window resize for the list to resize
        this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResizing);
        this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.OnWindowResized);
    }
}

Microsoft.WebPortal.Views.AddSubscriptionsView.prototype.onDestroy = function () {
    /// <summary>
    /// Called when the view is about to be destroyed.
    /// </summary>

    if (this.subscriptionsList) {
        this.subscriptionsList.destroy();
    }

    if ($(this.elementSelector)[0]) {
        // if the element is there, clear its bindings and clean up its content
        ko.cleanNode($(this.elementSelector)[0]);
        $(this.elementSelector).empty();
    }
}

Microsoft.WebPortal.Views.AddSubscriptionsView.prototype.onAddOfferClicked = function () {
    /// <summary>
    /// Called when the user wants to add more offers.
    /// </summary>

    var self = this;

    var selectOfferButton = Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, 1, function () {
        if (self.OfferSelectionWizardViewModel.offerList.getSelectedRows().length <= 0) {
            self.OfferSelectionWizardViewModel.errorMessage("SelectOfferErrorMessage");
            return;
        }

        for (index = 0; index < self.OfferSelectionWizardViewModel.offerList.getSelectedRows().length; ++index) {
            self.AddOfferItemToView(self.OfferSelectionWizardViewModel.offerList.getSelectedRows()[index]);
        }

        self.webPortal.Services.Dialog.hide();
    });

    var cancelButton = Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.CANCEL, 1, function () {
        // clear the offersToAdd.
        self.webPortal.Services.Dialog.hide();
    });

    var portaloffersFetchProgress = $.Deferred();
    self.webPortal.Session.fetchPortalOffers(portaloffersFetchProgress);

    portaloffersFetchProgress.done(function (portalOffers) {
        self.OfferSelectionWizardViewModel = {
            offerList: new Microsoft.WebPortal.Views.List(self.webPortal, "#offerList", self),
            baseOffers: portalOffers,
            errorMessage: ko.observable("")
        };

        // TODO: In later iterations, we will support sorting and filtering
        //this.OfferSelectionWizardViewModel.offerList.setColumns([
        //    new Microsoft.WebPortal.Views.List.Column("Name", "min-width: 300px; width: 300px; white-space: normal;", true, false,
        //        self.webPortal.Resources.Strings.Plugins.AddOrUpdateOffer.Offer),
        //    new Microsoft.WebPortal.Views.List.Column("Category", "min-width: 100px; width: 100px;", true, false, self.webPortal.Resources.Strings.Plugins.AddOrUpdateOffer.Category),
        //    new Microsoft.WebPortal.Views.List.Column("Description", "min-width: 500px; white-space: normal;", false, false, self.webPortal.Resources.Strings.Plugins.AddOrUpdateOffer.Description)
        //]);

        self.OfferSelectionWizardViewModel.offerList.setColumns([
            new Microsoft.WebPortal.Views.List.Column("Name", "min-width: 300px; width: 300px; white-space: normal;", true, false,
                "Offer"),
            new Microsoft.WebPortal.Views.List.Column("Category", "min-width: 100px; width: 100px;", true, false, "Category"),
            new Microsoft.WebPortal.Views.List.Column("Description", "min-width: 500px; white-space: normal;", false, false, "Description")
        ]);

        // this.OfferSelectionWizardViewModel.offerList.setEmptyListUI(self.webPortal.Resources.Strings.Plugins.AddOrUpdateOffer.EmptyMicrosoftOfferListMessage);
        self.OfferSelectionWizardViewModel.offerList.setEmptyListUI("EmptyMicrosoftOfferListMessage");
        self.OfferSelectionWizardViewModel.offerList.enableStatusBar(false);
        self.OfferSelectionWizardViewModel.offerList.setSelectionMode(Microsoft.WebPortal.Views.List.SelectionMode.Multiple);
        self.OfferSelectionWizardViewModel.offerList.setSorting("Name", Microsoft.WebPortal.Views.List.SortDirection.Ascending, true);

        self.OfferSelectionWizardViewModel.offerList.set(portalOffers);
        self.OfferSelectionWizardViewModel.offerList.setComplete(true);

        self.webPortal.EventSystem.subscribe(Microsoft.WebPortal.Event.DialogShown, self.onSelectBaseOfferWizardShown, self);
        self.webPortal.Services.Dialog.show("offerPicker-template", self.OfferSelectionWizardViewModel, [selectOfferButton, cancelButton]);
        self.webPortal.Services.Dialog.showProgress();
    });
}

Microsoft.WebPortal.Views.AddSubscriptionsView.prototype.onSelectBaseOfferWizardShown = function (eventId, isShown) {
    /// <summary>
    /// Called when the dialog box is shown or hidden.
    /// </summary>
    /// <param name="eventId">The event ID.</param>
    /// <param name="isShown">Indicates whether the dialog is shown or hidden.</param>

    if (isShown) {
        // show the list and hide the progress bar once the dialog is shown
        this.OfferSelectionWizardViewModel.offerList.show();
        this.webPortal.Services.Dialog.hideProgress();
    }

    // stop listening to dialog box events
    this.webPortal.EventSystem.unsubscribe(Microsoft.WebPortal.Event.DialogShown, this.onSelectBaseOfferWizardShown, this);
}

//@ sourceURL=AddSubscriptionsView.js