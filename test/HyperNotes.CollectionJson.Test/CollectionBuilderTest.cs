using System;
using FluentAssertions;
using Ploeh.AutoFixture;
using Xunit;

namespace HyperNotes.CollectionJson.Test {
    public class CollectionBuilderTest {
        public CollectionBuilderTest() {
            collectionBuilder = DefineCollection.For<TestModel>(CollectionHref);
        }

        public class Build_method {
            [Fact]
            public void should_return_a_new_collectionJson_instance() {
                var collection = collectionBuilder
            }
        }

        private const string CollectionHref = "http://example.com/collection";
        private CollectionBuilder<TestModel> collectionBuilder;
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