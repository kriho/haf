using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace HAF.Converters {
    public class BooleanToGridLength : ValueConverter<bool, GridLength> {
        public GridLength TrueValue { get; set; }
        public GridLength FalseValue { get; set; }

        protected override GridLength convert(bool value) {
            return value ? this.TrueValue : this.FalseValue;
        }

        protected override bool convertBack(GridLength value) {
            return value == this.TrueValue;
        }
    }
}
