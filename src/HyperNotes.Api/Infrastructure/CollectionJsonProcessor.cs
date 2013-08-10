//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using HyperNotes.CollectionJson;
//using Nancy;
//using Nancy.Responses;
//using Nancy.Responses.Negotiation;

//namespace HyperNotes.Api.Infrastructure {
//    public class CollectionJsonProcessor : IResponseProcessor {

//        public CollectionJsonProcessor(IEnumerable<ISerializer> serializers) {
//            _serializer = serializers.FirstOrDefault(s => s.CanSerialize("application/json"));
//        }

//        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
//            return new ProcessorMatch {
//                ModelResult = MatchResult.DontCare,
//                RequestedContentTypeResult = GetContentTypeResult(requestedMediaRange)
//            };
//        }

//        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context) {
//            if (model == null) {
//                return new TextResponse("");
//            }

//            var definition = CollectionJsonDefinitionLocator.Find(model);

//            var collection = definition.CreateCollection(model);

//            return new JsonResponse(collection, _serializer);
//        }

//        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings {
//            get { return _extensionMappings; }
//        }

//        private static MatchResult GetContentTypeResult(MediaRange range) {
//            var isExactResult = range.IsWildcard
//                                || range.Matches(CollectionJson)
//                                || range.Matches("application/json")
//                                || range.Matches("text/json"); 

//            return isExactResult ? MatchResult.ExactMatch : MatchResult.NoMatch;
//        }


//        private readonly ISerializer _serializer;
//        private const string CollectionJson = "application/vnd.collection+json";

//        private static readonly IEnumerable<Tuple<string, MediaRange>> _extensionMappings =
//            new[] {new Tuple<string, MediaRange>("json", MediaRange.FromString(CollectionJson))};
//    };


//    public static class CollectionJsonDefinitionLocator {
//        static CollectionJsonDefinitionLocator() {
//            var assembly = Assembly.GetExecutingAssembly();

//            _cache = assembly.GetTypes()
//                .Where(t =>
//                    t.BaseType.IsGenericType()
//                    && t.BaseType.GetGenericTypeDefinition() == typeof(CollectionJsonDefinition<>)
//                )
//                .ToDictionary( t => t.BaseType.GenericTypeArguments[0], t => t );
//        }

//        public static dynamic Find(dynamic model) {
//            var modelType = ResolveType(model);

//            Type definition;

//            if (!_cache.TryGetValue(modelType, out definition)) {
//                throw new TypeLoadException("No CollectionJsonDefinition exist for type " + modelType.FullName);
//            }

//            return Activator.CreateInstance(definition);
//        }

//        private static Type ResolveType(object instance) {
//            var type = instance.GetType();

//            while (type != null) {
//                var iEnumerable = type.GetInterfaces()
//                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

//                if (iEnumerable != null) {
//                    return iEnumerable.GenericTypeArguments[0];
//                }

//                type = type.BaseType;
//            }

//            return instance.GetType();
//        }

//        private static readonly Dictionary<Type, Type> _cache;
//    }
//}