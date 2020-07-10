using System;
using NerdyMishka.Models;
using Xunit;

namespace Tests
{
    public class ComplexModelTests
    {
        [Fact]
        public void ModelConstructor()
        {
            var author = new Author();
            var model = author.AsModel();

            Assert.False(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);
            Assert.True(model.Descendants.Count == 1);
            Assert.True(model.ChangedProperies.Count == 0);
        }

        [Fact]
        public void AcceptChangesForChildObject()
        {
            var author = new Author();
            var model = author.AsModel();

            Assert.False(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);
            Assert.True(model.Descendants.Count == 1);
            Assert.True(model.ChangedProperies.Count == 0);

            author.ContactInfo = new ContactInfo();

            Assert.True(model.IsChanged);
            Assert.True(model.Descendants.Count == 2);
            Assert.True(model.ChangedProperies.Count == 1);

            model.AcceptChanges();

            Assert.False(model.IsChanged);
            Assert.True(model.Descendants.Count == 2);
            Assert.True(model.ChangedProperies.Count == 0);

            var model2 = author.ContactInfo.AsModel();
            Assert.False(model2.IsChanged, "IsChange should be false");
            Assert.True(model2.ChangedProperies.Count == 0, "ChangeProperties should be 0");
        }

        [Fact]
        public void AcceptChangesForChildCollection()
        {
            var author = new Author();
            var model = author.AsModel();
            var set = author.Books;
            var model2 = set.AsModel();
            Assert.False(model.IsChanged);
            Assert.True(model.IsNew);
            Assert.False(model.IsDeleted);
            Assert.True(model.Descendants.Count == 1);
            Assert.True(model.ChangedProperies.Count == 0);

            author.Books.Add(new Book(m => m.Name = "First"));
            author.Books.Add(new Book(m => m.Name = "Second"));

            Assert.True(model.IsChanged);
            Assert.True(model.Descendants.Count == 1);
            Assert.True(model.ChangedProperies.Count == 0);
            Assert.True(model2.Additions.Count == 2);
            Assert.True(model2.IsChanged);
            Assert.True(set.Count == 2);

            model.AcceptChanges();

            Assert.False(model.IsChanged);
            Assert.True(model.Descendants.Count == 1);
            Assert.False(model2.IsChanged);
            Assert.True(model2.Additions.Count == 0);
            Assert.True(set.Count == 2);
            Assert.True(model2.Descendants.Count == 2);
        }

        internal class Book : Model<Book>
        {
            private string name;

            public Book()
            {
                this.SetInitialValues(m =>
                {
                    m.Name = null;
                });
            }

            public Book(Action<Book> action)
                : this()
            {
                this.SetInitialValues(action);
            }

            public string Name
            {
                get => this.name;
                set => this.SetValue(nameof(this.Name), ref this.name, value);
            }
        }

        internal class ContactInfo : Model<ContactInfo>
        {
            private string phone;

            public ContactInfo()
            {
                this.BeginInit();
                this.Phone = null;
                this.EndInit();
            }

            public ContactInfo(Action<ContactInfo> action)
                : this()
            {
                this.SetInitialValues(action);
            }

            public string Phone
            {
                get => this.phone;
                set => this.SetValue(nameof(this.Phone), ref this.phone, value);
            }
        }

        // nameof is used because it creates an interned
        // string that is a constant value. which means
        // there is only one reference of the string
        // in the program.
        internal class Author : Model<Author>
        {
            private int? id;

            private string name;

            private ModelCollection<Book> books;

            private ContactInfo contactInfo;

            public Author()
            {
                this.BeginInit();
                this.Id = null;
                this.Name = null;
                this.Books = new ModelCollection<Book>();
                this.ContactInfo = null;
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

            public ContactInfo ContactInfo
            {
                get => this.contactInfo;
                set => this.SetValue(
                    nameof(this.ContactInfo),
                    ref this.contactInfo,
                    value);
            }

            public ModelCollection<Book> Books
            {
                get => this.books;
                private set =>
                    this.SetValue(nameof(this.Books), ref this.books, value);
            }
        }
    }
}