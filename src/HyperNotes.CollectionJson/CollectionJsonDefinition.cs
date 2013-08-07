using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperNotes.CollectionJson {
    public abstract class CollectionJsonDefinition<TModel> {
        public CollectionJson CreateCollection(IEnumerable<TModel> collection) {
            var builder = GetBuilder();
            return builder.Build(collection);
        }

        public CollectionJson CreateCollection(TModel model) {
            return CreateCollection(new[] {model});
        }

        protected abstract CollectionBuilder<TModel> GetBuilder();

        protected static Data Data(string name, string value = null, string prompt = null) {
            return new Data {name = name, value = value, prompt = prompt};
        }

        protected static Link Link(string rel, string href, string name = null, string prompt = null, string render =null) {
            return new Link { rel = rel, href = href, name = name, prompt = prompt, render = render };
        }

        protected static Query Query(string rel, string href, string name = null, string prompt = null, IEnumerable<Data> data = null) {
            return new Query { rel = rel, href = href, name = name, prompt = prompt, data = data };
        }
    }

    public static class DefineCollection {
        public static CollectionBuilder<TModel> For<TModel>(string href, string version = "1.0") {
            return new CollectionBuilder<TModel>( href, version );
        }
    }
    
    public class CollectionBuilder<TModel> {
        public CollectionJson Build(IEnumerable<TModel> model) {
            var collection = new Collection {
                href = _href,
                version = _version,
            };

            if (_links != null) {
                collection.links = _links;
            }

            if (model.Any()) {
                collection.items = model.Select(m => new Item {
                    href = _itemHrefGenerator(m),
                    links = _itemLinkGenerator(m),
                    data = _itemDataGenerator(m)
                });
            }

            if (_templateData != null) {
                collection.template = new Template {data = _templateData};
            }
            
            if (_queries != null) {
                collection.queries = _queries;
            }

            return new CollectionJson {collection = collection};
        }


        public CollectionBuilder<TModel> Links(params Link[] links) {
            _links = links;
            return this;
        } 

        public CollectionBuilder<TModel> Items(
            Func<TModel, string> href, 
            Func<TModel, IEnumerable<Data>> data = null, 
            Func<TModel, IEnumerable<Link>> links = null ) 
        {
            _itemHrefGenerator = href ?? (_ => null);
            _itemDataGenerator = data ?? (_ => null);
            _itemLinkGenerator = links ?? (_ => null);
            return this;
        } 

        public CollectionBuilder<TModel> Template(params Data[] data) {
            _templateData = data;
            return this;
        } 

        public CollectionBuilder<TModel> Queries(params Query[] queries) {
            _queries = queries;
            return this;
        }

        public CollectionBuilder(string href, string version) {
            _href = href;
            _version = version;
        }

        private string _href;
        private string _version;
        private IEnumerable<Link> _links;
        private Func<TModel, string> _itemHrefGenerator = (_ => null);
        private Func<TModel, IEnumerable<Data>> _itemDataGenerator = (_ => null);
        private Func<TModel, IEnumerable<Link>> _itemLinkGenerator = (_ => null);
        private IEnumerable<Data> _templateData;
        private IEnumerable<Query> _queries;
    }
}