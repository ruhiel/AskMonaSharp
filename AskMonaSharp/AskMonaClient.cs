using AskMonaSharp.Attribute;
using AskMonaSharp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public async Task<TopicList> TopicsListAsync()
        {
            var query = GetQuery(GetAllMethod(nameof(TopicsListAsync)));

            var baseUrl = _Host + query.Item1;

            var result = await client.GetStringAsync(baseUrl);

            return JsonConvert.DeserializeObject<TopicList>(result);
        }

        [Query(Method.GET, "/v1/responses/list")]
        public async Task<ResponseList> ResponsesList(int topicId)
        {
            var query = GetQuery(GetAllMethod(nameof(ResponsesList)));

            var baseUrl = _Host + query.Item1;

            var uri = new UriBuilder(baseUrl)
            {
                Query = CreateGetContent(t_id => topicId)
            }.ToString();

            var result = await client.GetStringAsync(uri);

            return JsonConvert.DeserializeObject<ResponseList>(result);
        }

        private static string CreateGetContent(params Expression<Func<object, object>>[] exprs)
        {
            var contents = HttpUtility.ParseQueryString(string.Empty);
            foreach (var expr in exprs)
            {
                var obj = expr.Compile().Invoke(null);
                if (obj == null) continue;

                var param = obj.ToString();
                if (string.IsNullOrEmpty(param)) continue;

                contents[expr.Parameters[0].Name] = HttpUtility.UrlEncode(param);
            }
            return contents.ToString();
        }


        private MethodInfo GetAllMethod(string methodName) => typeof(AskMonaClient).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        private MethodInfo GetAllMethod(string methodName, Type[] types) => typeof(AskMonaClient).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, types, null);

        private Tuple<string, Method> GetQuery(MethodBase methodBase)
        {
            var queryAttribute = (QueryAttribute)methodBase.GetCustomAttribute(typeof(QueryAttribute));
            return Tuple.Create(queryAttribute.Query, queryAttribute.Method);
        }
    }
}
