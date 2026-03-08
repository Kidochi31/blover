

using Blover.Zlover.Scanning;

namespace Blover.Zlover.Resolving
{
    internal class Environment
    {
        public Dictionary<string, Variable> Variables = new();
        public Environment? Parent;

        public Environment(Environment? parent = null)
        {
            Parent = parent;
        }

        public Variable? DeclareVariable(string name, Token? decToken)
        {
            if (Variables.ContainsKey(name))
            {
                return null;
            }
            Variable newVar = new(name, decToken);
            Variables[name] = newVar;
            return newVar;
        }

        public Variable? GetVariable(string name)
        {
            if (Variables.ContainsKey(name))
            {
                return Variables[name];
            }
            if (Parent is not null)
            {
                return Parent.GetVariable(name);
            }
            return null;
        }
    }
}