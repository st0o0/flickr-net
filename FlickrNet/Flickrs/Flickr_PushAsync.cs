using FlickrNet.CollectionModels;
using FlickrNet.Enums;
using FlickrNet.Flickrs.Results;
using FlickrNet.Models;
using System;
using System.Collections.Generic;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Get a list of subscriptions for the calling user.
        /// </summary>
        /// <param name="callback"></param>
        public void PushGetSubscriptionsAsync(Action<FlickrResult<SubscriptionCollection>> callback)
        {
            CheckRequiresAuthentication();

            Dictionary<string, string> parameters = new() { { "method", "flickr.push.getSubscriptions" } };

            GetResponseAsync<SubscriptionCollection>(parameters, callback);
        }

        /// <summary>
        /// Get a list of topics that are available for subscription.
        /// </summary>
        /// <param name="callback"></param>
        public void PushGetTopicsAsync(Action<FlickrResult<string[]>> callback)
        {
            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.push.getTopics" }
            };

            GetResponseAsync<UnknownResponse>(parameters, r =>
            {
                if (callback == null)
                {
                    return;
                }

                if (r.HasError)
                {
                    callback(new FlickrResult<string[]>() { Error = r.Error });
                    return;
                }

                string[] topics = r.Result.GetElementArray("topic", "name");

                callback(new FlickrResult<string[]>() { Result = topics });
                return;
            });
        }

        /// <summary>
        /// Subscribe to a particular topic.
        /// </summary>
        /// <param name="topic">The topic to subscribe to.</param>
        /// <param name="callback">The callback url.</param>
        /// <param name="verify">Either 'sync' or 'async'.</param>
        /// <param name="verifyToken">An optional token to be sent along with the verification.</param>
        /// <param name="leaseSeconds">The number of seconds the lease should be for.</param>
        /// <param name="woeIds">An array of WOE ids to listen to. Only applies if topic is 'geo'.</param>
        /// <param name="placeIds">An array of place ids to subscribe to. Only applies if topic is 'geo'.</param>
        /// <param name="latitude">The latitude to subscribe to. Only applies if topic is 'geo'.</param>
        /// <param name="longitude">The longitude to subscribe to. Only applies if topic is 'geo'.</param>
        /// <param name="radius">The radius to subscribe to. Only applies if topic is 'geo'.</param>
        /// <param name="radiusUnits">The raduis units to subscribe to. Only applies if topic is 'geo'.</param>
        /// <param name="accuracy">The accuracy of the geo search to subscribe to. Only applies if topic is 'geo'.</param>
        /// <param name="nsids">A list of Commons Institutes to subscribe to.
        /// Only applies if topic is 'commons'. If not present this argument defaults to all Flickr Commons institutions.</param>
        /// <param name="tags">A list of strings to be used for tag subscriptions.
        /// Photos with one or more of the tags listed will be included in the subscription.
        /// Only valid if the topic is 'tags'</param>
        /// <param name="callbackAction"></param>
        public void PushSubscribeAsync(string topic, string callback, string verify, string verifyToken,
                                       int leaseSeconds, int[] woeIds, string[] placeIds, double latitude,
                                       double longitude, int radius, RadiusUnit radiusUnits, GeoAccuracy accuracy,
                                       string[] nsids, string[] tags, Action<FlickrResult<NoResponse>> callbackAction)
        {
            CheckRequiresAuthentication();

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(callback))
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (string.IsNullOrEmpty(verify))
            {
                throw new ArgumentNullException(nameof(verify));
            }

            if (topic == "tags" && (tags == null || tags.Length == 0))
            {
                throw new InvalidOperationException("Must specify at least one tag is using topic of 'tags'");
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.push.subscribe" },
                { "topic", topic },
                { "callback", callback },
                { "verify", verify }
            };
            if (!string.IsNullOrEmpty(verifyToken))
            {
                parameters.Add("verify_token", verifyToken);
            }

            if (leaseSeconds > 0)
            {
                parameters.Add("lease_seconds",
                               leaseSeconds.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            }

            if (woeIds != null && woeIds.Length > 0)
            {
                List<string> woeIdList = new();
                foreach (int i in woeIds)
                {
                    woeIdList.Add(i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
                }
                parameters.Add("woe_ids", string.Join(",", woeIdList.ToArray()));
            }
            if (placeIds != null && placeIds.Length > 0)
            {
                parameters.Add("place_ids", string.Join(",", placeIds));
            }

            if (radiusUnits != RadiusUnit.None)
            {
                parameters.Add("radius_units", radiusUnits.ToString("d"));
            }

            GetResponseAsync<NoResponse>(parameters, callbackAction);
        }

        /// <summary>
        /// Unsubscribe from a particular push subscription.
        /// </summary>
        /// <param name="topic">The topic to unsubscribe from.</param>
        /// <param name="callback">The callback url to unsubscribe.</param>
        /// <param name="verify">Either 'sync' or 'async'.</param>
        /// <param name="verifyToken">The verification token to include in the unsubscribe verification process.</param>
        /// <param name="callbackAction"></param>
        public void PushUnsubscribeAsync(string topic, string callback, string verify, string verifyToken, Action<FlickrResult<NoResponse>> callbackAction)
        {
            CheckRequiresAuthentication();

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(callback))
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (string.IsNullOrEmpty(verify))
            {
                throw new ArgumentNullException(nameof(verify));
            }

            Dictionary<string, string> parameters = new()
            {
                { "method", "flickr.push.unsubscribe" },
                { "topic", topic },
                { "callback", callback },
                { "verify", verify }
            };
            if (!string.IsNullOrEmpty(verifyToken))
            {
                parameters.Add("verif_token", verifyToken);
            }

            GetResponseAsync<NoResponse>(parameters, callbackAction);
        }
    }
}