using System;

namespace LostGen {
    /// <summary>
    /// Throw this from PawnComponent.OnAdd when a required component is missing
    /// </summary>
    [Serializable]
    public class MissingComponentException<T> : Exception where T : PawnComponent {
        public MissingComponentException()
            : base("Pawn is missing a required " + typeof(T).FullName) { }
    }
}