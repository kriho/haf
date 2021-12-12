using HAF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace HAF {
  public interface ISettingsOwner {
    string Scope { get; }
  }
}