﻿#region License

/*
 * Copyright 2002-2011 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;
using System.Collections.Generic;

using Spring.Rest.Client;
using Spring.Http.Client;
using Spring.Http.Converters;

namespace Spring.Social.OAuth1
{
    /// <summary>
    /// Base class for OAuth1-based provider API bindings.
    /// </summary>
    /// <author>Craig Walls</author>
    /// <author>Bruno Baia (.NET)</author>
    public abstract class AbstractOAuth1ApiBinding : IApiBinding
    {
        private bool isAuthorized;
        private RestTemplate restTemplate;

        /// <summary>
        /// Gets a reference to the REST client backing this API binding and used to perform API calls. 
        /// </summary>
        /// <remarks>
        /// Callers may use the RestTemplate to invoke other API operations not yet modeled by the binding interface. 
        /// Callers may also modify the configuration of the RestTemplate to support unit testing the API binding with a mock server in a test environment. 
        /// During construction, subclasses may apply customizations to the RestTemplate needed to invoke a specific API.
        /// </remarks>
        public RestTemplate RestTemplate
        {
            get { return this.restTemplate; }
        }

        /// <summary>
        /// Constructs the API template without user authorization. 
        /// This is useful for accessing operations on a provider's API that do not require user authorization.
        /// </summary>
        protected AbstractOAuth1ApiBinding()
        {
            this.isAuthorized = false;
            this.restTemplate = new RestTemplate();
#if !SILVERLIGHT
            ((WebClientHttpRequestFactory)restTemplate.RequestFactory).Expect100Continue = false;
#endif
            this.restTemplate.MessageConverters = this.GetMessageConverters();
        }

        /// <summary>
        /// Constructs the API template with OAuth credentials necessary to perform operations on behalf of a user.
        /// </summary>
        /// <param name="consumerKey">The application's consumer key.</param>
        /// <param name="consumerSecret">The application's consumer secret.</param>
        /// <param name="accessToken">The access token.</param>
        /// <param name="accessTokenSecret">The access token secret.</param>
        protected AbstractOAuth1ApiBinding(String consumerKey, String consumerSecret, String accessToken, String accessTokenSecret)
        {
            this.isAuthorized = false;
            this.restTemplate = new RestTemplate();
#if !SILVERLIGHT
            ((WebClientHttpRequestFactory)restTemplate.RequestFactory).Expect100Continue = false;
#endif
            this.restTemplate.RequestInterceptors.Add(new OAuth1RequestInterceptor(consumerKey, consumerSecret, accessToken, accessTokenSecret));
            this.restTemplate.MessageConverters = this.GetMessageConverters();
        }

        #region IApiBinding Membres

        /// <summary>
        /// Returns true if this API binding has been authorized on behalf of a specific user.
        /// </summary>
        /// <remarks>
        /// If so, calls to the API are signed with the user's authorization credentials, indicating an application is invoking the API on a user's behalf. 
        /// If not, API calls do not contain any user authorization information. 
        /// Callers can use this status flag to determine if API operations requiring authorization can be invoked.
        /// </remarks>
        public bool IsAuthorized
        {
            get { return this.isAuthorized; }
        }

        #endregion

        /// <summary>
        /// Returns a list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </summary>
        /// <remarks>
        /// Override this method to add additional message converters or to replace the default list of message converters.
        /// </remarks>
        /// <returns>
        /// By default, this includes a <see cref="StringHttpMessageConverter"/> and a <see cref="FormHttpMessageConverter"/>.
        /// </returns>
        protected virtual IList<IHttpMessageConverter> GetMessageConverters()
        {
            IList<IHttpMessageConverter> messageConverters = new List<IHttpMessageConverter>(2);
            messageConverters.Add(new StringHttpMessageConverter());
            messageConverters.Add(new FormHttpMessageConverter());
            //TODO: Json converter
            return messageConverters;
        }
    }
}