using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.EntityFrameworkCore.Infrastructure
{
    public interface INameTransform
    {
        string Name { get; }

        CultureInfo Culture { get; set; }

        string Transform(string input);

        string PluralTransform(string input);

        string SingleTransform(string input);

        bool Overwrite { get; set; }
    }
}
