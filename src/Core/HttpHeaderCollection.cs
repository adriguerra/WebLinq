#region Copyright (c) 2016 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace WebLinq
{
    #region Imports

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http.Headers;
    using Collections;

    #endregion

    [DebuggerDisplay("Count = {Count}")]
    public sealed class HttpHeaderCollection : MapBase<string, IReadOnlyCollection<string>>
    {
        public static readonly HttpHeaderCollection Empty = new HttpHeaderCollection();

        readonly HttpHeaderCollection _link;

        public HttpHeaderCollection() :
            base(StringComparer.OrdinalIgnoreCase) {}

        HttpHeaderCollection(HttpHeaderCollection link, string key, IReadOnlyCollection<string> values) :
            base(key, values, link.Comparer)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (values == null) throw new ArgumentNullException(nameof(values));
            _link = link;
        }

        public HttpHeaderCollection Set(string key, string value) =>
            Set(key, ReadOnlyCollection.Singleton(value));

        public HttpHeaderCollection Set(string key, IEnumerable<string> values) =>
            new HttpHeaderCollection(this, key, values as IReadOnlyCollection<string>
                                                ?? Array.AsReadOnly(Value.ToArray()));

        public HttpHeaderCollection Set(string key, IReadOnlyCollection<string> values) =>
            new HttpHeaderCollection(this, key, values);

        internal HttpHeaderCollection Set(HttpHeaders headers) =>
            headers.Aggregate(this, (h, e) => h.Set(e.Key, e.Value));

        public HttpHeaderCollection Remove(string key) =>
            RemoveCore(Empty, key, (hs, k, vs) => hs.Set(k, vs));

        public override bool IsEmpty => _link == null;

        protected override IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>> Nodes =>
            GetNodesCore(this, h => h._link);
    }
}
