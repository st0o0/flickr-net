using FlickrNetCore.Enums;
using System;
using System.Xml;

namespace FlickrNetCore.Common
{
    /// <summary>
    /// Configuration settings for the Flickr.Net API Library.
    /// </summary>
    /// <remarks>
    /// <p>First, register the configuration section in the configSections section:</p>
    /// <p><code>&lt;configSections&gt;
    /// &lt;section name="flickrNet" type="FlickrNet.FlickrConfigurationManager,FlickrNet"/&gt;
    /// &lt;/configSections&gt;</code>
    /// </p>
    /// <p>
    /// Next, include the following config section:
    /// </p>
    /// <p><code>
    ///     &lt;flickrNet
    /// apiKey="1234567890abc"    // optional
    /// secret="2134123"        // optional
    /// token="234234"            // optional
    /// cacheSize="1234"        // optional, in bytes (defaults to 50 * 1024 * 1024 = 50MB)
    /// cacheTimeout="[d.]HH:mm:ss"    // optional, defaults to 1 day (1.00:00:00) - day component is optional
    /// // e.g. 10 minutes = 00:10:00 or 1 hour = 01:00:00 or 2 days, 12 hours = 2.12:00:00
    /// &gt;
    /// &lt;proxy        // proxy element is optional, but if included the attributes below are mandatory as mentioned
    /// ipaddress="127.0.0.1"    // mandatory
    /// port="8000"                // mandatory
    /// username="username"        // optional, but must have password if included
    /// password="password"        // optional, see username
    /// domain="domain"            // optional, used for Microsoft authenticated proxy servers
    /// /&gt;
    /// &lt;/flickrNet&gt;
    /// </code></p>
    /// </remarks>
    public class FlickrConfigurationSettings
    {
        private readonly string apiToken;
        private readonly string cacheLocation;
        private readonly SupportedService service;

        /// <summary>
        /// Loads FlickrConfigurationSettings with the settings in the config file.
        /// </summary>
        /// <param name="configNode">XmlNode containing the configuration settings.</param>
        public FlickrConfigurationSettings(XmlNode configNode)
        {
            if (configNode?.Attributes == null)
            {
                throw new ArgumentNullException(nameof(configNode));
            }

            foreach (XmlAttribute attribute in configNode.Attributes)
            {
                switch (attribute.Name)
                {
                    case "apiKey":
                        ApiKey = attribute.Value;
                        break;

                    case "secret":
                        SharedSecret = attribute.Value;
                        break;

                    case "token":
                        apiToken = attribute.Value;
                        break;

                    case "cacheDisabled":
                        try
                        {
                            CacheDisabled = bool.Parse(attribute.Value);
                            break;
                        }
                        catch (FormatException ex)
                        {
                            throw new System.Configuration.ConfigurationErrorsException("cacheDisbled should be \"true\" or \"false\"", ex, configNode);
                        }
                    case "cacheSize":
                        try
                        {
                            CacheSize = int.Parse(attribute.Value, System.Globalization.NumberFormatInfo.InvariantInfo);
                            break;
                        }
                        catch (FormatException ex)
                        {
                            throw new System.Configuration.ConfigurationErrorsException("cacheSize should be integer value", ex, configNode);
                        }
                    case "cacheTimeout":
                        try
                        {
                            CacheTimeout = TimeSpan.Parse(attribute.Value);
                            break;
                        }
                        catch (FormatException ex)
                        {
                            throw new System.Configuration.ConfigurationErrorsException("cacheTimeout should be TimeSpan value ([d:]HH:mm:ss)", ex, configNode);
                        }
                    case "cacheLocation":
                        cacheLocation = attribute.Value;
                        break;

                    case "service":
                        try
                        {
                            service = (SupportedService)Enum.Parse(typeof(SupportedService), attribute.Value, true);
                            break;
                        }
                        catch (ArgumentException ex)
                        {
                            throw new System.Configuration.ConfigurationErrorsException(
                                "service must be one of the supported services (See SupportedServices enum)", ex, configNode);
                        }

                    default:
                        throw new System.Configuration.ConfigurationErrorsException(
                            string.Format(
                                System.Globalization.CultureInfo.InvariantCulture,
                                "Unknown attribute '{0}' in flickrNet node", attribute.Name), configNode);
                }
            }

            foreach (XmlNode node in configNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "proxy":
                        break;

                    default:
                        throw new System.Configuration.ConfigurationErrorsException(
                            string.Format(System.Globalization.CultureInfo.InvariantCulture,
                            "Unknown node '{0}' in flickrNet node", node.Name), configNode);
                }
            }
        }

        /// <summary>
        /// API key. Null if not present. Optional.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Shared Secret. Null if not present. Optional.
        /// </summary>
        public string SharedSecret { get; }

        /// <summary>
        /// API token. Null if not present. Optional.
        /// </summary>
        public string ApiToken
        {
            get { return apiToken; }
        }

        /// <summary>
        /// Cache size in bytes. 0 if not present. Optional.
        /// </summary>
        public bool CacheDisabled { get; }

        /// <summary>
        /// Cache size in bytes. 0 if not present. Optional.
        /// </summary>
        public int CacheSize { get; }

        /// <summary>
        /// Cache timeout. Equals TimeSpan.MinValue is not present. Optional.
        /// </summary>
        public TimeSpan CacheTimeout { get; } = TimeSpan.MinValue;

        public string CacheLocation
        {
            get { return cacheLocation; }
        }

        public SupportedService Service
        {
            get { return service; }
        }
    }
}