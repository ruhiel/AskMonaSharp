using AskMonaSharp.Attribute;
using AskMonaSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AskMonaSharp
{
    public class AskMonaClient : IDisposable
    {
        private const string _Host = "http://askmona.org";

        private HttpClient client;

        public AskMonaClient()
        {
            client = new HttpClient();
        }

        public void Dispose() => client?.Dispose();

        [Query(Method.GET, "/v1/topics/list")]
        public async Task<TopicList> TopicsListAsync(Category? category = null, string tag = null, bool? safe = null, Order? order = null, int? limit = null, int? offset = null)
        {
            var query = GetQuery(GetAllMethod(nameof(TopicsListAsync)));

            var result = await client.GetStringAsync(CreateURI(query, new { cat_id = category.ToValue(), tag = tag, safe = safe.ToValue(), order = order.ToValue(), limit = limit, offset = offset}));

            return JsonConvert.DeserializeObject<TopicList>(result);
        }

        [Query(Method.GET, "/v1/responses/list")]
        public async Task<ResponseList> ResponsesList(int topicId, int? from = null, int? to = null, bool? topic_detail = null, bool? if_updated_since = null, int? if_modified_since = null)
        {
            var query = GetQuery(GetAllMethod(nameof(ResponsesList)));

            var result = await client.GetStringAsync(CreateURI(query, new { t_id = topicId, from = from, to = to, topic_detail = topic_detail.ToValue(), if_updated_since = if_updated_since.ToValue(), if_modified_since = if_modified_since}));

            return JsonConvert.DeserializeObject<ResponseList>(result);
        }

        private string CreateURI(Tuple<string, Method> query, object obj)
        {
            var uri = new UriBuilder(_Host + query.Item1)
            {
                Query = CreateGetContent(obj)
            }.ToString();

            Debug.WriteLine($"uri = {uri}");

            return uri;
        }

        private string GetParam(KeyValuePair<string, object> pair)
        {
            if(!(pair.Value is string) && pair.Value is IEnumerable enumerable)
            {
                return string.Join("&", enumerable.OfType<object>().Select(x => $"{pair.Key}={x}"));
            }
            else
            {
                return $"{pair.Key}={pair.Value}";
            }
        }

        private string CreateGetContent(object obj) => string.Join("&", obj.Compact().Select(x => GetParam(x)));


        private MethodInfo GetAllMethod(string methodName) => typeof(AskMonaClient).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private MethodInfo GetAllMethod(string methodName, Type[] types) => typeof(AskMonaClient).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, types, null);

        private Tuple<string, Method> GetQuery(MethodBase methodBase)
        {
            var queryAttribute = (QueryAttribute)methodBase.GetCustomAttribute(typeof(QueryAttribute));
            return Tuple.Create(queryAttribute.Query, queryAttribute.Method);
        }
    }
}
