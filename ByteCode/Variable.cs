namespace Blover.BC
{
    internal class Variable(string name)
    {
        static int numberCounter = 0;
        public string Name = name;
        public int Id = numberCounter++;

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
            return $"{Name}%{Id}";
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