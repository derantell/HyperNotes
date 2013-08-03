using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using HyperNotes.CollectionJson;
using Nancy.ModelBinding;
using Nancy.ModelBinding.DefaultBodyDeserializers;

namespace HyperNotes.Api.Infrastructure {
    public class CollectionJsonBodyDeserializer : IBodyDeserializer {
        public bool CanDeserialize(string contentType) {
            return "application/vnd.collection+json".Equals(contentType, StringComparison.InvariantCultureIgnoreCase);
        }


        public object Deserialize(string contentType, Stream bodyStream, BindingContext context) {
            var jsonBodyDeserializer = new JsonBodyDeserializer();
            
            var destinationType = context.DestinationType;
            context.DestinationType = typeof (Collection);
            
            var collection = (Collection) jsonBodyDeserializer.Deserialize("application/json", bodyStream, context);

            context.DestinationType = destinationType;

            var result = Activator.CreateInstance(destinationType);

            var properties = destinationType.GetProperties().ToDictionary(
                prop => prop.Name.ToUpper(), prop => prop);

            foreach (var data in collection.template.data) {
                var compareName = data.name.ToUpper();
                PropertyInfo property;
                if (properties.TryGetValue(compareName, out property)) {
                    property.SetValue(result, Convert.ChangeType(data.value, property.PropertyType));
                }
            }

            return result;
        }
    }
}