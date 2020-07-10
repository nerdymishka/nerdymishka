using System;
using NerdyMishka.Models;
using Xunit;

namespace Tests
{
    public class SimpleCollectionTests
    {
        [Fact]
        public void Constructor()
        {
            var set = new ModelCollection<Author>();
            var model = set.AsModel();

            Assert.True(model.Count == 0);
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 0);
        }

        [Fact]
        public void SetInitialValues()
        {
            var set = new ModelCollection<Author>();
            var model = set.AsModel();
            model.SetInitialValues(new[]
            {
                new Author()
                {
                    Name = "Test",
                },
                new Author()
                {
                    Id = 0,
                    Name = "13",
                },
            });
            Assert.True(model.Count == 2);

            // true because the models were changed
            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);
        }

        [Fact]
        public void RemoveValues()
        {
            var set = new ModelCollection<Author>();
            var model = set.AsModel();

            model.SetInitialValues(new[]
            {
                Author.Create(m =>
                {
                    m.Id = 12;
                }),
                Author.Create(m =>
                {
                    m.Name = "ShakesSpear";
                }),
            });
            Assert.True(model.Count == 2);

            // true because the models were changed
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            set.RemoveAt(0);
            Assert.True(model.IsChanged);
            Assert.True(model.Removals.Count == 1);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Descendants.Count == 1);
        }

        [Fact]
        public void AddValues()
        {
            var set = new ModelCollection<Author>();
            var model = set.AsModel();

            model.SetInitialValues(new[]
            {
                Author.Create(m =>
                {
                    m.Id = 12;
                }),
                Author.Create(m =>
                {
                    m.Name = "ShakesSpear";
                }),
            });
            Assert.True(model.Count == 2);

            // true because the models were changed
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            set.Add(new Author());
            Assert.True(model.IsChanged);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Additions.Count == 1);
            Assert.True(model.Descendants.Count == 3);
        }

        internal class Author : Model<Author>
        {
            private int? id;

            private string name;

            public Author()
            {
                this.BeginInit();
                this.Id = null;
                this.Name = null;
                this.EndInit();
            }

            public int? Id
            {
                get => this.id;
                set => this.Set("Id", ref this.id, value);
            }

            public string Name
            {
                get => this.name;
                set => this.Set("Name", ref this.name, value);
            }

            public static Author Create(Action<Author> action)
            {
                var author = new Author();
                author.AsModel().SetInitialValues(action);
                return author;
            }
        }
    }
}