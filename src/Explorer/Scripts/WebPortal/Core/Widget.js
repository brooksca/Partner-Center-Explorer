Microsoft.WebPortal.Core.Widget = function (webPortal, feature, title) {
    /// <summary>
    /// The presenter class is the base class all presenter must ultimately extend. Presenters can be activated, deactivated and destroyed.
    /// When a presenter is activated, it owns the UI and it can use the different portal services to render its feature. A presenter can
    /// be deactivated when the user navigates to another feature. It is still kept in memory but no longer owns the UI. A deactivated
    /// presenter can maintain its state so that when it is reactivated it will show the expected state. When a presenter is no longer needed
    /// it will be detroyed in which it should clean up all its resources.
    /// </summary>
    /// <param name="webPortal">The web portal instance.</param>
    /// <param name="feature">The feature this presenter owns.</param>
    /// <param name="title">The title of the presenter. This is useful for display purposes such as breadcrumb trails.</param>

    if (!webPortal) {
        throw new Error("Microsoft.WebPortal.Core.Widget.Constrcutor: Invalid web portal instance.");
    }

    this.webPortal = webPortal;

    this.webPortal.Helpers.throwIfNotSet(feature, "feature", "Microsoft.WebPortal.Core.Widget.Constructor");
    this.webPortal.Helpers.throwIfNotSet(title, "title", "Microsoft.WebPortal.Core.Widget.Constructor");

    this.feature = feature;
    this.title = ko.observable(title);

    this.state = Microsoft.WebPortal.Core.Presenter.State.Initialized;
    this.webPortal.Diagnostics.information(this.title() + " widget created");
}

Microsoft.WebPortal.Core.Widget.prototype.activate = function (context) {
    /// <summary>
    /// Called when the presenter is activated.
    /// </summary>
    /// <param name="context">An optional parameter sent to the presenter.</param>

    this.state = Microsoft.WebPortal.Core.Presenter.State.ForeGround;

    this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.WidgetActivated, {
        feature: this.feature,
        presenter: this,
        context: context
    });

    this.webPortal.Diagnostics.information(this.title() + " widget activated");
}

Microsoft.WebPortal.Core.Widget.prototype.deactivate = function (context) {
    /// <summary>
    /// Called when the presenter is no longer active.
    /// </summary>
    /// <param name="context">An optional parameter sent to the presenter.</param>

    this.state = Microsoft.WebPortal.Core.Widget.State.Background;

    this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.WidgetDeactivated, {
        Feature: this.feature,
        Presenter: this,
        Context: context
    });

    this.webPortal.Diagnostics.information(this.title() + " widget deactivated");
}

Microsoft.WebPortal.Core.Widget.prototype.destroy = function (context) {
    /// <summary>
    /// Called when the presenter is to be destroyed.
    /// </summary>
    /// <param name="context">An optional parameter sent to the presenter.</param>

    // deactivate the presenter first
    this.deactivate();

    this.state = Microsoft.WebPortal.Core.Widget.State.Destroyed;

    this.webPortal.EventSystem.broadcast(Microsoft.WebPortal.Event.WidgetDestroyed, {
        Feature: this.feature,
        Presenter: this,
        Context: context
    });

    this.webPortal.Diagnostics.information(this.title() + " widget destroyed");
}

/*
    The different state for a widget.
*/
Microsoft.WebPortal.Core.Widget.State = {
    /*
        Widget has been initialized.
    */
    Initialized: 0,

    /*
        Widget is now active and owns the UI.
    */
    ForeGround: 1,

    /*
        Widget is no longer active and does not own the UI anymore.
    */
    Background: 2,

    /*
        Widget has been destroyed.
    */
    Destroyed: 3
}

//@ sourceURL=Widget.js