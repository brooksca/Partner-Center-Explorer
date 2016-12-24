﻿// -----------------------------------------------------------------------
// <copyright file="PluginsSegment.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Explorer.Configuration.WebPortal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Handles the plugins configuration segment.
    /// </summary>
    public class PluginsSegment : ICloneable
    {
        /// <summary>
        /// Gets or sets plugin defaults.
        /// </summary>
        [JsonProperty]
        public PluginDefaults Defaults { get; set; }

        /// <summary>
        /// Gets or sets common plugin assets.
        /// </summary>
        [JsonProperty]
        public AssetsSegment Commons { get; set; }

        /// <summary>
        /// Gets or sets supported plugins.
        /// </summary>
        [JsonProperty]
        public IList<Plugin> Plugins { get; set; }

        /// <summary>
        /// Gets or sets the default plugin.
        /// </summary>
        [JsonProperty]
        public string DefaultPlugin { get; set; }

        /// <summary>
        /// Validates the plugin configuration and ensures it is consistent and meaningful to the client.
        /// </summary>
        public void Validate()
        {
            if (Defaults == null)
            {
                throw new InvalidOperationException("Portal defaults not found");
            }

            Defaults.Validate();

            if (Commons != null)
            {
                foreach (Assets commonAssets in Commons.Assets)
                {
                    commonAssets.Validate();
                }
            }

            if (Plugins == null || Plugins.Count <= 0)
            {
                throw new InvalidOperationException("Portal plugins not found. Please add plugins.");
            }

            // these dictionaries are used to detect plugin and feature duplications
            IDictionary<string, int> pluginHashtable = new Dictionary<string, int>();
            IDictionary<string, int> featureHashtable = new Dictionary<string, int>();

            foreach (Plugin plugin in this.Plugins)
            {
                if (plugin == null)
                {
                    throw new InvalidOperationException("Portal plugin cannot be null");
                }

                plugin.SetDefaults(this.Defaults);
                plugin.Validate(featureHashtable);

                if (pluginHashtable.ContainsKey(plugin.Name))
                {
                    throw new InvalidOperationException("Duplicate plugin: " + plugin.Name);
                }

                pluginHashtable[plugin.Name] = 0;

            }

            if (string.IsNullOrWhiteSpace(DefaultPlugin))
            {
                DefaultPlugin = Plugins[0].Name;
            }
            else
            {
                if (!pluginHashtable.ContainsKey(DefaultPlugin))
                {
                    throw new InvalidOperationException($"Default plugin not found: {DefaultPlugin}");
                }
            }
        }

        /// <summary>
        /// Deep clones the plugins segment.
        /// </summary>
        /// <returns>A deep copy of the plugin segment object.</returns>
        public object Clone()
        {
            PluginsSegment clone = new PluginsSegment
            {
                Commons = Commons.Clone() as AssetsSegment,
                DefaultPlugin = DefaultPlugin,
                Defaults = Defaults.Clone() as PluginDefaults,
                Plugins = new List<Plugin>(Plugins.Select(plugin => plugin.Clone() as Plugin))
            };

            return clone;
        }
    }
}