using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  public class ObjectValidationBatch: PropertyValidationBatch {
    internal Dictionary<string, PropertyValidationBatch> Properties = new Dictionary<string, PropertyValidationBatch>();

    private string targetPropertyName;

    public ObjectValidationBatch(string targetPropertyName = "") {
      this.targetPropertyName = targetPropertyName;
    }

    /// <summary>
    /// Validate the provided property of the object. The thrown errors will be associated with that property.
    /// </summary>
    /// <param name="propertyName">Name of the property that is validated.</param>
    /// <param name="propertyValidation">Action that performs the validation and throws errors using the <see cref="PropertyValidationBatch"/>.</param>
    public void ValidateProperty(string propertyName, Action<PropertyValidationBatch> propertyValidation) {
      if(this.targetPropertyName != "" && this.targetPropertyName != propertyName) {
        // object validation is targeting a specific property
        return;
      }
      if(!this.Properties.TryGetValue(propertyName, out var validationBatch)) {
        validationBatch = new PropertyValidationBatch();
        this.Properties.Add(propertyName, validationBatch);
      }
      propertyValidation.Invoke(validationBatch);
    }
  }
}
