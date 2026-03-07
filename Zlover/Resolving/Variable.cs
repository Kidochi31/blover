
namespace Blover.Zlover.Resolving
{
    internal class Variable(string name)
    {
        public string Identifier => $"{Name}%{IdNumber}";
        static int numberCounter = 0;
        public string Name = name;
        public int IdNumber = numberCounter++;

        public static bool operator ==(Variable a, Variable b)
        {
            return ReferenceEquals(a, b);
        }
        public static bool operator !=(Variable a, Variable b)
        {
            return !ReferenceEquals(a, b);
        }

        public override string ToString()
        {
            return Identifier;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}