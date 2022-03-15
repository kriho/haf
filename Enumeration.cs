using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAF {
  /// <summary>
  /// Abstact base class for object oriented enumerations.
  /// </summary>
  /// <remarks>
  /// See https://docs.microsoft.com/de-de/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types for more information.
  /// </remarks>
  /// <example>
  /// public class CardType: Enumeration {
  ///   public static CardType Amex = new CardType(1, nameof(Amex));
  ///   public static CardType Visa = new CardType(2, nameof(Visa));
  ///   public static CardType MasterCard = new CardType(3, nameof(MasterCard));
  /// 
  ///   public CardType(int id, string name): base(id, name) {
  ///   }
  /// }
  /// </example>
  public abstract class Enumeration: IComparable {
    /// <summary>
    /// Name of the enumeration.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Id of the enumeration.
    /// </summary>
    public int Id { get; private set; }

    protected Enumeration(int id, string name) {
      this.Id = id;
      this.Name = name;
    }

    public override string ToString() {
      return this.Name;
    }

    /// <summary>
    /// Get all enumerations from a type.
    /// </summary>
    /// <typeparam name="T">Enumeration type, must be derived from <see cref="Enumeration"/>.</typeparam>
    /// <returns>List of enumerations.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration {
      return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Select(f => f.GetValue(null))
        .OfType<T>();
    }

    /// <summary>
    /// Get enumeration from type with provided id.
    /// </summary>
    /// <typeparam name="T">Enumeration type, must be derived from <see cref="Enumeration"/>.</typeparam>
    /// <param name="id">Enumeration id that is used to find enumeration.</param>
    /// <returns>Found enumeration or null when no enumeration with the provided id exists.</returns>
    public static T Get<T>(int id) where T : Enumeration {
      return Enumeration.GetAll<T>().FirstOrDefault(e => e.Id == id);
    }

    public override bool Equals(object obj) {
      if (obj is Enumeration enumeration) {
        return Id.Equals(enumeration.Id) && GetType().Equals(obj.GetType());
      }
      return false;
    }

    public override int GetHashCode() {
      return this.Id;
    }

    public int CompareTo(object other) {
      return Id.CompareTo(((Enumeration)other).Id);
    }
    
    public static explicit operator int(Enumeration e) => e.Id;

    public static explicit operator string(Enumeration e) => e.Name;
  }
}
