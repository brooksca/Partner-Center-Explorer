Microsoft.WebPortal.Core.WidgetPresenter = function (webPortal, feature, title, widgetUrl, templateErrorMessage, templateProgressMessage) {
    /// <summary>
    /// The template presenter provides functionality that retrieve and render an HTML template from the server using the given template URL.
    /// HTML templates will be cached for performance improvement if the presenter is loaded again in the future.
    /// It provides hooks (callbacks) related to the rendering of the template and to the change in the presenter's state. Implement these hooks
    /// to add your own logic. These hooks are:
    /// 1. onActivate(context): called when the presenter is activated.
    /// 2. onRender(): called when the HTML template is rendered but not yet shown. Since the HTML is hidden, it may not be safe to read HTML element properties here thus yet.
    /// 3. onShow(): called after the HTML template is shown. UI dependent code can safely execute here.
    /// 4. onDeactivate(context): called when the presenter is deactivated.
    /// 5. onDestroy(context): called when the presenter is destroyed.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature this presenter owns.</param>
    /// <param name="title">The presenter title.</param>
    /// <param name="widgetUrl">The URL  of the HTML template to fetch and render.</param>
    /// <param name="templateErrorMessage">The error message to show in case of failure to retrieve the HTML template.</param>
    /// <param name="templateProgressMessage">The retry message to show in case of failure to retrieve the HTML template.</param>

    Microsoft.WebPortal.Core.Presenter.call(this, webPortal, feature, title);

    this.webPortal.Helpers.throwIfNotSet(widgetUrl, "widgetUrl", "Microsoft.WebPortal.Core.WidgetPresenter.Constructor");
    this.widgetUrl = widgetUrl;

    this.templateMessages = {
        errorMessage: templateErrorMessage || this.webPortal.Resources.Strings.TemplateLoadFailureMessage,
        progressMessage: templateProgressMessage || this.webPortal.Resources.Strings.TemplateLoadRetryMessage
    };
}

// extend the base presenter
$WebPortal.Helpers.inherit(Microsoft.WebPortal.Core.WidgetPresenter, Microsoft.WebPortal.Core.Widget);

Microsoft.WebPortal.Core.WidgetPresenter.prototype.activate = function (context) {
    /// <summary>
    /// Called when the presenter is activated.
    /// </summary>
    /// <param name="context">Context utilized to activate the widget.</param>

    this.webPortal.Helpers.throwIfNotSet(context, "context", "Microsoft.WebPortal.Core.WidgetPresenter.activate");


    if (this.onActivate) {
        // the subclass provided an activation hook, call it
        this.onActivate(context);
    }

    this._fetchMarkup(context);

    Microsoft.WebPortal.Core.Widget.prototype.activate.call(this, context);
}

Microsoft.WebPortal.Core.WidgetPresenter.prototype.deactivate = function (context) {
    /// <summary>
    /// Called when the widget is deactivated.
    /// </summary>
    /// <param name="context">An optional parameter.</param>

    if (this.onDeactivate) {
        // notify the subclass call back
        this.onDeactivate(context);
    }

    Microsoft.WebPortal.Core.Widget.prototype.deactivate.call(this, context);
}

Microsoft.WebPortal.Core.WidgetPresenter.prototype.destroy = function (context) {
    /// <summary>
    /// Called when the widget is to be destroyed.
    /// </summary>
    /// <param name="context">An optional parameter.</param>

    if (this.onDestroy) {
        // notify the subclass call back
        this.onDestroy(context);
    }

    Microsoft.WebPortal.Core.Widget.prototype.destroy.call(this, context);
}

Microsoft.WebPortal.Core.WidgetPresenter.prototype._renderUI = function () {
    /// <summary>
    /// Renders the HTML template and binds to view models.
    /// </summary>

    var self = this;

    this.webPortal.ContentPanel.render(this.template, $.Deferred().progress(function () {
        // template has been rendered and is in the process of showing
        if (self.onRender) {
            // notify the subclass call back
            self.onRender();
        }
    }).done(function () {
        // template is now shown
        if (self.onShow) {
            // notify the subclass call back
            self.onShow();
        }
    }));
}

Microsoft.WebPortal.Core.WidgetPresenter.prototype._fetchMarkup = function (context) {
    /// <summary>
    /// Retrieves the HTML template from the server and renders it.
    /// </summary>

    this.webPortal.ContentPanel.showProgress(true);

    var fetchMarkupOperation = this.webPortal.ServerCallManager.create(this.feature,
        this.webPortal.Helpers.ajaxCall(
            this.widgetUrl,
            Microsoft.WebPortal.HttpMethod.Post,
            context,
            Microsoft.WebPortal.ContentType.Json,
            120000), "FetchMarkUp(" + this.title() + ")", []);

    var self = this;

    var fetchFunction = function (existingNotification) {
        var notification = existingNotification;
        fetchMarkupOperation.execute().done(function (htmlTemplate) {
            self.webPortal.ContentPanel.hideProgress();

            if (notification) {
                // this success is a result of a retry, dismiss the progress notification
                notification.Dismiss();
            }

            // render the template
            self._renderUI();
        }).fail(function (result, status, error) {
            self.webPortal.ContentPanel.hideProgress();

            if (result.status == 403) {
                // the caller does not have the rights to view this feature
                notification = new Microsoft.WebPortal.Services.Notification(Microsoft.WebPortal.Services.Notification.NotificationType.Error, self.webPortal.Resources.Strings.AccessDeniedMessage);

                notification.buttons([
                    Microsoft.WebPortal.Services.Button.create(Microsoft.WebPortal.Services.Button.StandardButtons.OK, self.webPortal.Resources.Strings.OK, function () {
                        notification.dismiss();

                        // retract to the previous feature
                        if (!self.webPortal.Journey.retract()) {
                            // there is no previous feature, go to the portal's default feature
                            self.webPortal.Journey.start(Microsoft.WebPortal.Feature[self.webPortal.defaultTile().DefaultFeature]);
                        }
                    })
                ]);

                self.webPortal.Services.Notifications.add(notification);
            } else {
                // display an error notification
                self.webPortal.Helpers.displayRetryCancelErrorNotification(notification,
                    self.templateMessages.errorMessage, self.templateMessages.progressMessage, function (existingNotification) {
                        // reload the presenter upon if the user clicks retry
                        fetchFunction.call(self, existingNotification);
                    }
                );
            }
        });
    }

    fetchFunction();
}

//@ sourceURL=WidgetPresenter.js