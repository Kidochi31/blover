

namespace Blover.Zlover.Resolving
{
    internal class VerifyFunEnvironment
    {
        public Dictionary<string, VerifyFunction> VerifyFunctions;


        public VerifyFunEnvironment()
        {
            VerifyFunctions = new();
            VerifyFunctions["gte"] = new VerifyFunction("gte");
            VerifyFunctions["gt"] = new VerifyFunction("gt");
            VerifyFunctions["lte"] = new VerifyFunction("lte");
            VerifyFunctions["lt"] = new VerifyFunction("lt");
            VerifyFunctions["and"] = new VerifyFunction("and");
            VerifyFunctions["minus"] = new VerifyFunction("minus");
        }

        public VerifyFunction? GetVerifyFunction(string name)
        {
            if (VerifyFunctions.ContainsKey(name))
            {
                return VerifyFunctions[name];
            }
            return null;
        }
    }
}