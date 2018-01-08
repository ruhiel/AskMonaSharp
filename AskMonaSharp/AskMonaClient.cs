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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AskMonaSharp
{
    public class AskMonaClient : IDisposable
    {
        private const string _Host = "http://askmona.org";

        private HttpClient client;

        public AskMonaClient(string appSecretKey = default(string))
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
        public Task<SecretKey> Secretkey(int appID, string userAddress, string password, string appSecretKey) => Execute<SecretKey>(nameof(Secretkey), new { app_id = appID, app_secretkey = appSecretKey, u_address = userAddress, pass = password });

        [Query(Method.POST, "/v1/users/myprofile")]
        public Task<MyProfile> MyProfile(int appID, int userID, string userName, string profile, string appSecretKey, string authSecretKey)
        {
            var nonce = GetRandomString();
            var time = GetNowUnixTime();
            var authKey = CreateAuthKey(nonce, time, appSecretKey, authSecretKey);

            return Execute<MyProfile>(nameof(MyProfile), new { app_id = appID, u_id = userID, nonce = nonce, time = time, auth_key = authKey, u_name = userName, profile = profile });
        }

        [Query(Method.POST, "/v1/auth/verify")]
        public Task<Verify> Verify(int appID, int userID, string appSecretKey, string authSecretKey)
        {
            var nonce = GetRandomString();
            var time = GetNowUnixTime();
            var authKey = CreateAuthKey(nonce, time, appSecretKey, authSecretKey);

            return Execute<Verify>(nameof(Verify), new { app_id = appID, u_id = userID, nonce = nonce, time = time, auth_key = authKey});
        }

        private string CreateAuthKey(string nonce, long time, string appSecretKey, string authSecretKey) => $"{appSecretKey}{nonce}{time}{authSecretKey}".ToSHA256Hash().ToBase64String();


        private async Task<T> Execute<T>(string methodName, object obj)
        {
            var query = GetQuery(GetAllMethod(methodName));

            var result = default(string);
            if(query.Item2 == Method.GET)
            {
                result = await client.GetStringAsync(CreateURI(query, obj));
            }
            else
            {
                var content = new FormUrlEncodedContent(obj.Compact().ToDictionary(x => x.Key, y => y.Value.ToString()));

                var response = await client.PostAsync(CreateURI(query), content);

                result = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<T>(result);
        }

        private string GetRandomString(int length = 32)
        {
            var random = new byte[32];

            //RNGCryptoServiceProviderクラスのインスタンスを作成
            var rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            //または、次のようにもできる
            //System.Security.Cryptography.RandomNumberGenerator rng =
            //    System.Security.Cryptography.RandomNumberGenerator.Create();

            //バイト配列に暗号化に使用する厳密な値のランダムシーケンスを設定
            rng.GetBytes(random);

            return random.ToBase64String();
        }

        /// <summary>
        /// 現在のUNIX時刻を作成＆リターン
        /// </summary>
        public static long GetNowUnixTime()
        {
            //現在時刻のDateTimeオブジェクト
            var dt = DateTime.Now;
            //UTC時刻に変更
            dt = dt.ToUniversalTime();
            //現在時刻のDateTimeからUNIXエポック時刻のDateTimeを引いて、その結果を秒数で表す
            return (long)dt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

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
