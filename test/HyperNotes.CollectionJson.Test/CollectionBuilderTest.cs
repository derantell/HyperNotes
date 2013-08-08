using System;
using System.Linq;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;

namespace HyperNotes.CollectionJson.Test {
    public class CollectionBuilderTest {
        
        public class Build_method {
            private Fixture _fixture;

            public Build_method() {
                _fixture = new Fixture();
            }

            [Fact]
            public void should_return_a_new_collectionJson_instance_with_specified_href() {
                var builder = DefineCollection.For<TestModel>(CollectionHref);
                var model = _fixture.Create<TestModel>();

                var doc = builder.Build(new[]{model});

                doc.collection.href.Should().Be(CollectionHref);
                doc.collection.version.Should().Be("1.0");
            }
        }

        public class Links_method {
            private Fixture _fixture;

            public Links_method() {
                _fixture = new Fixture();
            }

            [Fact]
            public void should_add_links_to_the_collection() {
                var builder = DefineCollection.For<TestModel>(CollectionHref);
                var model = _fixture.Create<TestModel>();
                var links = _fixture.CreateMany<Link>(3).ToArray();

                var doc = builder
                    .Links(links)
                    .Build(new[]{model});

                doc.collection.links.ShouldBeEquivalentTo(links);
            }
        }


        public class Items_method {
            private Fixture _fixture;

            public Items_method() {
                _fixture = new Fixture();
            }

            [Fact]
            public void should_add_generators_to_builder() {
                var builder = DefineCollection.For<TestModel>( CollectionHref);
                var models = _fixture.CreateMany<TestModel>(3);

                builder.Items(
                    href: model => CollectionHref + "/" + model.Name,
                    links: model => new[] {new Link()},
                    data: model => new[] {
                        new Data {name = "name", prompt = "Name", value = model.Name},
                        new Data {name = "age", prompt = "Age", value = model.Age.ToString()},
                        new Data {name = "created", prompt = "Creation date", value = model.CreationDate.ToString()},
                    }
                );

                var doc = builder.Build(models);

                doc.collection.items.Should().HaveCount(3);
            }
        }
        private const string CollectionHref = "http://example.com/collection";
    }

    public class DefineCollectionTest {
        public class For_method {

            [Fact]
            public void should_return_a_builder_for_the_specified_model_class() {
                var builder = DefineCollection.For<TestModel>("http://example.com/collection");

                builder.Should().BeOfType<CollectionBuilder<TestModel>>();
            }
        }
    }

    public class TestModel {
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime CreationDate { get; set; }
    }

}