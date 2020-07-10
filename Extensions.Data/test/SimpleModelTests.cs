using System;
using NerdyMishka.Models;
using Xunit;

namespace Tests
{
    public class SimpleModelTests
    {
        [Fact]
        public void ModelConstructor()
        {
            var author = new Author();
            var model = author.AsModel();
            Assert.True(model.ChangedProperies.Count == 0);
            Assert.False(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);
        }

        [Fact]
        public void SetInitialValues()
        {
            var author = new Author(m =>
            {
                m.Id = 12;
                m.Name = "Shakespear";
            });
            var model = author.AsModel();
            Assert.Equal("Shakespear", author.Name);
            Assert.Equal(12, author.Id.Value);
            Assert.True(model.ChangedProperies.Count == 0);
            Assert.False(model.IsChanged);
            Assert.False(model.IsNew);
            Assert.False(model.IsDeleted);
        }

        [Fact]
        public void TrackPropertyChanges()
        {
            var author = new Author
            {
                Id = 0,
                Name = "Alexander",
            };
            var model = author.AsModel();
            Assert.True(model.ChangedProperies.Count == 2);
            Assert.True(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);
        }

        [Fact]
        public void AcceptPropertyChanges()
        {
            var author = new Author
            {
                Id = 0,
                Name = "Alexander",
            };
            var model = author.AsModel();
            Assert.True(model.ChangedProperies.Count == 2);
            Assert.True(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);

            model.AcceptChanges();
            Assert.Equal("Alexander", author.Name);
            Assert.True(model.ChangedProperies.Count == 0);
            Assert.False(model.IsNew);
        }

        [Fact]
        public void RejectPropertyChanges()
        {
            var author = new Author
            {
                Id = 0,
                Name = "Alexander",
            };
            var model = author.AsModel();
            Assert.True(model.ChangedProperies.Count == 2);
            Assert.True(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);

            model.RejectChanges();
            Assert.Null(author.Name);
            Assert.True(model.ChangedProperies.Count == 0);
            Assert.True(model.IsNew);
        }

        // nameof is used because it creates an interned
        // string that is a constant value. which means
        // there is only one reference of the string
        // in the program.
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
