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
        public Task<TopicList> TopicsListAsync(Category? category = null, string tag = null, bool? safe = null, Order? order = null, int? limit = null, int? offset = null) => Execute<TopicList>(nameof(TopicsListAsync), new { cat_id = category.ToValue(), tag = tag, safe = safe.ToValue(), order = order.ToValue(), limit = limit, offset = offset });

        [Query(Method.GET, "/v1/responses/list")]
        public Task<ResponseList> ResponsesList(int topicId, int? from = null, int? to = null, bool? topic_detail = null, bool? if_updated_since = null, int? if_modified_since = null) => Execute<ResponseList>(nameof(TopicsListAsync), new { t_id = topicId, from = from, to = to, topic_detail = topic_detail.ToValue(), if_updated_since = if_updated_since.ToValue(), if_modified_since = if_modified_since });

        [Query(Method.GET, "/v1/users/profile")]
        public Task<Profile> Profile(int userId) => Execute<Profile>(nameof(Profile), new { u_id = userId });

        [Query(Method.POST, "/v1/auth/secretkey")]
        public async Task<SecretKey> Secretkey(int appID, string appSecretkey, string userAddress, string password)
        {
            var query = GetQuery(GetAllMethod(nameof(Secretkey)));

            var content = new FormUrlEncodedContent(new { app_id = appID, app_secretkey = appSecretkey, u_address = userAddress, pass = password }.Compact().ToDictionary(x => x.Key, y => y.Value.ToString()));

            var response = await client.PostAsync(CreateURI(query), content);

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SecretKey>(result);
        }

        private async Task<T> Execute<T>(string methodName, object obj)
        {
            var query = GetQuery(GetAllMethod(methodName));

            var result = await client.GetStringAsync(CreateURI(query, obj));

            return JsonConvert.DeserializeObject<T>(result);
        }

        private string GetRandomString(int length = 32) => Guid.NewGuid().ToString("N").Substring(0, length);

        private long UnixEpochTime() => (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;

        private string CreateURI(Tuple<string, Method> query, object obj = null)
        {
            var uri = new UriBuilder(_Host + query.Item1)
            {
                Query = obj == null ? null : CreateGetContent(obj)
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
