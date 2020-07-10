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

            // true because the child (descendant) models
            // are changed after the constructor was called.
            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);
        }

        [Fact]
        public void AcceptChangesForDescendants()
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

            // true because the child (descendant) models
            // are changed after the constructor was called.
            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            model.AcceptChanges();

            // accept changes should call AcceptChanges for each
            // descendant.
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            var first = set[0].AsModel();
            Assert.False(first.IsChanged);

            var second = set[1].AsModel();
            Assert.False(second.IsChanged);
        }

        [Fact]
        public void AcceptChanges()
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

            set.Add(new Author()
            {
                Name = "Byron",
            });

            set.RemoveAt(0);

            Assert.True(model.Count == 2);

            // true because the child (descendant) models
            // are changed after the constructor was called.
            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 1);
            Assert.True(model.Removals.Count == 1);
            Assert.True(model.Descendants.Count == 2);

            model.AcceptChanges();

            // accept changes should call AcceptChanges for each
            // descendant.
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            var first = set[0].AsModel();
            Assert.False(first.IsChanged);

            var second = set[1].AsModel();
            Assert.False(second.IsChanged);
        }

        [Fact]
        public void RejectChanges()
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

            set.Add(new Author()
            {
                Name = "Byron",
            });

            var byron = set[2];

            set.RemoveAt(0);

            Assert.True(model.Count == 2);

            // true because the child (descendant) models
            // are changed after the constructor was called.
            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 1);
            Assert.True(model.Removals.Count == 1);
            Assert.True(model.Descendants.Count == 2);

            model.RejectChanges();

            // RejectChanges should call RejectChanges for each
            // descendant.
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            var first = set[0].AsModel();
            Assert.False(first.IsChanged);

            var second = set[1].AsModel();
            Assert.False(second.IsChanged);

            Assert.Collection(set,
                (item) => Assert.NotEqual(byron, item),
                (item) => Assert.NotEqual(byron, item));

            // removal should happen be reject changes is
            // called on the descendant model
            Assert.Equal("Byron", byron.Name);
        }

        [Fact]
        public void RejectChangesForDescendants()
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

            // true because the child (descendant) models
            // are changed after the constructor was called.
            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            model.RejectChanges();

            // accept changes should call RejectChanges for each
            // descendant. The collection will now have no changes.
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            var first = set[0];
            Assert.Null(first.Name);

            var second = set[1];
            Assert.Null(second.Name);
            Assert.Null(second.Id);
        }

        [Fact]
        public void RemoveValues()
        {
            var set = new ModelCollection<Author>();
            var model = set.AsModel();

            model.SetInitialValues(new[]
            {
                new Author(m =>
                {
                    m.Id = 12;
                }),
                new Author(m =>
                {
                    m.Name = "ShakesSpear";
                }),
            });
            Assert.True(model.Count == 2);

            // false because the models are created
            // with initial state loaded and the
            // collection is not modified.
            Assert.False(model.IsChanged);
            Assert.True(model.Additions.Count == 0);
            Assert.True(model.Removals.Count == 0);
            Assert.True(model.Descendants.Count == 2);

            set.RemoveAt(0);

            // true because the collection is modified.
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
                new Author(m =>
                {
                    m.Id = 12;
                }),
                new Author(m =>
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

        [Fact]
        public void ReplaceValues()
        {
            var set = new ModelCollection<Author>();
            var model = set.AsModel();

            model.SetInitialValues(new[]
            {
                new Author(m =>
                {
                    m.Id = 12;
                }),
                new Author(m =>
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

            var author3 = new Author((m) =>
            {
                m.Name = "Byron";
            });

            var old = set[1];
            set[1] = author3;

            Assert.True(model.IsChanged);
            Assert.True(model.Additions.Count == 1);
            Assert.True(model.Removals.Count == 1);
            Assert.True(model.Descendants.Count == 2);
            Assert.Collection(model.Additions, (item) => Assert.Equal(author3, item));
            Assert.Collection(model.Removals, (item) => Assert.Equal(old, item));
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

            public Author(Action<Author> setInitialValues)
                : this()
            {
                this.SetInitialValues(setInitialValues);
            }

            public int? Id
            {
                get => this.id;
                set => this.SetValue(nameof(this.Id), ref this.id, value);
            }

            public string Name
            {
                get => this.name;
                set => this.SetValue(nameof(this.Name), ref this.name, value);
            }
        }
    }
}