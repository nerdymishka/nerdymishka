using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

namespace NerdyMishka.EntityFrameworkCore.Infrastructure
{
    public class PluralSnakeCaseNameTransform : INameTransform
    {
        public string Name => "Plural Snake Case Name Transform";
        public CultureInfo Culture { get; set; }

        public string SingleTransform(string input)
        {
            if (input is null)
                return input;

            return input.Singularize().Underscore();
        }

        public bool Overwrite { get; set; } = true;
        public string Transform(string input)
        {
            if (input is null)
                return input;

            return input.Underscore();
        }

        public string PluralTransform(string input)
        {
            if (input is null)
                return input;

            return input.Pluralize().Underscore();
        }
    }
}
