using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HAF {

  /// <summary>
  /// abstact base class for creating object oriented enumerations
  /// </summary>
  /// <remarks>
  /// see https://docs.microsoft.com/de-de/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types for more information
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

    public string Name { get; private set; }

    public int Id { get; private set; }

    protected Enumeration(int id, string name) {
      this.Id = id;
      this.Name = name;
    }

    public override string ToString() {
      return this.Name;
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration {
      return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Select(f => f.GetValue(null))
        .Cast<T>();
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
  }
}
