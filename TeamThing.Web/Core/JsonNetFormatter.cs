//using System;
//using System.IO;
//using System.Json;
//using System.Linq;
//using System.Net;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Converters;
//using Newtonsoft.Json.Serialization;

//namespace TeamThing.Web.Core
//{
//    public class JsonNetFormatter : JsonMediaTypeFormatter 
//    {
//        private readonly JsonSerializerSettings jsonSerializerSettings;

//        public JsonNetFormatter()
//        {
//            jsonSerializerSettings =
//                new JsonSerializerSettings
//                    {
//                        ContractResolver =
//                            new CamelCasePropertyNamesContractResolver()
//                    };

//            // Fill out the mediatype and encoding we support
//            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
//            //Encoding = new UTF8Encoding(false, true);
//        }

//        protected override bool CanReadType(Type type)
//        {
//            return true; //WE WILL HANDLE IKeyValues with the base implmentation
//            //return type != typeof(IKeyValueModel);
//        }

//        protected override bool CanWriteType(Type type)
//        {
//            //if (type == typeof(JsonValue) || type == typeof(JsonObject) || type == typeof(JsonArray))
//            //    return false;
//            return true;
//        }

//        protected override Task<object> OnReadFromStreamAsync(Type type,
//            Stream stream, HttpContentHeaders contentHeaders,
//            FormatterContext formatterContext)
//        {
//            Task<object> task;
//            if (type == typeof(IKeyValueModel))
//            {

//                task = base.OnReadFromStreamAsync(type, stream, contentHeaders, formatterContext);
//            }
//            else
//            {
//                task = Task<object>.Factory.StartNew(() =>
//                {
//                    var settings = new JsonSerializerSettings()
//                    {
//                        NullValueHandling = NullValueHandling.Ignore,
//                    };

//                    var sr = new StreamReader(stream);
//                    var jreader = new JsonTextReader(sr);

//                    var ser = new JsonSerializer();
//                    ser.Converters.Add(new IsoDateTimeConverter());

//                    object val = ser.Deserialize(jreader, type);
//                    return val;

//                });
//            }

//            return task;
//        }

//        protected override Task OnWriteToStreamAsync(Type type, object value,
//            Stream stream, HttpContentHeaders contentHeaders,
//            FormatterContext formatterContext,
//            TransportContext transportContext)
//        {

//            Task task = null;
//            //use the standard serilizizer for these items.
//            if (type == typeof(JsonValue) || type == typeof(JsonObject) || type == typeof(JsonArray))
//            {
//                task = base.OnWriteToStreamAsync(type, value, stream, contentHeaders, formatterContext, transportContext);
//            }
//            else
//            {
//                // Create a serializer
//                var serializer = JsonSerializer.Create(jsonSerializerSettings);

//                // Create task writing the serialized content
//                task = Task.Factory.StartNew(
//                    () =>
//                    {
//                        using (var jsonTextWriter =
//                            new JsonTextWriter(new StreamWriter(stream))
//                                {
//                                    Formatting = Formatting.Indented,
//                                    CloseOutput = false
//                                })
//                        {
//                            serializer.Serialize(jsonTextWriter, value);
//                            jsonTextWriter.Flush();
//                        }
//                    });
//            }

//            return task;
//        }
//    }
//}
